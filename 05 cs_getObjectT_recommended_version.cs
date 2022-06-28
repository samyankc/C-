public class IndexOf_Method
{
  //public static string RemoveSensitiveKeywords( string s ) { return s; }
    public static T getObject<T>( DataRow row ) where T : new() 
    {
        return getObject_impl<T>( new DataRow[]{ row })[ 0 ]; 
    }

    public static List<T> getObject<T>( DataTable table ) where T : new() 
    {
        return getObject_impl<T>( table.AsEnumerable() ); 
    }

    public static List<T> getObject_impl<T>( IEnumerable<DataRow> RowContainer ) where T : new()
    {
        var ref_table = RowContainer.First().Table;
        var RowCount = RowContainer.Count();
        
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
}