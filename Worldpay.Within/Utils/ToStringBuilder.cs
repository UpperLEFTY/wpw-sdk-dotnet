using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Worldpay.Innovation.WPWithin.Utils
{

    /// <summary>
    /// Convenience utility class for building good implementations of <see cref="object.ToString()"/>.
    /// </summary>
    /// <remarks>
    /// Based onhttp://www.jroller.com/DhavalDalal/entry/equals_hashcode_and_tostring_builders.
    /// Code licensed under CC-3.0.
    /// </remarks>
    /// <typeparam name="T">The type of objects that we are generated a string for.</typeparam>
    public class ToStringBuilder<T>
    {
        private readonly T _target;
        private readonly string _typeName;
        private const string Delimiter = "=";
        private readonly IList<string> _values = new List<string>();

        public ToStringBuilder(T target)
        {
            this._target = target;
            _typeName = target.GetType().Name;
        }

        public ToStringBuilder<T> Append<TProperty>(Expression<Func<T, TProperty>> propertyOrField)
        {
            MemberExpression expression = propertyOrField.Body as MemberExpression;
            if (expression == null)
            {
                throw new ArgumentException("Expecting Property or Field Expression");
            }
            string name = expression.Member.Name;
            Func<T, TProperty> func = propertyOrField.Compile();
            TProperty returnValue = func(_target);
            string value = (returnValue == null) ? "null" : returnValue.ToString();
            _values.Add($"{name}{Delimiter}{value}");
            return this;
        }

        public override string ToString()
        {
            return $"{_typeName}:{{{string.Join(",", _values)}}}";
        }
    }
}
