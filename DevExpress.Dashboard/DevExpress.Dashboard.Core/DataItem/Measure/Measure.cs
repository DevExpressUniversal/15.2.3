#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data.PivotGrid;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
namespace DevExpress.DashboardCommon {
	public enum SummaryType {
		Count = PivotSummaryType.Count,
		Sum = PivotSummaryType.Sum,
		Min = PivotSummaryType.Min,
		Max = PivotSummaryType.Max,
		Average = PivotSummaryType.Average,
		StdDev = PivotSummaryType.StdDev,
		StdDevp = PivotSummaryType.StdDevp,
		Var = PivotSummaryType.Var,
		Varp = PivotSummaryType.Varp,
		CountDistinct = 10
	}
	[
	DesignerSerializer(TypeNames.ISupportInitializeCodeDomSerializer, TypeNames.CodeDomSerializer),
	TypeConverter(TypeNames.MeasureConverter)
	]
	public class Measure : DataItem {
		const string xmlSummaryType = "SummaryType";
		internal static string GetSummaryTypeCaption(SummaryType summaryType) {
			switch(summaryType) {
				case SummaryType.Count:
					return DashboardLocalizer.GetString(DashboardStringId.SummaryTypeCount);
				case SummaryType.CountDistinct:
					return DashboardLocalizer.GetString(DashboardStringId.SummaryTypeCountDistinct);
				case SummaryType.Sum:
					return DashboardLocalizer.GetString(DashboardStringId.SummaryTypeSum);
				case SummaryType.Min:
					return DashboardLocalizer.GetString(DashboardStringId.SummaryTypeMin);
				case SummaryType.Max:
					return DashboardLocalizer.GetString(DashboardStringId.SummaryTypeMax);
				case SummaryType.Average:
					return DashboardLocalizer.GetString(DashboardStringId.SummaryTypeAverage);
				case SummaryType.StdDev:
					return DashboardLocalizer.GetString(DashboardStringId.SummaryTypeStdDev);
				case SummaryType.StdDevp:
					return DashboardLocalizer.GetString(DashboardStringId.SummaryTypeStdDevp);
				case SummaryType.Var:
					return DashboardLocalizer.GetString(DashboardStringId.SummaryTypeVar);
				case SummaryType.Varp:
					return DashboardLocalizer.GetString(DashboardStringId.SummaryTypeVarp);
				default:
					throw new Exception(Helper.GetUnknownEnumValueMessage(summaryType));
			}
		}
		SummaryType summaryType;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("MeasureSummaryType"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(MeasureDefinition.DefaultSummaryType)
		]
		public SummaryType SummaryType {
			get { return summaryType; }
			set {
				if(value != summaryType) {
					MeasureDefinition definition = GetMeasureDefinition();
					summaryType = value;
					OnChanged(ChangeReason.ClientData, definition);
				}
			}
		}
		protected internal override int ActualGroupIndex { get { return Dimension.DefaultGroupIndex; } }
		protected override bool FormatDecimalAsCurrency { get { return summaryType != SummaryType.Count && summaryType != SummaryType.CountDistinct && summaryType != SummaryType.Var && summaryType != SummaryType.Varp; } }
		protected override bool CanFormatValueAsDateTime { get { return IsNativeActualDataFieldType; } }
		protected internal override bool CanSpecifyDateTimeFormat { get { return base.CanSpecifyDateTimeFormat && IsNativeActualDataFieldType; } }
		protected internal override bool CanSpecifyNumericFormat {
			get {
				if(base.CanSpecifyNumericFormat)
					return true;
				if(DataSourceSchema != null && DataSourceSchema.GetIsOlap())
					return true;
				if((DataSourceSchema == null || !DataSourceSchema.IsAggregateCalcField(DataMember)) && (summaryType == SummaryType.Count || summaryType == SummaryType.CountDistinct))
					return true;
				return false;
			}
		}
		bool IsNativeActualDataFieldType { get { return summaryType == SummaryType.Min || summaryType == SummaryType.Max; } }
		internal override DataFieldType ActualDataFieldType {
			get {
				DataFieldType actualDataFieldType = base.ActualDataFieldType;
				DataFieldType calculatedMeasureType = DataFieldType.Double;
				if(DataSourceSchema != null && (DataSourceSchema.IsAggregateCalcField(DataMember) || DataSourceSchema.GetIsOlap())) {
					return DataSourceSchema.GetIsOlap() && actualDataFieldType == DataFieldType.Custom ? calculatedMeasureType : actualDataFieldType;
				} else {
					if(summaryType == SummaryType.Count || summaryType == SummaryType.CountDistinct)
						return DataFieldType.Integer;
					if(summaryType == SummaryType.Sum || summaryType == SummaryType.Average || summaryType == SummaryType.StdDev ||
						summaryType == SummaryType.StdDevp || summaryType == SummaryType.Var || summaryType == SummaryType.Varp)
						return DataFieldType.Decimal;
					return actualDataFieldType;
				}
			}
		}
		public Measure()
			: this((string)null) {
		}
		public Measure(string dataMember)
			: this(new MeasureDefinition(dataMember)) {
		}
		public Measure(string dataMember, SummaryType summaryType)
			: this(new MeasureDefinition(dataMember, summaryType)) {
		}
		public Measure(string id, string dataMember)
			: this(id, new MeasureDefinition(dataMember)) {
		}
		public Measure(string id, string dataMember, SummaryType summaryType)
			: this(id, new MeasureDefinition(dataMember, summaryType)) {
		}
		public Measure(MeasureDefinition definition)
			: this(null, definition) {
		}
		public Measure(string id, MeasureDefinition definition)
			: base(id, definition) {
			summaryType = definition.SummaryType;
		}
		public MeasureDefinition GetMeasureDefinition() {
			return new MeasureDefinition(DataMember, SummaryType);
		}
		public override DataItemDefinition GetDefinition() {
			return GetMeasureDefinition();
		}
		protected override string GetNameSuffix() {
			if(DataSourceSchema!= null && DataSourceSchema.IsAggregateCalcField(DataMember))
				return null;
			return GetSummaryTypeCaption(SummaryType);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(SummaryType != MeasureDefinition.DefaultSummaryType)
				element.Add(new XAttribute(xmlSummaryType, SummaryType));
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			string summaryTypeAttr = XmlHelper.GetAttributeValue(element, xmlSummaryType);
			if(summaryTypeAttr != null)
				summaryType = XmlHelper.EnumFromString<SummaryType>(summaryTypeAttr);
		}
		protected override ValueFormatViewModel CreateDefaultValueFormatViewModel() {
			return new ValueFormatViewModel(CreateNumericFormatViewModel());
		}
		internal IList<SummaryType> GetSupportedSummaryTypes() {
			IDataSourceSchema dataSource = DataSourceSchema;
			if(dataSource == null || dataSource.GetIsOlap())
				return null;
			DataField dataField = dataSource.GetField(DataMember);
			if(dataField == null || dataSource.IsAggregateCalcField(DataMember))
				return  null;
			SummaryTypeInfo summaryTypeInfo = Context != null ? Context.GetSummaryTypeInfo(this) : null;
			if(summaryTypeInfo == null)
				return null;
			return GetSupportedSummaryTypesCore(dataField, summaryTypeInfo);
		}
		IList<SummaryType> GetSupportedSummaryTypesCore(DataField dataField, SummaryTypeInfo summaryTypeInfo) {
			DataFieldType fieldType = dataField.FieldType;
			if(DataBindingHelper.IsText(fieldType))
				return summaryTypeInfo.TextSummaryTypes;
			else if(DataBindingHelper.IsBoolean(fieldType))
				return summaryTypeInfo.BooleanSummaryTypes;
			else if(DataBindingHelper.IsDateTime(fieldType))
				return summaryTypeInfo.DateTimeSummaryTypes;
			else if(DataBindingHelper.IsNumeric(fieldType))
				return summaryTypeInfo.NumericSummaryTypes;
			else if(DataBindingHelper.IsCustom(fieldType)) {
				if(dataField.IsConvertible) {
					if(dataField.IsComparable)
						return summaryTypeInfo.ConvertibleComparableSummaryTypes;
					else
						return summaryTypeInfo.ConvertibleSummaryTypes;
				}
				else {
					if(dataField.IsComparable)
						return summaryTypeInfo.ComparableSummaryTypes;
					else
						return summaryTypeInfo.ObjectSummaryTypes;
				}
			}
			else
				return summaryTypeInfo.ObjectSummaryTypes;
		}
		protected override void Assign(DataItem dataItem) {
			base.Assign(dataItem);
			Measure measure = dataItem as Measure;
			if (measure != null)
				SummaryType = measure.SummaryType;
		}
		internal Measure Clone() {
			Measure clone = new Measure();
			clone.Assign(this);
			return clone;
		}
	}
}
