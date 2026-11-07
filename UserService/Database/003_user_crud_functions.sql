CREATE OR REPLACE FUNCTION get_user_by_id(
       _id INTEGER
)
RETURNS SETOF Users AS $$ 
BEGIN
       return query 
       select * from Users where id = _id;
End;
$$ language plpgsql;
   
CREATE OR REPLACE FUNCTION get_user_by_name_surname (
       _name TEXT,
       _surname TEXT
)
RETURNS SETOF Users as $$
BEGIN 
    RETURN query 
    select * from Users
    where (_name = name) and (surname = _surname);
end; 
$$ language plpgsql;
   
Create or replace function update_user(
       _id INTEGER,
       _name TEXT,
       _surname TEXT,
       _password TEXT,
       _age INTEGER
)
RETURNS Boolean as $$
Begin
    UPDATE Users
    set
        password =_password,
        name = _name, 
        surname = _surname,
        age = _age
    where id = _id;
    return Found; 
End;
$$ language plpgsql;
   
Create or replace function delete_user(
       _id INTEGER
)
RETURNS boolean as $$
begin 
    DELETE FROM Users
    where id = _id;
    return Found;
End;
$$ language plpgsql;