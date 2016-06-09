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
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(PercentOptionsTypeConverter))
	]
	public class PercentOptions : ChartElement {
		const int DefaultValuePercentageAccuracy = 4;
		bool defaultValueAsPercent;
		bool valueAsPercent;
		int percentageAccuracy = DefaultValuePercentageAccuracy;
		internal int PercentPrecision { get { return Math.Max(percentageAccuracy - 2, 0); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PercentOptionsValueAsPercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PercentOptions.ValueAsPercent"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool ValueAsPercent {
			get { return valueAsPercent; }
			set {
				if (value != valueAsPercent) {
					SendNotification(new ElementWillChangeNotification(this));
					valueAsPercent = value;
					if (valueAsPercent)
						SynchronizeValueNumericOptionsPrecision();
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PercentOptionsPercentageAccuracy"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PercentOptions.PercentageAccuracy"),
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public int PercentageAccuracy {
			get { return percentageAccuracy; }
			set {
				if (value <= 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPercentageAccuracy));
				if (value != percentageAccuracy) {
					SendNotification(new ElementWillChangeNotification(this));
					percentageAccuracy = value;
					SynchronizeValueNumericOptionsPrecision();
					RaiseControlChanged();
				}
			}
		}
		internal PercentOptions(ChartElement owner) : this(owner, true) {
		}
		internal PercentOptions(ChartElement owner, bool defaultValueAsPercent) : base(owner) {
			this.defaultValueAsPercent = defaultValueAsPercent;
			valueAsPercent = defaultValueAsPercent;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "ValueAsPercent":
					return ShouldSerializeValueAsPercent();
				case "PercentageAccuracy":
					return ShouldSerializePercentageAccuracy();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeValueAsPercent() {
			return valueAsPercent != defaultValueAsPercent;
		}
		void ResetValueAsPercent() {
			ValueAsPercent = defaultValueAsPercent;
		}
		bool ShouldSerializePercentageAccuracy() {
			return percentageAccuracy != DefaultValuePercentageAccuracy;
		}
		void ResetPercentageAccuracy() {
			PercentageAccuracy = DefaultValuePercentageAccuracy;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeValueAsPercent() || ShouldSerializePercentageAccuracy();
		}
		#endregion
		void SynchronizeValueNumericOptionsPrecision() {
			if (ChartContainer != null && ChartContainer.DesignMode && !Loading) {
				PointOptions pointOptions = Owner as PointOptions;
				if (pointOptions != null && pointOptions.ValueNumericOptions.Format == NumericFormat.Percent)
					pointOptions.ValueNumericOptions.SynchronizePrecision(PercentPrecision);
			}
		}
		protected override ChartElement CreateObjectForClone() {
			return new PercentOptions(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			PercentOptions options = obj as PercentOptions;
			if (options != null) {
				valueAsPercent = options.valueAsPercent;
				percentageAccuracy = options.percentageAccuracy;
			}
		}
		public override bool Equals(object obj) {
			PercentOptions options = obj as PercentOptions;
			return options != null && valueAsPercent == options.valueAsPercent && percentageAccuracy == options.percentageAccuracy;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
