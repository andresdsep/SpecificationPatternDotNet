﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace SpecificationPatternDotNet
{
    internal static class ExpressionExtensions
    {
        public static Expression<Func<TDerivedEntity, bool>> AndAlso<TEntity, TDerivedEntity>(
            this Expression<Func<TEntity, bool>> firstExpression,
            Expression<Func<TDerivedEntity, bool>> secondExpression)
            where TDerivedEntity : TEntity
        {
            return firstExpression.Compose(secondExpression, Expression.AndAlso);
        }

        public static Expression<Func<TDerivedEntity, bool>> OrElse<TEntity, TDerivedEntity>(
            this Expression<Func<TEntity, bool>> firstExpression,
            Expression<Func<TDerivedEntity, bool>> secondExpression)
            where TDerivedEntity : TEntity
        {
            return firstExpression.Compose(secondExpression, Expression.OrElse);
        }

        private static Expression<Func<TDerivedEntity, bool>> Compose<TEntity, TDerivedEntity>(
            this Expression<Func<TEntity, bool>> firstExpression,
            Expression<Func<TDerivedEntity, bool>> secondExpression,
            Func<Expression, Expression, Expression> mergeFunc)
            where TDerivedEntity : TEntity
        {
            var derivedParameter = Expression.Parameter(typeof (TDerivedEntity));

            var firstParameter = firstExpression.Parameters.Single();
            var firstParameterVisitor = new ParameterVisitor(firstParameter, derivedParameter);
            var firstBody = firstParameterVisitor.Visit(firstExpression.Body);

            var secondParameter = secondExpression.Parameters.Single();
            var secondParameterVisitor = new ParameterVisitor(secondParameter, derivedParameter);
            var secondBody = secondParameterVisitor.Visit(secondExpression.Body);

            return Expression.Lambda<Func<TDerivedEntity, bool>>(
                mergeFunc(firstBody, secondBody), derivedParameter);
        }

        public static Expression<Func<TEntity, bool>> Not<TEntity>(this Expression<Func<TEntity, bool>> expression)
        {
            var unaryExpression = Expression.Not(expression.Body);

            return Expression.Lambda<Func<TEntity, bool>>(unaryExpression, expression.Parameters);
        }
    }
}