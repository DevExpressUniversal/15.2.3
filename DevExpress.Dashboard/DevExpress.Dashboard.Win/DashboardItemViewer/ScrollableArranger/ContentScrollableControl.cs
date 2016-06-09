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

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Security;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.DashboardCommon.Service;
using DevExpress.Utils.Gesture;
using System.Collections.Generic;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class ContentScrollableControl : XtraUserControl, IGestureClient {
		const int RoundedBorderRadius = 5;
		static void DrawItemBounds(Graphics graph, Brush brush, ISizable item, bool drawBorder) {
			Rectangle bounds = new Rectangle(item.Left, item.Top, item.Width, item.Height);
			GraphicsPath path = GetRoundedRect(bounds, RoundedBorderRadius);
			SmoothingMode smoothingMode = graph.SmoothingMode;
			graph.SmoothingMode = SmoothingMode.HighQuality;
			try {
				graph.FillPath(brush, path);
				if(drawBorder) {
					using (Pen pen = new Pen(brush))
						graph.DrawPath(pen, path);
				}
			}
			finally {
				graph.SmoothingMode = smoothingMode;
			}
		}
		static GraphicsPath GetRoundedRect(Rectangle rect, int radius) {
			GraphicsPath path = new GraphicsPath();
			path.StartFigure();
			if(radius <= 0) {
				path.AddRectangle(rect);
			}
			else {
				int diameter = 2 * radius;
				Rectangle arcRect = new Rectangle(rect.Left, rect.Top, diameter, diameter);
				path.AddArc(arcRect, 180, 90);
				arcRect.X = rect.Right - diameter;
				path.AddArc(arcRect, 270, 90);
				arcRect.Y = rect.Bottom - diameter;
				path.AddArc(arcRect, 0, 90);
				arcRect.X = rect.Left;
				path.AddArc(arcRect, 90, 90);
			}
			path.CloseFigure();
			return path;
		}
		readonly IWinContentProvider contentProvider;
		readonly ContentScrollableControlModel model;
		GestureHelper touchHelper;
		DevExpress.XtraEditors.HScrollBar hScrollBar;
		DevExpress.XtraEditors.VScrollBar vScrollBar;
		Brush scrollFillBrush = new SolidBrush(SystemColors.Control);
		Brush selectedItemBrush = new SolidBrush(SystemColors.ButtonShadow);
		Brush highlightedItemBrush = new SolidBrush(SystemColors.ButtonHighlight);
		Point touchOverpan = Point.Empty;
		IList<ISizable> highlightedItems;
		IList<ISizable> selectedItems;
		public ContentScrollableControlModel Model {
			get { return model; }
		}
		public int ScrollHeight { get { return hScrollBar.Height; } }
		public int ScrollWidth { get { return vScrollBar.Width; } }
		public ContentScrollableControl(IWinContentProvider contentProvider) {
			touchHelper = new GestureHelper(this);
			this.contentProvider = contentProvider;
			this.contentProvider.Painted += OnContentPainted;
			this.contentProvider.ContentControl.MouseClick += OnContentMouseClick;
			this.contentProvider.ContentControl.MouseDoubleClick += OnContentMouseDoubleClick;
			this.contentProvider.ContentControl.MouseDown += OnContentMouseDown;
			this.contentProvider.ContentControl.MouseUp += OnContentMouseUp;
			this.contentProvider.ContentControl.MouseMove += OnContentMouseMove;
			this.contentProvider.ContentControl.MouseLeave += OnContentMouseLeave;
			this.contentProvider.ContentControl.Parent = this;
			this.contentProvider.ContentControl.MouseWheel += OnContentMouseWheel;
			this.contentProvider.ContentControl.KeyDown += OnContentKeyDown;
			this.contentProvider.ContentControl.KeyUp += OnContentKeyUp;
			this.contentProvider.ContentControl.MouseEnter += OnContentMouseEnter;
			this.contentProvider.ContentControl.MouseHover += OnContentMouseHover;
			DashboardWinHelper.SetParentLookAndFeel(this.contentProvider.ContentControl, LookAndFeel);
			AutoScroll = false;
			model = new ContentScrollableControlModel(contentProvider.ContentProvider);
			model.ContentChanged += OnContentChanged;
			model.RequestClientSize += (s, e) => {
				e.ClientSize = ClientSize;
			};
			InitializeScrollBars();
			SetLookAndFeel();
			LookAndFeel.StyleChanged += (sender, e) => {
				SetLookAndFeel();
			};
			SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
			BorderStyle = BorderStyle.None;
		}
		public void ClearSelection() {
			if(selectedItems != null)
				selectedItems.Clear();
			InvalidateContent();
		}
		public IValuesProvider GetHitItem(Point location) {
			return model.GetHitItem(location);
		}
		public void SelectValues(IList values) {
			selectedItems = model.GetItemsByValues(values);
			InvalidateContent();
		}
		public void HighlightValues(IList values) {
			highlightedItems = model.GetItemsByValues(values);
			InvalidateContent();
		}
		public void PrepareScrollingState(ItemViewerClientState state) {
			if(hScrollBar.Enabled && hScrollBar.ActualVisible)
				state.HScrollingState = new ScrollingState { PositionRatio = model.HPositionRatio, VirtualSize = model.VirtualSize.Width, ScrollBarSize = model.HScrollSize };
			if(vScrollBar.Enabled && vScrollBar.ActualVisible)
				state.VScrollingState = new ScrollingState { PositionRatio = model.VPositionRatio, VirtualSize = model.VirtualSize.Height, ScrollBarSize = model.VScrollSize };
		}
		void SetLookAndFeel() {
			Skin skin = CommonSkins.GetSkin(LookAndFeel);
			Color selectedColor = skin[CommonSkins.SkinSelection].Border.GetLeft();
			Color highlightedColor = skin[CommonSkins.SkinSelection].Color.GetBackColor();
			selectedItemBrush = new SolidBrush(selectedColor);
			highlightedItemBrush = new SolidBrush(highlightedColor);
			scrollFillBrush = new SolidBrush(skin.Colors.GetColor(CommonColors.Control));
			vScrollBar.LookAndFeel.Assign(LookAndFeel);
			hScrollBar.LookAndFeel.Assign(LookAndFeel);
			ScrollBarBase.ApplyUIMode(hScrollBar);
			ScrollBarBase.ApplyUIMode(vScrollBar);
		}
		void InitializeScrollBars() {
			hScrollBar = new DevExpress.XtraEditors.HScrollBar();
			hScrollBar.Scroll += OnHScroll;
			Controls.Add(hScrollBar);
			vScrollBar = new DevExpress.XtraEditors.VScrollBar();
			vScrollBar.Scroll += OnVScroll;
			Controls.Add(vScrollBar);
			model.InitializeScrollBars(new ScrollBarWrapper(hScrollBar), new ScrollBarWrapper(vScrollBar));
		}
		void ChangeHScrollOffset(int offset) {
			if (model.ScrollMode == ScrollMode.Both || model.ScrollMode == ScrollMode.Horizontal) {
				int newValue = model.OffsetX - offset;
				if (newValue > hScrollBar.Maximum - hScrollBar.LargeChange)
					newValue = hScrollBar.Maximum - hScrollBar.LargeChange;
				if (newValue < 0)
					newValue = 0;
				model.OffsetX = newValue;
				hScrollBar.Value = newValue;
			}
		}
		void ChangeVScrollOffset(int offset) {
			if (model.ScrollMode == ScrollMode.Both || model.ScrollMode == ScrollMode.Vertical) {
				int newValue = model.OffsetY - offset;
				if (newValue > vScrollBar.Maximum - vScrollBar.LargeChange)
					newValue = vScrollBar.Maximum - vScrollBar.LargeChange;
				if (newValue < 0)
					newValue = 0;
				model.OffsetY = newValue;
				vScrollBar.Value = newValue;
			}
		}
		void OnHScroll(object sender, ScrollEventArgs e) {
			model.OffsetX = e.NewValue;
			hScrollBar.Value = e.NewValue;
		}
		void OnVScroll(object sender, ScrollEventArgs e) {
			model.OffsetY = e.NewValue;
			vScrollBar.Value = e.NewValue;
		}
		void OnContentChanged(object sender, EventArgs e) {
			highlightedItems = null;
			InvalidateContent();
		}
		void OnContentPainted(object sender, PaintEventArgs e) {
			Graphics graph = e.Graphics;
			if(highlightedItems != null)
				foreach (ISizable  highlightedItem in highlightedItems)
					DrawItemBounds(graph, highlightedItemBrush, highlightedItem, true);
			if(selectedItems != null) {
				foreach(ISizable item in selectedItems)
					DrawItemBounds(graph, selectedItemBrush, item, true);
			}
		}
		void OnContentMouseMove(object sender, MouseEventArgs e) {
			OnMouseMove(e);
		}
		void OnContentMouseClick(object sender, MouseEventArgs e) {
			OnMouseClick(e);
		}
		void OnContentMouseDoubleClick(object sender, MouseEventArgs e) {
			OnMouseDoubleClick(e);
		}
		void OnContentMouseDown(object sender, MouseEventArgs e) {
			OnMouseDown(e);
		}
		void OnContentMouseUp(object sender, MouseEventArgs e) {
			OnMouseUp(e);
		}
		void OnContentMouseLeave(object sender, EventArgs e) {
			OnMouseLeave(e);
		}
		void OnContentMouseWheel(object sender, MouseEventArgs e) {
			ChangeVScrollOffset((int)e.Delta);
			OnMouseWheel(e);
		}
		void OnContentMouseEnter(object sender, EventArgs e) {
			OnMouseEnter(e);
		}
		void OnContentMouseHover(object sender, EventArgs e) {
			OnMouseHover(e);
		}
		void OnContentKeyDown(object sender, KeyEventArgs e) {
			OnKeyDown(e);
		}
		void OnContentKeyUp(object sender, KeyEventArgs e) {
			OnKeyUp(e);
		}
		#region ITouchClient Members
		void IGestureClient.OnTwoFingerTap(GestureArgs info) { }
		void IGestureClient.OnPressAndTap(GestureArgs info) { }
		IntPtr IGestureClient.Handle { get { return Handle; } }
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); } }
		Point IGestureClient.PointToClient(Point p) { return PointToClient(p); }
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) { }
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) { }
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) { }
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(info.GF == GF.BEGIN) {
				touchOverpan = Point.Empty;
				return;
			}
			if(delta.Y != 0) {
				int previousValue = vScrollBar.Value;
				ChangeVScrollOffset(delta.Y);
				if(previousValue == vScrollBar.Value)
					touchOverpan.Y += delta.Y;
			}
			if(delta.X != 0) {
				int previousValue = hScrollBar.Value;
				ChangeHScrollOffset(delta.X);
				if(previousValue == hScrollBar.Value)
					touchOverpan.X += delta.X;
			}
			overPan = touchOverpan;
		}
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			return Bounds.Contains(point) ? new GestureAllowArgs[] { GestureAllowArgs.Pan } : GestureAllowArgs.None;
		}
		#endregion
