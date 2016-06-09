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
using System.Drawing.Design;
using System.Xml.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public abstract class DataItem : IDataItemRepositoryProvider, ISupportInitialize, IEditNameProvider {
		const string xmlID = "ID";
		const string xmlDataMember = "DataMember";
		const string EmptyName = "<DataMember is not set>";
		readonly DataItemNumericFormat numericFormat;
		readonly DataItemDateTimeFormat dateTimeFormat;
		readonly Locker loadingLocker = new Locker();
		readonly NameBox nameBox = new NameBox("Name");
		string id;
		string dataMember;
		[
		Browsable(false),
		DefaultValue(null)
		]
		public string ID {
			get { return id; } 
			set { id = value; } 
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemDataMember"),
#endif
		Category(CategoryNames.Data), 
		DefaultValue(null),
		Localizable(false),
		Editor(TypeNames.DataItemDataMemberEditor, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)
		]
		public string DataMember { 
			get { return dataMember; } 
			set {
				if(dataMember == value)
					return;
				dataMember = value; 
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemNumericFormat"),
#endif
		Category(CategoryNames.Format),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DataItemNumericFormat NumericFormat { get { return numericFormat; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemDateTimeFormat"),
#endif
		Category(CategoryNames.Format), 
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DataItemDateTimeFormat DateTimeFormat { get { return dateTimeFormat; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(true)
		]
		public string Name { get { return nameBox.Name; } set { nameBox.Name = value; } }
		[
		Browsable(false)
		]
		public Type DataSourceFieldType { get { return DataSourceSchema != null ? DataSourceSchema.GetFieldSourceType(dataMember) : null; } }
		internal string DisplayName { get { return nameBox.DisplayName; } }
		internal string GroupName {
			get {
				if (!string.IsNullOrEmpty(dataMember) && DataSourceSchema != null && DataSourceSchema.IsOlapHierarchyDataField(dataMember))
					return ConstructName(DataSourceSchema.GetFieldCaption(dataMember));
				return DisplayName;
			}
		}
		string DefaultName {
			get {
				string baseName = null;
				if (!string.IsNullOrEmpty(dataMember))
					baseName = DataSourceSchema != null ? DataSourceSchema.GetDataMemberCaption(dataMember) : dataMember;
				return ConstructName(baseName);
			}
		}
		string IEditNameProvider.EditName { get { return nameBox.EditName; } set { nameBox.EditName = value; } }
		string IEditNameProvider.DisplayName { get { return DisplayName; } }
		internal bool Loading { get { return loadingLocker.IsLocked; } }
		protected internal abstract int ActualGroupIndex { get; }
		protected internal IDataItemContext Context { get; set; }
		IChangeService ChangeService { get { return Context != null ? Context.ChangeService : null; } }
		IDataItemRepositoryProvider DataItemRepositoryProvider { get { return Context != null ? Context.DataItemRepositoryProvider : null; } }
		IDataSourceSchemaProvider DataSourceSchemaProvider { get { return Context != null ? Context.DataSourceSchemaProvider : null; } }
		ICurrencyCultureNameProvider CurrencyCultureNameProvider { get { return Context != null ? Context.CurrencyCultureNameProvider : null; } }
		DataItemRepository DataItemRepository { get { return DataItemRepositoryProvider != null ? DataItemRepositoryProvider.DataItemRepository : null; } }
		protected IDataSourceSchema DataSourceSchema { get { return DataSourceSchemaProvider != null ? DataSourceSchemaProvider.DataSourceSchema : null; } }
		protected internal string ActualId { get { return string.IsNullOrEmpty(id) ? (DataItemRepository != null ? DataItemRepository.GetActualID(this) : null) : id; } }
		protected internal string SerializableUniqueName { get { return DataItemRepository != null ? DataItemRepository.GetSerializableUniqueName(this) : null; } }
		DataItemRepository IDataItemRepositoryProvider.DataItemRepository { get { return DataItemRepository; } }
		protected abstract bool FormatDecimalAsCurrency { get; }
		protected abstract bool CanFormatValueAsDateTime { get; }
		protected internal virtual bool CanSpecifyDateTimeFormat { get { return DataFieldType == DataFieldType.DateTime; } }
		protected internal virtual bool CanSpecifyNumericFormat {
			get {
				switch(DataFieldType) {
					case DataFieldType.Integer:
					case DataFieldType.Float:
					case DataFieldType.Double:
					case DataFieldType.Decimal:
						return true;
					default:
						return false;
				}
			}
		}
		internal DataFieldType DataFieldType { get { return DataSourceSchema != null ? DataSourceSchema.GetFieldType(dataMember) : DataFieldType.Unknown; } }
		internal virtual DataFieldType ActualDataFieldType { get { return DataFieldType; } }
		protected DataItem(string id, DataItemDefinition definition) {
			Guard.ArgumentNotNull(definition, "definition");
			this.id = id;
			this.dataMember = definition.DataMember;
			numericFormat = new DataItemNumericFormat(this);
			dateTimeFormat = new DataItemDateTimeFormat(this);
			nameBox.NameChanged += (s, e) => OnChanged(ChangeReason.View, null);
			nameBox.RequestDefaultName += (s, e) => e.DefaultName = DefaultName;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void BeginInit() {
			loadingLocker.Lock();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void EndInit() {
			loadingLocker.Unlock();
		}
		public override string ToString() {
			return DisplayName;
		}
		string ConstructName(string baseName) {
			if (baseName != null) {				
				IDataSourceSchema dataSource = DataSourceSchema;
				if(dataSource != null)
					return dataSource.ConstructName(baseName, GetNameSuffix());
				return baseName;
			}
			return EmptyName;
		}
		public abstract DataItemDefinition GetDefinition();
		internal bool EqualsByDefinition(DataItemDefinition definition) {
			return GetDefinition().Equals(definition);
		}
		internal bool EqualsByDefinition(DataItem dataItem) {
			return dataItem != null && EqualsByDefinition(dataItem.GetDefinition());
		}
		protected internal void OnChanged() {
			OnChanged(ChangeReason.ClientData, null);
		}
		protected virtual void OnChanged(ChangeReason reason, DataItemDefinition definition) {
			IChangeService changeService = ChangeService;
			if (changeService != null)
				changeService.OnChanged(new ChangedEventArgs(reason, this, definition));
		}
		protected internal virtual void SaveToXml(XElement element) {
			if(!string.IsNullOrEmpty(ID))
				element.Add(new XAttribute(xmlID, id));
			if(!string.IsNullOrEmpty(DataMember))
				element.Add(new XAttribute(xmlDataMember, dataMember));
			if (numericFormat.ShouldSerialize())
				element.Add(numericFormat.SaveToXml());
			if (dateTimeFormat.ShouldSerialize())
				element.Add(dateTimeFormat.SaveToXml());
			nameBox.SaveToXml(element);
		}
		protected internal virtual void LoadFromXml(XElement element) {
			id = XmlHelper.GetAttributeValue(element, xmlID);
			dataMember = XmlHelper.GetAttributeValue(element, xmlDataMember);
			XElement numericFormatElement = element.Element(DataItemNumericFormat.XmlNumericFormat);
			if (numericFormatElement != null)
				numericFormat.LoadFromXml(numericFormatElement);
			XElement dateTimeFormatElement = element.Element(DataItemDateTimeFormat.XmlDateTimeFormat);
			if (dateTimeFormatElement != null)
				dateTimeFormat.LoadFromXml(dateTimeFormatElement);
		}
		internal string LoadNameFromXml(XElement element) {
			return nameBox.LoadNameFromXml(element);
		}
		protected internal virtual void OnEndLoading() { 
		}
		protected abstract string GetNameSuffix();
		protected abstract ValueFormatViewModel CreateDefaultValueFormatViewModel();
		internal NumericFormatViewModel CreateNumericFormatViewModel() {
			return CreateNumericFormatViewModel(this.numericFormat);
		}
		internal NumericFormatViewModel CreateNumericFormatViewModel(DataItemNumericFormat numericFormat) {
			ICurrencyCultureNameProvider currencyCultureNameProvider = CurrencyCultureNameProvider;
			string currencyCultureName = currencyCultureNameProvider != null ? currencyCultureNameProvider.CurrencyCultureName : Helper.DefaultCurrencyCultureName;
			return numericFormat.CreateViewModel(false, GetNumericFormatType(), currencyCultureName);
		}
		internal ValueFormatViewModel CreateValueFormatViewModel() {
			switch(DataFieldType) {
				case DataFieldType.DateTime:
					if(CanFormatValueAsDateTime)
						return new ValueFormatViewModel(new DateTimeFormatViewModel(DateTimeFormat));
					return CreateDefaultValueFormatViewModel();					
				case DataFieldType.Integer:
				case DataFieldType.Float:
				case DataFieldType.Double:
				case DataFieldType.Decimal:
					return new ValueFormatViewModel(CreateNumericFormatViewModel());
				default:
					return CreateDefaultValueFormatViewModel();
			}
		}
		internal NumericFormatViewModel CreateKpiFormatViewModel(DeltaValueType deltaValueType) {
			ICurrencyCultureNameProvider currencyCultureNameProvider = CurrencyCultureNameProvider;
			if(currencyCultureNameProvider != null) {
				switch(deltaValueType) {
					case DeltaValueType.PercentVariation:
						return new NumericFormatViewModel(NumericFormatType.Percent, 2, DataItemNumericUnit.Ones, false, true, 0, currencyCultureNameProvider.CurrencyCultureName);
					case DeltaValueType.AbsoluteVariation:
						return new NumericFormatViewModel(NumericFormatType.Number, 0, DataItemNumericUnit.Auto, false, true, 3, currencyCultureNameProvider.CurrencyCultureName);
					case DeltaValueType.PercentOfTarget:
						return new NumericFormatViewModel(NumericFormatType.Percent, 2, DataItemNumericUnit.Ones, false, false, 0, currencyCultureNameProvider.CurrencyCultureName);
					default:
						return CreateNumericFormatViewModel();
				}
			}
			else
				return null;
		}
		internal NumericFormatType GetNumericFormatType() {
			switch(DataFieldType) {
				case DataFieldType.Integer:
				case DataFieldType.Float:
				case DataFieldType.Double:
					return NumericFormatType.Number;
				case DataFieldType.Decimal:
					return FormatDecimalAsCurrency ? NumericFormatType.Currency : NumericFormatType.Number;
				default:
					return NumericFormatType.General;
			}
		}
		void AssignCore(DataItem dataItem) {
			DateTimeFormat.Assign(dataItem.DateTimeFormat);
			NumericFormat.Assign(dataItem.NumericFormat);
			Name = dataItem.Name;
			DataMember = dataItem.DataMember;
		}
		protected virtual void WeakAssign(DataItem dataItem) {
			AssignCore(dataItem);
		}
		protected virtual void Assign(DataItem dataItem) {
			AssignCore(dataItem);
		}
	}
}
