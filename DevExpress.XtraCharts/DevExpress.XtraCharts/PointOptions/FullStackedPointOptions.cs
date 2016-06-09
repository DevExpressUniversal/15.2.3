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
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraCharts {
	public abstract class FullStackedPointOptions : PointOptions {
		PercentOptions percentOptions;
		protected internal override bool ShouldSynchronizeValuePercentPrecision { get { return percentOptions.ValueAsPercent; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FullStackedPointOptionsPercentOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FullStackedPointOptions.PercentOptions"), 
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public PercentOptions PercentOptions { get { return percentOptions; } }
		public FullStackedPointOptions() : base() {
			this.percentOptions = new PercentOptions(this);
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "PercentOptions")
				return ShouldSerializePercentOptions();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializePercentOptions() {
			return false;
		}		
		#endregion
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			FullStackedPointOptions options = obj as FullStackedPointOptions;
			if (options != null)
				percentOptions.Assign(options.percentOptions);
		}
		public override bool Equals(object obj) {
			FullStackedPointOptions options = obj as FullStackedPointOptions;
			return options != null && base.Equals(obj) && percentOptions.Equals(options.percentOptions);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		protected internal override int GetValuePercentPrecisionForSynchronization() {
			return percentOptions.PercentPrecision;
		}
	}
	[
	TypeConverter(typeof(FullStackedAreaPointOptions.TypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FullStackedAreaPointOptions : FullStackedPointOptions {
		#region inner classes
		internal class TypeConverter : PointOptionsTypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor)) {
					ConstructorInfo ci = typeof(FullStackedAreaPointOptions).GetConstructor(new Type[] {});
					return new InstanceDescriptor(ci, null, false);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new FullStackedAreaPointOptions();
		}
	}
	[
	TypeConverter(typeof(FullStackedBarPointOptions.TypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FullStackedBarPointOptions : FullStackedPointOptions {
		#region inner classes
		internal class TypeConverter : PointOptionsTypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor)) {
					ConstructorInfo ci = typeof(FullStackedBarPointOptions).GetConstructor(new Type[] {});
					return new InstanceDescriptor(ci, null, false);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new FullStackedBarPointOptions();
		}
	}
	[
	TypeConverter(typeof(FullStackedLinePointOptions.TypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FullStackedLinePointOptions : FullStackedPointOptions {
		#region inner classes
		internal class TypeConverter : PointOptionsTypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor)) {
					ConstructorInfo ci = typeof(FullStackedLinePointOptions).GetConstructor(new Type[] { });
					return new InstanceDescriptor(ci, null, false);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new FullStackedLinePointOptions();
		}
	}
}
