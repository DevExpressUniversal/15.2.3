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
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.LookAndFeel;
using DevExpress.Utils.DragDrop;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.Utils.Mdi;
namespace DevExpress.XtraTabbedMdi {
	public enum FloatPageDragMode { 
		Default, Preview, FullWindow 
	}
	public class DropHint : BaseDragHelperForm {
		XtraTabbedMdiManager managerCore;
		Brush borderBrush;
		Brush brush;
		public DropHint(XtraTabbedMdiManager manager)
			: base(10000, 100) {
			DoubleBuffered = true;
			Opacity = 0.5;
			managerCore = manager;
			borderBrush = new SolidBrush(Color.FromArgb(0xff, 0x57, 0x79, 0xAD));
			brush = new SolidBrush(Color.FromArgb(0xff, 0xA5, 0xC2, 0xE4));
		}
		protected bool IsDisposing;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				IsDisposing = true;
				managerCore = null;
				if(brush != null) {
					brush.Dispose();
					brush = null;
				}
				if(borderBrush != null) {
					borderBrush.Dispose();
					borderBrush = null;
				}
			}
			base.Dispose(disposing);
		}
		public XtraTabbedMdiManager Manager {
			get { return managerCore; }
		}
		Rectangle Content;
		Rectangle Header;
		public void Docking(Point point) {
			Content = Manager.ViewInfo.PageBounds;
			bool horz = (Manager.HeaderLocation == XtraTab.TabHeaderLocation.Top) || (Manager.HeaderLocation == XtraTab.TabHeaderLocation.Bottom);
			int headerIndex;
			Rectangle header = CalcPageHeaderBounds(horz ? point.X : point.Y, Content, horz, out headerIndex);
			if(Header != header) {
				Manager.floatFormDockingPageIndex = headerIndex;
				Header = header;
				Invalidate();
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(IsDisposing) return;
			e.Graphics.FillRectangle(borderBrush, Content);
			e.Graphics.FillRectangle(borderBrush, Header);
			e.Graphics.FillRectangle(brush, Rectangle.Inflate(Content, -4, -4));
			if(!Header.IsEmpty)
				e.Graphics.FillRectangle(brush, CalcPageHeaderFillRect(Header));
		}
		Rectangle CalcPageHeaderFillRect(Rectangle header) {
			switch(Manager.HeaderLocation) {
				case XtraTab.TabHeaderLocation.Left:
					return new Rectangle(header.Left + 4, header.Top + 4, header.Width, header.Height - 8);
				case XtraTab.TabHeaderLocation.Right:
					return new Rectangle(header.Left - 4, header.Top + 4, header.Width, header.Height - 8);
				case XtraTab.TabHeaderLocation.Bottom:
					return new Rectangle(header.Left + 4, header.Top - 4, header.Width - 8, header.Height);
				default:
					return new Rectangle(header.Left + 4, header.Top + 4, header.Width - 8, header.Height);
			}
		}
		Rectangle CalcPageHeaderBounds(int pos, Rectangle pageBounds, bool horz, out int pageIndex) {
			pageIndex = -1;
			if(Manager.ViewInfo.HeaderInfo.VisiblePages.Count == 0)
				return Rectangle.Empty;
			Rectangle header = (Manager.ViewInfo.SelectedTabPage != null) ?
				Manager.ViewInfo.SelectedTabPageViewInfo.Bounds : Rectangle.Empty;
			foreach(BaseTabPageViewInfo pInfo in Manager.ViewInfo.HeaderInfo.VisiblePages) {
				header = pInfo.Bounds;
				if(BaseTabHeaderViewInfo.HitTestHeader(header, pos, horz)) {
					pageIndex = Manager.ViewInfo.HeaderInfo.AllPages.IndexOf(pInfo);
					return header;
				}
			}
			pageIndex = Manager.ViewInfo.HeaderInfo.AllPages.Count;
			pos = BaseTabHeaderViewInfo.CorrectPos(Manager.IsRightToLeftLayout(), pageBounds, header, horz);
			return new Rectangle(horz ? pos : header.Left, horz ? header.Top : pos, header.Width, header.Height);
		}
	}
	public class DragFrame : BaseDragHelperForm {
		Form targetCore;
		Image previewCore;
		XtraTabbedMdiManager managerCore;
		public XtraTabbedMdiManager Manager {
			get { return managerCore; }
		}
		public DragFrame(XtraTabbedMdiManager manager)
			: base(10000, 100) {
			Opacity = 0.75;
			KeyPreview = true;
			managerCore = manager;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(previewCore != null) {
					previewCore.Dispose();
					previewCore = null;
				}
				targetCore = null;
				managerCore = null;
			}
			base.Dispose(disposing);
		}
		public Form Target {
			get { return targetCore; }
			set {
				if(targetCore == value) return;
				targetCore = value;
				OnTargetChanged();
			}
		}
		ObjectPainter painter;
		protected void OnTargetChanged() {
			Rectangle bounds = Manager.ViewInfo.Bounds;
			painter = GetBackgroundPainter();
			previewCore = new Bitmap(bounds.Width, bounds.Height);
			using(Graphics g = Graphics.FromImage(Preview)) {
				using(GraphicsCache cache = new GraphicsCache(g)) {
					DrawTab(cache, bounds);
					DrawContent(g);
				}
			}
		}
		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e) {
			base.OnPreviewKeyDown(e);
			if(e.KeyCode == Keys.Escape)
				Manager.CancelFloating(true);
		}
		protected void DrawContent(Graphics g) {
			using(Image formPreview = MDIChildPreview.Create(Target, Color.Empty)) {
				g.DrawImage(formPreview, Manager.ViewInfo.PageClientBounds);
			}
		}
		protected void DrawTab(GraphicsCache cache, Rectangle bounds) {
			TabDrawArgs args = new TabDrawArgs(cache, Manager.ViewInfo, bounds);
			AllowDrawPage(false, Target);
			Manager.Painter.Draw(args);
			AllowDrawPage(true, Target);
		}
		protected virtual void DrawBackground(GraphicsCache cache, Rectangle bounds) {
			ObjectInfoArgs info = new HeaderObjectInfoArgs();
			info.Bounds = bounds;
			info.Cache = cache;
			painter.DrawObject(info);
		}
		ObjectPainter GetBackgroundPainter() {
			UserLookAndFeel lf = Manager.ViewInfo.TabControl.LookAndFeel;
			BaseLookAndFeelPainters painters = DevExpress.LookAndFeel.LookAndFeelPainterHelper.GetPainter(lf, lf.ActiveStyle);
			return painters.Header;
		}
		void AllowDrawPage(bool allow, Form exclude) {
			foreach(BaseTabPageViewInfo pageInfo in Manager.ViewInfo.HeaderInfo.VisiblePages) {
				if(((XtraMdiTabPage)pageInfo.Page).MdiChild != exclude)
					pageInfo.AllowDraw = allow;
			}
		}
		public Image Preview {
			get { return previewCore; }
		}
		protected override void OnPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				DrawBackground(cache, ClientRectangle);
				e.Graphics.DrawImage(Preview, Rectangle.Inflate(ClientRectangle, -3, -4));
			}
		}
	}
}
