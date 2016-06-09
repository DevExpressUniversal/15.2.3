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
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Drawing.Design;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design;
using System.ComponentModel;
using DevExpress.XtraReports.Design.Adapters;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Design.MouseTargets {
	public class MouseTargetBase : ControlAdapterBase, IMouseTarget, IDisposable {
		bool selectedOnMouseDown = true;
		public MouseTargetBase(XRControl xrControl, IServiceProvider servProvider)
			: base(xrControl, servProvider) {
		}
		public virtual void Dispose() { 
		}
		public virtual void HandleDoubleClick(object sender, BandMouseEventArgs e) {
		}
		public virtual void HandleMouseMove(object sender, BandMouseEventArgs e) {
			((Control)sender).Cursor = GetCursor(new Point(e.X, e.Y));
		}
		protected virtual Cursor GetCursor(Point pt) {
			IToolboxService tbxService = GetToolboxService();
			if(tbxService != null && tbxService.SetCursor())
				return Cursor.Current;
			if(XRControl.CanHaveChildren)
				return Cursors.Default;
			return Cursors.SizeAll;
		}
		protected ToolboxItem GetSelectedToolboxItem() {
			IToolboxService tbxService = GetToolboxService();
			return (tbxService != null) ? tbxService.GetSelectedToolboxItem(Host) : null;
		}
		protected IToolboxService GetToolboxService() {
			return GetService(typeof(IToolboxService)) as IToolboxService;
		}
		public virtual void HandleMouseDown(object sender, BandMouseEventArgs e) {
			Point mousePosition = Control.MousePosition;
			bool needsSelectionChangeOnMouseUp = (Control.ModifierKeys & Keys.Control) == Keys.Control && IsXRControlSelected(XRControl);
			if(needsSelectionChangeOnMouseUp) {
				selectedOnMouseDown = false;
			} else {
				Designer.SelectComponent();
				selectedOnMouseDown = true;
			}
			if(!e.Button.IsLeft() || Control.ModifierKeys != Keys.None)
				return;
			ToolboxItem tbxItem = GetSelectedToolboxItem();
			CapturePaintService captPaintSvc = (CapturePaintService)GetService(typeof(CapturePaintService));
			if(tbxItem != null) {
				captPaintSvc.Start(BandViewSvc.View, new BoundsDelegate(CommitToolPicked), mousePosition);
			} else if(XRControlDesignerBase.CursorEquals(Cursors.Default) && XRControl.CanHaveChildren) {
				captPaintSvc.Start(BandViewSvc.View, new BoundsDelegate(CommitSelection), mousePosition);
			}
			DrawControlShadow();
		}
		bool IsXRControlSelected(IComponent component) {
			ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));
			if(selectionService.GetComponentSelected(component))
				return true;
			WinControlContainer wcc = component as WinControlContainer;
			if(wcc == null || wcc.WinControl == null)
				return false;
			return selectionService.GetComponentSelected(wcc.WinControl);
		}
		void CommitToolPicked(RectangleF bounds) {
			ToolboxItem tbxItem = GetSelectedToolboxItem();
			if(tbxItem != null && ReportDesigner != null)
				ReportDesigner.ToolPicked(tbxItem);
		}
		private void CommitSelection(RectangleF bounds) {
			CommitSelection(bounds, XRControl);
		}
		public void CommitSelection(RectangleF bounds, IComponent defaultSelection) {
			if(bounds.Width < SystemInformation.DragSize.Width && bounds.Height < SystemInformation.DragSize.Height)
				return;
			ArrayList components = new ArrayList();
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			PrepareSelectedComponentsList(components, (XtraReportBase)host.RootComponent, bounds);
			if(components.Count == 0 && defaultSelection != null)
				components.Add(defaultSelection);
			Designer.SelectComponents(components, SelectionTypes.Auto);
		}
		protected virtual void PrepareSelectedComponentsList(ArrayList components, XtraReportBase report, RectangleF bounds) {
			foreach(XRControl control in XRControl.Controls) {
				RectangleF r = BandViewSvc.GetControlScreenBounds(control);
				if(!components.Contains(control) && !r.Contains(bounds) && r.IntersectsWith(bounds))
					components.Add(control);
			}
		}
		public virtual void HandleMouseLeave(object sender, EventArgs e) {
		}
		void DrawControlShadow() {
			if(XRControl is Band)
				return;
			RulerService rulerSvc = (RulerService)GetService(typeof(RulerService));
			rulerSvc.DrawShadows(new RectangleF[] { BandViewSvc.GetControlScreenBounds(XRControl) }, XRControl.Band.NestedLevel);
		}
		public virtual void HandleMouseUp(object sender, BandMouseEventArgs e) {
			if(!selectedOnMouseDown) {
				Designer.SelectComponent();
				selectedOnMouseDown = true;
			}
			RulerService rulerSvc = (RulerService)GetService(typeof(RulerService));
			rulerSvc.HideShadows();
		}
		public virtual bool IsDisposed {
			get { return XRControl == null || XRControl.Site == null; }
		}
		public virtual bool ContainsPoint(Point pt, BandViewInfo viewInfo) {
			return false;
		}
	}
}
