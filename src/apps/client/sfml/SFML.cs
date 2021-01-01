using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

using lib;

using Optional;

namespace cl
{





	class Client
	{

		private static bool s_running = true;

		private static void onAppExit( object sender, EventArgs e )
		{
			s_running = false;
		}



		static void Main( string[] args )
		{

			sv.Main server = new sv.Main( args[0] );

			server.startup();

			while( s_running )
			{
				System.Threading.Thread.Sleep( 1 );
			}

			server.shutdown();




		}

		#region Test


		#endregion //Test

	}
}
