FROM postgres:14.2-alpine AS installer

RUN buildDependencies="ca-certificates make git bison build-base flex clang clang-dev llvm12 llvm12-libs" \
  && apk add --update ${buildDependencies}
WORKDIR /usr/build
RUN git clone https://github.com/sibedge-llc/plv8-build.git \
  && cd plv8-build && git checkout pg14-3.0.0-alpine
RUN git clone https://github.com/postgrespro/jsquery.git \
  && cd jsquery && make USE_PGXS=1 && make USE_PGXS=1 install
RUN apk del ${buildDependencies}


FROM postgres:14.2-alpine
ARG PLV8_VERSION=3.0.0

RUN apk add --update libstdc++
COPY --from=installer /usr/local/lib/postgresql/jsquery.so /usr/local/lib/postgresql
COPY --from=installer /usr/local/share/postgresql/extension/jsquery--1.0--1.1.sql /usr/local/share/postgresql/extension
COPY --from=installer /usr/local/share/postgresql/extension/jsquery--1.1.sql /usr/local/share/postgresql/extension
COPY --from=installer /usr/local/share/postgresql/extension/jsquery.control /usr/local/share/postgresql/extension
COPY --from=installer /usr/build/plv8-build/extension/ /usr/local/share/postgresql/extension/
COPY --from=installer /usr/build/plv8-build/pkglibdir/plv8-${PLV8_VERSION}.so /usr/local/lib/postgresql/plv8-${PLV8_VERSION}.so
