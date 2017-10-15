# LyDbLib

<h3>sample code</h3>

<h4>table class</h4>


    public class sample
    {
        public sample() { }
        public long id { get; set; }
        public int Num { get; set; }
        public string Text { get; set; }
    }


<h4>using LyEntity</h4>

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
            
            


<h3>sample view</h3>


 <div style="word-wrap:break-word;width:100px">     
 
            DELETE FROM sample
            INSERT INTO sample (id, Num, Text)
            OUTPUT INSERTED.*
            VALUES (@EX0, @EX1, @EX2)
            parameter ==============
            EX0 = 0
            EX1 = 0
            EX2 = Insert

            INSERT INTO sample (id, Num, Text)
            OUTPUT INSERTED.*
            VALUES (@EX0, @EX1, @EX2)
            parameter ==============
            EX0 = 1
            EX1 = 0
            EX2 = Insert

            INSERT INTO sample (id, Num, Text)
            OUTPUT INSERTED.*
            VALUES (@EX0, @EX1, @EX2)
            parameter ==============
            EX0 = 2
            EX1 = 0
            EX2 = Insert

            INSERT INTO sample (id, Num, Text)
            OUTPUT INSERTED.*
            VALUES (@EX0, @EX1, @EX2)
            parameter ==============
            EX0 = 3
            EX1 = 0
            EX2 = Insert

            INSERT INTO sample (id, Num, Text)
            OUTPUT INSERTED.*
            VALUES (@EX0, @EX1, @EX2)
            parameter ==============
            EX0 = 4
            EX1 = 0
            EX2 = Insert

            UPDATE sample
            Set Text=@EX0
            OUTPUT INSERTED.*
            WHERE (id=@EX1)
            parameter ==============
            EX0 = Update
            EX1 = 1

            DELETE FROM sample
            WHERE (id=@EX0)
            parameter ==============
            EX0 = 2

            SELECT id, Num, Text
            FROM sample
            WHERE (id>@EX0)
            ORDER BY id, Num, Text Desc
            parameter ==============
            EX0 = 0

            1 0 Update
            3 0 Insert
            4 0 Insert
            
            
            INSERT INTO sample (id, Num, Text)
            OUTPUT INSERTED.*
            VALUES (@EX0, @EX1, @EX2)
            parameter ==============
            EX0 = 14
            EX1 = 0
            EX2 = Insert

            UPDATE sample
            Set Text=@EX0
            OUTPUT INSERTED.*
            WHERE (id>@EX1)
            parameter ==============
            EX0 = Update
            EX1 = 12

            13 0 Update
            14 0 Update
 </div>           
      
