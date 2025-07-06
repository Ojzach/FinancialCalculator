

using System.Data.Common;
using System.Reflection.Metadata;

namespace FinancialCalculator.Commands
{
    class RelayCommand : CommandBase
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public override void Execute(object parameter) => _execute(parameter);
        public override bool CanExecute(object parameter) => (_canExecute == null || _canExecute(parameter));
    }
}
