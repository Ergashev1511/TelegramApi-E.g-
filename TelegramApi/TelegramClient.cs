using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TelegramApi
{
    public class TelegramClient
    {
        private Td.Client _client;

        public TelegramClient()
        {
            // TDLib logs sozlanishi
            Td.Client.Execute(new TdApi.SetLogVerbosityLevel(0));
            Td.Client.Execute(new TdApi.SetLogStream(new TdApi.LogStreamFile("tdlib.log", 1 << 27, false)));

            // Mijozni ishga tushirish
            _client = Td.Client.Create(new UpdateHandler());
            new Thread(() => Td.Client.Run()).Start();
        }

        public Td.Client GetClient() => _client;

        private class UpdateHandler : Td.ClientResultHandler
        {
            public void OnResult(TdApi.BaseObject @object)
            {
                // Avtorizatsiya yangilanishlari uchun xabarlarni boshqarish
                if (@object is TdApi.UpdateAuthorizationState update)
                {
                    AuthorizationManager.OnAuthorizationStateUpdated(update.AuthorizationState);
                }
            }
        }
    }
}
