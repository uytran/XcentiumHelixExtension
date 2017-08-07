using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xcentium.HelixExtension
{
    public static class CommandHelper
    {
        /// <summary>
        /// This function gets the selected folder under one of the 3 layers: Feature, Foundation, Project. If there is nothing selected immediately under, it returns null.
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="solutionName"></param>
        public static UIHierarchyItem GetSelectedHelixModule(out string layerName, out string solutionName)
        {
            var _applicationObject = GetActiveIDE();
            //SelectedItems for some reason doesn't have access to parent
            //This way is hard coded, but very fast, and follows Helix structure exactly
            UIHierarchy uih = _applicationObject.ToolWindows.SolutionExplorer;
            solutionName = uih.UIHierarchyItems.Item(1).Name;
            var featureItem = uih.GetItem(solutionName + "\\Feature");
            var projectItem = uih.GetItem(solutionName + "\\Project");
            var foundationItem = uih.GetItem(solutionName + "\\Foundation");
            UIHierarchyItem selectedItem = null;
            layerName = "";

            if (featureItem != null && projectItem != null && foundationItem != null)
            {
                foreach (UIHierarchyItem i in featureItem.UIHierarchyItems)
                {
                    if (i.IsSelected)
                    {
                        selectedItem = i;
                        layerName = "Feature";
                    }
                }
                foreach (UIHierarchyItem i in projectItem.UIHierarchyItems)
                {
                    if (i.IsSelected)
                    {
                        selectedItem = i;
                        layerName = "Project";
                    }
                }
                foreach (UIHierarchyItem i in foundationItem.UIHierarchyItems)
                {
                    if (i.IsSelected)
                    {
                        selectedItem = i;
                        layerName = "Foundation";
                    }
                }

            }

            return selectedItem;
        }

        /// <summary>
        /// This function gets the selected layer folder. If one of the 3 layer folders is not selected, it returns null.
        /// </summary>
        public static UIHierarchyItem GetSelectedHelixLayer()
        {
            var _applicationObject = GetActiveIDE();
            //SelectedItems for some reason doesn't have access to parent
            //This way is hard coded, but very fast, and follows Helix structure exactly
            UIHierarchy uih = _applicationObject.ToolWindows.SolutionExplorer;
            var solutionName = uih.UIHierarchyItems.Item(1).Name;
            var featureItem = uih.GetItem(solutionName + "\\Feature");
            var projectItem = uih.GetItem(solutionName + "\\Project");
            var foundationItem = uih.GetItem(solutionName + "\\Foundation");
            UIHierarchyItem selectedItem = null;
            if (featureItem.IsSelected)
                selectedItem = featureItem;
            if (projectItem.IsSelected)
                selectedItem = projectItem;
            if (foundationItem.IsSelected)
                selectedItem = foundationItem;

            return selectedItem;
        }

        /// <summary>
        /// Gets the name of the solution currently open in DTE
        /// </summary>
        public static string GetSolutionName()
        {
            var _applicationObject = GetActiveIDE();
            UIHierarchy uih = _applicationObject.ToolWindows.SolutionExplorer;
            return uih.UIHierarchyItems.Item(1).Name;
        }

        /// <summary>
        /// This function creates a project at the level of the given solution folder. Also needs layer and solution name to create the full project name (e.g. xHelix.Feature.Accounts)
        /// </summary>
        /// <param name="selectedFolder"></param>
        /// <param name="layerName"></param>
        /// <param name="solutionName"></param>
        /// <param name="_applicationObject"></param>
        public static void AddProject(UIHierarchyItem selectedFolder, string layerName, string solutionName)
        {
            if (selectedFolder != null)
            {
                var _applicationObject = GetActiveIDE();
                var folderAsProject = selectedFolder.Object as Project;
                var moduleName = folderAsProject.Name;
                var solutionFolder = folderAsProject.Object as SolutionFolder;
                var dir = new FileInfo(_applicationObject.Solution.FullName).Directory;
                string filePath = String.Concat(dir, "\\src\\", layerName, "\\", moduleName, "\\code");

                var currentSolution = (Solution2)_applicationObject.Solution;
                var templatePath = currentSolution.GetProjectTemplate("HelixProject.zip", "CSharp");
                solutionFolder.AddFromTemplate(templatePath, filePath, String.Concat(solutionName, ".", layerName, ".", moduleName));
            }
        }

        /// <summary>
        /// This function creates a test project at the level of the given solution folder. Also needs layer and solution name to create the full project name (e.g. xHelix.Feature.Accounts)
        /// </summary>
        /// <param name="selectedFolder"></param>
        /// <param name="layerName"></param>
        /// <param name="solutionName"></param>
        /// <param name="_applicationObject"></param>
        public static void AddTestProject(UIHierarchyItem selectedFolder, string layerName, string solutionName)
        {
            if (selectedFolder != null)
            {
                var _applicationObject = GetActiveIDE();
                var folderAsProject = selectedFolder.Object as Project;
                var moduleName = folderAsProject.Name;
                var solutionFolder = folderAsProject.Object as SolutionFolder;
                var dir = new FileInfo(_applicationObject.Solution.FullName).Directory;
                string filePath = String.Concat(dir, "\\src\\", layerName, "\\", moduleName, "\\Tests");

                var currentSolution = (Solution2)_applicationObject.Solution;
                var templatePath = currentSolution.GetProjectTemplate("Windows\\1033\\EmptyProject\\csEmptyProject.vstemplate", "CSharp");
                var testProjectName = String.Concat(solutionName, ".", layerName, ".", moduleName, ".Test");
                solutionFolder.AddFromTemplate(templatePath, filePath, testProjectName);
                var newProject = Projects().Where(p=>p.Name == testProjectName).FirstOrDefault();

                newProject.ProjectItems.AddFromTemplate(GetGenericClassTemplate(), moduleName + "Tests.cs");
            }
        }

        /// <summary>
        /// Adds base folder and a starter controller for MVC projects
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="moduleName"></param>
        /// <param name="areaName"></param>
        public static void AddMVCFolders(string projectName, string moduleName, string areaName = "")
        {
            var newProject = Projects().Where(p => p.Name == projectName).FirstOrDefault();
            if (areaName != "")
            {
                var areaFolder = newProject.ProjectItems.AddFolder("Areas");
                var moduleFolder = areaFolder.ProjectItems.AddFolder(areaName);
                var cFolder = moduleFolder.ProjectItems.AddFolder("Controllers");
                cFolder.ProjectItems.AddFromTemplate(GetGenericClassTemplate(), moduleName + "Controller.cs");
                moduleFolder.ProjectItems.AddFolder("Views");
                moduleFolder.ProjectItems.AddFolder("Models");
            }
            else
            {
                var cFolder = newProject.ProjectItems.AddFolder("Controllers");
                cFolder.ProjectItems.AddFromTemplate(GetGenericClassTemplate(), moduleName + "Controller.cs");
                newProject.ProjectItems.AddFolder("Views");
                newProject.ProjectItems.AddFolder("Models");
            }
        }

        public static DTE2 GetActiveIDE()
        {
            // Get an instance of currently running Visual Studio IDE.
            DTE2 dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            return dte2;
        }
        public static string GetGenericClassTemplate()
        {
            var _applicationObject = GetActiveIDE();
            var currentSolution = (Solution2)_applicationObject.Solution;
            return currentSolution.GetProjectItemTemplate("Code\\1033\\Class\\Class.vstemplate", "CSharp");
        }
        public static DirectoryInfo GetSolutionFileDirectory()
        {
            var _applicationObject = GetActiveIDE();
            return new FileInfo(_applicationObject.Solution.FullName).Directory;
        }

        /// <summary>
        /// Gets a recursive list of all projects in the solution
        /// </summary>
        public static IList<Project> Projects()
        {
            Projects projects = GetActiveIDE().Solution.Projects;
            List<Project> list = new List<Project>();
            var item = projects.GetEnumerator();
            while (item.MoveNext())
            {
                var project = item.Current as Project;
                if (project == null)
                {
                    continue;
                }

                if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    list.AddRange(GetSolutionFolderProjects(project));
                }
                else
                {
                    list.Add(project);
                }
            }

            return list;
        }

        private static IEnumerable<Project> GetSolutionFolderProjects(Project solutionFolder)
        {
            List<Project> list = new List<Project>();
            for (var i = 1; i <= solutionFolder.ProjectItems.Count; i++)
            {
                var subProject = solutionFolder.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                {
                    continue;
                }

                // If this is another solution folder, do a recursive call, otherwise add
                if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    list.AddRange(GetSolutionFolderProjects(subProject));
                }
                else
                {
                    list.Add(subProject);
                }
            }
            return list;
        }
    }
}
