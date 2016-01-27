using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Aelena.SimpleExtensions.ListExtensions;
using Aelena.SimpleExtensions.StringExtensions;
using Aelena.SimpleExtensions.ObjectExtensions;

namespace Aelena.SimpleExtensions.Tests
{
	[TestFixture]
	public class ExtensionsTests
	{


		[Test]
		public void NullStringIsSafelyReturnedAsEmptyString ()
		{
			String _str = null;
			Assert.IsTrue ( String.Empty.Equals ( _str.ToStringSafe () ) );
		}

		[Test]
		public void NonNullStringIsSafelyReturnedAsItself ()
		{
			String _str = "aAa_123";
			Assert.IsTrue ( "aAa_123".Equals ( _str.ToStringSafe () ) );
		}

		[Test]
		public void NullObjectIsSafelyReturnedAsEmptyString ()
		{
			Object _o = null;
			Assert.IsTrue ( String.Empty.Equals ( _o.ToStringSafe () ) );
		}

		[Test]
		public void NullStringIsSafelyReturnedAsDefaultValue ()
		{
			String _str = null;
			string _defValue = "_";
			Assert.IsTrue ( _defValue.Equals ( _str.ToStringSafe ( "_" ) ) );
		}


		[Test]
		public void NullObjectIsSafelyReturnedAsDefaultValue ()
		{
			Object _o = null;
			string _defValue = " ";
			Assert.IsTrue ( _defValue.Equals ( _o.ToStringSafe ( _defValue ) ) );
		}

		[Test]
		public void NullObjectIsSafelyReturnedAsEmptyStringWhenDefaultValueIsNull ()
		{
			Object _o = null;
			string _defValue = null;
			Assert.IsTrue ( String.Empty.Equals ( _o.ToStringSafe ( _defValue ) ) );
		}

		[Test]
		public void NullStringIsSafelyReturnedAsEmptyStringWhenDefaultValueIsNull ()
		{
			String _str = null;
			string _defValue = null;
			Assert.IsTrue ( String.Empty.Equals ( _str.ToStringSafe ( _defValue ) ) );
		}


		[Test]
		public void SubstringSafeOnNullString ()
		{
			String _str = null;
			Assert.IsTrue ( String.Empty.Equals ( _str.SubstringSafe ( 0, 1 ) ), "Substring on null string did not return empty string" );
		}

		[Test]
		public void SubstringSafeOnEmptyString ()
		{
			String _str = "";
			Assert.IsTrue ( String.Empty.Equals ( _str.SubstringSafe ( 0, 1 ) ), "Substring on empty string did not return empty string" );
		}

		[Test]
		public void SubstringSafeInitialIndexTooHighAndEndIndexLower ()
		{
			String _str = "1234";
			Assert.IsTrue ( String.Empty.Equals ( _str.SubstringSafe ( 11, 1 ) ), "" );
		}

		[Test]
		public void SubstringSafeInitialIndexTooHighAndEndIndexTooHigh ()
		{
			String _str = "1234";
			Assert.IsTrue ( String.Empty.Equals ( _str.SubstringSafe ( 11, 14 ) ), "" );
		}

		[Test]
		public void SubstringSafeInitialIndexBelowZeroandEndIndexTooHigh ()
		{
			String _str = "1234";
			Assert.IsTrue ( String.Empty.Equals ( _str.SubstringSafe ( -11, 14 ) ), "" );
		}

		[Test]
		public void SubstringSafeInitialIndexBelowZeroandEndIndexBelowZero ()
		{
			String _str = "1234";
			Assert.IsTrue ( String.Empty.Equals ( _str.SubstringSafe ( -11, -4 ) ), "" );
		}

		[Test]
		public void SubstringSafeInitialIndexBelowZeroandEndIndexBelowZero_2 ()
		{
			String _str = "1234";
			Assert.IsTrue ( String.Empty.Equals ( _str.SubstringSafe ( -11, -14 ) ), "" );
		}

		[Test]
		public void SubstringSafeValidString ()
		{
			String _str = "1234";
			Assert.IsTrue ( "12".Equals ( _str.SubstringSafe ( 0, 2 ) ), "" );
		}

		[Test]
		public void SubstringSafeValidStringLengthTooBigReturnsStringEmpty ()
		{
			String _str = "1234";
			Assert.IsTrue ( String.Empty.Equals ( _str.SubstringSafe ( 0, 21 ) ), "" );
		}

		[Test]
		public void SubstringSafeValidStringInitialindexTooBigReturnsStringEmpty ()
		{
			String _str = "1234";
			Assert.IsTrue ( String.Empty.Equals ( _str.SubstringSafe ( 10, 21 ) ), "" );
		}

		[Test]
		public void SubstringSafeValidStringInitialindexTooBigReturnsStringEmpty_2 ()
		{
			String _str = "1234";
			Assert.IsTrue ( String.Empty.Equals ( _str.SubstringSafe ( 10, -2 ) ), "" );
		}

		[Test]
		public void ToStringExpandedSimpleObject ()
		{

			TestDummyA tda = new TestDummyA () { FieldA = "Hello", FieldB = "World", FieldC = 999 };
			Assert.IsTrue ( "FieldA : Hello - FieldB : World - FieldC : 999" == tda.ToStringExpanded () );

		}

		[Test]
		public void InterpolateObject_1 ()
		{
			var order = new Order () { Name = "Sample Order", Description = "Order for 500 beer cans", Date = DateTime.Today, OrderID = 43 };
			string template = "this is the summary for Order '#{Name}', ordered on #{Date}. Order # is #{OrderID} ( '#{Description}' )";
			var _interpolated = template.Interpolate ( order );

			Assert.IsFalse ( _interpolated.IndexOf ( "#" ) >= 0 );
		}


		[Test]
		public void NewTakeExtensionShouldWorkOnACorrectListOfString ()
		{
			IEnumerable<string> _data = new List<string> () { "a", "kl", "do", "d", "cm", "y", "u", "··", "·t" };
			var _sub = _data.Take ( 2, 4 );
			Assert.That ( _sub.First () == "do" );
			Assert.That ( _sub.Last () == "cm" );
			Assert.That ( _sub.Count () == 3 );
		}

