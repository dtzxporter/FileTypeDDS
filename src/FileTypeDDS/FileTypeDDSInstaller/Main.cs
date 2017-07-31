using dnlib.DotNet.Emit;
using dnpatch;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileTypeDDSInstaller
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void BackOut_Click(object sender, EventArgs e)
        {
            // Just close
            this.Close();
        }

        private string GetNGENPath()
        {
            // Calculate the path
            string fxDir = (UIntPtr.Size == 8) ? "Framework64" : "Framework";
            string fxPathBase = "%WINDIR%\\Microsoft.NET\\" + fxDir + "\\v";
            string fxPath = fxPathBase + Environment.Version.ToString(3) + "\\";
            string fxPathExp = Environment.ExpandEnvironmentVariables(fxPath);
            return Path.Combine(fxPathExp, "ngen.exe");
        }

        private void RunProcess(string ProcessPath, string Args)
        {
            // Run a process and wait
            ProcessStartInfo psi = new ProcessStartInfo(ProcessPath, Args);
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process process = Process.Start(psi);
            process.WaitForExit();
        }

        private void RunPatcher()
        {
            // Locate the Paint.NET exe
            var ProgramFilesBase = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var ProgramFilesNew = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            // Final path
            var ProgramPath = "";

            // Check each path
            if (File.Exists(Path.Combine(ProgramFilesBase, "paint.net\\PaintDotNet.exe")))
            {
                ProgramPath = Path.Combine(ProgramFilesBase, "paint.net\\PaintDotNet.exe");
            }
            else if (File.Exists(Path.Combine(ProgramFilesNew, "paint.net\\PaintDotNet.exe")))
            {
                ProgramPath = Path.Combine(ProgramFilesNew, "paint.net\\PaintDotNet.exe");
            }
            else
            {
                // Ask the user to locate it
                this.Invoke((Action)delegate
                {
                    // Ask
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && openFileDialog.FileName.Contains("PaintDotNet.exe"))
                    {
                        // Set
                        ProgramPath = openFileDialog.FileName;
                    }
                    else
                    {
                        // Alert and close
                        MessageBox.Show("Failed to locate Paint.NET. Please verify that it is installed properly.", "FileTypeDDS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Close
                        this.Close();
                    }
                });
            }

            // Load the patcher
            var Patch = new Patcher(ProgramPath);
            // Patcher check
            Target PatcherCheckTarget = new Target()
            {
                Namespace = "PaintDotNet.Data",
                Class = "PdnFileTypes",
                Method = ".cctor"
            };
            // Find it, if it's found, we can patch
            if (Patch.FindInstruction(PatcherCheckTarget, Instruction.Create(OpCodes.Ldc_I4_8)) > 0)
            {
                // Allocate a default DDS patcher
                Target DDSDefaultTarget = new Target()
                {
                    Namespace = "PaintDotNet.Data",
                    Class = "PdnFileTypes",
                    Method = ".cctor",
                    Indices = new[] { 59, 60, 61, 62 } // Indicies for appending the built-in DDS filetype to the array
                };
                // Remove the DDS built-in type
                Patch.RemoveInstruction(DDSDefaultTarget);

                // The new size op
                Instruction opCode = Instruction.Create(OpCodes.Ldc_I4_7);
                // Allocate a array size patcher
                Target ArrayPatchTarget = new Target()
                {
                    Namespace = "PaintDotNet.Data",
                    Class = "PdnFileTypes",
                    Method = ".cctor",
                    Instruction = opCode,
                    Index = 29                      // Index of the built-in filetypes array size
                };
                // Replace the array size
                Patch.ReplaceInstruction(ArrayPatchTarget);
                // Save it, but backup the original
                Patch.Save(true);
            }            

            // Perform NGen installation
            var NGenPath = GetNGENPath();

            // Uninstall first, then install
            RunProcess(NGenPath, "uninstall \"" + ProgramPath + "\"");
            RunProcess(NGenPath, "install \"" + ProgramPath + "\"");

            // Now that Paint.NET is ready, copy over the plugin for the arch we're on
            try
            {
                if (UIntPtr.Size == 8)
                {
                    // Copy x64
                    File.Copy("FileTypeDDS64.dll", Path.Combine(Path.GetDirectoryName(ProgramPath), "FileTypes\\FileTypeDDS64.dll"), true);
                }
                else
                {
                    // Copy x32
                    File.Copy("FileTypeDDS32.dll", Path.Combine(Path.GetDirectoryName(ProgramPath), "FileTypes\\FileTypeDDS32.dll"), true);
                }
            }
            catch
            {
                // Nothing, alert user if anything. Should. Not. Happen.
            }

            // Done, invoke and set dialog
            this.Invoke((Action)delegate
            {
                // Alert
                MessageBox.Show("FileTypeDDS has been installed. You may now relaunch Paint.NET.", "FileTypeDDS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Close
                this.Close();
            });
        }

        private void InstallGo_Click(object sender, EventArgs e)
        {
            // Disable us
            this.InstallGo.Enabled = false;
            this.BackOut.Enabled = false;
            // Make visible
            this.ProgressLoad.Visible = true;
            // Run the patcher
            Task.Run((Action)RunPatcher);
        }
    }
}
