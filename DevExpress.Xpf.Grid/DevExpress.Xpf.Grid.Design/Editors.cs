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

#if SILVERLIGHT
extern alias Platform;
#endif
using Microsoft.Windows.Design.PropertyEditing;
using System.Windows;
using Microsoft.Windows.Design.Model;
using System.Windows.Controls;
using System.Collections.Generic;
using System;
using Microsoft.Windows.Design;
using System.Windows.Input;
using System.Windows.Data;
using DevExpress.Xpf.Design;
using DevExpress.Xpf.Core.Design;
#if SILVERLIGHT
using FrameworkElement = Platform::System.Windows.FrameworkElement;
using AssemblyInfo = Platform::AssemblyInfo;
using Platform::DevExpress.Data;
using Platform::DevExpress.Xpf.Data.Native;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Xpf.Editors;
using Platform::DevExpress.Xpf.Core.Native;
using Platform::DevExpress.Xpf.Grid;
using Platform::DevExpress.Xpf.Grid.Native;
using Platform::DevExpress.Xpf.Grid.LookUp;
using Platform::DevExpress.Xpf.Editors.ExpressionEditor;
using Platform::DevExpress.Xpf.Core;
#else
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Data;
using DevExpress.Xpf.Data.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.LookUp;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Editors.ExpressionEditor;
using DevExpress.Xpf.Core;
#endif
using System.Linq;
using DevExpress.Xpf.Core.ConditionalFormatting;
namespace DevExpress.Xpf.Grid.Design {
	internal static class GridDesignTimeHelper {
		static ResourceDictionary editorsTemplates;
		public static ResourceDictionary EditorsTemplates { 
			get {
				if(editorsTemplates == null)
					editorsTemplates = new ResourceDictionary() { Source = new Uri("/DevExpress.Xpf.Grid" + AssemblyInfo.VSuffix + ".Design;component/GridEditorsTemplates.xaml", UriKind.Relative) };
				return editorsTemplates;
			}
		}
		public static Type[] GetAvailableViewTypes(DataControlBase dataControl) {
			List<Type> result = new List<Type>();
			result.Add(typeof(TableView));
			if(!(dataControl is GridControl) || ((GridControl)dataControl).DetailDescriptor == null) {
				result.Add(typeof(TreeListView));
#if !SILVERLIGHT
			result.Add(typeof(CardView));
#endif
			}
			return result.ToArray();
		}
		public static readonly Type[] GridAvailableViewTypes = new Type[] { 
			typeof(TableView), 
#if !SILVERLIGHT
			typeof(CardView), 
#endif
			typeof(TreeListView), 
		};
		public static readonly Type[] GridDetailDescriptorTypes = new Type[] {
			typeof(DataControlDetailDescriptor),
			typeof(TabViewDetailDescriptor),
			typeof(ContentDetailDescriptor)
		};
		public static readonly Type[] TreeListAvailableViewTypes = new Type[] { 
			typeof(TreeListView), 
		};
		public static readonly char[] AvailablePasswordChars = new char[] { '*', (char)9679, 'x', '#', 'o' };
		public static ModelItem GetPrimarySelection(PropertyValue propertyValue) {
			return GetPrimarySelection(propertyValue.ParentProperty.Context);
		}
		public static ModelItem GetPrimarySelection(EditingContext context) {
			return context.Items.GetValue<Microsoft.Windows.Design.Interaction.Selection>().PrimarySelection;
		}
		public static DataControlBase GetGrid(PropertyValue columnPropertyValue) {
			ModelItem selectedItem = GetPrimarySelection(columnPropertyValue);
			ModelItem gridItem = IsGridSelected(columnPropertyValue) ? selectedItem : BarManagerDesignTimeHelper.FindParentByType<DataControlBase>(selectedItem);
			return gridItem.View != null ? gridItem.View.PlatformObject as DataControlBase : null;
		}
		public static bool IsGridSelected(PropertyValue columnPropertyValue) {
			ModelItem selectedItem = GetPrimarySelection(columnPropertyValue);
			return IsGridItem(selectedItem);
		}
		public static bool IsGridItem(ModelItem selectedItem) {
			return selectedItem.IsItemOfType(typeof(DataControlBase));
		}
		public static ModelItem CreateModelItem(EditingContext context, Type elementType, CreateOptions createOptions = CreateOptions.InitializeDefaults) {
			ModelItem modelItem = ModelFactory.CreateItem(context, elementType, createOptions, null);
			ClearProperty(modelItem, "HorizontalAlignment");
			ClearProperty(modelItem, "VerticalAlignment");
			return modelItem;
		}
		static void ClearProperty(ModelItem modelItem, string propertyName) {
			ModelProperty property = modelItem.Properties.Find(propertyName);
			if(property != null)
				property.ClearValue();
		}
		public static ModelItemCollection GetGridColumnsCollection(ModelItem gridModelItem) {
			return gridModelItem.Properties["Columns"].Collection;
		}
		public static string GetNoDesignTimeDataSourceMassage(string formatString, Type controlType) {
			return string.Format(formatString, controlType.Name, "DesignTimeDataObjectType", "ItemsSource");
		}
		public static bool IsFieldListAvailable(DataControlBase dataControl) {
			DesignTimeDataSource designTimeDataSource = GridControlHelper.GetActualItemsSource(dataControl) as DesignTimeDataSource;
			return GridControlHelper.GetActualItemsSource(dataControl) != null && (designTimeDataSource == null || designTimeDataSource.AreRealColumnsAvailable);
		}
		public static ModelItem GetBandModelItem(ModelItem dataControl, BandBase band, List<ModelItem> bandsCache = null) {
			if(bandsCache != null) {
				foreach(var b in bandsCache) {
					if(b.GetCurrentValue() == band)
						return b;
				}
			}
			if(dataControl == null)
				return null;
			ModelProperty bands = dataControl.Properties["Bands"];
			return GetBandModelItemCore(bands, band);
		}
		static ModelItem GetBandModelItemCore(ModelProperty rootBand, BandBase band) {
			if(rootBand == null)
				return null;
			foreach(var b in rootBand.Collection) {
				if(b.GetCurrentValue() == band)
					return b;
				ModelItem result = GetBandModelItemCore(b.Properties["Bands"], band);
				if(result != null)
					return result;
			}
			return null;
		}
		public static ModelItemCollection GetBottomLeftBandColumnsCollection(ModelItem bandsOwnerModel, List<ModelItem> bandsCache) {
			ModelItem bandModel = GetBottomLeftBand(bandsOwnerModel, null, bandsCache);
			if(bandModel == null)
				return null;
			return bandModel.Properties["Columns"].Collection;
		}
		public static List<Tuple<ColumnBase, int, ColumnSortOrder, int>> GetGroupSortColumns(DataControlBase grid) {
			List<Tuple<ColumnBase, int, ColumnSortOrder, int>> groupSortCache = new List<Tuple<ColumnBase, int, ColumnSortOrder, int>>();
			IColumnCollection columns = GridControlHelper.GetColumns(grid);
			foreach(ColumnBase c in columns) {
				if(c.SortOrder != ColumnSortOrder.None)
					groupSortCache.Add(new Tuple<ColumnBase, int, ColumnSortOrder, int>(c, c is GridColumn ? ((GridColumn)c).GroupIndex : -1, c.SortOrder, c.SortIndex));
			}
			return groupSortCache;
		}
		public static void ApplyGroupSortColumns(DataControlBase grid, List<Tuple<ColumnBase, int, ColumnSortOrder, int>> groupSortCache) {
			IColumnCollection columns = GridControlHelper.GetColumns(grid);
			columns.BeginUpdate();
			foreach(ColumnBase c in columns) {
				var info = groupSortCache.Where(i => i.Item1 == c).ToList().FirstOrDefault();
				if(info == null)
					continue;
				if(c is GridColumn)
					((GridColumn)c).GroupIndex = info.Item2;
				c.SortOrder = info.Item3;
				c.SortIndex = info.Item4;
			}
			columns.EndUpdate();
		}
		public static void FillBandsColumnsAndRestoreGrouping(DataControlBase grid) {
			List<Tuple<ColumnBase, int, ColumnSortOrder, int>> groupSortCache = GetGroupSortColumns(grid);
			GridControlHelper.FillBandsColumns(grid);
			ApplyGroupSortColumns(grid, groupSortCache);
		}
		public static ModelItem GetBottomLeftBand(ModelItem bandModel, ModelItem bandToSkip, List<ModelItem> bandsCache) {
			if(bandModel == bandToSkip)
				return null;
			IBandsOwner bandOwner = bandModel.GetCurrentValue() as IBandsOwner;
			List<BandBase> bands = null;
			if(bandOwner == null) {
				bands = new List<BandBase>();
				foreach(var b in bandModel.Properties["Bands"].Collection)
					bands.Add(b.GetCurrentValue() as BandBase);
			}
			else
				bands = bandOwner.VisibleBands;
			foreach(IBandsOwner b in bands) {
				if(bandToSkip != null && bandToSkip.GetCurrentValue() == b)
					continue;
				ModelItem result = GetBottomLeftBand(GetBandModelItem(bandModel, (BandBase)b, bandsCache), bandToSkip, bandsCache);
				if(result != null)
					return result;
			}
			if(bandModel.GetCurrentValue() is DataControlBase)
				return null;
			return bandModel;
		}
		public static void DeleteBandAndMoveColumnsToRoot(ModelItem bandModel) {
			object parent = bandModel.Parent.GetCurrentValue();
			ModelItem dataControlModel = DevExpress.Xpf.Core.Design.BarManagerDesignTimeHelper.FindParentByType<DataControlBase>(bandModel);
			List<ModelItem> allBandsModels = GetAllBandsModels(dataControlModel);
			ModelItem bottomLeftBand = GetBottomLeftBand(dataControlModel, bandModel, allBandsModels);
			if(bottomLeftBand == null) {
				MoveColumnsFromBandToDataControl(dataControlModel, bandModel);
				RemoveBandModel(allBandsModels, dataControlModel, bandModel);
				return;
			}
			List<ModelItem> columns = GetAllColumnsFromBandOwner(bandModel);
			ModelItemCollection columnCollection = bottomLeftBand.Properties["Columns"].Collection;
			RemoveBandModel(allBandsModels, dataControlModel, bandModel);
			foreach(var c in columns)
				columnCollection.Add(c);
		}
		static void RemoveBandModel(List<ModelItem> allBandsModels, ModelItem dataControlModel, ModelItem modelItem) {
			if(dataControlModel.Properties["Bands"].Collection.Contains(modelItem)) {
				dataControlModel.Properties["Bands"].Collection.Remove(modelItem);
				return;
			}
			BandBase ownerBand = GridControlHelper.GetBandOwner(modelItem.GetCurrentValue() as BandBase) as BandBase;
			if(ownerBand != null)
				GetBandModelItem(dataControlModel, ownerBand, allBandsModels).Properties["Bands"].Collection.Remove(modelItem);
			dataControlModel.Properties["Bands"].Collection.Remove(modelItem);
		}
		static void MoveColumnsFromBandToDataControl(ModelItem dataControlModel, ModelItem bandModel) {
			foreach(var c in GetAllColumnsFromBandOwner(bandModel))
				dataControlModel.Properties["Columns"].Collection.Add(c);
			dataControlModel.Properties["Bands"].Collection.Clear();
		}
		public static string GetIndexedName(List<ModelItem> sourceCollection, string defaultName, string propertyName) {
			int avaiableValue = 1;
			foreach(var c in sourceCollection) {
				if(!c.Properties[propertyName].IsSet || !c.Properties[propertyName].Value.GetCurrentValue().ToString().StartsWith(defaultName))
					continue;
				string currentStringValue = c.Properties[propertyName].Value.GetCurrentValue().ToString().Replace(defaultName, "");
				int tempValue = 0;
				if(int.TryParse(currentStringValue, out tempValue) && avaiableValue < ++tempValue)
					avaiableValue = tempValue;
			}
			return String.Format(defaultName + "{0}", avaiableValue);
		}
		public static int FindDefaultNameIndex(ModelItem dataControlModel, string propertyName, string defaultName, Func<ModelItem, List<ModelItem>> getElementsAction) {
			int result = 1;
			if(dataControlModel == null)
				return result;
			foreach(ModelItem element in getElementsAction(dataControlModel)) {
				string caption = element.Properties[propertyName].Value.GetCurrentValue() as string;
				if(caption == null || !caption.StartsWith(defaultName))
					continue;
				caption = caption.Remove(0, defaultName.Length);
				int localColumnNumber = 0;
				if(Int32.TryParse(caption, out localColumnNumber) && localColumnNumber + 1 > result)
					result = localColumnNumber + 1;
			}
			return result;
		}
		public static List<ModelItem> GetAllColumnsFromBandOwner(ModelItem bandsOwnerModel) {
			List<ModelItem> columns = new List<ModelItem>();
			foreach(var b in bandsOwnerModel.Properties["Bands"].Collection)
				columns.AddRange(GetAllColumnsFromBandOwner(b));
			foreach(var c in bandsOwnerModel.Properties["Columns"].Collection)
				columns.Add(CloneModelItemHelper.CloneItem(bandsOwnerModel.Root.Context, c));
			return columns;
		}
		public static List<ModelItem> GetAllColumnsFromBandOwnerSkipClone(ModelItem bandsOwnerModel) {
			List<ModelItem> columns = new List<ModelItem>();
			foreach(var b in bandsOwnerModel.Properties["Bands"].Collection)
				columns.AddRange(GetAllColumnsFromBandOwnerSkipClone(b));
			foreach(var c in bandsOwnerModel.Properties["Columns"].Collection)
				columns.Add(c);
			return columns;
		}
		public static List<ModelItem> GetAllBandsModels(ModelItem bandModel) {
			List<ModelItem> result = new List<ModelItem>();
			foreach(var b in bandModel.Properties["Bands"].Collection) {
				result.Add(b);
				result.AddRange(GetAllBandsModels(b));
			}
			return result;
		}
	}
	internal abstract class GridDialogPropertyValueEditorBase : DialogPropertyValueEditor {
		public GridDialogPropertyValueEditorBase() {
			this.InlineEditorTemplate = GridDesignTimeHelper.EditorsTemplates["GridDialogPropertyValueEditorTemplate"] as DataTemplate;
		}
		public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource) {
			ModelItem selectedItem = GridDesignTimeHelper.GetPrimarySelection(propertyValue);
			DataControlBase dataControl = GetGrid(propertyValue);
			if(dataControl == null) {
				MessageBox.Show(CantShowEditorMessage);
				return;
			}
			string newStringValue = ShowEditorCore(dataControl, propertyValue, selectedItem);
			if(newStringValue != propertyValue.StringValue)
				propertyValue.StringValue = newStringValue;
		}
		protected virtual DataControlBase GetGrid(PropertyValue propertyValue) {
			return GridDesignTimeHelper.GetGrid(propertyValue);
		}
		protected abstract string CantShowEditorMessage { get; }
		protected abstract string ShowEditorCore(DataControlBase grid, PropertyValue propertyValue, ModelItem selectedItem);
	}
	internal class UnboundExpressionEditor : GridDialogPropertyValueEditorBase {
		protected override string CantShowEditorMessage { get { return SR.CantShowUnboundExpressionEditorMessage; } }
		protected override string ShowEditorCore(DataControlBase dataControl, PropertyValue propertyValue, ModelItem selectedItem) {
			ColumnBase column = GridDesignTimeHelper.IsGridSelected(propertyValue) ?
				(ColumnBase)propertyValue.ParentProperty.ParentValue.Value :
				GridControlHelper.GetColumns(dataControl)[(string)selectedItem.Properties["FieldName"].ComputedValue];
			GridControlHelper.GetView(dataControl).ShowUnboundExpressionEditor(column);
			return column.UnboundExpression;
		}
	}
	internal class DesignTimeExpressionEditorControl : ExpressionEditorControl {
		public DesignTimeExpressionEditorControl(IDataColumnInfo columnInfo)
			: base(columnInfo) { }
	}
	internal class FieldNameEditor : PropertyValueEditor {
		public FieldNameEditor() {
			this.InlineEditorTemplate = GridDesignTimeHelper.EditorsTemplates["ColumnFieldNameEditorTemplate"] as DataTemplate;
		}
	}
	public abstract class DropDownComboBoxDesigTimeDecorator : ComboBoxDesigTimeDecorator {
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			Control.DropDownOpened += new EventHandler(OnDropDownOpened);
		}
		protected override void UnsubscribeControlEvents() {
			base.UnsubscribeControlEvents();
			Control.DropDownOpened -= new EventHandler(OnDropDownOpened);
		}
		protected abstract void OnDropDownOpened(object sender, EventArgs e);
	}
	public class FieldNameComboBoxDesigTimeDecorator : DropDownComboBoxDesigTimeDecorator {
		protected override void OnDropDownOpened(object sender, EventArgs e) {
			DataControlBase dataControl = GridDesignTimeHelper.GetGrid((PropertyValue)DataContext);
			if(dataControl != null && GridDesignTimeHelper.IsFieldListAvailable(dataControl)) {
				Control.ItemsSource = GridControlHelper.GetAllColumnNames(dataControl);
			} else {
				Control.ItemsSource = null;
				Control.Items.Clear();
				string message = dataControl != null ? GridDesignTimeHelper.GetNoDesignTimeDataSourceMassage(SR.NoAvailableFieldNamesMessage, dataControl != null ? dataControl.GetType() : typeof(GridControl)) : SR.CantShowFieldNameEditorMessage;
				Control.Items.Add(new ComboBoxItem() { 
					Content = new TextBox() {
						Text = message, 
						Margin = new Thickness(8), 
						TextWrapping = TextWrapping.Wrap,
						MaxWidth = 200,
					}, 
					IsEnabled = false 
				});
			}
		}
	}
