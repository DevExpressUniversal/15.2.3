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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using LayoutUnit = System.Int32;
using ModelUnit = System.Int32;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Layout.Engine;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region ChangeTableVirtualColumnRightCommand
	public class ChangeTableVirtualColumnRightCommand : RichEditSelectionCommand {
		#region Fields
		readonly VirtualTableColumn column;
		readonly LayoutUnit value;
		#endregion
		public ChangeTableVirtualColumnRightCommand(IRichEditControl control, VirtualTableColumn column, LayoutUnit value)
			: base(control) {
			Guard.ArgumentNotNull(column, "column");
			this.column = column;
			this.value = value;
		}
		#region Properties
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		#endregion
		protected internal override bool PerformChangeSelection() {
			return true;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return false;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Tables);
			ApplyDocumentProtectionToSelectedParagraphs(state);
			ApplyDocumentProtectionToTable(state, column.TableViewInfo.Table);
		}
		protected internal override void PerformModifyModel() {
			Debug.Assert(column.Elements.Count > 0);
			TableElementPairUniqueCollection elements = CreateTableElementPairUniqueCollection(column.Elements);
			if (!IsPairsValid(elements))
				return;
			TableViewInfo tableViewInfo = column.TableViewInfo;			
			Table table = tableViewInfo.Table;
			FromAutoToRealWidthsTableCalculator.ApplyRealWidths(tableViewInfo);
			SortedList<LayoutUnit> alignmentedPositions = tableViewInfo.VerticalBorderPositions.AlignmentedPosition;
			SortedList<LayoutUnit> initialPositions = tableViewInfo.VerticalBorderPositions.InitialPositions;
			LayoutUnit textAreaOffset = tableViewInfo.TextAreaOffset;
			int newLayoutPosition = value - tableViewInfo.Column.Bounds.Left;
			if (textAreaOffset != 0) {
				int[] initialPositionsArray = new int[initialPositions.Count];
				int count = initialPositions.Count;
				for (int i = 0; i < count; i++)
					initialPositionsArray[i] = initialPositions[i] - textAreaOffset;
				initialPositions = new SortedList<ModelUnit>();
				for (int i = 0; i < count; i++)
					initialPositions.Add(initialPositionsArray[i]);
			}
			TableRow row = column.Elements[0].Row;
			LayoutUnit newModelPosition = CalculateModelPositionByLayoutPosition(row, newLayoutPosition, initialPositions, alignmentedPositions);
			int newModelPositionIndex = initialPositions.BinarySearch(newModelPosition);			
			if(newModelPositionIndex < 0) {				
				Debug.Assert(newModelPositionIndex < 0);
				newModelPositionIndex = ~newModelPositionIndex;				
				ChangeCellSpans(table, newModelPositionIndex - 1);
				initialPositions = initialPositions.Clone();
				initialPositions.Add(newModelPosition);
			}
			bool shouldSetFixedLayout = FromAutoToRealWidthsTableCalculator.ApplyNewWidth(elements, initialPositions, newModelPositionIndex, DocumentModel.ToDocumentLayoutUnitConverter, column);
			NormalizeTable(table, initialPositions);
			if (shouldSetFixedLayout && table.TableLayout != TableLayoutType.Fixed)
				table.TableLayout = TableLayoutType.Fixed;
			SetTablePreferredWidth(table, initialPositions);
		}		
		protected virtual LayoutUnit CalculateModelPositionByLayoutPosition(TableRow row, int layoutPosition, SortedList<int> initialPositions, SortedList<int> alignmentPositions) {
			LayoutUnit delta = layoutPosition - alignmentPositions[row.LayoutProperties.GridBefore];
			return initialPositions[row.GridBefore] + delta;
		}
		protected virtual int FindModelPositionIndexByLayoutPositionIndex(TableRow tableRow, int layoutPosition) {
			int modelPositionIndex = tableRow.GridBefore;
			layoutPosition -= tableRow.LayoutProperties.GridBefore;
			int cellIndex = 0;
			TableCellCollection cells = tableRow.Cells;
			int count = cells.Count;
			while(layoutPosition > 0 && cellIndex < count) {
				TableCell cell = cells[cellIndex];
				modelPositionIndex += cell.ColumnSpan;
				layoutPosition -= cell.LayoutProperties.ColumnSpan;
				cellIndex++;
			}
			return modelPositionIndex;
		}
		protected virtual ModelUnit GetActualWidth(WidthUnit width) {
			if (width.Type != WidthUnitType.ModelUnits)
				return 0;
			else
				return width.Value;
		}
		private void SetTablePreferredWidth(Table table, SortedList<int> initialPositions) {			
			WidthUnit tablePreferredWidth = table.PreferredWidth;
			if (tablePreferredWidth.Type == WidthUnitType.Nil || tablePreferredWidth.Type == WidthUnitType.Auto)
				return;
			TableRow firstRow = table.Rows.First;
			ModelUnit totalWidth = GetActualWidth(firstRow.WidthBefore) + GetActualWidth(firstRow.WidthAfter);
			TableCellCollection cells = firstRow.Cells;
			int count = cells.Count;
			for (int i = 0; i < count; i++) {
				totalWidth += GetActualWidth(cells[i].PreferredWidth);
			}			
			ModelUnit newPreferredWidth = totalWidth;
			if (!FromAutoToRealWidthsTableCalculator.ShouldSetNewPreferredWidth(table, newPreferredWidth))
				return;
			TableProperties tableProperties = table.TableProperties;
			tableProperties.PreferredWidth.CopyFrom(new WidthUnitInfo(WidthUnitType.ModelUnits, Math.Max(newPreferredWidth, 1)));
		}
		protected virtual void NormalizeTable(Table table, SortedList<LayoutUnit> initialPositions) {
			int minGridBefore = Int32.MaxValue;
			ModelUnit minWidthBefore = 0;
			int minGridAfter = Int32.MaxValue;
			ModelUnit minWidthAfter = 0;
			TableRowProcessorDelegate findMinGridBeforeAfter = delegate(TableRow row) {
				int gridBefore = row.GridBefore;
				if (gridBefore < minGridBefore) {
					minGridBefore = gridBefore;
					minWidthBefore = row.WidthBefore.Type == WidthUnitType.ModelUnits ? row.WidthBefore.Value : 0;
				}
				int gridAfter = row.GridAfter;
				if (gridAfter < minGridAfter) {
					minGridAfter = gridAfter;
					minWidthAfter = row.WidthAfter.Type == WidthUnitType.ModelUnits ? row.WidthAfter.Value : 0;
				}
			};
			table.ForEachRow(findMinGridBeforeAfter);
			if (minGridBefore > 0) {
				LayoutUnit value = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(initialPositions[0]) + minWidthBefore - column.TableViewInfo.ModelRelativeIndent;
				if (table.NestedLevel > 0)
					value = Math.Max(0, value);
				table.TableProperties.TableIndent.CopyFrom(new WidthUnitInfo(WidthUnitType.ModelUnits, value));				
			}
			TableRowProcessorDelegate decreaseWidthBeforeAfter = delegate(TableRow row) {
				if (minGridBefore > 0) {
					ModelUnit newWidthBeforeValue = row.WidthBefore.Value - minWidthBefore;
					row.Properties.WidthBefore.CopyFrom(new WidthUnitInfo(WidthUnitType.ModelUnits, newWidthBeforeValue));
				}
				if (minGridAfter > 0) {
					ModelUnit newWidthAfterValue = row.WidthAfter.Value - minWidthAfter;
					row.Properties.WidthAfter.CopyFrom(new WidthUnitInfo(WidthUnitType.ModelUnits, newWidthAfterValue));
				}
			};
			table.ForEachRow(decreaseWidthBeforeAfter);
			table.NormalizeCellColumnSpans();			
		}
		protected virtual bool IsPairsValid(TableElementPairUniqueCollection pairs) {
			Dictionary<TableElementAccessorBase, TableElementPair> expectedNextElements = new Dictionary<TableElementAccessorBase, TableElementPair>();
			int count = pairs.Count;
			TableRowAlignment expectedRowAlignment = pairs[0].Element.Row.TableRowAlignment;
			for (int i = 0; i < count; i++) {
				TableElementPair pair = pairs[i];
				if (pair.Element.Row.TableRowAlignment != expectedRowAlignment)
					return false;
				TableElementAccessorBase nextElement = pair.NextElement;
				if (nextElement != null) {
					List<TableElementAccessorBase> verticalSpanElements = nextElement.GetVerticalSpanElements();
					int verticalSpanElementCount = verticalSpanElements.Count;
					for (int j = 0; j < verticalSpanElementCount; j++) {
						TableElementAccessorBase element = verticalSpanElements[j];
						if (!expectedNextElements.ContainsKey(element))
							expectedNextElements.Add(element, pair);
					}
				}
			}
			for (int i = 0; i < count; i++) {
				TableElementPair pair = pairs[i];
				if(pair.NextElement != null)
					expectedNextElements.Remove(pair.NextElement);
			}
			return expectedNextElements.Count == 0;
		}
		protected virtual TableElementPairUniqueCollection CreateTableElementPairUniqueCollection(TableElementAccessorCollection elements) {
			TableElementPairUniqueCollection result = new TableElementPairUniqueCollection();
			int count = elements.Count;
			for (int i = 0; i < count; i++) {
				TableElementAccessorBase element = elements[i];
				AddCellCore(result, element);
			}
			return result;			
		}
		protected virtual void AddCellCore(TableElementPairUniqueCollection pairs, TableElementAccessorBase element) {
			List<TableElementAccessorBase> verticalSpanElements = element.GetVerticalSpanElements();
			int verticalSpanElementCount = verticalSpanElements.Count;
			for (int i = 0; i < verticalSpanElementCount; i++)
				pairs.Add(verticalSpanElements[i]);
		}
		protected virtual void ChangeCellSpans(Table table, int columnIndex) {
			TableCellProcessorDelegate changeSpan = delegate(TableCell cell) {
				int startColumnIndex = cell.GetStartColumnIndexConsiderRowGrid();
				if (startColumnIndex > columnIndex)
					return;
				int endColumnIndex = cell.GetEndColumnIndexConsiderRowGrid();
				if (endColumnIndex < columnIndex)
					return;
				cell.Properties.ColumnSpan = cell.ColumnSpan + 1;
			};
			table.ForEachCell(changeSpan);
			TableRowProcessorDelegate changeGridBefore = delegate(TableRow row) {
				int gridBefore = row.GridBefore;
				if (columnIndex >= gridBefore)
					return;
				if (gridBefore == 0)
					row.Properties.WidthBefore.CopyFrom(new WidthUnitInfo(WidthUnitType.ModelUnits, 0));
				row.Properties.GridBefore = gridBefore + 1;
			};
			table.ForEachRow(changeGridBefore);
			TableRowProcessorDelegate changeGridAfter = delegate(TableRow row) {
				int gridAfter = row.GridAfter;				
				int startColumnIndex = row.LastCell.GetEndColumnIndexConsiderRowGrid() + 1;
				if (startColumnIndex > columnIndex)
					return;
				if (gridAfter == 0)
					 row.Properties.WidthAfter.CopyFrom(new WidthUnitInfo(WidthUnitType.ModelUnits, 0));
				row.Properties.GridAfter = gridAfter + 1;
			};
			table.ForEachRow(changeGridAfter);
		}
		protected virtual void ChangeTableLeftOffset() {
			throw new NotImplementedException();
		}
	}
	#endregion
	public class FromAutoToRealWidthsTableCalculator {
		public static void ApplyRealWidths(TableViewInfo tableViewInfo) {
			Table table = tableViewInfo.Table;
			ApplyRealWidthToTable(table, tableViewInfo.VerticalBorderPositions.InitialPositions);
		}
		public static void ApplyRealWidthToTable(Table table, SortedList<int> positions) {
			TableCellProcessorDelegate applyRealWidth = delegate(TableCell cell) {
				ApplyRealWidthToCell(cell, positions);
			};
			table.ForEachCell(applyRealWidth);
			TableRowProcessorDelegate applyRealWidthToRow = delegate(TableRow row) {
				ApplyRealWidthToRow(row, positions);
			};
			table.ForEachRow(applyRealWidthToRow);
		}
		public static bool ApplyNewWidth(TableElementPairUniqueCollection pairs, SortedList<int> initialPositions, int newModelPositionIndex, DocumentModelUnitToLayoutUnitConverter converter, VirtualTableColumn column) {
			bool shouldSetFixedLayout = false;
			int count = pairs.Count;
			for (int i = 0; i < count; i++) {
				TableElementPair pair = pairs[i];
				TableElementAccessorBase element = pair.Element;
				TableElementAccessorBase nextElement = pair.NextElement;
				int leftModelPositionIndex = element.GetStartColumnIndex();
				ModelUnit elementWidth = converter.ToModelUnits(GetPreferredWidthByTotalWidth(element, initialPositions[newModelPositionIndex] - initialPositions[leftModelPositionIndex]));
				if (nextElement != null) {
					ModelUnit initialElementWidth = element.PreferredWidth.Value;
					ModelUnit initialNextElementWidth = nextElement.PreferredWidth.Value;
					ModelUnit initialSummaryWidth = initialElementWidth + initialNextElementWidth;
					int rightModelPositionIndex = nextElement.GetEndColumnIndex();
					ModelUnit nextElementWidth;
					if (newModelPositionIndex == leftModelPositionIndex || newModelPositionIndex == rightModelPositionIndex) {
						Debug.Assert((newModelPositionIndex == 0 && newModelPositionIndex == leftModelPositionIndex) || (newModelPositionIndex == initialPositions.Count - 1 && newModelPositionIndex == rightModelPositionIndex));
						nextElementWidth = Math.Max(converter.ToModelUnits(GetPreferredWidthByTotalWidth(element, initialPositions[rightModelPositionIndex] - initialPositions[leftModelPositionIndex])), 0);
					}
					else
						nextElementWidth = initialSummaryWidth - elementWidth;
					element.ModelColumnSpan = newModelPositionIndex - leftModelPositionIndex;
					element.PreferredWidth = new WidthUnitInfo(WidthUnitType.ModelUnits, elementWidth);
					shouldSetFixedLayout |= elementWidth < converter.ToModelUnits(element.GetMinContentWidth());
					int nextElementColumnSpan = rightModelPositionIndex - newModelPositionIndex;
					nextElement.ModelColumnSpan = nextElementColumnSpan;
					nextElement.PreferredWidth = new WidthUnitInfo(WidthUnitType.ModelUnits, nextElementWidth);
					shouldSetFixedLayout |= nextElementWidth < converter.ToModelUnits(nextElement.GetMinContentWidth());
				}
				else {
					element.ModelColumnSpan = newModelPositionIndex - leftModelPositionIndex;
					element.PreferredWidth = new WidthUnitInfo(WidthUnitType.ModelUnits, elementWidth);
					shouldSetFixedLayout |= elementWidth < converter.ToModelUnits(element.GetMinContentWidth());
				}
			}
			if (newModelPositionIndex == 0) {
				Table table = pairs[0].Element.Row.Table;
				ModelUnit newValue = converter.ToModelUnits(initialPositions[0]) - column.TableViewInfo.ModelRelativeIndent;
				if (table.NestedLevel > 0)
					newValue = Math.Max(newValue, 0);
				table.TableProperties.TableIndent.CopyFrom(new WidthUnitInfo(WidthUnitType.ModelUnits, newValue));
				ModelUnit offset = converter.ToModelUnits(initialPositions[1] - initialPositions[0]);
				TableRowProcessorDelegate increaseWidthBefore = delegate(TableRow row) {
					if (row.GridBefore > 0) {
						row.Properties.WidthBefore.Type = WidthUnitType.ModelUnits;
						row.Properties.WidthBefore.Value = row.WidthBefore.Value + offset;
					}
				};
				table.ForEachRow(increaseWidthBefore);
			}
			else if (newModelPositionIndex == initialPositions.Count - 1) {
				Table table = pairs[0].Element.Row.Table;
				int lastModelPositionIndex = initialPositions.Count - 1;
				ModelUnit offset = converter.ToModelUnits(initialPositions[lastModelPositionIndex] - initialPositions[lastModelPositionIndex - 1]);
				TableRowProcessorDelegate increaseWidthAfter = delegate(TableRow row) {
					if (row.GridAfter > 0) {
						ModelUnit newWidthAfter = row.WidthAfter.Value + offset;
						row.Properties.WidthAfter.CopyFrom(new WidthUnitInfo(WidthUnitType.ModelUnits, newWidthAfter));
					}
				};
				table.ForEachRow(increaseWidthAfter);
			}
			return shouldSetFixedLayout;
		}
		static LayoutUnit GetPreferredWidthByTotalWidth(TableElementAccessorBase element, LayoutUnit totalWidth) {
			return totalWidth;
		}
		static LayoutUnit GetPreferredWidthByTotalWidth(TableCell cell, LayoutUnit totalWidth) {
			return totalWidth;
		}
		public static void ApplyRealWidthToRow(TableRow row, SortedList<int> positions) {
			int gridBefore = row.GridBefore;
			if (gridBefore > 0) {
				int startIndex = row.GridBefore;
				LayoutUnit totalWidth = positions[startIndex] - positions[0];
				ModelUnit newWidthBefore = Math.Max(row.DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(totalWidth), 1);
				if (ShouldSetNewWidthBefore(row, newWidthBefore)) {
					row.Properties.WidthBefore.CopyFrom(new WidthUnitInfo(WidthUnitType.ModelUnits, newWidthBefore));
				}
			}
			int gridAfter = row.GridAfter;
			if (gridAfter > 0) {
				int startIndex = row.LastCell.GetEndColumnIndexConsiderRowGrid() + 1;
				int endIndex = startIndex + gridAfter;
				LayoutUnit totalWidth = positions[endIndex] - positions[startIndex];
				ModelUnit newWidthAfter = Math.Max(row.DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(totalWidth), 1);
				if (ShouldSetNewWidthAfter(row, newWidthAfter)) {
					row.Properties.WidthAfter.CopyFrom(new WidthUnitInfo(WidthUnitType.ModelUnits, newWidthAfter));
				}
			}
		}
		public static void ApplyRealWidthToCell(TableCell cell, SortedList<int> positions) {
			int startIndex = cell.GetStartColumnIndexConsiderRowGrid();
			int endIndex = startIndex + cell.ColumnSpan;
			LayoutUnit totalWidth = positions[endIndex] - positions[startIndex];
			ModelUnit newPreferredWidth = Math.Max(cell.DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(GetPreferredWidthByTotalWidth(cell, totalWidth)), 1);
			if (ShouldSetNewPreferredWidth(cell, newPreferredWidth)) {
				PreferredWidth preferredWidth = cell.Properties.PreferredWidth;
				preferredWidth.Type = WidthUnitType.ModelUnits;
				preferredWidth.Value = newPreferredWidth;
			}
		}
		public static bool ShouldSetNewPreferredWidth(Table table, ModelUnit newPreferredWidth) {
			TableProperties properties = table.TableProperties;
			if (!properties.UsePreferredWidth)
				return true;
			return ShouldSetNewWidthCore(table.PreferredWidth, newPreferredWidth, table.DocumentModel.ToDocumentLayoutUnitConverter);
		}
		static bool ShouldSetNewPreferredWidth(TableCell cell, ModelUnit newPreferredWidth) {
			TableCellProperties properties = cell.Properties;
			if (!properties.UsePreferredWidth)
				return true;
			return ShouldSetNewWidthCore(cell.PreferredWidth, newPreferredWidth, cell.DocumentModel.ToDocumentLayoutUnitConverter);
		}
		static  bool ShouldSetNewWidthBefore(TableRow row, ModelUnit newWidth) {
			TableRowProperties properties = row.Properties;
			if (!properties.UseWidthBefore)
				return true;
			return ShouldSetNewWidthCore(properties.WidthBefore, newWidth, row.DocumentModel.ToDocumentLayoutUnitConverter);
		}
		static bool ShouldSetNewWidthAfter(TableRow row, ModelUnit newWidth) {
			TableRowProperties properties = row.Properties;
			if (!properties.UseWidthAfter)
				return true;
			return ShouldSetNewWidthCore(properties.WidthAfter, newWidth, row.DocumentModel.ToDocumentLayoutUnitConverter);
		}
		static bool ShouldSetNewWidthCore(WidthUnit currentWidth, ModelUnit newWidth, DocumentModelUnitToLayoutUnitConverter converter) {
			if (currentWidth.Type != WidthUnitType.ModelUnits)
				return true;
			LayoutUnit delta = converter.ToModelUnits(2);
			return Math.Abs(currentWidth.Value - newWidth) > delta;
		}
	}
	#region TableCellPair
	public class TableElementPair {
		TableElementAccessorBase element;
		TableElementAccessorBase nextElement;
		bool nextElementValid;
		public TableElementPair(TableElementAccessorBase element) {
			this.element = element;
		}
		public TableElementAccessorBase Element { get { return element; } }
		public TableElementAccessorBase NextElement {
			get {
				if (!nextElementValid) {
					nextElement = GetNextElement();
					nextElementValid = true;
				}
				return nextElement;
			}
		}
		protected virtual TableElementAccessorBase GetNextElement() {
			return element.GetNextElement();
		}
	}
	#endregion
	#region TableElementPairUniqueCollection
	public class TableElementPairUniqueCollection {
		List<TableElementPair> innerList;
		Dictionary<TableElementAccessorBase, int> mapElementPairIndex;
		public TableElementPairUniqueCollection() {
			this.innerList = new List<TableElementPair>();
			this.mapElementPairIndex = new Dictionary<TableElementAccessorBase, int>();
		}
		public int Add(TableElementAccessorBase element) {
			int index;
			if (mapElementPairIndex.TryGetValue(element, out index))
				return index;			
			index = innerList.Count;
			innerList.Add(new TableElementPair(element));
			mapElementPairIndex.Add(element, index);
			return index;
		}
		public int Count { get { return innerList.Count; } }
		public TableElementPair this[int index] { get { return innerList[index]; } }
	}
	#endregion
	#region TableElementAccessorBase
	public abstract class TableElementAccessorBase {
		public abstract TableRow Row { get; }
		public abstract int LayoutColumnSpan { get; }
		public abstract int ModelColumnSpan { get; set; }
		public abstract WidthUnitInfo PreferredWidth { get; set; }
		public abstract TableElementAccessorBase GetNextElement();
		public abstract int GetStartColumnIndex();
		public abstract List<TableElementAccessorBase> GetVerticalSpanElements();
		protected abstract bool IsElementSelected(SelectedCellsCollection selectedCells);
		public virtual int GetEndColumnIndex() {
			return GetStartColumnIndex() + ModelColumnSpan;
		}
		protected virtual WidthUnitInfo CoercePreferredWidth(WidthUnitInfo info) {
			if (ModelColumnSpan > 0)
				return new WidthUnitInfo(WidthUnitType.ModelUnits, Math.Max(info.Value, 1));
			else
				return new WidthUnitInfo(WidthUnitType.Nil, 0);
		}
		public virtual bool IsRightBoundarySelected(SelectedCellsCollection selectedCells) {
			return IsElementSelected(selectedCells) || IsNextElementSelected(selectedCells);
		}
		protected virtual bool IsNextElementSelected(SelectedCellsCollection selectedCells) {
			TableElementAccessorBase nextElement = GetNextElement();
			return nextElement != null && nextElement.IsElementSelected(selectedCells);
		}
		public abstract int GetMinContentWidth();
		public abstract int GetMinRightBorderPosition(TableViewInfo tableViewInfo);
		public abstract int GetMaxRightBorderPosition(TableViewInfo tableViewInfo);
		public abstract int GetRightBorderPosition(TableViewInfo tableViewInfo);
	}
	#endregion
	#region TableElementAccessorCollection
	public class TableElementAccessorCollection : List<TableElementAccessorBase> {
	}
	#endregion 
	#region TableRowAccessorBase (abstract class)
	public abstract class TableRowAccessorBase : TableElementAccessorBase {
		readonly TableRow row;
		protected TableRowAccessorBase(TableRow row) {
			Guard.ArgumentNotNull(row, "row");
			this.row = row;
		}
		public override TableRow Row { get { return row; } }
		public override List<TableElementAccessorBase> GetVerticalSpanElements() {
			List<TableElementAccessorBase> result = new List<TableElementAccessorBase>();
			result.Add(this);
			return result;
		}
		public override int GetMinContentWidth() {
			return 0;
		}
	}
	#endregion
	#region TableRowBeforeAccessorBase
	public class TableRowBeforeAccessor : TableRowAccessorBase {
		public TableRowBeforeAccessor(TableRow row)
			: base(row) {			
		}
		public override int LayoutColumnSpan { get { return Row.LayoutProperties.GridBefore; } }
		public override int ModelColumnSpan {
			get {
				return Row.GridBefore;
			}
			set {
				Row.Properties.GridBefore = value;
			}
		}
		public override WidthUnitInfo PreferredWidth {
			get {
				return Row.WidthBefore.Info;
			}
			set {
				Row.Properties.WidthBefore.CopyFrom(CoercePreferredWidth(value));
			}
		}
		public override TableElementAccessorBase GetNextElement() {
			return new TableCellAccessor(Row.Cells.First);
		}
		public override bool Equals(object obj) {
			TableRowBeforeAccessor other = obj as TableRowBeforeAccessor;
			if (other == null)
				return false;
			else
				return Row == other.Row;
		}
		public override int GetHashCode() {
			return Row.GetHashCode();
		}
		public override int GetStartColumnIndex() {
			return 0;
		}
		protected override bool IsElementSelected(SelectedCellsCollection selectedCells) {
			return false;
		}
		public override int GetMinRightBorderPosition(TableViewInfo tableViewInfo) {
			return Int32.MinValue + 1000;
		}
		public override int GetMaxRightBorderPosition(TableViewInfo tableViewInfo) {
			return GetNextElement().GetRightBorderPosition(tableViewInfo);
		}
		public override int GetRightBorderPosition(TableViewInfo tableViewInfo) {
			return GetNextElement().GetMinRightBorderPosition(tableViewInfo);
		}
	}
	#endregion
	#region TableRowAfterAccessorBase
	public class TableRowAfterAccessor : TableRowAccessorBase {
		public TableRowAfterAccessor(TableRow row)
			: base(row) {			
		}
		public override int LayoutColumnSpan { get { return Row.LayoutProperties.GridAfter; } }
		public override int ModelColumnSpan {
			get {
				return Row.GridAfter;
			}
			set {
				Row.Properties.GridAfter = value;
			}
		}
		public override WidthUnitInfo PreferredWidth {
			get {
				return Row.WidthAfter.Info;
			}
			set {
				Row.Properties.WidthAfter.CopyFrom(CoercePreferredWidth(value));
			}
		}
		public override TableElementAccessorBase GetNextElement() {
			return null;
		}
		public override bool Equals(object obj) {
			TableRowAfterAccessor other = obj as TableRowAfterAccessor;
			if (other == null)
				return false;
			else
				return Row == other.Row;
		}
		public override int GetHashCode() {
			return Row.GetHashCode();
		}
		public override int GetStartColumnIndex() {
			TableCell lastCell = Row.Cells.Last;
			return TableCellVerticalBorderCalculator.GetStartColumnIndex(lastCell, false) + lastCell.ColumnSpan;
		}
		protected override bool IsElementSelected(SelectedCellsCollection selectedCells) {
			return false;
		}
		public override int GetMinRightBorderPosition(TableViewInfo tableViewInfo) {
			TableCell lastCell = Row.Cells.Last;
			return tableViewInfo.GetAlignmentedPosition(TableCellVerticalBorderCalculator.GetStartColumnIndex(lastCell, true) + lastCell.LayoutProperties.ColumnSpan);
		}
		public override int GetMaxRightBorderPosition(TableViewInfo tableViewInfo) {
			return Int32.MaxValue - 1000;
		}
		public override int GetRightBorderPosition(TableViewInfo tableViewInfo) {
			return Int32.MaxValue - 1000;
		}
	}
	#endregion
	#region TableCellAccessorBase
	public class TableCellAccessor : TableElementAccessorBase {
		TableCell cell;
		public TableCellAccessor(TableCell cell) {
			this.cell = cell;
		}
		public override TableRow Row { get { return Cell.Row; } }
		public TableCell Cell { get { return cell; } }
		public override int LayoutColumnSpan { get { return Cell.LayoutProperties.ColumnSpan; } }
		public override int ModelColumnSpan {
			get {
				return Cell.ColumnSpan;
			}
			set {
				Cell.Properties.ColumnSpan= value;
			}
		}
		public override WidthUnitInfo PreferredWidth {
			get {
				return Cell.PreferredWidth.Info;
			}
			set {
				Cell.Properties.PreferredWidth.CopyFrom(CoercePreferredWidth(value));
			}
		}
		public override TableElementAccessorBase GetNextElement() {
			TableRow row = Cell.Row;
			TableCellCollection cells = row.Cells;
			int cellIndex = cells.IndexOf(Cell);
			if (cellIndex + 1 < cells.Count)
				return new TableCellAccessor(cells[cellIndex + 1]);
			else 
				return new TableRowAfterAccessor(row);			
		}
		public override bool Equals(object obj) {
			TableCellAccessor other = obj as TableCellAccessor;
			if (other == null)
				return false;
			else
				return Cell == other.Cell;
		}
		public override int GetHashCode() {
			return Cell.GetHashCode();
		}
		public override int GetStartColumnIndex() {
			return TableCellVerticalBorderCalculator.GetStartColumnIndex(Cell, false);
		}
		public override List<TableElementAccessorBase> GetVerticalSpanElements() {
			List<TableCell> cells = TableCellVerticalBorderCalculator.GetVerticalSpanCells(Cell, GetStartColumnIndex(), false);
			List<TableElementAccessorBase> result = new List<TableElementAccessorBase>();
			int count = cells.Count;
			for(int i = 0; i < count; i++)
				result.Add(new TableCellAccessor(cells[i]));
			return result;
		}
		protected override bool IsElementSelected(SelectedCellsCollection selectedCells) {
			return selectedCells.IsWholeCellSelected(Cell);
		}
		public override int GetMinContentWidth() {			
			return Cell.LayoutProperties.ContainerWidthsInfo.MinWidth;
		}
		public override int GetMinRightBorderPosition(TableViewInfo tableViewInfo) {
			return tableViewInfo.GetAlignmentedPosition(TableCellVerticalBorderCalculator.GetStartColumnIndex(Cell, true));
		}
		public override int GetMaxRightBorderPosition(TableViewInfo tableViewInfo) {
			return GetNextElement().GetRightBorderPosition(tableViewInfo);
		}
		public override int GetRightBorderPosition(TableViewInfo tableViewInfo) {
			return tableViewInfo.GetAlignmentedPosition(TableCellVerticalBorderCalculator.GetStartColumnIndex(Cell, true) + Cell.LayoutProperties.ColumnSpan);
		}
	}
	#endregion
}
