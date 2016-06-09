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

namespace DevExpress.XtraVerticalGrid.Data {
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using DevExpress.Data;
	using DevExpress.XtraVerticalGrid.Rows;
	using System.Collections;
	using System.Globalization;
	using DevExpress.XtraEditors.Repository;
	using System.Collections.Generic;
	using System.ComponentModel.Design;
	using System.Drawing.Design;
	using DevExpress.XtraEditors.DXErrorProvider;
	using DevExpress.Data.Filtering;
	public class VGridDataManager : CurrencyDataController {
		DataModeHelper dataModeHelper;
		bool isDisposing;
		public VGridDataManager() {
			this.dataModeHelper = CreateDataModeHelper(null);
		}
		public void SetGridDataSource(BindingContext bindingContext, object dataSource, string dataMember) {
			this.dataModeHelper = CreateDataModeHelper(dataSource);
			SetDataSource(bindingContext, dataSource, dataMember);
		}
		protected virtual DataModeHelper CreateDataModeHelper(object dataSource) {
			if(dataSource == null)
				return new UnboundDataModeHelper(this);
			return new BoundDataModeHelper(this);
		}
		public DataModeHelper DataModeHelper { get { return dataModeHelper; } }
		public virtual int RecordCount { get { return DataModeHelper.RecordCount; } }
		public bool AllowAppend { get { return DataModeHelper.AllowAppend; } }
		public override void Dispose() {
			isDisposing = true;
			base.Dispose();
		}
		public override void PopulateColumns() {
			if(this.isDisposing) return;
			base.PopulateColumns();
		}
	}
	public class PGridDataManager : VGridDataManager {
		PropertyGridControl serviceProvider;
		public PGridDataManager(PropertyGridControl serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		protected override DataModeHelper CreateDataModeHelper(object dataSource) {
			return new PGridDataModeHelper(this, (object[])dataSource, serviceProvider);
		}
	}
	public abstract class DataModeHelper {
		VGridDataManager manager;
		public DataModeHelper(VGridDataManager manager) {
			this.manager = manager;
		}
		public virtual bool IsBound { get { return true; } }
		public abstract bool FilterByRows { get; }
		public abstract void AddNewRecord();
		public abstract void DeleteRecord(int recordIndex);
		public virtual bool IsCellDefaultValue(RowProperties props, int recordIndex) {
			return true;
		}
		public virtual int GetCorrectRecordIndex(int recordIndex) {
			return recordIndex;
		}
		public object GetCellValue(RowProperties props, int recordIndex) {
			if(props == null) return null;
			return GetCellValueCore(props, recordIndex);
		}
		public virtual string GetRowDataError(RowProperties props, int recordIndex) {
			if(props == null)
				return Manager.GetErrorText(recordIndex);
			return Manager.GetErrorText(recordIndex, props.RowHandle);
		}
		public virtual DevExpress.XtraEditors.DXErrorProvider.ErrorType GetRowDataErrorType(RowProperties props, int recordIndex) {
			if(props == null) return DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default;
			return Manager.GetErrorType(recordIndex, props.RowHandle);
		} 
		public virtual void SetCellValue(RowProperties props, int recordIndex, object value, bool fromEditor) {
			if(props == null) return;
			SetCellValueCore(props, recordIndex, value, fromEditor);
		}
		public virtual Type GetDataType(RowProperties props) {
			DataColumnInfo col = Manager.Columns[props.RowHandle];
			if(col == null)
				return GetInvalidDataColumnType(props);
			return col.Type;
		}
		public virtual int GetRowHandleByFieldName(RowProperties props) { return -1; }
		public virtual int CheckValidRecord(int recordIndex) {
			if(recordIndex == CurrencyDataController.InvalidRow) return -1;
			if(recordIndex > RecordCount - 1) recordIndex = RecordCount - 1;
			if(recordIndex < 0) recordIndex = 0;
			return recordIndex;
		}
		public static int NewItemRecord { get { return CurrencyDataController.NewItemRow; } }
		public abstract bool GetPredefinedReadOnlyByFieldName(RowProperties props);
		public abstract void CancelCurrentRowEdit();
		public abstract void BeginCurrentRowEdit();
		protected internal virtual void StartNewItemRow() { }
		protected internal virtual void EndNewItemRow() { }
		protected virtual Type GetInvalidDataColumnType(RowProperties props) { return typeof(object); }
		protected abstract object GetCellValueCore(RowProperties props, int recordIndex);
		protected abstract void SetCellValueCore(RowProperties props, int recordIndex, object value, bool endCurrentEdit);
		public abstract int Position { get; }
		protected internal abstract int RecordCount { get; }
		protected VGridDataManager Manager { get { return manager; } }
		public abstract bool NewItemRecordMode { get; }
		public abstract bool AllowAppend { get; }
		internal protected virtual void Invalidate() {}
		public abstract void FilterChanged(VGridControlBase grid);
	}
	public class BoundDataModeHelper : DataModeHelper {
		bool newItemRecordMode;
		public BoundDataModeHelper(VGridDataManager manager)
			: base(manager) {
			this.newItemRecordMode = false;
		}
		public override bool FilterByRows { get { return false; } }
		public override void AddNewRecord() {
			Manager.AddNewRow();
		}
		public override void DeleteRecord(int recordIndex) { Manager.DeleteRow(recordIndex); }
		protected DataColumnInfo GetDataColumn(RowProperties props) {
			return Manager.Columns[props.FieldName];
		}
		public override int GetRowHandleByFieldName(RowProperties props) {
			DataColumnInfo dc = GetDataColumn(props);
			if(dc != null) return dc.Index;
			return base.GetRowHandleByFieldName(props);
		}
		public override bool GetPredefinedReadOnlyByFieldName(RowProperties props) {
			if(props.UnboundType != UnboundColumnType.Bound)
				return props.ReadOnly.HasValue && props.ReadOnly.Value;
			DataColumnInfo dc = GetDataColumn(props);
			if(dc != null)
				return dc.ReadOnly || !Manager.AllowEdit;
			return true;
		}
		public override int GetCorrectRecordIndex(int recordIndex) {
			if(NewItemRecordMode && recordIndex == RecordCount - 1) recordIndex = NewItemRecord;
			return recordIndex;
		}
		public override string GetRowDataError(RowProperties props, int recordIndex) {
			return base.GetRowDataError(props, GetCorrectRecordIndex(recordIndex));
		}
		protected override object GetCellValueCore(RowProperties props, int recordIndex) {
			return Manager.GetRowValue(GetCorrectRecordIndex(recordIndex), props.RowHandle);
		}
		protected override void SetCellValueCore(RowProperties props, int recordIndex, object value, bool endCurrentEdit) {
			recordIndex = GetCorrectRecordIndex(recordIndex);
			Manager.SetRowValue(GetCorrectRecordIndex(recordIndex), props.RowHandle, value);
			if(!endCurrentEdit) {
				if(!(NewItemRecordMode && recordIndex == NewItemRecord)) Manager.EndCurrentRowEdit();
			}
			if(props.Row.IsConnected)
				props.Row.Grid.InvalidateRowCells(props.Row, recordIndex == NewItemRecord ? RecordCount - 1 : recordIndex);
		}
		protected internal override void StartNewItemRow() {
			this.newItemRecordMode = true;
		}
		protected internal override void EndNewItemRow() {
			this.newItemRecordMode = false;
		}
		public override int CheckValidRecord(int recordIndex) {
			int recordDisplayIndex = recordIndex;
			if(NewItemRecordMode && (recordDisplayIndex == RecordCount - 1 || recordDisplayIndex == NewItemRecord))
				return NewItemRecord;
			return base.CheckValidRecord(recordDisplayIndex);
		}
		public override void CancelCurrentRowEdit() {
			Manager.CancelCurrentRowEdit();
		}
		public override void BeginCurrentRowEdit() {
			Manager.BeginCurrentRowEdit();
		}
		public override bool NewItemRecordMode { get { return newItemRecordMode; } }
		public override int Position {
			get {
				if(NewItemRecordMode) return RecordCount - 1;
				return Manager.CurrentControllerRow;
			}
		}
		protected internal override int RecordCount { get { return Manager.VisibleCount + (NewItemRecordMode ? 1 : 0); } }
		public override bool AllowAppend { get { return (Manager.AllowNew && Manager.AllowEdit); } }
		public override void FilterChanged(VGridControlBase grid) {
			Manager.FilterCriteria = grid.FilterCriteria & grid.FindFilterCriteria;
		}
	}
	public class UnboundDataModeHelper : DataModeHelper {
		public UnboundDataModeHelper(VGridDataManager manager) : base(manager) { }
		public override bool IsBound { get { return false; } }
		public override bool FilterByRows { get { return true; } }
		public override void AddNewRecord() { }
		public override void DeleteRecord(int recordIndex) { }
		public override bool GetPredefinedReadOnlyByFieldName(RowProperties props) { return false; }
		protected override Type GetInvalidDataColumnType(RowProperties props) {
			if(props.Value != null) return props.Value.GetType();
			return base.GetInvalidDataColumnType(props);
		}
		protected override object GetCellValueCore(RowProperties props, int recordIndex) { return props.ValueCore; }
		protected override void SetCellValueCore(RowProperties props, int recordIndex, object value, bool endCurrentEdit) {
			if(endCurrentEdit && props.ValueCore != null) {
				try {
					value = Convert.ChangeType(value, props.RowType);
				} catch { }
			}
			props.ValueCore = value;
		}
		public override void CancelCurrentRowEdit() { }
		public override void BeginCurrentRowEdit() { }
		public override int Position { get { return 0; } }
		public override bool NewItemRecordMode { get { return false; } }
		public override bool AllowAppend { get { return false; } }
		protected internal override int RecordCount { get { return 1; } }
		public override void FilterChanged(VGridControlBase grid) {
			grid.ResetVisibleRows();
		}
	}
	public class PGridDataModeHelper : DataModeHelper {
		public const string RootPropertyName = PropertyHelper.RootPropertyName;
		public const string CaptionColumnName = "Caption";
		object dataSource;
		bool isMultiSource;
		PropertyGridControl grid;
		PGridDataModeHelperContextCache contextCache;
		GetDescriptorContextCommand getContextCommand;
		bool reset = false;
		string changedImmutableFieldName = null;
		public override bool FilterByRows { get { return true; } }
		protected PropertyGridControl Grid { get { return grid; } }
		protected IServiceProvider ServiceProvider { get { return Grid as IServiceProvider; } }
		internal string ChangedImmutableFieldName { get { return changedImmutableFieldName; } set { changedImmutableFieldName = value; } }
		internal protected PGridDataModeHelperContextCache ContextCache {
			get {
				if(contextCache == null)
					contextCache = new PGridDataModeHelperContextCache();
				return contextCache;
			}
		}
		internal bool Reset { get { return reset; } set { reset = value; } }
		protected GetDescriptorContextCommand GetContextCommand {
			get {
				if(getContextCommand == null)
					getContextCommand = new GetDescriptorContextCommand();
				return getContextCommand;
			}
		} 
		public PGridDataModeHelper(PGridDataManager manager, object[] dataSource, PropertyGridControl grid)
			: base(manager) {
			if(dataSource != null) {
				this.isMultiSource = dataSource.Length > 1;
				this.dataSource = this.isMultiSource ? dataSource : (dataSource.Length == 0 ? null : dataSource[0]);
			}
			this.grid = grid;
		}
		public override void AddNewRecord() { }
		public override void DeleteRecord(int recordIndex) { }
		public override bool GetPredefinedReadOnlyByFieldName(RowProperties props) {
			DescriptorContext context = GetDescriptorContext(props.FieldName);
			if(context == null)
				return true;
			return !context.IsValueEditable;
		}
		public bool HasRefreshPropertiesAttribute(RowProperties properties) {
			PropertyDescriptor pd = GetDescriptorContext(properties.FieldName).PropertyDescriptor;
			if(pd == null) return false;
			RefreshPropertiesAttribute r = (RefreshPropertiesAttribute)pd.Attributes[typeof(RefreshPropertiesAttribute)];
			if(r != null && r.Equals(RefreshPropertiesAttribute.All))
				return true;
			return false;
		}
		protected override Type GetInvalidDataColumnType(RowProperties props) {
			if(props.Value != null) return props.Value.GetType();
			return base.GetInvalidDataColumnType(props);
		}
		protected override object GetCellValueCore(RowProperties props, int recordIndex) {
			return GetDescriptorContext(props.FieldName).Value;
		}
		public virtual string GetTextData(RowProperties props, int recordIndex) {
			DescriptorContext context = GetDescriptorContext(props.FieldName);
			return PropertyHelper.ConvertToString(context, context.Value);
		}
		public void InvalidateCache() {
			ContextCache.Clear();
		}
		internal bool IsMultiSource {
			get { return isMultiSource; }
		}
		public override string GetRowDataError(RowProperties props, int recordIndex) {
			if(props == null) return string.Empty;
			return GetErrorInfo(props).ErrorText;
		}
		ErrorInfo GetErrorInfo(RowProperties rowProperties) {
			IDXDataErrorInfo dxDataErrorInfo = GetDescriptorContext(rowProperties.FieldName).Instance as IDXDataErrorInfo;
			if(dxDataErrorInfo == null) return new ErrorInfo();
			ErrorInfo errorInfo = new ErrorInfo();
			dxDataErrorInfo.GetPropertyError(rowProperties.FieldName, errorInfo);
			return errorInfo;
		}
		public override ErrorType GetRowDataErrorType(RowProperties props, int recordIndex) {
			if(props == null) return DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default;
			ErrorInfo errorInfo = GetErrorInfo(props);
			if(!string.IsNullOrEmpty(errorInfo.ErrorText)) return errorInfo.ErrorType;
			return ErrorType.None;
		}
		public override Type GetDataType(RowProperties props) {
			DescriptorContext context = GetDescriptorContext(props.FieldName);
			PropertyDescriptor pd = context.PropertyDescriptor;
			if(pd == null)
				return GetInvalidDataColumnType(props);
			return pd.PropertyType;
		}
		public virtual bool CanResetDefaultValue(RowProperties properties) {
			DescriptorContext context = GetDescriptorContext(properties.FieldName);
			if(context == null || context.PropertyDescriptor == null)
				return false;
			DescriptorContext actualContext = context.IsImmutable ? context.ParentContext : context;
			return actualContext.PropertyDescriptor.CanResetValue(actualContext.Instance);
		}
		protected override void SetCellValueCore(RowProperties props, int recordIndex, object value, bool endCurrentEdit) {
			DescriptorContext context = GetDescriptorContext(props.FieldName);
			if(context.PropertyDescriptor == null || context.Instance == null)
				return;
			try {
				object settingValue = PropertyHelper.TryConvertFromDifferentType(context, value);
				using(DesignerTransaction transaction = CreateTransaction()) {
					IComponentChangeService componentChangedService = PropertyHelper.GetComponentChangeService(Grid);
					if (componentChangedService != null) {
						componentChangedService.OnComponentChanging(context.Instance, context.PropertyDescriptor);
						NotifyParent(props, componentChangedService, (obj, propertyDescriptor) => componentChangedService.OnComponentChanging(obj, propertyDescriptor));
					}
					string parentFieldName = FieldNameHelper.GetParentFieldName(props.FieldName);
					if(!string.IsNullOrEmpty(parentFieldName)) {
						DescriptorContext parentContext = GetDescriptorContext(parentFieldName);
						if(context.IsImmutable) {
							ChangedImmutableFieldName = parentFieldName;
							if(Reset) {
								PropertyHelper.Reset(parentContext);
							} else {
								if(IsMultiSource) {
									CreateInstances(settingValue, context, parentContext);
								} else {
									PropertyHelper.CreateInstance(settingValue, context, parentContext, Grid.GetBrowsableAttributesArray());
								}
							}
						} else {
							if(Reset) {
								PropertyHelper.Reset(context);
							} else {
								context.SetValue(settingValue);
							}
							Type parentPropertyType = context.Instance.GetType();
							bool setParentProperty = parentPropertyType.IsValueType || parentPropertyType.IsArray;
							if(setParentProperty && !props.Row.ParentRow.Properties.GetReadOnly())
								SetParentProperty(context, parentContext);
						}
					} else {
						if(Reset) {
							PropertyHelper.Reset(context);
						} else {
							context.SetValue(settingValue);
						}
					}
					ContextCache.Clear();
					if(componentChangedService != null) {
						componentChangedService.OnComponentChanged(context.Instance, context.PropertyDescriptor, null, settingValue);
						NotifyParent(props, componentChangedService, (obj, propertyDescriptor) => componentChangedService.OnComponentChanged(obj, propertyDescriptor, null, null));
					}
					props.sink.RowPropertiesChanged(props, RowChangeTypeEnum.Value);
					CommitTransaction(transaction);
				}
			} catch(Exception e) {
				PropertyHelper.ThrowUnwindedException(e);
			}
		}
		void SetParentProperty(DescriptorContext context, DescriptorContext parentContext) {
			if(IsMultiSource) {
				if(parentContext.PropertyDescriptor != null) {
					MultiObjectPropertyDescriptor multiObjectPropertyDescriptor = (MultiObjectPropertyDescriptor)parentContext.PropertyDescriptor;
					multiObjectPropertyDescriptor.SetValues(parentContext.Instance, context.Instance);
				}
			} else {
				parentContext.SetValue(context.Instance);
			}
		}
		void NotifyParent(RowProperties properties, IComponentChangeService componentChangedService, Action<object, PropertyDescriptor> notifyAction) {
			string currentFieldName = properties.FieldName;
			while(true) {
				string parentFieldName = FieldNameHelper.GetParentFieldName(currentFieldName);
				ITypeDescriptorContext currentContext = GetDescriptorContext(currentFieldName);
				if(string.IsNullOrEmpty(parentFieldName) || !PropertyHelper.NotifyParentProperty(currentContext))
					return;
				ITypeDescriptorContext parentContext = GetDescriptorContext(parentFieldName);
				notifyAction(parentContext.Instance, parentContext.PropertyDescriptor);
				currentFieldName = parentFieldName;
			}
		}
		DesignerTransaction CreateTransaction() {
			IComponent component = Grid.SelectedObject as IComponent;
			if(component == null || component.Site == null || !component.Site.DesignMode)
				return null;
			IDesignerHost host = (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
			if(host == null)
				return null;
			return host.CreateTransaction();
		}
		void CommitTransaction(DesignerTransaction transaction) {
			if(transaction != null)
				transaction.Commit();
		}
		object[] CreateInstances(object settingValue, ITypeDescriptorContext multiContext, ITypeDescriptorContext parentMultiContext) {
			object[] instances = ((MultiObjectPropertyDescriptor)parentMultiContext.PropertyDescriptor).GetValues((object[])parentMultiContext.Instance);
			object[] newInstances = new object[instances.Length];
			for(int i = 0; i < instances.Length; i++) {
				DescriptorContext singleContext = new DescriptorContext(instances[i], multiContext.PropertyDescriptor, ServiceProvider);
				DescriptorContext parentSingleContext = new DescriptorContext(((object[])parentMultiContext.Instance)[i], ((MultiObjectPropertyDescriptor)parentMultiContext.PropertyDescriptor)[i], ServiceProvider);
				newInstances[i] = PropertyHelper.CreateInstance(settingValue, singleContext, parentSingleContext, Grid.GetBrowsableAttributesArray());
			}
			return newInstances;
		}
		public override bool IsCellDefaultValue(RowProperties props, int recordIndex) {
			if(props == null) return true;
			return !GetDescriptorContext(props.FieldName).ShouldSerializeValue;
		}
		public DescriptorContext GetDescriptorContext(string propertyName) {
			return GetDescriptorContextCore(propertyName, IsMultiSource);
		}
		public DescriptorContext GetSingleDescriptorContext(string propertyName) {
			return GetDescriptorContextCore(propertyName, false);
		}
		protected virtual DescriptorContext GetDescriptorContextCore(string propertyName, bool isMultiSource) {
			if(Grid == null)
				return new DescriptorContext(null, null, null);
			object dataSource = isMultiSource ? this.dataSource : Grid.SelectedObject;
			GetContextCommand.Initialize(isMultiSource, ContextCache, dataSource, ServiceProvider, Grid.GetBrowsableAttributesArray());
			DescriptorContext context = GetContextCommand.Execute(propertyName);
			GetContextCommand.Release();
			return context;
		}
		public override void CancelCurrentRowEdit() { }
		public override void BeginCurrentRowEdit() { }
		public override int Position { get { return 0; } }
		public override bool NewItemRecordMode { get { return false; } }
		public override bool AllowAppend { get { return false; } }
		protected internal override int RecordCount { get { return 1; } }
		protected internal override void  Invalidate() {
 			base.Invalidate();
			InvalidateCache();
		}
		public override void FilterChanged(VGridControlBase grid) {
			grid.ResetVisibleRows();
		}
	}
}
