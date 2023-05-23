using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler
{
    /// <summary>
    /// Тип данных список
    /// </summary>
    internal class VarListType : IExprType
    {
        /// <summary>
        /// Базовый тип переменной
        /// </summary>
        public TypeCode Code { get; set; }

        /// <summary>
        /// Признак spy-объекта
        /// </summary>
        public bool IsSpyObject { get; set; } = false;

        /// <summary>
        /// Имя переменной
        /// </summary>
        public string Name { get; set; }


        public VarListType(TypeCode typeCode)
        {
            this.Code = typeCode;
        }

        public VarListType(TypeCode typeCode, string varName) : this(typeCode)
        {
            this.Name = varName;
        }

        public IExprType Clone()
        {
            return  new VarListType(this.Code, this.Name);
        }
    }
}
