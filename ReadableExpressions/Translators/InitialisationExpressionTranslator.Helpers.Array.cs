﻿namespace AgileObjects.ReadableExpressions.Translators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal partial class InitialisationExpressionTranslator
    {
        private class ArrayInitExpressionHelper : InitExpressionHelperBase<NewArrayExpression, NewArrayExpression>
        {
            public ArrayInitExpressionHelper(IExpressionTranslatorRegistry registry)
                : base(registry)
            {
            }

            protected override NewArrayExpression GetNewExpression(NewArrayExpression expression)
            {
                var arrayElementType = expression.Type.GetElementType();

                return Expression.NewArrayBounds(
                    arrayElementType,
                    Expression.Constant(expression.Expressions.Count));
            }

            protected override bool ConstructorIsParameterless(NewArrayExpression newExpression)
            {
                return false;
            }

            protected override IEnumerable<string> GetInitialisations(NewArrayExpression expression)
            {
                return expression.Expressions.Select(Registry.Translate);
            }
        }
    }
}
