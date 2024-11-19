using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TdLib;
using Telegram.Td;
using static TdLib.TdApi;

class Program
{
    //apiId: 22754021, apiHash: "b5fc64692abe7ea2f0f2506ea10ac81f", phoneNumber: "+998500035360"
    static async Task Main(string[] args)
    { 
        var telegramClient = new TelegramClient();
        await telegramClient.InitializeAsync(apiId: 24802873, apiHash: "de727370af401ab103fb0ebbc2a1ed2f", phoneNumber: "+998908832603");
        if (!telegramClient.IsAuthenticated)
        {
            Console.WriteLine("Tasdiqlash kodi kelganidan keyin kiritishingiz kerak.");
        }

        var chatService = new ChatService(telegramClient.Client);
        var messageService = new MessageService(telegramClient.Client);


        while (!telegramClient.IsAuthenticated)
        {
            Console.WriteLine("Tasdiqlash kodi kiritilmagan. Kod kiritishni istaysizmi? (y/n): ");
            var userInput = Console.ReadLine();

            if (userInput?.ToLower() == "y")
            {
                Console.WriteLine("Tasdiqlash kodini kiriting: ");
                var code = Console.ReadLine();
                if (!string.IsNullOrEmpty(code))
                {
                    await telegramClient.Client.CheckAuthenticationCodeAsync(code);
                }
            }
            else if (userInput?.ToLower() == "n")
            {
                Console.WriteLine("Jarayon to'xtatildi. Telegramga ulanish qaytarildi.");
                break;
            }
            else
            {
                Console.WriteLine("Iltimos, 'y' yoki 'n' ni tanlang.");
            }

            // Tasdiqlash jarayonida biroz kutish
            await Task.Delay(2000);
        }


        var allChats = await chatService.GetAllChatsAsync();

       
        var groups = chatService.FilterGroups(allChats);

        Console.WriteLine("Akkauntingiz ulangan guruhlar:");

        foreach (var group in groups)
        {
            Console.WriteLine($"Guruh nomi: {group.Title}, ID: {group.Id}");
        }

        // Guruh ID tanlash
        Console.Write("Guruh ID kiriting: ");
        var groupId = Convert.ToInt64(await Task.Run(() => Console.ReadLine()));

        var messages = await messageService.GetMessagesFromGroupAsync(groupId);

      

        foreach (var message in messages)
        {
            if (message.Content is TdApi.MessageContent.MessageText messageText)
            {
                Console.WriteLine($"Matn: {messageText.Text.Text}");

                long chatId = 7832251761;

                await telegramClient.Client.ExecuteAsync(new TdApi.SendMessage
                {
                    ChatId = chatId,
                    InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                    {
                        Text = new TdApi.FormattedText
                        {
                            Text = messageText.Text.Text,
                        }
                    }
                });
            }
        }
    }
    
}
