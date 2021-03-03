using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using ComPanel.Modules;
using ComPanel.Connection;
using System.IO.Ports;
using ComPanel.Resources.Controls;
using ComPanel.DataFlow;
using ComPanel.RawData;
using ComPanel.Configuration;

namespace ComPanel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _window;
        private DataFlowController _dataFlowController;
        private DataFormater _dataFormater;
        private ComPanelConfiguration _configuration;

        public App() 
            : base()
        {
            _window = new MainWindow();
            LoadConfigurations();            
            _dataFormater = new HexDataFormater();

            var connection = new SerialPortConnection();

            _dataFlowController = new DataFlowController(connection.Send);
            _dataFlowController.OnUnexpectedDataRecieved = (data, length) =>
                _window.DataFlowPanel.Children.Add(new DataFlowPair(null, _dataFormater.ToString(data, length)));

            connection.OnDataRecieved += _dataFlowController.RecieveData;

            _window.ConnectClick += () => connection.Connect(_configuration.PortSettings);
            _window.DisconnectClick += connection.Disonnect;

            LoadModules();
            
            _window.Show();
        }

        private void LoadModules()
        {
            var modules = new ComPanelModulesLoader().LoadModulesFromCatalog("ModulesLibs");

            foreach (var fileModulePair in modules)
            {
                var module = fileModulePair.Value;
                _window.ModulesMenu.AddModuleButton(module.GetName(), () =>
                {
                    _window.ContentStackPanel.Children.Clear();
                    _window.ContentStackPanel.Children.Add(module.GetPanel());
                });

                module.Send = (data, length, onRecieved) =>
                {
                    var dataPair = new DataFlowPair();
                    dataPair.SetSent(_dataFormater.ToString(data, length));

                    if (onRecieved != null)
                        onRecieved += (d, l) => dataPair.SetRecieved(_dataFormater.ToString(d, l));

                    _window.DataFlowPanel.Children.Add(dataPair);
                    _dataFlowController.Send(data, length);
                };
            }
        }

        private void LoadConfigurations()
        {
            var configPath = Path.Combine(Environment.CurrentDirectory, "Configs", "config_1.json");
            var staticConfigPath = Path.Combine(Environment.CurrentDirectory, "config.json");

            if (!Directory.Exists("Configs"))
                Directory.CreateDirectory("Configs");
            
            _configuration = JSONFileSerializer.Load<ComPanelConfiguration>(configPath);
            if (_configuration == null)
                _configuration = new ComPanelConfiguration();

            var staticConfig = JSONFileSerializer.Load<ComPanelStaticConfiguration>(staticConfigPath);
            if (staticConfig == null)
            {
                staticConfig = new ComPanelStaticConfiguration();
                JSONFileSerializer.Save(staticConfigPath, staticConfig);
            }                

            _window.ConnectButton.Click += (sender, args) => JSONFileSerializer.Save(configPath, _configuration);

            _window.PortComboBox.ItemsSource = PortSettings.GetPortNames();
            _window.BaudRateComboBox.ItemsSource = staticConfig.PortSettingsValues.BaudRateValues;
            _window.ParityComboBox.ItemsSource = Enum.GetValues(typeof(Parity));
            _window.StopBitsComboBox.ItemsSource = new StopBits[] {StopBits.One, StopBits.Two, StopBits.OnePointFive};
            _window.TimeOutMSComboBox.ItemsSource = staticConfig.PortSettingsValues.TimeoutValues;
            _window.DataBitsComboBox.ItemsSource = staticConfig.PortSettingsValues.DataBitsValues;

            _window.PortComboBox.SelectedItem = _configuration.PortSettings.PortName;
            _window.BaudRateComboBox.SelectedItem = _configuration.PortSettings.BaudRate;
            _window.ParityComboBox.SelectedItem = _configuration.PortSettings.Parity;
            _window.StopBitsComboBox.SelectedItem = _configuration.PortSettings.StopBits;
            _window.TimeOutMSComboBox.SelectedItem = _configuration.PortSettings.TimeOutMS;
            _window.DataBitsComboBox.SelectedItem = _configuration.PortSettings.DataBits;

            _window.PortComboBox.SelectionChanged += (sender, args) =>
            {
                var selected = _window.PortComboBox.SelectedItem;
                if (selected != null)
                    _configuration.PortSettings.PortName = (string)selected;
            };

            _window.BaudRateComboBox.SelectionChanged += (sender, args) =>
            {
                var selected = _window.BaudRateComboBox.SelectedItem;
                if (selected != null)
                    _configuration.PortSettings.BaudRate = (int)selected;
            };

            _window.ParityComboBox.SelectionChanged += (sender, args) =>
            {
                var selected = _window.ParityComboBox.SelectedItem;
                if (selected != null)
                    _configuration.PortSettings.Parity = (Parity)selected;
            };

            _window.StopBitsComboBox.SelectionChanged += (sender, args) =>
            {
                var selected = _window.StopBitsComboBox.SelectedItem;
                if (selected != null)
                    _configuration.PortSettings.StopBits = (StopBits)selected;
            };

            _window.TimeOutMSComboBox.SelectionChanged += (sender, args) =>
            {
                var selected = _window.TimeOutMSComboBox.SelectedItem;
                if (selected != null)
                    _configuration.PortSettings.TimeOutMS = (int)selected;
            };

            _window.DataBitsComboBox.SelectionChanged += (sender, args) =>
            {
                var selected = _window.DataBitsComboBox.SelectedItem;
                if (selected != null)
                    _configuration.PortSettings.DataBits = (int)selected;
            };            
        }
    }
}
