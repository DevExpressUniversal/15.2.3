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
using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;
namespace DevExpress.ExpressApp.Design.Commands {
	public class MarkerManager : IVsTextMarkerClient {
		private _DTE dte;
		private System.IServiceProvider serviceProvider;
		private IVsTextLineMarker currentMarker;
		private IVsTextLines FindTextBuffer(String filePath) {
			if(String.IsNullOrEmpty(filePath))
				return null;
			Microsoft.VisualStudio.Shell.Interop.IVsRunningDocumentTable runningDocumentTable = serviceProvider.GetService(typeof(Microsoft.VisualStudio.Shell.Interop.IVsRunningDocumentTable)) as Microsoft.VisualStudio.Shell.Interop.IVsRunningDocumentTable;
			if(runningDocumentTable == null)
				return null;
			Microsoft.VisualStudio.Shell.Interop.IVsHierarchy pHier;
			uint itemid;
			IntPtr punkDocData;
			uint dwCookie;
			int hRetVal = runningDocumentTable.FindAndLockDocument((uint)Microsoft.VisualStudio.Shell.Interop._VSRDTFLAGS.RDT_NoLock, filePath, out pHier, out itemid, out punkDocData, out dwCookie);
			if(hRetVal != 0) {
				return null;
			}
			IVsTextLines textBuffer = null;
			if(punkDocData != IntPtr.Zero) {
				object textBufferObject = System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(punkDocData);
				System.Runtime.InteropServices.Marshal.Release(punkDocData);
				textBuffer = textBufferObject as IVsTextLines;
				if(textBuffer == null) {
					Microsoft.VisualStudio.Shell.Interop.IVsTextBufferProvider textBufferProvider = textBufferObject as Microsoft.VisualStudio.Shell.Interop.IVsTextBufferProvider;
					if(textBufferProvider != null) {
						if(textBufferProvider.GetTextBuffer(out textBuffer) != 0)
							textBuffer = null;
					}
				}
			}
			return textBuffer;
		}
		private int GetMarkerType(Guid guidMarker) {
			int result = -1;
			IVsTextManager manager = serviceProvider.GetService(typeof(VsTextManagerClass)) as IVsTextManager;
			if(manager != null) {
				int hRetVal = manager.GetRegisteredMarkerTypeID(ref guidMarker, out result);
				if(hRetVal != 0) {
					result = -1;
				}
			}
			return result;
		}
		public MarkerManager(_DTE dte, System.IServiceProvider serviceProvider) {
			this.dte = dte;
			this.serviceProvider = serviceProvider;
		}
		public void DeleteMarker() {
			if(currentMarker != null) {
				try {
					currentMarker.UnadviseClient();
					currentMarker.Invalidate();
				}
				catch { }
				currentMarker = null;
			}
		}
		public void DrawMarker(String fileName, Int32 startLine, Int32 endLine) {
			if(dte.MainWindow != null) {
				dte.MainWindow.SetFocus();
			}
			EnvDTE.Window window = dte.OpenFile(Constants.vsViewKindCode, fileName);
			if(window != null) {
				window.Activate();
				TextSelection textSelection = (TextSelection)window.Document.Selection;
				textSelection.GotoLine(endLine + 1, false);
			}
			IVsTextLines textBuffer = FindTextBuffer(fileName);
			int markerType = GetMarkerType(new Guid("6d799b25-7f32-4173-a4eb-521827d4ae4f"));
			if((textBuffer != null) && (markerType != -1)) {
				IVsTextLineMarker[] markers = new IVsTextLineMarker[1];
				textBuffer.CreateLineMarker(markerType, startLine, 0, endLine, 1000, this, markers);
				currentMarker = markers[0];
			}
		}
		#region IVsTextMarkerClient Members
		public int ExecMarkerCommand(IVsTextMarker pMarker, int iItem) {
			return 0;
		}
		public int GetMarkerCommandInfo(IVsTextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf) {
			return 0;
		}
		public int GetTipText(IVsTextMarker pMarker, string[] pbstrText) {
			return 0;
		}
		public void MarkerInvalidated() {
		}
		public int OnAfterMarkerChange(IVsTextMarker pMarker) {
			return 0;
		}
		public void OnAfterSpanReload() {
		}
		public void OnBeforeBufferClose() {
		}
		public void OnBufferSave(string pszFileName) {
		}
		#endregion
	}
}
