DROP TABLE IF EXISTS "UserComics";
DROP TABLE IF EXISTS "Users";
DROP TABLE IF EXISTS "Comics";

CREATE TABLE "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Username" TEXT NOT NULL,
    "Email" TEXT NOT NULL UNIQUE,
    "Password" TEXT NOT NULL
);

CREATE TABLE "Comics" (
    "Id" SERIAL PRIMARY KEY,
    "SeriesName" TEXT NOT NULL,
    "VolumeNumber" INTEGER NOT NULL DEFAULT 1,
    "IssueNumber" INTEGER NOT NULL,
    "Title" TEXT NOT NULL,
    "Author" TEXT NOT NULL,
    "Genre" TEXT NOT NULL,
    "SecondaryGenre" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "CoverImagePath" TEXT NOT NULL,
    "PdfPath" TEXT NOT NULL,
    "IsIReadPick" BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE "UserComics" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "ComicId" INTEGER NOT NULL,
    "Shelf" TEXT NOT NULL,
    "ProgressPercent" INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("ComicId") REFERENCES "Comics"("Id") ON DELETE CASCADE,
    UNIQUE("UserId", "ComicId")
);