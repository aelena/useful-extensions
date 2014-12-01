using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aelena.SimpleExtensions.StringExtensions;

namespace Aelena.SimpleExtensions.ListExtensions
{
	public static class Extensions
	{
		/// <summary>
		/// Joins together a series of elements using their ToString() functions. 
		/// </summary>
		/// <param name="series"></param>
		/// <returns></returns>
		public static string JoinTogether<T> ( this IEnumerable<T> series, string separator = null )
		{
			if ( series == null )
				throw new ArgumentNullException ( "series", "Cannot join together a null series of elements" );

			if ( separator == null )
				separator = String.Empty;

			string _s = String.Empty;
			if ( series.Count () <= 25 )
			{
				foreach ( var s in series )
					_s += s.ToString () + separator;
			}
			else
			{
				var _sb = new StringBuilder ();
				foreach ( var s in series )
					_sb.AppendFormat ( "{0}{1}", s.ToString (), separator );
				_s = _sb.ToString ();
			}
			return _s.RemoveLast ( separator );

		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Joins together a series of Symbols. 
		/// This is basically a matter of concatenating their tokens
		/// </summary>
		/// <param name="series"></param>
		/// <returns></returns>
		public static string JoinTogetherBetween<T> ( this IEnumerable<T> series, int from, int to, string separator = null )
		{
			if ( series == null )
				throw new ArgumentNullException (  "series", "Cannot join together a null series of elements");

			if ( from < 0 )
				throw new ArgumentException ( "from", "Cannot specify a starting position lower than 0" );

			if ( to < from )
				throw new ArgumentException ( "to", "Cannot specify a upper index lower than the starting index" );

			if ( separator == null )
				separator = String.Empty;

			series = series.From ( from ).To ( to - from );

			string _s = String.Empty;
			if ( ( to - from ) <= 25 )
			{
				foreach ( var s in series )
					_s += s.ToString () + separator;
			}
			else
			{
				var _sb = new StringBuilder ();
				foreach ( var s in series )
					_sb.AppendFormat ( "{0}{1}", s.ToString (), separator );
				_s = _sb.ToString ();
			}
			return _s.RemoveLast ( separator );

		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Extension to Take where a caller can indicate from and to parameters.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static IEnumerable<T> Take<T> ( this IEnumerable<T> list, int from, int to )
		{
			return list.ToList ().GetRange ( from, ( to - from ) + 1 );
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns a subset of an instance of IEnumerable starting from the "from" position.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="from"></param>
		/// <returns></returns>
		public static IEnumerable<T> From<T> ( this IEnumerable<T> list, int from )
		{
			return list.ToList ().GetRange ( from, ( list.Count () - from ) );
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns from 0 to the "to" position on a IEnumerable instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static IEnumerable<T> To<T> ( this IEnumerable<T> list, int to )
		{
			return list.ToList ().GetRange ( 0, ++to );
		}


		// ---------------------------------------------------------------------------------

		/// <summary>
		/// Extension method to see if any of the 'searches' elements
		/// is contained in the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="searches"></param>
		/// <returns></returns>
		public static bool ContainsAny<T> ( this IEnumerable<T> list, IEnumerable<T> searches )
		{
			if ( list == null )
				throw new ArgumentNullException ( "list", "the list cannot be null." );

			if ( searches != null )
				foreach ( var s in searches )
					if ( list.Contains ( s ) )
						return true;

			return false;
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Extension method to see if any of the 'searches' elements
		/// is contained in the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="searches"></param>
		/// <returns>
		/// Returns a Tuple<bool,object> indicating true or false if an item was found or not
		/// and if it was, then the second item in the tuple, the object, will contain that value.
		/// </returns>
		public static Tuple<bool, object> ContainsAny2<T> ( this IEnumerable<T> list, IEnumerable<T> searches )
		{
			if ( list == null )
				throw new ArgumentNullException ( "list", "the list cannot be null." );

			if ( searches != null )
				foreach ( var s in searches )
					if ( list.Contains ( s ) )
						return new Tuple<bool, object> ( true, s );
			return new Tuple<bool, object> ( false, null );
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Gets the penultimate element in a list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T Penultimate<T> ( this IEnumerable<T> list ) where T : class
		{

			if ( list != null )
				return list.ElementAt ( list.Count () - 2 );

			return null;

		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns the element that sits in the indicated position,
		/// starting from 1 and starting from the back of the collection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static T ElementAtFromLast<T> ( this IEnumerable<T> list, int index ) where T : class
		{
			if ( list != null )
				return list.Reverse ().ElementAt ( index - 1 );
			return null;

		}

		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Checks if an element of type T exists in a list of Ts.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool In<T> ( this T value, IEnumerable<T> list )
		{
			if ( list == null )
				return false;
			return list.Contains ( value );
		}

		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns a boolean value to tell if the instance is null or empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty<T> ( this IEnumerable<T> list )
		{
			if ( list == null )
				return true;
			return list.Count () == 0;
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns a new list of T after the first element that matches
		/// the function passed.
		/// If no element in the list matches the function, it returns null.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="func"></param>
		/// <returns></returns>
		public static IEnumerable<T> TakeAfter<T> ( this IEnumerable<T> list, Func<T, bool> func )
		{
			int i = 0;
			foreach ( var l in list )
			{
				if ( func ( l ) )
				{
					return list.Skip ( ++i );
				}
				i++;
			}
			return null;
		}


		// ---------------------------------------------------------------------------------


		public static bool HasItems<T> ( this IEnumerable<T> t )
		{
			return t != null && t.Count () > 0;
		}



		// ---------------------------------------------------------------------------------


		public static IEnumerable<T> InsertMultiple<T> ( this IEnumerable<T> list, List<KeyValuePair<int, T>> itemsToInsert )
		{
			if ( list == null )
				throw new ArgumentNullException ( "list", "the list cannot be null." );

			if ( itemsToInsert != null )
			{
				List<T> _l = list.ToList ();
				foreach ( var i in itemsToInsert )
				{
					if ( i.IsFirst ( list ) )
						_l.Insert ( i.Key, i.Value );
					else
						_l.Insert ( i.Key + 1, i.Value );

				}
				return _l;
			}

			return list;

		}


		// ---------------------------------------------------------------------------------


	}
}
