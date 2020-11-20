using System.Windows.Controls;

namespace ComPanel.Resources.Controls
{
    /// <summary>
    /// Interaction logic for DataFlowPair.xaml
    /// </summary>
    public partial class DataFlowPair : UserControl
    {
        public DataFlowPair(string sent, string recieved)
        {
            InitializeComponent();
            SentTextBlock.Text = sent;
            RecievedTextBlock.Text = recieved;
        }

        public DataFlowPair()
        {
            InitializeComponent();
        }

        public void SetSent(string sent)
        {
            SentTextBlock.Text = sent;
        }

        public void SetRecieved(string recieved)
        {
            RecievedTextBlock.Text = recieved;
        }
    }
}
