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
using System.Drawing;
using System.Text;
using DevExpress.Data.PivotGrid;
using DevExpress.Compatibility.System.Drawing;
using System.Linq;
namespace DevExpress.XtraPivotGrid.Data {
	public abstract class CellSizeProviderBase {
		public const int FieldValueTextOffset = 2;
		protected const int DefaultCellSizeCountLimit = 1000;
		protected const int DefaultFieldValueSizeCountLimit = 300;
		public const int DefaultLeftCellPadding = 2;
		public const int DefaultRightCellPadding = 2;
		public const int DefaultTopCellPadding = 0;
		public const int DefaultBottomCellPadding = 0;
		protected TimeSpan DefaultCellSizeLifeTime = new TimeSpan(0, 1, 0),
			  MinClearCheckPeriod = new TimeSpan(0, 0, 30);
		readonly PivotVisualItemsBase visualItems;
		readonly PivotGridData data;
		readonly Dictionary<int, Size> columnAreaSizeCache, rowAreaSizeCache;
		readonly FieldValuePositionContainer columnLefts, columnTops, rowLefts, rowTops;
		int[] rowItemsHeightCache, columnItemsWidthCache, rowItemsWidthCache, columnItemsHeightCache;
		protected PivotVisualItemsBase VisualItems { get { return visualItems; } }
		protected int RowTreeLevelOffset { get { return visualItems.RowTreeLevelOffset; } }
		protected int RowMaxExpandedLevel { get { return visualItems.RowMaxExpandedLevel; } }
		protected int RowTreeLevelWidth { get { return visualItems.RowTreeLevelWidth; } }
		protected PivotFieldItemCollection FieldItems { get { return data.FieldItems; } }
		protected PivotGridData Data { get { return data; } }
		public int DefaultFieldWidth { get { return PivotGridFieldBase.DefaultWidth; } }
		public virtual int LeftCellPadding { get { return DefaultLeftCellPadding; } }
		public virtual int RightCellPadding { get { return DefaultRightCellPadding; } }
		public virtual int TopCellPadding { get { return DefaultTopCellPadding; } }
		public virtual int BottomCellPadding { get { return DefaultBottomCellPadding; } }
		public CellSizeProviderBase(PivotGridData data, PivotVisualItemsBase visualItems) {
			this.data = data;
			this.visualItems = visualItems;
			this.visualItems.AfterCalculating += VisualItemsCalculated;
			CreateLevelsSizeCache(visualItems);
			this.columnAreaSizeCache = new Dictionary<int, Size>();
			this.rowAreaSizeCache = new Dictionary<int, Size>();
			this.rowTops = new FieldValuePositionContainer(DefaultFieldValueSizeCountLimit, DefaultCellSizeLifeTime, MinClearCheckPeriod);
			this.columnLefts = new FieldValuePositionContainer(DefaultFieldValueSizeCountLimit, DefaultCellSizeLifeTime, MinClearCheckPeriod);
			this.columnTops = new FieldValuePositionContainer(DefaultFieldValueSizeCountLimit, DefaultCellSizeLifeTime, MinClearCheckPeriod);
			this.rowLefts = new FieldValuePositionContainer(DefaultFieldValueSizeCountLimit, DefaultCellSizeLifeTime, MinClearCheckPeriod);
		}
		void VisualItemsCalculated(object sender, EventArgs e) {
			CreateLevelsSizeCache(visualItems);
		}
		void CreateLevelsSizeCache(PivotVisualItemsBase visualItems) {
			rowItemsHeightCache = new int[visualItems.RowCount];
			columnItemsWidthCache = new int[visualItems.ColumnCount];
			rowItemsWidthCache = new int[visualItems.GetItemsCreator(false).GetUnpagedItems().LevelCount];
			columnItemsHeightCache = new int[visualItems.GetLevelCount(true)];
		}
		public Size GetLastLevelItemSize(bool isColumn, int lastLevelIndex) {
			Dictionary<int, Size> cache = isColumn ? columnAreaSizeCache : rowAreaSizeCache;
			Size res;
			if(cache.TryGetValue(lastLevelIndex, out res))
				return res;
			res = GetLastLevelItemSizeCore(visualItems.GetLastLevelUnpagedItem(isColumn, lastLevelIndex));
			cache.Add(lastLevelIndex, res);
			return res;
		}
		public int GetWidthDifference(bool isColumn, int left, int right) {
			if(right < left)
				return -GetWidthDifference(isColumn, right, left);
			if(right == left)
				return 0;
			if(right - left == 1) {
				return GetItemWidth(left, isColumn);
			} else {
				int result;
				PositionDic dic;
				FieldValuePositionContainer mainDic = isColumn ? columnLefts : rowLefts;
				if(!mainDic.TryGetValue(left, right, out result, out dic)) {
					result = GetWidthDifferenceCore(dic, isColumn, left, right - 1) + GetItemWidth(right - 1, isColumn);
					dic.Dic.Add(right, result);
					mainDic.OnAdd();
				}
				return result;
			}
		}
		int GetWidthDifferenceCore(PositionDic dic, bool isColumn, int left, int right) {
			int oldRight = right;
			int result = 0;
			bool founded = false;
			while(left != right && !founded) {
				if(!dic.Dic.TryGetValue(right, out result)) {
					right--;
				} else {
					founded = true;
				}
			}			
			for(int i = right; i < oldRight; i++)
				result += GetItemWidth(i, isColumn);
			return result;
		}
		public int GetHeightDifference(bool isColumn, int top, int bottom) {
			if(bottom < top)
				return -GetHeightDifference(isColumn, bottom, top);
		  	if(bottom == top)
				return 0;
			if(bottom - top == 1) {
				return GetItemHeight(top, isColumn);
			} else {
				int result;
				PositionDic dic;
				FieldValuePositionContainer mainDic = isColumn ? columnTops : rowTops;
				if(!mainDic.TryGetValue(top, bottom, out result, out dic)) {
					result = GetHeightDifferenceCore(dic, isColumn, top, bottom - 1) + GetItemHeight(bottom - 1, isColumn);
					dic.Dic.Add(bottom, result);
					mainDic.OnAdd();
				}
				return result;
			}
		}
		int GetHeightDifferenceCore(PositionDic dic, bool isColumn, int top, int bottom) {
			int oldBottom = bottom;
			int result = 0;
			bool founded = false;
			while(top != bottom && !founded) {
				if(!dic.Dic.TryGetValue(bottom, out result)) {
					bottom--;
				} else {
					founded = true;
				}
			}			
			for(int i = bottom; i < oldBottom; i++)
				result += GetItemHeight(i, isColumn);
			return result;
		}
		int GetItemWidth(int level, bool isColumn) {
			if(level < 0)
				level = 0;
			visualItems.EnsureIsCalculated();
			if(isColumn) {
				if(level >= columnItemsWidthCache.Length)
					return GetColumnFieldWidth(visualItems.GetLastLevelUnpagedItem(isColumn, level));
				int width = columnItemsWidthCache[level];
				if(width != 0)
					return width;
				else {
					width = GetColumnFieldWidth(visualItems.GetLastLevelUnpagedItem(isColumn, level));
					columnItemsWidthCache[level] = width;
					return width;
				}
			} else {
				if(level >= rowItemsWidthCache.Length)
					return GetRowFieldWidthByLevel(level);
				int width = rowItemsWidthCache[level];
				if(width != 0)
					return width;
				else {
					width = GetRowFieldWidthByLevel(level);
					rowItemsWidthCache[level] = width;
					return width;
				}
			}
		}
		int GetItemHeight(int level, bool isColumn) {
			if(level < 0)
				level = 0;
			 visualItems.EnsureIsCalculated();
			if(isColumn) {
				if(level >= columnItemsHeightCache.Length)
					return GetColumnFieldHeight(level);
				int height = columnItemsHeightCache[level];
				if(height != 0)
					return height;
				else {
					height = GetColumnFieldHeight(level);
					columnItemsHeightCache[level] = height;
					return height;
				}
			} else {
				if(level >= rowItemsHeightCache.Length)
					return GetRowFieldHeight(visualItems.GetLastLevelUnpagedItem(isColumn, level));
				int height = rowItemsHeightCache[level];
				if(height != 0)
					return height;
				else {
					height = GetRowFieldHeight(visualItems.GetLastLevelUnpagedItem(isColumn, level));
					rowItemsHeightCache[level] = height;
					return height;
				}
			}
		}
		public Size GetLastLevelItemSize(PivotFieldValueItem item) {
			if(item != null) {
				Size res = Size.Empty;
				Dictionary<int, Size> cache = item.IsColumn ? columnAreaSizeCache : rowAreaSizeCache;
				if(cache.TryGetValue(item.MinLastLevelIndex, out res))
					return res;
			}
			return GetLastLevelItemSizeCore(item);
		}
		protected virtual Size GetLastLevelItemSizeCore(PivotFieldValueItem item) {
			if(item == null)
				return new Size(PivotGridFieldBase.DefaultWidth, GetRowFieldHeight(null));
			int lastLevel = item.IsColumn ? item.MinLastLevelIndex : item.StartLevel;
			int level = item.IsColumn ? item.StartLevel : item.MinLastLevelIndex;
			return new Size(GetWidthDifference(item.IsColumn, lastLevel, lastLevel + 1), GetHeightDifference(item.IsColumn, level, level + 1));
		}
		protected object[] GetItemKey(PivotFieldValueItem item) {
			return VisualItems.GetItemsCreator(item.IsColumn).GetItemKey(item);
		}
		protected abstract int GetRowFieldHeight(PivotFieldValueItem item);
		protected abstract int GetColumnFieldHeight(int columnAreaLevel);
		protected virtual int GetColumnFieldWidth(PivotFieldValueItem item) {
			int width = PivotGridFieldBase.DefaultWidth;
			if(item != null) {
				PivotFieldItemBase field = item.ResizingField;
				if(field != null) {
					width = field.Width;
				}
			}
			return width;
		}
		public int GetFieldWidth(PivotArea area, int level) {
			PivotFieldItemBase field;
			if(area == PivotArea.ColumnArea)
				field = FieldItems.GetFieldItemByLevel(true, level);
			else
				if(area == PivotArea.RowArea)
					field = FieldItems.GetFieldItemByLevel(false, level);
				else
					field = FieldItems.GetFieldItemByArea(area, level);
			return field != null ? field.Width : DefaultFieldWidth;
		}
		protected int GetDataFieldLevel(bool isColumn) {
			int index = Data.OptionsDataField.DataFieldAreaIndex;
			if(index == 0 && FieldItems.GetFieldCountByArea(isColumn ? PivotArea.ColumnArea : PivotArea.RowArea) == 0 && !Data.ShouldRemoveGrandTotalHeader(isColumn))
				index++;
			return index;
		}
		protected int GetRowFieldWidthByLevel(int rowAreaLevel) {
			if(FieldItems.GetFieldCountByArea(PivotArea.RowArea) == 0) {
				PivotFieldValueItem visualItem = visualItems.GetLastLevelUnpagedItem(false, 0);
				PivotFieldItemBase item = visualItem != null ? visualItem.ResizingField ?? FieldItems.DataFieldItem : FieldItems.DataFieldItem;
				return item.Width;
			}
			if(Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree) {
				if(Data.GetIsDataFieldsVisible(false) && rowAreaLevel == GetDataFieldLevel(false))
					return FieldItems.DataFieldItem.Width;
				if(rowAreaLevel < RowMaxExpandedLevel)
					return VisualItems.RowTreeLevelOffset;
				if(rowAreaLevel == RowMaxExpandedLevel)
					return VisualItems.RowTreeLevelWidth;
				if(rowAreaLevel == 1 && FieldItems.DataFieldCount > 1 && FieldItems.RowFieldCount == 0)
					return RowTreeLevelWidth;
				return 0;
			} else {
				return GetFieldWidth(PivotArea.RowArea, rowAreaLevel);
			}
		}
		public virtual void Clear() {
			ClearSizeCaches();
		}
		protected void ClearSizeCaches() {
			columnAreaSizeCache.Clear();
			rowAreaSizeCache.Clear();
			rowTops.Clear();
			columnLefts.Clear();
			columnTops.Clear();
			rowLefts.Clear();
			CreateLevelsSizeCache(visualItems);
		}
		protected PivotFieldItemBase DataFieldItem {
			get {
				return data.FieldItems.DataFieldItem;
			}
		}
	}
	public abstract class CellSizeProvider : CellSizeProviderBase {
		Dictionary<PivotFieldItemBase, int> headerHeightsCache = new Dictionary<PivotFieldItemBase, int>();
		FieldValueHeightsCache fieldValueHeightsCache = new FieldValueHeightsCache();
		int defaultFieldValueHeight = 0;
		int defaultHeaderHeight;
		public virtual int DefaultFieldValueHeight {
			get {
				if(defaultFieldValueHeight == 0)
					defaultFieldValueHeight = GetFieldValueHeight(1, PivotGridValueType.Value, null);
				return defaultFieldValueHeight;
			}
		}
		public CellSizeProvider(PivotGridData data, PivotVisualItemsBase visualItems)
			: base(data, visualItems) {
		}
		public virtual int HeaderWidthOffset { get { return this.Data.OptionsView.HeaderWidthOffset; } }
		public virtual int HeaderHeightOffset { get { return this.Data.OptionsView.HeaderHeightOffset; } }
		public virtual int DefaultHeaderHeight {
			get {
				if(defaultHeaderHeight == 0)
					defaultHeaderHeight = CalculateHeaderHeight(null);
				return defaultHeaderHeight;
			}
		}
		protected abstract int GetFieldValueHeight(int lineCount, PivotGridValueType valueType, PivotFieldItemBase field);
		protected string GetMeasureCaption(int lineCount) {
			const string measuringString = "Qq";
			StringBuilder res = new StringBuilder();
			for(int i = 0; i < lineCount; i++) {
				res.Append(measuringString);
				if(i != lineCount - 1)
					res.AppendLine();
			}
			return res.ToString();
		}
		public int GetHeaderHeight(PivotFieldItemBase field) {
			int height;
			if(!this.headerHeightsCache.TryGetValue(field, out height)) {
				height = CalculateHeaderHeight(field);
				this.headerHeightsCache.Add(field, height);
			}
			return height;
		}
		public int GetFieldValueTotalHeight(PivotFieldItemBase field, int lineCount) {
			int height = fieldValueHeightsCache.GetTotalHeight(field, lineCount);
			if(height == -1) {
				height = GetFieldValueHeight(lineCount, PivotGridValueType.Total, field);
				fieldValueHeightsCache.SetTotalHeight(field, lineCount, height);
			}
			return height;
		}
		public int GetFieldValueGrandTotalHeight(PivotFieldItemBase field, int lineCount) {
			int height = fieldValueHeightsCache.GetGrandTotalHeight(field, lineCount);
			if(height == -1) {
				height = GetFieldValueHeight(lineCount, PivotGridValueType.GrandTotal, field);
				fieldValueHeightsCache.SetGrandTotalHeight(field, lineCount, height);
			}
			return height;
		}
		public int GetFieldValueHeight(PivotFieldItemBase field, int lineCount) {
			int height = fieldValueHeightsCache.GetValueHeight(field, lineCount);
			if(height == -1) {
				height = GetFieldValueHeight(lineCount, PivotGridValueType.Value, field);
				fieldValueHeightsCache.SetValueHeight(field, lineCount, height);
			}
			return height;
		}
		public int GetHeaderAreaHeight(PivotArea area) {
			int height = DefaultHeaderHeight;
			foreach(KeyValuePair<PivotFieldItemBase, int> pair in headerHeightsCache) {
				if(pair.Key.Area == area && pair.Value > height)
					height = pair.Value;
			}
			return height + HeaderHeightOffset * 2;
		}
		protected internal abstract int CalculateHeaderHeight(PivotFieldItemBase field);
		protected internal abstract int CalculateHeaderWidth(PivotFieldItemBase field);
		protected internal abstract int CalculateHeaderWidthOffset(PivotFieldItemBase field);
		public override void Clear() {
			base.Clear();
			this.headerHeightsCache.Clear();
			this.fieldValueHeightsCache.Clear();
			this.defaultFieldValueHeight = 0;
			this.defaultHeaderHeight = 0;
		}
		protected override int GetColumnFieldHeight(int columnAreaLevel) {
			PivotGridValueType valueType = PivotGridValueType.Value;
			if(FieldItems.GetFieldItemsByArea(PivotArea.ColumnArea, true).Count < 2)
				valueType = PivotGridValueType.GrandTotal;
			PivotFieldItemBase field = FieldItems.GetFieldItemByLevel(true, columnAreaLevel);
			PivotFieldValueItemsCreator creator = VisualItems.GetItemsCreator(true);
			if(VisualItems.GetLevelCount(true) == columnAreaLevel + 1 && creator.UnpagedItemsCount > 0) {
				if((creator.GetLastLevelItem(0).ValueType != PivotGridValueType.Value || creator.GetLastLevelItem(creator.LastLevelItemCount - 1).ValueType != PivotGridValueType.Value) && creator.DataLevel == columnAreaLevel)
					return Math.Max(GetHeightCore(field, valueType, columnAreaLevel, true), GetHeightCore(FieldItems.DataFieldItem, valueType, columnAreaLevel, true));
			} else {
				if(creator.GrandTotalItemCount == creator.LastLevelItemCount)
					return GetHeightCore(FieldItems.GetFieldItemByArea(PivotArea.ColumnArea, columnAreaLevel), valueType, columnAreaLevel, true);
			}
			if(field != null && field.IsDataField)
				return FieldItems.GetFieldItemsByArea(PivotArea.DataArea, false).Concat(new PivotFieldItemBase[] { field }).Max(s => GetHeightCore(s, valueType, columnAreaLevel, true));
			return GetHeightCore(field, valueType, columnAreaLevel, true);
		}
		public int GetColumnItemHeight(PivotFieldValueItem item) {
			switch(item.ValueType) {
				case PivotGridValueType.CustomTotal:
				case PivotGridValueType.Total:
					return GetTotalFieldHeight(item);
				case PivotGridValueType.GrandTotal:
					return GetGrandTotalFieldHeight(item);
				case PivotGridValueType.Value:
					return GetFieldValueHeight(item);
				default:
					throw new ArgumentOutOfRangeException("Item.ValueType");
			}
		}
		public virtual int GetTotalFieldHeight(PivotFieldValueItem item) {
			if(!item.IsColumn || item.IsLastFieldLevel && item.StartLevel == item.EndLevel)
				return GetFieldValueTotalHeight(item.Field, GetLineCount(item.IsColumn, item.Field));
			int result = 0;
			for(int i = item.StartLevel; i <= item.EndLevel; i++) {
				PivotFieldItemBase field = FieldItems.GetFieldItemByLevel(true, i);
				if(field != null && field.IsDataField && !item.IsLastFieldLevel)
					field = FieldItems.GetFieldItemByLevel(true, i + 1);
				result += GetFieldValueHeight(field, GetLineCount(item.IsColumn, field));
			}
			if(result == 0)
				result = DefaultFieldValueHeight;
			return result;
		}
		protected virtual int GetFieldValueHeight(PivotFieldValueItem item) {
			int result = 0;
			for(int i = item.StartLevel; i <= item.EndLevel; i++) {
				PivotFieldItemBase field = FieldItems.GetFieldItemByLevel(true, i);
				result += GetFieldValueHeight(field, GetLineCount(item.IsColumn, field));
			}
			int grandTotalHeight = GetFieldValueGrandTotalHeight(null, 1);
			if(FieldItems.GetFieldCountByArea(PivotArea.ColumnArea) == 1 && result < grandTotalHeight)
				result = grandTotalHeight;
			return result;
		}
		protected virtual int GetGrandTotalFieldHeight(PivotFieldValueItem item) {
			if(item.Field != null && item.EndLevel == item.StartLevel)
				return GetFieldValueGrandTotalHeight(item.Field, GetLineCount(item.IsColumn, item.Field));
			if(!item.IsColumn)
				return GetFieldValueGrandTotalHeight(item.Field, 1);
			int result = 0;
			int fieldsCount = FieldItems.GetFieldCountByArea(PivotArea.ColumnArea);
			for(int i = 0; i < fieldsCount; i++) {
				PivotFieldItemBase field = FieldItems.GetFieldItemByArea(PivotArea.ColumnArea, i);
				result += GetFieldValueHeight(field, GetLineCount(item.IsColumn, field));
			}
			if(fieldsCount != 0 && fieldsCount == item.CellLevelCount - 1)
				result += GetFieldValueHeight(FieldItems.DataFieldItem, GetLineCount(item.IsColumn, FieldItems.DataFieldItem));
			if(result == 0)
				result = DefaultFieldValueHeight;
			return Math.Max(result, GetFieldValueGrandTotalHeight(item.Field, GetLineCount(item.IsColumn, item.Field)));
		}
		protected override int GetRowFieldHeight(PivotFieldValueItem item) {
			return GetHeightCore(FieldItems.GetFieldItemByLevel(false, item.StartLevel), item.ValueType, item.StartLevel, false); 
		}
		int GetHeightCore(PivotFieldItemBase field, PivotGridValueType valueType, int level, bool isColumn) {
			int lineCount = GetLineCount(field == null || !field.IsColumnOrRow ? isColumn : field.IsColumn, field);
			switch(valueType) {
				case PivotGridValueType.CustomTotal:
				case PivotGridValueType.Total:
					return GetFieldValueTotalHeight(field, lineCount);
				case PivotGridValueType.GrandTotal:
					return GetFieldValueGrandTotalHeight(field, lineCount);
				case PivotGridValueType.Value:
					return GetFieldValueHeight(field, lineCount);
				default:
					throw new ArgumentOutOfRangeException("Item.ValueType");
			}
		}
		int GetLineCount(bool isColumn, PivotFieldItemBase field) {
			const int lineCount = 1;
			if(field == null)
				return lineCount;
			return isColumn ? field.ColumnValueLineCount : field.RowValueLineCount;
		}
		public virtual int GetFieldValueSeparator(PivotFieldValueItem fieldValueItem) {
			return 0;
		}
#if !SL
		public int GetFieldValuePrintSeparator(PivotFieldValueItem fieldValue) {
			if(fieldValue.ValueType != PivotGridValueType.Value && fieldValue.ValueType != PivotGridValueType.GrandTotal)
				return 0;
			if(!IsTheSameLevelWithRoot(fieldValue))
				return 0;
			return fieldValue.IsColumn ? Data.OptionsPrint.ColumnFieldValueSeparator : Data.OptionsPrint.RowFieldValueSeparator;
		}
#endif
		bool IsTheSameLevelWithRoot(PivotFieldValueItem fieldValue) {
			if(fieldValue.MinLastLevelIndex == 0)
				return false;
			int level = Data.GetObjectLevel(fieldValue.IsColumn, fieldValue.VisibleIndex);
			if(level <= 0)
				return true;
			for(int i = fieldValue.VisibleIndex - 1; i > 0; i--) {
				int parentLevel = Data.GetObjectLevel(fieldValue.IsColumn, i);
				if(parentLevel == 0)
					return true;
				if(level == parentLevel)
					return false;
			}
			return false;
		}
		public int GetDefaultFieldValueHeight(PivotFieldValueItem fieldValue) {
			int height;
			int lineCount = GetLineCount(fieldValue.IsColumn, fieldValue.Field);
			switch(fieldValue.ValueType) {
				case PivotGridValueType.CustomTotal:
				case PivotGridValueType.Total:
					height = GetTotalFieldHeight(fieldValue);
					break;
				case PivotGridValueType.GrandTotal:
					height = GetGrandTotalFieldHeight(fieldValue);
					break;
				case PivotGridValueType.Value:
					height = GetFieldValueHeight(fieldValue.Field, lineCount);
					break;
				default:
					throw new ArgumentOutOfRangeException("Item.ValueType");
			}
			return height;
		}
	}
	public class ResizingFieldsCache : Dictionary<PivotGridFieldBase, byte> {
		public ResizingFieldsCache() : base() { }
		public void Add(PivotGridFieldBase key, bool widthChanged, bool heightChanged) {
			if(Contains(key))
				return;
			byte value = 0;
			if(widthChanged)
				value |= 1;
			if(heightChanged)
				value |= 2;
			base.Add(key, value);
		}
		bool Contains(PivotGridFieldBase internalField) {
			byte value;
			return TryGetValue(internalField, out value);
		}
		public void RaiseFieldSizeChangedEvents(PivotGridData data) {
			foreach(KeyValuePair<PivotGridFieldBase, byte> pair in this) {
				data.OnFieldSizeChanged(pair.Key, (pair.Value & 1) != 0, (pair.Value & 2) != 0);
			}
		}
	}
	class DoubleKeyNullableDictionary<TKey1, TKey2, TValue>
		where TKey2 : class
		where TValue : new() {
		Dictionary<TKey1, NullableDictionary<TKey2, TValue>> dictionary = new Dictionary<TKey1, NullableDictionary<TKey2, TValue>>();
		public TValue GetOrCreateValue(TKey1 key1, TKey2 key2) {
			NullableDictionary<TKey2, TValue> innerDictionary;
			if(!dictionary.TryGetValue(key1, out innerDictionary)) {
				innerDictionary = new NullableDictionary<TKey2, TValue>();
				dictionary.Add(key1, innerDictionary);
			}
			TValue value;
			if(!innerDictionary.TryGetValue(key2, out value)) {
				value = new TValue();
				innerDictionary.Add(key2, value);
			}
			return value;
		}
		public void Clear() {
			foreach(KeyValuePair<TKey1, NullableDictionary<TKey2, TValue>> pair in dictionary)
				pair.Value.Clear();
			dictionary.Clear();
		}
	}
	class FieldValueHeightsCache {
		DoubleKeyNullableDictionary<int, PivotFieldItemBase, FieldValueMeasures> cache = new DoubleKeyNullableDictionary<int, PivotFieldItemBase, FieldValueMeasures>();
		FieldValueMeasures GetOrCreateMeasures(PivotFieldItemBase field, int lineCount) {
			return this.cache.GetOrCreateValue(lineCount, field);
		}
		public void SetValueHeight(PivotFieldItemBase field, int lineCount, int height) {
			FieldValueMeasures measures = GetOrCreateMeasures(field, lineCount);
			if(field == null) {
				measures.ValueHeight = height;
				return;
			}
			if(field.IsColumn && field.ColumnValueLineCount != lineCount)
				throw new ArgumentException("lineCount");
			if(!field.IsColumn && field.RowValueLineCount != lineCount)
				measures.CustomValueHeights.Add(lineCount, height);
			else
				measures.ValueHeight = height;
		}
		public void SetTotalHeight(PivotFieldItemBase field, int lineCount, int height) {
			FieldValueMeasures measures = GetOrCreateMeasures(field, lineCount);
			if(field.IsColumnOrRow && (field.IsColumn ? field.ColumnValueLineCount : field.RowValueLineCount) != lineCount)
				throw new ArgumentException("lineCount");
			measures.TotalHeight = height;
		}
		public void SetGrandTotalHeight(PivotFieldItemBase field, int lineCount, int height) {
			FieldValueMeasures measures = GetOrCreateMeasures(field, lineCount);
			if(field != null && field.IsColumnOrRow && (field.IsColumn ? field.ColumnValueLineCount : field.RowValueLineCount) != lineCount)
				throw new ArgumentException("lineCount");
			measures.GrandTotalHeight = height;
		}
		public int GetValueHeight(PivotFieldItemBase field, int lineCount) {
			FieldValueMeasures measures = GetOrCreateMeasures(field, lineCount);
			if(field == null)
				return measures.ValueHeight;
			if(field.IsColumn && field.ColumnValueLineCount != lineCount)
				throw new ArgumentException("lineCount");
			if(!field.IsColumn && field.RowValueLineCount != lineCount) {
				int height;
				if(!measures.CustomValueHeights.TryGetValue(lineCount, out height))
					return -1;
				else
					return height;
			} else
				return measures.ValueHeight;
		}
		public int GetTotalHeight(PivotFieldItemBase field, int lineCount) {
			FieldValueMeasures measures = GetOrCreateMeasures(field, lineCount);
			if(field.IsColumnOrRow && (field.IsColumn ? field.ColumnValueLineCount : field.RowValueLineCount) != lineCount)
				throw new ArgumentException("lineCount");
			return measures.TotalHeight;
		}
		public int GetGrandTotalHeight(PivotFieldItemBase field, int lineCount) {
			FieldValueMeasures measures = GetOrCreateMeasures(field, lineCount);
			if(field != null && field.IsColumnOrRow && (field.IsColumn ? field.ColumnValueLineCount : field.RowValueLineCount) != lineCount)
				throw new ArgumentException("lineCount");
			return measures.GrandTotalHeight;
		}
		public void Clear() {
			this.cache.Clear();
		}
	}
	#region Cache Dictionary Value
	class FieldValueMeasures {
		public int ValueHeight = -1;
		public int TotalHeight = -1;
		public int GrandTotalHeight = -1;
		public Dictionary<int, int> CustomValueHeights = new Dictionary<int, int>();
	}
	#endregion
}
