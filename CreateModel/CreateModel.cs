using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DbType = SqlSugar.DbType;

namespace CreateModel
{
    public partial class CreateModel : Form
    {
        public CreateModel()
        {
            InitializeComponent();

            string ReadMe = "1.(*)为必填选项，名称空间不填写则默认为ServerModels\r\n" +
                            "2.支持同时生成多个表，中间用英文字符','隔开\r\n" +
                            "3.生成文件放于：bin\\Debug\\Model文件夹下\r\n" +
                            "4.目前数据库仅支持SQLServer、其他数据库未适配";

            this.richTextBox1.Text = ReadMe;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string config = txtConfig.Text.Trim();
            string tabaleName = this.textTableName.Text.Trim();

            var tables = tabaleName.Split(',');
            try
            {
                ///循环解析所有的表
                foreach (var item in tables)
                {
                    ParseDatabase(config, item);
                }

                MessageBox.Show("生成完成！");
            }
            catch (Exception)
            {

                MessageBox.Show("生成失败！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

        }

        private void ParseDatabase(string config, string tabaleName)
        {
            //通过SqlSugar连接数据库
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = config,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true//自动释放
            });

            string sql = $@"SELECT
    TableName     = d.name,--case when a.colorder=1 then d.name else '' end,
    TableDes     = case when a.colorder=1 then isnull(f.value,'') else '' end,
    ColumnSort   = a.colorder,
    ColumnName     = a.name,
    IsIdentification       = case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then 1 else 0 end,
    PrimaryKey       = case when exists(SELECT 1 FROM sysobjects where xtype='PK' and parent_obj=a.id and name in (
                     SELECT name FROM sysindexes WHERE indid in(
                        SELECT indid FROM sysindexkeys WHERE id = a.id AND colid=a.colid))) then 1 else 0  end,
    Type       = b.name,
    --占用字节数 = a.length,
    Lenght       = COLUMNPROPERTY(a.id,a.name,'PRECISION'),
    --小数位数   = isnull(COLUMNPROPERTY(a.id,a.name,'Scale'),0),
    IsNull     = case when a.isnullable=1 then 1 else 0 end,
    -- 默认值     = isnull(e.text,''),
    ColumnDes   = isnull(g.[value],'')
FROM
    syscolumns a
left join
    systypes b
on
    a.xusertype=b.xusertype
inner join
    sysobjects d
on
    a.id=d.id  and d.xtype='U' and  d.name<>'dtproperties'
left join
    syscomments e
on
    a.cdefault=e.id
left join

    sys.extended_properties g
on
    a.id=g.major_id and a.colid=g.minor_id
left join
    sys.extended_properties f
on
    d.id=f.major_id and f.minor_id=0
where
    d.name='{tabaleName}'    --如果只查询指定表,加上此条件
order by
    a.id,a.colorder";

            var ss = db.Ado.SqlQuery<TableModel>(sql);
            JointStr(ss);
        }

        /// <summary>
        /// 解析数据库表
        /// </summary>
        /// <param name="lstTabInfo"></param>
        public void JointStr(List<TableModel> lstTabInfo)
        {
            string text1 = string.Empty;
            string tabBlank = "    ";
            //获取指定的名称空间
            string nameSpace = this.txtNameSpace.Text.Trim();
            //如果为空就默认为ServerModels
            if (string.IsNullOrWhiteSpace(nameSpace))
            {
                nameSpace = "ServerModels";
            }

            text1 = "using System;\n\n";
            text1 += "namespace " + nameSpace + "\n{\n";
            text1 += tabBlank + "public class " + lstTabInfo[0].TableName + "{\n";

            foreach (var item in lstTabInfo.OrderBy(a => a.ColumnSort))
            {
                string dataType = string.Empty;
                dataType = changetocsharptype(item.Type);
                text1 += tabBlank + tabBlank + "/// <summary>\n" + tabBlank + tabBlank + "/// " + item.ColumnDes + "\n" + tabBlank + tabBlank + "/// </summary>\n";
                text1 += tabBlank + tabBlank + "public " + dataType + (item.IsNull && dataType != "string" ? "?" : "") + " " + item.ColumnName + "{ get; set; } \n";
            }
            text1 += tabBlank + "}\n" + "}";
            string directory = AppDomain.CurrentDomain.BaseDirectory + "\\Model\\";
            StreamWriter sr;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string path = directory + "\\" + lstTabInfo[0].TableName + ".cs";
            if (File.Exists(path))
            {
                File.Delete(path);

            }
            sr = File.CreateText(path);
            sr.Write(text1.ToString());
            sr.Flush();
            sr.Close();

        }

        /// <summary>
        /// 数据库对应类型
        /// </summary>
        /// <param name="type">数据库中的数据类型 </param>
        /// <returns></returns>
        public static string changetocsharptype(string type)
        {
            string reval = string.Empty;
            switch (type.ToLower())
            {
                case "int":
                    reval = "int";
                    break;

                case "text":
                    reval = "string";
                    break;

                case "bigint":
                    reval = "Int64";
                    break;

                case "binary":
                    reval = "byte[]";
                    break;

                case "bit":
                    reval = "bool";
                    break;

                case "char":
                    reval = "string";
                    break;

                case "datetime":
                    reval = "DateTime";
                    break;
                case "date":
                    reval = "DateTime";
                    break;

                case "decimal":
                    reval = "decimal";
                    break;

                case "float":
                    reval = "decimal";
                    break;

                case "image":
                    reval = "byte[]";
                    break;

                case "money":
                    reval = "decimal";
                    break;

                case "nchar":
                    reval = "string";
                    break;

                case "ntext":
                    reval = "string";
                    break;

                case "numeric":
                    reval = "decimal";
                    break;

                case "nvarchar":
                    reval = "string";
                    break;

                case "real":
                    reval = "single";
                    break;

                case "smalldatetime":
                    reval = "DateTime";
                    break;

                case "smallint":
                    reval = "Int16";
                    break;

                case "smallmoney":
                    reval = "decimal";
                    break;

                case "timestamp":
                    reval = "DateTime";
                    break;

                case "tinyint":
                    reval = "byte";
                    break;

                case "uniqueidentifier":
                    reval = "guid";
                    break;

                case "varbinary":
                    reval = "byte[]";
                    break;

                case "varchar":
                    reval = "string";
                    break;

                case "variant":
                    reval = "object";
                    break;

                default:
                    reval = "string";
                    break;
            }
            return reval;
        }
    }
}