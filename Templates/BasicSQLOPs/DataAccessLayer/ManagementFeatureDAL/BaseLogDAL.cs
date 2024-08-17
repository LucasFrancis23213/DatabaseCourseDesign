using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Collections.Generic;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public abstract class BaseLogDAL<T> : BaseDAL
    {
        protected abstract string TableName { get; }
        protected abstract List<string> ColumnNames { get; }
        protected abstract Func<OracleDataReader, T> MapFromReader { get; }

        protected Tuple<bool, string> GetLogsAux(Dictionary<(string ColumnName, string Operator), object> parameters)
        {
            return DoQuery(QueryGenerator(parameters));
        }

        protected Tuple<bool, string> InsertNewLogAux(Dictionary<string, object> values)
        {
            return DoQuery(InsertGenerator(values));
        }

        private Func<Tuple<bool, string>> InsertGenerator(Dictionary<string, object> values)
        {
            return () =>
            {
                var queryResult = BasicSQLOps.InsertOperation(TableName, ColumnNames, new List<object>(values.Values));
                return queryResult;
            };
        }

        private Func<Tuple<bool, string>> QueryGenerator(Dictionary<(string ColumnName, string Operator), object> parameters)
        {
            return () =>
            {
                if (OracleConnection.State == ConnectionState.Open)
                {
                    try
                    {
                        var query = $"SELECT * FROM {TableName} WHERE 1=1";
                        var oracleParameters = new List<OracleParameter>();

                        foreach (var param in parameters)
                        {
                            var (columnName, operatorSymbol) = param.Key;

                            if (param.Value != null)
                            {
                                query += $" AND {columnName} {operatorSymbol} :{columnName}";
                                oracleParameters.Add(new OracleParameter($":{columnName}{operatorSymbol}", param.Value));
                            }
                        }

                        var command = new OracleCommand(query, OracleConnection);
                        command.Parameters.AddRange(oracleParameters.ToArray());

                        var reader = command.ExecuteReader();
                        var result = new List<T>();

                        while (reader.Read())
                        {
                            result.Add(MapFromReader(reader));
                        }

                        var jsonResult = JsonConvert.SerializeObject(result);
                        return Tuple.Create(true, jsonResult);
                    }
                    catch (Exception ex)
                    {
                        return Tuple.Create(false, ex.Message);
                    }
                }
                else
                {
                    return Tuple.Create(false, "Connection Error");
                }
            };
        }
    }
}