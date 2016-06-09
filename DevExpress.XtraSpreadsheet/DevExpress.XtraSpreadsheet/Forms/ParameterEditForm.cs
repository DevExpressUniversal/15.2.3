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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class ReferenceEditForm : XtraForm {
		#region Fields
		readonly IReferenceEditViewModel viewModel;
		Point expandReferenceEditSenderLocation;
		Size expandReferenceEditSenderSize;
		Control expandReferenceEditSenderParent;
		AnchorStyles expandReferenceEditSenderAnchor;
		Size expandFormSize;
		Size minimumExpandFormSize;
		const int SenderXYCoordinate = 7;
		#endregion
		public ReferenceEditForm(IReferenceEditViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			this.TopMost = true;
			InitializeComponent();
			DocumentModel.ReferenceEditMode = true;
			SpreadsheetControl.InnerControl.OnUpdateUI();
			SubscribeXtraSpreadsheetEvents();
		}
		public ReferenceEditForm() {
			InitializeComponent();
		}
		#region Properties
		protected IReferenceEditViewModel ViewModel { get { return viewModel; } }
		protected ISpreadsheetControl SpreadsheetControl { get { return ViewModel.Control; } }
		protected DevExpress.XtraSpreadsheet.SpreadsheetControl XtraSpreadsheetControl { get { return SpreadsheetControl as DevExpress.XtraSpreadsheet.SpreadsheetControl; } }
		protected DocumentModel DocumentModel { get { return SpreadsheetControl.InnerControl.DocumentModel; } }
		bool ShowReferenceSelection { get { return DocumentModel.ShowReferenceSelection; } set { DocumentModel.ShowReferenceSelection = value; } }
		#endregion
		#region Subscribe/Unsubscribe referenceEditControls' events
		protected internal virtual int SubscribeReferenceEditControlsEvents() {
			return ForEachControlResursive(this, SubscribeReferenceEditControlsEvents);
		}
		protected internal virtual void SubscribeReferenceEditControlsEvents(Control control) {
			ReferenceEditControl referenceEdit = control as ReferenceEditControl;
			if (referenceEdit != null) {
				referenceEdit.CollapsedChanged += OnReferenceEditRaiseCollapsedChanged;
				referenceEdit.GotFocus += OnReferenceEditGotFocus;
			}
			else
				control.GotFocus += OnControlGotFocus;
		}
		protected internal virtual int UnsubscribeReferenceEditControlsEvents() {
			return ForEachControlResursive(this, UnsubscribeReferenceEditControlsEvents);
		}
		protected internal virtual void UnsubscribeReferenceEditControlsEvents(Control control) {
			ReferenceEditControl referenceEdit = control as ReferenceEditControl;
			if (referenceEdit != null) {
				referenceEdit.CollapsedChanged -= OnReferenceEditRaiseCollapsedChanged;
				referenceEdit.GotFocus -= OnReferenceEditGotFocus;
			}
			else
				control.GotFocus -= OnControlGotFocus;
		}
		protected int ForEachControlResursive(Control root, Action<Control> action) {
			int count = 0;
			List<Control> children = new List<Control>();
			foreach (Control control in root.Controls)
				children.Add(control);
			foreach (Control control in children)
				count += ForEachControlResursiveCore(control, action);
			return count;
		}
		protected virtual int ForEachControlResursiveCore(Control control, Action<Control> action) {
			int count = 0;
			action(control);
			count++;
			Panel panel = control as Panel;
			if (panel != null)
				count += ForEachControlResursive(panel, action);
			ScrollableControl scrollableControl = control as ScrollableControl;
			if (scrollableControl != null)
				count += ForEachControlResursive(scrollableControl, action);
			return count;
		}
		#endregion
		#region Subscribe / Unsubscribe XtraSpreadsheet's events
		void SubscribeXtraSpreadsheetEvents() {
			if (XtraSpreadsheetControl != null) {
				XtraSpreadsheetControl.GotFocus += OnXtraSpreadsheetControlGotFocus;
				XtraSpreadsheetControl.BeforeDispose += OnXtraSpreadsheetControlBeforeDispose;
			}
		}
		void UnsubscribeXtraSpreadsheetEvents() {
			XtraSpreadsheetControl.GotFocus -= OnXtraSpreadsheetControlGotFocus;
			XtraSpreadsheetControl.BeforeDispose -= OnXtraSpreadsheetControlBeforeDispose;
		}
		#endregion
		void OnXtraSpreadsheetControlBeforeDispose(object sender, EventArgs e) {
			Close();
		}
		#region OnXtraSpreadsheetControlGotFocus
		void OnXtraSpreadsheetControlGotFocus(object sender, EventArgs e) {
			Action<Control> action = (Control control) => {
				ReferenceEditControl referenceEdit = control as ReferenceEditControl;
				if (referenceEdit != null && referenceEdit.Activated) {
					ShowReferenceSelection = true;
				}
			};
			ForEachControlResursive(this, action);
		}
		#endregion
		#region OnReferenceEditRaiseCollapsedChanged
		void OnReferenceEditRaiseCollapsedChanged(object sender, CollapsedChangedEventArgs e) {
			if (e.Collapsed)
				Collapse(sender);
			else
				Expand(sender);
		}
		#endregion
		#region Collapse
		protected virtual void Collapse(object sender) {
			Action<Control> action = (Control control) => {
				if (Object.ReferenceEquals(control, sender))
					CollapseCore((ReferenceEditControl)control);
				else
					control.Visible = false;
			};
			UnsubscribeReferenceEditControlsEvents();
			try {
				ForEachControlResursive(this, action);
			}
			finally {
				SubscribeReferenceEditControlsEvents();
			}
		}
		void CollapseCore(ReferenceEditControl control) {
			expandReferenceEditSenderLocation = control.Location;
			expandReferenceEditSenderSize = control.Size;
			expandReferenceEditSenderParent = control.Parent;
			expandReferenceEditSenderAnchor = control.Anchor;
			control.Location = new Point(SenderXYCoordinate, SenderXYCoordinate);
			control.Size = new Size(ClientSize.Width - SenderXYCoordinate * 2, control.Height);
			control.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			expandFormSize = this.Size;
			minimumExpandFormSize = this.MinimumSize;
			int formHeaderHeight = Size.Height - ClientSize.Height;
			Size clientSize = new Size(ClientSize.Width, control.Height + SenderXYCoordinate * 2);
			MinimumSize = new Size(clientSize.Width, clientSize.Height + formHeaderHeight);
			ClientSize = clientSize;
			if (control.Parent != this) {
				object editValue = control.EditValue;
				control.Parent.Controls.Remove(control);
				this.Controls.Add(control);
				control.EditValue = editValue;
				control.Focus();
			}
		}
		#endregion
		#region OnControlGotFocus
		void OnControlGotFocus(object sender, EventArgs e) {
			ResetOldIsActive();
			ShowReferenceSelection = false;
			RedrawSpreadsheetControl();
		}
		#endregion
		#region ResetOldIsActive
		void ResetOldIsActive() {
			Action<Control> action = (Control control) => {
				ReferenceEditControl referenceEdit = control as ReferenceEditControl;
				if (referenceEdit != null && referenceEdit.Activated) {
					referenceEdit.Activated = false;
				}
			};
			ForEachControlResursive(this, action);
		}
		#endregion
		#region OnReferenceEditGotFocus
		void OnReferenceEditGotFocus(object sender, EventArgs e) {
			ResetOldIsActive();
			ReferenceEditControl referenceEdit = sender as ReferenceEditControl;
			referenceEdit.Activated = true;
			ShowReferenceSelection = referenceEdit.Selection.Count != 0;
			RedrawSpreadsheetControl();
		}
		#endregion
		#region RedrawSpreadsheetControl
		protected void RedrawSpreadsheetControl() {
			if (SpreadsheetControl.InnerControl.Owner != null)
				SpreadsheetControl.InnerControl.Owner.Redraw();
		}
		#endregion
		#region Expand
		protected virtual void Expand(object sender) {
			SuspendLayout();
			try {
				MinimumSize = minimumExpandFormSize;
				Size = expandFormSize;
				Action<Control> action = (Control control) => {
					if (Object.ReferenceEquals(control, sender))
						ExpandCore((ReferenceEditControl)control);
					else
						control.Visible = true;
				};
				UnsubscribeReferenceEditControlsEvents();
				try {
					ForEachControlResursive(this, action);
				}
				finally {
					SubscribeReferenceEditControlsEvents();
				}
			}
			finally {
				ResumeLayout(true);
			}
		}
		void ExpandCore(ReferenceEditControl control) {
			control.Location = expandReferenceEditSenderLocation;
			control.Size = expandReferenceEditSenderSize;
			control.Anchor = expandReferenceEditSenderAnchor;
			if (expandReferenceEditSenderParent != this && expandReferenceEditSenderParent != null) {
				object editValue = control.EditValue;
				this.Controls.Remove(control);
				expandReferenceEditSenderParent.Controls.Add(control);
				control.EditValue = editValue;
				control.Focus();
			}
		}
		#endregion
		#region Dispose
		protected override void Dispose(bool disposing) {
			try {
				if (disposing)
					DisposeCore();
			}
			finally {
				base.Dispose(disposing);
			}
		}
		void DisposeCore() {
			if (ViewModel == null || SpreadsheetControl == null)
				return;
			if (SpreadsheetControl.InnerControl != null) {
				if (DocumentModel != null && (DocumentModel.ShowReferenceSelection || DocumentModel.ReferenceEditMode)) {
					DocumentModel.ShowReferenceSelection = false;
					DocumentModel.ReferenceEditMode = false;
					RedrawSpreadsheetControl();
				}
				SpreadsheetControl.InnerControl.OnUpdateUI();
			}
			UnsubscribeReferenceEditControlsEvents();
			UnsubscribeXtraSpreadsheetEvents();
		}
		#endregion
	}
	public enum FormViewState { 
		Expand,
		CollapsedAfterContinueSelection,
		CollapsedAfterCollapsedButtonClick
	}
}
