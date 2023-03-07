using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    class SubTask :BaseTask, Itask
    {
        private static int countsub;

        public SubTask() : base() { }
        public SubTask(string Subtask):base(Subtask)
        {
            countsub++;
        }
        public  void Print() { Console.Write("\t"); Console.WriteLine($"Create:{Date}   {Task}"); }
        public  void Add(List<Itask> itasks, int pos = 0) 
        {
            if (pos > itasks.Count)
                Console.WriteLine("No such task exists.");
            else
                ((Tasks)itasks[pos]).AddSubTask(Task);
        }

        public void Delete(List<Itask> itasks, int TaskIndex, int SubIndex = 0)
        {

            if (countsub  == 0 ||TaskIndex > itasks.Count - countsub || 
                SubIndex > ((Tasks)itasks[TaskIndex]).Subtasklist.Count)
                Console.WriteLine("No such object exists");
            else
                ((Tasks)itasks[TaskIndex]).Subtasklist.RemoveAt(SubIndex);
        }
    }
}
