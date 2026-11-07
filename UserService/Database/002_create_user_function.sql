CREATE OR REPLACE FUNCTION create_user(
       _login TEXT,
       _password TEXT,
       _name TEXT,
       _surname TEXT,
       _age INTEGER
)
RETURNS INTEGER AS $$
DECLARE
       new_user_id INTEGER; 
BEGIN 
    IF EXISTS (SELECT 1 FROM Users Where login = _login) Then 
       return 0;
    end IF;
    
    INSERT INTO Users(login, password, name,surname,age)
    values (_login, _password, _name, _surname, _age)
    Returning id INTO new_user_id;
    RETURN new_user_id;
end; 
$$ LANGUAGE plpgsql;