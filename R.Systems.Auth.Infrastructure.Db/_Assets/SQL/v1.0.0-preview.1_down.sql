START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211216190049_add_user_email_unique_constraint') THEN
    DROP INDEX "user"."IX_user_email";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211216190049_add_user_email_unique_constraint') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20211216190049_add_user_email_unique_constraint';
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211205110211_unify_id_columns') THEN
    ALTER TABLE "user"."user" RENAME COLUMN id TO user_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211205110211_unify_id_columns') THEN
    ALTER TABLE "user".role RENAME COLUMN id TO role_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211205110211_unify_id_columns') THEN
    ALTER TABLE "user"."user" ALTER COLUMN user_id RESTART WITH 1;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211205110211_unify_id_columns') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20211205110211_unify_id_columns';
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211123183511_add_refresh_token_cols') THEN
    ALTER TABLE "user"."user" DROP COLUMN refresh_token;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211123183511_add_refresh_token_cols') THEN
    ALTER TABLE "user"."user" DROP COLUMN refresh_token_expire_date_time_utc;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211123183511_add_refresh_token_cols') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20211123183511_add_refresh_token_cols';
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211101143612_add_user_role') THEN
    DELETE FROM "user".role
    WHERE role_id = 2;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211101143612_add_user_role') THEN
    ALTER TABLE "user".role ALTER COLUMN role_id RESTART WITH 1;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211101143612_add_user_role') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20211101143612_add_user_role';
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211023100719_init_db') THEN
    DROP TABLE "user".user_role;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211023100719_init_db') THEN
    DROP TABLE "user".role;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211023100719_init_db') THEN
    DROP TABLE "user"."user";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20211023100719_init_db') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20211023100719_init_db';
    END IF;
END $EF$;
COMMIT;

