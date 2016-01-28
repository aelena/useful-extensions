using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aelena.SimpleExtensions.ListExtensions;
using Aelena.SimpleExtensions.StringExtensions;
using Aelena.SimpleExtensions.ObjectExtensions;
using Xunit;

namespace Aelena.SimpleExtensions.Tests
{
    public class ExtensionsTests
    {


        [Fact]
        public void NullStringIsSafelyReturnedAsEmptyString ()
        {
            String _str = null;
            Assert.True ( String.Empty.Equals ( _str.ToStringSafe () ) );
        }

        [Fact]
        public void NonNullStringIsSafelyReturnedAsItself ()
        {
            String _str = "aAa_123";
            Assert.True ( "aAa_123".Equals ( _str.ToStringSafe () ) );
        }

        [Fact]
        public void NullObjectIsSafelyReturnedAsEmptyString ()
        {
            Object _o = null;
            Assert.True ( String.Empty.Equals ( _o.ToStringSafe () ) );
        }

        [Fact]
        public void NullStringIsSafelyReturnedAsDefaultValue ()
        {
            String _str = null;
            string _defValue = "_";
            Assert.True ( _defValue.Equals ( _str.ToStringSafe ( "_" ) ) );
        }


        [Fact]
        public void NullObjectIsSafelyReturnedAsDefaultValue ()
        {
            Object _o = null;
            string _defValue = " ";
            Assert.True ( _defValue.Equals ( _o.ToStringSafe ( _defValue ) ) );
        }

        [Fact]
        public void NullObjectIsSafelyReturnedAsEmptyStringWhenDefaultValueIsNull ()
        {
            Object _o = null;
            string _defValue = null;
            Assert.True ( String.Empty.Equals ( _o.ToStringSafe ( _defValue ) ) );
        }

        [Fact]
        public void NullStringIsSafelyReturnedAsEmptyStringWhenDefaultValueIsNull ()
        {
            String _str = null;
            string _defValue = null;
            Assert.True ( String.Empty.Equals ( _str.ToStringSafe ( _defValue ) ) );
        }


        [Fact]
        public void SubstringSafeOnNullString ()
        {
            String _str = null;
            Assert.True ( String.Empty.Equals ( _str.SubstringSafe ( 0, 1 ) ), "Substring on null string did not return empty string" );
        }

        [Fact]
        public void SubstringSafeOnEmptyString ()
        {
            String _str = "";
            Assert.True ( String.Empty.Equals ( _str.SubstringSafe ( 0, 1 ) ), "Substring on empty string did not return empty string" );
        }

        [Fact]
        public void SubstringSafeInitialIndexTooHighAndEndIndexLower ()
        {
            String _str = "1234";
            Assert.True ( String.Empty.Equals ( _str.SubstringSafe ( 11, 1 ) ), "" );
        }

        [Fact]
        public void SubstringSafeInitialIndexTooHighAndEndIndexTooHigh ()
        {
            String _str = "1234";
            Assert.True ( String.Empty.Equals ( _str.SubstringSafe ( 11, 14 ) ), "" );
        }

        [Fact]
        public void SubstringSafeInitialIndexBelowZeroandEndIndexTooHigh ()
        {
            String _str = "1234";
            Assert.True ( String.Empty.Equals ( _str.SubstringSafe ( -11, 14 ) ), "" );
        }

        [Fact]
        public void SubstringSafeInitialIndexBelowZeroandEndIndexBelowZero ()
        {
            String _str = "1234";
            Assert.True ( String.Empty.Equals ( _str.SubstringSafe ( -11, -4 ) ), "" );
        }

        [Fact]
        public void SubstringSafeInitialIndexBelowZeroandEndIndexBelowZero_2 ()
        {
            String _str = "1234";
            Assert.True ( String.Empty.Equals ( _str.SubstringSafe ( -11, -14 ) ), "" );
        }

        [Fact]
        public void SubstringSafeValidString ()
        {
            String _str = "1234";
            Assert.True ( "12".Equals ( _str.SubstringSafe ( 0, 2 ) ), "" );
        }

        [Fact]
        public void SubstringSafeValidStringLengthTooBigReturnsStringEmpty ()
        {
            String _str = "1234";
            Assert.True ( String.Empty.Equals ( _str.SubstringSafe ( 0, 21 ) ), "" );
        }

        [Fact]
        public void SubstringSafeValidStringInitialindexTooBigReturnsStringEmpty ()
        {
            String _str = "1234";
            Assert.True ( String.Empty.Equals ( _str.SubstringSafe ( 10, 21 ) ), "" );
        }

        [Fact]
        public void SubstringSafeValidStringInitialindexTooBigReturnsStringEmpty_2 ()
        {
            String _str = "1234";
            Assert.True ( String.Empty.Equals ( _str.SubstringSafe ( 10, -2 ) ), "" );
        }

        [Fact]
        public void ToStringExpandedSimpleObject ()
        {

            TestDummyA tda = new TestDummyA () { FieldA = "Hello", FieldB = "World", FieldC = 999 };
            Assert.True ( "FieldA : Hello - FieldB : World - FieldC : 999" == tda.ToStringExpanded () );

        }

        [Fact]
        public void InterpolateObject_1 ()
        {
            var order = new Order () { Name = "Sample Order", Description = "Order for 500 beer cans", Date = DateTime.Today, OrderID = 43 };
            string template = "this is the summary for Order '#{Name}', ordered on #{Date}. Order # is #{OrderID} ( '#{Description}' )";
            var _interpolated = template.Interpolate ( order );

            Assert.False ( _interpolated.IndexOf ( "#" ) >= 0 );
        }


