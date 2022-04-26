public class IndexOf_Method
{
  //public static string RemoveSensitiveKeywords( string s ) { return s; }
  public static T getObject<T>( DataRow row ) where T : new() 
  {
      return getObject_impl<T, DataRow[]>( new DataRow[]{ row }, row.Table, 1 )[ 0 ];
  }
  
  public static List<T> getObject<T>( DataTable table ) where T : new() 
  {
      return getObject_impl<T, DataRowCollection>( table.Rows, table, table.Rows.Count ); 
  }
  
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
}