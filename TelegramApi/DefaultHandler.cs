using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using TdApi = Telegram.Td.Api;
namespace TelegramApi
{
   

    public class DefaultHandler : Td.ClientResultHandler
    {
        public void OnResult(TdApi.BaseObject @object)
        {
            Console.WriteLine(@object.ToString());
        }
    }

}
