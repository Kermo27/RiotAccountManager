using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using RiotAccountManager.MAUI.Data.Models;
using RiotAccountManager.MAUI.Data.Repositories;
using RiotAccountManager.MAUI.Services.EncryptionService;
using RiotAccountManager.MAUI.Services.RiotClientService;

namespace RiotAccountManager.MAUI.Components.Pages;

public partial class DashboardBase : ComponentBase
{
    [Inject]
    protected AccountRepository Repository { get; set; } = null!;

    [Inject]
    protected IRiotClientService RiotClient { get; set; } = null!;

    [Inject]
    protected IEncryptionService Encryptor { get; set; } = null!;
    
    [Inject]
    private ILogger<DashboardBase> Logger { get; set; } = null!;


    protected List<Account> Accounts = new();
    protected Account NewAccount = new();
    protected Account EditAccountModel = new();
    protected bool ShowAddDialog;
    protected bool ShowEditDialog;
    protected bool ShowDeleteConfirmation;

    private string _accountToDeleteId = string.Empty;

    protected string ErrorMessage { get; set; } = string.Empty;

    protected readonly List<string> Regions = new()
    {
        "EUW",
        "NA",
        "EUNE",
        "BR",
        "TR",
        "RU",
        "OCE",
        "LAN",
        "LAS",
        "JP",
        "KR",
    };

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await LoadAccounts();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while initializing the dashboard.");
            ErrorMessage = "An unexpected error occurred. Please try again later.";
        }
    }


    private async Task LoadAccounts() => Accounts = await Repository.GetAllAsync();

    protected void OpenAddDialog()
    {
        NewAccount = new Account
        {
            Id = string.Empty,
            Nickname = string.Empty,
            Username = string.Empty,
            Password = string.Empty,
        };
        ErrorMessage = string.Empty;
        ShowAddDialog = true;
    }

    protected void CloseAddDialog()
    {
        ShowAddDialog = false;
        ErrorMessage = string.Empty;
        StateHasChanged();
    }

    protected void OpenEditDialog(string accountId)
    {
        var account = Accounts.FirstOrDefault(a => a.Id == accountId);
        if (account != null)
        {
            EditAccountModel = new Account
            {
                Id = account.Id,
                Nickname = account.Nickname,
                Username = account.Username,
                Region = account.Region,
                EncryptedPassword = account.EncryptedPassword,
            };
            ErrorMessage = string.Empty;
            ShowEditDialog = true;
        }
    }

    protected void CloseEditDialog()
    {
        ShowEditDialog = false;
        ErrorMessage = string.Empty;
        StateHasChanged();
    }

    protected async Task SaveNewAccount()
    {
        try
        {
            ErrorMessage = string.Empty;

            if (!ValidateNickname(NewAccount.Nickname))
            {
                ErrorMessage = "Nickname must be in the format 'name#tag', with up to 5 characters after '#'.";
                return;
            }
            if (string.IsNullOrWhiteSpace(NewAccount.Username))
            {
                ErrorMessage = "Username is required!";
                return;
            }

            Console.WriteLine($"Saved password: {NewAccount.Password}");
            if (string.IsNullOrWhiteSpace(NewAccount.Password))
            {
                ErrorMessage = "Password is required for new accounts!";
                return;
            }

            var account = new Account
            {
                Id = Guid.NewGuid().ToString(),
                Nickname = NewAccount.Nickname,
                Username = NewAccount.Username,
                Region = NewAccount.Region,
                EncryptedPassword = Encryptor.Encrypt(NewAccount.Password)
            };

            Accounts.Add(account);
            await Repository.SaveAllAsync(Accounts);
            CloseAddDialog();
            await LoadAccounts();
            Logger.LogInformation("New account '{Nickname}' created successfully.", NewAccount.Nickname);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving new account '{Nickname}'.", NewAccount.Nickname);
            ErrorMessage = $"Error saving account: {ex.Message}";
        }
    }

    protected async Task SaveEditedAccount()
    {
        try
        {
            ErrorMessage = string.Empty;

            if (!ValidateNickname(EditAccountModel.Nickname))
            {
                ErrorMessage = "Nickname must be in the format 'name#tag', with up to 5 characters after '#'.";
                return;
            }
            if (string.IsNullOrWhiteSpace(EditAccountModel.Username))
            {
                ErrorMessage = "Username is required!";
                return;
            }

            var account = Accounts.FirstOrDefault(a => a.Id == EditAccountModel.Id);
            if (account != null)
            {
                account.Nickname = EditAccountModel.Nickname;
                account.Username = EditAccountModel.Username;
                account.Region = EditAccountModel.Region;

                if (!string.IsNullOrWhiteSpace(EditAccountModel.Password))
                {
                    account.EncryptedPassword = Encryptor.Encrypt(EditAccountModel.Password);
                }

                await Repository.SaveAllAsync(Accounts);
                CloseEditDialog();
                await LoadAccounts();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error updating account: {ex.Message}";
        }
    }

    protected void DeleteAccount(string accountId)
    {
        _accountToDeleteId = accountId;
        ShowDeleteConfirmation = true;
    }

    protected async Task ConfirmDelete()
    {
        Accounts.RemoveAll(a => a.Id == _accountToDeleteId);
        await Repository.SaveAllAsync(Accounts);
        ShowDeleteConfirmation = false;
        await LoadAccounts();
    }

    protected void CancelDelete() => ShowDeleteConfirmation = false;

    protected async Task Login(Account account)
    {
        await RiotClient.AutoLogin(account);
    }

    protected static async Task OpenBuyMeACoffee()
    {
        const string url = "https://buymeacoffee.com/kermo";
        
        await Launcher.Default.OpenAsync(url);
    }
    
    private bool ValidateNickname(string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            return false;

        var regex = new Regex(@"^[^#]+#[^#]{1,5}$");
        return regex.IsMatch(nickname);
    }
}
