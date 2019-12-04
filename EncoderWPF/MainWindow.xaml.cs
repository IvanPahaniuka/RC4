using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using Encoders;
using System.Threading;

namespace EncoderWPF
{
    public partial class MainWindow : Window
    {
        public const int BUFFER_SIZE = 67_108_863;

        /// <summary>
        /// Окно шифратора
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Открывает диалоговое окно получения пути к файлу 
        /// </summary>
        /// <returns>Путь к файлу или null</returns>
        public string GetPath()
        {
            OpenFileDialog OF = new OpenFileDialog();

            if (OF.ShowDialog() == true)
                return OF.FileName;

            return null;
        }

        /// <summary>
        /// Заменяет значение пути в textBox на новое
        /// </summary>
        /// <param name="textBox">TextBox, в котором необходимо произвести замену пути</param>
        public void UpdatePath(TextBox textBox)
        {
            string path = GetPath();
            if (path != null)
                textBox.Text = path;
        }

        /// <summary>
        /// Открывает окно генератора ключей
        /// </summary>
        public void CreateKey()
        {
            KeyWindow KW = new KeyWindow();
            KW.Show();
        }

        /// <summary>
        /// Шифрует файл по пути inputPath в outputPath, используя ключи в keysPath
        /// </summary>
        /// <param name="inputPath">Входной файл для шифровки</param>
        /// <param name="outputPath">Выходной файл для дешифровки</param>
        /// <param name="keysPath">Файл с ключами</param>
        public void Encrypt(string inputPath, string outputPath, string keysPath)
        {
            if (!File.Exists(inputPath) || !File.Exists(keysPath))
            {
                MessageBox.Show("Файл не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FileStream inputStream = new FileStream(inputPath, FileMode.Open);
            FileStream outputStream = new FileStream(outputPath, FileMode.Create);
            FileStream keysStream = new FileStream(keysPath, FileMode.Open);

            string title = "";
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(() =>
                {
                    title = Title;
                    Title = $"{title} (0%)";
                    encryptButton.IsEnabled = false;
                    decryptButton.IsEnabled = false;
                }));

            byte[] keys = new byte[256];
            int len = keysStream.Read(keys, 0, 256);
            Array.Resize(ref keys, len);

            Encoders.Encoder encoder = new XOREncoder(new RC4KeyBuilder(keys));

            int count = 1;
            byte[] buffer = new byte[BUFFER_SIZE];
            long inLength = inputStream.Length;
            long i = 0;

            while (count > 0)
            {
                count = inputStream.Read(buffer, 0, BUFFER_SIZE);
                encoder.Encrypt(buffer, ref buffer);
                outputStream.Write(buffer, 0, count);

                i += count;
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, 
                    new Action(() => 
                        Title = $"{title} ({string.Format("{0:f1}", i *100f/inLength)}%)"
                    ));
            }

            inputStream.Close();
            outputStream.Close();

            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(() =>
                {
                    Title = title;
                    encryptButton.IsEnabled = true;
                    decryptButton.IsEnabled = true;
                }));
        }


        private void OutputSelect_Click(object sender, RoutedEventArgs e)
        {
            UpdatePath(outputTextBox);
        }

        private void KeySelect_Click(object sender, RoutedEventArgs e)
        {
            UpdatePath(keyTextBox);
        }

        private void SourceSelect_Click(object sender, RoutedEventArgs e)
        {
            UpdatePath(sourceTextBox);
        }

        private void CreateKey_Click(object sender, RoutedEventArgs e)
        {
            CreateKey();
        }

        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(() => {
                string inputPath = "", outputPath = "", keysPath = "";
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(() => {
                        inputPath = sourceTextBox.Text;
                        outputPath = outputTextBox.Text;
                        keysPath = keyTextBox.Text;
                    }));

                Encrypt(inputPath, outputPath, keysPath);
            });
            thread.Start();
        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                string inputPath = "", outputPath = "", keysPath = "";
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(() => {
                        inputPath = sourceTextBox.Text;
                        outputPath = outputTextBox.Text;
                        keysPath = keyTextBox.Text;
                    }));

                Encrypt(outputPath, inputPath, keysPath);
            });
            thread.Start();
        }
    }
}
