//        Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//            * Redistributions of source code must retain the above copyright
//              notice, this list of conditions and the following disclaimer.
//            * Redistributions in binary form must reproduce the above copyright
//              notice, this list of conditions and the following disclaimer in the
//              documentation and/or other materials provided with the distribution.
//            * Neither the name of author nor the
//              names of its contributors may be used to endorse or promote products
//              derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL  AUTHOR BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

namespace DataEngine.XQuery
{
    public enum Descriptor
    {
        Root,
        Version,
        Library,
        Query,
        ModuleNamespace,
        BoundarySpace,
        DefaultCollation,
        BaseUri,
        ConstructionDecl,
        Ordering,
        DefaultOrder,
        CopyNamespace,
        ImportSchema,
        Namespace,
        DefaultElement,
        DefaultFunction,
        ImportModule,
        VarDecl,
        DeclareFunction,
        TypeDecl,
        OptionDecl,
        FLWORExpr,
        For,
        Let,
        ForClauseOperator,
        LetClauseOperator,
        Where,
        OrderBy,
        StableOrderBy,
        Modifier,
        Some,
        Every,
        QuantifiedExprOper,
        Typeswitch,
        Case,
        If,
        OrExpr,
        Occurrence,
        DocumentNode,
        ProcessingInstruction,
        Element,
        Attribute,
        SchemaElement,
        SchemaAttribute,
        Or,
        And,
        ValueComp,
        GeneralComp,
        NodeComp,
        Range,
        Add,
        Mul,
        Union,
        Concatenate,
        IntersectExcept,
        InstanceOf,
        TreatAs,
        CastableAs,
        CastAs,
        Unary,
        Validate,
        Pragma,
        ExtensionExpr,
        Child,
        Descendant,
        AxisStep,
        FilterExpr,
        KindTest,
        Predicate,
        PredicateList,
        ForwardStep,
        AbbrevForward,
        AttributeAbbrev,
        ReverseStep,
        Wildcard1,
        Wildcard2,
        ParenthesizedExpr,
        ContextItem,
        Ordered,
        Unordered,
        Funcall,
        DirElemConstructor,
        DirAttribute,
        DirCommentConstructor,
        DirPIConstructor,
        EnclosedExpr,
        CDataSection,
        CompDocConstructor,
        CompElemConstructor,
        CompAttrConstructor,
        CompTextConstructor,
        CompCommentConstructor,
        CompPIConstructor,
        MappingExpr,
        Atomize
    };
}