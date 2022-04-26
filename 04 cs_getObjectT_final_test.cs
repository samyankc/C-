// https://docs.microsoft.com/en-us/dotnet/api/system.data.datatable?view=netframework-4.5
using System;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Benchmark
{
  public static Stopwatch sw = new Stopwatch();
  public const int Repeat = 100;

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
        return sw.ElapsedTicks / BaselineSampleSize;
    }
  public static long swBaseline = SetBaseline();

  public static void Run( string Title, Action TestFunction )
    {
        sw.Reset();
        sw.Start();
        for( int i = 0; i < Repeat; ++i ) TestFunction();
        sw.Stop();

        Console.WriteLine( " Result: {0,-24}|{1,7} ticks", Title, ( sw.ElapsedTicks - swBaseline ) / Repeat );
    }
};

public struct cachedTypeInfo<T>
{
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

  public static R[] ExtractFromProperty<R>( Func<PropertyInfo, R> Extractor )
    {
        var Ps     = typeof( T ).GetProperties();
        R[] Result = new R[ Ps.Length ];
        int index  = 0;
        foreach( var P in Ps )
            Result[ index++ ] = Extractor( P );
        return Result;
    }

  public static readonly PropertyInfo[] PropertyInfos = ExtractFromProperty( P => P );
  public static readonly string[] PropertyFieldNames = ExtractFromProperty( P => P.Name.ToLower() );
  public static readonly Type[] PropertyActualTypes = ExtractFromProperty( P => Nullable.GetUnderlyingType( P.PropertyType ) != null ? Nullable.GetUnderlyingType( P.PropertyType ) : P.PropertyType );
  public static readonly CaseInsensitivePattern[] PropertyFieldPatterns = ExtractFromProperty( P => CaseInsensitivePattern.BuildFrom( P.Name ) );
};

public class TestSample
{
  public static DataTable MakeSample()
    {
        DataTable t = new DataTable( "Data Table - Sample" );

        DataColumn[] cols = {
            new DataColumn( "ID", typeof( int ) ),                      //
            new DataColumn( "This_Is_Int", typeof( int ) ),             //
            new DataColumn( "This_Is_String", typeof( string ) ),       //
            new DataColumn( "This_Is_Also_Int", typeof( int ) ),        //
            new DataColumn( "This_Is_Also_String", typeof( string ) ),  //
            new DataColumn( "These", typeof( string ) ),                //
            new DataColumn( "Fields", typeof( string ) ),               //
            new DataColumn( "Will", typeof( string ) ),                 //
            new DataColumn( "Not", typeof( string ) ),                  //
            new DataColumn( "Be", typeof( string ) ),                   //
            new DataColumn( "In", typeof( string ) ),                   //
            new DataColumn( "Any", typeof( string ) ),                  //
            new DataColumn( "Test", typeof( string ) ),                 //
            new DataColumn( "Class", typeof( string ) )                 //
            // new DataColumn( "THIS_IS_INT", typeof( int ) ),             //
            // new DataColumn( "THIS_IS_STRING", typeof( string ) ),       //
            // new DataColumn( "THIS_IS_ALSO_INT", typeof( int ) ),        //
            // new DataColumn( "THIS_IS_ALSO_STRING", typeof( string ) ),  //
            // new DataColumn( "THESE", typeof( string ) ),                //
            // new DataColumn( "FIELDS", typeof( string ) ),               //
            // new DataColumn( "WILL", typeof( string ) ),                 //
            // new DataColumn( "NOT", typeof( string ) ),                  //
            // new DataColumn( "BE", typeof( string ) ),                   //
            // new DataColumn( "IN", typeof( string ) ),                   //
            // new DataColumn( "ANY", typeof( string ) ),                  //
            // new DataColumn( "TEST", typeof( string ) ),                 //
            // new DataColumn( "CLASS", typeof( string ) )                 //

        };

        t.Columns.AddRange( cols );
        t.PrimaryKey = new DataColumn[]{ t.Columns[ "ID" ] };

        Object[] rows = {
            new Object[]{ 1, 10, "row 1 str 1", 100, "row 1 str 2", "row 1 str 3" },  //
            new Object[]{ 2, 20, "row 2 str 1", 200, "row 2 str 2", "row 2 str 3" },  //
            new Object[]{ 3, 30, "row 3 str 1", 300, "row 3 str 2", "row 3 str 3" },  //
            new Object[]{ 4, 40, "row 4 str 1", 400, "row 4 str 2", "row 4 str 3" },  //
            new Object[]{ 5, 50, "row 5 str 1", 500, "row 5 str 2", "row 5 str 3" },  //
            new Object[]{ 6, 60, "row 6 str 1", 600, "row 6 str 2", "row 6 str 3" },  //
            new Object[]{ 7, 70, "row 7 str 1", 700, "row 7 str 2", "row 7 str 3" },  //
            new Object[]{ 8, 80, "row 8 str 1", 800, "row 8 str 2", "row 8 str 3" },  //
            new Object[]{ 9, 90, "row 9 str 1", 900, "row 9 str 2", "row 9 str 3" }   //
        };

        foreach( Object[] row in rows )
            t.Rows.Add( row );

        int ExtraRowsCount    = 3000;
        int NewTotalRowsCount = t.Rows.Count + ExtraRowsCount;
        for( int i = t.Rows.Count + 1; i < NewTotalRowsCount + 1; ++i ) t.Rows.Add( new Object[]{ i, i * 10, "extra str1", i * 100, "extra str2", "extra str3" } );

        return t;
    }
  public static DataTable SampleTable = MakeSample();
};

