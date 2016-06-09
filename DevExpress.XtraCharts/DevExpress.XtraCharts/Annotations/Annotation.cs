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
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	public abstract class Annotation : ChartElementNamed, IAnnotationShapeOwner, ISupportInitialize, IXtraSupportCreateContentPropertyValue, IXtraSupportAfterDeserialize, IAnnotationDragElement, IBackground, INamedElement {
		internal static Point StartOffset {
			get { return new Point(10, 10); }
		}
		const bool DefaultVisible = true;
		const int DefaultAngle = 0;
		const int DefaultPadding = 3;
		const int DefaultShapeFillet = 5;
		const AnnotationConnectorStyle DefaultConnectorStyle = AnnotationConnectorStyle.Tail;
		const ShapeKind DefaultShapeKind = ShapeKind.RoundedRectangle;
		const bool DefaultLabelMode = false;
		const bool DefaultRuntimeAnchoring = false;
		const bool DefaultRuntimeMoving = false;
		const bool DefaultRuntimeResizing = false;
		const bool DefaultRuntimeRotation = false;
		const int DefaultZOrder = 0;
		readonly Shadow shadow;
		readonly RectangleIndents padding;
		bool visible = DefaultVisible;
		bool loading = false;
		int angle = DefaultAngle;
		int shapeFillet = DefaultShapeFillet;
		int height;
		int width;
		AnnotationShape shape;
		AnnotationConnectorStyle connectorStyle = DefaultConnectorStyle;
		AnnotationAnchorPoint anchorPoint;
		readonly AspxSerializerWrapper<AnnotationAnchorPoint> anchorPointSerializerWrapper;
		AnnotationShapePosition shapePosition;
		readonly AspxSerializerWrapper<AnnotationShapePosition> shapePositionSerializerWrapper;
		Color backColor;
		CustomBorder border;
		DiagramPoint lastLocation = new DiagramPoint();
		DiagramPoint lastAnchorPosition = new DiagramPoint();
		ShapeKind shapeKind = DefaultShapeKind;
		RectangleFillStyle fillStyle;
		bool labelMode = DefaultLabelMode;
		bool runtimeAnchoring = DefaultRuntimeAnchoring;
		bool runtimeMoving = DefaultRuntimeMoving;
		bool runtimeResizing = DefaultRuntimeResizing;
		bool runtimeRotation = DefaultRuntimeRotation;
		int zOrder = DefaultZOrder;
		bool runtimeOperationSelect;
		internal bool ScrollingSupported {
			get {
				return (anchorPoint is PaneAnchorPoint || anchorPoint is SeriesPointAnchorPoint) && shapePosition is RelativePosition;
			}
		}
		internal bool LabelModeSupported {
			get { return AnnotationLabelModeUtils.IsLabelModeSupported(this); }
		}
		internal DiagramPoint LastLocation {
			get { return lastLocation; }
		}
		internal DiagramPoint LastAnchorPosition {
			get { return lastAnchorPosition; }
		}
		protected AnnotationShape Shape {
			get { return shape; }
		}
		protected internal override bool Loading {
			get { return loading || base.Loading; }
		}
		protected internal abstract bool ActualAutoSize {
			get;
			set;
		}
		protected internal abstract string NamePrefix {
			get;
		}
		protected abstract AnnotationAppearance Appearance {
			get;
		}
		static Color DefaultBackColor {
			get { return Color.Empty; }
		}
		internal Color ActualBackColor {
			get {
				if (backColor != Color.Empty)
					return backColor;
				else {
					if (Appearance != null) {
						return Appearance.BackColor;
					}
					return Color.Empty;
				}
			}
		}
		internal Color ActualBorderColor {
			get {
				Color color;
				if (border.Color != Color.Empty)
					color = border.Color;
				else {
					if (Appearance != null)
						color = Appearance.BorderColor;
					else
						color = Color.Empty;
				}
				return GraphicUtils.GetColor(color, ((IHitTest)this).State);
			}
		}
		internal RectangleFillStyle ActualFillStyle {
			get {
				if (fillStyle.FillMode == FillMode.Empty && Appearance != null)
					return Appearance.FillStyle;
				else
					return fillStyle;
			}
		}
		internal bool ActualLabelMode {
			get { return LabelModeSupported && labelMode; }
		}
		internal bool CanPerformAnchoring {
			get { return CanPerformAnnotationOperation(runtimeAnchoring) && !(anchorPoint is SeriesPointAnchorPoint); }
		}
		internal bool CanPerformMoving {
			get { return CanPerformAnnotationOperation(runtimeMoving); }
		}
		internal bool CanPerformResizing {
			get { return CanPerformAnnotationOperation(runtimeResizing); }
		}
		internal bool CanPerformRotation {
			get { return CanPerformAnnotationOperation(runtimeRotation); }
		}
		internal bool RuntimeOperationSelect {
			get { return runtimeOperationSelect; }
			set { runtimeOperationSelect = value; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationAngle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.Angle"),
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public int Angle {
			get { return angle; }
			set {
				if (value < -360 || value > 360)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectTextAnnotationAngle));
				if (value != angle) {
					SendNotification(new ElementWillChangeNotification(this));
					angle = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.BackColor"),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public Color BackColor {
			get { return backColor; }
			set {
				if (value == backColor)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				backColor = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationBorder"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.Border"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public CustomBorder Border {
			get { return border; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationConnectorStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.ConnectorStyle"),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public AnnotationConnectorStyle ConnectorStyle {
			get { return connectorStyle; }
			set {
				if (value == connectorStyle)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				connectorStyle = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.FillStyle"),
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
	DevExpressXtraChartsLocalizedDescription("AnnotationLabelMode"),
#endif
		Category(Categories.Behavior),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.LabelMode"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool LabelMode {
			get { return labelMode; }
			set {
				if (value != labelMode) {
					SendNotification(new ElementWillChangeNotification(this));
					labelMode = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationPadding"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.Padding"),
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
	DevExpressXtraChartsLocalizedDescription("AnnotationShapeKind"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.ShapeKind"),
		Category(Categories.Appearance),
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public ShapeKind ShapeKind {
			get { return shapeKind; }
			set {
				if (value == shapeKind)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				shapeKind = value;
				CreateShape();
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationShapeFillet"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.ShapeFillet"),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public int ShapeFillet {
			get { return shapeFillet; }
			set {
				if (value < 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectShapeFillet));
				if (value == shapeFillet)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				shapeFillet = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.Visible"),
		Category(Categories.Appearance),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return visible; }
			set {
				if (value == visible)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				visible = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationAnchorPoint"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.AnchorPoint"),
		Category(Categories.Layout),
		TypeConverter(typeof(AnnotationAnchorPointTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor("DevExpress.XtraCharts.Design.AnnotationAnchorPointEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public AnnotationAnchorPoint AnchorPoint {
			get { return anchorPoint; }
			set {
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAnchorPoint));
				SendNotification(new ElementWillChangeNotification(this));
				SetAnchorPoint(value);
				RaiseControlChanged();
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		NestedTagProperty
		]
		public IList AnchorPointSerializable {
			get { return anchorPointSerializerWrapper; }
		}
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
	DevExpressXtraChartsLocalizedDescription("AnnotationShapePosition"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.ShapePosition"),
		Category(Categories.Layout),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor("DevExpress.XtraCharts.Design.AnnotationShapePositionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public AnnotationShapePosition ShapePosition {
			get { return shapePosition; }
			set {
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectShapePosition));
				SendNotification(new ElementWillChangeNotification(this));
				SetShapePosition(value);
				RaiseControlChanged();
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		NestedTagProperty
		]
		public IList ShapePositionSerializable {
			get { return shapePositionSerializerWrapper; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationShadow"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.Shadow"),
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
	DevExpressXtraChartsLocalizedDescription("AnnotationHeight"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.Height"),
		Category(Categories.Layout),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		ResetNotSupported
		]
		public int Height {
			get { return height; }
			set {
				if (value <= 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAnnotationHeight));
				if (height == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				SetHeight(value);
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationWidth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.Width"),
		Category(Categories.Layout),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		ResetNotSupported
		]
		public int Width {
			get { return width; }
			set {
				if (value <= 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAnnotationWidth));
				if (width == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				SetWidth(value);
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationRuntimeAnchoring"),
#endif
		Category(Categories.Behavior),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.RuntimeAnchoring"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool RuntimeAnchoring {
			get { return runtimeAnchoring; }
			set {
				if (value != runtimeAnchoring) {
					SendNotification(new ElementWillChangeNotification(this));
					runtimeAnchoring = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationRuntimeMoving"),
#endif
		Category(Categories.Behavior),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.RuntimeMoving"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool RuntimeMoving {
			get { return runtimeMoving; }
			set {
				if (value != runtimeMoving) {
					SendNotification(new ElementWillChangeNotification(this));
					runtimeMoving = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationRuntimeResizing"),
#endif
		Category(Categories.Behavior),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.RuntimeResizing"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool RuntimeResizing {
			get { return runtimeResizing; }
			set {
				if (value != runtimeResizing) {
					SendNotification(new ElementWillChangeNotification(this));
					runtimeResizing = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationRuntimeRotation"),
#endif
		Category(Categories.Behavior),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.RuntimeRotation"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool RuntimeRotation {
			get { return runtimeRotation; }
			set {
				if (value != runtimeRotation) {
					SendNotification(new ElementWillChangeNotification(this));
					runtimeRotation = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AnnotationZOrder"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Annotation.ZOrder"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int ZOrder {
			get { return zOrder; }
			set {
				if (value < 0 || value >= 100)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAnnotationZOrder));
				if (value != zOrder) {
					SendNotification(new ElementWillChangeNotification(this));
					zOrder = value;
					RaiseControlChanged();
				}
			}
		}
		public Annotation(string name)
			: base(name) {
			backColor = DefaultBackColor;
			border = new CustomBorder(this, true, Color.Empty);
			fillStyle = new RectangleFillStyle(this);
			padding = new RectangleIndents(this, DefaultPadding);
			shadow = new Shadow(this);
			SetShapePosition(new FreePosition());
			SetAnchorPoint(new ChartAnchorPoint());
			CreateShape();
			anchorPointSerializerWrapper = new AspxSerializerWrapper<AnnotationAnchorPoint>(delegate() { return AnchorPoint; },
				delegate(AnnotationAnchorPoint value) { AnchorPoint = value; });
			shapePositionSerializerWrapper = new AspxSerializerWrapper<AnnotationShapePosition>(delegate() { return ShapePosition; },
				delegate(AnnotationShapePosition value) { ShapePosition = value; });
		}
		#region IBackground implementation
		FillStyleBase IBackground.FillStyle { get { return FillStyle; } }
		BackgroundImage IBackground.BackImage { get { return null; } }
		bool IBackground.BackImageSupported { get { return false; } }
		#endregion
		#region IAnnotationShapeOwner implementation
		bool IAnnotationShapeOwner.BorderVisible { get { return Border.ActualVisibility; } }
		int IAnnotationShapeOwner.BorderThickness { get { return Border.ActualThickness; } }
		Color IAnnotationShapeOwner.BorderColor { get { return ActualBorderColor; } }
		RectangleFillStyle IAnnotationShapeOwner.FillStyle { get { return ActualFillStyle; } }
		Color IAnnotationShapeOwner.BackColor { get { return ActualBackColor; } }
		int IAnnotationShapeOwner.ShapeFillet { get { return shapeFillet; } }
		Shadow IAnnotationShapeOwner.Shadow { get { return Shadow; } }
		AnnotationConnectorStyle IAnnotationShapeOwner.ConnectorStyle { get { return ConnectorStyle; } }
		int IAnnotationShapeOwner.Angle { get { return Angle; } }
		#endregion
		#region IAnnotationNamedElement implementation
		string INamedElement.Name { get { return Name; } }
		string INamedElement.NamePrefix { get { return NamePrefix; } }
		#endregion
		#region IHitTest implementation
		HitTestState hitTestState = new HitTestState();
		object IHitTest.Object { get { return this; } }
		HitTestState IHitTest.State { get { return hitTestState; } }
		#endregion
		#region ISupportInitialize implementation
		void ISupportInitialize.BeginInit() {
			this.loading = true;
		}
		void ISupportInitialize.EndInit() {
			this.loading = false;
		}
		#endregion
		#region XtraSerializing
		object IXtraSupportCreateContentPropertyValue.Create(XtraItemEventArgs e) {
			return e.Item.Name == "ShapePosition" || e.Item.Name == "AnchorPoint" ? XtraSerializingUtils.GetContentPropertyInstance(e, SerializationUtils.ExecutingAssembly, SerializationUtils.PublicNamespace) : null;
		}
		void IXtraSupportAfterDeserialize.AfterDeserialize(XtraItemEventArgs e) {
			if (e.Item.Name == "ShapePosition")
				ShapePosition = (AnnotationShapePosition)e.Item.Value;
			if (e.Item.Name == "AnchorPoint")
				AnchorPoint = (AnnotationAnchorPoint)e.Item.Value;
		}
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "TypeNameSerializable")
				return true;
			if (propertyName == "ShapePosition")
				return true;
			if (propertyName == "AnchorPoint")
				return true;
			if (propertyName == "Height")
				return ShouldSerializeHeight();
			if (propertyName == "Width")
				return ShouldSerializeWidth();
			if (propertyName == "Shadow")
				return ShouldSerializeShadow();
			if (propertyName == "Angle")
				return ShouldSerializeAngle();
			if (propertyName == "Visible")
				return ShouldSerializeVisible();
			if (propertyName == "ShapeKind")
				return ShouldSerializeShapeKind();
			if (propertyName == "BackColor")
				return ShouldSerializeBackColor();
			if (propertyName == "ShapeFillet")
				return ShouldSerializeShapeFillet();
			if (propertyName == "ConnectorStyle")
				return ShouldSerializeConnectorStyle();
			if (propertyName == "Padding")
				return ShouldSerializePadding();
			if (propertyName == "RuntimeAnchoring")
				return ShouldSerializeRuntimeAnchoring();
			if (propertyName == "RuntimeMoving")
				return ShouldSerializeRuntimeMoving();
			if (propertyName == "RuntimeResizing")
				return ShouldSerializeRuntimeResizing();
			if (propertyName == "RuntimeRotation")
				return ShouldSerializeRuntimeRotation();
			if (propertyName == "ZOrder")
				return ShouldSerializeZOrder();
			if (propertyName == "LabelMode")
				return ShouldSerializeLabelMode();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeHeight() {
			return !ActualAutoSize;
		}
		bool ShouldSerializeWidth() {
			return !ActualAutoSize;
		}
		bool ShouldSerializeAnchorPoint() {
			return anchorPoint != null && ChartContainer != null && ChartContainer.ControlType != ChartContainerType.WebControl;
		}
		void ResetAnchorPoint() {
			AnchorPoint = null;
		}
		bool ShouldSerializeAnchorPointSerializable() {
			return anchorPoint != null && ChartContainer != null && ChartContainer.ControlType == ChartContainerType.WebControl;
		}
		bool ShouldSerializeShapePosition() {
			return shapePosition != null && ChartContainer != null && ChartContainer.ControlType != ChartContainerType.WebControl;
		}
		void ResetShapePosition() {
			ShapePosition = null;
		}
		bool ShouldSerializeShapePositionSerializable() {
			return shapePosition != null && ChartContainer != null && ChartContainer.ControlType == ChartContainerType.WebControl;
		}
		bool ShouldSerializeShadow() {
			return shadow.ShouldSerialize();
		}
		bool ShouldSerializeAngle() {
			return angle != DefaultAngle;
		}
		void ResetAngle() {
			Angle = DefaultAngle;
		}
		bool ShouldSerializeVisible() {
			return visible != DefaultVisible;
		}
		void ResetVisible() {
			Visible = DefaultVisible;
		}
		bool ShouldSerializeShapeKind() {
			return shapeKind != DefaultShapeKind;
		}
		void ResetShapeKind() {
			ShapeKind = DefaultShapeKind;
		}
		bool ShouldSerializeBackColor() {
			return backColor != DefaultBackColor;
		}
		void ResetBackColor() {
			BackColor = DefaultBackColor;
		}
		bool ShouldSerializeShapeFillet() {
			return shapeKind == ShapeKind.RoundedRectangle && shapeFillet != DefaultShapeFillet;
		}
		void ResetShapeFillet() {
			ShapeFillet = DefaultShapeFillet;
		}
		bool ShouldSerializeConnectorStyle() {
			return connectorStyle != DefaultConnectorStyle;
		}
		void ResetConnectorStyle() {
			ConnectorStyle = DefaultConnectorStyle;
		}
		bool ShouldSerializePadding() {
			return padding.ShouldSerialize();
		}
		bool ShouldSerializeRuntimeAnchoring() {
			return runtimeAnchoring != DefaultRuntimeAnchoring;
		}
		void ResetRuntimeAnchoring() {
			RuntimeAnchoring = DefaultRuntimeAnchoring;
		}
		bool ShouldSerializeRuntimeMoving() {
			return runtimeMoving != DefaultRuntimeMoving;
		}
		void ResetRuntimeMoving() {
			RuntimeMoving = DefaultRuntimeMoving;
		}
		bool ShouldSerializeRuntimeResizing() {
			return runtimeResizing != DefaultRuntimeResizing;
		}
		void ResetRuntimeResizing() {
			runtimeResizing = DefaultRuntimeResizing;
		}
		bool ShouldSerializeRuntimeRotation() {
			return runtimeRotation != DefaultRuntimeRotation;
		}
		void ResetRuntimeRotation() {
			RuntimeRotation = DefaultRuntimeRotation;
		}
		bool ShouldSerializeZOrder() {
			return zOrder != DefaultZOrder;
		}
		void ResetZOrder() {
			ZOrder = DefaultZOrder;
		}
		bool ShouldSerializeLabelMode() {
			return labelMode != DefaultLabelMode;
		}
		void ResetLabelMode() {
			LabelMode = DefaultLabelMode;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() ||
				ShouldSerializeHeight() ||
				ShouldSerializeWidth() ||
				ShouldSerializeAnchorPoint() ||
				ShouldSerializeAnchorPointSerializable() ||
				ShouldSerializeShapePosition() ||
				ShouldSerializeShapePositionSerializable() ||
				ShouldSerializeShadow() ||
				ShouldSerializeAngle() ||
				ShouldSerializeVisible() ||
				ShouldSerializeShapeKind() ||
				ShouldSerializeBackColor() ||
				ShouldSerializeShapeFillet() ||
				ShouldSerializeConnectorStyle() ||
				ShouldSerializeRuntimeAnchoring() ||
				ShouldSerializeRuntimeMoving() ||
				ShouldSerializeRuntimeResizing() ||
				ShouldSerializeRuntimeRotation() ||
				ShouldSerializeZOrder() ||
				ShouldSerializeLabelMode() ||
				ShouldSerializePadding();
		}
		#endregion
		#region IAnnotationDragElement Members
		void IAnnotationDragElement.DoSelect() {
			runtimeOperationSelect = true;
		}
		#endregion
		bool CanPerformAnnotationOperation(bool runtimeOperationEnabled) {
			IChartContainer chartContainer = CommonUtils.FindChartContainer(this);
			if (!ActualLabelMode && chartContainer != null) {
				return chartContainer.DesignMode ? chartContainer.ControlType != ChartContainerType.WebControl :
				chartContainer.ControlType == ChartContainerType.WinControl && !chartContainer.Chart.Is3DDiagram && runtimeOperationEnabled;
			}
			else
				return false;
		}
		void AssignAnchorPoint(AnnotationAnchorPoint templateAnchorPoint) {
			if (!anchorPoint.GetType().Equals(templateAnchorPoint.GetType()))
				SetAnchorPoint((AnnotationAnchorPoint)templateAnchorPoint.Clone());
			else
				anchorPoint.Assign(templateAnchorPoint);
		}
		void AssignShapePosition(AnnotationShapePosition templateShapePosition) {
			if (!shapePosition.GetType().Equals(templateShapePosition.GetType()))
				SetShapePosition((AnnotationShapePosition)templateShapePosition.Clone());
			else
				shapePosition.Assign(templateShapePosition);
		}
		void CreateShape() {
			shape = AnnotationShape.CreateInstance(this);
		}
		void SetAnchorPoint(AnnotationAnchorPoint value) {
			anchorPoint = value;
			anchorPoint.Owner = this;
		}
		void SetShapePosition(AnnotationShapePosition value) {
			shapePosition = value;
			shapePosition.Owner = this;
		}
		void DisposeBorder() {
			if (border != null) {
				border.Dispose();
				border = null;
			}
		}
		void DisposeFillStyle() {
			if (fillStyle != null) {
				fillStyle.Dispose();
				fillStyle = null;
			}
		}
		protected abstract Size CalculateInnerSize();
		protected override void Dispose(bool disposing) {
			if (disposing && !IsDisposed) {
				DisposeBorder();
				DisposeFillStyle();
			}
			base.Dispose(disposing);
		}
		protected void SaveLayout(DiagramPoint location, DiagramPoint anchorPosition) {
			this.lastAnchorPosition = anchorPosition;
			this.lastLocation = location;
		}
		internal bool CanDrag() {
			if (visible && (CanPerformMoving || CanPerformResizing || CanPerformAnchoring || CanPerformRotation))
				return true;
			return false;
		}
		internal void UpdateSize() {
			if (!ActualAutoSize)
				return;
			Size innerSize = CalculateInnerSize();
			Size outerSize = Shape.CalcOuterSize(innerSize);
			outerSize.Width += Padding.Left + Padding.Right;
			outerSize.Height += Padding.Top + Padding.Bottom;
			if (Border != null && Border.ActualVisibility) {
				outerSize.Height += Border.ActualThickness * 2;
				outerSize.Width += Border.ActualThickness * 2;
			}
			height = outerSize.Height;
			width = outerSize.Width;
		}
		internal void OnEndLoading() {
			if (anchorPoint != null)
				anchorPoint.OnEndLoading();
			if (shapePosition != null)
				shapePosition.OnEndLoading();
		}
		internal void SetHeight(int value) {
			if (value > 0) {
				height = value;
				ActualAutoSize = false;
			}
		}
		internal void SetWidth(int value) {
			if (value > 0) {
				width = value;
				ActualAutoSize = false;
			}
		}
		internal void SetAngle(int value) {
			angle = value;
		}
		protected internal abstract AnnotationViewData CalculateViewData(TextMeasurer textMeasurer, AnnotationLayout shapeLayout, AnnotationLayout anchorPointLayout);
		protected internal abstract AnnotationViewData CalculateViewData(TextMeasurer textMeasurer, AnnotationLayout shapeLayout, AnnotationLayout anchorPointLayout, Rectangle allowedBoundsForAnnotationPlacing);
		protected override bool ProcessChanged(ChartElement sender, ChartUpdateInfoBase changeInfo) {
			UpdateSize();
			return base.ProcessChanged(sender, changeInfo);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Annotation annotation = obj as Annotation;
			if (annotation == null)
				return;
			AssignAnchorPoint(annotation.anchorPoint);
			AssignShapePosition(annotation.shapePosition);
			shadow.Assign(annotation.shadow);
			height = annotation.height;
			width = annotation.width;
			angle = annotation.angle;
			connectorStyle = annotation.ConnectorStyle;
			backColor = annotation.backColor;
			visible = annotation.visible;
			shapeKind = annotation.shapeKind;
			shapeFillet = annotation.shapeFillet;
			runtimeAnchoring = annotation.runtimeAnchoring;
			runtimeMoving = annotation.runtimeMoving;
			runtimeResizing = annotation.runtimeResizing;
			runtimeRotation = annotation.runtimeRotation;
			zOrder = annotation.zOrder;
			labelMode = annotation.labelMode;
			border.Assign(annotation.border);
			fillStyle.Assign(annotation.fillStyle);
			padding.Assign(annotation.padding);
			if (!Loading) {
				anchorPoint.OnEndLoading();
				shapePosition.OnEndLoading();
			}
			CreateShape();
		}
	}
}
