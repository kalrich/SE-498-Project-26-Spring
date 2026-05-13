DROP TABLE IF EXISTS "Checkouts";
DROP TABLE IF EXISTS "ComicReviews";
DROP TABLE IF EXISTS "ReadingHistories";
DROP TABLE IF EXISTS "FavoriteComics";
DROP TABLE IF EXISTS "UserComics";
DROP TABLE IF EXISTS "MarvelCharacters";
DROP TABLE IF EXISTS "CharacterImages";
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

CREATE TABLE "CharacterImages" (
    "Id" SERIAL PRIMARY KEY,
    "Alias" TEXT NOT NULL,
    "ImagePath" TEXT NOT NULL
);

CREATE TABLE "MarvelCharacters" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Alias" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "ImagePath" TEXT NOT NULL
);

CREATE TABLE "UserComics" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "ComicId" INTEGER NOT NULL,
    "Shelf" TEXT NOT NULL,
    "ProgressPercent" INTEGER NOT NULL DEFAULT 0,
    "CurrentPage" INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("ComicId") REFERENCES "Comics"("Id") ON DELETE CASCADE,
    UNIQUE("UserId", "ComicId")
);

CREATE TABLE "Checkouts" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "ComicId" INTEGER NOT NULL,
    "CheckoutDate" TIMESTAMPTZ NOT NULL,
    "DueDate" TIMESTAMPTZ NOT NULL,
    "ReturnDate" TIMESTAMPTZ NULL,
    "Status" TEXT NOT NULL DEFAULT 'Active',
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("ComicId") REFERENCES "Comics"("Id") ON DELETE CASCADE
);

CREATE TABLE "FavoriteComics" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "ComicId" INTEGER NOT NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("ComicId") REFERENCES "Comics"("Id") ON DELETE CASCADE,
    UNIQUE("UserId", "ComicId")
);

CREATE TABLE "ReadingHistories" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "ComicId" INTEGER NOT NULL,
    "CurrentPage" INTEGER NOT NULL DEFAULT 1,
    "ProgressPercent" INTEGER NOT NULL DEFAULT 0,
    "LastReadAt" TIMESTAMPTZ NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("ComicId") REFERENCES "Comics"("Id") ON DELETE CASCADE,
    UNIQUE("UserId", "ComicId")
);

CREATE TABLE "ComicReviews" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "ComicId" INTEGER NOT NULL,
    "Rating" INTEGER NOT NULL,
    "Comment" TEXT NOT NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL,
    "UpdatedAt" TIMESTAMPTZ NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("ComicId") REFERENCES "Comics"("Id") ON DELETE CASCADE,
    UNIQUE("UserId", "ComicId")
);

CREATE INDEX "IX_Checkouts_UserId" ON "Checkouts" ("UserId");
CREATE INDEX "IX_Checkouts_ComicId" ON "Checkouts" ("ComicId");
CREATE INDEX "IX_FavoriteComics_ComicId" ON "FavoriteComics" ("ComicId");
CREATE INDEX "IX_ReadingHistories_ComicId" ON "ReadingHistories" ("ComicId");
CREATE INDEX "IX_ComicReviews_ComicId" ON "ComicReviews" ("ComicId");
