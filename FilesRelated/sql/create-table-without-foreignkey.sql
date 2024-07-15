CREATE TABLE ITEM_CATEGORIES (
    CATEGORY_ID INT PRIMARY KEY,
    CATEGORY_NAME VARCHAR(20) NOT NULL
);

CREATE TABLE ITEM_TAGS (
    TAG_ID INT PRIMARY KEY,
    TAG_NAME VARCHAR(20) NOT NULL
);

CREATE TABLE REVIEWCODES (
    REVIEW_ID INT PRIMARY KEY,
    REVIEW_INFO VARCHAR(20) NOT NULL
);

CREATE TABLE ADVERTISEMENTS (
    AD_ID INT PRIMARY KEY,
    AD_CONTENT VARCHAR(255),
    AD_PICTURE BLOB,
    AD_URL VARCHAR(255),
    AD_TYPE VARCHAR(20),
    START_TIME TIMESTAMP,
    END_TIME TIMESTAMP
);

CREATE TABLE SECURITY_EVENTS (
    ENENT_ID INT PRIMARY KEY,
    EVENT_TYPE VARCHAR(20) NOT NULL,
    EVENT_DETAILS VARCHAR(255),
    STATUS VARCHAR(20),
    OCCURRENCE_TIME TIMESTAMP
);

CREATE TABLE USERS (
    USER_ID INT PRIMARY KEY,
    USER_NAME VARCHAR(20) NOT NULL,
    PASSWORD_ VARCHAR(20) NOT NULL,
    CONTACT VARCHAR(20)
);

ALTER TABLE USERS RENAME TO USERS_BACKUP;

CREATE TABLE USERS (
    USER_ID INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    USER_NAME VARCHAR(20) NOT NULL,
    PASSWORD_ VARCHAR(20) NOT NULL,
    CONTACT VARCHAR(20)
);

ALTER TABLE USERS
ADD CONSTRAINT USERS_USER_NAME_UNIQUE UNIQUE (USER_NAME);

