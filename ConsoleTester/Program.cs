using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Encoders;
using System.IO;

namespace ConsoleTester
{
    class Program
    {
        public static int BUFFER_SIZE = 67_108_863;

        static void Main(string[] args)
        {
            Console.Write("Введите количество базовых ключей (от 0 до 256): ");
            int n = byte.Parse(Console.ReadLine());
            byte[] baseKey = new byte[n];
            for (int i = 0; i < n; i++)
            {
                Console.Write($"Введите ключ №{i+1} (от 0 до 256): ");
                baseKey[i] = byte.Parse(Console.ReadLine());
            }

            RC4KeyBuilder keyBuilder = new RC4KeyBuilder(baseKey);
            XOREncoder encoder = new XOREncoder(keyBuilder);

            Console.Write("Введите путь к файлу: ");
            string path = Console.ReadLine();

            if (!File.Exists(path))
            {
                Console.WriteLine("Файл не существует...");
                Console.WriteLine("Нажмите на любую клавишу для продолжения...");
                Console.ReadKey();
                return;
            }

            Console.Write("Введите путь к выходному файлу: ");
            string pathOut = Console.ReadLine();

            Console.WriteLine("Шифруем...");
            Encrypt(path, pathOut, encoder);
            Console.WriteLine("Успешно!");

            Console.WriteLine("Нажмите на любую клавишу для продолжения...");
            Console.ReadKey();
        }

        public static void Encrypt(string pathInput, string pathOutput, Encoders.Encoder encoder)
        {
            FileStream OF = new FileStream(pathInput, FileMode.Open);
            FileStream SF = new FileStream(pathOutput, FileMode.Create);

            int count = 1;
            byte[] buffer = new byte[BUFFER_SIZE];

            while (count > 0)
            {
                count = OF.Read(buffer, 0, BUFFER_SIZE);
                encoder.Encrypt(buffer, ref buffer);
                SF.Write(buffer, 0, count);
            }

            OF.Close();
            SF.Close();
        }        
    }
}