        [Fact]
        public void NewTakeExtensionShouldWorkOnACorrectListOfString ()
        {
            IEnumerable<string> _data = new List<string> () { "a", "kl", "do", "d", "cm", "y", "u", "··", "·t" };
            var _sub = _data.Take ( 2, 4 );
            Assert.True ( _sub.First () == "do" );
            Assert.True ( _sub.Last () == "cm" );
            Assert.True ( _sub.Count () == 3 );
        }

        // ---------------------------------------------------------------------------------

        [Fact]
        public void NewTakeExtensionShouldWorkOnACorrectListOfInt ()
        {
            var _data = new List<Int32> () { 6, 3, 6, 76, 544, 54, 2, 72, 445, 23, 55, 91 };
            var _sub = _data.Take ( 3, 6 );
            Assert.True ( _sub.First () == 76 );
            Assert.True ( _sub.Last () == 2 );
            Assert.True ( _sub.Count () == 4 );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void NewTakeExtensionShouldWorkOnACorrectListOfObjects ()
        {

            DummyPerson p1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
            DummyPerson p2 = new DummyPerson { Age = 33, Name = "Shelley", Occupation = "Carpenter" };
            DummyPerson p3 = new DummyPerson { Age = 43, Name = "Gars", Occupation = "Heavy drinker" };
            DummyPerson p4 = new DummyPerson { Age = 26, Name = "Linda", Occupation = "Consultant" };
            DummyPerson p5 = new DummyPerson { Age = 39, Name = "Carmela", Occupation = "Social media expert" };
            DummyPerson p6 = new DummyPerson { Age = 52, Name = "Pixie", Occupation = "Middle manager" };

            var people = new List<DummyPerson> () { p1, p2, p3, p4, p5, p6 };
            Assert.True ( people.Take ( 2, 2 ).First () == p3 );
            Assert.True ( people.Take ( 2, 4 ).Last () == p5 && people.Take ( 2, 4 ).ElementAt ( 1 ) == p4 );

        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void NewToFromExtensionShouldWorkOnACorrectListOfString ()
        {
            IEnumerable<string> _data = new List<string> () { "a", "kl", "do", "d", "cm", "y", "u", "··", "·t" };
            var _sub = _data.From ( 2 ).To ( 2 );
            Assert.True ( _sub.First () == "do" );
            Assert.True ( _sub.Last () == "cm" );
            Assert.True ( _sub.Count () == 3 );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void NewToFromExtensionsShouldWorkOnACorrectListOfInt ()
        {
            var _data = new List<Int32> () { 6, 3, 6, 76, 544, 54, 2, 72, 445, 23, 55, 91 };
            var _sub = _data.From ( 3 ).To ( 3 );
            Assert.True ( _sub.First () == 76 );
            Assert.True ( _sub.Last () == 2 );
            Assert.True ( _sub.Count () == 4 );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void ContainsAnyTests ()
        {
            var _l = new List<string> () { "a", "b", "c", "d", "e", "f" };
            var _s = new List<string> () { "c", "r" };
            Assert.True ( _l.ContainsAny ( _s ) );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void ContainsAnyTests_1 ()
        {
            var _l = new List<string> () { "a", "b", "c", "d", "e", "f" };
            var _s = new List<string> () { "aa", "r" };
            Assert.True ( !_l.ContainsAny ( _s ) );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void ContainsAnyTests_2 ()
        {
            var _l = new List<Int32> () { 34, 45, 3, 425, 63, 3, 52, 354, 23, 4 };
            var _s = new List<Int32> () { 57, 128 };
            Assert.True ( !_l.ContainsAny ( _s ) );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void ContainsAnyTests_3 ()
        {
            var _l = new List<Int32> () { 34, 45, 3, 425, 63, 3, 52, 354, 23, 4 };
            var _s = new List<Int32> () { 128, 99, 23 };
            Assert.True ( _l.ContainsAny ( _s ) );
        }


        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( "this is my string yeah", false, true )]
        [InlineData ( "that is my string yeah", false, false )]
        [InlineData ( "well, yeah, about that...", false, true )]
        [InlineData ( "usually, an island are inhabited by islanders", true, true )]
        [InlineData ( "usually, island are inhabited by islanders", false, true )]
        [InlineData ( "usually, islands are inhabited by islanders", true, false )]
        [InlineData ( "usually, islands are inhabited by islanders", false, true )]
        [InlineData ( "the islanders were crazy", true, false )]
        [InlineData ( "the islanders were crazy", false, true )]
        [InlineData ( "", false, false )]
        [InlineData ( "", true, false )]
        public void ContainsAnyForStringTests ( string teststring, bool asWord, bool result )
        {
            Assert.True ( teststring.ContainsAny ( new List<String> () { "no", "I", "don't", "know", "about", "this", "island" }, asWord ) == result );
        }


        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( "this is my string yeah", "this", false, "this" )]
        [InlineData ( "that is my string yeah", "", false, "" )]
        [InlineData ( "well, yeah, about that...", "about", false, "about" )]
        [InlineData ( "usually, an island is inhabited by islanders", "island", true, "island" )]
        [InlineData ( "the islanders were crazy", "", true, "" )]
        [InlineData ( "the islanders were crazy", "island", false, "island" )]
        [InlineData ( "usually, island are inhabited by islanders", "island", true, "island" )]
        [InlineData ( "usually, island are inhabited by islanders", "island", false, "island" )]
        [InlineData ( "usually, islands are inhabited by islanders", "", true, "" )]
        [InlineData ( "usually, islands are inhabited by islanders", "island", false, "island" )]
        [InlineData ( "", "", false, "" )]
        [InlineData ( "", "", true, "" )]
        public void ContainsAny2ForStringTests ( string teststring, string item, bool asWord, string result )
        {
            Assert.True ( teststring.GetFirstOccurrence ( new List<String> () { "no", "I", "don't", "know", "about", "this", "island" }, asWord ) == result );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void ContainsAny2Tests ()
        {
            var _l = new List<string> () { "a", "b", "c", "d", "e", "f" };
            var _s = new List<string> () { "c", "r" };
            Assert.True ( _l.ContainsAny2 ( _s ).Item1 );
            Assert.True ( _l.ContainsAny2 ( _s ).Item2.ToString () == "c" );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void ContainsAny2Tests_1 ()
        {
            var _l = new List<string> () { "a", "b", "c", "d", "e", "f" };
            var _s = new List<string> () { "aa", "r" };
            Assert.True ( !_l.ContainsAny2 ( _s ).Item1 );
            Assert.True ( _l.ContainsAny2 ( _s ).Item2 == null );
        }


        // ---------------------------------------------------------------------------------

        [Theory]
        [InlineData ( null, "A", 1, "A" )]
        [InlineData ( "A", null, 1, "A" )]
        [InlineData ( "A", null, 3, "A" )]
        [InlineData ( "", "A", 1, "A" )]
        [InlineData ( "", "A", 0, "" )]
        [InlineData ( "", "A", 4, "AAAA" )]
        [InlineData ( null, "A", 4, "AAAA" )]
        [InlineData ( "test string", "\t", 3, "\t\t\ttest string" )]
        [InlineData ( "test string", "\t", -13, "\ttest string" )]
        [InlineData ( "test string", "\t", 1, "\ttest string" )]
        [InlineData ( "test string", "\t", 0, "test string" )]
        [InlineData ( "test string", "this is my ", 1, "this is my test string" )]
        [InlineData ( "test string", "this is my ", 3, "this is my this is my this is my test string" )]
        public void PrependTests ( string s, string prepended, int reps, string result )
        {
            Assert.True ( s.Prepend ( prepended, reps ) == result );
        }


        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( null, "A", 1, "A" )]
        [InlineData ( "A", null, 1, "A" )]
        [InlineData ( "A", null, 3, "A" )]
        [InlineData ( "", "A", 1, "A" )]
        [InlineData ( "", "A", 0, "" )]
        [InlineData ( "", "A", 4, "AAAA" )]
        [InlineData ( null, "A", 4, "AAAA" )]
        [InlineData ( "test string", "\t", 3, "test string\t\t\t" )]
        [InlineData ( "test string", "\t", -13, "test string\t" )]
        [InlineData ( "test string", "\t", 1, "test string\t" )]
        [InlineData ( "test string", "\t", 0, "test string" )]
        [InlineData ( "test string", " this is my ", 1, "test string this is my " )]
        [InlineData ( "test string", " this is my ", 3, "test string this is my  this is my  this is my " )]
        public void AppendTests ( string s, string appendString, int reps, string result )
        {
            Assert.True ( s.Append ( appendString, reps ) == result );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void PenultimateTests ()
        {
            var strings = new List<String> () { "cd", "ef", "po", "tu", "tt" };
            Assert.True ( strings.Penultimate () == "tu" );
        }


        [Fact]
        public void PenultimateTests_02 ()
        {
            DummyPerson p1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
            DummyPerson p2 = new DummyPerson { Age = 33, Name = "Shelley", Occupation = "Carpenter" };
            DummyPerson p3 = new DummyPerson { Age = 43, Name = "Gars", Occupation = "Heavy drinker" };
            var list = new List<DummyPerson> () { p1, p2, p3 };
            Assert.True ( list.Penultimate () == p2 );

        }

        // ---------------------------------------------------------------------------------


        [Fact]
        public void ElementAtFromLastTests ()
        {
            var strings = new List<String> () { "cd", "ef", "po", "tu", "tt" };
            Assert.True ( strings.ElementAtFromLast ( 2 ) == "tu" );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void ElementAtFromLastTests_02 ()
        {
            DummyPerson p1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
            DummyPerson p2 = new DummyPerson { Age = 33, Name = "Shelley", Occupation = "Carpenter" };
            DummyPerson p3 = new DummyPerson { Age = 43, Name = "Gars", Occupation = "Heavy drinker" };
            var list = new List<DummyPerson> () { p1, p2, p3 };
            Assert.True ( list.ElementAtFromLast ( 2 ) == p2 );

        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void InTests_01 ()
        {
            Assert.True ( "A".In ( new [ ] { "A", "B", "BV", "I" } ) );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void InTests_02 ()
        {
            DummyPerson p1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
            DummyPerson p2 = new DummyPerson { Age = 33, Name = "Shelley", Occupation = "Carpenter" };
            DummyPerson p3 = new DummyPerson { Age = 43, Name = "Gars", Occupation = "Heavy drinker" };
            DummyPerson p4 = new DummyPerson { Age = 43, Name = "Emma", Occupation = "Meth maker" };
            var list = new List<DummyPerson> () { p1, p2, p3 };
            Assert.True ( !p4.In ( list ) );
            Assert.True ( p3.In ( list ) );
        }



        // ---------------------------------------------------------------------------------


        [Fact]
        public void ListIsNullOrEmptyTests_1 ()
        {
            List<string> l = null;
            Assert.True ( l.IsNullOrEmpty () );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void ListIsNullOrEmptyTests_2 ()
        {
            List<string> l = new List<string> ();
            Assert.True ( l.IsNullOrEmpty () );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void ListIsNullOrEmptyTests_3 ()
        {
            List<string> l = new List<string> () { "a" };
            Assert.True ( !l.IsNullOrEmpty () );
        }


        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( null, 0, false, "" )]
        [InlineData ( null, 0, true, "" )]
        [InlineData ( null, 3, false, "" )]
        [InlineData ( null, 3, true, "" )]
        [InlineData ( "", 0, false, "" )]
        [InlineData ( "", 0, true, "" )]
        [InlineData ( "", 2, false, "" )]
        [InlineData ( "", 2, true, "" )]
        [InlineData ( "abcd", 2, true, "ab" )]
        [InlineData ( "abcd", 2, false, "ab" )]
        [InlineData ( "abcd ", 2, true, "ab" )]
        [InlineData ( "abcd ", 2, false, "abc" )]
        [InlineData ( "abcd       ", 2, true, "ab" )]
        [InlineData ( "abcd    ", 2, false, "abcd  " )]
        [InlineData ( "abcd ", 12, true, "" )]
        [InlineData ( "abcd ", 12, false, "" )]
        [InlineData ( "abcd", 3, true, "a" )]
        [InlineData ( "abcd", 3, false, "a" )]
        [InlineData ( "abcd", 4, true, "" )]
        [InlineData ( "abcd", 4, false, "" )]
        [InlineData ( "abcd", 5, true, "" )]
        [InlineData ( "abcd", 5, false, "" )]
        public void ShouldRemoveFromEnd ( string value, int charCount, bool trimEndFirst, string result )
        {
            Assert.True ( value.RemoveFromEnd ( charCount, trimEndFirst ) == result );
        }


        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( null, "", true, "" )]
        [InlineData ( null, "", false, "" )]
        [InlineData ( "", "", true, "" )]
        [InlineData ( "", "", false, "" )]
        [InlineData ( "", "a", true, "" )]
        [InlineData ( "", "a", false, "" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "fox", false, " jumps over the lazy dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "fox", true, "jumps over the lazy dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "fox ", true, "jumps over the lazy dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "fox ", false, "jumps over the lazy dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "dog", true, "" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "dog", false, "" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "bull ", false, "the quick brown fox jumps over the lazy dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "bull ", true, "the quick brown fox jumps over the lazy dog" )]
        [InlineData ( " the quick brown fox jumps over the lazy dog", "bull ", false, " the quick brown fox jumps over the lazy dog" )]
        [InlineData ( " the quick brown fox jumps over the lazy dog", "bull ", true, " the quick brown fox jumps over the lazy dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy brown dog", "the", false, " quick brown fox jumps over the lazy brown dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy brown dog", "the", true, "quick brown fox jumps over the lazy brown dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy brown dog", "the ", false, "quick brown fox jumps over the lazy brown dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy brown dog", "the ", true, "quick brown fox jumps over the lazy brown dog" )]
        public void SubStringAfterTests ( string value, string marker, bool trimResults, string result )
        {

            Assert.True ( value.SubStringAfter ( marker, StringComparison.CurrentCulture, trimResults ) == result );

        }


        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( null, "", true, "" )]
        [InlineData ( null, "", false, "" )]
        [InlineData ( "", "", true, "" )]
        [InlineData ( "", "", false, "" )]
        [InlineData ( "", "a", true, "" )]
        [InlineData ( "", "a", false, "" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "fox", false, " jumps over the lazy dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "fox", true, "jumps over the lazy dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "fox ", true, "jumps over the lazy dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "fox ", false, "jumps over the lazy dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "dog", true, "" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "dog", false, "" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "bull ", false, "the quick brown fox jumps over the lazy dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy dog", "bull ", true, "the quick brown fox jumps over the lazy dog" )]
        [InlineData ( " the quick brown fox jumps over the lazy dog", "bull ", false, " the quick brown fox jumps over the lazy dog" )]
        [InlineData ( " the quick brown fox jumps over the lazy dog", "bull ", true, " the quick brown fox jumps over the lazy dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy brown dog", "brown", false, " dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy brown dog", "brown", true, "dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy brown dog", "the", false, " lazy brown dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy brown dog", "the", true, "lazy brown dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy brown dog", "the ", false, "lazy brown dog" )]
        [InlineData ( "the quick brown fox jumps over the lazy brown dog", "the ", true, "lazy brown dog" )]
        public void SubStringAfterLastTests ( string value, string marker, bool trimResults, string result )
        {

            Assert.True ( value.SubStringAfterLast ( marker, StringComparison.CurrentCulture, trimResults ) == result );

        }


        // ---------------------------------------------------------------------------------



        [Theory]
        [InlineData ( "{}", "{", "}", "" )]
        [InlineData ( "{amduscia}", "{", "}", "amduscia" )]
        [InlineData ( " a band like {amduscia}", "{", "}", "amduscia" )]
        [InlineData ( "the quick brown fox jumps over the lazy brown dog", "quick", "lazy", " brown fox jumps over the " )]
        [InlineData ( "{\"Country\":\"ES\"}", "{", "}", "\"Country\":\"ES\"" )]
        [InlineData ( " {\"Country\":\"ES\",\"Active\":true, \"Age\":30, \"Title\":\"No reason to fear this\" }", "{", "}", "\"Country\":\"ES\",\"Active\":true, \"Age\":30, \"Title\":\"No reason to fear this\" " )]
        public void TakeBetweenTests ( string original, string s1, string s2, string result )
        {
            Assert.True ( original.TakeBetween ( s1, s2 ) == result );
        }


        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( "this is the message \"the email is used generally for all the automatic communication with the supplier - e.g. payment advice\" that was said to me",
            "\"", "\"", "the email is used generally for all the automatic communication with the supplier - e.g. payment advice" )]
        [InlineData ( "this is the message \"the email is used generally for all the \"automatic\" communication with the supplier - e.g. payment advice\" that was said to me",
             "\"", "\"", "the email is used generally for all the \"automatic\" communication with the supplier - e.g. payment advice" )]
        [InlineData ( "and then, what about this text with no coincidences?",
            "\"", "\"", "" )]
        public void TakeBetweenTests2 ( string original, string s1, string s2, string result )
        {
            Assert.True ( original.TakeBetween ( s1, s2 ) == result );
        }


        // ---------------------------------------------------------------------------------



        [Theory]
        [InlineData ( "{'P', 'Q'}", " ", "{", "}", "{'P','Q'}" )]
        [InlineData ( "{'P',     'Q'}", " ", "{", "}", "{'P','Q'}" )]
        [InlineData ( "{'P', 'Q', '2', 'birds'}", " ", "{", "}", "{'P','Q','2','birds'}" )]
        [InlineData ( "{'P',   'Q', '2',    'birds'}", " ", "{", "}", "{'P','Q','2','birds'}" )]

        [InlineData ( "hello boys and girls {'P', 'Q'} what was that?", " ", "{", "}", "hello boys and girls {'P','Q'} what was that?" )]
        [InlineData ( "hello boys and girls {'P',     'Q'} what was that?", " ", "{", "}", "hello boys and girls {'P','Q'} what was that?" )]
        [InlineData ( "hello boys and girls {'P', 'Q', '2', 'birds'} what was that?", " ", "{", "}", "hello boys and girls {'P','Q','2','birds'} what was that?" )]
        [InlineData ( "hello boys and girls {'P',   'Q', '2',    'birds'} what was that?", " ", "{", "}", "hello boys and girls {'P','Q','2','birds'} what was that?" )]

        [InlineData ( "dog{'P', 'Q'}god", " ", "dog{", "}god", "dog{'P','Q'}god" )]
        [InlineData ( "dog{'P',     'Q'}god", " ", "dog{", "}god", "dog{'P','Q'}god" )]
        [InlineData ( "dog{'P', 'Q', '2', 'birds'}god", " ", "dog{", "}god", "dog{'P','Q','2','birds'}god" )]
        [InlineData ( "dog{'P',   'Q', '2',    'birds'}god", " ", "dog{", "}god", "dog{'P','Q','2','birds'}god" )]

        [InlineData ( "random stirng before dog{'P', 'Q'}god", " ", "dog{", "}god", "random stirng before dog{'P','Q'}god" )]
        [InlineData ( "random stirng before dog{'P',     'Q'}god", " ", "dog{", "}god", "random stirng before dog{'P','Q'}god" )]
        [InlineData ( "random stirng before dog{'P', 'Q', '2', 'birds'}god", " ", "dog{", "}god", "random stirng before dog{'P','Q','2','birds'}god" )]
        [InlineData ( "random stirng before dog{'P',   'Q', '2',    'birds'}god", " ", "dog{", "}god", "random stirng before dog{'P','Q','2','birds'}god" )]

        public void RemoveBetweenTests ( string original, string removee, string s1, string s2, string result )
        {
            Assert.True ( original.RemoveBetween ( removee, s1, s2 ) == result );
        }

        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( "{'P', 'Q'}", " ", "{", "}", "{'P','Q'}" )]
        [InlineData ( "{'P',  'Q'}", " ", "{", "}", "{'P','Q'}" )]
        [InlineData ( "{'P',          'Q', '2', 'birds'}", " ", "{", "}", "{'P','Q','2','birds'}" )]
        [InlineData ( "{'P',      'Q',     '2',    'birds'}", " ", "{", "}", "{'P','Q','2','birds'}" )]
        public void RemoveBetweenTests2 ( string original, string removee, string s1, string s2, string result )
        {
            Assert.True ( original.RemoveBetween ( new List<String> { " ", "\t" }, s1, s2 ) == result );
        }


        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( "{'P', - 'Q'}", " ", "{", "}", "{'P','Q'}" )]
        [InlineData ( "{'P',   -- 'Q'}", " ", "{", "}", "{'P','Q'}" )]
        [InlineData ( "{'P',      -  -    -   'Q', '2', 'birds'}", " ", "{", "}", "{'P','Q','2','birds'}" )]
        [InlineData ( "{'P',  - - #    'Q',     '2',    'birds'}", " ", "{", "}", "{'P','Q','2','birds'}" )]
        public void RemoveBetweenTests3 ( string original, string removee, string s1, string s2, string result )
        {
            Assert.True ( original.RemoveBetween ( new List<String> { " ", "\t", "-", "#" }, s1, s2 ) == result );
        }


        // ---------------------------------------------------------------------------------


        //[InlineData ( null, " ", "{", "}", "{'P','Q'}", ExpectedException = typeof ( ArgumentException ), ExpectedMessage = "String cannot be null" )]
        //[InlineData ( "}'P',   -- 'Q'{", " ", "{", "}", "{'P','Q'}", ExpectedException = typeof ( Exception ), ExpectedMessage = "End string cannot appear earlier than beginning string" )]
        //public string RemoveBetweenTestsExceptions ( string original, string removee, string s1, string s2 )
        //{
        //    return original.RemoveBetween ( new List<String> { " ", "\t", "-", "#" }, s1, s2 );
        //}


        // ---------------------------------------------------------------------------------


        // this one should not change as island is not contained as a word inside the subject sentence
        [Theory]
        [InlineData ( "the islanders were insane", "island", "villag", "the islanders were insane" )]
        [InlineData ( "the islanders were insane", "island", "", "the islanders were insane" )]
        [InlineData ( "the islands were quite beautiful in the summer", "islands", "villages", "the villages were quite beautiful in the summer" )]
        [InlineData ( "some islands were quite beautiful in summer, other islands were better in winter", "islands", "villages",
            "some villages were quite beautiful in summer, other villages were better in winter" )]
        public void ReplaceWordTests ( string teststring, string newstring, string oldstring, string result )
        {
            Assert.True ( teststring.ReplaceWord ( newstring, oldstring ) == result );
        }


        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( "let sleeping dogs lie", "t sping dogs i" )]
        public void ImprovedRemoveTests_1 ( string line, string result )
        {
            Assert.True ( line.MultipleRemove ( new List<string> () { "l", "e" } ) == result );
        }

        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( "'All the things she did' was a track performed by \"The Whatevers\"", "All the things she did was a track performed by The Whatevers" )]
        public void ImprovedRemoveTests_2 ( string line, string result )
        {
            Assert.True ( line.MultipleRemove ( new List<string> () { "\"", "'" } ) == result );
        }


        // ---------------------------------------------------------------------------------

        [Theory]
        [InlineData ( "It's raining cats and dogs", "It's raining  and " )]
        public void ImprovedRemoveTests_3 ( string line, string result )
        {
            Assert.True ( line.MultipleRemove ( new List<string> () { "cats", "dogs" } ) == result );
        }

        // ---------------------------------------------------------------------------------

        [Theory]
        [InlineData ( "let sleeping dogs lie", "l", "", "et sleeping dogs lie" )]
        [InlineData ( "let sleeping dogs lie", "t", "", "le sleeping dogs lie" )]
        [InlineData ( "let sleeping dogs lie", "p", "", "let sleeing dogs lie" )]

        [InlineData ( "let sleeping dogs lie", "l", "@", "@et sleeping dogs lie" )]
        [InlineData ( "let sleeping dogs lie", "t", "KUZ", "leKUZ sleeping dogs lie" )]
        [InlineData ( "let sleeping dogs lie", "p", "1234567890", "let slee1234567890ing dogs lie" )]
        public void ReplaceFirstTests ( string line, string ocurrence, string replacement, string result )
        {
            if ( String.IsNullOrEmpty ( replacement ) )
                Assert.True ( line.ReplaceFirst ( ocurrence ) == result );
            Assert.True ( line.ReplaceFirst ( ocurrence, replacement ) == result );
        }


        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( "let sleeping dogs lie", "l", "", "let sleeping dogs ie" )]
        [InlineData ( "let sleeping dogs lie", "t", "", "le sleeping dogs lie" )]
        [InlineData ( "let sleeping dogs lie", "i", "", "let sleeping dogs le" )]

        [InlineData ( "let sleeping dogs lie", "l", "@", "let sleeping dogs @ie" )]
        [InlineData ( "let sleeping dogs lie", "t", "KUZ", "leKUZ sleeping dogs lie" )]
        [InlineData ( "let sleeping dogs lie", "g", "KUZ", "let sleeping doKUZs lie" )]
        [InlineData ( "let sleeping dogs lie", "e", "1234567890", "let sleeping dogs li1234567890" )]
        public void ReplaceLastTests ( string line, string occurrence, string replacement, string result )
        {
            if ( String.IsNullOrEmpty ( replacement ) )
            {
                var _1 = line.ReplaceLast ( occurrence );
                Assert.True ( _1 == result );
            }
            else
            {
                var _2 = line.ReplaceLast ( occurrence, replacement );
                Assert.True ( result == _2 );
            }
        }

        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( "let sleeping dogs lie", "l", "", "et sleeping dogs ie" )]
        [InlineData ( "let sleeping dogs lie", "l", "a588", "a588et sleeping dogs a588ie" )]
        [InlineData ( "She said several things, like \"yeah\", \"like\" and \"whatever\", I did not like her speech", "\"", "",
            "She said several things, like yeah\", \"like\" and \"whatever, I did not like her speech" )]
        [InlineData ( "She said \"yeah\"", "\"", "", "She said yeah" )]
        [InlineData ( "\"yeah, she said", "\"", "", "yeah, she said" )]
        public void ReplaceFirstAndLastOnlyTests ( string line, string occurrence, string replacement, string result )
        {
            if ( String.IsNullOrEmpty ( replacement ) )
                Assert.True ( line.ReplaceFirstAndLastOnly ( occurrence ) == result );
            Assert.True ( line.ReplaceFirstAndLastOnly ( occurrence, replacement ) == result );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
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

        [Fact]
        public void InsertMultiple_Test01 ()
        {
            var arr = new List<string> { "A", "C", "E", "H", "J", null };
            var insertees = new List<KeyValuePair<int, string>> ();
            insertees.Add ( new KeyValuePair<int, string> ( 1, "B" ) );
            insertees.Add ( new KeyValuePair<int, string> ( 2, "D" ) );
            insertees.Add ( new KeyValuePair<int, string> ( 3, "G" ) );
            insertees.Add ( new KeyValuePair<int, string> ( 0, null ) );
            var _new = arr.InsertMultiple ( insertees );
            Assert.True ( _new.Count () == 10 );
            Assert.True ( _new.Last () == null );
            Assert.True ( _new.First () == null );
        }

        // ---------------------------------------------------------------------------------


        [Fact]
        public void GetKeys_Test_01 ()
        {
            var insertees = new List<KeyValuePair<int, string>> ();
            insertees.Add ( new KeyValuePair<int, string> ( 1, "B" ) );
            insertees.Add ( new KeyValuePair<int, string> ( 2, "D" ) );
            insertees.Add ( new KeyValuePair<int, string> ( 3, "G" ) );
            var _keys = insertees.GetKeys ();
            Assert.True ( _keys.Count () == 3 );
            Assert.True ( _keys.First () == 1 );
            Assert.True ( _keys.Last () == 3 );
        }

        [Fact]
        public void GetKeys_Test_02 ()
        {
            var insertees = new List<Tuple<int, string>> ();
            insertees.Add ( new Tuple<int, string> ( 1, "B" ) );
            insertees.Add ( new Tuple<int, string> ( 2, "D" ) );
            insertees.Add ( new Tuple<int, string> ( 3, "G" ) );
            var _keys = insertees.GetKeys ();
            Assert.True ( _keys.Count () == 3 );
            Assert.True ( _keys.First () == 1 );
            Assert.True ( _keys.Last () == 3 );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void IsFirstTest_01 ()
        {
            Object o = null;
            Assert.False ( o.IsFirst<Object> ( null ) );
        }

        // ---------------------------------------------------------------------------------

        [Fact]
        public void IsFirstTest_02 ()
        {
            Object o = null;
            List<Object> list = new List<object> () { null, null };
            Assert.True ( o.IsFirst<Object> ( list ) );
        }

        // ---------------------------------------------------------------------------------

        [Fact]
        public void IsFirstTest_03 ()
        {
            Object o = "garks";
            List<Object> list = new List<object> () { null, null };
            Assert.False ( o.IsFirst<Object> ( list ) );
        }

        // ---------------------------------------------------------------------------------

        [Fact]
        public void IsFirstTest_04 ()
        {
            Object o = "garks";
            List<Object> list = new List<object> () { "garks", "yeyue" };
            Assert.True ( o.IsFirst<Object> ( list ) );
        }

        // ---------------------------------------------------------------------------------

        [Fact]
        public void IsFirstTest_05 ()
        {
            DummyPerson p1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
            DummyPerson p2 = new DummyPerson { Age = 33, Name = "Shelley", Occupation = "Carpenter" };
            DummyPerson p3 = new DummyPerson { Age = 43, Name = "Gars", Occupation = "Heavy drinker" };
            var list = new List<DummyPerson> () { p1, p2, p3 };
            Assert.True ( p1.IsFirst<DummyPerson> ( list ) );
        }

        // ---------------------------------------------------------------------------------



        [Fact]
        public void JoinTogether_Strings_01 ()
        {
            var sut = new [ ] { "a", "b", "c", "d", "e" };
            Assert.True ( sut.JoinTogether () == "abcde" );
        }

        [Fact]
        public void JoinTogether_Strings_02 ()
        {
            var sut = new [ ] { "a", "b", "c", "d", "e" };
            Assert.True ( sut.JoinTogether ( " " ) == "a b c d e" );
        }

        [Fact]
        public void JoinTogether_Strings_03 ()
        {
            var sut = new [ ] { "a", "b", "c", "d", "e" };
            Assert.True ( sut.JoinTogether ( ", " ) == "a, b, c, d, e" );
        }

        [Fact]
        public void JoinTogether_Numbers_01 ()
        {
            var sut = new [ ] { 1, 2, 3, 4, 5 };
            Assert.True ( sut.JoinTogether () == "12345" );
        }

        [Fact]
        public void JoinTogether_Numbers_02 ()
        {
            var sut = new [ ] { 1, 2, 3, 4, 5 };
            Assert.True ( sut.JoinTogether ( " " ) == "1 2 3 4 5" );
        }

        [Fact]
        public void JoinTogether_Numbers_03 ()
        {
            var sut = new [ ] { 1, 2, 3, 4, 5 };
            Assert.True ( sut.JoinTogether ( ", " ) == "1, 2, 3, 4, 5" );
        }

        [Fact]
        public void JoinTogether_Decimals_01 ()
        {
            var sut = new [ ] { 1.31m, 2.33m, 3.01m, 4.99m, 5.25m };
            Assert.True ( sut.JoinTogether () == "1,312,333,014,995,25" );
        }

        [Fact]
        public void JoinTogether_Decimals_02 ()
        {
            var sut = new [ ] { 1.31m, 2.33m, 3.01m, 4.99m, 5.25m };
            Assert.True ( sut.JoinTogether ( " " ) == "1,31 2,33 3,01 4,99 5,25" );

        }

        [Fact]
        public void JoinTogether_Decimals_03 ()
        {
            var sut = new [ ] { 1.31m, 2.33m, 3.01m, 4.99m, 5.25m };
            Assert.True ( sut.JoinTogether ( ", " ) == "1,31, 2,33, 3,01, 4,99, 5,25" );

        }

        [Fact]
        public void JoinTogether_Object ()
        {
            DummyPerson p1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
            DummyPerson p1_1 = new DummyPerson { Age = 23, Name = "John", Occupation = "NEET" };
            DummyPerson p2 = new DummyPerson { Age = 33, Name = "Shelley", Occupation = "Carpenter" };
            DummyPerson p3 = new DummyPerson { Age = 43, Name = "Gars", Occupation = "Heavy drinker" };
            var list = new List<DummyPerson> () { p1, p2, p3 };

            Assert.True ( list.JoinTogether ( " " ) == String.Format ( "{0} {1} {2}", p1.ToString (), p2.ToString (), p3.ToString () ) );
            Assert.True ( list.JoinTogether ( ", " ) == String.Format ( "{0}, {1}, {2}", p1.ToString (), p2.ToString (), p3.ToString () ) );

        }

        // ---------------------------------------------------------------------------------

        [Fact]
        public void JoinTogetherBetween_Strings_01 ()
        {
            var sut = new [ ] { "a", "b", "c", "d", "e" };
            Assert.True ( sut.JoinTogetherBetween ( 0, 2 ) == "abc" );
        }

        [Fact]
        public void JoinTogetherBetween_Strings_02 ()
        {
            var sut = new [ ] { "a", "b", "c", "d", "e" };
            Assert.True ( sut.JoinTogetherBetween ( 0, 2, " " ) == "a b c" );
        }

        [Fact]
        public void JoinTogetherBetween_Strings_03 ()
        {
            var sut = new [ ] { "a", "b", "c", "d", "e" };
            Assert.True ( sut.JoinTogetherBetween ( 2, 2, " " ) == "c" );
        }


        [Fact]
        public void JoinTogetherBetween_Strings_04 ()
        {
            var sut = new [ ] { "a", "b", "c", "d", "e" };
            Assert.True ( sut.JoinTogetherBetween ( 2, 4, " " ) == "c d e" );
        }

        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( "a", "", "a" )]
        [InlineData ( "", "", "" )]
        [InlineData ( "", " ", "" )]
        [InlineData ( "", " - ", "" )]
        [InlineData ( "yeah!", "", "yeah!" )]
        [InlineData ( "yeah!", "!", "yeah" )]
        [InlineData ( "yeah!!", "!", "yeah!" )]
        [InlineData ( "yeah!!", "!!", "yeah" )]
        [InlineData ( "yeah!!", "!!!", "yeah!!" )]
        [InlineData ( "a", " ", "a" )]
        [InlineData ( "a ", " ", "a" )]
        [InlineData ( "a  ", " ", "a " )]
        [InlineData ( "a  ", "  ", "a" )]
        public void RemoveLast ( string sut, string removee, string result )
        {
            Assert.True ( sut.RemoveLast ( removee ) == result );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void HasItems_Test ()
        {
            List<string> list = null;
            Assert.True ( !list.HasElements () );
        }

        [Fact]
        public void HasItems_Test_01 ()
        {
            List<string> list = new List<string> ();
            Assert.True ( !list.HasElements () );
        }

        [Fact]
        public void HasItems_Test_02 ()
        {
            List<string> list = new List<string> () { "" };
            Assert.True ( list.HasElements () );
        }

        [Fact]
        public void HasItems_Test_03 ()
        {
            List<string> list = new List<string> () { "xx" };
            Assert.True ( list.HasElements () );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void TakeAfterTests ()
        {
            var a = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var c = a.TakeAfter ( x => x >= 6 );
            Assert.True ( c.Count () == 4 );
            Assert.True ( c.First () == 7 );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
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
            Assert.True ( c.Count () == 3 );
            Assert.True ( c.First ().Name == "Linda" );
            Assert.True ( c.Last ().Name == "Pixie" );
        }


        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( "àëíôüñ", "aeioun" )]
        public void RemoveDiacritics_Tests ( string sut, string result )
        {
            Assert.True ( sut.RemoveDiacritics () == result );
        }


        // ---------------------------------------------------------------------------------


      
        [Theory]
        [InlineData ( "HELLO WORLD", 5, "HELLO" )]
        [InlineData ( "HELLO WORLD", 27, "HELLO WORLD" )]
        [InlineData ( "HELLO WORLD", 10, "HELLO WORL" )]
        [InlineData ( "HELLO WORLD", 11, "HELLO WORLD" )]
        public void TruncateStringTest ( string sut, int take, string result )
        {
            Assert.True ( sut.Truncate ( take ) == result );
        }


        // ---------------------------------------------------------------------------------


        [Theory]
        [InlineData ( "HELLO WORLD", 5, " WORLD" )]
        [InlineData ( "HELLO WORLD", 27, "HELLO WORLD" )]
        [InlineData ( "HELLO WORLD", 10, "D" )]
        [InlineData ( "HELLO WORLD", 11, "" )]
        public void TakeFromStringTest ( string sut, int take, string result )
        {
            Assert.True ( sut.TakeFrom ( take ) == result );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void AreAllNull_Test01 ()
        {
            var sut = new DummyPerson ();   // has all null fields where null is the default for the type
            Assert.True ( sut.AreAllNull ( new [ ] { "Name", "Occupation" } ) );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void AreAllNull_Test02 ()
        {
            var sut = new DummyPerson ();
            sut.Name = "Jonus";
            Assert.True ( sut.AreAllNull ( new [ ] { "Occupation" } ) );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void AreAllNull_Test03 ()
        {
            var sut = new DummyPerson ();
            sut.Name = "Jonus";
            sut.Occupation = "full time obnoxious f*ck";
            Assert.True ( !sut.AreAllNull ( new [ ] { "Name", "Occupation" } ) );
        }


        // ---------------------------------------------------------------------------------



        [Fact]
        public void AreAllNull_Test04 ()
        {
            var sut = new DummyPerson2 ();   // has all null fields where null is the default for the type
            Assert.True ( sut.AreAllNull ( new [ ] { "Name", "Occupation" } ) );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void AreAllNull_Test05 ()
        {
            var sut = new DummyPerson2 ();
            sut.Name = "Jonus";
            Assert.True ( sut.AreAllNull ( new [ ] { "Occupation" } ) );
        }


        // ---------------------------------------------------------------------------------


        [Fact]
        public void AreAllNull_Test06 ()
        {
            var sut = new DummyPerson2 ();
            sut.Age = 23;
            Assert.True ( sut.AreAllNull ( new [ ] { "Name" } ) );
        }


        // ---------------------------------------------------------------------------------

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

    public class DummyPerson2
    {
        public string Name { get; set; }
        public Nullable<int> Age { get; set; }
        public string Occupation { get; set; }

        public override string ToString ()
        {
            return String.Format ( "{0}, {1}", Name, Age.Value );
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
