CREATE TABLE Posts (
    Id uniqueidentifier,
    [Text] varchar(255),
	LikesCount INT,
	CommentsCount INT
);

CREATE TABLE PostImages (
    Id uniqueidentifier,
    RelatedPostId uniqueidentifier,
    [Name] varchar(255)
);

INSERT INTO Posts (Id, [Text], CommentsCount, LikesCount)
VALUES (NEWID(), 'Well, today was a hard day', 0, 2);

INSERT INTO PostImages (Id, RelatedPostId, Name)
VALUES (NEWID(), '7FC05C81-9D5D-46F7-AA62-2B0B3DD7B60C', 'FirstImage');

INSERT INTO PostImages (Id, RelatedPostId, Name)
VALUES (NEWID(), '7FC05C81-9D5D-46F7-AA62-2B0B3DD7B60C', 'SecondImage');

SELECT Posts.Id, PostImages.Name
FROM Posts JOIN PostImages ON  PostImages.RelatedPostId =  Posts.Id;