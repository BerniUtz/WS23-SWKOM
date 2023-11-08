using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Mock_Server.Controllers;

public class WebSocketController : ControllerBase
{
    [Route("/ws/status")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await Echo(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static async Task Echo(WebSocket webSocket)
    {
        var jsonString = JsonSerializer.Serialize(new WebsocketConsumerStatusMessage
        {
            filename = "test.pdf",
            task_id = "123",
            current_progress = 100,
            max_progress = 100,
            status = ConsumerState.SUCCESS,
            message = "test",
            document_id = 1
        });

        await webSocket.SendAsync(Encoding.UTF8.GetBytes(jsonString), WebSocketMessageType.Text, true, CancellationToken.None);

        jsonString = JsonSerializer.Serialize(new WebsocketConsumerStatusMessage
        {
            filename = "test2.pdf",
            task_id = "124",
            current_progress = 3,
            max_progress = 3,
            status = ConsumerState.FAILED,
            message = "test",
            document_id = 2
        });

        await webSocket.SendAsync(Encoding.UTF8.GetBytes(jsonString), WebSocketMessageType.Text, true, CancellationToken.None);

        jsonString = JsonSerializer.Serialize(new WebsocketConsumerStatusMessage
        {
            filename = "test3.pdf",
            task_id = "125",
            current_progress = 70,
            max_progress = 100,
            status = ConsumerState.PROCESSING,
            message = "SWKOM is great!",
            document_id = 2
        });

        await webSocket.SendAsync(Encoding.UTF8.GetBytes(jsonString), WebSocketMessageType.Text, true, CancellationToken.None);

        jsonString = JsonSerializer.Serialize(new WebsocketConsumerStatusMessage
        {
            filename = "test4.pdf",
            task_id = "1",
            current_progress = 0,
            max_progress = 2,
            // STARTING; SUCCESS, FAILED, PROCESSING
            status = ConsumerState.STARTING,
            message = "STARTING NOW",
            document_id = 2
        });

        await webSocket.SendAsync(Encoding.UTF8.GetBytes(jsonString), WebSocketMessageType.Text, true, CancellationToken.None);

        // var buffer = new byte[1024 * 4];
        // var receiveResult = await webSocket.ReceiveAsync(
        //     new ArraySegment<byte>(buffer), CancellationToken.None);

        // while (!receiveResult.CloseStatus.HasValue)
        // {
        //     await webSocket.SendAsync(
        //         new ArraySegment<byte>(buffer, 0, receiveResult.Count),
        //         receiveResult.MessageType,
        //         receiveResult.EndOfMessage,
        //         CancellationToken.None);

        //     receiveResult = await webSocket.ReceiveAsync(
        //         new ArraySegment<byte>(buffer), CancellationToken.None);
        // }

        // await webSocket.CloseAsync(
        //     receiveResult.CloseStatus.Value,
        //     receiveResult.CloseStatusDescription,
        //     CancellationToken.None);
    }
}


public class WebsocketConsumerStatusMessage
{
    public string? filename { get; set; }
    public string? task_id { get; set; }
    public int current_progress { get; set; }
    public int max_progress { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ConsumerState? status { get; set; }
    public string? message { get; set; }
    public int document_id { get; set; }
}

public enum ConsumerState
{
    STARTING,
    SUCCESS,
    FAILED,
    PROCESSING
}