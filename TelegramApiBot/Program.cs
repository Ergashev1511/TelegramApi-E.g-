using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramApiBot;

namespace TelegramBotExample
{
    class Program
    {
        static string botToken = "7741716796:AAF9N4BJPXFb27Ud6YRVjKKZeZIe1Jv6JbY"; // Tokeningizni kiriting

        static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient(botToken);


            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // Receive all update types
            };

            var cts = new CancellationTokenSource();

            Console.WriteLine("Starting bot...");

            var updateHandler = new AdvancedBotUpdateHandler();

            botClient.StartReceiving(
            updateHandler: updateHandler,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

            Console.WriteLine("Bot is running. Press any key to exit.");
            Console.ReadKey();
            cts.Cancel();
        }





    }
}
