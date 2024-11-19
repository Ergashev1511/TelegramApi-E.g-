using System;
using System.Threading.Tasks;
using TdLib;

public class TelegramClient
{
    private readonly TdClient _client;
    private string _phoneNumber;
    private bool _isAuthenticated;

    public TelegramClient()
    {
        _client = new TdClient();
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
            Console.WriteLine("Xatolik: API ID, API Hash yoki telefon raqam noto'g'ri.");
            return;
        }

        _phoneNumber = phoneNumber;

        try
        {
            Console.WriteLine("Telegram mijozini sozlash boshlandi...");

            await _client.ExecuteAsync(new TdApi.SetTdlibParameters
            {
                DatabaseDirectory = "tdlib_new",
                UseMessageDatabase = true,
                UseSecretChats = true,
                ApiId = apiId,
                ApiHash = apiHash,
                SystemLanguageCode = "en",
                DeviceModel = "Desktop",
                SystemVersion = "Windows 11",
                ApplicationVersion = "1.0"
            });

            Console.WriteLine("Sozlamalar muvaffaqiyatli o'rnatildi.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik yuz berdi: {ex.Message}");
        }
    }

    private async Task HandleAuthorizationState(TdApi.AuthorizationState authorizationState)
    {
        Console.WriteLine($"Hozirgi holat: {authorizationState.GetType().Name}");

        switch (authorizationState)
        {
            case TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber:
                Console.WriteLine("Telefon raqami kiritish kutilmoqda...");
                await SetPhoneNumberAsync();
                break;

            case TdApi.AuthorizationState.AuthorizationStateWaitCode:
                Console.WriteLine("Tasdiqlash kodi kutilmoqda...");
                Console.WriteLine("Kod kiriting: ");
                var code = await Task.Run(() => Console.ReadLine());
                await CheckAuthenticationCodeAsync(code);
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

    private async Task SetPhoneNumberAsync()
    {
        try
        {
            await _client.ExecuteAsync(new TdApi.SetAuthenticationPhoneNumber
            {
                PhoneNumber = _phoneNumber
            });
            Console.WriteLine("Telefon raqam muvaffaqiyatli yuborildi.");
        }
        catch (TdLib.TdException ex)
        {
            Console.WriteLine($"Telefon raqamni yuborishda xatolik: {ex.Message}");
        }
    }

    private async Task CheckAuthenticationCodeAsync(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            Console.WriteLine("Tasdiqlash kodi kiritilmadi.");
            return;
        }

        try
        {
            await _client.ExecuteAsync(new TdApi.CheckAuthenticationCode
            {
                Code = code
            });
            Console.WriteLine("Tasdiqlash kodi to'g'ri!");
            _isAuthenticated = true;
        }
        catch (TdLib.TdException ex)
        {
            Console.WriteLine($"Tasdiqlash kodi xato: {ex.Message}");
        }
    }

    public async Task ResendCodeAsync()
    {
        try
        {
            await _client.ExecuteAsync(new TdApi.ResendAuthenticationCode());
            Console.WriteLine("Tasdiqlash kodi qayta yuborildi.");
        }
        catch (TdLib.TdException ex)
        {
            Console.WriteLine($"Tasdiqlash kodini qayta yuborishda xatolik: {ex.Message}");
        }
    }
}
