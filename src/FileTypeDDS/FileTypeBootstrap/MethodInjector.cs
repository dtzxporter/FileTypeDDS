using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FileTypeBootstrap
{
    internal class MethodInjector
    {
        /// <summary>
        /// Replace the target function with the specified patched method
        /// </summary>
        /// <param name="Target">The target function to patch</param>
        /// <param name="Patch">The method to patch it with</param>
        public static void InjectMethod(MethodInfo Target, MethodInfo Patch)
        {
            // Prepare methods
            RuntimeHelpers.PrepareMethod(Target.MethodHandle);
            RuntimeHelpers.PrepareMethod(Patch.MethodHandle);

            unsafe
            {
                if (IntPtr.Size == 4)
                {
                    // Get offsets
                    int* inj = (int*)Patch.MethodHandle.Value.ToPointer() + 2;
                    int* tar = (int*)Target.MethodHandle.Value.ToPointer() + 2;
                    // Check for debug
                    if (Debugger.IsAttached)
                    {
                        byte* injInst = (byte*)*inj;
                        byte* tarInst = (byte*)*tar;

                        int* injSrc = (int*)(injInst + 1);
                        int* tarSrc = (int*)(tarInst + 1);

                        *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
                    }
                    else
                    {
                        *tar = *inj;
                    }
                }
                else
                {
                    // Get offsets
                    long* inj = (long*)Patch.MethodHandle.Value.ToPointer() + 1;
                    long* tar = (long*)Target.MethodHandle.Value.ToPointer() + 1;
                    // Check for debug
                    if (Debugger.IsAttached)
                    {
                        byte* injInst = (byte*)*inj;
                        byte* tarInst = (byte*)*tar;


                        int* injSrc = (int*)(injInst + 1);
                        int* tarSrc = (int*)(tarInst + 1);

                        *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
                    }
                    else
                    {
                        *tar = *inj;
                    }
                }
            }
        }

        public static void InjectConstructor(ConstructorInfo Target, ConstructorInfo Patch)
        {
            // Prepare methods
            RuntimeHelpers.PrepareMethod(Target.MethodHandle);
            RuntimeHelpers.PrepareMethod(Patch.MethodHandle);

            unsafe
            {
                if (IntPtr.Size == 4)
                {
                    // Get offsets
                    int* inj = (int*)Patch.MethodHandle.Value.ToPointer() + 2;
                    int* tar = (int*)Target.MethodHandle.Value.ToPointer() + 2;
                    // Check for debug
                    if (Debugger.IsAttached)
                    {
                        byte* injInst = (byte*)*inj;
                        byte* tarInst = (byte*)*tar;

                        int* injSrc = (int*)(injInst + 1);
                        int* tarSrc = (int*)(tarInst + 1);

                        *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
                    }
                    else
                    {
                        *tar = *inj;
                    }
                }
                else
                {
                    // Get offsets
                    long* inj = (long*)Patch.MethodHandle.Value.ToPointer() + 1;
                    long* tar = (long*)Target.MethodHandle.Value.ToPointer() + 1;
                    // Check for debug
                    if (Debugger.IsAttached)
                    {
                        byte* injInst = (byte*)*inj;
                        byte* tarInst = (byte*)*tar;


                        int* injSrc = (int*)(injInst + 1);
                        int* tarSrc = (int*)(tarInst + 1);

                        *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
                    }
                    else
                    {
                        *tar = *inj;
                    }
                }
            }
        }
    }
}
