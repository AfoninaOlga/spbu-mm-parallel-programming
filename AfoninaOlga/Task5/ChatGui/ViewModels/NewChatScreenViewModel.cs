using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
using Chat;
using ReactiveUI;

namespace ChatGui.ViewModels;

public class NewChatScreenViewModel : ReactiveObject, IRoutableViewModel
{
    public NewChatScreenViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;

        this.WhenAnyValue(x => x.SelectedInterface, x => x.Username)
            .Subscribe(_ => UpdateCanConnect());

        var canCreateChat = this
            .WhenAnyValue(x => x.CanNavigateChat);
        GoChat = ReactiveCommand.CreateFromObservable(() =>
        {
            var connection = Connection.NewConnection(new IPEndPoint(SelectedInterface.Address, 0));
            return HostScreen.Router.Navigate.Execute(new ChatScreenViewModel(HostScreen, connection, Username));
        }, canCreateChat);

        var canGoBack = this
            .WhenAnyValue(x => x.HostScreen.Router.NavigationStack.Count)
            .Select(count => count > 0);
        GoBack = ReactiveCommand.CreateFromObservable(
            () => HostScreen.Router.NavigateBack.Execute(Unit.Default),
            canGoBack);
    }

    public IScreen HostScreen { get; }

    public string UrlPathSegment => Guid.NewGuid().ToString();

    public class NetworkInterfaceInfo
    {
        public NetworkInterfaceInfo(NetworkInterface networkInterface)
        {
            _networkInterface = networkInterface;
        }

        private readonly NetworkInterface _networkInterface;

        public string Name => _networkInterface.Name;

        public string Type => _networkInterface.NetworkInterfaceType.ToString();

        public IPAddress Address => _networkInterface
            .GetIPProperties()
            .UnicastAddresses
            .First(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
            .Address;
    }

    public NetworkInterfaceInfo? SelectedInterface { get; set; }

    public IEnumerable<NetworkInterfaceInfo> NetworkInterfaces => NetworkInterface
        .GetAllNetworkInterfaces()
        .Where(nInterface => nInterface.OperationalStatus == OperationalStatus.Up)
        .Select(nInterface => new NetworkInterfaceInfo(nInterface))
        .OrderBy(info => info.Type);


    public ReactiveCommand<Unit, IRoutableViewModel> GoChat { get; }

    public ReactiveCommand<Unit, Unit> GoBack { get; }
    
    private string? _username;

    [Required]
    public string? Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }
    
    private bool _canNavigateChat;
    
    public bool CanNavigateChat
    {
        get => _canNavigateChat;
        protected set => this.RaiseAndSetIfChanged(ref _canNavigateChat, value);
    }
    
    private void UpdateCanConnect()
    {
        CanNavigateChat =
            SelectedInterface != null
            && !string.IsNullOrEmpty(_username);
    }
}
