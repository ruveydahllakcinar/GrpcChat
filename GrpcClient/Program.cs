using Grpc.Core;
using Grpc.Net.Client;
using GrpcChat.Protos;
using System;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Press enter to cont....");
            Console.ReadLine();
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new StreamService.StreamServiceClient(channel);
            string username = "Rüv";
            using var stream = client.StartStreaming();
            var response = Task.Run(async () =>
            {
                await foreach (var rm in stream.ResponseStream.ReadAllAsync())
                    Console.WriteLine(rm.Message);
            });

            Console.WriteLine("Enter message to stream to server");
            while (true)
            {
                var text = Console.ReadLine();
                if (string.IsNullOrEmpty(text))
                    break;
                await stream.RequestStream.WriteAsync(new StreamMessage
                {
                    Username = username,
                    Message = text
                });

            }
            Console.WriteLine("Disconnectioning........");
            await stream.RequestStream.CompleteAsync();
            await response;
        }
    }
}
