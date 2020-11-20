using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ComPanel.Resources.Controls
{
    /// <summary>
    /// Interaction logic for ModulesMenu.xaml
    /// </summary>
    public partial class ModulesMenu : UserControl
    {
        private List<ModuleButton> buttons;

        public ModulesMenu()
        {
            buttons = new List<ModuleButton>();
            InitializeComponent();
        }

        public void AddModuleButton(string name, Action click = null)
        {
            var button = new ModuleButton(name, StackPanel.Children.Count == 0);
            buttons.Add(button);
            button.Click += (sender, args) => OnButtonClick(button);
            if (click != null)
                button.Click += (sender, args) => click();

            StackPanel.Children.Add(button);
        }

        public void OnButtonClick(ModuleButton clickedButton)
        {
            clickedButton.Highlight();
            foreach (var button in buttons)
            {
                if (button != clickedButton)
                    button.Darken();
            }
        }
    }
}
