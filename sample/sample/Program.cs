﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbExtensions;
using System.Linq.Expressions;
using System.Reflection;
using LyDbLib;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string ss = @"Data Source=.\sqlexpress;initial catalog=sample;Integrated Security=True";
            using (LyEntity<sample> db = new LyEntity<sample>(ss))
            {
                //delete all
                db.Delete();
                db.Execute();
                Console.WriteLine(db.GetCommandTextAndParameter());

                //Insert id 0~4
                for (int i = 0; i < 5; i++)
                {
                    db.Insert().Output().Values(new sample() { id = i, Text = "Insert" });
                    sample insertdata = db.Query().FirstOrDefault();
                    Console.WriteLine(db.GetCommandTextAndParameter());
                }

                //Update id 1
                db.Update().Set(new { Text = "Update" }).Output().Where(m => m.id == 1);
                sample updateData = db.Query().FirstOrDefault();
                Console.WriteLine(db.GetCommandTextAndParameter());

                //Delete id 2
                db.Delete().Where(m => m.id == 2);
                int iDel = db.Execute();
                Console.WriteLine(db.GetCommandTextAndParameter());

                //query id >0
                db.Select().Where(m => m.id >0).OrderBy(m => m, c => new { c.Text });
                List<sample> query = db.Query().ToList();
                Console.WriteLine(db.GetCommandTextAndParameter());
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.id} {item.Num} {item.Text}");
                }

                //QueryInsert id 10~14
                for (int i = 10; i < 15; i++)
                {
                    sample insertdata = db.QueryInsert(new sample() { id = i, Text = "Insert" });
                    Console.WriteLine(db.GetCommandTextAndParameter());
                }

                //QueryUpdate id >12
                List<sample> updateDatas = db.QueryUpdate(new { Text = "Update" }, m => m.id > 12).ToList();
                Console.WriteLine(db.GetCommandTextAndParameter());
                foreach (var item in updateDatas)
                {
                    Console.WriteLine($"{item.id} {item.Num} {item.Text}");
                }

            }


            Console.ReadLine();

        }
    }
    public class sample
    {
        public sample() { }
        public long id { get; set; }
        public int Num { get; set; }
        public string Text { get; set; }
    }
    public class sample2
    {
        public sample2() { }
        public long id { get; set; }
        public int Num { get; set; }
        public string Text { get; set; }
    }
}
