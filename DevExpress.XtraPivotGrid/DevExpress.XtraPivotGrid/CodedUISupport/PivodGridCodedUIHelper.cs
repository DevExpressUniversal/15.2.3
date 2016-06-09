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
using System.Windows.Forms;
using DevExpress.Utils.CodedUISupport;
using System.Drawing;
using DevExpress.XtraPivotGrid.ViewInfo;
using System.Reflection;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPivotGrid.Customization;
namespace DevExpress.XtraPivotGrid.CodedUISupport {
	class PivotGridCodedUIHelper : IPivotGridCodedUIHelper {
		RemoteObject remoteObject;
		public PivotGridCodedUIHelper(RemoteObject remoteObject) {
			this.remoteObject = remoteObject;
		}
		public PivotGridElementInfo GetPivotGridElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			PivotGridElementInfo result = new PivotGridElementInfo();
			PivotGridControl pivot = Control.FromHandle(windowHandle) as PivotGridControl;
			PivotGridViewInfo viewInfo = GetPivotViewInfo(pivot);
			PivotVisualItems visualItems = GetPivotVisualItems(pivot);
			if(pivot != null) {
				Point clientPoint = new Point(pointX, pointY);
				PivotGridHitInfo hitInfo = pivot.CalcHitInfo(clientPoint);
				switch(hitInfo.HitTest) {
					case PivotGridHitTest.Cell:
						FillCellElementInfo(pivot, hitInfo.CellInfo, ref result);
						break;
					case PivotGridHitTest.HeadersArea:
						result.Area = (PivotAreaType)((int)hitInfo.HeadersAreaInfo.Area);
						if(hitInfo.HeaderField != null) {
							if(hitInfo.HeadersAreaInfo.HeaderHitTest == PivotGridHeaderHitTest.Filter)
								result.ElementType = PivotGridElements.FieldHeaderFilterButton;
							else {
								result.ElementType = PivotGridElements.FieldHeader;
								PivotHeadersViewInfoBase headersInfo = GetHeadersInfo(viewInfo, result.Area);
								PivotHeaderViewInfoBase headerInfo = null;
								for(int i = 0; i < headersInfo.ChildCount; i++)
									if(headersInfo[i].Field.ExpressionFieldName == hitInfo.HeaderField.ExpressionFieldName)
										headerInfo = headersInfo[i];
								if(headerInfo != null) {
									OpenCloseButtonInfoArgs openCloseButtonInfo = null;
									PropertyInfo openCloseButtonInfoProperty = headerInfo.GetType().GetProperty("OpenCloseButtonInfo", BindingFlags.NonPublic | BindingFlags.Instance);
									if(openCloseButtonInfoProperty != null)
										openCloseButtonInfo = openCloseButtonInfoProperty.GetValue(headerInfo, new object[] { }) as OpenCloseButtonInfoArgs;
									if(openCloseButtonInfo != null)
										if(openCloseButtonInfo.Bounds.Contains(clientPoint))
											result.ElementType = PivotGridElements.FieldHeaderExpandButton;
								}
							}
							result.FieldName = hitInfo.HeaderField.ExpressionFieldName;
							result.DisplayText = hitInfo.HeaderField.HeaderDisplayText;
							result.AreaIndex = GetFieldAreaIndex(pivot, hitInfo.HeaderField);
							result.Visible = hitInfo.HeaderField.Visible;
						}
						else result.ElementType = PivotGridElements.HeadersArea;
						break;
					case PivotGridHitTest.Value:
						if(hitInfo.ValueInfo != null) {
							PivotFieldValueItem item = hitInfo.ValueInfo.Item;
							if(hitInfo.ValueInfo.ValueHitTest == PivotGridValueHitTest.ExpandButton)
								result.ElementType = PivotGridElements.FieldValueExpandButton;
							else {
								PivotFieldsAreaViewInfo fieldsAreaInfo;
								if(hitInfo.ValueInfo.IsColumn)
									fieldsAreaInfo = viewInfo.ColumnAreaFields;
								else fieldsAreaInfo = viewInfo.RowAreaFields;
								if(fieldsAreaInfo.GetSizingField(clientPoint) != null) {
									PivotFieldsAreaCellViewInfoBase fieldValueInfo = fieldsAreaInfo.GetViewInfoByItem(hitInfo.ValueInfo.Item);
									result.ElementType = PivotGridElements.FieldValueEdge;
									if(clientPoint.X < fieldValueInfo.ControlBounds.Left + viewInfo.FrameBorderWidth)
										if(item.IsColumn)
											item = visualItems.GetItem(item.IsColumn, item.MinLastLevelIndex - 1, item.Level);
										else item = visualItems.GetItem(item.IsColumn, item.MinLastLevelIndex, item.Level - 1);
								}
								else
									result.ElementType = PivotGridElements.FieldValue;
							}
							if(item.ValueType == PivotGridValueType.GrandTotal && item.Field == null) {
								if(item.Area == PivotArea.ColumnArea)
									result.FieldValueType = PivotFieldValueType.ColumnGrandTotal;
								else result.FieldValueType = PivotFieldValueType.RowGrandTotal;
							}
							else {
								result.FieldValueType = PivotFieldValueType.Value;
								result.FieldName = item.Field.ExpressionFieldName;
							}
							result.Area = (PivotAreaType)((int)item.Area);
							result.FieldValueValue = new ValueStruct(item.Value);
							result.FieldValueLastLevelIndex = item.MinLastLevelIndex;
							result.FieldValueLevel = item.Level;
							result.DisplayText = item.DisplayText;
							if(item.Field != null)
								result.FieldValueWidth = item.Field.Width;
						}
						break;
				}
			}
			return result;
		}
		protected PivotGridViewInfo GetPivotViewInfo(PivotGridControl pivot) {
			return pivot.GetType().GetProperty("ViewInfo", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(pivot, new object[] { }) as PivotGridViewInfo;
		}
		protected PivotVisualItems GetPivotVisualItems(PivotGridControl pivot) {
			return pivot.GetType().GetProperty("VisualItems", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(pivot, new object[] { }) as PivotVisualItems;
		}
		public string GetPivotGridElementRectangle(IntPtr windowHandle, PivotGridElementInfo elementInfo) {
			PivotGridControl pivot = Control.FromHandle(windowHandle) as PivotGridControl;
			if(pivot != null) {
				Rectangle rect = GetPivotGridElementRectangle(pivot, elementInfo);
				if(rect != Rectangle.Empty)
					return CodedUIUtils.ConvertToString(rect);
			}
			return null;
		}
		protected Rectangle GetPivotGridElementRectangle(PivotGridControl pivot, PivotGridElementInfo elementInfo) {
			PivotGridViewInfo viewInfo = GetPivotViewInfo(pivot);
			PivotVisualItems visualItems = GetPivotVisualItems(pivot);
			switch(elementInfo.ElementType) {
				case PivotGridElements.Cell:
					PivotCellEventArgs cellInfo = pivot.Cells.GetCellInfo(elementInfo.ColumnIndex, elementInfo.RowIndex);
					if(cellInfo != null)
						return cellInfo.Bounds;
					break;
				case PivotGridElements.FieldHeader:
				case PivotGridElements.HeadersArea:
				case PivotGridElements.FieldHeaderFilterButton:
				case PivotGridElements.FieldHeaderExpandButton:
					PivotHeadersViewInfoBase headersInfo = GetHeadersInfo(viewInfo, elementInfo.Area);
					return GetHeadersAreaElementRectangle(headersInfo, elementInfo);
				case PivotGridElements.FieldValue:
				case PivotGridElements.FieldValueExpandButton:
				case PivotGridElements.FieldValueEdge:
					PivotFieldsAreaCellViewInfoBase fieldValueInfo = GetPivotFieldsAreaCellViewInfo(viewInfo, visualItems, elementInfo);
					if(fieldValueInfo != null) {
						if(elementInfo.ElementType == PivotGridElements.FieldValueExpandButton && fieldValueInfo.OpenCloseButtonInfoArgs != null)
							return fieldValueInfo.OpenCloseButtonInfoArgs.Bounds;
						if(elementInfo.ElementType == PivotGridElements.FieldValueEdge)
							return new Rectangle(fieldValueInfo.ControlBounds.Right - viewInfo.FrameBorderWidth, fieldValueInfo.ControlBounds.Top, viewInfo.FrameBorderWidth * 2, fieldValueInfo.ControlBounds.Height);
						else return fieldValueInfo.ControlBounds;
					}
					break;
			}
			return Rectangle.Empty;
		}
		public PivotHeadersViewInfoBase GetHeadersInfo(PivotGridViewInfo viewInfo, PivotAreaType area) {
			switch(area) {
				case PivotAreaType.ColumnArea:
					return viewInfo.ColumnHeaders;
				case PivotAreaType.DataArea:
					return viewInfo.DataHeaders;
				case PivotAreaType.FilterArea:
					return viewInfo.FilterHeaders;
				case PivotAreaType.RowArea:
					return viewInfo.RowHeaders;
				default:
					return null;
			}
		}
		protected PivotFieldsAreaCellViewInfoBase GetPivotFieldsAreaCellViewInfo(PivotGridViewInfo viewInfo, PivotVisualItems visualItems, PivotGridElementInfo elementInfo) {
			if(elementInfo.FieldName == null && elementInfo.FieldValueType != PivotFieldValueType.Value) {
				bool isColumn = elementInfo.FieldValueType == PivotFieldValueType.ColumnGrandTotal;
				PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(visualItems, isColumn);
				if(itemsCreator != null) {
					PivotFieldsAreaViewInfo fieldsAreaViewInfo;
					if(isColumn) fieldsAreaViewInfo = viewInfo.ColumnAreaFields;
					else fieldsAreaViewInfo = viewInfo.RowAreaFields;
					return fieldsAreaViewInfo.GetItem(itemsCreator.LastLevelItemCount - 1, 0);
				}
			}
			else {
				PivotFieldValueItem fieldValueItem = GetPivotFieldValueItem(viewInfo, visualItems, elementInfo);
				if(fieldValueItem != null)
					if(fieldValueItem.IsColumn)
						return viewInfo.ColumnAreaFields.GetViewInfoByItem(fieldValueItem);
					else return viewInfo.RowAreaFields.GetViewInfoByItem(fieldValueItem);
			}
			return null;
		}
		protected PivotFieldValueItemsCreator GetItemsCreator(PivotVisualItems visualItems, bool isColumn) {
			MethodInfo getItemsCreatorMethod = visualItems.GetType().GetMethod("GetItemsCreator", BindingFlags.NonPublic | BindingFlags.Instance);
			if(getItemsCreatorMethod != null)
				return getItemsCreatorMethod.Invoke(visualItems, new object[] { isColumn }) as PivotFieldValueItemsCreator;
			return null;
		}
		protected PivotFieldValueItem GetPivotFieldValueItem(PivotGridViewInfo viewInfo, PivotVisualItems visualItems, PivotGridElementInfo elementInfo) {
			PivotFieldItem fieldItem = GetFieldItem(viewInfo, elementInfo);
			if(fieldItem != null) {
				if(fieldItem.Area == PivotArea.DataArea) {
					PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(visualItems, true);
					PivotFieldValueItem valueItem = GetPivotFieldValueItem(itemsCreator, fieldItem, elementInfo.FieldValueLastLevelIndex);
					if(valueItem == null) {
						itemsCreator = GetItemsCreator(visualItems, false);
						valueItem = GetPivotFieldValueItem(itemsCreator, fieldItem, elementInfo.FieldValueLastLevelIndex);
					}
					return valueItem;
				}
				else return visualItems.GetItem(fieldItem, elementInfo.FieldValueLastLevelIndex);
			}
			return null;
		}
		protected PivotFieldValueItem GetPivotFieldValueItem(PivotFieldValueItemsCreator itemsCreator, PivotFieldItem fieldItem, int lastLevelIndex) {
			PivotFieldValueItem valueItem = null;
			if(itemsCreator != null) {
				if(lastLevelIndex < itemsCreator.LastLevelItemCount) {
					valueItem = itemsCreator.GetLastLevelItem(lastLevelIndex);
					while(valueItem != null && valueItem.Field != fieldItem)
						valueItem = itemsCreator.GetParentItem(valueItem);
				}
			}
			return valueItem;
		}
		protected Rectangle GetHeadersAreaElementRectangle(PivotHeadersViewInfoBase headers, PivotGridElementInfo elementInfo) {
			if(elementInfo.ElementType == PivotGridElements.HeadersArea)
				return headers.Bounds;
			else {
				for(int i = 0; i < headers.ChildCount; i++)
					if(headers[i].Field != null && headers[i].Field.ExpressionFieldName == elementInfo.FieldName)
						switch(elementInfo.ElementType) {
							case PivotGridElements.FieldHeader:
								return headers[i].ControlBounds;
							case PivotGridElements.FieldHeaderExpandButton:
								OpenCloseButtonInfoArgs openCloseButtonInfo = null;
								PropertyInfo openCloseButtonInfoProperty = headers[i].GetType().GetProperty("OpenCloseButtonInfo", BindingFlags.NonPublic | BindingFlags.Instance);
								if(openCloseButtonInfoProperty != null)
									openCloseButtonInfo = openCloseButtonInfoProperty.GetValue(headers[i], new object[] { }) as OpenCloseButtonInfoArgs;
								if(openCloseButtonInfo != null)
									return openCloseButtonInfo.Bounds;
								else return Rectangle.Empty;
							case PivotGridElements.FieldHeaderFilterButton:
								if(headers[i].FilterInfo != null)
									return headers[i].FilterInfo.Bounds;
								break;
						}
			}
			return Rectangle.Empty;
		}
		protected PivotFieldItem GetFieldItem(PivotGridViewInfo viewInfo, PivotGridElementInfo elementInfo) {
			foreach(PivotFieldItem fieldItem in viewInfo.FieldItems)
				if(fieldItem.ExpressionFieldName == elementInfo.FieldName)
					return fieldItem;
			return null;
		}
		protected PivotGridField GetFieldFromName(PivotGridControl pivot, string name) {
			foreach(PivotGridField field in pivot.Fields)
				if(field.ExpressionFieldName == name)
					return field;
			if(pivot.Data.DataField != null && pivot.Data.DataField.ExpressionFieldName == name)
				return pivot.Data.DataField;
			return null;
		}
		protected int GetFieldAreaIndex(PivotGridControl pivot, PivotGridField field) {
			int areaIndex = field.AreaIndex;
			if(pivot.Data.DataField != null && field != pivot.Data.DataField && pivot.Data.DataField.Area == field.Area && pivot.Data.DataField.AreaIndex <= field.AreaIndex)
				areaIndex += 1;
			return areaIndex;
		}
		protected void SetFieldPosition(PivotGridControl pivot, PivotGridField field, PivotArea area, int areaIndex) {
			if(pivot.Data.DataField != null && field != pivot.Data.DataField && pivot.Data.DataField.Area == area) {
				field.Visible = false;
				if(areaIndex > pivot.Data.DataField.AreaIndex) {
					field.SetAreaPosition(area, areaIndex - 1);
				}
				else {
					field.SetAreaPosition(area, areaIndex);
					pivot.Data.DataField.AreaIndex = pivot.Data.DataField.AreaIndex + 1;
				}
			}
			else field.SetAreaPosition(area, areaIndex);
		}
		public void UpdatePivotGridElementInfo(IntPtr windowHandle, ref PivotGridElementInfo elementInfo) {
			PivotGridControl pivot = Control.FromHandle(windowHandle) as PivotGridControl;
			if(pivot == null)
				return;
			PivotGridViewInfo viewInfo = GetPivotViewInfo(pivot);
			PivotVisualItems visualItems = GetPivotVisualItems(pivot);
			switch(elementInfo.ElementType) {
				case PivotGridElements.FieldHeader:
					PivotGridField field = GetFieldFromName(pivot, elementInfo.FieldName);
					if(field != null) {
						elementInfo.DisplayText = field.HeaderDisplayText;
						elementInfo.Area = (PivotAreaType)(int)field.Area;
						elementInfo.AreaIndex = GetFieldAreaIndex(pivot, field);
						elementInfo.Visible = field.Visible;
					}
					break;
				case PivotGridElements.FieldValue:
					PivotFieldValueItem item = GetPivotFieldValueItem(viewInfo, visualItems, elementInfo);
					if(item != null) {
						elementInfo.Area = (PivotAreaType)(int)item.Area;
						elementInfo.FieldValueValue = new ValueStruct(item.Value);
						elementInfo.FieldValueLevel = item.Level;
						elementInfo.DisplayText = item.DisplayText;
						if(item.Field != null)
							elementInfo.FieldValueWidth = item.Field.Width;
					}
					break;
				case PivotGridElements.Cell:
					PivotCellEventArgs cellInfo = pivot.Cells.GetCellInfo(elementInfo.ColumnIndex, elementInfo.RowIndex);
					FillCellElementInfo(pivot, cellInfo, ref elementInfo);
					break;
			}
		}
		void FillCellElementInfo(PivotGridControl pivot, PivotCellEventArgs cellInfo, ref PivotGridElementInfo elementInfo) {
			elementInfo.ElementType = PivotGridElements.Cell;
			if(cellInfo != null) {
				elementInfo.RowIndex = cellInfo.RowIndex;
				elementInfo.ColumnIndex = cellInfo.ColumnIndex;
				elementInfo.CellValue = new ValueStruct(pivot.GetCellValue(cellInfo.ColumnIndex, cellInfo.RowIndex));
				elementInfo.DisplayText = cellInfo.DisplayText;
				if(cellInfo.ColumnField != null) {
					elementInfo.ColumnFieldName = cellInfo.ColumnField.ExpressionFieldName;
					if(cellInfo.ColumnValueType == PivotGridValueType.Value)
						elementInfo.ColumnFieldValue = new ValueStruct(cellInfo.Item.ColumnFieldValueItem.Value);
				}
				if(cellInfo.RowField != null) {
					elementInfo.RowFieldName = cellInfo.RowField.ExpressionFieldName;
					if(cellInfo.RowValueType == PivotGridValueType.Value)
						elementInfo.RowFieldValue = new ValueStruct(cellInfo.Item.RowFieldValueItem.Value);
				}
				if(cellInfo.DataField != null)
					elementInfo.DataFieldName = cellInfo.DataField.ExpressionFieldName;
			}
		}
		public void SetPivotGridElementProperty(IntPtr windowHandle, PivotGridElementInfo elementInfo, PivotElementPropertiesForSet propertyForSet) {
			PivotGridControl pivot = Control.FromHandle(windowHandle) as PivotGridControl;
			if(pivot != null) {
				pivot.BeginInvoke(new MethodInvoker(delegate() {
					SetPivotGridElementProperty(pivot, elementInfo, propertyForSet);
				}));
			}
		}
		protected void SetPivotGridElementProperty(PivotGridControl pivot, PivotGridElementInfo elementInfo, PivotElementPropertiesForSet propertyForSet) {
			PivotGridViewInfo viewInfo = GetPivotViewInfo(pivot);
			PivotVisualItems visualItems = GetPivotVisualItems(pivot);
			switch(elementInfo.ElementType) {
				case PivotGridElements.FieldHeader:
					PivotGridField field = GetFieldFromName(pivot, elementInfo.FieldName);
					if(field != null) {
						switch(propertyForSet) {
							case PivotElementPropertiesForSet.Visibility:
								field.Visible = elementInfo.Visible;
								break;
							case PivotElementPropertiesForSet.Position:
								if(!field.Visible || field.Area != (PivotArea)((int)elementInfo.Area) || GetFieldAreaIndex(pivot, field) != elementInfo.AreaIndex)
									SetFieldPosition(pivot, field, (PivotArea)((int)elementInfo.Area), elementInfo.AreaIndex);
								break;
						}
					}
					break;
				case PivotGridElements.FieldValue:
					PivotFieldValueItem item = GetPivotFieldValueItem(viewInfo, visualItems, elementInfo);
					if(item != null) {
						switch(propertyForSet) {
							case PivotElementPropertiesForSet.Width:
								if(item.Field != null)
									new PivotGridFieldUISetWidthAction(item.Field, viewInfo.Data).SetWidth(elementInfo.FieldValueWidth);
								break;
						}
					}
					break;
			}
		}
		public PivotGridElementInfo GetCustomizationListBoxItemInfo(IntPtr listBoxHandle, int pointX, int pointY) {
			PivotGridElementInfo result = new PivotGridElementInfo();
			PivotCustomizationTreeBoxBase listBox = Control.FromHandle(listBoxHandle) as PivotCustomizationTreeBoxBase;
			if(listBox != null) {
				PivotCustomizationTreeNodeBase item = listBox.GetItem(new Point(pointX, pointY)) as PivotCustomizationTreeNodeBase;
				if(item != null && item.Field != null) {
					result.ElementType = PivotGridElements.CustomizationListBoxItem;
					result.FieldName = item.Field.ExpressionFieldName;
					result.DisplayText = item.Field.HeaderDisplayText;
					result.CustomizationListBoxItemIndex = listBox.Items.IndexOf(item);
				}
			}
			return result;
		}
		public PivotGridElementInfo GetCustomizationListBoxItemInfo(IntPtr listBoxHandle, string fieldName) {
			PivotGridElementInfo result = new PivotGridElementInfo();
			PivotCustomizationTreeBoxBase listBox = Control.FromHandle(listBoxHandle) as PivotCustomizationTreeBoxBase;
			if(listBox != null) {
				for(int i = 0; i < listBox.Items.Count; i++) {
					PivotCustomizationTreeNodeBase item = listBox.GetItem(i) as PivotCustomizationTreeNodeBase;
					if(item != null && item.Field != null && item.Field.ExpressionFieldName == fieldName) {
						result.ElementType = PivotGridElements.CustomizationListBoxItem;
						result.FieldName = item.Field.ExpressionFieldName;
						result.DisplayText = item.Field.HeaderDisplayText;
						result.CustomizationListBoxItemIndex = i;
						result.Area = (PivotAreaType)(int)item.Field.Area;
						result.AreaIndex = item.Field.AreaIndex;
						result.Visible = item.Field.Visible;
						PivotGridControl pivot = listBox.CustomizationForm.ControlOwner as PivotGridControl;
						if(pivot != null) {
							PivotGridField field = GetFieldFromName(pivot, fieldName);
							if(field != null) {
								result.AreaIndex = GetFieldAreaIndex(pivot, field);
							}
						}
					}
				}
			}
			return result;
		}
		public IntPtr GetCustomizationFormHandle(IntPtr pivotHandle) {
			PivotGridControl pivot = Control.FromHandle(pivotHandle) as PivotGridControl;
			if(pivot != null && pivot.CustomizationForm != null && pivot.CustomizationForm.IsHandleCreated)
				return pivot.CustomizationForm.Handle;
			return IntPtr.Zero;
		}
		public IntPtr GetActiveEditorHandle(IntPtr pivotHandle) {
			PivotGridControl pivot = Control.FromHandle(pivotHandle) as PivotGridControl;
			if(pivot != null && pivot.ActiveEditor != null && pivot.ActiveEditor.IsHandleCreated)
				return pivot.ActiveEditor.Handle;
			else
				return IntPtr.Zero;
		}
		public IntPtr GetActiveEditorHandleOrMakeItVisible(IntPtr pivotHandle, int columnIndex, int rowIndex) {
			PivotGridControl pivot = Control.FromHandle(pivotHandle) as PivotGridControl;
			if(pivot != null) {
				if(pivot.Cells.FocusedCell.X != columnIndex || pivot.Cells.FocusedCell.Y != rowIndex)
					pivot.BeginInvoke(new MethodInvoker(delegate() {
						pivot.CloseEditor();
						pivot.Cells.FocusedCell = new Point(columnIndex, rowIndex);
						pivot.ShowEditor();
					}));
				else if(pivot.ActiveEditor == null || !pivot.ActiveEditor.IsHandleCreated)
					pivot.BeginInvoke(new MethodInvoker(delegate() {
						pivot.ShowEditor();
					}));
				else
					return pivot.ActiveEditor.Handle;
			}
			return IntPtr.Zero;
		}
		public int GetPivotFieldValueMinLastLevelIndex(IntPtr pivotHandle, PivotGridElementInfo fieldValueInfo) {
			PivotGridControl pivot = Control.FromHandle(pivotHandle) as PivotGridControl;
			if(pivot != null) {
				PivotGridViewInfo viewInfo = GetPivotViewInfo(pivot);
				PivotVisualItems visualItems = GetPivotVisualItems(pivot);
				PivotFieldValueItem item = GetPivotFieldValueItem(viewInfo, visualItems, fieldValueInfo);
				if(item != null)
					return item.MinLastLevelIndex;
			}
			return 0;
		}
	}
}
