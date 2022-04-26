using System;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

public struct CaseInsensitivePattern
{
  public string Pattern;
  public static CaseInsensitivePattern BuildFrom( string Source )
    {
        CaseInsensitivePattern R;
        R.Pattern = Source.ToLower();
        return R;
    }
  public bool Match( string Target )
    {
        if( Pattern.Length != Target.Length ) return false;
        for( int i = 0; i < Pattern.Length; ++i )
        {
            char c = Target[ i ];
            if( 'A' <= Target[ i ] && Target[ i ] <= 'Z' ) c = (char)( Target[ i ] + 32 );
            if( c != Pattern[ i ] ) return false;
        }
        return true;
    }
};

public class Benchmark
{
  public static Stopwatch sw = new Stopwatch();
  public const int Repeat = 20000;

  public static long SetBaseline()
    {
        Action F = ()          => {};
        int BaselineSampleSize = 1000;
        for( int n = 0; n < BaselineSampleSize; ++n )
        {
            sw.Start();
            for( int i = 0; i < Repeat; ++i ) { F(); }
            sw.Stop();
        }
        Console.WriteLine( "Benchmark System Baseline : {0} ticks.", sw.ElapsedTicks / BaselineSampleSize );
        return sw.ElapsedTicks / BaselineSampleSize;
    }
  public static long swBaseline = SetBaseline();

  public static void Run( string Title, Action TestFunction )
    {
        sw.Reset();
        sw.Start();
        for( int i = 0; i < Repeat; ++i ) TestFunction();
        sw.Stop();

        Console.WriteLine( "Benchmark Result: {0,-24}|{1,7} ticks", Title, sw.ElapsedTicks - swBaseline );
    }
};

// clang-format off
public class Test_Class_1
{
    public int This_Is_Int { get; set; }
    public int This_Is_Also_Int { get; set; }
    public string This_Is_String { get; set; }
};

public class Test_Class_2
{
    public string This_Is_String { get; set; }
    public string This_Is_Also_String { get; set; }
    public int This_Is_Int { get; set; }
    public int This_Is_Also_Int { get; set; }
    public string This_Is_Not_Int { get; set; }
};

public class Tester
{
    public static string[] Samples = new string[]{ "This_Is_String", "This_Is_Also_String", "This_Is_Int", "This_Is_Also_Int", "This_Is_Not_Int" };
    public static int DummyCounter = 0;
    public static volatile int DummySource = 123;
    public static volatile bool DummyBool = true;

  public static Action TestFor( Func<string, string, bool> Matcher, string[] Patterns )
    {
        return () =>
        {
            foreach( var P in Patterns )
                foreach( var S in Samples )
                    if( Matcher( P, S ) ) DummyCounter = DummySource;
        };
    }
    
  public static Action TestFor( CaseInsensitivePattern[] Patterns )
  {
        return () =>
        {
            foreach( var P in Patterns )
                foreach( var S in Samples )
                    if( P.Match( S ) ) DummyCounter = DummySource;
        };
  }
    
  public static Action TestFor( Predicate<string>[] Matchers )
    {
        return () =>
        {
            foreach( var P in Matchers )
                foreach( var S in Samples )
                    if( P( S ) ) DummyCounter = DummySource;
        };
    }
};
// clang-format on

public class Program
{
  public static string RemoveSensitiveKeywords( string src ) { return src; }

  public static bool isEquivalentString( string str1, string str2 )
    {
        if( str1.Length != str2.Length ) return false;
        for( int i = 0; i < str1.Length; ++i )
        {
            int c1 = str1[ i ];
            int c2 = str2[ i ];
            if( 'A' <= str1[ i ] && str1[ i ] <= 'Z' ) c1 = str1[ i ] + 32;
            if( 'A' <= str2[ i ] && str2[ i ] <= 'Z' ) c2 = str2[ i ] + 32;
            if( c1 != c2 ) return false;
        }
        return true;
    }

  public static Predicate<string> PatternMatcher( string Pattern_ )
    {
        var Pattern__ = Pattern_.ToLower();
        return ( string Target ) =>
        {
            var Pattern = Pattern__;
            if( Pattern.Length != Target.Length ) return false;
            for( int i = 0; i < Pattern.Length; ++i )
            {
                char c = Target[ i ];
                if( 'A' <= Target[ i ] && Target[ i ] <= 'Z' ) c = (char)( Target[ i ] + 32 );
                if( c != Pattern[ i ] ) return false;
            }
            return true;
        };
    }

