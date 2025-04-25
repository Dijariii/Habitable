using Avalonia.Controls;
using Avalonia.Input;
using Habitable.ViewModels;

namespace Habitable.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void OnToggleAuthMode(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is LoginViewModel viewModel)
        {
            viewModel.ToggleAuthMode();
        }
    }
}