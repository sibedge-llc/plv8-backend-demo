FROM node:12.22.1-buster-slim
WORKDIR /usr/deploy
RUN buildDependencies="ca-certificates make git build-essential python3 libpq-dev postgresql-client" \
  && apt-get update && apt-get install -y --no-install-recommends ${buildDependencies} \
  && git clone https://github.com/sibedge-llc/plv8-framework.git
RUN cd plv8-framework/Plv8 \
  && npm install -g node-gyp \
  && npm install -g npm \
  && npm install
ADD dump/${DUMP_FILE} ./plv8-framework/Sql
ADD dump/modify-data.sql ./plv8-framework/Sql
CMD cd plv8-framework \
  && PGPASSWORD=${ADMIN_PASSWORD} psql -h ${PLV8_POSTGRES_HOST} -p ${PLV8_POSTGRES_PORT} -U ${ADMIN_USER} -a -c "CREATE USER ${PLV8_POSTGRES_USER} WITH PASSWORD '${PLV8_POSTGRES_PASSWORD}';" \
  && PGPASSWORD=${ADMIN_PASSWORD} psql -h ${PLV8_POSTGRES_HOST} -p ${PLV8_POSTGRES_PORT} -U ${ADMIN_USER} -a -c "CREATE DATABASE ${PLV8_DB_NAME}" \
  && PGPASSWORD=${ADMIN_PASSWORD} psql -h ${PLV8_POSTGRES_HOST} -p ${PLV8_POSTGRES_PORT} -U ${ADMIN_USER} -a -c "GRANT ALL PRIVILEGES ON DATABASE ${PLV8_DB_NAME} TO ${PLV8_POSTGRES_USER};" \
  && PGPASSWORD=${ADMIN_PASSWORD} psql -h ${PLV8_POSTGRES_HOST} -p ${PLV8_POSTGRES_PORT} -U ${ADMIN_USER} -d ${PLV8_DB_NAME} -a -c "ALTER SCHEMA public OWNER TO ${PLV8_POSTGRES_USER}" \
  && PGPASSWORD=${ADMIN_PASSWORD} psql -h ${PLV8_POSTGRES_HOST} -p ${PLV8_POSTGRES_PORT} -U ${ADMIN_USER} -d ${PLV8_DB_NAME} -a -c "CREATE EXTENSION plv8;" \
  && PGPASSWORD=${ADMIN_PASSWORD} psql -h ${PLV8_POSTGRES_HOST} -p ${PLV8_POSTGRES_PORT} -U ${ADMIN_USER} -d ${PLV8_DB_NAME} -a -c "CREATE EXTENSION jsquery;" \
  && PGPASSWORD=${PLV8_POSTGRES_PASSWORD} psql -h ${PLV8_POSTGRES_HOST} -p ${PLV8_POSTGRES_PORT} -U ${PLV8_POSTGRES_USER} -d ${PLV8_DB_NAME} -a -f ./Sql/${DUMP_FILE} \  
  && PGPASSWORD=${ADMIN_PASSWORD} psql -h ${PLV8_POSTGRES_HOST} -p ${PLV8_POSTGRES_PORT} -U ${ADMIN_USER} -d ${PLV8_DB_NAME} -a -c "SET search_path = bookings, pg_catalog;" \  
  && PGPASSWORD=${ADMIN_PASSWORD} psql -h ${PLV8_POSTGRES_HOST} -p ${PLV8_POSTGRES_PORT} -U ${ADMIN_USER} -a -c "ALTER DATABASE airlines SET search_path = bookings, public;" \  
  && PGPASSWORD=${ADMIN_PASSWORD} psql -h ${PLV8_POSTGRES_HOST} -p ${PLV8_POSTGRES_PORT} -U ${ADMIN_USER} -a -c "ALTER DATABASE airlines SET bookings.lang = ru;" \  
  && PGPASSWORD=${PLV8_POSTGRES_PASSWORD} psql -h ${PLV8_POSTGRES_HOST} -p ${PLV8_POSTGRES_PORT} -U ${PLV8_POSTGRES_USER} -d ${PLV8_DB_NAME} -a -f ./Sql/SchemaSetup.sql \
  && PGPASSWORD=${PLV8_POSTGRES_PASSWORD} psql -h ${PLV8_POSTGRES_HOST} -p ${PLV8_POSTGRES_PORT} -U ${PLV8_POSTGRES_USER} -d ${PLV8_DB_NAME} -a -f ./Sql/GrapqQlCreate.sql \
  && PGPASSWORD=${PLV8_POSTGRES_PASSWORD} psql -h ${PLV8_POSTGRES_HOST} -p ${PLV8_POSTGRES_PORT} -U ${PLV8_POSTGRES_USER} -d ${PLV8_DB_NAME} -a -f ./Sql/modify-data.sql \
  && cd Plv8 \
  && npm run deploy
