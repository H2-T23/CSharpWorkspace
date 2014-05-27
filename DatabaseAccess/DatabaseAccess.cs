// DatabaseAccess.cs 
using System;
using System.IO;
using System.Data.SqlClient;

namespace DATABASEACCESS {

	class CDatabaseAccess {
		SqlConnection		con	= null;

		public CDatabaseAccess(){
		}

		~CDatabaseAccess(){
			Close();
		}

		public bool		IsOpen(){
			return(con != null);
		}

		public String	GenConnectionString( String strServer, String strDatabase, String strSecurity ){
			return String.Format("Data Source={0};Initial Catalog={1};Integrated Security={2};", strServer, strDatabase, strSecurity);
		}

		public bool		Open( String strServer, String strDatabase, String strSecurity ){
			String strConnection = GenConnectionString(strServer, strDatabase, strSecurity);

			Console.WriteLine( strConnection );

			bool	result = false;

			try{
				con		= new SqlConnection( strConnection );
				con.Open();

				result	= true;
			}
			catch( Exception ){
				result	= false;
				Close();
			}

			return result;
		}

		public void		Close(){
			if( IsOpen() ){
				con.Close();
				con.Dispose();
				con	= null;
			}
		}

		public bool				QueryProcedure( String strQuery ){

			if( !IsOpen() ){
				return false;
			}

			bool			result	= false;
		/*
			SqlCommand		cmd		= null;
			try{
				cmd = con.CreateCommand();
				cmd.CommandText	= strQuery;
				cmd.ExecuteNonQuery();

				result	= true;
			}
			catch( Exception ){
				result	= false;
			}
			finally{
				if( cmd != null ){
					cmd.Dispose();
				}
			}
		*/
			using(SqlCommand cmd = con.CreateCommand()){
				try{
					cmd.CommandText	= strQuery;
					cmd.ExecuteNonQuery();
					result	= true;
				}catch( Exception ){
					result	= false;
				}
			}
			return result;
		}

		public SqlDataReader	QueryFunction( String strQuery ){
			
			if( !IsOpen() ){
				return null;
			}

			SqlDataReader	result	= null;
		/*
			SqlCommand		cmd		= null;	
			try{
				cmd	= con.CreateCommand();
				cmd.CommandText	= strQuery;
				result	= cmd.ExecuteReader();
			}
			catch( Exception ){
				if( result != null ){
					result.Close();
					result	= null;
				}
			}
			finally{
				if( cmd != null ){
					cmd.Dispose();
				}
			}
		*/
			using(SqlCommand cmd = con.CreateCommand()){
				try{
					cmd.CommandText	= strQuery;
					result	= cmd.ExecuteReader();
				}
				catch( Exception ){
					if( result != null ){
						result.Close();
						result	= null;
					}
				}
			}
			return result;
		}		
	}

	class CMain {
		public static void	Main( String[] args ){
			CDatabaseAccess db = new CDatabaseAccess();

			Console.WriteLine( db.GenConnectionString("SQLServer","55","pp") );
		}
	}
}
