CREATE TABLE `comments`
(
       id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
       body TEXT
);
CREATE TABLE `main`
(
       id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
       a INTEGER NOT NULL DEFAULT "999" ,
       b INTEGER NOT NULL DEFAULT "1" ,
       c CHAR(1) NOT NULL DEFAULT "D" ,
       d VARCHAR(64) NOT NULL DEFAULT "BCD000" ,
       e INTEGER,
       f INTEGER,
       g CHAR(1),
       h VARCHAR(64),
       comment_id INTEGER
);
CREATE TABLE `tags`
(
       id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
       name VARCHAR(32) NOT NULL DEFAULT "?" ,
       comment_id INTEGER
);

CREATE INDEX main_cooment
 ON `main` (comment_id);

CREATE INDEX tag_comment
 ON `tags` (comment_id);
