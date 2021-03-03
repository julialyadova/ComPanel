using ComPanel.Connection;
using System;

namespace ComPanel.Configuration
{
    [Serializable]
    public class ComPanelConfiguration
    {
        public PortSettings PortSettings = new PortSettings();
        public string StartModule;
    }
}
