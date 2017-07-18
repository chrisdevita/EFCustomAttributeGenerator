//  Entity Designer Custom Attribute Generator
//  Copyright 2017 Christian DeVita - chris.devita@gmail.com
//  Based off of https://github.com/mthamil/EFDocumentationGenerator
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CustomAttributeGenerator
{
    /// <summary>
    /// An entity CustomAttribute source that pulls CustomAttribute from a SQL Server database.
    /// </summary>
    internal class DatabaseCustomAttributeSource : ICustomAttributeSource
    {
        /// <summary>
        /// Initializes a new <see cref="DatabaseCustomAttributeSource"/>.
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        public DatabaseCustomAttributeSource(string connectionString)
            : this(connectionString, cs => new SqlConnection(cs))
        {
        }

        /// <summary>
        /// Initializes a new <see cref="DatabaseCustomAttributeSource"/>.
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="connectionFactory">Creates database connections from a connection string</param>
        public DatabaseCustomAttributeSource(string connectionString, Func<string, IDbConnection> connectionFactory)
        {
            _connection = connectionFactory(connectionString);
            _connection.Open();
        }

        /// <see cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            _connection.Dispose();
        }

        /// <see cref="ICustomAttributeSource.GetCustomAttributes"/>
        public IEnumerable<CustomAttribute> GetCustomAttributes(string entityName, EntityProperty property = null)
        {
            List<CustomAttribute> customAttributes = new List<CustomAttribute>();

            bool useSecondLevel = property != null;

            var query = String.Format(@"
                        SELECT [CustomAttribute].[name],  [CustomAttribute].[value] FROM [sys].[schemas]
                        CROSS APPLY fn_listextendedproperty (
                            NULL, 
                            'schema', [sys].[schemas].[name], 
                            'table', @tableName,
                             {0}, {1}) as CustomAttribute
                        WHERE [sys].[schemas].[name] <> 'sys' AND 
                              [sys].[schemas].[name] NOT LIKE 'db\_%' ESCAPE '\'",
                    useSecondLevel ? GetSecondLevelType(property.Type) : "null",
                    useSecondLevel ? "@secondLevelName" : "null");

            using (SqlCommand command = (SqlCommand)_connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("tableName", entityName));

                if (useSecondLevel)
                    command.Parameters.Add(new SqlParameter("secondLevelName", property.Name));

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    customAttributes.Add(new CustomAttribute((string)reader["name"], (string)reader["value"]));
                }

                return customAttributes;
            }
        }

        private static string GetSecondLevelType(EntityPropertyType propertyType) =>
            propertyType == EntityPropertyType.Property ? "'column'" : "'constraint'";

        private readonly IDbConnection _connection;
    }
}