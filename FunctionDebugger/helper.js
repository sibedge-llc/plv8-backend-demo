exports.query = ` query {
    CategoryFamilies (take: 10) {
        Id
        CurrentVersion {
            Id
            Version
        }
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
    
    client.connectSync('postgresql://fm:1q2w3e$R@192.168.33.140:5432/FmStage');
    var ret = client.querySync(query);
    client.end();

    return ret;
}
