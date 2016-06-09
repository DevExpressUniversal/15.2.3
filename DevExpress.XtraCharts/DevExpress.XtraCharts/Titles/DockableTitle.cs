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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum ChartTitleDockStyle {
		Top,
		Bottom,
		Left,
		Right
	}
	[TypeConverter(typeof(DockableTitle))]
	public abstract class DockableTitle : MultilineTitle, ITextPropertiesProvider, ISupportVisibilityControlElement {
		#region Nested classes: DockingBase, HorizontalDocking, VerticalDocking, TopDocking, BottomDocking, LeftDocking, RightDocking
		public abstract class DockingBase {
			public static DockingBase CreateInstance(ChartTitleDockStyle dock) {
				switch (dock) {
					case ChartTitleDockStyle.Top:
						return new TopDocking();
					case ChartTitleDockStyle.Bottom:
						return new BottomDocking();
					case ChartTitleDockStyle.Left:
						return new LeftDocking();
					case ChartTitleDockStyle.Right:
						return new RightDocking();
					default:
						throw new DefaultSwitchException();
				}
			}
			public abstract int TextAngle { get; }
			public abstract NearTextPosition NearPosition { get; }
			public abstract Rectangle ModifyBounds(Rectangle bounds, int size);
			public abstract int GetSizeFromRect(Rectangle rect);
			public abstract Point CalcOrigin(Rectangle bounds, Rectangle rect, StringAlignment alignment);
		}
		public abstract class HorizontalDocking : DockingBase {
			public override int TextAngle { get { return 0; } }
			public override int GetSizeFromRect(Rectangle rect) {
				return rect.Height;
			}
			protected int GetPointX(Rectangle bounds, Rectangle rect, StringAlignment alignment) {
				switch (alignment) {
					case StringAlignment.Center:
						return bounds.X + bounds.Width / 2;
					case StringAlignment.Near:
						return bounds.X + rect.Width / 2;
					case StringAlignment.Far:
						return bounds.Right - rect.Width / 2;
					default:
						throw new DefaultSwitchException();
				}
			}
		}
		public abstract class VerticalDocking : DockingBase {
			public override int GetSizeFromRect(Rectangle rect) {
				return rect.Width;
			}
		}
		public class TopDocking : HorizontalDocking {
			public override NearTextPosition NearPosition { get { return NearTextPosition.Bottom; } }
			public override Rectangle ModifyBounds(Rectangle bounds, int size) {
				bounds.Y += size;
				bounds.Height -= size;
				return bounds;
			}
			public override Point CalcOrigin(Rectangle bounds, Rectangle rect, StringAlignment alignment) {
				int x = GetPointX(bounds, rect, alignment);
				int y = bounds.Y;
				return new Point(x, y);
			}
		}
		public class BottomDocking : HorizontalDocking {
			public override NearTextPosition NearPosition { get { return NearTextPosition.Top; } }
			public override Rectangle ModifyBounds(Rectangle bounds, int size) {
				bounds.Height -= size;
				return bounds;
			}
			public override Point CalcOrigin(Rectangle bounds, Rectangle rect, StringAlignment alignment) {
				int x = GetPointX(bounds, rect, alignment);
				int y = bounds.Bottom;
				return new Point(x, y);
			}
		}
		public class LeftDocking : VerticalDocking {
			public override int TextAngle { get { return 270; } }
			public override NearTextPosition NearPosition { get { return NearTextPosition.Right; } }
			public override Rectangle ModifyBounds(Rectangle bounds, int size) {
				bounds.X += size;
				bounds.Width -= size;
				return bounds;
			}
			public override Point CalcOrigin(Rectangle bounds, Rectangle rect, StringAlignment alignment) {
				switch (alignment) {
					case StringAlignment.Center:
						return new Point(bounds.X, bounds.Y + bounds.Height / 2);
					case StringAlignment.Near:
						return new Point(bounds.X, bounds.Bottom - rect.Height / 2);
					case StringAlignment.Far:
						return new Point(bounds.X, bounds.Y + rect.Height / 2);
					default:
						throw new DefaultSwitchException();
				}
			}
		}
		public class RightDocking : VerticalDocking {
			public override int TextAngle { get { return 90; } }
			public override NearTextPosition NearPosition { get { return NearTextPosition.Left; } }
			public override Rectangle ModifyBounds(Rectangle bounds, int size) {
				bounds.Width -= size;
				return bounds;
			}
			public override Point CalcOrigin(Rectangle bounds, Rectangle rect, StringAlignment alignment) {
				switch (alignment) {
					case StringAlignment.Center:
						return new Point(bounds.Right, bounds.Y + bounds.Height / 2);
					case StringAlignment.Near:
						return new Point(bounds.Right, bounds.Y + rect.Height / 2);
					case StringAlignment.Far:
						return new Point(bounds.Right, bounds.Bottom - rect.Height / 2);
					default:
						throw new DefaultSwitchException();
				}
			}
		}
		#endregion
		const int DefaultIndent = 5;
		const int DefaultMaxLineCount = 0;
		const bool DefaultWordWrap = false;
		const ChartTitleDockStyle DefaultDock = ChartTitleDockStyle.Top;
		const StringAlignment DefaultAlignment = StringAlignment.Center;
		const DefaultBoolean DefaultVisibility = DefaultBoolean.Default;
		bool wordWrap;
		bool automaticVisibility = true;
		int indent = DefaultIndent;
		int maxLineCount = DefaultMaxLineCount;
		DefaultBoolean visibility = DefaultVisibility;
		DockingBase docking;
		ChartTitleDockStyle dock;
		StringAlignment alignment = DefaultAlignment;
		internal bool AutomaticVisibility { get { return ChartContainer != null && !ChartContainer.Chart.AutoLayout ? true : automaticVisibility; } }
		internal DockingBase Docking { get { return docking; } }
		internal GRealRect2D Bounds { get; set; }
		protected abstract int VisibilityPriority { get; }
		protected override bool DefaultAntialiasing { get { return true; } }
		protected override bool DefaultVisible { get { return true; } }
		protected internal override bool ActualVisibility { get { return DefaultBooleanUtils.ToBoolean(Visibility, AutomaticVisibility); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DockableTitleWordWrap"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DockableTitle.WordWrap"),
		Category(Categories.Behavior),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public bool WordWrap {
			get { return wordWrap; }
			set {
				if (value != wordWrap) {
					SendNotification(new ElementWillChangeNotification(this));
					wordWrap = value;
					RaiseControlChanged();
				}
			}
		}
		[
		Obsolete("This property is now obsolete. Use the MaxLineCount property instead."),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public int MaximumLinesCount {
			get { return MaxLineCount; }
			set { MaxLineCount = value; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DockableTitleMaxLineCount"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DockableTitle.MaxLineCount"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int MaxLineCount {
			get { return maxLineCount; }
			set {
				if (value != maxLineCount) {
					if (value < 0 || value > 20)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectMaxLineCount));
					SendNotification(new ElementWillChangeNotification(this));
					maxLineCount = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DockableTitleDock"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DockableTitle.Dock"),
		Category(Categories.Behavior),
		TypeConverter(typeof(EnumTypeConverter)),
		XtraSerializableProperty
		]
		public ChartTitleDockStyle Dock {
			get { return dock; }
			set {
				if (value == dock)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				SetDock(value);
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DockableTitleAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DockableTitle.Alignment"),
		Category(Categories.Behavior),
		TypeConverter(typeof(StringAlignmentTypeConvertor)),
		Localizable(true),
		XtraSerializableProperty
		]
		public StringAlignment Alignment {
			get { return alignment; }
			set {
				if (value == alignment)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				alignment = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DockableTitleIndent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DockableTitle.Indent"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int Indent {
			get { return indent; }
			set {
				if (value != indent) {
					if (value < 0 || value >= 1000)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectChartTitleIndent));
					SendNotification(new ElementWillChangeNotification(this));
					indent = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DockableTitleVisibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DockableTitle.Visibility"),
		Category(Categories.Behavior),
		TypeConverter(typeof(DefaultBooleanConverter)),
		XtraSerializableProperty
		]
		public DefaultBoolean Visibility {
			get { return visibility; }
			set {
				if (visibility != value) {
					SendNotification(new ElementWillChangeNotification(this));
					visibility = value;
					RaiseControlChanged();
				}
			}
		}
		#region Obsolete properties
		[
		Obsolete("This property is obsolete now. Use the Visibility property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty
		]
		public new bool Visible {
			get { return ActualVisibility; }
			set { Visibility = DefaultBooleanUtils.ToDefaultBoolean(value); }
		}
		#endregion
		protected DockableTitle()
			: base(null) {
			SetDock(DefaultDock);
		}
		#region IHitTest implementation
		HitTestState hitTestState = new HitTestState();
		object IHitTest.Object { get { return HitObject; } }
		HitTestState IHitTest.State { get { return HitState; } }
		protected internal virtual object HitObject { get { return this; } }
		protected internal virtual HitTestState HitState { get { return hitTestState; } }
		#endregion
		#region ITextPropertiesProvider implementation
		RectangleFillStyle ITextPropertiesProvider.FillStyle { get { return RectangleFillStyle.Empty; } }
		RectangularBorder ITextPropertiesProvider.Border { get { return null; } }
		Shadow ITextPropertiesProvider.Shadow { get { return null; } }
		bool ITextPropertiesProvider.ChangeSelectedBorder { get { return true; } }
		bool ITextPropertiesProvider.CorrectBoundsByBorder { get { return false; } }
		Color ITextPropertiesProvider.BackColor { get { return Color.Empty; } }
		Color ITextPropertiesProvider.GetTextColor(Color color) { return ActualTextColor; }
		Color ITextPropertiesProvider.GetBorderColor(Color color) { return Color.Empty; }
		#endregion
		#region ISupportVisibilityControlElement
		int ISupportVisibilityControlElement.Priority { get { return VisibilityPriority; } }
		bool ISupportVisibilityControlElement.Visible {
			get { return automaticVisibility; }
			set { automaticVisibility = value; }
		}
		GRealRect2D ISupportVisibilityControlElement.Bounds {
			get { return Bounds; }
		}
		VisibilityElementOrientation ISupportVisibilityControlElement.Orientation {
			get {
				if (Dock == ChartTitleDockStyle.Left || Dock == ChartTitleDockStyle.Right)
					return VisibilityElementOrientation.Vertical;
				return VisibilityElementOrientation.Horizontal;
			}
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Dock":
					return ShouldSerializeDock();
				case "Alignment":
					return ShouldSerializeAlignment();
				case "Indent":
					return ShouldSerializeIndent();
				case "WordWrap":
					return ShouldSerializeWordWrap();
				case "MaxLineCount":
					return ShouldSerializeMaxLineCount();
				case "Visible":
					return false;
				case "Visibility":
					return ShouldSerializeVisibility();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeWordWrap() {
			return WordWrap != DefaultWordWrap;
		}
		void ResetWordWrap() {
			WordWrap = DefaultWordWrap;
		}
		bool ShouldSerializeDock() {
			return Dock != DefaultDock;
		}
		void ResetDock() {
			Dock = DefaultDock;
		}
		bool ShouldSerializeAlignment() {
			return Alignment != DefaultAlignment;
		}
		void ResetAlignment() {
			Alignment = DefaultAlignment;
		}
		bool ShouldSerializeIndent() {
			return Indent != DefaultIndent;
		}
		void ResetIndent() {
			Indent = DefaultIndent;
		}
		bool ShouldSerializeMaxLineCount() {
			return maxLineCount != DefaultMaxLineCount;
		}
		void ResetMaxLineCount() {
			MaxLineCount = DefaultMaxLineCount;
		}
		bool ShouldSerializeMaximumLinesCount() {
			return false;
		}
		bool ShouldSerializeVisibility() {
			return visibility != DefaultVisibility;
		}
		void ResetVisibility() {
			Visibility = DefaultVisibility;
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		void SetDock(ChartTitleDockStyle dock) {
			this.dock = dock;
			docking = DockingBase.CreateInstance(dock);
		}
		protected override Color GetTextColor(IChartAppearance actualAppearance) {
			return actualAppearance.WholeChartAppearance.TitleColor;
		}
		protected internal virtual string ConstrucActualText() {
			return Text;
		}
		internal DockableTitleViewData CalculateViewData(TextMeasurer textMeasurer, ref Rectangle bounds, HitRegionContainer occupiedRegion) {
			string actualText = ConstrucActualText();
			if (Visibility == DefaultBoolean.False || String.IsNullOrEmpty(actualText))
				return null;
			bool isTextMultiline = actualText.Contains("\n") || actualText.Contains("<br>") || actualText.Contains("<BR>") || actualText.Contains("<Br>") || actualText.Contains("<bR>");
			bool isTitleVertical = (dock == ChartTitleDockStyle.Left || dock == ChartTitleDockStyle.Right);
			int maxAllowedTextLengthPixels = isTitleVertical ? bounds.Height : bounds.Width;
			int maxWidth = maxAllowedTextLengthPixels;
			SizeF textSize = textMeasurer.MeasureString(actualText, Font);
			RotatedTextPainterNearLine painter = new RotatedTextPainterNearLine(Point.Empty, actualText, textSize, this, docking.NearPosition, docking.TextAngle, false, true, textMeasurer, maxWidth, MaxLineCount, wordWrap);
			Rectangle rect = painter.RoundedBounds;
			Point origin = docking.CalcOrigin(bounds, rect, alignment);
			Bounds = new GRealRect2D(rect.Left + origin.X, rect.Top + origin.Y, rect.Width, rect.Height);
			if (!ActualVisibility)
				return null;
			int size = docking.GetSizeFromRect(rect) + indent + 1;
			rect.Offset(origin);
			HitRegionContainer hitRegion = new HitRegionContainer(new HitRegion(rect));
			hitRegion.Exclude(occupiedRegion);
			IHitRegion region = (IHitRegion)occupiedRegion.Underlying.Clone();
			DockableTitleViewData viewData = new DockableTitleViewData(this, textMeasurer, actualText, region, hitRegion, origin, maxWidth, size, wordWrap);
			bounds = docking.ModifyBounds(bounds, size);
			occupiedRegion.Union(new HitRegion(rect));
			return viewData;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			DockableTitle title = obj as DockableTitle;
			if (title != null) {
				SetDock(title.dock);
				alignment = title.alignment;
				indent = title.indent;
				wordWrap = title.wordWrap;
				maxLineCount = title.maxLineCount;
				visibility = title.visibility;
			}
		}
		public override string ToString() {
			return Text;
		}
	}
	public abstract class DockableTitleCollectionBase : ChartCollectionBase {
		public DockableTitle this[int index] { get { return (DockableTitle)List[index]; } }
		protected DockableTitleCollectionBase(ChartElement owner)
			: base() {
			base.Owner = owner;
		}
		internal List<DockableTitleViewData> CalculateViewDataAndBoundsWithoutTitle(TextMeasurer textMeasurer, ref Rectangle bounds) {
			List<DockableTitleViewData> viewData = new List<DockableTitleViewData>();
			using (HitRegionContainer occupiedRegion = new HitRegionContainer()) {
				foreach (DockableTitle title in this) {
					DockableTitleViewData titleViewData = title.CalculateViewData(textMeasurer, ref bounds, occupiedRegion);
					if (titleViewData != null)
						viewData.Add(titleViewData);
				}
			}
			return viewData.Count == 0 ? null : viewData;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class DockableTitleViewData : IDisposable {
		DockableTitle title;
		string actualText;
		IHitRegion excludeRegion;
		HitRegionContainer hitRegion;
		RotatedTextPainterNearLine painter;
		int size;
		public DockableTitle Title { get { return title; } }
		public bool IsVerticalTitle { get { return title.Dock == ChartTitleDockStyle.Left || title.Dock == ChartTitleDockStyle.Right; } }
		public int Size { get { return size; } }
		public DockableTitleViewData(DockableTitle title, TextMeasurer textMeasurer, string actualText, IHitRegion excludeRegion, HitRegionContainer hitRegion, Point origin, int maxWidth, int size, bool wordWrap) {
			this.title = title;
			this.actualText = actualText;
			this.excludeRegion = excludeRegion;
			this.hitRegion = hitRegion;
			this.size = size;
			painter = new RotatedTextPainterNearLine(origin, actualText, textMeasurer.MeasureString(actualText, title.Font),
				title, title.Docking.NearPosition, title.Docking.TextAngle, false, true, textMeasurer, maxWidth, title.MaxLineCount, wordWrap);
		}
		public void Dispose() {
			if (excludeRegion != null) {
				excludeRegion.Dispose();
				excludeRegion = null;
			}
			if (hitRegion != null) {
				hitRegion.Dispose();
				hitRegion = null;
			}
		}
		public void Offset(int xOffset, int yOffset) {
			painter.Offset(xOffset, yOffset);
			if (hitRegion != null)
				hitRegion.Dispose();
			hitRegion = new HitRegionContainer(new HitRegion(painter.RoundedBounds));
			hitRegion.Exclude(excludeRegion);
		}
		public void Render(IRenderer renderer) {
			renderer.SetClipping((Region)excludeRegion.GetGDIRegion().Clone(), System.Drawing.Drawing2D.CombineMode.Exclude);
			HitTestController controller = CommonUtils.FindOwnerChart(title).HitTestController;
			IHitRegion region = (IHitRegion)hitRegion.Underlying.Clone();
			painter.Render(renderer, controller, region, null, Color.Empty);
			renderer.RestoreClipping();
		}
	}
}
