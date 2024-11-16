using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TdApi = Telegram.Td.Api;

namespace TelegramApi
{
    public static class ChatManager
    {
        public static void LoadChats()
        {
            var client = Program.TelegramClient.GetClient();
            client.Send(new TdApi.LoadChats(null, 100), new DefaultHandler());
        }

        public static void GetChatMessages(long chatId)
        {
            var client = Program.TelegramClient.GetClient();
            client.Send(
                new TdApi.GetChatHistory(chatId, 0, 0, 50, false),
                new MessagesHandler()
            );
        }

        private class MessagesHandler : Td.ClientResultHandler
        {
            public void OnResult(TdApi.BaseObject @object)
            {
                if (@object is TdApi.Messages messages)
                {
                    foreach (var message in messages.Messages_)
                    {
                        if (message.Content is TdApi.MessageText textContent)
                        {
                            Console.WriteLine($"[{message.Id}] {textContent.Text.Text}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Xabarlar yuklanmadi.");
                }
            }
        }
    }

}
