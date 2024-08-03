using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management
{
    public class Book : IEquatable<Book>
    {
        public int id;
        string name;
        string author;
        int ISBN;
        int currentCopyCount;
        int borrowedCopies;
        DateTime expireDate;
        public List<int> copyIDs;
        bool isExpired;
        public static Random random = new Random();
        public Book()
        {
            currentCopyCount = 1;
            borrowedCopies = 0;
            ISBN = GenerateISBN();
            copyIDs=new List<int>();
            isExpired= false;
        }
        public Book(Book book)
        {
            NAME=book.NAME;
            author=book.AUTHOR;
            ISBNNUMBER=book.ISBNNUMBER;
            currentCopyCount=book.currentCopyCount;
            borrowedCopies=book.borrowedCopies;
            expireDate=book.expireDate;
            copyIDs = new List<int>();
            isExpired = book.isExpired;
        }
        public bool Equals(Book other)
        {
            if (other == null) { return false; }

            return this.NAME == other.NAME && this.AUTHOR == other.AUTHOR && this.id==other.id;
        }
        public string NAME { get { return name; } set { name = value; } }
         
        public bool ISEXPIRED { get { return isExpired; } set { isExpired = value; } }
        public int ISBNNUMBER { get { return ISBN; } set { ISBN = value; } }
        public string AUTHOR { get { return author; } set { author = value; } }

        public DateTime ExpireDate { get { return expireDate; } set { expireDate = value; } }

        public int COPYCOUNT { get { return currentCopyCount; } set { currentCopyCount = value; } }


        public int BORROWEDCOPIES { get { return borrowedCopies; } set { borrowedCopies = value; } }


        public int GenerateISBN()
        {
            int a = random.Next(100_000_000, 1_000_000_000);
            //Console.WriteLine("Generated " + a + " random number");
            return a;
        }
        
    }
}
