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
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
namespace DevExpress.Utils.Gac {
	[SecuritySafeCritical]
	public static class GacHelper {
		public static IEnumerable<string> GetAssembliesList(string name) {
			IAssemblyName fusionName = null;
			IAssemblyEnum m_AssemblyEnum = null;
			List<string> list = new List<string>();
			int hr = CreateAssemblyNameObject(out fusionName, name, CreateAssemblyNameObjectFlags.CANOF_PARSE_DISPLAY_NAME, IntPtr.Zero);
			if (hr >= 0)
				hr = CreateAssemblyEnum(out m_AssemblyEnum, IntPtr.Zero, fusionName, AssemblyCacheFlags.GAC, IntPtr.Zero);
			if (hr >= 0) {
				while (true) {
					hr = m_AssemblyEnum.GetNextAssembly((IntPtr)0, out fusionName, 0);
					if (hr >= 0 && fusionName != null)
						list.Add(GetFullName(fusionName));
					else
						break;
				}
			}
			return list;
		}
		public static string GetAssemblyPath(string assemblyName) {
			AssemblyInfo aInfo = new AssemblyInfo();
			aInfo.cchBuf = 1024;
			aInfo.currentAssemblyPath = new String('\0', aInfo.cchBuf);
			IAssemblyCache ac = null;
			int hr = CreateAssemblyCache(out ac, 0);
			if (assemblyName.Contains("PublicKey"))
				assemblyName = assemblyName.Remove(assemblyName.IndexOf("PublicKey") - 2);
			if (hr >= 0)
				hr = ac.QueryAssemblyInfo(0, assemblyName, ref aInfo);
			return hr < 0 ? string.Empty : aInfo.currentAssemblyPath;
		}
		static string GetFullName(IAssemblyName assemblyName) {
			int pccDispName = 1024;
			StringBuilder sDisplayName = new StringBuilder(pccDispName);
			int hr = assemblyName.GetDisplayName(sDisplayName, ref pccDispName, (int)AssemblyNameDisplayFlags.ALL);
			return hr < 0 ? string.Empty : sDisplayName.ToString();
		}
		[DllImport("fusion.dll")]
		static extern int CreateAssemblyEnum(out IAssemblyEnum ppEnum, IntPtr pUnkReserved, IAssemblyName pName, AssemblyCacheFlags flags, IntPtr pvReserved);
		[DllImport("fusion.dll")]
		static extern int CreateAssemblyNameObject(out IAssemblyName ppAssemblyNameObj, [MarshalAs(UnmanagedType.LPWStr)] String szAssemblyName, CreateAssemblyNameObjectFlags flags, IntPtr pvReserved);
		[DllImport("fusion.dll")]
		static extern int CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);
	}
	enum AssemblyCacheUninstallDisposition {
		Unknown = 0,
		Uninstalled = 1,
		StillInUse = 2,
		AlreadyUninstalled = 3,
		DeletePending = 4,
		HasInstallReference = 5,
		ReferenceNotFound = 6
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
	interface IAssemblyCache {
		[PreserveSig()]
		int UninstallAssembly(int flags, [MarshalAs(UnmanagedType.LPWStr)] String assemblyName, InstallReference refData, out AssemblyCacheUninstallDisposition disposition);
		[PreserveSig()]
		int QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] String assemblyName, ref AssemblyInfo assemblyInfo);
		[PreserveSig()]
		int Reserved(int flags, IntPtr pvReserved, out Object ppAsmItem, [MarshalAs(UnmanagedType.LPWStr)] String assemblyName);
		[PreserveSig()]
		int Reserved(out Object ppAsmScavenger);
		[PreserveSig()]
		int InstallAssembly(int flags, [MarshalAs(UnmanagedType.LPWStr)] String assemblyFilePath, InstallReference refData);
	}
	[StructLayout(LayoutKind.Sequential)]
	class InstallReference {
		int cbSize;
		int flags;
		Guid guidScheme;
		[MarshalAs(UnmanagedType.LPWStr)]
		String identifier;
		[MarshalAs(UnmanagedType.LPWStr)]
		String description;
		public InstallReference(Guid guid, String id, String data) {
			cbSize = (int)(2 * IntPtr.Size + 16 + (id.Length + data.Length) * 2);
			flags = 0;
			if (flags == 0) { } 
			guidScheme = guid;
			identifier = id;
			description = data;
		}
		public Guid GuidScheme { get { return guidScheme; } }
		public String Identifier { get { return identifier; } }
		public String Description { get { return description; } }
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("CD193BC0-B4BC-11d2-9833-00C04FC31D2E")]
	interface IAssemblyName {
		[PreserveSig()]
		int SetProperty(int PropertyId, IntPtr pvProperty, int cbProperty);
		[PreserveSig()]
		int GetProperty(int PropertyId, IntPtr pvProperty, ref int pcbProperty);
		[PreserveSig()]
		int Finalize();
		[PreserveSig()]
		int GetDisplayName(StringBuilder pDisplayName, ref int pccDisplayName, int displayFlags);
		[PreserveSig()]
		int Reserved(ref Guid guid, Object obj1, Object obj2, String string1, Int64 llFlags, IntPtr pvReserved, int cbReserved, out IntPtr ppv);
		[PreserveSig()]
		int GetName(ref int pccBuffer, StringBuilder pwzName);
		[PreserveSig()]
		int GetVersion(out int versionHi, out int versionLow);
		[PreserveSig()]
		int IsEqual(IAssemblyName pAsmName, int cmpFlags);
		[PreserveSig()]
		int Clone(out IAssemblyName pAsmName);
	}
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("21b8916c-f28e-11d2-a473-00c04f8ef448")]
	interface IAssemblyEnum {
		[PreserveSig()]
		int GetNextAssembly(IntPtr pvReserved, out IAssemblyName ppName, int flags);
		[PreserveSig()]
		int Reset();
		[PreserveSig()]
		int Clone(out IAssemblyEnum ppEnum);
	}
	[Flags]
	enum AssemblyCacheFlags {
		GAC = 2,
	}
	enum CreateAssemblyNameObjectFlags {
		CANOF_DEFAULT = 0,
		CANOF_PARSE_DISPLAY_NAME = 1,
	}
	[Flags]
	enum AssemblyNameDisplayFlags {
		VERSION = 0x01,
		CULTURE = 0x02,
		PUBLIC_KEY_TOKEN = 0x04,
		PROCESSORARCHITECTURE = 0x20,
		RETARGETABLE = 0x80,
		ALL = VERSION | CULTURE | PUBLIC_KEY_TOKEN | PROCESSORARCHITECTURE | RETARGETABLE
	}
	[StructLayout(LayoutKind.Sequential)]
	struct AssemblyInfo {
		public int cbAssemblyInfo;
		public int assemblyFlags;
		public long assemblySizeInKB;
		[MarshalAs(UnmanagedType.LPWStr)]
		public String currentAssemblyPath;
		public int cchBuf;
	}
}
