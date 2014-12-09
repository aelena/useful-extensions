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

			var _s = String.Empty;
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
				throw new ArgumentNullException ( "series", "Cannot join together a null series of elements" );

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


		/// <summary>
		/// Provides a safer and integrated way to check if a collection
		/// is not a null instance and actually has elements in it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <returns></returns>
		public static bool HasElements<T> ( this IEnumerable<T> t )
		{
			return t != null && t.Count () > 0;
		}



		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns a new list with the original elements plus
		/// the new elements inserted at the specified positions.
		/// </summary>
		/// <typeparam name="T">Type of the elements in the list.</typeparam>
		/// <param name="list">Original list.</param>
		/// <param name="itemsToInsert">List of indices and new elements to 
		/// be inserted in the list.</param>
		/// <returns>Returns a new list with the original elements plus
		/// the new elements inserted at the specified positions.</returns>
		/// <exception cref="ArgumentNullException"/>
		public static IEnumerable<T> InsertMultiple<T> ( this IEnumerable<T> list, List<KeyValuePair<int, T>> itemsToInsert )
		{
			if ( list == null )
				throw new ArgumentNullException ( "list", "the list cannot be null." );


			//var upperIndex = new List<int> { itemsToInsert.GetKeys ().Max (), list.Count () }.Max ();

			//var _l = new List<T> ();
			//if ( itemsToInsert.HasElements () )
			//{
			//	for ( var i = 0; i < upperIndex; i++ )
			//	{
			//		// there can be three scenarios, 
			//		// i is an index of the original list,
			//		// i is an index in the new list,
			//		// i has no correspondence, therefore we insert null 
			//		// second scenario has preference, as in a normal insert operation
			//		if ( i < list.Count () )
			//			_l.Insert ( i, list.ElementAt ( i ) );

			//		if ( itemsToInsert.Any ( x => x.Key == i ) )
			//		{
			//			_l.Insert ( i, itemsToInsert.Where ( x => x.Key == i ).First ().Value );
			//			continue;
			//		}

			//		if ( i > list.Count () )
			//			_l.Insert ( i, default ( T ) );
			//	}

			//	return _l;
			//}

			if ( itemsToInsert.HasElements () )
			{
				List<T> _l = list.ToList ();
				foreach ( var i in itemsToInsert )
				{
					//if ( i.IsFirst ( list ) )
						_l.Insert ( i.Key, i.Value );
					//else
					//	_l.Insert ( i.Key + 1, i.Value );

				}
				return _l;
			}

			return list;

		}


		// ---------------------------------------------------------------------------------


		/// <exception cref="ArgumentNullException"/>
		public static List<Int32> GetKeys<T> ( this IEnumerable<KeyValuePair<int, T>> list )
		{
			if ( list == null )
				throw new ArgumentNullException ( "list", "the list cannot be null." );
			var _retList = new List<int> ();
			list.ToList ().ForEach ( x => _retList.Add ( x.Key ) );
			return _retList;
		}


		// ---------------------------------------------------------------------------------


		public static List<Int32> GetKeys<T> ( this IEnumerable<Tuple<int, T>> list )
		{
			if ( list == null )
				throw new ArgumentNullException ( "list", "the list cannot be null." );
			var _retList = new List<int> ();
			list.ToList ().ForEach ( x => _retList.Add ( x.Item1 ) );
			return _retList;
		}


		// ---------------------------------------------------------------------------------

		/// <summary>
		/// Returns the index of the first element that satisfies the function.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="func"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException" />
		public static int IndexOf<T> ( this IEnumerable<T> list, Func<T, bool> func )
		{
			if ( list == null )
				throw new ArgumentNullException ( "list", "The list to be searched cannot be null" );
			if ( func == null )
				throw new ArgumentNullException ( "func", "The search function cannot be null" );

			int i = 0;
			foreach ( var l in list )
			{
				if ( func ( l ) )
				{
					return i;
				}
				i++;
			}
			return -1;
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns the index of the first element that satisfies the function
		/// but it performs the search only after a certain index.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="func"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException" />
		/// <exception cref="ArgumentException" />
		public static int IndexOf<T> ( this IEnumerable<T> list, Func<T, bool> func, int afterIndex )
		{

			if ( list == null )
				throw new ArgumentNullException ( "list", "The list to be searched cannot be null" );
			if ( afterIndex < 0 )
				throw new ArgumentException ( "afterIndex", "The index from which to perform the search cannot be lower than zero" );
			if ( afterIndex > list.Count () )
				throw new ArgumentException ( "afterIndex", "The index from which to perform the search cannot greater than the number of elements in the list" );

			int i = 0;
			foreach ( var l in list.Skip ( afterIndex ) )
			{
				if ( func ( l ) )
				{
					return i;
				}
				i++;
			}
			return -1;
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Finds and returns the nth element in the collection that satisfies the function. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="func"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException" />
		/// <exception cref="ArgumentException" />
		public static T FindNth<T> ( this IEnumerable<T> list, Func<T, bool> func, int index )
		{

			if ( list == null )
				throw new ArgumentNullException ( "list", "The list to be searched cannot be null" );
			if ( func == null )
				throw new ArgumentNullException ( "func", "The search function cannot be null" );
			if ( index >= list.Count () )
				throw new ArgumentException ( "The index cannot greater than the number of elements in the list", "index" );

			var _all = list.Where ( func );
			if ( _all.HasElements () && index < list.Count () )
			{
				return _all.ElementAt ( index );
			}
			// do not like this for value types....
			return default ( T );
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Return the indices of all elements in the collection that satisfy
		/// the function passed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="func"></param>
		/// <returns></returns>
		public static IEnumerable<int> IndicesOf<T> ( this IEnumerable<T> list, Func<T, bool> func )
		{
			if ( list == null )
				throw new ArgumentNullException ( "list", "The list to be searched cannot be null" );
			if ( func == null )
				throw new ArgumentNullException ( "func", "The search function cannot be null" );
			var _indices = new List<int> ();
			var i = 0;
			foreach ( var l in list )
			{
				if ( func ( l ) )
					_indices.Add ( i );
				++i;
			}

			return _indices;
		}


		// ---------------------------------------------------------------------------------


		public static IEnumerable<T> RemoveAfter<T> ( this IEnumerable<T> list, Func<T, bool> func )
		{
			int i = 0;
			foreach ( var l in list )
			{
				if ( func ( l ) )
				{
					return list.Take ( ++i );
				}
				i++;
			}
			return list;
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns a boolean value that indicates if a char or string is contained
		/// between the marker strings passed.
		/// </summary>
		/// <param name="?"></param>
		/// <returns></returns>
		public static bool IsBetween ( this string sut, string search, string markerString, int startIndex = 0 )
		{

			// TODO: ADD EXCEPTION CONTROL
			// TODO: ADD CULTURE AND COMPARISON INFO
			var indexOfSut = search.IndexOf ( sut, startIndex );
			var indices = search.IndicesOfAll ( markerString );

			// indices must be of a even length, otherwise is not "between"
			if ( indices.Count () % 2 > 0 )
				throw new Exception ( "The marker string has to have an even number of occurrences" );

			if ( indices.Count () > 0 )
			{
				// split the indices in pairs
				var _split = indices.Split ( 2 );
				if ( indexOfSut.InBetweenAny ( _split ) )
				{
					return true;
				}
			}
			return false;
		}


		// ---------------------------------------------------------------------------------


		public static bool IsBetween ( this string sut, string search, string s1, string s2, int startIndex = 0 )
		{

			// TODO: ADD EXCEPTION CONTROL
			// TODO: ADD CULTURE AND COMPARISON INFO


			var indexOfSut = search.IndexOf ( sut, startIndex );
			var indices1 = search.IndicesOfAll ( s1 );
			var indices2 = search.IndicesOfAll ( s2 );

			// indices must be of a even length, otherwise is not "between"
			//if ( indices1.Count () % 2 > 0 )
			if ( indices1.Count () != indices2.Count () )
				throw new Exception ( "The marker string has to have an even number of occurrences" );

			if ( indices1.Count () > 0 )
			{
				var _split = new List<List<int>> ();
				for ( int i = 0; i < indices1.Count (); i++ )
					_split.Add ( new List<int> () { indices1.ElementAt ( i ), indices2.ElementAt ( i ) } );

				if ( indexOfSut.InBetweenAny ( _split ) )
				{
					return true;
				}
			}

			return false;

		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Splits a collection into a collection of collections
		/// where each element is of the same length as indicated by splitLength.
		/// Splitting a 9-element collection with a value of 3 for splitLength, yields
		/// a list of 3-element collections.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="splitLength"></param>
		/// <returns></returns>
		public static IEnumerable<IEnumerable<T>> Split<T> ( this IEnumerable<T> list, int splitLength )
		{
			if ( !list.HasElements () )
				throw new ArgumentException ( "A collection has to have items in order to be split" );
			if ( splitLength <= 0 || splitLength > list.Count () )
				throw new ArgumentException ( "Invalid split length" );

			var __list = new List<List<T>> ();
			List<T> __subList = new List<T> (); ;

			var i = 0;
			foreach ( var x in list )
			{
				if ( i > 0 && i % splitLength == 0 )
				{
					__list.Add ( __subList );
					__subList = new List<T> ();
				}

				__subList.Add ( x );
				i++;
			}

			// on exit add last split
			__list.Add ( __subList );

			return __list;

		}


		// ---------------------------------------------------------------------------------


		public static bool InBetweenAny ( this Int16 t, IEnumerable<IEnumerable<Int16>> intervals )
		{
			if ( !intervals.HasElements () )
				return false;

			foreach ( var interval in intervals )
			{
				if ( t <= interval.Max () && t >= interval.Min () )
					return true;
			}
			return false;
		}


		// ---------------------------------------------------------------------------------


		public static bool InBetweenAny ( this Int32 t, IEnumerable<IEnumerable<Int32>> intervals )
		{
			if ( !intervals.HasElements () )
				return false;

			foreach ( var interval in intervals )
			{
				if ( t <= interval.Max () && t >= interval.Min () )
					return true;
			}
			return false;
		}


		// ---------------------------------------------------------------------------------


		public static bool InBetweenAny ( this Int64 t, IEnumerable<IEnumerable<Int64>> intervals )
		{
			if ( !intervals.HasElements () )
				return false;

			foreach ( var interval in intervals )
			{
				if ( t <= interval.Max () && t >= interval.Min () )
					return true;
			}
			return false;
		}


		// ---------------------------------------------------------------------------------


		public static bool InBetweenAny ( this float t, IEnumerable<IEnumerable<float>> intervals )
		{
			if ( !intervals.HasElements () )
				return false;

			foreach ( var interval in intervals )
			{
				if ( t <= interval.Max () && t >= interval.Min () )
					return true;
			}
			return false;
		}


		// ---------------------------------------------------------------------------------


		public static bool InBetweenAny ( this Decimal t, IEnumerable<IEnumerable<Decimal>> intervals )
		{
			if ( !intervals.HasElements () )
				return false;

			foreach ( var interval in intervals )
			{
				if ( t <= interval.Max () && t >= interval.Min () )
					return true;
			}
			return false;
		}


		// ---------------------------------------------------------------------------------


		public static bool InBetweenAny ( this Double t, IEnumerable<IEnumerable<Double>> intervals, int roundToPrecision = 2 )
		{
			if ( !intervals.HasElements () )
				return false;

			foreach ( var interval in intervals )
			{
				if ( Math.Round ( t, roundToPrecision ) <= interval.Max () && Math.Round ( t, roundToPrecision ) >= interval.Min () )
					return true;
			}
			return false;
		}

		// ---------------------------------------------------------------------------------

		public static bool InBetweenAny ( this Int32 t, IEnumerable<IEnumerable<Int32>> intervals, out IEnumerable<Int32> interval )
		{
			if ( !intervals.HasElements () )
			{
				interval = null;
				return false;
			}

			foreach ( var _interval in intervals )
			{
				if ( t <= _interval.Max () && t >= _interval.Min () )
				{
					interval = _interval;
					return true;
				}
			}

			interval = null;
			return false;
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns all the elements in the collection which 
		/// indices are contained in the specifid list of indices.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <param name="indices"></param>
		/// <returns></returns>
		public static IEnumerable<T> ElementsAt<T> ( this IEnumerable<T> t, IEnumerable<int> indices )
		{
			if ( t.HasElements () )
			{
				var _list = new List<T> ();
				foreach ( var i in indices )
				{
					_list.Add ( t.ElementAt ( i ) );
				}
				return _list;
			}
			return null;
		}


		// ---------------------------------------------------------------------------------


		public static IEnumerable<T> ElementsAt<T> ( this IEnumerable<T> t, Func<T, bool> func )
		{
			if ( t.HasElements () )
			{
				var _list = new List<T> ();
				foreach ( var _t in t )
					if ( func ( _t ) )
						_list.Add ( _t );
				return _list;
			}
			return null;

		}


		// ---------------------------------------------------------------------------------


		public static IEnumerable<string> Split2 ( this string s,
												   IEnumerable<string> separators,
													StringSplitOptions options = StringSplitOptions.None,
													bool trimElements = false )
		{

			if ( String.IsNullOrEmpty ( s ) )
				return null;

			if ( !separators.HasElements () )
				throw new ArgumentNullException ( "separators", "The list must contain at least one element" );

			// get indices of all separators
			var __sepIndices = s.IndicesOfAll2 ( separators );
			var __splittedList = new List<string> ();
			var __offset = 0;
			foreach ( var i in __sepIndices )
			{
				if ( !trimElements )
					__splittedList.Add ( s.Substring ( __offset, i.Key - __offset ) );
				else
					__splittedList.Add ( s.Substring ( __offset, i.Key - __offset ).Trim () );

				__offset = i.Key + i.Value.Length;
			}

			// after last separator, add the string's remainder
			__splittedList.Add ( s.Substring ( __offset ) );

			if ( options == StringSplitOptions.RemoveEmptyEntries )
				__splittedList.RemoveAll ( x => x == String.Empty );

			return __splittedList;
		}


		// ---------------------------------------------------------------------------------


		public static IEnumerable<string> Split2 ( this string s,
												   IEnumerable<string> separators,
													string excludeBeginningString,
													string excludeEndString,
													StringSplitOptions options = StringSplitOptions.None,
													bool trimElements = false )
		{

			if ( String.IsNullOrEmpty ( s ) )
				return null;

			if ( !separators.HasElements () )
				throw new ArgumentNullException ( "separators", "The list must contain at least one element" );

			var excludedStrings = s.FindAllBetween ( excludeBeginningString, excludeEndString, true );
			// InBetweenAny
			var excludedIntervals = new List<List<int>> ();
			foreach ( var ex in excludedStrings )
			{
				excludedIntervals.Add ( new List<int> () { s.IndexOf ( ex ), ex.Length + s.IndexOf ( ex ) } );
			}

			// get indices of all separators
			var __sepIndices = s.IndicesOfAll2 ( separators );
			var __splittedList = new List<string> ();
			var __offset = 0;
			IEnumerable<int> __chosenInterval = new List<int> ();
			bool __controlFlag = false;
			foreach ( var i in __sepIndices )
			{
				if ( i.Key.InBetweenAny ( excludedIntervals, out __chosenInterval ) )
				{
					if ( !__controlFlag )
					{
						__splittedList.Add ( s.Substring ( __chosenInterval.Min (), __chosenInterval.Max () - __chosenInterval.Min () ) );
						__offset = __chosenInterval.Max ();
						__controlFlag = true;
					}
				}
				else
				{
					__controlFlag = false;
					if ( !trimElements )
						__splittedList.Add ( s.Substring ( __offset, i.Key - __offset ) );
					else
						__splittedList.Add ( s.Substring ( __offset, i.Key - __offset ).Trim () );
					__offset = i.Key + i.Value.Length;
				}
			}

			// after last separator, add the string's remainder
			__splittedList.Add ( s.Substring ( __offset ) );

			if ( options == StringSplitOptions.RemoveEmptyEntries )
				__splittedList.RemoveAll ( x => x == String.Empty );

			return __splittedList;
		}


		// ---------------------------------------------------------------------------------


		public static IEnumerable<string> Split2 ( this string s,
												   IEnumerable<string> separators,
													IEnumerable<Tuple<string, string>> excludingPairs,
													StringSplitOptions options = StringSplitOptions.None,
													bool trimElements = false )
		{

			if ( String.IsNullOrEmpty ( s ) )
				return new List<string> ();

			if ( !separators.HasElements () )
				throw new ArgumentNullException ( "separators", "The list must contain at least one element" );


			var excludedStrings = new List<string> ();
			foreach ( var t in excludingPairs )
				excludedStrings.AddRange ( s.FindAllBetween ( t.Item1, t.Item2, true ) );


			// InBetweenAny
			var excludedIntervals = new List<List<int>> ();
			foreach ( var ex in excludedStrings )
			{
				excludedIntervals.Add ( new List<int> () { s.IndexOf ( ex ), ex.Length + s.IndexOf ( ex ) } );
			}

			// get indices of all separators
			var __sepIndices = s.IndicesOfAll2 ( separators );
			var __splittedList = new List<string> ();
			var __offset = 0;
			IEnumerable<int> __chosenInterval = new List<int> ();
			bool __controlFlag = false;
			foreach ( var i in __sepIndices )
			{
				if ( i.Key.InBetweenAny ( excludedIntervals, out __chosenInterval ) )
				{
					if ( !__controlFlag )
					{
						__splittedList.Add ( s.Substring ( __chosenInterval.Min (), __chosenInterval.Max () - __chosenInterval.Min () ) );
						__offset = __chosenInterval.Max ();
						__controlFlag = true;
					}
				}
				else
				{
					__controlFlag = false;
					if ( !trimElements )
						__splittedList.Add ( s.Substring ( __offset, i.Key - __offset ) );
					else
						__splittedList.Add ( s.Substring ( __offset, i.Key - __offset ).Trim () );
					__offset = i.Key + i.Value.Length;
				}
			}

			// after last separator, add the string's remainder
			__splittedList.Add ( s.Substring ( __offset ) );

			if ( options == StringSplitOptions.RemoveEmptyEntries )
				__splittedList.RemoveAll ( x => x.Trim () == String.Empty );

			return __splittedList;
		}


		// ---------------------------------------------------------------------------------



	}
}
