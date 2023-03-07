using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TelegramBot
{
    interface ITodoList
    {

        void Add(in Itask task,int pos=0);
        public List<Itask> List { get; }
    }

     class Todolist :ITodoList
    {

        private List<Itask> todolists;

        private int count;
        public int Count { get => count; }
        public List<Itask> List { get => todolists; }

        public Todolist() : base()
        {
            todolists = new();
            count = 0;
        }
        public Todolist(in Tasks newtask)
        {
            todolists = new List<Itask>
                {
                    newtask
                };
            count++;
        }
        public void Add(in Itask task,int pos=0)
        {
            if (task is not null)
                task.Add(todolists, pos);
        }
   
        public void Delete(int TaskIndex, int SubIndex = -1)
        {
            if (SubIndex < -1 || TaskIndex == 0 || TaskIndex < 0)
                Console.WriteLine("Incorrect pos");
            else if (TaskIndex > 0 && SubIndex == -1)
            { TaskIndex--; todolists.RemoveAt(TaskIndex); }
            else if (TaskIndex > 0 && SubIndex > -1 && SubIndex != 0)
            { TaskIndex--;SubIndex--; ((Tasks)todolists[TaskIndex]).Subtasklist.RemoveAt(SubIndex); }
                
        }
        public void SetAccept(int number)
        {
            if (todolists == null || number > todolists?.Count || number < 0)
                Console.WriteLine("Out of range");
            else
            {
                number--;
                todolists[number].Finish = "✅";
                if (((Tasks)todolists[number]).Subtasklist.Count > 0)
                    foreach (var item in ((Tasks)todolists[number]).Subtasklist)
                        item.Finish = "✅";
            }
        }


    }
    }


