using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ComPanel.Modules
{
    class ComPanelModulesLoader
    {
        public Dictionary<string, Module> LoadModulesFromCatalog(string catalog)
        {
            var modules = new Dictionary<string,Module>();

            var catalogPath = Path.Combine(Environment.CurrentDirectory, catalog);
            var filePaths = Directory.GetFiles(catalogPath, "*.dll");

            foreach (var filePath in filePaths)
            {
                var moduleLoadContext = new ModuleLoadContext(filePath);
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var assembly = moduleLoadContext.LoadFromAssemblyName(new AssemblyName(fileName));

                foreach (Type type in assembly.GetTypes())
                {
                    try
                    {
                        if (typeof(Module).IsAssignableFrom(type))
                        {
                            modules[fileName] = Activator.CreateInstance(type) as Module;
                        }
                    }
                    catch (Exception) { }
                }
            }
            
            return modules;
        }
    }
}