#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123")]
#endif
		[SecuritySafeCritical]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			switch(keyData){
				case Keys.Up:
					ChangeVScrollOffset(vScrollBar.SmallChange);
					return true;
				case Keys.Down:
					ChangeVScrollOffset(-vScrollBar.SmallChange);
					return true;
				case Keys.Left:
					ChangeHScrollOffset(hScrollBar.SmallChange);
					return true;
				case  Keys.Right:
					ChangeHScrollOffset(-hScrollBar.SmallChange);
					return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			hScrollBar.OnAction(ScrollNotifyAction.MouseMove);
			vScrollBar.OnAction(ScrollNotifyAction.MouseMove);
		}
		protected override void OnMouseClick(MouseEventArgs e) {
			InvalidateContent();
			base.OnMouseClick(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if(highlightedItems != null) {
				highlightedItems = null;
				InvalidateContent();
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(!vScrollBar.TouchMode && !hScrollBar.TouchMode &&  model.ScrollMode == ScrollMode.Both)
				e.Graphics.FillRectangle(scrollFillBrush, ClientSize.Width - vScrollBar.Width, ClientSize.Height - hScrollBar.Height, vScrollBar.Width, hScrollBar.Height);
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			model.Arrange();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				scrollFillBrush.Dispose();
				selectedItemBrush.Dispose();
				highlightedItemBrush.Dispose();
			}
			contentProvider.ContentProvider.Changed -= OnContentChanged;
			contentProvider.Painted -= OnContentPainted;
			contentProvider.ContentControl.MouseClick -= OnContentMouseClick;
			contentProvider.ContentControl.MouseDoubleClick -= OnContentMouseDoubleClick;
			contentProvider.ContentControl.MouseDown -= OnContentMouseDown;
			contentProvider.ContentControl.MouseUp -= OnContentMouseUp;
			contentProvider.ContentControl.MouseMove -= OnContentMouseMove;
			contentProvider.ContentControl.MouseLeave -= OnContentMouseLeave;
			contentProvider.ContentControl.MouseEnter -= OnContentMouseEnter;
			contentProvider.ContentControl.MouseHover -= OnContentMouseHover;
			contentProvider.ContentControl.MouseWheel -= OnContentMouseWheel;
		}
		protected override void WndProc(ref Message m) {
			if(touchHelper.WndProc(ref m)) return;
			base.WndProc(ref m);
		}
		public void InvalidateContent() {
			contentProvider.ContentControl.Invalidate();
		}
	}
}
