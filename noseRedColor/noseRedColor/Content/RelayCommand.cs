using System.Windows.Input;

namespace noseRedColor.Content
{
    public class RelayCommand : ICommand
    {
        //  이벤트 실행할 내용
        private readonly Action<object> _execute;
        //  실행 가능 여부 확인용
        private readonly Func<object, bool> _canExecute;

        //  이벤트 실행 함수 연결 실패시 에러 발생
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        //  이벤트 제거시 사용
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);
    }
}
