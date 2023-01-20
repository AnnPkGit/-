CREATE TABLE Comments (
    Id uniqueidentifier,
    AuthorId uniqueidentifier,
	PostId uniqueidentifier,
	[Date] DateTimeOffset,
	LikesCount INT,
	Text varchar(500)
);


INSERT INTO Comments (Id, AuthorId, PostId, [Date], LikesCount, Text)
VALUES (NEWID(), 'E6F7F00A-4694-4CCD-44DD-08DAF151331F', '7C637FFC-8CEE-4593-87EF-E388CA258DEB', '2023-01-12T10:57:52.7052213+00:00', 0, 'Wizards!');

INSERT INTO Comments (Id, AuthorId, PostId, [Date], LikesCount, Text)
VALUES (NEWID(), 'E6F7F00A-4694-4CCD-44DD-08DAF151331F', '7C637FFC-8CEE-4593-87EF-E388CA258DEB', '2023-01-12T10:57:52.7052213+00:00', 0, 'a man who is believed to have magical powers and who uses them to harm or help other people.');