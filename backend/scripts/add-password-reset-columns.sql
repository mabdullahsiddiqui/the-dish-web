ALTER TABLE users."Users" ADD COLUMN IF NOT EXISTS "PasswordResetCode" character varying(10);
ALTER TABLE users."Users" ADD COLUMN IF NOT EXISTS "PasswordResetCodeExpiry" timestamp with time zone;

