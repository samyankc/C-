using System;
using System.Reflection;
using System.Diagnostics;



public class Test_Class_1
{
    public int This_Is_Int { get; set; }
    public int This_Is_Also_Int { get; set; }
    public string This_Is_String { get; set; }
}

public class Test_Class_2
{
    public string This_Is_String { get; set; }
    public string This_Is_Also_String { get; set; }
    public int This_Is_Int { get; set; }
    public int This_Is_Also_Int { get; set; }
    public string This_Is_Not_Int { get; set; }
}

public struct cachedTypeInfo<T>
{
	public static R[] ExtractFromProperty<R>(Func<PropertyInfo, R> Extractor)
	{
		var GivenProperties = typeof(T).GetProperties();
		R[] Result = new R[GivenProperties.Length];
		int index = 0;
		foreach (PropertyInfo Property in GivenProperties)
			Result[index++] = Extractor(Property);
		return Result;
	}

    public static readonly PropertyInfo[] PropertyInfos       = ExtractFromProperty(P => P);
	public static readonly string[]       PropertyLowerNames  = ExtractFromProperty(P => P.Name.ToLower());
	public static readonly Type[]         PropertyActualTypes = ExtractFromProperty(P => Nullable.GetUnderlyingType(P.PropertyType) != null 
                                                                                         ? Nullable.GetUnderlyingType(P.PropertyType) 
                                                                                         : P.PropertyType);
}



public class Program
{
    volatile static string dummy_str;
    volatile static Type dummy_type;
    static int Repeat = 20000;
    public static void Main()
    {

        long BaseLineTicks;

        { // #0
            var Ps = typeof(Test_Class_2).GetProperties();
            Stopwatch sw = new Stopwatch(); sw.Start(); 
            for(int i=0; i<Repeat; ++i)
            {
                foreach (var P in Ps ) {  }
                foreach (var P in Ps ) {  }
            }
            sw.Stop(); 
            BaseLineTicks = sw.ElapsedTicks;
            Console.WriteLine("===== Baseline ===== Ticks Taken: {0}",BaseLineTicks);
        }

        { // #1
            Stopwatch sw = new Stopwatch(); sw.Start(); 
            for(int i=0; i<Repeat; ++i)
            {
                foreach (var P in typeof(Test_Class_2).GetProperties() ) { dummy_str = P.Name; }
                foreach (var P in typeof(Test_Class_2).GetProperties() ) { dummy_type = P.PropertyType; }
            }
            sw.Stop(); Console.WriteLine("==================== Ticks Taken: {0}",sw.ElapsedTicks-BaseLineTicks);
        }

        { // #2
            Stopwatch sw = new Stopwatch(); sw.Start(); 
            for(int i=0; i<Repeat; ++i)
            {
                var Ps = typeof(Test_Class_2).GetProperties();
                foreach (var P in Ps ) { dummy_str = P.Name; }
                foreach (var P in Ps ) { dummy_type = P.PropertyType; }
            }
            sw.Stop(); Console.WriteLine("==================== Ticks Taken: {0}",sw.ElapsedTicks-BaseLineTicks);
        }

        { // #3
            Stopwatch sw = new Stopwatch(); sw.Start(); 
            var Ps = typeof(Test_Class_2).GetProperties();
            for(int i=0; i<Repeat; ++i)
            {
                foreach (var P in Ps ) { dummy_str = P.Name; }
                foreach (var P in Ps ) { dummy_type = P.PropertyType; }
            }
            sw.Stop(); Console.WriteLine("==================== Ticks Taken: {0}",sw.ElapsedTicks-BaseLineTicks);
        }

        { // #4
            Stopwatch sw = new Stopwatch(); sw.Start(); 
            for(int i=0; i<Repeat; ++i)
            {
                foreach (var P in cachedTypeInfo<Test_Class_2>.PropertyInfos ) { dummy_str = P.Name; }
                foreach (var P in cachedTypeInfo<Test_Class_2>.PropertyInfos ) { dummy_type = P.PropertyType; }
            }
            sw.Stop(); Console.WriteLine("==================== Ticks Taken: {0}",sw.ElapsedTicks-BaseLineTicks);
        }
        
        { // #5
            Stopwatch sw = new Stopwatch(); sw.Start(); 
            for(int i=0; i<Repeat; ++i)
            {
                foreach (var LowerName  in cachedTypeInfo<Test_Class_2>.PropertyLowerNames ) { dummy_str = LowerName; }
                foreach (var ActualType in cachedTypeInfo<Test_Class_2>.PropertyActualTypes) { dummy_type = ActualType; }
            }
            sw.Stop(); Console.WriteLine("==================== Ticks Taken: {0}",sw.ElapsedTicks-BaseLineTicks);
        }
    }
}


