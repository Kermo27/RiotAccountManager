using RiotAccountManager.MAUI.Components.Pages;

namespace RiotAccountManager.MAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new MainPage()) { Title = "Riot Account Manager" };
    }
}
