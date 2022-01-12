START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220110172146_incorrect_sign_in_verification') THEN
    ALTER TABLE "user"."user" ALTER COLUMN refresh_token_expire_date_time_utc TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220110172146_incorrect_sign_in_verification') THEN
    ALTER TABLE "user"."user" ADD last_incorrect_sign_in_date_time_utc timestamp with time zone NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220110172146_incorrect_sign_in_verification') THEN
    ALTER TABLE "user"."user" ADD num_of_incorrect_sign_in integer NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220110172146_incorrect_sign_in_verification') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20220110172146_incorrect_sign_in_verification', '6.0.0');
    END IF;
END $EF$;
COMMIT;

