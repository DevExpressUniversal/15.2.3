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
	[ToolboxItem(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ClientSideXtraGridMethods : ClientSideHelperBase {
		ClientSideHelper clientSideHelper;
		internal ClientSideXtraGridMethods(RemoteObject remoteObject, ClientSideHelper clientSideHelper)
			: base(remoteObject) {
			this.clientSideHelper = clientSideHelper;
		}
		const string HelperTypeName = "DevExpress.XtraGrid.CodedUISupport.GridCodedUIHelper";
		IGridCodedUIHelper gridCodedUIHelper;
		IGridCodedUIHelper Helper {
			get {
				if(gridCodedUIHelper == null)
					gridCodedUIHelper = this.GetHelper<IGridCodedUIHelper>(HelperTypeName, AssemblyInfo.SRAssemblyGrid);
				return gridCodedUIHelper;
			}
		}
		public bool AreGridMethodsLoaded() {
			return Helper != null;
		}
		public int GetGridElementFromPoint(IntPtr windowHandle, int x, int y, out int rowHandle, out string columnHandle, out string viewName) {
			int _rowHandle = 0;
			string _columnHandle = null, _viewName = null;
			int result = RunClientSideMethod(Helper, windowHandle, delegate() {
				return (int)Helper.GetGridElementFromPoint(windowHandle, x, y, out _rowHandle, out _columnHandle, out _viewName);
			});
			rowHandle = _rowHandle;
			columnHandle = _columnHandle;
			viewName = _viewName;
			return result;
		}
		public string GetGridElementRectangleOrMakeElementVisible(IntPtr windowHandle, int rowHandle, string columnName, string viewName, int elementType) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetGridElementRectangleOrMakeElementVisible(windowHandle, rowHandle, columnName, viewName, (GridControlElements)elementType);
			});
		}
		public int GetGridActiveEditorHandleOrSetActiveEditor(IntPtr windowHandle, int row, string columnName, string viewName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetGridActiveEditorHandleOrSetActiveEditor(windowHandle, row, columnName, viewName).ToInt32();
			});
		}
		public int GetGridActiveEditorHandle(IntPtr windowHandle, string viewName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetGridActiveEditorHandle(windowHandle, viewName).ToInt32();
			});
		}
		public void SetGridActiveEditorValue(IntPtr windowHandle, int rowHandle, string columnName, string viewName, ValueStruct value) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetGridActiveEditorValue(windowHandle, rowHandle, columnName, viewName, value);
			});
		}
		public bool GetGridViewColumnPosition(IntPtr windowHandle, string columnName, string viewName, bool getValueFromFields, out int visibleIndex, out int groupIndex) {
			int _visibleIndex = Int32.MinValue, _groupIndex = Int32.MinValue;
			bool result = RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetGridViewColumnPosition(windowHandle, columnName, viewName, getValueFromFields, out _visibleIndex, out _groupIndex);
			});
			visibleIndex = _visibleIndex;
			groupIndex = _groupIndex;
			return result;
		}
		public void SetGridViewColumnPosition(IntPtr windowHandle, string columnName, string viewName, int visibleIndex, int groupIndex) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetGridViewColumnPosition(windowHandle, columnName, viewName, visibleIndex, groupIndex);
			});
		}
		public bool GetAdvBandedGridViewColumnPosition(IntPtr windowHandle, string columnName, string viewName, out string bandName, out int rowIndex, out int visibleIndex, out int groupIndex, bool getRealVisibleIndex) {
			int _visibleIndex = Int32.MinValue, _groupIndex = Int32.MinValue, _rowIndex = Int32.MinValue;
			string _bandName = null;
			bool result = RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetAdvBandedGridViewColumnPosition(windowHandle, columnName, viewName, out _bandName, out _rowIndex, out _visibleIndex, out _groupIndex, getRealVisibleIndex);
			});
			visibleIndex = _visibleIndex;
			groupIndex = _groupIndex;
			rowIndex = _rowIndex;
			bandName = _bandName;
			return result;
		}
		public void SetAdvBandedGridViewColumnPosition(IntPtr windowHandle, string viewName, string columnName, string bandName, int rowIndex, int visibleIndex, int groupIndex) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetAdvBandedGridViewColumnPosition(windowHandle, viewName, columnName, bandName, rowIndex, visibleIndex, groupIndex);
			});
		}
		public int GetColumnWidth(IntPtr windowHandle, string columnName, string viewName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetColumnWidth(windowHandle, columnName, viewName);
			});
		}
		public void SetColumnWidth(IntPtr windowHandle, string columnName, string viewName, int columnWidth) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetColumnWidth(windowHandle, columnName, viewName, columnWidth);
			});
		}
		public string GetColumnFilterString(IntPtr windowHandle, string columnName, string viewName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetColumnFilterString(windowHandle, columnName, viewName);
			});
		}
		public void OpenCustomFilterWindow(IntPtr windowHandle, string columnName, string viewName) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.OpenCustomFilterWindow(windowHandle, columnName, viewName);
			});
		}
		public string GetActiveFilterString(IntPtr windowHandle, string viewName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetActiveFilterString(windowHandle, viewName);
			});
		}
		public void SetActiveFilterString(IntPtr windowHandle, string viewName, string filterString) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetActiveFilterString(windowHandle, viewName, filterString);
			});
		}
		public bool GetGridFocus(IntPtr windowHandle, string viewName, out int rowHandle, out string columnName) {
			int _rowHandle = Int32.MinValue;
			string _columnName = null;
			bool result = RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetGridFocus(windowHandle, viewName, out _rowHandle, out _columnName);
			});
			rowHandle = _rowHandle;
			columnName = _columnName;
			return result;
		}
		public void SetGridFocus(IntPtr windowHandle, int rowHandle, string columnName, string viewName) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetGridFocus(windowHandle, rowHandle, columnName, viewName);
			});
		}
		public void SetCellValue(IntPtr windowHandle, int rowHandle, string columnName, string viewName, string cellValue, string cellValueType) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetCellValue(windowHandle, rowHandle, columnName, viewName, cellValue, cellValueType);
			});
		}
		public int GetBandWidth(IntPtr windowHandle, string viewName, string bandName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBandWidth(windowHandle, viewName, bandName);
			});
		}
		public void SetBandWidth(IntPtr windowHandle, string viewName, string bandName, int bandWidth) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetBandWidth(windowHandle, viewName, bandName, bandWidth);
			});
		}
		public bool GetBandPosition(IntPtr windowHandle, string viewName, string bandName, out string parentBandName, out int visibleIndex) {
			int _visibleIndex = Int32.MinValue;
			string _parentBandName = null;
			bool result = RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBandPosition(windowHandle, viewName, bandName, out _parentBandName, out _visibleIndex);
			});
			visibleIndex = _visibleIndex;
			parentBandName = _parentBandName;
			return result;
		}
		public void SetBandPosition(IntPtr windowHandle, string viewName, string bandName, string parentBandName, int visibleIndex) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetBandPosition(windowHandle, viewName, bandName, parentBandName, visibleIndex);
			});
		}
		public int GetCustomizationListBoxItemIndex(IntPtr windowHandle, string innerElementName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetCustomizationListBoxItemIndex(windowHandle, innerElementName);
			});
		}
		public bool GetInnerElementInformationForCustomizationListBoxItem(IntPtr listBoxHandle, int itemIndex, out int gridHandleAsInt, out string viewName, out GridControlElements innerElementType, out string innerElementName) {
			IntPtr _gridHandle = IntPtr.Zero;
			string _viewName = null, _innerElementName = null;
			GridControlElements _innerElementType = new GridControlElements();
			bool result = RunClientSideMethod(Helper, listBoxHandle, delegate() {
				return Helper.GetInnerElementInformationForCustomizationListBoxItem(listBoxHandle, itemIndex, out _gridHandle, out _viewName, out _innerElementType, out _innerElementName);
			});
			gridHandleAsInt = _gridHandle.ToInt32();
			viewName = _viewName;
			innerElementType = _innerElementType;
			innerElementName = _innerElementName;
			return result;
		}
		public string GetColumnName(IntPtr windowHandle, int absoluteIndex, string viewName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetColumnName(windowHandle, absoluteIndex, viewName);
			});
		}
		public string GetCustomizationFormViewName(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetCustomizationFormViewName(windowHandle);
			});
		}
		public int GetCustomizationFormHandleOrShowIt(IntPtr windowHandle, string viewName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetCustomizationFormHandleOrShowIt(windowHandle, viewName).ToInt32();
			});
		}
		public string[] GetViewsNames(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetViewsNames(windowHandle);
			});
		}
		public ValueStruct GetGridElementValue(IntPtr windowHandle, int rowHandle, string columnName, string viewName, GridControlElements elementType) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetGridElementValue(windowHandle, rowHandle, columnName, viewName, elementType);
			});
		}
		public AppearanceObjectSerializable GetGridElementAppearance(IntPtr windowHandle, GridElementInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetGridElementAppearance(windowHandle, elementInfo);
			});
		}
		public string GetColumnNameFromGroupRow(IntPtr windowHandle, int rowHandle, string viewName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetColumnNameFromGroupRow(windowHandle, rowHandle, viewName);
			});
		}
		public void MakeGridElementVisible(IntPtr windowHandle, GridControlElements elementType, int rowHandle, string columnName, string viewName) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.MakeGridElementVisible(windowHandle, elementType, rowHandle, columnName, viewName);
			});
		}
		public string HandleViewViaReflection(IntPtr windowHandle, string viewName, string membersString, string newValue, string newValueType, bool isSet) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.HandleViewViaReflection(windowHandle, viewName, membersString, newValue, newValueType, isSet);
			});
		}
		public int GetViewTypeFromName(IntPtr windowHandle, string viewName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return (int)Helper.GetViewTypeFromName(windowHandle, viewName);
			});
		}
		public int GetColumnAbsoluteIndexFromName(IntPtr windowHandle, string viewName, string columnName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetColumnAbsoluteIndexFromName(windowHandle, viewName, columnName);
			});
		}
		public void SetLookUpEditBaseValue(IntPtr windowHandle, ValueStruct value, bool byDisplayMember) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetLookUpEditBaseValue(windowHandle, value, byDisplayMember);
			});
		}
		public int GetSetLookUpEditSelectedIndex(IntPtr windowHandle, int newValue, bool isSet) {
			if(Helper != null)
				return RunClientSideMethod(Helper, windowHandle, delegate() {
					return Helper.GetSetLookUpEditSelectedIndex(windowHandle, newValue, isSet);
				});
			else {
				return clientSideHelper.EditorsMethods.GetSetLookUpEditSelectedIndex(windowHandle, newValue, isSet);
			}
		}
		public int[] GetCellsRowHandlesByValue(IntPtr windowHandle, string columnName, string viewName, ValueStruct searchParams, bool findByText, bool findFirst) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetCellsRowHandlesByValue(windowHandle, columnName, viewName, searchParams, findByText, findFirst);
			});
		}
		public string GetCheckboxSelectorColumnName(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetCheckboxSelectorColumnName(windowHandle);
			});
		}
	}
}
