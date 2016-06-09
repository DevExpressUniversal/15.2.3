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
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(StockPointOptions.TypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class StockPointOptions : PointOptions {
		#region inner classes
		internal class TypeConverter : PointOptionsTypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) ||
					base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if(destinationType == typeof(InstanceDescriptor)) {
					ConstructorInfo ci = typeof(StockPointOptions).GetConstructor(new Type[] { });
					return new InstanceDescriptor(ci, null, false);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		#endregion
		const StockLevel defaultValueLevel = StockLevel.Close;
		StockLevel valueLevel = defaultValueLevel;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("StockPointOptionsValueLevel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.StockPointOptions.ValueLevel"),
		XtraSerializableProperty
		]
		public StockLevel ValueLevel {
			get { return this.valueLevel; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				this.valueLevel = value;
				RaiseControlChanged();
			}
		}
		public StockPointOptions() : base() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "ValueLevel")
				return ShouldSerializeValueLevel();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeValueLevel() {
			return false;
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new StockPointOptions();
		}
		protected internal override ValueToStringConverter CreateValueToStringConverter() {
			return new StockValueToStringConverter(ValueNumericOptions, ValueDateTimeOptions, valueLevel);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			StockPointOptions options = obj as StockPointOptions;
			if(options == null)
				return;
			this.valueLevel = options.valueLevel;
		}
		public override bool Equals(object obj) {
			StockPointOptions options = obj as StockPointOptions;
			return
				options != null &&
				base.Equals(obj) &&
				this.valueLevel == options.valueLevel;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class StockValueToStringConverter : ValueToStringConverter {
		readonly StockLevel valueLevel;
		public StockValueToStringConverter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions, StockLevel valueLevel) : base(numericOptions, dateTimeOptions) {
			this.valueLevel = valueLevel;
		}
		protected override object GetValue(object[] values) {
			switch (valueLevel) {
				case StockLevel.Low:
					return values[0];
				case StockLevel.High:
					return values[1];
				case StockLevel.Open:
					return values[2];
				case StockLevel.Close:
					return values[3];
				default:
					throw new DefaultSwitchException();
			}
		}
	}
}
