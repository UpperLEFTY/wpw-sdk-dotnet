using System;
using System.Linq.Expressions;

namespace Worldpay.Innovation.WPWithin.Utils
{

    /// <summary>
    /// Convenience utility class for building good implementations of <see cref="object.Equals(object)"/>.
    /// </summary>
    /// <remarks>
    /// Based on http://www.jroller.com/DhavalDalal/entry/equals_hashcode_and_tostring_builders.
    /// Code licensed under CC-3.0.
    /// </remarks>
    /// <typeparam name="T">The type of objects that will be compared.</typeparam>
    public class EqualsBuilder<T>
    {
        private readonly T _left;
        private readonly object _right;
        private bool _areEqual = true;

        public EqualsBuilder(T left, object right)
        {
            this._left = left;
            this._right = right;

            if (ReferenceEquals(left, right))
            {
                _areEqual = true;
                return;
            }

            if (ReferenceEquals(left, null))
            {
                _areEqual = false;
                return;
            }

            if (ReferenceEquals(right, null))
            {
                _areEqual = false;
                return;
            }

            if (left.GetType() != right.GetType())
            {
                _areEqual = false;
                return;
            }
        }

        public EqualsBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> propertyOrField)
        {
            if (!_areEqual || _left == null || _right == null)
            {
                return this;
            }

            MemberExpression expression = propertyOrField.Body as MemberExpression;
            if (expression == null)
            {
                throw new ArgumentException("Expecting Property or Field Expression of an object");
            }

            Func<T, TProperty> func = propertyOrField.Compile();
            TProperty leftValue = func(_left);
            TProperty rightValue = func((T)_right);

            if (leftValue == null && rightValue == null)
            {
                // _areEqual remains true if both are null
                return this;
            }

            if (leftValue == null || rightValue == null)
            {
                // If left or right are null then one must be null and the other isn't, because
                // we've already testing for both being null.
                _areEqual = false;
                return this;
            }

            _areEqual &= leftValue.Equals(rightValue);
            return this;
        }

        public bool Equals()
        {
            return _areEqual;
        }
    }
}
