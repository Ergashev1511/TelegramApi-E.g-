using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using TdApi = Telegram.Td.Api;


namespace TelegramApi
{
    public static class AuthorizationManager
    {
        private static volatile bool _haveAuthorization = false;

        public static void OnAuthorizationStateUpdated(TdApi.AuthorizationState authorizationState)
        {
            if (authorizationState is TdApi.AuthorizationStateReady)
            {
                Console.WriteLine("Avtorizatsiya muvaffaqiyatli.");
                _haveAuthorization = true;
            }
            else if (authorizationState is TdApi.AuthorizationStateWaitPhoneNumber)
            {
                Console.WriteLine("Telefon raqamni kiriting:");
                string phoneNumber = Console.ReadLine();
                Program.TelegramClient.GetClient().Send(new TdApi.SetAuthenticationPhoneNumber(phoneNumber, null), new DefaultHandler());
            }
            else if (authorizationState is TdApi.AuthorizationStateWaitCode)
            {
                Console.WriteLine("Tasdiqlash kodini kiriting:");
                string code = Console.ReadLine();
                Program.TelegramClient.GetClient().Send(new TdApi.CheckAuthenticationCode(code), new DefaultHandler());
            }
        }

        public static bool IsAuthorized() => _haveAuthorization;
    }

}