#if !SILVERLIGHT
	internal class FilterStringEditor : GridDialogPropertyValueEditorBase {
		protected override string CantShowEditorMessage { get { return SR.CantShowFilterStringEditorMessage; } }
		protected override string ShowEditorCore(DataControlBase dataControl, PropertyValue propertyValue, ModelItem selectedItem) {
			GridControlHelper.GetView(dataControl).ShowFilterEditor(null);
			return dataControl.FilterString;
		}
	}
	internal class FormatConditionExpressionEditor : GridDialogPropertyValueEditorBase {
		class EmptyColumnInfo : IDataColumnInfo {
			public EmptyColumnInfo(DataControlBase grid) {
				Columns = GridControlHelper.GetDataColumnInfo(grid).Cast<IDataColumnInfo>().ToList();
			}
			#region IDataColumnInfo Members
			public string Caption { get { return string.Empty; } }
			public List<IDataColumnInfo> Columns { get; set; }
			public DataControllerBase Controller { get { return null; } }
			public string FieldName { get { return string.Empty; } }
			public Type FieldType { get { return typeof(object); } }
			public string Name { get { return string.Empty; } }
			public string UnboundExpression { get { return string.Empty; } }
			#endregion
		}
		protected override DataControlBase GetGrid(PropertyValue propertyValue) {
			return GridDesignTimeHelper.GetGrid(propertyValue);
		}
		protected override string ShowEditorCore(DataControlBase grid, PropertyValue propertyValue, ModelItem selectedItem) {
			string expression = propertyValue.StringValue;
			ExpressionEditorControl expressionEditorControl = new ExpressionEditorControl(new EmptyColumnInfo(grid));
			DialogClosedDelegate closedHandler = delegate(bool? dialogResult) {
				if(dialogResult == true)
					expression = expressionEditorControl.Expression;
			};
			DevExpress.Xpf.Editors.ExpressionEditor.Native.ExpressionEditorHelper.ShowExpressionEditor(expressionEditorControl, GridControlHelper.GetView(grid), closedHandler);
			return expression;
		}
		protected override string CantShowEditorMessage { get { return SR.CantShowFormatConditionExpressionEditorMessage; } }
	}
	internal class FormatConditionExpressionFilterEditor : FormatConditionExpressionEditor {
		protected override string ShowEditorCore(DataControlBase grid, PropertyValue propertyValue, ModelItem selectedItem) {
			FilterControl filterControl = new FilterControl();
			filterControl.FilterCriteria = DevExpress.Data.Filtering.CriteriaOperator.TryParse(propertyValue.StringValue);
			filterControl.ShowBorder = false;
			filterControl.SourceControl = grid.FilteredComponent;
			FloatingContainer.ShowDialogContent(filterControl, GridControlHelper.GetView(grid), new Size(500, 350), new FloatingContainerParameters() {
				Title = "Custom Condition Editor",
				AllowSizing = true,
				ShowApplyButton = false,
				CloseOnEscape = false,
			});
			if(!Object.ReferenceEquals(filterControl.FilterCriteria, null))
				return filterControl.FilterCriteria.ToString();
			return propertyValue.StringValue;
		}
	}
	internal class ConditionFormatNameEditor : PropertyValueEditor {
		public ConditionFormatNameEditor() {
			this.InlineEditorTemplate = GridDesignTimeHelper.EditorsTemplates["ConditionFormatNameEditorTemplate"] as DataTemplate;
		}
	}
	public class ConditionFormatNameComboBoxDesigTimeDecorator : DropDownComboBoxDesigTimeDecorator {
		protected override void OnDropDownOpened(object sender, EventArgs e) {
			PropertyValue propertyValue = (PropertyValue)DataContext;
			DataControlBase grid = GridDesignTimeHelper.GetGrid(propertyValue);
			if(grid != null) {
				FormatConditionBase condition = FindFormatCondition(propertyValue);
				if(condition == null)
					condition = GridDesignTimeHelper.GetPrimarySelection(propertyValue).GetCurrentValue() as FormatConditionBase;
				if(condition != null) {
					Control.ItemsSource = GetFormatNames(condition, GridControlHelper.GetView(grid));
					return;
				}
			}
			MessageBox.Show(SR.CantShowFormatNameEditorMessage);
		}
		FormatConditionBase FindFormatCondition(PropertyValue propertyValue) {
			PropertyValue value = propertyValue;
			while(value != null && value.ParentProperty != null) {
				PropertyEntry entry = value.ParentProperty;
				if(typeof(FormatConditionBase).IsAssignableFrom(entry.PropertyType))
					return value.Value as FormatConditionBase;
				value = entry.ParentValue;
			}
			return null;
		}
		IEnumerable<string> GetFormatNames(FormatConditionBase format, DataViewBase view) {
			if(format == null)
				return null;
			string predefinedFormatsPropertyName = GridControlHelper.GetOwnerPredefinedFormatsPropertyName(format);
			FormatInfoCollection predefinedFormats = view.GetType().GetProperty(predefinedFormatsPropertyName).GetValue(view, null) as FormatInfoCollection;
			return predefinedFormats != null ? predefinedFormats.Select(x => x.FormatName) : null;
		}
	}
#endif
}
