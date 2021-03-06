namespace AgileObjects.ReadableExpressions.Translators
{
    using System;
    using System.Collections.Generic;
#if !NET35
    using System.Linq.Expressions;
#else
    using Expression = Microsoft.Scripting.Ast.Expression;
    using ExpressionType = Microsoft.Scripting.Ast.ExpressionType;
    using UnaryExpression = Microsoft.Scripting.Ast.UnaryExpression;
#endif

    internal struct UnaryExpressionTranslator : IExpressionTranslator
    {
        private static readonly Dictionary<ExpressionType, Func<string, string>> _operatorsByNodeType =
            new Dictionary<ExpressionType, Func<string, string>>
            {
                [ExpressionType.Decrement] = o => "--" + o,
                [ExpressionType.Increment] = o => "++" + o,
                [ExpressionType.IsTrue] = o => o,
                [ExpressionType.IsFalse] = o => "!" + o,
                [ExpressionType.OnesComplement] = o => "~" + o,
                [ExpressionType.PostDecrementAssign] = o => o + "--",
                [ExpressionType.PostIncrementAssign] = o => o + "++",
                [ExpressionType.PreDecrementAssign] = o => "--" + o,
                [ExpressionType.PreIncrementAssign] = o => "++" + o,
                [ExpressionType.Throw] = o => ("throw " + o).TrimEnd(),
                [ExpressionType.UnaryPlus] = o => "+" + o
            };

        public IEnumerable<ExpressionType> NodeTypes => _operatorsByNodeType.Keys;

        public string Translate(Expression expression, TranslationContext context)
        {
            var unary = (UnaryExpression)expression;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - Operand 
            // is null when using Expression.Rethrow():
            var operand = (unary.Operand != null)
                ? context.Translate(unary.Operand) : null;

            return _operatorsByNodeType[expression.NodeType].Invoke(operand);
        }
    }
}