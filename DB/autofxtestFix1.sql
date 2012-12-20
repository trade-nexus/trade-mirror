ALTER TABLE
    autofxtest.signals DROP COLUMN ticket;
ALTER TABLE
    autofxtest.signals DROP COLUMN groupid;
ALTER TABLE
    autofxtest.signals DROP COLUMN closed;
ALTER TABLE
    autofxtest.signals CHANGE created `processing time` DECIMAL
DROP TABLE
    `autofxtest`.`signal_archives`
ALTER TABLE
    autofxtest.users DROP COLUMN username;
ALTER TABLE
    autofxtest.users DROP COLUMN password;
ALTER TABLE
    autofxtest.users DROP COLUMN groupid