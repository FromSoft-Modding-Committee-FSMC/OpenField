using System;

using MoonSharp.Interpreter;

using OFC.Utility;
using OFC.IO.Hashing;

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

            luaScript.Call(luaScript.Globals["test1"]);
            luaScript.Call(luaScript.Globals["test2"]);
        }

        static void Main()
        {
            //LuaHello();

            using Game game = new Game();
            game.Run();
        }
    }
}
