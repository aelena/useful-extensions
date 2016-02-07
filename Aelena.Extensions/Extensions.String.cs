using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aelena.SimpleExtensions.ListExtensions;
using Aelena.SimpleExtensions.ObjectExtensions;

namespace Aelena.SimpleExtensions.StringExtensions
{
	public static class Extensions
	{


		/// <summary>
		/// Provides a safe version of ToString()
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
		/// Provides a safe version of ToString()
		/// and allows the caller to specify a default return value so that if the
		/// string is null the return value will be used.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="defaultValue"></param>
		/// <returns>Returns the object ToString output or the provided string
		/// value if the object is null.</returns>
		public static string ToStringSafe ( this String str, string defaultValue )
		{
			return ( null == str ) ? ( ( null == defaultValue ) ? String.Empty : defaultValue ) : str;
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Provides a safe version of Substring operation
		/// so that if the object is null an empty string is returned.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
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
		/// passed as an argument. By default it examines only Public and Instance fields.
		/// </summary>
		/// <param name="_string">templated string</param>
		/// <param name="instance">instance containing value</param>
		/// <returns></returns>
		public static string Interpolate ( this string _string, object instance )
		{
			string _interpolated = "";
			if ( !( String.IsNullOrEmpty ( _string ) ) && instance != null )
			{
				foreach ( var x in from x in instance.GetType ().GetProperties ( BindingFlags.Public | BindingFlags.Instance ) select x.Name )
					_interpolated = _interpolated.Replace ( "#{" + x + "}", instance.GetType ().GetProperty ( x ).GetValue ( instance, null ).ToStringSafe () );
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
		public static string TakeBetween ( this string value, string beginningString, string endString, bool trimResults = false )
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

			if ( trimResults )
				return value.Substring ( _index1, _index2 - _index1 ).Trim ();

			return value.Substring ( _index1, _index2 - _index1 );


		}


		// ---------------------------------------------------------------------------------


        public static IEnumerable<string> TakeBetweenMultiple ( this string value, string beginningString, string endString, bool trimResults = false )
        {
            var _results = new List<string> ();

            string @string = "";
            while ( !String.IsNullOrEmpty (value) && !String.IsNullOrEmpty ( @string = value.TakeBetween ( beginningString, endString, trimResults ) ) )
            {
                // now, make original string shorter, by removing found part
                value = value.SubStringAfter ( endString );
                _results.Add ( @string );
            }

            return _results;
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


		/// <summary>
		/// Returns a new string that represent whatever is between the first and the second marker string.
		/// It's like a Substring operation, only that it works by indicating other strings.
		/// This overload is different as it allows the caller to specify an initial marker string so that the
		/// search is performed only after that marker string's position.
		/// not indices or positions.
		/// </summary>
		/// <param name="value">String to search.</param>
		/// <param name="markString">marker strings that marks the initial substring for comparison</param>
		/// <param name="beginningString"></param>
		/// <param name="endString"></param>
		/// <param name="trimResults"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="IndexOutOfRangeException"/>
		public static string TakeBetween ( this string value, string markString, string beginningString, string endString, bool trimResults = false )
		{
			if ( String.IsNullOrWhiteSpace ( value ) )
				throw new ArgumentNullException ( "value", "String cannot be null" );
			if ( String.IsNullOrEmpty ( markString ) )
				throw new ArgumentNullException ( "markString", "String cannot be null or white space" );
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
				throw new IndexOutOfRangeException ( "End string cannot appear earlier than beginning string" );

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
		/// Returns a new string with all the occurrences of the specified list of
		/// string values removed.
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


		/// <summary>
		/// Returns a new string where only the first occurrence of a string is replaced
		/// by the designed replacement string.
		/// </summary>
		/// <param name="subject">Instance on which to perform the replacement.</param>
		/// <param name="occurrenceToRemove">String to be replaced.</param>
		/// <param name="replacement">Replacement string.</param>
		/// <returns>A new string where only the first occurrence of a string is replaced
		/// by the designed replacement string.</returns>
		/// <exception cref="ArgumentNullException"/>
		public static string ReplaceFirst ( this string subject, string occurrenceToRemove, string replacement = "" )
		{

			if ( String.IsNullOrWhiteSpace ( subject ) )
				throw new ArgumentNullException ( "subject", "String cannot be null" );
			if ( String.IsNullOrEmpty ( occurrenceToRemove ) )
				throw new ArgumentNullException ( "occurrenceToRemove", "String cannot be null" );

			var i = subject.IndexOf ( occurrenceToRemove );

			if ( i >= 0 )
				return string.Format ( "{0}{1}{2}", subject.Truncate ( i ), replacement, subject.TakeFrom ( i + occurrenceToRemove.Length ) );
			return subject;

		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns a new string where only the last occurrence of a string is replaced
		/// by the designed replacement string.
		/// </summary>
		/// <param name="subject">Instance on which to perform the replacement.</param>
		/// <param name="occurrenceToRemove">String to be replaced.</param>
		/// <param name="replacement">Replacement string.</param>
		/// <returns>A new string where only the last occurrence of a string is replaced
		/// by the designed replacement string.</returns>
		/// <exception cref="ArgumentNullException"/>
		public static string ReplaceLast ( this string subject, string occurrenceToRemove, string replacement = "" )
		{

			if ( String.IsNullOrWhiteSpace ( subject ) )
				throw new ArgumentNullException ( "subject", "String cannot be null" );
			if ( String.IsNullOrEmpty ( occurrenceToRemove ) )
				throw new ArgumentNullException ( "occurrenceToRemove", "String cannot be null" );

			var i = subject.LastIndexOf ( occurrenceToRemove );
			if ( i >= 0 )
                return string.Format ( "{0}{1}{2}", subject.Truncate ( i ), replacement, subject.TakeFrom ( i + occurrenceToRemove.Length ) );
			return subject;

		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns a new string where only the first and the last occurrences of a string is replaced
		/// by the designed replacement string.
		/// </summary>
		/// <param name="subject">Instance on which to perform the replacement.</param>
		/// <param name="occurrenceToRemove">String to be replaced.</param>
		/// <param name="replacement">Replacement string.</param>
		/// <returns>A new string where only the first and the last occurrence of a string is replaced
		/// by the designed replacement string.</returns>
		/// <exception cref="ArgumentNullException"/>
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
		/// within this instance has been removed.
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
				return string.Format ( "{0}{1}{2}", subject.Truncate ( i ), "", subject.TakeFrom ( i + removee.Length ) );
			return subject;
		}

		// ---------------------------------------------------------------------------------



		/// <summary>
		/// Returns a list of integers that indicate all the positions
		/// where the search string has been found in the original string.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="search"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		public static IEnumerable<int> IndicesOfAll ( this string value, string search, bool ignoreCase = false )
		{
			// TODO: ADD EXCEPTION CONTROL
			// TODO: ADD CULTURE AND COMPARISON INFO

			var i = 0;
			var _indices = new List<int> ();
			if ( !ignoreCase )
			{
				while ( ( i = value.IndexOf ( search, i ) ) != -1 )
				{
					_indices.Add ( i++ );
				}
			}
			else
			{
				while ( ( i = value.ToUpperInvariant ().IndexOf ( search.ToUpperInvariant (), i ) ) != -1 )
				{
					_indices.Add ( i++ );
				}
			}
			return _indices;
		}



		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns a new string that is a substring of all the characters
		/// between the opening and closing marking characters provided.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <param name="includeMarkers"></param>
		/// <returns></returns>
		public static IEnumerable<String> FindAllBetween ( this string value, string s1, string s2, bool includeMarkers = false )
		{
			int _1 = 0;
			var _occurrences = new List<String> ();

			if ( !includeMarkers )
			{
				while ( ( _1 = value.IndexOf ( s1, _1 ) ) != -1 )
				{
					var _2 = value.IndexOf ( s2, _1 + 1 );
					if ( _2 != -1 )
					{
						_occurrences.Add ( value.Substring ( ++_1, _2 - _1 ) );
					}
					_1 += _2 - _1;
					if ( _1 == -1 )
						break;
				}
			}
			else
			{
				while ( ( _1 = value.IndexOf ( s1, _1 ) ) != -1 )
				{
					var _2 = value.IndexOf ( s2, _1 + 1 );
					if ( _2 != -1 )
					{
						//_occurrences.Add ( value.Substring ( _1, ++_2 - _1 ) );
						_occurrences.Add ( value.Substring ( _1, _2 + s2.Length - _1 ) );
					}
					_1 += _2 - _1;
					if ( _1 == -1 )
						break;
				}
			}
			return _occurrences;
		}


		// ---------------------------------------------------------------------------------


		/// <summary>
		/// Returns a list of key-value pairs that indicate the positions and the strings themselves
		/// from the list of strings to be searched for that have been found on the searched string.
		/// </summary>
		/// <param name="value">String to be searched.</param>
		/// <param name="searches">List of strings to be searched for in the instance.</param>
		/// <param name="ignoreCase">Boolean parameter to indicate if the search is to be case 
		/// sensitive or not.</param>
		/// <returns>A list of <typeparamref name="KeyValuePair"/> with the index of string
		/// of each occurrence.</returns>
		public static IEnumerable<KeyValuePair<int, string>> IndicesOfAll2 ( this string value, IEnumerable<string> searches, bool ignoreCase = false )
		{
			// TODO: ADD EXCEPTION CONTROL
			// TODO: ADD CULTURE AND COMPARISON INFO

			if ( String.IsNullOrEmpty ( value ) )
				return null;

			if ( !searches.HasElements () )
				throw new ArgumentNullException ( "searches", "The list must contain at least one element" );

			var i = 0;
			var _indices = new List<KeyValuePair<int, string>> ();
			foreach ( var search in searches )
			{
				// reset counter here otherwise further operations fail
				i = 0;
				if ( !ignoreCase )
				{
					while ( ( i = value.IndexOf ( search, i ) ) != -1 )
					{
						_indices.Add ( new KeyValuePair<int, string> ( i++, search ) );
					}
				}
				else
				{
					while ( ( i = value.ToUpperInvariant ().IndexOf ( search.ToUpperInvariant (), i ) ) != -1 )
					{
						_indices.Add ( new KeyValuePair<int, string> ( i++, search ) );
					}
				}
			}

			return _indices;
		}


		// ---------------------------------------------------------------------------------


        /// <summary>
        /// Truncates a string to a maximum length.
        /// </summary>
        /// <param name="value">The string to truncate.</param>
        /// <param name="length">The maximum length of the returned string.</param>
        /// <returns>The input string, truncated to <paramref name="length"/> characters.</returns>
        public static string Truncate ( this string value, int length )
        {
            if ( value == null )
                throw new ArgumentNullException ( "value" );
            return value.Length <= length ? value : value.Substring ( 0, length );
        }


        // ---------------------------------------------------------------------------------


        /// <summary>
        /// Truncates a string to a maximum length.
        /// </summary>
        /// <param name="value">The string to truncate.</param>
        /// <param name="length">The maximum length of the returned string.</param>
        /// <returns>The input string, truncated to <paramref name="length"/> characters.</returns>
        public static string TakeFrom ( this string value, int position )
        {
            if ( value == null )
                throw new ArgumentNullException ( "value" );
            return position > value.Length ? value : value.Substring ( position );
        }

        
        // ---------------------------------------------------------------------------------


        public static string FromCharListToString ( this IEnumerable<char> source )
        {
            if ( source == null )
                throw new ArgumentNullException ( "source" );
            return new string ( source.ToArray () );
        }


        // ---------------------------------------------------------------------------------

	}
}


