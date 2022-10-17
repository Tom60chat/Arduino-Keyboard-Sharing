using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ArduinoKeyboard
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Refresh_Port();

            PortChat.Output += (s) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Output_TextBox.Text += s + "\n";
                });
            };
        }

        string[] FetchingArduino()
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("The following serial ports were found:");

            // Display each port name to the console.
            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }

            Console.ReadLine();
            return ports;
        }

        BackgroundWorker worker = new BackgroundWorker();

        void Arduino(string Port)
        {
            if (Port == null || Port == "")
                return;

            if (worker.IsBusy)
            {
                PortChat.Stop();
                Start_Button.Content = "Start";
                return;
            }

            worker.DoWork += (s, e) =>
            {
                PortChat.Start(Port);
            };

            Start_Button.Content = "Stop";
            worker.RunWorkerAsync();
        }

        void Refresh_Port()
        {
            Port_ComboBox.Items.Clear();
            foreach (string port in FetchingArduino())
                Port_ComboBox.Items.Add(port);

            if (Port_ComboBox.Items.Count != 0)
            {
                Start_Button.Visibility = Visibility.Visible;
                Port_ComboBox.SelectedItem = Port_ComboBox.Items[0];
            }
            else
                Start_Button.Visibility = Visibility.Hidden;
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            Refresh_Port();
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            if(Port_ComboBox.SelectedItem != null)
                Arduino(Port_ComboBox.SelectedItem.ToString());
        }
    }
}
