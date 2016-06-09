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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Commands;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Toolbox;
using DevExpress.XtraDiagram.ViewInfo;
using DevExpress.XtraToolbox;
namespace DevExpress.XtraDiagram.Handlers {
	public class DiagramControlHandler : IDisposable {
		DiagramControl diagram;
		public DiagramControlHandler(DiagramControl diagram) {
			this.diagram = diagram;
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			DiagramController.ProcessMouseDown(e.GetIMouseButtonArgs(this));
			DiagramControlHitInfo hitInfo = Diagram.CalcHitInfo(e.Location);
			if(!hitInfo.InEditSurface) {
				Diagram.AdornerController.EnsureEditSurfaceDestroyed();
			}
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			DiagramController.ProcessMouseUp(e.GetIMouseButtonArgs(this));
		}
		public virtual void OnMouseDoubleClick(MouseEventArgs e) {
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			DiagramController.ProcessMouseMove(e.GetIMouseButtonArgs(this));
		}
		public virtual void OnMouseWheel(MouseEventArgs e) {
			if((Control.ModifierKeys & Keys.Control) != 0) {
				DoZoom(e);
			}
			else {
				if(Diagram.IsVScrollVisible) DoMouseWheel(e);
			}
		}
		protected virtual void DoMouseWheel(MouseEventArgs e) {
			int distance = Diagram.DiagramViewInfo.GetMouseWheelDistance();
			Diagram.VScrollPos += (e.Delta > 0 ? -distance : distance);
		}
		protected virtual void DoZoom(MouseEventArgs e) {
			DiagramController.Zoom(e.Delta, e.GetIMouseButtonArgs(this));
			if(Diagram.IsTextEditMode) {
				Diagram.RefreshActiveEditor();
			}
		}
		public virtual void OnMouseEnter(EventArgs e) {
		}
		public virtual void OnMouseLeave(EventArgs e) {
			DiagramController.ProcessMouseLeave(Diagram.CreatePlatformMouseArgs());
		}
		public virtual void OnGotCapture() {
		}
		public virtual void OnLostCapture() {
			DiagramController.ProcessLostMouseCapture(Diagram.CreatePlatformMouseArgs());
		}
		public virtual void OnKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Escape && Diagram.IsTextEditMode) {
				Diagram.HideEditor();
				return;
			}
			DiagramController.ProcessKeyDown(e.GetPlatformKey(), Diagram.CreatePlatformMouseArgs());
		}
		public virtual void OnKeyUp(KeyEventArgs e) {
			DiagramController.ProcessKeyUp(e.GetPlatformKey(), Diagram.CreatePlatformMouseArgs());
		}
		protected internal void CaptureMouse() {
			Diagram.Capture = true;
		}
		protected internal void ReleaseMouse() {
			Diagram.Capture = false;
		}
		public virtual void OnToolboxItemDoubleClick(IToolboxItem item) {
			Diagram.CreateShape(item.GetShape());
		}
		public virtual void OnToolboxDragItemStart(ToolboxDragItemStartEventArgs e) {
			ShapeDescription kind = e.Item.GetShape();
			Diagram.Commands().Execute(DiagramCommands.StartDragToolCommand, null, new ShapeTool(kind));
			e.Cancel = true;
		}
		public virtual void OnToolboxGetItemImage(ToolboxGetItemImageEventArgs e) {
			ShapeDescription kind = e.Item.GetShape();
			if(kind != null) {
				e.Image = ToolboxUtils.CreateToolboxItemGlyph(kind, e.BestImageSize);
			}
		}
		protected DiagramController DiagramController { get { return Diagram.Controller; } }
		#region Dispose
		public void Dispose() {
			Dispose(true);
		} 
		#endregion
		protected virtual void Dispose(bool disposing) {
			this.diagram = null;
		}
		public DiagramControl Diagram { get { return diagram; } }
	}
}
