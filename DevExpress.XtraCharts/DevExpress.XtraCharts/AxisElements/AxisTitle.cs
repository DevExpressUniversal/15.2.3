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
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class AxisTitle : Title, ITextPropertiesProvider, ISupportVisibilityControlElement {
		const bool DefaultVisibleConst = false;
		const StringAlignment DefaultStringAlignment = StringAlignment.Center;
		const DefaultBoolean DefaultVisibility = DefaultBoolean.False;
		bool automaticVisibility = true;
		StringAlignment alignment = DefaultStringAlignment;
		DefaultBoolean visibility = DefaultVisibility;
		Axis2D Axis { get { return (Axis2D)base.Owner; } }
		internal bool AutomaticVisibility { get { return ChartContainer != null && !ChartContainer.Chart.AutoLayout ? DefaultVisible : automaticVisibility; } }
		internal GRealRect2D Bounds { get; set; }
		protected abstract ChartElementVisibilityPriority Priority { get; }
		protected override bool DefaultVisible { get { return DefaultVisibleConst; } }
		protected override Font DefaultFont { get { return DefaultFonts.Tahoma12; } }
		protected override bool DefaultAntialiasing { get { return true; } }
		protected internal override bool ActualVisibility { get { return DefaultBooleanUtils.ToBoolean(Visibility, AutomaticVisibility); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisTitleAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisTitle.Alignment"),
		TypeConverter(typeof(StringAlignmentTypeConvertor)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public StringAlignment Alignment {
			get { return alignment; }
			set {
				if (value != alignment) {
					SendNotification(new ElementWillChangeNotification(this));
					alignment = value;
					RaiseControlChanged();
				}
			}
		}
		[
		Obsolete("This property is obsolete now. Use the Visibility property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new bool Visible {
			get { return ActualVisibility; }
			set { Visibility = DefaultBooleanUtils.ToDefaultBoolean(value); }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisTitleVisibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisTitle.Visibility"),
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
		protected AxisTitle(Axis2D axis) : base(axis) {
		}
		#region IHitTest
		object IHitTest.Object { get { return this.Axis; } }
		HitTestState IHitTest.State { get { return ((IHitTest)this.Axis).State; } }
		#endregion
		#region ITextPropertiesProvider
		RectangleFillStyle ITextPropertiesProvider.FillStyle { get { return RectangleFillStyle.Empty; } }
		RectangularBorder ITextPropertiesProvider.Border { get { return null; } }
		Shadow ITextPropertiesProvider.Shadow { get { return null; } }
		StringAlignment ITextPropertiesProvider.Alignment { get { return StringFormat.GenericDefault.Alignment; } }
		bool ITextPropertiesProvider.ChangeSelectedBorder { get { return true; } }
		bool ITextPropertiesProvider.CorrectBoundsByBorder { get { return false; } }
		Color ITextPropertiesProvider.BackColor { get { return Color.Empty; } }
		Color ITextPropertiesProvider.GetTextColor(Color color) { return ActualTextColor; }
		Color ITextPropertiesProvider.GetBorderColor(Color color) { return Color.Empty; }
		#endregion
		#region ISupportVisibilityControlElement
		int ISupportVisibilityControlElement.Priority { get { return (int)Priority; } }
		bool ISupportVisibilityControlElement.Visible {
			get { return automaticVisibility; }
			set { automaticVisibility = value; }
		}
		GRealRect2D ISupportVisibilityControlElement.Bounds { get { return Bounds; } }
		VisibilityElementOrientation ISupportVisibilityControlElement.Orientation {
			get { return Axis.IsVertical ? VisibilityElementOrientation.Vertical : VisibilityElementOrientation.Horizontal; }
		}		
		#endregion
		#region ShouldSerialize & Reset
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case ("Alignment"):
					return ShouldSerializeAlignment();
				case "Visibility":
					return ShouldSerializeVisibility();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		bool ShouldSerializeAlignment() {
			return alignment != DefaultStringAlignment;
		}
		void ResetAlignment() {
			Alignment = DefaultStringAlignment;
		}
		bool ShouldSerializeVisibility() {
			return visibility != DefaultVisibility;
		}
		void ResetVisibility() {
			Visibility = DefaultVisibility;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeAlignment() || ShouldSerializeVisibility();
		}
		#endregion
		protected override Color GetTextColor(IChartAppearance actualAppearance) {
			return actualAppearance.XYDiagramAppearance.AxisTitleColor;
		}
		public override string ToString() {
			return "(AxisTitle)";
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			AxisTitle title = obj as AxisTitle;
			if (title != null) {
				alignment = title.alignment;
				visibility = title.visibility;
			}
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public sealed class AxisTitleX : AxisTitle {
		protected override ChartElementVisibilityPriority Priority { get { return ChartElementVisibilityPriority.AxisXTitle; } }
		protected override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.AxisXDefaultTitle); } }
		internal AxisTitleX() : base(null) {
		}
		internal AxisTitleX(AxisXBase axis) : base(axis) {
		}
		internal AxisTitleX(SwiftPlotDiagramAxisXBase axis) : base(axis) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new AxisTitleX();
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public sealed class AxisTitleY : AxisTitle {
		protected override ChartElementVisibilityPriority Priority { get { return ChartElementVisibilityPriority.AxisYTitle; } }
		protected override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.AxisYDefaultTitle); } }
		internal AxisTitleY() : base(null) {
		}
		internal AxisTitleY(AxisYBase axis) : base(axis) {
		}
		internal AxisTitleY(SwiftPlotDiagramAxisYBase axis) : base(axis) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new AxisTitleX();
		}
	}
}
