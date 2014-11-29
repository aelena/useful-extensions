using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aelena.Extensions
{
    public static class Extensions
    {

		/// <summary>
		/// Returns a boolean value that indicates if the object is
		/// the first in the specified collection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool IsFirst<T> ( this object value, IEnumerable<T> list)
		{
			if ( list == null )
				return false;

			if ( value == null && list.First () == null )
				return true;

			if ( value == null )
				return false;

			return value.Equals ( list.First () );

		}

    }
}
