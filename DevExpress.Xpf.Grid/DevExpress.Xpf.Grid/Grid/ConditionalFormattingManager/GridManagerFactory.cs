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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Editors.Settings;
using System.Windows.Data;
using System.Windows;
using DevExpress.Mvvm.Native;
using System.Windows.Input;
using DevExpress.Xpf.Grid.Native;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Utils;
using ConditionFormat = DevExpress.Xpf.Core.ConditionalFormatting.Format;
using System.Windows.Markup;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public class GridManagerFactory : IGridManagerFactory {
		#region IGridManagerFactory Members
		UIElement IGridManagerFactory.CreateGrid() {
			var grid = new GridControl();
			Setup(grid);
			return grid;
		}
		ContentControl IGridManagerFactory.CreatePreviewControl() {
			return new FormatPreviewControl();
		}
		#endregion
		const string AppliesToFieldname = "AppliesTo";
		const string ApplyToRowFieldname = "ApplyToRow";
		const string RowNameFieldName = "RowName";
		const string ColumnNameFieldName = "ColumnName";
		void Setup(GridControl grid) {
			TableView view = new TableView();
			SetupView(view);
			grid.View = view;
			SetupColumns(grid);
			grid.DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
			view.CellValueChanging += new CellValueChangedEventHandler(OnCellValueChanging);
			view.PreviewMouseDoubleClick += new MouseButtonEventHandler(OnRowDoubleClick);
			DesignerProperties.SetIsInDesignMode(grid, false);
		}
		void SetupColumns(GridControl grid) {
			var ruleColumn = new GridColumn();
			ruleColumn.FieldName = "Rule";
			ruleColumn.Header = GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Rule);
			ruleColumn.AllowFocus = false;
			grid.Columns.Add(ruleColumn);
			var formatColumn = new GridColumn();
			formatColumn.FieldName = "PreviewFormat";
			formatColumn.Header = GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Format);
			formatColumn.AllowFocus = false;
			formatColumn.CellTemplate = CreatePreviewTemplate();
			grid.Columns.Add(formatColumn);
			var applyToRowColumn = new GridColumn();
			applyToRowColumn.FieldName = ApplyToRowFieldname;
			applyToRowColumn.Header = GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ApplyToRow);
			applyToRowColumn.AllowEditing = DevExpress.Utils.DefaultBoolean.True;
			applyToRowColumn.CellTemplate = CreateApplyToRowTemplate();
			grid.Columns.Add(applyToRowColumn);
			var rowNameColumn = new GridColumn();
			rowNameColumn.FieldName = RowNameFieldName;
			rowNameColumn.AllowEditing = DevExpress.Utils.DefaultBoolean.True;
			rowNameColumn.EditSettings = new ComboBoxEditSettings { IsTextEditable = false, DisplayMember = "Caption", ValueMember = "FieldName" };			
			rowNameColumn.Visible = false;
			grid.Columns.Add(rowNameColumn);
			var columnNameColumn = new GridColumn();
			columnNameColumn.FieldName = ColumnNameFieldName;
			columnNameColumn.AllowEditing = DevExpress.Utils.DefaultBoolean.True;
			columnNameColumn.EditSettings = new ComboBoxEditSettings { IsTextEditable = false, DisplayMember = "Caption", ValueMember = "FieldName" };
			columnNameColumn.Visible = false;
			grid.Columns.Add(columnNameColumn);
			var applyColumn = new GridColumn();
			applyColumn.FieldName = AppliesToFieldname;
			applyColumn.EditSettings = new ComboBoxEditSettings { IsTextEditable = false, DisplayMember = "Caption", ValueMember = "FieldName" };
			applyColumn.AllowEditing = DevExpress.Utils.DefaultBoolean.True;
			grid.Columns.Add(applyColumn);
		}
		DataTemplate CreatePreviewTemplate() {
			string className = typeof(FormatPreviewControl).Name;
			string template =
				@"<DataTemplate " +
				@"xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" " +
				@"xmlns:" + XmlNamespaceConstants.GridPrefix + "=\"" + XmlNamespaceConstants.GridNamespaceDefinition + "\">" +
				@"<Border Padding=""0,3,8,3"">" +
				@"<" + XmlNamespaceConstants.GridPrefix + ":" + className + @" Content=""{Binding Value}""/>" +
				@"</Border>" +
				@"</DataTemplate>";
			return XamlReader.Parse(template) as DataTemplate;
		}
		DataTemplate CreateApplyToRowTemplate() {
			string className = typeof(DevExpress.Xpf.Editors.CheckEdit).Name;
			string template =
				@"<DataTemplate " +
				@"xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" " +
				@"xmlns:" + XmlNamespaceConstants.EditorsPrefix + "=\"" + XmlNamespaceConstants.EditorsNamespaceDefinition + "\">" +
				@"<" + XmlNamespaceConstants.EditorsPrefix + ":" + className + @" Name=""PART_Editor"" IsEnabled=""{Binding RowData.Row.CanApplyToRow}""/>" +
				@"</DataTemplate>";
			return XamlReader.Parse(template) as DataTemplate;
		}
		void SetupView(TableView view) {
			view.ShowGroupPanel = false;
			view.AllowResizing = false;
			view.AllowColumnFiltering = false;
			view.IsColumnMenuEnabled = false;
			view.AllowGrouping = false;
			view.AllowSorting = false;
			view.AutoWidth = true;
			view.AllowColumnMoving = false;
			view.ShowIndicator = false;
			view.ShowVerticalLines = false;
			view.AllowEditing = false;
			view.AllowPerPixelScrolling = true;
			view.RowMinHeight = 35d;
			view.EditorButtonShowMode = EditorButtonShowMode.ShowAlways;
			view.NavigationStyle = GridViewNavigationStyle.Cell;
			view.ShowSearchPanelMode = ShowSearchPanelMode.Never;
		}
		void SetupSource(GridControl grid, ManagerViewModel viewModel) {
			grid.SetBinding(GridControl.ItemsSourceProperty, new Binding("Items") { Source = viewModel });
			grid.SetBinding(GridControl.CurrentItemProperty, new Binding("SelectedItem") { Source = viewModel });
			grid.Columns[AppliesToFieldname].EditSettings.SetBinding(ComboBoxEditSettings.ItemsSourceProperty, new Binding("FieldNames") { Source = viewModel });
			grid.Columns[AppliesToFieldname].SetBinding(GridColumnBase.HeaderProperty, new Binding("ApplyToFieldNameCaption") { Source = viewModel });
			grid.Columns[RowNameFieldName].SetBinding(GridColumnBase.HeaderProperty, new Binding("ApplyToPivotRowCaption") { Source = viewModel });
			grid.Columns[ColumnNameFieldName].SetBinding(GridColumnBase.HeaderProperty, new Binding("ApplyToPivotColumnCaption") { Source = viewModel });
			if(viewModel == null || !viewModel.IsPivot) {
				grid.Columns[RowNameFieldName].Visible = false;
				grid.Columns[ColumnNameFieldName].Visible = false;
			} else {
				grid.Columns[ApplyToRowFieldname].Visible = false;
				grid.Columns[RowNameFieldName].Visible = true;
				grid.Columns[ColumnNameFieldName].Visible = true;
				grid.Columns[RowNameFieldName].EditSettings.SetBinding(ComboBoxEditSettings.ItemsSourceProperty, new Binding("PivotSpecialFieldNames") { Source = viewModel });
				grid.Columns[ColumnNameFieldName].EditSettings.SetBinding(ComboBoxEditSettings.ItemsSourceProperty, new Binding("PivotSpecialFieldNames") { Source = viewModel });
			}
		}
		void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			var grid = (GridControl)sender;
			(grid.DataContext as ManagerViewModel).Do(x => SetupSource(grid, x));
		}
		void OnRowDoubleClick(object sender, MouseButtonEventArgs e) {
			var tableView = sender as TableView;
			ITableViewHitInfo hitInfo = tableView.CalcHitInfo(e.OriginalSource as DependencyObject);
			if(hitInfo != null && hitInfo.InRow) {
				var vm = tableView.DataContext as ManagerViewModel;
				if(vm != null && vm.SelectedItem != null && e.ChangedButton == System.Windows.Input.MouseButton.Left) {
					vm.ShowEditDialog(vm.SelectedItem);
					e.Handled = true;
				}
			}
		}
		void OnCellValueChanging(object sender, CellValueChangedEventArgs e) {
			if(e.Column.FieldName == AppliesToFieldname || e.Column.FieldName == ApplyToRowFieldname || e.Column.FieldName == ColumnNameFieldName || e.Column.FieldName == RowNameFieldName)
				((TableView)sender).PostEditor();
		}
		string GetLocalizedString(ConditionalFormattingStringId id) {
			return ConditionalFormattingLocalizer.GetString(id);
		}
	}
}
namespace DevExpress.Xpf.Grid {
	public class FormatPreviewControl : ContentControl {
		public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(Freezable), typeof(FormatPreviewControl), new PropertyMetadata(null, (d, e) => ((FormatPreviewControl)d).UpdatePreview()));
		public Freezable Format {
			get { return (Freezable)GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}
		public FormatPreviewControl() {
			this.SetDefaultStyleKey(typeof(FormatPreviewControl));
		}
		void UpdatePreview() {
			var format = Format as Format;
			UpdatePreview<FontFamily>(FormatPreviewControl.FontFamilyProperty, ConditionFormat.FontFamilyProperty, null);
			UpdatePreview<Brush>(FormatPreviewControl.ForegroundProperty, ConditionFormat.ForegroundProperty, null);
			UpdatePreview<Brush>(FormatPreviewControl.BackgroundProperty, ConditionFormat.BackgroundProperty, null);
			UpdatePreview<Double>(FormatPreviewControl.FontSizeProperty, ConditionFormat.FontSizeProperty, 0d);
			UpdatePreview<FontStyle>(FormatPreviewControl.FontStyleProperty, ConditionFormat.FontStyleProperty, FontStyles.Normal);
			UpdatePreview<FontWeight>(FormatPreviewControl.FontWeightProperty, ConditionFormat.FontWeightProperty, FontWeights.Normal);
		}
		void UpdatePreview<T>(DependencyProperty previewProperty, DependencyProperty formatProperty, T defaultValue) {
			var format = Format as Format;
			if(format != null) {
				var formatValue = (T)format.GetValue(formatProperty);
				if(!Object.Equals(formatValue, defaultValue)) {
					SetValue(previewProperty, formatValue);
					return;
				}
			}
			ClearValue(previewProperty);
		}
	}
}
