create database dbms_lab_project

use dbms_lab_project

create table users (
	id int primary key identity(1,1),
	name varchar(50),
	email varchar(50),
	password varchar(70),
	other_user int foreign key references users(id)
)

select * from users

insert into users (name, email, password) values ('Fuzail', 'fuzail@gmail.com', '12345'), ('Farzan', 'farzan@gmail.com', '12345')

create table items (
	item_id int primary key identity(1,1),
	title varchar(50),
	description varchar(MAX),
	category varchar(40),
	lost_date date,
	location_found varchar(80),
	is_found varchar(20),
	id int foreign key references users(id),
	requesting_user int foreign key references users(id)
)

select * from items where id = 2

select * from items

create table claimItem (
	claimId int primary key identity(1,1),
	userId int foreign key references users(id),
	itemId int foreign key references items(item_id),
	requestingUserId int foreign key references users(id),
	hiddenDetails varchar(100),
	lostLocation varchar(50)
)

select * from claimItem

SELECT * FROM items WHERE id = 2 AND requesting_user IS NOT NULL