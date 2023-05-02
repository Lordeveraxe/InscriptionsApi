create table Students(
    student_id int NOT NULL IDENTITY(1,1) ,
    student_name varchar(100) NOT NULL,
    student_ln varchar(100) NOT NULL,
    type_doc_student varchar(50) NOT NULL,
    student_doc varchar(50) NOT NULL,
    student_status int NOT NULL CHECK(student_status IN('0','1','2','3')),
    student_genre varchar(10) NOT NULL CHECK(student_genre IN('M','F','O'))
);

create table Subjects(
    subject_id int NOT NULL IDENTITY(1,1) ,
    subject_name varchar(50) NOT NULL,
    subject_capacity int NOT NULL,
    subject_status int NOT NULL CHECK(subject_status IN('0','1'))
);

create table Inscriptions(
    incription_id int IDENTITY(1,1),
    student_id int NOT NULL,
    subject_id int NOT NULL,
    incription_date smalldatetime NOT NULL
);

alter table Students add PRIMARY KEY (student_id);


alter table Subjects add PRIMARY KEY (subject_id);

alter table Inscriptions add PRIMARY KEY (incription_id);

alter table Inscriptions add FOREIGN KEY (student_id) REFERENCES Students(student_id);
alter table Inscriptions add FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id);

SELECT CONVERT(datetime, incription_date) as incription_date
FROM Inscriptions;

ALTER TABLE Inscriptions 
ALTER COLUMN incription_date datetime NOT NULL;

ALTER TABLE Students 
ADD student_photo VARCHAR(255);

Create table Users(
user_id int NOT NULL IDENTITY(1,1) ,
user_name varchar(50) NOT NULL,
user_email varchar(50) NOT NULL,
user_state int NOT NULL CHECK(user_state IN('0','1'))
);

Create table Credentials(
user_id int NOT NULL,
user_password varchar(250) NOT NULL,
credential_salt varchar(255) NOT NULL
);

alter table Users add PRIMARY KEY (user_id);
alter table Credentials add PRIMARY KEY (user_id);
alter table Credentials add FOREIGN KEY (user_id) REFERENCES Users(user_id);

ALTER TABLE Users
ADD CONSTRAINT UserNameUnique UNIQUE (user_name);