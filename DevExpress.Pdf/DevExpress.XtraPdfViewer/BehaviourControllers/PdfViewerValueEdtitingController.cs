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

using DevExpress.Pdf.Native;
using DevExpress.Pdf.Drawing;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfViewerValueEditingController : PdfDisposableObject, IPdfViewerValueEditingController {
		readonly PdfViewer viewer;
		PdfEditorController controller;
		PdfStickyNote toolTip;
		IPdfViewerValueEditingCallBack editorCallback;
		PdfDocumentState documentState;
		public PdfViewerValueEditingController(PdfViewer viewer) {
			this.viewer = viewer;
		}
		public void PostValue() {
			if (controller != null)
				controller.PostValue();
		}
		public void CommitEditor() {
			PostValue();
		}
		public void HideTooltip() {
			if (toolTip != null) {
				toolTip.Hide();
				toolTip.Dispose();
				toolTip = null;
			}
			viewer.Focus();
		}
		public void ShowTooltip(PdfEditorSettings tooltipSettings) {
			PdfStickyNoteEditSettings stickySettings = (PdfStickyNoteEditSettings)tooltipSettings;
			PdfStickyNote stickyNote = new PdfStickyNote(stickySettings);
			toolTip = stickyNote;
			stickyNote.ShowStickyNote();
		}
		void IPdfViewerValueEditingController.CloseEditor() {
			DeleteEditor();
		}
		void IPdfViewerValueEditingController.ShowEditor(PdfEditorSettings editorSettings, IPdfViewerValueEditingCallBack editorCallback) {
			if (viewer.RotationAngle != 0) {
				editorCallback.HideEditor();
				return;
			}
			this.editorCallback = editorCallback;
			controller = PdfEditorController.Create(viewer, editorSettings, editorCallback);
			if (controller != null)
				controller.SetUp();
			documentState = viewer.DocumentState;
			documentState.DocumentStateChanged += OnDocumentStateChanged;
			viewer.DocumentChanged += OnViewerDocumentChanged;
		}
		void OnDocumentStateChanged(object sender, PdfDocumentStateChangedEventArgs e) {
			if (editorCallback != null && e.Flags.HasFlag(PdfDocumentStateChangedFlags.Rotate) && viewer.RotationAngle != 0)
				editorCallback.HideEditor();
		}
		void OnViewerDocumentChanged(object sender, PdfDocumentChangedEventArgs e) {
			DeleteToolTip();
			DeleteEditor();
		}
		void DeleteEditor() {
			if (documentState != null) {
				documentState.DocumentStateChanged -= OnDocumentStateChanged;
				documentState = null;
			}
			if (controller != null) {
				controller.Dispose();
				controller = null;
				viewer.DocumentChanged -= OnViewerDocumentChanged;
			}
			viewer.Focus();
		}
		void DeleteToolTip() {
			if (toolTip != null) {
				toolTip.HideStickyNote();
				toolTip.Dispose();
				toolTip = null;
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				DeleteToolTip();
				DeleteEditor();
			}
		}
	}
}
