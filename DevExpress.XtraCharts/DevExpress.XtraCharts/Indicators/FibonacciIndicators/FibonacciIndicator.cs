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
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum FibonacciIndicatorKind {
		FibonacciArcs,
		FibonacciFans,
		FibonacciRetracement
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(FibonacciIndicator.TypeConverter))
	]
	public class FibonacciIndicator : FinancialIndicator, IFibonacciIndicatorBehavior {
		#region Nested class: TypeConverter
		public class TypeConverter : ExpandableObjectConverter {
			static IList<string> GetFilteredProperties(object value) {
				List<string> filteredProperties = new List<string>();
				FibonacciIndicator fibonacciIndicator = TypeConverterHelper.GetElement<FibonacciIndicator>(value);
				if (fibonacciIndicator == null)
					return filteredProperties;
				if (!fibonacciIndicator.ShowLevel0PropertyEnabled)
					filteredProperties.Add("ShowLevel0");
				if (!fibonacciIndicator.ShowLevel100PropertyEnabled)
					filteredProperties.Add("ShowLevel100");
				if (!fibonacciIndicator.ShowAdditionalLevelsPropertyEnabled)
					filteredProperties.Add("ShowAdditionalLevels");
				return filteredProperties;
			}   
			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
				return FilterPropertiesUtils.FilterProperties(base.GetProperties(context, value, attributes), GetFilteredProperties(value));
			}
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) ? 
					new InstanceDescriptor(typeof(FibonacciIndicator).GetConstructor(new Type[0]), null, false) : 
					base.ConvertTo(context, culture, value, destinationType);
			}
		}
		#endregion
		const FibonacciIndicatorKind DefaultKind = FibonacciIndicatorKind.FibonacciArcs;
		static readonly Color DefaultBaseLevelColor = Color.Empty;
		readonly FibonacciIndicatorLabel label;
		readonly LineStyle baseLevelLineStyle;
		Color baseLevelColor = DefaultBaseLevelColor;
		FibonacciIndicatorKind kind;
		bool showLevel0;
		bool showLevel100;
		bool showLevel23_6;
		bool showLevel76_4;
		bool showAdditionalLevels;
		FibonacciIndicatorBehavior Behavior { get { return (FibonacciIndicatorBehavior)IndicatorBehavior; } }
		internal bool ShowLevel0PropertyEnabled { get { return Behavior.ShowLevel0PropertyEnabled; } }
		internal bool ShowLevel100PropertyEnabled { get { return Behavior.ShowLevel100PropertyEnabled; } }
		internal bool ShowAdditionalLevelsPropertyEnabled { get { return Behavior.ShowAdditionalLevelsPropertyEnabled; } }
		public override string IndicatorName { get { return ChartLocalizer.GetString(ChartStringId.IndFibonacciIndicator); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FibonacciIndicatorLabel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FibonacciIndicator.Label"),
		Category("Elements"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public FibonacciIndicatorLabel Label { get { return label; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FibonacciIndicatorBaseLevelLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FibonacciIndicator.BaseLevelLineStyle"),
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		NestedTagProperty
		]
		public LineStyle BaseLevelLineStyle { get { return baseLevelLineStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FibonacciIndicatorBaseLevelColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FibonacciIndicator.BaseLevelColor"),
		Category("Appearance"),
		XtraSerializableProperty
		]
		public Color BaseLevelColor {
			get { return baseLevelColor; }
			set {
				if (value != baseLevelColor) {
					SendNotification(new ElementWillChangeNotification(this));
					baseLevelColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FibonacciIndicatorShowLevel0"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FibonacciIndicator.ShowLevel0"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowLevel0 {
			get { return showLevel0; }
			set {
				if (value != showLevel0 && (ShowLevel0PropertyEnabled || Loading)) {
					SendNotification(new ElementWillChangeNotification(this));
					showLevel0 = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FibonacciIndicatorShowLevel100"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FibonacciIndicator.ShowLevel100"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowLevel100 {
			get { return showLevel100; }
			set {
				if (value != showLevel100 && (ShowLevel100PropertyEnabled || Loading)) {
					SendNotification(new ElementWillChangeNotification(this));
					showLevel100 = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FibonacciIndicatorShowLevel23_6"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FibonacciIndicator.ShowLevel23_6"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowLevel23_6 {
			get { return showLevel23_6; }
			set {
				if (value != showLevel23_6) {
					SendNotification(new ElementWillChangeNotification(this));
					showLevel23_6 = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FibonacciIndicatorShowLevel76_4"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FibonacciIndicator.ShowLevel76_4"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowLevel76_4 {
			get { return showLevel76_4; }
			set {
				if (value != showLevel76_4) {
					SendNotification(new ElementWillChangeNotification(this));
					showLevel76_4 = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FibonacciIndicatorShowAdditionalLevels"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FibonacciIndicator.ShowAdditionalLevels"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowAdditionalLevels {
			get { return showAdditionalLevels; }
			set {
				if (value != showAdditionalLevels && (ShowAdditionalLevelsPropertyEnabled || Loading)) {
					SendNotification(new ElementWillChangeNotification(this));
					showAdditionalLevels = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FibonacciIndicatorKind"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FibonacciIndicator.Kind"),
		Category("Behavior"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public FibonacciIndicatorKind Kind {
			get { return kind; }
			set {
				if (value != kind) {
					SendNotification(new ElementWillChangeNotification(this));
					SetKind(value);
					RaiseControlChanged();
				}
			}
		}
		public FibonacciIndicator() : this(DefaultKind, String.Empty) { }
		public FibonacciIndicator(FibonacciIndicatorKind kind) : this(kind, String.Empty) { }
		public FibonacciIndicator(FibonacciIndicatorKind kind, string name) : base(name) {
			label = new FibonacciIndicatorLabel(this);
			baseLevelLineStyle = new LineStyle(this, 1, true, DashStyle.Solid);
			SetKind(kind);
			showLevel0 = Behavior.DefaultShowLevel0;
			showLevel100 = Behavior.DefaultShowLevel100;
			showLevel23_6 = Behavior.DefaultShowLevel23_6;
			showLevel76_4 = Behavior.DefaultShowLevel76_4;
			showAdditionalLevels = Behavior.DefaultShowAdditionalLevels;
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeLabel() {
			return label.ShouldSerialize();
		}
		bool ShouldSerializeBaseLevelLineStyle() {
			return baseLevelLineStyle.ShouldSerialize();
		}
		bool ShouldSerializeBaseLevelColor() {
			return baseLevelColor != DefaultBaseLevelColor;
		}
		void ResetBaseLevelColor() {
			BaseLevelColor = DefaultBaseLevelColor;
		}
		bool ShouldSerializeKind() {
			return kind != DefaultKind;
		}
		void ResetKind() {
			Kind = DefaultKind;
		}
		bool ShouldSerializeShowLevel0() {
			return true; 
		}
		void ResetShowLevel0() {
			ShowLevel0 = Behavior.DefaultShowLevel0;
		}
		bool ShouldSerializeShowLevel100() {
			return true; 
		}
		void ResetShowLevel100() {
			showLevel100 = Behavior.DefaultShowLevel100;
		}
		bool ShouldSerializeShowLevel23_6(){
			return true; 
		}
		void ResetShowLevel23_6() {
			ShowLevel23_6 = Behavior.DefaultShowLevel23_6;
		}
		bool ShouldSerializeShowLevel76_4() {
			return true; 
		}
		void ResetShowLevel76_4() {
			ShowLevel76_4 = Behavior.DefaultShowLevel76_4;
		}
		bool ShouldSerializeShowAdditionalLevels() {
			return true; 
		}
		void ResetShowAdditionalLevels() {
			ShowAdditionalLevels = Behavior.DefaultShowAdditionalLevels;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Label":
					return ShouldSerializeLabel();
				case "BaseLevelLineStyle":
					return ShouldSerializeBaseLevelLineStyle();
				case "BaseLevelColor":
					return ShouldSerializeBaseLevelColor();
				case "ShowLevel0":
				case "ShowLevel100":
				case "ShowLevel23_6":
				case "ShowLevel76_4":
				case "ShowAdditionalLevels":
					return true;
				case "Kind":
					return ShouldSerializeKind();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region IFibonacciIndicatorBehavior implementation
		bool IFibonacciIndicatorBehavior.ShowLevel0PropertyEnabled { get { return ShowLevel0PropertyEnabled; } }
		bool IFibonacciIndicatorBehavior.ShowLevel100PropertyEnabled { get { return ShowLevel100PropertyEnabled; } }
		bool IFibonacciIndicatorBehavior.ShowAdditionalLevelsPropertyEnabled { get { return ShowAdditionalLevelsPropertyEnabled; } }
		#endregion
		void SetKind(FibonacciIndicatorKind kind) {
			this.kind = kind;
			UpdateBehavior();
		}
		protected override ChartElement CreateObjectForClone() {
			return new FibonacciIndicator();
		}
		protected override IndicatorBehavior CreateBehavior() {
			return FibonacciIndicatorBehavior.CreateInstance(kind, this);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			FibonacciIndicator fibonacciIndicator = obj as FibonacciIndicator;
			if (fibonacciIndicator != null) {
				this.label.Assign(fibonacciIndicator.label);
				this.baseLevelLineStyle.Assign(fibonacciIndicator.baseLevelLineStyle);
				this.baseLevelColor = fibonacciIndicator.baseLevelColor;
				this.showLevel0 = fibonacciIndicator.showLevel0;
				this.showLevel100 = fibonacciIndicator.showLevel100;
				this.showLevel23_6 = fibonacciIndicator.showLevel23_6;
				this.showLevel76_4 = fibonacciIndicator.showLevel76_4;
				this.showAdditionalLevels = fibonacciIndicator.showAdditionalLevels;
				SetKind(fibonacciIndicator.kind);
			}
		}		
	}
}
namespace DevExpress.XtraCharts.Native {
	public abstract class FibonacciIndicatorBehavior : FinancialIndicatorBehavior {
		const double level0 = 0;
		const double level100 = 1;
		const double level23_6 = 0.236;
		const double level76_4 = 0.764;
		static readonly double[] defaultLevels = { 0.382, 0.5, 0.618 };
		static readonly double[] additionalLevels = { 1.618, 2.618, 4.236 };
		protected static string ConstructLevelText(double level) {
			return (level * 100.0).ToString() + "%";
		}
		public static FibonacciIndicatorBehavior CreateInstance(FibonacciIndicatorKind kind, FibonacciIndicator fibonacciIndicator) {
			switch (kind) {
				case FibonacciIndicatorKind.FibonacciArcs:
					return new FibonacciArcsBehavior(fibonacciIndicator);
				case FibonacciIndicatorKind.FibonacciFans:
					return new FibonacciFansBehavior(fibonacciIndicator);
				case FibonacciIndicatorKind.FibonacciRetracement:
					return new FibonacciRetracementBehavior(fibonacciIndicator);
				default:
					throw new DefaultSwitchException();
			}
		}
		Color baseLevelColor;
		protected FibonacciIndicator FibonacciIndicator { get { return (FibonacciIndicator)Indicator; } }
		public Color BaseLevelColor { get { return baseLevelColor; } }
		public abstract bool DefaultShowLevel0 { get; }
		public abstract bool DefaultShowLevel100 { get; }
		public abstract bool DefaultShowLevel23_6 { get; }
		public abstract bool DefaultShowLevel76_4 { get; }
		public abstract bool DefaultShowAdditionalLevels { get; }
		public abstract bool ShowLevel0PropertyEnabled { get; }
		public abstract bool ShowLevel100PropertyEnabled { get; }
		public abstract bool ShowAdditionalLevelsPropertyEnabled { get; }
		public FibonacciIndicatorBehavior(FibonacciIndicator fibonacciIndicator) : base(fibonacciIndicator) {
		}
		protected internal override void UpdateColor(Color generatedColor) {
			base.UpdateColor(generatedColor);
			baseLevelColor = FibonacciIndicator.BaseLevelColor;
			if (baseLevelColor.IsEmpty)
				baseLevelColor = Color;
		}
		public List<double> GetBaseLevels() {
			List<double> baseLevels = new List<double>();
			if ((ShowLevel0PropertyEnabled && FibonacciIndicator.ShowLevel0) || (!ShowLevel0PropertyEnabled && DefaultShowLevel0))
				baseLevels.Add(level0);
			if ((ShowLevel100PropertyEnabled && FibonacciIndicator.ShowLevel100) || (!ShowLevel100PropertyEnabled && DefaultShowLevel100))
				baseLevels.Add(level100);
			return baseLevels;
		}
		public List<double> GetLevels() {
			List<double> levels = new List<double>(defaultLevels);
			if (FibonacciIndicator.ShowLevel23_6)
				levels.Add(level23_6);
			if (FibonacciIndicator.ShowLevel76_4)
				levels.Add(level76_4);
			if ((ShowAdditionalLevelsPropertyEnabled && FibonacciIndicator.ShowAdditionalLevels) || 
				(!ShowAdditionalLevelsPropertyEnabled && DefaultShowAdditionalLevels))
					levels.AddRange(additionalLevels);
			return levels;
		}
	}  
	public abstract class FibonacciIndicatorLayout : IndicatorLayout {
		readonly FibonacciIndicatorLabelsLayout labelsLayout;
		protected FibonacciIndicator FibonacciIndicator { get { return (FibonacciIndicator)Indicator; } }
		protected int BaseLevelThickness { 
			get { 
				FibonacciIndicator fibonacciIndicator = FibonacciIndicator;
				return GraphicUtils.CorrectThicknessByHitTestState(fibonacciIndicator.BaseLevelLineStyle.Thickness, 
					((IHitTest)fibonacciIndicator).State, 1); 
			} 
		}
		protected Color BaseLevelColor { 
			get { 
				FibonacciIndicator fibonacciIndicator = FibonacciIndicator;
				return GraphicUtils.CorrectColorByHitTestState(((FibonacciIndicatorBehavior)FibonacciIndicator.IndicatorBehavior).BaseLevelColor,
					((IHitTest)fibonacciIndicator).State); 
			} 
		}
		protected abstract void RenderBase(IRenderer renderer);
		public FibonacciIndicatorLayout(FibonacciIndicator fibonacciIndicator, FibonacciIndicatorLabelsLayout labelsLayout) : base(fibonacciIndicator) {
			this.labelsLayout = labelsLayout;
		}
		public override void Render(IRenderer renderer) {
			RenderBase(renderer);
			if (labelsLayout != null)
				labelsLayout.Render(renderer);
		}
	}		 
}
namespace DevExpress.XtraCharts.Native {
	public interface IFibonacciIndicatorBehavior {
		bool ShowLevel0PropertyEnabled { get; }
		bool ShowLevel100PropertyEnabled { get; }
		bool ShowAdditionalLevelsPropertyEnabled { get; }
	}
}
