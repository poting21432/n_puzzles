using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// <summary>
    /// UserControl_Puzzle.xaml 的互動邏輯
    /// </summary>
    /// 
    public partial class UserControl_Puzzles : UserControl
    {
        public bool IsReadOnly = false;
        public ObservableCollection<ButtonPuzzle> Button_Puzzles { get; set; }
        public UserControl_Puzzles()
        {
            InitializeComponent();
            Button_Puzzles = new ObservableCollection<ButtonPuzzle>();

            for (int i = 0; i < Puzzles.PSize; i++)
            {
                Button_Puzzles.Add(new ButtonPuzzle(Button_Pos_Click) { Data = Puzzles.DefaultValue[i] ,Tag = i});
            }
            ItemsControl_Puzzles.ItemsSource = Button_Puzzles;

        }
        public List<int> GetCurrentPuzzles()
        {
            List<int> Puzzlse = new List<int>();
            for (int i = 0; i < Puzzles.PSize; i++)
                Puzzlse.Add(Button_Puzzles[i].Data);
            return Puzzlse;
        }
        public bool Update(List<int> pid)
        {
            if (pid.Count != Puzzles.PSize)
                return false;
            for (int i = 0; i < Puzzles.PSize; i++)
            {
                Button_Puzzles[i].Data = pid[i];
                Button_Puzzles[i].Update();
            }
            return true;
        }
        
        private void Button_Pos_Click(ButtonPuzzle btn)
        {
            if (IsReadOnly)
                return;
            int pos = -1; int.TryParse(btn.Tag.ToString(),out pos);
            if (pos < 0) return;

            for (int i = 0; i < Puzzles.PSize; i++)
            {
                int data = Button_Puzzles[pos].Data;
                ///  0  1  2  3
                ///  4  5  6  7
                ///  8  9 10 11
                /// 12 13 14 15
                if (pos % 4 != 0 && Button_Puzzles[pos - 1].Data == 0)
                {
                    Button_Puzzles[pos - 1].Data = data;
                    Button_Puzzles[pos].Data = 0;
                }
                if ((pos + 1) % 4 != 0 && Button_Puzzles[pos + 1].Data == 0)
                {
                    Button_Puzzles[pos + 1].Data = data;
                    Button_Puzzles[pos].Data = 0;
                }
                if (pos >= Puzzles.PWidth && Button_Puzzles[pos - 4].Data == 0)
                {
                    Button_Puzzles[pos - 4].Data = data;
                    Button_Puzzles[pos].Data = 0;
                }
                if (pos < Puzzles.PSize - Puzzles.PWidth && Button_Puzzles[pos + 4].Data == 0)
                {
                    Button_Puzzles[pos + 4].Data = data;
                    Button_Puzzles[pos].Data = 0;
                }

            }
        }
    }
}
