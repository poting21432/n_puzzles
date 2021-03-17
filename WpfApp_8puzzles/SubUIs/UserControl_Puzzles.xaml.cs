using System;
using System.Collections.Generic;
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
        public int PuzzleData
        {
            get { return (int)GetValue(PuzzleDataProperty); }
            set {
                SetValue(PuzzleDataProperty, value);
                for (int i = 0; i < 9; i++)
                {
                    string ctrlname = string.Format("Button_P{0}", i);
                    ((Button_Puzzle)this.FindName(ctrlname)).Data = ConvertPuzzeID(i);
                }
                
            }
        }
        // Using a DependencyProperty as the backing store for PuzzleDataProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PuzzleDataProperty =
            DependencyProperty.Register("PuzzleData", typeof(int), typeof(UserControl_Puzzles), new PropertyMetadata(Puzzles.DefaultValue));
        
        public UserControl_Puzzles()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public int ConvertPuzzeID(int pid)
        {
            string data = PuzzleData.ToString(Puzzles.strF);;
            if (data.Length > 9)
                return 0;
            int outD = 0; int.TryParse(data[pid].ToString(),out outD);
            return outD;
        }
        private Action EmptyDelegate = delegate () { };
        
        public void ForceRefresh()
        {
            this.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
        private List<int> Movables = new List<int>();
        private void Button_Pos_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int pos = -1; int.TryParse(btn.Tag.ToString(),out pos);
            if (pos < 0) return;
            Movables.Clear();
            int IDof0 = Puzzles.FindPos1D(PuzzleData, 0);
            //可向右換
            if (!((IDof0 - (Puzzles.PWidth - 1)) % 3 == 0))
                Movables.Add(IDof0 + 1);
            //可向左換
            if (!(IDof0 % Puzzles.PWidth == 0))
                Movables.Add(IDof0 - 1);
            //可向上換
            if (!(IDof0 < Puzzles.PWidth))
                Movables.Add(IDof0 - 3);
            //可向下換
            if (!(IDof0 + Puzzles.PWidth > Puzzles.PSize - 1))
                Movables.Add(IDof0 + 3);

            if (Movables.Contains(pos))
                PuzzleData = Puzzles.ValueOfExchange(PuzzleData, IDof0, pos);
        }
    }
}
