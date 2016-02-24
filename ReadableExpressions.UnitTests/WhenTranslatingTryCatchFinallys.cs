﻿namespace AgileObjects.ReadableExpressions.UnitTests
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenTranslatingTryCatchFinallys
    {
        [TestMethod]
        public void ShouldTranslateATryAndGlobalCatch()
        {
            Expression<Action> writeHello = () => Console.Write("Hello");
            var exception = Expression.Variable(typeof(Exception), "ex");
            var globalCatch = Expression.Catch(exception, Expression.Empty());
            var tryCatch = Expression.TryCatch(writeHello.Body, globalCatch);

            var translated = tryCatch.ToReadableString();

            const string EXPECTED = @"
try
{
    Console.Write(""Hello"");
}
catch
{
}";
            Assert.AreEqual(EXPECTED.TrimStart(), translated);
        }

        [TestMethod]
        public void ShouldTranslateATryAndGlobalCatchWithExceptionUseAndRethrow()
        {
            Expression<Action<Exception>> writeException = ex => Console.Write(ex);
            var exception = writeException.Parameters.First();
            Expression<Action> writeHello = () => Console.Write("Hello");
            var rethrow = Expression.Throw(exception);
            var writeExceptionAndRethrow = Expression.Block(writeException.Body, rethrow);
            var globalCatch = Expression.Catch(exception, writeExceptionAndRethrow);
            var tryCatch = Expression.TryCatch(writeHello.Body, globalCatch);

            var translated = tryCatch.ToReadableString();

            const string EXPECTED = @"
try
{
    Console.Write(""Hello"");
}
catch (Exception ex)
{
    Console.Write(ex);
    throw;
}";
            Assert.AreEqual(EXPECTED.TrimStart(), translated);
        }
    }
}