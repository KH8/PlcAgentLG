using PlcAgentLicenceGenerator.Signature;

namespace PlcAgentLicenceGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            SignatureLabel.Content = "PC Signature: \n" + SignatureHandler.GetSignature();
        }
    }
}
