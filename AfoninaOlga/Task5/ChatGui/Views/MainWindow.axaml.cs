using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ChatGui.ViewModels;

namespace ChatGui.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }
}