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
	public abstract class SimplePointOptions : PointOptions {
		PercentOptions percentOptions;
		protected internal override bool ShouldSynchronizeValuePercentPrecision { get { return percentOptions.ValueAsPercent; } }
		protected abstract bool DefaultvalueAsPercent { get; }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SimplePointOptionsPercentOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SimplePointOptions.PercentOptions"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public PercentOptions PercentOptions { get { return this.percentOptions; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty(),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new DateTimeOptions ValueDateTimeOptions { get { return base.ValueDateTimeOptions; } }
		protected SimplePointOptions() : base() {
			percentOptions = new PercentOptions(this, DefaultvalueAsPercent);
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "PercentOptions")
				return ShouldSerializePercentOptions();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializePercentOptions() {
			return false;
		}
		#endregion        
		protected internal override int GetValuePercentPrecisionForSynchronization() {
			return percentOptions.PercentPrecision;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SimplePointOptions options = obj as SimplePointOptions;
			if (options == null)
				return;
			this.percentOptions.Assign(options.percentOptions);
		}
		public override bool Equals(object obj) {
			SimplePointOptions options = obj as SimplePointOptions;
			return
				options != null &&
				base.Equals(obj) &&
				this.percentOptions.Equals(options.percentOptions);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[
	TypeConverter(typeof(PiePointOptions.TypeConverter)),
		DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PiePointOptions : SimplePointOptions {
		internal class TypeConverter : PointOptionsTypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) ||
					base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if(destinationType == typeof(InstanceDescriptor)) {
					ConstructorInfo ci = typeof(PiePointOptions).GetConstructor(new Type[] { });
					return new InstanceDescriptor(ci, null, false);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		protected override bool DefaultvalueAsPercent { get { return true; } }
		public PiePointOptions() : base() {
			ValueNumericOptions.Format = NumericFormat.Percent;
		}
		protected override ChartElement CreateObjectForClone() {
			return new PiePointOptions();
		}
		public override bool Equals(object obj) {
			PiePointOptions options = obj as PiePointOptions;
			return
				options != null &&
				base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[
	TypeConverter(typeof(FunnelPointOptions.TypeConverter)),
		DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FunnelPointOptions : SimplePointOptions {
		internal class TypeConverter : PointOptionsTypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) ||
					base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor)) {
					ConstructorInfo ci = typeof(FunnelPointOptions).GetConstructor(new Type[] { });
					return new InstanceDescriptor(ci, null, false);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		protected override bool DefaultvalueAsPercent { get { return false; } }
		public FunnelPointOptions() : base() { }
		protected override ChartElement CreateObjectForClone() {
			return new FunnelPointOptions();
		}
		public override bool Equals(object obj) {
			FunnelPointOptions options = obj as FunnelPointOptions;
			return
				options != null &&
				base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
