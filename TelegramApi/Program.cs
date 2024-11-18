using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TdLib;
using static TdLib.TdApi;

class Program
{
     
    static async Task Main(string[] args)
    { 
        var telegramClient = new TelegramClient();
        await telegramClient.InitializeAsync(apiId: 22754021, apiHash: "b5fc64692abe7ea2f0f2506ea10ac81f", phoneNumber: "+998500035360");

       
        var chatService = new ChatService(telegramClient.Client);
        var messageService = new MessageService(telegramClient.Client);

        
        while (!telegramClient.IsAuthenticated)
        {
            await Task.Delay(10000); 
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
        var groupId = Convert.ToInt64(Console.ReadLine());

        var messages = await messageService.GetMessagesFromGroupAsync(groupId);

      

        foreach (var message in messages)
        {
            if (message.Content is TdApi.MessageContent.MessageText messageText)
            {
                Console.WriteLine($"Matn: {messageText.Text.Text}");

                long chatId = 2078159566;

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
