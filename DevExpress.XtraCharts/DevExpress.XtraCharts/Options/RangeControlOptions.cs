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

using System.ComponentModel;
using DevExpress.Utils.Serializing;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Design;
using System;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum RangeControlViewType {
		Line,
		Area
	}
	public class RangeControlOptionsTypeConverter : ExpandableObjectConverter {
		XYDiagram2DSeriesViewBase GetSeriesView(RangeControlOptions options) {
			if (options != null)
				return options.Owner as XYDiagram2DSeriesViewBase;
			return null;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			XYDiagram2DSeriesViewBase seriesView = GetSeriesView(TypeConverterHelper.GetElement<RangeControlOptions>(value));
			if (!(seriesView is FinancialSeriesViewBase))
				return FilterPropertiesUtils.FilterProperties(collection, new string[] { "ValueLevel" });
			return collection;
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(RangeControlOptionsTypeConverter))
	]
	public class RangeControlOptions : ChartElement {
		const bool DefaultVisible = true;
		const RangeControlViewType DefaultViewType = RangeControlViewType.Line;
		const ValueLevel DefaultValueLevel = ValueLevel.Open;
		bool visible = DefaultVisible;
		RangeControlViewType viewType = DefaultViewType;
		ValueLevel valueLevel = DefaultValueLevel;
		byte? transparency = null; 
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeControlOptionsVisible"),
#endif
		XtraSerializableProperty
		]
		public bool Visible {
			get { return visible; }
			set {
				if (this.visible != value) {
					SendNotification(new ElementWillChangeNotification(this));
					this.visible = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeControlOptionsViewType"),
#endif
		XtraSerializableProperty
		]
		public RangeControlViewType ViewType {
			get { return viewType; }
			set {
				if (this.viewType != value) {
					SendNotification(new ElementWillChangeNotification(this));
					this.viewType = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeControlOptionsValueLevel"),
#endif
		XtraSerializableProperty
		]
		public ValueLevel ValueLevel {
			get { return valueLevel; }
			set {
				if (this.valueLevel != value) {
					SendNotification(new ElementWillChangeNotification(this));
					this.valueLevel = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeControlOptionsSeriesTransparency"),
#endif
		XtraSerializableProperty,
		]
		public byte? SeriesTransparency {
			get { return transparency; }
			set {
				if (this.transparency != value) {
					SendNotification(new ElementWillChangeNotification(this));
					this.transparency = value;
					RaiseControlChanged();
				}
			}
		}
		internal RangeControlOptions(XYDiagram2DSeriesViewBase seriesView)
			: base(seriesView) {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Visible")
				return ShouldSerializeVisible();
			if (propertyName == "ViewType")
				return ShouldSerializeViewType();
			if (propertyName == "ValueLevel")
				return ShouldSerializeValueLevel();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeVisible() {
			return this.visible != DefaultVisible;
		}
		void ResetVisible() {
			Visible = DefaultVisible;
		}
		bool ShouldSerializeViewType() {
			return this.viewType != DefaultViewType;
		}
		void ResetViewType() {
			ViewType = DefaultViewType;
		}
		bool ShouldSerializeValueLevel() {
			return this.valueLevel != DefaultValueLevel;
		}
		void ResetValueLevel() {
			ValueLevel = DefaultValueLevel;
		}
		bool ShouldSerializeSeriesTransparency() {
			return transparency != null;
		}
		void ResetSeriesTransparency() {
			SeriesTransparency = null;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeVisible() || ShouldSerializeViewType() || ShouldSerializeValueLevel();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new RangeControlOptions(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RangeControlOptions options = obj as RangeControlOptions;
			if (options == null)
				return;
			this.visible = options.visible;
			this.viewType = options.viewType;
			this.valueLevel = options.valueLevel;
		}
	}
}
