using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;

namespace EncoderWPF
{
    public partial class KeyWindow : Window
    {
        /// <summary>
        /// Ключи
        /// </summary>
        public IEnumerable<byte> Keys { get; private set; }

        /// <summary>
        /// Окно генерации ключевой последовательности
        /// </summary>
        public KeyWindow()
        {
            Keys = new byte[0];
            InitializeComponent();
        }

        /// <summary>
        /// Добавляет элемент в список
        /// </summary>
        public void AddItem()
        {
            if (Keys.Count() >= 256)
            {
                MessageBox.Show("Список заполнен", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int selected = listBox.SelectedIndex;
            int id = selected >= 0 ? selected + 1 : 0;

            try
            {
                byte key = byte.Parse(keyTextBox.Text != "" ? keyTextBox.Text : "0");
                Keys = Keys.Take(id).Append(key).Concat(Keys.Skip(id));
                listBox.ItemsSource = Keys;
                listBox.SelectedIndex = selected >= 0 ? selected+1 : -1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            } 
        }

        /// <summary>
        /// Удаляет элемент из списока
        /// </summary>
        public void RemoveItem()
        {
            if (listBox.SelectedIndex < 0 && 
                listBox.SelectedIndex >= Keys.Count())
                return;

            Keys = Keys.Take(listBox.SelectedIndex).Concat(
                Keys.Skip(listBox.SelectedIndex+1)
                );
            listBox.ItemsSource = Keys;
        }

        /// <summary>
        /// Сбрасывает выделение элемента в списке
        /// </summary>
        public void ResetSelection()
        {
            listBox.SelectedIndex = -1;
        }

        /// <summary>
        /// Автоматически заполняет недостающие ключи в списке
        /// </summary>
        public void Auto()
        {
            Random random = new Random();

            int n = 256 - Keys.Count();
            for (int i = 0; i < n; i++)
                Keys = Keys.Append((byte)random.Next(256));

            listBox.ItemsSource = Keys;
        }

        /// <summary>
        /// Сохранение списка ключей
        /// </summary>
        public void Save()
        {
            SaveFileDialog SF = new SaveFileDialog();

            if (SF.ShowDialog() == true)
                File.WriteAllBytes(SF.FileName, Keys.ToArray());
        }

        /// <summary>
        /// Очистка списка ключей
        /// </summary>
        public void Clear()
        {
            Keys = new byte[0];
            listBox.ItemsSource = Keys;
        }






        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddItem();
        }

        private void ListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || (e.Key == Key.Up && listBox.SelectedIndex == 0))
                ResetSelection();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            RemoveItem();
        }

        private void Auto_Click(object sender, RoutedEventArgs e)
        {
            Auto();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }
    }
}
