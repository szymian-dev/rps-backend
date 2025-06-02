DO
$$
BEGIN
   IF NOT EXISTS (
      SELECT FROM pg_catalog.pg_roles WHERE rolname = 'RPSuser'
   ) THEN
      CREATE ROLE RPSuser WITH LOGIN PASSWORD 'rps1234';
      ALTER ROLE RPSuser CREATEDB; 
   END IF;

   IF NOT EXISTS (
      SELECT FROM pg_database WHERE datname = 'RPS'
   ) THEN

      CREATE DATABASE "RPS" OWNER RPSuser;
   END IF;
END
$$;