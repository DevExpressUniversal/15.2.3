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
using DevExpress.Services;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Mouse;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Internal {
	public partial class InnerSpreadsheetControl {
		Stack<MouseHandler> mouseHandlers;
		SpreadsheetMouseHandler defaultMouseHandler;
		public MouseHandler MouseHandler { get { return mouseHandlers.Peek(); } }
		protected internal Stack<MouseHandler> MouseHandlers { get { return mouseHandlers; } }
		protected internal virtual SpreadsheetMouseHandler CreateMouseHandler() {
			return Owner.CreateMouseHandler();
		}
		#region InitializeMouseHandlers
		protected internal virtual void InitializeMouseHandlers() {
			this.defaultMouseHandler = CreateMouseHandler();
			this.defaultMouseHandler.Initialize();
			this.mouseHandlers = new Stack<MouseHandler>();
			SetNewMouseHandler(this.defaultMouseHandler);
		}
		protected internal virtual void SetNewMouseHandler(MouseHandler mouseHandler) {
			this.mouseHandlers.Push(mouseHandler);
		}
		protected internal virtual void RestoreMouseHandler() {
			this.mouseHandlers.Pop();
		}
		#endregion
		public virtual void OnMouseMove(MouseEventArgs e) {
			if (IsDisposed)
				return;
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null)
				svc.OnMouseMove(e);
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			if (IsDisposed)
				return;
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null)
				svc.OnMouseDown(e);
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			if (IsDisposed)
				return;
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null)
				svc.OnMouseUp(e);
		}
		public virtual bool OnMouseWheel(MouseEventArgs e) {
			if (IsDisposed)
				return false;
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null) {
				svc.OnMouseWheel(e);
				return true;
			}
			else
				return false;
		}
		public virtual void OnDragEnter(DragEventArgs e) {
			if (IsDisposed)
				return;
			System.Diagnostics.Debug.WriteLine(String.Format("->OnDragEnter:{0}", GetHashCode()));
			SpreadsheetMouseHandler handler = MouseHandler as SpreadsheetMouseHandler;
			if (handler != null)
				handler.OnDragEnter(e);
		}
		public virtual void OnDragOver(DragEventArgs e) {
			if (IsDisposed)
				return;
			System.Diagnostics.Debug.WriteLine(String.Format("->OnDragOver:{0}", GetHashCode()));
			SpreadsheetMouseHandler handler = MouseHandler as SpreadsheetMouseHandler;
			if (handler != null)
				handler.OnDragOver(e);
		}
		public virtual void OnDragDrop(DragEventArgs e) {
			if (IsDisposed)
				return;
			System.Diagnostics.Debug.WriteLine(String.Format("->OnDragDrop:{0}", GetHashCode()));
			SpreadsheetMouseHandler handler = MouseHandler as SpreadsheetMouseHandler;
			if (handler != null)
				handler.OnDragDrop(e);
		}
		public virtual void OnDragLeave(EventArgs e) {
			if (IsDisposed)
				return;
			System.Diagnostics.Debug.WriteLine(String.Format("->OnDragLeave:{0}", GetHashCode()));
			SpreadsheetMouseHandler handler = MouseHandler as SpreadsheetMouseHandler;
			if (handler != null)
				handler.OnDragLeave(e);
		}
		public virtual void OnGiveFeedback(GiveFeedbackEventArgs e) {
			if (IsDisposed)
				return;
			System.Diagnostics.Debug.WriteLine(String.Format("->OnGiveFeedback:{0}", GetHashCode()));
			SpreadsheetMouseHandler handler = MouseHandler as SpreadsheetMouseHandler;
			if (handler != null)
				handler.OnGiveFeedback(e);
		}
		public virtual void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			if (IsDisposed)
				return;
			System.Diagnostics.Debug.WriteLine(String.Format("->OnQueryContinueDrag:{0}", GetHashCode()));
			SpreadsheetMouseHandler handler = MouseHandler as SpreadsheetMouseHandler;
			if (handler != null)
				handler.OnQueryContinueDrag(e);
		}
		protected internal virtual MouseCursorCalculator CreateMouseCursorCalculator() {
			return new MouseCursorCalculator(ActiveView);
		}
	}
}
