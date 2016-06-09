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

using System.Windows.Forms;
using System;
using DevExpress.XtraEditors;
using DevExpress.Utils.CodedUISupport;
using DevExpress.XtraVerticalGrid;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.XtraVerticalGrid.ViewInfo;
using DevExpress.XtraVerticalGrid.Rows;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraVerticalGrid.CodedUISupport
{
	class XtraVerticalGridCodedUIHelper : IXtraVerticalGridCodedUIHelper
	{
		RemoteObject remoteObject;
		public XtraVerticalGridCodedUIHelper(RemoteObject remoteObject)
		{
			this.remoteObject = remoteObject;
		}
		public VerticalGridElementInfo GetVerticalGridElementFromPoint(IntPtr windowHandle, int pointX, int pointY)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				Point clientPoint = new Point(pointX, pointY);
				VGridHitInfo hitInfo = vGrid.CalcHitInfo(clientPoint);
				switch (hitInfo.HitInfoType)
				{
					case HitInfoTypeEnum.ValueCell:
						return FillElementInfo(vGrid, hitInfo, VerticalGridElements.Cell);
					case HitInfoTypeEnum.HeaderCell:
						return FillElementInfo(vGrid, hitInfo, VerticalGridElements.HeaderCell);
					case HitInfoTypeEnum.ExpandButton:
						return FillElementInfo(vGrid, hitInfo, VerticalGridElements.ExpandButton);
					case HitInfoTypeEnum.RecordValueEdge:
						return FillElementInfo(vGrid, hitInfo, VerticalGridElements.RecordValueEdge);
					case HitInfoTypeEnum.HeaderSeparator:
						return FillElementInfo(vGrid, hitInfo, VerticalGridElements.HeaderSeparator);
					case HitInfoTypeEnum.RowEdge:
						return FillElementInfo(vGrid, hitInfo, VerticalGridElements.RowEdge);
					case HitInfoTypeEnum.MultiEditorCellSeparator:
						return FillElementInfo(vGrid, hitInfo, VerticalGridElements.MultiEditorCellSeparator);
					case HitInfoTypeEnum.Row:
						if (vGrid.IsCategoryRow(hitInfo.Row)) return FillElementInfo(vGrid, hitInfo, VerticalGridElements.CategoryRow);
						return FillElementInfo(vGrid, hitInfo, VerticalGridElements.Row);
					case HitInfoTypeEnum.BandEdge:
						return FillElementInfo(vGrid, hitInfo, VerticalGridElements.BandEdge);
					case HitInfoTypeEnum.HeaderCellImage:
						return FillElementInfo(vGrid, hitInfo, VerticalGridElements.HeaderCellImage);
					case HitInfoTypeEnum.CustomizationForm:
						return FillElementInfo(vGrid, hitInfo, VerticalGridElements.CustomizationForm);
				}
				return FillElementInfo();
			}
			return FillElementInfo();
		}
		public string GetVerticalGridElementRectangle(IntPtr windowHandle, VerticalGridElementInfo elementInfo)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				Rectangle result = GetVerticalGridElementRectangle(vGrid, elementInfo);
				if (result != Rectangle.Empty)
					return CodedUIUtils.ConvertToString(result);
			}
			return null;
		}
		protected const int invalidPosition = -100000;
		protected const int recordValueEdgeWidth = 3;
		protected const int rowEdgeHeight = 5;
		protected const int nonIndex = -1;
		protected const int searchMargin = 3;
		protected const string MultiEditorRow = "MultiEditorRow";
		protected const string EditorRow = "EditorRow";
		protected const string CategoryRow = "CategoryRow";
		protected Rectangle GetVerticalGridElementRectangle(VGridControlBase vGrid, VerticalGridElementInfo elementInfo)
		{
			BaseViewInfo viewInfo = vGrid.ViewInfo;
			if (viewInfo != null)
			{
				BaseRowHeaderInfo headerInfo = null;
				BaseRowViewInfo rowViewInfo = null;
				Rectangle bandRec = Rectangle.Empty;
				BaseRow firstRow = GetFirstRow(vGrid);
				BaseRow row = GetRowByName(vGrid, elementInfo.RowName);
				if (row != null)
				{
					headerInfo = GetHeaderInfoByRow(vGrid, row);
					rowViewInfo = viewInfo[row];
				}
				if (viewInfo.ViewRects.BandRects.Count > 0 && vGrid.RecordCount > 0)
					bandRec = (Rectangle)viewInfo.ViewRects.BandRects[0];
				switch (elementInfo.ElementType)
				{
					case VerticalGridElements.Cell:
						if (row != null)
						{
							RowValueInfo cellValueViewInfo = viewInfo.GetRowValueInfo(row, elementInfo.RecordIndex, elementInfo.CellIndex);
							if (cellValueViewInfo != null) return cellValueViewInfo.Bounds;
						}
						return Rectangle.Empty;
					case VerticalGridElements.HeaderCell:
						if (headerInfo != null)
							return GetHeaderCellInfoByRowCaption(headerInfo, elementInfo.RowCaption).CaptionRect;
						return Rectangle.Empty;
					case VerticalGridElements.ExpandButton:
						if (headerInfo != null) return headerInfo.ExpandButtonRect;
						else return Rectangle.Empty;
					case VerticalGridElements.RecordValueEdge:
						if (firstRow != null && rowViewInfo != null)
						{
							int firstValue_rightSide = rowViewInfo.RowRect.Left + viewInfo.FullRowHeaderWidth + viewInfo.ValueViewWidth + viewInfo.RC.VertLineWidth;
							return new Rectangle(firstValue_rightSide, (bandRec).Top + viewInfo[firstRow].RowRect.Height, recordValueEdgeWidth, bandRec.Height);
						}
						else return Rectangle.Empty;
					case VerticalGridElements.HeaderSeparator:
						if (bandRec != Rectangle.Empty)
						{
							int SeparatorPositionX;
							if (vGrid.LayoutStyle == LayoutViewStyle.MultiRecordView)
								SeparatorPositionX = bandRec.Left + viewInfo.RowHeaderWidth + viewInfo.RC.VertLineWidth + viewInfo.RC.separatorWidth / 2;
							else
							{
								Rectangle bandRectArea = Rectangle.Inflate(bandRec, -viewInfo.RC.VertLineWidth, -viewInfo.RC.HorzLineWidth);
								Rectangle[] rects = GetScaleRowRects(vGrid, bandRectArea);
								SeparatorPositionX = rects[0].Right + viewInfo.RC.VertLineWidth;
							}
							if (firstRow != null && viewInfo[firstRow] != null) return new Rectangle(SeparatorPositionX - 2, bandRec.Top + viewInfo[firstRow].RowRect.Height, 4, bandRec.Height);
						}
						return Rectangle.Empty;
					case VerticalGridElements.RowEdge:
						if (headerInfo != null)
						{
							int left = headerInfo.GetLeftViewPoint(viewInfo.CalcHelper);
							int top;
							Rectangle rowEdge = Rectangle.Empty;
							if (row.Fixed == FixedStyle.Bottom)
							{
								top = headerInfo.IndentBounds.Top - vGrid.OptionsView.FixedLineWidth - rowEdgeHeight / 2;
								rowEdge = new Rectangle(left, top, headerInfo.HeaderRect.Right - left, vGrid.OptionsView.FixedLineWidth / 2 + rowEdgeHeight);
							}
							else
							{
								top = headerInfo.IndentBounds.Bottom - rowEdgeHeight / 2 - viewInfo.RC.HorzLineWidth;
								rowEdge = new Rectangle(left, top, headerInfo.HeaderRect.Right - left, (row.Fixed == FixedStyle.Top) ? vGrid.OptionsView.FixedLineWidth + rowEdgeHeight : rowEdgeHeight);
							}
							return rowEdge;
						}
						else return Rectangle.Empty;
					case VerticalGridElements.MultiEditorCellSeparator:
						if (rowViewInfo != null && elementInfo.SeparatorIndex != nonIndex)
							if (elementInfo.RecordIndex != -1) return (Rectangle)(rowViewInfo as MultiEditorRowViewInfo).SeparatorRects[elementInfo.SeparatorIndex];
							else return (Rectangle)(rowViewInfo as MultiEditorRowViewInfo).HeaderInfo.SeparatorRects[elementInfo.SeparatorIndex];
						else return Rectangle.Empty;
					case VerticalGridElements.Row:
					case VerticalGridElements.CategoryRow:
						if (rowViewInfo != null) return rowViewInfo.RowRect;
						else return Rectangle.Empty;
					case VerticalGridElements.BandEdge:
						if (bandRec != Rectangle.Empty)
							return new Rectangle(bandRec.Right - 2, bandRec.Top, 4, bandRec.Height);
						else return Rectangle.Empty;
					case VerticalGridElements.HeaderCellImage:
						if (headerInfo != null)
							return (headerInfo.CaptionsInfo[elementInfo.CaptionIndex] as RowCaptionInfo).ImageRect;
						return Rectangle.Empty;
				}
			}
			return Rectangle.Empty;
		}
		private static BaseRow GetFirstRow(VGridControlBase vGrid)
		{
			return vGrid.Rows[0];
		}
		protected Rectangle[] GetScaleRowRects(VGridControlBase vGrid, Rectangle rowRect)
		{
			BaseViewInfo viewInfo = vGrid.ViewInfo;
			if (viewInfo != null)
			{
				MethodInfo scaleRowRectsMethod = viewInfo.GetType().GetMethod("ScaleRowRects", BindingFlags.NonPublic | BindingFlags.Instance);
				if (scaleRowRectsMethod != null)
					return scaleRowRectsMethod.Invoke(viewInfo, new object[] { rowRect }) as Rectangle[];
			}
			return null;
		}
		protected static BaseRow GetRowByName(VGridControlBase vGrid, string rowName)
		{
			FindChildRowByNameOperation operation = new FindChildRowByNameOperation(rowName);
			vGrid.RowsIterator.DoOperation(operation);
			return operation.Row;
		}
		protected static Rectangle CheckHitTestSeparatorWidth(Rectangle rec)
		{
			if (rec.Width < 4)
			{
				rec.X -= (4 - rec.Width) / 2;
				rec.Width = 4;
			}
			return rec;
		}
		protected static bool ExtendedFindCellSeparatorContent(Rectangle separatorRec, Point pt)
		{
			Rectangle extendedRect = Rectangle.Empty;
			for (int i = -searchMargin; i < searchMargin; i++)
			{
				for (int j = -searchMargin; j < searchMargin; j++)
				{
					extendedRect = new Rectangle(separatorRec.X + i, separatorRec.Y, separatorRec.Width, separatorRec.Height);
					if (extendedRect.Contains(pt)) return true;
				}
			}
			return false;
		}
		protected static int GetCaptionIndex(VGridControlBase vGrid, VGridHitInfo hitInfo)
		{
			if (hitInfo.HitInfoType == HitInfoTypeEnum.HeaderCellImage)
			{
				BaseRow row = GetRowByName(vGrid, hitInfo.Row.Name);
				if (row != null)
				{
					if (vGrid.IsMultiEditorRow(row))
					{
						for (int i = 0; i < row.HeaderInfo.CaptionsInfo.Count; i++)
						{
							if ((row.HeaderInfo.CaptionsInfo[i] as RowCaptionInfo).ImageRect.Contains(hitInfo.PtMouse)) return i;
						}
						return 0;
					}
					else return 0;
				}
			}
			return nonIndex;
		}
		protected static int GetSeparatorIndex(VGridControlBase vGrid, VGridHitInfo hitInfo)
		{
			if (hitInfo.HitInfoType == HitInfoTypeEnum.MultiEditorCellSeparator)
			{
				BaseRow row = GetRowByName(vGrid, hitInfo.Row.Name);
				if (row != null)
				{
					MultiEditorRowViewInfo rowViewInfo = vGrid.ViewInfo[row] as MultiEditorRowViewInfo;
					for (int i = 0; i < rowViewInfo.SeparatorRects.Count; i++)
					{
						if (ExtendedFindCellSeparatorContent((Rectangle)rowViewInfo.SeparatorRects[i], hitInfo.PtMouse))
							return i;
					}
					Rectangle separatorRec = Rectangle.Empty;
					for (int i = 0; i < rowViewInfo.HeaderInfo.SeparatorRects.Count; i++)
					{
						separatorRec = CheckHitTestSeparatorWidth((Rectangle)rowViewInfo.HeaderInfo.SeparatorRects[i]);
						if (separatorRec.Contains(hitInfo.PtMouse)) return i;
					}
				}
			}
			return nonIndex;
		}
		protected static VerticalGridElementInfo FillElementInfo(VGridControlBase vGrid, VGridHitInfo hitInfo, VerticalGridElements elementType)
		{
			VerticalGridElementInfo elementInfo = new VerticalGridElementInfo();
			elementInfo.ElementType = elementType;
			elementInfo.RecordIndex = hitInfo.RecordIndex;
			elementInfo.CellIndex = hitInfo.CellIndex;
			if (hitInfo.Row != null)
			{
				elementInfo.RowCaption = GetRowCaption(hitInfo);
				elementInfo.RowName = hitInfo.Row.Name;
			}
			elementInfo.SeparatorIndex = GetSeparatorIndex(vGrid, hitInfo);
			elementInfo.CaptionIndex = GetCaptionIndex(vGrid, hitInfo);
			return elementInfo;
		}
		protected static VerticalGridElementInfo FillElementInfo()
		{
			return new VerticalGridElementInfo();
		}
		protected static string GetRowCaption(VGridHitInfo hitInfo)
		{
			string rowCaption = "";
			if (hitInfo.CellIndex != -1)
				rowCaption = hitInfo.Row.GetRowProperties(hitInfo.CellIndex).Caption;
			else
				for (int i = 0; i < hitInfo.Row.RowPropertiesCount; i++)
				{
					rowCaption += hitInfo.Row.GetRowProperties(i).Caption;
					if (i < hitInfo.Row.RowPropertiesCount - 1) rowCaption += " ";
				}
			return rowCaption;
		}
		protected static string GetRowCaption(VGridControlBase vGrid, BaseRow row)
		{
			string rowCaption = "";
			if (vGrid.IsMultiEditorRow(row))
			{
				for (int i = 0; i < row.RowPropertiesCount; i++)
				{
					rowCaption += row.GetRowProperties(i).Caption;
					if (i < row.RowPropertiesCount - 1) rowCaption += " "; ;
				}
			}
			else rowCaption = row.Properties.Caption;
			return rowCaption;
		}
		public VerticalGridElementVariableInfo GetVerticalGridElementVariableInfo(IntPtr windowHandle, VerticalGridElementInfo elementInfo)
		{
			VerticalGridElementVariableInfo result = new VerticalGridElementVariableInfo();
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				BaseRow row = null;
				BaseRowHeaderInfo headerInfo = null;
				row = GetRowByName(vGrid, elementInfo.RowName);
				if (row != null)
				{
					if (vGrid.ViewInfo != null && vGrid.ViewInfo[row] != null)
					{
						headerInfo = GetHeaderInfoByRow(vGrid, row);
					}
				}
				switch (elementInfo.ElementType)
				{
					case VerticalGridElements.Cell:
						{
							if (row != null)
							{
								result.Value = GetCellValue(vGrid, row, elementInfo);
								if (vGrid.IsMultiEditorRow(row))
									result.DisplayText = vGrid.GetCellDisplayText((row as MultiEditorRow), elementInfo.RecordIndex, elementInfo.CellIndex);
								else
									result.DisplayText = vGrid.GetCellDisplayText(row, elementInfo.RecordIndex);
								result.IsElementFound = true;
								result.VisibleIndex = row.VisibleIndex;
								if (vGrid.ViewInfo != null)
								{
									RowValueInfo cellValueViewInfo = vGrid.ViewInfo.GetRowValueInfo(row, elementInfo.RecordIndex, elementInfo.CellIndex);
									if (cellValueViewInfo != null) result.Width = cellValueViewInfo.Bounds.Width;
								}
							}
							return result;
						}
					case VerticalGridElements.HeaderCell:
						{
							if (headerInfo != null)
							{
								result.IsElementFound = true;
								result.DisplayText = elementInfo.RowCaption;
								result.VisibleIndex = row.VisibleIndex;
								result.Width = GetHeaderCellInfoByRowCaption(headerInfo, elementInfo.RowCaption).CaptionRect.Width;
							}
							return result;
						}
					case VerticalGridElements.ExpandButton:
						{
							if (headerInfo != null)
							{
								result.IsElementFound = true;
								GetRowChildInfo(vGrid, row, ref result);
								result.Style = CodedUIUtils.ConvertToString(headerInfo.TreeButtonType);
							}
							return result;
						}
					case VerticalGridElements.HeaderCellImage:
						{
							if (headerInfo != null)
							{
								result.IsElementFound = true;
								GetRowChildInfo(vGrid, row, ref result);
							}
							return result;
						}
					case VerticalGridElements.Row:
					case VerticalGridElements.CategoryRow:
						{
							if (row != null)
							{
								RowValueInfo cellValueViewInfo;
								result.IsElementFound = true;
								result.DisplayText = GetRowCaption(vGrid, row);
								if (vGrid.IsEditorRow(row))
								{
									result.Style = EditorRow;
									result.CellLengths = CodedUIUtils.ConvertToString(vGrid.RecordWidth);
									result.HeaderCellLengths = CodedUIUtils.ConvertToString(vGrid.RowHeaderWidth);
								}
								else if (vGrid.IsMultiEditorRow(row))
								{
									result.Style = MultiEditorRow;
									foreach (MultiEditorRowProperties rowProp in (row as MultiEditorRow).PropertiesCollection)
									{
										cellValueViewInfo = vGrid.ViewInfo.GetRowValueInfo(row, 0, rowProp.CellIndex);
										if (cellValueViewInfo != null)
											result.CellLengths += CodedUIUtils.ConvertToString(cellValueViewInfo.Bounds.Width) + ",";
									}
									if (result.CellLengths != String.Empty && result.CellLengths != null) result.CellLengths = result.CellLengths.Substring(0, result.CellLengths.Length - 1);
									if (headerInfo != null)
									{
										foreach (RowCaptionInfo rc in headerInfo.CaptionsInfo)
											if (rc != null) result.HeaderCellLengths += CodedUIUtils.ConvertToString(rc.CaptionRect.Width) + ",";
										if (result.CellLengths != String.Empty && result.CellLengths != null) result.HeaderCellLengths = result.HeaderCellLengths.Substring(0, result.HeaderCellLengths.Length - 1);
									}
								}
								else if (vGrid.IsCategoryRow(row))
								{
									result.Style = CategoryRow;
									if (headerInfo != null)
										result.HeaderCellLengths = CodedUIUtils.ConvertToString((headerInfo.CaptionsInfo[0] as RowCaptionInfo).CaptionRect.Width);
								}
								result.Fixed = CodedUIUtils.ConvertToString(row.Fixed);
								if (row.ParentRow != null) result.ParentRow = row.ParentRow.Name;
								else result.ParentRow = vGrid.Name;
								if (vGrid.ViewInfo[row] != null)
									result.Height = vGrid.ViewInfo[row].RowRect.Height;
								if (row.Visible) result.VisibleIndex = row.VisibleIndex;
								GetRowChildInfo(vGrid, row, ref result);
							}
							return result;
						}
					case VerticalGridElements.MultiEditorCellSeparator:
						{
							if (row != null)
							{
								result.IsElementFound = true;
								result.Style = (row as MultiEditorRow).SeparatorKind.ToString();
								result.DisplayText = CodedUIUtils.ConvertToString((row as MultiEditorRow).SeparatorString);
							}
							return result;
						}
				}
				return result;
			}
			return result;
		}
		protected static void GetRowChildInfo(VGridControlBase vGrid, BaseRow row, ref VerticalGridElementVariableInfo elementInfo)
		{
			if (row != null)
			{
				int childrenCount;
				elementInfo.Expanded = row.Expanded;
				VGridRows childRows = GetRealChildInfo(vGrid, row, out childrenCount);
				elementInfo.ChildrenCount = childrenCount;
			}
		}
		protected static VGridRows GetRealChildInfo(VGridControlBase vGrid, BaseRow row, out int childrenCount)
		{
			VGridRows childRows = new VGridRows(vGrid);
			if (!row.Expanded && row.Visible)
			{
				vGrid.BeginInvoke(new MethodInvoker(delegate()
				{
					vGrid.BeginUpdate();
					row.Expanded = true;
					vGrid.EndUpdate();
				}));
				childrenCount = row.ChildRows.Count;
				for (int i = 0; i < row.ChildRows.Count; i++)
					childRows.Add(row.ChildRows[i] as BaseRow);
				vGrid.BeginInvoke(new MethodInvoker(delegate()
				{
					vGrid.BeginUpdate();
					row.Expanded = false;
					vGrid.EndUpdate();
				}));
			}
			else childrenCount = row.ChildRows.Count;
			return childRows;
		} 
		protected static ValueStruct GetCellValue(VGridControlBase vGrid, BaseRow row, VerticalGridElementInfo elementInfo)
		{
			ValueStruct cellValue = new ValueStruct();
			if (vGrid.IsMultiEditorRow(row))
				cellValue = new ValueStruct(vGrid.GetCellValue((row as MultiEditorRow), elementInfo.RecordIndex, elementInfo.CellIndex));
			else
				cellValue = new ValueStruct(vGrid.GetCellValue(row, elementInfo.RecordIndex));
			return cellValue;
		}
		public string GetFocusedCellValueAsString(IntPtr windowHandle)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				BaseRow row = vGrid.FocusedRow;
				if (row != null)
				{
					if (vGrid.IsMultiEditorRow(row))
						return CodedUIUtils.ConvertToString(vGrid.GetCellValue((row as MultiEditorRow), vGrid.FocusedRecord, vGrid.FocusedRecordCellIndex));
					else
						return CodedUIUtils.ConvertToString(vGrid.GetCellValue(row, vGrid.FocusedRecord));
				}
			}
			return null;
		}
		public bool GetCellValue(IntPtr windowHandle, string rowName, int recordIndex, int cellIndex, out string valueAsString, out string valueType)
		{
			object value;
			valueAsString = null;
			valueType = null;
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				BaseRow row = GetRowByName(vGrid, rowName);
				if (row != null)
				{
					if (vGrid.IsMultiEditorRow(row)) value = vGrid.GetCellValue((row as MultiEditorRow), vGrid.FocusedRecord, vGrid.FocusedRecordCellIndex);
					else value = vGrid.GetCellValue(row, vGrid.FocusedRecord);
					valueType = value.GetType().FullName;
					valueAsString = CodedUIUtils.ConvertToString(value);
					return true;
				}
				else return false;
			}
			return false;
		}
		protected static void SetCellValue(VGridControlBase vGrid, BaseRow row, VerticalGridElementInfo elementInfo, object value)
		{
			if (vGrid.IsMultiEditorRow(row))
				vGrid.SetCellValue((row as MultiEditorRow), elementInfo.RecordIndex, elementInfo.CellIndex, value);
			else
				vGrid.SetCellValue(row, elementInfo.RecordIndex, value);
		}
		public bool GetInnerElementInformationForCustomizationListBoxItem(IntPtr listBoxHandle, int itemIndex, out IntPtr gridHandle, out VerticalGridElementInfo elementInfo)
		{
			gridHandle = IntPtr.Zero;
			elementInfo = new VerticalGridElementInfo();
			VGridCustomizationForm.RowsListBox customListBox = Control.FromHandle(listBoxHandle) as VGridCustomizationForm.RowsListBox;
			if (itemIndex < customListBox.Items.Count)
			{
				BaseRow vGridRow = (customListBox.Items[itemIndex] as BaseRowHeaderInfo).Row;
				if (vGridRow != null)
				{
					gridHandle = vGridRow.Grid.Handle;
					VGridControlBase vGrid = Control.FromHandle(gridHandle) as VGridControlBase;
					if (vGrid != null)
					{
						elementInfo.RowName = vGridRow.Name;
						if (vGrid.IsCategoryRow(vGridRow)) elementInfo.ElementType = VerticalGridElements.CategoryRow;
						else elementInfo.ElementType = VerticalGridElements.Row;
						elementInfo.RowCaption = GetRowCaption(vGrid, vGridRow);
						return true;
					}
				}
			}
			return false;
		}
		public int GetCustomizationListBoxItemIndex(IntPtr WindowHandle, string innerElementName)
		{
			VGridControlBase vGrid = null;
			vGrid = Control.FromHandle(WindowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				BaseRow vGridRow = GetRowByName(vGrid, innerElementName);
				if (vGridRow != null)
				{
					if (vGrid.IsCategoryRow(vGridRow))
					{
						for (int i = 0; i < vGrid.CustomizationForm.CategoryHeaders.Count; i++)
							if ((vGrid.CustomizationForm.CategoryHeaders[i] as BaseRowHeaderInfo).Row == vGridRow)
								return i;
					}
					else for (int i = 0; i < vGrid.CustomizationForm.RowHeaders.Count; i++)
							if ((vGrid.CustomizationForm.RowHeaders[i] as BaseRowHeaderInfo).Row == vGridRow)
								return i;
				}
			}
			else
			{
				VGridCustomizationForm custForm = null;
				IntPtr gridHandle = IntPtr.Zero;
				DevExpress.XtraVerticalGrid.Rows.VGridCustomizationForm.RowsListBox listBox = Control.FromHandle(WindowHandle) as DevExpress.XtraVerticalGrid.Rows.VGridCustomizationForm.RowsListBox;
				if (listBox != null)
					custForm = listBox.Parent.Parent.Parent as VGridCustomizationForm;
				if (custForm != null) vGrid = custForm.GetType().GetProperty("Grid", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(custForm, new object[] { }) as VGridControlBase;
				if (vGrid != null)
				{
					BaseRow vGridRow = GetRowByName(vGrid, innerElementName);
					if (vGridRow != null)
					{
						if (vGrid.IsCategoryRow(vGridRow))
						{
							for (int i = 0; i < vGrid.CustomizationForm.CategoryHeaders.Count; i++)
								if ((vGrid.CustomizationForm.CategoryHeaders[i] as BaseRowHeaderInfo).Row == vGridRow)
									return i;
						}
						else for (int i = 0; i < vGrid.CustomizationForm.RowHeaders.Count; i++)
								if ((vGrid.CustomizationForm.RowHeaders[i] as BaseRowHeaderInfo).Row == vGridRow)
									return i;
					}
				}
				return -1;
			}
			return -1;
		}
		public PropertyDescriptionVariableInfo GetPropertyDescriptionVariableInfo(IntPtr windowHandle)
		{
			PropertyDescriptionVariableInfo result = new PropertyDescriptionVariableInfo();
			PropertyDescriptionControl control = Control.FromHandle(windowHandle) as PropertyDescriptionControl;
			if (control != null && control.PropertyGrid != null && control.PropertyGrid.FocusedRow != null)
			{
				PropertyDescriptor pd = control.PropertyGrid.GetPropertyDescriptor(control.PropertyGrid.FocusedRow);
				if (pd != null)
				{
					result.IsElementFound = true;
					result.ParentCategory = pd.Category;
					result.DisplayedProperty = pd.DisplayName;
					result.Description = pd.Description;
				}
			}
			return result;
		}
		public IntPtr GetCustomizationFormHandleOrShowIt(IntPtr windowHandle)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				if (vGrid.CustomizationForm == null)
				{
					vGrid.BeginInvoke(new MethodInvoker(delegate() { vGrid.CustomizationForm.Show(); }));
					return IntPtr.Zero;
				}
				else return vGrid.CustomizationForm.Handle;
			}
			else return IntPtr.Zero;
		}
		public bool GetVerticalGridFocus(IntPtr windowHandle, out string rowName, out int recordIndex, out int cellIndex)
		{
			rowName = null;
			recordIndex = cellIndex = -1;
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				if (vGrid.FocusedRow != null)
				{
					rowName = vGrid.FocusedRow.Name;
					recordIndex = vGrid.FocusedRecord;
					cellIndex = vGrid.FocusedRecordCellIndex;
					return true;
				}
				return false;
			}
			return false;
		}
		public void SetVerticalGridFocus(IntPtr windowHandle, string rowName, int recordIndex, int cellIndex)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				vGrid.BeginInvoke(new MethodInvoker(delegate()
				{
					BaseRow row = GetRowByName(vGrid, rowName);
					if (row != null)
					{
						vGrid.MakeRowVisible(row);
						vGrid.FocusedRow = row;
						if (recordIndex != -1 && cellIndex != -1)
						{
							vGrid.MakeRecordVisible(recordIndex);
							vGrid.FocusedRecord = recordIndex;
							vGrid.FocusedRecordCellIndex = cellIndex;
						}
					}
				}));
			}
		}
		public void UpdateScrollBars(IntPtr windowHandle, int scrollCount, bool isVert)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				if (isVert) vGrid.VertScroll(scrollCount);
				else vGrid.HorzScroll(scrollCount);
			}
		}
		public IntPtr GetVerticalGridActiveEditorHandle(IntPtr windowHandle)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				if (vGrid.ActiveEditor != null)
					return vGrid.ActiveEditor.Handle;
			}
			return IntPtr.Zero;
		}
		public IntPtr GetVerticalGridActiveEditorHandleOrSetActiveEditor(IntPtr windowHandle, string rowName, int recordIndex, int cellIndex)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				BaseRow row = GetRowByName(vGrid, rowName);
				if (row == null) return IntPtr.Zero;
				if (vGrid.FocusedRow == row && vGrid.FocusedRecord == recordIndex && vGrid.FocusedRecordCellIndex == cellIndex && vGrid.ActiveEditor == null)
				{
					vGrid.BeginInvoke(new MethodInvoker(delegate() { vGrid.ShowEditor(); }));
				}
				else if (vGrid.FocusedRow != row || vGrid.FocusedRecord != recordIndex || vGrid.FocusedRecordCellIndex != cellIndex || vGrid.ActiveEditor == null)
				{
					vGrid.BeginInvoke(new MethodInvoker(
		delegate()
		{
			vGrid.CloseEditor();
			vGrid.MakeRowVisible(row);
			vGrid.MakeRecordVisible(recordIndex);
			vGrid.FocusedRow = row;
			vGrid.FocusedRecord = recordIndex;
			vGrid.FocusedRecordCellIndex = cellIndex;
			vGrid.ShowEditor();
		})
	);
				}
				else if (vGrid.ActiveEditor != null)
					return vGrid.ActiveEditor.Handle;
			}
			return IntPtr.Zero;
		}
		public string GetRowName(IntPtr windowHandle, int rowIndex, string categoryName)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				BaseRow category = GetRowByName(vGrid, categoryName);
				if (category != null)
					foreach (BaseRow row in category.ChildRows)
						if (row.Index == rowIndex) return row.Name;
			}
			return null;
		}
		public string GetCategoryName(IntPtr windowHandle, int categoryIndex)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
				foreach (BaseRow row in vGrid.Rows)
					if (row.Index == categoryIndex)
						return row.Name;
			return null;
		}
		protected static BaseRowHeaderInfo GetHeaderInfoByRow(VGridControlBase vGrid, BaseRow row)
		{
			if (vGrid.ViewInfo[row] != null) return vGrid.ViewInfo[row].HeaderInfo;
			return null;
		}
		protected static RowCaptionInfo GetHeaderCellInfoByRowCaption(BaseRowHeaderInfo headerInfo, string rowCaption)
		{
			foreach (RowCaptionInfo captionInfo in headerInfo.CaptionsInfo)
				if (captionInfo.Caption == rowCaption) return captionInfo;
			return null;
		}
		public string GetDragRowEffect(IntPtr windowHandle, string sourceRowName, ref string destRowName, int oldRowVisibleIndex, string oldParentRowName, out bool canMoveRow, out bool clearDragAction)
		{
			canMoveRow = false;
			clearDragAction = false;
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				RowDragEffect effect = RowDragEffect.None;
				BaseRow dest = null;
				int sourceIndex = 0;
				BaseRow source = GetRowByName(vGrid, sourceRowName);
				if (source == null) return CodedUIUtils.ConvertToString(effect);
				if (vGrid.CustomizationForm != null && source.OptionsRow.AllowMoveToCustomizationForm == true)
				{
					if (vGrid.IsCategoryRow(source))
					{
						for (int i = 0; i < vGrid.CustomizationForm.CategoryHeaders.Count; i++)
							if ((vGrid.CustomizationForm.CategoryHeaders[i] as BaseRowHeaderInfo).Row == source)
							{
								destRowName = null;
								canMoveRow = true;
								if (!source.Visible) return CodedUIUtils.ConvertToString(RowDragEffect.InsertBefore);
							}
					}
					else for (int i = 0; i < vGrid.CustomizationForm.RowHeaders.Count; i++)
							if ((vGrid.CustomizationForm.RowHeaders[i] as BaseRowHeaderInfo).Row == source)
							{
								destRowName = null;
								canMoveRow = true;
								if (!source.Visible) return CodedUIUtils.ConvertToString(RowDragEffect.InsertBefore);
							}
					if (vGrid.Rows[source.Name].VisibleIndex == -1 && oldRowVisibleIndex == -1 && oldParentRowName != source.ParentRow.Name)
					{
						dest = source.ParentRow;
						effect = RowDragEffect.MoveChild; destRowName = dest.Name;
						canMoveRow = true;
						return CodedUIUtils.ConvertToString(effect);
					}
				}
				if (destRowName == vGrid.Name)
				{
					int sourceRowIndex = vGrid.Rows[source.Name].Index;
					if (vGrid.Rows[sourceRowIndex+1] == null) effect = RowDragEffect.MoveToEnd;
					else if (source.VisibleIndex == 0 || (vGrid.FixedTopRows != null && vGrid.FixedTopRows.Count != 0 && vGrid.FixedTopRows[vGrid.FixedTopRows.Count - 1].VisibleIndex == source.VisibleIndex-1)) { effect = RowDragEffect.InsertBefore; destRowName = vGrid.Rows[++sourceRowIndex].Name; }
					else { effect = RowDragEffect.InsertAfter; if (vGrid.Rows[sourceRowIndex-1] != null) destRowName = vGrid.Rows[--sourceRowIndex].Name; }
					canMoveRow = CanBeginDrag(vGrid, source);
					if (canMoveRow && ClearDragAction(vGrid, source, oldRowVisibleIndex, oldParentRowName)) clearDragAction = true;
				}
				else if (destRowName != null)
				{
					dest = GetRowByName(vGrid, destRowName);
					if (dest == null) return CodedUIUtils.ConvertToString(effect);
					else
					{
						canMoveRow = CanMoveRow(vGrid, source, dest, effect);
						if (canMoveRow)
						{
							if (dest.ChildRows.Count == 1) effect = RowDragEffect.MoveChild;
							else
							{
								sourceIndex = dest.ChildRows.IndexOf(source);
								if (sourceIndex == dest.ChildRows.Count - 1) { effect = RowDragEffect.InsertAfter; if (vGrid.Rows[sourceIndex-1] != null) destRowName = dest.ChildRows[--sourceIndex].Name; }
								else if (sourceIndex < dest.ChildRows.Count - 1) { effect = RowDragEffect.InsertBefore; if (vGrid.Rows[sourceIndex+1] != null) destRowName = dest.ChildRows[++sourceIndex].Name; }
							}
						}
						if (canMoveRow && ClearDragAction(vGrid, source, oldRowVisibleIndex, oldParentRowName)) clearDragAction = true;
					}
				}
				return CodedUIUtils.ConvertToString(effect);
			}
			return null;
		}
		protected static bool CanMoveRow(VGridControlBase vGrid, BaseRow source, BaseRow dest, RowDragEffect effect)
		{
			if (!CanBeginDrag(vGrid, source)) return false;
			if (source != null)
			{
				if (source == dest)
					return false;
				if (source == null || source.Grid != vGrid)
					return false;
				if (dest != null && dest.Fixed != source.Fixed)
					return false;
				if (dest != null && dest.Grid != vGrid)
					return false;
				if (dest != null && source.HasAsChild(dest))
					return false;
				if (effect == RowDragEffect.MoveChild)
				{
					source.Visible = true;
					return true;
				}
				return true;
			}
			return false;
		}
		protected static bool ClearDragAction(VGridControlBase vGrid, BaseRow source, int oldRowVisibleIndex, string oldParentRowName)
		{
			if (oldRowVisibleIndex == vGrid.Rows[source.Name].VisibleIndex)
			{
				if (vGrid.Rows[source.Name].ParentRow == null && oldParentRowName == vGrid.Name) return true;
				else if (vGrid.Rows[source.Name].ParentRow != null && oldParentRowName == vGrid.Rows[source.Name].ParentRow.Name) return true;
			}
			return false;
		}
		protected static bool CanBeginDrag(VGridControlBase vGrid, BaseRow row)
		{
			if (vGrid.CustomizationForm == null)
			{
				if (!vGrid.OptionsBehavior.DragRowHeaders) return false;
				if (!row.OptionsRow.AllowMove) return false;
			}
			return true;
		}
		protected void AllowMoveToCustomizationFormMethod(VGridControlBase vGrid, BaseRow row)
		{
			MethodInfo allowMoveToCustomizationFormMethod = vGrid.GetType().GetMethod("OpenRightCustomizationTabPage", BindingFlags.NonPublic | BindingFlags.Instance);
			if (allowMoveToCustomizationFormMethod != null)
				allowMoveToCustomizationFormMethod.Invoke(vGrid, new object[] { row });
			return;
		}
		public void OpenRightCustomizationTabPage(IntPtr windowHandle, string rowName)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				vGrid.CloseEditor();
				BaseRow row = GetRowByName(vGrid, rowName);
				if (row != null && vGrid.CustomizationForm != null)
					vGrid.BeginInvoke(new MethodInvoker(delegate() { AllowMoveToCustomizationFormMethod(vGrid, row); }));
			}
		}
		public void DragRowAction(IntPtr windowHandle, string sourceRowName, string rowDestName, string rowParentName, string dragRowEffect)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				vGrid.BeginInvoke(new MethodInvoker(delegate()
				{
					DragRowAction(vGrid, sourceRowName, rowDestName, rowParentName, dragRowEffect);
				}));
			}
		}
		protected static void DragRowAction(VGridControlBase vGrid, string sourceRowName, string rowDestName, string rowParentName, string dragRowEffect)
		{
			vGrid.CloseEditor();
			BaseRow dest = null;
			BaseRow parentRow = null;
			VGridHitTest ht = null;
			BaseRow source = GetRowByName(vGrid, sourceRowName);
			if (source != null && CanBeginDrag(vGrid, source) && dragRowEffect != null)
			{
				if (rowDestName != null)
				{
					if (rowParentName != vGrid.Name)
					{
						parentRow = GetRowByName(vGrid, rowParentName);
					   if (dragRowEffect != CodedUIUtils.ConvertToString(RowDragEffect.MoveChild) && rowDestName != rowParentName) parentRow.Expanded = true;
					}
					dest = GetRowByName(vGrid, rowDestName);
					if (vGrid.ViewInfo != null && vGrid.ViewInfo[dest] != null) ht = vGrid.ViewInfo.CalcHitTest(vGrid.ViewInfo[dest].RowRect.Location);
					vGrid.BeginUpdate();
					try
					{
						vGrid.MoveRow(source, dest, CodedUIUtils.ConvertFromString<RowDragEffect>(dragRowEffect));
						if (ht != null) vGrid.Handler.CheckPreserveChildRows(source, ht.HitInfoType);
						source.Visible = true;
					}
					finally
					{
						vGrid.EndUpdate();
					}
				}
				else
					if (dragRowEffect == CodedUIUtils.ConvertToString(RowDragEffect.InsertBefore))
					{
						vGrid.ContainerHelper.BeginAllowHideException();
						try
						{
							source.Visible = false;
						}
						finally
						{
							vGrid.ContainerHelper.EndAllowHideException();
						}
					}
					else if (dragRowEffect == CodedUIUtils.ConvertToString(RowDragEffect.MoveToEnd))
					{
						vGrid.MoveRow(source, dest, CodedUIUtils.ConvertFromString<RowDragEffect>(dragRowEffect));
						source.Visible = true;
					}
			}
		}
		public void ApplyVerticalGridElementVariableInfo(IntPtr windowHandle, VerticalGridElementInfo elementInfo, VerticalGridElementVariableInfo variableInfo)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				vGrid.BeginInvoke(new MethodInvoker(delegate()
				{
					ApplyVerticalGridElementVariableInfo(vGrid, elementInfo, variableInfo);
				}));
			}
		}
		protected void ApplyVerticalGridElementVariableInfo(VGridControlBase vGrid, VerticalGridElementInfo elementInfo, VerticalGridElementVariableInfo variableInfo)
		{
			BaseRow row = null;
			row = GetRowByName(vGrid, elementInfo.RowName);
			switch (elementInfo.ElementType)
			{
				case VerticalGridElements.Row:
				case VerticalGridElements.CategoryRow:
				case VerticalGridElements.HeaderCell:
					if (row != null)
					{
						if (variableInfo.Expanded != row.Expanded)
						{
							row.Expanded = variableInfo.Expanded;
							if (row.ChildRows.Count < 0) row.ParentRow.Expanded = variableInfo.Expanded;
						}
						if (variableInfo.Fixed != row.Fixed.ToString())
							row.Fixed = CodedUIUtils.ConvertFromString<FixedStyle>(variableInfo.Fixed);
						if (vGrid.ViewInfo[row] != null && vGrid.ViewInfo[row].RowRect.Height != variableInfo.Height)
							row.Height = variableInfo.Height;
						try
						{
							string[] cellLengthsAsStrings = variableInfo.CellLengths.Split(',');
							int[] cellLengths = new int[cellLengthsAsStrings.Length];
							for (int i = 0; i < cellLengthsAsStrings.Length; i++)
								cellLengths[i] = CodedUIUtils.ConvertFromString<int>(cellLengthsAsStrings[i]);
							string[] headerCellLengthsAsStrings = variableInfo.HeaderCellLengths.Split(',');
							int[] headerCellLengths = new int[headerCellLengthsAsStrings.Length];
							for (int i = 0; i < headerCellLengthsAsStrings.Length; i++)
								headerCellLengths[i] = CodedUIUtils.ConvertFromString<int>(headerCellLengthsAsStrings[i]);
							foreach (MultiEditorRowProperties rowProp in (row as MultiEditorRow).PropertiesCollection)
							{
								rowProp.CellWidth = cellLengths[rowProp.CellIndex];
								rowProp.Width = headerCellLengths[rowProp.CellIndex];
							}
						}
						catch
						{
							return;
						}
					}
					break;
				case VerticalGridElements.Cell:
					if (row != null)
					{
						ValueStruct cellValue = GetCellValue(vGrid, row, elementInfo);
						if (variableInfo.Value.ValueAsString != cellValue.ValueAsString || variableInfo.Value.ValueTypeName != cellValue.ValueTypeName)
						{
							object newValue = variableInfo.Value.ValueAsString;
							if (variableInfo.Value.ValueTypeName != null)
							{
								newValue = CodedUIUtils.ConvertFromString(variableInfo.Value.ValueAsString, variableInfo.Value.ValueTypeName);
							}
							vGrid.BeginInvoke(new MethodInvoker(delegate()
							{
								SetCellValue(vGrid, row, elementInfo, newValue);
							}));
						}
					}
					break;
			}
		}
		public void PostEditorValue(IntPtr windowHandle)
		{
			VGridControlBase vGrid = Control.FromHandle(windowHandle) as VGridControlBase;
			if (vGrid != null)
			{
				vGrid.BeginInvoke(new MethodInvoker(delegate()
				{
					vGrid.PostEditor();
				}));
			}
		}
	}
}
public class FindChildRowByNameOperation : DevExpress.XtraVerticalGrid.Rows.FindRowByNameOperation
{
	public FindChildRowByNameOperation(string name):base(name) {}
	internal override bool NeedsVisitUnloadedRows { get { return true; } }
}
