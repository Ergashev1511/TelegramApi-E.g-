using System;
using TdApi = Telegram.Td.Api;

namespace TelegramApi
{
    public class Program
    {
        public static TelegramClient TelegramClient { get; private set; }

        public static void Main(string[] args)
        {
            TelegramClient = new TelegramClient();

            while (true)
            {
                if (AuthorizationManager.IsAuthorized())
                {
                    Console.WriteLine("Buyruqni kiriting (mc - guruhlar, rm <chatId> - xabarlar, q - chiqish):");
                    string command = Console.ReadLine();
                    string[] commands = command.Split(' ');

                    try
                    {
                        switch (commands[0])
                        {
                            case "mc":
                                ChatManager.LoadChats();
                                break;
                            case "rm":
                                if (commands.Length > 1)
                                {
                                    ChatManager.GetChatMessages(long.Parse(commands[1]));
                                }
                                else
                                {
                                    Console.WriteLine("ChatId kiriting.");
                                }
                                break;
                            case "q":
                                return;
                            default:
                                Console.WriteLine("Noma'lum buyruq.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Xatolik: {ex.Message}");
                    }
                }
            }
        }
    }
}
