using System.Collections.Generic;
using System.IO;
using System;

using ImGuiNET;

using OFC.Utility;
using System.Drawing;
using System.Xml.Linq;
using System.Linq;

namespace OFE.EditorUI.Dialogue
{
    public class UIOpenFileDialog
    {
        private struct FileReference
        {
            public string name;
            public string path;
            public uint type;
            public string creationTime;
            public string size;
        }

        private System.Numerics.Vector2 dialogueMainSize = new(640, 480);
        private System.Numerics.Vector2 dialogueListSize = new(640, 450);

        private bool dialogueIsOpen = false;
        private bool dialogueShouldOpen = false;

        //FS Info
        private string formatFilter = "*.dll;*.ini;*.pdb";
        private List<FileReference> pathList = new List<FileReference>();
        private List<string> pathStack = new List<string>();
        private string currentPath = AppDomain.CurrentDomain.BaseDirectory;

        //Path Input Date
        private string pathInputStr = AppDomain.CurrentDomain.BaseDirectory;
        private const int maxPathInputLength = 256;

        private void EnumuratePath(string path)
        {
            pathList.Clear();

            DateTime creationTime;
            FileInfo fileInfo;

            //Enumrate Directories
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                creationTime = Directory.GetCreationTime(directory);

                pathList.Add(new FileReference
                {
                    name = Path.GetFileNameWithoutExtension(directory),
                    path = Path.Combine(path, directory),
                    type = 0,
                    creationTime = creationTime.ToString("yyyy/MM/dd h:mm tt"),
                    size = ""
                });
            }

            //Enumurate Files
            string[] files = formatFilter.Split(';').SelectMany(filter => Directory.GetFiles(path, filter)).ToArray();
            Array.Sort(files);

            foreach (string file in files)
            {
                fileInfo = new FileInfo(file);

                FileReference fr = new FileReference
                {
                    name = Path.GetFileName(file),
                    path = Path.Combine(path, file),
                    type = 1,
                    creationTime = fileInfo.CreationTime.ToString("yyyy/MM/dd h:mm tt")
                };

                //A shit bit of code to convert byte length to different units if required
                int numberOfDivisions = 0;
                float convertedSize = fileInfo.Length;

                while(convertedSize > 1024)
                {
                    convertedSize /= 1024f;
                    numberOfDivisions++;
                }

                convertedSize = MathF.Ceiling(convertedSize);

                switch (numberOfDivisions)
                {
                    default:
                        fr.size = $"{convertedSize} B";
                        break;

                    case 1:
                        fr.size = $"{convertedSize} kB";
                        break;

                    case 2:
                        fr.size = $"{convertedSize} MB";
                        break;

                    case 3:
                        fr.size = $"{convertedSize} GB";
                        break;
                }

                pathList.Add(fr);
            }
        }

        public bool ShowDialog()
        {
            dialogueShouldOpen = true;

            EnumuratePath(currentPath);

            return true;
        }

        public bool Draw()
        {
            if(dialogueShouldOpen)
            {
                ImGui.OpenPopup("Open File...");
                dialogueIsOpen = true;
                dialogueShouldOpen = false;
            }

            ImGui.SetNextWindowPos(ImGui.GetMainViewport().GetCenter(), ImGuiCond.None, new System.Numerics.Vector2(0.5f, 0.5f));
            if (ImGui.BeginPopupModal("Open File...", ref dialogueIsOpen, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove))
            {  
                //Draw Popup
                ImGui.BeginChild(1, dialogueMainSize);

                //Back Button
                if (ImGui.ArrowButton("##arrowbutton1", ImGuiDir.Left))
                {
                    if (pathStack.Count > 0)
                    {
                        currentPath = pathStack[0];
                        EnumuratePath(currentPath);
                    }

                    Log.Warn("Not Implemented (Properly).");
                }

                ImGui.SameLine();
                if (ImGui.ArrowButton("##arrowbutton2", ImGuiDir.Right))
                {
                    if (pathStack.Count > 0)
                    {
                        currentPath = pathStack[0];
                        EnumuratePath(currentPath);
                    }

                    Log.Warn("Not Implemented (Properly).");
                }

                //Path Input
                ImGui.SameLine();
                ImGui.PushItemWidth(dialogueMainSize.X - 84);
                ImGui.InputText("", ref pathInputStr, maxPathInputLength);
                ImGui.PopItemWidth();

                //Recent Path Stack
                ImGui.SameLine();
                if (ImGui.BeginCombo("##combo", pathInputStr, ImGuiComboFlags.NoPreview))
                {
                    for (int i = 0; i < pathStack.Count; ++i)
                    {
                        bool selected = (pathInputStr == pathStack[i]);
                        
                        if (ImGui.Selectable(pathStack[i], selected))
                        {
                            currentPath = pathStack[i];

                            EnumuratePath(currentPath);

                            pathInputStr = pathStack[i];
                        }

                        if (selected)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }
                    ImGui.EndCombo();
                }

                //File List
                ImGui.BeginChildFrame(2, dialogueListSize);

                if (ImGui.BeginTable("ofd_filelist", 3, ImGuiTableFlags.None))
                {
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.None, 64);
                    ImGui.TableSetupColumn("Size", ImGuiTableColumnFlags.None, 32);
                    ImGui.TableSetupColumn("Date (yyyy/mm/dd)", ImGuiTableColumnFlags.None, 32);
                    ImGui.TableSetupScrollFreeze(0, 1);
                    ImGui.TableHeadersRow();

                    //Enumurate
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF000000);
                    for(int i = 0; i < pathList.Count; ++i)
                    {
                        FileReference fr = pathList[i];

                        ImGui.TableNextColumn();
                        if(ImGui.Selectable(fr.name, false, ImGuiSelectableFlags.DontClosePopups | ImGuiSelectableFlags.SpanAllColumns))
                        {
                            if(fr.type == 0) // directory
                            {
                                //When the path count is greater than 16, remove the last path.
                                if(pathStack.Count == 16)
                                {
                                    pathStack.RemoveAt(pathStack.Count-1);
                                }

                                //If the path already exists, remove it
                                if(pathStack.Contains(currentPath))
                                {
                                    pathStack.Remove(currentPath);
                                }

                                //Push current path into the list.
                                pathStack.Insert(0, currentPath);

                                currentPath = fr.path;

                                EnumuratePath(currentPath);
                            }else
                            if(fr.type == 1) // file
                            {

                            }

                            Log.Info(fr.path);
                            break;
                        }
                        ImGui.TableNextColumn();
                        ImGui.Text(fr.size);
                        ImGui.TableNextColumn();
                        ImGui.Text(fr.creationTime);

                        ImGui.TableNextRow();
                    }
                    ImGui.PopStyleColor();

                    ImGui.EndTable();
                }

                //Finish File List
                ImGui.EndChildFrame();

                //Finish Draw Popup
                ImGui.EndChild();


                ImGui.EndPopup();
            }

            return false;
        }
    }
}
