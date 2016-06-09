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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Forms;
using System.Globalization;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region PivotTableFiltersCommandBase
	public abstract class PivotTableFiltersCommandBase : PivotTablePopupMenuCommandBase {
		protected PivotTableFiltersCommandBase(ISpreadsheetControl control) 
			: base(control) { 
		}
		#region Properties
		protected WorkbookDataContext Context { get { return ActiveSheet.DataContext; } }
		protected CultureInfo Culture { get { return Context.Culture; } }
		protected abstract PivotFilterType FilterType { get; }
		#endregion
		protected internal override void ExecuteCore() {
			CreateAddFilterCommand().Execute();
		}
		protected override bool IsChecked() {
			PivotFilter filter = GetFilter();
			return filter != null && IsCheckedCore(filter.FilterType);
		}
		protected virtual bool IsCheckedCore(PivotFilterType filterType) {
			return filterType == FilterType;
		}
		protected PivotAddFilterCommand CreateAddFilterCommand() {
			return CreateAddFilterCommand(FilterType);
		}
		protected PivotAddFilterCommand CreateAddFilterCommand(PivotFilterType filterType) {
			PivotAddFilterCommand command = new PivotAddFilterCommand(PivotTable, FieldIndex, filterType, Control.InnerControl.ErrorHandler);
			command.ShouldUnHideItems = true;
			return command;
		}
		protected virtual PivotFilter GetFilter() {
			return Info.LabelFilter;
		}
	}
	#endregion
	#region PivotTableCustomFiltersCommandBase
	public abstract class PivotTableCustomFiltersCommandBase<TViewModel> : PivotTableFiltersCommandBase where TViewModel : PivotTableFiltersViewModelBase {
		protected PivotTableCustomFiltersCommandBase(ISpreadsheetControl control) 
			: base(control) { 
		}
		protected internal override void ExecuteCore() {
			ShowForm(CreateViewModel());
		}
		protected abstract void ShowForm(TViewModel viewModel);
		protected abstract TViewModel CreateViewModelCore();
		protected TViewModel CreateViewModel() {
			TViewModel viewModel = CreateViewModelCore();
			viewModel.FilterType = FilterType;
			viewModel.FieldName = FieldName;
			viewModel.Command = this;
			PivotFilter filter = GetFilter();
			if (filter != null && filter.FilterType == FilterType)
				SetupViewModel(filter, viewModel);
			return viewModel;
		}
		protected abstract void SetupViewModel(PivotFilter filterColumn, TViewModel viewModel);
		public void ApplyChanges(TViewModel viewModel) {
			PivotAddFilterCommand command = CreateAddFilterCommand(viewModel.FilterType);
			command.FirstValue = viewModel.FirstValue;
			command.FirstValueIsDate = viewModel.FirstValueIsDate;
			if (viewModel.SecondValue != VariantValue.Missing) {
				command.SecondValue = viewModel.SecondValue;
				command.SecondValueIsDate = viewModel.SecondValueIsDate;
			}
			ApplyChangesCore(command, viewModel);
			command.Execute();
		}
		protected virtual void ApplyChangesCore(PivotAddFilterCommand command, TViewModel viewModel) {
		}
	}
	#endregion
}
