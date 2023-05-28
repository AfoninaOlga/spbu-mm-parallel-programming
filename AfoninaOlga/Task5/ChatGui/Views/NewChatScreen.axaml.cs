using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ChatGui.ViewModels;

namespace ChatGui.Views;

public partial class NewChatScreen : ReactiveUserControl<NewChatScreenViewModel>
{
    public NewChatScreen()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

