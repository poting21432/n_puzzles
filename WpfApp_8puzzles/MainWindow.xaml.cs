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

namespace WpfApp_8puzzles
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        //283164705
        //283164705
        Puzzles puzzles = new Puzzles();
        public MainWindow()
        {
            InitializeComponent();
            puzzles.SyncWith(MainPuzzles);
            this.DataContext = puzzles;
        }

        private void Button_ConfirmData_Click(object sender, RoutedEventArgs e)
        {
            
            int value = 0;
            if (int.TryParse(Textbox_Data.Text, out value))
            {
                puzzles.InitValue = value;
                puzzles.ValueSync = value;
                puzzles.SyncWith(MainPuzzles);
            }
                
            else
                return;
        }

        private void Button_ConfirmTarget_Click(object sender, RoutedEventArgs e)
        {
            int target = 0;
            if (int.TryParse(Textbox_Target.Text, out target))
                puzzles.TargetValue = target;
            else
            {
                MessageBox.Show("不合法的資料");
                return;
            }
        }

        private void Button_BFSSearch_Click(object sender, RoutedEventArgs e)
        {
            BFS_SearchTask();
        }

        public void BFS_SearchTask()
        {
            puzzles.BFS_Search(CheckBox_EnUI.IsChecked);
            
        }
        private void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            if (!puzzles.AnswerFound)
            {
                MessageBox.Show("此問題無解");
                return;
            }
            puzzles.DoAnswer();
        }

        private void Button_SetData_Click(object sender, RoutedEventArgs e)
        {
            Textbox_Data.Text = MainPuzzles.PuzzleData.ToString(Puzzles.strF);
        }
        private void Button_SetTarget_Click(object sender, RoutedEventArgs e)
        {
            Textbox_Target.Text = MainPuzzles.PuzzleData.ToString(Puzzles.strF);
        }
    }
}
