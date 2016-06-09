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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Reports.UserDesigner.Editors.Native;
using DevExpress.XtraReports.UI;
using DevExpress.Mvvm.POCO;
using System.Collections.Generic;
using DevExpress.Xpf.DataAccess.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using System.Windows.Data;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using System.Windows.Markup;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class SelectionModelToIsEnabledConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value == null) return false;
			var selectionModel = ((XRDiagramContextMenuItemsData)value).Diagram.SelectionModel;
			return ((IMultiModel)selectionModel).PropertiesProvider.Values.Count() < 2;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public class SelectionModelToIsEnabledConverterExtension : MarkupExtension {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new SelectionModelToIsEnabledConverter();
		}
	}
	public class FormattingRulesUITypeEditor : SingleSelectionCollectionEditor {
		public static readonly DependencyProperty ReportModelProperty;
		public static readonly DependencyProperty SelectedFormattingRuleSheetItemProperty;
		static readonly DependencyPropertyKey FormattingRuleSheetPropertyKey;
		public static readonly DependencyProperty FormattingRuleSheetProperty;
		static FormattingRulesUITypeEditor() {
			DependencyPropertyRegistrator<FormattingRulesUITypeEditor>.New()
				.Register(d => d.ReportModel, out ReportModelProperty, null, d => d.OnReportModelChanged())
				.Register(d => d.SelectedFormattingRuleSheetItem, out SelectedFormattingRuleSheetItemProperty, null)
				.RegisterReadOnly(d => d.FormattingRuleSheet, out FormattingRuleSheetPropertyKey, out FormattingRuleSheetProperty, null)
				.OverrideDefaultStyleKey()
				;
		}
		readonly ICommand moveRightCommand;
		public ICommand MoveRightCommand { get { return moveRightCommand; } }
		readonly ICommand moveAllRightCommand;
		public ICommand MoveAllRightCommand { get { return moveAllRightCommand; } }
		readonly ICommand moveLeftCommand;
		public ICommand MoveLeftCommand { get { return moveLeftCommand; } }
		readonly ICommand moveAllLeftCommand;
		public ICommand MoveAllLeftCommand { get { return moveAllLeftCommand; } }
		public FormattingRulesUITypeEditor() {
			FormattingRuleSheet = new ObservableCollection<FormattingRule>();
			moveRightCommand = DelegateCommandFactory.Create(MoveRight, () => FormattingRuleSheet.Count > 0, true);
			moveAllRightCommand = DelegateCommandFactory.Create(MoveAllRight, () => FormattingRuleSheet.Count > 0, true);
			moveLeftCommand = DelegateCommandFactory.Create(MoveLeft, () => Items.Count > 0, true);
			moveAllLeftCommand = DelegateCommandFactory.Create(MoveAllLeft, () => Items.Count > 0, true);
		}
		public ObservableCollection<FormattingRule> FormattingRuleSheet {
			get { return (ObservableCollection<FormattingRule>)GetValue(FormattingRuleSheetProperty); }
			private set { SetValue(FormattingRuleSheetPropertyKey, value); }
		}
		public XtraReportModelBase ReportModel {
			get { return (XtraReportModelBase)GetValue(ReportModelProperty); }
			set { SetValue(ReportModelProperty, value); }
		}
		public FormattingRule SelectedFormattingRuleSheetItem {
			get { return (FormattingRule)GetValue(SelectedFormattingRuleSheetItemProperty); }
			set { SetValue(SelectedFormattingRuleSheetItemProperty, value); }
		}
		void OnReportModelChanged() {
			if(ReportModel == null) return;
			var report = (XtraReport)ReportModel.XRObjectBase;
			FormattingRuleSheet.Clear();
			foreach(var formattingRule in report.FormattingRuleSheet)
				if(!report.IsAttachedRule(formattingRule))
					FormattingRuleSheet.Add(formattingRule);
			SelectedFormattingRuleSheetItem = FormattingRuleSheet.FirstOrDefault();
		}
		void MoveRight() {
			if(SelectedFormattingRuleSheetItem == null) return;
			controller.Add(SelectedFormattingRuleSheetItem);
			FormattingRuleSheet.Remove(SelectedFormattingRuleSheetItem);
			if(SelectedItem == null) SelectedItem = Items[0];
			if(SelectedFormattingRuleSheetItem == null)
				SelectedFormattingRuleSheetItem = FormattingRuleSheet.FirstOrDefault();
		}
		void MoveLeft() {
			if(SelectedItem == null) return;
			AddFormattingRuleSheetItem((IMultiModel)SelectedItem);
			RemoveItem();
			if(SelectedFormattingRuleSheetItem == null)
				SelectedFormattingRuleSheetItem = FormattingRuleSheet.FirstOrDefault();
		}
		void MoveAllRight() {
			foreach(var formattingRule in FormattingRuleSheet)
				controller.Add(formattingRule);
			FormattingRuleSheet.Clear();
			SelectedItem = Items[0];
		}
		void MoveAllLeft() {
			foreach(IMultiModel item in Items)
				AddFormattingRuleSheetItem(item);
			Items.Clear();
			SelectedFormattingRuleSheetItem = FormattingRuleSheet.First();
		}
		void AddFormattingRuleSheetItem(IMultiModel item) {
			var propertiesProvider = (PropertiesProvider<IDiagramItem, FormattingRule>)item.PropertiesProvider;
			FormattingRuleSheet.Add(propertiesProvider.Context.GetComponent(propertiesProvider.MainComponent));
		}
		public override object CreateItem() {
			throw new NotSupportedException();
		}
	}
}
