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
using System.Windows.Forms;
using DevExpress.Office.Internal;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetControl : IMouseWheelSupport , IMouseWheelScrollClient {
		MouseWheelScrollHelper mouseHelper;
		#region IMouseWheelScrollClient Members
		void IMouseWheelScrollClient.OnMouseWheel(MouseWheelScrollClientArgs e) {
			OnMouseWheelEx(e);
		}
		protected virtual void OnMouseWheelEx(MouseWheelScrollClientArgs e) {
			OfficeMouseWheelEventArgs ea = new OfficeMouseWheelEventArgs(e.Button, e.Clicks, e.X, e.Y, -e.Distance * SystemInformation.MouseWheelScrollDelta);
			ea.IsHorizontal = e.IsHMouseWheel;
			OnMouseWheelCore(ea);
		}
		bool IMouseWheelScrollClient.PixelModeHorz { get { return false; } }
		bool IMouseWheelScrollClient.PixelModeVert { get { return false; } }
		#endregion
		#region Mouse handling
		protected override void OnMouseMove(MouseEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode)
				return;
			if (VerticalScrollBar != null)
				VerticalScrollBar.OnAction(ScrollNotifyAction.MouseMove); 
			if (HorizontalScrollBar != null)
				HorizontalScrollBar.OnAction(ScrollNotifyAction.MouseMove); 
			if (InnerControl != null)
				InnerControl.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode)
				return;
			if (InnerControl != null)
				InnerControl.OnMouseDown(e);
			base.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode)
				return;
			if (InnerControl != null)
				InnerControl.OnMouseUp(e);
			(InnerControl as IGestureStateIndicator).OnGestureEnd();
			base.OnMouseUp(e);
		}
		protected sealed override void OnMouseWheel(MouseEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode)
				return;
			if (XtraForm.ProcessSmartMouseWheel(this, e))
				return;
			this.mouseHelper.OnMouseWheel(e);
			base.OnMouseWheel(e);
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode)
				return;
			if (InnerControl != null)
				InnerControl.OnMouseWheel(e);
		}
		protected override void OnDragEnter(DragEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode) {
				base.OnDragEnter(e);
				return;
			}
			if (InnerControl != null)
				InnerControl.OnDragEnter(e);
			base.OnDragEnter(e);
		}
		protected override void OnDragOver(DragEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode) {
				base.OnDragOver(e);
				return;
			}
			if (InnerControl != null)
				InnerControl.OnDragOver(e);
			base.OnDragOver(e);
		}
		protected override void OnDragDrop(DragEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode) {
				base.OnDragDrop(e);
				return;
			}
			if (InnerControl != null)
				InnerControl.OnDragDrop(e);
			base.OnDragDrop(e);
		}
		protected override void OnDragLeave(EventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode) {
				base.OnDragLeave(e);
				return;
			}
			if (InnerControl != null)
				InnerControl.OnDragLeave(e);
			base.OnDragLeave(e);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			if (IsDisposed)
				return;
			if (InnerControl != null)
				InnerControl.OnGiveFeedback(e);
			base.OnGiveFeedback(e);
		}
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			if (IsDisposed)
				return;
			if (InnerControl != null)
				InnerControl.OnQueryContinueDrag(e);
			base.OnQueryContinueDrag(e);
		}
		#endregion
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			this.mouseHelper.OnMouseWheel(e);
			base.OnMouseWheel(e);
		}
	}
}
