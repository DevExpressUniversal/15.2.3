#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace DevExpress.ExpressApp.HtmlPropertyEditor.Win {
	[ComImport, Guid("D001F200-EF97-11CE-9BC9-00AA00608E01"), InterfaceType((short)1)]
	public interface IOleUndoManager {
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Open([In, MarshalAs(UnmanagedType.Interface)] IOleParentUndoUnit pPUU);
		[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		int Close([In, MarshalAs(UnmanagedType.Interface)] IOleParentUndoUnit pPUU, [In, ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")] int fCommit);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Add([In, MarshalAs(UnmanagedType.Interface)] IOleUndoUnit pUU);
		[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		int GetOpenParentState([ComAliasName("Microsoft.VisualStudio.OLE.Interop.DWORD")] out uint pdwState);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void DiscardFrom([In, MarshalAs(UnmanagedType.Interface)] IOleUndoUnit pUU);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void UndoTo([In, MarshalAs(UnmanagedType.Interface)] IOleUndoUnit pUU);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void RedoTo([In, MarshalAs(UnmanagedType.Interface)] IOleUndoUnit pUU);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void EnumUndoable([MarshalAs(UnmanagedType.Interface)] out IEnumOleUndoUnits ppEnum);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void EnumRedoable([MarshalAs(UnmanagedType.Interface)] out IEnumOleUndoUnits ppEnum);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void GetLastUndoDescription([MarshalAs(UnmanagedType.BStr)] out string pBstr);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void GetLastRedoDescription([MarshalAs(UnmanagedType.BStr)] out string pBstr);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Enable([In, ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")] int fEnable);
	}
	[ComImport, Guid("B3E7C340-EF97-11CE-9BC9-00AA00608E01"), InterfaceType((short)1)]
	public interface IEnumOleUndoUnits {
		[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		int Next([In, ComAliasName("Microsoft.VisualStudio.OLE.Interop.ULONG")] uint celt, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0)] IOleUndoUnit[] rgelt, [ComAliasName("Microsoft.VisualStudio.OLE.Interop.ULONG")] out uint pceltFetched);
		[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		int Skip([In, ComAliasName("Microsoft.VisualStudio.OLE.Interop.ULONG")] uint celt);
		[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		int Reset();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumOleUndoUnits ppEnum);
	}
	[ComImport, InterfaceType((short)1), Guid("A1FAF330-EF97-11CE-9BC9-00AA00608E01")]
	public interface IOleParentUndoUnit : IOleUndoUnit {
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Open([In, MarshalAs(UnmanagedType.Interface)] IOleParentUndoUnit pPUU);
		[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		int Close([In, MarshalAs(UnmanagedType.Interface)] IOleParentUndoUnit pPUU, [In, ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")] int fCommit);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Add([In, MarshalAs(UnmanagedType.Interface)] IOleUndoUnit pUU);
		[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		int FindUnit([In, MarshalAs(UnmanagedType.Interface)] IOleUndoUnit pUU);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void GetParentState([ComAliasName("Microsoft.VisualStudio.OLE.Interop.DWORD")] out uint pdwState);
	}
	[ComImport, Guid("894AD3B0-EF97-11CE-9BC9-00AA00608E01"), InterfaceType((short)1)]
	public interface IOleUndoUnit {
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Do([In, MarshalAs(UnmanagedType.Interface)] IOleUndoManager pUndoManager);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void GetDescription([MarshalAs(UnmanagedType.BStr)] out string pBstr);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void GetUnitType(out Guid pClsid, [ComAliasName("Microsoft.VisualStudio.OLE.Interop.LONG")] out int plID);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void OnNextAdd();
	}
	[ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType((short)1)]
	public interface IServiceProvider {
		[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		int QueryService([In, ComAliasName("Microsoft.VisualStudio.OLE.Interop.REFGUID")] ref Guid guidService, [In, ComAliasName("Microsoft.VisualStudio.OLE.Interop.REFIID")] ref Guid riid, out IntPtr ppvObject);
	}
}
