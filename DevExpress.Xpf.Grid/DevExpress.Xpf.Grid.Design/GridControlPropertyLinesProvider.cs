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
using DependencyPropertyHelper = DevExpress.Xpf.Core.Design.DependencyPropertyHelper;
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Data;
using DevExpress.Xpf.Editors.ExpressionEditor;
using DevExpress.Xpf.Editors.ExpressionEditor.Native;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Core;
using System.Windows.Input;
using Microsoft.Windows.Design.Model;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Grid.Design {
	abstract class DataControlPropertyLinesProviderBase : PropertyLinesProviderBase {
		public DataControlPropertyLinesProviderBase(Type controlType) : base(controlType) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ActionListPropertyLineViewModel(new ColumnGenerationActionListPropertyLineContext(viewModel)));
			lines.Add(() => new ActionListPropertyLineViewModel(new AddColumnActionListPropertyLineContext(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new AddBandActionProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new CreateItemsSourceActionProvider(viewModel)));
			lines.Add(() => new SeparatorLineViewModel(viewModel) { IsLineVisible = false });
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DataControlBase.ItemsSourceProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DataControlBase.SelectedItemProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DataControlBase.SelectionModeProperty), typeof(MultiSelectMode)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DataControlBase.AutoGenerateColumnsProperty), typeof(AutoGenerateColumnsMode)));
			return lines;
		}
	}
	class AddColumnActionListPropertyLineContext : AddColumnActionListPropertyLineContextBase {
		protected override DataControlBase DataControl {
			get { return ModelItem.GetCurrentValue() as DataControlBase; }
		}
		public AddColumnActionListPropertyLineContext(IPropertyLineContext context)
			: base(context) { }
		protected override void OnSelectedItemExecute(MenuItemInfo param) {
			new AddColumnActionProvider(Context as FrameworkElementSmartTagPropertiesViewModel, param != null ? param.Tag.ToString() : String.Empty).CommandAction(null);
		}
	}
	class AddColumnInBandActionListPropertyLineContext : AddColumnActionListPropertyLineContextBase {
		protected override DataControlBase DataControl {
			get { return GetDataControl(); }
		}
		DataControlBase GetDataControl() {
			return BarManagerDesignTimeHelper.FindParentByType(typeof(DataControlBase), ((XpfModelItem)ModelItem).Value).GetCurrentValue() as DataControlBase;
		}
		public AddColumnInBandActionListPropertyLineContext(IPropertyLineContext context)
			: base(context) { }
		protected override void OnSelectedItemExecute(MenuItemInfo param) {
			new AddColumnInBandActionProvider(Context as FrameworkElementSmartTagPropertiesViewModel, param != null ? param.Tag.ToString() : String.Empty).CommandAction(null);
		}
	}
	abstract class AddColumnActionListPropertyLineContextBase : ActionListPropertyLineContext {
		protected IModelItem ModelItem { get { return Context.ModelItem; } }
		protected abstract DataControlBase DataControl { get; }
		public AddColumnActionListPropertyLineContextBase(IPropertyLineContext context)
			: base(context) { }
		protected override void UpdateItems(IEnumerable<MenuItemInfo> items) {
			InitializeItems(items);
		}
		void InitializeItems(IEnumerable<MenuItemInfo> items) {
			if(DataControl == null)
				return;
			GridControlHelper.InvalidateDesignTimeDataSource(DataControl);
			ObservableCollection<MenuItemInfo> Items = items as ObservableCollection<MenuItemInfo>;
			Items.Clear();
			Items.Add(new MenuItemInfo() { Command = SelectedItemCommand, Caption = SR.AddEmptyColumnDescription, Tag = String.Empty });
			foreach(string columnName in GridControlHelper.GetAllColumnNames(DataControl)) {
				Items.Add(new MenuItemInfo() { Command = SelectedItemCommand, Caption = string.Format("Add \"{0}\" column", columnName), Tag = columnName });
			}
			SelectedItem = Items[0];
		}
		protected override void InitializeItems() {
			Items = new ObservableCollection<MenuItemInfo>();
			InitializeItems(Items);
		}
	}
	enum SelectedCommandType { Smart, Expand }
	class ColumnGenerationActionListPropertyLineContext : ActionListPropertyLineContext {
		IModelItem ModelItem { get { return Context.ModelItem; } }
		DataControlBase DataControl { get { return ModelItem.GetCurrentValue() as DataControlBase; } }
		public ColumnGenerationActionListPropertyLineContext(IPropertyLineContext context)
			: base(context) { }
		protected override void InitializeItems() {
			ObservableCollection<MenuItemInfo> items = new ObservableCollection<MenuItemInfo>();
			items.Add(new MenuItemInfo() { Command = SelectedItemCommand, Caption = SR.GenerateColumns, Tag = SelectedCommandType.Smart });
			items.Add(new MenuItemInfo() { Command = SelectedItemCommand, Caption = SR.GenerateColumnsAndExpandProperties, Tag = SelectedCommandType.Expand });
			Items = items;
			SelectedItem = items[0];
		}
		protected override void OnSelectedItemExecute(MenuItemInfo param) {
			new GenerateColumnsActionProvider(Context as FrameworkElementSmartTagPropertiesViewModel, param != null ? (SelectedCommandType)param.Tag : SelectedCommandType.Smart).CommandAction(null);
		}
	}
	sealed class GridControlPropertyLinesProvider : DataControlPropertyLinesProviderBase {
		public GridControlPropertyLinesProvider() : base(typeof(GridControl)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			DataControlBase dataControl = ((IPropertyLineContext)viewModel).ModelItem.GetCurrentValue() as DataControlBase;
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => GridControl.ViewProperty), typeof(DataViewBase), DXTypeInfoInstanceSource.FromTypeList(GridDesignTimeHelper.GetAvailableViewTypes(dataControl))));
			lines.Add(() => new GridViewNestedPropertyLinesViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => GridControl.ViewProperty)));
			return lines;
		}
	}
	sealed class GridViewNestedPropertyLinesViewModel : NestedPropertyLinesViewModel {
		public new FrameworkElementSmartTagPropertiesViewModel Context { get { return base.Context as FrameworkElementSmartTagPropertiesViewModel; } }
		public GridViewNestedPropertyLinesViewModel(IPropertyLineContext context, string nestedProperty) : base(context, nestedProperty) { }
		public override void OnSelectedItemPropertyChanged(string propertyName) {
			base.OnSelectedItemPropertyChanged(propertyName);
			Context.Lines.Where(el => el is ActionPropertyLineViewModel).Cast<ActionPropertyLineViewModel>().ToList().ForEach(el => el.UpdateCommand());
		}
	}
	sealed class TreeListControlPropertyLinesProvider : DataControlPropertyLinesProviderBase {
		public TreeListControlPropertyLinesProvider() : base(typeof(TreeListControl)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TreeListControl.ViewProperty), typeof(DataViewBase), DXTypeInfoInstanceSource.FromTypeList(new Type[] { typeof(TreeListView) })));
			lines.Add(() => new NestedPropertyLinesViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TreeListControl.ViewProperty)));
			return lines;
		}
	}
	abstract class ViewPropertyLinesProviderBase : PropertyLinesProviderBase {
		public ViewPropertyLinesProviderBase(Type itemType)
			: base(itemType) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DataViewBase.AllowEditingProperty)));
			return lines;
		}
	}
	sealed class TableViewProperyLinesProvider : ViewPropertyLinesProviderBase {
		public TableViewProperyLinesProvider() : base(typeof(TableView)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TableView.AutoWidthProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TableView.AllowPerPixelScrollingProperty)));
			return lines;
		}
	}
	sealed class CardViewPropertyLinesProvider : ViewPropertyLinesProviderBase {
		public CardViewPropertyLinesProvider() : base(typeof(CardView)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => CardView.CardLayoutProperty), typeof(CardLayout)));
			return lines;
		}
	}
	sealed class TreeListViewPropertyLinesProvider : ViewPropertyLinesProviderBase {
		public TreeListViewPropertyLinesProvider() : base(typeof(TreeListView)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TreeListView.AutoWidthProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TreeListView.AllowPerPixelScrollingProperty)));
			return lines;
		}
	}
	sealed class ColumnPropertyLinesProvider : PropertyLinesProviderBase {
		public ColumnPropertyLinesProvider() : base(typeof(ColumnBase)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ColumnBase.FieldNameProperty)) { ItemsSource = GetFieldNames(viewModel.RuntimeSelectedItem) });
			if(CanExpandColumn(viewModel))
				lines.Add(() => ActionPropertyLineViewModel.CreateLine(new ExpandColumnActionProvider(viewModel)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BaseColumn.HeaderProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ColumnBase.UnboundTypeProperty), typeof(UnboundColumnType)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ColumnBase.UnboundExpressionProperty)) { Command = new ShowUnboundExpressionEditorCommand(viewModel), CommandParameter = ((IPropertyLineContext)viewModel).ModelItem });
			lines.Add(() => GetEditSettingsProperty(viewModel));
			lines.Add(() => new NestedPropertyLinesViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ColumnBase.EditSettingsProperty)));
			if(CanShowConditionalFormattingManager(viewModel.RuntimeSelectedItem))
				lines.Add(() => ActionPropertyLineViewModel.CreateLine(new ConditionalFormattingManagerActionProvider(viewModel)));
			return lines;
		}
		ItemListPropertyLineViewModel GetEditSettingsProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			ItemListPropertyLineViewModel result = new ItemListPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ColumnBase.EditSettingsProperty), typeof(BaseEditSettings), DXTypeInfoInstanceSource.FromTypeList(TypeLists.EditSettingsTypes.Where(t => (t.Item2 & PropertyTarget.Grid) != 0).Select(t => t.Item1)));
			result.SkipCopyingProperty.Add(DependencyPropertyHelper.GetPropertyName(() => TextEditSettings.MaskProperty), SkipCopyMaskProperty);
			result.SkipCopyingProperty.Add(DependencyPropertyHelper.GetPropertyName(() => TextEditSettings.MaskTypeProperty), SkipCopyMaskProperty);
			result.SkipCopyingProperty.Add(DependencyPropertyHelper.GetPropertyName(() => BaseEditSettings.StyleSettingsProperty), SkipCopyiStyleSettingsProperty);
			return result;
		}
		private bool SkipCopyiStyleSettingsProperty(IModelItem oldValue, IModelItem newValue) {
			return true;
		}
		static List<Type> SupportMaskEditSettings = new List<Type>() { 
			typeof(TextEditSettings), typeof(DateEditSettings), typeof(ButtonEditSettings), typeof(SpinEditSettings)
		};
		bool SkipCopyMaskProperty(IModelItem oldValue, IModelItem currentValue) {
			return !SupportMaskEditSettings.Contains(currentValue.ItemType);
		}
		bool CanExpandColumn(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			IEditingContext context = viewModel.RuntimeSelectedItem.Context;
			XpfModelItem item = viewModel.RuntimeSelectedItem as XpfModelItem;
			ModelItem modelItem = item.Value;
			if(item == null || item.Value.View == null) {
				ModelItem DataControl = BarManagerDesignTimeHelper.FindParentByType<DataControlBase>(modelItem);
				if(DataControl.Properties["Bands"].Collection.Count == 0)
					return true;
			}
			return false;
		}
		bool CanShowConditionalFormattingManager(IModelItem item) {
			return new ConditionalFormattingDialogServant().IsConditionalFormattingMenuAllowed(((XpfModelItem)item).Value);
		}
		IEnumerable<object> GetFieldNames(IModelItem item) {
			ModelItem dataControl = BarManagerDesignTimeHelper.FindParentByType<DataControlBase>((item as XpfModelItem).Value);
			if(dataControl == null || !GridDesignTimeHelper.IsFieldListAvailable(dataControl.GetCurrentValue() as DataControlBase))
				return null;
			return GridControlHelper.GetAllColumnNames(dataControl.GetCurrentValue() as DataControlBase);
		}
	}
	sealed class BandPropertyLinesProvider : PropertyLinesProviderBase {
		public BandPropertyLinesProvider() : base(typeof(BandBase)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BaseColumn.HeaderProperty)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new AddBandToBandActionProvider(viewModel)));
			lines.Add(() => new ActionListPropertyLineViewModel(new AddColumnInBandActionListPropertyLineContext(viewModel)));
			return lines;
		}
	}
	public class ShowUnboundExpressionEditorCommand : PropertyLineCommandCore {
		IModelItem columnModel;
		public ShowUnboundExpressionEditorCommand(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			columnModel = viewModel.RuntimeSelectedItem;
		}
		public override void Execute(object parameter) {
			ColumnBase column = columnModel.GetCurrentValue() as ColumnBase;
			CloseSmartTag(parameter as IModelItem);
			DataControlBase dataControl = DevExpress.Xpf.Core.Design.BarManagerDesignTimeHelper.FindParentByType<DataControlBase>(((XpfModelItem)columnModel).Value).GetCurrentValue() as DataControlBase;
			if(column == null || dataControl == null)
				return;
			DesignTimeExpressionEditorControl expressionEditorControl = new DesignTimeExpressionEditorControl(column);
			DialogClosedDelegate closedHandler = delegate(bool? dialogResult) {
				if(dialogResult == true)
					columnModel.Properties["UnboundExpression"].SetValue(expressionEditorControl.Expression);
				OpenSmartTag(parameter as IModelItem);
			};
			ExpressionEditorHelper.ShowExpressionEditor(expressionEditorControl, GridControlHelper.GetView(dataControl), closedHandler);
		}
	}
}
