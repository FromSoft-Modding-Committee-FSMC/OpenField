using MoonSharp.Interpreter;
using OFC.Audio;
using OFC.Resource;
using OFC.Utility;

namespace OFR
{
    class Program
    {
        public static void LuaHello()
        {
            string script =
             @"function test1()
                   passThrough();
                   print('ello from:');
               end

               function test2()
                   print('LUA');
               end

               function passThrough()
                   print('Again!..');
               end
             ";

            Script luaScript = new Script();

            luaScript.Options.DebugPrint = s => { Log.Write("LUA", 0x44CC44, s); };

            luaScript.DoString(script);

            DynValue v = luaScript.Globals.Get("Bob");
            v.IsNilOrNan();

            luaScript.Call(luaScript.Globals["test1"]);
            luaScript.Call(luaScript.Globals["test2"]);
        }

        static void Main()
        {
            //LuaHello();

            //Initialization of OpenFieldCore systems
            ResourceManager.Initialize();
            //AudioManager.Initialize();

            using Game game = new Game();
            game.Run();
        }
    }
}
