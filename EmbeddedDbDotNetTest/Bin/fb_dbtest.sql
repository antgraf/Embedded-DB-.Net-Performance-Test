/******************************************************************************/
/*                                 Generators                                 */
/******************************************************************************/

CREATE GENERATOR GEN_COMMENTS_ID;
SET GENERATOR GEN_COMMENTS_ID TO 0;

CREATE GENERATOR GEN_MAIN_ID;
SET GENERATOR GEN_MAIN_ID TO 0;

CREATE GENERATOR GEN_TAGS_ID;
SET GENERATOR GEN_TAGS_ID TO 0;


/******************************************************************************/
/*                                   Tables                                   */
/******************************************************************************/


CREATE TABLE COMMENTS (
    ID    INTEGER NOT NULL,
    BODY  BLOB SUB_TYPE 1 SEGMENT SIZE 10240
);


CREATE TABLE MAIN (
    ID          INTEGER NOT NULL,
    A           INTEGER DEFAULT 999 NOT NULL,
    B           SMALLINT DEFAULT 1 NOT NULL,
    C           CHAR(1) DEFAULT 'D' NOT NULL,
    D           VARCHAR(64) DEFAULT 'BCD000' NOT NULL,
    E           INTEGER,
    F           SMALLINT,
    G           CHAR(1),
    H           VARCHAR(64),
    COMMENT_ID  INTEGER
);


CREATE TABLE TAGS (
    ID          INTEGER,
    NAME        VARCHAR(32) DEFAULT '?' NOT NULL,
    COMMENT_ID  INTEGER NOT NULL
);



/******************************************************************************/
/*                                Primary Keys                                */
/******************************************************************************/

ALTER TABLE COMMENTS ADD CONSTRAINT PK_COMMENTS PRIMARY KEY (ID);
ALTER TABLE MAIN ADD CONSTRAINT PK_MAIN PRIMARY KEY (ID);


/******************************************************************************/
/*                                  Indices                                   */
/******************************************************************************/

CREATE INDEX "_IDX1" ON MAIN (COMMENT_ID);
CREATE INDEX "_IDX2" ON TAGS (COMMENT_ID);


/******************************************************************************/
/*                                  Triggers                                  */
/******************************************************************************/


SET TERM ^ ;


/******************************************************************************/
/*                            Triggers for tables                             */
/******************************************************************************/


/* Trigger: COMMENTS_BI */
CREATE TRIGGER COMMENTS_BI FOR COMMENTS
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (new.id is null) then
    new.id = gen_id(GEN_COMMENTS_ID,1);
end
^

/* Trigger: MAIN_BI */
CREATE TRIGGER MAIN_BI FOR MAIN
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (new.id is null) then
    new.id = gen_id(GEN_MAIN_ID,1);
end
^


/* Trigger: TAGS_BI */
CREATE TRIGGER TAGS_BI FOR TAGS
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (new.id is null) then
    new.id = gen_id(GEN_TAGS_ID,1);
end
^


SET TERM ; ^

