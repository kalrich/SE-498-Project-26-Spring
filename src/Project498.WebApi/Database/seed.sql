DELETE FROM "Checkouts";
DELETE FROM "UserComics";
DELETE FROM "MarvelCharacters";
DELETE FROM "CharacterImages";
DELETE FROM "Users";
DELETE FROM "Comics";

INSERT INTO "Users" ("Username", "Email", "Password") VALUES
('Peter Parker', 'peter@marvel.com', 'spiderman123'),
('Tony Stark', 'tony@marvel.com', 'ironman123'),
('Josh', 'Josh@gmail.com', 'password');

INSERT INTO "Comics"
("Id", "SeriesName", "VolumeNumber", "IssueNumber", "Title", "Author", "Genre", "SecondaryGenre", "Description", "CoverImagePath", "PdfPath", "IsIReadPick")
VALUES
(1, 'Daring Mystery', 1, 6, 'Daring Mystery Vol. 1 Issue 6', 'Marvel Comics', 'Mystery', 'Classic',
 'A classic Golden Age Marvel issue from Daring Mystery Comics.',
 '/images/covers/Daring_Mystery_Vol1_Iss6_COVER.jpeg',
 '/comics/Daring_Mystery_Vol1_Iss6.pdf', false),

(2, 'Human Torch', 1, 2, 'Human Torch Vol. 1 Issue 2', 'Marvel Comics', 'Action', 'Classic',
 'A Golden Age Human Torch issue with early Marvel action storytelling.',
 '/images/covers/Human_Torch_Vol1_Iss2_COVER.jpeg',
 '/comics/Human_Torch_Vol1_Iss2.pdf', false),

(3, 'Human Torch', 1, 3, 'Human Torch Vol. 1 Issue 3', 'Marvel Comics', 'Action', 'Adventure',
 'Another early Human Torch issue featuring vintage Marvel comics action.',
 '/images/covers/HumanTorch_Vol1_Iss3_COVER.jpeg',
 '/comics/HumanTorch_Vol1_Iss3.pdf', false),

(4, 'Marvel Mystery', 1, 10, 'Marvel Mystery Vol. 1 Issue 10', 'Marvel Comics', 'Mystery', 'Classic',
 'A classic Marvel Mystery issue with suspense, pulp-style action, and Golden Age artwork.',
 '/images/covers/Marvel_Mystery_Vol1_Iss10_COVER.jpeg',
 '/comics/Marvel_Mystery_Vol1_Iss10.pdf', false),

(5, 'Marvel Mystery', 1, 11, 'Marvel Mystery Vol. 1 Issue 11', 'Marvel Comics', 'Mystery', 'Classic',
 'A Golden Age Marvel Mystery issue full of action, suspense, and classic comic visuals.',
 '/images/covers/Marvel_Mystery_Vol1_Iss11_COVER.jpeg',
 '/comics/Marvel_Mystery_Vol1_Iss11.pdf', false),

(6, 'Marvel Mystery', 1, 12, 'Marvel Mystery Vol. 1 Issue 12', 'Marvel Comics', 'Mystery', 'Classic',
 'A classic Golden Age Marvel mystery issue from Volume 1, Issue 12.',
 '/images/covers/Marvel_Mystery_Vol1_Iss12_COVER.jpeg',
 '/comics/Marvel_Mystery_Vol1_Iss12.pdf', false),

(7, 'Marvel Mystery', 1, 14, 'Marvel Mystery Vol. 1 Issue 14', 'Marvel Comics', 'Mystery', 'Adventure',
 'Another early Marvel Mystery issue with classic pulp-era storytelling and action.',
 '/images/covers/Marvel_Mystery_Vol1_Iss14_COVER.jpeg',
 '/comics/Marvel_Mystery_Vol1_Iss14.pdf', false),

(8, 'Marvel Mystery', 1, 15, 'Marvel Mystery Vol. 1 Issue 15', 'Marvel Comics', 'Mystery', 'Action',
 'A Golden Age Marvel issue featuring suspense, action, and vintage comic art.',
 '/images/covers/Marvel_Mystery_Vol1_Iss15_COVER.jpeg',
 '/comics/Marvel_Mystery_Vol1_Iss15.pdf', false),

(9, 'Mystic Comics', 1, 4, 'Mystic Comics Vol. 1 Issue 4', 'Marvel Comics', 'Fantasy', 'Mystery',
 'A classic Mystic Comics issue featuring supernatural themes and early Marvel storytelling.',
 '/images/covers/Mystic_Comics_Vol1_Iss4_COVER.jpeg',
 '/comics/Mystic_Comics_Vol1_Iss4.pdf', true);

INSERT INTO "UserComics" ("UserId", "ComicId", "Shelf", "ProgressPercent", "CurrentPage") VALUES
(1, 1, 'UpNext', 0, 1),
(1, 2, 'UpNext', 0, 1),
(1, 3, 'CurrentlyReading', 15, 1),
(1, 6, 'CurrentlyReading', 35, 1),
(1, 4, 'Trending', 0, 1),
(1, 5, 'Trending', 0, 1),
(1, 9, 'Trending', 0, 1),
(1, 7, 'UpNext', 0, 1),
(1, 8, 'UpNext', 0, 1);

