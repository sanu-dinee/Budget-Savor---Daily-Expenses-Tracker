using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace budgetSavour
{
    internal class NewExpenses
    {

        public float amount { get; private set; }
        public string expenseCategory { get; private set; }
        public string Date { get; private set; }
        public string expenseDescription { get; private set; }
        public string uploadReceipt {  get; private set; }

        public int AccountNo {  get; private set; }

        public NewExpenses(int accountNo ,float Amount,string category,string date,string description,string receipt)
        {
            AccountNo= accountNo;
            amount = Amount;
            expenseCategory = category;
            Date = date;
            expenseDescription = description;
            uploadReceipt = receipt;

        }

        public NewExpenses(int accountNo ,float Amount, string category, string date, string description)
        {
            AccountNo= accountNo;
            amount = Amount;
            expenseCategory = category;
            Date = date;
            expenseDescription = description;


            

        }


    }
}
