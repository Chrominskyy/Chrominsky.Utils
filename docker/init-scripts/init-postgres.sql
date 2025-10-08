-- PostgreSQL initialization script for Chrominsky.Utils
-- This script creates sample tables using the library's base entities

-- Create a sample users table with full metadata tracking
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(255) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP,
    created_by VARCHAR(255),
    updated_by VARCHAR(255),
    status INTEGER NOT NULL DEFAULT 0
);

-- Create object versioning table for tracking changes
CREATE TABLE IF NOT EXISTS object_versions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    object_id UUID NOT NULL,
    object_type VARCHAR(255) NOT NULL,
    property_name VARCHAR(255) NOT NULL,
    old_value TEXT,
    new_value TEXT,
    changed_at TIMESTAMP NOT NULL DEFAULT NOW(),
    changed_by VARCHAR(255)
);

-- Create sample products table
CREATE TABLE IF NOT EXISTS products (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    price DECIMAL(18, 2) NOT NULL,
    stock_quantity INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP,
    created_by VARCHAR(255),
    updated_by VARCHAR(255),
    status INTEGER NOT NULL DEFAULT 0
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_users_status ON users(status);
CREATE INDEX IF NOT EXISTS idx_object_versions_object_id ON object_versions(object_id);
CREATE INDEX IF NOT EXISTS idx_products_status ON products(status);

-- Insert sample data
INSERT INTO users (id, username, email, password_hash, created_by, status)
VALUES 
    (gen_random_uuid(), 'admin', 'admin@example.com', '$2a$11$examplehashhere', 'system', 0),
    (gen_random_uuid(), 'testuser', 'test@example.com', '$2a$11$examplehashhere', 'system', 0)
ON CONFLICT (email) DO NOTHING;

INSERT INTO products (id, name, description, price, stock_quantity, created_by, status)
VALUES 
    (gen_random_uuid(), 'Sample Product 1', 'Description for product 1', 29.99, 100, 'system', 0),
    (gen_random_uuid(), 'Sample Product 2', 'Description for product 2', 49.99, 50, 'system', 0)
ON CONFLICT DO NOTHING;

COMMENT ON TABLE users IS 'Users table with BaseDatabaseEntity fields';
COMMENT ON TABLE object_versions IS 'Object versioning table for tracking entity changes';
COMMENT ON TABLE products IS 'Products table with BaseDatabaseEntity fields';
