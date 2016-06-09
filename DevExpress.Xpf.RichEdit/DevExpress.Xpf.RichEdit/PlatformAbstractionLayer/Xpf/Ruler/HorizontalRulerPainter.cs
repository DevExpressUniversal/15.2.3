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
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.RichEdit.Ruler;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Ruler;
using DevExpress.XtraRichEdit.Utils;
using PlatformColor = System.Windows.Media.Color;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
namespace DevExpress.Xpf.RichEdit.Controls.Internal {
	#region RulerActiveAreaControl
	public class RulerActiveAreaControl : Control {
		public RulerActiveAreaControl() {
			this.DefaultStyleKey = typeof(RulerActiveAreaControl);
		}
	}
	#endregion
	#region HorizontalRulerSpaceAreaControl
	public class HorizontalRulerSpaceAreaControl : Control {
		public HorizontalRulerSpaceAreaControl() {
			this.DefaultStyleKey = typeof(HorizontalRulerSpaceAreaControl);
		}
	}
	#endregion
	#region VerticalRulerSpaceAreaControl
	public class VerticalRulerSpaceAreaControl : Control {
		public VerticalRulerSpaceAreaControl() {
			this.DefaultStyleKey = typeof(VerticalRulerSpaceAreaControl);
		}
	}
	#endregion
	#region RulerTickMarkControl
	public class RulerTickMarkControl : Control {
		public RulerTickMarkControl() {
			this.DefaultStyleKey = typeof(RulerTickMarkControl);
		}
	}
	#endregion
	#region RulerNumberTickMarkControl
	public class RulerNumberTickMarkControl : Control {
		public RulerNumberTickMarkControl() {
			this.DefaultStyleKey = typeof(RulerNumberTickMarkControl);
		}
	}
	#endregion
	#region DefaultTabControl
	public class DefaultTabControl : Control {
		public DefaultTabControl() {
			this.DefaultStyleKey = typeof(DefaultTabControl);
		}
	}
	#endregion
	#region DisabledHotZoneControl
	public class DisabledHotZoneControl : Control {
		public DisabledHotZoneControl() {
			this.DefaultStyleKey = typeof(DisabledHotZoneControl);
		}
	}
	#endregion
	#region LeftIndentHotZoneControl
	public class LeftIndentHotZoneControl : HorizontalHotZoneControl {
		public LeftIndentHotZoneControl() {
			this.DefaultStyleKey = typeof(LeftIndentHotZoneControl);
		}
	}
	#endregion
	#region RightIndentHotZoneControl 
	public class RightIndentHotZoneControl : HorizontalHotZoneControl {
		public RightIndentHotZoneControl() {
			this.DefaultStyleKey = typeof(RightIndentHotZoneControl);
		}
	}
	#endregion
	#region FirstLineIndentHotZoneControl
	public class FirstLineIndentHotZoneControl : HorizontalHotZoneControl {
		public FirstLineIndentHotZoneControl() {
			this.DefaultStyleKey = typeof(FirstLineIndentHotZoneControl);
		}
	}
	#endregion
	#region LeftIndentBottomHotZoneControl
	public class LeftIndentBottomHotZoneControl : HorizontalHotZoneControl {
		public LeftIndentBottomHotZoneControl() {
			this.DefaultStyleKey = typeof(LeftIndentBottomHotZoneControl);
		}
	}
	#endregion
	#region LeftTabHotZoneControl
	public class LeftTabHotZoneControl : HorizontalHotZoneControl {
		public LeftTabHotZoneControl() {
			this.DefaultStyleKey = typeof(LeftTabHotZoneControl);
		}
	}
	#endregion
	#region HorizontalHotZoneControl
	public class HorizontalHotZoneControl : Control {
		public static readonly DependencyProperty EnabledVisibilityProperty = DependencyProperty.Register("EnabledVisibility", typeof(Visibility), typeof(HorizontalHotZoneControl), new PropertyMetadata(Visibility.Visible));
		public static readonly DependencyProperty DisabledVisibilityProperty = DependencyProperty.Register("DisabledVisibility", typeof(Visibility), typeof(HorizontalHotZoneControl), new PropertyMetadata(Visibility.Visible));
		public Visibility EnabledVisibility { get { return (Visibility)GetValue(EnabledVisibilityProperty); } set { SetValue(EnabledVisibilityProperty, value); } }
		public Visibility DisabledVisibility { get { return (Visibility)GetValue(DisabledVisibilityProperty); } set { SetValue(DisabledVisibilityProperty, value); } }
	}
	#endregion
	#region RightTabHotZoneControl
	public class RightTabHotZoneControl : HorizontalHotZoneControl {
		public RightTabHotZoneControl() {
			this.DefaultStyleKey = typeof(RightTabHotZoneControl);
		}
	}
	#endregion
	#region CenterTabHotZoneControl
	public class CenterTabHotZoneControl : HorizontalHotZoneControl {
		public CenterTabHotZoneControl() {
			this.DefaultStyleKey = typeof(CenterTabHotZoneControl);
		}
	}
	#endregion
	#region DecimalTabHotZoneControl
	public class DecimalTabHotZoneControl : HorizontalHotZoneControl {
		public DecimalTabHotZoneControl() {
			this.DefaultStyleKey = typeof(DecimalTabHotZoneControl);
		}
	}
	#endregion
	#region TabTypeToggleBackgroundControl
	public class TabTypeToggleBackgroundControl : Control {
		public TabTypeToggleBackgroundControl() {
			this.DefaultStyleKey = typeof(TabTypeToggleBackgroundControl);
		}
	}
	#endregion
	#region TabTypeToggleHotZoneControl
	public class TabTypeToggleHotZoneControl : Control {
		public TabTypeToggleHotZoneControl() {
			this.DefaultStyleKey = typeof(TabTypeToggleHotZoneControl);
		}
	}
	#endregion
	#region HorizontalTableHotZoneControl
	public class HorizontalTableHotZoneControl : HorizontalHotZoneControl {
		public HorizontalTableHotZoneControl() {
			this.DefaultStyleKey = typeof(HorizontalTableHotZoneControl);
		}
	}
	#endregion
	#region RulerPainterBase
	public class RulerPainterBase {
		readonly RulerControlBase ruler;
		readonly XpfDrawingSurface surface;
		readonly XpfPainter painter;
		public RulerPainterBase(RulerControlBase ruler) {
			this.ruler = ruler;
		}
		public RulerPainterBase(RulerControlBase ruler, XpfDrawingSurface surface) {
			this.ruler = ruler;
			this.surface = surface;
			this.painter = new XpfPainterOverwrite(ruler.DocumentModel.LayoutUnitConverter, surface);
		}
		protected internal virtual int VerticalTextPaddingBottom { get { return ruler.PixelsToLayoutUnitsV(2); } }
		protected internal virtual int VerticalTextPaddingTop { get { return ruler.PixelsToLayoutUnitsV(2); } }
		protected internal virtual int PaddingTop { get { return ruler.PixelsToLayoutUnitsV(5); } }
		protected internal virtual int PaddingBottom { get { return ruler.PixelsToLayoutUnitsV(6); } }
		protected virtual Orientation Orientation { get { return Orientation.Horizontal; } }
		public XpfPainter Painter { get { return painter; } }
		protected internal virtual void DrawTickMark(RulerTickmark tickMark) {
		}
		protected internal void DrawTickmarkHalf(RulerTickmark tickMark) {
			DrawTickmark(tickMark);
		}
		protected internal void DrawTickmarkQuarter(RulerTickmark tickMark) {
			DrawTickmark(tickMark);
		}
		internal void Clear() {
		}
		protected internal virtual void DrawRulerAreaBackground(Rectangle bounds) {
		}
		internal void DrawActiveArea(RectangleF bounds) {
			Painter.DrawControl<RulerActiveAreaControl>(bounds);
		}
		protected internal virtual void DrawSpaceArea(RectangleF bounds) {
		}
		internal void DrawTickmark(RulerTickmark tickMark) {
			Painter.DrawControl<RulerTickMarkControl>(tickMark.DisplayBounds);
		}
		internal void DrawTickmarkNumber(RulerTickmarkNumber tickMark) {
			System.Drawing.Rectangle bounds = new System.Drawing.Rectangle((int)tickMark.DisplayBounds.X, (int)tickMark.DisplayBounds.Y, (int)tickMark.DisplayBounds.Width, (int)tickMark.DisplayBounds.Height);
			if (Orientation == Orientation.Vertical)
				bounds.Y += bounds.Width;
			if (Orientation == System.Windows.Controls.Orientation.Horizontal)
				Painter.DrawString(tickMark.Number, ruler.TickMarkFont, ruler.NumberTickMarkColor, bounds.X, bounds.Y);
			else
				Painter.DrawVString(tickMark.Number, ruler.TickMarkFont, ruler.NumberTickMarkColor, bounds.X, bounds.Y);
		}
		protected internal virtual void DrawDefaultTabMark(Rectangle bounds) {
			 Painter.DrawControl<DefaultTabControl>(bounds);
		}
		internal void DrawTabTypeToggleBackground(RectangleF bounds) {
			 Painter.DrawControl<TabTypeToggleBackgroundControl>(bounds);
		}
		internal void DrawTabTypeToggle(RectangleF bounds) {
			 Painter.DrawControl<TabTypeToggleHotZoneControl>(bounds);
		}
	}
	#endregion
	#region HorizontalRulerPainter
	public class HorizontalRulerPainter : RulerPainterBase {
		readonly HorizontalRulerControl ruler;
		public HorizontalRulerPainter(HorizontalRulerControl ruler)
			: base(ruler) {
			this.ruler = ruler;
		}
		public HorizontalRulerPainter(HorizontalRulerControl ruler, XpfDrawingSurface surface)
			: base(ruler, surface) {
			this.ruler = ruler;
		}
		#region Properties
		public HorizontalRulerControl Ruler { get { return ruler; } }
		protected PlatformColor DefaultTabColor { get { return Colors.White; } }
		protected override Orientation Orientation { get { return Orientation.Horizontal; } }
		protected internal override int PaddingTop {
			get {
				if (ruler.Surface == null)
					ruler.ApplyTemplate();
				return ruler.PixelsToLayoutUnitsV((int)ruler.Surface.Margin.Top);
			}
		}
		protected internal override int PaddingBottom {
			get {
				if (ruler.Surface == null)
					ruler.ApplyTemplate();
				return ruler.PixelsToLayoutUnitsV((int)ruler.Surface.Margin.Bottom);
			}
		}
		#endregion
		protected internal virtual System.Drawing.Size CalculateHotZoneSize(HorizontalRulerHotZone hotZone) {
			return ruler.PixelsToLayoutUnits(new System.Drawing.Size(9, 7));
		}
		public void Draw() {
			DrawInPixels();
			Painter.FinishPaint();
		}
		protected internal void DrawInPixels() {
			RichEditControl control = ruler.RichEditControl;
			CaretPosition caretPosition = control.ActiveView.CaretPosition;
			caretPosition.Update(DocumentLayoutDetailsLevel.Column);
			if (caretPosition.PageViewInfo == null)
				return;
			DrawRuler();
		}
		protected internal virtual void DrawRuler() {
			DrawRulerAreaBackground(ruler.ViewInfo.DisplayClientBounds);
			DrawActiveAreas();
			DrawTickMarks();
			DrawSpaceAreas();
			DrawDefaultTabMarks();
			DrawHotZones();
			DrawTabTypeToggle();
		}
		protected internal void DrawTabTypeToggle() {
			int x = Ruler.GetPhysicalLeftInvisibleWidthInPixel();
			RectangleF typeToggelBackgroundBounds = ruler.ViewInfo.TabTypeToggleBackground.Bounds;
			typeToggelBackgroundBounds = new RectangleF(typeToggelBackgroundBounds.X + x, typeToggelBackgroundBounds.Y, typeToggelBackgroundBounds.Width, typeToggelBackgroundBounds.Height);
			RectangleF typeToggleHotZone = ruler.ViewInfo.TabTypeToggleHotZone.DisplayBounds;
			typeToggleHotZone = new RectangleF(typeToggleHotZone.X + x, typeToggleHotZone.Y, typeToggleHotZone.Width, typeToggleHotZone.Height);
			Rectangle margin = Ruler.CalculateHotZoneMarginInPixels(ruler.ViewInfo.TabTypeToggleHotZone);
			Rectangle hotZoneBounds = ruler.ViewInfo.TabTypeToggleHotZone.HotZoneBounds;
			hotZoneBounds = new Rectangle(hotZoneBounds.X + x + margin.Left, hotZoneBounds.Y + margin.Top, hotZoneBounds.Width, hotZoneBounds.Height);
			DrawTabTypeToggleBackground(typeToggelBackgroundBounds);
			DrawTabTypeToggle(typeToggleHotZone);
			ruler.Painter.DrawHotZoneCore(ruler.ViewInfo.TabTypeToggleHotZone.HotZone, hotZoneBounds.Location);
		}
		void DrawActiveAreas() {
			ruler.ViewInfo.DisplayActiveAreaCollection.ForEach(DrawActiveArea);
		}
		void DrawSpaceAreas() {
			ruler.ViewInfo.DisplaySpaceAreaCollection.ForEach(DrawSpaceArea);
		}
		void DrawTickMarks() {
			ruler.ViewInfo.RulerTickmarks.ForEach(DrawTickMark);
		}
		void DrawDefaultTabMarks() {
			ruler.ViewInfo.DisplayDefaultTabsCollection.ForEach(DrawDefaultTabMark);
		}
		void DrawHotZones() {
			ruler.ViewInfo.HotZones.ForEach(DrawHotZone);
		}
		protected internal void DrawHotZone(RulerHotZone hotZone) {
			DrawHotZoneCore(hotZone, hotZone.DisplayBounds.Location);
		}
		protected internal void DrawHotZoneCore(RulerHotZone hotZone, System.Drawing.Point location) {
			HorizontalHotZoneControl control;
			Type type = hotZone.GetType();
			if (type == typeof(LeftIndentHotZone))
				control = Painter.CreateObject<LeftIndentHotZoneControl>();
			else if (type == typeof(LeftBottomHotZone))
				control = Painter.CreateObject<LeftIndentBottomHotZoneControl>();
			else if (type == typeof(FirstLineIndentHotZone))
				control = Painter.CreateObject<FirstLineIndentHotZoneControl>();
			else if (type == typeof(RightIndentHotZone))
				control = Painter.CreateObject<RightIndentHotZoneControl>();
			else if (type == typeof(LeftTabHotZone))
				control = Painter.CreateObject<LeftTabHotZoneControl>();
			else if (type == typeof(RightTabHotZone))
				control = Painter.CreateObject<RightTabHotZoneControl>();
			else if (type == typeof(CenterTabHotZone))
				control = Painter.CreateObject<CenterTabHotZoneControl>();
			else if (type == typeof(DecimalTabHotZone))
				control = Painter.CreateObject<DecimalTabHotZoneControl>();
			else if (type == typeof(TableLeftBorderHotZone) || type == typeof(TableHotZone))
				control = Painter.CreateObject<HorizontalTableHotZoneControl>();
			else
				return;
			SetVisibility(hotZone, control);
			Painter.SetPositionInPixels(control, location.X, location.Y);
		}
		void SetVisibility(RulerHotZone hotZone, HorizontalHotZoneControl control) {
			if (hotZone.Enabled) {
				control.EnabledVisibility = Visibility.Visible;
				control.DisabledVisibility = Visibility.Collapsed;
			} else {
				control.EnabledVisibility = Visibility.Collapsed;
				control.DisabledVisibility = Visibility.Visible;
			}
		}
		internal System.Drawing.Size GetTabTypeToggleActiveAreaSize() {
			return new System.Drawing.Size(15, 15);
		}
		protected internal override void DrawTickMark(RulerTickmark tickMark) {
			tickMark.Draw(this);
		}
		protected internal override void DrawRulerAreaBackground(Rectangle bounds) {
			Painter.DrawControl<HorizontalRulerSpaceAreaControl>(bounds);
		}
		protected internal override void DrawSpaceArea(RectangleF bounds) {
			Painter.DrawControl<HorizontalRulerSpaceAreaControl>(bounds);
		}
	}
	#endregion
	#region VerticalRulerPainter
	public class VerticalRulerPainter : RulerPainterBase {
		readonly VerticalRulerControl ruler;
		public VerticalRulerPainter(VerticalRulerControl ruler)
			: base(ruler) {
			this.ruler = ruler;
		}
		public VerticalRulerPainter(VerticalRulerControl ruler, XpfDrawingSurface surface)
			: base(ruler, surface) {
			this.ruler = ruler;
		}
		#region Properties
		protected override Orientation Orientation { get { return Orientation.Vertical; } }
		public VerticalRulerControl Ruler { get { return ruler; } }
		protected PlatformColor DefaultTabColor { get { return Colors.White; } }
		protected internal override int PaddingTop {
			get {
				if (ruler.Surface == null)
					ruler.ApplyTemplate();
				return ruler.PixelsToLayoutUnitsV((int)ruler.Surface.Margin.Left);
			}
		}
		protected internal override int PaddingBottom {
			get {
				if (ruler.Surface == null)
					ruler.ApplyTemplate();
				return ruler.PixelsToLayoutUnitsV((int)ruler.Surface.Margin.Right);
			}
		}
		#endregion
		protected internal virtual System.Drawing.Size CalculateHotZoneSize(HorizontalRulerHotZone hotZone) {
			return new System.Drawing.Size();
		}
		public void Draw() {
			DrawInPixels();
			Painter.FinishPaint();
		}
		protected internal void DrawInPixels() {
			RichEditControl control = ruler.RichEditControl;
			CaretPosition caretPosition = control.ActiveView.CaretPosition;
			caretPosition.Update(DocumentLayoutDetailsLevel.Column);
			if (caretPosition.PageViewInfo == null)
				return;
			DrawRuler();
		}
		protected internal virtual void DrawRuler() {
			DrawRulerAreaBackground(ruler.ViewInfo.DisplayClientBounds);
			DrawActiveAreas();
			DrawTickMarks();
			DrawSpaceAreas();
			DrawHotZones();
		}
		void DrawActiveAreas() {
			ruler.ViewInfo.DisplayActiveAreaCollection.ForEach(DrawActiveArea);
		}
		void DrawSpaceAreas() {
			ruler.ViewInfo.DisplaySpaceAreaCollection.ForEach(DrawSpaceArea);
		}
		void DrawTickMarks() {
			ruler.ViewInfo.RulerTickmarks.ForEach(DrawTickMark);
		}
		void DrawHotZones() {
			ruler.ViewInfo.HotZones.ForEach(DrawHotZone);
		}
		protected internal void DrawHotZone(RulerHotZone hotZone) {
			DrawHotZoneCore(hotZone, hotZone.DisplayBounds.Location);
		}
		protected internal void DrawHotZoneCore(RulerHotZone hotZone, System.Drawing.Point location) {
			Type type = hotZone.GetType();
			if (type == typeof(VerticalTableHotZone))
				Painter.DrawControl<VerticalRulerSpaceAreaControl>(hotZone.DisplayBounds);
			else
				return;
		}
		void SetVisibility(RulerHotZone hotZone, HorizontalHotZoneControl control) {
			control.EnabledVisibility = Visibility.Visible;
			control.DisabledVisibility = Visibility.Collapsed;
		}
		protected internal override void DrawTickMark(RulerTickmark tickMark) {
			tickMark.Draw(this);
		}
		protected internal override void DrawRulerAreaBackground(Rectangle bounds) {
			Painter.DrawControl<VerticalRulerSpaceAreaControl>(bounds);
		}
		protected internal override void DrawSpaceArea(RectangleF bounds) {
			Painter.DrawControl<VerticalRulerSpaceAreaControl>(bounds);
		}
	}
	#endregion
}
