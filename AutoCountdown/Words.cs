using System;
using System.Linq;
using System.IO;

namespace AutoCountdown
{
    class Searcher
    {
        private string[] letters = new string[9];
        private string[] Dictionary = new string[1];
        public Searcher(string starter)
        {
            if (starter.Trim().Replace("\n\n","").Length != 9)
            {
                if (starter.Trim().Replace("\n\n", "").Length > 9)
                {
                    starter = starter.Trim().Replace("\n\n", "").Substring(0, 9);
                }
                else
                {
                    Exception up = new Exception("I need all 9 letters to file matches!");
                    throw up;
                }
            }

            for (int i = 0; i < 9; i++)
                letters[i] = starter.Substring(i, 1);

            Dictionary = File.ReadAllLines("../../../dict.txt");
            foreach (string word in Dictionary)
            {
                if (word.Length <= 9 && FitsIn(word,letters))
                {
                    Console.WriteLine("Word {0} matches",word);
                }
            }
        }


        private bool FitsIn(string word, string[] letters)
        {
            for (int i = 0; i < word.Length; i++)
            {
                string let = word.Substring(i, 1);
                if (!letters.Contains(let))
                    return false;
            }
            return true;
        }
    }
}
