using System.Text.Json;
using UniversalYogaCustomerApp.Models;

namespace UniversalYogaCustomerApp.Services;

public class FirestoreCourseService
{
    private const string BaseUrl = "https://firestore.googleapis.com/v1/projects/universalyogaadmin--pltm/databases/(default)/documents";

    private readonly HttpClient _httpClient;

    public FirestoreCourseService(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<List<AvailableClassItem>> GetAvailableClassesAsync(CancellationToken cancellationToken = default)
    {
        var coursesTask = GetCollectionDocumentsAsync("courses", cancellationToken);
        var instancesTask = GetCollectionDocumentsAsync("instances", cancellationToken);

        await Task.WhenAll(coursesTask, instancesTask);

        var courseDocs = coursesTask.Result;
        var instanceDocs = instancesTask.Result;

        var courses = courseDocs
            .Select(ToCourseDto)
            .Where(x => x is not null)
            .Cast<CourseDto>()
            .ToDictionary(x => x.Id, x => x);

        var availableClasses = new List<AvailableClassItem>();

        foreach (var instance in instanceDocs.Select(ToInstanceDto).Where(x => x is not null).Cast<InstanceDto>())
        {
            if (!courses.TryGetValue(instance.CourseLocalId, out var course))
            {
                continue;
            }

            availableClasses.Add(new AvailableClassItem
            {
                CourseId = course.Id,
                InstanceId = instance.Id,
                ClassType = course.Type,
                Day = course.DayOfWeek,
                Date = instance.Date,
                Time = course.Time,
                Teacher = instance.Teacher,
                Price = course.Price
            });
        }

        return availableClasses
            .OrderBy(x => x.Day)
            .ThenBy(x => x.Time)
            .ThenBy(x => x.Date)
            .ToList();
    }

    private async Task<List<JsonElement>> GetCollectionDocumentsAsync(string collectionName, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/{collectionName}", cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Firestore request failed for '{collectionName}' ({(int)response.StatusCode}): {responseBody}");
        }

        using var jsonDocument = JsonDocument.Parse(responseBody);
        if (!jsonDocument.RootElement.TryGetProperty("documents", out var documentsElement) || documentsElement.ValueKind != JsonValueKind.Array)
        {
            return new List<JsonElement>();
        }

        var items = new List<JsonElement>();
        foreach (var doc in documentsElement.EnumerateArray())
        {
            items.Add(doc.Clone());
        }

        return items;
    }

    private static CourseDto? ToCourseDto(JsonElement document)
    {
        if (!TryGetFields(document, out var fields))
        {
            return null;
        }

        var id = ReadInt(fields, "id");
        if (id is null)
        {
            return null;
        }

        return new CourseDto
        {
            Id = id.Value,
            DayOfWeek = ReadString(fields, "dayOfWeek") ?? string.Empty,
            Time = ReadString(fields, "time") ?? string.Empty,
            Price = ReadDouble(fields, "price") ?? 0,
            Type = ReadString(fields, "type") ?? string.Empty
        };
    }

    private static InstanceDto? ToInstanceDto(JsonElement document)
    {
        if (!TryGetFields(document, out var fields))
        {
            return null;
        }

        var id = ReadInt(fields, "id");
        var courseLocalId = ReadInt(fields, "courseLocalId");
        if (id is null || courseLocalId is null)
        {
            return null;
        }

        return new InstanceDto
        {
            Id = id.Value,
            CourseLocalId = courseLocalId.Value,
            Date = ReadString(fields, "date") ?? string.Empty,
            Teacher = ReadString(fields, "teacher") ?? string.Empty
        };
    }

    private static bool TryGetFields(JsonElement document, out JsonElement fields)
    {
        fields = default;
        return document.TryGetProperty("fields", out fields) && fields.ValueKind == JsonValueKind.Object;
    }

    private static string? ReadString(JsonElement fields, string fieldName)
    {
        return TryGetFieldContainer(fields, fieldName, out var container)
            && container.TryGetProperty("stringValue", out var stringValue)
            ? stringValue.GetString()
            : null;
    }

    private static int? ReadInt(JsonElement fields, string fieldName)
    {
        if (!TryGetFieldContainer(fields, fieldName, out var container))
        {
            return null;
        }

        if (container.TryGetProperty("integerValue", out var integerValueElement))
        {
            var raw = integerValueElement.GetString();
            if (int.TryParse(raw, out var parsedInt))
            {
                return parsedInt;
            }
        }

        if (container.TryGetProperty("doubleValue", out var doubleValueElement))
        {
            if (doubleValueElement.TryGetDouble(out var doubleValue))
            {
                return Convert.ToInt32(doubleValue);
            }
        }

        return null;
    }

    private static double? ReadDouble(JsonElement fields, string fieldName)
    {
        if (!TryGetFieldContainer(fields, fieldName, out var container))
        {
            return null;
        }

        if (container.TryGetProperty("doubleValue", out var doubleValueElement))
        {
            if (doubleValueElement.TryGetDouble(out var parsedDouble))
            {
                return parsedDouble;
            }
        }

        if (container.TryGetProperty("integerValue", out var integerValueElement))
        {
            var raw = integerValueElement.GetString();
            if (double.TryParse(raw, out var parsedFromInteger))
            {
                return parsedFromInteger;
            }
        }

        return null;
    }

    private static bool TryGetFieldContainer(JsonElement fields, string fieldName, out JsonElement container)
    {
        container = default;
        return fields.TryGetProperty(fieldName, out container) && container.ValueKind == JsonValueKind.Object;
    }

    private sealed class CourseDto
    {
        public int Id { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    private sealed class InstanceDto
    {
        public int Id { get; set; }
        public int CourseLocalId { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Teacher { get; set; } = string.Empty;
    }
}