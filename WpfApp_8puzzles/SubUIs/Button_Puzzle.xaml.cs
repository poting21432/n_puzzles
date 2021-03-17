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
    /// Button_Puzzle.xaml 的互動邏輯
    /// </summary>
    public partial class Button_Puzzle : Button
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
            DependencyProperty.Register("Data", typeof(int), typeof(Button_Puzzle), new PropertyMetadata(0));

        public Button_Puzzle()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
