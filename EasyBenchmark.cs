using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;


namespace EasyBenchmark
{
    [AttributeUsage( AttributeTargets.Method )]
    public class BenchmarkAttribute : Attribute { }

    public class BenchmarkCandidate
    {
        public MethodInfo MInfo { get; set; }
        public long Result { get; set; }
    }

    public class BenchmarkRunner
    {
        public static int RepetitionLimit = 100000;
        public static long BenchTimeLimit = 2000;

        public static void Run<TestClass>() where TestClass : new()
        {
            var CandidateList = new List<BenchmarkCandidate>();
            foreach( var M in typeof( TestClass ).GetMethods() )
                if( Attribute.IsDefined( M, typeof( BenchmarkAttribute ) ) )
                    CandidateList.Add( new BenchmarkCandidate() { MInfo = M, Result = 0 } );

            TestClass ClassInstance = new TestClass();
            var Timer = new Stopwatch();
            int LoopCounter;
            foreach( var Candidate in CandidateList )
            {
                Timer.Reset();
                Timer.Start();
                for( LoopCounter = 0; LoopCounter++ < RepetitionLimit && Timer.ElapsedMilliseconds < BenchTimeLimit; )
                    Candidate.MInfo.Invoke( ClassInstance, null );
                Timer.Stop();
                Candidate.Result = Timer.ElapsedTicks / LoopCounter;
            }

            #region PrintSummary
            int TitleWidth = 6 + CandidateList.Max( C => C.MInfo.Name.Length );
            int ResultWidth = 5 + CandidateList.Max( C => C.Result.ToString().Length );
            int TotalWidth = TitleWidth + ResultWidth;
            var HorizontalLine = new string( '-', TotalWidth );

            Console.WriteLine( "{0}\nBenchmark Summary\n{0}", HorizontalLine );
            Console.WriteLine( "Method{0}{1}Ticks\n{2}",
                               string.Empty.PadRight( TitleWidth - 6 ),
                               string.Empty.PadLeft( ResultWidth - 5 ),
                               HorizontalLine );
            foreach( var Candidate in CandidateList )
                Console.WriteLine( "{0}{1}{2}{3}",
                                    Candidate.MInfo.Name,
                                    string.Empty.PadRight( TitleWidth - Candidate.MInfo.Name.Length ),
                                    string.Empty.PadLeft( ResultWidth - Candidate.Result.ToString().Length ),
                                    Candidate.Result );
            Console.WriteLine( HorizontalLine );
            #endregion 

        }

    }
}
