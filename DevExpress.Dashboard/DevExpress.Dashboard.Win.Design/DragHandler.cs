#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardWin.ServiceModel.Design;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Handlers;
using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Design {
	public class DragHandlerBase : IDragHandler {
		public static ToolboxItem GetToolboxItem(IDataObject data, IDesignerHost host) {
			try {
				IToolboxService toolboxService = host.GetService(typeof(IToolboxService)) as IToolboxService;
				return (toolboxService != null) ? toolboxService.DeserializeToolboxItem(data, host) : null;
			} catch {
				return null;
			}
		}
		DashboardComponentDesigner designer;
		IDesignerHost host;
		protected DashboardComponentDesigner Designer {
			get {
				if(designer == null) {
					designer = (DashboardComponentDesigner)host.GetDesigner(host.RootComponent);
				}
				return designer;
			}
		}
		public DragHandlerBase(IDesignerHost host) {
			this.host = host;
		}
		public virtual void HandleDragEnter(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.None;
		}
		public virtual void HandleDragOver(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.None;
		}
		public virtual void HandleDragDrop(object sender, DragEventArgs e) {
			if (Designer.Designer != null && !Designer.Designer.IsDisposed)
				Designer.Designer.Focus();
		}
		public virtual void HandleDragLeave(object sender, EventArgs e) {
		}
		protected ToolboxItem GetToolboxItem(IDataObject data) {
			return GetToolboxItem(data, this.host);
		}
	}
	public class ToolboxItemDragHandler : DragHandlerBase {
		const string DashboardItemGroupTypeName = "DevExpress.DashboardWin.Design.Group";
		ToolboxItem toolboxItem;
		DashboardLayoutControlDragDropHelper helper;
		public virtual DashboardLayoutControlDragDropHelper DragHelper {
			get {
				if(helper == null) {
					helper = ((ILayoutDesignerMethods)Designer.LayoutControl).DragDropDispatcherClientHelper as DashboardLayoutControlDragDropHelper;
				}
				return helper;
			}
		}
		public ToolboxItemDragHandler(IDesignerHost host)
			: base(host) {
		}
		protected bool CanHandleDragging() {
			return toolboxItem != null;
		}
		public override void HandleDragEnter(object sender, DragEventArgs e) {
			toolboxItem = GetToolboxItem(e.Data);
			if(CanHandleDragging()) {
				BaseLayoutItem item = null;
				if(toolboxItem.TypeName == DashboardItemGroupTypeName)
					item = new DashboardLayoutControlGroupBase();
				else
					item = new LayoutControlItem();
				item.Tag = toolboxItem.Bitmap;
				DragDropDispatcherFactory.Default.DragItem = item;
				DragHelper.DoBeginDrag();
			}
			base.HandleDragEnter(sender, e);
			UpdateDragEffect(e);
		}
		public override void HandleDragOver(object sender, DragEventArgs e) {
			Point p = GetPoint(e);
			if(CanHandleDragging()) {
				DragHelper.UpdateForDragging(p);
				DragHelper.DoDragging(p);
			}
			base.HandleDragOver(sender, e);
			UpdateDragEffect(e);
		}
		public override void HandleDragLeave(object sender, EventArgs e) {
			if(CanHandleDragging()) {
				DragHelper.DoDragCancel();
				toolboxItem = null;
			}
			base.HandleDragLeave(sender, e);
		}
		Point GetPoint(DragEventArgs de) {
			Point p = Point.Empty;
			if(de != null) {
				p = new Point(de.X, de.Y);
				p = Designer.LayoutControl.PointToClient(p);
			}
			return p;
		}
		public override void HandleDragDrop(object sender, DragEventArgs e) {
			base.HandleDragDrop(sender, e);
			if(CanHandleDragging()) Designer.ToolPicked(toolboxItem, DragHelper);
			toolboxItem = null;
		}
		void UpdateDragEffect(DragEventArgs e) {
			e.Effect = (CanHandleDragging()) ? DragDropEffects.Copy : DragDropEffects.None;
		}
	}
}
