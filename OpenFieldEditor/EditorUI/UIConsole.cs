using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using ImGuiNET;
using OFC.Utility;

namespace OFE.EditorUI
{
    public class UIConsole : IUserInterface
    {
        private int maxLogLength = 100;
        private List<string> log;
        private string logBuffer = "";

        public UIConsole(int maxLoggedLines) 
        {
            maxLogLength = maxLoggedLines;

            log = new List<string>();

            //Redirect Log Output
            Log.SetOutputDelegate(s => { 
                if (log.Count == maxLogLength) 
                { 
                    log.RemoveAt(0); 
                }
                log.Add(s);

                Console.WriteLine(s);

                logBuffer = string.Join("\n", log.ToArray());
            });
        }

        public void Draw()
        {
            ImGui.Begin("Output");
            ImGui.BeginChild("Output_Scrollarea", default, false, ImGuiWindowFlags.AlwaysVerticalScrollbar);
            ImGui.TextWrapped(logBuffer);
            ImGui.EndChild();
            ImGui.End();
        }
    }
}
