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

        private string _signature;
        private string _encryptedSignature;

        private BlowFish.BlowFish _blowFishEncryptor;

        #endregion


        #region Construtors

        public MainWindow()
        {
            InitializeComponent();

            _signature = SignatureHandler.GetSignature();
            SignatureLabel.Text = "PC Signature: \n" + _signature;

            _blowFishEncryptor = new BlowFish.BlowFish(HexKey.Value);
        }

        #endregion


        #region EventHandlers

        private void GenerateLicense(object sender, RoutedEventArgs e)
        {
            StatusLabel.Content = "...";

            var nameToBeStored = NameTextBox.Text;
            var dateToBeStored = DateTime.Now.ToString(CultureInfo.InvariantCulture);

            _encryptedSignature = _blowFishEncryptor.Encrypt_CTR(_signature + nameToBeStored + dateToBeStored);

            // Create the new, empty data file.
            const string fileName = @"license.lic";
            if (File.Exists(fileName)) File.Delete(fileName);

            var fs = new FileStream(fileName, FileMode.CreateNew);

            // Create the writer for data.
            var w = new BinaryWriter(fs);
            // Write data to Test.data.
            w.Write(nameToBeStored);
            w.Write(_encryptedSignature);
            w.Write(dateToBeStored);

            w.Close();
            fs.Close();

            // Create the reader for data.
            fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var r = new BinaryReader(fs);
            // Read data from Test.data.

            var storedName = r.ReadString();
            var storedSignature = r.ReadString();
            var storedDate = r.ReadString();

            r.Close();
            fs.Close();

            if (Equals(_signature + storedName + storedDate, _blowFishEncryptor.Decrypt_CTR(storedSignature)))
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