INSERT INTO "CharacterImages" ("Id", "Alias", "ImagePath") VALUES
(1, 'Aquaman', '/images/dc-characters/aquaman.jpeg'),
(2, 'Batman', '/images/dc-characters/batman.jpeg'),
(3, 'Catwoman', '/images/dc-characters/catwoman.jpeg'),
(4, 'Constantine', '/images/dc-characters/constantine.jpeg'),
(5, 'Cyborg', '/images/dc-characters/cyborg.jpg'),
(6, 'The Flash', '/images/dc-characters/flash.jpeg'),
(7, 'Flash Wally West', '/images/dc-characters/flashwallywest.jpeg'),
(8, 'Green Arrow', '/images/dc-characters/greenarrow.jpeg'),
(9, 'Green Lantern', '/images/dc-characters/greenlantern.jpg'),
(10, 'Harley Quinn', '/images/dc-characters/harleyquinn.jpeg'),
(11, 'Joker', '/images/dc-characters/joker.jpeg'),
(12, 'Nightwing', '/images/dc-characters/nightwing.jpeg'),
(13, 'Poison Ivy', '/images/dc-characters/poisonivy.jpeg'),
(14, 'Shazam', '/images/dc-characters/shazam.jpeg'),
(15, 'Superman', '/images/dc-characters/superman.jpeg'),
(16, 'Wonder Woman', '/images/dc-characters/wonderwoman.jpg'),
(17, 'Zatanna', '/images/dc-characters/zatanna.jpeg'),
(18, 'Black Widow', '/images/marvel-characters/blackwidow.jpg'),
(19, 'Blue Blaze', '/images/marvel-characters/blueblaze.jpeg'),
(20, 'Dynamic Man', '/images/marvel-characters/dynamicman.jpg'),
(21, 'Electro', '/images/marvel-characters/electro.jpg'),
(22, 'Flexo the Rubber Man', '/images/marvel-characters/flexotherubberman.png'),
(23, 'Hercules', '/images/marvel-characters/hercules.jpg'),
(24, 'Human Torch', '/images/marvel-characters/humantorch.jpeg'),
(25, 'The Human Torch', '/images/marvel-characters/humantorch.jpeg'),
(26, 'Ka-Zar', '/images/marvel-characters/kazar.jpg'),
(27, 'Ka Zar', '/images/marvel-characters/kazar.jpg'),
(28, 'Marvel Boy', '/images/marvel-characters/marvelboy.jpg'),
(29, 'Terry Vance the Boy Detective', '/images/marvel-characters/terryvancetheboydetective.jpg'),
(30, 'The Angel', '/images/marvel-characters/theangel.jpg'),
(31, 'Angel', '/images/marvel-characters/theangel.jpg'),
(32, 'The Sub-Mariner', '/images/marvel-characters/thesubmariner.jpg'),
(33, 'Sub-Mariner', '/images/marvel-characters/thesubmariner.jpg'),
(34, 'Namor', '/images/marvel-characters/thesubmariner.jpg'),
(35, 'The Thin Man', '/images/marvel-characters/thethinman.jpg'),
(36, 'Thin Man', '/images/marvel-characters/thethinman.jpg');

INSERT INTO "MarvelCharacters" ("Id", "Name", "Alias", "Description", "ImagePath") VALUES
(1, 'Claire Voyant', 'Black Widow', 'A Golden Age Marvel character who becomes the supernatural Black Widow after death, acting as a grim vigilante tied to occult powers.',
 '/images/marvel-characters/blackwidow.jpg'),
(2, 'Spencer Keen', 'Blue Blaze', 'A costumed hero empowered by a mysterious blue flame, returning from suspended burial with superhuman strength and durability.',
 '/images/marvel-characters/blueblaze.jpeg'),
(3, 'Dynamic Man', 'Dynamic Man', 'An android created as an idealized man of the future who became a costumed hero and later a member of the World War II-era group known as the Twelve.',
 '/images/marvel-characters/dynamicman.jpg'),
(4, 'Electro', 'Electro', 'A powerful robot created by Professor Philo Zog and used in early Marvel stories to fight crime, corruption, disasters, and wartime threats.',
 '/images/marvel-characters/electro.jpg'),
(5, 'Flexo', 'Flexo the Rubber Man', 'A Golden Age robot-like hero made from living rubber, created by scientist brothers and used to fight crime.',
 '/images/marvel-characters/flexotherubberman.png'),
(6, 'Varen David', 'Hercules', 'A giant-sized Golden Age strongman raised in isolation to develop extraordinary physical and mental abilities.',
 '/images/marvel-characters/hercules.jpg'),
(7, 'Jim Hammond', 'Human Torch', 'An android created by Professor Phineas Horton who learned to control his flame powers and became one of Marvel''s earliest heroes.',
 '/images/marvel-characters/humantorch.jpeg'),
(8, 'Ka-Zar', 'Ka-Zar', 'A jungle adventurer associated with Marvel''s early pulp-inspired tradition of wilderness heroes and hidden-world adventure.',
 '/images/marvel-characters/kazar.jpg'),
(9, 'Robert Grayson', 'Marvel Boy', 'A space-raised hero associated with Uranian science and light-based powers, later remembered as one of several characters to use the Marvel Boy name.',
 '/images/marvel-characters/marvelboy.jpg'),
(10, 'Terry Vance', 'Terry Vance the Boy Detective', 'A brilliant teenage detective from Marvel''s Golden Age who solved crimes with sharp observation, ingenuity, and help from allies.',
 '/images/marvel-characters/terryvancetheboydetective.jpg'),
(11, 'Thomas Halloway', 'The Angel', 'A masked doctor and street-level vigilante from Marvel''s Golden Age who fought crime with skill, courage, and detective work.',
 '/images/marvel-characters/theangel.jpg'),
(12, 'Namor', 'The Sub-Mariner', 'The undersea prince of Atlantis and one of Marvel''s earliest heroes, known for his strength, flight, aquatic powers, and tense relationship with the surface world.',
 '/images/marvel-characters/thesubmariner.jpg'),
(13, 'Bruce Dickson', 'The Thin Man', 'A scientist-adventurer who discovered Kalahia and gained the ability to stretch and flatten his body into an extremely thin form.',
 '/images/marvel-characters/thethinman.jpg');
