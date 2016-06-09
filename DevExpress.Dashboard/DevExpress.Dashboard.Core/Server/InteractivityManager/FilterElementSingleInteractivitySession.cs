#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.Native;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.Server {
	public class FilterElementSingleInteractivitySession : FilterElementInteractivitySession {
		IList<ISelectionRow> actualSelected;
		IList<ISelectionRow> internalSelected = new List<ISelectionRow>();
		protected override IEnumerable<ISelectionRow> ActualSelected {
			get {
				if (actualSelected == null) {
					actualSelected = AllSelectionValues.HashSetIntersect(internalSelected).ToList();
					bool isSelectionCorrect = actualSelected.Count == 1 || (ShowAllValues && actualSelected.Count == AllSelectionValues.Count());
					if (!isSelectionCorrect)
						actualSelected = (ShowAllValues ? AllSelectionValues : AllSelectionValues.Take(1)).ToList();
				}
				return actualSelected;
			}
			set {
				if (value == null)
					actualSelected = null;
				else
					internalSelected = value.ToList();
			}
		}
		protected override bool IsEmptyCriteria { get { return (ActualSelected.Count() == AllSelectionValues.Count()); } }
		bool ShowAllValues { get { return DashboardItem.ShowAllValue; } }
		public FilterElementSingleInteractivitySession(FilterElementDashboardItem dashboardItem)
			: base(dashboardItem) {
		}
		protected override bool IsValidSelection(IEnumerable<ISelectionRow> selection) {
			int selectedCount = selection.Count();
			int allSelectionCount = AllSelectionValues.Count();
			bool emptyData = selectedCount == 0 && allSelectionCount == 0;
			return selectedCount == 1 || emptyData || (ShowAllValues && selectedCount == allSelectionCount);
		}
		protected override IEnumerable<ISelectionRow> ValidateValues(IEnumerable<ISelectionRow> selection) {
			return ShowAllValues ? selection : selection.Take(1);
		}
		protected override void PrepareMasterFilterState(MasterFilterState state) {
			state.IsSelectedValues = true;
			if (internalSelected.Count() > 0)
				state.ValuesSet = internalSelected.AsValuesSet();
		}
		protected override void ApplyMasterFilterState(MasterFilterState state) {
			if (!state.IsSelectedValues)
				throw new ArgumentException("FilterElementSingleSelectionController can not apply state with unselected values.");
			internalSelected = state.ValuesSet.AsSelectionRows().ToList();
		}
	}
}