  public struct cachedTypeInfo<T>
    {
      public static R[] ExtractFromProperty<R>( Func<PropertyInfo, R> Extractor )
        {
            var Ps     = typeof( T ).GetProperties();
            R[] Result = new R[ Ps.Length ];
            int index  = 0;
            foreach( var P in Ps )
                Result[ index++ ] = Extractor( P );
            return Result;
        }
        // clang-format off
      public static readonly PropertyInfo[]           PropertyInfos = ExtractFromProperty( P => P );
      public static readonly string[]                 PropertyFieldNames = ExtractFromProperty( P => P.Name.ToLower() );
      public static readonly Type[]                   PropertyActualTypes = ExtractFromProperty( P => Nullable.GetUnderlyingType( P.PropertyType ) != null ? Nullable.GetUnderlyingType( P.PropertyType ) : P.PropertyType );
      public static readonly CaseInsensitivePattern[] PropertyFieldPatterns = ExtractFromProperty( P => CaseInsensitivePattern.BuildFrom( P.Name ) ) ;
      public static readonly Predicate<string>[]      PropertyFieldPatternMatchers = ExtractFromProperty( P => PatternMatcher( P.Name ) );
        // clang-format on
    };

  public T getObject<T>( DataRow row, List<string> columnsName ) where T : new()
    {
        T obj = new T();
        try
        {
            for( int index = 0; index < cachedTypeInfo<T>.PropertyFieldNames.Length; ++index )
            {
                string columnname = columnsName.Find( cachedTypeInfo<Test_Class_2>.PropertyFieldPatternMatchers[ index ] );
                // string columnname = columnsName.Find( name => isEquivalentString( name, cachedTypeInfo<T>.PropertyFieldNames[ index ] ) );
                if( ! string.IsNullOrEmpty( columnname ) )
                {
                    // row[string] may throw ArgumentException if columnname not found
                    // should move try catch around it, and skip empty check on columnname?
                    string value = row[ columnname ].ToString().Replace( "$", "" );
                    // string value = row[ columnname ].ToString().Replace( "$", "" ).Replace( ",", "" ); //requires actual data for justification

                    if( ! string.IsNullOrEmpty( value ) )
                    {
                        Type ActualType = cachedTypeInfo<T>.PropertyActualTypes[ index ];
                        PropertyInfo PI = cachedTypeInfo<T>.PropertyInfos[ index ];

                        if( ( ! string.IsNullOrEmpty( value ) ) && ActualType == typeof( string ) )
                            PI.SetValue( obj, RemoveSensitiveKeywords( value ) );  // wouldn't RemoveSensitiveKeywords remove '$' ',' for me?
                        else
                            PI.SetValue( obj, Convert.ChangeType( value, ActualType ) );  // Convert.ChangeType() maybe unnecessary?
                    }
                }
            }
        }
        catch
        {} /*no-op*/
        return obj;
    }

  public static int Main()
    {
        Benchmark.Run( "isEquivalentString", Tester.TestFor( isEquivalentString, cachedTypeInfo<Test_Class_2>.PropertyFieldNames ) );
        Benchmark.Run( "CaseInsensitivePattern", Tester.TestFor( cachedTypeInfo<Test_Class_2>.PropertyFieldPatterns ) );
        Benchmark.Run( "PatternMatchers", Tester.TestFor( cachedTypeInfo<Test_Class_2>.PropertyFieldPatternMatchers ) );

        Console.WriteLine( "End." );
        return 0;
    }
};
/* we may do this instead

public T getObject<T>( DataRow row, List<string> columnsName ) where T : new()
{
    T obj = new T();
    for (int index = 0; index < cachedTypeInfo<T>.PropertyLowerNames.Length; ++index)
    {
        try
        {
            string value = row[ cachedTypeInfo<T>.PropertyNames[index] ].ToString().Replace( "$", "" );
            // assume somehow cachedTypeInfo<T>.PropertyNames[index] is the field name used by the database

            cachedTypeInfo<T>.PropertyInfos[index]
            .SetValue( obj,
                       cachedTypeInfo<T>.PropertyActualTypes[index] == typeof( string )
                       ? RemoveSensitiveKeywords( value ) : value );
        }
        catch
        {}
    }
    return obj;
}

//*/