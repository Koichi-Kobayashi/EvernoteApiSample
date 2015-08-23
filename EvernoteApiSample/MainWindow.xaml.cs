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
using EvernoteSDK;
using FirstFloor.ModernUI.Windows.Controls;

namespace EvernoteApiSample
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : ModernWindow
  {
    private const string EVERNOTE_HOST = "www.evernote.com";
    private const string EVERNOTE_SANDBOX_HOST = "sandbox.evernote.com";

    public MainWindow()
    {
      InitializeComponent();
      InitializeEvernote();
    }

    private void InitializeEvernote()
    {
      ENSession.SetSharedSessionConsumerKey(Properties.Settings.Default.ConsumerKey, Properties.Settings.Default.ConsumerSecret, EVERNOTE_SANDBOX_HOST);
      if (ENSession.SharedSession.IsAuthenticated == false)
      {
        ENSession.SharedSession.AuthenticateToEvernote();
      }
    }
  }
}
