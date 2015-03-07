using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace AutoCountdown
{
    class Searcher
    {
        private string[] Dictionary = new string[1];
        public Dictionary<string, int> Results = new Dictionary<string, int>();
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



            Dictionary = File.ReadAllLines("./dict.txt");
            //string[] letterss = letters;
            foreach (string word in Dictionary)
            {
                if (FitsIn(word.ToUpper(), starter.ToUpper()))
                {
                    if (word.Length > 5)
                    {
                        Results.Add(word, word.Length);
                    }
                }
            }
        }


        private bool FitsIn(string word, string OnTheTable)
        {
            string[] letters = new string[9];
            for (int i = 0; i < 9; i++)
                letters[i] = OnTheTable.Substring(i, 1);

            for (int i = 0; i < word.Length; i++)
            {
                string let = word.Substring(i, 1);
                if (!letters.Contains(let))
                    return false;
                else
                {
                    // Scan though and remove that one to ensure its not used again.
                    for (int n = 0; n < letters.Length; n++)
                    {
                        if (letters[n] == let)
                        {
                            letters[n] = "";
                            break;
                        }
                    }
                }
            }
            return true;
        }
    }
}
