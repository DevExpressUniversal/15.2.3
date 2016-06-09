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
	public class ClientSideXtraVerticalGridMethods : ClientSideHelperBase {
		internal ClientSideXtraVerticalGridMethods(RemoteObject remoteObject) : base(remoteObject) { }
		const string HelperTypeName = "DevExpress.XtraVerticalGrid.CodedUISupport.XtraVerticalGridCodedUIHelper";
		IXtraVerticalGridCodedUIHelper xtraVerticalGridCodedUIHelper;
		IXtraVerticalGridCodedUIHelper Helper {
			get {
				if(xtraVerticalGridCodedUIHelper == null)
					xtraVerticalGridCodedUIHelper = this.GetHelper<IXtraVerticalGridCodedUIHelper>(HelperTypeName, AssemblyInfo.SRAssemblyVertGrid);
				return xtraVerticalGridCodedUIHelper;
			}
		}
		public VerticalGridElementInfo GetVerticalGridElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetVerticalGridElementFromPoint(windowHandle, pointX, pointY);
			});
		}
		public string GetVerticalGridElementRectangle(IntPtr windowHandle, VerticalGridElementInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetVerticalGridElementRectangle(windowHandle, elementInfo);
			});
		}
		public VerticalGridElementVariableInfo GetVerticalGridElementVariableInfo(IntPtr windowHandle, VerticalGridElementInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetVerticalGridElementVariableInfo(windowHandle, elementInfo);
			});
		}
		public bool GetInnerElementInformationForCustomizationListBoxItem(IntPtr listBoxHandle, int itemIndex, out int gridHandleAsInt, out VerticalGridElementInfo elementInfo) {
			VerticalGridElementInfo _elementInfo = new VerticalGridElementInfo();
			IntPtr gridHandle = IntPtr.Zero;
			bool result = RunClientSideMethod(Helper, listBoxHandle, delegate() {
				return Helper.GetInnerElementInformationForCustomizationListBoxItem(listBoxHandle, itemIndex, out gridHandle, out _elementInfo);
			});
			gridHandleAsInt = gridHandle.ToInt32();
			elementInfo = _elementInfo;
			return result;
		}
		public int GetCustomizationListBoxItemIndex(IntPtr windowHandle, string innerElementName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetCustomizationListBoxItemIndex(windowHandle, innerElementName);
			});
		}
		public PropertyDescriptionVariableInfo GetPropertyDescriptionVariableInfo(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetPropertyDescriptionVariableInfo(windowHandle);
			});
		}
		public int GetCustomizationFormHandleOrShowIt(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetCustomizationFormHandleOrShowIt(windowHandle).ToInt32();
			});
		}
		public int GetVerticalGridActiveEditorHandle(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetVerticalGridActiveEditorHandle(windowHandle).ToInt32();
			});
		}
		public int GetVerticalGridActiveEditorHandleOrSetActiveEditor(IntPtr windowHandle, string rowName, int recordIndex, int cellIndex) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetVerticalGridActiveEditorHandleOrSetActiveEditor(windowHandle, rowName, recordIndex, cellIndex).ToInt32();
			});
		}
		public string GetRowName(IntPtr windowHandle, int rowIndex, string categoryName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetRowName(windowHandle, rowIndex, categoryName);
			});
		}
		public string GetCategoryName(IntPtr windowHandle, int categoryIndex) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetCategoryName(windowHandle, categoryIndex);
			});
		}
		public void ApplyVerticalGridElementVariableInfo(IntPtr windowHandle, VerticalGridElementInfo elementInfo, VerticalGridElementVariableInfo variableInfo) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.ApplyVerticalGridElementVariableInfo(windowHandle, elementInfo, variableInfo);
			});
		}
		public string GetDragRowEffect(IntPtr windowHandle, string sourceRowName, ref string destRowName, int oldRowVisibleIndex, string oldParentRowName, out bool canMoveRow, out bool clearDragAction) {
			bool _canMoveRow = false;
			bool _clearDragAction = false;
			string _destRowName = destRowName;
			string result = RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetDragRowEffect(windowHandle, sourceRowName, ref _destRowName, oldRowVisibleIndex, oldParentRowName, out _canMoveRow, out _clearDragAction);
			});
			destRowName = _destRowName;
			canMoveRow = _canMoveRow;
			clearDragAction = _clearDragAction;
			return result;
		}
		public void DragRowAction(IntPtr windowHandle, string sourceRowName, string rowDestName, string parentRow, string dragRowEffect) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.DragRowAction(windowHandle, sourceRowName, rowDestName, parentRow, dragRowEffect);
			});
		}
		public void OpenRightCustomizationTabPage(IntPtr windowHandle, string rowName) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.OpenRightCustomizationTabPage(windowHandle, rowName);
			});
		}
		public bool GetVerticalGridFocus(IntPtr windowHandle, out string rowName, out int recordIndex, out int cellIndex) {
			int _recordIndex = -1, _cellIndex = -1;
			string _rowName = null;
			bool result = RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetVerticalGridFocus(windowHandle, out _rowName, out _recordIndex, out _cellIndex);
			});
			recordIndex = _recordIndex;
			cellIndex = _cellIndex;
			rowName = _rowName;
			return result;
		}
		public void SetVerticalGridFocus(IntPtr windowHandle, string rowName, int recordIndex, int cellIndex) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetVerticalGridFocus(windowHandle, rowName, recordIndex, cellIndex);
			});
		}
		public string GetFocusedCellValueAsString(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetFocusedCellValueAsString(windowHandle);
			});
		}
		public bool GetCellValue(IntPtr windowHandle, string rowName, int recordIndex, int cellIndex, out string value, out string valueType) {
			string _value = null;
			string _valueType = null;
			bool result = RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetCellValue(windowHandle, rowName, recordIndex, cellIndex, out _value, out _valueType);
			});
			value = _value;
			valueType = _valueType;
			return result;
		}
		public void PostEditorValue(IntPtr windowHandle) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.PostEditorValue(windowHandle);
			});
		}
		public void UpdateScrollBars(IntPtr windowHandle, int scrollCount, bool isVert) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.UpdateScrollBars(windowHandle, scrollCount, isVert);
			});
		}
	}
}
