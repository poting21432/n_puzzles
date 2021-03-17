using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp_8puzzles
{
    public delegate void PuzzleClick(ButtonPuzzle btn);
    /// <summary>
    /// Button_Puzzle.xaml 的互動邏輯
    /// </summary>
    public partial class ButtonPuzzle : Button
    {
        public int Data
        {
            get {
                return (int)GetValue(DataProperty);
            }
            set {
                SetValue(DataProperty, value);
                this.Visibility = (Data == 0) ? Visibility.Hidden : 
                                                Visibility.Visible;
                this.Content = Data.ToString();
            }
        }
        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(int), typeof(ButtonPuzzle), new PropertyMetadata(0));
        public PuzzleClick puzzleClick;
        public ButtonPuzzle(PuzzleClick EventClick)
        {
            InitializeComponent();
            puzzleClick += EventClick;
            this.DataContext = this;
        }
        public void Update()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
            //EDIT:
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                          new Action(delegate { }));
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            puzzleClick(this);
        }
    }
}
