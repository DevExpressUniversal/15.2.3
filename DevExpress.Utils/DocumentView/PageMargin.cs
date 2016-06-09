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
using System.Collections;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.DocumentView.Controls;
namespace DevExpress.DocumentView
{
	public abstract class PageMargin {
		#region static
		public static string GetMarginSideDesc(float val, bool isMetric, float fromDpi) {
			return isMetric ?
				String.Format("{0} {1}",
					(int)GraphicsUnitConverter.Convert(val, fromDpi, GraphicsDpi.Millimeter),
					PreviewLocalizer.GetString(PreviewStringId.Margin_Millimeter)) :
				String.Format("{0:0.00} ''",
					GraphicsUnitConverter.Convert(val, fromDpi, GraphicsDpi.Inch));
		}
		#endregion
		protected DocumentViewerBase pc;
		private RectangleF marginRect = RectangleF.Empty;
		protected PreviewStringId resStringId;
		protected IPage Page { get { return pc.SelectedPage; }
		}
		public virtual MarginSide Side { get { return MarginSide.None; } 
		}
		public bool IsResizing { get { return (marginRect.IsEmpty == false); }
		}
		protected PageMargin(DocumentViewerBase pc) {
			this.pc = pc;
		}
		protected abstract void GetMarginLine(out PointF start, out PointF end, RectangleF marRect, RectangleF pageRect);
		protected abstract void UpdatePageSettings(float val);
		protected abstract float GetValue(PointF pt, RectangleF pageRect);
		protected bool IsHorizSide(MarginSide side) {
			return (side == MarginSide.Top || side == MarginSide.Bottom);
		}
		public void Draw(Graphics graph) {
			PointF start;
			PointF end;
			Pen pen = pc.Gdi.GetPen(Color.FromArgb(90, 0, 0, 0), 1);
			pen.DashStyle = DashStyle.Dash;
			if(GetMarginLine(out start, out end))
				DrawCore(graph, pen, start, end);
		}
		protected virtual void DrawCore(Graphics graph, Pen pen, PointF start, PointF end) {
			graph.DrawLine(pen, start, end);
		}
		public bool GetMarginLine(out PointF start, out PointF end) {
			start = PointF.Empty;
			end = PointF.Empty;
			RectangleF pageRect = pc.ViewManager.GetPageRect(Page);
			if(pageRect.IsEmpty) return false; 
			RectangleF rect = Page.ApplyMargins(pageRect);
			rect = PSUnitConverter.DocToPixel(rect, pc.Zoom, pc.ViewManager.ScrollPos);
			pageRect = PSUnitConverter.DocToPixel(pageRect, pc.Zoom, pc.ViewManager.ScrollPos);
			GetMarginLine(out start, out end, rect, pageRect);
			return true;
		}
		public MarginSide GetPointSide(PointF pt) {
			PointF start;
			PointF end;
			if( GetMarginLine(out start, out end) )
				if( LineContains(start, end, pt) ) return Side;
			return MarginSide.None;
		}
		public void DrawSide(PointF pt) {
			PointF start;
			PointF end;
			if(pt.IsEmpty) return;
			if( GetMarginLine(out start, out end) ) {
				Point pt1 = OffsetPoint(start, pt, Side);
				Point pt2 = OffsetPoint(end, pt, Side);
				RectangleF pageRect = pc.ViewManager.GetPageRect(Page);
				pageRect = PSUnitConverter.DocToPixel(pageRect, pc.Zoom, pc.ViewManager.ScrollPos);
				pt1 = Point.Round( RestrictPoint(pt1, pageRect) );
				pt2 = Point.Round( RestrictPoint(pt2, pageRect) );
				RectangleF rect = RectFromLine(pt1, pt2);
				if( IsHorizSide(Side) ) rect.Height = 1;
				else rect.Width = 1;
				SetMarginLayout(rect);
			}
		}
		private void SetMarginLayout(RectangleF r) {
			if( r.Equals(marginRect) ) return;
			pc.ViewControl.InvalidateRect(RectangleF.Inflate(marginRect,1,1), true);
			marginRect = r;
			Graphics gr = pc.ViewControl.CreateGraphics();
			gr.FillRectangle(SystemBrushes.WindowText, r);
			gr.Dispose();
		}
		public void EndResize(bool applyChanges) {
			if(!marginRect.IsEmpty && applyChanges) {
				pc.ViewControl.InvalidateRect(marginRect, false);
				float val = GetValue();
				marginRect = RectangleF.Empty;
				if( !PSNativeMethods.IsNaN(val) ) 
					UpdatePageSettings(val);
			} else 
				marginRect = RectangleF.Empty;
		}
		private float GetValue() {
			RectangleF pageRect = pc.ViewManager.GetPageRect(Page);
			if( !pageRect.IsEmpty ) {  			
				PointF pt = PSUnitConverter.PixelToDoc(marginRect.Location, pc.Zoom, pc.ViewManager.ScrollPos);
				return GetValue(pt,pageRect);
			}
			return Single.NaN;
		}
		private PointF RestrictPoint(PointF pt, RectangleF r) {
			pt.X = Math.Min(r.Right, Math.Max(pt.X, r.Left));
			pt.Y = Math.Min(r.Bottom, Math.Max(pt.Y, r.Top));
			return pt;
		}
		private Point OffsetPoint(PointF pt1, PointF pt2, MarginSide side) {
			Point pt = Point.Round(pt1);
			if( IsHorizSide(side) ) pt.Offset(0,(int)(pt2.Y - pt1.Y));
			else pt.Offset((int)(pt2.X - pt1.X), 0);
			return pt;
		}
		private bool IsHorizLine(PointF pt1, PointF pt2) {
			return (pt1.Y == pt2.Y);
		}
		private bool LineContains(PointF start, PointF end, PointF pt) {
			RectangleF r = RectFromLine(start, end);
			SizeF sz = IsHorizLine(start,end) ? new SizeF(0,2) : new SizeF(2,0);
			r.Inflate(sz);
			return r.Contains(pt);
		}
		private RectangleF RectFromLine(PointF start, PointF end) {
			return new RectangleF(start, new SizeF(end.X - start.X, end.Y - start.Y));
		}
		public string GetText() {
			return GetText( GetValue() );
		}
		public string GetText(float val) {
			if( PSNativeMethods.IsNaN(val) ) return "";
		return string.Format("{0}: {1}", PreviewLocalizer.GetString(resStringId), GetMarginSideDesc(val, pc.IsMetric, GraphicsDpi.Document));
		}
	}
	public class LeftMargin : PageMargin
	{
		public override MarginSide Side { get { return MarginSide.Left; } 
		}
		public LeftMargin(DocumentViewerBase pc) : base(pc) {
			resStringId = PreviewStringId.Margin_LeftMargin;
		}
		protected override void GetMarginLine(out PointF start, out PointF end, RectangleF marRect, RectangleF pageRect) {
			start = new PointF(marRect.Left, pageRect.Top);
			end = new PointF(marRect.Left, pageRect.Bottom);
		}
		protected override float GetValue(PointF pt, RectangleF pageRect) {
			return pt.X - pageRect.X;
		}
		protected override void UpdatePageSettings(float val) {
			pc.Document.PageSettings.LeftMargin = MarginsF.ToHundredths(val);
		}
		protected override void DrawCore(Graphics graph, Pen pen, PointF start, PointF end) {
			if(start.Y < graph.ClipBounds.Y)
				start.Y = graph.ClipBounds.Top;
			if(end.Y > graph.ClipBounds.Bottom)
				end.Y = graph.ClipBounds.Bottom;
			base.DrawCore(graph, pen, start, end);
		}
	}
	public class RightMargin : PageMargin
	{
		public override MarginSide Side { get { return MarginSide.Right; } 
		}
		public RightMargin(DocumentViewerBase pc) : base(pc) {
			resStringId = PreviewStringId.Margin_RightMargin;
		}
		protected override void GetMarginLine(out PointF start, out PointF end, RectangleF marRect, RectangleF pageRect) {
			start = new PointF(marRect.Right, pageRect.Top);
			end = new PointF(marRect.Right, pageRect.Bottom);
		}
		protected override void UpdatePageSettings(float val) {
			pc.Document.PageSettings.RightMargin = MarginsF.ToHundredths(val);
		}
		protected override float GetValue(PointF pt, RectangleF pageRect) {
			return pageRect.Right - pt.X;
		}
		protected override void DrawCore(Graphics graph, Pen pen, PointF start, PointF end) {
			if(start.Y < graph.ClipBounds.Y)
				start.Y = graph.ClipBounds.Top;
			if(end.Y > graph.ClipBounds.Bottom)
				end.Y = graph.ClipBounds.Bottom;
			base.DrawCore(graph, pen, start, end);
		}
	}
	public class TopMargin : PageMargin
	{
		public override MarginSide Side { get { return MarginSide.Top; } 
		}
		public TopMargin(DocumentViewerBase pc) : base(pc) {
			resStringId = PreviewStringId.Margin_TopMargin;
		}
		protected override void GetMarginLine(out PointF start, out PointF end, RectangleF marRect, RectangleF pageRect) {
			start = new PointF(pageRect.Left, marRect.Top);
			end = new PointF(pageRect.Right, marRect.Top);
		}
		protected override void UpdatePageSettings(float val) {
			pc.Document.PageSettings.TopMargin = MarginsF.ToHundredths(val);
		}
		protected override float GetValue(PointF pt, RectangleF pageRect) {
			return pt.Y - pageRect.Top;
		}
	}
	public class BottomMargin : PageMargin
	{
		public override MarginSide Side { get { return MarginSide.Bottom; } 
		}
		public BottomMargin(DocumentViewerBase pc) : base(pc) {
			resStringId = PreviewStringId.Margin_BottomMargin;
		}
		protected override void GetMarginLine(out PointF start, out PointF end, RectangleF marRect, RectangleF pageRect) {
			start = new PointF(pageRect.Left, marRect.Bottom);
			end = new PointF(pageRect.Right, marRect.Bottom);
		}
		protected override void UpdatePageSettings(float val) {
			pc.Document.PageSettings.BottomMargin = MarginsF.ToHundredths(val);
		}
		protected override float GetValue(PointF pt, RectangleF pageRect) {
			return pageRect.Bottom - pt.Y;
		}
	}
	public class PageMarginList : IEnumerable {
		PageMargin[] margins;
		public bool IsMarginResizing { get { return ActiveMargin != null && ActiveMargin.IsResizing; }
		}
		public MarginSide ActiveSide { get { return ActiveMargin != null ? ActiveMargin.Side : MarginSide.None; }
		}
		public PageMargin ActiveMargin { get; private set; }
		public PageMargin this[int index] { get { return margins[index]; } }
		public PageMarginList(DocumentViewerBase pc) {
			margins = new PageMargin[] { new LeftMargin(pc), new RightMargin(pc), new TopMargin(pc), new BottomMargin(pc) };
		}
		public void StartMarginResize(PointF pt) {
			ActiveMargin = GetPointMargin(pt);
			if(ActiveMargin != null)
				ActiveMargin.DrawSide(pt);
		}
		public void EndMarginResize(bool applyChanges) {
			if(ActiveMargin != null) {
				ActiveMargin.EndResize(applyChanges);
				ActiveMargin = null;
			}
		}
		public void DrawMovingSide(PointF pt) {
			if(ActiveMargin != null) ActiveMargin.DrawSide(pt);
		}
		public PageMargin GetPointMargin(PointF pt) {
			foreach(PageMargin mar in margins)
				if(mar.GetPointSide(pt) != MarginSide.None) return mar;
			return null;
		}
		public MarginSide GetPointSide(PointF pt) {
			PageMargin mar = GetPointMargin(pt);
			return (mar == null) ? MarginSide.None : mar.Side;
		}
		public void Draw(Graphics graph) {
			graph.ExecuteAndKeepState(() => {
				graph.ResetTransform();
				graph.PageUnit = GraphicsUnit.Display;
				foreach(PageMargin mar in margins) mar.Draw(graph);
			});
		}
		public IEnumerator GetEnumerator() {
			return margins.GetEnumerator();
		}
	}
}
