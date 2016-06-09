#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Reflection;
using System.Security.Policy;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm {
	[ComImport, Guid("E707DCDE-D1CD-11D2-BAB9-00C04F8ECEAE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAssemblyCache {
		int UninstallAssembly();
		[PreserveSig]
		int QueryAssemblyInfo(uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] string pszAssemblyName, ref ASSEMBLY_INFO pAsmInfo);
		int CreateAssemblyCacheItem();
		int CreateAssemblyScavenger();
		int InstallAssembly();
	}   
	[StructLayout(LayoutKind.Sequential)]
	internal struct ASSEMBLY_INFO {
		public uint cbAssemblyInfo;
		public uint dwAssemblyFlags;
		public ulong uliAssemblySizeInKB;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string pszCurrentAssemblyPathBuf;
		public uint cchBuf;
	}
	class TextTemplatingAssemblyResolver {
		public static string ResolveAssemblyReference(string assemblyReference) {
			if(string.IsNullOrEmpty(assemblyReference) ||  File.Exists(assemblyReference))
				return assemblyReference;
			if(!string.IsNullOrWhiteSpace(assemblyReference)) {				
				if(Path.IsPathRooted(assemblyReference)) {
					Zone zone = Zone.CreateFromUrl(new Uri(assemblyReference).AbsoluteUri);
					if((zone.SecurityZone == SecurityZone.Trusted) || (zone.SecurityZone == SecurityZone.MyComputer))
						return assemblyReference;
					return string.Empty;
				}
				string location = GetLocation(assemblyReference);
				if(!string.IsNullOrEmpty(location)) {
					return location;
				}
				IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
				if(assemblies != null)
					foreach(Assembly assembly in assemblies)
						if(string.Compare(assembly.GetName().Name, assemblyReference, true) == 0)
							return assembly.Location;
			}
			return assemblyReference;
		}
		[DllImport("fusion.dll", CharSet = CharSet.Auto)]
		internal static extern int CreateAssemblyCache(out IAssemblyCache ppAsmCache, uint dwReserved);
		internal static string GetLocation(string strongName) {
			IAssemblyCache cache;
			if(string.IsNullOrEmpty(strongName)) {
				throw new ArgumentNullException("strongName");
			}
			AssemblyName name = new AssemblyName(strongName);
			string str = null;
			if(CreateAssemblyCache(out cache, 0) == 0 && (cache != null)) {
				try {
					if(name.ProcessorArchitecture == ProcessorArchitecture.None) {
						string environmentVariable = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
						if(!string.IsNullOrEmpty(environmentVariable)) {
							str = GetLocationImpl(cache, strongName, environmentVariable);
							if((str != null) && (str.Length > 0))
								return str;
						}
						str = GetLocationImpl(cache, strongName, "MSIL");
						if((str != null) && (str.Length > 0))
							return str;
						return GetLocationImpl(cache, strongName, null);
					}
					str = GetLocationImpl(cache, strongName, null);
				}
				finally {
					Marshal.FinalReleaseComObject(cache);
				}
			}
			return str;
		}
		private static string GetLocationImpl(IAssemblyCache assemblyCache, string strongName, string targetProcessorArchitecture) {
			ASSEMBLY_INFO pAsmInfo = new ASSEMBLY_INFO {
				cbAssemblyInfo = (uint)Marshal.SizeOf(typeof(ASSEMBLY_INFO))
			};
			if(targetProcessorArchitecture != null) {
				strongName = strongName + ", ProcessorArchitecture=" + targetProcessorArchitecture;
			}
			int hr = assemblyCache.QueryAssemblyInfo(3, strongName, ref pAsmInfo);
			if((hr != 0 && (hr != -2147024774)) || (pAsmInfo.cbAssemblyInfo == 0))
				return string.Empty;
			pAsmInfo.pszCurrentAssemblyPathBuf = new string(new char[pAsmInfo.cchBuf]);
			if(assemblyCache.QueryAssemblyInfo(3, strongName, ref pAsmInfo) != 0)
				return string.Empty;
			return pAsmInfo.pszCurrentAssemblyPathBuf;
		}
	}
}
