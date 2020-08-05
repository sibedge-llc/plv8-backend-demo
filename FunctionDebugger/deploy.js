var db = require ("./helper.js");
var fs = require('fs');

fs.readFile('./createFunction.js', 'utf8', function(err, data)
{
    var beginMark = "/*BEGIN*/";
    var sqlOpenMark = "/*SQL";
    var sqlCloseMark = "SQL*/";

    var header = data.substr(0, data.indexOf(beginMark));

    var scriptHeader = header.substr(header.indexOf(sqlOpenMark) + sqlOpenMark.length);
    scriptHeader = scriptHeader.substr(0, scriptHeader.indexOf(sqlCloseMark)) + "AS $$";

    fs.readFile('./api.js', 'utf8', function(err, apiData)
    {
        var scriptApi = apiData.replace("exports.", "api.");

        var scriptBody = data.substr(data.indexOf(beginMark) + beginMark.length)
            .replace("exports.ret =", "return");

        var script = `${scriptHeader}
${scriptApi}
${scriptBody}
$$ LANGUAGE plv8;`;

        var result = db.execute(script);
        console.log(result);
    });
});
