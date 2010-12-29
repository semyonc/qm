/*    
    SQLXEngine - Implementation of ANSI-SQL specification and 
       SQL-engine for executing the SELECT SQL command across the different data sources.
    Copyright (C) 2008-2009  Semyon A. Chertkov (semyonc@gmail.com)

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
namespace DataEngine
{
    public enum Descriptor
    {
        Z,
        Root,
        SQuery,             // Подзапрос  
        Order,              // Сортировка (общ)
        Desc,               // Сортировка по убыванию (поле)
        Union,              // Оператор UNION
        Except,             // Оператор EXCEPT
        Intersect,          // Оператор INTERSECT 
        TableValue,         // Конструкция VALUES...
        RowValue,           // Векторная конструкция (...,...)
        Distinct,           // Оператор Distinct
        From,               // оператор From
        Where,              // оператор Where
        GroupBy,            // список колонок, учавствующих в группировке
        Having,             // условие HAVING
        Explicit,           // Явное описание таблицы в QueryTerm/QueryExp 
        Select,             // Список полей
        Alias,              // Псевдоним
        TableFields,        // Оператор TableName.* в Select
        DerivedColumns,     // 
        CrossJoin,          // оператор соединения CROSS JOIN
        UnionJoin,          // оператор соединения UNION JOIN
        NaturalJoin,        // оператор соединения NATURAL JOIN
        QualifiedJoin,      // оператор соединения с произвольным условием
        JoinType,           // Тип соединения в операторе Join
        JoinSpec,           // Спецификация соединения в QualifiedJoin
        Outer,              // спецификатор Outer
        Using,              // Named columns list в операторе Join
        Constraint,         // Constraint Join 
        Corresponding,      // 
        Concat,             // Оператор соединения строк
        Substring,          // Оператор выделения подстроки
        PosString,          // Оператор позиции в строке
        StringUpper,        // Upper
        StringLower,        // Lower
        StringConvert,      // Оператор CONVERT
        StringTrim,         // Trim 
        Inverse,            // Логическая инверсия условия
        Aggregate,          // Агрегатная функция 
        AggCount,           // Агрегатная функция Count(*)
        ColumnName,         // Имя используется как явное название колонки в таблице
        TableName,          // Имя является ссылкой на таблицу
        ColumnRef,          // Имя является ссылкой на столбец
        GroupingColumnRef,  // Имя является ссылкой на столбец участвующий в операторе GROUP BY
        NullIf,             // Оператор NullIf
        Coalesce,           // Оператор Coalesce
        Case,               // Оператор Case
        CaseBranch,         // Ветка оператора Case
        ElseBranch,         // Ветка Else оператора Case
        LogicalOR,          // Логическое ИЛИ
        LogicalAND,         // Логическое И
        BooleanTest,        // IS FALSE, IS TRUE, IS UNKNOWN...
        Pred,               // условие с логическим оператором
        QuantifiedPred,     // условие с квантором ALL,SOME,ANY
        Between,            // условие Between
        Like,               // условие Like
        Escape,             // escape для условия Like
        IsNull,             // условие IsNull
        Exists,             // условие Exists
        Unique,             // условие Unique
        Match,              // условие Match
        MatchType,          // Match: Unique, Partial 
        Overlaps,           // условие Overlaps
        InSet,              // условие In
        ValueList,          // Конструктор списка значений
        Branch,             // Скобки в выражении  
        UnaryMinus,         // Арифментический унарный минус
        Add,                // Арифметический оператор сложения
        Sub,                // Арифметический оператор вычитания
        Mul,                // Арифметический оператор умножения
        Div,                // Арифметический оператор деления
        Funcall,            // Вызов внешней функции
        Dynatable,          // Динамическая таблица
        // далее дескрипторы транформации и оптимизации
        Binding,            // Список подстановок параметров корреляций
        Link,               // Связь имени с внешним параметром
        HintJoin,           // Выбор метода соединения
        HintKeyPair,        // Соотв. ключей в QualifiedJoin
        HintSort,           // Сортировка полей 
        Dref,               // Разименование
        At,                 // Элемент массива
        Wref,               // Разименование имя//поле
        Prefix,             // Префикс имени  
        OptimizerHint,        
        HintNamespace,
        HintColumn,         
        HintFilter,         // Выполнение фильтрации на сервере
        HintServerSubquery, // Выполнение запроса на сервере
        Top,                // Ограничение количества записей возвращаемых запросом 
        // дескрипторы расширений SQLX
        XMLElement,
        XMLNamespaces,
        XMLAttributes,
        XMLRoot,
        XMLCDATA,
        XMLComment,
        XMLConcat,
        XMLForest,
        XMLForestAll,
        XMLAgg,
        XMLParse,
        XMLPI,
        XMLQuery,
        DeclNamespace,
        ContentOption,
        // Оператор CAST
        Cast,
        Typelen,
        Typeprec,
        Typescale
    }
}