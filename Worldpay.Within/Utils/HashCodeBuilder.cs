using System;
using System.Linq.Expressions;

namespace Worldpay.Innovation.WPWithin.Utils
{
    /// <summary>
    /// Convenience utility class for building good implementations of <see cref="object.GetHashCode()"/>.
    /// </summary>
    /// <remarks>
    /// Based on http://www.jroller.com/DhavalDalal/entry/equals_hashcode_and_tostring_builders.
    /// Code licensed under CC-3.0.
    /// </remarks>
    /// <typeparam name="T">The type of objects that we want the hash code for.</typeparam>
    public class HashCodeBuilder<T>
    {
        private readonly T _target;
        private int _hashCode = 17;

        public HashCodeBuilder(T target)
        {
            this._target = target;
        }

        public HashCodeBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> propertyOrField)
        {
            var expression = propertyOrField.Body as MemberExpression;
            if (expression == null)
            {
                throw new ArgumentException("Expecting Property or Field Expression of an object");
            }

            Func<T, TProperty> func = propertyOrField.Compile();
            TProperty value = func(_target);
            _hashCode += 31 * _hashCode + ((value == null) ? 0 : value.GetHashCode());
            return this;
        }

        public int HashCode => _hashCode;
    }
}
