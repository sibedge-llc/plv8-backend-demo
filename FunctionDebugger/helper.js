exports.query = ` query {
    Contractors {
        Id
        Name
        Comment
        EventsAgg {
          count
          distinctName
        }
      }
      EventsAgg {
         count
         distinctName
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
    
    client.connectSync('postgresql://dev:R7i{Rht*POeSkdh@192.168.33.140:5432/hcapital');
    var ret = client.querySync(query);
    client.end();

    return ret;
}
