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

            var signature = SignatureHandler.GetSignature();
            SignatureLabel.Text = "PC Signature: \n" + signature;

            var blowFishEncryptor = new BlowFish.BlowFish(HexKey.Value);

            var encryptedSignature = blowFishEncryptor.Encrypt_CTR(signature);
            var decryptedSignature = blowFishEncryptor.Decrypt_CTR(encryptedSignature);
        }

        private void GenerateLicence(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
