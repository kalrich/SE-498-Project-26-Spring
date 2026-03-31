-- CLEAR OLD DATA (optional but clutch for reruns)
DELETE FROM UserComics;
DELETE FROM Users;
DELETE FROM Comics;

-- USERS
INSERT INTO Users (Username, Password) VALUES
('josh', 'password'),
('demo', 'password');

-- COMICS / BOOKS
INSERT INTO Comics (Title, Author, Description, ImageUrl) VALUES
('Spider-Man: Blue', 'Jeph Loeb', 'A nostalgic Spider-Man story about love and loss.', ''),
('Batman: Year One', 'Frank Miller', 'The origin story of Batman and Jim Gordon.', ''),
('Dune', 'Frank Herbert', 'A sci-fi epic about politics, religion, and power.', ''),
('Percy Jackson', 'Rick Riordan', 'Greek mythology meets modern adventure.', '');

-- USER SHELVES
INSERT INTO UserComics (UserId, ComicId, Shelf) VALUES
(1, 1, 'Reading'),
(1, 2, 'To Read'),
(1, 3, 'Finished'),
(2, 4, 'Reading');