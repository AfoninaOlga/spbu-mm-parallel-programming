using System;
using System.Windows.Input;

namespace Task5Ui
{
	internal class Command : ICommand
	{
		private readonly Action execute;
		private readonly Func<bool> canExecute;

		public event EventHandler? CanExecuteChanged;

		public Command(Action execute, Func<bool> canExecute)
		{
			this.execute = execute;
			this.canExecute = canExecute;
		}

		public bool CanExecute(object? parameter) => canExecute();

		public void Execute(object? parameter) => execute();
	}
}
