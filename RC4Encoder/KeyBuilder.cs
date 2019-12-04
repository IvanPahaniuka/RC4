using System;

namespace Encoders
{
    public abstract class KeyBuilder: ICloneable
    {
        public abstract byte GetNextKey();
        public abstract object Clone(); 
    }

    public class RC4KeyBuilder : KeyBuilder
    {
        private byte[] keyBase;
        private int x, y;

        private RC4KeyBuilder()
        {
            keyBase = new byte[256];
        }

        /// <summary>
        /// Генератор ключа типа RC4
        /// </summary>
        /// <param name="rndKey">Базовый случайный ключ</param>
        public RC4KeyBuilder(byte[] rndKey)
            : this()
        {
            InitializeKey(rndKey);
        }

        /// <summary>
        /// Генерирует массив из следующих count ключей
        /// </summary>
        /// <param name="count">Размер массива</param>
        /// <returns>Массив ключей</returns>
        public byte[] GetNextKeys(long count)
        {
            byte[] res = new byte[count];

            for (int i = 0; i < count; i++)
                res[i] = GetNextKey();

            return res;
        }

        /// <summary>
        /// Генерирует следующий ключ
        /// </summary>
        /// <returns>Следующий ключ</returns>
        public override byte GetNextKey()
        {
            x = (x + 1) & 255;
            y = (y + keyBase[x]) & 255;

            byte t = keyBase[x];
            keyBase[x] = keyBase[y];
            keyBase[y] = t;

            return keyBase[(keyBase[x] + keyBase[y]) & 255];
        }

        /// <summary>
        /// Глубокое клонирование объекта
        /// </summary>
        /// <returns>Копия объекта</returns>
        public override object Clone()
        {
            byte[] keyBaseCopy = new byte[256];
            keyBase.CopyTo(keyBaseCopy, 0);

            return 
                new RC4KeyBuilder() {
                    keyBase = keyBaseCopy,
                    x = this.x,
                    y = this.y };
        }


        private void InitializeKey(byte[] rndKey)
        {
            for (int i = 0; i < 256; i++)
            {
                keyBase[i] = (byte)i;
            }

            if (rndKey.Length == 0)
            {
                Array.Resize(ref rndKey, 1);
                rndKey[0] = 0;
            }

            int keyLength = rndKey.Length;
            int j = 0;
            x = 0;
            y = 0;
            byte t;
            for (int i = 0; i < 256; i++)
            {
                j = (j + keyBase[i] + rndKey[i % keyLength]) % 256;

                t = keyBase[i];
                keyBase[i] = keyBase[j];
                keyBase[j] = t;
            }
        }
    }
}
