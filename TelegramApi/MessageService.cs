using System.Collections.Generic;
using System.Threading.Tasks;
using TdLib;
using Newtonsoft.Json;

public class MessageService
{
    private readonly TdClient _client;
    private Timer _timer;   
    public MessageService(TdClient client)
    {
        _client = client;
    }

    public async Task RetrieveMessagesPeriodically(long chatId)
    {
        var timer = new Timer(async _ => await GetMessagesFromGroupAsync(chatId), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

        Console.ReadLine();
        timer.Dispose();
    }
    public async Task<List<TdApi.Message>> GetMessagesFromGroupAsync(long chatId)
    {
        var messages = new List<TdApi.Message>();
        long fromMessageId = 0;

        while (true)
        {
            var history = await _client.ExecuteAsync(new TdApi.GetChatHistory
            {
                ChatId = chatId,
                Limit = 100,
                FromMessageId = fromMessageId,
                OnlyLocal = false
            });

            if (history is not TdApi.Messages msgs || msgs.Messages_.Length == 0)
            {
                break;
            }

            messages.AddRange(msgs.Messages_);
            fromMessageId = msgs.Messages_[^1].Id;
        }

        foreach (var message in messages)
        {
            if (message.Content is TdApi.MessageContent.MessageText messageText)
            {
                Console.WriteLine($"Matn: {messageText.Text.Text}");

                long targetChatId = 7832251761;  // default qilib yuboriladigan chat id si berilgan

                await _client.ExecuteAsync(new TdApi.SendMessage 
                { 
                    ChatId = targetChatId, InputMessageContent = new TdApi.InputMessageContent.InputMessageText 
                    { 
                        Text = new TdApi.FormattedText { Text = messageText.Text.Text } 
                    }
                });
            }
        }


        return messages;
    }
  

    public class MessageContent
    {
        public string Text { get; set; }
    }
}

//var messages = new List<TdApi.Message>();
//long fromMessageId = 0;

//while (messages.Count < limit)
//{
//    var history = await _client.ExecuteAsync(new TdApi.GetChatHistory
//    {
//        ChatId = chatId,
//        Limit = limit - messages.Count, 
//        FromMessageId = fromMessageId,
//        OnlyLocal = false
//    });

//    if (history is not TdApi.Messages msgs || msgs.Messages_.Length == 0)
//    {
//        break; 
//    }

//    messages.AddRange(msgs.Messages_);
//    fromMessageId = msgs.Messages_[^1].Id; 
//}

//return messages;