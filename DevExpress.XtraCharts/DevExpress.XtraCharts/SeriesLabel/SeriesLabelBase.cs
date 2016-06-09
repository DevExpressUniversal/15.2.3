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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[Obsolete("This enumeration is now obsolete. Use the PointView enumeration instead.")]
	public enum SeriesLabelValueType {
		Points,
		SeriesName
	}
	[TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder),
	DXDisplayNameAttribute.DefaultResourceFile)]
	public enum TextOrientation {
		Horizontal,
		TopToBottom,
		BottomToTop,
	}
	[ResourceFinder(typeof(ResFinder),
	DXDisplayNameAttribute.DefaultResourceFile)]
	public enum ResolveOverlappingMode {
		None = ResolveOverlappingModeCore.None,
		Default = ResolveOverlappingModeCore.Default,
		HideOverlapped = ResolveOverlappingModeCore.HideOverlapped,
		JustifyAroundPoint = ResolveOverlappingModeCore.JustifyAroundPoint,
		JustifyAllAroundPoint = ResolveOverlappingModeCore.JustifyAllAroundPoint
	}
	[TypeConverter(typeof(SeriesLabelBaseTypeConverter))]
	public abstract class SeriesLabelBase : ChartElement, ITextPropertiesProvider, ISupportInitialize, IXtraSerializable, IXtraSupportCreateContentPropertyValue, IXtraSupportAfterDeserialize, IBackground, ILabelBehaviorProvider, ISupportBorderVisibility {
		const StringAlignment defaultTextAlignment = StringAlignment.Center;
		internal const int DefaultResolveOverlappingMinIndent = -1;
		internal const ResolveOverlappingMode DefaultResolveOverlappingMode = ResolveOverlappingMode.None;
		internal const TextOrientation DefaulTextOrientation = TextOrientation.Horizontal;
		static readonly Font defaultFont = DefaultFonts.Tahoma8;
		bool loading;
		bool isSetResolveOverlappingMode = false;
		bool actualVisible = false;
		int maxWidth = 0;
		int maxLineCount = 0;
		int lineLength = 10;
		string text = String.Empty;
		DefaultBoolean enableAntialiasing = DefaultBoolean.Default;
		AspxSerializerWrapper<PointOptions> pointOptionsSerializerWrapper;
		Color textColor = Color.Empty;
		Color backColor = Color.Empty;
		Color lineColor = Color.Empty;
		Font font;
		Shadow shadow;
		StringAlignment textAlignment = defaultTextAlignment;
		RectangularBorder border;
		RectangleFillStyle fillStyle;
		ResolveOverlappingMode resolveOverlappingMode = DefaultResolveOverlappingMode;
		LineStyle lineStyle;
		TextOrientation textOrientation = DefaulTextOrientation;
		PointView deserializedPointView = PointView.Values;
		PointOptions pointOptions;
		DefaultBoolean visibility = DefaultBoolean.Default;
		DefaultBoolean lineVisibility = DefaultBoolean.Default;
		IDataLabelFormatter formatter;
		string textPattern;
		string pivotGridTextPattern;
		bool pointOptionsWasDeserialized = false;
		Diagram Diagram {
			get {
				bool diagramExists = SeriesBase != null && SeriesBase.View != null && SeriesBase.View.Chart != null;
				return diagramExists ? SeriesBase.View.Chart.Diagram : null;
			}
		}
		SeriesLabelAppearance LabelAppearance { 
			get { return GetSeriesLabelAppearance(); } 
		}
		bool UseTemplateHit {
			get {
				Series ownerSeries = SeriesBase as Series;
				return ownerSeries != null && ownerSeries.UseTemplateHit;
			}
		}
		bool ActualAntialiasing { get { return DefaultBooleanUtils.ToBoolean(enableAntialiasing, false); } }
		SeriesLabelBase TemplateLabel { 
			get { return SeriesBase.Owner.SeriesTemplate.Label; } 
		}
		protected Series Series {
			get {
				Series series = SeriesBase as Series;
				ChartDebug.Assert(series != null, "series can't be null");
				return series;
			}
		}
		protected virtual Color ActualTextColor { 
			get { return TextColor.IsEmpty ? LabelAppearance.TextColor : TextColor; } 
		}
		protected virtual bool Rotated { get { return false; } }
		internal string ActualTextPattern {
			get {
				if (!String.IsNullOrEmpty(textPattern))
					return textPattern;
				if (!String.IsNullOrEmpty(pivotGridTextPattern))
					return pivotGridTextPattern;
				return SeriesBase != null && SeriesBase.View != null ? SeriesBase.View.DefaultLabelTextPattern : PatternUtils.ValuesPattern;
			}
		}
		internal string PivotGridTextPattern {
			get { return pivotGridTextPattern; }
			set { pivotGridTextPattern = value; }
		}
		internal DefaultBoolean InternalVisibility { get { return visibility; } }
		internal SeriesBase SeriesBase {
			get { return (SeriesBase)base.Owner; }
			set {
				base.Owner = value;
				if (pointOptions == null)
					pointOptions = SeriesBase.View.CreatePointOptions();
				pointOptions.SeriesBase = value;
			}
		}
		internal IDataLabelFormatter Formatter {
			get { return formatter; }
			set {
				if (value != formatter) {
					SendNotification(new ElementWillChangeNotification(this));
					formatter = value;
					RaiseControlChanged();
				}
			}
		}
		protected internal PointOptions PointOptionsInternal {
			get { return pointOptions; }
			set {
				pointOptions = value;
				UpdateTextPattern(pointOptions);
			}
		}
		protected internal bool ActualLineVisible { get { return DefaultBooleanUtils.ToBoolean(lineVisibility, GetDefaultLineVisible(LabelAppearance)); } }
		protected internal abstract bool ShadowSupported { get; }
		protected internal abstract bool ConnectorSupported { get; }
		protected internal abstract bool ConnectorEnabled { get; }
		protected internal virtual bool ResolveOverlappingSupported { get { return true; } }
		protected internal virtual bool ResolveOverlappingEnabled { get { return true; } }
		protected internal virtual bool VerticalRotationSupported { get { return true; } }
		protected internal virtual bool DefaultVisible { get { return true; } }
		protected internal virtual Color ActualBackColor { get { return BackColor.IsEmpty ? LabelAppearance.BackColor : BackColor; } }
		protected internal virtual Color ActualBorderColor { get { return Border.Color.IsEmpty ? LabelAppearance.BorderColor : Border.Color; } }
		protected internal override bool Loading { get { return loading || base.Loading; } }
		#region Obsolete Properties
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(""),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		Obsolete("This property is obsolete now.")]
		public string HiddenSerializableString { get { return String.Empty; } set { } }
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("The SeriesBase.Label.Visible property has become obsolete. Use the SeriesBase.LabelsVisibilty property instead."),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public bool Visible {
			get { return SeriesBase != null ? SeriesBase.ActualLabelsVisibility : actualVisible; }
			set {
				actualVisible = value;
				DefaultBoolean newVisibilityValue = value ? DefaultBoolean.True : DefaultBoolean.False;
				if (SeriesBase != null) {
					SeriesBase.LabelsVisibility = newVisibilityValue;
					visibility = SeriesBase.LabelsVisibility == newVisibilityValue ? DefaultBoolean.Default : newVisibilityValue;
				}
				else
					visibility = newVisibilityValue;
			}
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the TextPattern property instead."),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public string Text {
			get { return pointOptions.Pattern; }
			set {
				if (SeriesBase == null)
					text = value;
				else
					SynchronizePointOptionsWithText(value);
			}
		}
		[Obsolete("This property is now obsolete. Use the LineVisibility property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public bool LineVisible {
			get { return ActualLineVisible; }
			set { LineVisibility = DefaultBooleanUtils.ToDefaultBoolean(value); }
		}
#pragma warning disable 618
		[Obsolete("This property is now obsolete. Use the PointOptions.PointView property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public SeriesLabelValueType ValueType {
			get {
				PointView pointView = pointOptions == null ? deserializedPointView : PointOptions.PointView;
				return pointView == PointView.SeriesName ? SeriesLabelValueType.SeriesName : SeriesLabelValueType.Points;
			}
			set {
				deserializedPointView = value == SeriesLabelValueType.SeriesName ? PointView.SeriesName : PointView.Values;
				if (PointOptions != null)
					PointOptions.PointView = deserializedPointView;
			}
		}
#pragma warning restore 618
		[Obsolete("This property is obsolete now. Use the TextPattern property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public PointOptions PointOptions {
			get { return pointOptions; }
			set {
				if (!Loading)
					throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgPointOptionsSettingRuntimeError));
				if (!Object.ReferenceEquals(pointOptions, value)) {
					SendNotification(new ElementWillChangeNotification(this));
					pointOptions = value;
					pointOptions.SeriesBase = SeriesBase;
					RaiseControlChanged();
					if (SeriesBase == null)
						pointOptionsWasDeserialized = true;
				}
			}
		}
		#endregion
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string TypeNameSerializable { 
			get { return this.GetType().Name; } 
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseTextColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.TextColor"),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color TextColor {
			get { return this.textColor; }
			set {
				if (value != this.textColor) {
					SendNotification(new ElementWillChangeNotification(this));
					this.textColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.BackColor"),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color BackColor {
			get { return this.backColor; }
			set {
				if (value != this.backColor) {
					SendNotification(new ElementWillChangeNotification(this));
					this.backColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseFont"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.Font"),
		TypeConverter(typeof(FontTypeConverter)),
		Category(Categories.Appearance),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Font Font {
			get { return font; }
			set {
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectFont));
				if (value != this.font) {
					SendNotification(new ElementWillChangeNotification(this));
					font = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseShadow"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.Shadow"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Shadow Shadow { 
			get { return this.shadow; } 
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseBorder"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.Border"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangularBorder Border { 
			get { return this.border; } 
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.FillStyle"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleFillStyle FillStyle { 
			get { return this.fillStyle; } 
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
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseEnableAntialiasing"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Legend.EnableAntialiasing"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		RefreshProperties(RefreshProperties.All),
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
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseLineLength"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.LineLength"),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int LineLength {
			get { return this.lineLength; }
			set {
				if (value != this.lineLength) {
					if (value < 0 || value >= 1000)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSeriesLabelLineLength));
					SendNotification(new ElementWillChangeNotification(this));
					this.lineLength = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseLineVisibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.LineVisibility"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DefaultBoolean LineVisibility {
			get { return this.lineVisibility; }
			set {
				if (value != this.lineVisibility) {
					SendNotification(new ElementWillChangeNotification(this));
					this.lineVisibility = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseLineColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.LineColor"),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color LineColor {
			get { return this.lineColor; }
			set {
				if (value != this.lineColor) {
					SendNotification(new ElementWillChangeNotification(this));
					this.lineColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.LineStyle"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public LineStyle LineStyle { 
			get { return this.lineStyle; } 
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseResolveOverlappingMinIndent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.ResolveOverlappingMinIndent"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public int ResolveOverlappingMinIndent {
			get {
				return Diagram != null ? Diagram.LabelsResolveOverlappingMinIndent : DefaultResolveOverlappingMinIndent;
			}
			set {
				if (Diagram != null)
					Diagram.LabelsResolveOverlappingMinIndent = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseResolveOverlappingMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.ResolveOverlappingMode"),
		TypeConverter(typeof(ResolveOverlappingModeTypeConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public ResolveOverlappingMode ResolveOverlappingMode {
			get { return resolveOverlappingMode; }
			set {
				if (value != resolveOverlappingMode) {
					if (!loading && !IsCorrectResolveOverlappingMode(value))
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgUnsupportedResolveOverlappingMode));
					SendNotification(new ElementWillChangeNotification(this));
					resolveOverlappingMode = value;
					isSetResolveOverlappingMode = true;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseTextOrientation"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.TextOrientation"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public virtual TextOrientation TextOrientation {
			get { return textOrientation; }
			set {
				if (value != textOrientation) {
					SendNotification(new ElementWillChangeNotification(this));
					textOrientation = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseMaxWidth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.MaxWidth"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int MaxWidth {
			get { return maxWidth; }
			set {
				if (value < 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLabelMaxWidth));
				if (value != maxWidth) {
					SendNotification(new ElementWillChangeNotification(this));
					maxWidth = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseMaxLineCount"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.MaxLineCount"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int MaxLineCount {
			get { return maxLineCount; }
			set {
				if (value < 0 || value > 20)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectMaxLineCount));
				if (value != maxLineCount) {
					SendNotification(new ElementWillChangeNotification(this));
					maxLineCount = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseTextAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.TextAlignment"),
		Category(Categories.Behavior),
		TypeConverter(typeof(StringAlignmentTypeConvertor)),
		Localizable(true),
		XtraSerializableProperty
		]
		public StringAlignment TextAlignment {
			get { return textAlignment; }
			set {
				if (value != textAlignment) {
					SendNotification(new ElementWillChangeNotification(this));
					textAlignment = value;
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		NestedTagProperty
		]
		public IList PointOptionsSerializable { 
			get { return pointOptionsSerializerWrapper; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesLabelBaseTextPattern"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesLabelBase.TextPattern"),
		Category(Categories.Behavior),
		Editor("DevExpress.XtraCharts.Design.SeriesLabelTextPatternEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty]
		public string TextPattern {
			get { return textPattern; }
			set {
				if (value != textPattern) {
					SendNotification(new ElementWillChangeNotification(this));
					textPattern = value;
					RaiseControlChanged();
				}
			}
		}
		protected SeriesLabelBase()
			: base() {
			fillStyle = new RectangleFillStyle(this, Color.Empty, FillMode.Solid);
			border = new OutsideRectangularBorder(this, true, Color.Empty);
			shadow = new Shadow(this);
			font = defaultFont;
			lineStyle = new LineStyle(this, 1, false);
			pointOptionsSerializerWrapper = new AspxSerializerWrapper<PointOptions>(delegate() { return pointOptions; },
				delegate(PointOptions value) { PointOptionsInternal = value; });
		}
		#region IBackground implementation
		FillStyleBase IBackground.FillStyle { get { return FillStyle; } }
		BackgroundImage IBackground.BackImage { get { return null; } }
		bool IBackground.BackImageSupported { get { return false; } }
		#endregion
		#region IHitTest implementation
		HitTestState hitTestState = new HitTestState();
		object IHitTest.Object {
			get { return UseTemplateHit && TemplateLabel != null ? ((IHitTest)TemplateLabel).Object : this; }
		}
		HitTestState IHitTest.State {
			get { return UseTemplateHit && TemplateLabel != null ? ((IHitTest)TemplateLabel).State : hitTestState; }
		}
		#endregion
		#region ITextProprtiesProvider implementation
		StringAlignment ITextPropertiesProvider.Alignment { get { return TextAlignment; } }
		bool ITextPropertiesProvider.ChangeSelectedBorder { get { return true; } }
		bool ITextPropertiesProvider.CorrectBoundsByBorder { get { return false; } }
		Color ITextPropertiesProvider.BackColor { get { return ActualBackColor; } }
		Color ITextPropertiesProvider.GetTextColor(Color color) { return CalcActualTextColor(color); }
		Color ITextPropertiesProvider.GetBorderColor(Color color) {
			Color actualBorderColor = ActualBorderColor;
			return border.Color == Color.Empty && actualBorderColor == Color.Empty ?
				BorderHelper.CalculateBorderColor(border, CalcLabelAutomaticColor(color), ((IHitTest)this).State) : actualBorderColor;
		}
		Shadow ITextPropertiesProvider.Shadow { get { return ShadowSupported ? Shadow : null; } }
		#endregion
		#region ILabelAppearenceProvider implementation
		bool ILabelBehaviorProvider.ConnectorSupported { get { return ConnectorSupported; } }
		bool ILabelBehaviorProvider.ConnectorEnabled { get { return ConnectorEnabled; } }
		bool ILabelBehaviorProvider.ShadowSupported { get { return ShadowSupported; } }
		bool ILabelBehaviorProvider.ResolveOverlappingEnabled { get { return ResolveOverlappingEnabled; } }
		bool ILabelBehaviorProvider.ResolveOverlappingSupported { get { return ResolveOverlappingSupported; } }
		bool ILabelBehaviorProvider.VerticalRotationSupported { get { return VerticalRotationSupported; } }
		bool ILabelBehaviorProvider.CheckResolveOverlappingMode(ResolveOverlappingMode mode) {
			return CheckResolveOverlappingMode(mode);
		}
		#endregion
		#region ISupportInitialize implemantation
		void ISupportInitialize.BeginInit() {
			loading = true;
		}
		void ISupportInitialize.EndInit() {
			if (!String.IsNullOrEmpty(text)) {
				SynchronizePointOptionsWithText(text);
				text = String.Empty;
			}
			if (!ResolveOverlappingSupported)
				ResolveOverlappingMode = ResolveOverlappingMode.None;
			loading = false;
		}
		#endregion
		#region ISupportBorderVisibility implementation
		bool ISupportBorderVisibility.BorderVisible {
			get {
				SeriesLabelAppearance appearance = LabelAppearance;
				return appearance != null ? appearance.ShowBorder : true;
			}
		}
		#endregion
		#region ISupportTextAntialiasing implementation
		bool ISupportTextAntialiasing.DefaultAntialiasing { get { return false; } }
		bool ISupportTextAntialiasing.Rotated { get { return Rotated; } }
		ChartElement ISupportTextAntialiasing.BackElement {
			get {
				SeriesViewBase view = SeriesBase != null ? SeriesBase.View : null;
				return (view is SimpleDiagramSeriesViewBase || view is RadarSeriesViewBase) ? this : null;
			}
		}
		Color ISupportTextAntialiasing.TextBackColor { get { return ActualBackColor; } }
		RectangleFillStyle ISupportTextAntialiasing.TextBackFillStyle { get { return fillStyle; } }
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Visible":
					return ShouldSerializeVisible();
				case "TextColor":
					return ShouldSerializeTextColor();
				case "BackColor":
					return ShouldSerializeBackColor();
				case "Font":
					return ShouldSerializeFont();
				case "Antialiasing":
					return false;
				case "EnableAntialiasing":
					return ShouldSerializeEnableAntialiasing();
				case "LineColor":
					return ShouldSerializeLineColor();
				case "LineVisible":
					return ShouldSerializeLineVisible();
				case "LineVisibility":
					return ShouldSerializeLineVisibility();
				case "LineLength":
					return ShouldSerializeLineLength();
				case "FillStyle":
					return ShouldSerializeFillStyle();
				case "Shadow":
					return ShouldSerializeShadow();
				case "Border":
					return ShouldSerializeBorder();
				case "LineStyle":
					return ShouldSerializeLineStyle();
				case "ResolveOverlappingMode":
					return ShouldSerializeResolveOverlappingMode();
				case "TextDirection":
					return ShouldSerializeTextOrientation();
				case "MaxWidth":
					return ShouldSerializeMaxWidth();
				case "MaxLineCount":
					return ShouldSerializeMaxLineCount();
				case "TextAlignment":
					return ShouldSerializeTextAlignment();
				case "PointOptions":
					return ShouldSerializePointOptions();
				case "TextPattern":
					return ShouldSerializeTextPattern();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			((ISupportInitialize)this).BeginInit();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			((ISupportInitialize)this).EndInit();
		}
		object IXtraSupportCreateContentPropertyValue.Create(XtraItemEventArgs e) {
			return e.Item.Name == "PointOptions" ? XtraSerializingUtils.GetContentPropertyInstance(e, SerializationUtils.ExecutingAssembly, SerializationUtils.PublicNamespace) : null;
		}
		void IXtraSupportAfterDeserialize.AfterDeserialize(XtraItemEventArgs e) {
			if (e.Item.Name == "PointOptions") {
				PointOptionsInternal = (PointOptions)e.Item.Value;
				pointOptionsWasDeserialized = true;
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		protected virtual bool ShouldSerializeVisible() {
			return false;
		}
		bool ShouldSerializeText() {
			return false;
		}
		bool ShouldSerializeValueType() {
			return false;
		}
		bool ShouldSerializeTextColor() {
			return !textColor.IsEmpty;
		}
		void ResetTextColor() {
			TextColor = Color.Empty;
		}
		bool ShouldSerializeBackColor() {
			return !backColor.IsEmpty;
		}
		void ResetBackColor() {
			BackColor = Color.Empty;
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
		bool ShouldSerializeLineColor() {
			return !lineColor.IsEmpty;
		}
		void ResetLineColor() {
			LineColor = Color.Empty;
		}
		bool ShouldSerializeLineLength() {
			return lineLength != 10;
		}
		void ResetLineLength() {
			LineLength = 10;
		}
		bool ShouldSerializeHiddenSerializableString() {
			return false;
		}
		bool ShouldSerializeFillStyle() {
			return FillStyle.ShouldSerialize();
		}
		bool ShouldSerializeShadow() {
			return Shadow.ShouldSerialize();
		}
		bool ShouldSerializeBorder() {
			return Border.ShouldSerialize();
		}
		bool ShouldSerializeLineStyle() {
			return LineStyle.ShouldSerialize();
		}
		bool ShouldSerializeLineVisible() {
			return false;
		}
		bool ShouldSerializeLineVisibility() {
			return lineVisibility != DefaultBoolean.Default;
		}
		void ResetLineVisibility() {
			LineVisibility = DefaultBoolean.Default;
		}
		bool ShouldSerializeResolveOverlappingMode() {
			return resolveOverlappingMode != DefaultResolveOverlappingMode;
		}
		void ResetResolveOverlappingMode() {
			ResolveOverlappingMode = DefaultResolveOverlappingMode;
		}
		bool ShouldSerializeTextOrientation() {
			return textOrientation != DefaulTextOrientation;
		}
		void ResetTextOrientation() {
			TextOrientation = DefaulTextOrientation;
		}
		bool ShouldSerializeMaxWidth() {
			return maxWidth != 0;
		}
		void ResetMaxWidth() {
			MaxWidth = 0;
		}
		bool ShouldSerializeMaxLineCount() {
			return maxLineCount > 0;
		}
		void ResetMaxLineCount() {
			MaxLineCount = 0;
		}
		bool ShouldSerializeTextAlignment() {
			return textAlignment != defaultTextAlignment;
		}
		void ResetTextAlignment() {
			TextAlignment = defaultTextAlignment;
		}
		bool ShouldSerializePointOptions() {
			return false;
		}
		bool ShouldSerializePointOptionsSerializable() {
			return ShouldSerializePointOptions() && ChartContainer != null && ChartContainer.ControlType == ChartContainerType.WebControl;
		}
		bool ShouldSerializeTextPattern() {
			return !String.IsNullOrEmpty(textPattern);
		}
		void ResetTextPattern() {
			TextPattern = string.Empty;
		}
		bool ShouldSerializeResolveOverlappingMinIndent() {
			return ResolveOverlappingMinIndent != DefaultResolveOverlappingMinIndent;
		}
		void ResetResolveOverlappingMinIndent() {
			ResolveOverlappingMinIndent = DefaultResolveOverlappingMinIndent;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeFont() || ShouldSerializeTextColor() || ShouldSerializeBackColor() ||
				ShouldSerializeVisible() || ShouldSerializeEnableAntialiasing() || ShouldSerializeLineColor() || ShouldSerializeLineVisible() ||
				ShouldSerializeLineLength() || ShouldSerializeFillStyle() || ShouldSerializeShadow() || ShouldSerializeBorder() ||
				ShouldSerializeLineStyle() || ShouldSerializeResolveOverlappingMode() || ShouldSerializeTextOrientation() || ShouldSerializeMaxWidth() ||
				ShouldSerializeMaxLineCount() || ShouldSerializeTextAlignment() || ShouldSerializeLineVisibility() || ShouldSerializeTextPattern();
		}
		#endregion
		void SynchronizePointOptionsWithText(string text) {
			SeriesBase series = SeriesBase;
			if (series != null) {
				series.BreakPointOptionsRelations();
				PointOptionsInternal.Pattern = PatternUtils.CorrectTextForPattern(text);
			}
		}
		SeriesLabelAppearance GetSeriesLabelAppearance() {
			IChartAppearance chartAppearance = CommonUtils.GetActualAppearance(this);
			if (chartAppearance != null && SeriesBase != null && SeriesBase.View != null) {
				if (((ISeriesView)SeriesBase.View).Is3DView)
					return chartAppearance.SeriesLabel3DAppearance;
				else
					return chartAppearance.SeriesLabel2DAppearance;
			}
			return null;
		}
		protected virtual string[] ConstructTexts(RefinedPoint refinedPoint) {
			string labelText = String.Empty;
			if (formatter == null) {
				PatternParser patternParser = new PatternParser(ActualTextPattern, Series.View);
				patternParser.SetContext(refinedPoint, Series);
				labelText = patternParser.GetText();
			}
			else
				labelText = formatter.GetDataLabelText(refinedPoint);
			return new string[] { labelText };
		}
		protected virtual bool GetDefaultLineVisible(SeriesLabelAppearance labelAppearance) {
			return labelAppearance != null ? labelAppearance.ShowConnector : true;
		}
		internal virtual void UpdateTextPattern(ChartElement sender) {
			if (SeriesBase != null && pointOptions != null && (sender == pointOptions || sender.Owner == pointOptions))
				textPattern = PointOptions.ConstructPatternFromPointOptions(pointOptions, SeriesBase.ActualArgumentScaleType, SeriesBase.ValueScaleType);
		}
		internal void OnEndLoading() {
			if (deserializedPointView == PointView.SeriesName)
				PointOptionsInternal.PointView = deserializedPointView;
			if (!IsCorrectResolveOverlappingMode(resolveOverlappingMode))
				resolveOverlappingMode = ResolveOverlappingMode.None;
			if (pointOptionsWasDeserialized)
				UpdateTextPattern(PointOptionsInternal);
		}
		internal void SetResolveOverlappingMode(ResolveOverlappingMode value) {
			if (isSetResolveOverlappingMode)
				return;
			if (!loading && !IsCorrectResolveOverlappingMode(value))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgUnsupportedResolveOverlappingMode));
			SendNotification(new ElementWillChangeNotification(this));
			resolveOverlappingMode = value;
			RaiseControlChanged();
		}
		internal void EnsureResolveOverlappingMode() {
			if (!ResolveOverlappingSupported) {
				ResolveOverlappingMode = ResolveOverlappingMode.None;
				return;
			}
			if (resolveOverlappingMode != ResolveOverlappingMode.None) {
				if (CheckResolveOverlappingMode(resolveOverlappingMode))
					ResolveOverlappingMode = resolveOverlappingMode;
				else
					ResolveOverlappingMode = ResolveOverlappingMode.Default;
			}
		}
		internal void SetNewPointOptions(PointOptions pointOptions) {
			this.pointOptions = pointOptions;
			UpdateTextPattern(pointOptions);
		}
		internal bool IsCorrectResolveOverlappingMode(ResolveOverlappingMode mode) {
			return mode == ResolveOverlappingMode.None || (ResolveOverlappingSupported && CheckResolveOverlappingMode(mode));
		}
		internal Color CalcActualTextColor(Color color) {
			return TextColor.IsEmpty ? (ActualTextColor.IsEmpty ? CalcLabelAutomaticColor(color) : ActualTextColor) : TextColor;
		}
		internal Color CalcLabelAutomaticColor(Color baseColor) {
			return HitTestColors.MixColors(Color.FromArgb(40, 0, 0, 0), baseColor);
		}
		internal Color GetActualConnectorColor(Color color) {
			if (lineColor.IsEmpty)
				return LabelAppearance.ConnectorColor.IsEmpty ? CalcLabelAutomaticColor(color) : LabelAppearance.ConnectorColor;
			return lineColor;
		}
		internal SeriesLabelViewData[] CalculateViewData(RefinedPoint refinedPoint) {
			if (!SeriesBase.ActualLabelsVisibility)
				return new SeriesLabelViewData[0];
			string[] texts = ConstructTexts(refinedPoint);
			SeriesLabelViewData[] viewData = new SeriesLabelViewData[texts.Length];
			for (int i = 0; i < texts.Length; i++)
				viewData[i] = new SeriesLabelViewData(refinedPoint.SeriesPoint, texts[i]);
			return viewData;
		}
		protected internal DiagramPoint CalculateFinishPoint(double angle, DiagramPoint startPoint) {
			return SeriesLabelHelper.CalculateFinishPoint(angle, startPoint, LineLength);
		}
		protected internal virtual void CalculateLayout(SeriesLabelLayoutList labelLayoutList, RefinedPointData pointData, TextMeasurer textMeasurer) {
		}
		protected internal virtual void CalculateLayout(SimpleDiagramSeriesLabelLayoutList labelLayoutList, SeriesPointLayout pointLayout, TextMeasurer textMeasurer) {
		}
		protected internal virtual bool CheckResolveOverlappingMode(ResolveOverlappingMode mode) {
			return mode != ResolveOverlappingMode.JustifyAllAroundPoint && mode != ResolveOverlappingMode.JustifyAroundPoint;
		}
		protected internal virtual ResolveOverlappingMode GetResolveOverlappingMode(Rectangle diagramBounds, IList<RefinedSeriesData> seriesDataList) {
			ResolveOverlappingMode mode = ResolveOverlappingMode.None;
			if (SeriesBase.ActualLabelsVisibility) {
				double diagramSize = diagramBounds.Width * diagramBounds.Height;
				double seriesLabelsSize = 0;
				foreach (RefinedSeriesData seriesData in seriesDataList)
					foreach (RefinedPointData pointData in seriesData)
						foreach (SeriesLabelViewData labelViewData in pointData.LabelViewData)
							seriesLabelsSize += labelViewData.TextSize.Width * labelViewData.TextSize.Height;
				if (seriesLabelsSize / diagramSize >= 0.1 && IsCorrectResolveOverlappingMode(ResolveOverlappingMode.HideOverlapped))
					mode = ResolveOverlappingMode.HideOverlapped;
				else if (IsCorrectResolveOverlappingMode(ResolveOverlappingMode.Default))
					mode = ResolveOverlappingMode.Default;
			}
			return mode;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SeriesLabelBase label = obj as SeriesLabelBase;
			if (label == null)
				return;
			maxWidth = label.maxWidth;
			maxLineCount = label.maxLineCount;
			textAlignment = label.TextAlignment;
			font = label.font;
			textColor = label.textColor;
			backColor = label.backColor;
			fillStyle.Assign(label.fillStyle);
			shadow.Assign(label.shadow);
			border.Assign(label.border);
			enableAntialiasing = label.enableAntialiasing;
			lineLength = label.lineLength;
			lineVisibility = label.lineVisibility;
			lineColor = label.lineColor;
			lineStyle.Assign(label.lineStyle);
			deserializedPointView = label.deserializedPointView;
			resolveOverlappingMode = label.resolveOverlappingMode;
			textOrientation = label.textOrientation;
			if (pointOptions != null && label.pointOptions != null)
				pointOptions.Assign(label.pointOptions);
			textPattern = label.TextPattern;
			pointOptionsWasDeserialized = label.pointOptionsWasDeserialized;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public interface ILabelBehaviorProvider {
		bool ConnectorSupported { get; }
		bool ConnectorEnabled { get; }
		bool ShadowSupported { get; }
		bool ResolveOverlappingEnabled { get; }
		bool ResolveOverlappingSupported { get; }
		bool VerticalRotationSupported { get; }
		bool CheckResolveOverlappingMode(ResolveOverlappingMode mode);
	}
}
