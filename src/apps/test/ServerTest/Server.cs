using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

using lib;

using Optional;

namespace sv
{





	class Server
	{

		private static bool s_running = true;

		private static void onAppExit( object sender, EventArgs e )
		{
			s_running = false;
		}



		static void Main( string[] args )
		{
			//RWTest();
			//ActTest();


			sv.Main server = new sv.Main( args[0] );

			server.startup();

			while( s_running )
			{
				System.Threading.Thread.Sleep( 1 );
			}

			server.shutdown();




		}

		#region Test

		static void RWTest()
		{
			ent.EntityId newId;

			{
				var comHealth = ent.ComHealth.create(m_healthOpt: 100.0f.Some());

				var m_coms = ImmutableArray<ent.ComList>.Empty;

				m_coms = m_coms.Add( new ent.ComList( comHealth ) );

				var nz = new net.NearZero( 0.25f );

				var ent1 = ent.Entity.create( m_comsOpt: m_coms.Some(), m_nzOpt: nz.Some() );

				newId = ent1.id;

				var memStream = new MemoryStream( 1024 );
				var send = new net.DOSend( memStream );

				ent1.DeltaFull( ent.Entity.def, send );

				memStream.Position = 0;

				var recv = new net.DORecv( memStream );

				var ent2 = ent.Entity.create();

				ent2.DeltaFull( ent2, recv );


			}
		}

		static void DBTest()
		{
			var db = new db.DB<ent.EntityId, ent.Entity>();

			ent.EntityId newId;

			{
				var comHealth = ent.ComHealth.create(m_healthOpt: 100.0f.Some());

				var m_coms = ImmutableArray<ent.ComList>.Empty;

				m_coms = m_coms.Add( new ent.ComList( comHealth ) );

				var ent1 = ent.Entity.create( m_comsOpt: m_coms.Some() );

				newId = ent1.id;

				using var tx = db.checkout();

				tx.add( ent1 );

			}

			{
				var (tx, ent) = db.checkout( newId );

				using( tx )
				{
					ent.MatchSome( ent =>
					{
						ent.ToString();
					} );
				}
			}

			{
				var (tx, ent) = db.checkout( newId );

				using( tx )
				{

					ent.MatchSome( ent =>
					{
						ent.ToString();
					} );



				}
			}
		}


		static void ActTest()
		{
			
			int valInt = 10;
			float valFloat = 10.0f;
			string valString0 = "Hello 0";
			string valString1 = "Hello 1";


			//var act2 = db.Act.create( LambdaToCall_0 );

			var act3 = db.Act.create( LambdaToCall_1, 10 );

			var act4 = db.Act.create( LambdaToCall_2, "Howdy" );


			var act0 = new db.Act( () => {
				lib.Log.info( $"The int is {valInt}" );
			} );

			var act1 = new db.Act( () => {
				lib.Log.info( $"The float is {valFloat}" );

			} );

			ActTestPart2( "best" );

		}

		static void ActTestPart2( string paramString )
		{
			string valString0 = "Hello 0";
			string valString1 = "Hello 1";

			var act2 = new db.Act( () => {
				lib.Log.info( $"The string is {valString0}" );
			} );

			var act3 = new db.Act( () => {
				lib.Log.info( $"The string is {valString1}" );
			} );
		}

		static void LambdaToCall_0()
		{
			lib.Log.info( $"LambdaToCall_0" );
		}

		static void LambdaToCall_1( int a )
		{
			lib.Log.info( $"LambdaToCall_1 {a}" );
		}

		static void LambdaToCall_2( string a )
		{
			lib.Log.info( $"LambdaToCall_1 {a}" );
		}

		#endregion //Test

	}
}
