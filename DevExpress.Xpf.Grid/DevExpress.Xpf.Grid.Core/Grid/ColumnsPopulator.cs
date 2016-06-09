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

using DevExpress.Core;
using DevExpress.Data.Helpers;
using DevExpress.Entity.Model;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
#if !SL
#else
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpf.Grid {
	public class ColumnGeneratorItemContext {
		public DataControlBase Control { get; private set; }
		public PropertyDescriptor PropertyDescriptor { get; private set; }
		internal ColumnGeneratorItemContext(DataControlBase control, PropertyDescriptor propertyDescriptor) {
			Control = control;
			PropertyDescriptor = propertyDescriptor;
		}
	}
}
namespace DevExpress.Xpf.Grid.Native {
	public abstract class ColumnSweeperBase {
		protected readonly IModelItemCollection columns;
		protected ColumnSweeperBase(IModelItemCollection columns) {
			this.columns = columns;
		}
		protected ColumnBase GetColumnObject(IModelItem column) {
			return (ColumnBase)column.GetCurrentValue();
		}
		protected abstract void OnClearColumns();
#if DEBUGTEST
		public static int ClearColumnsCallCount;
#endif
		public void ClearColumns() {
#if DEBUGTEST
			ClearColumnsCallCount++;
#endif
			OnClearColumns();
		}
	}
	public class DefaultColumnSweeper : ColumnSweeperBase {
		IModelItem columnsOwner;
		public DefaultColumnSweeper(IModelItemCollection columns, IModelItem columnsOwner)
			: base(columns) {
			this.columnsOwner = columnsOwner;
		}
		protected override void OnClearColumns() {
			foreach(IModelItemCollection columnsCollection in GetAllColumnsCollections(columnsOwner))
				columnsCollection.Clear();
		}
		List<IModelItemCollection> GetAllColumnsCollections(IModelItem root) {
			List<IModelItemCollection> localColumnsCollections = new List<IModelItemCollection>();
			foreach(IModelItem band in root.Properties["Bands"].Collection) {
				localColumnsCollections.AddRange(GetAllColumnsCollections(band));
			}
			localColumnsCollections.Add(root.Properties["Columns"].Collection);
			return localColumnsCollections;
		}
	}
	public class AddNewColumnSweeper : ColumnSweeperBase {
		public AddNewColumnSweeper(IModelItemCollection columns)
			: base(columns) {
		}
		protected override void OnClearColumns() {
			int i = 0;
			while(i < columns.Count()) {
				if(GetColumnObject(columns[i]).IsAutoGenerated)
					columns.RemoveAt(i);
				else
					i++;
			}
		}
	}
	public class ApplyColumnSettingsSweeper : ColumnSweeperBase {
		public ApplyColumnSettingsSweeper()
			: base(null) {
		}
		protected override void OnClearColumns() { }
	}
	public class AllColumnsInfo {
		readonly IModelItem dataControl;
		public AllColumnsInfo(IModelItem dataControl) {
			this.dataControl = dataControl;
		}
		Dictionary<string, IModelItem> allColumns = null;
		Dictionary<string, IModelItem> AllColumns {
			get {
				if(allColumns == null) {
					allColumns = new Dictionary<string, IModelItem>();
					FillAllColumns(dataControl);
					foreach(IModelItem column in SavedColumns)
						AddInAllColumnsHasheTable(column);
				}
				return allColumns;
			}
		}
		void FillAllColumns(IModelItem root) {
			if(root == null) return;
			foreach(IModelItem band in root.Properties["Bands"].Collection)
				FillAllColumns(band);
			foreach(IModelItem column in root.Properties["Columns"].Collection)
				AddInAllColumnsHasheTable(column);
		}
		public void SaveColumnsBeforeAddFirstBand() {
			foreach(IModelItem column in dataControl.Properties["Columns"].Collection)
				SavedColumns.Add(column);
		}
		public void AddInAllColumnsHasheTable(IModelItem column) {
			string key = ColumnModelItemCollectionWrapper.GetKey(column);
			if(key == null || AllColumns.ContainsKey(key))
				return;
			AllColumns.Add(key, column);
		}
		public void RemoveFromAllColumnsHasheTable(IModelItem column) {
			string key = ColumnModelItemCollectionWrapper.GetKey(column);
			if(key == null || !AllColumns.ContainsKey(key))
				return;
			AllColumns.Remove(key);
		}
		public bool IsSavedColumn(IModelItem column) {
			return SavedColumns.Contains(column);
		}
		public IModelItem FirstOrDefault(string fieldName) {
			IModelItem result = null;
			AllColumns.TryGetValue(fieldName, out result);
			return result;
		}
		List<IModelItem> SavedColumns = new List<IModelItem>();
		List<BandCandidate> bandCandidates = new List<BandCandidate>();
		public List<BandCandidate> BandCandidates { get { return bandCandidates; } }
	}
	public class BandCandidate {
		public BandCandidate(string fieldName) {
			FieldName = fieldName;
		}
		public string FieldName { get; private set; }
	}
	public class ColumnModelItemCollectionWrapper : ModelItemCollectionWrapper {
		readonly AllColumnsInfo columnsInfo;
		public ColumnModelItemCollectionWrapper(AllColumnsInfo columnsInfo, IModelItemCollection source)
			: base(source) {
			this.columnsInfo = columnsInfo;
		}
		public override void Add(IModelItem element) {
			base.Add(element);
			columnsInfo.AddInAllColumnsHasheTable(element);
		}
		public override void Remove(IModelItem element) {
			base.Remove(element);
			columnsInfo.RemoveFromAllColumnsHasheTable(element);
		}
		public IModelItem FindColumnInAllColumns(string fieldName) {
			return columnsInfo.FirstOrDefault(fieldName);
		}
		public bool IsSavedColumn(IModelItem column) {
			return columnsInfo.IsSavedColumn(column);
		}
		public List<BandCandidate> BandCandidates { get { return columnsInfo.BandCandidates; } }
	}
	public class ModelItemCollectionWrapper {
		readonly IModelItemCollection source;
		Dictionary<string, IModelItem> _modelItems = null;
		Dictionary<string, IModelItem> ModelItems {
			get {
				if(_modelItems == null)
					CreateAndFillHashTable();
				return _modelItems;
			}
		}
		void CreateAndFillHashTable() {
			List<IModelItem> sourceList = source.ToList();
			_modelItems = new Dictionary<string, IModelItem>(sourceList.Count);
			foreach(IModelItem modelItem in sourceList)
				AddToHashTable(modelItem);
		}
		void AddToHashTable(IModelItem column) {
			string key = GetKey(column);
			if(key == null || ModelItems.ContainsKey(key))
				return;
			ModelItems.Add(key, column);
		}
		void RemoveFromHashTable(IModelItem column) {
			string key = GetKey(column);
			if(key == null || !ModelItems.ContainsKey(key))
				return;
			ModelItems.Remove(key);
		}
		internal static string GetKey(IModelItem column) {
			return column.Properties["FieldName"].With(pr => pr.ComputedValue).With(c => c.ToString());
		}
		public ModelItemCollectionWrapper(IModelItemCollection source) {
			this.source = source;
		}
		public IModelItem FirstOrDefault(string fieldName) {
			IModelItem result = null;
			ModelItems.TryGetValue(fieldName, out result);
			return result;
		}
		public virtual void Remove(IModelItem element) {
			source.Remove(element);
			RemoveFromHashTable(element);
		}
		public virtual void Add(IModelItem element) {
			source.Add(element);
			AddToHashTable(element);
		}
	}
	public abstract class ColumnCreatorBase {
		protected readonly ColumnModelItemCollectionWrapper columnsWrapper;
		readonly IEditingContext context;
		public IEditingContext Context { get { return context; } }
		public ColumnCreatorBase(IEditingContext context, IModelItemCollection columns, AllColumnsInfo columnsInfo) {
			this.context = context;
			columnsWrapper = new ColumnModelItemCollectionWrapper(columnsInfo, columns);
		}
		protected virtual bool CanAddColumn(IModelItem column) {
			return true;
		}
		protected virtual void OnAddColumn(IModelItem column) {
			columnsWrapper.Add(column);
		}
		public void AddColumn(IModelItem column) {
			if(CanAddColumn(column))
				OnAddColumn(column);
		}
		public virtual IModelItem CreateColumn(IEdmPropertyInfo propertyInfo) {
			return context.CreateItem(ColumnType);
		}
		protected abstract Type ColumnType { get; }
	}
	public class SmartColumnsPopulator : RuntimeDefaultColumnsPopulator {
		public SmartColumnsPopulator(IModelItem dataControl, IModelItemCollection columns, AllColumnsInfo columnsInfo)
			: base(dataControl, columns, columnsInfo) {
		}
		public override IModelItem CreateColumn(IEdmPropertyInfo propertyInfo) {
			return GetSmartColumn(propertyInfo.Name).Do(c => MoveColumnToBand(c, propertyInfo));
		}
		protected override void OnAddColumn(IModelItem column) {
		}
	}
	public class ColumnAttributesPopulator : SmartColumnsPopulator {
		string fieldName;
		public ColumnAttributesPopulator(IModelItem dataControl, IModelItemCollection columns, IModelItem targetColumn, AllColumnsInfo columnsInfo)
			: base(dataControl, columns, columnsInfo) {
			this.fieldName = targetColumn.Properties["FieldName"].Value.GetCurrentValue().ToString();
		}
		public override IModelItem CreateColumn(IEdmPropertyInfo propertyInfo) {
			if(propertyInfo.Name != fieldName)
				return null;
			IModelItem column = columnsWrapper.FirstOrDefault(fieldName);
			if(column != null && column.Properties[ColumnBase.IsSmartProperty.GetName()].IsSet)
				column.Properties[ColumnBase.IsSmartProperty.GetName()].ClearValue();
			return column;
		}
	}
	public abstract class DefaultColumnsPopulator : ColumnCreatorBase {
		protected internal static ColumnBase GetColumnObject(IModelItem column) {
			return (ColumnBase)column.GetCurrentValue();
		}
		readonly IModelItem dataControl;
		public IModelItem DataControl { get { return dataControl; } }
		public DefaultColumnsPopulator(IModelItem dataControl, IModelItemCollection columns, AllColumnsInfo columnsInfo)
			: base(dataControl.Context, columns, columnsInfo) {
			this.dataControl = dataControl;
		}
		protected DataControlBase DataControlObject { get { return (DataControlBase)DataControl.GetCurrentValue(); } }
		protected override Type ColumnType { get { return DataControlObject.ColumnType; } }
		public override IModelItem CreateColumn(IEdmPropertyInfo propertyInfo) {
			IModelItem smartColumn = GetSmartColumn(propertyInfo.Name);
			if(smartColumn != null) {
				MoveColumnToBand(smartColumn, propertyInfo);
			}
			return smartColumn ?? CreateColumnFromColumnsGenerator(propertyInfo) ?? base.CreateColumn(propertyInfo);
		}
		protected void MoveColumnToBand(IModelItem column, IEdmPropertyInfo propertyInfo) {
			string bandName = String.IsNullOrWhiteSpace(propertyInfo.Attributes.GroupName) ? String.Empty : LayoutGroupInfo.GetGroupDescriptions(propertyInfo.Attributes.GroupName).Last();
			if(!columnsWrapper.IsSavedColumn(column)) {
				columnsWrapper.BandCandidates.Add(new BandCandidate((string)column.Properties["FieldName"].Value.GetCurrentValue()));
				return;
			}
			string columnName = (string)column.Properties["FieldName"].Value.GetCurrentValue();
			IModelItem targetBand = bandName == String.Empty ? FindFirstBottomBand(DataControl, DataControl.Properties["Bands"].Collection) : FindBandByName(DataControl.Properties["Bands"].Collection, bandName);
			if(targetBand == null)
				return;
			targetBand.Properties["Columns"].Collection.Add(column);
		}
		IModelItem FindBandByName(IModelItemCollection rootCollection, string name) {
			foreach(IModelItem band in rootCollection) {
				if((string)band.Properties["Header"].Value.GetCurrentValue() == name)
					return band;
				IModelItem targetBand = FindBandByName(band.Properties["Bands"].Collection, name);
				if(targetBand != null)
					return targetBand;
			}
			return null;
		}
		IModelItem FindFirstBottomBand(IModelItem root, IModelItemCollection rootCollection) {
			if(rootCollection.Count() == 0)
				return DataControl != root ? root : null;
			return rootCollection[0];
		}
		protected IModelItem GetSmartColumn(string fieldName) {
			return GetColumn(fieldName, true);
		}
		protected IModelItem GetColumn(string fieldName, bool isSmart = false) {
			IModelItem column = columnsWrapper.FirstOrDefault(fieldName);
			if(column != null) {
				if(IsValidColumn(column, fieldName, isSmart))
					return column;
				return null;
			}
			column = columnsWrapper.FindColumnInAllColumns(fieldName);
			if(column == null)
				return null;
			return IsValidColumn(column, fieldName, isSmart) ? column : null;
		}
		bool IsValidColumn(IModelItem column, string fieldName, bool isSmart) {
			ColumnBase columnObject = GetColumnObject(column);
			if(columnObject == null)
				return false;
			if(columnObject.FieldName != fieldName || columnObject.IsAutoGenerated)
				return false;
			if(isSmart)
				return columnObject.IsSmart;
			return true;
		}
		protected override void OnAddColumn(IModelItem column) {
			GetColumnObject(column).IsAutoGenerated = true;
			base.OnAddColumn(column);
		}
		protected sealed override bool CanAddColumn(IModelItem column) {
			if(GetColumn(column.Properties["FieldName"].Value.GetCurrentValue().ToString()) != null)
				return false;
			return DataControlObject.RaiseAutoGeneratingColumn((ColumnBase)column.GetCurrentValue());
		}
		protected abstract IModelItem CreateColumnFromColumnsGenerator(IEdmPropertyInfo propertyInfo);
	}
	public class RuntimeDefaultColumnsPopulator : DefaultColumnsPopulator {
		public RuntimeDefaultColumnsPopulator(IModelItem dataControl, IModelItemCollection columns, AllColumnsInfo columnsInfo)
			: base(dataControl, columns, columnsInfo) {
		}
		protected override IModelItem CreateColumnFromColumnsGenerator(IEdmPropertyInfo propertyInfo) {
			var columnFromGenerator = DataControlObject.CreateColumnFromColumnGenerator((PropertyDescriptor)propertyInfo.ContextObject);
			return columnFromGenerator != null ? new RuntimeModelItem((RuntimeEditingContext)DataControl.Context, columnFromGenerator, DataControl.Context.GetRoot()) : null;
		}
		public override IModelItem CreateColumn(IEdmPropertyInfo propertyInfo) {
			IModelItem column = base.CreateColumn(propertyInfo);
			((ColumnBase)column.GetCurrentValue()).FieldType = propertyInfo.PropertyType;
			return column;
		}
	}
	public class AddNewColumnsPopulator : RuntimeDefaultColumnsPopulator {
		public AddNewColumnsPopulator(IModelItem dataControl, IModelItemCollection columns, AllColumnsInfo columnsInfo) : base(dataControl, columns, columnsInfo) { }
		protected override void OnAddColumn(IModelItem column) {
			if(!ContainsColumn(column))
				base.OnAddColumn(column);
		}
		protected virtual bool ContainsColumn(IModelItem column) {
			string fieldName = GetColumnObject(column).With(c => c.FieldName);
			if(fieldName == null)
				return false;
			return columnsWrapper.FirstOrDefault(fieldName) != null;
		}
	}
	public class BandsGenerator : IGroupGenerator {
		readonly IModelItem bandsContainer;
		readonly IModelItem dataControl;
		readonly Type bandType;
		readonly Func<IModelItem, IModelItemCollection, AllColumnsInfo, EditorsGeneratorBase> createGeneratorCallback;
		readonly EditorsGeneratorBase editorsGenerator;
		readonly bool isRuntime;
		readonly bool isRootGenerator;
		readonly AllColumnsInfo columnsInfo;
		readonly bool allowCreateSubBands;
		public BandsGenerator(IModelItem dataControl, IModelItem bandsContainer, Type bandType, Func<IModelItem, IModelItemCollection, AllColumnsInfo, EditorsGeneratorBase> createGeneratorCallback, bool isRuntime, bool isRootGenerator, AllColumnsInfo columnsInfo, bool allowCreateSubBands = true) {
			this.dataControl = dataControl;
			this.bandsContainer = bandsContainer;
			this.bandType = bandType;
			this.createGeneratorCallback = createGeneratorCallback;
			this.isRootGenerator = isRootGenerator;
			this.columnsInfo = columnsInfo ?? new AllColumnsInfo(dataControl);
			this.editorsGenerator = createGeneratorCallback(dataControl, bandsContainer.Properties["Columns"].Collection, this.columnsInfo);
			this.isRuntime = isRuntime;
			this.allowCreateSubBands = allowCreateSubBands;
		}
		void IGroupGenerator.ApplyGroupInfo(string name, GroupView view, Orientation orientation) {
			if(name == null || dataControl == bandsContainer)
				return;
			if(!isRuntime || (bool)bandsContainer.Properties[BaseColumn.IsAutoGeneratedProperty.GetName()].Value.GetCurrentValue())
				bandsContainer.Properties.FindProperty(BaseColumn.HeaderProperty.GetName(), bandsContainer.ItemType).Do(x => x.SetValue(name));
		}
		IGroupGenerator IGroupGenerator.CreateNestedGroup(string name, GroupView view, Orientation orientation) {
			if(!allowCreateSubBands)
				return this;
			IModelItem band = GetBandFromBandOwner(name);
			return new BandsGenerator(dataControl, band, bandType, createGeneratorCallback, isRuntime, false, columnsInfo, allowCreateSubBands);
		}
		IModelItem GetBandFromBandOwner(string name) {
			IModelItem band = GetBandFromBandOwnerCore(name);
			if(band == null) {
				band = bandsContainer.Context.CreateItem(bandType);
				if(dataControl.Properties["Bands"].Collection.Count() == 0) {
					columnsInfo.SaveColumnsBeforeAddFirstBand();
					AddBandCandidatesToNewBand(band);
				}
				SetIsAutoGeneratedProperty(band);
				GetBandsCollection().Add(band);
			}
			return band;
		}
		void AddBandCandidatesToNewBand(IModelItem band) {
			List<IModelItem> columns = dataControl.Properties["Columns"].Collection.ToList();
			foreach(BandCandidate bandCandidate in columnsInfo.BandCandidates) {
				IModelItem column = columns.FirstOrDefault(c => (string)c.Properties["FieldName"].Value.GetCurrentValue() == bandCandidate.FieldName);
				if(column == null)
					continue;
				band.Properties["Columns"].Collection.Add(column);
			}
			columnsInfo.BandCandidates.Clear();
		}
		IModelItem GetBandFromBandOwnerCore(string name) {
			foreach(IModelItem band in GetBandsCollection()) {
				object header = GetBandHeader(band);
				if(header.ToString() == name)
					return band;
			}
			return null;
		}
		object GetBandHeader(IModelItem band) {
			return band.Properties["Header"].ComputedValue ?? String.Empty;
		}
		IModelItemCollection GetBandsCollection() {
			if(isRootGenerator)
				return dataControl.Properties["Bands"].Collection;
			return bandsContainer.Properties["Bands"].Collection;
		}
		BandBase GetBandBaseObject(IModelItem column) {
			return (BandBase)column.GetCurrentValue();
		}
		void SetIsAutoGeneratedProperty(IModelItem band) {
			if(isRuntime) {
				BandBase bandObject = GetBandBaseObject(band);
				bandObject.IsAutoGenerated = true;
			}
		}
		EditorsGeneratorBase IGroupGenerator.EditorsGenerator {
			get { return editorsGenerator; }
		}
		void IGroupGenerator.OnAfterGenerateContent() { }
	}
	public abstract class ModelGridColumnsGeneratorBase : EditorsGeneratorBase {
		protected readonly ColumnCreatorBase creator;
		protected IEditingContext Context { get { return creator.Context; } }
		protected readonly bool SkipXamlGenerationProperties;
		protected readonly bool ApplyOnlyForSmartColumns;
		public ModelGridColumnsGeneratorBase(ColumnCreatorBase creator, bool applyOnlyForSmartColumns, bool skipXamlGenerationProperties) {
			this.creator = creator;
			ApplyOnlyForSmartColumns = applyOnlyForSmartColumns;
			SkipXamlGenerationProperties = skipXamlGenerationProperties;
		}
		public override EditorsGeneratorTarget Target { get { return EditorsGeneratorTarget.GridControl; } }
		protected override EditorsGeneratorMode Mode { get { return EditorsGeneratorMode.EditSettings; } }
		protected sealed override void GenerateEditor(IEdmPropertyInfo property, Type editType, Initializer initializer) {
			GenerateEditor(property, null, editType != null ? Context.CreateItem(editType) : null, initializer, null, true);
		}
		protected virtual void GenerateEditor(IEdmPropertyInfo property, IModelItem column, IModelItem editSettings, Initializer initializer, string displayMember, bool setFieldName) {
			column = column ?? creator.CreateColumn(property);
			if(column == null) return;
			Action<DependencyProperty, object, bool?> setColumnProperty = (dp, v, applyOnlyForSmartColumn) => {
				bool _applyOnlyForSmartColumn = applyOnlyForSmartColumn ?? ApplyOnlyForSmartColumns;
				SetColumnPropertyValue(column, dp, v, SkipXamlGenerationProperties, _applyOnlyForSmartColumn);
			};
			AttributesApplier.ApplyBaseAttributes(propertyInfo: property,
					setDisplayMember: x => {
						string path = x + (string.IsNullOrEmpty(displayMember) ? null : "." + displayMember);
						if(setFieldName) column.SetValueIfNotSet(ColumnBase.FieldNameProperty, path);
						if(path != x) {
							string header = SplitStringHelper.SplitPascalCaseString(x);
							setColumnProperty(ColumnBase.HeaderProperty, header, null);
						}
					},
					setDescription: x => setColumnProperty(ColumnBase.HeaderToolTipProperty, x, false),
					setDisplayName: null,
					setDisplayShortName: x => setColumnProperty(ColumnBase.HeaderProperty, x, false),
					setEditable: x => {
						if(x.HasValue && !x.Value)
							setColumnProperty(ColumnBase.AllowEditingProperty, x.ToDefaultBoolean(), false);
					},
					setHidden: () => setColumnProperty(ColumnBase.VisibleProperty, false, false),
					setInvisible: () => setColumnProperty(ColumnBase.VisibleProperty, false, false),
					setReadOnly: () => setColumnProperty(ColumnBase.ReadOnlyProperty, true, false),
					setRequired: null);
			AttributesApplier.ApplyDisplayFormatAttributesForEditSettings(property, () => editSettings ?? (editSettings = column.Context.CreateItem(typeof(TextEditSettings))),
				(x, dp, v) => SetSettingsPropertyValue(column, x, dp, v, SkipXamlGenerationProperties, ApplyOnlyForSmartColumns));
			initializer.SetContainerProperties(column);
			if(editSettings != null) {
				initializer.SetEditProperties(column, editSettings);
				SetSettings(column, editSettings);
			}
			creator.AddColumn(column);
		}
		protected void SetSettings(IModelItem column, IModelItem editSettings) {
			if(!CanSetProperties(column, ApplyOnlyForSmartColumns)) return;
			if(IsEditSettingsSet(column)) {
				IModelItem localEditSettings = column.Properties["EditSettings"].Value as IModelItem;
				if(localEditSettings.ItemType != editSettings.ItemType)
					return;
				foreach(IModelProperty property in editSettings.Properties.Where(p => p.IsSet)) {
					IModelProperty localProperty = localEditSettings.Properties.FirstOrDefault(p => p.Name == property.Name);
					if(localProperty == null)
						continue;
					localProperty.SetValueIfNotSet(property.Value.GetCurrentValue());
				}
				return;
			}
			if(!SkipXamlGenerationProperties)
				column.SetValue(ColumnBase.EditSettingsProperty, editSettings);
			else
				TypeDescriptor.GetProperties(column.GetCurrentValue())[ColumnBase.EditSettingsProperty.GetName()].SetValue(column.GetCurrentValue(), editSettings.GetCurrentValue());
		}
		protected virtual bool IsEditSettingsSet(IModelItem column) {
			if(column.Properties["EditSettings"].IsSet) return true;
			DependencyObject columnObject = column.GetCurrentValue() as DependencyObject;
			if(column == null) return false;
			return DependencyObjectExtensions.IsPropertySet(columnObject, ColumnBase.EditSettingsProperty);
		}
		protected bool CanSetProperties(IModelItem column, bool applyOnlyForSmartColumn) {
			if(applyOnlyForSmartColumn) {
				bool isSmartColumn = (bool)column.GetProperty(ColumnBase.IsSmartProperty).Value.GetCurrentValue();
				if(!isSmartColumn) return false;
			}
			return true;
		}
		protected void SetColumnPropertyValue(IModelItem column, DependencyProperty property, object value, bool? skipXamlGenerationProperties = null, bool? applyOnlyForSmartColumn = null) {
			skipXamlGenerationProperties = skipXamlGenerationProperties ?? SkipXamlGenerationProperties;
			applyOnlyForSmartColumn = applyOnlyForSmartColumn ?? ApplyOnlyForSmartColumns;
			if(property.Name == "FieldName") {
				column.SetValueIfNotSet(property, value);
				return;
			}
			if(!CanSetProperties(column, applyOnlyForSmartColumn.Value)) return;
			if(!skipXamlGenerationProperties.Value) column.SetValueIfNotSet(property, value);
			else {
				var columnObject = column.GetCurrentValue();
				TypeDescriptor.GetProperties(columnObject)[property.Name].SetValue(columnObject, value);
			}
		}
		protected void SetSettingsPropertyValue(IModelItem column, IModelItem settings, DependencyProperty property, object value, bool? skipXamlGenerationProperties = null, bool? applyOnlyForSmartColumn = null) {
			skipXamlGenerationProperties = skipXamlGenerationProperties ?? SkipXamlGenerationProperties;
			applyOnlyForSmartColumn = applyOnlyForSmartColumn ?? ApplyOnlyForSmartColumns;
			if(!CanSetProperties(column, applyOnlyForSmartColumn.Value)) return;
			if(!skipXamlGenerationProperties.Value) settings.SetValueIfNotSet(property, value);
			else {
				object settingsObject = settings.GetCurrentValue();
				TypeDescriptor.GetProperties(settingsObject)[property.Name].SetValue(settingsObject, value);
			}
		}
	}
}
