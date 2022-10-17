using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Windows.Input;

namespace ArduinoKeyboard
{
    class PortChat
    {
        static bool _continue;
        static SerialPort _serialPort;
        public static Action<string> Output = (m) => { Console.WriteLine(m); };

        public static void Start(string Port)
        {
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            Thread readThread = new Thread(Read);
            Thread writeThread = new Thread(Write);

            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();

            // Allow the user to set the appropriate properties.
            _serialPort.PortName = Port;
            /*_serialPort.BaudRate = ;
            _serialPort.Parity = ;
            _serialPort.DataBits = ;
            _serialPort.StopBits = ;
            _serialPort.Handshake = ;*/

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 2000;
            //_serialPort.WriteTimeout = 2000;

            Output.Invoke("Open serial: " + _serialPort.PortName);
            _serialPort.Open();
            _continue = true;
            //readThread.Start();
            writeThread.Start();

            //readThread.Join();
            writeThread.Join();
            Output.Invoke("Close serial");
            _serialPort.Close();
        }

        private static void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    Output.Invoke(message);
                }
                catch (TimeoutException)
                {
                    Output.Invoke("Read Timeout"); ;
                }
            }
        }

        private static void Write()
        {
            Random random = new Random();
            List<Key> Old_keysDown = new List<Key>();
            while (_continue)
            {
                Thread.Sleep(50);
                try
                {
                    List<Key> keysDown = new List<Key>();
                    List<Key> keysUp = new List<Key>();
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (Key key in Enum.GetValues(typeof(Key)))
                            if (key != Key.None)
                                if (Keyboard.IsKeyDown(key))
                                        keysDown.Add(key);
                    });

                    foreach (Key Key in Old_keysDown)
                    {
                        if (!keysDown.Contains(Key))
                            keysUp.Add(Key);
                    }

                    string message = "";
                    foreach (Key key in keysDown)
                        message += "Down:" + key + "&";
                    foreach (Key key in keysUp)
                        message += "Up:" + key + "&";
                    if (message != "" && _serialPort.IsOpen)
                    {
                        _serialPort.Write(message);
                        Output.Invoke(message);
                    }
                    Old_keysDown = keysDown;
                }
                catch (TimeoutException)
                {
                    Output.Invoke("Write Timeout"); ;
                }
            }
        }

        public static void Stop()
        {
            Output.Invoke("Stop"); ;
            _continue = false;
        }
    }
}
