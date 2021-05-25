using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Lab5
{
    public static class Model
    {
        private const string Root = @"C:\storage";
        public static string GetFullPath(string userPath)
        {
            if (userPath == null)
            {
                return Root;
            }
            else
            {
                return Path.Combine(Root, userPath);
            }
        }
        public static List<string> FindAll(string path)
        {
            var ans = new List<string>();
            var dirs = Directory.GetDirectories(path);
            foreach (var directory in dirs)
            {
                DirectoryInfo info = new DirectoryInfo(directory);
                ans.Add(String.Concat("Directory: ", info.Name));
            }
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                FileInfo info = new FileInfo(file);
                ans.Add(String.Concat("File: ", info.Name));
            }
            return ans;
        }
        public static Dictionary<string, string> GetFileInfo(string path)
        {
            Dictionary<string, string> ans = new Dictionary<string, string>();
            FileInfo info = new FileInfo(path);
            ans.Add("Directory", info.DirectoryName);
            ans.Add("Length", info.Length.ToString());
            ans.Add("Extension", info.Extension);
            ans.Add("Name", info.Name);
            ans.Add("Full name", info.FullName);
            ans.Add("Last change", info.LastWriteTimeUtc.ToString());
            ans.Add("Was created", info.CreationTimeUtc.ToString());
            return ans;
        }
        public static Dictionary<string, string> GetDirInfo(string path)
        {
            Dictionary<string, string> ans = new Dictionary<string, string>();
            DirectoryInfo info = new DirectoryInfo(path);
            ans.Add("Name", info.Name);
            ans.Add("Full name", info.FullName);
            ans.Add("Last change", info.LastWriteTimeUtc.ToString());
            ans.Add("Was created", info.CreationTimeUtc.ToString());
            return ans;
        }
    }
}