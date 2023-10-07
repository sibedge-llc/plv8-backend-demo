$tmp = "dump-tmp.zip"
Invoke-WebRequest -OutFile $tmp http://passgen.alexfadeev.ru/scripts/demo-medium-20170815.zip
Expand-Archive -Path $tmp -DestinationPath .\db\node\dump -Force
$tmp | Remove-Item

git submodule update --init

Copy-Item "./db/connectionString.js" -Destination "./Plv8/Plv8/config"

docker-compose build --no-cache

docker-compose up -d
