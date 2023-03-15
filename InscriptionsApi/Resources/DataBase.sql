create table Students(
    id_student int not null IDENTITY(1,1) ,
    student_name varchar(100) not null,
    student_ln varchar(100) not null,
    type_doc_student varchar(50) not null,
    student_doc varchar(50) not null,
    student_status ENUM('0','1','2','3') not null,
    student_genre ENUM('M','F','O') not null
);

create table Subjects(
    subject_id int not null IDENTITY(1,1) ,
    subject_name varchar(50) not null,
    subject_capacity int not null,
    subject_status ENUM('0','1') not null
);

create table Inscriptions(
    incription_id int IDENTITY(1,1),
    student_id int not null,
    subject_id int not null,
    incription_date smalldatetime not null
);

alter table Students add PRIMARY KEY (student_id);


alter table Subjects add PRIMARY KEY (subject_id);


alter table Inscriptions add FOREIGN KEY (student_id) REFERENCES Students(student_id);
alter table Inscriptions add FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id);