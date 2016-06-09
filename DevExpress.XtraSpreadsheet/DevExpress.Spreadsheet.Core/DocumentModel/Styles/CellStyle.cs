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
using DevExpress.Office.Utils;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.History;
using DevExpress.Office.Drawing;
using System.ComponentModel;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region StyleCategory
	public enum StyleCategory {
		CustomStyle = 0x00,
		GoodBadNeutralStyle = 0x01,
		DataModelStyle = 0x02,
		TitleAndHeadingStyle = 0x03,
		ThemedCellStyle = 0x04,
		NumberFormatStyle = 0x05
	}
	#endregion
	#region CellStyleCollection
	public class CellStyleCollection : IBatchUpdateable, IBatchUpdateHandler {
		readonly Dictionary<int, CellStyleBase> innerList;
		BatchUpdateHelper batchUpdateHelper;
		bool deferredRaiseChanged;
		public CellStyleCollection() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.innerList = new Dictionary<int, CellStyleBase>();
		}
		#region Properties
		public CellStyleBase this[int index] { get { return innerList[index]; } }
		public int Count { get { return innerList.Count; } }
		IDictionary<int, CellStyleBase> InnerList { get { return innerList; } }
		public CellStyleBase Normal { get { return innerList[0]; } }
		#endregion
		#region Events
		#region StyleAdded
		StyleCollectionChangedEventHandler onStyleAdded;
		internal event StyleCollectionChangedEventHandler StyleAdded { add { onStyleAdded += value; } remove { onStyleAdded -= value; } }
		protected internal virtual void RaiseStyleAdded(CellStyleBase newStyle, int styleIndex) {
			if (onStyleAdded != null) {
				StyleCollectionChangedEventArgs args = new StyleCollectionChangedEventArgs(newStyle, styleIndex);
				onStyleAdded(this, args);
			}
		}
		#endregion
		#region StyleRemoved
		StyleCollectionChangedEventHandler onStyleRemoved;
		internal event StyleCollectionChangedEventHandler StyleRemoved { add { onStyleRemoved += value; } remove { onStyleRemoved -= value; } }
		protected internal virtual void RaiseStyleRemoved(CellStyleBase newStyle, int styleIndex) {
			if (onStyleRemoved != null) {
				StyleCollectionChangedEventArgs args = new StyleCollectionChangedEventArgs(newStyle, styleIndex);
				onStyleRemoved(this, args);
			}
		}
		#endregion
		#region CollectionClear
		EventHandler onCollectionClear;
		internal event EventHandler CollectionCleared { add { onCollectionClear += value; } remove { onCollectionClear -= value; } }
		protected internal virtual void RaiseCollectionCleared() {
			if (onCollectionClear != null) {
				onCollectionClear(this, EventArgs.Empty);
			}
		}
		#endregion
		#region CollectionChanged
		EventHandler onCollectionChanged;
		public event EventHandler CollectionChanged {
			add { onCollectionChanged += value; }
			remove { onCollectionChanged -= value; }
		}
		protected internal virtual void RaiseCollectionChanged() {
			if (onCollectionChanged != null)
				onCollectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal BuiltInCellStyle TryGetBuiltInCellStyleById(int id) {
			foreach (int index in innerList.Keys) {
				BuiltInCellStyle result = this[index] as BuiltInCellStyle;
				if (result != null && result.BuiltInId == id)
					return result;
			}
			return null;
		}
		protected internal CellStyleBase GetCellStyleByName(string name) {
			int index = GetCellStyleIndexByName(name);
			if (index == -1)
				return null;
			return InnerList[index];
		}
		protected internal int GetCellStyleIndexByName(string name) {
			foreach (int index in InnerList.Keys)
				if (this[index].Name == name)
					return index;
			return -1;
		}
		public void Add(CellStyleBase item) {
			item.AssignStyleIndex(Count);
			RaiseStyleAdded(item, Count);
			innerList.Add(Count, item);
			SubscribeStyleEvents(item);
			OnCollectionChanged();
		}
		void OnCollectionChanged() {
			if (IsUpdateLocked)
				this.deferredRaiseChanged = true;
			else
				RaiseCollectionChanged();
		}
		public void Remove(CellStyleBase item) {
			RaiseStyleRemoved(item, item.StyleIndex);
			item.SetHidden(true);
			OnCollectionChanged();
		}
		internal void Clear() {
			ForEach(UnsubscribeStyleEvents);
			RaiseCollectionCleared();
			innerList.Clear();
			OnCollectionChanged();
		}
		void SubscribeStyleEvents(CellStyleBase style) {
			style.Changed += OnStyleChanged;
		}
		void UnsubscribeStyleEvents(CellStyleBase style) {
			style.Changed -= OnStyleChanged;
		}
		void OnStyleChanged(object sender, EventArgs e) {
			OnCollectionChanged();
		}
		public void ForEach(Action<CellStyleBase> action) {
			Guard.ArgumentNotNull(action, "action");
			foreach (int index in InnerList.Keys)
				action(InnerList[index]);
		}
		public void CopyFrom(DocumentModel targetDocumentModel, CellStyleCollection cellStyles) {
			this.Clear();
			foreach (KeyValuePair<int, CellStyleBase> sourceCellStyleItem in cellStyles.InnerList) {
				int sourceId = sourceCellStyleItem.Key;
				CellStyleBase sourceCellStyle = sourceCellStyleItem.Value;
				CellStyleBase targetCellStyle = sourceCellStyle.Clone(targetDocumentModel, sourceCellStyle.Name);
				this.Add(targetCellStyle);
				System.Diagnostics.Debug.Assert(targetCellStyle.StyleIndex == sourceId);
			}
		}
		public IEnumerable<CellStyleBase> GetStyles() {
			return innerList.Values;
		}
		public IEnumerable<CellStyleBase> GetCustomStyles() {
			foreach (CellStyleBase item in innerList.Values) {
				if (item is CustomCellStyle)
					yield return item;
			}
		}
		public IEnumerable<CellStyleBase> GetModifiedBuiltInStyles() {
			foreach (CellStyleBase item in innerList.Values) {
				BuiltInCellStyle builtInStyle = item as BuiltInCellStyle;
				if (builtInStyle.CustomBuiltIn)
					yield return item;
			}
		}
		internal bool ContainsCellStyleName(string name) {
			return GetCellStyleIndexByName(name) != -1;
		}
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			deferredRaiseChanged = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (deferredRaiseChanged) {
				deferredRaiseChanged = false;
				RaiseCollectionChanged();
			}
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
	}
	#endregion
	#region CellStyleFormatIndexAccessor
	public class CellStyleFormatIndexAccessor : IIndexAccessor<CellStyleBase, CellStyleFormat, DocumentModelChangeActions> {
		#region IIndexAccessor Members
		public int GetIndex(CellStyleBase owner) {
			return owner.FormatIndex;
		}
		public int GetDeferredInfoIndex(CellStyleBase owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(CellStyleBase owner, int value) {
			owner.AssignCellStyleFormatIndex(value);
		}
		public int GetInfoIndex(CellStyleBase owner, CellStyleFormat value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public CellStyleFormat GetInfo(CellStyleBase owner) {
			return (CellStyleFormat)GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(CellStyleBase owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<FormatBase> GetInfoCache(CellStyleBase owner) {
			return owner.DocumentModel.Cache.CellFormatCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(CellStyleBase owner) {
			return new CellStyleFormatIndexChangeHistoryItem(owner);
		}
		public CellStyleFormat GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((CellStyleBatchUpdateHelper)helper).CellStyleFormat;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, CellStyleFormat info) {
			CellStyleBatchUpdateHelper cellStyleBatchUpdateHelper = helper as CellStyleBatchUpdateHelper;
			if (cellStyleBatchUpdateHelper.CellStyleFormat == null)
				cellStyleBatchUpdateHelper.CellStyleFormat = info.Clone();
			else
				cellStyleBatchUpdateHelper.CellStyleFormat.CopyFromDeferred(info); 
		}
		public void InitializeDeferredInfo(CellStyleBase owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(CellStyleBase owner, CellStyleBase from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(CellStyleBase owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region CellStyleBatchUpdateHelper
	public class CellStyleBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		CellStyleFormat cellStyleFormat;
		public CellStyleBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public CellStyleFormat CellStyleFormat { get { return cellStyleFormat; } set { cellStyleFormat = value; } }
		public override void BeginUpdateDeferredChanges() {
			cellStyleFormat.BeginUpdate();
		}
		public override void CancelUpdateDeferredChanges() {
			cellStyleFormat.CancelUpdate();
		}
		public override void EndUpdateDeferredChanges() {
			cellStyleFormat.EndUpdate();
		}
	}
	public class CellStyleBatchInitHelper : FormatBaseBatchUpdateHelper {
		public CellStyleBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region CellStyleBase
	public abstract class CellStyleBase : MultiIndexObject<CellStyleBase, DocumentModelChangeActions>, IFormatBase, IActualFormat, IFormatBaseBatchUpdateable, IActualApplyInfo {
		#region Static Members
		readonly static CellStyleFormatIndexAccessor cellStyleFormatIndexAccessor = new CellStyleFormatIndexAccessor();
		readonly static IIndexAccessorBase<CellStyleBase, DocumentModelChangeActions>[] indexAccessors = new IIndexAccessorBase<CellStyleBase, DocumentModelChangeActions>[] {
			cellStyleFormatIndexAccessor
		};
		public static CellStyleFormatIndexAccessor CellStyleFormatIndexAccessor { get { return cellStyleFormatIndexAccessor; } }
		#endregion
		#region Fields
		const int IsNotRegistered = -1;
		readonly DocumentModel documentModel;
		int formatIndex;
		string name;
		int styleIndex = IsNotRegistered;
		bool isHidden;
		bool raiseChanged;
		#endregion
		protected CellStyleBase(DocumentModel documentModel, string name) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentIsNotNullOrEmpty(name, "Name");
			this.documentModel = documentModel;
			this.name = name;
			formatIndex = CellFormatCache.DefaultCellStyleFormatIndex;
		}
		#region Properties
		internal int FormatIndex { get { return formatIndex; } }
		internal new CellStyleBatchUpdateHelper BatchUpdateHelper { get { return (CellStyleBatchUpdateHelper)base.BatchUpdateHelper; } }
		internal CellStyleFormat FormatInfo { get { return BatchUpdateHelper != null ? BatchUpdateHelper.CellStyleFormat : FormatInfoCore; } }
		CellStyleFormat FormatInfoCore { get { return (CellStyleFormat)DocumentModel.Cache.CellFormatCache[formatIndex]; } }
		internal new DocumentModel DocumentModel { get { return documentModel; } }
		DocumentModel IFormatBaseBatchUpdateable.DocumentModel { get { return this.DocumentModel; } }
		public string Name { get { return name; } protected set { name = value; } }
		public bool IsRegistered { get { return styleIndex != IsNotRegistered; } }
		public bool IsHidden { get { return isHidden; } }
		protected internal int StyleIndex { get { return styleIndex; } }
		public abstract bool IsBuiltIn { get; }
		public abstract int OutlineLevel { get; }
		#endregion
		#region Events
		#region Changed
		EventHandler onChanged;
		public event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected  override void OnFirstBeginUpdateCore() {
			base.OnFirstBeginUpdateCore();
			this.raiseChanged = false;
		}
		protected  override void OnLastEndUpdateCore() {
			base.OnLastEndUpdateCore();
			if (raiseChanged)
				RaiseChanged();
		}
		void OnChanged() {
			if (IsUpdateLocked)
				this.raiseChanged = true;
			else
				RaiseChanged();
		}
		protected internal void SetHidden(bool value) {
			SetHiddenCore(value);
			if (value)
				RemoveCellStyleForReferencedItem();
			RaiseChanged();
		}
		internal void SetHiddenCore(bool value) {
			isHidden = value;
		}
		protected internal virtual void RemoveCellStyleForReferencedItem() {
			DocumentModel.BeginUpdate();
			try {
				Dictionary<int, int> formatIndexTable = new Dictionary<int, int>();
				WorksheetCollection sheets = DocumentModel.Sheets;
				int count = sheets.Count;
				for (int i = 0; i < count; i++) {
					Worksheet sheet = sheets[i];
					RemoveCellStyleForReferencedCells(sheet, formatIndexTable);
					RemoveCellStyleForReferencedRows(sheet.Rows.GetExistingRows(0, sheet.MaxRowCount, false), formatIndexTable);
					RemoveCellStyleForReferencedColumns(sheet.Columns.GetExistingColumns(), formatIndexTable);
				}
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		void RemoveCellStyleForReferencedCells(Worksheet sheet, Dictionary<int, int> formatIndexTable) {
			CellPosition topLeft = new CellPosition(0, 0);
			CellPosition bottomRight = new CellPosition(sheet.MaxColumnCount - 1, sheet.MaxRowCount - 1);
			CellRange range = new CellRange(sheet, topLeft, bottomRight);
			foreach (ICellBase info in range.GetExistingCellsEnumerable()) {
				ICell cell = info as ICell;
				if (cell != null && cell.Style.StyleIndex == StyleIndex) {
					int formatIndex = cell.FormatIndex;
					if (formatIndexTable.ContainsKey(formatIndex))
						cell.SetCellFormatIndex(formatIndexTable[formatIndex]);
					else {
						cell.RemoveStyle();
						formatIndexTable.Add(formatIndex, cell.FormatIndex);
					}
				}
			}
		}
		void RemoveCellStyleForReferencedRows(IEnumerable<Row> rows, Dictionary<int, int> formatIndexTable) {
			foreach (Row row in rows)
				if (row.Style.StyleIndex == StyleIndex) {
					int formatIndex = row.FormatIndex;
					if (formatIndexTable.ContainsKey(formatIndex))
						row.AssignCellFormatIndex(formatIndexTable[formatIndex]);
					else {
						row.RemoveStyle();
						formatIndexTable.Add(formatIndex, row.FormatIndex);
					}
				}
		}
		void RemoveCellStyleForReferencedColumns(IEnumerable<Column> columns, Dictionary<int, int> formatIndexTable) {
			foreach (Column column in columns)
				if (column.Style.StyleIndex == StyleIndex) {
					int formatIndex = column.FormatIndex;
					if (formatIndexTable.ContainsKey(formatIndex))
						column.AssignCellFormatIndex(formatIndexTable[formatIndex]);
					else {
						column.RemoveStyle();
						formatIndexTable.Add(formatIndex, column.FormatIndex);
					}
				}
		}
		protected internal void AssignStyleIndex(int index) {
			this.styleIndex = index;
		}
		public override bool Equals(object obj) {
			CellStyleBase other = obj as CellStyleBase;
			if (other == null)
				return false;
			return
				Name == other.Name &&
				IsRegistered == other.IsRegistered &&
				IsHidden == other.IsHidden &&
				base.Equals(other);
		}
		public bool EqualsByFormatting(CellStyleBase other) {
			DocumentModel otherDocumentModel = other.DocumentModel;
			if (Object.ReferenceEquals(otherDocumentModel, DocumentModel))
				return FormatInfo.Equals(other.FormatInfo);
			return EqualsByFormattingForDifferentWorkbooks(other);
		}
		bool EqualsByFormattingForDifferentWorkbooks(CellStyleBase other) {
			bool result = false;
			try {
				BeginUpdate();
				other.BeginUpdate();
				result = FormatInfo.EqualsForDifferentWorkbooks(other.FormatInfo);
			} finally {
				EndUpdate();
				other.EndUpdate();
			}
			return result;
		}
		public override int GetHashCode() {
			int result = base.GetHashCode() ^ IsHidden.GetHashCode();
			if (!String.IsNullOrEmpty(Name))
				result ^= Name.GetHashCode();
			return result;
		}
		public override string ToString() {
			return Name;
		}
		#region MultiIndexObject
		protected override IDocumentModel GetDocumentModel() {
			return DocumentModel;
		}
		protected override IIndexAccessorBase<CellStyleBase, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		public override CellStyleBase GetOwner() {
			return this;
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new CellStyleBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new CellStyleBatchInitHelper(this);
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected  override void ApplyChanges(DocumentModelChangeActions actions) {
		}
		#endregion
		#region IActualFormat Members
		public IActualRunFontInfo ActualFont { get { return this; } }
		public IActualFillInfo ActualFill { get { return this; } }
		public IActualCellAlignmentInfo ActualAlignment { get { return this; } }
		public IActualBorderInfo ActualBorder { get { return this; } }
		public IActualCellProtectionInfo ActualProtection { get { return this; } }
		public string ActualFormatString { get { return FormatInfo.ActualFormatString; } }
		public int ActualFormatIndex { get { return FormatInfo.ActualFormatIndex; } }
		#region IActualRunFontInfo Members
		string IActualRunFontInfo.Name { get { return FormatInfo.ActualFont.Name; } }
		Color IActualRunFontInfo.Color { get { return FormatInfo.ActualFont.Color; } }
		bool IActualRunFontInfo.Bold { get { return FormatInfo.ActualFont.Bold; } }
		bool IActualRunFontInfo.Condense { get { return FormatInfo.ActualFont.Condense; } }
		bool IActualRunFontInfo.Extend { get { return FormatInfo.ActualFont.Extend; } }
		bool IActualRunFontInfo.Italic { get { return FormatInfo.ActualFont.Italic; } }
		bool IActualRunFontInfo.Outline { get { return FormatInfo.ActualFont.Outline; } }
		bool IActualRunFontInfo.Shadow { get { return FormatInfo.ActualFont.Shadow; } }
		bool IActualRunFontInfo.StrikeThrough { get { return FormatInfo.ActualFont.StrikeThrough; } }
		int IActualRunFontInfo.Charset { get { return FormatInfo.ActualFont.Charset; } }
		int IActualRunFontInfo.FontFamily { get { return FormatInfo.ActualFont.FontFamily; } }
		double IActualRunFontInfo.Size { get { return FormatInfo.ActualFont.Size; } }
		XlFontSchemeStyles IActualRunFontInfo.SchemeStyle { get { return FormatInfo.ActualFont.SchemeStyle; } }
		XlScriptType IActualRunFontInfo.Script { get { return FormatInfo.ActualFont.Script; } }
		XlUnderlineType IActualRunFontInfo.Underline { get { return FormatInfo.ActualFont.Underline; } }
		int IActualRunFontInfo.ColorIndex { get { return FormatInfo.ActualFont.ColorIndex; } }
		FontInfo IActualRunFontInfo.GetFontInfo() {
			return FormatInfo.ActualFont.GetFontInfo();
		}
		#endregion
		#region IActualFillInfo Members
		XlPatternType IActualFillInfo.PatternType { get { return FormatInfo.ActualFill.PatternType; } }
		Color IActualFillInfo.ForeColor { get { return FormatInfo.ActualFill.ForeColor; } }
		Color IActualFillInfo.BackColor { get { return FormatInfo.ActualFill.BackColor; } }
		int IActualFillInfo.ForeColorIndex { get { return FormatInfo.ActualFill.ForeColorIndex; } }
		int IActualFillInfo.BackColorIndex { get { return FormatInfo.ActualFill.BackColorIndex; } }
		bool IActualFillInfo.ApplyPatternType { get { return true; } }
		bool IActualFillInfo.ApplyForeColor { get { return true; } }
		bool IActualFillInfo.ApplyBackColor { get { return true; } }
		bool IActualFillInfo.IsDifferential { get { return false; } }
		IActualGradientFillInfo IActualFillInfo.GradientFill { get { return FormatInfo.ActualFill.GradientFill; } }
		IActualConvergenceInfo IActualGradientFillInfo.Convergence { get { return FormatInfo.ActualFill.GradientFill.Convergence; } }
		ModelFillType IActualFillInfo.FillType { get { return FormatInfo.ActualFill.FillType; } }
		ModelGradientFillType IActualGradientFillInfo.Type { get { return FormatInfo.ActualFill.GradientFill.Type; } }
		double IActualGradientFillInfo.Degree { get { return FormatInfo.ActualFill.GradientFill.Degree; } }
		IActualGradientStopCollection IActualGradientFillInfo.GradientStops { get { return FormatInfo.ActualFill.GradientFill.GradientStops; } }
		float IActualConvergenceInfo.Left { get { return FormatInfo.ActualFill.GradientFill.Convergence.Left; } }
		float IActualConvergenceInfo.Right { get { return FormatInfo.ActualFill.GradientFill.Convergence.Right; } }
		float IActualConvergenceInfo.Top { get { return FormatInfo.ActualFill.GradientFill.Convergence.Top; } }
		float IActualConvergenceInfo.Bottom { get { return FormatInfo.ActualFill.GradientFill.Convergence.Bottom; } }
		#endregion
		#region IActualCellAlignmentInfo Members
		bool IActualCellAlignmentInfo.WrapText { get { return FormatInfo.ActualAlignment.WrapText; } }
		bool IActualCellAlignmentInfo.JustifyLastLine { get { return FormatInfo.ActualAlignment.JustifyLastLine; } }
		bool IActualCellAlignmentInfo.ShrinkToFit { get { return FormatInfo.ActualAlignment.ShrinkToFit; } }
		int IActualCellAlignmentInfo.TextRotation { get { return FormatInfo.ActualAlignment.TextRotation; } }
		byte IActualCellAlignmentInfo.Indent { get { return FormatInfo.ActualAlignment.Indent; } }
		int IActualCellAlignmentInfo.RelativeIndent { get { return FormatInfo.ActualAlignment.RelativeIndent; } }
		XlHorizontalAlignment IActualCellAlignmentInfo.Horizontal { get { return FormatInfo.ActualAlignment.Horizontal; } }
		XlVerticalAlignment IActualCellAlignmentInfo.Vertical { get { return FormatInfo.ActualAlignment.Vertical; } }
		XlReadingOrder IActualCellAlignmentInfo.ReadingOrder { get { return FormatInfo.ActualAlignment.ReadingOrder; } }
		#endregion
		#region IActualBorderInfo Members
		XlBorderLineStyle IActualBorderInfo.LeftLineStyle { get { return FormatInfo.ActualBorder.LeftLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.RightLineStyle { get { return FormatInfo.ActualBorder.RightLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.TopLineStyle { get { return FormatInfo.ActualBorder.TopLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.BottomLineStyle { get { return FormatInfo.ActualBorder.BottomLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.HorizontalLineStyle { get { return FormatInfo.ActualBorder.HorizontalLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.VerticalLineStyle { get { return FormatInfo.ActualBorder.VerticalLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.DiagonalUpLineStyle { get { return FormatInfo.ActualBorder.DiagonalUpLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.DiagonalDownLineStyle { get { return FormatInfo.ActualBorder.DiagonalDownLineStyle; } }
		Color IActualBorderInfo.LeftColor { get { return FormatInfo.ActualBorder.LeftColor; } }
		Color IActualBorderInfo.RightColor { get { return FormatInfo.ActualBorder.RightColor; } }
		Color IActualBorderInfo.TopColor { get { return FormatInfo.ActualBorder.TopColor; } }
		Color IActualBorderInfo.BottomColor { get { return FormatInfo.ActualBorder.BottomColor; } }
		Color IActualBorderInfo.HorizontalColor { get { return FormatInfo.ActualBorder.HorizontalColor; } }
		Color IActualBorderInfo.VerticalColor { get { return FormatInfo.ActualBorder.VerticalColor; } }
		Color IActualBorderInfo.DiagonalColor { get { return FormatInfo.ActualBorder.DiagonalColor; } }
		int IActualBorderInfo.LeftColorIndex { get { return FormatInfo.ActualBorder.LeftColorIndex; } }
		int IActualBorderInfo.RightColorIndex { get { return FormatInfo.ActualBorder.RightColorIndex; } }
		int IActualBorderInfo.TopColorIndex { get { return FormatInfo.ActualBorder.TopColorIndex; } }
		int IActualBorderInfo.BottomColorIndex { get { return FormatInfo.ActualBorder.BottomColorIndex; } }
		int IActualBorderInfo.HorizontalColorIndex { get { return FormatInfo.ActualBorder.HorizontalColorIndex; } }
		int IActualBorderInfo.VerticalColorIndex { get { return FormatInfo.ActualBorder.VerticalColorIndex; } }
		int IActualBorderInfo.DiagonalColorIndex { get { return FormatInfo.ActualBorder.DiagonalColorIndex; } }
		bool IActualBorderInfo.Outline { get { return FormatInfo.ActualBorder.Outline; } }
		#endregion
		#region IActualCellProtectionInfo Members
		bool IActualCellProtectionInfo.Locked { get { return FormatInfo.ActualProtection.Locked; } }
		bool IActualCellProtectionInfo.Hidden { get { return FormatInfo.ActualProtection.Hidden; } }
		#endregion
		#region IActualApplyInfo Members
		public IActualApplyInfo ActualApplyInfo { get { return this; } }
		bool IActualApplyInfo.ApplyFont { get { return FormatInfo.ActualApplyInfo.ApplyFont; } }
		bool IActualApplyInfo.ApplyFill { get { return FormatInfo.ActualApplyInfo.ApplyFill; } }
		bool IActualApplyInfo.ApplyBorder { get { return FormatInfo.ActualApplyInfo.ApplyBorder; } }
		bool IActualApplyInfo.ApplyAlignment { get { return FormatInfo.ActualApplyInfo.ApplyAlignment; } }
		bool IActualApplyInfo.ApplyProtection { get { return FormatInfo.ActualApplyInfo.ApplyProtection; } }
		bool IActualApplyInfo.ApplyNumberFormat { get { return FormatInfo.ActualApplyInfo.ApplyNumberFormat; } }
		#endregion
		#endregion
		#region IFormatBase Members
		public IRunFontInfo Font { get { return this; } }
		public IFillInfo Fill { get { return this; } }
		public ICellAlignmentInfo Alignment { get { return this; } }
		public IBorderInfo Border { get { return this; } }
		public ICellProtectionInfo Protection { get { return this; } }
		public string FormatString {
			get { return FormatInfo.FormatString; }
			set {
				if (FormatInfo.FormatString == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetNumberFormat, value);
			}
		}
		DocumentModelChangeActions SetNumberFormat(FormatBase info, string value) {
			info.FormatString = value;
			return DocumentModelChangeActions.None; 
		}
		#region IRunFontInfo Members
		#region IRunFontInfo.Name
		string IRunFontInfo.Name {
			get { return FormatInfo.Font.Name; }
			set {
				if (FormatInfo.Font.Name == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontName, value);
			}
		}
		DocumentModelChangeActions SetFontName(FormatBase info, string value) {
			info.Font.Name = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Color
		Color IRunFontInfo.Color {
			get { return FormatInfo.Font.Color; }
			set {
				if (FormatInfo.Font.Color == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontColor, value);
			}
		}
		DocumentModelChangeActions SetFontColor(FormatBase info, Color value) {
			info.Font.Color = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Bold
		bool IRunFontInfo.Bold {
			get { return FormatInfo.Font.Bold; }
			set {
				if (FormatInfo.Font.Bold == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontBold, value);
			}
		}
		DocumentModelChangeActions SetFontBold(FormatBase info, bool value) {
			info.Font.Bold = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Condense
		bool IRunFontInfo.Condense {
			get { return FormatInfo.Font.Condense; }
			set {
				if (FormatInfo.Font.Condense == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontCondense, value);
			}
		}
		DocumentModelChangeActions SetFontCondense(FormatBase info, bool value) {
			info.Font.Condense = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Extend
		bool IRunFontInfo.Extend {
			get { return FormatInfo.Font.Extend; }
			set {
				if (FormatInfo.Font.Extend == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontExtend, value);
			}
		}
		DocumentModelChangeActions SetFontExtend(FormatBase info, bool value) {
			info.Font.Extend = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Italic
		bool IRunFontInfo.Italic {
			get { return FormatInfo.Font.Italic; }
			set {
				if (FormatInfo.Font.Italic == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontItalic, value);
			}
		}
		DocumentModelChangeActions SetFontItalic(FormatBase info, bool value) {
			info.Font.Italic = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Outline
		bool IRunFontInfo.Outline {
			get { return FormatInfo.Font.Outline; }
			set {
				if (FormatInfo.Font.Outline == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontOutline, value);
			}
		}
		DocumentModelChangeActions SetFontOutline(FormatBase info, bool value) {
			info.Font.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Shadow
		bool IRunFontInfo.Shadow {
			get { return FormatInfo.Font.Shadow; }
			set {
				if (FormatInfo.Font.Shadow == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontShadow, value);
			}
		}
		DocumentModelChangeActions SetFontShadow(FormatBase info, bool value) {
			info.Font.Shadow = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.StrikeThrough
		bool IRunFontInfo.StrikeThrough {
			get { return FormatInfo.Font.StrikeThrough; }
			set {
				if (FormatInfo.Font.StrikeThrough == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontStrikeThrough, value);
			}
		}
		DocumentModelChangeActions SetFontStrikeThrough(FormatBase info, bool value) {
			info.Font.StrikeThrough = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Charset
		int IRunFontInfo.Charset {
			get { return FormatInfo.Font.Charset; }
			set {
				if (FormatInfo.Font.Charset == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontCharset, value);
			}
		}
		DocumentModelChangeActions SetFontCharset(FormatBase info, int value) {
			info.Font.Charset = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.FontFamily
		int IRunFontInfo.FontFamily {
			get { return FormatInfo.Font.FontFamily; }
			set {
				if (FormatInfo.Font.FontFamily == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontFamily, value);
			}
		}
		DocumentModelChangeActions SetFontFamily(FormatBase info, int value) {
			info.Font.FontFamily = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Size
		double IRunFontInfo.Size {
			get { return FormatInfo.Font.Size; }
			set {
				if (FormatInfo.Font.Size == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontSize, value);
			}
		}
		DocumentModelChangeActions SetFontSize(FormatBase info, double value) {
			info.Font.Size = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.SchemeStyle
		XlFontSchemeStyles IRunFontInfo.SchemeStyle {
			get { return FormatInfo.Font.SchemeStyle; }
			set {
				if (FormatInfo.Font.SchemeStyle == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontSchemeStyle, value);
			}
		}
		DocumentModelChangeActions SetFontSchemeStyle(FormatBase info, XlFontSchemeStyles value) {
			info.Font.SchemeStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Script
		XlScriptType IRunFontInfo.Script {
			get { return FormatInfo.Font.Script; }
			set {
				if (FormatInfo.Font.Script == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontScript, value);
			}
		}
		DocumentModelChangeActions SetFontScript(FormatBase info, XlScriptType value) {
			info.Font.Script = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Underline
		XlUnderlineType IRunFontInfo.Underline {
			get { return FormatInfo.Font.Underline; }
			set {
				if (FormatInfo.Font.Underline == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFontUnderline, value);
			}
		}
		DocumentModelChangeActions SetFontUnderline(FormatBase info, XlUnderlineType value) {
			info.Font.Underline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IFillInfo Members
		#region IFillInfo.Clear
		void IFillInfo.Clear() {
			if (FormatInfo.ApplyFill)
				ClearFillCore();
		}
		void ClearFillCore() {
			DocumentModel.BeginUpdate();
			try {
				CellStyleFormat info = GetInfoForModification(cellStyleFormatIndexAccessor);
				info.Fill.Clear();
				ReplaceInfo(cellStyleFormatIndexAccessor, info, DocumentModelChangeActions.None);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region IFillInfo.PatternType
		XlPatternType IFillInfo.PatternType {
			get { return FormatInfo.Fill.PatternType; }
			set {
				if (FormatInfo.Fill.PatternType == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFillPatternType, value);
			}
		}
		DocumentModelChangeActions SetFillPatternType(FormatBase info, XlPatternType value) {
			info.Fill.PatternType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IFillInfo.ForeColor
		Color IFillInfo.ForeColor {
			get { return FormatInfo.Fill.ForeColor; }
			set {
				if (FormatInfo.Fill.ForeColor == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFillForeColor, value);
			}
		}
		DocumentModelChangeActions SetFillForeColor(FormatBase info, Color value) {
			info.Fill.ForeColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IFillInfo.BackColor
		Color IFillInfo.BackColor {
			get { return FormatInfo.Fill.BackColor; }
			set {
				if (FormatInfo.Fill.BackColor == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetFillBackColor, value);
			}
		}
		DocumentModelChangeActions SetFillBackColor(FormatBase info, Color value) {
			info.Fill.BackColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IFillInfo.GradientFill
		IGradientFillInfo IFillInfo.GradientFill { get { return this; } }
		#endregion
		#region IFillInfo.FillType
		ModelFillType IFillInfo.FillType {
			get { return FormatInfo.Fill.FillType; }
			set {
				if (FormatInfo.Fill.FillType == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetModelFillType, value);
			}
		}
		DocumentModelChangeActions SetModelFillType(FormatBase info, ModelFillType value) {
			info.Fill.FillType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IGradientFillInfo Members
		#region IGradientFillInfo.Convergence
		IConvergenceInfo IGradientFillInfo.Convergence { get { return this; } }
		#endregion
		#region IGradientFillInfo.GradientStops
		IGradientStopCollection IGradientFillInfo.GradientStops { get { return FormatInfo.Fill.GradientFill.GradientStops; } }
		#endregion
		#region IGradientFillInfo.Type
		ModelGradientFillType IGradientFillInfo.Type {
			get { return FormatInfo.Fill.GradientFill.Type; }
			set {
				if (FormatInfo.Fill.GradientFill.Type == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetGradientFillInfoType, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoType(FormatBase info, ModelGradientFillType value) {
			info.Fill.GradientFill.Type = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IGradientFillInfo.Degree
		double IGradientFillInfo.Degree {
			get { return FormatInfo.Fill.GradientFill.Degree; }
			set {
				if (FormatInfo.Fill.GradientFill.Degree == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetGradientFillInfoDegree, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoDegree(FormatBase info, double value) {
			info.Fill.GradientFill.Degree = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo Members
		#region IConvergenceInfo.Left
		float IConvergenceInfo.Left {
			get { return FormatInfo.Fill.GradientFill.Convergence.Left; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Left == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetGradientFillInfoLeft, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoLeft(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Left = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Right
		float IConvergenceInfo.Right {
			get { return FormatInfo.Fill.GradientFill.Convergence.Right; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Right == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetGradientFillInfoRight, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoRight(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Right = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Top
		float IConvergenceInfo.Top {
			get { return FormatInfo.Fill.GradientFill.Convergence.Top; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Top == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetGradientFillInfoTop, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoTop(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Top = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Bottom
		float IConvergenceInfo.Bottom {
			get { return FormatInfo.Fill.GradientFill.Convergence.Bottom; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Bottom == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetGradientFillInfoBottom, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoBottom(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Bottom = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		#endregion
		#region ICellAlignmentInfo Members
		#region ICellAlignmentInfo.WrapText
		bool ICellAlignmentInfo.WrapText {
			get { return FormatInfo.Alignment.WrapText; }
			set {
				if (FormatInfo.Alignment.WrapText == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetAlignmentWrapText, value);
			}
		}
		DocumentModelChangeActions SetAlignmentWrapText(FormatBase info, bool value) {
			info.Alignment.WrapText = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.JustifyLastLine
		bool ICellAlignmentInfo.JustifyLastLine {
			get { return FormatInfo.Alignment.JustifyLastLine; }
			set {
				if (FormatInfo.Alignment.JustifyLastLine == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetAlignmentJustifyLastLine, value);
			}
		}
		DocumentModelChangeActions SetAlignmentJustifyLastLine(FormatBase info, bool value) {
			info.Alignment.JustifyLastLine = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.ShrinkToFit
		bool ICellAlignmentInfo.ShrinkToFit {
			get { return FormatInfo.Alignment.ShrinkToFit; }
			set {
				if (FormatInfo.Alignment.ShrinkToFit == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetAlignmentShrinkToFit, value);
			}
		}
		DocumentModelChangeActions SetAlignmentShrinkToFit(FormatBase info, bool value) {
			info.Alignment.ShrinkToFit = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.TextRotation
		int ICellAlignmentInfo.TextRotation {
			get { return FormatInfo.Alignment.TextRotation; }
			set {
				if (FormatInfo.Alignment.TextRotation == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetAlignmentTextRotation, value);
			}
		}
		DocumentModelChangeActions SetAlignmentTextRotation(FormatBase info, int value) {
			info.Alignment.TextRotation = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Indent
		byte ICellAlignmentInfo.Indent {
			get { return FormatInfo.Alignment.Indent; }
			set {
				if (FormatInfo.Alignment.Indent == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetAlignmentIndent, value);
			}
		}
		DocumentModelChangeActions SetAlignmentIndent(FormatBase info, byte value) {
			info.Alignment.Indent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.RelativeIndent
		int ICellAlignmentInfo.RelativeIndent {
			get { return FormatInfo.Alignment.RelativeIndent; }
			set {
				if (FormatInfo.Alignment.RelativeIndent == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetAlignmentRelativeIndent, value);
			}
		}
		DocumentModelChangeActions SetAlignmentRelativeIndent(FormatBase info, int value) {
			info.Alignment.RelativeIndent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Horizontal
		XlHorizontalAlignment ICellAlignmentInfo.Horizontal {
			get { return FormatInfo.Alignment.Horizontal; }
			set {
				if (FormatInfo.Alignment.Horizontal == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetAlignmentHorizontal, value);
			}
		}
		DocumentModelChangeActions SetAlignmentHorizontal(FormatBase info, XlHorizontalAlignment value) {
			info.Alignment.Horizontal = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Vertical
		XlVerticalAlignment ICellAlignmentInfo.Vertical {
			get { return FormatInfo.Alignment.Vertical; }
			set {
				if (FormatInfo.Alignment.Vertical == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetAlignmentVertical, value);
			}
		}
		DocumentModelChangeActions SetAlignmentVertical(FormatBase info, XlVerticalAlignment value) {
			info.Alignment.Vertical = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.XlReadingOrder
		XlReadingOrder ICellAlignmentInfo.ReadingOrder {
			get { return FormatInfo.Alignment.ReadingOrder; }
			set {
				if (FormatInfo.Alignment.ReadingOrder == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetAlignmentReadingOrder, value);
			}
		}
		DocumentModelChangeActions SetAlignmentReadingOrder(FormatBase info, XlReadingOrder value) {
			info.Alignment.ReadingOrder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IBorderInfo Members
		#region IBorderInfo.LeftLineStyle
		XlBorderLineStyle IBorderInfo.LeftLineStyle {
			get { return FormatInfo.Border.LeftLineStyle; }
			set {
				if (FormatInfo.Border.LeftLineStyle == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderLeftLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.LeftLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.LeftColor
		Color IBorderInfo.LeftColor {
			get { return FormatInfo.Border.LeftColor; }
			set {
				if (FormatInfo.Border.LeftColor == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderLeftColor, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftColor(FormatBase info, Color value) {
			info.Border.LeftColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.LeftColorIndex
		int IBorderInfo.LeftColorIndex {
			get { return FormatInfo.Border.LeftColorIndex; }
			set {
				if (FormatInfo.Border.LeftColorIndex == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderLeftColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftColorIndex(FormatBase info, int value) {
			info.Border.LeftColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.RightLineStyle
		XlBorderLineStyle IBorderInfo.RightLineStyle {
			get { return FormatInfo.Border.RightLineStyle; }
			set {
				if (FormatInfo.Border.RightLineStyle == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderRightLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderRightLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.RightLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.RightColor
		Color IBorderInfo.RightColor {
			get { return FormatInfo.Border.RightColor; }
			set {
				if (FormatInfo.Border.RightColor == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderRightColor, value);
			}
		}
		DocumentModelChangeActions SetBorderRightColor(FormatBase info, Color value) {
			info.Border.RightColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.RightColorIndex
		int IBorderInfo.RightColorIndex {
			get { return FormatInfo.Border.RightColorIndex; }
			set {
				if (FormatInfo.Border.RightColorIndex == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderRightColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderRightColorIndex(FormatBase info, int value) {
			info.Border.RightColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.TopLineStyle
		XlBorderLineStyle IBorderInfo.TopLineStyle {
			get { return FormatInfo.Border.TopLineStyle; }
			set {
				if (FormatInfo.Border.TopLineStyle == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderTopLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderTopLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.TopLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.TopColor
		Color IBorderInfo.TopColor {
			get { return FormatInfo.Border.TopColor; }
			set {
				if (FormatInfo.Border.TopColor == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderTopColor, value);
			}
		}
		DocumentModelChangeActions SetBorderTopColor(FormatBase info, Color value) {
			info.Border.TopColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.TopColorIndex
		int IBorderInfo.TopColorIndex {
			get { return FormatInfo.Border.TopColorIndex; }
			set {
				if (FormatInfo.Border.TopColorIndex == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderTopColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderTopColorIndex(FormatBase info, int value) {
			info.Border.TopColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.BottomLineStyle
		XlBorderLineStyle IBorderInfo.BottomLineStyle {
			get { return FormatInfo.Border.BottomLineStyle; }
			set {
				if (FormatInfo.Border.BottomLineStyle == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderBottomLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.BottomLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.BottomColor
		Color IBorderInfo.BottomColor {
			get { return FormatInfo.Border.BottomColor; }
			set {
				if (FormatInfo.Border.BottomColor == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderBottomColor, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomColor(FormatBase info, Color value) {
			info.Border.BottomColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.BottomColorIndex
		int IBorderInfo.BottomColorIndex {
			get { return FormatInfo.Border.BottomColorIndex; }
			set {
				if (FormatInfo.Border.BottomColorIndex == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderBottomColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomColorIndex(FormatBase info, int value) {
			info.Border.BottomColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.HorizontalLineStyle
		XlBorderLineStyle IBorderInfo.HorizontalLineStyle {
			get { return FormatInfo.Border.HorizontalLineStyle; }
			set {
				if (FormatInfo.Border.HorizontalLineStyle == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderHorizontalLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.HorizontalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.HorizontalColor
		Color IBorderInfo.HorizontalColor {
			get { return FormatInfo.Border.HorizontalColor; }
			set {
				if (FormatInfo.Border.HorizontalColor == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderHorizontalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalColor(FormatBase info, Color value) {
			info.Border.HorizontalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.HorizontalColorIndex
		int IBorderInfo.HorizontalColorIndex {
			get { return FormatInfo.Border.HorizontalColorIndex; }
			set {
				if (FormatInfo.Border.HorizontalColorIndex == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderHorizontalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalColorIndex(FormatBase info, int value) {
			info.Border.HorizontalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.VerticalLineStyle
		XlBorderLineStyle IBorderInfo.VerticalLineStyle {
			get { return FormatInfo.Border.VerticalLineStyle; }
			set {
				if (FormatInfo.Border.VerticalLineStyle == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderVerticalLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.VerticalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.VerticalColor
		Color IBorderInfo.VerticalColor {
			get { return FormatInfo.Border.VerticalColor; }
			set {
				if (FormatInfo.Border.VerticalColor == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderVerticalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalColor(FormatBase info, Color value) {
			info.Border.VerticalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.VerticalColorIndex
		int IBorderInfo.VerticalColorIndex {
			get { return FormatInfo.Border.VerticalColorIndex; }
			set {
				if (FormatInfo.Border.VerticalColorIndex == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderVerticalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalColorIndex(FormatBase info, int value) {
			info.Border.VerticalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalUpLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalUpLineStyle {
			get { return FormatInfo.Border.DiagonalUpLineStyle; }
			set {
				if (FormatInfo.Border.DiagonalUpLineStyle == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderDiagonalUpLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalUpLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalUpLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalDownLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalDownLineStyle {
			get { return FormatInfo.Border.DiagonalDownLineStyle; }
			set {
				if (FormatInfo.Border.DiagonalDownLineStyle == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderDiagonalDownLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalDownLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalDownLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalColor
		Color IBorderInfo.DiagonalColor {
			get { return FormatInfo.Border.DiagonalColor; }
			set {
				if (FormatInfo.Border.DiagonalColor == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderDiagonalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalColor(FormatBase info, Color value) {
			info.Border.DiagonalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalColorIndex
		int IBorderInfo.DiagonalColorIndex {
			get { return FormatInfo.Border.DiagonalColorIndex; }
			set {
				if (FormatInfo.Border.DiagonalColorIndex == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderDiagonalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalColorIndex(FormatBase info, int value) {
			info.Border.DiagonalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.Outline
		bool IBorderInfo.Outline {
			get { return FormatInfo.Border.Outline; }
			set {
				if (FormatInfo.Border.Outline == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetBorderOutline, value);
			}
		}
		DocumentModelChangeActions SetBorderOutline(FormatBase info, bool value) {
			info.Border.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region ICellProtectionInfo Members
		#region ICellProtectionInfo.Locked
		bool ICellProtectionInfo.Locked {
			get { return FormatInfo.Protection.Locked; }
			set {
				if (FormatInfo.Protection.Locked == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetProtectionLocked, value);
			}
		}
		DocumentModelChangeActions SetProtectionLocked(FormatBase info, bool value) {
			info.Protection.Locked = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellProtectionInfo.Hidden
		bool ICellProtectionInfo.Hidden {
			get { return FormatInfo.Protection.Hidden; }
			set {
				if (FormatInfo.Protection.Hidden == value)
					return;
				BeginUpdate();
				SetPropertyValue(cellStyleFormatIndexAccessor, SetProtectionHidden, value);
				EndUpdate();
			}
		}
		DocumentModelChangeActions SetProtectionHidden(FormatBase info, bool value) {
			info.Protection.Hidden = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		#region Applies
		#region Applies.HasExtension
		public bool HasExtension {
			get { return FormatInfo.HasExtension; }
			set {
				if (FormatInfo.HasExtension == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetHasExtensionCore, value);
			}
		}
		DocumentModelChangeActions SetHasExtensionCore(FormatBase info, bool value) {
			((CellFormatBase)info).HasExtension = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.QuotePrefix
		public bool QuotePrefix {
			get { return FormatInfo.QuotePrefix; }
			set {
				if (FormatInfo.QuotePrefix == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetQuotePrefixCore, value);
			}
		}
		DocumentModelChangeActions SetQuotePrefixCore(FormatBase info, bool value) {
			((CellFormatBase)info).QuotePrefix = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.PivotButton
		public bool PivotButton {
			get { return FormatInfo.PivotButton; }
			set {
				if (FormatInfo.PivotButton == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetPivotButtonCore, value);
			}
		}
		DocumentModelChangeActions SetPivotButtonCore(FormatBase info, bool value) {
			((CellFormatBase)info).PivotButton = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyNumberFormat
		public bool ApplyNumberFormat {
			get { return FormatInfo.ApplyNumberFormat; }
			set {
				if (FormatInfo.ApplyNumberFormat == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetApplyNumberFormatCore, value);
			}
		}
		DocumentModelChangeActions SetApplyNumberFormatCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyNumberFormat = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyFont
		public bool ApplyFont {
			get { return FormatInfo.ApplyFont; }
			set {
				if (FormatInfo.ApplyFont == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetApplyFontCore, value);
			}
		}
		DocumentModelChangeActions SetApplyFontCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyFont = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyFill
		public bool ApplyFill {
			get { return FormatInfo.ApplyFill; }
			set {
				if (FormatInfo.ApplyFill == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetApplyFillCore, value);
			}
		}
		DocumentModelChangeActions SetApplyFillCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyFill = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyBorder
		public bool ApplyBorder {
			get { return FormatInfo.ApplyBorder; }
			set {
				if (FormatInfo.ApplyBorder == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetApplyBorderCore, value);
			}
		}
		DocumentModelChangeActions SetApplyBorderCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyBorder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyAlignment
		public bool ApplyAlignment {
			get { return FormatInfo.ApplyAlignment; }
			set {
				if (FormatInfo.ApplyAlignment == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetApplyAlignmentCore, value);
			}
		}
		DocumentModelChangeActions SetApplyAlignmentCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyAlignment = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyProtection
		public bool ApplyProtection {
			get { return FormatInfo.ApplyProtection; }
			set {
				if (FormatInfo.ApplyProtection == value)
					return;
				SetPropertyValue(cellStyleFormatIndexAccessor, SetApplyProtectionCore, value);
			}
		}
		DocumentModelChangeActions SetApplyProtectionCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyProtection = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		internal void AssignCellStyleFormatIndex(int value) {
			this.formatIndex = value;
			OnChanged();
		}
		public abstract CellStyleBase Clone(DocumentModel targetDocumentModel, string newStyleName);
		protected internal CellStyleFormat GetTargetCellStyleFormat(DocumentModel targetDocumentModel) {
			CellStyleFormat targetCellStyleFormat = new CellStyleFormat(targetDocumentModel);
			this.BeginUpdate();
			targetCellStyleFormat.BeginUpdate();
			try {
				targetCellStyleFormat.CopyFromDeferred(this.FormatInfo);
			} finally {
				targetCellStyleFormat.EndUpdate();
				this.EndUpdate();
			}
			return targetCellStyleFormat;
		}
		#region IncludeOperations
		#region IncludeNumberFormat
		public void IncludeNumberFormat(bool include) {
			IncludeCore(include, ApplyNumberFormat, SetOwnApplyNumberFormat, ApplyCellNumberFormat, CopyStyleNumberFormat);
		}
		void SetOwnApplyNumberFormat(bool value) {
			ApplyNumberFormat = value;
		}
		bool ApplyCellNumberFormat(CellFormat format) {
			return format.ApplyNumberFormat;
		}
		void CopyStyleNumberFormat(CellFormat format) {
			format.CopyNumberFormat(FormatInfo.NumberFormatInfo);
		}
		#endregion
		#region IncludeFont
		public void IncludeFont(bool include) {
			IncludeCore(include, ApplyFont, SetOwnApplyFont, ApplyCellFont, CopyStyleFont);
		}
		void SetOwnApplyFont(bool value) {
			ApplyFont = value;
		}
		bool ApplyCellFont(CellFormat format) {
			return format.ApplyFont;
		}
		void CopyStyleFont(CellFormat format) {
			format.CopyFont(FormatInfo.FontInfo);
		}
		#endregion
		#region IncludeFill
		public void IncludeFill(bool include) {
			IncludeCore(include, ApplyFill, SetOwnApplyFill, ApplyCellFill, CopyStyleFill);
		}
		void SetOwnApplyFill(bool value) {
			ApplyFill = value;
		}
		bool ApplyCellFill(CellFormat format) {
			return format.ApplyFill;
		}
		void CopyStyleFill(CellFormat format) {
			format.CopyFill(FormatInfo);
		}
		#endregion
		#region IncludeBorder
		public void IncludeBorder(bool include) {
			IncludeCore(include, ApplyBorder, SetOwnApplyBorder, ApplyCellBorder, CopyStyleBorder);
		}
		void SetOwnApplyBorder(bool value) {
			ApplyBorder = value;
		}
		bool ApplyCellBorder(CellFormat format) {
			return format.ApplyBorder;
		}
		void CopyStyleBorder(CellFormat format) {
			format.CopyBorder(FormatInfo.BorderInfo);
		}
		#endregion
		#region IncludeAlignment
		public void IncludeAlignment(bool include) {
			IncludeCore(include, ApplyAlignment, SetOwnApplyAlignment, ApplyCellAlignment, CopyStyleAlignment);
		}
		void SetOwnApplyAlignment(bool value) {
			ApplyAlignment = value;
		}
		bool ApplyCellAlignment(CellFormat format) {
			return format.ApplyAlignment;
		}
		void CopyStyleAlignment(CellFormat format) {
			format.CopyAlignment(FormatInfo.AlignmentInfo);
		}
		#endregion
		#region IncludeProtection
		public void IncludeProtection(bool include) {
			IncludeCore(include, ApplyProtection, SetOwnApplyProtection, ApplyCellProtection, CopyStyleProtection);
		}
		void SetOwnApplyProtection(bool value) {
			ApplyProtection = value;
		}
		bool ApplyCellProtection(CellFormat format) {
			return format.ApplyProtection;
		}
		void CopyStyleProtection(CellFormat format) {
			format.CopyProtection(FormatInfo);
		}
		#endregion
		delegate bool ApplyCellFormatting(CellFormat format);
		void IncludeCore(bool include, bool applyStyleFormatting, Action<bool> setOwnApply, ApplyCellFormatting applyCellFormatting, Action<CellFormat> copyFormattingTo) {
			if (IsHidden || !IsRegistered || (!include && !applyStyleFormatting))
				return;
			DocumentModel.BeginUpdate();
			try {
				CopyThisFormatting(applyCellFormatting, copyFormattingTo);
				setOwnApply(include);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		void CopyThisFormatting(ApplyCellFormatting applyCellFormatting, Action<CellFormat> copyFormattingTo) {
			CellFormatCache cache = documentModel.Cache.CellFormatCache;
			int count = cache.Count;
			for (int i = 0; i < count; i++) {
				CellFormat format = cache[i] as CellFormat;
				if (format != null && format.StyleIndex == styleIndex && !applyCellFormatting(format))
					copyFormattingTo(format);
			}
		}
		#endregion
		protected internal virtual bool Reset() {
			return false;
		}
	}
	#endregion
	#region CustomCellStyle
	public class CustomCellStyle : CellStyleBase {
		public CustomCellStyle(DocumentModel documentModel, string customName)
			: base(documentModel, customName) {
			AssignCellStyleFormatIndex(DocumentModel.StyleSheet.CellStyles.Normal.FormatIndex);
		}
		public CustomCellStyle(DocumentModel documentModel, string customName, CellStyleFormat styleFormat)
			: base(documentModel, customName) {
			AssignCellStyleFormatIndex(DocumentModel.Cache.CellFormatCache.AddItem(styleFormat));
		}
		#region Properties
		public override bool IsBuiltIn { get { return false; } }
		public override int OutlineLevel { get { return 0; } }
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			CustomCellStyle value = obj as CustomCellStyle;
			if (value == null)
				return false;
			return base.Equals(value);
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ GetType().GetHashCode();
		}
		#endregion
		public override CellStyleBase Clone(DocumentModel targetDocumentModel, string targetStyleName) {
			CellStyleFormat targetCellStyleFormat = GetTargetCellStyleFormat(targetDocumentModel);
			CustomCellStyle targetStyle = new CustomCellStyle(targetDocumentModel, targetStyleName, targetCellStyleFormat);
			return targetStyle;
		}
	}
	#endregion
	#region BuiltInCellStyle
	public class BuiltInCellStyle : CellStyleBase {
		readonly int builtInId;
		bool customBuiltIn;
		public BuiltInCellStyle(DocumentModel documentModel, int builtInId)
			: this(documentModel, builtInId, BuiltInCellStyleCalculator.CreateStyleFormat(documentModel, builtInId)) {
		}
		public BuiltInCellStyle(DocumentModel documentModel, int builtInId, CellStyleFormat styleFormat)
			: base(documentModel, BuiltInCellStyleCalculator.CalculateName(builtInId)) {
			this.builtInId = builtInId;
			AssignCellStyleFormatIndex(DocumentModel.Cache.CellFormatCache.AddItem(styleFormat));
		}
		#region Properties
		public int BuiltInId { get { return builtInId; } }
		public bool CustomBuiltIn { get { return customBuiltIn; } }
		public override bool IsBuiltIn { get { return true; } }
		public override int OutlineLevel { get { return 0; } }
		#endregion
		public override bool ReplaceInfo<TInfo>(IIndexAccessor<CellStyleBase, TInfo, DocumentModelChangeActions> indexHolder, TInfo newValue, DocumentModelChangeActions changeActions) {
			bool result = base.ReplaceInfo(indexHolder, newValue, changeActions);
			if (IsUpdateLocked) {
				if (!customBuiltIn)
					SetCustomBuiltIn(true);
				return false;
			}
			if (result && !customBuiltIn)
				SetCustomBuiltIn(true);
			return result;
		}
		void SetCustomBuiltIn(bool value) {
			if (IsRegistered && !IsHidden) {
				HistoryItem item = new ActionHistoryItem<bool>(DocumentModel, customBuiltIn, value, SetCustomBuiltInCore);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		protected internal void SetCustomBuiltInCore(bool value) {
			customBuiltIn = value;
		}
		#region Equals
		public override bool Equals(object obj) {
			BuiltInCellStyle value = obj as BuiltInCellStyle;
			if (value == null)
				return false;
			return
				base.Equals(value) &&
				BuiltInId == value.BuiltInId &&
				CustomBuiltIn == value.CustomBuiltIn;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), builtInId, customBuiltIn.GetHashCode());
		}
		#endregion
		public override CellStyleBase Clone(DocumentModel targetDocumentModel, string newStyleName) {
			BuiltInCellStyle targetStyle = new BuiltInCellStyle(targetDocumentModel, this.builtInId);
			targetStyle.SetCustomBuiltInCore(this.CustomBuiltIn);
			targetStyle.CopyFrom(this);
			return targetStyle;
		}
		protected internal override bool Reset() {
			DocumentModel.BeginUpdate();
			bool result = false;
			try {
				CellStyleFormat info = GetInfoForModification(CellStyleFormatIndexAccessor);
				if (BuiltInCellStyleCalculator.IsNormalStyle(builtInId))
					result = info.ResetToDefaultCellFormat();
				else {
					BuiltInCellStyleInfo builtInInfo = BuiltInCellStyleCalculator.GetBuiltInCellStyleInfo(builtInId);
					result = info.ResetToDefaultBuiltInFormat(builtInInfo);
				}
				if (result) {
					if (!IsUpdateLocked)					 
						base.ReplaceInfo(CellStyleFormatIndexAccessor, info, DocumentModelChangeActions.None);
					SetCustomBuiltIn(false);
				}
			} finally {
				DocumentModel.EndUpdate();
			}
			return result;
		}
	}
	#endregion
	#region OutlineCellStyle
	public class OutlineCellStyle : CellStyleBase {
		#region Static Members
		internal static string CalculateName(bool isRow, int outlineLevel) {
			if (outlineLevel <= 0)
				Exceptions.ThrowInternalException();
			return (isRow ? "RowLevel_" : "ColLevel_") + outlineLevel.ToString();
		}
		#endregion
		#region Fields
		readonly int builtInId;
		readonly int outlineLevel;
		bool customBuiltIn;
		#endregion
		public OutlineCellStyle(DocumentModel documentModel, bool isRow, int outlineLevel, CellStyleFormat styleFormat)
			: base(documentModel, CalculateName(isRow, outlineLevel)) {
			this.builtInId = isRow ? 1 : 2;
			this.outlineLevel = outlineLevel;
			AssignCellStyleFormatIndex(DocumentModel.Cache.CellFormatCache.AddItem(styleFormat));
		}
		#region Properties
		public int BuiltInId { get { return builtInId; } }
		public override int OutlineLevel { get { return outlineLevel; } }
		public bool CustomBuiltIn { get { return customBuiltIn; } set { customBuiltIn = value; } }
		public override bool IsBuiltIn { get { return false; } }
		#endregion
		protected override void ApplyChanges(DocumentModelChangeActions actions) {
			if (IsRegistered && !IsHidden)
				CustomBuiltIn = true;
		}
		#region Equals
		public override bool Equals(object obj) {
			OutlineCellStyle value = obj as OutlineCellStyle;
			if (value == null)
				return false;
			return
				base.Equals(value) &&
				BuiltInId == value.BuiltInId &&
				OutlineLevel == value.OutlineLevel &&
				CustomBuiltIn == value.CustomBuiltIn;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), builtInId, outlineLevel, customBuiltIn.GetHashCode());
		}
		#endregion
		public override CellStyleBase Clone(DocumentModel targetDocumentModel, string newStyleName) {
			CellStyleFormat targetCellStyleFormat = GetTargetCellStyleFormat(targetDocumentModel);
			bool isRow = builtInId == 1;
			System.Diagnostics.Debug.Assert(isRow || builtInId == 2);
			OutlineCellStyle targetCellStyle = new OutlineCellStyle(targetDocumentModel, isRow, outlineLevel, targetCellStyleFormat);
			return targetCellStyle;
		}
	}
	#endregion
	#region ImportCellStyleInfo
	public class ImportCellStyleInfo {
		public string Name { get; set; }
		public bool IsHidden { get; set; }
		public int BuiltInId { get; set; }
		public int OutlineLevel { get; set; }
		public int StyleFormatId { get; set; }
		public bool CustomBuiltIn { get; set; }
	}
	#endregion
	#region BuiltInCellStyleInfo
	public class BuiltInCellStyleInfo {
		#region Fields
		readonly string cellStyleName;
		readonly int numberFormatIndex;
		readonly RunFontInfo fontInfo;
		readonly FillInfo fillInfo;
		readonly BorderInfo borderInfo;
		readonly CellFormatFlagsInfo flagsInfo;
		readonly int fontColorKey;
		readonly int fillForeColorKey;
		readonly int fillBackColorKey;
		readonly int leftBorderColorKey;
		readonly int rightBorderColorKey;
		readonly int topBorderColorKey;
		readonly int bottomBorderColorKey;
		#endregion
		public BuiltInCellStyleInfo(string cellStyleName, int numberFormatIndex, bool fontBold, bool fontItalic, bool fontUnderline, int fontSize, string fontName, XlFontSchemeStyles schemeStyles, int fontColorKey, XlPatternType patternType, int fillForeColorKey, int fillBackColorKey, XlBorderLineStyle leftBorder, XlBorderLineStyle rightBorder, XlBorderLineStyle topBorder, XlBorderLineStyle bottomBorder, int leftBorderColorKey, int rightBorderColorKey, int topBorderColorKey, int bottomBorderColorKey, bool applyNumberFormat, bool applyFont, bool applyFill, bool applyBorder, bool applyAlignment, bool applyProtection) {
			this.cellStyleName = cellStyleName;
			this.numberFormatIndex = numberFormatIndex;
			this.fontInfo = CreateFontInfoWithoutColor(fontBold, fontItalic, fontUnderline, fontSize, fontName, schemeStyles);
			this.fillInfo = CreateFillInfoWithoutColor(patternType);
			this.borderInfo = CreateBorderInfoWithoutColor(leftBorder, rightBorder, topBorder, bottomBorder);
			this.flagsInfo = CreateFlagsInfo(applyNumberFormat, applyFont, applyFill, applyBorder, applyAlignment, applyProtection);
			this.fontColorKey = fontColorKey;
			this.fillForeColorKey = fillForeColorKey;
			this.fillBackColorKey = fillBackColorKey;
			this.leftBorderColorKey = leftBorderColorKey;
			this.rightBorderColorKey = rightBorderColorKey;
			this.topBorderColorKey = topBorderColorKey;
			this.bottomBorderColorKey = bottomBorderColorKey;
		}
		#region Properties
		protected internal string CellStyleName { get { return cellStyleName; } }
		protected internal int NumberFormatIndex { get { return numberFormatIndex; } }
		protected internal RunFontInfo FontInfo { get { return fontInfo; } }
		protected internal FillInfo FillInfo { get { return fillInfo; } }
		protected internal BorderInfo BorderInfo { get { return borderInfo; } }
		protected internal CellFormatFlagsInfo FlagsInfo { get { return flagsInfo; } }
		#endregion
		RunFontInfo CreateFontInfoWithoutColor(bool fontBold, bool fontItalic, bool fontUnderline, int fontSize, string fontName, XlFontSchemeStyles schemeStyles) {
			RunFontInfo result = new RunFontInfo();
			result.Bold = fontBold;
			result.Italic = fontItalic;
			result.Underline = fontUnderline ? XlUnderlineType.Single : XlUnderlineType.None;
			result.Size = fontSize;
			result.Name = fontName;
			result.SchemeStyle = schemeStyles;
			result.FontFamily = 2;
			return result;
		}
		FillInfo CreateFillInfoWithoutColor(XlPatternType patternType) {
			FillInfo result = new FillInfo();
			result.PatternType = patternType;
			return result;
		}
		BorderInfo CreateBorderInfoWithoutColor(XlBorderLineStyle leftBorder, XlBorderLineStyle rightBorder, XlBorderLineStyle topBorder, XlBorderLineStyle bottomBorder) {
			BorderInfo result = new BorderInfo();
			result.LeftLineStyle = leftBorder;
			result.RightLineStyle = rightBorder;
			result.TopLineStyle = topBorder;
			result.BottomLineStyle = bottomBorder;
			return result;
		}
		CellFormatFlagsInfo CreateFlagsInfo(bool applyNumberFormat, bool applyFont, bool applyFill, bool applyBorder, bool applyAlignment, bool applyProtection) {
			CellFormatFlagsInfo result = CellFormatFlagsInfo.DefaultStyle;
			result.ApplyNumberFormat = applyNumberFormat;
			result.ApplyFont = applyFont;
			result.ApplyFill = applyFill;
			result.ApplyBorder = applyBorder;
			result.ApplyAlignment = applyAlignment;
			result.ApplyProtection = applyProtection;
			return result;
		}
		protected internal CellStyleFormat CreateStyleFormat(DocumentModel documentModel) {
			CellStyleFormat result = new CellStyleFormat(documentModel);
			result.AssignNumberFormatIndex(NumberFormatIndex);
			DocumentCache cache = documentModel.Cache;
			RunFontInfo fontInfo = this.fontInfo.Clone();
			AssignFontColorIndex(fontInfo, cache);
			result.AssignFontIndex(cache.FontInfoCache.GetItemIndex(fontInfo));
			FillInfo fillInfo = this.fillInfo.Clone();
			AssignFillColorIndexes(fillInfo, cache);
			result.AssignFillIndex(cache.FillInfoCache.GetItemIndex(fillInfo));
			BorderInfo borderInfo = this.borderInfo.Clone();
			AssignBorderColorIndexes(borderInfo, cache);
			result.AssignBorderIndex(cache.BorderInfoCache.GetItemIndex(borderInfo));
			result.AssignCellFormatFlagsIndex(FlagsInfo.PackedValues);
			return result;
		}
		protected internal RunFontInfo CreateFontInfo(DocumentCache cache) {
			RunFontInfo result = fontInfo.Clone();
			AssignFontColorIndex(result, cache);
			cache.FontInfoCache.GetItemIndex(result);
			return result;
		}
		protected internal FillInfo CreateFillInfo(DocumentCache cache) {
			FillInfo result = fillInfo.Clone();
			AssignFillColorIndexes(result, cache);
			cache.FillInfoCache.GetItemIndex(result);
			return result;
		}
		protected internal BorderInfo CreateBorderInfo(DocumentCache cache) {
			BorderInfo result = borderInfo.Clone();
			AssignBorderColorIndexes(result, cache);
			cache.BorderInfoCache.GetItemIndex(result);
			return result;
		}
		bool IsEqualsColor(ColorModelInfoCache colorCache, int colorIndex, int colorKey) {
			return colorCache[colorIndex].Equals(BuiltInColorCalculator.GetColorModelInfo(colorKey));
		}
		protected internal bool CheckEqualsFontInfo(CellFormatBase format) {
			return
				fontInfo.EqualsNoColorIndex(format.FontInfo) &&
				IsEqualsColor(format.DocumentModel.Cache.ColorModelInfoCache, format.FontInfo.ColorIndex, fontColorKey);
		}
		protected internal bool CheckEqualsFillInfo(CellFormatBase format) {
			ColorModelInfoCache cache = format.DocumentModel.Cache.ColorModelInfoCache;
			FillInfo info = format.FillInfo;
			return
				fillInfo.EqualsNoColorIndexes(format.FillInfo) &&
				IsEqualsColor(cache, info.ForeColorIndex, fillForeColorKey) &&
				IsEqualsColor(cache, info.BackColorIndex, fillBackColorKey);
		}
		protected internal bool CheckEqualsBorderInfo(CellFormatBase format) {
			ColorModelInfoCache cache = format.DocumentModel.Cache.ColorModelInfoCache;
			BorderInfo info = format.BorderInfo;
			return
				borderInfo.EqualsNoColorIndexes(format.BorderInfo) &&
				IsEqualsColor(cache, info.LeftColorIndex, leftBorderColorKey) &&
				IsEqualsColor(cache, info.RightColorIndex, rightBorderColorKey) &&
				IsEqualsColor(cache, info.TopColorIndex, topBorderColorKey) &&
				IsEqualsColor(cache, info.BottomColorIndex, bottomBorderColorKey);
		}
		protected internal void AssignFontColorIndex(RunFontInfo info, DocumentCache cache) {
			info.ColorIndex = BuiltInColorCalculator.CreateColorIndex(cache, fontColorKey);
		}
		protected internal void AssignFillColorIndexes(FillInfo info, DocumentCache cache) {
			info.ForeColorIndex = BuiltInColorCalculator.CreateColorIndex(cache, fillForeColorKey);
			info.BackColorIndex = BuiltInColorCalculator.CreateColorIndex(cache, fillBackColorKey);
		}
		protected internal void AssignBorderColorIndexes(BorderInfo info, DocumentCache cache) {
			info.LeftColorIndex = BuiltInColorCalculator.CreateColorIndex(cache, leftBorderColorKey);
			info.RightColorIndex = BuiltInColorCalculator.CreateColorIndex(cache, rightBorderColorKey);
			info.TopColorIndex = BuiltInColorCalculator.CreateColorIndex(cache, topBorderColorKey);
			info.BottomColorIndex = BuiltInColorCalculator.CreateColorIndex(cache, bottomBorderColorKey);
		}
	}
	#endregion
	#region BuiltInCellStyleCalculator
	public static class BuiltInCellStyleCalculator {
		static Dictionary<int, BuiltInCellStyleInfo> builtInCellStyleInfoTable = CreateBuiltInCellStyleInfoTable();
		static Dictionary<int, BuiltInCellStyleInfo> CreateBuiltInCellStyleInfoTable() {
			Dictionary<int, BuiltInCellStyleInfo> result = new Dictionary<int, BuiltInCellStyleInfo>();
			result.Add(0, new BuiltInCellStyleInfo("Normal", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, -1, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, true, true, true, true, true, true));
			result.Add(3, new BuiltInCellStyleInfo("Comma", 43, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, -1, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, true, false, false, false, false, false));
			result.Add(4, new BuiltInCellStyleInfo("Currency", 44, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, -1, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, true, false, false, false, false, false));
			result.Add(5, new BuiltInCellStyleInfo("Percent", 9, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, -1, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, true, false, false, false, false, false));
			result.Add(6, new BuiltInCellStyleInfo("Comma [0]", 41, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, -1, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, true, false, false, false, false, false));
			result.Add(7, new BuiltInCellStyleInfo("Currency [0]", 42, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, -1, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, true, false, false, false, false, false));
			result.Add(8, new BuiltInCellStyleInfo("Hyperlink", 0, false, false, true, 11, "Calibri", XlFontSchemeStyles.Minor, 28, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, false, false, false, false));
			result.Add(9, new BuiltInCellStyleInfo("Followed Hyperlink", 0, false, false, true, 11, "Calibri", XlFontSchemeStyles.Minor, 29, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, false, false, false, false));
			result.Add(10, new BuiltInCellStyleInfo("Note", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, -1, XlPatternType.Solid, 46, -1, XlBorderLineStyle.Thin, XlBorderLineStyle.Thin, XlBorderLineStyle.Thin, XlBorderLineStyle.Thin, 37, 37, 37, 37, false, false, true, true, false, false));
			result.Add(11, new BuiltInCellStyleInfo("Warning Text", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 41, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, false, false, false, false));
			result.Add(12, new BuiltInCellStyleInfo("Emphasis 1", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, -1, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, true, true, true, true, true, true));
			result.Add(13, new BuiltInCellStyleInfo("Emphasis 2", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, -1, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, true, true, true, true, true, true));
			result.Add(14, new BuiltInCellStyleInfo("Emphasis 3", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, -1, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, true, true, true, true, true, true));
			result.Add(15, new BuiltInCellStyleInfo("Title", 0, true, false, false, 18, "Cambria", XlFontSchemeStyles.Major, 2, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, false, false, false, false));
			result.Add(16, new BuiltInCellStyleInfo("Heading 1", 0, true, false, false, 15, "Calibri", XlFontSchemeStyles.Minor, 2, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.Thick, -1, -1, -1, 3, false, true, false, true, false, false));
			result.Add(17, new BuiltInCellStyleInfo("Heading 2", 0, true, false, false, 13, "Calibri", XlFontSchemeStyles.Minor, 2, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.Thick, -1, -1, -1, 5, false, true, false, true, false, false));
			result.Add(18, new BuiltInCellStyleInfo("Heading 3", 0, true, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 2, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.Medium, -1, -1, -1, 4, false, true, false, true, false, false));
			result.Add(19, new BuiltInCellStyleInfo("Heading 4", 0, true, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 2, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, false, false, false, false));
			result.Add(20, new BuiltInCellStyleInfo("Input", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 32, XlPatternType.Solid, 44, -1, XlBorderLineStyle.Thin, XlBorderLineStyle.Thin, XlBorderLineStyle.Thin, XlBorderLineStyle.Thin, 33, 33, 33, 33, false, true, true, true, false, false));
			result.Add(21, new BuiltInCellStyleInfo("Output", 0, true, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 31, XlPatternType.Solid, 39, -1, XlBorderLineStyle.Thin, XlBorderLineStyle.Thin, XlBorderLineStyle.Thin, XlBorderLineStyle.Thin, 31, 31, 31, 31, false, true, true, true, false, false));
			result.Add(22, new BuiltInCellStyleInfo("Calculation", 0, true, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 40, XlPatternType.Solid, 39, -1, XlBorderLineStyle.Thin, XlBorderLineStyle.Thin, XlBorderLineStyle.Thin, XlBorderLineStyle.Thin, 33, 33, 33, 33, false, true, true, true, false, false));
			result.Add(23, new BuiltInCellStyleInfo("Check Cell", 0, true, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 0, XlPatternType.Solid, 36, -1, XlBorderLineStyle.Double, XlBorderLineStyle.Double, XlBorderLineStyle.Double, XlBorderLineStyle.Double, 31, 31, 31, 31, false, true, true, true, false, false));
			result.Add(24, new BuiltInCellStyleInfo("Linked Cell", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 40, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.Double, -1, -1, -1, 42, false, true, false, true, false, false));
			result.Add(25, new BuiltInCellStyleInfo("Total", 0, true, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.Thin, XlBorderLineStyle.Double, -1, -1, 3, 3, false, true, false, true, false, false));
			result.Add(26, new BuiltInCellStyleInfo("Good", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 30, XlPatternType.Solid, 38, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(27, new BuiltInCellStyleInfo("Bad", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 34, XlPatternType.Solid, 43, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(28, new BuiltInCellStyleInfo("Neutral", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 35, XlPatternType.Solid, 45, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(29, new BuiltInCellStyleInfo("Accent1", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 0, XlPatternType.Solid, 3, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(30, new BuiltInCellStyleInfo("20% - Accent1", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.Solid, 7, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(31, new BuiltInCellStyleInfo("40% - Accent1", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.Solid, 6, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(32, new BuiltInCellStyleInfo("60% - Accent1", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 0, XlPatternType.Solid, 4, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(33, new BuiltInCellStyleInfo("Accent2", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 0, XlPatternType.Solid, 8, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(34, new BuiltInCellStyleInfo("20% - Accent2", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.Solid, 11, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(35, new BuiltInCellStyleInfo("40% - Accent2", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.Solid, 10, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(36, new BuiltInCellStyleInfo("60% - Accent2", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 0, XlPatternType.Solid, 9, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(37, new BuiltInCellStyleInfo("Accent3", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 0, XlPatternType.Solid, 12, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(38, new BuiltInCellStyleInfo("20% - Accent3", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.Solid, 15, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(39, new BuiltInCellStyleInfo("40% - Accent3", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.Solid, 14, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(40, new BuiltInCellStyleInfo("60% - Accent3", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 0, XlPatternType.Solid, 13, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(41, new BuiltInCellStyleInfo("Accent4", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 0, XlPatternType.Solid, 16, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(42, new BuiltInCellStyleInfo("20% - Accent4", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.Solid, 19, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(43, new BuiltInCellStyleInfo("40% - Accent4", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.Solid, 18, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(44, new BuiltInCellStyleInfo("60% - Accent4", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 0, XlPatternType.Solid, 17, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(45, new BuiltInCellStyleInfo("Accent5", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 0, XlPatternType.Solid, 20, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(46, new BuiltInCellStyleInfo("20% - Accent5", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.Solid, 23, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(47, new BuiltInCellStyleInfo("40% - Accent5", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.Solid, 22, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(48, new BuiltInCellStyleInfo("60% - Accent5", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 0, XlPatternType.Solid, 21, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(49, new BuiltInCellStyleInfo("Accent6", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 0, XlPatternType.Solid, 24, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(50, new BuiltInCellStyleInfo("20% - Accent6", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.Solid, 27, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(51, new BuiltInCellStyleInfo("40% - Accent6", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.Solid, 26, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(52, new BuiltInCellStyleInfo("60% - Accent6", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 0, XlPatternType.Solid, 25, 47, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, true, false, false, false));
			result.Add(53, new BuiltInCellStyleInfo("Explanatory Text", 0, false, true, false, 11, "Calibri", XlFontSchemeStyles.Minor, 33, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, false, true, false, false, false, false));
			result.Add(54, new BuiltInCellStyleInfo("TableStyleLight1", 0, false, false, false, 11, "Calibri", XlFontSchemeStyles.Minor, 1, XlPatternType.None, -1, -1, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, XlBorderLineStyle.None, -1, -1, -1, -1, true, true, true, true, true, true));
			return result;
		}
		static List<int> builtInIdCellStyleCollection = CreateBuiltInIdCellStyleCollection();
		static List<int> CreateBuiltInIdCellStyleCollection() {
			List<int> result = new List<int>();
			result.Add(0);
			for (int i = 3; i <= 11; i++)
				result.Add(i);
			for (int i = 15; i <= 53; i++)
				result.Add(i);
			return result;
		}
		public static BuiltInCellStyleInfo GetBuiltInCellStyleInfo(int builtInId) {
			BuiltInCellStyleInfo result;
			if (!builtInCellStyleInfoTable.TryGetValue(builtInId, out result))
				Exceptions.ThrowInternalException();
			return result;
		}
		public static string CalculateName(int builtInId) {
			return GetBuiltInCellStyleInfo(builtInId).CellStyleName;
		}
		public static CellStyleFormat CreateStyleFormat(DocumentModel documentModel, int builtInId) {
			BuiltInCellStyleInfo info = GetBuiltInCellStyleInfo(builtInId);
			return info.CreateStyleFormat(documentModel);
		}
		public static bool IsNormalStyle(int builtInId) {
			return builtInId == 0;
		}
		public static bool IsValidateOutlineBuildInId(int builtInId) {
			return builtInId == 1 || builtInId == 2;
		}
		public static bool IsValidateBuiltInId(int builtInId) {
			return builtInCellStyleInfoTable.ContainsKey(builtInId);
		}
		public static bool IsBuiltInStyle(string name) {
			foreach (BuiltInCellStyleInfo info in builtInCellStyleInfoTable.Values)
				if (info.CellStyleName == name)
					return true;
			return false;
		}
		public static int CalculateBuiltInId(string name) {
			foreach(KeyValuePair<int, BuiltInCellStyleInfo> pair in builtInCellStyleInfoTable)
				if(pair.Value.CellStyleName == name)
					return pair.Key;
			return Int32.MinValue;
		}
		public static void AppendBuiltInCellStyles(StyleSheet styleSheet) {
			Guard.ArgumentNotNull(styleSheet, "styleSheet");
			DocumentModel workbook = styleSheet.Workbook;
			CellStyleCollection cellStyles = styleSheet.CellStyles;
			int count = builtInIdCellStyleCollection.Count;
			cellStyles.BeginUpdate();
			try {
				for (int i = 0; i < count; i++) {
					int builtInId = builtInIdCellStyleCollection[i];
					string cellStyleName = builtInCellStyleInfoTable[builtInId].CellStyleName;
					if (!cellStyles.ContainsCellStyleName(cellStyleName)) {
						BuiltInCellStyle style = new BuiltInCellStyle(workbook, builtInId);
						cellStyles.Add(style);
					}
				}
			} finally {
				cellStyles.EndUpdate();
			}
		}
		public static StyleCategory GetStyleCategory(CellStyleBase cellStyle) {
			BuiltInCellStyle builtInCellStyle = cellStyle as BuiltInCellStyle;
			if (builtInCellStyle == null)
				return StyleCategory.CustomStyle;
			return GetStyleCategory(builtInCellStyle.BuiltInId);
		}
		public static StyleCategory GetStyleCategory(int builtInId) {
			if (builtInId == 0 || (builtInId >= 26 && builtInId <= 28))
				return StyleCategory.GoodBadNeutralStyle;
			if ((builtInId >= 8 && builtInId <= 11) || (builtInId >= 20 && builtInId <= 24) || builtInId == 53)
				return StyleCategory.DataModelStyle;
			if ((builtInId >= 15 && builtInId <= 19) || builtInId == 25)
				return StyleCategory.TitleAndHeadingStyle;
			if (builtInId >= 29 && builtInId <= 52)
				return StyleCategory.ThemedCellStyle;
			if (builtInId >= 3 && builtInId <= 7)
				return StyleCategory.NumberFormatStyle;
			return StyleCategory.CustomStyle;
		}
	}
	#endregion
	#region BuiltInColorCalculator
	public static class BuiltInColorCalculator {
		static Dictionary<int, ColorModelInfo> builtInColorCollection = CreateBuiltInColorTable();
		static Dictionary<int, ColorModelInfo> CreateBuiltInColorTable() {
			Dictionary<int, ColorModelInfo> result = new Dictionary<int, ColorModelInfo>();
			result.Add(-1, ColorModelInfo.Create(DXColor.Empty));
			result.Add(0, ColorModelInfo.Create(new ThemeColorIndex(0)));
			result.Add(1, ColorModelInfo.Create(new ThemeColorIndex(1)));
			result.Add(2, ColorModelInfo.Create(new ThemeColorIndex(3)));
			result.Add(3, ColorModelInfo.Create(new ThemeColorIndex(4)));
			result.Add(4, ColorModelInfo.Create(new ThemeColorIndex(4), 0.39997558519241921));
			result.Add(5, ColorModelInfo.Create(new ThemeColorIndex(4), 0.499984740745262));
			result.Add(6, ColorModelInfo.Create(new ThemeColorIndex(4), 0.59999389629810485));
			result.Add(7, ColorModelInfo.Create(new ThemeColorIndex(4), 0.79998168889431442));
			result.Add(8, ColorModelInfo.Create(new ThemeColorIndex(5)));
			result.Add(9, ColorModelInfo.Create(new ThemeColorIndex(5), 0.39997558519241921));
			result.Add(10, ColorModelInfo.Create(new ThemeColorIndex(5), 0.59999389629810485));
			result.Add(11, ColorModelInfo.Create(new ThemeColorIndex(5), 0.79998168889431442));
			result.Add(12, ColorModelInfo.Create(new ThemeColorIndex(6)));
			result.Add(13, ColorModelInfo.Create(new ThemeColorIndex(6), 0.39997558519241921));
			result.Add(14, ColorModelInfo.Create(new ThemeColorIndex(6), 0.59999389629810485));
			result.Add(15, ColorModelInfo.Create(new ThemeColorIndex(6), 0.79998168889431442));
			result.Add(16, ColorModelInfo.Create(new ThemeColorIndex(7)));
			result.Add(17, ColorModelInfo.Create(new ThemeColorIndex(7), 0.39997558519241921));
			result.Add(18, ColorModelInfo.Create(new ThemeColorIndex(7), 0.59999389629810485));
			result.Add(19, ColorModelInfo.Create(new ThemeColorIndex(7), 0.79998168889431442));
			result.Add(20, ColorModelInfo.Create(new ThemeColorIndex(8)));
			result.Add(21, ColorModelInfo.Create(new ThemeColorIndex(8), 0.39997558519241921));
			result.Add(22, ColorModelInfo.Create(new ThemeColorIndex(8), 0.59999389629810485));
			result.Add(23, ColorModelInfo.Create(new ThemeColorIndex(8), 0.79998168889431442));
			result.Add(24, ColorModelInfo.Create(new ThemeColorIndex(9)));
			result.Add(25, ColorModelInfo.Create(new ThemeColorIndex(9), 0.39997558519241921));
			result.Add(26, ColorModelInfo.Create(new ThemeColorIndex(9), 0.59999389629810485));
			result.Add(27, ColorModelInfo.Create(new ThemeColorIndex(9), 0.79998168889431442));
			result.Add(28, ColorModelInfo.Create(new ThemeColorIndex(10)));
			result.Add(29, ColorModelInfo.Create(new ThemeColorIndex(11)));
			result.Add(30, ColorModelInfo.Create(DXColor.FromArgb(255, 0, 97, 0)));
			result.Add(31, ColorModelInfo.Create(DXColor.FromArgb(255, 63, 63, 63)));
			result.Add(32, ColorModelInfo.Create(DXColor.FromArgb(255, 63, 63, 118)));
			result.Add(33, ColorModelInfo.Create(DXColor.FromArgb(255, 127, 127, 127)));
			result.Add(34, ColorModelInfo.Create(DXColor.FromArgb(255, 156, 0, 6)));
			result.Add(35, ColorModelInfo.Create(DXColor.FromArgb(255, 156, 101, 0)));
			result.Add(36, ColorModelInfo.Create(DXColor.FromArgb(255, 165, 165, 165)));
			result.Add(37, ColorModelInfo.Create(DXColor.FromArgb(255, 178, 178, 178)));
			result.Add(38, ColorModelInfo.Create(DXColor.FromArgb(255, 198, 239, 206)));
			result.Add(39, ColorModelInfo.Create(DXColor.FromArgb(255, 242, 242, 242)));
			result.Add(40, ColorModelInfo.Create(DXColor.FromArgb(255, 250, 125, 0)));
			result.Add(41, ColorModelInfo.Create(DXColor.FromArgb(255, 255, 0, 0)));
			result.Add(42, ColorModelInfo.Create(DXColor.FromArgb(255, 255, 128, 1)));
			result.Add(43, ColorModelInfo.Create(DXColor.FromArgb(255, 255, 199, 206)));
			result.Add(44, ColorModelInfo.Create(DXColor.FromArgb(255, 255, 204, 153)));
			result.Add(45, ColorModelInfo.Create(DXColor.FromArgb(255, 255, 235, 156)));
			result.Add(46, ColorModelInfo.Create(DXColor.FromArgb(255, 255, 255, 204)));
			result.Add(47, ColorModelInfo.Create(65));
			return result;
		}
		public static int CreateColorIndex(DocumentCache cache, int colorKey) {
			return cache.ColorModelInfoCache.AddItem(GetColorModelInfo(colorKey));
		}
		public static ColorModelInfo GetColorModelInfo(int colorKey) {
			ColorModelInfo result;
			if (!builtInColorCollection.TryGetValue(colorKey, out result))
				Exceptions.ThrowInternalException();
			return result;
		}
	}
	#endregion
}
