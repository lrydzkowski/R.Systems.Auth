START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220123170109_add_test_table') THEN
    DROP TABLE "user".test;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220123170109_add_test_table') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20220123170109_add_test_table';
    END IF;
END $EF$;
COMMIT;

