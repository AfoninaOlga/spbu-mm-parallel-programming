using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using Chat;
using ReactiveUI;

namespace ChatGui.ViewModels;

public class ConnectToChatScreenViewModel : ReactiveObject, IRoutableViewModel
{
    public ConnectToChatScreenViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;

        this.WhenAnyValue(x => x.EndPoint, x => x.Username)
            .Subscribe(_ => UpdateCanConnect());

        var canConnectChat = this
            .WhenAnyValue(x => x.CanNavigateChat);
        GoChat = ReactiveCommand.CreateFromObservable(() =>
        {
            var connection = Connection.ConnectTo(IPEndPoint.Parse(EndPoint));
            return HostScreen.Router.Navigate.Execute(new ChatScreenViewModel(HostScreen, connection, Username));
        }, canConnectChat);

        GoChat.ThrownExceptions.Subscribe(error =>
        {
            MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("Не удаётся подключится",
                    $"Не удаётся подключится к чату по адресу {EndPoint}. Проверьте правильность адреса.").Show();
        });


        var canGoBack = this
            .WhenAnyValue(x => x.HostScreen.Router.NavigationStack.Count)
            .Select(count => count > 0);
        GoBack = ReactiveCommand.CreateFromObservable(
            () => HostScreen.Router.NavigateBack.Execute(Unit.Default),
            canGoBack);
    }

    public IScreen HostScreen { get; }

    public string UrlPathSegment => Guid.NewGuid().ToString();

    public ReactiveCommand<Unit, IRoutableViewModel> GoChat { get; }
    public ReactiveCommand<Unit, Unit> GoBack { get; }

    private string? _endPoint;

    [Required]
    public string? EndPoint
    {
        get => _endPoint;
        set => this.RaiseAndSetIfChanged(ref _endPoint, value);
    }

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
        IPEndPoint result;
        CanNavigateChat =
            !string.IsNullOrEmpty(_endPoint)
            && IPEndPoint.TryParse(_endPoint, out result)
            && !string.IsNullOrEmpty(_username);
    }
}
