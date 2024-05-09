using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MetarAppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string invalidLengthMsg = "ICAO must be 4 characters in length!";
        private readonly string invalidICAOMsg = "Invalid ICAO!";

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "METAR Parser";
        }

        private void SubmitICAOBtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the ICAO code from the textbox
            string icao = icaoTB.Text;

            // Check if the ICAO code is valid
            if (icao.Length != 4)
            {
                MessageBox.Show(invalidLengthMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get the METAR data from the API
            string metar = ApiHandler.RequestMetar(icao);

            // Check if the METAR data is valid
            if (metar == null || metar == "-1")
            {
                MessageBox.Show(invalidICAOMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Show the METAR page, hide ICAO page
            ICAOPage();
            METARPage();

            // Write the METAR data to the textbox
            MetarParser.GetParsedMetar(metar);

            metarTbEnc.Text = MetarParser.GetEncodedMetarAsString();
            metarTbDec.Text = MetarParser.GetDecodedMetarAsString();
            
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            // Shutdown the application on exit button click
            Application.Current.Shutdown();
        }

        private void ICAOPage()
        {
            if(icaoTB.IsVisible)
            {
                icaoTB.Visibility = Visibility.Hidden;
                icaoLbl.Visibility = Visibility.Hidden;
                submitIcaoBtn.Visibility = Visibility.Hidden;
            }
            else
            {
                icaoTB.Visibility = Visibility.Visible;
                icaoLbl.Visibility = Visibility.Visible;
                submitIcaoBtn.Visibility = Visibility.Visible;
            }
        }

        private void METARPage()
        {
            if(metarTbEnc.IsVisible)
            {
                metarTbEnc.Visibility = Visibility.Hidden;
                metarTbDec.Visibility = Visibility.Hidden;
            }
            else
            {
                metarTbEnc.Visibility = Visibility.Visible;
                metarTbDec.Visibility = Visibility.Visible;
            }
        }
    }

    public class ControlWriter : TextWriter
    {
        private Control textbox;
        public ControlWriter(Control textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {
            textbox.DataContext += value.ToString();
        }

        public override void Write(string value)
        {
            textbox.DataContext += value;
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}