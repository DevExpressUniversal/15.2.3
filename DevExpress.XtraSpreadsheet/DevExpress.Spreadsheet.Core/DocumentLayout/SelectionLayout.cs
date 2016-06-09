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

using System.Drawing;
using System.IO;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Layout;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region SelectionLayout
	public class SelectionLayout : DocumentItemLayout {
		#region Fields
		PageSelectionLayoutItem pageSelectionItem;
		CutCopyRangeDashBorderLayoutItem cutCopyRangeItem;
		PictureSelectionLayoutItemCollection pictureSelectionLayoutItems;
		PageDashSelectionLayoutItem pageDashSelectionLayoutItem;
		PrintRangeSelectionLayoutItem printRangeSelectionLayoutItem;
		RangeMailMergeLayoutItem mailMergeHeaderItem;
		RangeMailMergeLayoutItem mailMergeFooterItem;
		RangeMailMergeLayoutItem mailMergeDetailItem;
		readonly List<RangeMailMergeDetailLevelLayoutItem> mailMergeDetailLevelList;
		readonly List<RangeMailMergeGroupHeaderLayoutItem> mailMergeGroupHeadersList;
		readonly List<RangeMailMergeLayoutItem> mailMergeGroupFootersList;
		bool isMultiSelection;
		#endregion
		public SelectionLayout(SpreadsheetView view)
			: base(view) {
			mailMergeDetailLevelList = new List<RangeMailMergeDetailLevelLayoutItem>();
			mailMergeGroupHeadersList = new List<RangeMailMergeGroupHeaderLayoutItem>();
			mailMergeGroupFootersList = new List<RangeMailMergeLayoutItem>();
		}
		#region Properties
		public bool IsMultiSelection { get { return isMultiSelection; } }
		protected internal PageSelectionLayoutItem PageSelectionItem { get { return pageSelectionItem; } }
		protected PrintRangeSelectionLayoutItem PrintRangeSelectionLayoutItem { get { return printRangeSelectionLayoutItem; } }
		public RangeMailMergeLayoutItem MailMergeHeaderItem { get { return mailMergeHeaderItem; } }
		public RangeMailMergeLayoutItem MailMergeFooterItem { get { return mailMergeFooterItem; } }
		public RangeMailMergeLayoutItem MailMergeDetailItem { get { return mailMergeDetailItem; } }
		public List<RangeMailMergeDetailLevelLayoutItem> MailMergeDetailLevelList { get { return mailMergeDetailLevelList; } }
		public List<RangeMailMergeGroupHeaderLayoutItem> MailMergeGroupHeadersList { get { return mailMergeGroupHeadersList; } }
		public List<RangeMailMergeLayoutItem> MailMergeGroupFootersList { get { return mailMergeGroupFootersList; } }
		#endregion
		public override void Update(Page page) {
			Update();
		}
		public virtual void Update() {
			UpdatePrintRangeItem();
			UpdateRangeSelection();
			UpdateCutCopyRangeLayoutItem();
			UpdatePictureSelection();
			UpdateDashRangeSelection();
			UpdateMailMergeRangeItems();
		}
		void UpdateMailMergeRangeItems() {
			if (!DocumentModel.ShowMailMergeRanges)
				return;
			mailMergeHeaderItem = null;
			mailMergeFooterItem = null;
			mailMergeDetailItem = null;
			MailMergeOptions options = new MailMergeOptions(DocumentModel);
			if (options.IsEmpty)
				return;
			if (options.HeaderRange != null) {
				mailMergeHeaderItem = new RangeMailMergeLayoutItem(this, options.HeaderRange, RangeMailMergeLayoutItem.HeaderText, MailMergeDefinedNames.HeaderRange);
			}
			if (options.FooterRange != null) {
				mailMergeFooterItem = new RangeMailMergeLayoutItem(this, options.FooterRange, RangeMailMergeLayoutItem.FooterText, MailMergeDefinedNames.FooterRange);
			}
			if (options.DetailRange != null) {
				mailMergeDetailItem = new RangeMailMergeLayoutItem(this, options.DetailRange, RangeMailMergeLayoutItem.DetailText, MailMergeDefinedNames.DetailRange);
			}
			mailMergeDetailLevelList.Clear();
			for (int i = 0; i < options.DetailLevels.Count; i++) {
				mailMergeDetailLevelList.Add(new RangeMailMergeDetailLevelLayoutItem(this, options.DetailLevels[i],
																		  RangeMailMergeLayoutItem.DetailLevelText + i.ToString(),
																		  MailMergeDefinedNames.DetailLevel + i.ToString()));
			}
			mailMergeGroupHeadersList.Clear();
			mailMergeGroupFootersList.Clear();
			List<GroupInfo> groupInfo = options.GetGroupInfoList();
			for (int i = 0; i < groupInfo.Count; i++) {
				GroupInfo current = groupInfo[i];
				string groupIndex = current.DefinedName.Substring(MailMergeDefinedNames.GroupName.Length);
				if (current.Header != null)
					mailMergeGroupHeadersList.Add(new RangeMailMergeGroupHeaderLayoutItem(this, current.Header, RangeMailMergeLayoutItem.GroupHeaderText + groupIndex, MailMergeDefinedNames.GroupHeader + groupIndex));
				if (current.Footer != null)
					mailMergeGroupFootersList.Add(new RangeMailMergeLayoutItem(this, current.Footer, RangeMailMergeLayoutItem.GroupFooterText + groupIndex, MailMergeDefinedNames.GroupFooter + groupIndex));
			}
		}
		void UpdateDashRangeSelection() {
			SheetViewSelection selection = DocumentModel.ActiveSheet.ReferenceEditSelection;
			IList<CellRange> selectedParameterRanges = selection.SelectedRanges;
			IList<CellRange> notNestedParameterRanges = GetNotNestedRanges(selectedParameterRanges);
			CellPosition activeParameterPosition = selection.ActiveCell;
			CellRange mergedParameterCell = selection.GetActualCellRange(activeParameterPosition);
			this.pageDashSelectionLayoutItem = new PageDashSelectionLayoutItem(this, mergedParameterCell.TopLeft, mergedParameterCell.BottomRight);
			for (int i = 0; i < notNestedParameterRanges.Count; i++) {
				RangeBorderDashSelectionLayoutItem item = GetPageDashSelectionBorder(notNestedParameterRanges[i]);
				this.pageDashSelectionLayoutItem.BorderItems.Add(item);
			}
		}
		void UpdateRangeSelection() {
			if (DocumentModel.BehaviorOptions.Selection.HideSelection) {
				this.pageSelectionItem = null;
				return;
			}
			SheetViewSelection selection = DocumentModel.ActiveSheet.Selection;
			IList<CellRange> selectedRanges = GetSelectedRanges(selection);
			IList<CellRange> notIdenticalRanges = ExcludeIdenticalRanges(selectedRanges);
			IList<CellRange> notNestedRanges = GetNotNestedRanges(notIdenticalRanges);
			this.isMultiSelection = selectedRanges.Count > 1;
			CellPosition activePosition = selection.ActiveCell;
			CellRange mergedCell = selection.GetActualCellRange(activePosition);
			this.pageSelectionItem = new PageSelectionLayoutItem(this, mergedCell.TopLeft, mergedCell.BottomRight);
			for (int i = 0; i < notNestedRanges.Count; i++) {
				CellRange range = notNestedRanges[i];
				RangeSelectionLayoutItem item = CreateSelectedLayoutItem(notNestedRanges, range, i);
				pageSelectionItem.InnerItems.Add(item);
			}
			pageSelectionItem.BorderItem = IsMultiSelection ? CreatePageSelectionBorder(mergedCell) : CreatePageSelectionBorder(notNestedRanges[0]);
		}
		IList<CellRange> GetSelectedRanges(SheetViewSelection selection) {
			PivotSelection pivotSelection = DocumentModel.ActiveSheet.PivotSelection;
			if (!pivotSelection.HasInitSelection)
				return selection.SelectedRanges;
			IList<CellRange> selectedRanges = new List<CellRange>();
			CellRangeBase ranges = pivotSelection.GetCurrentSelection();
			UpdatePivotSelection(selection, ranges);
			foreach (CellRange range in ranges.GetAreasEnumerable())
				selectedRanges.Add(range);
			return selectedRanges;
		}
		void UpdatePivotSelection(SheetViewSelection selection, CellRangeBase ranges) {
			if (!selection.ActiveCell.EqualsPosition(ranges.TopLeft))
				selection.SetSelection(ranges, true);
		}
		void UpdateCutCopyRangeLayoutItem() {
			if (!DocumentModel.IsCopyCutMode) {
				this.cutCopyRangeItem = null;
				return;
			}
			CellRange range = DocumentModel.CopiedRangeProvider.Range;
			if (DocumentModel.ActiveSheet.SheetId != range.SheetId)
				this.cutCopyRangeItem = null;
			else
				this.cutCopyRangeItem = new CutCopyRangeDashBorderLayoutItem(this, range.TopLeft, range.BottomRight, range.Worksheet.SheetId);
		}
		void UpdatePrintRangeItem() {
			if (!DocumentModel.ViewOptions.ShowPrintArea) {
				printRangeSelectionLayoutItem = null;
				return;
			}
			PrintAreaCalculator calculator = new PrintAreaCalculator(DocumentModel.ActiveSheet);
			CellRangeBase printRange = calculator.GetDefinedNameRange();
			if (printRange != null)
				printRangeSelectionLayoutItem = new PrintRangeSelectionLayoutItem(this, printRange);
			else
				printRangeSelectionLayoutItem = null;
		}
		RangeBorderSelectionLayoutItem CreatePageSelectionBorder(CellRange range) {
			return new RangeBorderSelectionLayoutItem(this, range.TopLeft, range.BottomRight);
		}
		RangeBorderDashSelectionLayoutItem GetPageDashSelectionBorder(CellRange range) {
			return new RangeBorderDashSelectionLayoutItem(this, range.TopLeft, range.BottomRight);
		}
		void UpdatePictureSelection() {
			SheetViewSelection selection = DocumentModel.ActiveSheet.Selection;
			this.pictureSelectionLayoutItems = new PictureSelectionLayoutItemCollection();
			List<int> pictureIndexes = selection.SelectedDrawingIndexes;
			int pictureCount = pictureIndexes.Count;
			for (int i = 0; i < pictureCount; i++) {
				PictureSelectionLayoutItem item = new PictureSelectionLayoutItem(this, pictureIndexes[i]);
				pictureSelectionLayoutItems.Add(item);
			}
		}
		protected internal RangeSelectionLayoutItem CreateSelectedLayoutItem(IList<CellRange> ranges, CellRange range, int startIndex) {
			RangeSelectionLayoutItem item = new RangeSelectionLayoutItem(this, range.TopLeft, range.BottomRight);
			int count = ranges.Count;
			for (int i = startIndex; i < count; i++) {
				CellRange currentRange = ranges[i];
				if (currentRange == range)
					continue;
				VariantValue intersectionValue = range.IntersectionWith(currentRange);
				if (intersectionValue != VariantValue.ErrorNullIntersection)
					item.ClipRanges.Add(intersectionValue.CellRangeValue);
			}
			return item;
		}
		IList<CellRange> ExcludeIdenticalRanges(IList<CellRange> ranges) {
			IList<CellRange> result = new List<CellRange>(ranges);
			for (int i = result.Count - 1; i >= 0; i--) {
				CellRange currentRange = result[i];
				for (int j = i - 1; j >= 0; j--) {
					if (result[j].Equals(currentRange)) {
						result.Remove(currentRange);
						break;
					}
				}
			}
			return result;
		}
		protected internal IList<CellRange> GetNotNestedRanges(IList<CellRange> ranges) {
			IList<CellRange> result = new List<CellRange>();
			int count = ranges.Count;
			for (int i = 0; i < count; i++) {
				CellRange currentRange = ranges[i];
				if (!IsNestedRange(ranges, currentRange))
					result.Add(currentRange);
			}
			if (result.Count == 0 && count > 0)
				result.Add(ranges[0]);
			return result;
		}
		protected internal virtual bool IsNestedRange(IList<CellRange> ranges, CellRange range) {
			int count = ranges.Count;
			for (int i = 0; i < count; i++) {
				CellRange currentRange = ranges[i];
				if (currentRange != range && currentRange.Includes(range))
					return true;
			}
			return false;
		}
		public override void Invalidate() {
			if (pictureSelectionLayoutItems != null) {
				int count = pictureSelectionLayoutItems.Count;
				for (int i = 0; i < count; i++) {
					pictureSelectionLayoutItems[i] = null;
				}
				pictureSelectionLayoutItems = null;
			}
			if (pageSelectionItem != null) {
				pageSelectionItem.Invalidate();
				pageSelectionItem = null;
			}
			if (pageDashSelectionLayoutItem != null) {
				pageDashSelectionLayoutItem.Invalidate();
				pageDashSelectionLayoutItem = null;
			}
			if (cutCopyRangeItem != null) {
				cutCopyRangeItem.Invalidate();
				cutCopyRangeItem = null;
			}
			this.isMultiSelection = false;
		}
		public CutCopyRangeDashBorderLayoutItem GetCutCopyRange(Page page) {
			return cutCopyRangeItem;
		}
		public PageSelectionLayoutItem GetPageSelection(Page page) {
			if (DocumentModel.ActiveSheet.Selection.IsCommentSelected)
				return null;
			if (pictureSelectionLayoutItems == null || pictureSelectionLayoutItems.Count == 0)
				return pageSelectionItem;
			return null;
		}
		public PageDashSelectionLayoutItem GetPageDashSelection(Page page) {
			return pageDashSelectionLayoutItem;
		}
		public PictureSelectionLayoutItemCollection GetPictureSelection(Page page) {
			return pictureSelectionLayoutItems;
		}
		public PrintRangeSelectionLayoutItem GetPrintRangeSelection() {
			return printRangeSelectionLayoutItem;
		}
		protected internal Rectangle CalculateBounds(Page page, CellPosition topLeft, CellPosition bottomRight) {
			return page.CalculateRangeBounds(topLeft, bottomRight);
		}
		public virtual HotZone CalculateHotZone(Point point) {
			Page page = View.GetPageByPoint(point);
			return CalculateHotZone(point, page);
		}
		protected internal override HotZone CalculateHotZone(Point point, Page page) {
			PageSelectionLayoutItem selection = GetPageSelection(page);
			if (selection != null && page != null && !object.ReferenceEquals(selection.LastPage, page))
				selection.Update(page);
			HotZone result = CalculatePictureHotZone(point);
			if (result != null)
				return result;
			result = CalculateMailMergeHotZone(point);
			if (result != null)
				return result;
			return CalculateRangeDragHotZone(point);
		}
		HotZone CalculatePictureHotZone(Point point) {
			if (pictureSelectionLayoutItems == null)
				return null;
			float zoomFactor = View.ZoomFactor;
			int count = pictureSelectionLayoutItems.Count;
			for (int i = 0; i < count; i++) {
				HotZoneCollection hotZones = pictureSelectionLayoutItems[i].HotZones;
				HotZone hotZone = CalculateHotZoneCore(hotZones, point, zoomFactor);
				if (hotZone != null)
					return hotZone;
			}
			return null;
		}
		HotZone CalculateMailMergeHotZone(Point point) {
			HotZoneCollection hotZones = new HotZoneCollection();
			if (MailMergeHeaderItem != null)
				hotZones.AddRange(MailMergeHeaderItem.BorderItem.HotZones);
			if (MailMergeFooterItem != null)
				hotZones.AddRange(MailMergeFooterItem.BorderItem.HotZones);
			if (MailMergeDetailItem != null)
				hotZones.AddRange(MailMergeDetailItem.BorderItem.HotZones);
			foreach (RangeMailMergeDetailLevelLayoutItem detailLevelItem in mailMergeDetailLevelList)
				hotZones.AddRange(detailLevelItem.BorderItem.HotZones);
			foreach (RangeMailMergeGroupHeaderLayoutItem groupHeaderItem in mailMergeGroupHeadersList)
				hotZones.AddRange(groupHeaderItem.BorderItem.HotZones);
			foreach (RangeMailMergeLayoutItem groupFooterItem in mailMergeGroupFootersList)
				hotZones.AddRange(groupFooterItem.BorderItem.HotZones);
			if (hotZones.Count == 0)
				return null;
			return CalculateHotZoneCore(hotZones, point, View.ZoomFactor);
		}
		HotZone CalculateRangeDragHotZone(Point point) {
			if (pageSelectionItem == null || pageSelectionItem.BorderItem == null || !DocumentModel.BehaviorOptions.DragAllowed)
				return null;
			HotZoneCollection hotZones = pageSelectionItem.BorderItem.HotZones;
			return CalculateHotZoneCore(hotZones, point, View.ZoomFactor);
		}
		protected internal HotZone CalculateHotZoneCore(HotZoneCollection hotZones, Point point, float zoomFactor) {
			return CalculateHotZoneCore(hotZones, point, zoomFactor, LayoutUnitConverter);
		}
		HotZone CalculateHotZoneCore(HotZoneCollection hotZones, Point point, float zoomFactor, DocumentLayoutUnitConverter unitConverter) {
			return HotZoneCalculator.CalculateHotZone(hotZones, point, zoomFactor, unitConverter);
		}
	}
#endregion
#region SelectionLayoutItemCollection
	public class SelectionLayoutItemCollection : List<ISelectionLayoutItem> {
	}
#endregion
}
