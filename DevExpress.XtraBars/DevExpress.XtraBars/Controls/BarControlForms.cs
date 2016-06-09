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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Objects;
using DevExpress.Utils.Win;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils;
namespace DevExpress.XtraBars.Forms {
	[ToolboxItem(false)]
	public class CustomFloatingForm : CustomTopForm, ISupportToolTipsForm {
		protected bool isFormDragging;
		protected Point downPoint;
		internal CustomFloatingForm() {
			isFormDragging = false;
			downPoint = Point.Empty;
		}
		internal void FocusCore() {
			if(!BarManager.AllowFocusPopupForm)
				return;
			Form frm = Form.ActiveForm;
			Focus();
			if(frm != null)
				NativeMethods.SendMessage(frm.Handle, MSG.WM_NCACTIVATE, 1, IntPtr.Zero);
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			FocusCore();
		}
		internal void FireMouseDown(MouseEventArgs e) {
			OnMouseDown(e);	
		}
		protected virtual bool CanDragged(MouseEventArgs e) {
			return false;
		}
		internal bool IsFormDragging { 
			get { return isFormDragging; }
			set {
				if(isFormDragging == value && Capture == value) return;
				isFormDragging = value;
				DraggingChanged();
			}
		}
		protected virtual void DraggingChanged() { 
			Capture = IsFormDragging;
			Cursor.Current = (IsFormDragging ? Cursors.SizeAll : Cursors.Default);
			this.downPoint = Control.MousePosition;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			Capture = true;
			if(CanDragged(e))
				IsFormDragging = true;
			UpdateZOrder(IntPtr.Zero);
			base.OnMouseDown(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			if(!IsFormDragging) {
				if(CanDragged(e) && e.Button == MouseButtons.Left)
					IsFormDragging = true;
			}
			if(IsFormDragging) {
				if(downPoint != Control.MousePosition) {
					Point p = new Point(Location.X - (downPoint.X - Control.MousePosition.X),
						Location.Y - (downPoint.Y - Control.MousePosition.Y));
					downPoint = Control.MousePosition;
					OnFormMoved(downPoint);
				}
				return;
			}
			base.OnMouseMove(e);
		}
		protected virtual void OnFormMoved(Point p) {
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			IsFormDragging = false;
			base.OnMouseUp(e);
		}
		protected override void WndProc(ref Message msg) {
			base.WndProc(ref msg);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
		}
		bool ISupportToolTipsForm.ShowToolTipsFor(Form form) {
			return true;
		}
		bool ISupportToolTipsForm.ShowToolTipsWhenInactive {
			get { return true; }
		}
	}
}
