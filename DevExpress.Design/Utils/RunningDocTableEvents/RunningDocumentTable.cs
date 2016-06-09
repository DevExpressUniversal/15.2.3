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
using System.Runtime.InteropServices;
using System.Security.Permissions;
using DevExpress.Utils.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
namespace DevExpress.Utils.Format {
	public static class VsFileFormatter {
		[CLSCompliant(false)]
		public static void Format(EnvDTE.DTE dte, string fileName) {
			try {
				if(dte == null || string.IsNullOrEmpty(fileName))
					return;
				IVsInvisibleEditorManager invisibleEditorManager = DTEHelper.Query<IVsInvisibleEditorManager>(dte, typeof(SVsInvisibleEditorManager).GUID);
				if(invisibleEditorManager == null)
					return;
				IVsInvisibleEditor pEditor;
				int hRetVal = invisibleEditorManager.RegisterInvisibleEditor(fileName, null, (uint)_EDITORREGFLAGS.RIEF_ENABLECACHING, null, out pEditor);
				if(hRetVal < 0)
					return;
				IntPtr ppDocData = IntPtr.Zero;
				Guid IID_IVsTextLines = typeof(IVsTextLines).GUID;
				hRetVal = pEditor.GetDocData(0, ref IID_IVsTextLines, out ppDocData);
				IVsTextLines textLines = RunningDocumentTableService.GetTextLines(fileName);
				Format(dte, textLines);
			}
			catch {
			}
		}
		static void Format(EnvDTE.DTE dte, IVsTextLines textLines) {
			IVsLanguageTextOps languageTextOps = GetVsLanaguageInfo(dte, textLines) as IVsLanguageTextOps;
			if(languageTextOps == null)
				return;
			IVsTextLayer bufferLayer = textLines as IVsTextLayer;
			if(bufferLayer == null)
				return;
			TextSpan[] formatSpan = new TextSpan[1] { GetBufferTextSpan(textLines) };
			int hRetVal = languageTextOps.Format(bufferLayer, formatSpan);
		}
		static TextSpan GetBufferTextSpan(IVsTextLines buffer) {
			if(buffer == null)
				return new TextSpan();
			int iLine, iIndex;
			int hRetVal = buffer.GetLastLineIndex(out iLine, out iIndex);
			TextSpan span = new TextSpan();
			if(hRetVal < 0)
				return span;
			span.iStartLine = 0;
			span.iStartIndex = 0;
			span.iEndLine = iLine;
			span.iEndIndex = iIndex;
			return span;
		}
		static IVsLanguageInfo GetVsLanaguageInfo(EnvDTE.DTE dte, IVsTextLines textLines) {
			if(dte == null || textLines == null)
				return null;
			Guid languageServiceId = GetLanguageServiceId(textLines);
			if(languageServiceId == Guid.Empty)
				return null;
			return DTEHelper.Query<IVsLanguageInfo>(dte, languageServiceId);
		}
		static Guid GetLanguageServiceId(IVsTextLines buffer) {
			if(buffer == null)
				return Guid.Empty;
			Guid guidLangService;
			int hRetVal = buffer.GetLanguageServiceID(out guidLangService);
			if(hRetVal < 0)
				return Guid.Empty;
			return guidLangService;
		}
	}
	[CLSCompliant(false)]
	public class RunningDocumentTableService {
		static RunningDocumentTable rdt;
		public static void Subscribe(EnvDTE.DTE dte) {
			try {
				if(rdt != null || dte == null)
					return;
				rdt = new RunningDocumentTable(dte);
			}
			catch {
			}
		}
		public static void Unsubscribe() {
			try {
				if(rdt == null)
					return;
				rdt.Unsubscribe();
				rdt = null;
			}
			catch {
			}
		}
		internal static IVsTextLines GetTextLines(string filePath) {
			if(string.IsNullOrEmpty(filePath) || rdt == null)
				return null;
			return rdt.GetTextLines(filePath);
		}
	}
	[CLSCompliant(false)]
	public class RunningDocumentTable : IVsRunningDocTableEvents3 {
		const int S_OK = 0;
		uint dwCookie;
		Dictionary<string, IVsTextLines> buffers;
		IVsRunningDocumentTable rdt = null;
		public RunningDocumentTable(EnvDTE.DTE dte) {
			buffers = new Dictionary<string, IVsTextLines>();
			Subscribe(dte);
		}
		public void Subscribe(EnvDTE.DTE dte) {
			try {
				rdt = DTEHelper.Query<IVsRunningDocumentTable>(dte, typeof(SVsRunningDocumentTable).GUID);
				if(rdt == null)
					return;
				rdt.AdviseRunningDocTableEvents((IVsRunningDocTableEvents)this, out dwCookie);
			}
			catch {
			}
		}
		public void Unsubscribe() {
			try {
				if(rdt == null)
					return;
				rdt.UnadviseRunningDocTableEvents(dwCookie);
			}
			catch {
			}
		}
		public int OnAfterAttributeChange(uint docCookie, uint grfAttribs) {
			return S_OK;
		}
		public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame) {
			return S_OK;
		}
		public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) {
			SetVsTextLines((int)docCookie);
			return S_OK;
		}
		void SetVsTextLines(int cookie) {
			try {
				if(rdt == null || cookie == 0)
					return;
				uint grfRDTFlags, dwReadLocks, dwEditLocks, itemid;
				string bstrMkDocument;
				IVsHierarchy pHier;
				IntPtr punkDocData;
				int hRetVal = rdt.GetDocumentInfo((uint)cookie, out grfRDTFlags, out dwReadLocks, out dwEditLocks, out bstrMkDocument, out pHier, out itemid, out punkDocData);
				if(hRetVal < 0 || punkDocData == IntPtr.Zero || String.IsNullOrEmpty(bstrMkDocument) || (buffers != null && buffers.ContainsKey(bstrMkDocument)))
					return;
				object unkDocData = null;
				try {
					unkDocData = Marshal.GetObjectForIUnknown(punkDocData);
					if(!HasInterface(punkDocData, typeof(IVsTextBuffer).GUID) && !HasInterface(punkDocData, typeof(IVsTextBufferProvider).GUID))
						return;
				}
				finally {
					Marshal.Release(punkDocData);
				}
				if(unkDocData == null)
					return;
				IVsTextLines buffer = unkDocData as IVsTextLines;
				if(buffer == null)
					return;
				buffers.Add(bstrMkDocument, buffer);
			}
			catch {
			}
		}
		internal IVsTextLines GetTextLines(string filePath) {
			if(buffers == null || String.IsNullOrEmpty(filePath) || !buffers.ContainsKey(filePath))
				return null;
			return buffers[filePath];
		}
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		public static bool HasInterface(IntPtr punk, Guid iid) {
			if(punk == IntPtr.Zero)
				return false;
			IntPtr pv;
			int hRetVal = Marshal.QueryInterface(punk, ref iid, out pv);
			if(pv != IntPtr.Zero)
				Marshal.Release(pv);
			return hRetVal >= 0;
		}
		public int OnAfterSave(uint docCookie) {
			return S_OK;
		}
		public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame) {
			return S_OK;
		}
		public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) {
			return S_OK;
		}
		public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew) {
			return S_OK;
		}
		public int OnBeforeSave(uint docCookie) {
			return S_OK;
		}
	}
}
