using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace budgetSavour
{
    internal class appUser
    {
        public int AccountNo { get; protected set; }
        public string FName { get; protected set; }
        public string LName { get; protected set; }
        public string Email { get; protected set; }
        public string Password { get; protected set; }
        public string Salt { get; protected set; }
        public string AccountType { get; protected set; }
        public string ContactNo { get; protected set; }

        public appUser(string fName, string lName, string email, string password, string accountType, string contactNo,string salt)
        {
            FName = fName;
            LName = lName;
            Email = email;
            Password = password;
            AccountType = accountType;
            ContactNo = contactNo;
            Salt = salt;
                
        }

        public appUser() { }

        public appUser(int Accountno,string password,string salt,string email)
        {
            AccountNo = Accountno;
            Password = password;
            Salt = salt;
            Email = email;

        }

        public appUser(int AccountNo)
        {
            this.AccountNo=AccountNo;
        }

     
    }

    class familyAccountUser : appUser
    {
        public int FamilyPin { get; private set; }

        public familyAccountUser(string fName, string lName, string email, string password, string accountType, string contactNo, int familyPin,string salt) : base(fName, lName, email, password, accountType, contactNo, salt)
        {
            FamilyPin=familyPin;
        }

        
        public familyAccountUser() : base() { }

        public familyAccountUser(int accountNo, int familyPin) : base(accountNo)
        {
            FamilyPin = familyPin;
        }

    }
    // store session-like data=global session data
    public static class SessionManager
    {
        public static int CurrentUserAccount { get; set; }
    }
}
