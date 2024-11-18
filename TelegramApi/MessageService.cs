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

    
    public async Task<List<TdApi.Message>> GetMessagesFromGroupAsync(long chatId, int limit = 10)
    {
        var messages = new List<TdApi.Message>();
        long fromMessageId = 0;

        while (messages.Count < limit)
        {
            var history = await _client.ExecuteAsync(new TdApi.GetChatHistory
            {
                ChatId = chatId,
                Limit = limit - messages.Count, 
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

        return messages;
    }
  

    public class MessageContent
    {
        public string Text { get; set; }
    }
}