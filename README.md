# LyDbLib

<h3>sample code</h3>


            using (LyEntity<sample> db = new LyEntity<sample>(ss))
            {
                db.Delete();
                db.Execute();
                Console.WriteLine(db.GetCommandTextAndParameter());

                for (int i = 0; i < 5; i++)
                {
                    db.Insert().Output().Values(new sample() { id = i, Text = "Insert" });
                    sample ddddd = db.Query().FirstOrDefault();
                    Console.WriteLine(db.GetCommandTextAndParameter());

                }

                db.Update().Set(new { Text = "Update" }).Output().Where(m => m.id == 1);
                sample dddddu = db.Query().FirstOrDefault();
                Console.WriteLine(db.GetCommandTextAndParameter());

                db.Delete().Where(m => m.id == 2);
                int iDel = db.Execute();
                Console.WriteLine(db.GetCommandTextAndParameter());

                db.Select().Where(m => m.id >0).OrderBy(m => m, c => new { c.Text });
                List<sample> query = db.Query().ToList();
                Console.WriteLine(db.GetCommandTextAndParameter());

                foreach (var item in query)
                {
                    Console.WriteLine($"{item.id} {item.Num} {item.Text}");

                }
            }
            
            
            
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
 </div>           
      