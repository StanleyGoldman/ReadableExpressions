namespace AgileObjects.ReadableExpressions.Translators
{
    using System.Linq;
#if !NET35
    using System.Linq.Expressions;
#else
    using Expression = Microsoft.Scripting.Ast.Expression;
    using ExpressionType = Microsoft.Scripting.Ast.ExpressionType;
    using NewExpression = Microsoft.Scripting.Ast.NewExpression;
#endif
    using Extensions;
    using NetStandardPolyfills;

    internal class NewExpressionTranslator : ExpressionTranslatorBase
    {
        internal NewExpressionTranslator()
            : base(ExpressionType.New)
        {
        }

        public override string Translate(Expression expression, TranslationContext context)
        {
            var newExpression = (NewExpression)expression;

            if (newExpression.Type.IsAnonymous())
            {
                return GetAnonymousTypeCreation(newExpression, context);
            }

            var typeName = (newExpression.Type == typeof(object)) ? "Object" : newExpression.Type.GetFriendlyName();
            var parameters = context.TranslateParameters(newExpression.Arguments).WithParentheses();

            return "new " + typeName + parameters;
        }
        private static string GetAnonymousTypeCreation(NewExpression newExpression, TranslationContext context)
        {
            var constructorParameters = newExpression.Constructor.GetParameters();

            var arguments = newExpression
                .Arguments
                .Select((arg, i) => constructorParameters[i].Name + " = " + context.Translate(arg));

            var argumentsString = arguments.Join(", ");

            return "new { " + argumentsString + " }";
        }
    }
}