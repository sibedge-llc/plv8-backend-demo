exports.query = ` query {
    Families {
        Id
    }
}
`;
exports.schema = '';

exports.elog = function(type, message)
{
    console.log(message);
}

exports.execute = function(query)
{
    return [];
}

