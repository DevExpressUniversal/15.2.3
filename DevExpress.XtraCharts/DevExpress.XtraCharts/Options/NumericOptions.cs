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

using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
namespace DevExpress.XtraCharts {
	[DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions, "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(NumericOptionsTypeConverter))]
	public sealed class NumericOptions : ChartElement, INumericOptions {
		const int DefaultPrecision = 2;
		const NumericFormat DefaultFormat = NumericFormat.General;
		NumericFormat format = DefaultFormat;
		int precision = DefaultPrecision;
		bool formatPropertyWasSet = false;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("NumericOptionsFormat"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.NumericOptions.Format"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public NumericFormat Format {
			get { return format; }
			set {
				if (value != format) {
					SendNotification(new ElementWillChangeNotification(this));
					formatPropertyWasSet = true;
					format = value;
					if (ChartContainer != null && ChartContainer.DesignMode && !Loading) {
						if (format == NumericFormat.Percent) {
							PointOptions pointOptions = Owner as PointOptions;
							if (pointOptions != null && pointOptions.ShouldSynchronizeValuePercentPrecision)
								SynchronizePrecision(pointOptions.GetValuePercentPrecisionForSynchronization());
						}
					}
				}
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("NumericOptionsPrecision"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.NumericOptions.Precision"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int Precision {
			get { return precision; }
			set {
				if (value != precision) {
					if (value < 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectNumericPrecision));
					SendNotification(new ElementWillChangeNotification(this));
					precision = value;
					RaiseControlChanged();
				}
			}
		}
		NumericOptionsFormat INumericOptions.Format { get { return (NumericOptionsFormat)format; } }
		internal NumericOptions(ChartElement owner)
			: base(owner) {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Format":
					return true;
				case "Precision":
					return ShouldSerializePrecision();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeFormat() {
			if (this.format == DefaultFormat && !this.formatPropertyWasSet)
				return false;
			else
				return true;
		}
		void ResetFormat() {
			Format = DefaultFormat;
		}
		bool ShouldSerializePrecision() {
			return precision != DefaultPrecision;
		}
		void ResetPrecision() {
			Precision = DefaultPrecision;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeFormat() || ShouldSerializePrecision();
		}
		#endregion
		internal void SynchronizePrecision(int precision) {
			ChartDebug.Assert(precision >= 0);
			if (precision >= 0)
				this.precision = precision;
		}
		protected override ChartElement CreateObjectForClone() {
			return new NumericOptions(null);
		}
		public override string ToString() {
			return "(NumericOptions)";
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			NumericOptions options = obj as NumericOptions;
			if (options != null) {
				format = options.format;
				precision = options.precision;
			}
		}
		public override bool Equals(object obj) {
			NumericOptions options = obj as NumericOptions;
			return options != null && format == options.format && precision == options.precision;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
