﻿namespace AgileObjects.ReadableExpressions.Translators
{
    using System.Collections.Generic;
    using System.Linq;
#if !NET35
    using System.Linq.Expressions;
#else
    using Expression = Microsoft.Scripting.Ast.Expression;
    using NewArrayExpression = Microsoft.Scripting.Ast.NewArrayExpression;
#endif
    using Extensions;

    internal partial struct InitialisationExpressionTranslator
    {
        private class ArrayInitExpressionHelper : InitExpressionHelperBase<NewArrayExpression, NewArrayExpression>
        {
            public ArrayInitExpressionHelper()
                : base(null, null)
            {
            }

            protected override string GetNewExpressionString(NewArrayExpression initialisation, TranslationContext context)
            {
                if (initialisation.Expressions.Count == 0)
                {
                    return "new " + GetExplicitArrayType(initialisation) + "[0]";
                }

                var explicitType = GetExplicitArrayTypeIfRequired(initialisation);

                return "new" + explicitType + "[]";
            }

            private static string GetExplicitArrayType(Expression initialisation)
            {
                return initialisation.Type.GetElementType().GetFriendlyName();
            }

            private static string GetExplicitArrayTypeIfRequired(NewArrayExpression initialisation)
            {
                var expressionTypes = initialisation
                    .Expressions
                    .Project(exp => exp.Type)
                    .Distinct()
                    .ToArray();

                if (expressionTypes.Length == 1)
                {
                    return null;
                }

                return " " + GetExplicitArrayType(initialisation);
            }

            protected override IEnumerable<string> GetMemberInitialisations(
                NewArrayExpression arrayInitialisation,
                TranslationContext context)
            {
                return arrayInitialisation.Expressions.Project(context.TranslateAsCodeBlock);
            }
        }
    }
}
