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
using System.ComponentModel;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Data;
using System.Windows;
namespace DevExpress.Xpf.PivotGrid {
	public static class PivotGridCommands {
		static readonly RoutedCommand changeFieldValueExpanded = new RoutedCommand("ChangeFieldValueExpanded", typeof(PivotGridCommands));
		static readonly RoutedCommand collapseField = new RoutedCommand("CollapseField", typeof(PivotGridCommands));
		static readonly RoutedCommand expandField = new RoutedCommand("ExpandField", typeof(PivotGridCommands));
		static readonly RoutedCommand changeFieldSortOrder = new RoutedCommand("ChangeColumnSortOrder", typeof(PivotGridCommands));
		static readonly RoutedCommand showFieldList = new RoutedCommand("ShowFieldList", typeof(PivotGridCommands));
		static readonly RoutedCommand hideFieldList = new RoutedCommand("HideFieldList", typeof(PivotGridCommands));
		static readonly RoutedCommand reloadData = new RoutedCommand("ReloadData", typeof(PivotGridCommands));
		static readonly RoutedCommand hideField = new RoutedCommand("HideField", typeof(PivotGridCommands));
		static readonly RoutedCommand showUnboundExpressionEditor = new RoutedCommand("ShowUnboundExpressionEditor", typeof(PivotGridCommands));
		static readonly RoutedCommand showPrefilter = new RoutedCommand("ShowPrefilter", typeof(PivotGridCommands));
		static readonly RoutedCommand hidePrefilter = new RoutedCommand("HidePrefilter", typeof(PivotGridCommands));
		static readonly RoutedCommand resetPrefilter = new RoutedCommand("ResetPrefilter", typeof(PivotGridCommands));
		static readonly RoutedCommand showPrintPreview = new RoutedCommand("ShowPrintPreview", typeof(PivotGridCommands));
		static readonly RoutedCommand showPrintPreviewDialog = new RoutedCommand("ShowPrintPreviewDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand sortAscending = new RoutedCommand("SortAscending", typeof(PivotGridCommands));
		static readonly RoutedCommand sortDescending = new RoutedCommand("SortDescending", typeof(PivotGridCommands));
		static readonly RoutedCommand clearSorting = new RoutedCommand("ClearSorting", typeof(PivotGridCommands));
		static readonly RoutedCommand showLessThanFormatConditionDialog = new RoutedCommand("ShowLessThanFormatConditionDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand showGreaterThanFormatConditionDialog = new RoutedCommand("ShowGreaterThanFormatConditionDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand showEqualToFormatConditionDialog = new RoutedCommand("ShowEqualToFormatConditionDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand showBetweenFormatConditionDialog = new RoutedCommand("ShowBetweenFormatConditionDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand showTextThatContainsFormatConditionDialog = new RoutedCommand("ShowTextThatContainsFormatConditionDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand showADateOccurringFormatConditionDialog = new RoutedCommand("ShowADateOccurringFormatConditionDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand showCustomConditionFormatConditionDialog = new RoutedCommand("ShowCustomConditionFormatConditionDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand showTop10ItemsFormatConditionDialog = new RoutedCommand("ShowTop10ItemsFormatConditionDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand showBottom10ItemsFormatConditionDialog = new RoutedCommand("ShowBottom10ItemsFormatConditionDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand showTop10PercentFormatConditionDialog = new RoutedCommand("ShowTop10PercentFormatConditionDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand showBottom10PercentFormatConditionDialog = new RoutedCommand("ShowBottom10PercentFormatConditionDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand showAboveAverageFormatConditionDialog = new RoutedCommand("ShowAboveAverageFormatConditionDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand showBelowAverageFormatConditionDialog = new RoutedCommand("ShowBelowAverageFormatConditionDialog", typeof(PivotGridCommands));
		static readonly RoutedCommand clearFormatConditionsFromAllMeasures = new RoutedCommand("ClearFormatConditionsFromAllMeasures", typeof(PivotGridCommands));
		static readonly RoutedCommand clearFormatConditionsFromMeasure = new RoutedCommand("ClearFormatConditionsFromMeasure", typeof(PivotGridCommands));
		static readonly RoutedCommand showConditionalFormattingManager = new RoutedCommand("ShowConditionalFormattingManager", typeof(PivotGridCommands));
		static readonly RoutedCommand addFormatCondition = new RoutedCommand("AddFormatCondition", typeof(PivotGridCommands));
		static readonly RoutedCommand clearFormatConditionsFromIntersection = new RoutedCommand("ClearFormatConditionsFromIntersection", typeof(PivotGridCommands));
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsChangeFieldValueExpanded")]
#endif
		public static RoutedCommand ChangeFieldValueExpanded { get { return changeFieldValueExpanded; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsCollapseField")]
#endif
		public static RoutedCommand CollapseField { get { return collapseField; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsExpandField")]
#endif
		public static RoutedCommand ExpandField { get { return expandField; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsChangeFieldSortOrder")]
#endif
		public static RoutedCommand ChangeFieldSortOrder { get { return changeFieldSortOrder; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsShowFieldList")]
#endif
		public static RoutedCommand ShowFieldList { get { return showFieldList; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsHideFieldList")]
#endif
		public static RoutedCommand HideFieldList { get { return hideFieldList; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsReloadData")]
#endif
		public static RoutedCommand ReloadData { get { return reloadData; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsHideField")]
#endif
		public static RoutedCommand HideField { get { return hideField; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsShowUnboundExpressionEditor")]
#endif
		public static RoutedCommand ShowUnboundExpressionEditor { get { return showUnboundExpressionEditor; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsShowPrefilter")]
#endif
		public static RoutedCommand ShowPrefilter { get { return showPrefilter; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsHidePrefilter")]
#endif
		public static RoutedCommand HidePrefilter { get { return hidePrefilter; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsResetPrefilter")]
#endif
		public static RoutedCommand ResetPrefilter { get { return resetPrefilter; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsShowPrintPreview")]
#endif
		public static RoutedCommand ShowPrintPreview { get { return showPrintPreview; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsShowPrintPreviewDialog")]
#endif
		public static RoutedCommand ShowPrintPreviewDialog { get { return showPrintPreviewDialog; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsSortAscending")]
#endif
		public static RoutedCommand SortAscending { get { return sortAscending; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsSortDescending")]
#endif
		public static RoutedCommand SortDescending { get { return sortDescending; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCommandsClearSorting")]
#endif
		public static RoutedCommand ClearSorting { get { return clearSorting; } }
		public static RoutedCommand ShowLessThanFormatConditionDialog { get { return showLessThanFormatConditionDialog; } }
		public static RoutedCommand ShowGreaterThanFormatConditionDialog { get { return showGreaterThanFormatConditionDialog; } }
		public static RoutedCommand ShowEqualToFormatConditionDialog { get { return showEqualToFormatConditionDialog; } }
		public static RoutedCommand ShowBetweenFormatConditionDialog { get { return showBetweenFormatConditionDialog; } }
		public static RoutedCommand ShowTextThatContainsFormatConditionDialog { get { return showTextThatContainsFormatConditionDialog; } }
		public static RoutedCommand ShowADateOccurringFormatConditionDialog { get { return showADateOccurringFormatConditionDialog; } }
		public static RoutedCommand ShowCustomConditionFormatConditionDialog { get { return showCustomConditionFormatConditionDialog; } }
		public static RoutedCommand ShowTop10ItemsFormatConditionDialog { get { return showTop10ItemsFormatConditionDialog; } }
		public static RoutedCommand ShowBottom10ItemsFormatConditionDialog { get { return showBottom10ItemsFormatConditionDialog; } }
		public static RoutedCommand ShowTop10PercentFormatConditionDialog { get { return showTop10PercentFormatConditionDialog; } }
		public static RoutedCommand ShowBottom10PercentFormatConditionDialog { get { return showBottom10PercentFormatConditionDialog; } }
		public static RoutedCommand ShowAboveAverageFormatConditionDialog { get { return showAboveAverageFormatConditionDialog; } }
		public static RoutedCommand ShowBelowAverageFormatConditionDialog { get { return showBelowAverageFormatConditionDialog; } }
		public static RoutedCommand ClearFormatConditionsFromAllMeasures { get { return clearFormatConditionsFromAllMeasures; } }
		public static RoutedCommand ClearFormatConditionsFromMeasure { get { return clearFormatConditionsFromMeasure; } }
		public static RoutedCommand ShowConditionalFormattingManager { get { return showConditionalFormattingManager; } }
		public static RoutedCommand AddFormatCondition { get { return addFormatCondition; } }
		public static RoutedCommand ClearFormatConditionsFromIntersection { get { return clearFormatConditionsFromIntersection; } }
	}
	public class FormatConditionCommandParameters : DependencyObject, IDataColumnInfo {
		bool isManagerRule; 
		internal FormatConditionCommandParameters(bool isManagerRule) {
			this.isManagerRule = isManagerRule;
		}
		public FormatConditionCommandParameters() {
			this.isManagerRule = false;
		}
		internal bool IsManagerRule { get { return isManagerRule; } }
		public PivotGridField Measure {
			get { return (PivotGridField)GetValue(MeasureProperty); }
			set { SetValue(MeasureProperty, value); }
		}
		public static readonly DependencyProperty MeasureProperty =
			DependencyProperty.Register("Measure", typeof(PivotGridField), typeof(FormatConditionCommandParameters), new UIPropertyMetadata(null));
		public PivotGridField Row {
			get { return (PivotGridField)GetValue(RowProperty); }
			set { SetValue(RowProperty, value); }
		}
		public static readonly DependencyProperty RowProperty =
			DependencyProperty.Register("Row", typeof(PivotGridField), typeof(FormatConditionCommandParameters), new UIPropertyMetadata(null));
		public PivotGridField Column {
			get { return (PivotGridField)GetValue(ColumnProperty); }
			set { SetValue(ColumnProperty, value); }
		}
		public static readonly DependencyProperty ColumnProperty =
			DependencyProperty.Register("Column", typeof(PivotGridField), typeof(FormatConditionCommandParameters), new UIPropertyMetadata(null));
		string IDataColumnInfo.Caption {
			get { return Measure.Caption; }
		}
		System.Collections.Generic.List<IDataColumnInfo> IDataColumnInfo.Columns {
			get { throw new NotImplementedException(); }
		}
		DataControllerBase IDataColumnInfo.Controller {
			get { return null; }
		}
		string IDataColumnInfo.FieldName {
			get { return Measure.Name; }
		}
		Type IDataColumnInfo.FieldType {
			get { return Measure.DataType; }
		}
		string IDataColumnInfo.Name {
			get { return Measure.Name; }
		}
		string IDataColumnInfo.UnboundExpression {
			get { return Measure.UnboundExpression; }
		}
	}
}
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class PivotGridCommandsWrapper {
		public RoutedCommand ChangeFieldValueExpanded { get { return PivotGridCommands.ChangeFieldValueExpanded; } }
		public RoutedCommand CollapseField { get { return PivotGridCommands.CollapseField; } }
		public RoutedCommand ExpandField { get { return PivotGridCommands.ExpandField; } }
		public RoutedCommand ChangeFieldSortOrder { get { return PivotGridCommands.ChangeFieldSortOrder; } }
		public RoutedCommand ShowFieldList { get { return PivotGridCommands.ShowFieldList; } }
		public RoutedCommand HideFieldList { get { return PivotGridCommands.HideFieldList; } }
		public RoutedCommand ReloadData { get { return PivotGridCommands.ReloadData; } }
		public RoutedCommand HideField { get { return PivotGridCommands.HideField; } }
		public RoutedCommand ShowUnboundExpressionEditor { get { return PivotGridCommands.ShowUnboundExpressionEditor; } }
		public RoutedCommand ShowPrefilter { get { return PivotGridCommands.ShowPrefilter; } }
		public RoutedCommand HidePrefilter { get { return PivotGridCommands.HidePrefilter; } }
		public RoutedCommand ResetPrefilter { get { return PivotGridCommands.ResetPrefilter; } }
		public RoutedCommand SortAscending { get { return PivotGridCommands.SortAscending; } }
		public RoutedCommand SortDescending { get { return PivotGridCommands.SortDescending; } }
		public RoutedCommand ClearSorting { get { return PivotGridCommands.ClearSorting; } }
		public ICommand ShowLessThanFormatConditionDialog { get; private set; }
		public ICommand ShowGreaterThanFormatConditionDialog { get; private set; }
		public ICommand ShowEqualToFormatConditionDialog { get; private set; }
		public ICommand ShowBetweenFormatConditionDialog { get; private set; }
		public ICommand ShowTextThatContainsFormatConditionDialog { get; private set; }
		public ICommand ShowADateOccurringFormatConditionDialog { get; private set; }
		public ICommand ShowCustomConditionFormatConditionDialog { get; private set; }
		public ICommand ShowTop10ItemsFormatConditionDialog { get; private set; }
		public ICommand ShowBottom10ItemsFormatConditionDialog { get; private set; }
		public ICommand ShowTop10PercentFormatConditionDialog { get; private set; }
		public ICommand ShowBottom10PercentFormatConditionDialog { get; private set; }
		public ICommand ShowAboveAverageFormatConditionDialog { get; private set; }
		public ICommand ShowBelowAverageFormatConditionDialog { get; private set; }
		public ICommand ClearFormatConditionsFromAllMeasures { get; private set; }
		public ICommand ClearFormatConditionsFromMeasure { get; private set; }
		public ICommand ShowConditionalFormattingManager { get; private set; }
		public ICommand AddFormatCondition { get; private set; }
		public ICommand ClearFormatConditionsFromIntersection { get; private set; }
		PivotGridControl pivotGrid;
		public PivotGridCommandsWrapper(PivotGridControl pivotGrid) {
			this.pivotGrid = pivotGrid;
			ShowLessThanFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.LessThan));
			ShowGreaterThanFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.GreaterThan));
			ShowEqualToFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.EqualTo));
			ShowBetweenFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Between));
			ShowTextThatContainsFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.TextThatContains));
			ShowADateOccurringFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.ADateOccurring));
			ShowCustomConditionFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.CustomCondition));
			ShowTop10ItemsFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Top10Items));
			ShowBottom10ItemsFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Bottom10Items));
			ShowTop10PercentFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Top10Percent));
			ShowBottom10PercentFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Bottom10Percent));
			ShowAboveAverageFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.AboveAverage));
			ShowBelowAverageFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.BelowAverage));
			ClearFormatConditionsFromAllMeasures = CreateDelegateCommand(x => pivotGrid.ClearFormatConditionsFromAllMeasures());
			ClearFormatConditionsFromMeasure = CreateDelegateCommand(x => pivotGrid.ClearFormatConditionsFromMeasure(GetField(x)));
			ShowConditionalFormattingManager = CreateDelegateCommand(x => pivotGrid.ShowConditionalFormattingManager(GetField(x)));
			AddFormatCondition = CreateDelegateCommand(x => pivotGrid.AddFormatCondition((FormatConditionBase)x));
			ClearFormatConditionsFromIntersection = CreateDelegateCommand(x => {
				FormatConditionCommandParameters item = (FormatConditionCommandParameters)x;
				pivotGrid.ClearFormatConditionsFromIntersection(item.Row, item.Column);
			});
		}
		protected DelegateCommand<object> CreateDelegateCommand(Action<object> executeMethod) {
			return CreateDelegateCommand(executeMethod, null);
		}
		protected DelegateCommand<object> CreateDelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod) {
			return CreateDelegateCommand<object>(executeMethod, canExecuteMethod);
		}
		protected DelegateCommand<T> CreateDelegateCommand<T>(Action<T> executeMethod, Func<T, bool> canExecuteMethod) {
			return DelegateCommandFactory.Create<T>(executeMethod, canExecuteMethod, false);
		}
		void ShowFormatConditionDialog(object field, FormatConditionDialogType dialogKind) {
			FormatConditionCommandParameters settings = (FormatConditionCommandParameters)field;
			pivotGrid.ShowFormatConditionDialog(settings.Measure, settings.Row, settings.Column, dialogKind);
		}
		static PivotGridField GetField(object field) {
			FormatConditionCommandParameters set = field as FormatConditionCommandParameters;
			if(set != null)
				return set.Measure;
			return field as PivotGridField ?? ((PivotGridInternalField)field).Wrapper;
		}
	}
	interface IPivotConditionalFormattingCommands : IConditionalFormattingCommands {
		ICommand ClearFormatConditionsFromIntersection { get; }
	}
	public class FormatConditionsCommands : IPivotConditionalFormattingCommands {
		PivotGridControl pivotGrid;
		public FormatConditionsCommands(PivotGridControl pivotGrid) {
			this.pivotGrid = pivotGrid;
		}
		ICommand IConditionalFormattingCommands.AddFormatCondition {
			get { return pivotGrid.Commands.AddFormatCondition; }
		}
		ICommand IConditionalFormattingCommands.ClearFormatConditionsFromAllColumns {
			get { return pivotGrid.Commands.ClearFormatConditionsFromAllMeasures; }
		}
		ICommand IConditionalFormattingCommands.ClearFormatConditionsFromColumn {
			get { return pivotGrid.Commands.ClearFormatConditionsFromMeasure; }
		}
		ICommand IPivotConditionalFormattingCommands.ClearFormatConditionsFromIntersection {
			get { return pivotGrid.Commands.ClearFormatConditionsFromIntersection; }
		}
		ICommand IConditionalFormattingCommands.ShowADateOccurringFormatConditionDialog {
			get { return pivotGrid.Commands.ShowADateOccurringFormatConditionDialog; }
		}
		ICommand IConditionalFormattingCommands.ShowAboveAverageFormatConditionDialog {
			get { return pivotGrid.Commands.ShowAboveAverageFormatConditionDialog; }
		}
		ICommand IConditionalFormattingCommands.ShowBelowAverageFormatConditionDialog {
			get { return pivotGrid.Commands.ShowBelowAverageFormatConditionDialog; }
		}
		ICommand IConditionalFormattingCommands.ShowBetweenFormatConditionDialog {
			get { return pivotGrid.Commands.ShowBetweenFormatConditionDialog; }
		}
		ICommand IConditionalFormattingCommands.ShowBottom10ItemsFormatConditionDialog {
			get { return pivotGrid.Commands.ShowBottom10ItemsFormatConditionDialog; }
		}
		ICommand IConditionalFormattingCommands.ShowBottom10PercentFormatConditionDialog {
			get { return pivotGrid.Commands.ShowBottom10PercentFormatConditionDialog; }
		}
		ICommand IConditionalFormattingCommands.ShowConditionalFormattingManager {
			get { return pivotGrid.Commands.ShowConditionalFormattingManager; }
		}
		ICommand IConditionalFormattingCommands.ShowCustomConditionFormatConditionDialog {
			get { return pivotGrid.Commands.ShowCustomConditionFormatConditionDialog; }
		}
		ICommand IConditionalFormattingCommands.ShowEqualToFormatConditionDialog {
			get { return pivotGrid.Commands.ShowEqualToFormatConditionDialog; }
		}
		ICommand IConditionalFormattingCommands.ShowGreaterThanFormatConditionDialog {
			get { return pivotGrid.Commands.ShowGreaterThanFormatConditionDialog; }
		}
		ICommand IConditionalFormattingCommands.ShowLessThanFormatConditionDialog {
			get { return pivotGrid.Commands.ShowLessThanFormatConditionDialog; }
		}
		ICommand IConditionalFormattingCommands.ShowTextThatContainsFormatConditionDialog {
			get { return pivotGrid.Commands.ShowTextThatContainsFormatConditionDialog; }
		}
		ICommand IConditionalFormattingCommands.ShowTop10ItemsFormatConditionDialog {
			get { return pivotGrid.Commands.ShowTop10ItemsFormatConditionDialog; }
		}
		ICommand IConditionalFormattingCommands.ShowTop10PercentFormatConditionDialog {
			get { return pivotGrid.Commands.ShowTop10PercentFormatConditionDialog; }
		}
	}
}
