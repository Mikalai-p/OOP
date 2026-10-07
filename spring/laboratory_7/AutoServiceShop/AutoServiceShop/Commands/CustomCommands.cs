using System.Windows.Input;

namespace AutoServiceShop.Commands
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand ResetRating = new RoutedUICommand(
            "Сбросить рейтинг",
            "ResetRating",
            typeof(CustomCommands),
            new InputGestureCollection { new KeyGesture(Key.R, ModifierKeys.Control) }
        );
    }
}