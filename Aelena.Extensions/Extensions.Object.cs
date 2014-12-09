using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aelena.SimpleExtensions.ObjectExtensions
{
	public static class Extensions
	{

		/// <summary>
		/// Provides a safe version of ToString()
		/// so that if the object is null an empty string is returned 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>Returns the object ToString output or an empty string
		/// if the object is null.</returns>
		public static string ToStringSafe ( this Object obj )
		{
			return obj.ToStringSafe ( String.Empty );
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Provides a safe version of ToString()
		/// and allows the caller to specify a default return value so that if the
		/// string is null the return value will be used.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="defaultValue">Default value in case the instance is null.</param>
		/// <returns>Returns the object ToString output or the provided string
		/// value if the object is null.</returns>
		public static string ToStringSafe ( this object obj, string defaultValue )
		{
			return ( null == obj ) ? ( ( null == defaultValue ) ? String.Empty : defaultValue ) : Convert.ToString ( obj );
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Provides an enhanced ToString() that shows all public properties of a given object,
		/// (using reflection) which can be useful for debugging, logging, etc.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static string ToStringExpanded ( this object t )
		{
			if ( null == t )
				return String.Empty;

			StringBuilder _sb = new StringBuilder ();

			var _list = t.GetType ().GetProperties ();


			foreach ( var _l in _list )
			{
				_sb.AppendFormat ( "{0} : {1}{2}", _l.Name, _l.GetValue ( t, null ), " - " );
			}

			// cut trailing stuff
			return _sb.ToString ().Substring ( 0, _sb.Length - 3 );

		}


		// ---------------------------------------------------------------------------------
	}
}
