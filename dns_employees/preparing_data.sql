If(db_id(N'las_users_db_prod') IS NULL) create database las_users_db_prod;
GO
use las_users_db_prod;
GO
CREATE TABLE Employees(    
    id int IDENTITY(1,1) PRIMARY KEY CLUSTERED,    
    Name nvarchar(255) NULL,    
    Department nvarchar(50) NULL,    
    Title nvarchar(50) NULL,    
    Manager_id int NULL,    
    Hire_date date NULL,    
    INDEX PK_Employees NONCLUSTERED (Manager_id),
);       
GO
/* TO GET ALL EMPLYEES AND GET EMPLOYEE BY ID*/    
create procedure spGetEmployees    
as    
begin     
    select Employees.*, manager.Name as Manager_name
    from Employees
    LEFT JOIN Employees manager ON manager.id = Employees.manager_id
end     
GO
/* TO CREATE NEW EMPLOYEE*/    
create procedure spAddNew    
(    
@Name nvarchar(255),    
@Department nvarchar(50),    
@Title nvarchar(50),    
@Manager_id int,    
@Hire_date date    
)    
as    
begin    
    INSERT INTO dbo.Employees 
        ( 
            Name, 
	        Department, 
	        Title,
	        Manager_id,
	        Hire_date
        ) 
    VALUES 
        (  
           @Name,
	       @Department,
	       @Title,
	       @Manager_id,
	       @Hire_date 
        ) 
end    
GO
/* TO UPDATE EMPLOYEE*/    
create procedure spUpdateEmployee    
(    
@id int,    
@Name nvarchar(255),    
@Department nvarchar(50),    
@Title nvarchar(50),    
@Manager_id int,    
@Hire_date date   
)    
as    
begin    
    update Employees     
    set Name=@Name,Department=@Department,Title=@Title,Manager_id=@Manager_id,Hire_date=@Hire_date    
    where id=@id    
end    
GO
/* TO DELETE EMPLOYEE*/    
create procedure spDeleteEmployee    
(    
@id int    
)    
as    
begin    
    delete from Employees where id=@id    
end
GO
/* TO FIND EMPLOYEE BY ID */
create procedure spFindEmployeeById
(@id int)
as
begin
    select * from Employees where id=@id
end
GO
/* TO FIND ALL MANAGERS */
create procedure spEmployeesAllManagers
as
begin
    select distinct em.* from Employees em
    inner join Employees child ON em.id = child.manager_id
    order by em.Manager_id, em.Department, em.Title, em.Name, em.Hire_date, em.id
end
GO
/* FIND POSSIBLE MANAGERS LIST */
create  procedure spEmployeesPossibleManagersList
(@current_id int)
as
begin
    select * from Employees where id!=@current_id
end