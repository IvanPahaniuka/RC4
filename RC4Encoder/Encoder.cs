using System;

namespace Encoders
{
    public abstract class Encoder
    {
        public abstract void Encrypt(byte[] arr, ref byte[] arrRes);
        public abstract void Decrypt(byte[] arr, ref byte[] arrRes);
    }

    public class XOREncoder: Encoder
    {
        private KeyBuilder baseBuilder;

        private KeyBuilder keyBuilder;
        /// <summary>
        /// Генератор ключей
        /// </summary>
        public KeyBuilder KeyBuilder
        {
            get => baseBuilder;
            set
            {
                baseBuilder = value ?? throw new ArgumentNullException("Значение KeyBuilder не может быть null");
                keyBuilder = baseBuilder.Clone() as KeyBuilder;
            }
        }

        /// <summary>
        /// XOR шифратор 
        /// </summary>
        /// <param name="keyBuilder">Генератор ключа</param>
        /// <exception cref="ArgumentNullException"/>
        public XOREncoder(KeyBuilder keyBuilder)
        {
            KeyBuilder = keyBuilder;
        }

        /// <summary>
        /// Шифрует массив байтов
        /// </summary>
        /// <param name="arr">Массив байтов для шифровки</param>
        /// <param name="arrRes">Массив, в который будет сохранен результат шифровки</param>
        public override void Encrypt(byte[] arr, ref byte[] arrRes)
        {
            Decrypt(arr, ref arrRes);
        }

        /// <summary>
        /// Расшифровывает массив байтов
        /// </summary>
        /// <param name="arr">Зашифрованный массив байтов</param>
        /// /// <param name="arrRes">Массив, в который будет сохранен результат дешифровки</param>
        public override void Decrypt(byte[] arr, ref byte[] arrRes)
        {
            if (arr == null)
                return;

            int len = arr.Length;
            if (arrRes == null)
                arrRes = new byte[len];

            if (arrRes.Length < len)
                Array.Resize(ref arrRes, len);

            for (int i = 0; i < len; i++)
                arrRes[i] = (byte)(arr[i] ^ KeyBuilder.GetNextKey());

            KeyBuilder = baseBuilder.Clone() as KeyBuilder;
        }
    }
}
