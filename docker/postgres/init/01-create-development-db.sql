SELECT 'CREATE DATABASE wholesale_platform_dev'
WHERE NOT EXISTS (
    SELECT FROM pg_database WHERE datname = 'wholesale_platform_dev'
)\gexec
