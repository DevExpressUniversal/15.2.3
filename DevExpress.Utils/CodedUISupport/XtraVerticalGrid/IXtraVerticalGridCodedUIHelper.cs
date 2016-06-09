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
	public interface IXtraVerticalGridCodedUIHelper {
		VerticalGridElementInfo GetVerticalGridElementFromPoint(IntPtr windowHandle, int pointX, int pointY);
		string GetVerticalGridElementRectangle(IntPtr windowHandle, VerticalGridElementInfo elementInfo);
		VerticalGridElementVariableInfo GetVerticalGridElementVariableInfo(IntPtr windowHandle, VerticalGridElementInfo elementInfo);
		bool GetInnerElementInformationForCustomizationListBoxItem(IntPtr listBoxHandle, int itemIndex, out IntPtr gridHandle, out VerticalGridElementInfo elementInfo);
		int GetCustomizationListBoxItemIndex(IntPtr windowHandle, string innerElementName);
		PropertyDescriptionVariableInfo GetPropertyDescriptionVariableInfo(IntPtr windowHandle);
		IntPtr GetCustomizationFormHandleOrShowIt(IntPtr windowHandle);
		IntPtr GetVerticalGridActiveEditorHandle(IntPtr windowHandle);
		IntPtr GetVerticalGridActiveEditorHandleOrSetActiveEditor(IntPtr windowHandle, string rowName, int recordIndex, int cellIndex);
		String GetRowName(IntPtr windowHandle, int rowIndex, string categoryName);
		String GetCategoryName(IntPtr windowHandle, int categoryIndex);
		void ApplyVerticalGridElementVariableInfo(IntPtr windowHandle, VerticalGridElementInfo elementInfo, VerticalGridElementVariableInfo variableInfo);
		string GetDragRowEffect(IntPtr windowHandle, string sourceRowName, ref string destRowName, int oldRowVisibleIndex, string oldParentRowName, out bool canMoveRow, out bool clearDragAction);
		void DragRowAction(IntPtr windowHandle, string sourceRowName, string rowDestName, string parentRow, string dragRowEffect);
		void OpenRightCustomizationTabPage(IntPtr windowHandle, string rowName);
		bool GetVerticalGridFocus(IntPtr windowHandle, out string rowName, out int recordIndex, out int cellIndex);
		void SetVerticalGridFocus(IntPtr windowHandle, string rowName, int recordIndex, int cellIndex);
		string GetFocusedCellValueAsString(IntPtr windowHandle);
		bool GetCellValue(IntPtr windowHandle, string rowName, int recordIndex, int cellIndex, out string value, out string valueType);
		void PostEditorValue(IntPtr windowHandle);
		void UpdateScrollBars(IntPtr windowHandle, int scrollCount, bool isVert);
	}
}
