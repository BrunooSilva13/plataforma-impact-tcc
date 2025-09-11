-- Criar o banco de dados se não existir
DO $$
BEGIN
   IF NOT EXISTS (SELECT FROM pg_database WHERE datname = 'clientdb') THEN
      CREATE DATABASE clientdb;
   END IF;
END
$$;

\c clientdb;

-- Criar a tabela de clientes se não existir
CREATE TABLE IF NOT EXISTS clients (
    id UUID PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    surname VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    birthdate DATE NOT NULL,
    created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITHOUT TIME ZONE DEFAULT NOW(),
    isactive BOOLEAN DEFAULT TRUE
);
