using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aelena.SimpleExtensions.StringExtensions
{
	public static class Extensions
	{


		/// <summary>
		/// This is an extension method that provides a safe version of ToString
		/// so that if the string is null an empty string is returned 
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToStringSafe ( this String str )
		{
			return ToStringSafe ( str, String.Empty );
		}


		  // ---------------------------------------------------------------------------------


		/// <summary>
		/// This is an extension method that provides a safe version of ToString
		/// so that if the object is null an empty string is returned 
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToStringSafe ( this Object obj )
		{
			return ToStringSafe ( obj, String.Empty );
		}


		  // ---------------------------------------------------------------------------------


		/// <summary>
		/// This is an extension method that provides a safe version of ToString
		/// and allows the caller to specify a default return value so that if the
		/// string is null the return value will be used.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static string ToStringSafe ( this object obj, string defaultValue )
		{
			return ( null == obj ) ? ( ( null == defaultValue ) ? String.Empty : defaultValue ) : Convert.ToString ( obj );
		}


		  // ---------------------------------------------------------------------------------


		/// <summary>
		/// This is an extension method that provides a safe version of to string
		/// and allows the caller to specify a default return value so that if the
		/// string is null the return value will be used.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static string ToStringSafe ( this String str, string defaultValue )
		{
			return ( null == str ) ? ( ( null == defaultValue ) ? String.Empty : defaultValue ) : str;
		}


		  // ---------------------------------------------------------------------------------


		public static string SubstringSafe ( this String str, int startIndex, int length )
		{
			if ( !String.IsNullOrEmpty ( str ) && startIndex >= 0 && str.Length >= startIndex + length )
			{
				return str.Substring ( startIndex, length );
			}
			return String.Empty;
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


		/// <summary>
		/// Generics-based method that extends a String by enabling it 
		/// to return a different string representation corresponding 
		/// to type T specified.
		/// <example>
		/// <![CDATA[ 
		/// String decimalString = "1277,4848";
		/// var returnValue = decimalString.ParseToStringSafe<Decimal> ( "N2" );
		/// returnValue is now "1.277,48"
		/// ]]>
		/// </example>
		/// </summary>
		/// <param name="s"></param>
		/// <param name="t"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string ParseToStringSafe<T> ( this String s, string format = "", string defaultValue = "" ) where T : IConvertible
		{

			try
			{

				if ( !String.IsNullOrEmpty ( s ) )
				{
					if ( String.IsNullOrEmpty ( format ) )
						return ( ( T ) Convert.ChangeType ( s, typeof ( T ) ) ).ToString ();
					else
					{
						var _t = ( T ) Convert.ChangeType ( s, typeof ( T ) );
						if ( _t is Decimal )
							return _t.ToDecimal ( null ).ToString ( format );
						if ( _t is Int16 )
							return _t.ToInt16 ( null ).ToString ( format );
						if ( _t is UInt16 )
							return _t.ToUInt16 ( null ).ToString ( format );
						if ( _t is Int32 )
							return _t.ToInt32 ( null ).ToString ( format );
						if ( _t is UInt32 )
							return _t.ToUInt32 ( null ).ToString ( format );
						if ( _t is Int64 )
							return _t.ToInt64 ( null ).ToString ( format );
						if ( _t is UInt64 )
							return _t.ToUInt64 ( null ).ToString ( format );
						if ( _t is Double )
							return _t.ToDouble ( null ).ToString ( format );
						if ( _t is Single )
							return _t.ToSingle ( null ).ToString ( format );
						if ( _t is DateTime )
							return _t.ToDateTime ( null ).ToString ( format );
					}
				}
			}
			catch ( Exception ex )
			{
				// if conversion fails, return "", basically because we want this 
				// to not break caller's code
				return s;
			}

			if ( !String.IsNullOrEmpty ( defaultValue ) )
				return defaultValue;
			return "";

		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Extension method to interpolate a templated string with actual values from and object
		/// passed as an argument.
		/// </summary>
		/// <param name="_string">templated string</param>
		/// <param name="o">instance containing value</param>
		/// <returns></returns>
		public static string Interpolate ( this string _string, object o )
		{
			string _interpolated = "";
			if ( !( String.IsNullOrEmpty ( _string ) ) && o != null )
			{
				foreach ( var x in from x in o.GetType ().GetProperties ( BindingFlags.Public | BindingFlags.Instance ) select x.Name )
					_interpolated = _interpolated.Replace ( "#{" + x + "}", o.GetType ().GetProperty ( x ).GetValue ( o, null ).ToStringSafe () );
			}
			return _interpolated;
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Removes all diacritics for a given string.
		/// </summary>
		/// <param name="inputString"></param>
		/// <returns></returns>
		public static string RemoveDiacritics ( this string inputString )
		{


			string stFormD = inputString.Normalize ( NormalizationForm.FormD );
			StringBuilder sb = new StringBuilder ();

			for ( int ich = 0; ich < stFormD.Length; ich++ )
			{
				UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory ( stFormD [ ich ] );
				if ( uc != UnicodeCategory.NonSpacingMark )
					sb.Append ( stFormD [ ich ] );
			}

			return sb.ToString ().Normalize ( NormalizationForm.FormC );
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Extension method to see if any of the strings in the 'searches' collection
		/// is contained in the list. It returns true on the first occurrence.
		/// </summary>
		/// <param name="searchee"></param>
		/// <param name="searches">List of strings to be searched for in the instance.</param>
		/// <param name="wordBoundaries">Boolean optional parameter to indicate if the ocurrence
		/// is required to be a word or not. By default it is false.</param>
		/// <returns>Returns a boolean value that indicates if any of the strings
		/// was found in the instance.</returns>
		/// <exception cref="ArgumentNullException"/>
		public static bool ContainsAny ( this string searchee, IEnumerable<string> searches, bool wordBoundaries = false )
		{
			if ( searchee == null )
				throw new ArgumentNullException ( "the string cannot be null.", "searchee" );

			if ( !wordBoundaries )
			{
				if ( searches != null )
					foreach ( var s in searches )
						if ( searchee.Contains ( s ) )
							return true;
			}
			else
			{
				if ( searches != null )
				{
					foreach ( var s in searches )
					{
						var _rx = "\\b" + s + "\\b";
						var _matches = Regex.Matches ( searchee, _rx );
						if ( _matches != null && _matches.Count > 0 )
							return true;
					}
				}
			}

			return false;

		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Extension method to see if any of the 'searches' elements
		/// is contained in the list.
		/// The first occurrence found is returned.
		/// </summary>
		/// <param name="searchee"></param>
		/// <param name="searches"></param>
		/// <param name="wordBoundaries"></param>
		/// <returns>Returns a boolean value to indicate if the search was successful
		/// and returns also the value itself.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static string GetFirstOccurrence ( this string searchee, IEnumerable<string> searches, bool wordBoundaries = false )
		{
			if ( searchee == null )
				throw new ArgumentNullException ( "the string cannot be null.", "searchee" );

			if ( !wordBoundaries )
			{
				if ( searches != null )
					foreach ( var s in searches )
						if ( searchee.Contains ( s ) )
							return s;
			}
			else
			{
				if ( searches != null )
				{
					foreach ( var s in searches )
					{
						var _rx = "\\b" + s + "\\b";
						var _matches = Regex.Matches ( searchee, _rx );
						if ( _matches != null && _matches.Count > 0 )
							return s;
					}
				}
			}

			return String.Empty;

		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Prepends a given string to another string a number of times 
		/// as specified by the repetitions parameter, which is 1 by default.
		/// (by default 1).
		/// </summary>
		/// <param name="s"></param>
		/// <param name="prependValue">Value to prepend to the string. If it is null, 
		/// it is then assumed to be an empty string and there is no exception thrown.</param>
		/// <param name="repetitions">number of times the string will be prepended.</param>
		/// <returns>A new string with the prepended value(s).</returns>
		public static string Prepend ( this string s, string prependValue, int repetitions = 1 )
		{
			if ( s == null )
				s = "";
			if ( prependValue == null )
				prependValue = "";

			if ( repetitions < 0 )
				repetitions = 1;

			for ( int i = 0; i < repetitions; i++ )
				s = prependValue + s;

			return s;
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Appends a given string to another string a number of times 
		/// as specified by the repetitions parameter, which is 1 by default.
		/// (by default 1).
		/// </summary>
		/// <param name="s"></param>
		/// <param name="prependValue">Value to prepend to the string. If it is null, 
		/// it is then assumed to be an empty string and there is no exception thrown.</param>
		/// <param name="repetitions">number of times the string will be prepended.</param>
		/// <returns>A new string with the appended value(s).</returns>
		public static string Append ( this string s, string appendValue, int repetitions = 1 )
		{
			if ( s == null )
				s = "";

			if ( appendValue == null )
				appendValue = "";

			if ( repetitions < 0 )
				repetitions = 1;

			for ( int i = 0; i < repetitions; i++ )
				s = s + appendValue;

			return s;
		}


		// ---------------------------------------------------------------------------------

		/// <summary>
		/// This extension removes n number of characters starting from the end of the string.
		/// If the string is null or empty, it returns String.Empty, therefore it does not crash on null.
		/// 
		/// If the number of characters to substract from the end is bigger that the actual
		/// character count in the string it returns String.Empty, therefore it does not crash.
		/// 
		/// Caller can specify in a end trim is wanted or not, in case whitespace needs to be preserved.
		/// By default this is assumed to be false, so pass true for the trimming to take effect.
		/// </summary>
		/// <param name="value">String on which to perform the operation.</param>
		/// <param name="numberOfCharacters">Number of characters to remove from the end of the string.</param>
		/// <param name="trimEndFirst">Boolean value that indicates if a TrimEnd is to be done 
		/// before the removal.</param>
		/// <returns></returns>
		public static string RemoveFromEnd ( this string value, int numberOfCharacters, bool trimEndFirst = false )
		{
			if ( String.IsNullOrEmpty ( value ) )
				return string.Empty;

			if ( trimEndFirst )
				value = value.TrimEnd ();

			if ( numberOfCharacters > value.Length )
				return String.Empty;

			return value.Remove ( value.Length - numberOfCharacters );

		}


		// ---------------------------------------------------------------------------------



		/// <summary>
		/// Makes a substring operation where the criteria is that the
		/// substring starts from the end of the first occurrence of the mark string.
		/// 
		/// So in a string like 'the quick brown fox jumps over the lazy dog' 
		/// where the mark string is 'fox', returns ' jumps over the lazy dog'
		/// including the white space or not depending on the optional parameter, 
		/// for which the default is false.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="markString"></param>
		/// <param name="trimResults"></param>
		/// <returns></returns>
		public static string SubStringAfter ( this string value, string markString, StringComparison comparisonOptions = StringComparison.CurrentCulture, bool trimResults = false )
		{
			if ( String.IsNullOrEmpty ( value ) )
				return String.Empty;

			var _index = value.IndexOf ( markString, comparisonOptions );
			if ( _index >= 0 )
			{
				var s = value.Substring ( _index + markString.Length );
				if ( trimResults )
					return s.Trim ();
				return s;
			}

			return value;
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Makes a substring operation where the criteria is that the
		/// substring starts from the end of last ocurrence of the mark string.
		/// 
		/// So in a string like 'the quick brown fox jumps over the lazy dog' 
		/// where the mark string is 'fox', returns ' jumps over the lazy dog'
		/// including the white space or not depending on the optional parameter, 
		/// for which the default is false.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="markString"></param>
		/// <param name="trimResults"></param>
		/// <returns></returns>
		public static string SubStringAfterLast ( this string value, string markString,
			StringComparison comparisonOptions = StringComparison.CurrentCulture, bool trimResults = false )
		{
			if ( String.IsNullOrEmpty ( value ) )
				return String.Empty;

			var _index = value.LastIndexOf ( markString, comparisonOptions );
			if ( _index >= 0 )
			{
				var s = value.Substring ( _index + markString.Length );
				if ( trimResults )
					return s.Trim ();
				return s;
			}

			return value;
		}

		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns the substring contained between two marker strings.
		/// </summary>
		/// <param name="value">String instance to be searched.</param>
		/// <param name="beginningString">String marking the beginning of the search.</param>
		/// <param name="endString">String marking the end of the search.</param>
		/// <returns>The substring contained between the two markers, if found, or empty string
		/// otherwise.</returns>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="Exception" />
		public static string TakeBetween ( this string value, string beginningString, string endString )
		{
			if ( String.IsNullOrWhiteSpace ( value ) )
				throw new ArgumentNullException ( "String instance cannot be null" );

			if ( String.IsNullOrEmpty ( beginningString ) )
				throw new ArgumentNullException ( "beginningString", "String instance cannot be null or empty" );

			if ( String.IsNullOrEmpty ( beginningString ) )
				throw new ArgumentNullException ( "endString", "String instance cannot be null or empty" );

			int _index1 = 0;
			int _index2 = 0;
			if ( beginningString != endString )
			{
				_index1 = value.IndexOf ( beginningString );
				_index2 = value.IndexOf ( endString );
			}
			else
			{
				_index1 = value.IndexOf ( beginningString );
				_index2 = value.LastIndexOf ( endString );
			}

			if ( _index2 < _index1 )
				throw new Exception ( "End string cannot appear earlier than beginning string" );

			if ( _index1 == -1 && _index2 == -1 )
				return String.Empty;

			_index1 += beginningString.Length;

			return value.Substring ( _index1, _index2 - _index1 );


		}


		// ---------------------------------------------------------------------------------



		/// <summary>
		/// Removes all ocurrences of a string in the interval denoted by the 
		/// beginningString and endString markers.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="stringToRemove"></param>
		/// <param name="beginningString"></param>
		/// <param name="endString"></param>
		/// <returns></returns>
		public static string RemoveBetween ( this string value, string stringToRemove, string beginningString, string endString )
		{
			return value.RemoveBetween ( new List<string> { stringToRemove }, beginningString, endString );

		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Removes all ocurrences of a string in the interval denoted by the 
		/// beginningString and endString markers.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="stringToRemove"></param>
		/// <param name="beginningString"></param>
		/// <param name="endString"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"/>
		public static string RemoveBetween ( this string value, IEnumerable<string> stringsToRemove, string beginningString, string endString )
		{
			if ( String.IsNullOrWhiteSpace ( value ) )
				throw new ArgumentException ( "String cannot be null" );

			if ( String.IsNullOrEmpty ( beginningString ) )
				throw new ArgumentNullException ( "beginningString", "String instance cannot be null or empty" );

			if ( String.IsNullOrEmpty ( beginningString ) )
				throw new ArgumentNullException ( "endString", "String instance cannot be null or empty" );

			if ( stringsToRemove == null )
				return value;

			var _index1 = value.IndexOf ( beginningString );
			var _index2 = value.IndexOf ( endString );

			if ( _index1 == -1 || _index2 == -1 )
				return value;

			if ( _index2 < _index1 )
				throw new Exception ( "End string cannot appear earlier than beginning string" );

			var _subset = value.Substring ( _index1, ( _index2 - _index1 ) + endString.Length );
			foreach ( var s in stringsToRemove )
				_subset = _subset.Replace ( s, String.Empty );

			var _ = String.Format ( "{0}{1}{2}", value.Substring ( 0, _index1 ), _subset, value.Substring ( _index2 + endString.Length ) );
			return _;

		}


		// ---------------------------------------------------------------------------------


		public static string TakeBetween ( this string value, string markString, string beginningString, string endString, bool trimResults = false )
		{
			if ( String.IsNullOrWhiteSpace ( value ) )
				throw new ArgumentException ( "value", "String cannot be null" );
			if ( String.IsNullOrWhiteSpace ( markString ) )
				throw new ArgumentException ( "markString", "String cannot be null or white space" );
			if ( String.IsNullOrEmpty ( beginningString ) )
				throw new ArgumentNullException ( "beginningString", "String instance cannot be null or empty" );
			if ( String.IsNullOrEmpty ( beginningString ) )
				throw new ArgumentNullException ( "endString", "String instance cannot be null or empty" );

			value = value.SubStringAfter ( markString );

			var _index1 = value.IndexOf ( beginningString );
			var _index2 = value.IndexOf ( endString );

			if ( _index1 == -1 || _index2 == -1 )
				return value;

			if ( _index2 < _index1 )
				throw new Exception ( "End string cannot appear earlier than beginning string" );

			_index1 += beginningString.Length;

			if ( trimResults )
				return value.Substring ( _index1, _index2 - _index1 ).Trim ();

			return value.Substring ( _index1, _index2 - _index1 );


		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns a new string where all ocurrences of a specified string in the instance are 
		/// replaced with another string value, but only when they have word boundaries, that is
		/// any number of spaces or tabs.
		/// </summary>
		/// <param name="subject">String to replace occurrences in.</param>
		/// <param name="previousValue">Previous value which all occurrences have to be replaced</param>
		/// <param name="newValue">New value to be inserted in the place of the occurrences of oldValue</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"/>
		public static string ReplaceWord ( this string subject, string previousValue, string newValue )
		{
			if ( String.IsNullOrEmpty ( previousValue ) )
				throw new ArgumentNullException ( "previousValue", "String instance cannot be null or empty" );
			if ( null == newValue )
				throw new ArgumentNullException ( "previousValue", "String instance cannot be null" );

			var _rx = @"([\t\s]|\b)+" + previousValue + @"([\t\s)]|\b)+";
			var _matches = Regex.Matches ( subject, _rx );
			if ( _matches != null && _matches.Count > 0 )
			{
				var _replacements = new List<Tuple<string, string>> ();
				for ( int i = 0; i < _matches.Count; i++ )
					_replacements.Add ( new Tuple<string, string> ( _matches [ i ].Value, _matches [ i ].Value.Replace ( previousValue, newValue ) ) );

				foreach ( var s in _replacements )
					subject = subject.Replace ( s.Item1, s.Item2 );

				return subject;
			}

			return subject;

		}



		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns a new string with all the occurrences of the specified list removed.
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="occurrencesToRemove"></param>
		/// <returns></returns>
		public static string MultipleRemove ( this string subject, IEnumerable<string> occurrencesToRemove )
		{
			if ( String.IsNullOrWhiteSpace ( subject ) )
				throw new ArgumentException ( "subject", "String cannot be null" );

			if ( occurrencesToRemove == null || occurrencesToRemove.Count () == 0 )
				return subject;

			foreach ( var o in occurrencesToRemove )
				subject = subject.Replace ( o, "" );

			return subject;
		}


		// ---------------------------------------------------------------------------------

		public static string Take ( this string subject, int index )
		{
			if ( String.IsNullOrWhiteSpace ( subject ) )
				throw new ArgumentException ( "String cannot be null (subject)" );
			if ( index < 0 )
				throw new ArgumentException ( "Index cannot be zero or less than zero." );

			return subject.Substring ( 0, index );

		}


		// ---------------------------------------------------------------------------------


		public static string Skip ( this string subject, int index )
		{
			if ( String.IsNullOrWhiteSpace ( subject ) )
				throw new ArgumentException ( "String cannot be null (subject)" );
			if ( index < 0 )
				throw new ArgumentException ( "Index cannot be zero or less than zero." );
			if ( index > subject.Length )
				throw new ArgumentException ( "Index cannot be bigger than actual string length" );

			return subject.Substring ( index );

		}


		// ---------------------------------------------------------------------------------
		public static string ReplaceFirst ( this string subject, string occurrenceToRemove, string replacement = "" )
		{

			if ( String.IsNullOrWhiteSpace ( subject ) )
				throw new ArgumentException ( "String cannot be null (subject)" );
			if ( String.IsNullOrWhiteSpace ( occurrenceToRemove ) )
				throw new ArgumentException ( "String cannot be null (occurrenceToRemove)" );

			var i = subject.IndexOf ( occurrenceToRemove );

			if ( i >= 0 )
				return string.Format ( "{0}{1}{2}", subject.Take ( i ), replacement, subject.Skip ( i + occurrenceToRemove.Length ) );
			return subject;

		}


		// ---------------------------------------------------------------------------------


		public static string ReplaceLast ( this string subject, string occurrenceToRemove, string replacement = "" )
		{

			if ( String.IsNullOrWhiteSpace ( subject ) )
				throw new ArgumentException ( "String cannot be null (subject)" );
			if ( String.IsNullOrWhiteSpace ( occurrenceToRemove ) )
				throw new ArgumentException ( "String cannot be null (occurrenceToRemove)" );

			var i = subject.LastIndexOf ( occurrenceToRemove );
			if ( i >= 0 )
				return string.Format ( "{0}{1}{2}", subject.Take ( i ), replacement, subject.Skip ( i + occurrenceToRemove.Length ) );
			return subject;

		}


		// ---------------------------------------------------------------------------------


		public static string ReplaceFirstAndLastOnly ( this string subject, string occurrenceToRemove, string replacement = "" )
		{
			if ( String.IsNullOrWhiteSpace ( subject ) )
				throw new ArgumentException ( "String cannot be null (subject)" );
			if ( String.IsNullOrWhiteSpace ( occurrenceToRemove ) )
				throw new ArgumentException ( "String cannot be null (occurrenceToRemove)" );

			subject = subject.ReplaceFirst ( occurrenceToRemove, replacement );
			subject = subject.ReplaceLast ( occurrenceToRemove, replacement );

			return subject;
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns a new string in which the last ocurrence of a specified string 
		/// within this instance has been removed
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="removee">string to remove from the instance.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"/>
		public static string RemoveLast ( this string subject, string removee )
		{
			if ( subject == null )
				throw new ArgumentNullException ( "String cannot be null", "subject" );
			if ( removee == null )
				throw new ArgumentNullException ( "String cannot be null", "remove" );

			if ( subject == String.Empty && removee == String.Empty )
				return String.Empty;

			var i = subject.LastIndexOf ( removee );
			if ( i >= 0 )
				return string.Format ( "{0}{1}{2}", subject.Take ( i ), "", subject.Skip ( i + removee.Length ) );
			return subject;
		}

		// ---------------------------------------------------------------------------------

	}
}
