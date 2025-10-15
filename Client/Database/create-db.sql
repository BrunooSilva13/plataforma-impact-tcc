-- Habilita a extensão necessária para gerar UUID
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Criação da tabela clients com UUID como ID e novos campos
CREATE TABLE IF NOT EXISTS clients (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    surname VARCHAR(100) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    birthdate DATE NOT NULL,
    isactive BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);
