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

ALTER TABLE Posts
ADD [Date] DateTimeOffset;

INSERT INTO Posts ([Date])
VALUES ('2023-01-12T09:51:52.7052213+02:00');

UPDATE Posts
SET Date = '2023-01-12T09:51:52.7052213+02:00'
WHERE Text = 'Well, today was a hard day';

ALTER TABLE Posts
ADD [AuthorId] uniqueidentifier;

UPDATE Posts
SET AuthorId = 'E6F7F00A-4694-4CCD-44DD-08DAF151331F';

