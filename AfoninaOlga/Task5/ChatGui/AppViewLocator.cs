using System;
using ChatGui.ViewModels;
using ChatGui.Views;
using ReactiveUI;

namespace ChatGui;

public class AppViewLocator : IViewLocator
{
    public IViewFor ResolveView<T>(T viewModel, string? contract = null) => viewModel switch
    {
        NewChatScreenViewModel context => new NewChatScreen { DataContext = context },
        ConnectToChatScreenViewModel context => new ConnectToChatScreen { DataContext = context },
        
        ChatScreenViewModel context => new ChatScreen { DataContext = context },
        
        _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
    };
}
