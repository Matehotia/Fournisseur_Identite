-- Configurations

DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_database WHERE datname = 'identity_provider') THEN
        CREATE DATABASE identity_provider;
    END IF;
END $$;

-- CREATE USER web with password 'web';
-- ALTER DATABASE identity_provider OWNER TO web;

\c identity_provider
