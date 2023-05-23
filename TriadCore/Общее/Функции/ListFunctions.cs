using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TriadCore
{
    /// <summary>
    /// Функции для работы со списками
    /// </summary>
    public static class ListFunctions
    {
        /// <summary>
        /// Получение количества элементов списка
        /// </summary>
        /// <param name="list">список</param>
        /// <returns>Количество элементов списка</returns>
        public static int ListSize(IList list)
        {
            return list.Count;
        }

        /// <summary>
        /// Добавление элемента в конец списка
        /// </summary>
        /// <param name="list">список</param>
        /// <param name="item">добавляемый элемент</param>
        /// <returns></returns>
        public static void ListAdd(IList list, object item)
        {
            list.Add(item);
        }

        /// <summary>
        /// Удаление первого вхождения элемента
        /// </summary>
        /// <param name="list">список</param>
        /// <param name="item">удаляемый элемент</param>
        /// <returns></returns>
        public static void ListRemove(IList list, object item)
        {
            list.Remove(item);
        }

        /// <summary>
        /// Добавление элемента в позицию index
        /// </summary>
        /// <param name="list">список</param>
        /// <param name="index">индекс</param>
        /// <param name="item">добавляемый элемент</param>
        /// <returns></returns>
        public static void ListInsert(IList list, int index, object item)
        {
            list.Insert(index, item);
        }

        /// <summary>
        /// Удаление элемента по индексу
        /// </summary>
        /// <param name="list">список</param>
        /// <param name="index">индекс</param>
        /// <returns></returns>
        public static void ListRemoveAt(IList list, int index)
        {
            list.RemoveAt(index);
        }
    }
}
