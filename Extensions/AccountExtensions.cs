using RiotAccountManager.MAUI.Data.Models;

namespace RiotAccountManager.MAUI.Extensions;

public static class AccountExtensions
{
    public static bool IsExistingAccount(this Account account) =>
        !string.IsNullOrEmpty(account.Username);

    public static void UpdateFrom(this Account existing, Account source)
    {
        existing.Nickname = source.Nickname;
        existing.Username = source.Username;
        existing.Region = source.Region;
    }
}
