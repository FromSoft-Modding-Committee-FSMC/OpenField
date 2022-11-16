using OFC.Utility;
using System;
using System.IO;
using System.Text.Json;


namespace OFE.Data
{
    public struct ProjectFileVersion
    {
        /// <summary>
        /// Simply the project file version
        /// </summary>
        public uint projectVersion { get; set; }

        /// <summary>
        /// Which version of OFE the project file was created with
        /// </summary>
        public string creatorVersion { get; set; }

        /// <summary>
        /// Which version of OFE the project file was last edited with
        /// </summary>
        public string editorVersion { get; set; }
    }

    public struct ProjectFileProperties
    {
        /// <summary>
        /// The name of the project
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// A description of the project
        /// </summary>
        public string description { get; set; }
    }

    public class ProjectFile
    {
        private struct ProjectFileJson
        {
            public ProjectFileVersion version { get; set; }
            public ProjectFileProperties properties { get; set; }
        }

        //Data
        private const int fileVersion = 1;
        private ProjectFileVersion _versionInfo;
        private ProjectFileProperties _propeties;

        //Properties
        public uint ProjectVersion
        {
            get { return _versionInfo.projectVersion; }
        }
        public string CreatorVersion
        {
            get { return _versionInfo.creatorVersion; }
        }
        public string EditorVersion
        {
            get { return _versionInfo.editorVersion; }
        }


        public ProjectFile()
        {

        }

        public static bool LoadFromFile(string filepath, out ProjectFile project)
        {
            try
            {
                string jsonString = File.ReadAllText(filepath);
                ProjectFileJson jsonProject = JsonSerializer.Deserialize<ProjectFileJson>(jsonString);

                project = new ProjectFile();
                project._versionInfo = jsonProject.version;
                project._propeties = jsonProject.properties;

                //* Toggle this debugging stuff on by adding a forward slash to the comment.
                Log.Info("Loaded Project!");
                Log.Info("\tVersion:");
                Log.Info($"\t\tProject: {project._versionInfo.projectVersion}");
                Log.Info($"\t\tCreator: {project._versionInfo.creatorVersion}");
                Log.Info($"\t\tEditor: {project._versionInfo.editorVersion}");

                Log.Info("\tProperties:");
                Log.Info($"\t\tName: {project._propeties.name}");
                Log.Info($"\t\tDescription: {project._propeties.description}");
                //*/
            }
            catch (Exception ex)
            {
                Log.Write("Exception", 0xFF4444, ex.Message);
                Log.Write("Stack Trace", 0xCCCCCC, $"\n{ex.StackTrace}");
                project = null;
                return false;
            }

            return true;
        }
    }
}
