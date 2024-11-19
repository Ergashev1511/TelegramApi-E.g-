using System;
using System.Threading.Tasks;
using TdLib;

public class TelegramClient
{
    private readonly TdClient _client;
    private bool _isAuthenticated = false;

    public TelegramClient()
    {
        _client = new TdClient();

        // Subscribe to updates (including authorization state changes)
        _client.UpdateReceived += async (sender, update) =>
        {
            if (update is TdApi.Update.UpdateAuthorizationState state)
            {
                await HandleAuthorizationState(state.AuthorizationState);
            }
        };

    }

    public TdClient Client => _client;
    public bool IsAuthenticated => _isAuthenticated;


    public async Task InitializeAsync(int apiId, string apiHash, string phoneNumber)
    {
        if (apiId <= 0 || string.IsNullOrEmpty(apiHash) || string.IsNullOrEmpty(phoneNumber))
        {
            Console.WriteLine("Xatolik: API ID, API Hash va telefon raqamini tekshiring.");
            return;
        }

        try
        {
            Console.WriteLine("Telegram mijoziga ulanish boshlandi...");

            // Set Telegram client parameters (API credentials)
            await _client.ExecuteAsync(new TdApi.SetTdlibParameters
            {
                DatabaseDirectory = "tdlib",
                UseMessageDatabase = true,
                UseSecretChats = true,
                ApiId = apiId,
                ApiHash = apiHash,
                SystemLanguageCode = "en",
                DeviceModel = "Desktop",
                SystemVersion = "Windows 11",
                ApplicationVersion = "1.0"
            });

            Console.WriteLine("Telegram sozlamalari muvaffaqiyatli o'rnatildi.");

            // Start phone number authentication
            await _client.ExecuteAsync(new TdApi.SetAuthenticationPhoneNumber
            {
                PhoneNumber = phoneNumber
            });

            Console.WriteLine("Telefon raqam kiritildi. Tasdiqlash kodi kutilmoqda...");

            // Wait for the authentication process to complete
            while (!_isAuthenticated)
            {
                await Task.Delay(1000);  // Wait for a second before checking again
            }

            Console.WriteLine("Autentifikatsiya muvaffaqiyatli amalga oshirildi!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik yuz berdi: {ex.Message}");
        }
    }

    public async Task AuthenticateAsync(string phoneCode)
    {
        if (string.IsNullOrEmpty(phoneCode))
        {
            Console.WriteLine("Tasdiqlash kodi kiritilmagan.");
            return;
        }

        try
        {
            // Execute the authentication check with the provided code
            var result = await _client.ExecuteAsync(new TdApi.CheckAuthenticationCode
            {
                Code = phoneCode
            });

            // Success message
            Console.WriteLine($"Autentifikatsiya muvaffaqiyatli: {result}");
            _isAuthenticated = true;
        }
        catch (Exception ex)
        {
            // Handle failed authentication
            Console.WriteLine($"Autentifikatsiya xatoligi: {ex.Message}");
        }
    }

    private async Task HandleAuthorizationState(TdApi.AuthorizationState authorizationState)
    {
        Console.WriteLine($"Hozirgi holat: {authorizationState.GetType().Name}");

        switch (authorizationState)
        {
            case TdApi.AuthorizationState.AuthorizationStateWaitTdlibParameters:
                Console.WriteLine("Telegram kutubxona parametrlari kutilmoqda...");
                break;

            case TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber:
                Console.WriteLine("Telefon raqami kiritish kutilmoqda...");
                break;

            case TdApi.AuthorizationState.AuthorizationStateWaitCode:
                Console.WriteLine("Tasdiqlash kodi kutilmoqda...");
                Console.WriteLine("Code ni kiriting: ");
                var code = Console.ReadLine();
                await AuthenticateAsync(code);
                break;

            case TdApi.AuthorizationState.AuthorizationStateReady:
                Console.WriteLine("Telegramga muvaffaqiyatli ulanildi!");
                _isAuthenticated = true;
                break;

            case TdApi.AuthorizationState.AuthorizationStateLoggingOut:
                Console.WriteLine("Telegramdan chiqish amalga oshmoqda...");
                break;

            case TdApi.AuthorizationState.AuthorizationStateClosing:
                Console.WriteLine("Telegram mijoz yopilmoqda...");
                break;

            case TdApi.AuthorizationState.AuthorizationStateClosed:
                Console.WriteLine("Telegram mijoz yopildi.");
                break;

            default:
                Console.WriteLine("Noma'lum autentifikatsiya holati.");
                break;
        }
    }

    // Method to retrieve the phone code if required
    public async Task RetrieveCodeAsync()
    {
        // Handling other potential states like waiting for a code again after it expires, etc.
        // This method may be used to allow the user to request the code again if necessary
    }
}
