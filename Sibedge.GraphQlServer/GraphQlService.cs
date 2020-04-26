namespace Sibedge.GraphQlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using Models;
    using Models.Introspection;
    using Newtonsoft.Json;
    using Type = Models.Introspection.Type;

    /// <summary> GraphQL service </summary>
    public class GraphQlService
    {
        private IDbConnection _connection;

        /// <summary> ctor </summary>
        public GraphQlService(IDbConnection connection)
        {
            _connection = connection;
        }

        /// <summary> Execute graphQL query </summary>
        /// <param name="query"> Query data </param>
        public async ValueTask<string> PerformQuery(GraphQlQuery query)
        {
            string json;

            if (query.OperationName == "IntrospectionQuery")
            {
                var schema = await this.GetIntrospectionData();

                var data = new
                {
                    data = new
                    {
                        __schema = schema
                    }
                };

                json = JsonConvert.SerializeObject(data);
            }
            else
            {
                var sql = $"SELECT graphql.execute('{query.Query}', 'public');";
                json = (await _connection.QueryAsync<string>(sql)).First();
            }

            return json;
        }

        private async ValueTask<IntrospectionSchema> GetIntrospectionData()
        {
            var result = new IntrospectionSchema
            {
                Directives = new List<object>(),
                MutationType = new NamedItem("Mutation"),
                SubscriptionType = new NamedItem("Subscription"),
                QueryType = new NamedItem("Query"),
                Types = await this.GetTypes()
            };

            return result;
        }

        private async ValueTask<List<Element>> GetTypes()
        {
            var ret = new List<Element>();
            var fieldInfoList = (await this.GetFieldInfo()).ToList();

            ret.Add(this.CreateNode(fieldInfoList));

            foreach (var dataType in fieldInfoList.Select(x => x.DataType).Distinct())
            {
                ret.Add(new Element
                {
                    Name = dataType.Replace(' ','_'),
                    Description = $"The '{dataType}' scalar type.",
                    Kind = Kinds.Scalar
                });
            }

            return ret;
        }

        private Task<IEnumerable<FieldInfo>> GetFieldInfo()
        {
            var sql = @"SELECT gc.table_name AS ""TableName"", gc.column_name AS ""ColumnName"",
                          ic.data_type AS ""DataType"", ic.is_nullable='YES' AS ""IsNullable"" FROM graphql.schema_columns gc
                        LEFT JOIN information_schema.columns ic ON gc.table_name=ic.table_name AND gc.column_name=ic.column_name
                          WHERE ic.table_schema::name = 'public'::name;";

            return _connection.QueryAsync<FieldInfo>(sql);
        }

        private Element CreateNode(List<FieldInfo> fieldInfoList)
        {
            var ret = new Element
            {
                Name = "Node",
                Description = "An object with an ID",
                Fields = new List<Field>
                {
                    new Field
                    {
                        Name = "id",
                        Description = "The id of the object.",
                        Type = Type.CreateNonNull(Kinds.Scalar, "Id")
                    }
                },
                Kind = Kinds.Interface,
                PossibleTypes = new List<Type>()
            };

            foreach (var tableName in fieldInfoList.Select(x => x.TableName).Distinct())
            {
                ret.PossibleTypes.Add(new Type(Kinds.Object, tableName));
            }

            return ret;
        }
    }
}
