using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.PlatformUI;
using EnvDTE80;
using EnvDTE;

namespace Xcentium.HelixExtension
{
    [Guid(AdvancedProjectDialog.AdvancedProjectDialogGuidString)]
    public partial class AdvancedProjectDialog : DialogWindow
    {
        public const string AdvancedProjectDialogGuidString = "9fd1c94e-b0da-48ad-b41b-768a3c7e5af1";

        public AdvancedProjectDialog() : base()
        {
            this.InitializeComponent();
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            var moduleName = this.tbModuleName.Text;
            if (String.IsNullOrEmpty(moduleName))
            {
                MessageBox.Show("Module name is required", "Helix Advanced Project Add");
                return;
            }
            var solutionName = CommandHelper.GetSolutionName();
            var selectedLayer = CommandHelper.GetSelectedHelixLayer();
            var folderAsProject = selectedLayer.Object as Project;
            var solutionFolder = folderAsProject.Object as SolutionFolder;
            var projectName = String.Concat(solutionName, ".", selectedLayer.Name, ".", moduleName);

            solutionFolder.AddSolutionFolder(moduleName);
            var newFolder = selectedLayer.UIHierarchyItems.Item(moduleName);
            CommandHelper.AddProject(newFolder, selectedLayer.Name, solutionName);

            //test project
            if (cbIncludeTest.IsChecked.HasValue && cbIncludeTest.IsChecked.Value)
                CommandHelper.AddTestProject(newFolder, selectedLayer.Name, solutionName);
            //MVC areas
            if (cbMVC.IsChecked.HasValue && cbMVC.IsChecked.Value && String.IsNullOrEmpty(tbAreaName.Text))  
                CommandHelper.AddMVCFolders(projectName, moduleName);
            else if(cbMVC.IsChecked.HasValue && cbMVC.IsChecked.Value)
                CommandHelper.AddMVCFolders(projectName, moduleName, tbAreaName.Text);
            //serialization
            //if (cbIncludeSerial.IsChecked.HasValue && cbIncludeSerial.IsChecked.Value)
            //{
            //    var dir = CommandHelper.GetSolutionFileDirectory();
            //    string baseFilePath = String.Concat(dir, "\\src\\", selectedLayer.Name, "\\", moduleName, "\\serialization");
            //    SerializationHelper.CreateModuleRenderingFolder(baseFilePath, moduleName, selectedLayer.Name);
            //}

            MessageBox.Show("Project has been added", "Helix Advanced Project Add");
            this.Close();
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void cbMVC_Checked(object sender, RoutedEventArgs e)
        {
            this.rowAreaSupport.Visibility = Visibility.Visible;
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void cbMVC_Unchecked(object sender, RoutedEventArgs e)
        {
            this.rowAreaSupport.Visibility = Visibility.Collapsed;
            tbAreaName.Text = "";
        }
    }

    
}