		// ---------------------------------------------------------------------------------

		[Test]
		public void NewTakeExtensionShouldWorkOnACorrectListOfInt ()
		{
			var _data = new List<Int32> () { 6, 3, 6, 76, 544, 54, 2, 72, 445, 23, 55, 91 };
			var _sub = _data.Take ( 3, 6 );
			Assert.That ( _sub.First () == 76 );
			Assert.That ( _sub.Last () == 2 );
			Assert.That ( _sub.Count () == 4 );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void NewTakeExtensionShouldWorkOnACorrectListOfObjects ()
		{

			DummyPerson p1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
			DummyPerson p2 = new DummyPerson { Age = 33, Name = "Shelley", Occupation = "Carpenter" };
			DummyPerson p3 = new DummyPerson { Age = 43, Name = "Gars", Occupation = "Heavy drinker" };
			DummyPerson p4 = new DummyPerson { Age = 26, Name = "Linda", Occupation = "Consultant" };
			DummyPerson p5 = new DummyPerson { Age = 39, Name = "Carmela", Occupation = "Social media expert" };
			DummyPerson p6 = new DummyPerson { Age = 52, Name = "Pixie", Occupation = "Middle manager" };

			var people = new List<DummyPerson> () { p1, p2, p3, p4, p5, p6 };
			Assert.That ( people.Take ( 2, 2 ).First () == p3 );
			Assert.That ( people.Take ( 2, 4 ).Last () == p5 && people.Take ( 2, 4 ).ElementAt ( 1 ) == p4 );

		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void NewToFromExtensionShouldWorkOnACorrectListOfString ()
		{
			IEnumerable<string> _data = new List<string> () { "a", "kl", "do", "d", "cm", "y", "u", "··", "·t" };
			var _sub = _data.From ( 2 ).To ( 2 );
			Assert.That ( _sub.First () == "do" );
			Assert.That ( _sub.Last () == "cm" );
			Assert.That ( _sub.Count () == 3 );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void NewToFromExtensionsShouldWorkOnACorrectListOfInt ()
		{
			var _data = new List<Int32> () { 6, 3, 6, 76, 544, 54, 2, 72, 445, 23, 55, 91 };
			var _sub = _data.From ( 3 ).To ( 3 );
			Assert.That ( _sub.First () == 76 );
			Assert.That ( _sub.Last () == 2 );
			Assert.That ( _sub.Count () == 4 );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void ContainsAnyTests ()
		{
			var _l = new List<string> () { "a", "b", "c", "d", "e", "f" };
			var _s = new List<string> () { "c", "r" };
			Assert.That ( _l.ContainsAny ( _s ) );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void ContainsAnyTests_1 ()
		{
			var _l = new List<string> () { "a", "b", "c", "d", "e", "f" };
			var _s = new List<string> () { "aa", "r" };
			Assert.That ( !_l.ContainsAny ( _s ) );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void ContainsAnyTests_2 ()
		{
			var _l = new List<Int32> () { 34, 45, 3, 425, 63, 3, 52, 354, 23, 4 };
			var _s = new List<Int32> () { 57, 128 };
			Assert.That ( !_l.ContainsAny ( _s ) );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void ContainsAnyTests_3 ()
		{
			var _l = new List<Int32> () { 34, 45, 3, 425, 63, 3, 52, 354, 23, 4 };
			var _s = new List<Int32> () { 128, 99, 23 };
			Assert.That ( _l.ContainsAny ( _s ) );
		}


		// ---------------------------------------------------------------------------------


		[TestCase ( "this is my string yeah", false, ExpectedResult = true )]
		[TestCase ( "that is my string yeah", false, ExpectedResult = false )]
		[TestCase ( "well, yeah, about that...", false, ExpectedResult = true )]
		[TestCase ( "usually, an island are inhabited by islanders", true, ExpectedResult = true )]
		[TestCase ( "usually, island are inhabited by islanders", false, ExpectedResult = true )]
		[TestCase ( "usually, islands are inhabited by islanders", true, ExpectedResult = false )]
		[TestCase ( "usually, islands are inhabited by islanders", false, ExpectedResult = true )]
		[TestCase ( "the islanders were crazy", true, ExpectedResult = false )]
		[TestCase ( "the islanders were crazy", false, ExpectedResult = true )]
		[TestCase ( "", false, ExpectedResult = false )]
		[TestCase ( "", true, ExpectedResult = false )]
		public bool ContainsAnyForStringTests ( string teststring, bool asWord )
		{
			var _ = teststring.ContainsAny ( new List<String> () { "no", "I", "don't", "know", "about", "this", "island" }, asWord );
			return _;
		}


		// ---------------------------------------------------------------------------------


		[TestCase ( "this is my string yeah", "this", false, ExpectedResult = "this" )]
		[TestCase ( "that is my string yeah", "", false, ExpectedResult = "" )]
		[TestCase ( "well, yeah, about that...", "about", false, ExpectedResult = "about" )]
		[TestCase ( "usually, an island is inhabited by islanders", "island", true, ExpectedResult = "island" )]
		[TestCase ( "the islanders were crazy", "", true, ExpectedResult = "" )]
		[TestCase ( "the islanders were crazy", "island", false, ExpectedResult = "island" )]
		[TestCase ( "usually, island are inhabited by islanders", "island", true, ExpectedResult = "island" )]
		[TestCase ( "usually, island are inhabited by islanders", "island", false, ExpectedResult = "island" )]
		[TestCase ( "usually, islands are inhabited by islanders", "", true, ExpectedResult = "" )]
		[TestCase ( "usually, islands are inhabited by islanders", "island", false, ExpectedResult = "island" )]
		[TestCase ( "", "", false, ExpectedResult = "" )]
		[TestCase ( "", "", true, ExpectedResult = "" )]
		public string ContainsAny2ForStringTests ( string teststring, string item, bool asWord )
		{
			var t = teststring.GetFirstOccurrence ( new List<String> () { "no", "I", "don't", "know", "about", "this", "island" }, asWord );
			return t;
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void ContainsAny2Tests ()
		{
			var _l = new List<string> () { "a", "b", "c", "d", "e", "f" };
			var _s = new List<string> () { "c", "r" };
			Assert.That ( _l.ContainsAny2 ( _s ).Item1 );
			Assert.That ( _l.ContainsAny2 ( _s ).Item2.ToString () == "c" );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void ContainsAny2Tests_1 ()
		{
			var _l = new List<string> () { "a", "b", "c", "d", "e", "f" };
			var _s = new List<string> () { "aa", "r" };
			Assert.That ( !_l.ContainsAny2 ( _s ).Item1 );
			Assert.That ( _l.ContainsAny2 ( _s ).Item2 == null );
		}


		// ---------------------------------------------------------------------------------


		[TestCase ( null, "A", 1, ExpectedResult = "A" )]
		[TestCase ( "A", null, 1, ExpectedResult = "A" )]
		[TestCase ( "A", null, 3, ExpectedResult = "A" )]
		[TestCase ( "", "A", 1, ExpectedResult = "A" )]
		[TestCase ( "", "A", 0, ExpectedResult = "" )]
		[TestCase ( "", "A", 4, ExpectedResult = "AAAA" )]
		[TestCase ( null, "A", 4, ExpectedResult = "AAAA" )]
		[TestCase ( "test string", "\t", 3, ExpectedResult = "\t\t\ttest string" )]
		[TestCase ( "test string", "\t", -13, ExpectedResult = "\ttest string" )]
		[TestCase ( "test string", "\t", 1, ExpectedResult = "\ttest string" )]
		[TestCase ( "test string", "\t", 0, ExpectedResult = "test string" )]
		[TestCase ( "test string", "this is my ", 1, ExpectedResult = "this is my test string" )]
		[TestCase ( "test string", "this is my ", 3, ExpectedResult = "this is my this is my this is my test string" )]

		public string PrependTests ( string s, string prepended, int reps )
		{
			return s.Prepend ( prepended, reps );
		}


		// ---------------------------------------------------------------------------------


		[TestCase ( null, "A", 1, ExpectedResult = "A" )]
		[TestCase ( "A", null, 1, ExpectedResult = "A" )]
		[TestCase ( "A", null, 3, ExpectedResult = "A" )]
		[TestCase ( "", "A", 1, ExpectedResult = "A" )]
		[TestCase ( "", "A", 0, ExpectedResult = "" )]
		[TestCase ( "", "A", 4, ExpectedResult = "AAAA" )]
		[TestCase ( null, "A", 4, ExpectedResult = "AAAA" )]
		[TestCase ( "test string", "\t", 3, ExpectedResult = "test string\t\t\t" )]
		[TestCase ( "test string", "\t", -13, ExpectedResult = "test string\t" )]
		[TestCase ( "test string", "\t", 1, ExpectedResult = "test string\t" )]
		[TestCase ( "test string", "\t", 0, ExpectedResult = "test string" )]
		[TestCase ( "test string", " this is my ", 1, ExpectedResult = "test string this is my " )]
		[TestCase ( "test string", " this is my ", 3, ExpectedResult = "test string this is my  this is my  this is my " )]

		public string AppendTests ( string s, string appendString, int reps )
		{
			return s.Append ( appendString, reps );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void PenultimateTests ()
		{
			var strings = new List<String> () { "cd", "ef", "po", "tu", "tt" };
			Assert.That ( strings.Penultimate () == "tu" );
		}


		[Test]
		public void PenultimateTests_02 ()
		{
			DummyPerson p1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
			DummyPerson p2 = new DummyPerson { Age = 33, Name = "Shelley", Occupation = "Carpenter" };
			DummyPerson p3 = new DummyPerson { Age = 43, Name = "Gars", Occupation = "Heavy drinker" };
			var list = new List<DummyPerson> () { p1, p2, p3 };
			Assert.That ( list.Penultimate () == p2 );

		}

		// ---------------------------------------------------------------------------------


		[Test]
		public void ElementAtFromLastTests ()
		{
			var strings = new List<String> () { "cd", "ef", "po", "tu", "tt" };
			Assert.That ( strings.ElementAtFromLast ( 2 ) == "tu" );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void ElementAtFromLastTests_02 ()
		{
			DummyPerson p1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
			DummyPerson p2 = new DummyPerson { Age = 33, Name = "Shelley", Occupation = "Carpenter" };
			DummyPerson p3 = new DummyPerson { Age = 43, Name = "Gars", Occupation = "Heavy drinker" };
			var list = new List<DummyPerson> () { p1, p2, p3 };
			Assert.That ( list.ElementAtFromLast ( 2 ) == p2 );

		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void InTests_01 ()
		{
			Assert.IsTrue ( "A".In ( new [] { "A", "B", "BV", "I" } ) );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void InTests_02 ()
		{
			DummyPerson p1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
			DummyPerson p2 = new DummyPerson { Age = 33, Name = "Shelley", Occupation = "Carpenter" };
			DummyPerson p3 = new DummyPerson { Age = 43, Name = "Gars", Occupation = "Heavy drinker" };
			DummyPerson p4 = new DummyPerson { Age = 43, Name = "Emma", Occupation = "Meth maker" };
			var list = new List<DummyPerson> () { p1, p2, p3 };
			Assert.IsTrue ( !p4.In ( list ) );
			Assert.IsTrue ( p3.In ( list ) );
		}



		// ---------------------------------------------------------------------------------


		[Test]
		public void ListIsNullOrEmptyTests_1 ()
		{
			List<string> l = null;
			Assert.That ( l.IsNullOrEmpty () );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void ListIsNullOrEmptyTests_2 ()
		{
			List<string> l = new List<string> ();
			Assert.That ( l.IsNullOrEmpty () );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void ListIsNullOrEmptyTests_3 ()
		{
			List<string> l = new List<string> () { "a" };
			Assert.That ( !l.IsNullOrEmpty () );
		}


		// ---------------------------------------------------------------------------------


		[TestCase ( null, 0, false, ExpectedResult = "" )]
		[TestCase ( null, 0, true, ExpectedResult = "" )]
		[TestCase ( null, 3, false, ExpectedResult = "" )]
		[TestCase ( null, 3, true, ExpectedResult = "" )]
		[TestCase ( "", 0, false, ExpectedResult = "" )]
		[TestCase ( "", 0, true, ExpectedResult = "" )]
		[TestCase ( "", 2, false, ExpectedResult = "" )]
		[TestCase ( "", 2, true, ExpectedResult = "" )]
		[TestCase ( "abcd", 2, true, ExpectedResult = "ab" )]
		[TestCase ( "abcd", 2, false, ExpectedResult = "ab" )]
		[TestCase ( "abcd ", 2, true, ExpectedResult = "ab" )]
		[TestCase ( "abcd ", 2, false, ExpectedResult = "abc" )]
		[TestCase ( "abcd       ", 2, true, ExpectedResult = "ab" )]
		[TestCase ( "abcd    ", 2, false, ExpectedResult = "abcd  " )]
		[TestCase ( "abcd ", 12, true, ExpectedResult = "" )]
		[TestCase ( "abcd ", 12, false, ExpectedResult = "" )]
		[TestCase ( "abcd", 3, true, ExpectedResult = "a" )]
		[TestCase ( "abcd", 3, false, ExpectedResult = "a" )]
		[TestCase ( "abcd", 4, true, ExpectedResult = "" )]
		[TestCase ( "abcd", 4, false, ExpectedResult = "" )]
		[TestCase ( "abcd", 5, true, ExpectedResult = "" )]
		[TestCase ( "abcd", 5, false, ExpectedResult = "" )]
		public string ShouldRemoveFromEnd ( string value, int charCount, bool trimEndFirst )
		{
			return value.RemoveFromEnd ( charCount, trimEndFirst );
		}


		// ---------------------------------------------------------------------------------


		[TestCase ( null, "", true, ExpectedResult = "" )]
		[TestCase ( null, "", false, ExpectedResult = "" )]
		[TestCase ( "", "", true, ExpectedResult = "" )]
		[TestCase ( "", "", false, ExpectedResult = "" )]
		[TestCase ( "", "a", true, ExpectedResult = "" )]
		[TestCase ( "", "a", false, ExpectedResult = "" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "fox", false, ExpectedResult = " jumps over the lazy dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "fox", true, ExpectedResult = "jumps over the lazy dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "fox ", true, ExpectedResult = "jumps over the lazy dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "fox ", false, ExpectedResult = "jumps over the lazy dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "dog", true, ExpectedResult = "" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "dog", false, ExpectedResult = "" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "bull ", false, ExpectedResult = "the quick brown fox jumps over the lazy dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "bull ", true, ExpectedResult = "the quick brown fox jumps over the lazy dog" )]
		[TestCase ( " the quick brown fox jumps over the lazy dog", "bull ", false, ExpectedResult = " the quick brown fox jumps over the lazy dog" )]
		[TestCase ( " the quick brown fox jumps over the lazy dog", "bull ", true, ExpectedResult = " the quick brown fox jumps over the lazy dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy brown dog", "the", false, ExpectedResult = " quick brown fox jumps over the lazy brown dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy brown dog", "the", true, ExpectedResult = "quick brown fox jumps over the lazy brown dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy brown dog", "the ", false, ExpectedResult = "quick brown fox jumps over the lazy brown dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy brown dog", "the ", true, ExpectedResult = "quick brown fox jumps over the lazy brown dog" )]
		public string SubStringAfterTests ( string value, string marker, bool trimResults )
		{

			return value.SubStringAfter ( marker, StringComparison.CurrentCulture, trimResults );

		}


		// ---------------------------------------------------------------------------------


		[TestCase ( null, "", true, ExpectedResult = "" )]
        [TestCase ( null, "", false, ExpectedResult = "" )]
        [TestCase ( "", "", true, ExpectedResult = "" )]
        [TestCase ( "", "", false, ExpectedResult = "" )]
        [TestCase ( "", "a", true, ExpectedResult = "" )]
        [TestCase ( "", "a", false, ExpectedResult = "" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "fox", false, ExpectedResult = " jumps over the lazy dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "fox", true, ExpectedResult = "jumps over the lazy dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "fox ", true, ExpectedResult = "jumps over the lazy dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "fox ", false, ExpectedResult = "jumps over the lazy dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "dog", true, ExpectedResult = "" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "dog", false, ExpectedResult = "" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "bull ", false, ExpectedResult = "the quick brown fox jumps over the lazy dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy dog", "bull ", true, ExpectedResult = "the quick brown fox jumps over the lazy dog" )]
		[TestCase ( " the quick brown fox jumps over the lazy dog", "bull ", false, ExpectedResult = " the quick brown fox jumps over the lazy dog" )]
		[TestCase ( " the quick brown fox jumps over the lazy dog", "bull ", true, ExpectedResult = " the quick brown fox jumps over the lazy dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy brown dog", "brown", false, ExpectedResult = " dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy brown dog", "brown", true, ExpectedResult = "dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy brown dog", "the", false, ExpectedResult = " lazy brown dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy brown dog", "the", true, ExpectedResult = "lazy brown dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy brown dog", "the ", false, ExpectedResult = "lazy brown dog" )]
		[TestCase ( "the quick brown fox jumps over the lazy brown dog", "the ", true, ExpectedResult = "lazy brown dog" )]
		public string SubStringAfterLastTests ( string value, string marker, bool trimResults )
		{

			return value.SubStringAfterLast ( marker, StringComparison.CurrentCulture, trimResults );

		}


		// ---------------------------------------------------------------------------------



		[TestCase ( "{}", "{", "}", ExpectedResult = "" )]
		[TestCase ( "{amduscia}", "{", "}", ExpectedResult = "amduscia" )]
		[TestCase ( " a band like {amduscia}", "{", "}", ExpectedResult = "amduscia" )]
		[TestCase ( "the quick brown fox jumps over the lazy brown dog", "quick", "lazy", ExpectedResult = " brown fox jumps over the " )]
		[TestCase ( "{\"Country\":\"ES\"}", "{", "}", ExpectedResult = "\"Country\":\"ES\"" )]
		[TestCase ( " {\"Country\":\"ES\",\"Active\":true, \"Age\":30, \"Title\":\"No reason to fear this\" }", "{", "}", ExpectedResult = "\"Country\":\"ES\",\"Active\":true, \"Age\":30, \"Title\":\"No reason to fear this\" " )]
		public string TakeBetweenTests ( string original, string s1, string s2 )
		{
			return original.TakeBetween ( s1, s2 );
		}


		// ---------------------------------------------------------------------------------


		[TestCase ( "this is the message \"the email is used generally for all the automatic communication with the supplier - e.g. payment advice\" that was said to me",
			"\"", "\"", ExpectedResult = "the email is used generally for all the automatic communication with the supplier - e.g. payment advice" )]
		[TestCase ( "this is the message \"the email is used generally for all the \"automatic\" communication with the supplier - e.g. payment advice\" that was said to me",
			 "\"", "\"", ExpectedResult = "the email is used generally for all the \"automatic\" communication with the supplier - e.g. payment advice" )]
		[TestCase ( "and then, what about this text with no coincidences?",
			"\"", "\"", ExpectedResult = "" )]
		public string TakeBetweenTests2 ( string original, string s1, string s2 )
		{
			return original.TakeBetween ( s1, s2 );
		}


		// ---------------------------------------------------------------------------------



		[TestCase ( "{'P', 'Q'}", " ", "{", "}", ExpectedResult = "{'P','Q'}" )]
		[TestCase ( "{'P',     'Q'}", " ", "{", "}", ExpectedResult = "{'P','Q'}" )]
		[TestCase ( "{'P', 'Q', '2', 'birds'}", " ", "{", "}", ExpectedResult = "{'P','Q','2','birds'}" )]
		[TestCase ( "{'P',   'Q', '2',    'birds'}", " ", "{", "}", ExpectedResult = "{'P','Q','2','birds'}" )]

		[TestCase ( "hello boys and girls {'P', 'Q'} what was that?", " ", "{", "}", ExpectedResult = "hello boys and girls {'P','Q'} what was that?" )]
		[TestCase ( "hello boys and girls {'P',     'Q'} what was that?", " ", "{", "}", ExpectedResult = "hello boys and girls {'P','Q'} what was that?" )]
		[TestCase ( "hello boys and girls {'P', 'Q', '2', 'birds'} what was that?", " ", "{", "}", ExpectedResult = "hello boys and girls {'P','Q','2','birds'} what was that?" )]
		[TestCase ( "hello boys and girls {'P',   'Q', '2',    'birds'} what was that?", " ", "{", "}", ExpectedResult = "hello boys and girls {'P','Q','2','birds'} what was that?" )]

		[TestCase ( "dog{'P', 'Q'}god", " ", "dog{", "}god", ExpectedResult = "dog{'P','Q'}god" )]
		[TestCase ( "dog{'P',     'Q'}god", " ", "dog{", "}god", ExpectedResult = "dog{'P','Q'}god" )]
		[TestCase ( "dog{'P', 'Q', '2', 'birds'}god", " ", "dog{", "}god", ExpectedResult = "dog{'P','Q','2','birds'}god" )]
		[TestCase ( "dog{'P',   'Q', '2',    'birds'}god", " ", "dog{", "}god", ExpectedResult = "dog{'P','Q','2','birds'}god" )]

		[TestCase ( "random stirng before dog{'P', 'Q'}god", " ", "dog{", "}god", ExpectedResult = "random stirng before dog{'P','Q'}god" )]
		[TestCase ( "random stirng before dog{'P',     'Q'}god", " ", "dog{", "}god", ExpectedResult = "random stirng before dog{'P','Q'}god" )]
		[TestCase ( "random stirng before dog{'P', 'Q', '2', 'birds'}god", " ", "dog{", "}god", ExpectedResult = "random stirng before dog{'P','Q','2','birds'}god" )]
		[TestCase ( "random stirng before dog{'P',   'Q', '2',    'birds'}god", " ", "dog{", "}god", ExpectedResult = "random stirng before dog{'P','Q','2','birds'}god" )]

		public string RemoveBetweenTests ( string original, string removee, string s1, string s2 )
		{
			return original.RemoveBetween ( removee, s1, s2 );
		}

		// ---------------------------------------------------------------------------------


		[TestCase ( "{'P', 'Q'}", " ", "{", "}", ExpectedResult = "{'P','Q'}" )]
		[TestCase ( "{'P',  'Q'}", " ", "{", "}", ExpectedResult = "{'P','Q'}" )]
		[TestCase ( "{'P',          'Q', '2', 'birds'}", " ", "{", "}", ExpectedResult = "{'P','Q','2','birds'}" )]
		[TestCase ( "{'P',      'Q',     '2',    'birds'}", " ", "{", "}", ExpectedResult = "{'P','Q','2','birds'}" )]
		public string RemoveBetweenTests2 ( string original, string removee, string s1, string s2 )
		{
			return original.RemoveBetween ( new List<String> { " ", "\t" }, s1, s2 );
		}


		// ---------------------------------------------------------------------------------


		[TestCase ( "{'P', - 'Q'}", " ", "{", "}", ExpectedResult = "{'P','Q'}" )]
		[TestCase ( "{'P',   -- 'Q'}", " ", "{", "}", ExpectedResult = "{'P','Q'}" )]
		[TestCase ( "{'P',      -  -    -   'Q', '2', 'birds'}", " ", "{", "}", ExpectedResult = "{'P','Q','2','birds'}" )]
		[TestCase ( "{'P',  - - #    'Q',     '2',    'birds'}", " ", "{", "}", ExpectedResult = "{'P','Q','2','birds'}" )]
		public string RemoveBetweenTests3 ( string original, string removee, string s1, string s2 )
		{
			return original.RemoveBetween ( new List<String> { " ", "\t", "-", "#" }, s1, s2 );
		}


		// ---------------------------------------------------------------------------------


        //[TestCase ( null, " ", "{", "}", ExpectedResult = "{'P','Q'}", ExpectedException = typeof ( ArgumentException ), ExpectedMessage = "String cannot be null" )]
        //[TestCase ( "}'P',   -- 'Q'{", " ", "{", "}", ExpectedResult = "{'P','Q'}", ExpectedException = typeof ( Exception ), ExpectedMessage = "End string cannot appear earlier than beginning string" )]
        //public string RemoveBetweenTestsExceptions ( string original, string removee, string s1, string s2 )
        //{
        //    return original.RemoveBetween ( new List<String> { " ", "\t", "-", "#" }, s1, s2 );
        //}


		// ---------------------------------------------------------------------------------


		// this one should not change as island is not contained as a word inside the subject sentence
		[TestCase ( "the islanders were insane", "island", "villag", ExpectedResult = "the islanders were insane" )]
		[TestCase ( "the islanders were insane", "island", "", ExpectedResult = "the islanders were insane" )]
		[TestCase ( "the islands were quite beautiful in the summer", "islands", "villages", ExpectedResult = "the villages were quite beautiful in the summer" )]
		[TestCase ( "some islands were quite beautiful in summer, other islands were better in winter", "islands", "villages",
			ExpectedResult = "some villages were quite beautiful in summer, other villages were better in winter" )]
		public string ReplaceWordTests ( string teststring, string newstring, string oldstring )
		{
			return teststring.ReplaceWord ( newstring, oldstring );
		}


		// ---------------------------------------------------------------------------------


		[TestCase ( "let sleeping dogs lie", ExpectedResult = "t sping dogs i" )]
		public string ImprovedRemoveTests_1 ( string line )
		{
			return line.MultipleRemove ( new List<string> () { "l", "e" } );
		}

		// ---------------------------------------------------------------------------------


		[TestCase ( "'All the things she did' was a track performed by \"The Whatevers\"", ExpectedResult = "All the things she did was a track performed by The Whatevers" )]
		public string ImprovedRemoveTests_2 ( string line )
		{
			return line.MultipleRemove ( new List<string> () { "\"", "'" } );
		}


		// ---------------------------------------------------------------------------------

		[TestCase ( "It's raining cats and dogs", ExpectedResult = "It's raining  and " )]
		public string ImprovedRemoveTests_3 ( string line )
		{
			return line.MultipleRemove ( new List<string> () { "cats", "dogs" } );
		}

		// ---------------------------------------------------------------------------------

		[TestCase ( "let sleeping dogs lie", "l", "", ExpectedResult = "et sleeping dogs lie" )]
		[TestCase ( "let sleeping dogs lie", "t", "", ExpectedResult = "le sleeping dogs lie" )]
		[TestCase ( "let sleeping dogs lie", "p", "", ExpectedResult = "let sleeing dogs lie" )]

		[TestCase ( "let sleeping dogs lie", "l", "@", ExpectedResult = "@et sleeping dogs lie" )]
		[TestCase ( "let sleeping dogs lie", "t", "KUZ", ExpectedResult = "leKUZ sleeping dogs lie" )]
		[TestCase ( "let sleeping dogs lie", "p", "1234567890", ExpectedResult = "let slee1234567890ing dogs lie" )]
		public string ReplaceFirstTests ( string line, string ocurrence, string replacement )
		{
			if ( String.IsNullOrEmpty ( replacement ) )
				return line.ReplaceFirst ( ocurrence );
			return line.ReplaceFirst ( ocurrence, replacement );
		}


		// ---------------------------------------------------------------------------------


		[TestCase ( "let sleeping dogs lie", "l", "", ExpectedResult = "let sleeping dogs ie" )]
		[TestCase ( "let sleeping dogs lie", "t", "", ExpectedResult = "le sleeping dogs lie" )]
		[TestCase ( "let sleeping dogs lie", "i", "", ExpectedResult = "let sleeping dogs le" )]

		[TestCase ( "let sleeping dogs lie", "l", "@", ExpectedResult = "let sleeping dogs @ie" )]
		[TestCase ( "let sleeping dogs lie", "t", "KUZ", ExpectedResult = "leKUZ sleeping dogs lie" )]
		[TestCase ( "let sleeping dogs lie", "g", "KUZ", ExpectedResult = "let sleeping doKUZs lie" )]
		[TestCase ( "let sleeping dogs lie", "e", "1234567890", ExpectedResult = "let sleeping dogs li1234567890" )]
		public string ReplaceLastTests ( string line, string occurrence, string replacement )
		{
			if ( String.IsNullOrEmpty ( replacement ) )
				return line.ReplaceLast ( occurrence );
			return line.ReplaceLast ( occurrence, replacement );
		}

		// ---------------------------------------------------------------------------------


		[TestCase ( "let sleeping dogs lie", "l", "", ExpectedResult = "et sleeping dogs ie" )]
		[TestCase ( "let sleeping dogs lie", "l", "a588", ExpectedResult = "a588et sleeping dogs a588ie" )]
		[TestCase ( "She said several things, like \"yeah\", \"like\" and \"whatever\", I did not like her speech", "\"", "",
			ExpectedResult = "She said several things, like yeah\", \"like\" and \"whatever, I did not like her speech" )]
		[TestCase ( "She said \"yeah\"", "\"", "", ExpectedResult = "She said yeah" )]
		[TestCase ( "\"yeah, she said", "\"", "", ExpectedResult = "yeah, she said" )]
		public string ReplaceFirstAndLastOnlyTests ( string line, string occurrence, string replacement )
		{
			if ( String.IsNullOrEmpty ( replacement ) )
				return line.ReplaceFirstAndLastOnly ( occurrence );

			return line.ReplaceFirstAndLastOnly ( occurrence, replacement );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void InsertMultiple_Test02 ()
		{
			var words = new List<string> { "hey", "you", "what", "doing" };
			var insertees = new List<KeyValuePair<int, string>> ();
			insertees.Add ( new KeyValuePair<int, string> ( 1, "!" ) );
			insertees.Add ( new KeyValuePair<int, string> ( 1, "just" ) );
			insertees.Add ( new KeyValuePair<int, string> ( 3, "are" ) );
			insertees.Add ( new KeyValuePair<int, string> ( 3, "you" ) );
			insertees.Add ( new KeyValuePair<int, string> ( 4, "here" ) );
			var _new = words.InsertMultiple ( insertees );
		}

		[Test]
		public void InsertMultiple_Test01 ()
		{
			var arr = new List<string> { "A", "C", "E", "H", "J", null };
			var insertees = new List<KeyValuePair<int, string>> ();
			insertees.Add ( new KeyValuePair<int, string> ( 1, "B" ) );
			insertees.Add ( new KeyValuePair<int, string> ( 2, "D" ) );
			insertees.Add ( new KeyValuePair<int, string> ( 3, "G" ) );
			insertees.Add ( new KeyValuePair<int, string> ( 0, null ) );
			var _new = arr.InsertMultiple ( insertees );
			Assert.That ( _new.Count () == 10 );
			Assert.That ( _new.Last () == null );
			Assert.That ( _new.First () == null );
		}

		// ---------------------------------------------------------------------------------


		[Test]
		public void GetKeys_Test_01 ()
		{
			var insertees = new List<KeyValuePair<int, string>> ();
			insertees.Add ( new KeyValuePair<int, string> ( 1, "B" ) );
			insertees.Add ( new KeyValuePair<int, string> ( 2, "D" ) );
			insertees.Add ( new KeyValuePair<int, string> ( 3, "G" ) );
			var _keys = insertees.GetKeys ();
			Assert.That ( _keys.Count () == 3 );
			Assert.That ( _keys.First () == 1 );
			Assert.That ( _keys.Last () == 3 );
		}

		[Test]
		public void GetKeys_Test_02 ()
		{
			var insertees = new List<Tuple<int, string>> ();
			insertees.Add ( new Tuple<int, string> ( 1, "B" ) );
			insertees.Add ( new Tuple<int, string> ( 2, "D" ) );
			insertees.Add ( new Tuple<int, string> ( 3, "G" ) );
			var _keys = insertees.GetKeys ();
			Assert.That ( _keys.Count () == 3 );
			Assert.That ( _keys.First () == 1 );
			Assert.That ( _keys.Last () == 3 );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void IsFirstTest_01 ()
		{
			Object o = null;
			Assert.IsFalse ( o.IsFirst<Object> ( null ) );
		}

		// ---------------------------------------------------------------------------------

		[Test]
		public void IsFirstTest_02 ()
		{
			Object o = null;
			List<Object> list = new List<object> () { null, null };
			Assert.IsTrue ( o.IsFirst<Object> ( list ) );
		}

		// ---------------------------------------------------------------------------------

		[Test]
		public void IsFirstTest_03 ()
		{
			Object o = "garks";
			List<Object> list = new List<object> () { null, null };
			Assert.IsFalse ( o.IsFirst<Object> ( list ) );
		}

		// ---------------------------------------------------------------------------------

		[Test]
		public void IsFirstTest_04 ()
		{
			Object o = "garks";
			List<Object> list = new List<object> () { "garks", "yeyue" };
			Assert.IsTrue ( o.IsFirst<Object> ( list ) );
		}

		// ---------------------------------------------------------------------------------

		[Test]
		public void IsFirstTest_05 ()
		{
			DummyPerson p1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
			DummyPerson p2 = new DummyPerson { Age = 33, Name = "Shelley", Occupation = "Carpenter" };
			DummyPerson p3 = new DummyPerson { Age = 43, Name = "Gars", Occupation = "Heavy drinker" };
			var list = new List<DummyPerson> () { p1, p2, p3 };
			Assert.IsTrue ( p1.IsFirst<DummyPerson> ( list ) );
		}

		// ---------------------------------------------------------------------------------



		[Test]
		public void JoinTogether_Strings_01 ()
		{
			var sut = new [] { "a", "b", "c", "d", "e" };
			Assert.That ( sut.JoinTogether () == "abcde" );
		}

		[Test]
		public void JoinTogether_Strings_02 ()
		{
			var sut = new [] { "a", "b", "c", "d", "e" };
			Assert.That ( sut.JoinTogether ( " " ) == "a b c d e" );
		}

		[Test]
		public void JoinTogether_Strings_03 ()
		{
			var sut = new [] { "a", "b", "c", "d", "e" };
			Assert.That ( sut.JoinTogether ( ", " ) == "a, b, c, d, e" );
		}

		[Test]
		public void JoinTogether_Numbers_01 ()
		{
			var sut = new [] { 1, 2, 3, 4, 5 };
			Assert.That ( sut.JoinTogether () == "12345" );
		}

		[Test]
		public void JoinTogether_Numbers_02 ()
		{
			var sut = new [] { 1, 2, 3, 4, 5 };
			Assert.That ( sut.JoinTogether ( " " ) == "1 2 3 4 5" );
		}

		[Test]
		public void JoinTogether_Numbers_03 ()
		{
			var sut = new [] { 1, 2, 3, 4, 5 };
			Assert.That ( sut.JoinTogether ( ", " ) == "1, 2, 3, 4, 5" );
		}

		[Test]
		public void JoinTogether_Decimals_01 ()
		{
			var sut = new [] { 1.31m, 2.33m, 3.01m, 4.99m, 5.25m };
			Assert.That ( sut.JoinTogether () == "1,312,333,014,995,25" );
		}

		[Test]
		public void JoinTogether_Decimals_02 ()
		{
			var sut = new [] { 1.31m, 2.33m, 3.01m, 4.99m, 5.25m };
			Assert.That ( sut.JoinTogether ( " " ) == "1,31 2,33 3,01 4,99 5,25" );

		}

		[Test]
		public void JoinTogether_Decimals_03 ()
		{
			var sut = new [] { 1.31m, 2.33m, 3.01m, 4.99m, 5.25m };
			Assert.That ( sut.JoinTogether ( ", " ) == "1,31, 2,33, 3,01, 4,99, 5,25" );

		}

		[Test]
		public void JoinTogether_Object ()
		{
			DummyPerson p1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
			DummyPerson p1_1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
			DummyPerson p2 = new DummyPerson { Age = 33, Name = "Shelley", Occupation = "Carpenter" };
			DummyPerson p3 = new DummyPerson { Age = 43, Name = "Gars", Occupation = "Heavy drinker" };
			var list = new List<DummyPerson> () { p1, p2, p3 };

			Assert.That ( list.JoinTogether ( " " ) == String.Format ( "{0} {1} {2}", p1.ToString (), p2.ToString (), p3.ToString () ) );
			Assert.That ( list.JoinTogether ( ", " ) == String.Format ( "{0}, {1}, {2}", p1.ToString (), p2.ToString (), p3.ToString () ) );

		}

		// ---------------------------------------------------------------------------------

		[Test]
		public void JoinTogetherBetween_Strings_01 ()
		{
			var sut = new [] { "a", "b", "c", "d", "e" };
			Assert.That ( sut.JoinTogetherBetween ( 0, 2 ) == "abc" );
		}

		[Test]
		public void JoinTogetherBetween_Strings_02 ()
		{
			var sut = new [] { "a", "b", "c", "d", "e" };
			Assert.That ( sut.JoinTogetherBetween ( 0, 2, " " ) == "a b c" );
		}

		[Test]
		public void JoinTogetherBetween_Strings_03 ()
		{
			var sut = new [] { "a", "b", "c", "d", "e" };
			Assert.That ( sut.JoinTogetherBetween ( 2, 2, " " ) == "c" );
		}


		[Test]
		public void JoinTogetherBetween_Strings_04 ()
		{
			var sut = new [] { "a", "b", "c", "d", "e" };
			Assert.That ( sut.JoinTogetherBetween ( 2, 4, " " ) == "c d e" );
		}

		// ---------------------------------------------------------------------------------


		[TestCase ( "a", "", ExpectedResult = "a" )]
		[TestCase ( "", "", ExpectedResult = "" )]
		[TestCase ( "", " ", ExpectedResult = "" )]
		[TestCase ( "", " - ", ExpectedResult = "" )]
		[TestCase ( "yeah!", "", ExpectedResult = "yeah!" )]
		[TestCase ( "yeah!", "!", ExpectedResult = "yeah" )]
		[TestCase ( "yeah!!", "!", ExpectedResult = "yeah!" )]
		[TestCase ( "yeah!!", "!!", ExpectedResult = "yeah" )]
		[TestCase ( "yeah!!", "!!!", ExpectedResult = "yeah!!" )]
		[TestCase ( "a", " ", ExpectedResult = "a" )]
		[TestCase ( "a ", " ", ExpectedResult = "a" )]
		[TestCase ( "a  ", " ", ExpectedResult = "a " )]
		[TestCase ( "a  ", "  ", ExpectedResult = "a" )]
		public string RemoveLast ( string sut, string removee )
		{
			return sut.RemoveLast ( removee );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void HasItems_Test ()
		{
			List<string> list = null;
			Assert.That ( !list.HasElements () );
		}

		[Test]
		public void HasItems_Test_01 ()
		{
			List<string> list = new List<string> ();
			Assert.That ( !list.HasElements () );
		}

		[Test]
		public void HasItems_Test_02 ()
		{
			List<string> list = new List<string> () { "" };
			Assert.That ( list.HasElements () );
		}

		[Test]
		public void HasItems_Test_03 ()
		{
			List<string> list = new List<string> () { "xx" };
			Assert.That ( list.HasElements () );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void TakeAfterTests ()
		{
			var a = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			var c = a.TakeAfter ( x => x >= 6 );
			Assert.That ( c.Count () == 4 );
			Assert.That ( c.First () == 7 );
		}


		// ---------------------------------------------------------------------------------


		[Test]
		public void TakeAfterTests_02 ()
		{
			DummyPerson p1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
			DummyPerson p2 = new DummyPerson { Age = 33, Name = "Shelley", Occupation = "Carpenter" };
			DummyPerson p3 = new DummyPerson { Age = 43, Name = "Gars", Occupation = "Heavy drinker" };
			DummyPerson p4 = new DummyPerson { Age = 26, Name = "Linda", Occupation = "Consultant" };
			DummyPerson p5 = new DummyPerson { Age = 39, Name = "Carmela", Occupation = "Social media expert" };
			DummyPerson p6 = new DummyPerson { Age = 52, Name = "Pixie", Occupation = "Middle manager" };

			var people = new List<DummyPerson> () { p1, p2, p3, p4, p5, p6 };

			// do not confuse with .Where, this is just take what comes after the first matching
			var c = people.TakeAfter ( x => x.Age > 40 );
			Assert.That ( c.Count () == 3 );
			Assert.That ( c.First ().Name == "Linda" );
			Assert.That ( c.Last ().Name == "Pixie" );
		}


		// ---------------------------------------------------------------------------------


		[TestCase ( "àëíôüñ", ExpectedResult = "aeioun" )]
		public string RemoveDiacritics_Tests ( string sut )
		{
			return sut.RemoveDiacritics ();
		}

	}


	// ---------------------------------------------------------------------------------


	#region " --- dummy class --- "

	public class DummyPerson
	{
		public string Name { get; set; }
		public int Age { get; set; }
		public string Occupation { get; set; }

		public override string ToString ()
		{
			return String.Format ( "{0}, {1}", Name, Age );
		}

	}

	class TestDummyA
	{
		public string FieldA { get; set; }
		public string FieldB { get; set; }
		public int FieldC { get; set; }
		private string YouShouldNotComeUp { get; set; }

		public TestDummyA ()
		{
			this.YouShouldNotComeUp = "Nopes!";
		}

	}

	class TestDummyB
	{
		public TestDummyA TestDummyA { get; set; }
		public string FieldA { get; set; }
		public string FieldB { get; set; }
		public int FieldC { get; set; }
		private string YouShouldNotComeUp { get; set; }
	}

	class Order
	{
		public int OrderID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime Date { get; set; }

	}


	#endregion
}
