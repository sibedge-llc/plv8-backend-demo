wget http://passgen.alexfadeev.ml/scripts/demo-medium-20170815.zip

unzip ./demo-medium-20170815.zip -d ./db/node/dump

rm ./demo-medium-20170815.zip

git submodule update --init

cp ./db/connectionString.js ./Plv8/Plv8/config/

docker-compose build

docker-compose up -d
