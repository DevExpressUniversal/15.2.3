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
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	public abstract class FilterDynamicCommandBase : ShowCustomFilterFormCommandBase {
		protected FilterDynamicCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override GenericFilterOperator FilterOperator { get { return GenericFilterOperator.None; } }
		protected override bool IsDateTimeFilter { get { return true; } }
		protected abstract DynamicFilterType FilterType { get; }
		protected internal override void ExecuteCore() {
			ApplyChanges(CreateViewModel());
		}
		protected override void ModifyFilterColumn(AutoFilterColumn filterColumn, GenericFilterViewModel viewModel) {
			filterColumn.DynamicFilterType = FilterType;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Checked = (ObtainCurrentDynamicFilterType() == FilterType);
		}
		protected AutoFilterColumn ObtainCurrentAutoFilterColumn() {
			if (Accessor == null)
				return null;
			CellRange range = Accessor.GetSortOrFilterRange();
			if (range == null)
				return null;
			AutoFilterBase filter = Accessor.Filter;
			if (filter == null)
				return null;
			return GetFilterColumn(filter, range);
		}
		protected DynamicFilterType ObtainCurrentDynamicFilterType() {
			AutoFilterColumn filterColumn = ObtainCurrentAutoFilterColumn();
			if (filterColumn == null)
				return DynamicFilterType.Null;
			return filterColumn.DynamicFilterType;
		}
	}
}
