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
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region IPivotStyleOwner
	public interface IPivotStyleOwner {
		IPivotStyleOptions Options { get; }
		IPivotTableLocation Location { get; }
		PivotStyleFormatCache StyleFormatCache { get; }
		PivotZoneFormattingCache PivotZoneCache { get; set; }
		int ColumnFieldsCount { get; }
		int RowFieldsCount { get; }
		int DataFieldsCount { get; }
		int PageFieldsCount { get; }
		void CacheColumnItemStripeInfo(int columnItemIndex, TableStyleStripeInfo info);
		TableStyleStripeInfo GetColumnItemStripeInfo(int columnItemIndex);
		void CacheRowItemStripeInfo(int rowItemIndex, TableStyleStripeInfo info);
		TableStyleStripeInfo GetRowItemStripeInfo(int rowItemIndex);
		int GetDataColumnItemIndex(int absoluteDataIndex);
		int GetDataRowItemIndex(int absoluteDataIndex);
		IPivotZone GetPivotZoneByCellPosition(CellPosition cellPosition);
	}
	#endregion
	#region IPivotStyleOptions
	public interface IPivotStyleOptions {
		bool ShowColumnHeaders { get; }
		bool ShowColumnStripes { get; }
		bool ShowRowHeaders { get; }
		bool ShowRowStripes { get; }
		bool ShowLastColumn { get; }
		bool HasColumnGrandTotals { get; }
		bool HasRowGrandTotals { get; }
	}
	#endregion
	#region PivotTableStyleInfo
	public class PivotTableStyleInfo : IPivotStyleOptions {
		internal static byte DefaultPackedValues = MaskShowColumnHeaders + MaskShowRowHeaders + MaskShowLastColumn + MaskHasShowLastColumn;
		internal static string DefaultStyleName = TableStyleName.DefaultStyleName.Name;
		#region Fields
		const byte MaskShowColumnHeaders = 1;
		const byte MaskShowColumnStripes = 2;
		const byte MaskShowLastColumn = 4;
		const byte MaskShowRowHeaders = 8;
		const byte MaskShowRowStripes = 0x10;
		const byte MaskHasShowLastColumn = 0x20;
		readonly PivotTable table;
		string styleName = DefaultStyleName;
		byte packedValues = DefaultPackedValues;
		#endregion
		public PivotTableStyleInfo(PivotTable table) {
			Guard.ArgumentNotNull(table, "table");
			this.table = table;
		}
		#region Properties
		public PivotTable Table { get { return table; } }
		public bool ShowColumnHeaders { get { return GetBooleanValue(MaskShowColumnHeaders); } set { SetBooleanValue(MaskShowColumnHeaders, value); } }
		public bool ShowColumnStripes { get { return GetBooleanValue(MaskShowColumnStripes); } set { SetBooleanValue(MaskShowColumnStripes, value); } }
		public bool ShowRowHeaders { get { return GetBooleanValue(MaskShowRowHeaders); } set { SetBooleanValue(MaskShowRowHeaders, value); } }
		public bool ShowRowStripes { get { return GetBooleanValue(MaskShowRowStripes); } set { SetBooleanValue(MaskShowRowStripes, value); } }
		public bool ShowLastColumn {
			get { return GetBooleanValue(MaskShowLastColumn); }
			set {
				DocumentModel.History.BeginTransaction();
				try {
					SetBooleanValue(MaskShowLastColumn, value);
					HasShowLastColumn = true;
				} finally {
					DocumentModel.History.EndTransaction();
				}
			}
		}
		public bool HasShowLastColumn { get { return GetBooleanValue(MaskHasShowLastColumn); } set { SetBooleanValue(MaskHasShowLastColumn, value); } }
		public string StyleName {
			get { return styleName; }
			set {
				if (TableStyleName.CompareStrings(styleName, value))
					return;
				bool isEmptyValue = String.IsNullOrEmpty(value);
				if (isEmptyValue && TableStyleName.CheckDefaultStyleName(styleName))
					return;
				string newValue = isEmptyValue ? DefaultStyleName : value;
				ApplyHistoryItem(new ChangePivotTableStyleInfoStyleNameHistoryItem(this, styleName, newValue));
			}
		}
		public bool ApplyTableStyle { get { return !TableStyleName.CheckDefaultStyleName(styleName) && TableStyles.ContainsStyleName(styleName); } }
		public TableStyle Style { get { return ApplyTableStyle ? TableStyles[styleName] : null; } }
		DocumentModel DocumentModel { get { return table.DocumentModel; } }
		TableStyleCollection TableStyles { get { return DocumentModel.StyleSheet.TableStyles; } }
		#endregion
		protected internal void SetStyleNameCore(string value) {
			UnSubscribeCacheEvent(styleName);
			SubscribeCacheEvent(value);
			table.CalculationInfo.InvalidateStyleFormatCache();
			styleName = value;
		}
		protected internal void SetBooleanValueCore(byte mask, bool value) {
			table.CalculationInfo.InvalidateStyleFormatCache();
			PackedValues.SetBoolBitValue(ref this.packedValues, mask, value);
		}
		void SubscribeCacheEvent(string styleName) {
			if (TableStyles.ContainsStyleName(styleName))
				TableStyles[styleName].Cache.OnInvalid += OnInvalidFormatCache;
		}
		void UnSubscribeCacheEvent(string styleName) {
			if (TableStyles.ContainsStyleName(styleName))
				TableStyles[styleName].Cache.OnInvalid -= OnInvalidFormatCache;
		}
		void OnInvalidFormatCache(object sender, EventArgs e) {
			table.CalculationInfo.InvalidateStyleFormatCache();
		}
		#region Internal
		void ApplyHistoryItem(HistoryItem item) {
			DocumentModel.History.Add(item);
			item.Execute();
		}
		void SetBooleanValue(byte mask, bool value) {
			if (GetBooleanValue(mask) != value) 
				ApplyHistoryItem(new ChangePivotTableStyleInfoBooleanHistoryItem(this, GetBooleanValue(mask), value, mask));
		}
		bool GetBooleanValue(uint mask) {
			return PackedValues.GetBoolBitValue(this.packedValues, mask);
		}
		#endregion
		#region IPivotStyleOptions members
		bool IPivotStyleOptions.HasColumnGrandTotals { get { return table.ColumnGrandTotals; } }
		bool IPivotStyleOptions.HasRowGrandTotals { get { return table.RowGrandTotals; } }
		public void CopyFrom(PivotTableStyleInfo source) {
			this.packedValues = source.packedValues;
			this.styleName = source.styleName;
		}
		#endregion
		public void CopyFromNoHistory(PivotTableStyleInfo source) {
			styleName = source.styleName;
			packedValues = source.packedValues;
		}
	}
	#endregion
}
