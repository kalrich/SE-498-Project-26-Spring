DELETE FROM UserComics;
DELETE FROM Users;
DELETE FROM Comics;

INSERT INTO Users (Username, Email, Password) VALUES
('Peter Parker', 'peter@marvel.com', 'spiderman123'),
('Tony Stark', 'tony@marvel.com', 'ironman123'),
('Josh', 'Josh@gmail.com', 'password');

INSERT INTO Comics
(Id, Title, Author, Genre, SecondaryGenre, Description, CoverImagePath, PdfPath, IsIReadPick)
VALUES
(1, 'Daring Mystery Vol. 1 Issue 6', 'Marvel Comics', 'Mystery', 'Classic',
 'A classic Golden Age Marvel issue from Daring Mystery Comics.',
 '/images/covers/Daring_Mystery_Vol1_Iss6_COVER.jpeg',
 '/comics/Daring_Mystery_Vol1_Iss6.pdf', 0),

(2, 'Human Torch Vol. 1 Issue 2', 'Marvel Comics', 'Action', 'Classic',
 'A Golden Age Human Torch issue with early Marvel action storytelling.',
 '/images/covers/Human_Torch_Vol1_Iss2_COVER.jpeg',
 '/comics/Human_Torch_Vol1_Iss2.pdf', 0),

(3, 'Human Torch Vol. 1 Issue 3', 'Marvel Comics', 'Action', 'Adventure',
 'Another early Human Torch issue featuring vintage Marvel comics action.',
 '/images/covers/HumanTorch_Vol1_Iss3_COVER.jpeg',
 '/comics/HumanTorch_Vol1_Iss3.pdf', 0),

(4, 'Marvel Mystery Vol. 1 Issue 10', 'Marvel Comics', 'Mystery', 'Classic',
 'A classic Marvel Mystery issue with suspense, pulp-style action, and Golden Age artwork.',
 '/images/covers/Marvel_Mystery_Vol1_Iss10_COVER.jpeg',
 '/comics/Marvel_Mystery_Vol1_Iss10.pdf', 0),

(5, 'Marvel Mystery Vol. 1 Issue 11', 'Marvel Comics', 'Mystery', 'Classic',
 'A Golden Age Marvel Mystery issue full of action, suspense, and classic comic visuals.',
 '/images/covers/Marvel_Mystery_Vol1_Iss11_COVER.jpeg',
 '/comics/Marvel_Mystery_Vol1_Iss11.pdf', 0),

(6, 'Marvel Mystery Vol. 1 Issue 12', 'Marvel Comics', 'Mystery', 'Classic',
 'A classic Golden Age Marvel mystery issue from Volume 1, Issue 12.',
 '/images/covers/Marvel_Mystery_Vol1_Iss12_COVER.jpeg',
 '/comics/Marvel_Mystery_Vol1_Iss12.pdf', 0),

(7, 'Marvel Mystery Vol. 1 Issue 13', 'Marvel Comics', 'Mystery', 'Adventure',
 'An early Marvel Mystery issue with classic Golden Age storytelling and cover art.',
 '/images/covers/Marvel_Mystery_Vol1_Iss13_COVER.jpeg',
 '/comics/Marvel_Mystery_Vol1_Iss13.pdf', 0),

(8, 'Marvel Mystery Vol. 1 Issue 14', 'Marvel Comics', 'Mystery', 'Adventure',
 'Another early Marvel Mystery issue with classic pulp-era storytelling and action.',
 '/images/covers/Marvel_Mystery_Vol1_Iss14_COVER.jpeg',
 '/comics/Marvel_Mystery_Vol1_Iss14.pdf', 0),

(9, 'Marvel Mystery Vol. 1 Issue 15', 'Marvel Comics', 'Mystery', 'Action',
 'A Golden Age Marvel issue featuring suspense, action, and vintage comic art.',
 '/images/covers/Marvel_Mystery_Vol1_Iss15_COVER.jpeg',
 '/comics/Marvel_Mystery_Vol1_Iss15.pdf', 0),

(10, 'Mystic Comics Vol. 1 Issue 4', 'Marvel Comics', 'Fantasy', 'Mystery',
 'A classic Mystic Comics issue featuring supernatural themes and early Marvel storytelling.',
 '/images/covers/Mystic_Comics_Vol1_Iss4_COVER.jpeg',
 '/comics/Mystic_Comics_Vol1_Iss4.pdf', 1);

INSERT INTO UserComics (UserId, ComicId, Shelf, ProgressPercent) VALUES
(1, 1, 'UpNext', 0),
(1, 2, 'UpNext', 0),
(1, 3, 'CurrentlyReading', 15),
(1, 6, 'CurrentlyReading', 35),
(1, 10, 'Completed', 100),
(1, 4, 'Trending', 0),
(1, 5, 'Trending', 0),
(1, 9, 'Trending', 0),
(1, 7, 'UpNext', 0),
(1, 8, 'UpNext', 0);