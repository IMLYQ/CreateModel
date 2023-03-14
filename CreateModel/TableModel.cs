namespace CreateModel
{
    public class TableModel
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表说明
        /// </summary>
        public string TableDes { get; set; }

        /// <summary>
        /// 字段序号
        /// </summary>
        public int ColumnSort { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        public bool IsIdentification { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public bool PrimaryKey { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public string Lenght { get; set; }

        /// <summary>
        /// 允许空
        /// </summary>
        public bool IsNull { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        public string ColumnDes { get; set; }
    }
}