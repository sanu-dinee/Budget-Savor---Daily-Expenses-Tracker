using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace budgetSavour
{
    internal class Debt
    {
        public string LenderName { get; private set; }
        public float Amount { get; private set; }
        public string LendDate { get; private set; }
        public string DueDate { get; private set; }
        public string Status { get; private set; }

        public float PaidAmount { get; private set; }

        public int AccountNo { get; private set; }

       public Debt(string lenderName,float amount,string lDate,string dDate,string status,int accountNo){
            
            LenderName = lenderName;
            Amount = amount;
            LendDate = lDate;
            DueDate = dDate;
            Status = status;
            AccountNo = accountNo;
        
        }

        public Debt(string lenderName, float amount, string dDate, string status, float paidAmount ,int accountNo)
        {

            LenderName = lenderName;
            Amount = amount;
            DueDate = dDate;
            Status = status;
            AccountNo = accountNo;
            this.PaidAmount= paidAmount;

        }

        public Debt(int accountNo, float paidAmount,string status,string lenderName)
        {
            LenderName = lenderName; 
            AccountNo=accountNo;
            PaidAmount = paidAmount;
            Status = status;
           



        }

        public Debt(int accountNo, float paidAmount,string lenderName, float amount)
        {
            LenderName = lenderName;
            AccountNo = accountNo;
            PaidAmount = paidAmount;
            Amount = amount;




        }

        }
    public static class Progress
    {
        public static float Amount { get; set; }
        public static float paidAmount { get; set; }
    }
}