// clang-format off
public class Test_Class_1
{
    public int Not_This_One_1{ get; set; }
    public int Not_This_One_2{ get; set; }
    public string Not_This_One_3 { get; set; }
    
    public int This_Is_Int { get; set; }
    public int This_Is_Also_Int { get; set; }
    public string This_Is_String { get; set; }
};

public class Test_Class_2
{
    public string This_Is_String { get; set; }
    public string This_Is_Also_String { get; set; }
    public int This_Is_Not_In_DataTable { get; set; }
    public int This_Is_Int { get; set; }
    public string This_Is_Not_Int { get; set; }
    
    public string This_Is_St1ring { get; set; }
    public string This_Is1_Also_String { get; set; }
    public int This_Is_No2t_In_DataTable { get; set; }
    public int This_Is_I3nt { get; set; }
    public string This_I4s_Not_Int { get; set; }
    
    public string This_Is_7String { get; set; }
    public string This_Is5_Also_String { get; set; }
    public int This_Is_8Not_In_DataTable { get; set; }
    public int This_Is_9Int { get; set; }
    public string This_Is0_Not_Int { get; set; }
};

// clang-format on

public class PatternMatcher_Method
{
  public static string RemoveSensitiveKeywords( string s ) { return s; }

  public static T getObject<T>( DataRow row ) where T : new() { return getObject_impl<T, DataRow[]>( new DataRow[]{ row }, row.Table, 1 )[ 0 ]; }

  public static List<T> getObject<T>( DataTable table ) where T : new() { return getObject_impl<T, DataRowCollection>( table.Rows, table, table.Rows.Count ); }

  public static List<T> getObject_impl<T, R>( R RowContainer, DataTable ref_table, int RowCount ) where T : new() where R : IEnumerable
    {
        List<T> Result = new List<T>( RowCount );
        for( int i = 0; i < RowCount; ++i ) Result.Add( new T() );

        var PropertyInfos = cachedTypeInfo<T>.PropertyInfos;
        var FieldPatterns = cachedTypeInfo<T>.PropertyFieldPatterns;
        foreach( DataColumn col in ref_table.Columns )
        {
            for( int i = 0; i < FieldPatterns.Length; ++i )
                if( FieldPatterns[ i ].Match( col.ColumnName ) )
                {
                    var FieldPropertyInfo = cachedTypeInfo<T>.PropertyInfos[ i ];
                    var FieldActualType   = cachedTypeInfo<T>.PropertyActualTypes[ i ];
                    int ResultIndex       = -1;

                    foreach( DataRow row in RowContainer )  // assume stable iteration order ??
                    {
                        ++ResultIndex;
                        string value = row[ col ].ToString().Replace( "$", "" );
                        if( ! string.IsNullOrEmpty( value ) )
                        {
                            if( FieldActualType == typeof( string ) )
                                FieldPropertyInfo.SetValue( Result[ ResultIndex ], RemoveSensitiveKeywords( value ) );
                            else
                                FieldPropertyInfo.SetValue( Result[ ResultIndex ], Convert.ChangeType( value, FieldActualType ) );
                        }
                    }
                    break;
                }
        }
        return Result;
    }

