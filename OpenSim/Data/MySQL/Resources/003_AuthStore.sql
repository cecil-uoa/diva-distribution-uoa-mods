BEGIN;

ALTER TABLE `auth` ADD COLUMN `accountType` VARCHAR(32) NOT NULL DEFAULT 'UserAccount';

COMMIT;