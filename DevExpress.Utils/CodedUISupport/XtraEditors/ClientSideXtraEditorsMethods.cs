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
	public class ClientSideXtraEditorsMethods : ClientSideHelperBase {
		internal ClientSideXtraEditorsMethods(RemoteObject remoteObject) : base(remoteObject) { }
		const string HelperTypeName = "DevExpress.XtraEditors.CodedUISupport.XtraEditorsCodedUIHelper";
		IXtraEditorsCodedUIHelper xtraEditorsCodedUIHelper;
		IXtraEditorsCodedUIHelper Helper {
			get {
				if(xtraEditorsCodedUIHelper == null)
					xtraEditorsCodedUIHelper = this.GetHelper<IXtraEditorsCodedUIHelper>(HelperTypeName, AssemblyInfo.SRAssemblyEditors);
				return xtraEditorsCodedUIHelper;
			}
		}
		public int GetButtonEditButtonIndex(IntPtr windowHandle, int x, int y) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetButtonEditButtonIndex(windowHandle, x, y);
			});
		}
		public string GetButtonEditButtonRectangle(IntPtr windowHandle, int index) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetButtonEditButtonRectangle(windowHandle, index);
			});
		}
		public int GetBaseListBoxControlItemIndex(IntPtr windowHandle, int x, int y) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBaseListBoxControlItemIndex(windowHandle, x, y);
			});
		}
		public string GetBaseListBoxControlItemRectangle(IntPtr windowHandle, int index) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBaseListBoxControlItemRectangle(windowHandle, index);
			});
		}
		public void SetCheckedComboBoxEditCheckedIndices(IntPtr windowHandle, int[] checkedIndices) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetCheckedComboBoxEditCheckedIndices(windowHandle, checkedIndices);
			});
		}
		public void SetCheckedListBoxCheckedIndices(IntPtr windowHandle, int[] indices) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetCheckedListBoxCheckedIndices(windowHandle, indices);
			});
		}
		public int GetZoomTrackBarControlButton(IntPtr windowHandle, int x, int y) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return (int)Helper.GetZoomTrackBarControlButton(windowHandle, x, y);
			});
		}
		public string GetZoomTrackBarControlButtonRectangle(IntPtr windowHandle, int zoomTrackBarButton) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetZoomTrackBarControlButtonRectangle(windowHandle, (ZoomTrackBarButtons)zoomTrackBarButton);
			});
		}
		public void SetRangeTrackBarControlValue(IntPtr windowHandle, int min, int max) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetRangeTrackBarControlValue(windowHandle, min, max);
			});
		}
		public int GetNavigatorBaseButtonIndex(IntPtr windowHandle, int x, int y) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetNavigatorBaseButtonIndex(windowHandle, x, y);
			});
		}
		public string GetNavigatorBaseButtonRectangle(IntPtr windowHandle, int index) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetNavigatorBaseButtonRectangle(windowHandle, index);
			});
		}
		public bool IsNavigatorBaseButtonEnabled(IntPtr windowHandle, int index) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.IsNavigatorBaseButtonEnabled(windowHandle, index);
			});
		}
		public int GetRadioGroupItemIndex(IntPtr windowHandle, int x, int y) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetRadioGroupItemIndex(windowHandle, x, y);
			});
		}
		public bool IsColorDialogOpeningClick(IntPtr windowHandle, int x, int y) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.IsColorDialogOpeningClick(windowHandle, x, y);
			});
		}
		public int GetFilterControlActiveEditorHandleOrSetActiveEditor(IntPtr windowHandle, int nodeIndex, int elementIndex) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetFilterControlActiveEditorHandleOrSetActiveEditor(windowHandle, nodeIndex, elementIndex).ToInt32();
			});
		}
		public void SetFilterControlFilterCriteria(IntPtr windowHandle, string criteriaString) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetFilterControlFilterCriteria(windowHandle, criteriaString);
			});
		}
		public int GetXtraTabControlHeaderIndex(IntPtr windowHandle, int x, int y) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetXtraTabControlHeaderIndex(windowHandle, x, y);
			});
		}
		public string GetXtraTabControlHeaderRectangle(IntPtr windowHandle, int pageIndex) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetXtraTabControlHeaderRectangle(windowHandle, pageIndex);
			});
		}
		public bool IsXtraTabControlHeaderEnabled(IntPtr windowHandle, int pageIndex) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.IsXtraTabControlHeaderEnabled(windowHandle, pageIndex);
			});
		}
		public void SetMRUEditItems(IntPtr windowHandle, string[] mRUItems) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetMRUEditItems(windowHandle, mRUItems);
			});
		}
		public void SetColorEditColor(IntPtr windowHandle, string colorAsString) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetColorEditColor(windowHandle, colorAsString);
			});
		}
		public void ListenEditValueChanged(IntPtr windowHandle) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.ListenEditValueChanged(windowHandle);
			});
		}
		public int[] GetCheckedComboBoxEditCheckedIndices(IntPtr checkedComboBoxEditHandle, IntPtr listBoxHandle) {
			return RunClientSideMethod(Helper, checkedComboBoxEditHandle, delegate() {
				return Helper.GetCheckedComboBoxEditCheckedIndices(checkedComboBoxEditHandle, listBoxHandle);
			});
		}
		public int GetXtraTabControlPageHandleOrSetPageSelected(IntPtr windowHandle, string pageName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetXtraTabControlPageHandleOrSetPageSelected(windowHandle, pageName).ToInt32();
			});
		}
		public void SetDateTimeEditValue(IntPtr windowHandle, string valueAsString) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetDateTimeEditValue(windowHandle, valueAsString);
			});
		}
		public void SetLookUpEditBaseValue(IntPtr windowHandle, ValueStruct value, bool byDisplayMember) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetLookUpEditBaseValue(windowHandle, value, byDisplayMember);
			});
		}
		public int GetSetLookUpEditSelectedIndex(IntPtr windowHandle, int newValue, bool isSet) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetSetLookUpEditSelectedIndex(windowHandle, newValue, isSet);
			});
		}
		public void SetListBoxSelectedItems(IntPtr windowHandle, string[] selectedItems) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetListBoxSelectedItems(windowHandle, selectedItems);
			});
		}
		public void SetCheckedComboBoxEditCheckedItems(IntPtr windowHandle, string[] checkedItems) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetCheckedComboBoxEditCheckedItems(windowHandle, checkedItems);
			});
		}
		public void SetCheckedListBoxCheckedItems(IntPtr windowHandle, string[] checkedItems) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetCheckedListBoxCheckedItems(windowHandle, checkedItems);
			});
		}
		public int[] GetCheckedListBoxCheckedIndices(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetCheckedListBoxCheckedIndices(windowHandle);
			});
		}
	}
}
