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

            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
            {
                Console.WriteLine($"CallbackQuery keldi: {update.CallbackQuery.Data}");
            }
            else
            {
                Console.WriteLine("CallbackQuery mavjud emas yoki noto'g'ri.");
            }
            
            if (update.Message?.Text == "/start")
            {
                await HandleStartCommand(botClient, update.Message.Chat.Id, cancellationToken);
                return;
            }

            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
            {
                await HandleCallbackQuery(botClient, update.CallbackQuery, cancellationToken);
                return;
            }

            if (update.ChatJoinRequest != null)
            {
                await HandleChatJoinRequest(botClient, update.ChatJoinRequest, cancellationToken);
                return;
            }
        }

        private async Task HandleStartCommand(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Foydalanuvchi ID: {chatId}, `/start` komandasi yuborildi.");

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("To'lov qilish", "payment") }
            });
          

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Siz ushbu botni ishlatish uchun to'lov qilishingiz kerak.",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
            );
        }

        private async Task HandleChatJoinRequest(ITelegramBotClient botClient, ChatJoinRequest joinRequest, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Chat ID: {joinRequest.Chat.Id}, Chat Title: {joinRequest.Chat.Title}");

            if (joinRequest.Chat.Id == -1002413825295)
            {
                await botClient.SendTextMessageAsync(
                    chatId: joinRequest.UserChatId,
                    text: "Salom! Iltimos, ushbu botni ishlatish uchun /start tugmasini bosing.",
                    cancellationToken: cancellationToken
                );
            }
        }

        private async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            if (callbackQuery?.Data == null)
            {
                Console.WriteLine("CallbackQuery data bo'sh yoki null.");
                return;
            }

            Console.WriteLine($"CallbackQuery keldi: {callbackQuery.Data}");

            if (callbackQuery.Data == "payment")
            {
                await StartPaymentProcess(botClient, callbackQuery, cancellationToken);
            }
        }

        private static async Task StartPaymentProcess(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var providerToken = "your_payment_provider_token"; // To'lov tokeni
            var amount = 1000; // To'lov summasi

            try
            {
                // To'lov havolasini yaratish
                var paymentUrl = $"https://paycom.uz/payment/{providerToken}?amount={amount}&chat_id={chatId}";

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"To'lovni amalga oshirish uchun quyidagi havolaga bosing:\n{paymentUrl}",
                    cancellationToken: cancellationToken
                );

                // To'lov muvaffaqiyatli bo'lgan holatda
                bool isPaymentSuccessful = true; // To'lov natijasini tekshiring
                if (isPaymentSuccessful)
                {tr
                    long groupId = -1002413825295; // Guruh ID  Default qilib berildi
                                                   
                    await botClient.ApproveChatJoinRequestAsync(
                        chatId: groupId,
                        userId: callbackQuery.From.Id,
                        cancellationToken: cancellationToken
                    );

                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Sizning to'lovingiz qabul qilindi va siz guruhga qo'shildingiz!",
                        cancellationToken: cancellationToken
                    );
                }
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
