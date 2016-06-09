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
namespace DevExpress.XtraMap.Drawing {
	internal static class MemoryWrapper {
		[DllImport("kernel32.dll", EntryPoint = "RtlFillMemory", SetLastError = false)]
		public static extern void FillMemory(IntPtr destination, uint length, byte fill);
		[DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
		public static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);
	}
	internal static class MarshalHelper {
		[SecuritySafeCritical]
		internal static IntPtr AllocHGlobal(int cb) {
			return Marshal.AllocHGlobal(cb);
		}
		[SecuritySafeCritical]
		internal static void FreeHGlobal(IntPtr hglobal) {
			Marshal.FreeHGlobal(hglobal);
		}
		[SecuritySafeCritical]
		internal static void Copy(byte[] source, int startIndex, IntPtr destination, int length) {
			Marshal.Copy(source, startIndex, destination, length);
		}
		[SecuritySafeCritical]
		internal static void Copy(IntPtr source, double[] destination, int startIndex, int length) {
			Marshal.Copy(source, destination, startIndex, length);
		}
		[SecuritySafeCritical]
		internal static void Copy(double[] source, int startIndex, IntPtr destination, int length) {
			Marshal.Copy(source, startIndex, destination, length);
		}
		[SecuritySafeCritical]
		internal static int ReleaseComObject(object o) {
			return Marshal.ReleaseComObject(o);
		}
		[SecuritySafeCritical]
		internal static IntPtr UnsafeAddrOfPinnedArrayElement(Array arr, int index) {
			return Marshal.UnsafeAddrOfPinnedArrayElement(arr, index);
		}
		[SecuritySafeCritical]
		internal static int SizeOf(object structure) {
			return Marshal.SizeOf(structure);
		}
		[SecuritySafeCritical]
		internal static int SizeOf(Type t) {
			return Marshal.SizeOf(t);
		}
		[SecuritySafeCritical]
		internal static void WriteIntPtr(IntPtr ptr, IntPtr val) {
			Marshal.WriteIntPtr(ptr, val);
		}
		[SecuritySafeCritical]
		internal static void StructureToPtr(object structure, IntPtr ptr, bool deleteOld) {
			Marshal.StructureToPtr(structure, ptr, deleteOld);
		}
		[SecuritySafeCritical]
		internal static void FillMemory(IntPtr destination, uint length, byte fill){
			MemoryWrapper.FillMemory(destination, length, fill);
		}
		[SecuritySafeCritical]
		public static void CopyMemory(IntPtr destination, IntPtr source, uint length) {
			MemoryWrapper.CopyMemory(destination, source, length);
		}
	}
}
