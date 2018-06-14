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

namespace WpfPhotoFilter
{
    /// <summary>
    /// FuncButton.xaml 的互動邏輯
    /// </summary>
    public partial class FuncButton : UserControl
    {
        public string Text
        {
            get { return ButtonText.Text; }
            set { ButtonText.Text = value; }
        }

        public FuncButton()
        {
            InitializeComponent();
        }
    }
}
