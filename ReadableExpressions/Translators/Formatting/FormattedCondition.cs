namespace AgileObjects.ReadableExpressions.Translators.Formatting
{
    using System;
    using Extensions;
#if !NET35
    using System.Linq.Expressions;
#else
    using BinaryExpression = Microsoft.Scripting.Ast.BinaryExpression;
    using Expression = Microsoft.Scripting.Ast.Expression;
    using ExpressionType = Microsoft.Scripting.Ast.ExpressionType;

#endif

    internal class FormattedCondition : FormattableExpressionBase
    {
        private readonly Expression _condition;
        private readonly TranslationContext _context;
        private readonly string _singleLineTest;

        public FormattedCondition(
            Expression condition,
            TranslationContext context)
        {
            _condition = condition;
            _context = context;

            var test = context.TranslateAsCodeBlock(condition);

            if (test.IsMultiLine())
            {
                if (test.StartsWith(Environment.NewLine + '{', StringComparison.Ordinal))
                {
                    test = test.Indented();
                }

                test = test.TrimStart();
            }

            _singleLineTest = FinaliseCondition(test.WithSurroundingParentheses());

            MultipleLineTranslationFactory = GetMultipleLineTranslation;
        }

        protected override Func<string> SingleLineTranslationFactory => () => _singleLineTest;

        protected override Func<string> MultipleLineTranslationFactory { get; }

        private string GetMultipleLineTranslation()
        {
            if (IsNotRelevantBinary(_condition, out var conditionBinary))
            {
                return _singleLineTest;
            }

            var conditionLeft = new FormattedCondition(conditionBinary.Left, _context);
            var conditionOperator = BinaryExpressionTranslator.GetOperator(conditionBinary);
            var conditionRight = new FormattedCondition(conditionBinary.Right, _context);

            var condition = $@"
{conditionLeft} {conditionOperator}
{conditionRight.ToString().Indented()}".TrimStart().WithSurroundingParentheses();

            return FinaliseCondition(condition);
        }

        private static bool IsNotRelevantBinary(Expression condition, out BinaryExpression binary)
        {
            switch (condition.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    binary = (BinaryExpression)condition;
                    return false;
            }

            binary = null;
            return true;
        }

        private static string FinaliseCondition(string condition)
        {
            return condition;
        }
    }
}