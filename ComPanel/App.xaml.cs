using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using ComPanel.ModulesLoader;
using ComPanel.Connection;
using System.IO.Ports;
using ComPanel.Resources.Controls;
using ComPanel.DataFlow;
using ComPanel.RawData;

namespace ComPanel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Connection.Connection _connection;
        private MainWindow _window;
        private PortSettings _portSettings;
        private DataFlowController _dataFlowController;
        private DataFormater _dataFormater;

        public App() 
            : base()
        {
            _connection = new Test.TestConnection();
            _portSettings = new PortSettings();
            _dataFormater = new HexDataFormater();

            _dataFlowController = new DataFlowController(_connection.Send);
            _dataFlowController.OnEventuallyDataRecieved = DisplayEventuallyRecievedData;
            _connection.OnDataRecieved = _dataFlowController.OnDataRecieved;

            _window = new MainWindow();
            _window.PortComboBox.ItemsSource = PortSettings.GetPortNames();
            _window.PortComboBox.SelectedIndex = 0;
            _window.PortComboBox.SelectionChanged += (sender, args) =>
            {
                var selected = _window.PortComboBox.SelectedItem;
                if (selected != null)
                    _portSettings.PortName = (string)selected;
            };

            _window.BaudRateComboBox.ItemsSource = PortSettings.GetCommonBaudRates();
            _window.BaudRateComboBox.SelectedIndex = 0;

            _window.ParityComboBox.ItemsSource = Enum.GetValues(typeof(Parity));
            _window.ParityComboBox.SelectedIndex = 0;

            _window.DataBitsComboBox.ItemsSource = new int[] { 1, 2, 4, 8, 16 };
            _window.DataBitsComboBox.SelectedIndex = 0;

            _window.StopBitsComboBox.ItemsSource = Enum.GetValues(typeof(StopBits));
            _window.StopBitsComboBox.SelectedIndex = 0;

            _window.TimeOutMSComboBox.ItemsSource = new int[] { 100, 200, 500, 1000 };
            _window.TimeOutMSComboBox.SelectedIndex = 0;

            //Adding modules to modules menu
            foreach (var module in LoadModules())
            {
                _window.ModulesMenu.AddModuleButton("module", () =>
                {
                    _window.ContentStackPanel.Children.Clear();
                    _window.ContentStackPanel.Children.Add(module.GetPanel());
                });

                module.Send = (data, length, onRecieved) =>
                {
                    var dataPair = new DataFlowPair();
                    dataPair.SetSent(_dataFormater.ToString(data, length));

                    if (onRecieved != null)
                        _dataFlowController.Send(data, length, (d, l) =>
                        {
                            onRecieved(data, length);
                            dataPair.SetRecieved(_dataFormater.ToString(d, l));
                        });
                    else
                        _dataFlowController.Send(data, length);

                    _window.DataFlowPanel.Children.Add(dataPair);
                };
            }
            _window.Show();
        }

        private void DisplayEventuallyRecievedData(byte[] data, int length)
        {
            _window.DataFlowPanel.Children.Add(new DataFlowPair(null, _dataFormater.ToString(data, length)));
        }

        private IEnumerable<Module> LoadModules()
        {
            var modules = new List<Module>();

            var path = Path.Combine(Environment.CurrentDirectory, "Modules", "Modbus.dll");
            var moduleLoadContext = new ModuleLoadContext(path);
            var assembly = moduleLoadContext.LoadFromAssemblyName(new System.Reflection.AssemblyName(Path.GetFileNameWithoutExtension(path)));

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(Module).IsAssignableFrom(type))
                {
                    try
                    {
                        modules.Add(Activator.CreateInstance(type) as Module);
                    }
                    catch (Exception) { }
                }
            }

            return modules;
        }
    }
}
