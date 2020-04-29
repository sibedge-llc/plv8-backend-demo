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
        private static string[] FilterOperators = new [] { "less", "greater", "equals", "contains" };
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
            var foreignKeyInfoList = (await this.GetForeignKeyInfo()).ToList();

            ret.Add(this.CreateNode(fieldInfoList));
            ret.Add(this.CreateQuery(fieldInfoList));
            ret.AddRange(this.CreateTables(fieldInfoList, foreignKeyInfoList));
            ret.AddRange(this.CreateFilters(fieldInfoList));

            // Data types
            ret.Add(new Element
            {
                Name = "Id",
                Description = "The `Id` scalar type represents a unique identifier.",
                Kind = Kinds.Scalar
            });

            foreach (var dataType in fieldInfoList.Select(x => x.DataType).Distinct())
            {
                ret.Add(new Element
                {
                    Name = dataType.ToTypeName(),
                    Description = $"The '{dataType}' scalar type.",
                    Kind = Kinds.Scalar
                });
            }

            // Mutation, subscription
            ret.Add(new Element
            {
                Name = "Mutation",
                Interfaces = new List<Type>(),
                Fields = new List<Field>(),
                Kind = Kinds.Object
            });

            ret.Add(new Element
            {
                Name = "Subscription",
                Interfaces = new List<Type>(),
                Fields = new List<Field>(),
                Kind = Kinds.Object
            });

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

        private Task<IEnumerable<ForeignKeyInfo>> GetForeignKeyInfo()
        {
            var sql = @"SELECT table_name AS ""TableName"", column_name AS ""ColumnName"",
                          foreign_table_name AS ""ForeignTableName"", foreign_column_name AS ""ForeignColumnName""
                        FROM graphql.schema_foreign_keys";

            return _connection.QueryAsync<ForeignKeyInfo>(sql);
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

        private Element CreateQuery(List<FieldInfo> fieldInfoList)
        {
            var ret = new Element
            {
                Name = "Query",
                Interfaces = new List<Type>(),
                Kind = Kinds.Object,
                Fields = new List<Field>()
            };

            foreach (var tableName in fieldInfoList.Select(x => x.TableName).Distinct())
            {
                ret.Fields.Add(new Field
                {
                    Name = tableName,
                    Type = Type.CreateNonNullList(Kinds.Object, tableName),
                    Args = new List<InputField>
                    {
                        new InputField
                        {
                            Name = "id",
                            Type = new Type(Kinds.InputObject, "IdFilter")
                        },
                        new InputField
                        {
                            Name = "filter",
                            Type = new Type(Kinds.InputObject, $"{tableName}Filter")
                        },
                        new InputField
                        {
                            Name = "orderBy",
                            Type = new Type(Kinds.Enum, $"{tableName}OrderBy")
                        },
                        new InputField
                        {
                            Name = "orderByDescending",
                            Type = new Type(Kinds.Enum, $"{tableName}OrderByDescending")
                        },                        new InputField
                        {
                            Name = "skip",
                            Type = new Type(Kinds.InputObject, "Skip")
                        },
                        new InputField
                        {
                            Name = "take",
                            Type = new Type(Kinds.InputObject, "Take")
                        }
                    }
                });
            }

            return ret;
        }

        private List<Element> CreateTables(List<FieldInfo> fieldInfoList, List<ForeignKeyInfo> foreignKeyList)
        {
            var ret = new List<Element>();

            var tables = fieldInfoList.GroupBy(x => x.TableName);

            foreach (var table in tables)
            {
                var element = new Element
                {
                    Name = table.Key,
                    Description = table.Key,
                    Interfaces = new List<Type> { new Type(Kinds.Interface, "Node") },
                    Kind = Kinds.Object,
                    Fields = new List<Field>()
                };

                foreach (var column in table)
                {
                    var dataTypeName = column.ColumnName.ToLowerInvariant() == "id"
                        ? "Id"
                        : column.DataType.ToTypeName();

                    element.Fields.Add(new Field
                    {
                        Name = column.ColumnName,
                        Type = column.IsNullable
                            ? new Type(Kinds.Scalar, dataTypeName)
                            : Type.CreateNonNull(Kinds.Scalar, dataTypeName)
                    });
                }

                var singleLinks = foreignKeyList.Where(x => x.TableName == table.Key);
                foreach (var singleLink in singleLinks)
                {
                    element.Fields.Add(new Field
                    {
                        Name = singleLink.ColumnName.EndsWith("Id")
                            ? singleLink.ColumnName.Substring(0, singleLink.ColumnName.Length - 2)
                            : singleLink.ColumnName,
                        Type = new Type(Kinds.Object, singleLink.ForeignTableName)
                    });
                }

                var multipleLinks = foreignKeyList.Where(x => x.ForeignTableName == table.Key);
                foreach (var multipleLink in multipleLinks)
                {
                    element.Fields.Add(new Field
                    {
                        Name = multipleLink.TableName,
                        Type = new Type(Kinds.Object, multipleLink.TableName),
                        Args = new List<InputField>
                        {
                            new InputField
                            {
                                Name = "filter",
                                Type = new Type(Kinds.InputObject, $"{multipleLink.TableName}Filter")
                            },
                            new InputField
                            {
                                Name = "orderBy",
                                Type = new Type(Kinds.Enum, $"{multipleLink.TableName}OrderBy")
                            },
                            new InputField
                            {
                                Name = "orderByDescending",
                                Type = new Type(Kinds.Enum, $"{multipleLink.TableName}OrderByDescending")
                            },
                            new InputField
                            {
                                Name = "skip",
                                Type = new Type(Kinds.InputObject, "Skip")
                            },
                            new InputField
                            {
                                Name = "take",
                                Type = new Type(Kinds.InputObject, "Take")
                            }
                        }
                    });
                }

                ret.Add(element);
            }

            return ret;
        }

        private List<Element> CreateFilters(List<FieldInfo> fieldInfoList)
        {
            var ret = new List<Element>
            {
                new Element
                {
                    Name = "IdFilter",
                    Kind = Kinds.InputObject,
                    InputFields = new List<InputField>
                    {
                        new InputField
                        {
                            Name = "id",
                            Description = "The id of the object.",
                            Type = Type.CreateNonNull(Kinds.Scalar, "Id")
                        }
                    }
                },
                new Element
                {
                    Name = "OperatorFilter",
                    Kind = Kinds.InputObject,
                    InputFields = FilterOperators.Select(x => new InputField
                    {
                        Name = x,
                        Description = $"'{x}' operator.",
                        Type = Type.CreateNonNull(Kinds.Scalar, "text")
                    }).ToList()
                },
                new Element
                {
                    Name = "Skip",
                    Kind = Kinds.InputObject,
                    InputFields = new List<InputField>
                    {
                        new InputField
                        {
                            Name = "skip",
                            Description = "Number of rows to skip.",
                            Type = Type.CreateNonNull(Kinds.Scalar, "Skip")
                        }
                    }
                },
                new Element
                {
                    Name = "Take",
                    Kind = Kinds.InputObject,
                    InputFields = new List<InputField>
                    {
                        new InputField
                        {
                            Name = "take",
                            Description = "Number of rows to take.",
                            Type = Type.CreateNonNull(Kinds.Scalar, "Take")
                        }
                    }
                },
            };

            var tables = fieldInfoList.GroupBy(x => x.TableName);

            foreach (var table in tables)
            {
                ret.Add(new Element
                {
                    Name = $"{table.Key}Filter",
                    Kind = Kinds.InputObject,
                    InputFields = table.Select(x => new InputField
                    {
                        Name = x.ColumnName,
                        Description = x.ColumnName,
                        Type = Type.CreateNonNull(Kinds.Object, "OperatorFilter")
                    }).ToList()
                });

                ret.Add(new Element
                {
                    Name = $"{table.Key}OrderBy",
                    Kind = Kinds.Enum,
                    EnumValues = table.Select(x => new EnumValue()
                    {
                        Name = x.ColumnName
                    }).ToList()
                });

                ret.Add(new Element
                {
                    Name = $"{table.Key}OrderByDescending",
                    Kind = Kinds.Enum,
                    EnumValues = table.Select(x => new EnumValue()
                    {
                        Name = x.ColumnName
                    }).ToList()
                });
            }

            return ret;
        }
    }
}
