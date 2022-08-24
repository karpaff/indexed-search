using System;
using System.Collections.Generic;
using System.IO;

namespace IndexedFileSearch
{
    class MainClass
    {

        public static List<File>[] HashTable = new List<File>[256];

        public static void Main(string[] args)
        {
            Console.WriteLine("Enter the path to the folder to index: ");
            string path = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Indexing files...");

            try { CreateHashTable(path); }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine();
                Console.WriteLine("The specified folder is not found. Select another folder.");
                Console.ReadKey();
                return;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine();
                Console.WriteLine("The specified folder cannot be accessed. Select another folder.");
                Console.ReadKey();
                return;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            Console.WriteLine();

            Console.WriteLine("The files in the specified folder are successfully indexed.");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("Enter 1 to find the file.\nEnter 2 to exit the program.");
                byte ans;
                try
                {
                    ans = byte.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine();
                    Console.WriteLine("The command is not recognized. Try again.");
                    break;
                }

                if (ans == 1)
                {
                    Console.WriteLine();
                    Console.WriteLine("Enter a filename to search for: ");
                    string searchfor = Console.ReadLine();
                    Console.WriteLine();
                    
                    Display(SearchFile(searchfor), searchfor);
                    Console.ReadLine();
                }
                else if (ans == 2) break;
                else Console.WriteLine("There is no such command.");
            }
        }

        // Formatted output of the result
        public static void Display(List<string> res, string searchfor)
        {
            if (res.Count == 0) Console.WriteLine("File '" + searchfor + "' is missing in the specified folder and its subfolders.");
            else
            {
                Console.WriteLine("File '" + searchfor + "' is found in the following folders: ");
                foreach (string r in res)
                    Console.WriteLine("• " + r);
            }
        }

        // Returns a list of paths to folders that contain the file fname
        public static List<string> SearchFile(string fname)
        {
            fname = fname.ToLower();
            byte hashCode;

            List<string> res = new List<string>();

            try
            {
                hashCode = GetHash(fname);
                foreach (File file in HashTable[hashCode])
                    if (file.ShortName == fname)
                        res.Add(file.FullName);
            }
            catch { }

            return res;
        }

        // Fills the hash table
        public static void CreateHashTable(string path)
        {
            Console.Write("*");
            foreach (string fpath in Directory.GetFiles(path))
            {
                File file = new File(fpath, Path.GetFileName(fpath).ToLower());

                byte hashCode = GetHash(file.ShortName);

                try { HashTable[hashCode].Add(file); }
                catch
                {
                    HashTable[hashCode] = new List<File>();
                    HashTable[hashCode].Add(file);
                }
            }

            foreach (string dpath in Directory.GetDirectories(path))
                try { CreateHashTable(dpath); }
                catch { }
        }

        // Hash function
        public static byte GetHash(string input)
        {
            uint sum = 0;
            foreach (char c in input)
                sum += c;

            return (byte)(sum / 256);
        }

        public struct File
        {
            public string FullName;
            public string ShortName;

            public File(string fulln, string shortn)
            {
                FullName = fulln;
                ShortName = shortn;
            }
        }
    }
}
