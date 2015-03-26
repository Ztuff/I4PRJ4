using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using SmartFridgeDAL.AdoNetUoW;

namespace SmartFridgeDAL.Repository
{
    public class ListRepository : Repository<List>
    {

        public ListRepository(IContext context) : base(context)
        {
        }

        public override void Insert(List list)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"INSERT INTO List (ListName) OUTPUT INSERTED.ListId VALUES(@ListName)";
                var param = command.CreateParameter();
                param.ParameterName = "@ListName";
                param.Value = list.ListName;
                command.Parameters.Add(param);
                list.ListId = (int)command.ExecuteScalar();
            }
        }

        public override void Update(List list)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"UPDATE List SET ListName = @ListName WHERE ListId = @ListId";
                var nameParam = command.CreateParameter();
                nameParam.ParameterName = "@ListName";
                nameParam.Value = list.ListName;
                command.Parameters.Add(nameParam);
                var idParam = command.CreateParameter();
                idParam.ParameterName = "@ListId";
                idParam.Value = list.ListId;
                command.Parameters.Add(idParam);
                command.ExecuteNonQuery();
            }
        }

        public override void Delete(List list)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"DELETE FROM List Where ListId = @ListId";
                var param = command.CreateParameter();
                param.ParameterName = "@ListId";
                param.Value = list.ListId;
                command.Parameters.Add(param);
                command.ExecuteNonQuery();
            }
        }

        public override IEnumerable<List> GetAll()
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"SELECT * From List";
                return ToList(command);
            }
        }

        public IEnumerable<List> GetByName(string name)
        {
            using (var command = Context.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM List WHERE ListName = @ListName";
                var param = command.CreateParameter();
                param.ParameterName = "@ListName";
                param.Value = name;
                command.Parameters.Add(param);
                return ToList(command);
            }
        }

        protected override void Map(IDataRecord record, List list)
        {
            list.ListId = (int) record["ListId"];
            list.ListName = (string) record["ListName"];
        }
    }
}
