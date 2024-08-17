ALTER TABLE NOTIFICATION_LOGS DROP CONSTRAINT NOTIFICATION_LOGS_PK;

ALTER TABLE NOTIFICATION_LOGS DROP COLUMN NOTIFICATION_ID;

ALTER TABLE NOTIFICATION_LOGS
    ADD NOTIFICATION_LOG_ID INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY;

ALTER TABLE NOTIFICATION_LOGS 
    ADD CONSTRAINT FK_NOTIFICATION_LOGS_USER_ID FOREIGN KEY (USER_ID) REFERENCES USERS(USER_ID);

ALTER TABLE NOTIFICATION_LOGS RENAME COLUMN NOTIFICATION_LOG_ID TO NOTIFICATION_ID;
