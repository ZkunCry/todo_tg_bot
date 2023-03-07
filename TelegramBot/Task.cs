using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
     class Tasks:BaseTask, Itask
    {
      
        private List<SubTask> subtasklist = new List<SubTask>();
        public List<SubTask> Subtasklist { get=>subtasklist; set =>subtasklist =value; }
        public Tasks():base(){}
        public Tasks(string task):base(task){}
        public void Print() => Console.WriteLine($"Create:{Date}   {Task}");
        public void Add(List<Itask> itasks, int pos = 0) => itasks.Add(this);
        public void AddSubTask(in string SubTaskString) =>subtasklist.Add(new SubTask(SubTaskString));

        public void Delete(List<Itask> itasks, int TaskIndex, int SubIndex = 0)
        {
            if (TaskIndex > itasks.Count)
                Console.WriteLine("No such object exists");
            else
                itasks.RemoveAt(TaskIndex);
        }
    }
}
