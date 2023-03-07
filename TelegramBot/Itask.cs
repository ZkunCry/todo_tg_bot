using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    interface Itask
    {
         string Date { get; set; }
         string Finish { get; set; }
         string Task { get; set; }
         void Print();
         void Add(List<Itask> itasks, int pos = 0);
         void Delete(List<Itask> itasks, int TaskIndex, int SubIndex = 0);

    }
}
