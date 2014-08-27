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
            SignatureLabel.Content = "PC Signature: \n" + signature;

            var blowFishEncryptor = new BlowFish.BlowFish(HexKey.Value);

            var encryptedSignature = blowFishEncryptor.Encrypt_CTR(signature);
            SignatureEncryptedLabel.Content = "PC Signature Encrypted: \n" + encryptedSignature;

            var decryptedSignature = blowFishEncryptor.Decrypt_CTR(encryptedSignature);
            SignatureDecryptedLabel.Content = "PC Signature Decrypted: \n" + decryptedSignature;
        }
    }
}
