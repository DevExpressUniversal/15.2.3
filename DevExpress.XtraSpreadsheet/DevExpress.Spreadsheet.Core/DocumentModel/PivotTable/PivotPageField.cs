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
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotPageFieldCollection
	public class PivotPageFieldCollection : PivotFieldReferenceCollection<PivotPageField> {
		public PivotPageFieldCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		public void CopyFromNoHistory(PivotTable newPivot, PivotPageFieldCollection source) {
			ClearCore();
			Capacity = source.Count;
			foreach (PivotPageField item in source) {
				PivotPageField newItem = new PivotPageField(newPivot, item.FieldIndex);
				newItem.CopyFromNoHistory(item);
				AddCore(newItem);
			}
		}
	}
	#endregion
	#region PivotPageField
	public class PivotPageField : PivotFieldReference {
		readonly PivotTable pivotTable;
		string hDisplayName;
		string hUniqueName;
		int hierarchyIndex;
		int itemIndex;
		public PivotPageField(PivotTable pivotTable, int fieldIndex)
			: base(fieldIndex) {
			this.pivotTable = pivotTable;
			this.itemIndex = -1;
		}
		#region Properties
		public DocumentModel DocumentModel { get { return pivotTable.DocumentModel; } }
		public PivotTable PivotTable { get { return pivotTable; } }
		#region ItemIndex
		public int ItemIndex {
			get { return itemIndex; }
			set {
				if (itemIndex != value) {
					pivotTable.CheckActiveTransaction();
					SetItemIndex(value);
				}
			}
		}
		protected internal void SetItemIndex(int value) {
			HistoryHelper.SetValue(DocumentModel, itemIndex, value, SetItemIndexCore);
		}
		protected internal void SetItemIndexCore(int value) {
			this.itemIndex = value;
			pivotTable.CalculationInfo.InvalidateLayout();
		}
		#endregion
		#region HierarchyIndex
		public int HierarchyIndex {
			get { return hierarchyIndex; }
			set {
				if (hierarchyIndex != value) {
					pivotTable.CheckActiveTransaction();
					SetHierarchyIndex(value);
				}
			}
		}
		protected internal void SetHierarchyIndex(int value) {
			HistoryHelper.SetValue(DocumentModel, hierarchyIndex, value, SetHierarchyIndexCore);
		}
		protected internal void SetHierarchyIndexCore(int value) {
			this.hierarchyIndex = value;
			pivotTable.CalculationInfo.InvalidateLayout();
		}
		#endregion
		#region HierarchyDisplayName
		public string HierarchyDisplayName {
			get { return hDisplayName; }
			set {
				if (StringExtensions.CompareInvariantCultureIgnoreCase(hDisplayName, value) != 0) {
					pivotTable.CheckActiveTransaction();
					SetHierarchyDisplayName(value);
				}
			}
		}
		protected internal void SetHierarchyDisplayName(string value) {
			HistoryHelper.SetValue(DocumentModel, hDisplayName, value, StringExtensions.ComparerInvariantCultureIgnoreCase, SetHierarchyDisplayNameCore);
		}
		protected internal void SetHierarchyDisplayNameCore(string value) {
			this.hDisplayName = value;
			pivotTable.CalculationInfo.InvalidateWorksheetData();
		}
		#endregion
		#region HierarchyUniqueName
		public string HierarchyUniqueName {
			get { return hUniqueName; }
			set {
				if (StringExtensions.CompareInvariantCultureIgnoreCase(hUniqueName, value) != 0) {
					pivotTable.CheckActiveTransaction();
					SetHierarchyUniqueName(value);
				}
			}
		}
		protected internal void SetHierarchyUniqueName(string value) {
			HistoryHelper.SetValue(DocumentModel, hUniqueName, value, StringExtensions.ComparerInvariantCultureIgnoreCase, SetHierarchyUniqueNameCore);
		}
		protected internal void SetHierarchyUniqueNameCore(string value) {
			this.hUniqueName = value;
			pivotTable.CalculationInfo.InvalidateWorksheetData();
		}
		#endregion
		#endregion
		public void CopyFrom(PivotPageField sourceItem) {
			this.hDisplayName = sourceItem.hDisplayName;
			this.hierarchyIndex = sourceItem.hierarchyIndex;
			this.hUniqueName= sourceItem.hUniqueName;
			this.itemIndex = sourceItem.itemIndex;
		}
		public void CopyFromNoHistory(PivotPageField source) {
			Debug.Assert(FieldIndex == source.FieldIndex);
			hDisplayName = source.hDisplayName;
			hUniqueName = source.hUniqueName;
			hierarchyIndex = source.hierarchyIndex;
			itemIndex = source.itemIndex;
		}
	}
	#endregion
}
