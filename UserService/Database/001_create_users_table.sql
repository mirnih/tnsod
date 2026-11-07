create table Users(

                      Id SERIAL PRIMARY KEY,

                      Name TEXT NOT NULL,

                      Age INTEGER NOT NULL,

                      Login TEXT NOT NULL UNIQUE,

                      Password TEXT NOT NULL,

                      Surname TEXT NOT NULL

)