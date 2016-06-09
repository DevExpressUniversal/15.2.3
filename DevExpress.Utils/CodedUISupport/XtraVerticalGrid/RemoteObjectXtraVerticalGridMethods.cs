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
	public class RemoteObjectXtraVerticalGridMethods : IXtraVerticalGridCodedUIHelper {
		ClientSideHelpersManager HelperManager;
		internal RemoteObjectXtraVerticalGridMethods(ClientSideHelpersManager manager) {
			this.HelperManager = manager;
		}
		public VerticalGridElementInfo GetVerticalGridElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return HelperManager.Get(windowHandle) == null ? new VerticalGridElementInfo() : HelperManager.Get(windowHandle).VerticalGridMethods.GetVerticalGridElementFromPoint(windowHandle, pointX, pointY);
		}
		public string GetVerticalGridElementRectangle(IntPtr windowHandle, VerticalGridElementInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).VerticalGridMethods.GetVerticalGridElementRectangle(windowHandle, elementInfo);
		}
		public VerticalGridElementVariableInfo GetVerticalGridElementVariableInfo(IntPtr windowHandle, VerticalGridElementInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? new VerticalGridElementVariableInfo() : HelperManager.Get(windowHandle).VerticalGridMethods.GetVerticalGridElementVariableInfo(windowHandle, elementInfo);
		}
		public bool GetInnerElementInformationForCustomizationListBoxItem(IntPtr listBoxHandle, int itemIndex, out IntPtr gridHandle, out VerticalGridElementInfo elementInfo) {
			ClientSideHelper clientSideHelper = HelperManager.Get(listBoxHandle);
			if(clientSideHelper == null) { gridHandle = IntPtr.Zero; elementInfo = new VerticalGridElementInfo(); return false; }
			int gridHandleAsInt;
			bool result = clientSideHelper.VerticalGridMethods.GetInnerElementInformationForCustomizationListBoxItem(listBoxHandle, itemIndex, out gridHandleAsInt, out elementInfo);
			gridHandle = new IntPtr(gridHandleAsInt);
			return result;
		}
		public int GetCustomizationListBoxItemIndex(IntPtr windowHandle, string innerElementName) {
			return HelperManager.Get(windowHandle) == null ? -1 : HelperManager.Get(windowHandle).VerticalGridMethods.GetCustomizationListBoxItemIndex(windowHandle, innerElementName);
		}
		public PropertyDescriptionVariableInfo GetPropertyDescriptionVariableInfo(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? new PropertyDescriptionVariableInfo() : HelperManager.Get(windowHandle).VerticalGridMethods.GetPropertyDescriptionVariableInfo(windowHandle);
		}
		public IntPtr GetCustomizationFormHandleOrShowIt(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? IntPtr.Zero : new IntPtr(HelperManager.Get(windowHandle).VerticalGridMethods.GetCustomizationFormHandleOrShowIt(windowHandle));
		}
		public IntPtr GetVerticalGridActiveEditorHandle(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? IntPtr.Zero : new IntPtr(HelperManager.Get(windowHandle).VerticalGridMethods.GetVerticalGridActiveEditorHandle(windowHandle));
		}
		public IntPtr GetVerticalGridActiveEditorHandleOrSetActiveEditor(IntPtr windowHandle, string rowName, int recordIndex, int cellIndex) {
			return HelperManager.Get(windowHandle) == null ? IntPtr.Zero : new IntPtr(HelperManager.Get(windowHandle).VerticalGridMethods.GetVerticalGridActiveEditorHandleOrSetActiveEditor(windowHandle, rowName, recordIndex, cellIndex));   
		}
		public string GetRowName(IntPtr windowHandle, int rowIndex, string categoryName) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).VerticalGridMethods.GetRowName(windowHandle, rowIndex, categoryName);
		}
		public string GetCategoryName(IntPtr windowHandle, int categoryIndex) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).VerticalGridMethods.GetCategoryName(windowHandle, categoryIndex);
		}
		public void ApplyVerticalGridElementVariableInfo(IntPtr windowHandle, VerticalGridElementInfo elementInfo, VerticalGridElementVariableInfo variableInfo) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).VerticalGridMethods.ApplyVerticalGridElementVariableInfo(windowHandle, elementInfo, variableInfo);
		}
		public string GetDragRowEffect(IntPtr windowHandle, string sourceRowName, ref string destRowName, int oldRowVisibleIndex, string oldParentRowName, out bool canMoveRow, out bool clearDragAction) {
			if(HelperManager.Get(windowHandle) == null) { canMoveRow = false; destRowName = null; clearDragAction = false; return null; }
			return HelperManager.Get(windowHandle).VerticalGridMethods.GetDragRowEffect(windowHandle, sourceRowName, ref destRowName, oldRowVisibleIndex, oldParentRowName, out canMoveRow, out clearDragAction);
		}
		public void DragRowAction(IntPtr windowHandle, string sourceRowName, string rowDestName, string parentRow, string dragRowEffect) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).VerticalGridMethods.DragRowAction(windowHandle, sourceRowName, rowDestName, parentRow, dragRowEffect);
		}
		public void OpenRightCustomizationTabPage(IntPtr windowHandle, string rowName) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).VerticalGridMethods.OpenRightCustomizationTabPage(windowHandle, rowName);
		}
		public bool GetVerticalGridFocus(IntPtr windowHandle, out string rowName, out int recordIndex, out int cellIndex) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) { rowName = null; recordIndex = cellIndex = -1; return false; }
			return clientSideHelper.VerticalGridMethods.GetVerticalGridFocus(windowHandle, out rowName, out recordIndex, out cellIndex);
		}
		public void SetVerticalGridFocus(IntPtr windowHandle, string rowName, int recordIndex, int cellIndex) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).VerticalGridMethods.SetVerticalGridFocus(windowHandle, rowName, recordIndex, cellIndex);
		}
		public string GetFocusedCellValueAsString(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).VerticalGridMethods.GetFocusedCellValueAsString(windowHandle);
		}
		public bool GetCellValue(IntPtr windowHandle, string rowName, int recordIndex, int cellIndex, out string value, out string valueType) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) { value = null; valueType = null; return false; }
			return clientSideHelper.VerticalGridMethods.GetCellValue(windowHandle, rowName, recordIndex, cellIndex, out value, out valueType);
		}
		public void PostEditorValue(IntPtr windowHandle) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).VerticalGridMethods.PostEditorValue(windowHandle);
		}
		public IntPtr GetGridActiveEditorHandleOrSetActiveEditor(IntPtr windowHandle, string rowName, int recordIndex, int cellIndex) {
			return IntPtr.Zero;
		}
		public void UpdateScrollBars(IntPtr windowHandle, int scrollCount, bool isVert) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).VerticalGridMethods.UpdateScrollBars(windowHandle, scrollCount, isVert);
		}
	}
}
