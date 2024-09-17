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

namespace TractorSupporter.View.Controls
{
    /// <summary>
    /// Interaction logic for NavigationButtons.xaml
    /// </summary>
    public partial class NavigationButtons : UserControl
    {
        public NavigationButtons()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty BackCommandProperty =
            DependencyProperty.Register("BackCommand", typeof(ICommand), typeof(NavigationButtons));

        public static readonly DependencyProperty ForwardCommandProperty =
            DependencyProperty.Register("ForwardCommand", typeof(ICommand), typeof(NavigationButtons));

        public ICommand BackCommand
        {
            get { return (ICommand)GetValue(BackCommandProperty); }
            set { SetValue(BackCommandProperty, value); }
        }

        public ICommand ForwardCommand
        {
            get { return (ICommand)GetValue(ForwardCommandProperty); }
            set { SetValue(ForwardCommandProperty, value); }
        }
    }
}
