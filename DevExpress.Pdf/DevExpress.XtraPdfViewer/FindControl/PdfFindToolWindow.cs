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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Win;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraPdfViewer.Commands;
using DevExpress.XtraPdfViewer.FindControl;
namespace DevExpress.XtraPdfViewer.Native {
	[DXToolboxItem(false)]
	public class PdfFindToolWindow : BasePopupToolWindow {
		readonly PdfViewer viewer;
		readonly PdfDocumentViewer documentViewer;
		PdfFindControl findControl;
		bool hiddenForSize;
		bool closing;
		PdfFindControl FindControl {
			get {
				if (findControl == null)
					findControl = new PdfFindControl();
				return findControl;
			}
		}
		protected override Control OwnerControl { get { return documentViewer == null ? null : documentViewer.ViewControl; } }
		protected override Control MessageRoutingTarget { get { return documentViewer.ViewControl; } }
		protected override ISupportLookAndFeel LookAndFeelProvider { get { return documentViewer; } }
		protected override int HorzIndent { get { return 0; } }
		protected override int VertIndent { get { return 0; } }
		protected override Size FormSize {
			get {
				Control content = Content;
				return content == null ? Size.Empty : content.Size;
			}
		}
		protected override bool SyncLocationWithOwner { get { return true; } }
		protected override PopupToolWindowAnchor AnchorType { get { return PopupToolWindowAnchor.Top; } }
		protected override PopupToolWindowAnimation AnimationType { get { return PopupToolWindowAnimation.Slide; } }
		protected override Point FormLocation { get { return Point.Empty; } }
		protected override bool KeepParentFormActive { get { return true; } }
		protected override bool AllowMessageRouting { get { return true; } }
		public bool CanShow { get { return documentViewer.ViewControl.Size.Height >= Height; } }
		public PdfFindDialogOptions FindDialogOptions { 
			get { return FindControl.FindDialogOptions; }
			set { FindControl.FindDialogOptions = value; }
		}
		public PdfFindToolWindow(PdfViewer viewer) {
			this.viewer = viewer;
			documentViewer = viewer.Viewer;
			KeyPreview = true;
			viewer.SizeChanged += new EventHandler(OnOwnerControlSizeChanged);
			PdfFindControl control = Content as PdfFindControl;
			if (control != null) {
				control.SetViewer(viewer);
				control.Close += new EventHandler(OnControlClose);
			}
			RegisterRoutedMessage(MSG.WM_MOUSEWHEEL);
			KeyPreview = true;
			KeyDown += new KeyEventHandler(OnKeyDown);
		}
		void OnKeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.C && e.Control && viewer.HasSelection) {
				PdfFindControl control = Content as PdfFindControl;
				if (control != null)
					foreach (Control childControl in control.Controls) {
						TextEdit textEdit = childControl as TextEdit;
						if (textEdit != null && textEdit.SelectionLength > 0 && textEdit.ContainsFocus)
							return;
					}
				new PdfCopyCommand(viewer).Execute();
			}
		}
		void OnOwnerControlSizeChanged(object sender, EventArgs e) {
			if (Visible) {
				if (!CanShow) {
					Visible = false;
					hiddenForSize = true;
				}
			}
			else if (hiddenForSize && CanShow) {
				Visible = true;
				hiddenForSize = false;
			}
		}
		void OnControlClose(object sender, EventArgs e) {
			if (!closing)
				HideToolWindow();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				KeyDown -= new KeyEventHandler(OnKeyDown);
				viewer.SizeChanged -= new EventHandler(OnOwnerControlSizeChanged);
				PdfFindControl control = Content as PdfFindControl;
				if (control != null) {
					control.Close -= new EventHandler(OnControlClose);
					control.Dispose();
				}
			}
		}
		protected override Control CreateContentControl() {
			return FindControl;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if (!closing && e.KeyCode == Keys.Escape)
				HideToolWindow();
		}
		protected override void OnStartAnimation(PopupToolWindowAnimationEventArgs e) {
			base.OnStartAnimation(e);
			if (!e.IsShowing)
				closing = true;
		}
		protected override void OnEndAnimation(PopupToolWindowAnimationEventArgs e) {
			base.OnEndAnimation(e);
			if (e.IsShowing) {
				PdfFindControl control = Content as PdfFindControl;
				if (control != null)
					FocusControl(control.TextEdit);
			}
			else {
				viewer.Focus();
				closing = false;
			}
		}
	}
}
