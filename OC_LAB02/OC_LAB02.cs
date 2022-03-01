using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Diagnostics;

public class OC_LAB02
{
    static string path = @"C:\Documents";
    public delegate bool BruteForceTest(ref char[] testChars);
    public static void Main(string[] args)
    {

    mark:
        Console.WriteLine("1-Работа с файлами, 2-хэширование, 3-ломать, 4 многопоточность, 0-выход: ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                CrateFile();
                goto mark;
            case "2":
                SHA_256();
                goto mark;
            case "3":
                BrutMain();
                goto mark;
            case "4":
                Theard();
                Thread.Sleep(10000);
                goto mark;
            case "5":
                goto mark;
            default:
                break;

        }
    }
    static void CrateFile()
    {
    mark:
        Console.WriteLine("1-запись, 2-чтение, 3-удаление, 0-выход: ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                // запись в файл
                Write();
                goto mark;
            case "2":
                // чтение из файла
                Read();
                goto mark;
            case "3":
                // удаление файла
                Delet();
                goto mark;
            default:
                break;

        }




    }
    // запись в файл
    static void Write()
    {
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        if (!dirInfo.Exists)
        {
            dirInfo.Create();
        }
        Console.WriteLine("Название файла:");
        string name = Console.ReadLine();
        Console.WriteLine("Введите данные для записи в файл:");
        string text = Console.ReadLine();
        // запись в файл
        using (FileStream fstream = new FileStream($"{path}\\{name}", FileMode.OpenOrCreate))
        {
            // преобразуем строку в байты
            byte[] array = System.Text.Encoding.Default.GetBytes(text);
            // запись массива байтов в файл
            fstream.Write(array, 0, array.Length);
            Console.WriteLine("данные записаны в файл");
        }
    }
    // чтение из файла
    static void Read()
    {
        Console.WriteLine("Название файла:");
        string name = Console.ReadLine();
        using (FileStream fstream = File.OpenRead($"{path}\\{name}"))
        {

            // преобразуем строку в байты
            byte[] array = new byte[fstream.Length];
            // считываем данные
            fstream.Read(array, 0, array.Length);
            // декодируем байты в строку
            string textFromFile = System.Text.Encoding.Default.GetString(array);
            Console.WriteLine($"Текст из файла: {textFromFile}");
        }
    }
    // удаление файла
    static void Delet()
    {
        Console.WriteLine("Название файла:");
        string name = Console.ReadLine();
        File.Delete($"{path}\\{name}");
        Console.WriteLine("Файл удалён");
        Console.ReadLine();
    }
    static void SHA_256()
    {
        if (Directory.Exists(path))
        {
            // Create a DirectoryInfo object representing the specified directory.
            var dir = new DirectoryInfo(path);
            // Get the FileInfo objects for every file in the directory.
            FileInfo[] files = dir.GetFiles();
            // Initialize a SHA256 hash object.
            using (SHA256 mySHA256 = SHA256.Create())
            {
                // Compute and print the hash values for each file in directory.
                foreach (FileInfo fInfo in files)
                {
                    using (FileStream fileStream = fInfo.Open(FileMode.Open))
                    {
                        try
                        {
                            // Create a fileStream for the file.
                            // Be sure it's positioned to the beginning of the stream.
                            fileStream.Position = 0;
                            // Compute the hash of the fileStream.
                            byte[] hashValue = mySHA256.ComputeHash(fileStream);
                            // Write the name and hash value of the file to the console.
                            Console.Write($"{fInfo.Name}: ");
                            PrintByteArray(hashValue);
                        }
                        catch (IOException e)
                        {
                            Console.WriteLine($"I/O Exception: {e.Message}");
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            Console.WriteLine($"Access Exception: {e.Message}");
                        }
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("The directory specified could not be found.");
        }
    }
    // Display the byte array in a readable format.
    static void PrintByteArray(byte[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Console.Write($"{array[i]:X2}");
            if ((i % 4) == 3) Console.Write(" ");
        }
        Console.WriteLine();
    }
    //брутфорс
    public static bool BruteForce(string testChars, int startLength, int endLength, BruteForceTest testCallback)
    {
        for (int len = startLength; len <= endLength; ++len)
        {
            char[] chars = new char[len];

            for (int i = 0; i < len; ++i)
                chars[i] = testChars[0];

            if (testCallback(ref chars))
                return true;

            for (int i1 = len - 1; i1 > -1; --i1)
            {
                int i2 = 0;

                for (i2 = testChars.IndexOf(chars[i1]) + 1; i2 < testChars.Length; ++i2)
                {
                    chars[i1] = testChars[i2];

                    if (testCallback(ref chars))
                        return true;

                    for (int i3 = i1 + 1; i3 < len; ++i3)
                    {
                        if (chars[i3] != testChars[testChars.Length - 1])
                        {
                            i1 = len;
                            goto outerBreak;
                        }
                    }
                }

            outerBreak:
                if (i2 == testChars.Length)
                    chars[i1] = testChars[0];
            }
        }

        return false;
    }
    static void BrutMain()
    {
    mark:
        Console.WriteLine("ввод: ");
        string choice = Console.ReadLine();
        if (choice.Length <= 5)
        {
            BruteForceTest testCallback = delegate (ref char[] testChars)
            {
                var str = new string(testChars);
                return (str == $"{choice}");//наш пароль от сейфа :)
            };
            bool result = BruteForce("abcdefghijklmnopqrstuvwxyz", 1, 5, testCallback);
            Console.WriteLine(result);
            Console.ReadKey();
        }
        else
        {
            Console.WriteLine("Неверный пароль!");
            goto mark;
        }



    }
    static void Theard()
    {
        object locker = new();
        Stopwatch stopWatch = new Stopwatch();
        Console.WriteLine("Колл-во потоков:");
        int theards = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Дождитесь меню!");
        for (int i = 1; i <= theards; i++)
        {
            stopWatch.Start();
            Thread myThread = new(Print);
            myThread.Name = $"Поток {i}";   // устанавливаем имя для каждого потока
            myThread.Start();
        }
        // действия, выполняемые во втором потокке
        void Print()
        {
            lock (locker)
            {

                for (int i = 0; i < 5; i++)
                {
                    Thread currentThread = Thread.CurrentThread;
                    Console.WriteLine($"{Thread.CurrentThread.Name}: {i}");

                }
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
                Console.WriteLine("RunTime " + elapsedTime);
            }

        }
    }
}

