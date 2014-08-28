using System;
using System.Globalization;
using System.IO;
using System.Windows;
using PlcAgentLicenseGenerator.Signature;

namespace PlcAgentLicenseGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Variables

        private readonly string _signature;
        private string _encryptedSignature;

        private BlowFish.BlowFish _blowFishEncryptor;

        #endregion


        #region Construtors

        public MainWindow()
        {
            InitializeComponent();

            _signature = SignatureHandler.GetSignature();
            SignatureLabel.Text = "PC Signature: \n" + _signature;
        }

        #endregion


        #region EventHandlers

        private void GenerateLicense(object sender, RoutedEventArgs e)
        {
            StatusLabel.Content = "...";

            var nameToBeStored = NameTextBox.Text;
            var dateToBeStored = DateTime.Now.ToString(CultureInfo.InvariantCulture);

            // Create the new, empty data file.
            const string fileName = @"license.lic";
            if (File.Exists(fileName)) File.Delete(fileName);

            var fs = new FileStream(fileName, FileMode.CreateNew);

            // Create the writer for data.
            var w = new BinaryWriter(fs);

            // Write data.
            _blowFishEncryptor = new BlowFish.BlowFish(HexKey.SignatureValue);
            _encryptedSignature = _blowFishEncryptor.Encrypt_CTR(_signature);
            w.Write(_encryptedSignature);

            w.Write(nameToBeStored);
            w.Write(dateToBeStored);

            _encryptedSignature = _blowFishEncryptor.Encrypt_CTR(nameToBeStored + dateToBeStored);
            _blowFishEncryptor = new BlowFish.BlowFish(HexKey.PersonalValue);
            _encryptedSignature = _blowFishEncryptor.Encrypt_CTR(_encryptedSignature);
            w.Write(_encryptedSignature);

            w.Close();
            fs.Close();

            // Create the reader for data.
            fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var r = new BinaryReader(fs);
            // Read data from Test.data.

            var storedSignature = r.ReadString();
            var storedName = r.ReadString();
            var storedDate = r.ReadString();
            var storedPersonalSignature = r.ReadString();

            r.Close();
            fs.Close();

            storedPersonalSignature = _blowFishEncryptor.Decrypt_CTR(storedPersonalSignature);

            _blowFishEncryptor = new BlowFish.BlowFish(HexKey.SignatureValue);

            if (!Equals(storedName + storedDate, _blowFishEncryptor.Decrypt_CTR(storedPersonalSignature)))
            {
                File.Delete(fileName);
                StatusLabel.Content = "license creation failed";
                return;
            }

            if (Equals(_signature, _blowFishEncryptor.Decrypt_CTR(storedSignature)))
                StatusLabel.Content = "license file has been created for: " + storedName;
            else
            {
                File.Delete(fileName);
                StatusLabel.Content = "license creation failed";
            }
        }

        #endregion

    }
}
