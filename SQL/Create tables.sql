CREATE TABLE Students(
Student_id int PRIMARY KEY IDENTITY(1,1),
Surname varchar(50),
Name varchar(50),
Patronymic varchar(50),
email varchar(50),
phone_number varchar(50),
password varchar(50)
);

CREATE TABLE Employee(
Employee_id int PRIMARY KEY IDENTITY(1,1),
Surname varchar(50),
Name varchar(50),
Patronymic varchar(50),
email varchar(50),
phone_number varchar(50),
password varchar(50)
);

CREATE TABLE Work_types(
id_type int,
name_type int
);

CREATE TABLE Marks(
Mark_id int PRIMARY KEY IDENTITY(1,1),
Mark int,
Student_id int FOREIGN KEY REFERENCES Students(Student_id),
Lesson_id int FOREIGN KEY REFERENCES Lessons(Lesson_id),
id_type int FOREIGN KEY REFERENCES Work_types(id_type)
);

CREATE TABLE Disciplines(
Discipline_id int PRIMARY KEY IDENTITY(1,1),
Discipline_name varchar(50)
);

CREATE TABLE Lessons(
Lesson_id int PRIMARY KEY IDENTITY(1,1),
Employee_id int,
Discipline_id int,
Group_id int,
);

CREATE TABLE Groups(
Group_id int PRIMARY KEY IDENTITY(1,1),
Group_name varchar(50)
);

CREATE TABLE Group_List(
Student_id int,
Group_id int FOREIGN KEY REFERENCES Groups(Group_id)
);

