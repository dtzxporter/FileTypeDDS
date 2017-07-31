using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FileTypeBootstrap
{
    public class InjectMethod
    {
        public static bool IsInterfaceImplemented(Type derivedType, Type interfaceType)
        {
            return false;
        }

        public static Document LoadWrap(Stream input)
        {
            System.Windows.Forms.MessageBox.Show("LOL");
            return null;
        }

        public static FileType[] InternalFileTypes()
        {
            System.Windows.Forms.MessageBox.Show("LOL");
            return null;
        }

        public void FileTypeCTOR()
        {

        }
    }

    public class TestMeDo : PropertyBasedFileType
    {
        public TestMeDo(): base("lol", FileTypeFlags.None, new string[]{".lol"})
        {

        }

        public override PropertyCollection OnCreateSavePropertyCollection()
        {
            throw new NotImplementedException();
        }

        protected override void OnSaveT(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback)
        {
            throw new NotImplementedException();
        }

        protected override Document OnLoad(Stream input)
        {
            throw new NotImplementedException();
        }
    }

    public class FileTypeBootstrapper : PropertyBasedEffect
    {
        public FileTypeBootstrapper()
            : base(FileTypeBootstrapper.StaticName, FileTypeBootstrapper.StaticIcon, "FileTypeBootstrap", EffectFlags.ForceAliasedSelectionQuality)
        {
            if (Global.WasPatched == false)
            {
                // Prepare to inject wrapper methods
                var BaseAssembly = Assembly.GetEntryAssembly();
                var BaseInjectType = BaseAssembly.GetType("PaintDotNet.Data.Dds.DdsFileType");

                string test = "";

                foreach (MethodInfo m in BaseInjectType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    test += " " + m.Name;
                }

                System.Windows.Forms.MessageBox.Show("win" + test);

                // Get the wrapper method
                var WrapperMethod = BaseInjectType.GetMethod("OnLoad", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                // We now got the method to wrap
                if (WrapperMethod != null)
                {
                    System.Windows.Forms.MessageBox.Show("Got target");
                    // Get it
                    var PatchMethod = typeof(TestMeDo).GetMethod("OnLoad", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    // Check
                    if (PatchMethod != null)
                    {
                        System.Windows.Forms.MessageBox.Show("Got patch, Patching...");
                        // Inject them
                        MethodInjector.InjectMethod(WrapperMethod, PatchMethod);
                    }
                }

                

                Global.WasPatched = true;
            }
        }

        // A blank name
        public static string StaticName { get { return string.Empty; } }
        // A blank icon
        public static Image StaticIcon { get { return null; } }

        // Implementation of an effect, not used
        protected override void OnRender(Rectangle[] rois, int startIndex, int length)
        {
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            // Default result
            return new PropertyCollection(new List<Property>());
        }
    }
}
