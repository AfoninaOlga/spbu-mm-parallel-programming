using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChatGui.ViewModels;

namespace ChatGui.Views;

public partial class ChatScreen : ReactiveUserControl<ChatScreenViewModel>
{
    public ChatScreen()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // this.WhenActivated((CompositeDisposable disposable) => { });
        AvaloniaXamlLoader.Load(this);
    }
}

