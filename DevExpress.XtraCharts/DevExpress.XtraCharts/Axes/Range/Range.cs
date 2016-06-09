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
using System.Globalization;
namespace DevExpress.XtraCharts {
	public abstract class Range : ChartElement {
		const bool DefaultAuto = true;
		const bool DefaultAutoSideMargins = true;
		readonly protected RangeDataBase data;
		RangeDataBase.Deserializer deserializer;
		internal RangeDataBase.Deserializer Deserializer {
			get {
				if (deserializer == null)
					deserializer = data.GetDeserializer();
				return deserializer;
			}
		}
		internal RangeDataBase RangeData { get { return data; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeMinValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Range.MinValue"),
		TypeConverter(typeof(RangeValueTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All)
		]
		public object MinValue {
			get { return data.MinValue; }
			set {
				ResetDeserializer();
				data.MinValue = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeMaxValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Range.MaxValue"),
		TypeConverter(typeof(RangeValueTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All)
		]
		public object MaxValue {
			get { return data.MaxValue; }
			set {
				ResetDeserializer();
				data.MaxValue = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeMinValueInternal"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Range.MinValueInternal"),
		RefreshProperties(RefreshProperties.All),
		]
		public double MinValueInternal {
			get { return data.MinValueInternal; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeMaxValueInternal"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Range.MaxValueInternal"),
		RefreshProperties(RefreshProperties.All),
		]
		public double MaxValueInternal {
			get { return data.MaxValueInternal; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeAuto"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Range.Auto"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty
		]
		public bool Auto {
			get { return data.CorrectionMode == RangeCorrectionMode.Auto; }
			set {
				ResetDeserializer();
				data.CorrectionMode = value ? RangeCorrectionMode.Auto : RangeCorrectionMode.Values;
				if (Loading)
					Deserializer.CorrectionMode = value ? RangeCorrectionMode.Auto : RangeCorrectionMode.Values;
				else if (value)
					deserializer = null;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeAutoSideMargins"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Range.AutoSideMargins"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public bool AutoSideMargins {
			get { return data.AutoSideMargins == SideMarginMode.Enable || data.AutoSideMargins == SideMarginMode.UserEnable; }
			set {
				if (value != (data.AutoSideMargins == SideMarginMode.Enable || data.AutoSideMargins == SideMarginMode.UserEnable)) {
					ResetDeserializer();
					data.AutoSideMargins = value ? SideMarginMode.UserEnable : SideMarginMode.UserDisable;
				}
				if (Loading)
					Deserializer.AutoSideMargins = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeSideMarginsValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Range.SideMarginsValue"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public double SideMarginsValue {
			get { return data.SideMarginsValue; }
			set {
				ResetDeserializer();
				data.SideMarginsValue = value;
				if (Loading)
					Deserializer.SideMarginsValue = value;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty(),
		XtraSerializableProperty
		]
		public virtual string MinValueSerializable {
			get { return SerializingUtils.ConvertToSerializable(data.MinValue); }
			set { Deserializer.MinValueSerializable = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty(),
		XtraSerializableProperty
		]
		public virtual string MaxValueSerializable {
			get { return SerializingUtils.ConvertToSerializable(data.MaxValue); }
			set { Deserializer.MaxValueSerializable = value; }
		}
		internal Range(RangeDataBase data)
			: base(data.Axis) {
			this.data = data;
		}
		public void SetMinMaxValues(object minValue, object maxValue) {
			ResetDeserializer();
			data.SetRange(minValue, maxValue, double.NaN, double.NaN, true);
			if (!data.Axis.IsValuesAxis)
				data.Axis.UpdateAutoMeasureUnit(true);
		}
		void ResetDeserializer() {
			if (!Loading)
				deserializer = null;
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeMinValueSerializable() {
			return data.CorrectionMode != RangeCorrectionMode.Auto;
		}
		bool ShouldSerializeMaxValueSerializable() {
			return data.CorrectionMode != RangeCorrectionMode.Auto;
		}
		bool ShouldSerializeMinValueInternal() {
			return false;
		}
		bool ShouldSerializeMaxValueInternal() {
			return false;
		}
		bool ShouldSerializeMinValue() {
			return Auto != DefaultAuto;
		}
		void ResetMinValue() {
			Auto = DefaultAuto;
		}
		bool ShouldSerializeMaxValue() {
			return Auto != DefaultAuto;
		}
		void ResetMaxValue() {
			Auto = DefaultAuto;
		}
		bool ShouldSerializeAuto() {
			return Auto != DefaultAuto;
		}
		void ResetAuto() {
			Auto = DefaultAuto;
		}
		bool ShouldSerializeAutoSideMargins() {
			return AutoSideMargins != DefaultAutoSideMargins;
		}
		void ResetAutoSideMargins() {
			AutoSideMargins = DefaultAutoSideMargins;
		}
		bool ShouldSerializeSideMarginsValue() {
			return AutoSideMargins != DefaultAutoSideMargins;
		}
		void ResetSideMarginsValue() {
			AutoSideMargins = true;
		}
		protected internal override bool ShouldSerialize() {
			return
				ShouldSerializeAutoSideMargins() ||
				ShouldSerializeSideMarginsValue() ||
				ShouldSerializeMinValueSerializable() ||
				ShouldSerializeMaxValueSerializable() ||
				ShouldSerializeAuto() ||
				ShouldSerializeMinValueInternal() ||
				ShouldSerializeMaxValueInternal();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Auto":
					return ShouldSerializeAuto();
				case "MinValueSerializable":
					return ShouldSerializeMinValueSerializable();
				case "MaxValueSerializable":
					return ShouldSerializeMaxValueSerializable();
				case "MinValueInternal":
					return ShouldSerializeMinValueInternal();
				case "MaxValueInternal":
					return ShouldSerializeMaxValueInternal();
				case "AutoSideMargins":
					return ShouldSerializeAutoSideMargins();
				case "SideMarginsValue":
					return ShouldSerializeSideMarginsValue();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		internal RangeDataBase.Deserializer GetDeserializer() { return deserializer; }
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Range range = obj as Range;
			this.deserializer = null;
			if (range != null && range.deserializer != null) {
				this.deserializer = new RangeDataBase.Deserializer(data.Axis);
				this.deserializer.Assign(range.deserializer);
			}
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					  "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(VisualRangeTypeConverter))
	]
	public class VisualRange : Range {
		VisualRangeData Data { get { return (VisualRangeData)data; } }
		internal VisualRange(VisualRangeData data)
			: base(data) { }
		#region ShouldSerialize & Reset
		protected internal override bool ShouldSerialize() {
			return data.CorrectionMode != RangeCorrectionMode.Auto && base.ShouldSerialize();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return data.CorrectionMode != RangeCorrectionMode.Auto && base.XtraShouldSerialize(propertyName);
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new VisualRange(Data);
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(WholeRangeTypeConverter))
	]
	public class WholeRange : Range {
		const bool DefaultAlwaysShowZeroLevel = true;
		internal WholeRange(WholeRangeData data)
			: base(data) { }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("WholeRangeAlwaysShowZeroLevel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.VisualRange.AlwaysShowZeroLevel"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public bool AlwaysShowZeroLevel {
			get { return data.AlwaysShowZeroLevel; }
			set { data.AlwaysShowZeroLevel = value; }
		}
		protected override ChartElement CreateObjectForClone() {
			return new WholeRange((WholeRangeData)data);
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeAlwaysShowZeroLevel() {
			return AlwaysShowZeroLevel != DefaultAlwaysShowZeroLevel;
		}
		void ResetAlwaysShowZeroLevel() {
			AlwaysShowZeroLevel = DefaultAlwaysShowZeroLevel;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeAlwaysShowZeroLevel();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "AlwaysShowZeroLevel":
					return ShouldSerializeAlwaysShowZeroLevel();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
	}
}
