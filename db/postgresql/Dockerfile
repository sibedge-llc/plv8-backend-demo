FROM postgres:13.4 AS installer

RUN buildDependencies="ca-certificates make git postgresql-server-dev-13 bison build-essential flex" \
  && apt-get update && apt-get install -y --no-install-recommends ${buildDependencies}

WORKDIR /usr/build
RUN git clone https://github.com/postgrespro/jsquery.git
RUN cd jsquery && make USE_PGXS=1 && make USE_PGXS=1 install


FROM sibedge/postgres-plv8:13.4-3.0.0

ENV PG_MAJOR_VERSION=13
COPY --from=installer /usr/lib/postgresql/${PG_MAJOR_VERSION}/lib/jsquery.so /usr/lib/postgresql/${PG_MAJOR_VERSION}/lib
COPY --from=installer /usr/share/postgresql/${PG_MAJOR_VERSION}/extension/jsquery--1.0--1.1.sql /usr/share/postgresql/${PG_MAJOR_VERSION}/extension
COPY --from=installer /usr/share/postgresql/${PG_MAJOR_VERSION}/extension/jsquery--1.1.sql /usr/share/postgresql/${PG_MAJOR_VERSION}/extension
COPY --from=installer /usr/share/postgresql/${PG_MAJOR_VERSION}/extension/jsquery.control /usr/share/postgresql/${PG_MAJOR_VERSION}/extension
