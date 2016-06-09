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
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using System.Windows.Data;
using DevExpress.Xpf.Data;
using DevExpress.Mvvm.Native;
using System.Collections.Specialized;
using System.Windows.Controls;
using DevExpress.Xpf.GridData;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Grid {
	public partial class DataViewBase {
		public static bool DisableOptimizedModeVerification = false;
		static partial void RegisterClassCommandBindings() {
			Type ownerType = typeof(DataViewBase);
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.ChangeColumnSortOrder, new ExecutedRoutedEventHandler(OnChangeColumnSortOrder)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.ClearColumnFilter, (d, e) => ((DataViewBase)d).ClearColumnFilter(e), (d, e) => ((DataViewBase)d).OnCanClearColumnFilter(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.ShowFilterEditor, (d, e) => ((DataViewBase)d).ShowFilterEditor(e), (d, e) => ((DataViewBase)d).OnCanShowFilterEditor(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.ShowColumnChooser, (d, e) => ((DataViewBase)d).ShowColumnChooser(), (d, e) => ((DataViewBase)d).OnCanShowColumnChooser(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.HideColumnChooser, (d, e) => ((DataViewBase)d).HideColumnChooser(), (d, e) => ((DataViewBase)d).OnCanHideColumnChooser(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.MovePrevCell, (d, e) => ((DataViewBase)d).MovePrevCell(), (d, e) => e.CanExecute = ((DataViewBase)d).CanMovePrevCell()));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.MoveNextCell, (d, e) => ((DataViewBase)d).MoveNextCell(), (d, e) => e.CanExecute = ((DataViewBase)d).CanMoveNextCell()));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.MoveFirstCell, (d, e) => ((DataViewBase)d).MoveFirstCell(), (d, e) => e.CanExecute = ((DataViewBase)d).CanMoveFirstCell()));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.MoveLastCell, (d, e) => ((DataViewBase)d).MoveLastCell(), (d, e) => e.CanExecute = ((DataViewBase)d).CanMoveLastCell()));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.MoveFirstRow, (d, e) => ((DataViewBase)d).MoveFirstRow(), (d, e) => CanExecuteWithCheckActualView(e, () => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.CanPrevRow())));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.MoveLastRow, (d, e) => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.MoveLastOrLastMasterRow(), (d, e) => CanExecuteWithCheckActualView(e, () => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.CanNextRow())));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.MovePrevRow, (d, e) => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.Navigation.OnUp(false), (d, e) => CanExecuteWithCheckActualView(e, () => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.CanPrevRow())));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.MoveNextRow, (d, e) => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.Navigation.OnDown(), (d, e) => CanExecuteWithCheckActualView(e, () => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.CanNextRow())));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.MovePrevPage, (d, e) => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.Navigation.OnPageUp(), (d, e) => CanExecuteWithCheckActualView(e, () => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.CanPrevRow())));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.MoveNextPage, (d, e) => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.Navigation.OnPageDown(), (d, e) => CanExecuteWithCheckActualView(e, () => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.CanNextRow())));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.ClearFilter, (d, e) => ((DataViewBase)d).ClearFilter(), (d, e) => ((DataViewBase)d).OnCanClearFilter(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.DeleteFocusedRow, (d, e) => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.DeleteFocusedRow(), (d, e) => CanExecuteWithCheckActualView(e, () => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.CanDeleteFocusedRow())));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.EditFocusedRow, (d, e) => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.EditFocusedRow(), (d, e) => CanExecuteWithCheckActualView(e, () => ((DataViewBase)d).MasterRootRowsContainer.FocusedView.CanEditFocusedRow())));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.CancelEditFocusedRow, (d, e) => ((DataViewBase)d).CancelEditFocusedRow(), (d, e) => ((DataViewBase)d).OnCanCancelEditFocusedRow(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.EndEditFocusedRow, (d, e) => ((DataViewBase)d).EndEditFocusedRow(), (d, e) => ((DataViewBase)d).OnCanEndEditFocusedRow(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.ShowUnboundExpressionEditor, (d, e) => ((DataViewBase)d).ShowUnboundExpressionEditor(e), (d, e) => ((DataViewBase)d).OnCanShowUnboundExpressionEditor(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DataControlCommands.ShowUnboundExpressionEditor, (d, e) => ((DataViewBase)d).ShowUnboundExpressionEditor(e), (d, e) => ((DataViewBase)d).OnCanShowUnboundExpressionEditor(e)));
		}
		static void CanExecuteWithCheckActualView(CanExecuteRoutedEventArgs e, Func<bool> canExecute) {
			if(e.Source is DataViewBase && e.OriginalSource is DependencyObject && e.Source != ((DependencyObject)e.OriginalSource).GetValue(DataControlBase.ActiveViewProperty))
				e.CanExecute = false;
			else
				e.CanExecute = canExecute();
		}  
		protected override void OnVisualParentChanged(DependencyObject oldParent) {
			DependencyObject visualParent = VisualTreeHelper.GetParent(this);
			if(visualParent != null) {
				Binding binding = new Binding();
				binding.Path = new PropertyPath("(0)", ThemeManager.TreeWalkerProperty);
				binding.Source = visualParent;
				SetBinding(ThemeManager.TreeWalkerProperty, binding);
			} else {
				BindingOperations.ClearBinding(this, ThemeManager.TreeWalkerProperty);
			}
			base.OnVisualParentChanged(oldParent);
		}
		static void OnChangeColumnSortOrder(object sender, ExecutedRoutedEventArgs e) {
			((DataViewBase)sender).OnColumnHeaderClick(e.Parameter as ColumnBase);
		}
		void ClearColumnFilter(ExecutedRoutedEventArgs e) {
			ColumnBase column = (ColumnBase)GetColumnByCommandParameter(e.Parameter);
			if(column != null)
				DataControl.ClearColumnFilter(column);
		}
		void ShowFilterEditor(ExecutedRoutedEventArgs e) {
			ColumnBase defaultColumn = (ColumnBase)GetColumnByCommandParameter(e.Parameter);
			ShowFilterEditor(defaultColumn);
		}
		void ShowUnboundExpressionEditor(ExecutedRoutedEventArgs e) {
			ShowUnboundExpressionEditor(e.Parameter);
		}
		void OnCanClearColumnFilter(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanClearColumnFilter(GetColumnByCommandParameter(e.Parameter));
		}
		void OnCanShowFilterEditor(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanShowFilterEditor(GetColumnByCommandParameter(e.Parameter));
		}
		void OnCanShowColumnChooser(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanShowColumnChooser();
		}
		void OnCanHideColumnChooser(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanHideColumnChooser();
		}
		void OnCanClearFilter(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanClearFilter();
		}
		void OnCanDeleteFocusedRow(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanDeleteFocusedRow();
		}
		void OnCanEditFocusedRow(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanEditFocusedRow();
		}
		void OnCanShowUnboundExpressionEditor(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanShowUnboundExpressionEditor(e.Parameter);
		}
	}
	partial interface ITableView : IFormatsOwner {
		FormatConditionCollection FormatConditions { get; }
		UseLightweightTemplates? UseLightweightTemplates { get; set; }
		DataTemplate RowDetailsTemplate { get; set; }
		DataTemplateSelector RowDetailsTemplateSelector { get; set; }
		DataTemplateSelector ActualRowDetailsTemplateSelector { get; }
		DependencyPropertyKey ActualRowDetailsTemplateSelectorPropertyKey { get; }
		RowDetailsVisibilityMode RowDetailsVisibilityMode { get; set; }
		DataTemplate FormatConditionDialogServiceTemplate { get; set;}
		DataTemplate ConditionalFormattingManagerServiceTemplate { get; set; }
		bool UseRowDetailsTemplate(int rowHandle);
		void ShowFormatConditionDialog(ColumnBase column, FormatConditionDialogType dialogKind);
		void AddFormatCondition(FormatConditionBase formatCondition);
		bool AllowConditionalFormattingManager { get; set; }
		bool AllowConditionalFormattingMenu { get; set; }
	}
	public enum RowDetailsVisibilityMode {
		Collapsed,
		Visible,
		VisibleWhenFocused
	}
	[Flags]
	public enum UseLightweightTemplates {
		None = 0x0,
		Row = 0x1,
		GroupRow = 0x2,
		All = Row | GroupRow
	}
	[Flags]
	public enum UseCardLightweightTemplates {
		None = 0x0,	  
		GroupRow = 0x1,
		All = GroupRow
	}
	public class DefaultDataTemplate : DataTemplate {
	}
	public class DefaultControlTemplate : ControlTemplate {
	}
}
namespace DevExpress.Xpf.Grid.Native {
	partial class TableViewBehavior : IFormatConditionCollectionOwner, INotifyPropertyChanged {
		#region lightweight templates
#if DEBUGTEST
		internal 
#endif
		static UseLightweightTemplates? DefaultUseLightweightTemplates = null;
		internal static DependencyProperty RegisterUseLightweightTemplatesProperty(Type ownerType) {
			return DependencyProperty.Register("UseLightweightTemplates", typeof(UseLightweightTemplates?), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.OnUseLightweightTemplatesChanged()));
		}
		internal static DependencyProperty RegisterRowDetailsTemplateProperty(Type ownerType) {
			return DependencyProperty.Register("RowDetailsTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.UpdateActualRowDetailsTemplateSelector()));
		}
		internal static DependencyProperty RegisterRowDetailsTemplateSelectorProperty(Type ownerType) {
			return DependencyProperty.Register("RowDetailsTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.UpdateActualRowDetailsTemplateSelector()));
		}
		internal static DependencyPropertyKey RegisterActualRowDetailsTemplateSelectorProperty(Type ownerType) {
			return DependencyProperty.RegisterReadOnly("ActualRowDetailsTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((ITableView)d).TableViewBehavior.UpdateDetails()));
		}
		internal static DependencyProperty RegisterRowDetailsVisibilityModeProperty(Type ownerType) {
			return DependencyProperty.Register("RowDetailsVisibilityMode", typeof(RowDetailsVisibilityMode), ownerType, new FrameworkPropertyMetadata(RowDetailsVisibilityMode.VisibleWhenFocused, (d, e) => ((ITableView)d).TableViewBehavior.UpdateDetails()));
		}
#if DEBUGTEST
		internal
#endif
		UseLightweightTemplates ActualUseLightweightTemplates {
			get { return ((ITableView)View).UseLightweightTemplates ?? ((ITableView)View.RootView).UseLightweightTemplates ?? DefaultUseLightweightTemplates ?? UseLightweightTemplates.All; }
		}
		internal bool canChangeUseLightweightTemplates = true;
		void ForbidChangeUseLightweightTemplatesProperty() {
			canChangeUseLightweightTemplates = false;
		}
		protected virtual void UpdateActualRowDetailsTemplateSelector() {
			View.UpdateActualTemplateSelector(TableView.ActualRowDetailsTemplateSelectorPropertyKey, TableView.RowDetailsTemplateSelector, TableView.RowDetailsTemplate);
		}
		protected internal virtual void OnCellItemsControlLoaded() {
		}
		void UpdateDetails() {
			UpdateViewRowData(x => x.UpdateDetails());
		}
		void OnUseLightweightTemplatesChanged() {
			if(!canChangeUseLightweightTemplates && !DataViewBase.DisableOptimizedModeVerification)
				throw new InvalidOperationException("Can't change the UseLightweightTemplates property after the GridControl has been initialized.");
			View.UpdateColumnsAppearance();
			UpdateActualDataRowTemplateSelector();
			ValidateRowStyle(TableView.RowStyle);
		}
		internal bool UseLightweightTemplatesHasFlag(UseLightweightTemplates flag) { return ActualUseLightweightTemplates.HasFlag(flag); }
		internal FrameworkElement CreateElement(Func<FrameworkElement> lightweightDelegate, Func<FrameworkElement> ordinaryDelegate, UseLightweightTemplates flag) {
			if(canChangeUseLightweightTemplates) {
				ForbidChangeUseLightweightTemplatesProperty();
				ValidateRowStyle(TableView.RowStyle);
			}
			if(UseLightweightTemplatesHasFlag(flag))
				return lightweightDelegate();
			else
				return ordinaryDelegate();
		}
		internal virtual bool UseDataRowTemplate(RowData rowData) {
			if(!IsNullOrDefaultTemplate(TableView.DataRowTemplate))
				return true;
			return TableView.DataRowTemplateSelector != null && !IsNullOrDefaultTemplate(TableView.DataRowTemplateSelector.SelectTemplate(rowData, null));
		}
		bool IsNullOrDefaultTemplate(DataTemplate template) {
			return template == null || template is DefaultDataTemplate;
		}
		internal override bool UseRowDetailsTemplate(int rowHandle) {
			if(TableView.RowDetailsTemplate == null && TableView.RowDetailsTemplateSelector == null) return false;
			if(TableView.RowDetailsVisibilityMode == RowDetailsVisibilityMode.Collapsed) return false;
			return TableView.RowDetailsVisibilityMode == RowDetailsVisibilityMode.Visible || rowHandle == View.FocusedRowHandle;
		}
		#endregion
		#region Format conditions
		static FormatConditionCollection GetFormatConditions(DataControlBase dataControl) {
			return ((TableViewBehavior)dataControl.DataView.ViewBehavior).FormatConditions;
		}
		FormatConditionCollection formatConditions;
		internal FormatConditionCollection FormatConditions { get { return formatConditions ?? (formatConditions = new FormatConditionCollection(this)); } }
		internal override IEnumerable<IColumnInfo> GetServiceUnboundColumns() {
			return FormatConditions.GetUnboundColumns();
		}
		ConditionalFormatSummaryInfo[] originalSummaries;
		ServiceSummaryItem[] convertedSummaries;
		internal override IEnumerable<ServiceSummaryItem> GetServiceSummaries() {
			if(originalSummaries != FormatConditions.Summaries) {
				originalSummaries = FormatConditions.Summaries;
				convertedSummaries = FormatConditions.Summaries
					.Select(x => ConditionalFormatSummaryInfoHelper.ToSummaryItem(x.SummaryType, x.FieldName, DataControl.DataProviderBase))
					.ToArray();
			}
			return convertedSummaries;
		}
		internal override void CopyToDetail(DataControlBase dataControl) {
			base.CopyToDetail(dataControl);
			CloneDetailHelper.CopyToCollection<FormatConditionBase>(FormatConditions, GetFormatConditions(dataControl));
		}
		void IFormatConditionCollectionOwner.OnFormatConditionCollectionChanged(FormatConditionChangeType changeType) {
			DataControl.Do(x => x.AttachToFormatConditions(changeType));
		}
		void IFormatConditionCollectionOwner.SyncFormatCondtitionPropertyWithDetails(FormatConditionBase item, DependencyPropertyChangedEventArgs e) {
			DataControl.Do(x => x.GetDataControlOriginationElement().NotifyPropertyChanged(x, e.Property,
				dc => CloneDetailHelper.SafeGetDependentCollectionItem<FormatConditionBase>(item, FormatConditions, GetFormatConditions(dc)),
				typeof(FormatConditionBase)));
		}
		void IFormatConditionCollectionOwner.SyncFormatCondtitionCollectionWithDetails(NotifyCollectionChangedEventArgs e) {
			DataControl.Do(x => x.GetDataControlOriginationElement().NotifyCollectionChanged(x,
				dc => GetFormatConditions(dc),
				item => CloneDetailHelper.CloneElement<FormatConditionBase>((FormatConditionBase)item),
			e));
		}
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { } remove { } } 
		#endregion
		#region Serialization
		protected internal virtual void OnDeserializeCreateFormatCondition(XtraCreateCollectionItemEventArgs e) {
			XtraPropertyInfo typeNamePropertyInfo = e.Item.ChildProperties["TypeName"];
			if(typeNamePropertyInfo == null)
				return;
			FormatConditionBase condition = CreateFormatCondition(typeNamePropertyInfo.Value.ToString());
			if(condition == null)
				return;
			condition.OnDeserializeStart();
			if(e.Item.ChildProperties["Format"] != null) {
				DependencyProperty formatProperty = condition.FormatPropertyForBinding;
				condition.SetValue(formatProperty, Activator.CreateInstance(formatProperty.PropertyType));
			}
			FormatConditions.Add(condition);
			e.CollectionItem = condition;
		}
		protected internal virtual void OnDeserializeFormatConditionsStart() {
			FormatConditions.BeginUpdate();
		}
		protected internal virtual void OnDeserializeFormatConditionsEnd() {
			foreach(FormatConditionBase condition in FormatConditions)
				condition.OnDeserializeEnd();
			FormatConditions.EndUpdate();
		}
		protected virtual FormatConditionBase CreateFormatCondition(string conditionTypeName) {
			var assembly = typeof(FormatConditionBase).Assembly;
			if(assembly == null) return null;
			Type type = assembly.GetType(String.Format("{0}.{1}", XmlNamespaceConstants.GridNamespace, conditionTypeName));
			if(type == null) return null;
			return Activator.CreateInstance(type) as FormatConditionBase;
		}
		#endregion
	}
}
