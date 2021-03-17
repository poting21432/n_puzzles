using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfApp_8puzzles
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        //283164705
        //283164705
        private Puzzles puzzles;
        public MainWindow()
        {
            InitializeComponent();
            puzzles = new Puzzles(MainPuzzles);
            Button_Play.IsEnabled = false;
            Puzzles_First.IsReadOnly = true;
            Puzzles_End.IsReadOnly = true;
        }

        private void Button_BFSSearch_Click(object sender, RoutedEventArgs e)
        {
            Button_BFSSearch.IsEnabled = false;
            puzzles.UpdateInitValueFromUI(Puzzles_First);
            puzzles.BFS_Search();
            (new Thread(() =>
            {
                SyncUI();
            }) { IsBackground = true }).Start();
        }
        private void SyncUI()
        {
            while(true)
            {
                this.Dispatcher.Invoke(() =>
                {
                    TextBlock_TimeCost.Text = puzzles.TimeCost.ToString("F2");
                    TextBlock_Cost.Text = puzzles.CurrentDegree.ToString();
                    TextBlock_Degree.Text = puzzles.Parent.Count.ToString();

                    Button_Play.IsEnabled = puzzles.AnswerFound;
                    Button_BFSSearch.IsEnabled = puzzles.AnswerFound;
                });
                Thread.Sleep(1000);
            }
        }
        private void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            if (puzzles.Degree == -1)
                return;
            foreach (ulong stae in puzzles.Route)
            {
                List<int> LState = new List<int>();
                Puzzles.Hash2State(stae, ref LState);
                MainPuzzles.Update(LState);
                Thread.Sleep(400);
            }
        }

        private void Button_SetData_Click(object sender, RoutedEventArgs e)
        {
            Puzzles_First.Update(MainPuzzles.GetCurrentPuzzles());
        }
    }
}