  public static void RunBenchmark()
    {
        Test_Class_1 Dummy_Test_Object_1;
        Test_Class_2 Dummy_Test_Object_2;
        List<Test_Class_1> Dummy_Test_List_1;
        List<Test_Class_2> Dummy_Test_List_2;
        Console.WriteLine( "Benchmarking : {0}", MethodBase.GetCurrentMethod().DeclaringType.Name );

        var SampleTableClone = TestSample.SampleTable.Clone();
        SampleTableClone.ImportRow( TestSample.SampleTable.Rows[ 2 ] );
        Benchmark.Run(
            "Single Row Table", () => {
                Dummy_Test_Object_1 = getObject<Test_Class_1>( SampleTableClone )[ 0 ];
                Dummy_Test_Object_2 = getObject<Test_Class_2>( SampleTableClone )[ 0 ];
            } );

        Benchmark.Run(
            "Single Row", () => {
                Dummy_Test_Object_1 = getObject<Test_Class_1>( TestSample.SampleTable.Rows[ 2 ] );
                Dummy_Test_Object_2 = getObject<Test_Class_2>( TestSample.SampleTable.Rows[ 2 ] );
            } );
        Benchmark.Run(
            "Whole Table", () => {
                Dummy_Test_List_1 = getObject<Test_Class_1>( TestSample.SampleTable );
                Dummy_Test_List_2 = getObject<Test_Class_2>( TestSample.SampleTable );
            } );
    }
  public static void RunUnitTest()
    {
        Console.WriteLine( "Unit Testing : {0}", MethodBase.GetCurrentMethod().DeclaringType.Name );

        int TestCount = 0;
        int PassCount = 0;

        foreach( DataRow row in TestSample.SampleTable.Rows )
        {
            var Result1 = getObject<Test_Class_1>( row );
            foreach( var Property in typeof( Test_Class_1 ).GetProperties() )
            {
                try
                {
                    var Expected = row[ Property.Name ];
                    var Actual   = Property.GetValue( Result1 );
                    ++TestCount;
                    if( Expected.Equals( Actual ) )
                        ++PassCount;
                    else
                        Console.WriteLine( "Test #{0} : Fail*  - [{1}] vs [{2}] ", TestCount, Expected, Actual );
                }
                catch
                {}
            }
        }

        foreach( DataRow row in TestSample.SampleTable.Rows )
        {
            var Result1 = getObject<Test_Class_2>( row );
            foreach( var Property in typeof( Test_Class_2 ).GetProperties() )
            {
                try
                {
                    var Expected = row[ Property.Name ];
                    var Actual   = Property.GetValue( Result1 );
                    ++TestCount;
                    if( Expected.Equals( Actual ) )
                        ++PassCount;
                    else
                        Console.WriteLine( "Test #{0} : Fail*  - [{1}] vs [{2}] ", TestCount, Expected, Actual );
                }
                catch
                {}
            }
        }

        {
            int RowIndex = 0;
            foreach( var Result2 in getObject<Test_Class_1>( TestSample.SampleTable ) )  // List<T>
            {
                DataRow row = TestSample.SampleTable.Rows[ RowIndex++ ];
                foreach( var Property in typeof( Test_Class_1 ).GetProperties() )
                {
                    try
                    {
                        var Expected = row[ Property.Name ];
                        var Actual   = Property.GetValue( Result2 );
                        ++TestCount;
                        if( Expected.Equals( Actual ) )
                            ++PassCount;
                        else
                            Console.WriteLine( "Test #{0} : Fail*  - [{1}] vs [{2}] ", TestCount, Expected, Actual );
                    }
                    catch
                    {}
                }
            }
        }

        {
            int RowIndex = 0;
            foreach( var Result2 in getObject<Test_Class_2>( TestSample.SampleTable ) )  // List<T>
            {
                DataRow row = TestSample.SampleTable.Rows[ RowIndex++ ];
                foreach( var Property in typeof( Test_Class_2 ).GetProperties() )
                {
                    try
                    {
                        var Expected = row[ Property.Name ];
                        var Actual   = Property.GetValue( Result2 );
                        ++TestCount;
                        if( Expected.Equals( Actual ) )
                            ++PassCount;
                        else
                            Console.WriteLine( "Test #{0} : Fail*  - [{1}] vs [{2}] ", TestCount, Expected, Actual );
                    }
                    catch
                    {}
                }
            }
        }

        Console.WriteLine( "   Pass Rate : [ {0} % ] in {1} test cases", PassCount * 100 / TestCount, TestCount );
    }
};

