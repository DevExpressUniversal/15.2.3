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
	public interface IGridCodedUIHelper {
		GridControlElements GetGridElementFromPoint(IntPtr windowHandle, int x, int y, out int rowHandle, out string columnName, out string viewName);
		string GetGridElementRectangleOrMakeElementVisible(IntPtr windowHandle, int rowHandle, string columnName, string viewName, GridControlElements elementType);
		IntPtr GetGridActiveEditorHandleOrSetActiveEditor(IntPtr windowHandle, int row, string columnName, string viewName);
		IntPtr GetGridActiveEditorHandle(IntPtr windowHandle, string viewName);
		void SetGridActiveEditorValue(IntPtr gridHandle, int rowHandle, string columnName, string viewName, ValueStruct value);
		bool GetGridViewColumnPosition(IntPtr windowHandle, string columnName, string viewName, bool getValueFromFields, out int visibleIndex, out int groupIndex);
		void SetGridViewColumnPosition(IntPtr windowHandle, string columnName, string viewName, int visibleIndex, int groupIndex);
		bool GetAdvBandedGridViewColumnPosition(IntPtr windowHandle, string columnName, string viewName, out string bandName, out int rowIndex, out int visibleIndex, out int groupIndex, bool getRealVisibleIndex);
		void SetAdvBandedGridViewColumnPosition(IntPtr windowHandle, string viewName, string columnName, string bandName, int rowIndex, int visibleIndex, int groupIndex);
		int GetColumnWidth(IntPtr windowHandle, string columnName, string viewName);
		void SetColumnWidth(IntPtr windowHandle, string columnName, string viewName, int columnWidth);
		string GetColumnFilterString(IntPtr windowHandle, string columnName, string viewName);
		void OpenCustomFilterWindow(IntPtr windowHandle, string columnName, string viewName);
		string GetActiveFilterString(IntPtr windowHandle, string viewName);
		void SetActiveFilterString(IntPtr windowHandle, string viewName, string filterString);
		bool GetGridFocus(IntPtr windowHandle, string viewName, out int rowHandle, out string columnName);
		void SetGridFocus(IntPtr windowHandle, int rowHandle, string columnName, string viewName);
		void SetCellValue(IntPtr windowHandle, int rowHandle, string columnName, string viewName, string cellValue, string cellValueType);
		int GetBandWidth(IntPtr windowHandle, string viewName, string bandName);
		void SetBandWidth(IntPtr windowHandle, string viewName, string bandName, int bandWidth);
		bool GetBandPosition(IntPtr windowHandle, string viewName, string bandName, out string parentBandName, out int visibleIndex);
		void SetBandPosition(IntPtr windowHandle, string viewName, string bandName, string parentBandName, int visibleIndex);
		int GetCustomizationListBoxItemIndex(IntPtr windowHandle, string innerElementName);
		bool GetInnerElementInformationForCustomizationListBoxItem(IntPtr listBoxHandle, int itemIndex, out IntPtr gridHandle, out string viewName, out GridControlElements innerElementType, out string innerElementName);
		string GetColumnName(IntPtr windowHandle, int absoluteIndex, string viewName);
		string GetColumnNameFromGroupRow(IntPtr windowHandle, int rowHandle, string viewName);
		string GetCustomizationFormViewName(IntPtr windowHandle);
		IntPtr GetCustomizationFormHandleOrShowIt(IntPtr windowHandle, string viewName);
		string[] GetViewsNames(IntPtr windowHandle);
		ValueStruct GetGridElementValue(IntPtr windowHandle, int rowHandle, string columnName, string viewName, GridControlElements elementType);
		AppearanceObjectSerializable GetGridElementAppearance(IntPtr windowHandle, GridElementInfo elementInfo);
		void MakeGridElementVisible(IntPtr windowHandle, GridControlElements elementType, int rowHandle, string columnName, string viewName);
		string HandleViewViaReflection(IntPtr gridHandle, string viewName, string membersString, string newValue, string newValueType, bool isSet);
		GridControlViews GetViewTypeFromName(IntPtr windowHandle, string viewName);
		int GetColumnAbsoluteIndexFromName(IntPtr windowHandle, string viewName, string columnName);
		void SetLookUpEditBaseValue(IntPtr windowHandle, ValueStruct value, bool byDisplayMember);
		int GetSetLookUpEditSelectedIndex(IntPtr windowHandle, int newValue, bool isSet);
		int[] GetCellsRowHandlesByValue(IntPtr windowHandle, string columnName, string viewName, ValueStruct searchParams, bool findByText, bool findFirst);
		string GetCheckboxSelectorColumnName(IntPtr windowHandle);
	}
}
