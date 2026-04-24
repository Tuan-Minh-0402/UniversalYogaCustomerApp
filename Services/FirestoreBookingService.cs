using System.Text;
using System.Text.Json;
using UniversalYogaCustomerApp.Models;

namespace UniversalYogaCustomerApp.Services;

public class FirestoreBookingService
{
    private const string BookingEndpoint = "https://firestore.googleapis.com/v1/projects/universalyogaadmin--pltm/databases/(default)/documents/bookings";

    private readonly HttpClient _httpClient;

    public FirestoreBookingService(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task SubmitBookingAsync(BookingRecord booking, CancellationToken cancellationToken = default)
    {
        var payload = BuildFirestorePayload(booking);
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(BookingEndpoint, content, cancellationToken);
        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Booking submit failed ({(int)response.StatusCode}): {responseText}");
        }
    }

    private static string BuildFirestorePayload(BookingRecord booking)
    {
        var body = new
        {
            fields = new
            {
                bookingId = new { stringValue = booking.BookingId },
                courseId = new { integerValue = booking.CourseId.ToString() },
                instanceId = new { integerValue = booking.InstanceId.ToString() },
                classType = new { stringValue = booking.ClassType },
                day = new { stringValue = booking.Day },
                date = new { stringValue = booking.Date },
                time = new { stringValue = booking.Time },
                teacher = new { stringValue = booking.Teacher },
                price = new { doubleValue = booking.Price },
                customerName = new { stringValue = booking.CustomerName },
                customerEmail = new { stringValue = booking.CustomerEmail },
                createdAt = new { timestampValue = booking.CreatedAtUtc.ToString("o") }
            }
        };

        return JsonSerializer.Serialize(body);
    }
}