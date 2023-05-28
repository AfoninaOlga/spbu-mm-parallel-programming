using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Collections;
using Chat;
using Chat.NetworkMessage;
using ReactiveUI;

namespace ChatGui.ViewModels;

using Chat = Chat.Chat;

public class ChatScreenViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public ChatScreenViewModel(IScreen screen, Connection connection, string username)
    {
        _chat = new Chat(connection, username, _messages, _members);
        HostScreen = screen;
        this.WhenActivated(disposables =>
        {
            _chat.Start();
            Disposable
                .Create(() => { _chat.Dispose(); })
                .DisposeWith(disposables);
        });
    }

    private readonly Chat _chat;

    public string MeInfo => $"{_chat.Me.Username} ({_chat.Me.Ip}:{_chat.Me.Port})";

    public IScreen HostScreen { get; }

    public string UrlPathSegment => Guid.NewGuid().ToString();

    
    private AvaloniaList<string> _messages = new AvaloniaList<string>();

    public AvaloniaList<string> Messages => _messages;

    private AvaloniaList<MemberInfo> _members = new();
    public AvaloniaList<MemberInfo> Members => _members;

    private string _messageToSend;

    public string MessageToSend
    {
        get => _messageToSend;
        set => this.RaiseAndSetIfChanged(ref _messageToSend, value);
    }

    public void OnSendClick()
    {
        if (string.IsNullOrEmpty(MessageToSend)) return;
        _chat.SendText(MessageToSend);
        MessageToSend = "";
    }
    public ReactiveCommand<Unit, Unit> GoBack => HostScreen.Router.NavigateBack;

    public ViewModelActivator Activator { get; } = new ViewModelActivator();
}
