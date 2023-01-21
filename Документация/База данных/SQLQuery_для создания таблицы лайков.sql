Create table LikeReactions (
    Id uniqueidentifier,
	RelatedPostId uniqueidentifier,
	RelatedUserId uniqueidentifier,
	IsLiked int
);

INSERT INTO LikeReactions (Id, RelatedPostId, RelatedUserId, IsLiked)
VALUES (NEWID(), '7C637FFC-8CEE-4593-87EF-E388CA258DEB', 'E6F7F00A-4694-4CCD-44DD-08DAF151331F', 1);