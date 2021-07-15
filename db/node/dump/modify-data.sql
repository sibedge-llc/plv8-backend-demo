WITH s1 AS MATERIALIZED (
SELECT passenger_name, (array_agg(passenger_id))[1] AS passenger_id FROM tickets
  GROUP BY passenger_name)
UPDATE tickets t
  SET passenger_id = s1.passenger_id
  FROM s1
  WHERE t.passenger_name=s1.passenger_name;

CREATE VIEW public.passengers AS
  SELECT passenger_id AS id, passenger_name AS name
  FROM tickets;

INSERT INTO graphql.additional_foreign_keys(
            column_name, foreign_table_name, foreign_column_name, table_name)
    VALUES ('passenger_id', 'passengers', 'id', 'tickets');

REFRESH MATERIALIZED VIEW graphql.schema_foreign_keys;
REFRESH MATERIALIZED VIEW graphql.schema_columns;
