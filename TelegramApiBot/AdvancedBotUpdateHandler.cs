using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramApiBot
{
    public class AdvancedBotUpdateHandler : IUpdateHandler
    {
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {

            Console.WriteLine($"Xatolik yuz berdi: {exception.Message}");
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Yangilanish turi: {update.Type}");

            if (update.ChatJoinRequest != null)
            {
                var joinRequest = update.ChatJoinRequest;

                Console.WriteLine($"Chat ID: {joinRequest.Chat.Id}, Chat Title: {joinRequest.Chat.Title}");

                // Ma'lum guruhni tekshirish
                if (joinRequest.Chat.Id == -1002413825295) // Guruhning ID'sini yozing
                {
                    Console.WriteLine($"So'rov {joinRequest.Chat.Title} guruhidan kelgan.");

                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[] { InlineKeyboardButton.WithCallbackData("To'lov qilish", "payment") }
                    });

                    await botClient.SendTextMessageAsync(
                        chatId: joinRequest.From.Id,
                        text: "Siz ushbu guruhga qo'shilish uchun to'lov qilishingiz kerak.",
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken
                    );
                }
            }
            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
            {
                var callbackQuery = update.CallbackQuery;

                if (callbackQuery.Data == "payment")
                {
                    await StartPaymentProcess(botClient, callbackQuery, cancellationToken);
                }
            }
        }


        private static async Task StartPaymentProcess(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var chatId = callbackQuery.Message.Chat.Id;

            var providerToken = "your_payment_provider_token";  // Paycom yoki boshqa to'lov provayderining tokeni
            var amount = 1000; 

            try
            {
                // Paycom yoki boshqa provayder orqali to'lovni amalga oshirish
                var paymentUrl = $"https://paycom.uz/payment/{providerToken}?amount={amount}&chat_id={chatId}"; // Misol

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"To'lovni amalga oshirish uchun quyidagi havolaga bosing:\n{paymentUrl}",
                    cancellationToken: cancellationToken
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"To'lovni amalga oshirishda xatolik: {ex.Message}");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "To'lovni amalga oshirishda xatolik yuz berdi. Iltimos, keyinroq urinib ko'ring.",
                    cancellationToken: cancellationToken
                );
            }
        }
    
   }
}
