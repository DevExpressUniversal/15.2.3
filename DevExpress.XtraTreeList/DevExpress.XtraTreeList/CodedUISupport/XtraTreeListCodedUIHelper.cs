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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.CodedUISupport;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.ViewInfo;
namespace DevExpress.XtraTreeList.CodedUISupport {
	class XtraTreeListCodedUIHelper : IXtraTreeListCodedUIHelper {
		RemoteObject remoteObject;
		public XtraTreeListCodedUIHelper(RemoteObject remoteObject) {
			this.remoteObject = remoteObject;
		}
		public TreeListElementInfo GetTreeListElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			TreeListElementInfo result = new TreeListElementInfo();
			TreeList control = Control.FromHandle(windowHandle) as TreeList;
			if(control != null) {
				Point clientPoint = new Point(pointX, pointY);
				TreeListHitInfo hitInfo = control.CalcHitInfo(clientPoint);
				if(hitInfo.Node != null)
					if(hitInfo.Node is TreeListAutoFilterNode) {
						result.IsAutoFilterRowElement = true;
						result.NodesHierarchy = new int[0];
					}
					else result.NodesHierarchy = GetNodesHierarchy(hitInfo.Node);
				if(hitInfo.Column != null) {
					result.ColumnName = GetColumnName(hitInfo.Column);
					result.ColumnCaption.Value = GetColumnText(hitInfo.Column);
				}
				switch(hitInfo.HitInfoType) {
					case HitInfoType.Cell:
						result.ElementType = TreeListElements.Cell;
						break;
					case HitInfoType.Column:
						result.ElementType = TreeListElements.ColumnHeader;
						break;
					case HitInfoType.ColumnEdge:
						result.ElementType = TreeListElements.ColumnHeaderEdge;
						break;
					case HitInfoType.ColumnFilterButton:
						result.ElementType = TreeListElements.ColumnFilterButton;
						break;
					case HitInfoType.Row:
						result.ElementType = TreeListElements.Node;
						break;
					case HitInfoType.NodeCheckBox:
						result.ElementType = TreeListElements.NodeCheckBox;
						break;
					case HitInfoType.Button:
						result.ElementType = TreeListElements.NodeButton;
						break;
					case HitInfoType.RowIndicator:
						result.ElementType = TreeListElements.RowIndicator;
						break;
					case HitInfoType.RowIndicatorEdge:
						if(control.OptionsBehavior.ResizeNodes && !control.OptionsBehavior.AutoNodeHeight)
							result.ElementType = TreeListElements.RowIndicatorEdge;
						else result.ElementType = TreeListElements.RowIndicator;
						break;
					case HitInfoType.SelectImage:
						result.ElementType = TreeListElements.NodeSelectImage;
						break;
					case HitInfoType.StateImage:
						result.ElementType = TreeListElements.NodeStateImage;
						break;
					case HitInfoType.FilterPanel:
						result.ElementType = TreeListElements.FilterPanel;
						break;
					case HitInfoType.FilterPanelActiveButton:
						result.ElementType = TreeListElements.FilterPanelActiveButton;
						break;
					case HitInfoType.FilterPanelCloseButton:
						result.ElementType = TreeListElements.FilterPanelCloseButton;
						break;
					case HitInfoType.FilterPanelCustomizeButton:
						result.ElementType = TreeListElements.FilterPanelCustomizeButton;
						break;
					case HitInfoType.FilterPanelMRUButton:
						result.ElementType = TreeListElements.FilterPanelMRUButton;
						break;
					case HitInfoType.FilterPanelText:
						result.ElementType = TreeListElements.FilterPanelText;
						break;
					case HitInfoType.ColumnButton:
						result.ElementType = TreeListElements.ColumnButton;
						break;
					case HitInfoType.AutoFilterRow:
						if(hitInfo.Column != null) {
							result.ElementType = TreeListElements.Cell;
						}
						else result.ElementType = TreeListElements.AutoFilterRow;
						break;
					case HitInfoType.RowFooter:
						if(hitInfo.Column != null)
							result.ElementType = TreeListElements.RowFooterCell;
						else result.ElementType = TreeListElements.RowFooter;
						break;
					case HitInfoType.SummaryFooter:
						if(hitInfo.Column != null)
							result.ElementType = TreeListElements.SummaryFooterCell;
						else result.ElementType = TreeListElements.SummaryFooter;
						break;
					case HitInfoType.FixedLeftDiv:
						result.ElementType = TreeListElements.FixedColumnsDiv;
						result.Side = Side.Left;
						break;
					case HitInfoType.FixedRightDiv:
						result.ElementType = TreeListElements.FixedColumnsDiv;
						result.Side = Side.Right;
						break;
				}
			}
			return result;
		}
		public string GetTreeListElementRectangle(IntPtr windowHandle, TreeListElementInfo elementInfo) {
			TreeList control = Control.FromHandle(windowHandle) as TreeList;
			if(control != null) {
				Rectangle result = GetTreeListElementRectangle(control, elementInfo);
				if(result != Rectangle.Empty)
					return CodedUIUtils.ConvertToString(result);
			}
			return null;
		}
		protected Rectangle GetTreeListElementRectangle(TreeList treeList, TreeListElementInfo elementInfo) {
			TreeListViewInfo viewInfo = treeList.ViewInfo;
			ColumnInfo columnInfo = null;
			RowInfo rowInfo = null;
			TreeListColumn column = GetColumnFromName(treeList, elementInfo.ColumnName);
			TreeListNode node = GetNode(treeList, elementInfo);
			if(column != null)
				columnInfo = viewInfo.ColumnsInfo[column];
			if(elementInfo.IsAutoFilterRowElement)
				rowInfo = viewInfo.AutoFilterRowInfo;
			else if(node != null)
				rowInfo = viewInfo.RowsInfo[node];
			switch(elementInfo.ElementType) {
				case TreeListElements.ColumnHeader:
					if(columnInfo != null)
						return columnInfo.Bounds;
					break;
				case TreeListElements.ColumnHeaderEdge:
					if(columnInfo != null)
						return new Rectangle(columnInfo.Bounds.Right - (ControlUtils.ColumnResizeEdgeSize + ControlUtils.ColumnResizeEdgeSize / 2), columnInfo.Bounds.Y, ControlUtils.ColumnResizeEdgeSize + ControlUtils.ColumnResizeEdgeSize / 2, columnInfo.Bounds.Height);
					break;
				case TreeListElements.Cell:
					if(column != null && rowInfo != null)
						foreach(CellInfo ci in rowInfo.Cells)
							if(ci.Column == column)
								return ci.Bounds;
					break;
				case TreeListElements.Node:
					if(rowInfo != null)
						return rowInfo.Bounds;
					break;
				case TreeListElements.NodeCheckBox:
					if(rowInfo != null)
						return rowInfo.CheckBounds;
					break;
				case TreeListElements.NodeButton:
					if(rowInfo != null)
						return rowInfo.ButtonBounds;
					break;
				case TreeListElements.NodeSelectImage:
					if(rowInfo != null)
						return rowInfo.SelectImageBounds;
					break;
				case TreeListElements.NodeStateImage:
					if(rowInfo != null)
						return rowInfo.StateImageBounds;
					break;
				case TreeListElements.RowIndicator:
					if(rowInfo != null)
						if(rowInfo.IndicatorInfo != null)
							return rowInfo.IndicatorInfo.Bounds;
					break;
				case TreeListElements.RowIndicatorEdge:
					if(rowInfo != null)
						if(rowInfo.IndicatorInfo != null)
							return new Rectangle(rowInfo.IndicatorInfo.Bounds.X, rowInfo.IndicatorInfo.Bounds.Bottom - TreeListViewInfo.RowIndicatorEdgeHeight, rowInfo.IndicatorInfo.Bounds.Width, TreeListViewInfo.RowIndicatorEdgeHeight);
					break;
				case TreeListElements.ColumnButton:
					return viewInfo.ViewRects.IndicatorBounds;
				case TreeListElements.ColumnFilterButton:
					if(columnInfo != null)
						foreach(DrawElementInfo innerElement in columnInfo.InnerElements)
							if(innerElement.ElementInfo is TreeListFilterButtonInfoArgs)
								if(innerElement.Visible)
									return innerElement.ElementInfo.Bounds;
					break;
				case TreeListElements.SummaryFooterCell:
					if(viewInfo.SummaryFooterInfo != null)
						if(column != null) {
							FooterItem footerCell = viewInfo.SummaryFooterInfo[column];
							if(footerCell != null)
								return footerCell.ItemBounds;
						}
					break;
				case TreeListElements.SummaryFooter:
					if(viewInfo.SummaryFooterInfo != null)
						return viewInfo.SummaryFooterInfo.Bounds;
					break;
				case TreeListElements.RowFooter:
				case TreeListElements.RowFooterCell:
					RowFooterInfo rowFooterInfo = GetRowFooterInfo(node);
					if(rowFooterInfo != null) {
						if(elementInfo.ElementType == TreeListElements.RowFooter)
							return rowFooterInfo.Bounds;
						else if(column != null) {
							FooterItem rowFooterItem = rowFooterInfo[column];
							if(rowFooterItem != null)
								return rowFooterItem.ItemBounds;
						}
					}
					break;
				case TreeListElements.FixedColumnsDiv:
					if(elementInfo.Side == Side.Left) {
						Rectangle fixedColumnsRect = viewInfo.ViewRects.FixedLeft;
						fixedColumnsRect.X = fixedColumnsRect.Right - treeList.FixedLineWidth;
						fixedColumnsRect.Width = treeList.FixedLineWidth;
						return fixedColumnsRect;
					}
					else if(elementInfo.Side == Side.Right) {
						Rectangle fixedColumnsRect = viewInfo.ViewRects.FixedRight;
						fixedColumnsRect.X = fixedColumnsRect.X;
						fixedColumnsRect.Width = treeList.FixedLineWidth;
						return fixedColumnsRect;
					}
					break;
				case TreeListElements.FilterPanel:
					if(viewInfo.FilterPanel != null)
						return viewInfo.FilterPanel.Bounds;
					break;
				case TreeListElements.FilterPanelActiveButton:
					if(viewInfo.FilterPanel != null)
						if(viewInfo.FilterPanel.ActiveButtonInfo != null)
							return viewInfo.FilterPanel.ActiveButtonInfo.Bounds;
					break;
				case TreeListElements.FilterPanelCloseButton:
					if(viewInfo.FilterPanel != null)
						if(viewInfo.FilterPanel.CloseButtonInfo != null)
							return viewInfo.FilterPanel.CloseButtonInfo.Bounds;
					break;
				case TreeListElements.FilterPanelCustomizeButton:
					if(viewInfo.FilterPanel != null)
						if(viewInfo.FilterPanel.CustomizeButtonInfo != null)
							return viewInfo.FilterPanel.CustomizeButtonInfo.Bounds;
					break;
				case TreeListElements.FilterPanelMRUButton:
					if(viewInfo.FilterPanel != null)
						if(viewInfo.FilterPanel.MRUButtonInfo != null)
							return viewInfo.FilterPanel.MRUButtonInfo.Bounds;
					break;
				case TreeListElements.FilterPanelText:
					if(viewInfo.FilterPanel != null)
						return viewInfo.FilterPanel.TextBounds;
					break;
			}
			return Rectangle.Empty;
		}
		protected string GetColumnName(TreeListColumn column) {
			if(column.Name != String.Empty) {
				int colsCount = 0;
				foreach(TreeListColumn col in column.TreeList.Columns) {
					if(col == column)
						break;
					if(col.Name == column.Name)
						colsCount++;
				}
				string name = column.Name;
				if(colsCount != 0)
					name = string.Format("{0}[{1}]", name, colsCount);
				return name;
			}
			else return string.Format("{0}[{1}]", column.GetType().Name, column.TreeList.Columns.IndexOf(column));
		}
		protected TreeListColumn GetColumnFromName(TreeList control, string columnName) {
			if(columnName != null)
				foreach(TreeListColumn column in control.Columns)
					if(GetColumnName(column) == columnName)
						return column;
			return null;
		}
		protected string GetColumnText(TreeListColumn column) {
			if(column != null)
				if(string.IsNullOrEmpty(column.Caption))
					return column.FieldName;
				else return column.Caption;
			return null;
		}
		protected int[] GetNodesHierarchy(TreeListNode node) {
			List<int> hierarchy = new List<int>();
			if(node != null) {
				TreeListNode currentNode = node;
				while(true) {
					if(currentNode.ParentNode != null) {
						hierarchy.Insert(0, currentNode.ParentNode.Nodes.IndexOf(currentNode));
						currentNode = currentNode.ParentNode;
					}
					else {
						hierarchy.Insert(0, currentNode.TreeList.Nodes.IndexOf(currentNode));
						break;
					}
				}
			}
			return hierarchy.ToArray();
		}
		protected TreeListNode GetNodeFromHierarchy(TreeList control, int[] nodesHierarchy) {
			TreeListNode result = null;
			if(nodesHierarchy != null) {
				TreeListNodes collection = control.Nodes;
				foreach(int index in nodesHierarchy) {
					if(collection != null && index < collection.Count) {
						result = collection[index];
						if(result.HasChildren)
							collection = result.Nodes;
						else collection = null;
					}
					else {
						result = null;
						break;
					}
				}
			}
			return result;
		}
		protected RowFooterInfo GetRowFooterInfo(TreeListNode node) {
			if(node != null) {
				foreach(RowFooterInfo footerInfo in node.TreeList.ViewInfo.RowsInfo.RowFooters)
					if(footerInfo.Node == node)
						return footerInfo;
			}
			return null;
		}
		public TreeListElementVariableInfo GetTreeListElementVariableInfo(IntPtr windowHandle, TreeListElementInfo elementInfo) {
			TreeListElementVariableInfo result = new TreeListElementVariableInfo();
			TreeList treeList = Control.FromHandle(windowHandle) as TreeList;
			if(treeList != null) {
				TreeListColumn column = GetColumnFromName(treeList, elementInfo.ColumnName);
				TreeListNode node = GetNode(treeList, elementInfo);
				switch(elementInfo.ElementType) {
					case TreeListElements.ColumnHeader:
						if(column != null) {
							result.IsElementFound = true;
							result.VisibleIndex = column.VisibleIndex;
							result.Width = column.Width;
							result.DisplayText = GetColumnText(column);
							result.Fixed = CodedUIUtils.ConvertToString(column.Fixed);
							result.SortOrder = column.SortOrder;
							result.SortIndex = column.SortIndex;
							if(column.FilterInfo != null && !object.ReferenceEquals(column.FilterInfo.FilterCriteria, null))
								result.FilterString = column.FilterInfo.FilterCriteria.LegacyToString();
						}
						break;
					case TreeListElements.Cell:
						if(column != null && node != null) {
							result.IsElementFound = true;
							result.DisplayText = node.GetDisplayText(column);
							result.Value = new ValueStruct(node.GetValue(column));
							result.Focused = node.Focused && treeList.FocusedColumn == column;
							result.Selected = node.Selected;
						}
						break;
					case TreeListElements.Node:
						if(node != null) {
							result.IsElementFound = true;
							result.Expanded = node.Expanded;
							result.Checked = node.Checked;
							result.Selected = node.Selected;
							result.Focused = node.Focused;
							if(node.Nodes != null)
								result.NodesCount = node.Nodes.Count;
							result.Height = treeList.RowHeight;
							result.StateImageIndex = node.StateImageIndex;
							result.Tag = new ValueStruct(node.Tag);
						}
						break;
					case TreeListElements.NodeCheckBox:
						if(node != null) {
							result.IsElementFound = true;
							result.Checked = node.Checked;
						}
						break;
					case TreeListElements.NodeButton:
						if(node != null) {
							result.IsElementFound = true;
							result.Expanded = node.Expanded;
						}
						break;
					case TreeListElements.SummaryFooterCell:
						if(column != null) {
							FooterItem footerItem = treeList.ViewInfo.SummaryFooterInfo[column];
							if(footerItem != null) {
								result.IsElementFound = true;
								result.DisplayText = footerItem.ItemText;
								result.Value = new ValueStruct(treeList.GetSummaryValue(column));
							}
						}
						break;
					case TreeListElements.RowFooterCell:
						RowFooterInfo rowFooterInfo = GetRowFooterInfo(node);
						if(rowFooterInfo != null)
							if(column != null) {
								FooterItem rowFooterItem = rowFooterInfo[column];
								if(rowFooterItem != null) {
									result.IsElementFound = true;
									result.DisplayText = rowFooterItem.ItemText;
									TreeListNodes siblings;
									if(node.ParentNode != null)
										siblings = node.ParentNode.Nodes;
									else siblings = node.TreeList.Nodes;
									result.Value = new ValueStruct(treeList.GetGroupSummaryValue(column, siblings));
								}
							}
						break;
					case TreeListElements.FilterPanel:
					case TreeListElements.FilterPanelActiveButton:
					case TreeListElements.FilterPanelCloseButton:
					case TreeListElements.FilterPanelCustomizeButton:
					case TreeListElements.FilterPanelMRUButton:
					case TreeListElements.FilterPanelText:
						if(treeList.ViewInfo.FilterPanel != null) {
							result.IsElementFound = true;
							result.FilterString = treeList.ViewInfo.FilterPanel.DisplayText;
							result.Checked = treeList.ViewInfo.FilterPanel.FilterActive;
						}
						break;
				}
			}
			return result;
		}
		public void ApplyTreeListElementVariableInfo(IntPtr windowHandle, TreeListElementInfo elementInfo, TreeListElementVariableInfo variableInfo) {
			TreeList treeList = Control.FromHandle(windowHandle) as TreeList;
			if(treeList != null) {
				TreeListColumn column = GetColumnFromName(treeList, elementInfo.ColumnName);
				TreeListNode node = GetNode(treeList, elementInfo);
				treeList.BeginInvoke(new MethodInvoker(delegate() {
					switch(elementInfo.ElementType) {
						case TreeListElements.ColumnHeader:
							if(column != null) {
								if(variableInfo.PropertyForSet == TreeListPropertiesForSet.Width) {
									int index = column.VisibleIndex;
									int cx = variableInfo.Width - column.VisibleWidth;
									if(cx != 0) {
										treeList.ResizeColumn(index, cx, treeList.ViewInfo.ViewRects.Rows.Right);
										treeList.Columns.FireChanged();
									}
								}
								else if(variableInfo.PropertyForSet == TreeListPropertiesForSet.VisibleIndex) {
									if(column.VisibleIndex != variableInfo.VisibleIndex) {
										if(variableInfo.VisibleIndex < 0)
											column.Visible = false;
										else if(variableInfo.VisibleIndex > column.VisibleIndex && column.VisibleIndex >= 0)
											column.VisibleIndex = variableInfo.VisibleIndex + 1;
										else column.VisibleIndex = variableInfo.VisibleIndex;
										column.FireChanged();
									}
								}
								else if(variableInfo.PropertyForSet == TreeListPropertiesForSet.FilterString) {
									column.FilterInfo.Clear();
									treeList.AddFilter(variableInfo.FilterString);
								}
								else if(variableInfo.PropertyForSet == TreeListPropertiesForSet.Fixed) {
									column.Fixed = CodedUIUtils.ConvertFromString<FixedStyle>(variableInfo.Fixed);
								}
							}
							break;
						case TreeListElements.Node:
							if(node != null) {
								if(variableInfo.PropertyForSet == TreeListPropertiesForSet.Expanded) {
									node.Expanded = variableInfo.Expanded;
								}
								else if(variableInfo.PropertyForSet == TreeListPropertiesForSet.Checked) {
									node.Checked = variableInfo.Checked;
									treeList.SetNodeCheckState(node, variableInfo.Checked ? CheckState.Checked : CheckState.Unchecked, treeList.OptionsBehavior.AllowRecursiveNodeChecking);
									treeList.RaiseAfterCheckNode(node);
								}
								else if(variableInfo.PropertyForSet == TreeListPropertiesForSet.Height) {
									treeList.RowHeight = variableInfo.Height;
								}
								else if(variableInfo.PropertyForSet == TreeListPropertiesForSet.StateImageIndex) {
									node.StateImageIndex = variableInfo.StateImageIndex;
								}
							}
							break;
						case TreeListElements.FilterPanelActiveButton:
						case TreeListElements.FilterPanelCloseButton:
						case TreeListElements.FilterPanelCustomizeButton:
						case TreeListElements.FilterPanelMRUButton:
						case TreeListElements.FilterPanelText:
						case TreeListElements.FilterPanel:
							if(variableInfo.PropertyForSet == TreeListPropertiesForSet.Checked)
								treeList.ActiveFilterEnabled = variableInfo.Checked;
							else if(variableInfo.PropertyForSet == TreeListPropertiesForSet.FilterString)
								treeList.ActiveFilterString = variableInfo.FilterString;
							break;
						case TreeListElements.Cell:
							if(node != null && column != null) {
								if(variableInfo.PropertyForSet == TreeListPropertiesForSet.Value)
									node.SetValue(column, CodedUIUtils.GetValue(variableInfo.Value));
							}
							break;
					}
				}));
			}
		}
		public TreeListCustomizationListBoxItemInfo GetTreeListCustomizationListBoxItemInfo(IntPtr windowHandle, int pointX, int pointY) {
			TreeListCustomizationListBox listBox = Control.FromHandle(windowHandle) as TreeListCustomizationListBox;
			if(listBox != null) {
				ColumnInfo ci = listBox.GetColumnInfoByPoint(new Point(pointX, pointY));
				if(ci != null)
					return GetTreeListCustomizationListBoxItemInfo(listBox, ci.Column);
			}
			return new TreeListCustomizationListBoxItemInfo();
		}
		protected TreeListCustomizationListBoxItemInfo GetTreeListCustomizationListBoxItemInfo(TreeListCustomizationListBox listBox, TreeListColumn column) {
			{
				TreeListCustomizationListBoxItemInfo result = new TreeListCustomizationListBoxItemInfo();
				if(column != null) {
					result.ColumnCaption.Value = GetColumnText(column);
					result.ColumnName = GetColumnName(column);
					result.VisibleIndex = column.VisibleIndex;
					result.Width = column.Width;
					result.Index = listBox.Items.IndexOf(column);
					result.Rectangle = CodedUIUtils.ConvertToString(listBox.GetItemRectangle(result.Index));
					result.Fixed = CodedUIUtils.ConvertToString(column.Fixed);
				}
				return result;
			}
		}
		public TreeListCustomizationListBoxItemInfo GetTreeListCustomizationListBoxItemInfo(IntPtr windowHandle, string columnName) {
			TreeListCustomizationListBox listBox = Control.FromHandle(windowHandle) as TreeListCustomizationListBox;
			if(listBox != null) {
				foreach(TreeListColumn column in listBox.Items)
					if(GetColumnName(column) == columnName) {
						return GetTreeListCustomizationListBoxItemInfo(listBox, column);
					}
			}
			return new TreeListCustomizationListBoxItemInfo();
		}
		public string[] GetColumnsNames(IntPtr windowHandle) {
			TreeList treeList = Control.FromHandle(windowHandle) as TreeList;
			if(treeList != null) {
				List<string> columnsNames = new List<string>();
				foreach(TreeListColumn column in treeList.Columns)
					columnsNames.Add(GetColumnName(column));
				return columnsNames.ToArray();
			}
			return null;
		}
		public TreeListElementInfo GetSetFocusedCell(IntPtr windowHandle, TreeListElementInfo focusedCell) {
			TreeList treeList = Control.FromHandle(windowHandle) as TreeList;
			TreeListElementInfo result = new TreeListElementInfo();
			if(treeList != null) {
				if(focusedCell.ElementType == TreeListElements.Cell) {
					treeList.BeginInvoke(new MethodInvoker(delegate() {
						TreeListColumn column = GetColumnFromName(treeList, focusedCell.ColumnName);
						TreeListNode node = GetNode(treeList, focusedCell);
						treeList.FocusedNode = node;
						treeList.FocusedColumn = column;
					}));
				}
				else {
					result.ElementType = TreeListElements.Cell;
					if(treeList.FocusedNode != null)
						if(treeList.FocusedNode is TreeListAutoFilterNode) {
							result.IsAutoFilterRowElement = true;
							result.NodesHierarchy = new int[0];
						}
						else result.NodesHierarchy = GetNodesHierarchy(treeList.FocusedNode);
					if(treeList.FocusedColumn != null) {
						result.ColumnName = GetColumnName(treeList.FocusedColumn);
						result.ColumnCaption.Value = GetColumnText(treeList.FocusedColumn);
					}
				}
			}
			return result;
		}
		public IntPtr GetActiveEditorHandle(IntPtr treeListHandle) {
			TreeList treeList = Control.FromHandle(treeListHandle) as TreeList;
			if(treeList != null) {
				if(treeList.ActiveEditor != null && treeList.ActiveEditor.IsHandleCreated)
					return treeList.ActiveEditor.Handle;
			}
			return IntPtr.Zero;
		}
		public IntPtr GetActiveEditorHandleOrShowIt(IntPtr treeListHandle, TreeListElementInfo cellInfo) {
			TreeList treeList = Control.FromHandle(treeListHandle) as TreeList;
			if(treeList != null) {
				TreeListColumn column = GetColumnFromName(treeList, cellInfo.ColumnName);
				TreeListNode node = GetNode(treeList, cellInfo);
				if(treeList.FocusedNode == node && treeList.FocusedColumn == column && treeList.ActiveEditor != null && treeList.ActiveEditor.IsHandleCreated)
					return treeList.ActiveEditor.Handle;
				else treeList.BeginInvoke(new MethodInvoker(delegate() {
						treeList.FocusedNode = node;
						treeList.FocusedColumn = column;
						treeList.ShowEditor();
					}));
			}
			return IntPtr.Zero;
		}
		protected TreeListNode GetNode(TreeList treeList, TreeListElementInfo elementInfo) {
			if(elementInfo.IsAutoFilterRowElement)
				return treeList.Nodes.AutoFilterNode;
			else if(elementInfo.NodesHierarchy != null && elementInfo.NodesHierarchy.Length > 0)
				return GetNodeFromHierarchy(treeList, elementInfo.NodesHierarchy);
			return null;
		}
		public void MakeTreeListElementVisible(IntPtr windowHandle, TreeListElementInfo elementInfo) {
			TreeList treeList = Control.FromHandle(windowHandle) as TreeList;
			if(treeList != null)
				treeList.BeginInvoke(new MethodInvoker(delegate() {
					TreeListColumn column = GetColumnFromName(treeList, elementInfo.ColumnName);
					TreeListNode node = GetNode(treeList, elementInfo);
					switch(elementInfo.ElementType) {
						case TreeListElements.Node:
						case TreeListElements.NodeButton:
						case TreeListElements.NodeCheckBox:
						case TreeListElements.NodeSelectImage:
						case TreeListElements.NodeStateImage:
						case TreeListElements.RowIndicator:
						case TreeListElements.AutoFilterRow:
						case TreeListElements.RowIndicatorEdge:
							if(node != null)
								treeList.MakeNodeVisible(node);
							break;
						case TreeListElements.ColumnButton:
						case TreeListElements.ColumnFilterButton:
						case TreeListElements.ColumnHeader:
						case TreeListElements.ColumnHeaderEdge:
							if(column != null)
								MakeColumnVisible(column);
							break;
						case TreeListElements.Cell:
							if(node != null && column != null) {
								treeList.MakeNodeVisible(node);
								MakeColumnVisible(column);
							}
							break;
					}
				}));
		}
		protected void MakeColumnVisible(TreeListColumn column) {
			TreeList treeList = column.TreeList;
			if(!column.Visible)
				column.Visible = true;
			if(column.Fixed == FixedStyle.None) {
				int columnLeft = 0;
				for(int i = 0; i < treeList.Columns.Count; i++)
					if(treeList.Columns[i].Visible && treeList.Columns[i].VisibleIndex < column.VisibleIndex && treeList.Columns[i].Fixed == FixedStyle.None)
						columnLeft += treeList.Columns[i].Width;
				if(columnLeft < treeList.LeftCoord ^ columnLeft + column.Width > treeList.LeftCoord + treeList.ViewInfo.ViewRects.Client.Width - treeList.ViewInfo.ViewRects.FixedRight.Width - treeList.ViewInfo.ViewRects.FixedLeft.Width)
					treeList.LeftCoord = columnLeft;
			}
		}
		public TreeListElementInfo[] GetSelectedNodes(IntPtr windowHandle) {
			TreeList treeList = Control.FromHandle(windowHandle) as TreeList;
			if(treeList != null) {
				TreeListElementInfo[] result = new TreeListElementInfo[treeList.Selection.Count];
				for(int i = 0; i < result.Length; i++) {
					result[i].NodesHierarchy = GetNodesHierarchy(treeList.Selection[i]);
					result[i].ElementType = TreeListElements.Node;
				}
				return result;
			}
			return null;
		}
		public void SetSelectedNodes(IntPtr windowHandle, TreeListElementInfo[] nodesInfo) {
			TreeList treeList = Control.FromHandle(windowHandle) as TreeList;
			if(treeList != null) {
				treeList.BeginInvoke(new MethodInvoker(delegate() {
					if(nodesInfo != null && nodesInfo.Length > 0) {
						List<TreeListNode> nodes = new List<TreeListNode>();
						foreach(TreeListElementInfo nodeInfo in nodesInfo) {
							TreeListNode node = GetNodeFromHierarchy(treeList, nodeInfo.NodesHierarchy);
							if(node != null)
								nodes.Add(node);
						}
						treeList.Selection.Set(nodes);
					}
					else treeList.Selection.Clear();
				}));
			}
		}
		public IntPtr GetCustomizationFormHandle(IntPtr treeListHandle) {
			TreeList treeList = Control.FromHandle(treeListHandle) as TreeList;
			if(treeList != null && treeList.CustomizationForm != null && treeList.CustomizationForm.IsHandleCreated)
				return treeList.CustomizationForm.Handle;
			return IntPtr.Zero;
		}
	}
}
