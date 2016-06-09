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
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.Office.PInvoke {
	#region ISecurityInformation
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("965FC360-16FF-11d0-91CB-00AA00BBB723")]
	public interface ISecurityInformation {
		void GetObjectInformation(ref Win32.SI_OBJECT_INFO object_info);
		void GetSecurity(int requestInformation, IntPtr securityDescriptor, bool fDefault);
		void SetSecurity(int requestInformation, IntPtr securityDescriptor);
		void GetAccessRight(IntPtr guidObject, int dwFlags, [MarshalAs(UnmanagedType.LPArray)] out Win32.SI_ACCESS[] access, ref int accessCount, ref int defaultAccess);
		void MapGeneric(IntPtr guidObjectType, IntPtr aceFlags, IntPtr mask);
		void GetInheritTypes(ref Win32.SI_INHERIT_TYPE inheritType, IntPtr inheritTypesCount);
		void PropertySheetPageCallback(IntPtr hwnd, int uMsg, int uPage);
	}
	#endregion
	#region SimpleSecurityInformation
	public class SimpleSecurityInformation : ISecurityInformation {
		Win32.SI_OBJECT_INFO objectInfo = new Win32.SI_OBJECT_INFO();
		Win32.SI_ACCESS[] accessList = new Win32.SI_ACCESS[] { };
		Win32.GENERIC_MAPPING mapping = new Win32.GENERIC_MAPPING();
		public Win32.SI_OBJECT_INFO ObjectInfo { get { return objectInfo; } set { objectInfo = value; } }
		public Win32.SI_ACCESS[] AccessList { get { return accessList; } set { accessList = value; } }
		public Win32.GENERIC_MAPPING GenericMapping { get { return mapping; } set { mapping = value; } }
		public string ObjectName { get { return objectInfo.szObjectName; } set { objectInfo.szObjectName = value; } }
		#region ISecurityInformation implementation
		public virtual void GetObjectInformation(ref Win32.SI_OBJECT_INFO objectInfo) {
			objectInfo = this.objectInfo;
		}
		public virtual void GetSecurity(int requestInformation, IntPtr ppSecurityDescriptor, bool fDefault) {
		}
		public virtual void SetSecurity(int requestInformation, IntPtr securityDescriptor) {
		}
		public virtual void GetAccessRight(IntPtr guidObject, int dwFlags, out Win32.SI_ACCESS[] access, ref int accessCount, ref int defaultAccess) {
			access = this.accessList;
			accessCount = accessList.Length;
			defaultAccess = 0;
		}
		public virtual void GetInheritTypes(ref Win32.SI_INHERIT_TYPE inheritType, IntPtr inheritTypesCount) {
		}
		public virtual void PropertySheetPageCallback(IntPtr hwnd, int uMsg, int uPage) {
		}
		public virtual void MapGeneric(IntPtr guidObjectType, IntPtr aceFlags, IntPtr mask) {
			Win32.MapGenericMask(mask, ref this.mapping);
		}
		#endregion
	}
	#endregion
	public static partial class Win32 {
		const int SDDL_REVISION = 1;
		[SecuritySafeCritical]
		public static void MapGenericMask(IntPtr mask, ref Win32.GENERIC_MAPPING map) {
			PInvokeSafeNativeMethods.MapGenericMask(mask, ref map);
		}
		[SecuritySafeCritical]
		public static bool EditSecurity(IWin32Window parent, ISecurityInformation securityInformation) {
			IntPtr handle = (parent == null) ? IntPtr.Zero : parent.Handle;
			return PInvokeSafeNativeMethods.EditSecurity(handle, securityInformation);
		}
		[SecuritySafeCritical]
		public static void ConvertStringSecurityDescriptorToSecurityDescriptor(string stringSecurityDescriptor, IntPtr ppSecurityDescriptor) {
			PInvokeSafeNativeMethods.ConvertStringSecurityDescriptorToSecurityDescriptorW(stringSecurityDescriptor, SDDL_REVISION, ppSecurityDescriptor, IntPtr.Zero);
		}
		[SecuritySafeCritical]
		public static string ConvertSecurityDescriptorToStringSecurityDescriptor(IntPtr securityDescriptor, SECURITY_INFORMATION securityInformation) {
			IntPtr buffer;
			long bufferLength;
			int error = PInvokeSafeNativeMethods.ConvertSecurityDescriptorToStringSecurityDescriptorA(securityDescriptor, SDDL_REVISION, securityInformation, out buffer, out bufferLength);
			if (error == 0)
				return String.Empty;
			if (bufferLength <= 0)
				return String.Empty;
			string result = Marshal.PtrToStringAnsi(buffer);
			Marshal.FreeHGlobal(buffer);
			return result;
		}
		[SecuritySafeCritical]
		public static void SetSecurityDescriptorControl(IntPtr handle, SECURITY_DESCRIPTOR_CONTROL controlBitsOfInterest, SECURITY_DESCRIPTOR_CONTROL controlBitsToSet) {
			PInvokeSafeNativeMethods.SetSecurityDescriptorControl(handle, controlBitsOfInterest, controlBitsToSet);
		}
		[SecuritySafeCritical]
		public static bool ConvertStringSidToSid(string stringSid, out IntPtr ptrSid) {
			return PInvokeSafeNativeMethods.ConvertStringSidToSid(stringSid, out ptrSid);
		}
		[SecuritySafeCritical]
		public static bool SetSecurityDescriptorOwner(IntPtr pSecurityDescriptor, IntPtr pOwner, int bOwnerDefaulted) {
			return PInvokeSafeNativeMethods.SetSecurityDescriptorOwner(pSecurityDescriptor, pOwner, bOwnerDefaulted) != 0;
		}
		[SecuritySafeCritical]
		public static bool SetSecurityDescriptorGroup(IntPtr pSecurityDescriptor, IntPtr pOwner, int bOwnerDefaulted) {
			return PInvokeSafeNativeMethods.SetSecurityDescriptorGroup(pSecurityDescriptor, pOwner, bOwnerDefaulted) != 0;
		}
		#region SI_ACCESS
		[StructLayout(LayoutKind.Sequential)]
		public struct SI_ACCESS {
			public IntPtr guidObjectType;
			public int mask;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string szName;
			public int dwFlags;
			public static readonly int SizeOf = Marshal.SizeOf(typeof(SI_ACCESS));
		}
		#endregion
		#region SI_INHERIT_TYPE
		[StructLayout(LayoutKind.Sequential)]
		public struct SI_INHERIT_TYPE {
			public IntPtr guidObjectType;
			public int dwFlags;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string szName;
		}
		#endregion
		#region SI_OBJECT_INFO
		[StructLayout(LayoutKind.Sequential)]
		public struct SI_OBJECT_INFO {
			public int dwFlags;
			public IntPtr hInstance;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string szServerName;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string szObjectName;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string szPageTitle;
			public Guid guidObjectType;
		}
		#endregion
		#region GENERIC_MAPPING
		[StructLayout(LayoutKind.Sequential)]
		public struct GENERIC_MAPPING {
			public int GenericRead;
			public int GenericWrite;
			public int GenericExecute;
			public int GenericAll;
		}
		#endregion
		#region SECURITY_DESCRIPTOR_CONTROL
		[Flags]
		public enum SECURITY_DESCRIPTOR_CONTROL {
			SE_OWNER_DEFAULTED = 0x0001,
			SE_GROUP_DEFAULTED = 0x0002,
			SE_DACL_PRESENT = 0x0004,
			SE_DACL_DEFAULTED = 0x0008,
			SE_SACL_DEFAULTED = 0x0008,
			SE_SACL_PRESENT = 0x0010,
			SE_DACL_AUTO_INHERIT_REQ = 0x0100,
			SE_SACL_AUTO_INHERIT_REQ = 0x0200,
			SE_DACL_AUTO_INHERITED = 0x0400,
			SE_SACL_AUTO_INHERITED = 0x0800,
			SE_DACL_PROTECTED = 0x1000,
			SE_SACL_PROTECTED = 0x2000,
			SE_RM_CONTROL_VALID = 0x4000,
			SE_SELF_RELATIVE = 0x8000,
		}
		#endregion
		#region SECURITY_INFORMATION
		[Flags]
		public enum SECURITY_INFORMATION : int {
			OWNER_SECURITY_INFORMATION = 0x00000001,
			GROUP_SECURITY_INFORMATION = 0x00000002,
			DACL_SECURITY_INFORMATION = 0x00000004,
			SACL_SECURITY_INFORMATION = 0x00000008,
			UNPROTECTED_SACL_SECURITY_INFORMATION = 0x10000000,
			UNPROTECTED_DACL_SECURITY_INFORMATION = 0x20000000,
			PROTECTED_SACL_SECURITY_INFORMATION = 0x40000000,
		}
		#endregion
		#region SI_ACCESS_FLAG
		public enum SI_ACCESS_FLAG {
			SI_ACCESS_SPECIFIC = 0x00010000,
			SI_ACCESS_GENERAL = 0x00020000,
			SI_ACCESS_CONTAINER = 0x00040000,
			SI_ACCESS_PROPERTY = 0x00080000
		}
		#endregion
	}
	static partial class PInvokeSafeNativeMethods {
		[DllImport("aclui.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool EditSecurity(IntPtr hwnd, ISecurityInformation psi);
		[DllImport("advapi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern void MapGenericMask(IntPtr mask, ref Win32.GENERIC_MAPPING map);
		[DllImport("advapi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern void SetSecurityDescriptorControl(IntPtr handle, Win32.SECURITY_DESCRIPTOR_CONTROL controlBitsOfInterest, Win32.SECURITY_DESCRIPTOR_CONTROL controlBitsToSet);
		[DllImport("advapi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int ConvertSecurityDescriptorToStringSecurityDescriptorA(
			IntPtr securityDescriptor,
			int requestedStringSDRevision,
			Win32.SECURITY_INFORMATION securityInformation,
			[Out]
			out IntPtr stringSecurityDescriptor,
			[Out]
			out long stringSecurityDescriptorLen);
		[DllImport("advapi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int ConvertStringSecurityDescriptorToSecurityDescriptorW(
			[MarshalAs(UnmanagedType.LPWStr)]
			string stringSecurityDescriptor,
			int stringSDRevision,
			IntPtr ppSecurityDescriptor,
			IntPtr securityDescriptorSize
		);
		[DllImport("advapi32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool ConvertStringSidToSid(string stringSid, out IntPtr ptrSid);
		[DllImport("advapi32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int SetSecurityDescriptorOwner(IntPtr pSecurityDescriptor, IntPtr pOwner, int bOwnerDefaulted);
		[DllImport("advapi32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int SetSecurityDescriptorGroup(IntPtr pSecurityDescriptor, IntPtr pOwner, int bOwnerDefaulted);
	}
}
