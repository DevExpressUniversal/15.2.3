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
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.XtraPivotGrid;
using System.ComponentModel;
namespace DevExpress.Xpf.PivotGrid {
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public class FilterPanelControl : FilterPanelControlBase {
		public FilterPanelControl() { }
		protected override void FilterPanelMRUComboBoxSelectedIndexChanged(object sender, RoutedEventArgs args) {
			CriteriaOperatorInfo selectedFilter = GetSelectedFilter();
			if(selectedFilter != null) {
				PivotGridControl pivot = this.DataContext as PivotGridControl;
				if(pivot != null)
					pivot.PrefilterCriteria = selectedFilter.FilterOperator;
			}
		}
	}
	public enum ShowPrefilterPanelMode {
		Default,
		ShowAlways,
		Never
	}
	class DisplayCriteriaHelper : IDisplayCriteriaGeneratorNamesSource {
		PivotGridControl pivotGrid;
		public DisplayCriteriaHelper(PivotGridControl pivotGrid) {
			this.pivotGrid = pivotGrid;
		}
		PivotGridControl PivotGrid {
			get { return pivotGrid; }
		}
		#region IDisplayCriteriaGeneratorNamesSource Members
		public string GetDisplayPropertyName(OperandProperty property) {
			PivotGridField pivotField = GetPivotGridFieldByFieldNameOrUnboundFieldNameOrName(property);
			if(pivotField != null) {
				return pivotField.DisplayText;
			} else {
				return PivotGridFieldBase.InvalidPropertyDisplayText;
			}
		}
		public string GetValueScreenText(OperandProperty property, object value) {
			if(value as FilterItem != null)
				return value.ToString();
			PivotGridFieldBase field = PivotGrid.Data.GetFieldByNameOrDataControllerColumnName(property.PropertyName);
			if(field == null || value == null) {
				return value != null ? value.ToString() : string.Empty;
			} else {
				return field.GetValueText(value);
			}
		}
		#endregion
		PivotGridField GetPivotGridFieldByFieldNameOrUnboundFieldNameOrName(OperandProperty property) {
			PivotGridFieldBase field = PivotGrid.Data.GetFieldByNameOrDataControllerColumnName(property.PropertyName);
			return field != null ? field.GetWrapper() : null;
		}
	}
}
namespace DevExpress.Xpf.PivotGrid.Internal {
	[DXToolboxBrowsable(false)]
	public class PivotFilterControl : FilterControl {
		public PivotFilterControl() : base() { }
		protected override string GetDefaultColumnCaption(IClauseNode node) {
			return PivotGridFieldBase.InvalidPropertyDisplayText;
		}
	}
}
