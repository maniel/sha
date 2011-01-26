using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace sha {
    class Program {
        static uint[] k = { 0x5A827999, 0x6ED9EBA1, 0x8F1BBCDC, 0xCA62C1D6 };
        static void Main(string[] args) {
            Console.WriteLine("Podaj nazwe pliku");
            String fn = Console.ReadLine();
            byte[] buf = File.ReadAllBytes(fn);
            uint[] h = hash(buf);
            Console.WriteLine("{0:X8}{1:X8}{2:X8}{3:X8}{4:X8}", h[0], h[1], h[2], h[3], h[4]);
            Console.ReadLine();
        }

        public static uint[] hash(byte[] buf){
            uint[] H = { 0x67452301, 0xEFCDAB89, 0x98BADCFE , 0x10325476, 0xC3D2E1F0 };
            int origsize = buf.Length;
            Array.Resize(ref buf, origsize + 1);
            buf[origsize] |= 0x80;
            int pad = 64;
            int len = ((buf.Length + pad - 1) / pad) * pad;
            Array.Resize(ref buf, len);
            long last8bytes = BitConverter.ToInt64(buf, buf.Length - 8);
            if (last8bytes != 0)
                Array.Resize(ref buf, buf.Length + 64);
            byte[] sizebytes = BitConverter.GetBytes(origsize * 8);
            Array.Reverse(sizebytes);
            Array.Copy(sizebytes, 0, buf, buf.Length - sizebytes.Length, sizebytes.Length);
            int n = buf.Length / 64;
            uint[] w = new uint[80];
            uint[][] M = new uint[n][];
            for (int i = 0; i < M.Length; i++) {
                M[i] = new uint[16];
                for (int j = 0; j < 16; j++) {
                    int index = i * 64 + j * 4;
                    byte[] b = new byte[4];
                    Array.Copy(buf, index, b, 0, 4);
                    Array.Reverse(b);
                    uint uinteger = BitConverter.ToUInt32(b, 0);
                    M[i][j] = uinteger;
                }
            }



            for (int i = 0; i < M.Length; i++) {
                uint a = H[0];
                uint b = H[1];
                uint c = H[2];
                uint d = H[3];
                uint e = H[4];

                for (int j = 0; j < 16; j++)
                    w[j] = M[i][j];
                for (int j = 16; j < 80; j++)
                    w[j] = rot((w[j - 3] ^ w[j - 8] ^ w[j - 14] ^ w[j - 16]), 1);
                for (int j = 0; j < 80; j++) {
                    int s = Convert.ToInt32(Math.Floor(j / 20.0));
                    uint t = (rot(a, 5) + e + w[j] + f(j, b, c, d) + k[s]);
                    e = d;
                    d = c;
                    c = rot(b, 30);
                    b = a;
                    a = t;              
                }
                
                H[0] = (H[0] + a);
                H[1] = (H[1] + b);
                H[2] = (H[2] + c);
                H[3] = (H[3] + d);
                H[4] = (H[4] + e);
            }
            return H; 
        }


        public static uint f(int t, uint B, uint C, uint D) {
            if (t >= 0 && t <= 19)
                return (B & C) | ((~B) & D);
            else if (t >= 20 && t <= 39)
                return B ^ C ^ D;
            else if (t >= 40 && t <= 59)
                return (B & D) | (B & C) | (C & D);
            else if (t >= 60 && t <= 79)
                return B ^ C ^ D;
            return 0;

        }

        public static uint rot(uint num, int cnt) {
            return (num << cnt) | (num >> (32 - cnt));
        }
    }
}
