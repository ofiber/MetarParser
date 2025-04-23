using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Dark.Net; // Used for Light/Dark mode

namespace MetarAppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Invalid entry error messages
        private readonly string invalidLengthMsg = "ICAO must be 4 characters in length!";
        private readonly string invalidICAOMsg = "Invalid ICAO!";
        private readonly string invalidMETARMsg = "The METAR you entered is invalid! Please try again.";

        public MainWindow()
        {
            // Set application theme to user's system theme
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Auto);

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
                // If invalid, show error message and return
                MessageBox.Show(invalidLengthMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (!ICAODict.icaoDict.ContainsKey(icao))
            {
                // If ICAO is not contained in list of global airports, show error message and return
                MessageBox.Show(invalidICAOMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Get the METAR data from the API
            string metar = ApiHandler.RequestMetar(icao);

            // Check if the METAR data is valid
            if (metar == null || metar == "-1")
            {
                // If invalid, show error message and return
                MessageBox.Show(invalidICAOMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Set progress bar to visible, and run the progress method
            progressBar.Visibility = Visibility.Visible;
            Progress();

            // Parse the METAR
            MetarParser.GetParsedMetar(metar);

            // Write original and parsed METAR to text boxes
            metarTbEnc.Text = MetarParser.GetEncodedMetarAsString();
            metarTbDec.Text = MetarParser.GetDecodedMetarAsString();
        }

        private async void Progress()
        {
            // Wait 1.75 seconds before hiding the progress bar
            await Task.Delay(1750);
            progressBar.Visibility = Visibility.Hidden;

            // Hide main menu elements, show METAR display elements
            ICAOPage();
            METARPage();
        }

        private void SubmitMETARBtn_Click(object sender, RoutedEventArgs e)
        {
            string metar = usrMetarTb.Text;

            if(!ValidateMetar.Validate(metar))
            {
                MessageBox.Show(invalidMETARMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                usrMetarTb.Clear();
                return;
            }

            // Set progress bar to visible, and run the progress method
            progressBar.Visibility = Visibility.Visible;
            Progress();

            MetarParser.GetParsedMetar(metar);

            metarTbEnc.Text = MetarParser.GetEncodedMetarAsString();
            metarTbDec.Text = MetarParser.GetDecodedMetarAsString();
        }
        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            // Shutdown the application on exit button click
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if(result == MessageBoxResult.Yes)
                Application.Current.Shutdown();
            else
                return;
        }

        // Function to press escape and quit application

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            //Console.Clear();

            // If on main menu, do nothing
            if(metarTbDec.Visibility == Visibility.Hidden)
                return;
            else
            {
                metarTbEnc.Clear();
                metarTbDec.Clear();

                icaoTB.Clear();
                usrMetarTb.Clear();

                MetarParser.ResetMetarStrings();

                METARPage();
                ICAOPage();
            }
        }

        private void ICAOPage()
        {
            // Show or hide ICAO entry elements
            if(icaoTB.IsVisible)
            {
                icaoTB.Visibility = Visibility.Hidden;
                icaoLbl.Visibility = Visibility.Hidden;
                submitIcaoBtn.Visibility = Visibility.Hidden;
                metarLbl.Visibility = Visibility.Hidden;
                usrMetarTb.Visibility = Visibility.Hidden;
                submitMetarBtn.Visibility = Visibility.Hidden;
            }
            else
            {
                icaoTB.Visibility = Visibility.Visible;
                icaoLbl.Visibility = Visibility.Visible;
                submitIcaoBtn.Visibility = Visibility.Visible;
                metarLbl.Visibility = Visibility.Visible;
                usrMetarTb.Visibility= Visibility.Visible;
                submitMetarBtn.Visibility= Visibility.Visible;
            }
        }

        private void METARPage()
        {
            // Show or hide METAR display elements
            if(metarTbEnc.IsVisible)
            {
                metarTbEnc.Visibility = Visibility.Hidden;
                metarTbDec.Visibility = Visibility.Hidden;
                backBtn.Visibility = Visibility.Hidden;
            }
            else
            {
                metarTbEnc.Visibility = Visibility.Visible;
                metarTbDec.Visibility = Visibility.Visible;
                backBtn.Visibility = Visibility.Visible;
            }
        }

        private void icaoTB_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
                SubmitICAOBtn_Click(sender, e);
        }

        private void usrMetarTb_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
                SubmitMETARBtn_Click(sender, e);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape && backBtn.Visibility == Visibility.Visible)
                BackBtn_Click(sender, e);
            else if(e.Key == System.Windows.Input.Key.Escape && backBtn.Visibility == Visibility.Hidden)
                ExitBtn_Click(sender, e);
        }
    }

    /// <summary>
    /// Allows use of Console.Write/WriteLine calls
    /// to be output to WPF elements instead of console
    /// </summary>
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