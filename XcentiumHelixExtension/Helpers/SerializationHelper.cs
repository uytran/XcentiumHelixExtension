using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xcentium.HelixExtension
{
    public static class SerializationHelper
    {

        /// <summary>
        /// Creates a Unicorn .yml file with the minimum required lines
        /// </summary>
        /// <param name="filePath">Full system file path with extension (e.g. C:\serial\Feature.yml)</param>
        /// <param name="parent">Sitecore parent item Guid</param>
        /// <param name="template">Sitecore template guid</param>
        /// <param name="sitecorePath">(e.g. /sitecore/layout/Renderings/Feature/Disclaimers)</param>
        public static void CreateSerializationFile(string filePath, Guid parent, Guid template, string sitecorePath)
        {
            File.Create(filePath).Dispose();
            using (TextWriter tw = new StreamWriter(filePath))
            {
                tw.WriteLine("---");
                tw.WriteLine($@"ID: ""{Guid.NewGuid().ToString()}""");
                tw.WriteLine($@"Parent: ""{parent.ToString()}""");
                tw.WriteLine($@"Template: ""{template.ToString()}""");
                tw.WriteLine($@"Path: ""{sitecorePath}""");
                tw.WriteLine("DB: master");
                tw.WriteLine("Languages:");
                tw.WriteLine("- Language: en");
                tw.WriteLine("  Versions:");
                tw.WriteLine("  - Version: 1");
                tw.WriteLine("    Fields:");
                tw.WriteLine(@"    - ID: ""25bed78c-4957-4165-998a-ca1b52f67497""");
                tw.WriteLine("      Hint: __Created");
                tw.WriteLine($"      Value: {DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ")}");
                tw.Close();
            }

        }

        /// <summary>
        /// Creates yml file for sitecore/layout/renderings/layername/modulename
        /// </summary>
        /// <param name="filePath">base file path for serialization</param>
        /// <param name="moduleName"></param>
        /// <param name="layerName"></param>
        /// <param name="subFolder">optional for multi-site group setups</param>
        public static void CreateModuleRenderingFolder(string filePath, string moduleName, string layerName, string subFolder = "")
        {
            if (subFolder != "")
                subFolder += "/";
            var sitecoreFolder = $"/sitecore/layout/Renderings/{layerName}/{subFolder}{moduleName}";
            var thisFilePath = string.Concat(filePath, $@"\\{layerName}.{moduleName}.Renderings\\{moduleName}.yml");
            CreateSerializationFile(filePath, Guid.Empty, Templates.RenderingFolder.ID, sitecoreFolder);
        }

        //habitat default parent folders
        public struct Templates
        {
            public struct TemplateFolder
            {
                public static readonly Guid ID = new Guid("{0437FEE2-44C9-46A6-ABE9-28858D9FEE8C}");
            }
            public struct RenderingFolder
            {
                public static readonly Guid ID = new Guid("{7EE0975B-0698-493E-B3A2-0B2EF33D0522}");
            }
            public struct LayoutFolder
            {
                public static readonly Guid ID = new Guid("{93227C5D-4FEF-474D-94C0-F252EC8E8219}");
            }
            public struct CommonFolder
            {
                public static readonly Guid ID = new Guid("{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}");
            }
        }
    }
}