public class IndexOf_Method
{
  public static string RemoveSensitiveKeywords( string s ) { return s; }

  public static T getObject<T>( DataRow row ) where T : new() { return getObject_impl<T, DataRow[]>( new DataRow[]{ row }, row.Table, 1 )[ 0 ]; }
  public static List<T> getObject<T>( DataTable table ) where T : new() { return getObject_impl<T, DataRowCollection>( table.Rows, table, table.Rows.Count ); }
  public static List<T> getObject_impl<T, R>( R RowContainer, DataTable ref_table, int RowCount ) where T : new() where R : IEnumerable
    {
        List<T> Result = new List<T>( RowCount );
        for( int i = 0; i < RowCount; ++i ) Result.Add( new T() );

        foreach( var Property in typeof( T ).GetProperties() )
        {
            int ColIndex = ref_table.Columns.IndexOf( Property.Name );
            if( ColIndex != -1 )
            {
                int ResultIndex = -1;
                foreach( DataRow row in RowContainer )
                {
                    ++ResultIndex;
                    string value = row[ ColIndex ].ToString().Replace( "$", "" );
                    if( ! string.IsNullOrEmpty( value ) )
                    {
                        if( Property.PropertyType == typeof( string ) )
                            Property.SetValue( Result[ ResultIndex ], RemoveSensitiveKeywords( value ) );
                        else
                            Property.SetValue( Result[ ResultIndex ], Convert.ChangeType( value, Property.PropertyType ) );
                    }
                }
            }
        }
        return Result;
    }
    
  public static void RunBenchmark()
    {
        Test_Class_1 Dummy_Test_Object_1;
        Test_Class_2 Dummy_Test_Object_2;
        List<Test_Class_1> Dummy_Test_List_1;
        List<Test_Class_2> Dummy_Test_List_2;
        Console.WriteLine( "Benchmarking : {0}", MethodBase.GetCurrentMethod().DeclaringType.Name );

        var SampleTableClone = TestSample.SampleTable.Clone();
        SampleTableClone.ImportRow( TestSample.SampleTable.Rows[ 2 ] );
        Benchmark.Run(
            "Single Row Table", () => {
                Dummy_Test_Object_1 = getObject<Test_Class_1>( SampleTableClone )[ 0 ];
                Dummy_Test_Object_2 = getObject<Test_Class_2>( SampleTableClone )[ 0 ];
            } );

        Benchmark.Run(
            "Single Row", () => {
                Dummy_Test_Object_1 = getObject<Test_Class_1>( TestSample.SampleTable.Rows[ 2 ] );
                Dummy_Test_Object_2 = getObject<Test_Class_2>( TestSample.SampleTable.Rows[ 2 ] );
            } );
        Benchmark.Run(
            "Whole Table", () => {
                Dummy_Test_List_1 = getObject<Test_Class_1>( TestSample.SampleTable );
                Dummy_Test_List_2 = getObject<Test_Class_2>( TestSample.SampleTable );
            } );
    }
  public static void RunUnitTest()
    {
        Console.WriteLine( "Unit Testing : {0}", MethodBase.GetCurrentMethod().DeclaringType.Name );

        int TestCount = 0;
        int PassCount = 0;

        foreach( DataRow row in TestSample.SampleTable.Rows )
        {
            var Result1 = getObject<Test_Class_1>( row );
            foreach( var Property in typeof( Test_Class_1 ).GetProperties() )
            {
                try
                {
                    var Expected = row[ Property.Name ];
                    var Actual   = Property.GetValue( Result1 );
                    ++TestCount;
                    if( Expected.Equals( Actual ) )
                        ++PassCount;
                    else
                        Console.WriteLine( "Test #{0} : Fail*  - [{1}] vs [{2}] ", TestCount, Expected, Actual );
                }
                catch
                {}
            }
        }

        foreach( DataRow row in TestSample.SampleTable.Rows )
        {
            var Result1 = getObject<Test_Class_2>( row );
            foreach( var Property in typeof( Test_Class_2 ).GetProperties() )
            {
                try
                {
                    var Expected = row[ Property.Name ];
                    var Actual   = Property.GetValue( Result1 );
                    ++TestCount;
                    if( Expected.Equals( Actual ) )
                        ++PassCount;
                    else
                        Console.WriteLine( "Test #{0} : Fail*  - [{1}] vs [{2}] ", TestCount, Expected, Actual );
                }
                catch
                {}
            }
        }

        {
            int RowIndex = 0;
            foreach( var Result2 in getObject<Test_Class_1>( TestSample.SampleTable ) )  // List<T>
            {
                DataRow row = TestSample.SampleTable.Rows[ RowIndex++ ];
                foreach( var Property in typeof( Test_Class_1 ).GetProperties() )
                {
                    try
                    {
                        var Expected = row[ Property.Name ];
                        var Actual   = Property.GetValue( Result2 );
                        ++TestCount;
                        if( Expected.Equals( Actual ) )
                            ++PassCount;
                        else
                            Console.WriteLine( "Test #{0} : Fail*  - [{1}] vs [{2}] ", TestCount, Expected, Actual );
                    }
                    catch
                    {}
                }
            }
        }

        {
            int RowIndex = 0;
            foreach( var Result2 in getObject<Test_Class_2>( TestSample.SampleTable ) )  // List<T>
            {
                DataRow row = TestSample.SampleTable.Rows[ RowIndex++ ];
                foreach( var Property in typeof( Test_Class_2 ).GetProperties() )
                {
                    try
                    {
                        var Expected = row[ Property.Name ];
                        var Actual   = Property.GetValue( Result2 );
                        ++TestCount;
                        if( Expected.Equals( Actual ) )
                            ++PassCount;
                        else
                            Console.WriteLine( "Test #{0} : Fail*  - [{1}] vs [{2}] ", TestCount, Expected, Actual );
                    }
                    catch
                    {}
                }
            }
        }

        Console.WriteLine( "   Pass Rate : [ {0} % ] in {1} test cases", PassCount * 100 / TestCount, TestCount );
    }
};

