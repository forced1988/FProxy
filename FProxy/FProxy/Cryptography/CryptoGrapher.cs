using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FProxy.Cryptography
{
    public class CryptoGrapher
    {
        class CryptCounter
        {
            UInt16 m_Counter = 0;

            public byte Key2
            {
                get { return (byte)(m_Counter >> 8); }
            }

            public byte Key1
            {
                get { return (byte)(m_Counter & 0xFF); }
            }

            public void Increment()
            {
                m_Counter++;
            }
        }

        private CryptCounter _decryptCounter;
        private CryptCounter _encryptCounter;
        private byte[] _cryptKey1;
        private byte[] _cryptKey2;

        public CryptoGrapher()
        {
            _decryptCounter = new CryptCounter();
            _encryptCounter = new CryptCounter();
            _cryptKey1 = new byte[0x100];
            _cryptKey2 = new byte[0x100];
            byte i_key1 = 0x9D;
            byte i_key2 = 0x62;
            for (int i = 0; i < 0x100; i++)
            {
                _cryptKey1[i] = i_key1;
                _cryptKey2[i] = i_key2;
                i_key1 = (byte)((0x0F + (byte)(i_key1 * 0xFA)) * i_key1 + 0x13);
                i_key2 = (byte)((0x79 - (byte)(i_key2 * 0x5C)) * i_key2 + 0x6D);
            }
        }

        public void Decrypt(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= (byte)0xAB;
                buffer[i] = (byte)(buffer[i] >> 4 | buffer[i] << 4);
                buffer[i] ^= (byte)(_cryptKey1[_decryptCounter.Key1] ^ _cryptKey2[_decryptCounter.Key2]);
                _decryptCounter.Increment();
            }
        }
        public void Encrypt(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= (byte)0xAB;
                buffer[i] = (byte)(buffer[i] >> 4 | buffer[i] << 4);
                buffer[i] ^= (byte)(_cryptKey1[_encryptCounter.Key1] ^ _cryptKey2[_encryptCounter.Key2]);
                _encryptCounter.Increment();
            }
        }

        public void EncryptBackwards(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= (byte)(_cryptKey2[_encryptCounter.Key2] ^ _cryptKey1[_encryptCounter.Key1]);
                buffer[i] = (byte)(buffer[i] >> 4 | buffer[i] << 4);
                buffer[i] ^= (byte)0xAB;

                _encryptCounter.Increment();
            }
        }
        public void DecryptBackwards(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= (byte)(_cryptKey2[_decryptCounter.Key2] ^ _cryptKey1[_decryptCounter.Key1]);
                buffer[i] = (byte)(buffer[i] >> 4 | buffer[i] << 4);
                buffer[i] ^= (byte)0xAB;

                _decryptCounter.Increment();
            }
        }

        public void GenerateKeys(UInt32 CryptoKey, UInt32 AccountID)
        {
            UInt32 tmpkey1 = 0, tmpkey2 = 0;
            tmpkey1 = ((CryptoKey + AccountID) ^ (0x4321)) ^ CryptoKey;
            tmpkey2 = tmpkey1 * tmpkey1;

            for (int i = 0; i < 256; i++)
            {
                int right = ((3 - (i % 4)) * 8);
                int left = ((i % 4)) * 8 + right;
                _cryptKey1[i] ^= (byte)(tmpkey1 << right >> left);
                _cryptKey2[i] ^= (byte)(tmpkey2 << right >> left);
            }
        }

    }
}
