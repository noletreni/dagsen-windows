using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace dagsen
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            date.Text = DateTime.Today.ToString("d.M.yyyy");
            System.Net.Http.HttpClient hc = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response = await hc.GetAsync("http://api.teknolog.fi/taffa/sv/today/");
            response.EnsureSuccessStatusCode();
            menu.Text = await response.Content.ReadAsStringAsync();


            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text05);

            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = menu.Text;


            XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text03);
            XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
            squareTileTextAttributes[0].AppendChild(squareTileXml.CreateTextNode(menu.Text));
            IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
            tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(tileXml);

            tileNotification.ExpirationTime = DateTime.Now.Date.AddDays(1);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);



        }

    }
}
