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
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet {
	#region PivotItemType
	public enum PivotItemType {
		Blank = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Blank,
		Data = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Data,
		Default = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.DefaultValue,
		Sum = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Sum,
		Count = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.CountA,
		Average = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Avg,
		Max = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Max,
		Min = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Min,
		Product = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Product,
		CountNumbers = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Count,
		StdDev = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.StdDev,
		StdDevp = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.StdDevP,
		Var = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Var,
		Varp = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.VarP,
		Grand = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Grand
	}
	#endregion
	#region PivotItem
	public interface PivotItem {
		string Caption { get; set; }
		CellValue Value { get; }
		bool ShowDetails { get; set; }
		bool Visible { get; set; }
		void MoveUp();
		void MoveDown();
		void MoveToBeginning();
		void MoveToEnd();
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using System.Diagnostics;
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	partial class NativePivotItem : NativeObjectBase, PivotItem {
		#region Fields
		readonly Model.PivotItem modelItem;
		readonly NativePivotField nativeField;
		int itemIndex;
		#endregion
		public NativePivotItem(Model.PivotItem modelItem, NativePivotField nativeField) {
			Guard.ArgumentNotNull(modelItem, "modelItem");
			Guard.ArgumentNotNull(nativeField, "nativeField");
			this.modelItem = modelItem;
			this.nativeField = nativeField;
		}
		#region Properties
		protected internal NativePivotField Parent { get { return nativeField; } }
		protected internal int ItemIndex { get { return itemIndex; } set { itemIndex = value; } } 
		Model.WorkbookDataContext Context { get { return modelItem.DocumentModel.DataContext; } }
		Model.PivotTable ModelPivotTable { get { return modelItem.PivotTable; } }
		#region Caption
		public string Caption {
			get {
				CheckValid();
				return modelItem.ItemUserCaption;
			}
			set {
				CheckValid();
				modelItem.ItemUserCaption = value;
			}
		}
		#endregion
		#region Value
		public CellValue Value {
			get {
				CheckValid();
				Debug.Assert(modelItem.ItemType == Model.PivotFieldItemType.Data);
				Model.IPivotCacheRecordValue modelSharedRecord = nativeField.CacheField.SharedItems[modelItem.ItemIndex];
				Model.VariantValue modelValue = modelSharedRecord.ToVariantValue(nativeField.CacheField, Context);
				if (modelValue.IsError)
					return CellValue.GetNativeErrorValue(modelValue.ErrorValue.Type);
				CellValue result = new CellValue(modelValue, Context);
				result.IsDateTime = modelSharedRecord.ValueType == Model.PivotCacheRecordValueType.DateTime;
				return result;
			}
		}
		#endregion
		#region ShowDetails
		public bool ShowDetails {
			get {
				CheckValid();
				return !modelItem.HideDetails;
			}
			set {
				CheckValid();
				ModelPivotTable.BeginTransaction(ApiErrorHandler.Instance);
				try {
					modelItem.HideDetails = !value;
				}
				finally {
					ModelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region Visible
		public bool Visible {
			get {
				CheckValid();
				return !modelItem.IsHidden;
			}
			set {
				CheckValid();
				if (modelItem.IsHidden == !value)
					return;
				Model.TogglePivotItemVisibilityCommand command = new Model.TogglePivotItemVisibilityCommand(nativeField.ModelItem, itemIndex, ApiErrorHandler.Instance);
				command.Execute();
			}
		}
		#endregion
		#endregion
		public void MoveUp() {
			MoveCore(Model.PivotItemMoveType.Up);
		}
		public void MoveDown() {
			MoveCore(Model.PivotItemMoveType.Down);
		}
		public void MoveToBeginning() {
			MoveCore(Model.PivotItemMoveType.ToBeginning);
		}
		public void MoveToEnd() {
			MoveCore(Model.PivotItemMoveType.ToEnd);
		}
		void MoveCore(Model.PivotItemMoveType moveType) {
			CheckValid();
			Model.MovePivotFieldItemCommand command = new Model.MovePivotFieldItemCommand(Parent.ModelItem, itemIndex, moveType, ApiErrorHandler.Instance);
			command.Execute();
		}
	}
}
