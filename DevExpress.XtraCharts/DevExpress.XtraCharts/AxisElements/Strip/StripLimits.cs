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
using System.Globalization;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[TypeConverter(typeof(StripLimitTypeConverter))]
	public abstract class StripLimit : ChartElement, IStripLimit {
		const bool DefaultEnabled = true;
		object axisValue;
		string axisValueSerializable;
		double value;
		bool enabled = true;
		internal Strip Strip { 
			get { return (Strip)Owner; } 
		}
		protected abstract double Infinity { 
			get; 
		}
		protected abstract string ValueExceptionString { 
			get; 
		}
		protected internal double ActualValue {
			get { return enabled ? value : Infinity; } 
		}
		double IStripLimit.Value { 
			get { return ActualValue; } 
		}
		IAxisData Axis { 
			get { return Owner == null ? null : ((Strip)Owner).Axis; } 
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty(),
		XtraSerializableProperty]
		public string AxisValueSerializable {
			get { return SerializingUtils.ConvertToSerializable(AxisValue); }
			set {
				AxisValue = value;
				if (Strip == null || Strip.Axis == null || Loading)
					axisValueSerializable = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("StripLimitAxisValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.StripLimit.AxisValue"),
		Category(Categories.Behavior),
		TypeConverter(typeof(AxisValueTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public object AxisValue {
			get { return axisValue; }
			set {
				if (!Loading && !CanSetAxisValue(value))
					throw new ArgumentException(ValueExceptionString);
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectStripLimitAxisValue));
				SendNotification(new ElementWillChangeNotification(this));
				axisValue = (Strip == null || Strip.Axis == null || Loading) ? value : Strip.Axis.ConvertBasedOnScaleType(value);
				SyncAxisValueAndEnabled();
				RaiseControlChanged(new AxisElementUpdateInfo(this, Axis as IAxisData));
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("StripLimitEnabled"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.StripLimit.Enabled"),
		RefreshProperties(RefreshProperties.All),
		Category(Categories.Behavior),
		XtraSerializableProperty,
		TypeConverter(typeof(BooleanTypeConverter)),]
		public bool Enabled {
			get { return enabled; }
			set {
				if (value != enabled) {
					SendNotification(new ElementWillChangeNotification(this));
					enabled = value;
					RaiseControlChanged(new AxisElementUpdateInfo(this, Axis as IAxisData));
					RaiseControlChanged();
					if (Strip != null)
						Strip.CorrectLimits(Axis as Axis2D);
				}
			}
		}
		protected StripLimit(Strip strip, double defaultValue)
			: base(strip) {
			value = defaultValue;
		}
		#region IAxisValueContainer implementation
		IAxisData IAxisValueContainer.Axis { get { return Axis; } }
		bool IAxisValueContainer.IsEnabled { get { return Strip.Visible && Enabled; } }
		CultureInfo IAxisValueContainer.Culture {
			get {
				return (axisValueSerializable != null && axisValueSerializable.Equals(axisValue)) ?
					CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
			}
		}
		void IAxisValueContainer.SetValue(double value) {
			if (Double.IsInfinity(value))
				enabled = false;
			else
				this.value = value;
		}
		double IAxisValueContainer.GetValue() {
			return value;
		}
		void IAxisValueContainer.SetAxisValue(object axisValue) {
			if (!Object.Equals(axisValue, this.axisValue)) {
				this.axisValue = axisValue;
				axisValueSerializable = null;
			}
		}
		object IAxisValueContainer.GetAxisValue() {
			return axisValue;
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeAxisValueSerializable() {
			return enabled;
		}
		bool ShouldSerializeValue() {
			return false;
		}
		bool ShouldSerializeEnabled() {
			return enabled != DefaultEnabled;
		}
		void ResetEnabled() {
			Enabled = DefaultEnabled;
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "AxisValueSerializable":
					return ShouldSerializeAxisValueSerializable();
				case "Enabled":
					return ShouldSerializeEnabled();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected abstract bool CanSetAxisValue(object axisValue);
		protected abstract bool CanSetValue(double value);
		internal void SyncAxisValueAndEnabled() {
			if (axisValue is double)
				enabled = !Double.IsInfinity((double)axisValue);
			else if (axisValue is DateTime)
				enabled = (DateTime)axisValue != DateTime.MinValue && (DateTime)axisValue != DateTime.MaxValue;
			else
				enabled = true;
		}
		public override string ToString() {
			return "(StripLimit)";
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			StripLimit limit = obj as StripLimit;
			if (limit != null) {
				axisValue = limit.AxisValue;
				value = limit.value;
				enabled = limit.enabled;
				axisValueSerializable = limit.axisValueSerializable;
			}
		}
	}
	[DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")]
	public sealed class MinStripLimit : StripLimit {
		const double defailtValue = 0.0;
		MaxStripLimit maxLimit;
		protected override double Infinity { 
			get { return Double.NegativeInfinity; } 
		}
		protected override string ValueExceptionString {
			get { return ChartLocalizer.GetString(ChartStringId.MsgIncorrectStripMinLimit); }
		}
		internal MinStripLimit(Strip strip)
			: base(strip, defailtValue) {
		}
		protected override bool CanSetAxisValue(object axisValue) {
			return Owner == null || !maxLimit.Enabled || StripLimitsUtils.CheckLimits(Strip.Axis, axisValue, maxLimit.AxisValue);
		}
		protected override bool CanSetValue(double value) {
			return value < ((IAxisValueContainer)maxLimit).GetValue();
		}
		protected override ChartElement CreateObjectForClone() {
			return new MinStripLimit(null);
		}
		internal void SetMaxLimit(MaxStripLimit maxLimit) {
			ChartDebug.Assert(maxLimit != null, "maxLimit can't be null");
			this.maxLimit = maxLimit;
		}
	}
	[DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")]
	public sealed class MaxStripLimit : StripLimit {
		const double defaultValue = 1.0;
		MinStripLimit minLimit;
		protected override double Infinity { 
			get { return Double.PositiveInfinity; } 
		}
		protected override string ValueExceptionString { 
			get { return ChartLocalizer.GetString(ChartStringId.MsgIncorrectStripMaxLimit); } 
		}
		internal MaxStripLimit(Strip strip)
			: base(strip, defaultValue) {
		}
		protected override bool CanSetAxisValue(object axisValue) {
			return Owner == null || !minLimit.Enabled || StripLimitsUtils.CheckLimits(Strip.Axis, minLimit.AxisValue, axisValue);
		}
		protected override bool CanSetValue(double value) {
			return value > ((IAxisValueContainer)minLimit).GetValue();
		}
		protected override ChartElement CreateObjectForClone() {
			return new MaxStripLimit(null);
		}
		internal void SetMinLimit(MinStripLimit minLimit) {
			ChartDebug.Assert(minLimit != null, "minLimit can't be null");
			this.minLimit = minLimit;
		}
	}
}
