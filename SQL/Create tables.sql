CREATE TABLE Students(
Student_id int IDENTITY(1,1),
Surname varchar(50),
Name varchar(50),
Patronymic varchar(50),
email varchar(50),
phone_number varchar(50),
password varchar(50)
);

CREATE TABLE Employee(
Employee_id int IDENTITY(1,1),
Surname varchar(50),
Name varchar(50),
Patronymic varchar(50),
email varchar(50),
phone_number varchar(50),
password varchar(50)
);

CREATE TABLE Marks(
Mark_id int IDENTITY(1,1),
Mark int,
Student_id int,
Lesson_id int
);

CREATE TABLE Disciplines(
Discipline_id int IDENTITY(1,1),
Discipline_name varchar(50)
);

CREATE TABLE Lessons(
Lesson_id int IDENTITY(1,1),
Employee_id int,
Discipline_id int,
Group_id int,
);

CREATE TABLE Groups(
Group_id int IDENTITY(1,1),
Group_name varchar(50)
);

CREATE TABLE Group_List(
Student_id int,
Group_id int,
);