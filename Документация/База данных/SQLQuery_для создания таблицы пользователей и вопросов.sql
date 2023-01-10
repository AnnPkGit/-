CREATE TABLE Users (
    Id uniqueidentifier,
    RealName varchar(255),
    Email varchar(255),
    [Login] varchar(255),
    [Password] varchar(255),
	SecretQuestionId uniqueidentifier,
	SecretQuestionAnswear varchar(255),
	Avatar varchar(255),
);

CREATE TABLE SecretQuestions (
    Id uniqueidentifier,
	Question varchar(255),
);

--команды для теста
INSERT INTO SecretQuestions (Id, Question)
VALUES (NEWID(), 'Favourite color');

--обязательно заменить поле с ! перед выполнением запроса
INSERT INTO Users (Id, RealName, Email, [Login], [Password], SecretQuestionId, SecretQuestionAnswear, Avatar)
VALUES (NEWID(), 'Ann', 'email@em.com', 'An', '123', '!тут должен быть айдишник из таблицы web_secret_questions', 'red', NULL); 

SELECT Users.RealName, SecretQuestions.Question, Users.SecretQuestionAnswear
FROM Users JOIN SecretQuestions ON  Users.SecretQuestionId =  SecretQuestions.Id;

ALTER TABLE Users
ADD PasswordSalt varchar(255);