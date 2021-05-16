using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Shared {

    public class Utilities {
        public static string GetSHA256(string str) {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0 ; i < stream.Length ; i++)
                sb.AppendFormat("{0:x2}" , stream[i]);
            return sb.ToString();
        }

        public static int GetRandomPin(int pinLength) {
            var random = new Random();
            var stringBuilder = new StringBuilder();
            for (int i = 0 ; i < pinLength ; i++) {

                int randomNumber = random.Next(10);
                //TODO: Make the code property of EmailVerification a string instead of an int
                while (i == 0 && randomNumber == 0) {
                    randomNumber = random.Next(10);
                }
                stringBuilder.Append(randomNumber);
            }
            return Convert.ToInt32(stringBuilder.ToString());
        }
    }
}
