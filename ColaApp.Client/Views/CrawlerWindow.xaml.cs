using ColaApp.Client.ViewModels;
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
using System.Windows.Shapes;

namespace ColaApp.Client.Views
{
    /// <summary>
    /// Interaction logic for CrawlerWindow.xaml
    /// </summary>
    public partial class CrawlerWindow : BaseWindow
    {
        private CrawlerViewModel crawlerViewModel;

        public CrawlerWindow()
        {
            InitializeComponent();            
            crawlerViewModel = new CrawlerViewModel();
            this.DataContext = crawlerViewModel;
        }
    }
}
