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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {   
	public class TableCellsManager : IDocumentModelStructureChangedListener {
		#region Fields        
		class TableCellNodeComparer : IComparer<TableCellNode> {
			#region IComparer<TableCellNode> Members
			public int Compare(TableCellNode x, TableCellNode y) {
				return x.Cell.StartParagraphIndex - y.Cell.StartParagraphIndex;
			}
			#endregion
		}
		class TableCellNodeAndCellComparer : IComparable<TableCellNode> {
			TableCell cellToInsert;
			public TableCellNodeAndCellComparer(TableCell cellToInsert) {
				this.cellToInsert = cellToInsert;
			}
			#region IComparable<TableCellNode> Members
			public int CompareTo(TableCellNode other) {
				TableCell otherCell = other.Cell;
				if (IsNestedCell(cellToInsert, otherCell))
					return 0;
				if (IsNestedCell(otherCell, cellToInsert))
					return 1;
				else
					return otherCell.StartParagraphIndex - cellToInsert.StartParagraphIndex;
			}
			#endregion
			bool IsNestedCell(TableCell nestedCell, TableCell outerCell) {
				return nestedCell.StartParagraphIndex >= outerCell.StartParagraphIndex && nestedCell.EndParagraphIndex <= outerCell.EndParagraphIndex;
			}
		}
		class TableCellNodeAndParagraphIndexComparer : IComparable<TableCellNode> {
			ParagraphIndex index;
			public TableCellNodeAndParagraphIndexComparer(ParagraphIndex index) {
				this.index = index;
			}
			#region IComparable<TableCellNode> Members
			public int CompareTo(TableCellNode other) {
				if (index < other.Cell.StartParagraphIndex)
					return 1;
				if (index > other.Cell.EndParagraphIndex)
					return -1;
				return 0;
			}
			#endregion
		}
		public class TableCellNode {
			TableCell cell;
			SortedList<TableCellNode> childNodes;
			public TableCellNode()
				: this(null) {
			}
			TableCellNode(TableCell cell) {
				this.cell = cell;
			}
			public TableCell Cell { get { return cell; } }
			protected bool IsRootNode { get { return Cell != null; } }
			protected internal SortedList<TableCellNode> ChildNodes { get { return childNodes; } }	
			public void Add(TableCell cell) {
				if (childNodes == null) {
					this.childNodes = CreateChildNodes(cell);
					return;
				}
				int childIndex = childNodes.BinarySearch(new TableCellNodeAndCellComparer(cell));				
				if (childIndex >= 0) {					
					AddChildAtPosition(cell, childIndex);
					return;
				}
				int nextCellIndex = ~childIndex;
				if (nextCellIndex < childNodes.Count) {
					TableCellNode nextNode = childNodes[nextCellIndex];
					TableCell nextCell = nextNode.Cell;
					if (nextCell.EndParagraphIndex < cell.EndParagraphIndex) {
						TableCellNode newNode = new TableCellNode(cell);
						newNode.childNodes = newNode.CreateChildNodesCore();
						Table prevTable = null;
						while (nextCellIndex < childNodes.Count &&
							((childNodes[nextCellIndex].Cell.EndParagraphIndex < cell.EndParagraphIndex) ||
							(childNodes[nextCellIndex].Cell.EndParagraphIndex == cell.EndParagraphIndex && childNodes[nextCellIndex].Cell.Table.NestedLevel > cell.Table.NestedLevel))) {
							nextNode = childNodes[nextCellIndex];
							childNodes.RemoveAt(nextCellIndex);
							newNode.childNodes.Add(nextNode);
							nextNode.Cell.Table.SetParentCell(newNode.Cell);
							Table nextTable = nextNode.Cell.Table;
							if(!object.ReferenceEquals(nextTable, prevTable)) {
								nextTable.SetParentCell(newNode.Cell);
								prevTable = nextTable;
							}
						}
						childNodes.Add(newNode);
						return;
					}
					else {
						childNodes.Insert(nextCellIndex, new TableCellNode(cell));
						return;
					}
				}
				childNodes.Insert(childNodes.Count, new TableCellNode(cell));
			}
			SortedList<TableCellNode> CreateChildNodes(TableCell firstCell) {
				SortedList<TableCellNode> result = CreateChildNodesCore();
				result.Add(new TableCellNode(firstCell));
				return result;
			}
			SortedList<TableCellNode> CreateChildNodesCore() {
				return new SortedList<TableCellNode>(new TableCellNodeComparer());
			}
			private void AddChildAtPosition(TableCell cell, int position) {
				TableCellNode otherCellNode = childNodes[position];
				TableCell otherCell = otherCellNode.Cell;
				if (otherCell.Table.NestedLevel < cell.Table.NestedLevel) {
					otherCellNode.Add(cell);
					return;
				}
				Debug.Assert(otherCell.Table.NestedLevel > cell.Table.NestedLevel &&
					cell.StartParagraphIndex == otherCell.StartParagraphIndex && cell.EndParagraphIndex == otherCell.EndParagraphIndex);
				childNodes.RemoveAt(position);
				TableCellNode newNode = new TableCellNode(cell);
				newNode.childNodes = newNode.CreateChildNodesCore();
				newNode.childNodes.Add(otherCellNode);
				childNodes.Add(newNode);
			}
			public TableCell GetCellByParagraphIndex(ParagraphIndex paragraphIndex) {
				if (childNodes == null)
					return cell;
				int index = childNodes.BinarySearch(new TableCellNodeAndParagraphIndexComparer(paragraphIndex));
				if (index < 0)
					return cell;
				else
					return childNodes[index].GetCellByParagraphIndex(paragraphIndex); 
			}
			public void ResetCachedTableLayoutInfo(ParagraphIndex from, ParagraphIndex to) {
				if (childNodes == null)
					return;
				int fromIndex = childNodes.BinarySearch(new TableCellNodeAndParagraphIndexComparer(from));
				if (fromIndex >= 0) {
					childNodes[fromIndex].Cell.Table.ResetCachedLayoutInfo();
					childNodes[fromIndex].ResetCachedTableLayoutInfo(from, to);
					fromIndex++;
				}
				else
					fromIndex = ~fromIndex;
				int toIndex = childNodes.BinarySearch(new TableCellNodeAndParagraphIndexComparer(to));
				if (toIndex >= 0) {
					childNodes[toIndex].Cell.Table.ResetCachedLayoutInfo();
					childNodes[toIndex].ResetCachedTableLayoutInfo(from, to);
				}
				else
					toIndex = ~toIndex;
				toIndex--;
				for (int i = fromIndex; i <= toIndex; i++) {
					childNodes[i].Cell.Table.ResetCachedLayoutInfo();
					childNodes[i].ResetCachedTableLayoutInfoRecursive();
				}
			}
			void ResetCachedTableLayoutInfoRecursive() {
				if (childNodes == null)
					return;
				int count = childNodes.Count;
				for (int i = 0; i < count; i++) {
					childNodes[i].Cell.Table.ResetCachedLayoutInfo();
					childNodes[i].ResetCachedTableLayoutInfoRecursive();
				}
			}
			public override string ToString() {
#if DEBUGTEST || DEBUG
				if (Cell == null)
					return base.ToString();
				return String.Format("Node {0}-{1}", Cell.StartParagraphIndex, Cell.EndParagraphIndex);
#else
					return base.ToString();
#endif
			}
			public bool IsParagraphInCell(ParagraphIndex paragraphIndex) {
				if (childNodes == null)
					return false;
				int index = childNodes.BinarySearch(new TableCellNodeAndParagraphIndexComparer(paragraphIndex));
				return index >= 0;
			}
			public TableCell GetCellByNestingLevel(ParagraphIndex paragraphIndex, int nestingLevel, TableCell cellFromPreviousLevel) {
				if (childNodes == null)
					return cellFromPreviousLevel;
				int index = childNodes.BinarySearch(new TableCellNodeAndParagraphIndexComparer(paragraphIndex));
				if (index < 0)
					return cellFromPreviousLevel;
				TableCellNode node = childNodes[index];
				TableCell cellNode = node.Cell;
				if (cellNode.Table.NestedLevel == nestingLevel)
					return cellNode;
				else
					return node.GetCellByNestingLevel(paragraphIndex, nestingLevel, cellNode);
			}
			protected internal virtual List<TableCell> GetCellsByParagraphIndex(ParagraphIndex paragraphIndex, int nestedLevel) {
				List<TableCell> result = new List<TableCell>();
				GetCellsByParagraphIndexCore(paragraphIndex, nestedLevel, result);
				return result;
			}
			void GetCellsByParagraphIndexCore(ParagraphIndex paragraphIndex, int nestedLevel, List<TableCell> cells) {
				if (ChildNodes == null)
					return;
				int index = childNodes.BinarySearch(new TableCellNodeAndParagraphIndexComparer(paragraphIndex));
				if (index < 0)
					return;
				TableCellNode node = childNodes[index];
				TableCell currentCell = node.Cell;
				if (currentCell.Table.NestedLevel >= nestedLevel)
					cells.Add(currentCell);
				node.GetCellsByParagraphIndexCore(paragraphIndex, nestedLevel, cells);
			}
			protected internal virtual TableCellNode GetSubTree(ParagraphIndex start, ParagraphIndex end, int fromNestedLevel) {
				if (childNodes == null)
					return null;
				TableCellNode result = new TableCellNode();
				if (result.childNodes == null) 
					result.childNodes = CreateChildNodesCore();
				int startIndex = childNodes.BinarySearch(new TableCellNodeAndParagraphIndexComparer(start));
				if (startIndex < 0) {
					int endIndex = childNodes.BinarySearch(new TableCellNodeAndParagraphIndexComparer(end));
					if (endIndex < 0) {
						if (~endIndex < 0)
							Exceptions.ThrowInternalException();
						endIndex = Math.Min(~endIndex, ChildNodes.Count - 1);
					}
					for (int i = ~startIndex; i <= endIndex; i++) {
						if (childNodes[i].Cell.StartParagraphIndex <= end && childNodes[i].Cell.EndParagraphIndex <= end)
							result.ChildNodes.Add(childNodes[i]);
					}
					return result;
				}
				TableCellNode currentNode = childNodes[startIndex];
				if (currentNode.Cell.Table.NestedLevel < fromNestedLevel) {
					int endIndex = childNodes.BinarySearch(new TableCellNodeAndParagraphIndexComparer(end));
					if (endIndex < 0) {
						endIndex = startIndex;
						TableCellNode another = childNodes[endIndex].GetSubTree(start, end, fromNestedLevel);
						if (another == null)
							return result;
					}
					for (int i = startIndex; i <= endIndex; i++) {
						currentNode = childNodes[i];
						TableCellNode another = currentNode.GetSubTree(start, end, fromNestedLevel);
						if (another != null)
							result.ChildNodes.Add(another);
					}
					return result;
				}
				if (currentNode.Cell.EndParagraphIndex == end) {
					result.ChildNodes.Add(currentNode);
					return result;
				}
				if (currentNode.Cell.StartParagraphIndex >= start && currentNode.Cell.EndParagraphIndex <= end) {
					result.ChildNodes.Add(currentNode);
					int endindex = childNodes.BinarySearch(new TableCellNodeAndParagraphIndexComparer(end));
					if (endindex < 0) {
						endindex = childNodes.Count - 1;
					}
					for (int i = startIndex + 1; i <= endindex; i++)
						if (childNodes[i].Cell.StartParagraphIndex <= end && childNodes[i].Cell.EndParagraphIndex <= end)
						result.ChildNodes.Add(childNodes[i]);
				}
				else
					return currentNode.GetSubTree(start, end, fromNestedLevel);				
				return result;
			}
			protected internal virtual void RemoveCellNode(TableCellNode targetCellNode, TableCellNode sourceCellNode, ParagraphIndex start, ParagraphIndex end) {
				for (int i = 0; i < sourceCellNode.childNodes.Count; i++) {
				}
			}
			public void OnParagraphRemove(ParagraphIndex index) {
				if (cell != null)
					cell.EndParagraphIndex--;			
				if (childNodes == null)
					return;
				int cellIndex = childNodes.BinarySearch(new TableCellNodeAndParagraphIndexComparer(index));
				if (cellIndex >= 0) {
					childNodes[cellIndex].OnParagraphRemove(index);
					cellIndex++;
				}
				else
					cellIndex = ~cellIndex;
				int count = childNodes.Count;
				for (int i = cellIndex; i < count; i++) {
					childNodes[i].ShiftParagraphIndex(-1);
				}
			}
			public TableCellNode OnParagraphInserted(ParagraphIndex index, bool cellToInsert) {
				if (cell != null) {
					Debug.Assert(cell.StartParagraphIndex <= index && index <= cell.EndParagraphIndex);
					cell.EndParagraphIndex++;
				}
				if (childNodes == null)
					return null;
				int cellIndex = childNodes.BinarySearch(new TableCellNodeAndParagraphIndexComparer(index));
				int nextCellIndex = cellIndex;
				if (nextCellIndex < 0)
					nextCellIndex = ~nextCellIndex;
				else
					if(!cellToInsert)
						nextCellIndex++;				
				int childCount = childNodes.Count;
				for (int i = nextCellIndex; i < childCount; i++)
					childNodes[i].ShiftParagraphIndex(1);
				return (cellIndex >= 0 && !cellToInsert) ? childNodes[cellIndex] : null;
			}
			void ShiftParagraphIndex(int delta) {
				cell.StartParagraphIndex += delta;
				cell.EndParagraphIndex += delta;
				if (childNodes == null)
					return;
				int count = childNodes.Count;
				for (int i = 0; i < count; i++) {
					childNodes[i].ShiftParagraphIndex(delta);
				}
			}
			public void Remove(TableCell cell) {
				if (childNodes == null)
					return;
				int count = childNodes.Count;
				for (int i = 0; i < count; i++) {
					TableCell nestedCell = childNodes[i].Cell;
					if (cell == nestedCell) {
						RemoveCore(i);
						return;
					}
					else
						if (cell.StartParagraphIndex >= nestedCell.StartParagraphIndex && cell.EndParagraphIndex <= nestedCell.EndParagraphIndex &&
							cell.Table.NestedLevel > nestedCell.Table.NestedLevel) {
							childNodes[i].Remove(cell);
							return;
						}
				}
			}
			void RemoveCore(int nodeIndex) {
				TableCellNode removedNode = childNodes[nodeIndex];
				childNodes.RemoveAt(nodeIndex);
				if (removedNode.childNodes == null)
					return;
				int count = removedNode.childNodes.Count;
				for (int i = 0; i < count; i++) {
					this.childNodes.Add(removedNode.childNodes[i]);					
					removedNode.childNodes[i].Cell.Table.SetParentCell(Cell);
					RecalcNestedLevelRecursive(removedNode.childNodes[i]);
				}
			}
			void RecalcNestedLevelRecursive(TableCellNode cellNode) {
				SortedList<TableCellNode> childNodes = cellNode.childNodes;
				if(childNodes == null)
					return;
				int count = childNodes.Count;
				for (int i = 0; i < count; i++) {
					childNodes[i].Cell.Table.RecalNestedLevel();
					RecalcNestedLevelRecursive(childNodes[i]);
				}
			}			
		}
		class TableCellTree {
			TableCellNode root;
			public TableCellTree() {
				this.root = new TableCellNode();
			}
			public void Add(TableCell cell) {
				root.Add(cell);
			}
			public TableCell GetCellByParagraphIndex(ParagraphIndex index) {
				return root.GetCellByParagraphIndex(index);
			}
			public void ResetCachedTableLayoutInfo(ParagraphIndex from, ParagraphIndex to) {
				root.ResetCachedTableLayoutInfo(from, to);
			}
			public bool IsParagraphInCell(ParagraphIndex index) {
				return root.IsParagraphInCell(index);
			}
			public void OnParagraphRemove(ParagraphIndex index) {
				root.OnParagraphRemove(index);
			}
			public void OnParagraphInserted(ParagraphIndex index, TableCell cell) {
				TableCellNode node = OnParagraphInsertedRecursive(index, cell);
#if DEBUGTEST || DEBUG
				TableCellNode innerNode =
#endif
				node.OnParagraphInserted(index, true);
#if DEBUGTEST || DEBUG
				Debug.Assert(innerNode == null);
#endif
			}
			public void Clear() {
				root = new TableCellNode();
			}
			public void Remove(TableCell cell) {
				root.Remove(cell);
			}
			public TableCell GetCellByNestingLevel(ParagraphIndex paragraphIndex, int nestingLevel) {
				return root.GetCellByNestingLevel(paragraphIndex, nestingLevel, null);
			}
			public List<TableCell> GetCellsByParagraphIndex(ParagraphIndex paragraphIndex, int nestedLevel) {
				return root.GetCellsByParagraphIndex(paragraphIndex, nestedLevel);
			}
			public TableCellNode GetCellSubTree(ParagraphIndex startParagraphIndex, ParagraphIndex endParagraphIndex, int fromNestedLevel) {
				return root.GetSubTree(startParagraphIndex, endParagraphIndex, fromNestedLevel);
			}
			TableCellNode OnParagraphInsertedRecursive(ParagraphIndex index, TableCell targetCell) {
				if (targetCell != null) {
					TableCellNode parentNode = OnParagraphInsertedRecursive(index, targetCell.Table.ParentCell);
					TableCellNode result = parentNode.OnParagraphInserted(index, false);
					Debug.Assert(result != null);
					return result;
				}
				else {					
					return root;
				}
			}
		}
		TableCellTree cellTree;
		readonly PieceTable pieceTable;
		#endregion
		public TableCellsManager(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.cellTree = new TableCellTree();
		}
		#region Properties
		protected virtual PieceTable PieceTable { get { return pieceTable; } }
		TableCellTree CellTree { get { return cellTree; } }
		#endregion
		public void Clear() {
			cellTree.Clear();
		}
		#region IDocumentModelStructureChangedListener Members
		void IDocumentModelStructureChangedListener.OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			Debug.Assert(Object.ReferenceEquals(this.PieceTable, pieceTable));
			OnParagraphInserted(sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			Debug.Assert(Object.ReferenceEquals(this.PieceTable, pieceTable));
			OnParagraphRemove(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			Debug.Assert(Object.ReferenceEquals(this.PieceTable, pieceTable));
			OnParagraphMerged(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			Debug.Assert(Object.ReferenceEquals(this.PieceTable, pieceTable));
			OnRunInserted(paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			Debug.Assert(Object.ReferenceEquals(this.PieceTable, pieceTable));
			OnRunRemoved(paragraphIndex, runIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnBeginMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnEndMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			Debug.Assert(Object.ReferenceEquals(this.PieceTable, pieceTable));
			OnRunSplit(paragraphIndex, runIndex, splitOffset);
		}
		void IDocumentModelStructureChangedListener.OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			Debug.Assert(Object.ReferenceEquals(this.PieceTable, pieceTable));
			OnRunJoined(paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			Debug.Assert(Object.ReferenceEquals(this.PieceTable, pieceTable));
			OnRunMerged(paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			Debug.Assert(Object.ReferenceEquals(this.PieceTable, pieceTable));
			OnRunUnmerged(paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
			Debug.Assert(Object.ReferenceEquals(this.PieceTable, pieceTable));
			OnFieldRemoved(fieldIndex);
		}
		void IDocumentModelStructureChangedListener.OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
			Debug.Assert(Object.ReferenceEquals(this.PieceTable, pieceTable));
			OnFieldInserted(fieldIndex);
		}
		#endregion
		protected virtual void OnFieldInserted(int fieldIndex) {
		}
		protected virtual void OnFieldRemoved(int fieldIndex) {
		}
		protected virtual void OnRunUnmerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
		}
		protected virtual void OnRunMerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
		}
		protected virtual void OnRunJoined(ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
		}
		protected virtual void OnRunSplit(ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
		}
		protected virtual void OnRunRemoved(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
		}
		protected virtual void OnRunInserted(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
		}
		protected virtual void OnParagraphRemove(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			CellTree.OnParagraphRemove(paragraphIndex);
		}
		protected virtual void OnParagraphMerged(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			CellTree.OnParagraphRemove(paragraphIndex - 1);
		}
		protected virtual void OnParagraphInserted(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			if (isParagraphMerged)
				paragraphIndex--;
			if (cell != null) {
				if (paragraphIndex > cell.EndParagraphIndex)
					paragraphIndex = cell.EndParagraphIndex;
				if (paragraphIndex < cell.StartParagraphIndex)
					paragraphIndex = cell.StartParagraphIndex;
			}
			CellTree.OnParagraphInserted(paragraphIndex, cell);
		}
		public virtual TableCell GetCell(Paragraph paragraph) {
			return CellTree.GetCellByParagraphIndex(paragraph.Index);
		}
		public virtual void ResetCachedTableLayoutInfo(ParagraphIndex from, ParagraphIndex to) {			
			if (from <= ParagraphIndex.Zero && to >= new ParagraphIndex(pieceTable.Paragraphs.Count - 1)) {
				TableCollection tables = pieceTable.Tables;
				int count = tables.Count;
				for (int i = 0; i < count; i++) {
					tables[i].ResetCachedLayoutInfo();
				}
			}
			else
				CellTree.ResetCachedTableLayoutInfo(from, to);
		}
		public virtual bool IsInCell(Paragraph paragraph) {
			return CellTree.IsParagraphInCell(paragraph.Index);
		}
		public TableCell GetCellByNestingLevel(ParagraphIndex paragraphIndex, int nestingLevel) {
			return CellTree.GetCellByNestingLevel(paragraphIndex, nestingLevel);
		}
		public List<TableCell> GetCellsByParagraphIndex(ParagraphIndex paragraphIndex, int nestedLevel) {
			return CellTree.GetCellsByParagraphIndex(paragraphIndex, nestedLevel);
		}
		public TableCellNode GetCellSubTree(ParagraphIndex startParagraphIndex, ParagraphIndex endParagraphIndex, int fromNestedLevel) {
			return CellTree.GetCellSubTree(startParagraphIndex, endParagraphIndex, fromNestedLevel);
		}
		public void ConvertParagraphsIntoTableCells(TableRow row, ParagraphIndex paragraphIndex, int paragraphCount) {
			ParagraphIndex count = paragraphIndex + paragraphCount;
			for (ParagraphIndex i = paragraphIndex; i < count; i++) {
				TableCell cell = new TableCell(row);
				row.Cells.AddInternal(cell);
				InitializeTableCell(cell, i, i);
			}
		}
		public void RevertParagraphsFromTableCells(TableRow row) {
			TableCellCollection cells = row.Cells;
			int cellCount = cells.Count;
			for (int cellIndex = cellCount - 1; cellIndex >= 0; cellIndex--) {
				TableCell cell = cells[cellIndex];
				CellTree.Remove(cell);
				row.Cells.DeleteInternal(cell);
			}
		}
		public Table CreateTableCore(ParagraphIndex firstParagraphIndex, int rowCount, int cellCount) {
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			TableCell parentCell = GetCell(paragraphs[firstParagraphIndex]);
			Table table = new Table(PieceTable, parentCell, rowCount, cellCount);
			ParagraphIndex paragraphIndex = firstParagraphIndex;
			TableRowCollection rows = table.Rows;
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				TableCellCollection cells = rows[rowIndex].Cells;
				for (int cellIndex = 0; cellIndex < cellCount; cellIndex++) {
					TableCell cell = cells[cellIndex];
					InitializeTableCell(cell, paragraphIndex, paragraphIndex);
					paragraphIndex++;
				}
			}
			PieceTable.Tables.Add(table);
			return table;
		}
		public void RemoveTable(Table table) {
			TableRowCollection rows = table.Rows;
			int rowCount = rows.Count;
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				TableCellCollection cells = rows[rowIndex].Cells;
				int cellCount = cells.Count;
				for (int cellIndex = 0; cellIndex < cellCount; cellIndex++) {
					TableCell cell = cells[cellIndex];
					CellTree.Remove(cell);
				}
			}
			PieceTable.Tables.Remove(table.Index);
		}
		public void InsertTable(Table table) {
			TableRowCollection rows = table.Rows;
			int rowCount = rows.Count;
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				TableCellCollection cells = rows[rowIndex].Cells;
				int cellCount = cells.Count;
				for (int cellIndex = 0; cellIndex < cellCount; cellIndex++) {
					TableCell cell = cells[cellIndex];
					CellTree.Add(cell);
				}
			}
			PieceTable.Tables.Add(table);
		}
		public void InitializeTableCell(TableCell cell, ParagraphIndex startParagraphIndex, ParagraphIndex endParagraphIndex) {
			cell.StartParagraphIndex = startParagraphIndex;
			cell.EndParagraphIndex = endParagraphIndex;
			CellTree.Add(cell);
		}
		public void RemoveTableCell(TableCell cell) {
			CellTree.Remove(cell);
		}
	}
	public class TableCollection : IEnumerable<Table> {
		readonly NotificationCollection<Table> tables;
		public TableCollection() {
			this.tables = new NotificationCollection<Table>();
		}
		#region Properties
		public int Count { get { return tables.Count; } }
		public Table this[int index] { get { return tables[index]; } }
		#region First
		public Table First {
			get {
				if (Count <= 0)
					return null;
				else
					return tables[0];
			}
		}
		#endregion
		#region Last
		public Table Last {
			get {
				if (Count <= 0)
					return null;
				else
					return tables[Count - 1];
			}
		}
		#endregion
		#endregion
		#region Events
		public event CollectionChangedEventHandler<Table> CollectionChanged { add { tables.CollectionChanged += value; } remove { tables.CollectionChanged -= value; } }
		#endregion
		public void Remove(int index) {
			for (int i = index + 1; i < Count; i++)
				tables[i].Index--;
			tables.RemoveAt(index);
		}
		public void RemoveLast() {
			tables.RemoveAt(Count - 1);
		}
		public void Add(Table item) {
			if (item.Index < 0) {
				item.Index = Count;
				tables.Add(item);
				return;
			}
			tables.Insert(item.Index, item);
			for (int i = item.Index + 1; i < Count; i++)
				tables[i].Index++;
		}
		public bool Contains(Table item) {
			return tables.Contains(item);
		}
		public void Clear() {
			tables.Clear();
		}
		public int IndexOf(Table item) {
			return tables.IndexOf(item);
		}
		public void ForEach(Action<Table> action) {
			tables.ForEach(action);
		}
		public IEnumerator<Table> GetEnumerator() {
			return tables.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
	public partial class PieceTable {
		public const int wordMaxColCount = 63;
		internal virtual bool IsTableCellStyleAvailable { get { return false; } }
		internal bool MakeTableWordCompatible(Table table) {
			bool modified = false;
			DocumentModel.BeginUpdate();
			table.Normalize();
			table.NormalizeRows();
			try {
				table.Rows.ForEach(row => {
					int w = 0;
					int oldW;
					int lastColSpan = -1;
					int colsToMerge = 0;
					TableCell last = null;
					row.Cells.ForEach(cell => {
						oldW = w;
						w += cell.ColumnSpan;
						if(w > wordMaxColCount) {
							colsToMerge += cell.ColumnSpan;
							if(oldW < wordMaxColCount) {
								last = cell;
								lastColSpan = wordMaxColCount - oldW;
							}
							else {
								TableCell prev = cell.Previous;
								if(last == null) {
									last = prev;
									lastColSpan = prev.ColumnSpan;
									colsToMerge += prev.ColumnSpan;
								}
							}
						}
					});
					if(last != null) {
						MergeTableCellsHorizontally(last, colsToMerge);
						last.ColumnSpan = lastColSpan;
						modified = true;
					}
				});
				if(modified) {
					table.Normalize();
					table.NormalizeRows();
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
			return modified;
		}
	}
}
