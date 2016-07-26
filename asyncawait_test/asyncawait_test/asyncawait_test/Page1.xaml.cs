using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace asyncawait_test
{
    public partial class Page1 : ContentPage
    {
        public Page1()
        {
            InitializeComponent();
        }
    }

    public class ViewModel : INotifyPropertyChanged
    {

        public const string MyListPropertyName = "MyList";

        private ObservableCollection<string> _myList = new ObservableCollection<string>();

        /// <summary>
        /// ListViewのItemsSource
        /// </summary>
        public ObservableCollection<string> MyList
        {
            get
            {
                return _myList;
            }

            set
            {
                if (_myList == value)
                {
                    return;
                }

                _myList = value;
                RaisePropertyChanged(MyListPropertyName);
            }
        }

        private RelayCommand _myCommand;

        /// <summary>
        /// ボタンのコマンド
        /// </summary>
        public RelayCommand MyCommand
        {
            get
            {
                return _myCommand
                    ?? (_myCommand = new RelayCommand(
                                          async () =>
                                          {
                                              await Task.Factory.StartNew(TestMethod);
                                          }));
            }
        }

        //awaitで呼び出されるため別スレッドで実行されるメソッド
        private void TestMethod()
        {
            //これならOK
            Device.BeginInvokeOnMainThread(() => MyList.Add("aaa"));
        }

        #region INotifyPropertyChangedの実装とプロパティ変更メソッド

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

    public class RelayCommand : ICommand
    {
        private Action _action;
        public RelayCommand(Action action)
        {
            _action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke();
        }
    }

}
