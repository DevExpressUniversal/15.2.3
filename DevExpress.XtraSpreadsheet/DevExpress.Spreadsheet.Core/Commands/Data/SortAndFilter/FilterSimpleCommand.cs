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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FilterSimpleCommand
	public class FilterSimpleCommand : ShowFilterFormCommandBase<SimpleFilterViewModel> {
		public FilterSimpleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterSimple; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterSimple; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterSimpleDescription; } }
		#endregion
		protected internal override void ExecuteCore() {
			Control.ShowSimpleFilterForm(CreateViewModel());
		}
		protected internal override SimpleFilterViewModel CreateViewModel() {
			SimpleFilterViewModel result = new SimpleFilterViewModel(Control);
			CellRange range = Accessor.GetSortOrFilterRange();
			AutoFilterBase autoFilter = Accessor.Filter;
			if (autoFilter != null) {
				AutoFilterColumn filterColumn = GetFilterColumn(autoFilter, range);
				result.SetupViewModel(autoFilter, filterColumn);
			}
			return result;
		}
		protected override void ModifyFilter(CellRange range, SimpleFilterViewModel viewModel) {
			DevExpress.Utils.Guard.ArgumentNotNull(Accessor.Filter, "filter is null");
			AutoFilterColumn filterColumn = GetFilterColumn(Accessor.Filter, range);
			filterColumn.Clear();
			viewModel.ModifyFilter(filterColumn);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive && !ActiveSheet.ReadOnly);
			ApplyActiveSheetProtection(state, !Protection.AutoFiltersLocked);
			state.Visible = IsVisible();
		}
		bool IsVisible() {
			DataSortOrFilterAccessor accessor = new DataSortOrFilterAccessor(DocumentModel);
			accessor.GetSortOrFilterRange();
			AutoFilterBase filter = accessor.Filter;
			return filter != null && filter.Range.Height > 1;
		}
		public override bool Validate(SimpleFilterViewModel viewModel) {
			foreach (FilterValueNode node in viewModel.Root.Children)
				if (node.IsChecked)
					return true;
			return false;
		}
	}
	#endregion
}
