﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algodat_UE3.SearchAlgos
{
    class BinarySearch
    {
        public int CompareCount {get; private set; }

        public BinarySearch()
        {
            this.CompareCount = 0;
        }

        public int Search(int number, int[] compareArray)
        {
            int FirstIndex = 0;
            int LastIndex = compareArray.Length;

            if (number > compareArray[compareArray.Length - 1])
            {
                this.CompareCount++;
                return -1;
            }

            if (number < compareArray[0])
            {
                this.CompareCount++;
                return -1;
            }
 
            while (FirstIndex <= LastIndex)
            {
                this.CompareCount++;

                int Mid = this.FindMiddle(FirstIndex, LastIndex);
                if (number > compareArray[Mid])
                {
                    FirstIndex = Mid + 1;
                }
                else if (number < compareArray[Mid])
                {
                    LastIndex = Mid - 1;
                }
                else
                {
                    return Mid;
                }
            }
            return -1;
        }

        private int FindMiddle(int first, int last)
        {
            return (first + last) / 2;
        }
    }
}
