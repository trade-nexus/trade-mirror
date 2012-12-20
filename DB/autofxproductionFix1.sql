ALTER TABLE
    autofxproduction.signals DROP COLUMN ticket;
ALTER TABLE
    autofxproduction.signals DROP COLUMN groupid;
ALTER TABLE
    autofxproduction.signals DROP COLUMN closed;
ALTER TABLE
    autofxproduction.signals CHANGE created `processing time` DECIMAL
DROP TABLE
    `autofxproduction`.`signal_archives`
ALTER TABLE
    autofxproduction.users DROP COLUMN username;
ALTER TABLE
    autofxproduction.users DROP COLUMN password;
ALTER TABLE
    autofxproduction.users DROP COLUMN groupid