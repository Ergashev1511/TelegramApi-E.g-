using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TdLib;
using Telegram.Td.Api;

public class ChatService
{
    private readonly TdClient _client;

    public ChatService(TdClient client)
    {
        _client = client;
    }

    public async Task<List<TdApi.Chat>> GetAllChatsAsync()
    {
        var chatList = new List<TdApi.Chat>();

        var chatIdsResult = await _client.ExecuteAsync(new TdApi.GetChats { Limit = 10 });

        if (chatIdsResult is TdApi.Chats chatIds)
        {
            foreach (var chatId in chatIds.ChatIds)
            {
                var chat = await _client.ExecuteAsync(new TdApi.GetChat { ChatId = chatId });
                chatList.Add(chat);
            }
        }

        return chatList;
    }

    public List<TdApi.Chat> FilterGroups(List<TdApi.Chat> chats)
    {
        return chats.Where(chat =>
            chat.Type is TdApi.ChatType.ChatTypeBasicGroup ||
            chat.Type is TdApi.ChatType.ChatTypeSupergroup).ToList();
    }
}


