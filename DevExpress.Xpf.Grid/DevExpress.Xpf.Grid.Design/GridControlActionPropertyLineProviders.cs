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

extern alias Platform;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Design;
using System.Windows;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard;
using DevExpress.Design.SmartTags;
#if SILVERLIGHT
using FrameworkElement = Platform::System.Windows.FrameworkElement;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Xpf.Editors;
using Platform::DevExpress.Xpf.Grid.LookUp;
using Platform::DevExpress.Xpf.Editors.Helpers;
using Platform::DevExpress.Xpf.Grid.Native;
using Platform::DevExpress.Xpf.Grid;
#else
using DevExpress.Xpf.Grid.Native;
#endif
namespace DevExpress.Xpf.Grid.Design {
	class GenerateColumnsActionProvider : DataControlActionProviderBase {
		SelectedCommandType generationType;
		public GenerateColumnsActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel, SelectedCommandType generationType)
			: base(ownerViewModel) {
			this.generationType = generationType;
		}
		protected override string GetCommandText() {
			return "Generate Columns";
		}
		protected override void OnCommandExecute(object param) {
			if(DataControl == null || ModelItem == null)
				return;
			if(!GridDesignTimeHelper.IsFieldListAvailable(DataControl)) {
				MessageBox.Show(GridDesignTimeHelper.GetNoDesignTimeDataSourceMassage(SR.CantPopulateColumnsMessage, DataControl.GetType()), SR.CantPopulateColumnsCaption);
				return;
			}
			if(IsSetBands() || GetGridColumns().Count > 0) {
				if(MessageBox.Show(SR.ShouldClearExistingColumnsMessage, SR.ShouldClearExistingColumnsCaption, MessageBoxButton.YesNo) == MessageBoxResult.No) {
					return;
				}
				NeedRefreshTag = true;
			}
			PerformEditAction(SR.PopulateColumnsDescription, (scope) => {
				IDesignTimeAdornerBase provider = GridControlHelper.GetDesignTimeAdorner(GridControlHelper.GetView(DataControl));
				provider.InvalidateDataSource();
				if(IsSetBands()) {
					RemoveAllBands();
					scope.Update();
				}
				DataControlBase updatedDataControl = ModelItem.GetCurrentValue() as DataControlBase;
				provider = GridControlHelper.GetDesignTimeAdorner(GridControlHelper.GetView(updatedDataControl));
				provider.SkipColumnXamlGenerationProperties = generationType == SelectedCommandType.Smart;
				GridControlHelper.InvalidateDesignTimeDataSource(DataControl);
				IModelItem dataControlModelItem = Context.ModelItem;
				GridControlHelper.PopulateColumns(dataControlModelItem, (dc, columns, canGenerateNewColumns, columnsInfo) => new DataControlAdornerProvider.DesignTimeColumnsPopulator(dc, columns, generationType, columnsInfo));
				scope.Update();
				dataControlModelItem = Context.ModelItem;
				if(dataControlModelItem.Properties["Bands"].Collection.Count() > 0) {
					IModelItem firstBand = dataControlModelItem.Properties["Bands"].Collection.First();
					while(dataControlModelItem.Properties["Columns"].Collection.Count() > 0) {
						IModelItem column = dataControlModelItem.Properties["Columns"].Collection.First();
						dataControlModelItem.Properties["Columns"].Collection.Remove(column);
						firstBand.Properties["Columns"].Collection.Add(column);
					}
				}
			});
			RefreshTag();
		}
	}
	class ExpandColumnActionProvider : DataControlActionProviderBase {
		public ExpandColumnActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) {
		}
		protected override string GetCommandText() {
			return "Load Column Properties From Data";
		}
		protected override void OnCommandExecute(object param) {
			GridControlHelper.GetDesignTimeAdorner(GridControlHelper.GetView(DataControlModel.GetCurrentValue() as DataControlBase)).SkipColumnXamlGenerationProperties = false;
			PerformEditAction(SR.PopulateColumnsDescription, (scope) => {
				GridControlHelper.ApplyColumnAttributes(XpfModelItem.FromModelItem(DataControlModel), XpfModelItem.FromModelItem(ModelItem));
			});
		}
	}
	abstract class AddColumnActionProviderBase : DataControlActionProviderBase {
		string fieldName;
		public AddColumnActionProviderBase(FrameworkElementSmartTagPropertiesViewModel ownerViewModel, string fieldName)
			: base(ownerViewModel) {
			this.fieldName = fieldName;
		}
		protected override string GetCommandText() {
			return "Add Column";
		}
		protected abstract ModelItemCollection GetColumnRoot();
		protected void CreateColumnItemAndAddToRoot() {
			int newIndex = GridDesignTimeHelper.FindDefaultNameIndex(DataControlModel, "FieldName", GetDefaultColumnName(), GridDesignTimeHelper.GetAllColumnsFromBandOwnerSkipClone);
			ModelItem columnItem = GridDesignTimeHelper.CreateModelItem(ModelItem.Root.Context, ColumnType);
			columnItem.Properties["FieldName"].SetValue(String.IsNullOrEmpty(fieldName) ? GetDefaultColumnName() + newIndex.ToString() : fieldName);
			if(!String.IsNullOrEmpty(fieldName))
				columnItem.Properties["IsSmart"].SetValue(true);
			GetColumnRoot().Add(columnItem);
		}
		protected sealed override void OnCommandExecute(object param) {
			if(IsContainsColumnWithSameName()) {
				MessageBox.Show(String.Format("The \"{0}\" column already created. Please remove it before adding new column with this FieldName", GetDefaultColumnName()), "Error", MessageBoxButton.OK);
				return;
			}
			OnCommandExecuteCore(param);
		}
		protected abstract void OnCommandExecuteCore(object param);
		bool IsContainsColumnWithSameName() {
			if(!String.IsNullOrEmpty(fieldName)) {
				ModelItem duplicatedColumn = GetColumnRoot().FirstOrDefault(item => item.Properties["FieldName"].Value.GetCurrentValue().ToString() == fieldName);
				return duplicatedColumn != null;
			}
			return false;
		}
		string GetDefaultColumnName() {
			return "Column";
		}
	}
	class AddColumnInBandActionProvider : AddColumnActionProviderBase {
		public AddColumnInBandActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel, string fieldName)
			: base(ownerViewModel, fieldName) { }
		protected override ModelItemCollection GetColumnRoot() {
			ModelItem lastChildBand = ModelItem;
			IBandsOwner bandsOwner = lastChildBand.GetCurrentValue() as IBandsOwner;
			while(bandsOwner.VisibleBands.Count > 0) {
				lastChildBand = GetModelFromVisibleBand(lastChildBand.Properties["Bands"].Collection, bandsOwner.VisibleBands[0]);
				bandsOwner = bandsOwner.VisibleBands[0];
			}
			return lastChildBand.Properties["Columns"].Collection;
		}
		ModelItem GetModelFromVisibleBand(ModelItemCollection bands, IBandsOwner band) {
			foreach(ModelItem b in bands) {
				if(b.GetCurrentValue() == band)
					return b;
			}
			return null;
		}
		protected override void OnCommandExecuteCore(object param) {
			PerformEditAction(SR.AddColumnDescription, delegate {
				CreateColumnItemAndAddToRoot();
			});
		}
	}
	class AddColumnActionProvider : AddColumnActionProviderBase {
		public AddColumnActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel, string fieldName)
			: base(ownerViewModel, fieldName) { }
		protected override void OnCommandExecuteCore(object param) {
			var cache = GridDesignTimeHelper.GetGroupSortColumns(DataControl);
			PerformEditAction(SR.AddColumnDescription, delegate {
				if(!AllColumnsSetInXaml()) {
					ModelItem.Properties[DesignHelper.GetPropertyName(DataControlBase.AutoGenerateColumnsProperty)].ClearValue();
					Columns.Clear();
				}
				CreateColumnItemAndAddToRoot();
			});
			GridControlHelper.FillBandsColumns(DataControl);
			GridDesignTimeHelper.ApplyGroupSortColumns(DataControl, cache);
		}
		protected override ModelItemCollection GetColumnRoot() {
			if(IsSetBands())
				return GridDesignTimeHelper.GetBottomLeftBandColumnsCollection(DataControlModel, GridDesignTimeHelper.GetAllBandsModels(DataControlModel));
			return GetGridColumns();
		}
		bool AllColumnsSetInXaml() {
			foreach(ColumnBase column in Columns) {
				if(GetColumnModelItem(column) == null)
					return false;
			}
			return true;
		}
		ModelItem GetColumnModelItem(ColumnBase column) {
			int index = Columns.IndexOf(column);
			ModelItemCollection columnItems = GetGridColumns();
			return 0 <= index && index < columnItems.Count ? columnItems[index] : null;
		}
	}
	abstract class AddBandActionProviderBase : DataControlActionProviderBase {
		public AddBandActionProviderBase(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Add Band";
		}
		protected string GetDefaultBandName() {
			return "Band";
		}
	}
	class AddBandToBandActionProvider : AddBandActionProviderBase {
		public AddBandToBandActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override void OnCommandExecute(object param) {
			int newIndex = GridDesignTimeHelper.FindDefaultNameIndex(DataControlModel, "Header", GetDefaultBandName(), GridDesignTimeHelper.GetAllBandsModels);
			GridControlHelper.DoBandMoveAction(DataControlModel.GetCurrentValue() as DataControlBase, () => {
				PerformEditAction(SR.AddBandDescription, (scope) => {
					ModelItem item = GridDesignTimeHelper.CreateModelItem(ModelItem.Context, ModelItem.ItemType);
					item.Properties["Header"].SetValue(GetDefaultBandName() + newIndex.ToString());
					AddColumnsToBandModel(item);
					ModelItem.Properties["Bands"].Collection.Add(item);
				});
			});
		}
		void AddColumnsToBandModel(ModelItem item) {
			if(ModelItem.Properties["Bands"].Collection.Count != 0 || ModelItem.Properties["Columns"].Collection.Count == 0)
				return;
			foreach(var c in ModelItem.Properties["Columns"].Collection)
				item.Properties["Columns"].Collection.Add(CloneModelItemHelper.CloneItem(ModelItem.Root.Context, c));
			ModelItem.Properties["Columns"].Collection.Clear();
		}
	}
	class AddBandActionProvider : AddBandActionProviderBase {
		public AddBandActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) {
#if !SL
			CanExecuteAction = new Func<object, bool>((o) =>
				ModelItem.View != null && (!ModelItem.Properties["View"].IsSet || ModelItem.Properties["View"].Value.ItemType != typeof(CardView))
			);
#endif
		}
		protected override void OnCommandExecute(object param) {
			if(!IsSetBands()) {
				PerformEditAction(SR.DeleteColumnsDescription, (scope) => {
					ModelItem band = GetNewBandModelItem();
					DataControlModel.Properties["Bands"].Collection.Add(band);
					scope.Update();
					MoveExistingColumnsToBand(DataControlModel.Properties["Bands"].Collection[0]);
				});
			}
			else {
				PerformEditAction(SR.AddBandDescription, (scope) => DataControlModel.Properties["Bands"].Collection.Add(GetNewBandModelItem()));
			}
		}
		protected void MoveExistingColumnsToBand(ModelItem band) {
			if(DataControlModel.Properties["Columns"].Collection.Count == 0)
				return;
			List<ModelItem> columns = new List<ModelItem>();
			foreach(var c in DataControlModel.Properties["Columns"].Collection)
				columns.Add(c);
			DataControlModel.Properties["Columns"].Collection.Clear();
			foreach(var c in columns)
				band.Properties["Columns"].Collection.Add(c);
		}
		Type BandType {
			get {
				if(DataControl is TreeListControl)
					return typeof(TreeListControlBand);
				return typeof(GridControlBand);
			}
		}
		ModelItem GetNewBandModelItem() {
			int newIndex = GridDesignTimeHelper.FindDefaultNameIndex(DataControlModel, "Header", GetDefaultBandName(), GridDesignTimeHelper.GetAllBandsModels);
			ModelItem item = GridDesignTimeHelper.CreateModelItem(ModelItem.Context, BandType);
			item.Properties["Header"].SetValue(GetDefaultBandName() + newIndex);
			return item;
		}
	}
	class CreateDataSourceActionProvider : DataControlActionProviderBase {
		public CreateDataSourceActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel) : base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Create Data Source...";
		}
		protected override void OnCommandExecute(object param) {
			ItemsSourceWizardWindow wizard = new ItemsSourceWizardWindow();
			GridDataAccessTechnologyCollectionProvider provider = new GridDataAccessTechnologyCollectionProvider(GridControlHelper.GetView(DataControl));
			wizard.DataContext = new ViewModelGenerator(provider).CreateViewModel(HelpMeChooseLinkHelper.Link);
			wizard.Closed += new System.EventHandler(wizard_Closed);
			wizard.ShowDialog();
		}
		void wizard_Closed(object sender, EventArgs e) {
			ItemsSourceWizardViewModel viewModel = (ItemsSourceWizardViewModel)((ItemsSourceWizardWindow)sender).DataContext;
			if(!viewModel.IsApply)
				return;
			DataSourceGeneratorContainer generatorContainer = (DataSourceGeneratorContainer)viewModel.DataSourceConfigurationViewModel.ConfiguratedDataSourceType;
			generatorContainer.Generator.Generate(generatorContainer.Properties.Select(d => d.DataContext), ModelItem);
		}
	}
	abstract class DataControlActionProviderBase : CommandActionLineProvider {
		protected DataControlBase DataControl { get; private set; }
		protected ModelItem ModelItem { get; private set; }
		protected bool NeedRefreshTag { get; set; }
		protected virtual Type ColumnType {
			get {
				if(DataControl is TreeListControl || ModelItem.ItemType == typeof(TreeListControlBand))
					return typeof(TreeListColumn);
				return typeof(GridColumn);
			}
		}
		protected void RefreshTag() {
			if(!NeedRefreshTag || ModelItem == null)
				return;
			SelectionHelper.SetSelection(ModelItem.Root.Context, ModelItem.Root);
			SelectionHelper.SetSelection(ModelItem.Root.Context, ModelItem);
		}
		protected bool IsSetBands() {
			return GetBandsCount() > 0;
		}
		protected ModelItem DataControlModel {
			get { return ModelItem.GetCurrentValue() is DataControlBase ? ModelItem : BarManagerDesignTimeHelper.FindParentByType<DataControlBase>(ModelItem); }
		}
		protected void RemoveAllBands() {
			DataControlModel.Properties["Bands"].Collection.Clear();
		}
		protected int GetBandsCount() {
			return DataControlModel.Properties["Bands"].Collection.Count;
		}
		public DataControlActionProviderBase(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) {
			SetProperties();
		}
		void SetProperties() {
			XpfModelItem item = Context.ModelItem as XpfModelItem;
			ModelItem = item.Value;
			if(item == null || item.Value.View == null)
				return;
			DataControl = item.Value.View.PlatformObject as DataControlBase;
		}
		protected void PerformEditAction(string description, Action<IModelEditingScope> editAction) {
			using(IModelEditingScope batchedChangeRoot = Context.ModelItem.BeginEdit(description)) {
				editAction(batchedChangeRoot);
				batchedChangeRoot.Complete();
			}
		}
		protected IColumnCollection Columns { get { return GridControlHelper.GetColumns(DataControl); } }
		protected ModelItemCollection GetGridColumns() {
			return GridDesignTimeHelper.GetGridColumnsCollection(ModelItem);
		}
	}
	class ConditionalFormattingManagerActionProvider : DataControlActionProviderBase {
		ModelItem columnModel;
		public ConditionalFormattingManagerActionProvider(FrameworkElementSmartTagPropertiesViewModel viewModel)
			: base(viewModel) {
			XpfModelItem columnXpfModelItem = viewModel.RuntimeSelectedItem as XpfModelItem;
			if(columnXpfModelItem != null)
				columnModel = columnXpfModelItem.Value;
		}
		protected override string GetCommandText() {
			return "Manage Conditional Formatting Rules";
		}
		protected override void OnCommandExecute(object param) {
			if(columnModel == null)
				return;
			ConditionalFormattingDialogServant servant = new ConditionalFormattingDialogServant();
			servant.PrepareForConditionCommand(columnModel);
			var commands = servant.GetCommands(columnModel);
			if(commands == null)
				return;
			ColumnBase column = columnModel.GetCurrentValue() as ColumnBase;
			if(column == null)
				return;
			commands.ShowConditionalFormattingManager.Execute(column);
		}
	}
}