public class Original_Method
{
  public static string RemoveSensitiveKeywords( string s ) { return s; }

  public static T getObject<T>( DataRow row, List<string> ColName ) where T : new() { return getObject_impl<T>( row, ColName ); }

  public static List<T> getObject<T>( DataTable table, List<string> ColName ) where T : new()
    {
        List<T> Result = new List<T>( table.Rows.Count );
        foreach( DataRow row in table.Rows )
            Result.Add( getObject_impl<T>( row, ColName ) );
        return Result;
    }

  public static T getObject_impl<T>( DataRow row, List<string> ColNames ) where T : new()
    {
        T Result = new T();

        foreach( var Property in typeof( T ).GetProperties() )
        {
            string ColName = ColNames.Find( name => Property.Name.ToLower() == name.ToLower() );

            if( ! string.IsNullOrEmpty( ColName ) )
            {
                string value = row[ ColName ].ToString().Replace( "$", "" );
                if( ! string.IsNullOrEmpty( value ) )
                {
                    if( Property.PropertyType == typeof( string ) )
                        Property.SetValue( Result, RemoveSensitiveKeywords( value ) );
                    else
                        Property.SetValue( Result, Convert.ChangeType( value, Property.PropertyType ) );
                }
            }
        }
        return Result;
    }
  public static void RunBenchmark()
    {
        Test_Class_1 Dummy_Test_Object_1;
        Test_Class_2 Dummy_Test_Object_2;
        List<Test_Class_1> Dummy_Test_List_1;
        List<Test_Class_2> Dummy_Test_List_2;
        Console.WriteLine( "Benchmarking : {0}", MethodBase.GetCurrentMethod().DeclaringType.Name );

        
        List<string> ColNames = new List<string>(TestSample.SampleTable.Columns.Count);
        foreach(DataColumn Col in TestSample.SampleTable.Columns )
            ColNames.Add(Col.ColumnName);
      
        var SampleTableClone = TestSample.SampleTable.Clone();
        SampleTableClone.ImportRow( TestSample.SampleTable.Rows[ 2 ] );
        Benchmark.Run(
            "Single Row Table", () => {
                Dummy_Test_Object_1 = getObject<Test_Class_1>( SampleTableClone,ColNames)[ 0 ];
                Dummy_Test_Object_2 = getObject<Test_Class_2>( SampleTableClone,ColNames)[ 0 ];
            } );

        Benchmark.Run(
            "Single Row", () => {
                Dummy_Test_Object_1 = getObject<Test_Class_1>( TestSample.SampleTable.Rows[ 2 ],ColNames );
                Dummy_Test_Object_2 = getObject<Test_Class_2>( TestSample.SampleTable.Rows[ 2 ],ColNames );
            } );
        Benchmark.Run(
            "Whole Table", () => {
                Dummy_Test_List_1 = getObject<Test_Class_1>( TestSample.SampleTable,ColNames );
                Dummy_Test_List_2 = getObject<Test_Class_2>( TestSample.SampleTable,ColNames );
            } );
    }
  public static void RunUnitTest()
    {
        Console.WriteLine( "Unit Testing : {0}", MethodBase.GetCurrentMethod().DeclaringType.Name );

        List<string> ColNames = new List<string>(TestSample.SampleTable.Columns.Count);
        foreach(DataColumn Col in TestSample.SampleTable.Columns )
            ColNames.Add(Col.ColumnName);
      

        int TestCount = 0;
        int PassCount = 0;

        foreach( DataRow row in TestSample.SampleTable.Rows )
        {
            var Result1 = getObject<Test_Class_1>( row ,ColNames );
            foreach( var Property in typeof( Test_Class_1 ).GetProperties() )
            {
                try
                {
                    var Expected = row[ Property.Name ];
                    var Actual   = Property.GetValue( Result1 );
                    ++TestCount;
                    if( Expected.Equals( Actual ) )
                        ++PassCount;
                    else
                        Console.WriteLine( "Test #{0} : Fail*  - [{1}] vs [{2}] ", TestCount, Expected, Actual );
                }
                catch
                {}
            }
        }

        foreach( DataRow row in TestSample.SampleTable.Rows )
        {
            var Result1 = getObject<Test_Class_2>( row ,ColNames);
            foreach( var Property in typeof( Test_Class_2 ).GetProperties() )
            {
                try
                {
                    var Expected = row[ Property.Name ];
                    var Actual   = Property.GetValue( Result1 );
                    ++TestCount;
                    if( Expected.Equals( Actual ) )
                        ++PassCount;
                    else
                        Console.WriteLine( "Test #{0} : Fail*  - [{1}] vs [{2}] ", TestCount, Expected, Actual );
                }
                catch
                {}
            }
        }

        {
            int RowIndex = 0;
            foreach( var Result2 in getObject<Test_Class_1>( TestSample.SampleTable,ColNames ) )  // List<T>
            {
                DataRow row = TestSample.SampleTable.Rows[ RowIndex++ ];
                foreach( var Property in typeof( Test_Class_1 ).GetProperties() )
                {
                    try
                    {
                        var Expected = row[ Property.Name ];
                        var Actual   = Property.GetValue( Result2 );
                        ++TestCount;
                        if( Expected.Equals( Actual ) )
                            ++PassCount;
                        else
                            Console.WriteLine( "Test #{0} : Fail*  - [{1}] vs [{2}] ", TestCount, Expected, Actual );
                    }
                    catch
                    {}
                }
            }
        }

        {
            int RowIndex = 0;
            foreach( var Result2 in getObject<Test_Class_2>( TestSample.SampleTable ,ColNames) )  // List<T>
            {
                DataRow row = TestSample.SampleTable.Rows[ RowIndex++ ];
                foreach( var Property in typeof( Test_Class_2 ).GetProperties() )
                {
                    try
                    {
                        var Expected = row[ Property.Name ];
                        var Actual   = Property.GetValue( Result2 );
                        ++TestCount;
                        if( Expected.Equals( Actual ) )
                            ++PassCount;
                        else
                            Console.WriteLine( "Test #{0} : Fail*  - [{1}] vs [{2}] ", TestCount, Expected, Actual );
                    }
                    catch
                    {}
                }
            }
        }

        Console.WriteLine( "   Pass Rate : [ {0} % ] in {1} test cases", PassCount * 100 / TestCount, TestCount );
    }
};

public class Program
{
  public static void Main()
    {
        PatternMatcher_Method.RunUnitTest();
        IndexOf_Method.RunUnitTest();
        // Original_Method.RunUnitTest();

        Console.WriteLine( "----------------------" );

        PatternMatcher_Method.RunBenchmark();
        IndexOf_Method.RunBenchmark();
        // Original_Method.RunBenchmark();
    }
};