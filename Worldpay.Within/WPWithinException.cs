using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrift;

namespace Worldpay.Innovation.WPWithin
{

    /** Used for all exceptions that can be thrown by the toolkit.
     */
    public class WPWithinException : Exception
    {

        public WPWithinException() : base() { }

        public WPWithinException(string message) : base(message) { }

        public WPWithinException(string message, Exception innerException) : base(message, innerException) { }

        public WPWithinException(Exception innerException, String fmt, params Object[] parameters) :
            base(String.Format(fmt, parameters), innerException)
        { }

        internal WPWithinException(TApplicationException tae) : this(tae, "Exception from WPWithin SDK: {0}", tae.Message)
        { }
    }
}
