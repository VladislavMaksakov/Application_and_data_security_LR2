using System.Text;

namespace CipherComparison
{
   public static class CaesarCipher
   {
      public static string Process(string text, int shift)
      {
         char[] buffer = text.ToCharArray();
         for (int i = 0; i < buffer.Length; i++)
         {
            char letter = buffer[i];
            if (char.IsLetter(letter))
            {
               char offset = char.IsUpper(letter) ? 'A' : 'a';
               buffer[i] = (char)(offset + (letter - offset + shift % 26 + 26) % 26);
            }
         }
         return new string(buffer);
      }
      public static void PrintBruteForce(string encryptedText)
      {
         Console.WriteLine("\n[CRACKING] Спроба злому Цезаря (Brute Force):");
         string preview = encryptedText.Length > 50 ? encryptedText.Substring(0, 50) : encryptedText;
         for (int shift = 1; shift < 26; shift++)
         {
            Console.WriteLine($"   Зсув -{shift}: {Process(preview, -shift)}");
         }
      }
   }
   public static class VigenereCipher
   {
      public static string Encrypt(string text, string key, bool decrypt = false)
      {
         StringBuilder result = new StringBuilder();
         int keyIndex = 0;
         key = key.ToUpper();

         foreach (char c in text)
         {
            if (char.IsLetter(c))
            {
               char offset = char.IsUpper(c) ? 'A' : 'a';
               int letterIdx = c - offset;
               int keyShift = key[keyIndex % key.Length] - 'A';
               if (decrypt) keyShift = -keyShift;

               char newChar = (char)(offset + (letterIdx + keyShift + 26) % 26);
               result.Append(newChar);
               keyIndex++;
            }
            else result.Append(c);
         }
         return result.ToString();
      }
   }

   class Program
   {
      static void Main(string[] args)
      {
         Console.OutputEncoding = Encoding.UTF8;

         // Персональні дані
         string lastName = "Maksakov";
         string dob = "05.03.2004";
         int caesarKey = dob.Where(char.IsDigit).Sum(c => c - '0'); // Сума цифр дати народження (14)
         string vigenereKey = lastName;

         while (true)
         {
            Console.Clear();
            Console.WriteLine("=== ЛАБОРАТОРНА: ПОРІВНЯННЯ ШИФРІВ ===");
            Console.WriteLine($"Студент: Максаков В.О. | Ключі: Цезар={caesarKey}, Віженер=\"{vigenereKey}\"");
            Console.WriteLine(new string('=', 60));
            Console.Write("\nВведіть текст для шифрування (або 'exit' для виходу): ");

            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit")
               break;
            string cEnc = CaesarCipher.Process(input, caesarKey);
            string vEnc = VigenereCipher.Encrypt(input, vigenereKey);

            Console.WriteLine("\n >> РЕЗУЛЬТАТИ:");
            Console.WriteLine($"{"Алгоритм",-10} | {"Результат (перші 40 симв.)",-45}");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"{"Цезар",-10} | {(cEnc.Length > 40 ? cEnc.Substring(0, 40) + "..." : cEnc)}");
            Console.WriteLine($"{"Віженер",-10} | {(vEnc.Length > 40 ? vEnc.Substring(0, 40) + "..." : vEnc)}");
            CaesarCipher.PrintBruteForce(cEnc);
            Console.WriteLine("\nНатисніть Enter, щоб продовжити...");
            Console.ReadLine();
         }
      }
   }
}