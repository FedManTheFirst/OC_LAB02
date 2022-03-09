using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Linq;
//Отдельно создать перед работай файлы с хэшами
public class OC_LAB02
{
    #region Private variables

    // the secret password which we will try to find via brute force
    static string password;
    private static string result1;
    private static string result2;
    private static readonly Encoding encoding = Encoding.UTF8;
    private static string path = @"D:\Documents";

    private static bool isMatched = false;

    /* The length of the charactersToTest Array is stored in a
     * additional variable to increase performance  */
    private static int charactersToTestLength = 0;
    private static long computedKeys = 0;

    /* An array containing the characters which will be used to create the brute force keys,
     * if less characters are used (e.g. only lower case chars) the faster the password is matched  */
    private static char[] charactersToTest =
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
        'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
        'u', 'v', 'w', 'x', 'y', 'z'
    };

    #endregion
    public static void Main(string[] args)
    {
    mark:
        Console.WriteLine("1-Работа с файлами, 2-брутфорс,: ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                CrateFile();
                goto mark;
            case "2":
                Console.WriteLine("1-Хэш из файла, 2-Ввод хэша: ");
                string hash = Console.ReadLine();
                switch (hash)
                {
                    case "1":
                        Console.WriteLine("Название файла:");
                        string name = Console.ReadLine();
                        using (FileStream fstream = File.OpenRead($"{path}\\{name}"))
                        {
                            // преобразуем строку в байты
                            byte[] array = new byte[fstream.Length];
                            // считываем данные
                            fstream.Read(array, 0, array.Length);
                            // декодируем байты в строку
                            password = System.Text.Encoding.Default.GetString(array);
                            Console.WriteLine($"Текст из файла:{password}");
                        }
                        break;
                    case "2":
                        Console.WriteLine("Введите хэш:");
                        password = Console.ReadLine();
                        break;
                    default:
                        break;
                }
                Theard();
                Thread.Sleep(100000);
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
                Del();
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
    static void Del()
    {
        Console.WriteLine("Название файла:");
        string name = Console.ReadLine();
        File.Delete($"{path}\\{name}");
        Console.WriteLine("Файл удалён");
        Console.ReadLine();
    }
    //брутфорс
    public static void Brutmain()
    {
        
        

            var timeStarted = DateTime.Now;
            Console.WriteLine("Start BruteForce - {0}", timeStarted.ToString());

            // The length of the array is stored permanently during runtime
            charactersToTestLength = charactersToTest.Length;

            // The length of the password is unknown, so we have to run trough the full search space
            var estimatedPasswordLength = 5;

            while (!isMatched)
            {
                /* The estimated length of the password will be increased and every possible key for this
                 * key length will be created and compared against the password */
                
                startBruteForce(estimatedPasswordLength);
            }

            Console.WriteLine("Password matched. - {0}", DateTime.Now.ToString());
            Console.WriteLine("Time passed: {0}s", DateTime.Now.Subtract(timeStarted).TotalSeconds);
            Console.WriteLine("Resolved password: {0}", result1);
            Console.WriteLine("Resolved hesh: {0}", result2);
            Console.WriteLine("Computed keys: {0}", computedKeys);
            Thread.Sleep(1000);
        
    }

    #region Private methods

    /// <summary>
    /// Starts the recursive method which will create the keys via brute force
    /// </summary>
    /// <param name="keyLength">The length of the key</param>
    private static void startBruteForce(int keyLength)
    {
        var keyChars = createCharArray(keyLength, charactersToTest[0]);
        // The index of the last character will be stored for slight perfomance improvement
        var indexOfLastChar = keyLength - 1;
        createNewKey(0, keyChars, keyLength, indexOfLastChar);
    }

    /// <summary>
    /// Creates a new char array of a specific length filled with the defaultChar
    /// </summary>
    /// <param name="length">The length of the array</param>
    /// <param name="defaultChar">The char with whom the array will be filled</param>
    /// <returns></returns>
    private static char[] createCharArray(int length, char defaultChar)
    {
        return (from c in new char[length] select defaultChar).ToArray();
    }

    /// <summary>
    /// This is the main workhorse, it creates new keys and compares them to the password until the password
    /// is matched or all keys of the current key length have been checked
    /// </summary>
    /// <param name="currentCharPosition">The position of the char which is replaced by new characters currently</param>
    /// <param name="keyChars">The current key represented as char array</param>
    /// <param name="keyLength">The length of the key</param>
    /// <param name="indexOfLastChar">The index of the last character of the key</param>
    private static void createNewKey(int currentCharPosition, char[] keyChars, int keyLength, int indexOfLastChar)
    {
        var nextCharPosition = currentCharPosition + 1;
        // We are looping trough the full length of our charactersToTest array
        for (int i = 0; i < charactersToTestLength; i++)
        {
            /* The character at the currentCharPosition will be replaced by a
             * new character from the charactersToTest array => a new key combination will be created */
            keyChars[currentCharPosition] = charactersToTest[i];

            // The method calls itself recursively until all positions of the key char array have been replaced
            if ((currentCharPosition < indexOfLastChar) )
            {
                createNewKey(nextCharPosition, keyChars, keyLength, indexOfLastChar);
            }
            else
            {
                // A new key has been created, remove this counter to improve performance
                computedKeys++;

                /* The char array will be converted to a string and compared to the password. If the password
                 * is matched the loop breaks and the password is stored as result. */
                //Console.WriteLine(new String(keyChars));
                    using (SHA256 sha256Hash = SHA256.Create())
                        if ((GetHash(sha256Hash, new String(keyChars)) == password)|| (isMatched == true))
                        {
                            if (!isMatched)
                            {
                                isMatched = true;
                                result1 = new String(keyChars);
                                result2 = GetHash(sha256Hash, new String(keyChars));
                            
                            }
                            return;
                        }
            }
        }
    }
    private static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {

        // Convert the input string to a byte array and compute the hash.
        byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        var sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }
    private static void Theard()
    {
        object locker = new();
        Console.WriteLine("Колл-во потоков:");
        int theards = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Дождитесь!");
        for (int i = 1; i <= theards; i++)
        {
            lock (locker)
            {
                Thread myThread = new(Brutmain);
                myThread.Name = $"Поток {i}";   // устанавливаем имя для каждого потока
                myThread.Start();

            }
        }

    }

    #endregion
}
