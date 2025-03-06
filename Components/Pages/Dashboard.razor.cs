using Microsoft.AspNetCore.Components;
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
    protected NavigationManager NavigationManager { get; set; } = null!;

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

    protected override void OnInitialized() => LoadAccounts();

    protected void LoadAccounts() => Accounts = Repository.GetAll();

    protected void OpenAddDialog()
    {
        NewAccount = new Account
        {
            Id = string.Empty,
            Nickname = string.Empty,
            Username = string.Empty,
            Password = string.Empty,
            Region = "EUW",
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

            if (string.IsNullOrWhiteSpace(NewAccount.Nickname))
            {
                ErrorMessage = "Account name is required!";
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

            NewAccount.Id = Guid.NewGuid().ToString();
            NewAccount.EncryptedPassword = Encryptor.Encrypt(NewAccount.Password);
            Accounts.Add(NewAccount);

            Repository.SaveAll(Accounts);
            CloseAddDialog();
            LoadAccounts();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving account: {ex.Message}";
        }
    }

    protected async Task SaveEditedAccount()
    {
        try
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(EditAccountModel.Nickname))
            {
                ErrorMessage = "Account name is required!";
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

                Repository.SaveAll(Accounts);
                CloseEditDialog();
                LoadAccounts();
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

    protected void ConfirmDelete()
    {
        Accounts.RemoveAll(a => a.Id == _accountToDeleteId);
        Repository.SaveAll(Accounts);
        ShowDeleteConfirmation = false;
        LoadAccounts();
    }

    protected void CancelDelete() => ShowDeleteConfirmation = false;

    protected async Task Login(Account account)
    {
        NavigateToDetails(account.Username);

        await RiotClient.AutoLogin(account);
    }

    protected static async Task OpenBuyMeACoffee()
    {
        const string url = "https://buymeacoffee.com/kermo";
        try
        {
            await Launcher.Default.OpenAsync(url);
        }
        catch (Exception ex)
        {
            // Handle exception
        }
    }

    protected void NavigateToDetails(string username) =>
        NavigationManager.NavigateTo($"/accountDetails/{username}");
}
