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
using System.ComponentModel;
namespace DevExpress.Utils.CodedUISupport {
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class RemoteObjectXtraGridMethods : IGridCodedUIHelper {
		ClientSideHelpersManager HelperManager;
		internal RemoteObjectXtraGridMethods(ClientSideHelpersManager manager) {
			this.HelperManager = manager;
		}
		public GridControlElements GetGridElementFromPoint(IntPtr windowHandle, int x, int y, out int rowHandle, out string columnName, out string viewName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) { rowHandle = 0; columnName = null; viewName = null; return GridControlElements.Unknown; }
			GridControlElements result = (GridControlElements)clientSideHelper.GridMethods.GetGridElementFromPoint(windowHandle, x, y, out rowHandle, out columnName, out viewName);
			return result;
		}
		public string GetGridElementRectangleOrMakeElementVisible(IntPtr windowHandle, int rowHandle, string columnName, string viewName, GridControlElements elementType) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return null;
			string result = clientSideHelper.GridMethods.GetGridElementRectangleOrMakeElementVisible(windowHandle, rowHandle, columnName, viewName, (int)elementType);
			return result;
		}
		public IntPtr GetGridActiveEditorHandleOrSetActiveEditor(IntPtr windowHandle, int row, string columnName, string viewName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return IntPtr.Zero;
			return new IntPtr(clientSideHelper.GridMethods.GetGridActiveEditorHandleOrSetActiveEditor(windowHandle, row, columnName, viewName));
		}
		public IntPtr GetGridActiveEditorHandle(IntPtr windowHandle, string viewName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return IntPtr.Zero;
			return new IntPtr(clientSideHelper.GridMethods.GetGridActiveEditorHandle(windowHandle, viewName));
		}
		public void SetGridActiveEditorValue(IntPtr gridHandle, int rowHandle, string columnName, string viewName, ValueStruct value) {
			if(HelperManager.Get(gridHandle) != null) HelperManager.Get(gridHandle).GridMethods.SetGridActiveEditorValue(gridHandle, rowHandle, columnName, viewName, value);
		}
		public bool GetGridViewColumnPosition(IntPtr windowHandle, string columnName, string viewName, bool getValueFromFields, out int visibleIndex, out int groupIndex) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) { visibleIndex = groupIndex = Int32.MinValue; return false; }
			bool result = clientSideHelper.GridMethods.GetGridViewColumnPosition(windowHandle, columnName, viewName, getValueFromFields, out visibleIndex, out groupIndex);
			return result;
		}
		public void SetGridViewColumnPosition(IntPtr windowHandle, string columnName, string viewName, int visibleIndex, int groupIndex) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.GridMethods.SetGridViewColumnPosition(windowHandle, columnName, viewName, visibleIndex, groupIndex);
		}
		public bool GetAdvBandedGridViewColumnPosition(IntPtr windowHandle, string columnName, string viewName, out string bandName, out int rowIndex, out int visibleIndex, out int groupIndex, bool getRealVisibleIndex) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) { visibleIndex = groupIndex = rowIndex = Int32.MinValue; bandName = null; return false; }
			bool result = clientSideHelper.GridMethods.GetAdvBandedGridViewColumnPosition(windowHandle, columnName, viewName, out bandName, out rowIndex, out visibleIndex, out groupIndex, getRealVisibleIndex);
			return result;
		}
		public void SetAdvBandedGridViewColumnPosition(IntPtr windowHandle, string viewName, string columnName, string bandName, int rowIndex, int visibleIndex, int groupIndex) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.GridMethods.SetAdvBandedGridViewColumnPosition(windowHandle, viewName, columnName, bandName, rowIndex, visibleIndex, groupIndex);
		}
		public int GetColumnWidth(IntPtr windowHandle, string columnName, string viewName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return -1;
			int result = clientSideHelper.GridMethods.GetColumnWidth(windowHandle, columnName, viewName);
			return result;
		}
		public void SetColumnWidth(IntPtr windowHandle, string columnName, string viewName, int columnWidth) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.GridMethods.SetColumnWidth(windowHandle, columnName, viewName, columnWidth);
		}
		public string GetColumnFilterString(IntPtr windowHandle, string columnName, string viewName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return null;
			return clientSideHelper.GridMethods.GetColumnFilterString(windowHandle, columnName, viewName);
		}
		public void OpenCustomFilterWindow(IntPtr windowHandle, string columnName, string viewName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.GridMethods.OpenCustomFilterWindow(windowHandle, columnName, viewName);
		}
		public string GetActiveFilterString(IntPtr windowHandle, string viewName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return null;
			return clientSideHelper.GridMethods.GetActiveFilterString(windowHandle, viewName);
		}
		public void SetActiveFilterString(IntPtr windowHandle, string viewName, string filterString) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.GridMethods.SetActiveFilterString(windowHandle, viewName, filterString);
		}
		public bool GetGridFocus(IntPtr windowHandle, string viewName, out int rowHandle, out string columnName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) { rowHandle = Int32.MinValue; columnName = null; return false; }
			return clientSideHelper.GridMethods.GetGridFocus(windowHandle, viewName, out rowHandle, out columnName);
		}
		public void SetGridFocus(IntPtr windowHandle, int rowHandle, string columnName, string viewName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.GridMethods.SetGridFocus(windowHandle, rowHandle, columnName, viewName);
		}
		public void SetCellValue(IntPtr windowHandle, int rowHandle, string columnName, string viewName, string cellValue, string cellValueType) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.GridMethods.SetCellValue(windowHandle, rowHandle, columnName, viewName, cellValue, cellValueType);
		}
		public int GetBandWidth(IntPtr windowHandle, string viewName, string bandName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return Int32.MinValue;
			return clientSideHelper.GridMethods.GetBandWidth(windowHandle, viewName, bandName);
		}
		public void SetBandWidth(IntPtr windowHandle, string viewName, string bandName, int bandWidth) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.GridMethods.SetBandWidth(windowHandle, viewName, bandName, bandWidth);
		}
		public bool GetBandPosition(IntPtr windowHandle, string viewName, string bandName, out string parentBandName, out int visibleIndex) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) { parentBandName = null; visibleIndex = Int32.MinValue; return false; }
			return clientSideHelper.GridMethods.GetBandPosition(windowHandle, viewName, bandName, out parentBandName, out visibleIndex);
		}
		public void SetBandPosition(IntPtr windowHandle, string viewName, string bandName, string parentBandName, int visibleIndex) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.GridMethods.SetBandPosition(windowHandle, viewName, bandName, parentBandName, visibleIndex);
		}
		public int GetCustomizationListBoxItemIndex(IntPtr windowHandle, string innerElementName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return -1;
			return clientSideHelper.GridMethods.GetCustomizationListBoxItemIndex(windowHandle, innerElementName);
		}
		public bool GetInnerElementInformationForCustomizationListBoxItem(IntPtr listBoxHandle, int itemIndex, out IntPtr gridHandle, out string viewName, out GridControlElements innerElementType, out string innerElementName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(listBoxHandle);
			if(clientSideHelper == null) { gridHandle = IntPtr.Zero; viewName = innerElementName = null; innerElementType = GridControlElements.Unknown; return false; }
			int gridHandleAsInt;
			bool result = clientSideHelper.GridMethods.GetInnerElementInformationForCustomizationListBoxItem(listBoxHandle, itemIndex, out gridHandleAsInt, out viewName, out innerElementType, out innerElementName);
			gridHandle = new IntPtr(gridHandleAsInt);
			return result;
		}
		public string GetColumnName(IntPtr windowHandle, int absoluteIndex, string viewName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return null;
			return clientSideHelper.GridMethods.GetColumnName(windowHandle, absoluteIndex, viewName);
		}
		public string GetCustomizationFormViewName(IntPtr windowHandle) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return null;
			return clientSideHelper.GridMethods.GetCustomizationFormViewName(windowHandle);
		}
		public IntPtr GetCustomizationFormHandleOrShowIt(IntPtr windowHandle, string viewName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return IntPtr.Zero;
			return new IntPtr(clientSideHelper.GridMethods.GetCustomizationFormHandleOrShowIt(windowHandle, viewName));
		}
		public string[] GetViewsNames(IntPtr windowHandle) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return null;
			return clientSideHelper.GridMethods.GetViewsNames(windowHandle);
		}
		public ValueStruct GetGridElementValue(IntPtr windowHandle, int rowHandle, string columnName, string viewName, GridControlElements elementType) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return new ValueStruct();
			return clientSideHelper.GridMethods.GetGridElementValue(windowHandle, rowHandle, columnName, viewName, elementType);
		}
		public AppearanceObjectSerializable GetGridElementAppearance(IntPtr windowHandle, GridElementInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).GridMethods.GetGridElementAppearance(windowHandle, elementInfo);
		}
		public string GetColumnNameFromGroupRow(IntPtr windowHandle, int rowHandle, string viewName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return null;
			return clientSideHelper.GridMethods.GetColumnNameFromGroupRow(windowHandle, rowHandle, viewName);
		}
		public void MakeGridElementVisible(IntPtr windowHandle, GridControlElements elementType, int rowHandle, string columnName, string viewName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.GridMethods.MakeGridElementVisible(windowHandle, elementType, rowHandle, columnName, viewName);
		}
		public string HandleViewViaReflection(IntPtr gridHandle, string viewName, string membersString, string newValue, string newValueType, bool isSet) {
			ClientSideHelper clientSideHelper = HelperManager.Get(gridHandle);
			if(clientSideHelper == null) return null;
			return clientSideHelper.GridMethods.HandleViewViaReflection(gridHandle, viewName, membersString, newValue, newValueType, isSet);
		}
		public GridControlViews GetViewTypeFromName(IntPtr windowHandle, string viewName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return GridControlViews.Undefined;
			return (GridControlViews)clientSideHelper.GridMethods.GetViewTypeFromName(windowHandle, viewName);
		}
		public int GetColumnAbsoluteIndexFromName(IntPtr windowHandle, string viewName, string columnName) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return -1;
			return clientSideHelper.GridMethods.GetColumnAbsoluteIndexFromName(windowHandle, viewName, columnName);
		}
		public void SetLookUpEditBaseValue(IntPtr windowHandle, ValueStruct value, bool byDisplayMember) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).GridMethods.SetLookUpEditBaseValue(windowHandle, value, byDisplayMember);
		}
		public int GetSetLookUpEditSelectedIndex(IntPtr windowHandle, int newValue, bool isSet) {
			return HelperManager.Get(windowHandle) == null ? -1 : HelperManager.Get(windowHandle).GridMethods.GetSetLookUpEditSelectedIndex(windowHandle, newValue, isSet);
		}
		public int[] GetCellsRowHandlesByValue(IntPtr windowHandle, string columnName, string viewName, ValueStruct searchParams, bool findByText, bool findFirst) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).GridMethods.GetCellsRowHandlesByValue(windowHandle, columnName, viewName, searchParams, findByText, findFirst);
		}
		public string GetCheckboxSelectorColumnName(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).GridMethods.GetCheckboxSelectorColumnName(windowHandle);
		}
	}
}
