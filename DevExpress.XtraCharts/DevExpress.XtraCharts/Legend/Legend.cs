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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(LegendTypeConverter))
	]
	public class Legend : ChartElement, IBackground, IHitTest, ISupportVisibilityControlElement, ISupportTextAntialiasing {
		const int DefaultPadding = -1;
		const int DefaultMargins = 5;
		const int DefaultHorizontalIndent = 4;
		const int DefaultVerticalIndent = 2;
		const int DefaultTextOffset = 2;
		const bool DefaultVisible = true;
		const bool DefaultMarkerVisible = true;
		const bool DefaultTextVisible = true;
		const bool DefaultEquallySpacedItems = true;
		const bool DefaultAntialiasing = false;
		const bool DefaultUseCheckboxes = false;
		const double DefaultMaxHorizontalPercentage = 100.0;
		const double DefaultMaxVerticalPercentage = 100.0;
		const LegendAlignmentHorizontal DefaultAignmentHorizontal = LegendAlignmentHorizontal.RightOutside;
		const LegendAlignmentVertical DefaultAlignmentVertical = LegendAlignmentVertical.Top;
		const LegendDirection DefaultDirection = LegendDirection.TopToBottom;
		const DefaultBoolean DefaultVisibility = DefaultBoolean.Default;
		static readonly Size defaultMarkerSize = new Size(20, 16);
		static readonly Font defaultFont = DefaultFonts.Tahoma8;
		static void ValidatePercentValue(double val) {
			if (val < 0 || val > 100)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPercentValue));
		}
		readonly RectangleIndents padding;
		readonly RectangleIndents margins;
		readonly RectangleFillStyle fillStyle;
		readonly BackgroundImage backImage;
		readonly RectangularBorder border;
		readonly Shadow shadow;
		readonly HitTestState hitTestState = new HitTestState();
		bool markerVisible = DefaultMarkerVisible;
		bool textVisible = DefaultTextVisible;
		int horizontalIndent = DefaultHorizontalIndent;
		int verticalIndent = DefaultVerticalIndent;
		double maxHorizontalPercentage = DefaultMaxHorizontalPercentage;
		double maxVerticalPercentage = DefaultMaxVerticalPercentage;
		LegendAlignmentHorizontal alignmentHorizontal = DefaultAignmentHorizontal;
		LegendAlignmentVertical alignmentVertical = DefaultAlignmentVertical;
		LegendDirection direction = DefaultDirection;
		bool equallySpacedItems = DefaultEquallySpacedItems;
		int textOffset = DefaultTextOffset;
		Size markerSize = defaultMarkerSize;
		Color backColor;
		Color textColor;
		Font font;
		DefaultBoolean enableAntialiasing = DefaultBoolean.Default;
		bool useCheckBoxes = DefaultUseCheckboxes;
		bool automaticVisibility = DefaultVisible;
		DefaultBoolean visibility = DefaultVisibility;
		int itemsCount;
		bool ActualAntialiasing { get { return DefaultBooleanUtils.ToBoolean(enableAntialiasing, DefaultAntialiasing); } }
		internal Chart Chart { 
			get { return (Chart)base.Owner; } 
		}
		internal bool ActualVisible {
			get { return ActualVisibility && (markerVisible || textVisible); } 
		}
		internal bool IsOutside { 
			get {
				return alignmentHorizontal == LegendAlignmentHorizontal.LeftOutside  || 
					   alignmentHorizontal == LegendAlignmentHorizontal.RightOutside ||
					   alignmentVertical == LegendAlignmentVertical.TopOutside ||
					   alignmentVertical == LegendAlignmentVertical.BottomOutside;
			}
		}
		internal bool IsMaxPercentagesDefault {
			get { return maxHorizontalPercentage == DefaultMaxHorizontalPercentage && maxVerticalPercentage == DefaultMaxVerticalPercentage; }
		}
		internal LegendAppearance Appearance { 
			get { return CommonUtils.GetActualAppearance(this).LegendAppearance; } 
		}
		internal RectangleIndents ActualPadding { 
			get {
				RectangleIndents actualPadding = (RectangleIndents)padding.Clone();
				RectangleIndents appearancePadding = Appearance.Padding;
				if (actualPadding.Left == -1)
					actualPadding.Left = appearancePadding.Left;
				if (actualPadding.Top == -1)
					actualPadding.Top = appearancePadding.Top;
				if (actualPadding.Right == -1)
					actualPadding.Right = appearancePadding.Right;
				if (actualPadding.Bottom == -1)
					actualPadding.Bottom = appearancePadding.Bottom;
				return actualPadding;
			}
		}
		internal RectangleFillStyle ActualFillStyle { 
			get { return fillStyle.FillMode == FillMode.Empty ? Appearance.FillStyle : fillStyle; } 
		}
		internal Color ActualBackColor { 
			get { return backColor.IsEmpty ? Appearance.BackColor : backColor; } 
		}
		internal Color ActualTextColor { 
			get { return textColor.IsEmpty ? Appearance.TextColor : textColor; } 
		}
		internal GRealRect2D Bounds { get; set; }
		internal bool AutomaticVisibility { get { return Chart != null && !Chart.AutoLayout ? DefaultVisible : automaticVisibility; } }
		internal int ItemsCount { get { return itemsCount; } }
		protected internal bool ActualVisibility { get { return DefaultBooleanUtils.ToBoolean(Visibility, AutomaticVisibility); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendPadding"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.Padding"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleIndents Padding { 
			get { return padding; } 
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendMargins"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.Margins"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleIndents Margins {
			get { return margins; } 
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendBackImage"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.BackImage"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public BackgroundImage BackImage {
			get { return backImage; } 
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.FillStyle"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleFillStyle FillStyle {
			get { return fillStyle; } 
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendBorder"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.Border"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangularBorder Border { 
			get { return border; } 
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendShadow"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.Shadow"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Shadow Shadow { 
			get { return shadow; } 
		}
		[
		Obsolete("This property is obsolete now. Use the Visibility property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return ActualVisibility; }
			set { Visibility = DefaultBooleanUtils.ToDefaultBoolean(value); }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendVisibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.Visibility"),
		Category(Categories.Appearance),
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
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendMarkerVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.MarkerVisible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool MarkerVisible {
			get { return markerVisible; }
			set {
				if (value != markerVisible) {
					SendNotification(new ElementWillChangeNotification(this));
					markerVisible = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendTextVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.TextVisible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool TextVisible {
			get { return textVisible; }
			set {
				if (value != textVisible) {
					SendNotification(new ElementWillChangeNotification(this));
					textVisible = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendHorizontalIndent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.HorizontalIndent"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int HorizontalIndent {
			get { return horizontalIndent; }
			set {
				if (value != horizontalIndent) {
					if (value < 0 || value >= 1000)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLegendHorizontalIndent));
					SendNotification(new ElementWillChangeNotification(this));
					horizontalIndent = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendVerticalIndent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.VerticalIndent"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int VerticalIndent {
			get { return verticalIndent; }
			set {
				if (value != verticalIndent) {
					if (value < 0 || value >= 1000)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLegendVerticalIndent));
					SendNotification(new ElementWillChangeNotification(this));
					verticalIndent = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendMaxHorizontalPercentage"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.MaxHorizontalPercentage"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double MaxHorizontalPercentage {
			get { return maxHorizontalPercentage; }
			set {
				if (value != maxHorizontalPercentage) {
					ValidatePercentValue(value);
					SendNotification(new ElementWillChangeNotification(this));
					maxHorizontalPercentage = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendMaxVerticalPercentage"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.MaxVerticalPercentage"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double MaxVerticalPercentage {
			get { return maxVerticalPercentage; }
			set {
				if (value != maxVerticalPercentage) {
					ValidatePercentValue(value);
					SendNotification(new ElementWillChangeNotification(this));
					maxVerticalPercentage = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendAlignmentHorizontal"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.AlignmentHorizontal"),
		Category(Categories.Behavior),
		Localizable(true),
		XtraSerializableProperty
		]
		public LegendAlignmentHorizontal AlignmentHorizontal {
			get { return alignmentHorizontal; }
			set {
				if (value != alignmentHorizontal) {
					SendNotification(new ElementWillChangeNotification(this));
					alignmentHorizontal = value;
					Chart.DataContainer.PivotGridDataSourceOptions.UpdateLegend();
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendAlignmentVertical"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.AlignmentVertical"),
		Category(Categories.Behavior),
		Localizable(true),
		XtraSerializableProperty
		]
		public LegendAlignmentVertical AlignmentVertical {
			get { return alignmentVertical; }
			set {
				if (value != alignmentVertical) {
					SendNotification(new ElementWillChangeNotification(this));
					alignmentVertical = value;
					Chart.DataContainer.PivotGridDataSourceOptions.UpdateLegend();
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendDirection"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.Direction"),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public LegendDirection Direction {
			get { return direction; }
			set {
				if (value != direction) {
					SendNotification(new ElementWillChangeNotification(this));
					direction = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendEquallySpacedItems"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.EquallySpacedItems"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool EquallySpacedItems {
			get { return equallySpacedItems; }
			set {
				if (value != equallySpacedItems) {
					if (!Loading && (direction == LegendDirection.BottomToTop || direction == LegendDirection.TopToBottom))
						throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgEquallySpacedItemsNotUsable), direction));
					SendNotification(new ElementWillChangeNotification(this));
					equallySpacedItems = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendTextOffset"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.TextOffset"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int TextOffset {
			get { return textOffset; }
			set {
				if (value != textOffset) {
					if (value < 0 || value >= 1000)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLegendTextOffset));
					SendNotification(new ElementWillChangeNotification(this));
					textOffset = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendMarkerSize"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.MarkerSize"),
		TypeConverter(typeof(SizeTypeConverter)),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public Size MarkerSize {
			get { return markerSize; }
			set {
				if (value != markerSize) {
					if (value.Width <= 0 || value.Width >= 1000 || value.Height <= 0 || value.Height >= 1000)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLegendMarkerSize));
					SendNotification(new ElementWillChangeNotification(this));
					markerSize = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.BackColor"),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public Color BackColor {
			get { return backColor; }
			set {
				if (value != backColor) {
					SendNotification(new ElementWillChangeNotification(this));
					backColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendTextColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.TextColor"),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public Color TextColor {
			get { return textColor; }
			set {
				if (value != textColor) {
					SendNotification(new ElementWillChangeNotification(this));
					textColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendFont"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.Font"),
		TypeConverter(typeof(FontTypeConverter)),
		Category(Categories.Appearance),
		Localizable(true),
		XtraSerializableProperty
		]
		public Font Font {
			get { return font; }
			set {
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectFont));
				if (value != font) {
					SendNotification(new ElementWillChangeNotification(this));
					font = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendEnableAntialiasing"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.EnableAntialiasing"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public DefaultBoolean EnableAntialiasing {
			get { return enableAntialiasing; }
			set {
				if (enableAntialiasing == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				enableAntialiasing = value;
				RaiseControlChanged();
			}
		}
		[
		Obsolete("This property is now obsolete. Use the EnableAntialiasing property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty
		]
		public bool Antialiasing {
			get { return ActualAntialiasing; }
			set { EnableAntialiasing = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		[
		Obsolete("This property is obsolete now. Use the VerticalIndent and Padding properties instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public int SpacingVertical {
			get { return 0; }
			set {
				if (Loading) {
					padding.Top = value;
					padding.Bottom = value;
					verticalIndent = value;
				}
			}
		}
		[
		Obsolete("This property is obsolete now. Use the HorizontalIndent, TextOffset and Padding properties instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public int SpacingHorizontal {
			get { return 0; }
			set {
				if (Loading) {
					padding.Left = value;
					padding.Right = value;
					horizontalIndent = value * 2;
					textOffset = value;
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LegendUseCheckBoxes"),
#endif
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty,
		DXDisplayNameIgnore
		]
		public bool UseCheckBoxes {
			get { return useCheckBoxes; }
			set {
				if (value != useCheckBoxes) {
					SendNotification(new ElementWillChangeNotification(this));
					useCheckBoxes = value;
					if (Chart != null)
						Chart.DataContainer.PivotGridDataSourceOptions.UpdateAutoLayoutSettings();
					RaiseControlChanged(new DevExpress.Charts.Native.SeriesGroupsInteractionUpdateInfo(this));
				}
			}
		}
		internal Legend(Chart chart) : base(chart) {
			font = defaultFont;
			padding = new RectangleIndents(this, DefaultPadding);
			margins = new RectangleIndents(this, DefaultMargins);
			fillStyle = new RectangleFillStyle(this);
			backImage = new BackgroundImage(this);
			border = new OutsideRectangularBorder(this, true, Color.Empty);
			shadow = new Shadow(this);
		}
		#region IBackground implementation
		FillStyleBase IBackground.FillStyle { get { return FillStyle; } }
		bool IBackground.BackImageSupported { get { return true; } }
		#endregion
		#region IHitTest implementation
		object IHitTest.Object { get { return this; } }
		HitTestState IHitTest.State { get { return hitTestState; } }
		#endregion
		#region ISupportVisibilityControlElement
		int ISupportVisibilityControlElement.Priority { get { return (int)ChartElementVisibilityPriority.Legend; } }
		bool ISupportVisibilityControlElement.Visible {
			get { return automaticVisibility; }
			set { automaticVisibility = value; }
		}
		GRealRect2D ISupportVisibilityControlElement.Bounds {
			get { return Bounds; }
		}
		VisibilityElementOrientation ISupportVisibilityControlElement.Orientation {
			get {
				if (AlignmentHorizontal == LegendAlignmentHorizontal.LeftOutside || AlignmentHorizontal == LegendAlignmentHorizontal.RightOutside) {
					if (AlignmentVertical == LegendAlignmentVertical.TopOutside || AlignmentVertical == LegendAlignmentVertical.BottomOutside)
						return VisibilityElementOrientation.Corner;
					else
						return VisibilityElementOrientation.Vertical;
				}
				else if (AlignmentVertical == LegendAlignmentVertical.TopOutside || AlignmentVertical == LegendAlignmentVertical.BottomOutside)
					return VisibilityElementOrientation.Horizontal;
				else
					return VisibilityElementOrientation.Inside;
			}
		}
		#endregion
		#region ISupportTextAntialiasing implementation
		bool ISupportTextAntialiasing.DefaultAntialiasing { get { return DefaultAntialiasing; } }
		bool ISupportTextAntialiasing.Rotated { get { return false; } }
		Color ISupportTextAntialiasing.TextBackColor { get { return ActualBackColor; } }
		RectangleFillStyle ISupportTextAntialiasing.TextBackFillStyle { get { return ActualFillStyle; } }
		ChartElement ISupportTextAntialiasing.BackElement { get { return null; } }
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializePadding() {
			return padding.ShouldSerialize();
		}
		bool ShouldSerializeMargins() {
			return margins.ShouldSerialize();
		}
		bool ShouldSerializeFillStyle() {
			return fillStyle.ShouldSerialize();
		}
		bool ShouldSerializeBackImage() {
			return backImage.ShouldSerialize();
		}
		bool ShouldSerializeBorder() {
			return border.ShouldSerialize();
		}
		bool ShouldSerializeShadow() {
			return shadow.ShouldSerialize();
		}
		bool ShouldSerializeVisible() {
			return false;
		}
		bool ShouldSerializeVisibility() {
			return visibility != DefaultVisibility;
		}
		void ResetVisibility() {
			Visibility = DefaultVisibility;
		}
		bool ShouldSerializeMarkerVisible() {
			return this.markerVisible != DefaultMarkerVisible;
		}
		void ResetMarkerVisible() {
			MarkerVisible = DefaultMarkerVisible;
		}
		bool ShouldSerializeTextVisible() {
			return this.textVisible != DefaultTextVisible;
		}
		void ResetTextVisible() {
		   TextVisible = DefaultTextVisible;
		}
		bool ShouldSerializeHorizontalIndent() {
			return this.horizontalIndent != DefaultHorizontalIndent;
		}
		void ResetHorizontalIndent() {
		   HorizontalIndent = DefaultHorizontalIndent;
		}
		bool ShouldSerializeVerticalIndent() {
			return this.verticalIndent != DefaultVerticalIndent;
		}
		void ResetVerticalIndent() {
			VerticalIndent = DefaultVerticalIndent;
		}
		bool ShouldSerializeMaxHorizontalPercentage() {
			return maxHorizontalPercentage != DefaultMaxHorizontalPercentage;
		}
		void ResetMaxHorizontalPercentage() {
			MaxHorizontalPercentage = DefaultMaxHorizontalPercentage;
		}
		bool ShouldSerializeMaxVerticalPercentage() {
			return this.maxVerticalPercentage != DefaultMaxVerticalPercentage;
		}
		void ResetMaxVerticalPercentage() {
			MaxVerticalPercentage = DefaultMaxVerticalPercentage;
		}
		bool ShouldSerializeAlignmentHorizontal() {
			return this.alignmentHorizontal != DefaultAignmentHorizontal;
		}
		void ResetAlignmentHorizontal() {
			AlignmentHorizontal = DefaultAignmentHorizontal;
		}
		bool ShouldSerializeAlignmentVertical() {
			return this.alignmentVertical != DefaultAlignmentVertical;
		}
		void ResetAlignmentVertical() {
			AlignmentVertical = DefaultAlignmentVertical;
		}
		bool ShouldSerializeDirection() {
			return this.direction != DefaultDirection;
		}
		void ResetDirection() {
			Direction = DefaultDirection;
		}
		bool ShouldSerializeEquallySpacedItems() {
			return this.equallySpacedItems != DefaultEquallySpacedItems;
		}
		void ResetEquallySpacedItems() {
			EquallySpacedItems = DefaultEquallySpacedItems;
		}
		bool ShouldSerializeTextOffset() {
			return this.textOffset != DefaultTextOffset;
		}
		void ResetTextOffset() {
			TextOffset = DefaultTextOffset;
		}
		bool ShouldSerializeMarkerSize() {
			return !markerSize.Equals(defaultMarkerSize);
		}
		void ResetMarkerSize() {
			MarkerSize = defaultMarkerSize;
		}
		bool ShouldSerializeBackColor() {
			return !backColor.IsEmpty;
		}
		void ResetBackColor() {
			BackColor = Color.Empty;
		}
		bool ShouldSerializeTextColor() {
			return !textColor.IsEmpty;
		}
		void ResetTextColor() {
			TextColor = Color.Empty;
		}
		bool ShouldSerializeFont() {
			return !font.Equals(defaultFont);
		}
		void ResetFont() {
			Font = defaultFont;
		}
		bool ShouldSerializeAntialiasing() {
			return false;
		}
		bool ShouldSerializeEnableAntialiasing() {
			return this.enableAntialiasing != DefaultBoolean.Default;
		}
		void ResetEnableAntialiasing() {
			EnableAntialiasing = DefaultBoolean.Default;
		}
		bool ShouldSerializeSpacingHorizontal() {
			return false;
		}
		bool ShouldSerializeSpacingVertical() {
			return false;
		}
		bool ShouldSerializeUseCheckBoxes() {
			return this.useCheckBoxes != DefaultUseCheckboxes;
		}
		void ResetUseCheckBoxes() {
			UseCheckBoxes = DefaultUseCheckboxes;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializePadding() || ShouldSerializeMargins() || ShouldSerializeFillStyle() ||
				ShouldSerializeBackImage() || ShouldSerializeBorder() || ShouldSerializeShadow() || ShouldSerializeVisibility() ||
				ShouldSerializeMarkerVisible() || ShouldSerializeTextVisible() || ShouldSerializeHorizontalIndent() ||
				ShouldSerializeVerticalIndent() || ShouldSerializeMaxHorizontalPercentage() || ShouldSerializeMaxVerticalPercentage() ||
				ShouldSerializeAlignmentHorizontal() || ShouldSerializeAlignmentVertical() || ShouldSerializeDirection() ||
				ShouldSerializeEquallySpacedItems() || ShouldSerializeTextOffset() || ShouldSerializeMarkerSize() || ShouldSerializeBackColor() ||
				ShouldSerializeTextColor() || ShouldSerializeFont() || ShouldSerializeEnableAntialiasing() || ShouldSerializeUseCheckBoxes();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Padding":
					return ShouldSerializePadding();
				case "Margins":
					return ShouldSerializeMargins();
				case "FillStyle":
					return ShouldSerializeFillStyle();
				case "BackImage":
					return ShouldSerializeBackImage();
				case "Border":
					return ShouldSerializeBorder();
				case "Shadow":
					return ShouldSerializeShadow();
				case "Visible":
					return ShouldSerializeVisible();
				case "Visibility":
					return ShouldSerializeVisibility();
				case "MarkerVisible":
					return ShouldSerializeMarkerVisible();
				case "TextVisible":
					return ShouldSerializeTextVisible();
				case "HorizontalIndent":
					return ShouldSerializeHorizontalIndent();
				case "VerticalIndent":
					return ShouldSerializeVerticalIndent();
				case "MaxHorizontalPercentage":
					return ShouldSerializeMaxHorizontalPercentage();
				case "MaxVerticalPercentage":
					return ShouldSerializeMaxVerticalPercentage();
				case "AlignmentHorizontal":
					return ShouldSerializeAlignmentHorizontal();
				case "AlignmentVertical":
					return ShouldSerializeAlignmentVertical();
				case "Direction":
					return ShouldSerializeDirection();
				case "EquallySpacedItems":
					return ShouldSerializeEquallySpacedItems();
				case "TextOffset":
					return ShouldSerializeTextOffset();
				case "MarkerSize":
					return ShouldSerializeMarkerSize();
				case "BackColor":
					return ShouldSerializeBackColor();
				case "TextColor":
					return ShouldSerializeTextColor();
				case "Font":
					return ShouldSerializeFont();
				case "Antialiasing":
					return false;
				case "EnableAntialiasing":
					return ShouldSerializeEnableAntialiasing();
				case "SpacingHorizontal":
					return ShouldSerializeSpacingHorizontal();
				case "SpacingVertical":
					return ShouldSerializeSpacingVertical();
				case "UseCheckBoxes":
					return ShouldSerializeUseCheckBoxes();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		internal void SetItemsCount(int itemsCount) {
			this.itemsCount = itemsCount;
		}
		protected override void Dispose(bool disposing) {
			if (disposing && !IsDisposed) {
				if (fillStyle != null)
					fillStyle.Dispose();
				if (backImage != null)
					backImage.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override ChartElement CreateObjectForClone() {
			return new Legend(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Legend legend = obj as Legend;
			if (legend != null) {
				padding.Assign(legend.padding);
				margins.Assign(legend.margins);
				fillStyle.Assign(legend.fillStyle);
				backImage.Assign(legend.backImage);
				border.Assign(legend.border);
				shadow.Assign(legend.shadow);
				visibility = legend.visibility;
				markerVisible = legend.markerVisible;
				textVisible = legend.textVisible;
				horizontalIndent = legend.horizontalIndent;
				verticalIndent = legend.verticalIndent;
				maxHorizontalPercentage = legend.maxHorizontalPercentage;
				maxVerticalPercentage = legend.maxVerticalPercentage;
				alignmentHorizontal = legend.alignmentHorizontal;
				alignmentVertical = legend.alignmentVertical;
				direction = legend.direction;
				equallySpacedItems = legend.equallySpacedItems;
				textOffset = legend.textOffset;
				markerSize = legend.markerSize;
				backColor = legend.backColor;
				textColor = legend.TextColor;
				font = legend.font;
				enableAntialiasing = legend.enableAntialiasing;
				useCheckBoxes = legend.useCheckBoxes;
			}
		}
	}
}
