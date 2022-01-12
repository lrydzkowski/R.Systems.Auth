START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220110172146_incorrect_sign_in_verification') THEN
    ALTER TABLE "user"."user" DROP COLUMN last_incorrect_sign_in_date_time_utc;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220110172146_incorrect_sign_in_verification') THEN
    ALTER TABLE "user"."user" DROP COLUMN num_of_incorrect_sign_in;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220110172146_incorrect_sign_in_verification') THEN
    ALTER TABLE "user"."user" ALTER COLUMN refresh_token_expire_date_time_utc TYPE timestamp without time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220110172146_incorrect_sign_in_verification') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20220110172146_incorrect_sign_in_verification';
    END IF;
END $EF$;
COMMIT;

