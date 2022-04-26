using System;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

public struct Tester
{
    public static List<string> Samples = new List<string>{ "This_Is_Also_Int", "HOW ABOUT ALL CAPITAL", "no This is not part of it." ,"This_Is_Also_String", "This_Is_Not_Int", "This_Is_String" };
    public static int DummyCounter = 0;
    public static volatile int DummySource = 123;
    public static volatile bool DummyBool = true;
};
// clang-format on

public struct CaseInsensitivePattern
{
  public string Pattern;
  public static CaseInsensitivePattern BuildFrom( string Source )
    {
        CaseInsensitivePattern R;
        R.Pattern = Source.ToLower();
        return R;
    }

   [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
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

   [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
  public int IndexIn( List<string> NameList )
    {
        for( int i = 0; i < NameList.Count; ++i )
            if( Match( NameList[ i ] ) ) return i;
        return -1;
    }
};

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

  public static readonly PropertyInfo[]                   PropertyInfos = ExtractFromProperty( P => P );
  public static readonly string[]                    PropertyFieldNames = ExtractFromProperty( P => P.Name.ToLower() );
  public static readonly Type[]                     PropertyActualTypes = ExtractFromProperty( P => Nullable.GetUnderlyingType( P.PropertyType ) != null ? Nullable.GetUnderlyingType( P.PropertyType ) : P.PropertyType );
  public static readonly CaseInsensitivePattern[] PropertyFieldPatterns = ExtractFromProperty( P => CaseInsensitivePattern.BuildFrom( P.Name ) );
};




public class Program
{
    public static void getObject_Old<T>( List<string> columnsName )
    {
        for (int index = 0; index < cachedTypeInfo<T>.PropertyFieldNames.Length; ++index)
        {
            string columnname = columnsName.Find( name => name.ToLower() == cachedTypeInfo<T>.PropertyFieldNames[ index ] );
            // string columnname = columnsName.Find( name => string.Compare( name, cachedTypeInfo<T>.PropertyFieldNames[ index ], StringComparison.CurrentCultureIgnoreCase )==0 );
            if( !string.IsNullOrEmpty( columnname ) )
            {
                Tester.DummyCounter += Tester.DummySource;
            }
        }
    }
    
    public static void getObject_New<T>( List<string> columnsName )
    {
        for( int index = 0; index < cachedTypeInfo<T>.PropertyFieldPatterns.Length; ++index )
        {
            int ColumnIndex = cachedTypeInfo<T>.PropertyFieldPatterns[ index ].IndexIn( columnsName );
            if( ColumnIndex != -1 )  // -1 means not found
            {
                Tester.DummyCounter += Tester.DummySource;
            }
        }
    }

    public static void TestOldWay()
    {
        getObject_Old<Test_Class_1>(Tester.Samples);
        getObject_Old<Test_Class_2>(Tester.Samples);
        getObject_Old<Test_Class_1>(Tester.Samples);
        getObject_Old<Test_Class_2>(Tester.Samples);
    }

    public static void TestNewWay()
    {
        getObject_New<Test_Class_1>(Tester.Samples);
        getObject_New<Test_Class_2>(Tester.Samples);
        getObject_New<Test_Class_1>(Tester.Samples);
        getObject_New<Test_Class_2>(Tester.Samples);
    }

  public static void Main()
    {
        Benchmark.Run( "Old Way", TestOldWay );
        Benchmark.Run( "New Way", TestNewWay );

        Console.WriteLine( "End." );
    }
};