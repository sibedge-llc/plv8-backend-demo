CREATE SCHEMA audit;

CREATE TABLE audit.write_log
(
    id SERIAL PRIMARY KEY NOT NULL,
    table_name TEXT NOT NULL,
	operation TEXT NOT NULL,
	account_id INTEGER NOT NULL,
	made TIMESTAMP WITH TIME ZONE NOT NULL,
	query JSONB,
	result JSONB
);
