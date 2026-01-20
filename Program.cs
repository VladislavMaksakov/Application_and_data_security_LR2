using System.Text;

namespace CipherComparison
{
   // === КЛАС ДЛЯ ШИФРУ ЦЕЗАРЯ ===
   // Реалізує простий моноалфавітний шифр зі зсувом
   public static class CaesarCipher
   {
      // Метод Process працює як для шифрування, так і для дешифрування.
      // shift > 0 -> шифрування, shift < 0 -> дешифрування.
      public static string Process(string text, int shift)
      {
         char[] buffer = text.ToCharArray(); // Перетворюємо рядок у масив символів для посимвольної обробки

         for (int i = 0; i < buffer.Length; i++)
         {
            char letter = buffer[i];

            // Шифруємо ТІЛЬКИ літери. Пробіли, цифри та знаки пунктуації пропускаємо.
            if (char.IsLetter(letter))
            {
               // Визначаємо базовий символ ('A' для великих, 'a' для малих),
               // щоб працювати з діапазоном 0..25
               char offset = char.IsUpper(letter) ? 'A' : 'a';

               // === ФОРМУЛА ШИФРУВАННЯ ===
               // 1. (letter - offset): переводимо символ у число від 0 до 25 (A=0, B=1...)
               // 2. + shift % 26: додаємо зсув (і страхуємося від дуже великих ключів)
               // 3. + 26: додаємо 26 перед фінальним %, щоб уникнути від'ємних чисел при розшифруванні
               // 4. % 26: замикаємо алфавіт у кільце (після Z йде A)
               buffer[i] = (char)(offset + (letter - offset + shift % 26 + 26) % 26);
            }
         }
         return new string(buffer); // Збираємо масив назад у рядок
      }

      // === ФУНКЦІЯ ЗЛОМУ (BRUTE FORCE) ===
      // Демонструє слабкість шифру Цезаря перебором усіх 25 варіантів
      public static void PrintBruteForce(string encryptedText)
      {
         Console.WriteLine("\n[CRACKING] Спроба злому Цезаря (Brute Force):");

         // Беремо тільки перші 50 символів, щоб не засмічувати консоль довгим текстом
         string preview = encryptedText.Length > 50 ? encryptedText.Substring(0, 50) : encryptedText;

         // Цикл перебирає всі можливі ключі від 1 до 25
         for (int shift = 1; shift < 26; shift++)
         {
            // Викликаємо Process з від'ємним зсувом (-shift), щоб спробувати розшифрувати
            Console.WriteLine($"   Зсув -{shift}: {Process(preview, -shift)}");
         }
      }
   }

   // === КЛАС ДЛЯ ШИФРУ ВІЖЕНЕРА ===
   // Реалізує поліалфавітний шифр (стійкіший, бо зсув постійно змінюється)
   public static class VigenereCipher
   {
      public static string Encrypt(string text, string key, bool decrypt = false)
      {
         StringBuilder result = new StringBuilder(); // Використовуємо StringBuilder для ефективної роботи з рядками
         int keyIndex = 0; // Індекс для відстеження поточної літери КЛЮЧА
         key = key.ToUpper(); // Приводимо ключ до верхнього регістру для зручності

         foreach (char c in text)
         {
            // Як і в Цезаря, обробляємо тільки літери
            if (char.IsLetter(c))
            {
               char offset = char.IsUpper(c) ? 'A' : 'a'; // Визначаємо регістр літери тексту

               int letterIdx = c - offset; // Позиція літери тексту в алфавіті (0-25)

               // === ВИЗНАЧЕННЯ ЗСУВУ ===
               // Беремо літеру ключа. Використовуємо %, щоб "зациклити" ключ.
               // Наприклад, якщо ключ "KEY", а текст довгий: K, E, Y, K, E, Y...
               int keyShift = key[keyIndex % key.Length] - 'A';

               // Якщо потрібно розшифрувати, просто віднімаємо зсув замість додавання
               if (decrypt) keyShift = -keyShift;

               // Застосовуємо зсув (аналогічна формула як у Цезаря)
               char newChar = (char)(offset + (letterIdx + keyShift + 26) % 26);
               result.Append(newChar);

               // Зсуваємо індекс ключа тільки коли зашифрували літеру.
               // На пробілах ключ не зсувається!
               keyIndex++;
            }
            else
            {
               // Якщо не літера (пробіл, цифра) — додаємо без змін
               result.Append(c);
            }
         }
         return result.ToString();
      }
   }

   class Program
   {
      static void Main(string[] args)
      {
         Console.OutputEncoding = Encoding.UTF8; // Вмикаємо підтримку кирилиці в консолі

         // === ГЕНЕРАЦІЯ ПЕРСОНАЛЬНИХ КЛЮЧІВ ===
         string lastName = "Maksakov";
         string dob = "05.03.2004";

         // LINQ-запит: беремо рядок "05.03.2004", фільтруємо тільки цифри, 
         // перетворюємо char '5' в int 5 і сумуємо їх.
         // '0'+'5'+'0'+'3'+'2'+'0'+'0'+'4' = 14
         int caesarKey = dob.Where(char.IsDigit).Sum(c => c - '0');

         string vigenereKey = lastName; // Ключ для Віженера - прізвище

         // Нескінченний цикл роботи програми
         while (true)
         {
            Console.Clear();
            // "Шапка" інтерфейсу
            Console.WriteLine("=== ЛАБОРАТОРНА: ПОРІВНЯННЯ ШИФРІВ ===");
            Console.WriteLine($"Студент: Максаков В.О. | Ключі: Цезар={caesarKey}, Віженер=\"{vigenereKey}\"");
            Console.WriteLine(new string('=', 60));

            Console.Write("\nВведіть текст для шифрування (або 'exit' для виходу): ");
            string input = Console.ReadLine();

            // Перевірка на вихід з програми
            if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit")
               break;

            // === ВИКОНАННЯ ШИФРУВАННЯ ===
            string cEnc = CaesarCipher.Process(input, caesarKey);
            string vEnc = VigenereCipher.Encrypt(input, vigenereKey);

            // === ВИВЕДЕННЯ РЕЗУЛЬТАТІВ У ТАБЛИЦЮ ===
            Console.WriteLine("\n >> РЕЗУЛЬТАТИ:");
            // Форматування рядків: {-10} означає вирівнювання по лівому краю на 10 символів
            Console.WriteLine($"{"Алгоритм",-10} | {"Результат (перші 40 симв.)",-45}");
            Console.WriteLine(new string('-', 60));

            // Обрізаємо текст, якщо він довший за 40 символів, для красивого вигляду
            Console.WriteLine($"{"Цезар",-10} | {(cEnc.Length > 40 ? cEnc.Substring(0, 40) + "..." : cEnc)}");
            Console.WriteLine($"{"Віженер",-10} | {(vEnc.Length > 40 ? vEnc.Substring(0, 40) + "..." : vEnc)}");

            // Демонстрація злому Цезаря
            CaesarCipher.PrintBruteForce(cEnc);

            Console.WriteLine("\nНатисніть Enter, щоб продовжити...");
            Console.ReadLine();
         }
      }
   }
}