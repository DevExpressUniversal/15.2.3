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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum CrosshairSnapMode {
		NearestArgument = CrosshairSnapModeCore.NearestArgument,
		NearestValue = CrosshairSnapModeCore.NearestValue
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum CrosshairLabelMode {
		ShowForEachSeries = CrosshairLabelModeCore.ShowForEachSeries,
		ShowForNearestSeries = CrosshairLabelModeCore.ShowForNearestSeries,
		ShowCommonForAllSeries = CrosshairLabelModeCore.ShowCommonForAllSeries,
	}
	[
	TypeConverter(typeof(CrosshairOptionsTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class CrosshairOptions : ChartElement, ICrosshairOptions, IXtraSupportCreateContentPropertyValue, IXtraSupportAfterDeserialize {
		const CrosshairSnapMode DefaultSnapMode = CrosshairSnapMode.NearestArgument;
		const CrosshairLabelMode DefaultCrosshairLabelMode = CrosshairLabelMode.ShowCommonForAllSeries;
		const bool DefaultShowArgumentLine = true;
		const bool DefaultShowOnlyInFocusedPane = true;
		const bool DefaultShowValueLabels = false;
		const bool DefaultShowArgumentLabels = false;
		const bool DefaultShowCrosshairLabels = true;
		const bool DefaultShowValueLine = false;
		const bool DefaultShowGroupHeaders = true;
		const bool DefaultHighlightPoints = true;
		readonly Color defaultCrosshairLineColor;
		readonly AspxSerializerWrapper<CrosshairLabelPosition> commonLabelPositionSerializerWrapper;
		bool showOnlyInFocusedPane = DefaultShowOnlyInFocusedPane;
		bool showArgumentLabels = DefaultShowArgumentLabels;
		bool showValueLabels = DefaultShowValueLabels;
		bool showCrosshairLabels = DefaultShowCrosshairLabels;
		bool showArgumentLine = DefaultShowArgumentLine;
		bool showValueLine = DefaultShowValueLine;
		bool showGroupHeaders = DefaultShowGroupHeaders;
		bool highlightPoints = DefaultHighlightPoints;
		string groupHeaderPattern;
		CrosshairSnapMode snapMode = DefaultSnapMode;
		CrosshairLabelMode crosshairLabelMode = DefaultCrosshairLabelMode;
		CrosshairLabelPosition commonLabelPosition;
		Color argumentLineColor;
		LineStyle argumentLineStyle;
		Color valueLineColor;
		LineStyle valueLineStyle;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsShowArgumentLabels"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.ShowArgumentLabels"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowArgumentLabels {
			get { return showArgumentLabels; }
			set {
				if (value != showArgumentLabels) {
					SendNotification(new ElementWillChangeNotification(this));
					showArgumentLabels = value;
					RaiseControlChanged();
				}
			} 
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsShowValueLabels"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.ShowValueLabels"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowValueLabels {
			get { return showValueLabels; }
			set {
				if (value != showValueLabels) {
					SendNotification(new ElementWillChangeNotification(this));
					showValueLabels = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsShowCrosshairLabels"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.ShowCrosshairLabels"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowCrosshairLabels {
			get { return showCrosshairLabels; }
			set {
				if (value != showCrosshairLabels) {
					SendNotification(new ElementWillChangeNotification(this));
					showCrosshairLabels = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsShowOnlyInFocusedPane"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.ShowOnlyInFocusedPane"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowOnlyInFocusedPane {
			get { return showOnlyInFocusedPane; }
			set {
				if (value != showOnlyInFocusedPane) {
					SendNotification(new ElementWillChangeNotification(this));
					showOnlyInFocusedPane = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsCrosshairLabelMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.CrosshairLabelMode"),
		XtraSerializableProperty
		]
		public CrosshairLabelMode CrosshairLabelMode {
			get { return crosshairLabelMode; }
			set {
				if (value != crosshairLabelMode) {
					SendNotification(new ElementWillChangeNotification(this));
					crosshairLabelMode = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsSnapMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.SnapMode"),
		XtraSerializableProperty
		]
		public CrosshairSnapMode SnapMode { 
			get { return snapMode; }
			set {
				if (value != snapMode) {
					SendNotification(new ElementWillChangeNotification(this));
					snapMode = value;
					RaiseControlChanged(new PropertyUpdateInfo(this, "SnapMode"));
				}
			} 
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsShowArgumentLine"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.ShowArgumentLine"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowArgumentLine {
			get { return showArgumentLine; }
			set {
				if (value != showArgumentLine) {
					SendNotification(new ElementWillChangeNotification(this));
					showArgumentLine = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsShowValueLine"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.ShowValueLine"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowValueLine {
			get { return showValueLine; }
			set {
				if (value != showValueLine) {
					SendNotification(new ElementWillChangeNotification(this));
					showValueLine = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsCommonLabelPosition"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.CommonLabelPosition"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor("DevExpress.XtraCharts.Design.CrosshairLabelPositionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
		public CrosshairLabelPosition CommonLabelPosition {
			get {
				return commonLabelPosition;
			}
			set {
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectCrosshairPosition));
				SendNotification(new ElementWillChangeNotification(this));
				commonLabelPosition = value;
				commonLabelPosition.Owner = this;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsArgumentLineColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.ArgumentLineColor"),
		XtraSerializableProperty
		]
		public Color ArgumentLineColor { 
			get { return argumentLineColor; }
			set {
				if (value != argumentLineColor) {
					SendNotification(new ElementWillChangeNotification(this));
					argumentLineColor = value;
					RaiseControlChanged();
				}				
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsArgumentLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.ArgumentLineStyle"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public LineStyle ArgumentLineStyle { get { return argumentLineStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsValueLineColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.ValueLineColor"),
		XtraSerializableProperty
		]
		public Color ValueLineColor {
			get { return valueLineColor; }
			set {
				if (value != valueLineColor) {
					SendNotification(new ElementWillChangeNotification(this));
					valueLineColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsValueLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.ValueLineStyle"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public LineStyle ValueLineStyle { get { return valueLineStyle; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		NestedTagProperty
		]
		public IList CommonLabelPositionSerializable { get { return commonLabelPositionSerializerWrapper; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsHighlightPoints"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.HighlightPoints"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool HighlightPoints {
			get { return highlightPoints; }
			set {
				if (value != highlightPoints) {
					SendNotification(new ElementWillChangeNotification(this));
					highlightPoints = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsShowGroupHeaders"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.ShowGroupHeaders"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowGroupHeaders {
			get { return showGroupHeaders; }
			set {
				if (value != showGroupHeaders) {
					SendNotification(new ElementWillChangeNotification(this));
					showGroupHeaders = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairOptionsGroupHeaderPattern"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairOptions.GroupHeaderPattern"),
		Editor("DevExpress.XtraCharts.Design.GroupHeaderPatternEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty
		]
		public string GroupHeaderPattern {
			get { return groupHeaderPattern; }
			set {
				if (value != groupHeaderPattern) {
					SendNotification(new ElementWillChangeNotification(this));
					groupHeaderPattern = value;
					RaiseControlChanged();
				}
			}
		}
		internal CrosshairOptions(Chart chart) : base(chart) {
			commonLabelPosition = new CrosshairMousePosition();
			commonLabelPosition.Owner = this;
			commonLabelPositionSerializerWrapper = new AspxSerializerWrapper<CrosshairLabelPosition>(delegate() { return CommonLabelPosition; },
				delegate(CrosshairLabelPosition value) { CommonLabelPosition = value; });
			argumentLineStyle = new LineStyle(this);
			valueLineStyle = new LineStyle(this);
			defaultCrosshairLineColor = Color.FromArgb(0xDE, 0x39, 0xCD);
			argumentLineColor = defaultCrosshairLineColor;
			valueLineColor = defaultCrosshairLineColor;
		}
		#region ICrosshairOptions
		CrosshairSnapModeCore ICrosshairOptions.SnapMode { get { return (CrosshairSnapModeCore)SnapMode; } }
		CrosshairLabelModeCore ICrosshairOptions.LabelMode { get { return (CrosshairLabelModeCore)CrosshairLabelMode; } }
		ICrosshairFreePosition ICrosshairOptions.LabelPosition { get { return commonLabelPosition; } }
		bool ICrosshairOptions.ShowTail {
			get {
				if (crosshairLabelMode == CrosshairLabelMode.ShowCommonForAllSeries)
					return false;
				else
					return true;
			}
		}
		#endregion
		#region XtraSeralizing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "ShowArgumentLabels":
					return ShouldSerializeShowArgumentLabels();
				case "ShowValueLabels":
					return ShouldSerializeShowValueLabels();
				case "ShowCrosshairLabels":
					return ShouldSerializeShowCrosshairLabels();
				case "ShowOnlyInFocusedPane":
					return ShouldSerializeShowOnlyInFocusedPane();
				case "HighlightPoints":
					return ShouldSerializeHighlightPoints();
				case "SnapMode":
					return ShouldSerializeSnapMode();
				case "CrosshairLabelMode":
					return ShouldSerializeCrosshairLabelMode();
				case "ShowArgumentLine":
					return ShouldSerializeShowArgumentLine();
				case "ShowValueLine":
					return ShouldSerializeShowValueLine();
				case "CommonLabelPosition":
					return commonLabelPosition.ShouldSerialize();
				case "ShowGroupHeaders":
					return ShouldSerializeShowGroupHeaders();
				case "GroupHeaderPattern":
					return ShouldSerializeGroupHeaderPattern();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		object IXtraSupportCreateContentPropertyValue.Create(XtraItemEventArgs e) {
			return e.Item.Name == "CommonLabelPosition" ? XtraSerializingUtils.GetContentPropertyInstance(e, SerializationUtils.ExecutingAssembly, SerializationUtils.PublicNamespace) : null;
		}
		void IXtraSupportAfterDeserialize.AfterDeserialize(XtraItemEventArgs e) {
			if (e.Item.Name == "CommonLabelPosition")
				CommonLabelPosition = (CrosshairLabelPosition)e.Item.Value;
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeShowArgumentLabels() {
			return showArgumentLabels != DefaultShowArgumentLabels;
		}
		void ResetShowArgumentLabels() {
			ShowArgumentLabels = DefaultShowArgumentLabels;
		}
		bool ShouldSerializeShowValueLabels() {
			return showValueLabels != DefaultShowValueLabels;
		}
		void ResetShowValueLabels() {
			ShowValueLabels = DefaultShowValueLabels;
		}
		bool ShouldSerializeShowCrosshairLabels() {
			return showCrosshairLabels != DefaultShowCrosshairLabels;
		}
		void ResetShowCrosshairLabels() {
			ShowCrosshairLabels = DefaultShowCrosshairLabels;
		}
		bool ShouldSerializeShowOnlyInFocusedPane() {
			return showOnlyInFocusedPane != DefaultShowOnlyInFocusedPane;
		}
		void ResetShowOnlyInFocusedPane() {
			ShowOnlyInFocusedPane = DefaultShowOnlyInFocusedPane;
		}
		bool ShouldSerializeHighlightPoints() {
			return highlightPoints != DefaultHighlightPoints;
		}
		void ResetHighlightPoints() {
			HighlightPoints = DefaultHighlightPoints;
		}
		bool ShouldSerializeSnapMode() {
			return snapMode != DefaultSnapMode;
		}
		void ResetSnapMode() {
			SnapMode = DefaultSnapMode;
		}
		bool ShouldSerializeCrosshairLabelMode() {
			return crosshairLabelMode != DefaultCrosshairLabelMode;
		}
		void ResetCrosshairLabelMode() {
			CrosshairLabelMode = DefaultCrosshairLabelMode;
		}
		bool ShouldSerializeShowArgumentLine() {
			return showArgumentLine != DefaultShowArgumentLine;
		}
		void ResetShowArgumentLine() {
			ShowArgumentLine = DefaultShowArgumentLine;
		}
		bool ShouldSerializeShowValueLine() {
			return showValueLine != DefaultShowValueLine;
		}
		void ResetShowValueLine() {
			ShowValueLine = DefaultShowValueLine;
		}
		bool ShouldSerializeArgumentLineColor() {
			return argumentLineColor != this.defaultCrosshairLineColor;
		}
		void ResetArgumentLineColor() {
			ArgumentLineColor = this.defaultCrosshairLineColor;
		}
		bool ShouldSerializeValueLineColor() {
			return valueLineColor != this.defaultCrosshairLineColor;
		}
		void ResetValueLineColor() {
			ValueLineColor = this.defaultCrosshairLineColor;
		}
		bool ShouldSerializeCommonLabelPosition() {
			return commonLabelPosition != null && commonLabelPosition.ShouldSerialize() && ChartContainer != null && ChartContainer.ControlType != ChartContainerType.WebControl;
		}
		void ResetCommonLabelPosition() {
			CommonLabelPosition = new CrosshairMousePosition();
		}
		bool ShouldSerializeCommonLabelPositionSerializable() {
			return commonLabelPosition != null && commonLabelPosition.ShouldSerialize() && ChartContainer != null && ChartContainer.ControlType == ChartContainerType.WebControl;
		}
		bool ShouldSerializeShowGroupHeaders() {
			return showGroupHeaders != DefaultShowGroupHeaders;
		}
		void ResetShowGroupHeaders() {
			ShowGroupHeaders = DefaultShowGroupHeaders;
		}
		bool ShouldSerializeGroupHeaderPattern() {
			return !String.IsNullOrEmpty(groupHeaderPattern);
		}
		void ResetGroupHeaderPattern() {
			GroupHeaderPattern = string.Empty;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeShowArgumentLabels() ||
				ShouldSerializeShowValueLabels() ||
				ShouldSerializeShowCrosshairLabels() ||
				ShouldSerializeShowOnlyInFocusedPane() ||
				ShouldSerializeSnapMode() ||
				ShouldSerializeShowArgumentLine() ||
				ShouldSerializeShowValueLine() ||
				ShouldSerializeCommonLabelPosition() ||
				ShouldSerializeCommonLabelPositionSerializable() ||
				ShouldSerializeShowGroupHeaders() ||
				ShouldSerializeGroupHeaderPattern() ||
				ShouldSerializeHighlightPoints();
		}
		#endregion
		void AssignLabelPosition(CrosshairLabelPosition templateLabelPosition) {
			if (!commonLabelPosition.GetType().Equals(templateLabelPosition.GetType()))
				SetLabelPosition((CrosshairLabelPosition)templateLabelPosition.Clone());
			else
				commonLabelPosition.Assign(templateLabelPosition);
		}
		void SetLabelPosition(CrosshairLabelPosition value) {
			commonLabelPosition = value;
			commonLabelPosition.Owner = this;
		}
		protected override ChartElement CreateObjectForClone() {
			return new CrosshairOptions(null);
		}
		internal void OnEndLoading() {
			commonLabelPosition.OnEndLoading();
		}
		internal string[] GetAvailablePatternPlaceholders(){
			return SnapMode == CrosshairSnapMode.NearestArgument ? new string[1] { ToolTipPatternUtils.ArgumentPattern } : new string[1] { ToolTipPatternUtils.ValuePattern };
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			CrosshairOptions options = obj as CrosshairOptions;
			if (options != null) {
				showArgumentLabels = options.showArgumentLabels;
				showValueLabels = options.showValueLabels;
				showCrosshairLabels = options.showCrosshairLabels;
				showOnlyInFocusedPane = options.showOnlyInFocusedPane;
				snapMode = options.snapMode;
				crosshairLabelMode = options.crosshairLabelMode;
				showArgumentLine = options.showArgumentLine;
				showValueLine = options.showValueLine;
				argumentLineColor = options.argumentLineColor;
				argumentLineStyle = options.argumentLineStyle;
				valueLineColor = options.valueLineColor;
				valueLineStyle = options.valueLineStyle;
				AssignLabelPosition(options.commonLabelPosition);
				showGroupHeaders = options.showGroupHeaders;
				groupHeaderPattern = options.groupHeaderPattern;
				highlightPoints = options.highlightPoints;
				OnEndLoading();
			}
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
		"System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public abstract class CrosshairLabelPosition : ChartElement, ICrosshairFreePosition {
		const int DefaultOffset = 12;
		int offsetX = DefaultOffset;
		int offsetY = DefaultOffset;
		protected virtual bool IsMousePosition { get { return true; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string TypeNameSerializable { get { return this.GetType().Name; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairLabelPositionOffsetX"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairLabelPosition.OffsetX"),
		XtraSerializableProperty
		]
		public int OffsetX {
			get { return offsetX; }
			set {
				if (offsetX != value) {
					SendNotification(new ElementWillChangeNotification(this));
					offsetX = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairLabelPositionOffsetY"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairLabelPosition.OffsetY"),
		XtraSerializableProperty
		]
		public int OffsetY {
			get { return offsetY; }
			set {
				if (offsetY != value) {
					SendNotification(new ElementWillChangeNotification(this));
					offsetY = value;
					RaiseControlChanged();
				}
			}
		}
		#region XtraSeralizing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "OffsetX":
					return ShouldSerializeOffsetX();
				case "OffsetY":
					return ShouldSerializeOffsetY();
				case "TypeNameSerializable":
					return true;
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeOffsetX() {
			return offsetX != DefaultOffset;
		}
		void ResetOffsetX() {
			OffsetX = DefaultOffset;
		}
		bool ShouldSerializeOffsetY() {
			return offsetY != DefaultOffset;
		}
		void ResetOffsetY() {
			OffsetY = DefaultOffset;
		}
		bool ShouldSerializeTypeNameSerializable() {
			return !(this is CrosshairMousePosition);
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeTypeNameSerializable() || ShouldSerializeOffsetX() || ShouldSerializeOffsetY();
		}
		#endregion
		#region ICrosshairFreePosition implementation
		bool ICrosshairFreePosition.IsMousePosition { get { return IsMousePosition; } }
		GRealRect2D ICrosshairFreePosition.DockBounds { get { return GetDockBounds(); } }
		DockCornerCore ICrosshairFreePosition.DockCorner { get { return GetDockCorner(); } }
		GRealPoint2D ICrosshairFreePosition.Offset { get { return new GRealPoint2D(offsetX, offsetY); } }
		#endregion
		protected virtual GRealRect2D GetDockBounds() { 
			return GRealRect2D.Empty;
		}
		protected virtual DockCornerCore GetDockCorner() { 
			return DockCornerCore.TopRight;
		}
		protected internal virtual void OnEndLoading() {
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			CrosshairLabelPosition position = obj as CrosshairLabelPosition;
			if (position != null) {
				offsetX = position.offsetX;
				offsetY = position.offsetY;
			}
		}
	}
	[
	TypeConverter(typeof(CrosshairMousePositionTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
		"System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class CrosshairMousePosition : CrosshairLabelPosition {
		public CrosshairMousePosition() {
		}
		protected override ChartElement CreateObjectForClone() {
			return new CrosshairMousePosition();
		}
	}
	[
	TypeConverter(typeof(CrosshairFreePositionTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
		"System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class CrosshairFreePosition : CrosshairLabelPosition {
		const DockCorner DefaultDockCorner = DockCorner.LeftTop;
		IDockTarget dockTarget = null;
		DockCorner dockCorner = DefaultDockCorner;
		string deserializedDockTargetName;
		protected override bool IsMousePosition { get { return false; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairFreePositionDockCorner"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairFreePosition.DockCorner"),
		XtraSerializableProperty
		]
		public DockCorner DockCorner {
			get { return dockCorner; }
			set {
				if (dockCorner != value) {
					SendNotification(new ElementWillChangeNotification(this));
					dockCorner = value;
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string DockTargetName {
			get {
				if (dockTarget != null)
					return dockTarget.Name;
				return string.Empty;
			}
			set { deserializedDockTargetName = value; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CrosshairFreePositionDockTarget"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CrosshairFreePosition.DockTarget"),
		TypeConverter(typeof(CrosshairFreePositionDockTargetTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public ChartElement DockTarget {
			get { return (ChartElement)dockTarget; }
			set {
				if (value != dockTarget) {
					SendNotification(new ElementWillChangeNotification(this));
					if (value != null)
						CommonUtils.CheckDockTarget(value, this);
					dockTarget = value as IDockTarget;
					RaiseControlChanged();
				}
			}
		}
		public CrosshairFreePosition() {
		}
		#region XtraSeralizing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "DockCorner":
					return ShouldSerializeDockCorner();
				case "DockTargetName":
					return ShouldSerializeDockTargetName();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeDockCorner() {
			return dockCorner != DefaultDockCorner;
		}
		void ResetDockCorner() {
			DockCorner = DefaultDockCorner;
		}
		bool ShouldSerializeDockTargetName() {
			return dockTarget != null;
		}
		bool ShouldSerializeDockTarget() {
			return dockTarget != null;
		}
		void ResetDockTarget() {
			DockTarget = null;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeDockCorner() || ShouldSerializeDockTargetName();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new CrosshairFreePosition();
		}
		protected override GRealRect2D GetDockBounds() {
			XYDiagramPaneBase pane = dockTarget as XYDiagramPaneBase;
			if (pane == null || !pane.LastMappingBounds.HasValue)
				return GRealRect2D.Empty;
			Rectangle mappingBounds = pane.LastMappingBounds.Value;
			return new GRealRect2D(mappingBounds.X, mappingBounds.Y, mappingBounds.Width, mappingBounds.Height);
		}
		protected override DockCornerCore GetDockCorner() {
			return (DockCornerCore)DockCorner;
		}
		protected internal override void OnEndLoading() {
			XYDiagram2D diagram = CommonUtils.GetXYDiagram2D(this);
			if (diagram == null || deserializedDockTargetName == String.Empty)
				return;
			dockTarget = diagram.FindPaneByName(deserializedDockTargetName);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			CrosshairFreePosition position = obj as CrosshairFreePosition;
			if (position != null) {
				dockTarget = position.dockTarget;
				DockTargetName = position.DockTargetName;
				dockCorner = position.dockCorner;
			}
		}
	}
}
