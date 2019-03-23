using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileTypeDDSEffect
{
    [EffectCategory(EffectCategory.DoNotDisplay), PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "FileTypeDDS")]
    public class PatcherEffect : PropertyBasedEffect
    {
        public PatcherEffect()
            : base("FileTypeDDSPatcher", null, "", EffectFlags.None)
        {
            try
            {
                // Prepare to patch the built-in file type array
                var PaintAssembly = Assembly.GetEntryAssembly();

                // Find the internal file type array
                var Type = PaintAssembly.GetType("PaintDotNet.Data.PdnFileTypes");
                var Field = Type.GetField("fileTypes", BindingFlags.NonPublic | BindingFlags.Static);

                // Get it's value, then, replace with one that doesn't have the DDS
                var Result = (FileType[])Field.GetValue(null);
                var NewResult = Result.Where(x => !x.Name.ToLower().Contains("dds")).ToArray<FileType>();

                // Patch it, we want full access...
                Field.SetValue(null, NewResult);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("FileTypeDDS - Failed to patch the plugin (Report this on Github) [" + ex.Message + "]", "FileTypeDDS");
            }
        }

        protected override void OnRender(Rectangle[] rois, int startIndex, int length)
        {
            // Nothing...
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            return new PropertyCollection(new List<Property>());
        }
    }
}
