using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;

namespace ChatGui.ViewModels;

public class MainWindowViewModel : ReactiveObject, IScreen
{
    public MainWindowViewModel()
    {
        GoNewChat  = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(new NewChatScreenViewModel(this)));
        GoConnectToChat  = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(new ConnectToChatScreenViewModel(this)));
    }

    public RoutingState Router { get; } = new();

    public ReactiveCommand<Unit, IRoutableViewModel> GoNewChat { get; }

    public ReactiveCommand<Unit, IRoutableViewModel> GoConnectToChat { get; }
}
