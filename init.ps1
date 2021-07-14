$tmp = "dump-tmp.zip"
Invoke-WebRequest -OutFile $tmp http://graphql.alexfadeev.net/scripts/demo-medium-20170815.zip
Expand-Archive -Path $tmp -DestinationPath .\db\node\dump -Force
$tmp | Remove-Item

git submodule update --init
