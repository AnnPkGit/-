CREATE TABLE web_users (
    Id uniqueidentifier,
    RealName varchar(255),
    Email varchar(255),
    [Login] varchar(255),
    [Password] varchar(255),
	SecretQuestionId uniqueidentifier,
	SecretQuestionAnswear varchar(255),
	Avatar varchar(255),
);

CREATE TABLE web_secret_questions (
    Id uniqueidentifier,
	Question varchar(255),
);

--команды для теста
INSERT INTO web_secret_questions (Id, Question)
VALUES (NEWID(), 'Favourite color');

--обязательно заменить поле с ! перед выполнением запроса
INSERT INTO web_users (Id, RealName, Email, [Login], [Password], SecretQuestionId, SecretQuestionAnswear, Avatar)
VALUES (NEWID(), 'Ann', 'email@em.com', 'An', '123', '!тут должен быть айдишник из таблицы web_secret_questions', 'red', NULL); 

SELECT web_users.RealName, web_secret_questions.Question, web_users.SecretQuestionAnswear
FROM web_users JOIN web_secret_questions ON  web_users.SecretQuestionId =  web_secret_questions.Id;