using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    class BaseTask
    {
        private string date;
        private string finish;
        private string task;

        public BaseTask()
        {
            finish = "❌";
            task = null;
            date = DateTime.Now.ToString().Remove(DateTime.Now.ToString().Length - 3, 3);
        }
        public BaseTask(string task):this()
        {
           this.task = task;
        }
        public string Finish
        {
            get => finish;
            set
            {
                if (this.GetType() == typeof(Todolist))
                    Console.WriteLine("Error");
                else
                    finish = value;
            }
        }
        public string Task
        {
            get => task;
            set => task = value;
        }
        public string Date { get => date; set => date = value; }
    }
}
