exports.query = ` query {
    Families {
        Id
        Name
      }
  }
`;

exports.schema = 'public';

exports.elog = function(type, message)
{
    console.log(message);
}

exports.execute = function(query)
{
    var Client = require('pg-native');
    var client = new Client();
    
    client.connectSync('postgresql://alexey:1@localhost:5432/fm');
    var ret = client.querySync(query);
    client.end();

    return ret;
}
