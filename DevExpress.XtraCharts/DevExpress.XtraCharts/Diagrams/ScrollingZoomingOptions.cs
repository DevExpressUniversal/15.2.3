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
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ScrollingOptions : ChartElement {
		const bool DefaultUseKeyboard = true;
		const bool DefaultUseMouse = true;
		bool useKeyboard = DefaultUseKeyboard;
		bool useMouse = DefaultUseMouse;
		protected internal virtual bool HasWays { get { return UseKeyboard || UseMouse; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollingOptionsUseKeyboard"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollingOptions.UseKeyboard"),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool UseKeyboard {
			get { return useKeyboard; }
			set {
				if (useKeyboard != value) {
					SendNotification(new ElementWillChangeNotification(this));
					useKeyboard = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollingOptionsUseMouse"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollingOptions.UseMouse"),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool UseMouse {
			get { return useMouse; }
			set {
				if (useMouse != value) {
					SendNotification(new ElementWillChangeNotification(this));
					useMouse = value;
					RaiseControlChanged();
				}
			}
		}
		internal ScrollingOptions(Diagram diagram) : base(diagram) {
		}
		#region ShouldSerialize & Reset
		protected bool ShouldSerializeProperties() {
			IChartContainer chartContainer = ChartContainer;
			return chartContainer == null || chartContainer.ControlType != ChartContainerType.WebControl;
		}
		bool ShouldSerializeUseKeyboard() {
			return ShouldSerializeProperties() && (useKeyboard != DefaultUseKeyboard);
		}
		void ResetUseKeyboard() {
			UseKeyboard = DefaultUseKeyboard;
		}
		bool ShouldSerializeUseMouse() {
			return ShouldSerializeProperties() && (useMouse != DefaultUseMouse);
		}
		void ResetUseMouse() {
			UseMouse = DefaultUseMouse;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeUseKeyboard() || ShouldSerializeUseMouse();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "UseKeyboard":
					return ShouldSerializeUseKeyboard();
				case "UseMouse":
					return ShouldSerializeUseMouse();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new ScrollingOptions(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ScrollingOptions options = obj as ScrollingOptions;
			if (options != null) {
				useKeyboard = options.useKeyboard;
				useMouse = options.useMouse;
			}
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ScrollingOptions2D : ScrollingOptions {
		const bool DefaultUseScrollBars = true;
		const bool DefaultUseTouchDevice = true;
		bool useScrollBars = DefaultUseScrollBars;
		bool useTouchDevice = DefaultUseTouchDevice;
		protected internal override bool HasWays { get { return base.HasWays || UseScrollBars || UseTouchDevice; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollingOptions2DUseScrollBars"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollingOptions2D.UseScrollBars"),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool UseScrollBars {
			get { return useScrollBars; }
			set {
				if (useScrollBars != value) {
					SendNotification(new ElementWillChangeNotification(this));
					useScrollBars = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollingOptions2DUseTouchDevice"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollingOptions2D.UseTouchDevice"),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool UseTouchDevice {
			get { return useTouchDevice; }
			set {
				if (useTouchDevice != value) {
					SendNotification(new ElementWillChangeNotification(this));
					useTouchDevice = value;
					RaiseControlChanged();
				}
			}
		}
		internal ScrollingOptions2D(XYDiagram2D diagram) : base(diagram) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeUseScrollBars() {
			return ShouldSerializeProperties() && (useScrollBars != DefaultUseScrollBars);
		}
		void ResetUseScrollBars() {
			UseScrollBars = DefaultUseScrollBars;
		}
		bool ShouldSerializeUseTouchDevice() {
			return ShouldSerializeProperties() && (useTouchDevice != DefaultUseTouchDevice);
		}
		void ResetUseTouchDevice() {
			UseTouchDevice = DefaultUseTouchDevice;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeUseScrollBars() || ShouldSerializeUseTouchDevice();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "UseScrollBars":
					return ShouldSerializeUseScrollBars();
				case "UseTouchDevice":
					return ShouldSerializeUseTouchDevice();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new ScrollingOptions2D(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ScrollingOptions2D options = obj as ScrollingOptions2D;
			if (options != null) {
				useScrollBars = options.useScrollBars;
				useTouchDevice = options.useTouchDevice;
			}
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ZoomingOptions : ChartElement {
		const bool DefaultUseKeyboard = true;
		const bool DefaultUseMouseWheel = true;
		const bool DefaultUseKeyboardWithMouse = true;
		const bool DefaultUseTouchDevice = true;
		bool useKeyboard = DefaultUseKeyboard;
		bool useMouseWheel = DefaultUseMouseWheel;
		bool useKeyboardWithMouse = DefaultUseKeyboardWithMouse;
		bool useTouchDevice = DefaultUseTouchDevice;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ZoomingOptionsUseKeyboard"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ZoomingOptions.UseKeyboard"),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool UseKeyboard {
			get { return useKeyboard; }
			set {
				if (useKeyboard != value) {
					SendNotification(new ElementWillChangeNotification(this));
					useKeyboard = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ZoomingOptionsUseKeyboardWithMouse"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ZoomingOptions.UseKeyboardWithMouse"),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool UseKeyboardWithMouse {
			get { return useKeyboardWithMouse; }
			set {
				if (useKeyboardWithMouse != value) {
					SendNotification(new ElementWillChangeNotification(this));
					useKeyboardWithMouse = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ZoomingOptionsUseMouseWheel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ZoomingOptions.UseMouseWheel"),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool UseMouseWheel {
			get { return useMouseWheel; }
			set {
				if (useMouseWheel != value) {
					SendNotification(new ElementWillChangeNotification(this));
					useMouseWheel = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ZoomingOptionsUseTouchDevice"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ZoomingOptions.UseTouchDevice"),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool UseTouchDevice {
			get { return useTouchDevice; }
			set {
				if (useTouchDevice != value) {
					SendNotification(new ElementWillChangeNotification(this));
					useTouchDevice = value;
					RaiseControlChanged();
				}
			}
		}
		internal ZoomingOptions(Diagram diagram) : base(diagram) {
		}
		#region ShouldSerialize & Reset
		protected bool ShouldSerializeProperties() {
			IChartContainer chartContainer = ChartContainer;
			return chartContainer == null || chartContainer.ControlType != ChartContainerType.WebControl;
		}
		bool ShouldSerializeUseKeyboard() {
			return ShouldSerializeProperties() && (useKeyboard != DefaultUseKeyboard);
		}
		void ResetUseKeyboard() {
			UseKeyboard = DefaultUseKeyboard;
		}
		bool ShouldSerializeUseKeyboardWithMouse() {
			return ShouldSerializeProperties() && (useKeyboardWithMouse != DefaultUseKeyboardWithMouse);
		}
		void ResetUseKeyboardWithMouse() {
			UseKeyboardWithMouse = DefaultUseKeyboardWithMouse;
		}
		bool ShouldSerializeUseMouseWheel() {
			return ShouldSerializeProperties() && (useMouseWheel != DefaultUseMouseWheel);
		}
		void ResetUseMouseWheel() {
		   UseMouseWheel = DefaultUseMouseWheel;
		}
		bool ShouldSerializeUseTouchDevice() {
			return ShouldSerializeProperties() && (useTouchDevice != DefaultUseTouchDevice);
		}
		void ResetUseTouchDevice() {
			UseTouchDevice = DefaultUseTouchDevice;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeUseKeyboard() || ShouldSerializeUseKeyboardWithMouse() || ShouldSerializeUseMouseWheel() || ShouldSerializeUseTouchDevice();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "UseKeyboard":
					return ShouldSerializeUseKeyboard();
				case "UseKeyboardWithMouse":
					return ShouldSerializeUseKeyboardWithMouse();
				case "UseMouseWheel":
					return ShouldSerializeUseMouseWheel();
				case "UseTouchDevice":
					return ShouldSerializeUseTouchDevice();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new ZoomingOptions(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ZoomingOptions options = obj as ZoomingOptions;
			if (options != null) {
				useKeyboard = options.useKeyboard;
				useKeyboardWithMouse = options.useKeyboardWithMouse;
				useMouseWheel = options.useMouseWheel;
				useTouchDevice = options.useTouchDevice;
			}
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ZoomingOptions2D : ZoomingOptions {
		const double MinPossibleZoomPercent = 100.0;
		const double DefaultMaxZoomPercent = 10000.0;
		double axisXMaxZoomPercent = DefaultMaxZoomPercent;
		double axisYMaxZoomPercent = DefaultMaxZoomPercent;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ZoomingOptions2DAxisXMaxZoomPercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ZoomingOptions2D.AxisXMaxZoomPercent"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public double AxisXMaxZoomPercent {
			get { return axisXMaxZoomPercent; }
			set {
				if (axisXMaxZoomPercent != value) {
					if (!Loading && value < MinPossibleZoomPercent)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectMaxZoomPercent));
					SendNotification(new ElementWillChangeNotification(this));
					axisXMaxZoomPercent = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ZoomingOptions2DAxisYMaxZoomPercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ZoomingOptions2D.AxisYMaxZoomPercent"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public double AxisYMaxZoomPercent {
			get { return axisYMaxZoomPercent; }
			set {
				if (axisYMaxZoomPercent != value) {
					if (!Loading && value < MinPossibleZoomPercent)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectMaxZoomPercent));
					SendNotification(new ElementWillChangeNotification(this));
					axisYMaxZoomPercent = value;
					RaiseControlChanged();
				}
			}
		}
		internal ZoomingOptions2D(XYDiagram2D diagram)
			: base(diagram) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeAxisXMaxZoomPercent() {
			return ShouldSerializeProperties() && axisXMaxZoomPercent != DefaultMaxZoomPercent;
		}
		void ResetAxisXMaxZoomPercent() {
			AxisXMaxZoomPercent = DefaultMaxZoomPercent;
		}
		bool ShouldSerializeAxisYMaxZoomPercent() {
			return ShouldSerializeProperties() && axisYMaxZoomPercent != DefaultMaxZoomPercent;
		}
		void ResetAxisYMaxZoomPercent() {
			AxisYMaxZoomPercent = DefaultMaxZoomPercent;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeAxisXMaxZoomPercent() || ShouldSerializeAxisYMaxZoomPercent();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "AxisXMaxZoomPercent":
					return ShouldSerializeAxisXMaxZoomPercent();
				case "AxisYMaxZoomPercent":
					return ShouldSerializeAxisYMaxZoomPercent();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new ZoomingOptions2D(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ZoomingOptions2D options = obj as ZoomingOptions2D;
			if (options != null) {
				axisXMaxZoomPercent = options.axisXMaxZoomPercent;
				axisYMaxZoomPercent = options.axisYMaxZoomPercent;
			}
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RotationOptions : ChartElement {
		const bool DefaultUseMouse = true;
		const bool DefaultUseTouchDevice = true;
		bool useMouse = DefaultUseMouse;
		bool useTouchDevice = DefaultUseTouchDevice;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RotationOptionsUseMouse"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RotationOptions.UseMouse"),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool UseMouse {
			get { return useMouse; }
			set {
				if (useMouse != value) {
					SendNotification(new ElementWillChangeNotification(this));
					useMouse = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RotationOptionsUseTouchDevice"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RotationOptions.UseTouchDevice"),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool UseTouchDevice {
			get { return useTouchDevice; }
			set {
				if (useTouchDevice != value) {
					SendNotification(new ElementWillChangeNotification(this));
					useTouchDevice = value;
					RaiseControlChanged();
				}
			}
		}
		internal RotationOptions(Diagram3D diagram) : base(diagram) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeProperties() {
			IChartContainer chartContainer = ChartContainer;
			return chartContainer == null || chartContainer.ControlType != ChartContainerType.WebControl;
		}
		bool ShouldSerializeUseMouse() {
			return ShouldSerializeProperties() && (useMouse != DefaultUseMouse);
		}
		void ResetUseMouse() {
			UseMouse = DefaultUseMouse;
		}
		bool ShouldSerializeUseTouchDevice() {
			return ShouldSerializeProperties() && (useTouchDevice != DefaultUseTouchDevice);
		}
		void ResetUseTouchDevice() {
			UseTouchDevice = DefaultUseTouchDevice;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeUseMouse() || ShouldSerializeUseTouchDevice();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "UseMouse":
					return ShouldSerializeUseMouse();
				case "UseTouchDevice":
					return ShouldSerializeUseTouchDevice();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new RotationOptions(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RotationOptions options = obj as RotationOptions;
			if (options != null) {
				useMouse = options.useMouse;
				useTouchDevice = options.useTouchDevice;
			}
		}
	}
}
