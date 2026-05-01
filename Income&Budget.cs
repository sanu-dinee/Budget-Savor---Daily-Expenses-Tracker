using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace budgetSavour
{
    internal class Income_Budget
    {
        public float Budget { get; private set; }
        public float Income { get; private set; }
        public string Month { get; private set; }
        public int AccountNo { get; private set; }

        public Income_Budget(float budget, float income, string month, int accountNo)
        {
            Budget = budget;
            Income = income;
            Month = month;
            AccountNo = accountNo;
        }


    }
}
