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

using DevExpress.DashboardWin.Native;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDashboardLayout;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.ServiceModel.Design {
	public class DashboardViewerSelectionBehaviorDesignTime : IDisposable {
		const int SelectionBorderSize = 3;
		readonly DashboardViewer viewer;
		readonly IServiceProvider serviceProvider;
		readonly DashboardBorderPainter painter;
		MouseLocation oldMouseLocation = MouseLocation.Undefined;
		MouseLocation mouseLocation = MouseLocation.Undefined;
		IDashboardOwnerService OwnerService { get { return serviceProvider.RequestServiceStrictly<IDashboardOwnerService>(); } }
		IDashboardViewerInvalidateService InvalidateService { get { return serviceProvider.RequestServiceStrictly<IDashboardViewerInvalidateService>(); } }
		IDashboardGuiContextService GuiContext { get { return serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>(); } }		
		IDashboardSelectionService DashboadSelectionService { get { return serviceProvider.RequestServiceStrictly<IDashboardSelectionService>(); } }
		bool IsDashboardEmpty { get { return OwnerService.IsDashboardEmpty; } }
		bool IsDashboardSelected { get { return DashboadSelectionService.IsDashboardSelected; } }
		DashboardLayoutControl LayoutControl { get { return viewer.LayoutControl; } }
		DashboardTitleControl Title { get { return viewer.Title; } }
		MouseLocation MouseLocation {
			get { return mouseLocation; }
			set {
				if (value != mouseLocation) {
					oldMouseLocation = mouseLocation;
					mouseLocation = value;
					bool shouldInvalidate = false;
					switch (mouseLocation) {
						case MouseLocation.Layout:
							shouldInvalidate = !IsDashboardSelected && IsDashboardEmpty;
							break;
						case MouseLocation.Title:
							shouldInvalidate = !IsDashboardSelected && !IsDashboardEmpty;
							break;
						case MouseLocation.Undefined:
							shouldInvalidate = oldMouseLocation == MouseLocation.Title && !IsDashboardSelected;
							break;
					}
					if(shouldInvalidate)
						InvalidateService.InvalidateViewer();
				}
			}
		}
		public DashboardViewerSelectionBehaviorDesignTime(DashboardViewer viewer, IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(viewer, "viewer");
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.viewer = viewer;
			this.serviceProvider = serviceProvider;
			painter = new DashboardBorderPainter(GuiContext.LookAndFeel);
			viewer.Paint += OnViewerPaint;
			LayoutControl.MouseEnter += OnLayoutControlMouseEnter;
			LayoutControl.MouseLeave += OnLayoutControlMouseLeave;
			LayoutControl.MouseDown += OnLayoutControlMouseDown;
			Title.MouseEnter += OnTitleMouseEnter;
			Title.MouseLeave += OnTitleMouseLeave;
			Title.MouseDown += OnTitleMouseDown;
		}
		public void Dispose() {
			viewer.Paint -= OnViewerPaint;
			LayoutControl.MouseEnter -= OnLayoutControlMouseEnter;
			LayoutControl.MouseLeave -= OnLayoutControlMouseLeave;
			LayoutControl.MouseDown -= OnLayoutControlMouseDown;
			Title.MouseEnter -= OnTitleMouseEnter;
			Title.MouseLeave -= OnTitleMouseLeave;
			Title.MouseDown -= OnTitleMouseDown;
		}
		void OnViewerPaint(object sender, PaintEventArgs e) {
			if (IsDashboardSelected)
				DrawBorder(e, ObjectState.Selected);
			else if (MouseLocation == MouseLocation.Title || (MouseLocation == MouseLocation.Layout && IsDashboardEmpty))
				DrawBorder(e, ObjectState.Hot);
		}
		void OnLayoutControlMouseEnter(object sender, EventArgs e) {
			MouseLocation = MouseLocation.Layout;
		}
		void OnLayoutControlMouseLeave(object sender, EventArgs e) {
			MouseLocation = MouseLocation.Undefined;
		}
		void OnTitleMouseEnter(object sender, EventArgs e) {
			MouseLocation = MouseLocation.Title;
		}
		void OnTitleMouseLeave(object sender, EventArgs e) {
			MouseLocation = MouseLocation.Undefined;
		}
		void OnLayoutControlMouseDown(object sender, MouseEventArgs e) {
			if (IsDashboardEmpty)
				SelectDashboard();				
		}
		void OnTitleMouseDown(object sender, MouseEventArgs e) {
			SelectDashboard();				
		}
		void SelectDashboard() {
			DashboadSelectionService.SelectDashboard();
		}
		void DrawBorder(PaintEventArgs e, ObjectState state) {
			Rectangle paintRect = viewer.ClientRectangle;
			Padding viewerPadding = viewer.Padding;
			paintRect = new Rectangle(
				paintRect.Left + viewerPadding.Left,
				paintRect.Top + viewerPadding.Top,
				paintRect.Width - viewerPadding.Horizontal,
				paintRect.Height - viewerPadding.Vertical);
			paintRect.Inflate(SelectionBorderSize, SelectionBorderSize);
			ObjectInfoArgs oia = new ObjectInfoArgs();
			oia.Graphics = e.Graphics;
			oia.Bounds = paintRect;
			oia.State = state;
			painter.DrawObject(oia);
		}
	}
	public enum MouseLocation {
		Undefined,
		Layout,
		Title
	}
}
