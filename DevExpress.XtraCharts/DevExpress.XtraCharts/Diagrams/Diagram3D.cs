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
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.GLGraphics;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class Diagram3D : Diagram, IScrollingZoomingOptions {
		const int MaxZoomPercent = 500;
		const double ZoomPercentStep = 1.5;
		const double MaxScrollPercent = 100.0;
		const RotationType DefaultRotationType = RotationType.UseMouseAdvanced;
		const RotationOrder DefaultRotationOrder = RotationOrder.XYZ;
		const bool DefaultPerspectiveEnabled = true;
		const bool DefaultRuntimeRotation = false;
		const bool DefaultRuntimeZooming = false;
		const bool DefaultRuntimeScrolling = false;
		internal const double Epsilon = 0.01;
		public const int DefaultZoomPercent = 100;
		public const double DefaultScrollPercent = 0.0;
		readonly ScrollingOptions scrollingOptions;
		readonly ZoomingOptions zoomingOptions;
		readonly RotationOptions rotationOptions;
		bool perspectiveEnabled = DefaultPerspectiveEnabled;
		bool runtimeRotation = DefaultRuntimeRotation;
		bool runtimeZooming = DefaultRuntimeZooming;
		bool runtimeScrolling = DefaultRuntimeScrolling;
		double[] rotationMatrix;
		double horizontalScrollPercent = DefaultScrollPercent;
		double verticalScrollPercent = DefaultScrollPercent;
		int rotationAngleX;
		int rotationAngleY;
		int rotationAngleZ;
		int perspectiveAngle;
		int zoomPercent = DefaultZoomPercent;
		RotationType rotationType = DefaultRotationType;
		RotationOrder rotationOrder = DefaultRotationOrder;
		Rectangle lastDiagramBounds;
		bool InternalCanRotateWithTouch { get { return RuntimeRotation && RotationOptions.UseTouchDevice && RotationType != RotationType.UseAngles; } }
		bool InternalCanRotateWithMouse { get { return (Chart.Container.DesignMode || RuntimeRotation) && RotationType != RotationType.UseAngles && RotationOptions.UseMouse; } }
		bool InternalCanZoom { get { return Chart.Container.DesignMode || RuntimeZooming; } }
		bool InternalCanScroll { 
			get { return (Chart.Container.DesignMode || runtimeScrolling) && lastDiagramBounds.Width != 0 && lastDiagramBounds.Height != 0; }
		}
		protected virtual int DefaultRotationAngleX { get { return 20; } }
		protected virtual int DefaultRotationAngleY { get { return -40; } }
		protected virtual int DefaultRotationAngleZ { get { return 0; } }
		protected virtual int DefaultPerspectiveAngle { get { return 50; } }
		protected internal override bool CanZoomIn { get { return InternalCanZoom && zoomPercent < MaxZoomPercent; } }
		protected internal override bool CanZoomInViaRect { get { return false; } }
		protected internal override bool CanZoomOut { get { return InternalCanZoom && zoomPercent > 1; } }
		protected internal override bool CanZoomWithTouch { get { return ZoomingOptions.UseTouchDevice; } }
		protected internal override bool CanPan { get { return RotationOptions.UseTouchDevice; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DRotationType"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.RotationType"),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		NonTestableProperty(),
		XtraSerializableProperty
		]
		public RotationType RotationType {
			get { return rotationType; }
			set {
				if (value != rotationType) {
					SendNotification(new ElementWillChangeNotification(this));
					if (!Loading && value == RotationType.UseMouseStandard) {
						rotationAngleZ = 0;
						rotationType = RotationType.UseAngles;
						CalculateRotationMatrix();
					}
					rotationType = value;
					CalculateRotationMatrix();
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DRotationOrder"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.RotationOrder"),
		Category(Categories.Behavior),
		NonTestableProperty(),
		XtraSerializableProperty
		]
		public RotationOrder RotationOrder {
			get { return rotationOrder; }
			set {
				if (value != rotationOrder) {
					SendNotification(new ElementWillChangeNotification(this));
					rotationOrder = value;
					CalculateRotationMatrix();
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DRotationAngleX"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.RotationAngleX"),
		Category(Categories.Behavior),
		NonTestableProperty(),
		XtraSerializableProperty
		]
		public int RotationAngleX {
			get { return rotationAngleX; }
			set {
				if (value != rotationAngleX) {
					SendNotification(new ElementWillChangeNotification(this));
					rotationAngleX = value;
					CalculateRotationMatrix();
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DRotationAngleY"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.RotationAngleY"),
		Category(Categories.Behavior),
		NonTestableProperty(),
		XtraSerializableProperty
		]
		public int RotationAngleY {
			get { return rotationAngleY; }
			set {
				if (value != rotationAngleY) {
					SendNotification(new ElementWillChangeNotification(this));
					rotationAngleY = value;
					CalculateRotationMatrix();
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DRotationAngleZ"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.RotationAngleZ"),
		Category(Categories.Behavior),
		NonTestableProperty(),
		XtraSerializableProperty
		]
		public int RotationAngleZ {
			get { return rotationAngleZ; }
			set {
				if (value != rotationAngleZ) {
					SendNotification(new ElementWillChangeNotification(this));
					rotationAngleZ = value;
					CalculateRotationMatrix();
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public double[] RotationMatrix {
			get { 
				if (rotationMatrix == null)
					CalculateRotationMatrix();
				return rotationMatrix; 
			}
			set {
				CheckRotationMatrix(value);
				SendNotification(new ElementWillChangeNotification(this));
				rotationMatrix = value;
				RaiseControlChanged();
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty(),
		XtraSerializableProperty
		]
		public string RotationMatrixSerializable {
			get {
				string result = String.Empty;
				foreach (double num in RotationMatrix)
					result += num.ToString(NumberFormatInfo.InvariantInfo) + ";";
				return result.Substring(0, result.Length - 1);
			}
			set {
				string[] strings = value.Split(new char[] { ';' });
				double[] matrix = new double[strings.Length];
				for (int i = 0; i < strings.Length; i++)
					matrix[i] = Double.Parse(strings[i], NumberFormatInfo.InvariantInfo);
				RotationMatrix = matrix;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DPerspectiveEnabled"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.PerspectiveEnabled"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool PerspectiveEnabled {
			get { return perspectiveEnabled; }
			set {
				if (value != perspectiveEnabled) {
					SendNotification(new ElementWillChangeNotification(this));
					perspectiveEnabled = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DPerspectiveAngle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.PerspectiveAngle"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int PerspectiveAngle {
			get { return perspectiveAngle; }
			set {
				if (value != perspectiveAngle) {
					if (value < 0 || value >= 180)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPerspectiveAngle));
					SendNotification(new ElementWillChangeNotification(this));
					perspectiveAngle = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DZoomPercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.ZoomPercent"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int ZoomPercent {
			get { return zoomPercent; }
			set {
				if (value != zoomPercent) {
					if (ClampZoomPercent(value) != value)
						throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectZoomPercent), MaxZoomPercent));
					int oldZoomPercent = zoomPercent;
					SendNotification(new ElementWillChangeNotification(this));
					zoomPercent = value;
					RaiseControlChanged();
					if (Chart != null && !Chart.Loading) {
						ChartZoomEventType type = zoomPercent > oldZoomPercent || zoomPercent == MaxZoomPercent ? ChartZoomEventType.ZoomIn : ChartZoomEventType.ZoomOut;
						Chart.ContainerAdapter.OnZoom3D(new ChartZoom3DEventArgs(oldZoomPercent, zoomPercent, type));
					}
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DHorizontalScrollPercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.HorizontalScrollPercent"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double HorizontalScrollPercent {
			get { return horizontalScrollPercent; }
			set {
				if (value != horizontalScrollPercent) {
					if (!CheckScrollPercent(value))
						throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectHorizontalScrollPercent), MaxScrollPercent));
					SendNotification(new ElementWillChangeNotification(this));
					SetScroll(value, verticalScrollPercent);
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DVerticalScrollPercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.VerticalScrollPercent"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double VerticalScrollPercent {
			get { return verticalScrollPercent; }
			set {
				if (value != verticalScrollPercent) {
					if (!CheckScrollPercent(value))
						throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectVerticalScrollPercent), MaxScrollPercent));
					SendNotification(new ElementWillChangeNotification(this));
					SetScroll(horizontalScrollPercent, value);
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DRuntimeRotation"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.RuntimeRotation"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
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
	DevExpressXtraChartsLocalizedDescription("Diagram3DRuntimeZooming"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.RuntimeZooming"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool RuntimeZooming {
			get { return runtimeZooming; }
			set {
				if (value != runtimeZooming) {
					SendNotification(new ElementWillChangeNotification(this));
					runtimeZooming = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DRuntimeScrolling"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.RuntimeScrolling"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool RuntimeScrolling {
			get { return runtimeScrolling; }
			set {
				if (value != runtimeScrolling) {
					SendNotification(new ElementWillChangeNotification(this));
					runtimeScrolling = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DScrollingOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.ScrollingOptions"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public ScrollingOptions ScrollingOptions { get { return scrollingOptions; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DZoomingOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.ZoomingOptions"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public ZoomingOptions ZoomingOptions { get { return zoomingOptions; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Diagram3DRotationOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Diagram3D.RotationOptions"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RotationOptions RotationOptions { get { return rotationOptions; } }
		public Diagram3D() {
			rotationAngleX = DefaultRotationAngleX;
			rotationAngleY = DefaultRotationAngleY;
			rotationAngleZ = DefaultRotationAngleZ;
			rotationMatrix = new double[16];
			rotationType = RotationType.UseAngles;
			CalculateRotationMatrix();
			rotationType = DefaultRotationType;
			perspectiveAngle = DefaultPerspectiveAngle;
			scrollingOptions = new ScrollingOptions(this);
			zoomingOptions = new ZoomingOptions(this);
			rotationOptions = new RotationOptions(this);
		}
		#region IScrollingZoomingOptions
		bool IScrollingZoomingOptions.UseKeyboardScrolling { get { return scrollingOptions.UseKeyboard; } }
		bool IScrollingZoomingOptions.UseKeyboardZooming { get { return zoomingOptions.UseKeyboard; } }
		bool IScrollingZoomingOptions.UseKeyboardWithMouseZooming { get { return zoomingOptions.UseKeyboardWithMouse; } }
		bool IScrollingZoomingOptions.UseMouseScrolling { get { return scrollingOptions.UseMouse; } }
		bool IScrollingZoomingOptions.UseMouseWheelZooming { get { return zoomingOptions.UseMouseWheel; } }
		bool IScrollingZoomingOptions.UseScrollBarsScrolling { get { return false; } }
		bool IScrollingZoomingOptions.UseTouchDeviceZooming { get { return zoomingOptions.UseTouchDevice; } }
		bool IScrollingZoomingOptions.UseTouchDevicePanning { get { return rotationOptions.UseTouchDevice; } }
		bool IScrollingZoomingOptions.UseTouchDeviceRotation { get { return rotationOptions.UseTouchDevice; } }
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeRotationType() {
			return RotationType != DefaultRotationType;
		}
		void ResetRotationType() {
			RotationType = DefaultRotationType;
		}
		bool ShouldSerializeAngleRotation() {
			return rotationType == RotationType.UseAngles;
		}
		bool ShouldSerializeRotationOrder() {
			return ShouldSerializeAngleRotation() && (rotationOrder != DefaultRotationOrder);
		}
		void ResetRotationOrder() {
			RotationOrder = DefaultRotationOrder;
		}
		bool ShouldSerializeRotationAngleX() {
			return ShouldSerializeAngleRotation() && rotationAngleX != DefaultRotationAngleX;
		}
		void ResetRotationAngleX() {
			RotationAngleX = DefaultRotationAngleX;
		}
		bool ShouldSerializeRotationAngleY() {
			return ShouldSerializeAngleRotation() && rotationAngleY != DefaultRotationAngleY;
		}
		void ResetRotationAngleY() {
			RotationAngleY = DefaultRotationAngleY;
		}
		bool ShouldSerializeRotationAngleZ() {
			return ShouldSerializeAngleRotation() && rotationAngleZ != DefaultRotationAngleZ;
		}
		void ResetRotationAngleZ() {
			RotationAngleZ = DefaultRotationAngleZ;
		}
		bool ShouldSerializeRotationMatrixSerializable() {
			return !ShouldSerializeAngleRotation();
		}
		bool ShouldSerializePerspectiveEnabled() {
			return perspectiveEnabled != DefaultPerspectiveEnabled;
		}
		void ResetPerspectiveEnabled() {
			PerspectiveEnabled = DefaultPerspectiveEnabled;
		}
		bool ShouldSerializePerspectiveAngle() {
			return perspectiveAngle != DefaultPerspectiveAngle;
		}
		void ResetPerspectiveAngle() {
			PerspectiveAngle = DefaultPerspectiveAngle;
		}
		bool ShouldSerializeZoomPercent() {
			return zoomPercent != DefaultZoomPercent;
		}
		void ResetZoomPercent() {
			ZoomPercent = DefaultZoomPercent;
		}
		bool ShouldSerializeHorizontalScrollPercent() {
			return horizontalScrollPercent != DefaultScrollPercent;
		}
		void ResetHorizontalScrollPercent() {
			HorizontalScrollPercent = DefaultScrollPercent;
		}
		bool ShouldSerializeVerticalScrollPercent() {
			return verticalScrollPercent != DefaultScrollPercent;
		}
		void ResetVerticalScrollPercent() {
			VerticalScrollPercent = DefaultScrollPercent;
		}
		bool ShouldSerializeRuntimeRotation() {
			return runtimeRotation != DefaultRuntimeRotation;;
		}
		void ResetRuntimeRotation() {
			RuntimeRotation = DefaultRuntimeRotation;
		}
		bool ShouldSerializeRuntimeZooming() {
			return runtimeZooming != DefaultRuntimeZooming;
		}
		void ResetRuntimeZooming() {
			RuntimeZooming = DefaultRuntimeZooming;
		}
		bool ShouldSerializeRuntimeScrolling() {
			return runtimeScrolling != DefaultRuntimeScrolling;
		}
		void ResetRuntimeScrolling() {
			RuntimeScrolling = DefaultRuntimeScrolling;
		}
		bool ShouldSerializeScrollingOptions() {
			return scrollingOptions.ShouldSerialize();
		}
		bool ShouldSerializeZoomingOptions() {
			return zoomingOptions.ShouldSerialize();
		}
		bool ShouldSerializeRotationOptions() {
			return rotationOptions.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeRotationType() || ShouldSerializeRotationOrder() || ShouldSerializeRotationAngleX() ||
				ShouldSerializeRotationAngleY() || ShouldSerializeRotationAngleZ() || ShouldSerializeRotationMatrixSerializable() ||
				ShouldSerializePerspectiveEnabled() || ShouldSerializePerspectiveAngle() || ShouldSerializeZoomPercent() ||
				ShouldSerializeHorizontalScrollPercent() || ShouldSerializeVerticalScrollPercent() || ShouldSerializeRuntimeRotation() ||
				ShouldSerializeRuntimeZooming() || ShouldSerializeRuntimeScrolling() || ShouldSerializeScrollingOptions() ||
				ShouldSerializeZoomingOptions() || ShouldSerializeRotationOptions();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "RotationType":
					return ShouldSerializeRotationType();
				case "RotationOrder":
					return ShouldSerializeRotationOrder();
				case "RotationAngleX":
					return ShouldSerializeRotationAngleX();
				case "RotationAngleY":
					return ShouldSerializeRotationAngleY();
				case "RotationAngleZ":
					return ShouldSerializeRotationAngleZ();
				case "RotationMatrixSerializable":
					return ShouldSerializeRotationMatrixSerializable();
				case "PerspectiveEnabled":
					return ShouldSerializePerspectiveEnabled();
				case "PerspectiveAngle":
					return ShouldSerializePerspectiveAngle();
				case "ZoomPercent":
					return ShouldSerializeZoomPercent();
				case "HorizontalScrollPercent":
					return ShouldSerializeHorizontalScrollPercent();
				case "VerticalScrollPercent":
					return ShouldSerializeVerticalScrollPercent();
				case "RuntimeRotation":
					return ShouldSerializeRuntimeRotation();
				case "RuntimeZooming":
					return ShouldSerializeRuntimeZooming();
				case "RuntimeScrolling":
					return ShouldSerializeRuntimeScrolling();
				case "ScrollingOptions":
					return ShouldSerializeScrollingOptions();
				case "ZoomingOptions":
					return ShouldSerializeZoomingOptions();
				case "RotationOptions":
					return ShouldSerializeRotationOptions();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		void CheckRotationMatrix(double[] matrix) {
			if (matrix.Length != 16)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectTransformationMatrix));
			double det = matrix[0] * (matrix[5] * matrix[10] - matrix[6] * matrix[9]) +
						 matrix[1] * (matrix[6] * matrix[8] - matrix[4] * matrix[10]) +
						 matrix[2] * (matrix[4] * matrix[9] - matrix[5] * matrix[8]);
			if (ComparingUtils.CompareDoubles(det, 1.0, Diagram3D.Epsilon) != 0 || 
				ComparingUtils.CompareDoubles(matrix[3], 0.0, Diagram3D.Epsilon) != 0 ||
				ComparingUtils.CompareDoubles(matrix[7], 0.0, Diagram3D.Epsilon) != 0 || 
				ComparingUtils.CompareDoubles(matrix[11], 0.0, Diagram3D.Epsilon) != 0 || 
				ComparingUtils.CompareDoubles(matrix[12], 0.0, Diagram3D.Epsilon) != 0 || 
				ComparingUtils.CompareDoubles(matrix[13], 0.0, Diagram3D.Epsilon) != 0 || 
				ComparingUtils.CompareDoubles(matrix[14], 0.0, Diagram3D.Epsilon) != 0 || 
				ComparingUtils.CompareDoubles(matrix[15], 1.0, Diagram3D.Epsilon) != 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectTransformationMatrix));
		}
		void CalculateRotationMatrix() {
			if (rotationType == RotationType.UseAngles) 
				using (GraphicsCommand command = CreateRotationGraphicsCommand()) {
					double[] projection;
					GLHelper.CalculateMatrices(command, out rotationMatrix, out projection);
				}
		}
		void UpdateRuntimeProperties(Diagram3D diagram) {
			runtimeRotation = diagram.runtimeRotation;
			runtimeZooming = diagram.runtimeZooming;
			runtimeScrolling = diagram.runtimeScrolling;
		}
		void SetScroll(double newHorizontalScrollPercent, double newVerticalScrollPercent) {
			double oldHorizontalScrollPercent = horizontalScrollPercent;
			double oldVerticalScrollPercent = verticalScrollPercent;
			bool isHorizaontal = horizontalScrollPercent != newHorizontalScrollPercent;
			bool isVertical = verticalScrollPercent != newVerticalScrollPercent;
			if (isHorizaontal)
				horizontalScrollPercent = newHorizontalScrollPercent;
			if (isVertical)
				verticalScrollPercent = newVerticalScrollPercent;
			if (Chart != null && !Chart.Loading) {
				ChartScrollDirection scrollDirection = isHorizaontal ? isVertical ? ChartScrollDirection.Both
					: ChartScrollDirection.Horizontal : ChartScrollDirection.Vertical;
				ChartScroll3DEventArgs eventArgs = new ChartScroll3DEventArgs(scrollDirection, oldHorizontalScrollPercent,
					horizontalScrollPercent, oldVerticalScrollPercent, verticalScrollPercent);
				Chart.ContainerAdapter.OnScroll3D(eventArgs);
			}
		}
		int ClampZoomPercent(double value) {
			if (value < 1)
				return 1;
			if (value > MaxZoomPercent)
				return MaxZoomPercent;
			return Convert.ToInt32(value);
		}
		double ClampScrollPercent(double value) {
			if (value < -MaxScrollPercent)
				return -MaxScrollPercent;
			if (value > MaxScrollPercent)
				return MaxScrollPercent;
			return value;
		}
		bool CheckScrollPercent(double value) {
			return ClampScrollPercent(value) == value;
		}
		bool CanRotateAtPoint(Point point) {
			return InternalCanRotateWithMouse && lastDiagramBounds.Contains(point);
		}
		bool CanScrollAtPoint(Point point) {
			return InternalCanScroll && lastDiagramBounds.Contains(point) && scrollingOptions.UseMouse;
		}
		bool PerformScrolling(int dx, int dy) {
			if (!InternalCanScroll)
				return false;
			double newHorizontalScrollPercent = ClampScrollPercent(horizontalScrollPercent + (double)dx / lastDiagramBounds.Width * 10000.0 / zoomPercent);
			double newVerticalScrollPercent = ClampScrollPercent(verticalScrollPercent - (double)dy / lastDiagramBounds.Height * 10000.0 / zoomPercent);
			if (newHorizontalScrollPercent == horizontalScrollPercent && newVerticalScrollPercent == verticalScrollPercent)
				return false;
			SetScroll(newHorizontalScrollPercent, newVerticalScrollPercent);
			return true;
		}
		bool PerformRotation(int dx, int dy) {
			double factor = Math.PI * 0.25;
			double xAngle = dx * factor;
			double yAngle = dy * factor;
			if (rotationType == RotationType.UseMouseStandard)
				GLHelper.PerformRotationStandard(RotationMatrix, xAngle, yAngle);
			else
				GLHelper.PerformRotation(RotationMatrix, xAngle, yAngle);
			return true;
		}
		internal GraphicsCommand CreateRotationGraphicsCommand() {
			if (rotationType != RotationType.UseAngles)
				return new TransformGraphicsCommand(rotationMatrix);
			GraphicsCommand command = new ContainerGraphicsCommand();
			RotateGraphicsCommand xRotationCommand = new RotateGraphicsCommand(rotationAngleX, new DiagramVector(1.0, 0.0, 0.0));
			RotateGraphicsCommand yRotationCommand = new RotateGraphicsCommand(rotationAngleY, new DiagramVector(0.0, 1.0, 0.0));
			RotateGraphicsCommand zRotationCommand = new RotateGraphicsCommand(rotationAngleZ, new DiagramVector(0.0, 0.0, 1.0));
			switch (rotationOrder) {
				case RotationOrder.XYZ:
					command.AddChildCommand(xRotationCommand);
					command.AddChildCommand(yRotationCommand);
					command.AddChildCommand(zRotationCommand);
					break;
				case RotationOrder.XZY:
					command.AddChildCommand(xRotationCommand);
					command.AddChildCommand(zRotationCommand);
					command.AddChildCommand(yRotationCommand);
					break;
				case RotationOrder.YXZ:
					command.AddChildCommand(yRotationCommand);
					command.AddChildCommand(xRotationCommand);
					command.AddChildCommand(zRotationCommand);
					break;
				case RotationOrder.YZX:
					command.AddChildCommand(yRotationCommand);
					command.AddChildCommand(zRotationCommand);
					command.AddChildCommand(xRotationCommand);
					break;
				case RotationOrder.ZXY:
					command.AddChildCommand(zRotationCommand);
					command.AddChildCommand(xRotationCommand);
					command.AddChildCommand(yRotationCommand);
					break;
				case RotationOrder.ZYX:
					command.AddChildCommand(zRotationCommand);
					command.AddChildCommand(yRotationCommand);
					command.AddChildCommand(xRotationCommand);
					break;
			}
			return command;
		}
		protected void UpdateLastDiagramBounds(Rectangle bounds) {
			lastDiagramBounds = bounds;
		}
		protected internal override INativeGraphics CreateNativeGraphics(Graphics gr, IntPtr hdc, Rectangle bounds, Rectangle windowsBounds) {
			Rectangle rect = CalculateViewportBounds(bounds, windowsBounds);
			OpenGLGraphics graphics = new OpenGLGraphics(gr, hdc, rect, Chart == null ? null : Chart.TextureCache);
			if (graphics.Initialized)
				return graphics;
			graphics.Dispose();
			return null;
		}
		Rectangle CalculateViewportBounds(Rectangle bounds, Rectangle windowsBounds) {
			if(windowsBounds != Rectangle.Empty)
				return new Rectangle(bounds.X, windowsBounds.Height - (bounds.Y + bounds.Height), bounds.Width, bounds.Height);
			return bounds;
		}
		protected internal override void OnEndLoading() {
 			base.OnEndLoading();
			if (!Chart.Container.DesignMode && runtimeRotation && rotationType == RotationType.UseAngles) {
				CalculateRotationMatrix();
				rotationType = RotationType.UseMouseAdvanced;
			}
		}
		protected internal override bool CanDrag(Point point, MouseButtons button) {
			switch (button) {
				case MouseButtons.Left:
					return CanRotateAtPoint(point);
				case MouseButtons.Middle:
					return CanScrollAtPoint(point);
				default:
					return CanRotateAtPoint(point) || CanScrollAtPoint(point);
			}
		}
		protected internal override bool PerformDragging(int x, int y, int dx, int dy, ChartScrollEventType scrollEventType, Object focusedElement) {
			switch (scrollEventType) {
				case ChartScrollEventType.LeftButtonMouseDrag:
					return InternalCanRotateWithMouse ? PerformRotation(dx, dy) : false;
				case ChartScrollEventType.Gesture:
					return InternalCanRotateWithTouch ? PerformRotation(dx, dy) : false;
				default:
					return PerformScrolling(dx, dy);
			}
		}
		protected internal override bool CanZoom(Point point) {
			return InternalCanZoom && lastDiagramBounds.Contains(point);
		}
		protected internal override void Zoom(int delta, ZoomingKind zoomingKind, Object focusedElement) {
			ZoomPercent = ClampZoomPercent(zoomPercent + delta * 3);
		}
		protected internal override void ZoomIn(Point center) {
			if (CanZoomIn)
				ZoomPercent = ClampZoomPercent(zoomPercent * ZoomPercentStep);
		}
		protected internal override void ZoomOut(Point center) {
			if (CanZoomOut)
				ZoomPercent = ClampZoomPercent(zoomPercent / ZoomPercentStep);
		}
		protected internal override string GetDesignerHint(Point p) {
			string result = String.Empty;
			if (CanRotateAtPoint(p))
				result = ChartLocalizer.GetString(ChartStringId.Msg3DRotationToolTip);
			if (CanScrollAtPoint(p)) {
				if (result.Length != 0)
					result += "\n";
				result += ChartLocalizer.GetString(ChartStringId.Msg3DScrollingToolTip);
			}
			return result;
		}
		protected internal override void UpdateDiagramProperties(Diagram diagram) {
			Diagram3D diagram3D = diagram as Diagram3D;
			if (diagram3D == null)
				return;
			UpdateRuntimeProperties(diagram3D);
		}
		protected internal override void BeginGestureZoom(Point center, double zoomDelta) {
			PerformGestureZoom(zoomDelta);
		}
		protected internal override void PerformGestureZoom(double zoomDelta) {
			if (RuntimeZooming)
				ZoomPercent = ClampZoomPercent(ZoomPercent * zoomDelta);
		}
		protected internal override bool PerformGestureRotation(double degreeDelta) {
			if (InternalCanRotateWithTouch) {
				GLHelper.PerformRotation(RotationMatrix, degreeDelta);
				return true;
			}
			return false;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Diagram3D diagram = obj as Diagram3D;
			if (diagram != null) {
				rotationType = diagram.rotationType;
				rotationOrder = diagram.rotationOrder;
				rotationAngleX = diagram.rotationAngleX;
				rotationAngleY = diagram.rotationAngleY;
				rotationAngleZ = diagram.rotationAngleZ;
				if (diagram.rotationMatrix != null) {
					rotationMatrix = new double[diagram.rotationMatrix.Length];
					Array.Copy(diagram.rotationMatrix, rotationMatrix, 16);
				}
				perspectiveEnabled = diagram.perspectiveEnabled;
				perspectiveAngle = diagram.perspectiveAngle;
				zoomPercent = diagram.zoomPercent;
				horizontalScrollPercent = diagram.horizontalScrollPercent;
				verticalScrollPercent = diagram.verticalScrollPercent;
				scrollingOptions.Assign(diagram.scrollingOptions);
				zoomingOptions.Assign(diagram.zoomingOptions);
				rotationOptions.Assign(diagram.rotationOptions);
				UpdateRuntimeProperties(diagram);
			}
		}
	}
}
