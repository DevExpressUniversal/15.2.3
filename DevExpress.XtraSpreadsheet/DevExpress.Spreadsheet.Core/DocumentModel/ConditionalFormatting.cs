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
using System.IO;
using System.Reflection;
using DevExpress.Data.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.History;
using System.Diagnostics.CodeAnalysis;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using System.Globalization;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
#else
using System.Drawing;
using DevExpress.Office.Model;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region ConditionalFormattingType
	public enum ConditionalFormattingType {
		Formula,
		ColorScale,
		DataBar,
		IconSet,
		Unknown = -1
	}
	#endregion
	#region IConditionalFormattingVisitor
	public interface IConditionalFormattingVisitor {
		bool Visit(FormulaConditionalFormatting formatting);
		bool Visit(ColorScaleConditionalFormatting formatting);
		bool Visit(DataBarConditionalFormatting formatting);
		bool Visit(IconSetConditionalFormatting formatting);
	}
	#endregion
	#region ConditionalFormatting (abstract class)
	#region IConditionalFormatting
	public interface IConditionalFormatting {
		bool IsPivot { get; set; }
		CellRangeBase CellRange { get; }
		void SetCellRange(CellRangeBase newRange);
		int Priority { get; }
		bool StopIfTrue { get; set; }
		bool SupportsDifferentialFormat { get; }
		ConditionalFormattingType Type { get; }
		ConditionalFormattingRuleType RuleType { get; }
		bool IsValid { get; }
		void InvalidateOrMarkDeleted();
	}
	#endregion
	#region ConditionalFormattingBatchUpdateHelper
	public class ConditionalFormattingBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		ConditionalFormattingInfo conditionalFormattingInfo;
		DifferentialFormat differentialFormat;
		public ConditionalFormattingBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public ConditionalFormattingInfo ConditionalFormattingInfo { get { return conditionalFormattingInfo; } set { conditionalFormattingInfo = value; } }
		public DifferentialFormat DifferentialFormat { get { return differentialFormat; } set { differentialFormat = value; } }
	}
	#endregion
	#region ConditionalFormattingDifferentialFormatIndexAccessor
	public class ConditionalFormattingDifferentialFormatIndexAccessor : IIndexAccessor<ConditionalFormatting, FormatBase, DocumentModelChangeActions> {
		#region IIndexAccessor<ConditionalFormatting,FormatBase,DocumentModelChangeActions> Members
		public int GetInfoIndex(ConditionalFormatting owner, FormatBase value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public FormatBase GetInfo(ConditionalFormatting owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public FormatBase GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((ConditionalFormattingBatchUpdateHelper)helper).DifferentialFormat;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, FormatBase info) {
			((ConditionalFormattingBatchUpdateHelper)helper).DifferentialFormat = (DifferentialFormat)info.Clone();
		}
		#endregion
		#region IIndexAccessorBase<ConditionalFormatting,DocumentModelChangeActions> Members
		public int GetIndex(ConditionalFormatting owner) {
			return owner.DifferentialFormatIndex;
		}
		public void SetIndex(ConditionalFormatting owner, int value) {
			owner.DifferentialFormatIndex = value;
		}
		public bool IsIndexValid(ConditionalFormatting owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		public int GetDeferredInfoIndex(ConditionalFormatting owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void InitializeDeferredInfo(ConditionalFormatting owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(ConditionalFormatting owner, ConditionalFormatting from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(ConditionalFormatting owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(ConditionalFormatting owner) {
			return new ConditionalFormattingFormatIndexChangeHistoryItem(owner);
		}
		#endregion
		UniqueItemsCache<FormatBase> GetInfoCache(ConditionalFormatting owner) {
			return owner.DocumentModel.Cache.CellFormatCache;
		}
	}
	#endregion
	#region ConditionalFormattingInfoIndexAccessor
	public class ConditionalFormattingInfoIndexAccessor : IIndexAccessor<ConditionalFormatting, ConditionalFormattingInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<ConditionalFormatting,ConditionalFormattingInfo,DocumentModelChangeActions> Members
		public int GetInfoIndex(ConditionalFormatting owner, ConditionalFormattingInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public ConditionalFormattingInfo GetInfo(ConditionalFormatting owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public ConditionalFormattingInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((ConditionalFormattingBatchUpdateHelper)helper).ConditionalFormattingInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, ConditionalFormattingInfo info) {
			((ConditionalFormattingBatchUpdateHelper)helper).ConditionalFormattingInfo = info.Clone();
		}
		#endregion
		#region IIndexAccessorBase<ConditionalFormatting,DocumentModelChangeActions> Members
		public int GetIndex(ConditionalFormatting owner) {
			return owner.ConditionalFormattingIndex;
		}
		public void SetIndex(ConditionalFormatting owner, int value) {
			owner.ConditionalFormattingIndex = value;
		}
		public bool IsIndexValid(ConditionalFormatting owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		public int GetDeferredInfoIndex(ConditionalFormatting owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void InitializeDeferredInfo(ConditionalFormatting owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(ConditionalFormatting owner, ConditionalFormatting from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(ConditionalFormatting owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(ConditionalFormatting owner) {
			return new ConditionalFormattingIndexChangeHistoryItem(owner);
		}
		#endregion
		ConditionalFormattingInfoCache GetInfoCache(ConditionalFormatting owner) {
			return owner.DocumentModel.Cache.ConditionalFormattingInfoCache;
		}
	}
	#endregion
	#region ConditionalFormatting
	public abstract class ConditionalFormatting : MultiIndexObject<ConditionalFormatting, DocumentModelChangeActions>, IConditionalFormatting, IComparable<ConditionalFormatting> {
		public const int DefaultPriorityValue = 1;
		#region Static Members
		[SuppressMessage("Microsoft.Performance", "CA1051:DoNotDeclareVisibleInstanceFields ")]
		protected internal const string OpenXmlUri = "{78C0D931-6437-407d-A8EE-F0AAD7539E65}";
		protected const int UnassignedValueObjectIndex = -1;
		readonly static ConditionalFormattingInfoIndexAccessor conditionalFormattingInfoIndexAccessor = new ConditionalFormattingInfoIndexAccessor();
		readonly static ConditionalFormattingDifferentialFormatIndexAccessor differentialFormatIndexAccessor = new ConditionalFormattingDifferentialFormatIndexAccessor();
		readonly static IIndexAccessorBase<ConditionalFormatting, DocumentModelChangeActions>[] indexAccessors = new IIndexAccessorBase<ConditionalFormatting, DocumentModelChangeActions>[] {
			conditionalFormattingInfoIndexAccessor,
			differentialFormatIndexAccessor
		};
		public static ConditionalFormattingInfoIndexAccessor ConditionalFormattingInfoIndexAccessor { get { return conditionalFormattingInfoIndexAccessor; } }
		public static ConditionalFormattingDifferentialFormatIndexAccessor DifferentialFormatIndexAccessor { get { return differentialFormatIndexAccessor; } }
		#endregion
		#region Fields
		int condFormatIndex;
		int diffFormatIndex;
		CellRangeBase cellRange;
		readonly Worksheet sheet;
		int priority = DefaultPriorityValue;
		#endregion
		protected ConditionalFormatting(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			DifferentialFormatIndex = CellFormatCache.DefaultDifferentialFormatIndex;
		}
		protected ConditionalFormatting(Worksheet sheet, CellRangeBase cellRange)
			: this(sheet) {
			Guard.ArgumentNotNull(cellRange, "cellRange");
			CellRangeInternalNoHistory = cellRange.Clone();
		}
		#region Properties
		internal new ConditionalFormattingBatchUpdateHelper BatchUpdateHelper { get { return (ConditionalFormattingBatchUpdateHelper)base.BatchUpdateHelper; } }
		ConditionalFormattingInfo ConditionalFormattingInfoCore { get { return ConditionalFormattingInfoIndexAccessor.GetInfo(this); } }
		DifferentialFormat DifferentialFormatInfoCore { get { return (DifferentialFormat)DifferentialFormatIndexAccessor.GetInfo(this); } }
		protected internal int ConditionalFormattingIndex { get { return condFormatIndex; } set { condFormatIndex = value; } }
		protected internal int DifferentialFormatIndex { get { return diffFormatIndex; } set { diffFormatIndex = value; } }
		protected internal ConditionalFormattingInfo ConditionalFormattingInfo {
			get { return BatchUpdateHelper != null ? BatchUpdateHelper.ConditionalFormattingInfo : ConditionalFormattingInfoCore; }
		}
		protected internal DifferentialFormat DifferentialFormatInfo {
			get { return BatchUpdateHelper != null ? BatchUpdateHelper.DifferentialFormat : DifferentialFormatInfoCore; }
		}
		public Worksheet Sheet { get { return sheet; } }
		public new DocumentModel DocumentModel { get { return sheet.Workbook; } }
		protected internal CellRangeBase CellRangeInternalNoHistory {
			get { return cellRange; }
			set {
				if (Object.ReferenceEquals(cellRange, value))
					return;
				CellRangeBase oldValue = cellRange;
				cellRange = value;
				this.Sheet.ConditionalFormattings.NotifyItemCellRangeChanged(this, oldValue);
			}
		}
		public CellRangeBase CellRange { get { return cellRange; } set { SetCellRange(value); } }
		public bool IsValid { get { return CheckValidity(); } } 
		#region bool IsPivot
		public bool IsPivot {
			get { return ConditionalFormattingInfo.IsPivot; }
			set {
				if (IsPivot == value)
					return;
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetIsPivotCore, value);
			}
		}
		DocumentModelChangeActions SetIsPivotCore(ConditionalFormattingInfo info, bool value) {
			info.IsPivot = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region int Priority
		public int Priority {
			get { return priority; }
			set {
				if (value == priority)
					return;
				SetPriorityWithPrioritiesCorrection(value);
			}
		}
		protected internal void SetPriorityWithPrioritiesCorrection(int value) {
			Sheet.ConditionalFormattings.ChangePriorityWithPrioritiesCorrection(this, value);
		}
		internal void SetPriority(int value) {
			if (priority == value)
				return;
			DocumentHistory history = DocumentModel.History;
			ConditionalFormattingPriorityChangeHistoryItem historyItem = new ConditionalFormattingPriorityChangeHistoryItem(this, priority, value);
			history.Add(historyItem);
			historyItem.Execute();
		}
		internal void SetPriorityCore(int value) {
			this.priority = value;
		}
		#endregion
		#region bool StopIfTrue
		public bool StopIfTrue {
			get {
				return ConditionalFormattingInfo.StopIfTrue;
			}
			set {
				if (StopIfTrue == value)
					return;
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetStopIfTrueCore, value);
			}
		}
		DocumentModelChangeActions SetStopIfTrueCore(ConditionalFormattingInfo info, bool value) {
			info.StopIfTrue = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public virtual bool SupportsDifferentialFormat { get { return false; } }
		public virtual ConditionalFormattingType Type { get { return ConditionalFormattingType.Unknown; } }
		public virtual ConditionalFormattingRuleType RuleType { get { return ConditionalFormattingRuleType.Unknown; } }
		#endregion
		public abstract bool Visit(IConditionalFormattingVisitor visitor);
		public void InvalidateOrMarkDeleted() {
			SetCellRangeOrNull(null);
		}
		protected virtual bool CheckValidity() {
			return (CellRangeInternalNoHistory != null) && (Priority > 0);
		}
		#region MultiIndexObject members
		protected override IDocumentModel GetDocumentModel() {
			return sheet.Workbook;
		}
		protected override IIndexAccessorBase<ConditionalFormatting, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		public override ConditionalFormatting GetOwner() {
			return this;
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new ConditionalFormattingBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new ConditionalFormattingBatchUpdateHelper(this); 
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected  override void ApplyChanges(DocumentModelChangeActions actions) {
		}
		#endregion
		#region CellRange helpers
		protected virtual void OnCellRangeChanged() { ; }
		protected internal virtual void SetCellRangeNoHistoryAndRebindComparer(CellRangeBase cellRange) {
			if (cellRange != null)
				CellRangeInternalNoHistory = cellRange.Clone();
			else
				CellRangeInternalNoHistory = null;
			OnCellRangeChanged();
		}
		public void SetCellRange(CellRangeBase newValue) {
			Guard.ArgumentNotNull(newValue, "CellRange");
			if (object.ReferenceEquals(newValue, CellRange))
				return;
			SetCellRangeOrNull(newValue);
		}
		protected internal void SetCellRangeOrNull(CellRangeBase newValue) {
			DocumentHistory history = DocumentModel.History;
			ConditionalFormattingCellRangeHistoryItem historyItem = new ConditionalFormattingCellRangeHistoryItem(this, CellRangeInternalNoHistory, newValue);
			history.Add(historyItem);
			historyItem.Execute();
		}
		#endregion
		protected void ProcessSetBound(ConditionalFormattingValueChangeHistoryItem item) {
			DocumentHistory history = DocumentModel.History;
			history.Add(item);
			item.Execute();
		}
		protected void ProcessSetColor(ConditionalFormattingColorChangeHistoryItem item) {
			DocumentHistory history = DocumentModel.History;
			history.Add(item);
			item.Execute();
		}
		#region helper functions for derived classes
		protected int IndexFromValue(ConditionalFormattingValueObject value) {
			return (value != null) ? value.Index : UnassignedValueObjectIndex;
		}
		protected int IndexFromValue(ConditionalFormattingValueObject value, Worksheet sheet) {
			return (value != null) ? value.Clone(sheet).Index : UnassignedValueObjectIndex;
		}
		protected ColorModelInfo ColorFromIndex(int valueIndex) {
			ColorModelInfo result = DocumentModel.Cache.ColorModelInfoCache[valueIndex];
			return result;
		}
		protected int IndexFromColorModel(ColorModelInfo info) {
			int result = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(info);
			return result;
		}
		protected int IndexFromColor(Color value) {
			return IndexFromColorModel(ColorModelInfo.Create(value));
		}
		protected bool TryGetCellValue(ICellBase cell, WorkbookDataContext context, out double result) {
			VariantValue variantValue = cell.Value;
			if (variantValue.IsNumeric) {
				result = variantValue.NumericValue;
				return true;
			}
			result = 0;
			return false;
		}
		protected bool TryGetPointValue(ConditionalFormattingValueObject expr, WorkbookDataContext context, out double result) {
			VariantValue exprValue = expr.Evaluate(context);
			if (exprValue.IsNumeric) {
				result = exprValue.NumericValue;
				return true;
			}
			result = 0;
			return false;
		}
		protected ConditionalFormattingValueObject AcceptValueByInfoIndex(int infoIndex) {
			if (infoIndex < 0)
				return null;
			ConditionalFormattingValueObject result = new ConditionalFormattingValueObject(Sheet, infoIndex);
			result.SetOwner(this);
			return result;
		}
		protected ConditionalFormattingValueObject HoldValue(ConditionalFormattingValueObject source) {
			if (source != null) {
				ConditionalFormattingValueObject result = source.Clone(Sheet);
				if (result != null)
					result.SetOwner(this);
				return result;
			}
			return null;
		}
		protected bool IsValueTypeMax(ConditionalFormattingValueObjectType value) {
			return (value == ConditionalFormattingValueObjectType.Max) || (value == ConditionalFormattingValueObjectType.AutoMax);
		}
		protected bool IsValueTypeNotMax(ConditionalFormattingValueObjectType value) {
			return (value != ConditionalFormattingValueObjectType.Max) && (value != ConditionalFormattingValueObjectType.AutoMax);
		}
		protected bool IsValueTypeMin(ConditionalFormattingValueObjectType value) {
			return (value == ConditionalFormattingValueObjectType.Min) || (value == ConditionalFormattingValueObjectType.AutoMin);
		}
		protected bool IsValueTypeNotMin(ConditionalFormattingValueObjectType value) {
			return (value != ConditionalFormattingValueObjectType.Min) && (value != ConditionalFormattingValueObjectType.AutoMin);
		}
		#endregion
		#region Notifications
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			RemoveCellMode mode = notificationContext.Mode;
			ShiftCellsNotificationMode cfMode;
			switch (mode) {
				case RemoveCellMode.ShiftCellsLeft:
					cfMode = ShiftCellsNotificationMode.ShiftLeft;
					break;
				case RemoveCellMode.ShiftCellsUp:
					cfMode = ShiftCellsNotificationMode.ShiftUp;
					break;
				default:
					return;
			}
			ProcessNotification(notificationContext, cfMode);
		}
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			InsertCellMode mode = notificationContext.Mode;
			ShiftCellsNotificationMode cfMode;
			switch (mode) {
				case InsertCellMode.ShiftCellsDown:
					cfMode = ShiftCellsNotificationMode.ShiftDown;
					break;
				case InsertCellMode.ShiftCellsRight:
					cfMode = ShiftCellsNotificationMode.ShiftRight;
					break;
				default:
					return;
			}
			ProcessNotification(notificationContext, cfMode);
		}
		protected virtual void ProcessNotification(InsertRemoveRangeNotificationContextBase notificationContext, ShiftCellsNotificationMode mode) {
			RangeNotificationInfo info = ConditionalFormattingNotificator.GetRangeNotificationInfo(notificationContext.Range, CellRange, Sheet, mode, false);
			CellRangeBase newRange = info.GetMergedRange();
			if (object.ReferenceEquals(newRange, CellRange))
				return;
			SetCellRangeOrNull(newRange);
		}
		#endregion
		public ConditionalFormatting CreateInstance(Worksheet targetWorksheet) {
			ConditionalFormatting result = CreateInstanceCore(targetWorksheet);
			if (CellRange != null)
				result.CellRangeInternalNoHistory = CellRange.Clone(targetWorksheet);
			return result;
		}
		protected abstract ConditionalFormatting CreateInstanceCore(Worksheet targetWorksheet);
		#region CopyFrom helpers
		protected void CopyBaseData(ConditionalFormatting source) {
			ConditionalFormattingInfoIndexAccessor infoIndexAccessor = ConditionalFormatting.ConditionalFormattingInfoIndexAccessor;
			infoIndexAccessor.SetIndex(this, infoIndexAccessor.GetInfoIndex(this, infoIndexAccessor.GetInfo(source)));
			this.priority = source.priority;
		}
		protected void CopyFormatData(ConditionalFormatting source) {
			ConditionalFormattingDifferentialFormatIndexAccessor infoIndexAccessor = ConditionalFormatting.DifferentialFormatIndexAccessor;
			FormatBase formatInfo = infoIndexAccessor.GetInfo(source);
			FormatBase newFormat = formatInfo.CreateEmptyClone(DocumentModel);
			newFormat.CopyFrom(formatInfo);
			infoIndexAccessor.SetIndex(this, infoIndexAccessor.GetInfoIndex(this, newFormat));
		}
		#endregion
		public abstract void CopyFrom(ConditionalFormatting source);
		public override string ToString() {
			if (cellRange == null)
				throw new InvalidOperationException();
			return String.Concat(base.ToString(), " ", cellRange.ToString(), " ", condFormatIndex.ToString(), " ", this.DifferentialFormatIndex.ToString());
		}
		public override bool Equals(object obj) {
			if (object.ReferenceEquals(this, obj))
				return true;
			if (!base.Equals(obj))
				return false;
			ConditionalFormatting other = obj as ConditionalFormatting;
			if (other == null)
				return false;
			if (!object.ReferenceEquals(sheet, other.sheet) || priority != other.priority)
				return false;
			if (cellRange == null) {
				if (other.cellRange != null)
					return false;
			}
			else {
				if (other.cellRange == null)
					return false;
				if (!cellRange.Equals(other.CellRange))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), sheet.GetHashCode(), cellRange.GetHashCode(), priority);
		}
		public int CompareTo(ConditionalFormatting other) {
			if (Priority < other.Priority)
				return -1;
			if (Priority == other.Priority)
				return 0;
			return 1;
		}
	}
	#endregion
	#endregion
	#region ColorScaleConditionalFormatting
	#region ColorScaleItemIndex
	public enum ColorScaleItemIndex {
		LowPoint = 0,
		MiddlePoint,
		HighPoint
	}
	#endregion
	#region ColorScalePointData
	public class ColorScalePointData : ICloneable<ColorScalePointData> {
		protected internal ColorScalePointData(ConditionalFormattingValueObject point, ColorModelInfo color) {
			Guard.ArgumentNotNull(point, "point");
			Guard.ArgumentNotNull(color, "color");
			Point = point;
			ColorModelInfo = color;
		}
		protected internal ColorScalePointData(KeyValuePair<ConditionalFormattingValueObject, ColorModelInfo> value)
			: this(value.Key, value.Value) {
		}
		public ColorScalePointData(ConditionalFormattingValueObject point, Color color)
			: this(point, ColorModelInfo.Create(color)) {
		}
		public ColorScalePointData(KeyValuePair<ConditionalFormattingValueObject, Color> value)
			: this(value.Key, value.Value) {
		}
		#region Properties
		public ConditionalFormattingValueObject Point { get; set; }
		public Color Color { get { return ColorModelInfo.ToRgb(Point.DocumentModel.StyleSheet.Palette, Point.DocumentModel.OfficeTheme.Colors); } set { ColorModelInfo = ColorModelInfo.Create(value); } }
		protected internal ColorModelInfo ColorModelInfo { get; set; }
		#endregion
		#region ICloneable<ColorScalePointData> Members
		public ColorScalePointData Clone() {
			return new ColorScalePointData(Point, ColorModelInfo.Clone());
		}
		#endregion
	}
	#endregion
	#region ColorScaleType
	public enum ColorScaleType {
		Invalid, 
		Color2,  
		Color3   
	}
	#endregion
	#region IColorScaleConditionalFormatting
	public interface IColorScaleConditionalFormatting {
		ConditionalFormattingValueObject LowPointValue { get; set; }
		Color LowPointColor { get; set; }
		ConditionalFormattingValueObject MiddlePointValue { get; set; }
		Color MiddlePointColor { get; set; }
		ConditionalFormattingValueObject HighPointValue { get; set; }
		Color HighPointColor { get; set; }
		ColorScaleType ScaleType { get; }
		Color EvaluateColor(ICell cell);
	}
	#endregion
	public class ColorScaleConditionalFormatting : ConditionalFormatting, IColorScaleConditionalFormatting {
		#region Fields
		ConditionalFormattingValueObject lowPointValue;
		ConditionalFormattingValueObject middlePointValue;
		ConditionalFormattingValueObject highPointValue;
		int lowPointColorIndex;
		int middlePointColorIndex;
		int highPointColorIndex;
		#endregion
		internal ColorScaleConditionalFormatting(Worksheet sheet)
			: base(sheet) {
			ConditionalFormattingIndex = ConditionalFormattingInfoCache.DefaultItemIndex;
		}
		ColorScaleConditionalFormatting(Worksheet sheet, CellRangeBase cellRange)
			: base(sheet, cellRange) {
			ConditionalFormattingIndex = ConditionalFormattingInfoCache.DefaultItemIndex;
		}
		public ColorScaleConditionalFormatting(Worksheet sheet, CellRangeBase cellRange, ColorScalePointData lowPoint, ColorScalePointData middlePoint, ColorScalePointData highPoint)
			: this(sheet, cellRange) {
			Guard.ArgumentNotNull(lowPoint, "lowPoint");
			Guard.ArgumentNotNull(highPoint, "highPoint");
			lowPointValue = HoldValue(lowPoint.Point);
			LowPointColorIndex = IndexFromColorModel(lowPoint.ColorModelInfo);
			if (middlePoint != null) {
				middlePointValue = HoldValue(middlePoint.Point);
				MiddlePointColorIndex = IndexFromColorModel(middlePoint.ColorModelInfo);
			}
			highPointValue = HoldValue(highPoint.Point);
			HighPointColorIndex = IndexFromColorModel(highPoint.ColorModelInfo);
		}
		public ColorScaleConditionalFormatting(Worksheet sheet, CellRangeBase cellRange, ColorScalePointData[] data)
			: this(sheet, cellRange) {
			switch (data.Length) {
				case 0:
					break;
				case 1:
					lowPointValue = AcceptPointValue(data[0]);
					LowPointColorIndex = IndexFromColorModel(data[0].ColorModelInfo);
					break;
				case 2:
					lowPointValue = AcceptPointValue(data[0]);
					LowPointColorIndex = IndexFromColorModel(data[0].ColorModelInfo);
					highPointValue = AcceptPointValue(data[1]);
					HighPointColorIndex = IndexFromColorModel(data[1].ColorModelInfo);
					break;
				default:
					lowPointValue = AcceptPointValue(data[0]);
					LowPointColorIndex = IndexFromColorModel(data[0].ColorModelInfo);
					middlePointValue = AcceptPointValue(data[1]);
					MiddlePointColorIndex = IndexFromColorModel(data[1].ColorModelInfo);
					highPointValue = AcceptPointValue(data[2]);
					HighPointColorIndex = IndexFromColorModel(data[2].ColorModelInfo);
					break;
			}
		}
		public ColorScaleConditionalFormatting(Worksheet sheet, CellRangeBase cellRange, ConditionalFormattingValueObject[] points, ColorModelInfo[] colors)
			: this(sheet, cellRange) {
			switch (points.Length) {
				case 0:
					break;
				case 1:
					lowPointValue = HoldValue(points[0]);
					LowPointColorIndex = IndexFromColorModel(colors[0]);
					break;
				case 2:
					lowPointValue = HoldValue(points[0]);
					LowPointColorIndex = IndexFromColorModel(colors[0]);
					highPointValue = HoldValue(points[1]);
					HighPointColorIndex = IndexFromColorModel(colors[1]);
					break;
				default:
					lowPointValue = HoldValue(points[0]);
					LowPointColorIndex = IndexFromColorModel(colors[0]);
					middlePointValue = HoldValue(points[1]);
					MiddlePointColorIndex = IndexFromColorModel(colors[1]);
					highPointValue = HoldValue(points[2]);
					HighPointColorIndex = IndexFromColorModel(colors[2]);
					break;
			}
		}
		#region Properties
		protected internal int LowPointValueIndex { get { return IndexFromValue(lowPointValue); } }
		protected internal int LowPointColorIndex { get { return lowPointColorIndex; } set { lowPointColorIndex = value; } }
		protected internal int MiddlePointValueIndex { get { return IndexFromValue(middlePointValue); } }
		protected internal int MiddlePointColorIndex { get { return middlePointColorIndex; } set { middlePointColorIndex = value; } }
		protected internal int HighPointValueIndex { get { return IndexFromValue(highPointValue); } }
		protected internal int HighPointColorIndex { get { return highPointColorIndex; } set { highPointColorIndex = value; } }
		public ConditionalFormattingValueObject LowPointValue { get { return GetPointValue(lowPointValue); } set { SetLowPointValue(value); } }
		public Color LowPointColor { get { return ColorFromIndex(LowPointColorIndex).ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors); } set { SetLowPointColor(value); } }
		public ConditionalFormattingValueObject MiddlePointValue { get { return GetPointValue(middlePointValue); } set { SetMiddlePointValue(value); } }
		public Color MiddlePointColor { get { return ColorFromIndex(MiddlePointColorIndex).ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors); } set { SetMiddlePointColor(value); } }
		public ConditionalFormattingValueObject HighPointValue { get { return GetPointValue(highPointValue); } set { SetHighPointValue(value); } }
		public Color HighPointColor { get { return ColorFromIndex(HighPointColorIndex).ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors); } set { SetHighPointColor(value); } }
		public ColorScaleType ScaleType { get { return GetColorScaleType(); } }
		public override ConditionalFormattingType Type { get { return ConditionalFormattingType.ColorScale; } }
		public override ConditionalFormattingRuleType RuleType { get { return ConditionalFormattingRuleType.ColorScale; } }
		#endregion
		public override bool Visit(IConditionalFormattingVisitor visitor) {
			return visitor.Visit(this);
		}
		bool CheckValidityOfBoundaryPoints() {
			return IsValueTypeNotMax(lowPointValue.ValueType) && IsValueTypeNotMin(highPointValue.ValueType);
		}
		bool CheckValidityOf3rdPoint() {
			ConditionalFormattingValueObjectType valueType = middlePointValue.ValueType;
			return IsValueTypeNotMax(valueType) && IsValueTypeNotMin(valueType);
		}
		protected override bool CheckValidity() {
			bool hasNoMiddlePoint = middlePointValue == null;
			return base.CheckValidity() &&
				   (lowPointValue != null) && (highPointValue != null) &&
				   CheckValidityOfBoundaryPoints() && (hasNoMiddlePoint || CheckValidityOf3rdPoint());
		}
		ConditionalFormattingValueObject GetPointValue(ConditionalFormattingValueObject fromValue) {
			return (fromValue != null) ? fromValue.Clone() : null;
		}
		ConditionalFormattingValueObject AcceptPointValue(ColorScalePointData value) {
			return HoldValue(value.Point);
		}
		#region explicit high-level access
		void SetLowPointValue(ConditionalFormattingValueObject value) {
			int oldIndex = LowPointValueIndex;
#if DEBUGTEST
			int newIndex = IndexFromValue(value, Sheet);
#else
			int newIndex = value.Clone(Sheet).Index;
#endif
			ConditionalFormattingValueChangeHistoryItem item = new ConditionalFormattingValueChangeHistoryItem(DocumentModel, this, (int)ColorScaleItemIndex.LowPoint, oldIndex, newIndex);
			ProcessSetBound(item);
		}
		void SetLowPointColor(Color value) {
			int index = IndexFromColor(value);
			if (index >= 0) {
				ConditionalFormattingColorChangeHistoryItem item = new ConditionalFormattingColorChangeHistoryItem(DocumentModel, this, (int)ColorScaleItemIndex.LowPoint, LowPointColorIndex, index);
				ProcessSetColor(item);
			}
		}
		void SetMiddlePointValue(ConditionalFormattingValueObject value) {
			int oldIndex = MiddlePointValueIndex;
			int newIndex = IndexFromValue(value, Sheet);
			ConditionalFormattingValueChangeHistoryItem item = new ConditionalFormattingValueChangeHistoryItem(DocumentModel, this, (int)ColorScaleItemIndex.MiddlePoint, oldIndex, newIndex);
			ProcessSetBound(item);
		}
		void SetMiddlePointColor(Color value) {
			int index = IndexFromColor(value);
			if (index >= 0) {
				ConditionalFormattingColorChangeHistoryItem item = new ConditionalFormattingColorChangeHistoryItem(DocumentModel, this, (int)ColorScaleItemIndex.MiddlePoint, MiddlePointColorIndex, index);
				ProcessSetColor(item);
			}
		}
		void SetHighPointValue(ConditionalFormattingValueObject value) {
			int oldIndex = HighPointValueIndex;
#if DEBUGTEST
			int newIndex = IndexFromValue(value, Sheet);
#else
			int newIndex = value.Clone(Sheet).Index;
#endif
			ConditionalFormattingValueChangeHistoryItem item = new ConditionalFormattingValueChangeHistoryItem(DocumentModel, this, (int)ColorScaleItemIndex.HighPoint, oldIndex, newIndex);
			ProcessSetBound(item);
		}
		void SetHighPointColor(Color value) {
			int index = IndexFromColor(value);
			if (index >= 0) {
				ConditionalFormattingColorChangeHistoryItem item = new ConditionalFormattingColorChangeHistoryItem(DocumentModel, this, (int)ColorScaleItemIndex.HighPoint, HighPointColorIndex, index);
				ProcessSetColor(item);
			}
		}
		#endregion
		#region indexed low-level access
		protected internal void SetPointValueIndex(ColorScaleItemIndex what, int value) {
			switch (what) {
				case ColorScaleItemIndex.HighPoint:
					highPointValue = AcceptValueByInfoIndex(value);
					break;
				case ColorScaleItemIndex.LowPoint:
					lowPointValue = AcceptValueByInfoIndex(value);
					break;
				case ColorScaleItemIndex.MiddlePoint:
					middlePointValue = AcceptValueByInfoIndex(value);
					break;
			}
		}
		protected internal void SetPointColorIndex(ColorScaleItemIndex what, int value) {
			switch (what) {
				case ColorScaleItemIndex.HighPoint:
					HighPointColorIndex = value;
					break;
				case ColorScaleItemIndex.LowPoint:
					LowPointColorIndex = value;
					break;
				case ColorScaleItemIndex.MiddlePoint:
					MiddlePointColorIndex = value;
					break;
			}
		}
		#endregion
		void ApplyIndexChange() {
			if (CellRangeInternalNoHistory != null) {
				if (lowPointValue != null)
					lowPointValue.SetOwner(this);
				if (middlePointValue != null)
					middlePointValue.SetOwner(this);
				if (highPointValue != null)
					highPointValue.SetOwner(this);
			}
		}
		protected override void OnCellRangeChanged() {
			ApplyIndexChange();
		}
		#region Color helper functions
		protected int RoundTowardZero(double value) {
			int roundedValue = Convert.ToInt32(Math.Floor(value));
			if (value >= 0)
				return roundedValue;
			return roundedValue + 1;
		}
		protected Color EvaluateColor(double part, Color from, Color to) {
			int A = to.A - from.A;
			int R = to.R - from.R;
			int G = to.G - from.G;
			int B = to.B - from.B;
			A = RoundTowardZero(part * A) + from.A; 
			R = RoundTowardZero(part * R) + from.R;
			G = RoundTowardZero(part * G) + from.G;
			B = RoundTowardZero(part * B) + from.B;
			return DXColor.FromArgb(A, R, G, B);
		}
		protected Color EvaluateColor(double value, double low, double high, Color lowColor, Color highColor) {
			double offset = value - low;
			double range = high - low;
			double relativeValue = offset / range;
			Color result = EvaluateColor(relativeValue, lowColor, highColor);
			return result;
		}
		#endregion
		public Color EvaluateColor(ICell cell) {
			double cellValue;
			double lowValue;
			double middleValue;
			double highValue;
			if (CellRangeInternalNoHistory == null)
				return DXColor.Empty;
			WorkbookDataContext dataContext = DocumentModel.DataContext;
			dataContext.PushCurrentCell(cell);
			try {
				if (!TryGetCellValue(cell, dataContext, out cellValue))
					return DXColor.Empty;
				if (!TryGetPointValue(lowPointValue, dataContext, out lowValue))
					return DXColor.Empty;
				if (!TryGetPointValue(highPointValue, dataContext, out highValue))
					return DXColor.Empty;
				if (highValue < lowValue)
					return DXColor.Empty;
				if (cellValue <= lowValue)
					return LowPointColor;
				if (cellValue >= highValue)
					return HighPointColor;
				ColorScaleType scaleType = ScaleType;
				switch (scaleType) {
					case ColorScaleType.Color2:
						return EvaluateColor(cellValue, lowValue, highValue, LowPointColor, HighPointColor);
					case ColorScaleType.Color3:
						if (!TryGetPointValue(middlePointValue, dataContext, out middleValue))
							return DXColor.Empty;
						if (middleValue <= lowValue || middleValue >= highValue)
							return DXColor.Empty;
						if (cellValue < middleValue)
							return EvaluateColor(cellValue, lowValue, middleValue, LowPointColor, MiddlePointColor);
						return EvaluateColor(cellValue, middleValue, highValue, MiddlePointColor, HighPointColor);
				}
				return DXColor.Empty; 
			}
			finally {
				dataContext.PopCurrentCell();
			}
		}
		protected ColorScaleType GetColorScaleType() {
			if ((lowPointValue != null) && (highPointValue != null))
				return (middlePointValue != null) ? ColorScaleType.Color3 : ColorScaleType.Color2;
			return ColorScaleType.Invalid;
		}
		protected internal static ColorScaleConditionalFormatting CreateCopy(ColorScaleConditionalFormatting source, Worksheet targetSheet) {
			ColorScaleConditionalFormatting result = source.CreateInstance(targetSheet) as ColorScaleConditionalFormatting;
			result.CopyFrom(source);
			return result;
		}
		protected override ConditionalFormatting CreateInstanceCore(Worksheet targetWorksheet) {
			return new ColorScaleConditionalFormatting(targetWorksheet);
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			ColorScaleConditionalFormatting cf = obj as ColorScaleConditionalFormatting;
			if (cf == null)
				return false;
			return
				object.Equals(lowPointValue, cf.lowPointValue) &&
				object.Equals(middlePointValue, cf.middlePointValue) &&
				object.Equals(highPointValue, cf.highPointValue) &&
				lowPointColorIndex == cf.lowPointColorIndex &&
				middlePointColorIndex == cf.middlePointColorIndex &&
				highPointColorIndex == cf.highPointColorIndex;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(
				base.GetHashCode(),
				HashCodeCalculator.CalcHashCode32(lowPointValue.GetHashCode(), middlePointValue.GetHashCode(), highPointValue.GetHashCode(), lowPointColorIndex, middlePointColorIndex, highPointColorIndex)
				);
		}
		public override void CopyFrom(ConditionalFormatting source) {
			Guard.ArgumentNotNull(source, "source");
			ColorScaleConditionalFormatting sourceColorScaleCF = source as ColorScaleConditionalFormatting;
			System.Diagnostics.Debug.Assert(sourceColorScaleCF != null);
			CopyBaseData(source);
			lowPointValue = HoldValue(sourceColorScaleCF.LowPointValue);
			middlePointValue = HoldValue(sourceColorScaleCF.MiddlePointValue);
			highPointValue = HoldValue(sourceColorScaleCF.HighPointValue);
			lowPointColorIndex = IndexFromColor(sourceColorScaleCF.LowPointColor);
			middlePointColorIndex = IndexFromColor(sourceColorScaleCF.MiddlePointColor);
			highPointColorIndex = IndexFromColor(sourceColorScaleCF.HighPointColor);
		}
	}
	#endregion
	#region DataBarConditionalFormatting
	#region DataBarItemIndex
	public enum DataBarItemIndex {
		LowBound = 0,
		HighBound
	}
	#endregion
	#region ConditionalFormattingDataBarDirection
	public enum ConditionalFormattingDataBarDirection {
		Context,
		LeftToRight,
		RightToLeft
	}
	#endregion
	#region ConditionalFormattingDataBarAxisPosition
	public enum ConditionalFormattingDataBarAxisPosition {
		None,
		Balanced,
		Center,
		Automatic = Balanced,
		Middle = Center
	}
	#endregion
	#region ConditionalFormattingDataBarEvaluationResult
	public struct ConditionalFormattingDataBarEvaluationResult {
		public double MinValue { get; set; }
		public double MaxValue { get; set; }
		public double Value { get; set; }
		public double MinLength { get; set; }
		public double LengthBias { get; set; }
		public double Length { get; set; }
	}
	#endregion
	#region IDataBarConditionalFormatting
	public interface IDataBarConditionalFormatting {
		int MinLength { get; set; }
		int MaxLength { get; set; }
		ConditionalFormattingDataBarAxisPosition AxisPosition { get; set; }
		ConditionalFormattingDataBarDirection Direction { get; set; }
		bool GradientFill { get; set; }
		Color Color { get; set; }
		Color NegativeValueColor { get; set; }
		Color BorderColor { get; set; }
		Color NegativeValueBorderColor { get; set; }
		Color AxisColor { get; set; }
		bool ShowValue { get; set; }
		double EvaluateBarWidth(ICell cell);
	}
	#endregion
	public class DataBarConditionalFormatting : ConditionalFormatting, IDataBarConditionalFormatting {
		#region Fields
		internal const int fillColorItemIndex = 0;
		internal const int negativeFillColorItemIndex = 1;
		internal const int borderColorItemIndex = 2;
		internal const int negativeBorderColorItemIndex = 3;
		internal const int axisColorItemIndex = 4;
		const int unassignedColorValue = ColorModelInfoCache.DefaultItemIndex;
		ConditionalFormattingValueObject lowBound;
		ConditionalFormattingValueObject highBound;
		int colorIndex;
		int axisColorIndex;
		int borderColorIndex;
		int negativeValueColorIndex;
		int negativeValueBorderColorIndex;
		#endregion
		internal DataBarConditionalFormatting(Worksheet sheet)
			: base(sheet) {
			ConditionalFormattingIndex = ConditionalFormattingInfoCache.DefaultDataBarItemIndex;
		}
		DataBarConditionalFormatting(Worksheet sheet, CellRangeBase cellRange)
			: base(sheet, cellRange) {
			ConditionalFormattingIndex = ConditionalFormattingInfoCache.DefaultDataBarItemIndex;
		}
		protected internal DataBarConditionalFormatting(Worksheet sheet, CellRangeBase cellRange, ConditionalFormattingValueObject lowBound, ConditionalFormattingValueObject highBound, ColorModelInfo color)
			: this(sheet, cellRange) {
			Guard.ArgumentNotNull(lowBound, "lowBound");
			Guard.ArgumentNotNull(highBound, "highBound");
			Guard.ArgumentNotNull(color, "color");
			this.lowBound = lowBound.Clone();
			this.highBound = highBound.Clone();
			ApplyIndexChange();
			this.colorIndex = IndexFromColorModel(color);
			this.axisColorIndex = unassignedColorValue;
			this.borderColorIndex = unassignedColorValue;
			this.negativeValueColorIndex = unassignedColorValue;
			this.negativeValueBorderColorIndex = unassignedColorValue;
		}
		protected DataBarConditionalFormatting(Worksheet sheet, CellRangeBase cellRange, ColorModelInfo color)
			: this(sheet, cellRange) {
			Guard.ArgumentNotNull(color, "color");
			this.lowBound = new ConditionalFormattingValueObject(sheet, ConditionalFormattingValueObjectType.Min, 0);
			this.highBound = new ConditionalFormattingValueObject(sheet, ConditionalFormattingValueObjectType.Max, 0);
			ApplyIndexChange();
			this.colorIndex = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(color);
		}
		public DataBarConditionalFormatting(Worksheet sheet, CellRangeBase cellRange, ConditionalFormattingValueObject lowBound, ConditionalFormattingValueObject highBound, Color color)
			: this(sheet, cellRange, lowBound, highBound, ColorModelInfo.Create(color)) {
		}
		public DataBarConditionalFormatting(Worksheet sheet, CellRangeBase cellRange, Color color)
			: this(sheet, cellRange, ColorModelInfo.Create(color)) {
		}
		#region Properties
		protected internal int AxisColorIndex { get { return axisColorIndex; } set { axisColorIndex = value; } }
		protected internal int BorderColorIndex { get { return borderColorIndex; } set { borderColorIndex = value; } }
		protected internal int NegativeValueColorIndex { get { return negativeValueColorIndex; } set { negativeValueColorIndex = value; } }
		protected internal int NegativeValueBorderColorIndex { get { return negativeValueBorderColorIndex; } set { negativeValueBorderColorIndex = value; } }
		protected internal int ColorIndex { get { return colorIndex; } set { colorIndex = value; } }
		protected internal int LowBoundIndex { get { return IndexFromValue(lowBound); } }
		protected internal int HighBoundIndex { get { return IndexFromValue(highBound); } }
		#region ConditionalFormattingDataBarAxisPosition AxisPosition
		public ConditionalFormattingDataBarAxisPosition AxisPosition {
			get {
				DataBarConditionalFormattingInfo dbInfo = ConditionalFormattingInfo as DataBarConditionalFormattingInfo;
				return (dbInfo != null) ? dbInfo.AxisPosition : DataBarConditionalFormattingInfo.DefaultAxisPosition;
			}
			set {
				if (AxisPosition == value)
					return;
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetAxisPositionCore, value);
			}
		}
		DocumentModelChangeActions SetAxisPositionCore(ConditionalFormattingInfo info, ConditionalFormattingDataBarAxisPosition value) {
			DataBarConditionalFormattingInfo dbInfo = info as DataBarConditionalFormattingInfo;
			if (dbInfo != null) {
				dbInfo.AxisPosition = value;
			}
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ConditionalFormattingDataBarDirection Direction
		public ConditionalFormattingDataBarDirection Direction {
			get {
				DataBarConditionalFormattingInfo dbInfo = ConditionalFormattingInfo as DataBarConditionalFormattingInfo;
				return (dbInfo != null) ? dbInfo.Direction : DataBarConditionalFormattingInfo.DefaultDirection;
			}
			set {
				if (Direction == value)
					return;
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetDirectionCore, value);
			}
		}
		DocumentModelChangeActions SetDirectionCore(ConditionalFormattingInfo info, ConditionalFormattingDataBarDirection value) {
			DataBarConditionalFormattingInfo dbInfo = info as DataBarConditionalFormattingInfo;
			if (dbInfo != null) {
				dbInfo.Direction = value;
			}
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region int MinLength
		public int MinLength {
			get {
				DataBarConditionalFormattingInfo dbInfo = ConditionalFormattingInfo as DataBarConditionalFormattingInfo;
				return (dbInfo != null) ? dbInfo.MinWidth : DataBarConditionalFormattingInfo.DefaultMinWidth;
			}
			set {
				if (MinLength == value)
					return;
				if ((value < 0) || (value > 100))
					Exceptions.ThrowArgumentException("MinWidth", value);
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetMinWidthCore, value);
			}
		}
		DocumentModelChangeActions SetMinWidthCore(ConditionalFormattingInfo info, int value) {
			DataBarConditionalFormattingInfo dbInfo = info as DataBarConditionalFormattingInfo;
			dbInfo.MinWidth = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region int MaxLength
		public int MaxLength {
			get {
				DataBarConditionalFormattingInfo dbInfo = ConditionalFormattingInfo as DataBarConditionalFormattingInfo;
				return (dbInfo != null) ? dbInfo.MaxWidth : DataBarConditionalFormattingInfo.DefaultMaxWidth;
			}
			set {
				if (MaxLength == value)
					return;
				if ((value < 0) || (value > 100))
					Exceptions.ThrowArgumentException("MinWidth", value);
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetMaxWidthCore, value);
			}
		}
		DocumentModelChangeActions SetMaxWidthCore(ConditionalFormattingInfo info, int value) {
			DataBarConditionalFormattingInfo dbInfo = info as DataBarConditionalFormattingInfo;
			dbInfo.MaxWidth = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region bool GradientFill
		public bool GradientFill {
			get {
				DataBarConditionalFormattingInfo dbInfo = ConditionalFormattingInfo as DataBarConditionalFormattingInfo;
				return (dbInfo != null) ? dbInfo.GradientFill : DataBarConditionalFormattingInfo.DefaultGradientFillValue;
			}
			set {
				if (GradientFill == value)
					return;
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetGradientFillCore, value);
			}
		}
		DocumentModelChangeActions SetGradientFillCore(ConditionalFormattingInfo info, bool value) {
			DataBarConditionalFormattingInfo dbInfo = info as DataBarConditionalFormattingInfo;
			dbInfo.GradientFill = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region bool ShowValue
		public bool ShowValue {
			get {
				DataBarConditionalFormattingInfo dbInfo = ConditionalFormattingInfo as DataBarConditionalFormattingInfo;
				return (dbInfo != null) ? dbInfo.ShowValue : false;
			}
			set {
				if (ShowValue == value)
					return;
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetShowValueCore, value);
			}
		}
		DocumentModelChangeActions SetShowValueCore(ConditionalFormattingInfo info, bool value) {
			DataBarConditionalFormattingInfo dbInfo = info as DataBarConditionalFormattingInfo;
			dbInfo.ShowValue = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public ConditionalFormattingValueObject LowBound { get { return GetLowBound(); } set { SetLowBound(value); } }
		public ConditionalFormattingValueObject HighBound { get { return GetHighBound(); } set { SetHighBound(value); } }
		public Color Color { get { return GetColor(); } set { SetColor(value); } }
		public Color AxisColor { get { return GetAxisColor(); } set { SetAxisColor(value); } }
		public Color BorderColor { get { return GetBorderColor(); } set { SetBorderColor(value); } }
		public Color NegativeValueColor { get { return GetNegativeValueColor(); } set { SetNegativeValueColor(value); } }
		public Color NegativeValueBorderColor { get { return GetNegativeValueBorderColor(); } set { SetNegativeValueBorderColor(value); } }
		public bool IsNegativeBorderColorSameAsPositive { get { return negativeValueBorderColorIndex == borderColorIndex; } }
		public bool IsNegativeColorSameAsPositive { get { return negativeValueColorIndex == colorIndex; } }
		public bool IsBorderColorAssigned { get { return borderColorIndex != unassignedColorValue; } }
		public override ConditionalFormattingType Type { get { return ConditionalFormattingType.DataBar; } }
		public override ConditionalFormattingRuleType RuleType { get { return ConditionalFormattingRuleType.DataBar; } }
		#endregion
		#region Properties helpers
		protected void SetColor(Color color) {
			ColorModelInfo colorInfo = ColorModelInfo.Create(color);
			int index = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(colorInfo);
			if (index != ColorIndex) {
				DocumentHistory history = DocumentModel.History;
				ConditionalFormattingColorChangeHistoryItem item = new ConditionalFormattingColorChangeHistoryItem(DocumentModel, this, fillColorItemIndex, ColorIndex, index);
				history.Add(item);
				item.Execute();
			}
		}
		protected Color GetColor() {
			ColorModelInfo colorInfo = DocumentModel.Cache.ColorModelInfoCache[ColorIndex];
			return colorInfo.ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors);
		}
		protected void SetAxisColor(Color color) {
			ColorModelInfo colorInfo = ColorModelInfo.Create(color);
			int index = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(colorInfo);
			if (index != AxisColorIndex) {
				DocumentHistory history = DocumentModel.History;
				ConditionalFormattingColorChangeHistoryItem item = new ConditionalFormattingColorChangeHistoryItem(DocumentModel, this, axisColorItemIndex, AxisColorIndex, index);
				history.Add(item);
				item.Execute();
			}
		}
		protected Color GetAxisColor() {
			if (axisColorIndex == unassignedColorValue)
				return DXColor.Empty;
			ColorModelInfo colorInfo = DocumentModel.Cache.ColorModelInfoCache[axisColorIndex];
			return colorInfo.ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors);
		}
		protected void SetBorderColor(Color color) {
			ColorModelInfo colorInfo = ColorModelInfo.Create(color);
			int index = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(colorInfo);
			if (index != BorderColorIndex) {
				DocumentHistory history = DocumentModel.History;
				ConditionalFormattingColorChangeHistoryItem item = new ConditionalFormattingColorChangeHistoryItem(DocumentModel, this, borderColorItemIndex, BorderColorIndex, index);
				history.Add(item);
				item.Execute();
			}
		}
		protected Color GetBorderColor() {
			if (borderColorIndex == unassignedColorValue)
				return DXColor.Empty;
			ColorModelInfo colorInfo = DocumentModel.Cache.ColorModelInfoCache[borderColorIndex];
			return colorInfo.ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors);
		}
		protected void SetNegativeValueBorderColor(Color color) {
			ColorModelInfo colorInfo = ColorModelInfo.Create(color);
			int index = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(colorInfo);
			if (index != NegativeValueBorderColorIndex) {
				DocumentHistory history = DocumentModel.History;
				ConditionalFormattingColorChangeHistoryItem item = new ConditionalFormattingColorChangeHistoryItem(DocumentModel, this, negativeBorderColorItemIndex, NegativeValueBorderColorIndex, index);
				history.Add(item);
				item.Execute();
			}
		}
		protected Color GetNegativeValueBorderColor() {
			if (negativeValueBorderColorIndex == unassignedColorValue)
				return DXColor.Empty;
			ColorModelInfo colorInfo = DocumentModel.Cache.ColorModelInfoCache[negativeValueBorderColorIndex];
			return colorInfo.ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors);
		}
		protected void SetNegativeValueColor(Color color) {
			ColorModelInfo colorInfo = ColorModelInfo.Create(color);
			int index = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(colorInfo);
			if (index != NegativeValueColorIndex) {
				DocumentHistory history = DocumentModel.History;
				ConditionalFormattingColorChangeHistoryItem item = new ConditionalFormattingColorChangeHistoryItem(DocumentModel, this, negativeFillColorItemIndex, NegativeValueColorIndex, index);
				history.Add(item);
				item.Execute();
			}
		}
		protected Color GetNegativeValueColor() {
			if (negativeValueColorIndex == unassignedColorValue)
				return DXColor.Empty;
			ColorModelInfo colorInfo = DocumentModel.Cache.ColorModelInfoCache[negativeValueColorIndex];
			return colorInfo.ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors);
		}
		#region LowBound helpers
		protected ConditionalFormattingValueObject GetLowBound() {
			return (lowBound != null) ? lowBound.Clone() : null;
		}
		protected void SetLowBound(ConditionalFormattingValueObject value) {
#if DEBUGTEST
			int newIndex = (value != null) ? value.Index : -1;
#else
			int newIndex = value.Index;
#endif
			ConditionalFormattingValueChangeHistoryItem item = new ConditionalFormattingValueChangeHistoryItem(DocumentModel, this, (int)DataBarItemIndex.LowBound, LowBoundIndex, newIndex);
			ProcessSetBound(item);
		}
		protected internal void SetLowBoundIndex(int value) {
			lowBound = new ConditionalFormattingValueObject(Sheet, value);
		}
		#endregion
		#region HighBound helpers
		protected ConditionalFormattingValueObject GetHighBound() {
			return (highBound != null) ? highBound.Clone() : null;
		}
		protected void SetHighBound(ConditionalFormattingValueObject value) {
#if DEBUGTEST
			int newIndex = (value != null) ? value.Index : -1;
#else
			int newIndex = value.Index;
#endif
			ConditionalFormattingValueChangeHistoryItem item = new ConditionalFormattingValueChangeHistoryItem(DocumentModel, this, (int)DataBarItemIndex.HighBound, HighBoundIndex, newIndex);
			ProcessSetBound(item);
		}
		protected internal void SetHighBoundIndex(int value) {
			highBound = new ConditionalFormattingValueObject(Sheet, value);
		}
		#endregion
		protected internal void SetBoundIndex(DataBarItemIndex what, int value) {
			switch (what) {
				case DataBarItemIndex.HighBound:
					highBound = AcceptValueByInfoIndex(value);
					break;
				case DataBarItemIndex.LowBound:
					lowBound = AcceptValueByInfoIndex(value);
					break;
			}
		}
		#endregion
		protected override bool CheckValidity() {
			return base.CheckValidity() &&
				   (lowBound != null) && (highBound != null) &&
				   IsValueTypeNotMax(lowBound.ValueType) && IsValueTypeNotMin(highBound.ValueType);
		}
		public override bool Visit(IConditionalFormattingVisitor visitor) {
			return visitor.Visit(this);
		}
		public double EvaluateBarWidth(ICell cell) {
			ConditionalFormattingDataBarEvaluationResult value = Evaluate(cell);
			return value.Length;
		}
		public ConditionalFormattingDataBarEvaluationResult Evaluate(ICell cell) {
			ConditionalFormattingDataBarEvaluationResult result = new ConditionalFormattingDataBarEvaluationResult() { Length = -1 };
			if (CellRangeInternalNoHistory != null) {
				WorkbookDataContext dataContext = DocumentModel.DataContext;
				double value;
				if (TryGetCellValue(cell, dataContext, out value)) {
					double lowerBound;
					double upperBound;
					if (TryGetPointValue(lowBound, dataContext, out lowerBound) && TryGetPointValue(highBound, dataContext, out upperBound)) {
						if (lowerBound > upperBound) {
							double temp = lowerBound;
							lowerBound = upperBound;
							upperBound = temp;
						}
						result.MinValue = lowerBound;
						result.MaxValue = upperBound;
						result.MinLength = MinLength * 0.01;
						result.LengthBias = (MaxLength - MinLength) * 0.01;
						if (value >= lowerBound) {
							if (value >= upperBound) {
								result.Value = upperBound;
								result.Length = MaxLength;
							}
							else {
								result.Value = value;
								result.Length = MinLength + ((value - lowerBound) / (upperBound - lowerBound) * (MaxLength - MinLength));
							}
						}
					}
				}
			}
			return result;
		}
		void ApplyIndexChange() {
			if (CellRangeInternalNoHistory != null) {
				if (lowBound != null)
					lowBound.SetOwner(this);
				if (highBound != null)
					highBound.SetOwner(this);
			}
		}
		protected override void OnCellRangeChanged() {
			ApplyIndexChange();
		}
		protected internal static DataBarConditionalFormatting CreateCopy(DataBarConditionalFormatting source, Worksheet targetSheet) {
			DataBarConditionalFormatting result = source.CreateInstance(targetSheet) as DataBarConditionalFormatting;
			result.CopyFrom(source);
			return result;
		}
		protected override ConditionalFormatting CreateInstanceCore(Worksheet targetWorksheet) {
			return new DataBarConditionalFormatting(targetWorksheet);
		}
		public override void CopyFrom(ConditionalFormatting source) {
			Guard.ArgumentNotNull(source, "source");
			DataBarConditionalFormatting sourceDataBarCF = source as DataBarConditionalFormatting;
			System.Diagnostics.Debug.Assert(sourceDataBarCF != null);
			CopyBaseData(source);
			lowBound = HoldValue(sourceDataBarCF.LowBound);
			highBound = HoldValue(sourceDataBarCF.HighBound);
			ColorIndex = IndexFromColor(sourceDataBarCF.Color);
			NegativeValueColorIndex = IndexFromColor(sourceDataBarCF.NegativeValueColor);
			AxisColorIndex = IndexFromColor(sourceDataBarCF.AxisColor);
			BorderColorIndex = IndexFromColor(sourceDataBarCF.BorderColor);
			NegativeValueBorderColorIndex = IndexFromColor(sourceDataBarCF.NegativeValueBorderColor);
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			DataBarConditionalFormatting cf = obj as DataBarConditionalFormatting;
			if (cf == null)
				return false;
			return object.Equals(lowBound, cf.lowBound) && object.Equals(highBound, cf.highBound) &&
				colorIndex == cf.colorIndex &&
				axisColorIndex == cf.axisColorIndex &&
				borderColorIndex == cf.borderColorIndex &&
				negativeValueColorIndex == cf.negativeValueColorIndex &&
				negativeValueBorderColorIndex == cf.negativeValueBorderColorIndex;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(
				HashCodeCalculator.CalcHashCode32(base.GetHashCode(), lowBound.GetHashCode(), highBound.GetHashCode()),
				colorIndex, axisColorIndex, borderColorIndex, negativeValueColorIndex, negativeValueBorderColorIndex
				);
		}
	}
	#endregion
	#region FormulaConditionalFormatting
	#region IFormulaConditionalFormatting
	public interface IFormulaConditionalFormatting {
		bool Evaluate(ICell cell);
	}
	#endregion
	public abstract class FormulaConditionalFormatting : ConditionalFormatting, IFormulaConditionalFormatting, IDifferentialFormat, IRunFontInfo, IFillInfo, IBorderInfo, ICellAlignmentInfo, ICellProtectionInfo, IGradientFillInfo, IConvergenceInfo {
		#region Static
		protected static ParsedThingVariantValue CellValue = new ParsedThingVariantValue(VariantValue.Empty);
		protected static ParsedThingRefRel PtgRefRel = new ParsedThingRefRel(new CellOffset(0, 0, CellOffsetType.Offset, CellOffsetType.Offset)) { DataType = OperandDataType.Value };
		protected static ParsedThingBoolean PtgTrue = new ParsedThingBoolean() { Value = true };
		protected static ParsedThingNumeric PtgZero = new ParsedThingNumeric() { Value = 0 };
		protected static ParsedThingNumeric PtgOne = new ParsedThingNumeric() { Value = 1 };
		protected static int funcAnd = FormulaCalculator.GetFunctionByInvariantName("AND").Code;
		protected static int funcAverage = FormulaCalculator.GetFunctionByInvariantName("AVERAGE").Code;
		protected static int funcCount = FormulaCalculator.GetFunctionByInvariantName("COUNT").Code;
		protected static int funcCountIf = FormulaCalculator.GetFunctionByInvariantName("COUNTIF").Code;
		protected static int funcEDate = FormulaCalculator.GetFunctionByInvariantName("EDATE").Code;
		protected static int funcFloor = FormulaCalculator.GetFunctionByInvariantName("FLOOR").Code;
		protected static int funcIf = FormulaCalculator.GetFunctionByInvariantName("IF").Code;
		protected static int funcIsBlank = FormulaCalculator.GetFunctionByInvariantName("ISBLANK").Code;
		protected static int funcIsError = FormulaCalculator.GetFunctionByInvariantName("ISERROR").Code;
		protected static int funcIsNumber = FormulaCalculator.GetFunctionByInvariantName("ISNUMBER").Code;
		protected static int funcLarge = FormulaCalculator.GetFunctionByInvariantName("LARGE").Code;
		protected static int funcLeft = FormulaCalculator.GetFunctionByInvariantName("LEFT").Code;
		protected static int funcLen = FormulaCalculator.GetFunctionByInvariantName("LEN").Code;
		protected static int funcMax = FormulaCalculator.GetFunctionByInvariantName("MAX").Code;
		protected static int funcMin = FormulaCalculator.GetFunctionByInvariantName("MIN").Code;
		protected static int funcMonth = FormulaCalculator.GetFunctionByInvariantName("MONTH").Code;
		protected static int funcNot = FormulaCalculator.GetFunctionByInvariantName("NOT").Code;
		protected static int funcOr = FormulaCalculator.GetFunctionByInvariantName("OR").Code;
		protected static int funcRank = FormulaCalculator.GetFunctionByInvariantName("RANK").Code;
		protected static int funcRight = FormulaCalculator.GetFunctionByInvariantName("RIGHT").Code;
		protected static int funcRoundDown = FormulaCalculator.GetFunctionByInvariantName("ROUNDDOWN").Code;
		protected static int funcSearch = FormulaCalculator.GetFunctionByInvariantName("SEARCH").Code;
		protected static int funcSmall = FormulaCalculator.GetFunctionByInvariantName("SMALL").Code;
		protected static int funcStDev = FormulaCalculator.GetFunctionByInvariantName("STDEV").Code;
		protected static int funcToday = FormulaCalculator.GetFunctionByInvariantName("TODAY").Code;
		protected static int funcTrim = FormulaCalculator.GetFunctionByInvariantName("TRIM").Code;
		protected static int funcWeekday = FormulaCalculator.GetFunctionByInvariantName("WEEKDAY").Code;
		protected static int funcYear = FormulaCalculator.GetFunctionByInvariantName("YEAR").Code;
		#endregion
		ParsedExpression cachedExpression;
		protected FormulaConditionalFormatting(Worksheet sheet, CellRangeBase cellRange)
			: base(sheet, cellRange) {
			ConditionalFormattingIndex = ConditionalFormattingInfoCache.DefaultItemIndex;
		}
		#region Properties
		protected ParsedExpression CachedExpression { get { return cachedExpression; } set { cachedExpression = value; } }
		public virtual ConditionalFormattingOperator Operator { get { return ConditionalFormattingOperator.Unknown; } }
		public override ConditionalFormattingType Type { get { return ConditionalFormattingType.Formula; } }
		public override bool SupportsDifferentialFormat { get { return true; } }
		#region IRunFontInfo Members
		#region string IRunFontInfo.Name
		string IRunFontInfo.Name {
			get { return DifferentialFormatInfo.Font.Name; }
			set {
				if ((DifferentialFormatInfo.Font.Name == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontName)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontName, value);
			}
		}
		DocumentModelChangeActions SetFontName(FormatBase info, string value) {
			info.Font.Name = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IRunFontInfo.Color
		Color IRunFontInfo.Color {
			get { return DifferentialFormatInfo.Font.Color; }
			set {
				if ((DifferentialFormatInfo.Font.Color == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontColor, value);
			}
		}
		DocumentModelChangeActions SetFontColor(FormatBase info, Color value) {
			info.Font.Color = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.Bold
		bool IRunFontInfo.Bold {
			get { return DifferentialFormatInfo.Font.Bold; }
			set {
				if ((DifferentialFormatInfo.Font.Bold == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontBold)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontBold, value);
			}
		}
		DocumentModelChangeActions SetFontBold(FormatBase info, bool value) {
			info.Font.Bold = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.Condense
		bool IRunFontInfo.Condense {
			get { return DifferentialFormatInfo.Font.Condense; }
			set {
				if ((DifferentialFormatInfo.Font.Condense) == value && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontCondense)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontCondense, value);
			}
		}
		DocumentModelChangeActions SetFontCondense(FormatBase info, bool value) {
			info.Font.Condense = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.Extend
		bool IRunFontInfo.Extend {
			get { return DifferentialFormatInfo.Font.Extend; }
			set {
				if ((DifferentialFormatInfo.Font.Extend == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontExtend)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontExtend, value);
			}
		}
		DocumentModelChangeActions SetFontExtend(FormatBase info, bool value) {
			info.Font.Extend = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.Italic
		bool IRunFontInfo.Italic {
			get { return DifferentialFormatInfo.Font.Italic; }
			set {
				if ((DifferentialFormatInfo.Font.Italic == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontItalic)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontItalic, value);
			}
		}
		DocumentModelChangeActions SetFontItalic(FormatBase info, bool value) {
			info.Font.Italic = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.Outline
		bool IRunFontInfo.Outline {
			get { return DifferentialFormatInfo.Font.Outline; }
			set {
				if ((DifferentialFormatInfo.Font.Outline == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontOutline)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontOutline, value);
			}
		}
		DocumentModelChangeActions SetFontOutline(FormatBase info, bool value) {
			info.Font.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.Shadow
		bool IRunFontInfo.Shadow {
			get { return DifferentialFormatInfo.Font.Shadow; }
			set {
				if ((DifferentialFormatInfo.Font.Shadow == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontShadow)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontShadow, value);
			}
		}
		DocumentModelChangeActions SetFontShadow(FormatBase info, bool value) {
			info.Font.Shadow = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.StrikeThrough
		bool IRunFontInfo.StrikeThrough {
			get { return DifferentialFormatInfo.Font.StrikeThrough; }
			set {
				if ((DifferentialFormatInfo.Font.StrikeThrough == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontStrikeThrough)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontStrikeThrough, value);
			}
		}
		DocumentModelChangeActions SetFontStrikeThrough(FormatBase info, bool value) {
			info.Font.StrikeThrough = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IRunFontInfo.Charset
		int IRunFontInfo.Charset {
			get { return DifferentialFormatInfo.Font.Charset; }
			set {
				if ((DifferentialFormatInfo.Font.Charset == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontCharset)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontCharset, value);
			}
		}
		DocumentModelChangeActions SetFontCharset(FormatBase info, int value) {
			info.Font.Charset = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IRunFontInfo.FontFamily
		int IRunFontInfo.FontFamily {
			get { return DifferentialFormatInfo.Font.FontFamily; }
			set {
				if (DifferentialFormatInfo.Font.FontFamily == value && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontFamily)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontFamily, value);
			}
		}
		DocumentModelChangeActions SetFontFamily(FormatBase info, int value) {
			info.Font.FontFamily = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region double IRunFontInfo.Size
		double IRunFontInfo.Size {
			get { return DifferentialFormatInfo.Font.Size; }
			set {
				if ((DifferentialFormatInfo.Font.Size == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontSize)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontSize, value);
			}
		}
		DocumentModelChangeActions SetFontSize(FormatBase info, double value) {
			info.Font.Size = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlFontSchemeStyles IRunFontInfo.SchemeStyle
		XlFontSchemeStyles IRunFontInfo.SchemeStyle {
			get { return DifferentialFormatInfo.Font.SchemeStyle; }
			set {
				if ((DifferentialFormatInfo.Font.SchemeStyle == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontSchemeStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontSchemeStyle, value);
			}
		}
		DocumentModelChangeActions SetFontSchemeStyle(FormatBase info, XlFontSchemeStyles value) {
			info.Font.SchemeStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlScriptType IRunFontInfo.Script
		XlScriptType IRunFontInfo.Script {
			get { return DifferentialFormatInfo.Font.Script; }
			set {
				if ((DifferentialFormatInfo.Font.Script == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontScript)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontScript, value);
			}
		}
		DocumentModelChangeActions SetFontScript(FormatBase info, XlScriptType value) {
			info.Font.Script = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlUnderlineType IRunFontInfo.Underline
		XlUnderlineType IRunFontInfo.Underline {
			get { return DifferentialFormatInfo.Font.Underline; }
			set {
				if ((DifferentialFormatInfo.Font.Underline == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFontUnderline)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontUnderline, value);
			}
		}
		DocumentModelChangeActions SetFontUnderline(FormatBase info, XlUnderlineType value) {
			info.Font.Underline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IBorderInfo Members
		#region XlBorderLineStyle IBorderInfo.LeftLineStyle
		XlBorderLineStyle IBorderInfo.LeftLineStyle {
			get { return DifferentialFormatInfo.Border.LeftLineStyle; }
			set {
				if ((DifferentialFormatInfo.Border.LeftLineStyle == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyLeftLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderLeftLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.LeftLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.RightLineStyle
		XlBorderLineStyle IBorderInfo.RightLineStyle {
			get { return DifferentialFormatInfo.Border.RightLineStyle; }
			set {
				if ((DifferentialFormatInfo.Border.RightLineStyle == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyRightLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderRightLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderRightLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.RightLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.TopLineStyle
		XlBorderLineStyle IBorderInfo.TopLineStyle {
			get { return DifferentialFormatInfo.Border.TopLineStyle; }
			set {
				if ((DifferentialFormatInfo.Border.TopLineStyle == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyTopLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderTopLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderTopLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.TopLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.BottomLineStyle
		XlBorderLineStyle IBorderInfo.BottomLineStyle {
			get { return DifferentialFormatInfo.Border.BottomLineStyle; }
			set {
				if ((DifferentialFormatInfo.Border.BottomLineStyle == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyBottomLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderBottomLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.BottomLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.DiagonalUpLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalUpLineStyle {
			get { return DifferentialFormatInfo.Border.DiagonalUpLineStyle; }
			set {
				if ((DifferentialFormatInfo.Border.DiagonalUpLineStyle == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyDiagonalLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderDiagonalUpLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalUpLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalUpLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.DiagonalDownLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalDownLineStyle {
			get { return DifferentialFormatInfo.Border.DiagonalDownLineStyle; }
			set {
				if ((DifferentialFormatInfo.Border.DiagonalDownLineStyle) == value && DifferentialFormatInfo.BorderOptionsInfo.ApplyDiagonalLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderDiagonalDownLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalDownLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalDownLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.HorizontalLineStyle
		XlBorderLineStyle IBorderInfo.HorizontalLineStyle {
			get { return DifferentialFormatInfo.Border.HorizontalLineStyle; }
			set {
				if ((DifferentialFormatInfo.Border.HorizontalLineStyle == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyHorizontalLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderHorizontalLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.HorizontalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.VerticalLineStyle
		XlBorderLineStyle IBorderInfo.VerticalLineStyle {
			get { return DifferentialFormatInfo.Border.VerticalLineStyle; }
			set {
				if ((DifferentialFormatInfo.Border.VerticalLineStyle == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyVerticalLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderVerticalLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.VerticalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IBorderInfo.Outline
		bool IBorderInfo.Outline {
			get { return DifferentialFormatInfo.Border.Outline; }
			set {
				if ((DifferentialFormatInfo.Border.Outline == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyOutline)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderOutline, value);
			}
		}
		DocumentModelChangeActions SetBorderOutline(FormatBase info, bool value) {
			info.Border.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.LeftColor
		Color IBorderInfo.LeftColor {
			get { return DifferentialFormatInfo.Border.LeftColor; }
			set {
				if ((DifferentialFormatInfo.Border.LeftColor == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyLeftColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderLeftColor, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftColor(FormatBase info, Color value) {
			info.Border.LeftColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.RightColor
		Color IBorderInfo.RightColor {
			get { return DifferentialFormatInfo.Border.RightColor; }
			set {
				if ((DifferentialFormatInfo.Border.RightColor == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyRightColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderRightColor, value);
			}
		}
		DocumentModelChangeActions SetBorderRightColor(FormatBase info, Color value) {
			info.Border.RightColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.TopColor
		Color IBorderInfo.TopColor {
			get { return DifferentialFormatInfo.Border.TopColor; }
			set {
				if ((DifferentialFormatInfo.Border.TopColor == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyTopColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderTopColor, value);
			}
		}
		DocumentModelChangeActions SetBorderTopColor(FormatBase info, Color value) {
			info.Border.TopColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.BottomColor
		Color IBorderInfo.BottomColor {
			get { return DifferentialFormatInfo.Border.BottomColor; }
			set {
				if ((DifferentialFormatInfo.Border.BottomColor == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyBottomColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderBottomColor, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomColor(FormatBase info, Color value) {
			info.Border.BottomColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.DiagonalColor
		Color IBorderInfo.DiagonalColor {
			get { return DifferentialFormatInfo.Border.DiagonalColor; }
			set {
				if ((DifferentialFormatInfo.Border.DiagonalColor == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyDiagonalColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderDiagonalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalColor(FormatBase info, Color value) {
			info.Border.DiagonalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.HorizontalColor
		Color IBorderInfo.HorizontalColor {
			get { return DifferentialFormatInfo.Border.HorizontalColor; }
			set {
				if ((DifferentialFormatInfo.Border.HorizontalColor == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyHorizontalColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderHorizontalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalColor(FormatBase info, Color value) {
			info.Border.HorizontalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.VerticalColor
		Color IBorderInfo.VerticalColor {
			get { return DifferentialFormatInfo.Border.VerticalColor; }
			set {
				if ((DifferentialFormatInfo.Border.VerticalColor == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyVerticalColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderVerticalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalColor(FormatBase info, Color value) {
			info.Border.VerticalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.LeftColorIndex
		int IBorderInfo.LeftColorIndex {
			get { return DifferentialFormatInfo.Border.LeftColorIndex; }
			set {
				if ((DifferentialFormatInfo.Border.LeftColorIndex == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyLeftColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderLeftColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftColorIndex(FormatBase info, int value) {
			info.Border.LeftColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.RightColorIndex
		int IBorderInfo.RightColorIndex {
			get { return DifferentialFormatInfo.Border.RightColorIndex; }
			set {
				if ((DifferentialFormatInfo.Border.RightColorIndex == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyRightColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderRightColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderRightColorIndex(FormatBase info, int value) {
			info.Border.RightColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.TopColorIndex
		int IBorderInfo.TopColorIndex {
			get { return DifferentialFormatInfo.Border.TopColorIndex; }
			set {
				if ((DifferentialFormatInfo.Border.TopColorIndex == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyTopColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderTopColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderTopColorIndex(FormatBase info, int value) {
			info.Border.TopColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.BottomColorIndex
		int IBorderInfo.BottomColorIndex {
			get { return DifferentialFormatInfo.Border.BottomColorIndex; }
			set {
				if ((DifferentialFormatInfo.Border.BottomColorIndex == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyBottomColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderBottomColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomColorIndex(FormatBase info, int value) {
			info.Border.BottomColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.DiagonalColorIndex
		int IBorderInfo.DiagonalColorIndex {
			get { return DifferentialFormatInfo.Border.DiagonalColorIndex; }
			set {
				if ((DifferentialFormatInfo.Border.DiagonalColorIndex == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyDiagonalColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderDiagonalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalColorIndex(FormatBase info, int value) {
			info.Border.DiagonalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.HorizontalColorIndex
		int IBorderInfo.HorizontalColorIndex {
			get { return DifferentialFormatInfo.Border.HorizontalColorIndex; }
			set {
				if ((DifferentialFormatInfo.Border.HorizontalColorIndex == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyHorizontalColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderHorizontalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalColorIndex(FormatBase info, int value) {
			info.Border.HorizontalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.VerticalColorIndex
		int IBorderInfo.VerticalColorIndex {
			get { return DifferentialFormatInfo.Border.VerticalColorIndex; }
			set {
				if ((DifferentialFormatInfo.Border.VerticalColorIndex == value) && DifferentialFormatInfo.BorderOptionsInfo.ApplyVerticalColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderVerticalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalColorIndex(FormatBase info, int value) {
			info.Border.VerticalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IFillInfo Members
		#region IFillInfo.ClearFill
		void IFillInfo.Clear() {
			if (!DifferentialFormatInfo.MultiOptionsInfo.ApplyFillNone)
				ClearFillCore();
		}
		void ClearFillCore() {
			DocumentModel.BeginUpdate();
			try {
				DifferentialFormat info = (DifferentialFormat)GetInfoForModification(DifferentialFormatIndexAccessor);
				info.Fill.Clear();
				ReplaceInfo(DifferentialFormatIndexAccessor, info, DocumentModelChangeActions.None);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region XlPatternType IFillInfo.PatternType
		XlPatternType IFillInfo.PatternType {
			get { return DifferentialFormatInfo.Fill.PatternType; }
			set {
				if ((DifferentialFormatInfo.Fill.PatternType == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFillPatternType)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFillPatternType, value);
			}
		}
		DocumentModelChangeActions SetFillPatternType(FormatBase info, XlPatternType value) {
			info.Fill.PatternType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IFillInfo.ForeColor
		Color IFillInfo.ForeColor {
			get { return DifferentialFormatInfo.Fill.ForeColor; }
			set {
				if ((DifferentialFormatInfo.Fill.ForeColor == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFillForeColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFillForeColor, value);
			}
		}
		DocumentModelChangeActions SetFillForeColor(FormatBase info, Color value) {
			info.Fill.ForeColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IFillInfo.BackColor
		Color IFillInfo.BackColor {
			get { return DifferentialFormatInfo.Fill.BackColor; }
			set {
				if ((DifferentialFormatInfo.Fill.BackColor == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyFillBackColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFillBackColor, value);
			}
		}
		DocumentModelChangeActions SetFillBackColor(FormatBase info, Color value) {
			info.Fill.BackColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IGradientFillInfo IFillInfo.GradientFill
		IGradientFillInfo IFillInfo.GradientFill { get { return this; } }
		#endregion
		#region ModelFillType FillType
		ModelFillType IFillInfo.FillType {
			get { return DifferentialFormatInfo.Fill.FillType; }
			set {
				if (DifferentialFormatInfo.Fill.FillType == value)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetModelFillType, value);
			}
		}
		DocumentModelChangeActions SetModelFillType(FormatBase info, ModelFillType value) {
			info.Fill.FillType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IGradientFillInfo Members
		IConvergenceInfo IGradientFillInfo.Convergence { get { return this; } }
		IGradientStopCollection IGradientFillInfo.GradientStops { get { return DifferentialFormatInfo.Fill.GradientFill.GradientStops; } }
		#region IGradientFillInfo.Type
		ModelGradientFillType IGradientFillInfo.Type {
			get { return DifferentialFormatInfo.Fill.GradientFill.Type; }
			set {
				if (DifferentialFormatInfo.Fill.GradientFill.Type == value)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetGradientFillType, value);
			}
		}
		DocumentModelChangeActions SetGradientFillType(FormatBase info, ModelGradientFillType value) {
			info.Fill.GradientFill.Type = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IGradientFillInfo.Degree
		double IGradientFillInfo.Degree {
			get { return DifferentialFormatInfo.Fill.GradientFill.Degree; }
			set {
				if (DifferentialFormatInfo.Fill.GradientFill.Degree == value)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetGradientFillDegree, value);
			}
		}
		DocumentModelChangeActions SetGradientFillDegree(FormatBase info, double value) {
			info.Fill.GradientFill.Degree = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo Members
		#region IConvergenceInfo.Left
		float IConvergenceInfo.Left {
			get { return DifferentialFormatInfo.Fill.GradientFill.Convergence.Left; }
			set {
				if (DifferentialFormatInfo.Fill.GradientFill.Convergence.Left == value)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetConvergenceLeft, value);
			}
		}
		DocumentModelChangeActions SetConvergenceLeft(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Left = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Right
		float IConvergenceInfo.Right {
			get { return DifferentialFormatInfo.Fill.GradientFill.Convergence.Right; }
			set {
				if (DifferentialFormatInfo.Fill.GradientFill.Convergence.Right == value)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetConvergenceRight, value);
			}
		}
		DocumentModelChangeActions SetConvergenceRight(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Right = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Top
		float IConvergenceInfo.Top {
			get { return DifferentialFormatInfo.Fill.GradientFill.Convergence.Top; }
			set {
				if (DifferentialFormatInfo.Fill.GradientFill.Convergence.Top == value)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetConvergenceTop, value);
			}
		}
		DocumentModelChangeActions SetConvergenceTop(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Top = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Bottom
		float IConvergenceInfo.Bottom {
			get { return DifferentialFormatInfo.Fill.GradientFill.Convergence.Bottom; }
			set {
				if (DifferentialFormatInfo.Fill.GradientFill.Convergence.Bottom == value)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetConvergenceBottom, value);
			}
		}
		DocumentModelChangeActions SetConvergenceBottom(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Bottom = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		#endregion
		#region ICellAlignmentInfo Members
		#region bool ICellAlignmentInfo.WrapText
		bool ICellAlignmentInfo.WrapText {
			get { return DifferentialFormatInfo.Alignment.WrapText; }
			set {
				if ((DifferentialFormatInfo.Alignment.WrapText == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyAlignmentWrapText)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetWrapText, value);
			}
		}
		DocumentModelChangeActions SetWrapText(FormatBase info, bool value) {
			info.Alignment.WrapText = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool ICellAlignmentInfo.JustifyLastLine
		bool ICellAlignmentInfo.JustifyLastLine {
			get { return DifferentialFormatInfo.Alignment.JustifyLastLine; }
			set {
				if ((DifferentialFormatInfo.Alignment.JustifyLastLine == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyAlignmentJustifyLastLine)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetJustifyLastLine, value);
			}
		}
		DocumentModelChangeActions SetJustifyLastLine(FormatBase info, bool value) {
			info.Alignment.JustifyLastLine = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool ICellAlignmentInfo.ShrinkToFit
		bool ICellAlignmentInfo.ShrinkToFit {
			get { return DifferentialFormatInfo.Alignment.ShrinkToFit; }
			set {
				if ((DifferentialFormatInfo.Alignment.ShrinkToFit == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyAlignmentShrinkToFit)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetShrinkToFit, value);
			}
		}
		DocumentModelChangeActions SetShrinkToFit(FormatBase info, bool value) {
			info.Alignment.ShrinkToFit = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int ICellAlignmentInfo.TextRotation
		int ICellAlignmentInfo.TextRotation {
			get { return DifferentialFormatInfo.Alignment.TextRotation; }
			set {
				if ((DifferentialFormatInfo.Alignment.TextRotation == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyAlignmentTextRotation)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetTextRotation, value);
			}
		}
		DocumentModelChangeActions SetTextRotation(FormatBase info, int value) {
			info.Alignment.TextRotation = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region byte ICellAlignmentInfo.Indent
		byte ICellAlignmentInfo.Indent {
			get { return DifferentialFormatInfo.Alignment.Indent; }
			set {
				if ((DifferentialFormatInfo.Alignment.Indent == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyAlignmentIndent)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetIndent, value);
			}
		}
		DocumentModelChangeActions SetIndent(FormatBase info, byte value) {
			info.Alignment.Indent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int ICellAlignmentInfo.RelativeIndent
		int ICellAlignmentInfo.RelativeIndent {
			get { return DifferentialFormatInfo.Alignment.RelativeIndent; }
			set {
				if ((DifferentialFormatInfo.Alignment.RelativeIndent == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyAlignmentRelativeIndent)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetRelativeIndent, value);
			}
		}
		DocumentModelChangeActions SetRelativeIndent(FormatBase info, int value) {
			info.Alignment.RelativeIndent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlHorizontalAlignment ICellAlignmentInfo.Horizontal
		XlHorizontalAlignment ICellAlignmentInfo.Horizontal {
			get { return DifferentialFormatInfo.Alignment.Horizontal; }
			set {
				if ((DifferentialFormatInfo.Alignment.Horizontal == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyAlignmentHorizontal)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetHorizontal, value);
			}
		}
		DocumentModelChangeActions SetHorizontal(FormatBase info, XlHorizontalAlignment value) {
			info.Alignment.Horizontal = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlVerticalAlignment ICellAlignmentInfo.Vertical
		XlVerticalAlignment ICellAlignmentInfo.Vertical {
			get { return DifferentialFormatInfo.Alignment.Vertical; }
			set {
				if ((DifferentialFormatInfo.Alignment.Vertical == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyAlignmentVertical)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetVertical, value);
			}
		}
		DocumentModelChangeActions SetVertical(FormatBase info, XlVerticalAlignment value) {
			info.Alignment.Vertical = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ReadingOrder ICellAlignmentInfo.ReadingOrder
		XlReadingOrder ICellAlignmentInfo.ReadingOrder {
			get { return DifferentialFormatInfo.Alignment.ReadingOrder; }
			set {
				if ((DifferentialFormatInfo.Alignment.ReadingOrder == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyAlignmentReadingOrder)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetReadingOrder, value);
			}
		}
		DocumentModelChangeActions SetReadingOrder(FormatBase info, XlReadingOrder value) {
			info.Alignment.ReadingOrder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region ICellProtectionInfo Members
		#region bool ICellProtectionInfo.Locked
		bool ICellProtectionInfo.Locked {
			get { return DifferentialFormatInfo.Protection.Locked; }
			set {
				if ((DifferentialFormatInfo.Protection.Locked == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyProtectionLocked)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetLocked, value);
			}
		}
		DocumentModelChangeActions SetLocked(FormatBase info, bool value) {
			info.Protection.Locked = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool ICellProtectionInfo.Hidden
		bool ICellProtectionInfo.Hidden {
			get { return DifferentialFormatInfo.Protection.Hidden; }
			set {
				if ((DifferentialFormatInfo.Protection.Hidden == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyProtectionHidden)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetHidden, value);
			}
		}
		DocumentModelChangeActions SetHidden(FormatBase info, bool value) {
			info.Protection.Hidden = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		protected abstract void RefreshCachedExpression();
		protected CellPosition GetCellRangeAnchor() {
			return CellRange != null ? CellRange.TopLeft : new CellPosition(0, 0);
		}
		protected string GetStringValue(ParsedExpression expression) {
			return GetStringValue(expression, GetCellRangeAnchor());
		}
		protected string GetStringValue(ParsedExpression expression, CellPosition currentCell) {
			if (expression == null)
				return null;
			WorkbookDataContext context = DocumentModel.DataContext;
			context.PushCurrentCell(currentCell);
			try {
				return "=" + expression.BuildExpressionString(context);
			}
			finally {
				context.PopCurrentCell();
			}
		}
		#region IDifferentialFormat Members
		public IRunFontInfo Font { get { return this; } }
		public ICellAlignmentInfo Alignment { get { return this; } }
		public ICellProtectionInfo Protection { get { return this; } }
		public IBorderInfo Border { get { return this; } }
		public IFillInfo Fill { get { return this; } }
		#region string IDifferentialFormat.FormatString
		public string FormatString {
			get { return DifferentialFormatInfo.FormatString; }
			set {
				if ((DifferentialFormatInfo.FormatString == value) && DifferentialFormatInfo.MultiOptionsInfo.ApplyNumberFormat)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetNumberFormat, value);
			}
		}
		DocumentModelChangeActions SetNumberFormat(FormatBase info, string value) {
			((DifferentialFormat)info).FormatString = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		public override bool Visit(IConditionalFormattingVisitor visitor) {
			return visitor.Visit(this);
		}
		public virtual bool Evaluate(ICell cell) {
			ParsedExpression cachedExpression = CachedExpression;
			if (cachedExpression == null)
				return false;
			Guard.ArgumentNotNull(cell, "cell");
			CellValue.Value = cell.Value;
			WorkbookDataContext context = Sheet.DataContext;
			context.PushCurrentCell(cell);
			try {
				VariantValue result = cachedExpression.Evaluate(context).ToBoolean(context);
				if (!result.IsError)
					return result.BooleanValue;
			}
			finally {
				context.PopCurrentCell();
			}
			return false;
		}
		protected override void OnCellRangeChanged() {
			RefreshCachedExpression();
		}
		protected internal static FormulaConditionalFormatting CreateCopy(FormulaConditionalFormatting source, Worksheet targetSheet) {
			FormulaConditionalFormatting result = source.CreateInstance(targetSheet) as FormulaConditionalFormatting;
			result.CopyFrom(source);
			return result;
		}
		public override void CopyFrom(ConditionalFormatting source) {
			Guard.ArgumentNotNull(source, "source");
			FormulaConditionalFormatting sourceFormulaCF = source as FormulaConditionalFormatting;
			System.Diagnostics.Debug.Assert(sourceFormulaCF != null);
			CopyBaseData(source);
			CopyFormatData(source);
			CopyFormulaData(sourceFormulaCF);
			CachedExpression = sourceFormulaCF.CachedExpression;
		}
		protected abstract void CopyFormulaData(FormulaConditionalFormatting source);
	}
	#endregion
	#region IconSetConditionalFormatting
	#region IIconSetConditionalFormatting
	public interface IIconSetConditionalFormatting {
		bool Percent { get; set; }
		bool Reversed { get; set; }
		bool ShowValue { get; set; }
		IconSetType IconSet { get; set; }
		bool IsCustom { get; set; }
		void SetPointValue(int itemIndex, ConditionalFormattingValueObject value);
		ConditionalFormattingValueObject GetPointValue(int index);
		bool IsValueExist(int itemIndex);
		int EvaluateIconIndex(ICellBase cell);
		void SetCustomIcon(int index, ConditionalFormattingCustomIcon value);
		ConditionalFormattingCustomIcon GetIcon(int index);
	}
	#endregion
	public class IconSetConditionalFormatting : ConditionalFormatting, IIconSetConditionalFormatting {
		#region Static
		internal static Dictionary<IconSetType, int> expectedPointsNumberTable = CreateExpectedPointsNumberTable();
		static readonly int maxExpectedPointsNumber = GetMaxPointNumber(expectedPointsNumberTable);
		static Dictionary<IconSetType, int> CreateExpectedPointsNumberTable() {
			Dictionary<IconSetType, int> result = new Dictionary<IconSetType, int>();
			result.Add(IconSetType.Arrows3, 3);
			result.Add(IconSetType.ArrowsGray3, 3);
			result.Add(IconSetType.Flags3, 3);
			result.Add(IconSetType.TrafficLights13, 3);
			result.Add(IconSetType.TrafficLights23, 3);
			result.Add(IconSetType.Signs3, 3);
			result.Add(IconSetType.Symbols3, 3);
			result.Add(IconSetType.Symbols23, 3);
			result.Add(IconSetType.Stars3, 3);
			result.Add(IconSetType.Triangles3, 3);
			result.Add(IconSetType.Arrows4, 4);
			result.Add(IconSetType.ArrowsGray4, 4);
			result.Add(IconSetType.RedToBlack4, 4);
			result.Add(IconSetType.Rating4, 4);
			result.Add(IconSetType.TrafficLights4, 4);
			result.Add(IconSetType.Arrows5, 5);
			result.Add(IconSetType.ArrowsGray5, 5);
			result.Add(IconSetType.Rating5, 5);
			result.Add(IconSetType.Quarters5, 5);
			result.Add(IconSetType.Boxes5, 5);
			return result;
		}
		static int GetMaxPointNumber(Dictionary<IconSetType, int> values) {
			int result = 1;
			foreach (KeyValuePair<IconSetType, int> item in values) {
				int count = item.Value;
				if (count > result)
					result = count;
			}
			return result;
		}
		#endregion
		ConditionalFormattingValueObject[] pointValues;
		int expectedPointsNumber;
		internal IconSetConditionalFormatting(Worksheet sheet)
			: base(sheet) {
			ConditionalFormattingIndex = ConditionalFormattingInfoCache.DefaultIconSetItemIndex;
			if (!expectedPointsNumberTable.TryGetValue(IconSet, out expectedPointsNumber))
				ExpectedPointsNumber = 0;
			this.pointValues = new ConditionalFormattingValueObject[maxExpectedPointsNumber];
		}
		public IconSetConditionalFormatting(Worksheet sheet, CellRangeBase cellRange, IconSetType iconSet, ConditionalFormattingValueObject[] points)
			: base(sheet, cellRange) {
			IconSetConditionalFormattingInfo info = sheet.Workbook.Cache.ConditionalFormattingInfoCache[ConditionalFormattingInfoCache.DefaultIconSetItemIndex].Clone() as IconSetConditionalFormattingInfo;
			info.IconSet = iconSet;
			ConditionalFormattingIndex = sheet.Workbook.Cache.ConditionalFormattingInfoCache.GetItemIndex(info);
			this.pointValues = new ConditionalFormattingValueObject[maxExpectedPointsNumber];
			if (expectedPointsNumberTable.TryGetValue(iconSet, out expectedPointsNumber)) {
				int limit = Math.Min(ExpectedPointsNumber, points.Length);
				for (int i = 0; i < limit; ++i) {
					this.pointValues[i] = points[i].Clone();
					this.pointValues[i].SetOwner(this);
				}
			}
		}
		#region Properties
		public int ExpectedPointsNumber { get { return expectedPointsNumber; } protected set { expectedPointsNumber = value; } }
		#region bool Percent
		public bool Percent {
			get {
				IconSetConditionalFormattingInfo isInfo = ConditionalFormattingInfo as IconSetConditionalFormattingInfo;
				return (isInfo != null) ? isInfo.Percent : false;
			}
			set {
				if (Percent == value)
					return;
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetPercentCore, value);
			}
		}
		DocumentModelChangeActions SetPercentCore(ConditionalFormattingInfo info, bool value) {
			IconSetConditionalFormattingInfo isInfo = info as IconSetConditionalFormattingInfo;
			isInfo.Percent = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region bool Reversed
		public bool Reversed {
			get {
				IconSetConditionalFormattingInfo isInfo = ConditionalFormattingInfo as IconSetConditionalFormattingInfo;
				return (isInfo != null) ? isInfo.Reversed : false;
			}
			set {
				if (Reversed == value)
					return;
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetReversedCore, value);
			}
		}
		DocumentModelChangeActions SetReversedCore(ConditionalFormattingInfo info, bool value) {
			IconSetConditionalFormattingInfo isInfo = info as IconSetConditionalFormattingInfo;
			isInfo.Reversed = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region bool ShowValue
		public bool ShowValue {
			get {
				IconSetConditionalFormattingInfo isInfo = ConditionalFormattingInfo as IconSetConditionalFormattingInfo;
				return (isInfo != null) ? isInfo.ShowValue : false;
			}
			set {
				if (ShowValue == value)
					return;
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetShowValueCore, value);
			}
		}
		DocumentModelChangeActions SetShowValueCore(ConditionalFormattingInfo info, bool value) {
			IconSetConditionalFormattingInfo isInfo = info as IconSetConditionalFormattingInfo;
			isInfo.ShowValue = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region bool IsCustom
		public bool IsCustom {
			get {
				IconSetConditionalFormattingInfo isInfo = ConditionalFormattingInfo as IconSetConditionalFormattingInfo;
				return (isInfo != null) ? isInfo.CustomIconSet : false;
			}
			set {
				if (IsCustom == value)
					return;
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetIsCustomCore, value);
			}
		}
		DocumentModelChangeActions SetIsCustomCore(ConditionalFormattingInfo info, bool value) {
			IconSetConditionalFormattingInfo isInfo = info as IconSetConditionalFormattingInfo;
			isInfo.CustomIconSet = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IconSetType IconSet
		public IconSetType IconSet {
			get {
				IconSetConditionalFormattingInfo isInfo = ConditionalFormattingInfo as IconSetConditionalFormattingInfo;
				return (isInfo != null) ? isInfo.IconSet : IconSetType.None;
			}
			set {
				if (IconSet == value)
					return;
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetIconSetCore, value);
			}
		}
		DocumentModelChangeActions SetIconSetCore(ConditionalFormattingInfo info, IconSetType value) {
			IconSetConditionalFormattingInfo isInfo = info as IconSetConditionalFormattingInfo;
			if (isInfo != null) {
				isInfo.IconSet = value;
			}
			return DocumentModelChangeActions.None;
		}
		#endregion
		public override ConditionalFormattingType Type { get { return ConditionalFormattingType.IconSet; } }
		public override ConditionalFormattingRuleType RuleType { get { return ConditionalFormattingRuleType.IconSet; } }
		#endregion
		bool CheckValidityOfFirstPoint() {
			ConditionalFormattingValueObject pointValue = pointValues[0];
			return (pointValue != null) && IsValueTypeNotMax(pointValue.ValueType);
		}
		bool CheckValidityOfInternalPoints() {
			for (int i = 1; i < (ExpectedPointsNumber - 1); ++i) {
				ConditionalFormattingValueObject pointValue = pointValues[i];
				if (pointValue == null)
					return false;
				ConditionalFormattingValueObjectType valueType = pointValue.ValueType;
				if (IsValueTypeMax(valueType) || IsValueTypeMin(valueType))
					return false;
			}
			return true;
		}
		bool CheckValidityOfLastPoint() {
			ConditionalFormattingValueObject pointValue = pointValues[ExpectedPointsNumber - 1];
			return (pointValue != null) && IsValueTypeNotMin(pointValue.ValueType);
		}
		protected override bool CheckValidity() {
			return base.CheckValidity() && (ExpectedPointsNumber > 1) && CheckValidityOfFirstPoint() && CheckValidityOfInternalPoints() && CheckValidityOfLastPoint();
		}
		public override bool Visit(IConditionalFormattingVisitor visitor) {
			return visitor.Visit(this);
		}
		protected bool CheckIndex(int itemIndex) {
			if (itemIndex < 0 || itemIndex >= ExpectedPointsNumber)
				return false;
			return true;
		}
		protected internal int GetPointValueIndex(int itemIndex) {
			ConditionalFormattingValueObject valueObject = this.pointValues[itemIndex];
			return IndexFromValue(valueObject);
		}
		protected internal void SetPointValueIndex(int itemIndex, int value) {
			this.pointValues[itemIndex] = (value >= 0) ? new ConditionalFormattingValueObject(Sheet, value) : null;
		}
		public void SetPointValue(int itemIndex, ConditionalFormattingValueObject value) {
#if !DEBUGTEST
			Guard.ArgumentNotNull(value, "value"); 
#endif
			if (!CheckIndex(itemIndex))
				return;
			int oldIndex = GetPointValueIndex(itemIndex);
#if DEBUGTEST
			int newIndex = IndexFromValue(value);
#else
			int newIndex = value.Index;
#endif
			if (oldIndex != newIndex) {
				DocumentHistory history = DocumentModel.History;
				ConditionalFormattingValueChangeHistoryItem item = new ConditionalFormattingValueChangeHistoryItem(DocumentModel, this, itemIndex, oldIndex, newIndex);
				history.Add(item);
				item.Execute();
			}
		}
		public ConditionalFormattingValueObject GetPointValue(int index) {
			if (CheckIndex(index)) {
				int valueIndex = GetPointValueIndex(index);
				if (index >= 0)
					return new ConditionalFormattingValueObject(Sheet, valueIndex);
			}
			return null;
		}
		public ConditionalFormattingValueObject[] GetPointValues() {
			ConditionalFormattingValueObject[] result = new ConditionalFormattingValueObject[ExpectedPointsNumber];
			Array.Copy(pointValues, result, expectedPointsNumber);
			return result;
		}
		public void SetCustomIcon(int index, ConditionalFormattingCustomIcon value) {
			if (!IsCustom)
				return;
			if (!CheckIndex(index))
				return;
			if (GetIconCore(index) == value)
				return;
			if (!IconSetConditionalFormattingInfo.IconSetContainIcon(value.IconSet, value.IconIndex))
				return;
			ConditionalFormattingCustomIconHolder valueHolder = new ConditionalFormattingCustomIconHolder(index, value);
			SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetCustomIconCore, valueHolder);
		}
		DocumentModelChangeActions SetCustomIconCore(ConditionalFormattingInfo info, ConditionalFormattingCustomIconHolder value) {
			IconSetConditionalFormattingInfo isInfo = info as IconSetConditionalFormattingInfo;
			if (isInfo != null) {
				isInfo.SetCustomIcon(value.Index, value.CustomIcon);
			}
			return DocumentModelChangeActions.None;
		}
		public void SetCustomIcons(ConditionalFormattingCustomIcon[] values) {
			Guard.ArgumentNotNull(values, "values");
			if (!IsCustom)
				return;
			long newIconHash;
			IconSetConditionalFormattingInfo isInfo = ConditionalFormattingInfo as IconSetConditionalFormattingInfo;
			if (isInfo != null) {
				newIconHash = isInfo.ModifiedIconValue(values);
				if (isInfo.IsSameIconSequence(newIconHash))
					return;
				SetPropertyValue(ConditionalFormattingInfoIndexAccessor, SetCustomIconsCore, newIconHash);
			}
		}
		DocumentModelChangeActions SetCustomIconsCore(ConditionalFormattingInfo info, long newValue) {
			IconSetConditionalFormattingInfo isInfo = info as IconSetConditionalFormattingInfo;
			if (isInfo != null) {
				isInfo.ApplyModifiedIconValue(newValue);
			}
			return DocumentModelChangeActions.None;
		}
		ConditionalFormattingCustomIcon GetIconCore(int index) {
			IconSetConditionalFormattingInfo isInfo = ConditionalFormattingInfo as IconSetConditionalFormattingInfo;
			return (isInfo != null) ? isInfo.GetIcon(index) : new ConditionalFormattingCustomIcon(IconSetType.None, 0);
		}
		public ConditionalFormattingCustomIcon GetIcon(int index) {
			if (!CheckIndex(index))
				return null; 
			return GetIconCore(index);
		}
		public bool IsValueExist(int itemIndex) {
			return CheckIndex(itemIndex) && (GetPointValueIndex(itemIndex) >= 0);
		}
		void ApplyIndexChange() {
			if (CellRangeInternalNoHistory != null) {
				for (int i = 0; i < ExpectedPointsNumber; ++i) {
					ConditionalFormattingValueObject item = this.pointValues[i];
					if (item != null)
						item.SetOwner(this);
				}
			}
		}
		protected void AfterChangeIconSet() {
			if (!expectedPointsNumberTable.TryGetValue(IconSet, out expectedPointsNumber))
				ExpectedPointsNumber = 0;
		}
		protected  override void ApplyChanges(DocumentModelChangeActions actions) {
			base.ApplyChanges(actions);
			AfterChangeIconSet();
		}
		protected override void OnCellRangeChanged() {
			ApplyIndexChange();
		}
		protected int GetIconIndexNormal(WorkbookDataContext dataContext, double cellValue) {
			int result = -1;
			for (int i = ExpectedPointsNumber - 1; i >= 0; --i) {
				double pointValue;
				if (TryGetPointValue(pointValues[i], dataContext, out pointValue))
					if (cellValue >= pointValue) {
						result = i;
						break;
					}
			}
			return result;
		}
		public int EvaluateIconIndex(ICellBase cell) {
			int result = -1;
			if (CellRangeInternalNoHistory != null) {
				WorkbookDataContext context = DocumentModel.DataContext;
				double cellValue;
				if (TryGetCellValue(cell, context, out cellValue)) {
					context.PushCurrentWorksheet(cell.Sheet);
					context.PushCurrentCell(cell.Position);
					try {
						result = GetIconIndexNormal(context, cellValue);
					}
					finally {
						context.PopCurrentCell();
						context.PopCurrentWorksheet();
					}
					if (Reversed && (result >= 0))
						result = pointValues.Length - result - 1;
				}
			}
			return result;
		}
		protected internal static IconSetConditionalFormatting CreateCopy(IconSetConditionalFormatting source, Worksheet targetSheet) {
			IconSetConditionalFormatting result = source.CreateInstance(targetSheet) as IconSetConditionalFormatting;
			result.CopyFrom(source);
			return result;
		}
		protected override ConditionalFormatting CreateInstanceCore(Worksheet targetWorksheet) {
			return new IconSetConditionalFormatting(targetWorksheet);
		}
		protected void CopyPoints(IconSetConditionalFormatting source) {
			for (int i = 0; i < source.ExpectedPointsNumber; ++i)
				pointValues[i] = HoldValue(source.pointValues[i]);
		}
		public override void CopyFrom(ConditionalFormatting source) {
			Guard.ArgumentNotNull(source, "source");
			IconSetConditionalFormatting sourceIconSetCF = source as IconSetConditionalFormatting;
			System.Diagnostics.Debug.Assert(sourceIconSetCF != null);
			CopyBaseData(source);
			CopyPoints(sourceIconSetCF);
			AfterChangeIconSet();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			IconSetConditionalFormatting cf = obj as IconSetConditionalFormatting;
			if (cf == null)
				return false;
			if (pointValues.Length != cf.pointValues.Length)
				return false;
			for (int i = 0; i < pointValues.Length; ++i)
				if (pointValues[i] != cf.pointValues[i])
					return false;
			return true;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), pointValues.GetHashCode());
		}
	}
	#endregion
	#region ConditionalFormattingCustomIcon
	public class ConditionalFormattingCustomIcon : IEquatable<ConditionalFormattingCustomIcon> {
		public IconSetType IconSet { get; set; }
		public int IconIndex { get; set; }
		public ConditionalFormattingCustomIcon(IconSetType iconSet, int iconIndex) {
			IconSet = iconSet;
			IconIndex = iconIndex;
		}
		public override bool Equals(object obj) {
			ConditionalFormattingCustomIcon otherCustomIcon = obj as ConditionalFormattingCustomIcon;
			if (otherCustomIcon == null)
				return false;
			return Equals(otherCustomIcon);
		}
		public bool Equals(ConditionalFormattingCustomIcon otherCustomIcon) {
			if (otherCustomIcon == null) {
				return false;
			}
			return (IconSet == otherCustomIcon.IconSet) && (IconIndex == otherCustomIcon.IconIndex);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32((int)IconSet, IconIndex);
		}
	}
	struct ConditionalFormattingCustomIconHolder : IEquatable<ConditionalFormattingCustomIconHolder> {
		public ConditionalFormattingCustomIconHolder(int index, ConditionalFormattingCustomIcon icon) {
			this.Index = index;
			this.CustomIcon = icon;
		}
		public readonly int Index;
		public readonly ConditionalFormattingCustomIcon CustomIcon;
		public override bool Equals(object obj) {
			if (!(obj is ConditionalFormattingCustomIconHolder))
				return false;
			ConditionalFormattingCustomIconHolder other = (ConditionalFormattingCustomIconHolder)obj;
			return this.Equals(other);
		}
		public bool Equals(ConditionalFormattingCustomIconHolder other) {
			return Index == other.Index && CustomIcon.Equals(other.CustomIcon);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(Index, CustomIcon.GetHashCode());
		}
	}
	#endregion
	public static class ConditionalFormattingIconSet {
		const string resourcePath = "DevExpress.XtraSpreadsheet.Images.IconSets.";
		static readonly Image[] arrows3 = CreateArrows3IconSet();
		static readonly Image[] arrows4 = CreateArrows4IconSet();
		static readonly Image[] arrows5 = CreateArrows5IconSet();
		static readonly Image[] arrowsGrey3 = CreateArrowsGrey3IconSet();
		static readonly Image[] arrowsGrey4 = CreateArrowsGrey4IconSet();
		static readonly Image[] arrowsGrey5 = CreateArrowsGrey5IconSet();
		static readonly Image[] flags3 = CreateFlags3IconSet();
		static readonly Image[] boxes5 = CreateBoxes5IconSet();
		static readonly Image[] quarters5 = CreateQuarters5IconSet();
		static readonly Image[] rating4 = CreateRating4IconSet();
		static readonly Image[] rating5 = CreateRating5IconSet();
		static readonly Image[] stars3 = CreateStars3IconSet();
		static readonly Image[] redToBlack4 = CreateRedToBlack4IconSet();
		static readonly Image[] signs3 = CreateSigns3IconSet();
		static readonly Image[] symbols3 = CreateSymbols3IconSet();
		static readonly Image[] symbols23 = CreateSymbols23IconSet();
		static readonly Image[] trafficLights3 = CreateTrafficLights3IconSet();
		static readonly Image[] trafficLights23 = CreateTrafficLights23IconSet();
		static readonly Image[] trafficLights4 = CreateTrafficLights4IconSet();
		static readonly Image[] triangles3 = CreateTriangles3IconSet();
		static readonly Dictionary<IconSetType, Image[]> iconSetTable = CreateIconSetTable();
		static Image[] CreateArrows3IconSet() {
			Image[] result = new Image[] {
				GetBitmap("Arrows3_1"),
				GetBitmap("Arrows3_2"),
				GetBitmap("Arrows3_3"),
			};
			return result;
		}
		static Image[] CreateArrows4IconSet() {
			Image[] result = new Image[] {
				GetBitmap("Arrows4_1"),
				GetBitmap("Arrows4_2"),
				GetBitmap("Arrows4_3"),
				GetBitmap("Arrows4_4")
			};
			return result;
		}
		static Image[] CreateArrows5IconSet() {
			Image[] result = new Image[] {
				GetBitmap("Arrows5_1"),
				GetBitmap("Arrows5_2"),
				GetBitmap("Arrows5_3"),
				GetBitmap("Arrows5_4"),
				GetBitmap("Arrows5_5")
			};
			return result;
		}
		static Image[] CreateArrowsGrey3IconSet() {
			Image[] result = new Image[] {
				GetBitmap("ArrowsGrey3_1"),
				GetBitmap("ArrowsGrey3_2"),
				GetBitmap("ArrowsGrey3_3")
			};
			return result;
		}
		static Image[] CreateArrowsGrey4IconSet() {
			Image[] result = new Image[] {
				GetBitmap("ArrowsGrey4_1"),
				GetBitmap("ArrowsGrey4_2"),
				GetBitmap("ArrowsGrey4_3"),
				GetBitmap("ArrowsGrey4_4")
			};
			return result;
		}
		static Image[] CreateArrowsGrey5IconSet() {
			Image[] result = new Image[] {
				GetBitmap("ArrowsGrey5_1"),
				GetBitmap("ArrowsGrey5_2"),
				GetBitmap("ArrowsGrey5_3"),
				GetBitmap("ArrowsGrey5_4"),
				GetBitmap("ArrowsGrey5_5")
			};
			return result;
		}
		static Image[] CreateFlags3IconSet() {
			Image[] result = new Image[] {
				GetBitmap("Flags3_1"),
				GetBitmap("Flags3_2"),
				GetBitmap("Flags3_3")
			};
			return result;
		}
		static Image[] CreateBoxes5IconSet() {
			Image[] result = new Image[] {
				GetBitmap("Boxes5_1"),
				GetBitmap("Boxes5_2"),
				GetBitmap("Boxes5_3"),
				GetBitmap("Boxes5_4"),
				GetBitmap("Boxes5_5")
			};
			return result;
		}
		static Image[] CreateQuarters5IconSet() {
			Image[] result = new Image[] {
				GetBitmap("Quarters5_1"),
				GetBitmap("Quarters5_2"),
				GetBitmap("Quarters5_3"),
				GetBitmap("Quarters5_4"),
				GetBitmap("Quarters5_5")
			};
			return result;
		}
		static Image[] CreateRating4IconSet() {
			Image[] result = new Image[] {
				GetBitmap("Rating4_1"),
				GetBitmap("Rating4_2"),
				GetBitmap("Rating4_3"),
				GetBitmap("Rating4_4")
			};
			return result;
		}
		static Image[] CreateRating5IconSet() {
			Image[] result = new Image[] {
				GetBitmap("Rating5_1"),
				GetBitmap("Rating5_2"),
				GetBitmap("Rating5_3"),
				GetBitmap("Rating5_4"),
				GetBitmap("Rating5_5")
			};
		   return result;
		}
		static Image[] CreateStars3IconSet() {
			Image[] result = new Image[] {
				GetBitmap("Stars3_1"),
				GetBitmap("Stars3_2"),
				GetBitmap("Stars3_3")
			};
			return result;
		}
		static Image[] CreateRedToBlack4IconSet() {
			Image[] result = new Image[] {
				GetBitmap("RedToBlack4_1"),
				GetBitmap("RedToBlack4_2"),
				GetBitmap("RedToBlack4_3"),
				GetBitmap("RedToBlack4_4")
			};
			return result;
		}
		static Image[] CreateSigns3IconSet() {
			Image[] result = new Image[] {
				GetBitmap("Signs3_1"),
				GetBitmap("Signs3_2"),
				GetBitmap("Signs3_3")
			};
			return result;
		}
		static Image[] CreateSymbols3IconSet() {
			Image[] result = new Image[] {
				GetBitmap("Symbols3_1"),
				GetBitmap("Symbols3_2"),
				GetBitmap("Symbols3_3"),
			};
			return result;
		}
		static Image[] CreateSymbols23IconSet() {
			Image[] result = new Image[] {
				GetBitmap("Symbols23_1"),
				GetBitmap("Symbols23_2"),
				GetBitmap("Symbols23_3")
			};
			return result;
		}
		static Image[] CreateTrafficLights3IconSet() {
			Image[] result = new Image[] {
				GetBitmap("TrafficLights3_1"),
				GetBitmap("TrafficLights3_2"),
				GetBitmap("TrafficLights3_3")
			};
			return result;
		}
		static Image[] CreateTrafficLights23IconSet() {
			Image[] result = new Image[] {
				GetBitmap("TrafficLights23_1"),
				GetBitmap("TrafficLights23_2"),
				GetBitmap("TrafficLights23_3")
			};
			return result;
		}
		static Image[] CreateTrafficLights4IconSet() {
			Image[] result = new Image[] {
				GetBitmap("TrafficLights4_1"),
				GetBitmap("TrafficLights4_2"),
				GetBitmap("TrafficLights4_3"),
				GetBitmap("TrafficLights4_4")
			};
			return result;
		}
		static Image[] CreateTriangles3IconSet() {
			Image[] result = new Image[] {
				GetBitmap("Triangles3_1"),
				GetBitmap("Triangles3_2"),
				GetBitmap("Triangles3_3")
			};
			return result;
		}
		static Dictionary<IconSetType, Image[]> CreateIconSetTable() {
			Dictionary<IconSetType, Image[]> result = new Dictionary<IconSetType, Image[]>();
			result.Add(IconSetType.Arrows3, arrows3);
			result.Add(IconSetType.Arrows4, arrows4);
			result.Add(IconSetType.Arrows5, arrows5);
			result.Add(IconSetType.ArrowsGray3, arrowsGrey3);
			result.Add(IconSetType.ArrowsGray4, arrowsGrey4);
			result.Add(IconSetType.ArrowsGray5, arrowsGrey5);
			result.Add(IconSetType.Flags3, flags3);
			result.Add(IconSetType.Boxes5, boxes5);
			result.Add(IconSetType.Quarters5, quarters5);
			result.Add(IconSetType.Rating4, rating4);
			result.Add(IconSetType.Rating5, rating5);
			result.Add(IconSetType.Stars3, stars3);
			result.Add(IconSetType.RedToBlack4, redToBlack4);
			result.Add(IconSetType.Signs3, signs3);
			result.Add(IconSetType.Symbols3, symbols3);
			result.Add(IconSetType.Symbols23, symbols23);
			result.Add(IconSetType.TrafficLights13, trafficLights3);
			result.Add(IconSetType.TrafficLights23, trafficLights23);
			result.Add(IconSetType.TrafficLights4, trafficLights4);
			result.Add(IconSetType.Triangles3, triangles3);
			return result;
		}
		static Image GetBitmap(string name) {
			return CreateBitmapFromResources(resourcePath + name + ".png");
		}
#if (!SL)
		public static Bitmap CreateBitmapFromResources(string name) {
#if DXPORTABLE
			return CommandResourceImageLoader.CreateBitmapFromResources(name, typeof(ConditionalFormattingIconSet).GetTypeInfo().Assembly);
#else
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
			return (Bitmap)ImageTool.ImageFromStream(stream);
#endif
		}
#else
		public static Image CreateBitmapFromResources(string name) {
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
			if(stream == null) return null;
			Image image = new Image();
			BitmapImage b = new BitmapImage();
			b.SetSource(stream);
			image.Source = b;
			return image;
		}
#endif
		public static Image GetImage(IconSetType iconSetType, int index) {
			Image[] iconSet;
			if (!iconSetTable.TryGetValue(iconSetType, out iconSet))
				return null;
			if (index < 0 || index >= iconSet.Length)
				return null;
			return iconSet[iconSet.Length - 1 - index];
		}
	}
	public abstract class DifferentialFormatConditionalFormatVisitor<T> : IConditionalFormattingVisitor {
		readonly ICell cell;
		protected DifferentialFormatConditionalFormatVisitor(ICell cell) {
			this.cell = cell;
		}
		public ICell Cell { get { return cell; } }
		public T Value { get; set; }
		public virtual bool Visit(FormulaConditionalFormatting formatting) {
			if (!formatting.Evaluate(cell))
				return false;
			DifferentialFormat format = formatting.DifferentialFormatInfo;
			if (ShouldApplyValue(format))
				Value = GetValue(format);
			return true;
		}
		protected abstract bool ShouldApplyValue(DifferentialFormat format);
		protected abstract T GetValue(DifferentialFormat format);
		public virtual bool Visit(ColorScaleConditionalFormatting formatting) {
			return true;
		}
		public virtual bool Visit(DataBarConditionalFormatting formatting) {
			return true;
		}
		public virtual bool Visit(IconSetConditionalFormatting formatting) {
			return true;
		}
	}
	public class FontColorConditionalFormatVisitor : DifferentialFormatConditionalFormatVisitor<Color> {
		public FontColorConditionalFormatVisitor(ICell cell)
			: base(cell) {
		}
		protected override bool ShouldApplyValue(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontColor;
		}
		protected override Color GetValue(DifferentialFormat format) {
			return format.Font.Color;
		}
	}
	#region ConditionalFormattingFormatAccumulator
	public class ConditionalFormattingFormatAccumulator {
		#region Fields
		[Flags]
		enum BinaryValue {
			None = 0,
			StrikeThroughValue = 1,
			StrikeThroughModified = 2,
			UnderlineModified = 4,
			ColorModified = 8,
			BoldValue = 16,
			ItalicValue = 32,
			StyleModified = 64,
			ColorScaleFillModified = 128
		}
		const BinaryValue fontModifiedMask = BinaryValue.StrikeThroughModified | BinaryValue.StyleModified | BinaryValue.UnderlineModified;
		const BinaryValue anythingModified = BinaryValue.ColorScaleFillModified | BinaryValue.StrikeThroughModified | BinaryValue.UnderlineModified | BinaryValue.StyleModified | BinaryValue.ColorModified;
		int borderIndex = -1; 
		int patternFillIndex = -1;
		int gradientFillIndex = -1;
		int numberFormatIndex = -1;
		BinaryValue packedValues;
		XlUnderlineType underline;
		Color color;
		Color colorScaleFill;
		int versionStamp = -1;
		int stoppedAtPriority = Int32.MaxValue;
		#endregion
		#region Properties
		public bool FontModified { get { return (packedValues & fontModifiedMask) != 0; } }
		public int VersionStamp { get { return versionStamp; } set { versionStamp = value; } }
		public bool BorderAssigned { get { return borderIndex >= 0; } }
		public int BorderIndex { get { return borderIndex; } set { borderIndex = value; } }
		public bool FillAssigned { get { return PatternFillAssigned || GradientFillAssigned; } }
		public ModelFillType FillType { get { return PatternFillAssigned ? ModelFillType.Pattern : ModelFillType.Gradient; } }
		public bool PatternFillAssigned { get { return patternFillIndex >= 0; } }
		public int PatternFillIndex { get { return patternFillIndex; } set { patternFillIndex = value; } }
		public bool GradientFillAssigned { get { return gradientFillIndex >= 0; } }
		public int GradientFillIndex { get { return gradientFillIndex; } set { gradientFillIndex = value; } }
		public bool StrikeThroughModified { get { return (packedValues & BinaryValue.StrikeThroughModified) != 0; } }
		public bool StrikeThrough {
			get { return (packedValues & BinaryValue.StrikeThroughValue) != 0; }
			set {
				packedValues |= BinaryValue.StrikeThroughModified;
				ApplyBooleanValue(BinaryValue.StrikeThroughValue, value);
			}
		}
		public bool ColorModified { get { return (packedValues & BinaryValue.ColorModified) != 0; } }
		public Color Color {
			get { return color; }
			set {
				packedValues |= BinaryValue.ColorModified;
				color = value;
			}
		}
		public bool ColorScaleFillModified { get { return (packedValues & BinaryValue.ColorScaleFillModified) != 0; } }
		public Color ColorScaleFill {
			get { return colorScaleFill; }
			set {
				packedValues |= BinaryValue.ColorScaleFillModified;
				colorScaleFill = value;
			}
		}
		public bool BoldAndItalicModified { get { return (packedValues & BinaryValue.StyleModified) != 0; } }
		public bool Bold {
			get { return (packedValues & BinaryValue.BoldValue) != 0; }
			set {
				packedValues |= BinaryValue.StyleModified;
				ApplyBooleanValue(BinaryValue.BoldValue, value);
			}
		}
		public bool Italic {
			get { return (packedValues & BinaryValue.ItalicValue) != 0; }
			set {
				packedValues |= BinaryValue.StyleModified;
				ApplyBooleanValue(BinaryValue.ItalicValue, value);
			}
		}
		public bool UnderlineModified { get { return (packedValues & BinaryValue.UnderlineModified) != 0; } }
		public XlUnderlineType Underline {
			get { return underline; }
			set {
				packedValues |= BinaryValue.UnderlineModified;
				underline = value;
			}
		}
		public bool NumberFormatModified { get { return numberFormatIndex >= 0; } }
		public bool Modified { get { return ((packedValues & anythingModified) != 0) || BorderAssigned || FillAssigned || NumberFormatModified; } }
		public int NumberFormatIndex { get { return numberFormatIndex; } set { numberFormatIndex = value; } }
		public int StoppedAtPriority { get { return stoppedAtPriority; } set { stoppedAtPriority = value; } }
		#endregion
		public void Reset(int versionStamp) {
			packedValues &= ~anythingModified;
			patternFillIndex = -1;
			borderIndex = -1;
			numberFormatIndex = -1;
			gradientFillIndex = -1;
			stoppedAtPriority = Int32.MaxValue;
			this.versionStamp = versionStamp;
		}
		#region Helpers
		void ApplyBooleanValue(BinaryValue mask, bool value) {
			if (value)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		#endregion
		public void Update(ICell cell) {
			int versionStamp = cell.DocumentModel.ContentVersion;
			if (versionStamp != this.versionStamp) {
				Reset(versionStamp);
				ConditionalFormatUpdateVisitor visitor = new ConditionalFormatUpdateVisitor(cell, this);
				IList<ConditionalFormatting> conditionalFormattings = cell.Worksheet.ConditionalFormattings.GetBackColorConditionalFormatting(cell);
				ConditionalFormattingCollection.Visit(conditionalFormattings, visitor);
			}
		}
		public bool HasVisibleFill(DocumentModel documentModel) {
			if (ColorScaleFillModified || GradientFillAssigned)
				return true;
			if (PatternFillAssigned)
				return documentModel.Cache.FillInfoCache[PatternFillIndex].HasVisible(documentModel, true);
			return false;
		}
	}
	public class ConditionalFormatUpdateVisitor : IConditionalFormattingVisitor {
		readonly ICell cell;
		readonly ConditionalFormattingFormatAccumulator accumulator;
		const int unassignedBorderIndex = BorderInfoCache.DefaultItemIndex; 
		const int unassignedFillIndex = FillInfoCache.DefaultItemIndex; 
		const int unassignedGradientFillIndex = GradientFillInfoCache.DefaultItemIndex; 
		const int unassignedFontIndex = RunFontInfoCache.DefaultItemIndex; 
		const int unassignedFontColorIndex = ColorModelInfoCache.DefaultItemIndex;
		const int borderMask = BorderOptionsInfo.MaskApplyBottomLineStyle | BorderOptionsInfo.MaskApplyHorizontalLineStyle | BorderOptionsInfo.MaskApplyLeftLineStyle | BorderOptionsInfo.MaskApplyRightLineStyle | BorderOptionsInfo.MaskApplyTopLineStyle | BorderOptionsInfo.MaskApplyVerticalLineStyle;
		const int fontMask = MultiOptionsInfo.MaskApplyFontBold | MultiOptionsInfo.MaskApplyFontColor | MultiOptionsInfo.MaskApplyFontItalic | MultiOptionsInfo.MaskApplyFontStrikeThrough | MultiOptionsInfo.MaskApplyFontUnderline;
		public ConditionalFormatUpdateVisitor(ICell cell, ConditionalFormattingFormatAccumulator accumulator) {
			this.cell = cell;
			this.accumulator = accumulator;
		}
		#region Helper functions
		void ProcessBorders(DifferentialFormat diffFmtInfo, ConditionalFormattingFormatAccumulator accumulator) {
			if (((diffFmtInfo.BorderOptionsInfo.PackedValues & borderMask) != 0) && !accumulator.BorderAssigned) {
				int borderIndex = diffFmtInfo.BorderIndex;
				accumulator.BorderIndex = borderIndex;
			}
		}
		void ProcessFill(DifferentialFormat diffFmtInfo, ConditionalFormattingFormatAccumulator accumulator) {
			MultiOptionsInfo multiFlags = diffFmtInfo.MultiOptionsInfo;
			if (!accumulator.FillAssigned) {
				if (diffFmtInfo.Fill.FillType == ModelFillType.Gradient) {
					accumulator.PatternFillIndex = -1;
					accumulator.GradientFillIndex = diffFmtInfo.GradientFillInfoIndex;
				}
				else {
					const int patternFillMask = MultiOptionsInfo.MaskApplyFillBackColor | MultiOptionsInfo.MaskApplyFillForeColor | MultiOptionsInfo.MaskApplyFillPatternType;
					if ((multiFlags.PackedValues & patternFillMask) != 0) {
						accumulator.PatternFillIndex = diffFmtInfo.FillIndex;
						accumulator.GradientFillIndex = -1;
					}
				}
			}
		}
		void ProcessFont(DifferentialFormat diffFmtInfo, ConditionalFormattingFormatAccumulator accumulator) {
			MultiOptionsInfo multiFlags = diffFmtInfo.MultiOptionsInfo;
			if ((multiFlags.PackedValues & fontMask) != 0) {
				RunFontInfo fontInfo = diffFmtInfo.FontInfo;
				if (!accumulator.ColorModified) {
					if (fontInfo.ColorIndex != unassignedFontColorIndex) {
						DocumentModel documentModel = cell.Context.Workbook;
						accumulator.Color = fontInfo.GetColorModelInfo(documentModel).ToRgb(documentModel.StyleSheet.Palette, documentModel.OfficeTheme.Colors);
					}
				}
				if (!accumulator.StrikeThroughModified && multiFlags.ApplyFontStrikeThrough)
					accumulator.StrikeThrough = fontInfo.StrikeThrough;
				if (!accumulator.BoldAndItalicModified) {
					if (multiFlags.ApplyFontBold)
						accumulator.Bold = fontInfo.Bold;
					if (multiFlags.ApplyFontItalic)
						accumulator.Italic = fontInfo.Italic;
				}
				if (!accumulator.UnderlineModified && multiFlags.ApplyFontUnderline)
					accumulator.Underline = fontInfo.Underline;
			}
		}
		void ProcessNumberFormat(DifferentialFormat diffFmtInfo, ConditionalFormattingFormatAccumulator accumulator) {
			MultiOptionsInfo multiFlags = diffFmtInfo.MultiOptionsInfo;
			if (multiFlags.ApplyNumberFormat && !accumulator.NumberFormatModified)
				accumulator.NumberFormatIndex = diffFmtInfo.NumberFormatIndex;
		}
		#endregion
		#region IConditionalFormattingVisitor Members
		public bool Visit(FormulaConditionalFormatting formatting) {
			if (formatting.Evaluate(cell)) {
				DifferentialFormat diffFmtInfo = formatting.DifferentialFormatInfo;
				ProcessBorders(diffFmtInfo, accumulator);
				ProcessFill(diffFmtInfo, accumulator);
				ProcessFont(diffFmtInfo, accumulator);
				ProcessNumberFormat(diffFmtInfo, accumulator);
				if (formatting.StopIfTrue) {
					accumulator.StoppedAtPriority = formatting.Priority;
					return true;
				}
			}
			return false;
		}
		public bool Visit(ColorScaleConditionalFormatting formatting) {
			if (!accumulator.ColorScaleFillModified) {
				Color value = formatting.EvaluateColor(cell);
				if (!DXColor.IsTransparentOrEmpty(value))
					accumulator.ColorScaleFill = value;
			}
			return false;
		}
		public bool Visit(DataBarConditionalFormatting formatting) {
			return false; 
		}
		public bool Visit(IconSetConditionalFormatting formatting) {
			return false; 
		}
		#endregion
	}
	#endregion
	#region DefaultConditionalFormattingStyleCollection
	public static class DefaultConditionalFormattingStyleCollection {
		static List<ConditionalFormattingStyle> styles = PopulateStyles();
		const XlBorderLineStyle DefaultBorderLineStyle = XlBorderLineStyle.None;
		public static List<ConditionalFormattingStyle> Styles { get { return styles; } }
		static List<ConditionalFormattingStyle> PopulateStyles() {
			List<ConditionalFormattingStyle> result = new List<ConditionalFormattingStyle>();
			ConditionalFormattingStyle style1 = GetStyle(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.ConditionalFormattingStyle_LightRedFillWithDarkRedText), DXColor.FromArgb(0xFF, 0x9C, 0x00, 0x06), DXColor.FromArgb(0xFF, 0xFF, 0xC7, 0xCE), DXColor.Empty, DefaultBorderLineStyle);
			ConditionalFormattingStyle style2 = GetStyle(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.ConditionalFormattingStyle_YellowFillWithDarkYellowText), DXColor.FromArgb(0xFF, 0x9C, 0x65, 0x00), DXColor.FromArgb(0xFF, 0xFF, 0xEB, 0x9C), DXColor.Empty, DefaultBorderLineStyle);
			ConditionalFormattingStyle style3 = GetStyle(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.ConditionalFormattingStyle_GreenFillWithDarkGreenText), DXColor.FromArgb(0xFF, 0x00, 0x61, 0x00), DXColor.FromArgb(0xFF, 0xC6, 0xEF, 0xCE), DXColor.Empty, DefaultBorderLineStyle);
			ConditionalFormattingStyle style4 = GetStyle(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.ConditionalFormattingStyle_LightRedFill), DXColor.Empty, DXColor.FromArgb(0xFF, 0xFF, 0xC7, 0xCE), DXColor.Empty, DefaultBorderLineStyle);
			ConditionalFormattingStyle style5 = GetStyle(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.ConditionalFormattingStyle_RedText), DXColor.FromArgb(0xFF, 0x9C, 0x00, 0x06), DXColor.Empty, DXColor.Empty, DefaultBorderLineStyle);
			ConditionalFormattingStyle style6 = GetStyle(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.ConditionalFormattingStyle_RedBorder), DXColor.Empty, DXColor.Empty, DXColor.FromArgb(0xFF, 0x9C, 0x00, 0x06), XlBorderLineStyle.Thin);
			result.Add(style1);
			result.Add(style2);
			result.Add(style3);
			result.Add(style4);
			result.Add(style5);
			result.Add(style6);
			return result;
		}
		static ConditionalFormattingStyle GetStyle(string displayName, Color fontColor, Color fillColor, Color borderColor, XlBorderLineStyle borderLineStyle) {
			ConditionalFormattingStyle style = new ConditionalFormattingStyle();
			style.DisplayName = displayName;
			style.FontColor = fontColor;
			style.FillBackColor = fillColor;
			style.FillPatternType = XlPatternType.Solid;
			style.BorderColor = borderColor;
			style.BorderLineStyle = borderLineStyle;
			return style;
		}
	}
	#endregion
	#region ConditionalFormattingAccumulatorCollection
	internal class ConditionalFormattingFormatCollection {
		Dictionary<int, ConditionalFormattingFormatAccumulator> storage;
		public ConditionalFormattingFormatCollection() {
			storage = new Dictionary<int, ConditionalFormattingFormatAccumulator>();
		}
		int CalculateHash(ICell cell) {
			return cell != null ? ((cell.RowIndex << 14) | (cell.ColumnIndex & 0xff)) : 0; 
		}
		public ConditionalFormattingFormatAccumulator this[ICell cell] {
			get {
				ConditionalFormattingFormatAccumulator result;
				int hashValue = CalculateHash(cell);
				if (!storage.TryGetValue(hashValue, out result)) {
					result = new ConditionalFormattingFormatAccumulator();
					storage.Add(hashValue, result);
				}
				return result;
			}
		}
	}
	#endregion
}
