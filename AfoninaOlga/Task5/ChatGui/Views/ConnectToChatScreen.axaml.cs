using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChatGui.ViewModels;

namespace ChatGui.Views;

public partial class ConnectToChatScreen : ReactiveUserControl<ConnectToChatScreenViewModel>
{
    public ConnectToChatScreen()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

