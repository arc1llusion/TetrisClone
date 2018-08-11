

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsPhone_Tetris.Utility
{
    /// <summary>
    /// Static class for the random instance. It is in fact used in more than once place.
    /// </summary>
    public static class RandomGenerator
    {
        /// <summary>
        /// Random instance used for random number generation and an initial Seed
        /// </summary>
        public static readonly Random Instance = new Random();
    }


    /// <summary>
    /// The random bag will create a sequential list of 0-n inclusive of n and will pick a permutation from that amount.
    /// 
    /// In the case of Tetris, it will choose from 0-6 and empty the bag before resetting.
    /// </summary>
    public class RandomBag
    {     
        /// <summary>
        /// Bag that ensures a somewhat even spread of tetrominos
        /// </summary>
        private List<int> bag = new List<int>();

        /// <summary>
        /// Identifies the maximum number in the list to permutate. That is, the number of permutations will be n! (n factorial)
        /// </summary>
        private readonly int Max;

        /// <summary>
        /// Constructor initializes bag for random generation. This ensures that permutations of the 7 pieces are chosen
        /// </summary>
        /// <param name="max">The inclusive maximum to find permutations</param>
        public RandomBag(int max)
        {
            this.Max = max;
            ResetBag();
        }

        /// <summary>
        /// Gets a random number to represent a Tetromino by the Tetris Standard. This eventually represents a permutation of 7.
        /// 
        /// 7! = 5040 different possible permutations.
        /// </summary>
        /// <returns>Integer to represent a number for a Tetris piece</returns>
        public int GetRandomNumberForTetris() {
            int ret = bag[RandomGenerator.Instance.Next(0, bag.Count)];
            bag.Remove(ret);

            if (bag.Count == 0)
                ResetBag();

            return ret;
        }

        /// <summary>
        /// Resets the bag to the beginning state with all possible values
        /// </summary>
        public void ResetBag()
        {
            bag.Clear();

            for (int i = 0; i <= Max; i++)
                bag.Add(i);
        }
    }
}
