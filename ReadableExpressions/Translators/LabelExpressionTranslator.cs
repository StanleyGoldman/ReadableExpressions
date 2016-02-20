namespace AgileObjects.ReadableExpressions.Translators
{
    using System.Linq.Expressions;

    internal class LabelExpressionTranslator : ExpressionTranslatorBase
    {
        public LabelExpressionTranslator()
            : base(ExpressionType.Label)
        {
        }

        public override string Translate(Expression expression, IExpressionTranslatorRegistry translatorRegistry)
        {
            var label = (LabelExpression)expression;
            var labelName = label.Target.Name.Unindented();

            return $@"
{labelName}:";
        }
    }
}