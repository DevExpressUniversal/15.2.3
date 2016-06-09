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
	public class RemoteObjectXtraEditorsMethods : IXtraEditorsCodedUIHelper {
		ClientSideHelpersManager HelperManager;
		internal RemoteObjectXtraEditorsMethods(ClientSideHelpersManager manager) {
			this.HelperManager = manager;
		}
		public int GetButtonEditButtonIndex(IntPtr buttonEditHandle, int x, int y) {
			ClientSideHelper clientSideHelper = HelperManager.Get(buttonEditHandle);
			if(clientSideHelper == null) return -1;
			int index = clientSideHelper.EditorsMethods.GetButtonEditButtonIndex(buttonEditHandle, x, y);
			return index;
		}
		public string GetButtonEditButtonRectangle(IntPtr buttonEditHandle, int index) {
			ClientSideHelper clientSideHelper = HelperManager.Get(buttonEditHandle);
			if(clientSideHelper == null) return null;
			string result = clientSideHelper.EditorsMethods.GetButtonEditButtonRectangle(buttonEditHandle, index);
			return result;
		}
		public int GetBaseListBoxControlItemIndex(IntPtr baseListBoxControlHandle, int x, int y) {
			ClientSideHelper clientSideHelper = HelperManager.Get(baseListBoxControlHandle);
			if(clientSideHelper == null) return -1;
			int index = clientSideHelper.EditorsMethods.GetBaseListBoxControlItemIndex(baseListBoxControlHandle, x, y);
			return index;
		}
		public string GetBaseListBoxControlItemRectangle(IntPtr baseListBoxControlHandle, int index) {
			ClientSideHelper clientSideHelper = HelperManager.Get(baseListBoxControlHandle);
			if(clientSideHelper == null) return null;
			string result = clientSideHelper.EditorsMethods.GetBaseListBoxControlItemRectangle(baseListBoxControlHandle, index);
			return result;
		}
		public void SetCheckedComboBoxEditCheckedIndices(IntPtr windowHandle, int[] checkedIndices) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.EditorsMethods.SetCheckedComboBoxEditCheckedIndices(windowHandle, checkedIndices);
		}
		public void SetCheckedListBoxCheckedIndices(IntPtr windowHandle, int[] indices) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.EditorsMethods.SetCheckedListBoxCheckedIndices(windowHandle, indices);
		}
		public ZoomTrackBarButtons GetZoomTrackBarControlButton(IntPtr windowHandle, int x, int y) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return ZoomTrackBarButtons.None;
			int zoomTrackBarButton = clientSideHelper.EditorsMethods.GetZoomTrackBarControlButton(windowHandle, x, y);
			if(zoomTrackBarButton == (int)ZoomTrackBarButtons.ZoomIn || zoomTrackBarButton == (int)ZoomTrackBarButtons.ZoomOut) return (ZoomTrackBarButtons)zoomTrackBarButton;
			return ZoomTrackBarButtons.None;
		}
		public string GetZoomTrackBarControlButtonRectangle(IntPtr windowHandle, ZoomTrackBarButtons zoomTrackBarButton) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return null;
			string result = clientSideHelper.EditorsMethods.GetZoomTrackBarControlButtonRectangle(windowHandle, (int)zoomTrackBarButton);
			return result;
		}
		public void SetRangeTrackBarControlValue(IntPtr windowHandle, int min, int max) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.EditorsMethods.SetRangeTrackBarControlValue(windowHandle, min, max);
		}
		public int GetNavigatorBaseButtonIndex(IntPtr windowHandle, int x, int y) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return -1;
			int index = clientSideHelper.EditorsMethods.GetNavigatorBaseButtonIndex(windowHandle, x, y);
			return index;
		}
		public string GetNavigatorBaseButtonRectangle(IntPtr windowHandle, int index) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return null;
			string result = clientSideHelper.EditorsMethods.GetNavigatorBaseButtonRectangle(windowHandle, index);
			return result;
		}
		public bool IsNavigatorBaseButtonEnabled(IntPtr windowHandle, int index) {
			return HelperManager.Get(windowHandle) == null ? false : HelperManager.Get(windowHandle).EditorsMethods.IsNavigatorBaseButtonEnabled(windowHandle, index);
		}
		public int GetRadioGroupItemIndex(IntPtr windowHandle, int x, int y) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return -1;
			int index = clientSideHelper.EditorsMethods.GetRadioGroupItemIndex(windowHandle, x, y);
			return index;
		}
		public bool IsColorDialogOpeningClick(IntPtr windowHandle, int x, int y) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return false;
			bool result = clientSideHelper.EditorsMethods.IsColorDialogOpeningClick(windowHandle, x, y);
			return result;
		}
		public IntPtr GetFilterControlActiveEditorHandleOrSetActiveEditor(IntPtr windowHandle, int nodeIndex, int elementIndex) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return IntPtr.Zero;
			IntPtr result = new IntPtr(clientSideHelper.EditorsMethods.GetFilterControlActiveEditorHandleOrSetActiveEditor(windowHandle, nodeIndex, elementIndex));
			return result;
		}
		public void SetFilterControlFilterCriteria(IntPtr windowHandle, string criteriaString) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.EditorsMethods.SetFilterControlFilterCriteria(windowHandle, criteriaString);
		}
		public int GetXtraTabControlHeaderIndex(IntPtr windowHandle, int x, int y) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return -1;
			return clientSideHelper.EditorsMethods.GetXtraTabControlHeaderIndex(windowHandle, x, y);
		}
		public string GetXtraTabControlHeaderRectangle(IntPtr windowHandle, int pageIndex) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return null;
			return clientSideHelper.EditorsMethods.GetXtraTabControlHeaderRectangle(windowHandle, pageIndex);
		}
		public bool IsXtraTabControlHeaderEnabled(IntPtr windowHandle, int pageIndex) {
			return HelperManager.Get(windowHandle) == null ? false : HelperManager.Get(windowHandle).EditorsMethods.IsXtraTabControlHeaderEnabled(windowHandle, pageIndex);
		}
		public void SetMRUEditItems(IntPtr windowHandle, string[] mRUItems) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.EditorsMethods.SetMRUEditItems(windowHandle, mRUItems);
		}
		public void SetColorEditColor(IntPtr windowHandle, string colorAsString) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.EditorsMethods.SetColorEditColor(windowHandle, colorAsString);
		}
		public void ListenEditValueChanged(IntPtr windowHandle) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return;
			clientSideHelper.EditorsMethods.ListenEditValueChanged(windowHandle);
		}
		public int[] GetCheckedComboBoxEditCheckedIndices(IntPtr checkedComboBoxEditHandle, IntPtr listBoxHandle) {
			ClientSideHelper clientSideHelper = HelperManager.Get(checkedComboBoxEditHandle);
			if(clientSideHelper == null) return null;
			return clientSideHelper.EditorsMethods.GetCheckedComboBoxEditCheckedIndices(checkedComboBoxEditHandle, listBoxHandle);
		}
		public IntPtr GetXtraTabControlPageHandleOrSetPageSelected(IntPtr windowHandle, string pageName) {
			return HelperManager.Get(windowHandle) == null ? IntPtr.Zero : new IntPtr(HelperManager.Get(windowHandle).EditorsMethods.GetXtraTabControlPageHandleOrSetPageSelected(windowHandle, pageName));
		}
		public void SetDateTimeEditValue(IntPtr windowHandle, string valueAsString) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).EditorsMethods.SetDateTimeEditValue(windowHandle, valueAsString);
		}
		public void SetLookUpEditBaseValue(IntPtr windowHandle, ValueStruct value, bool byDisplayMember) {
			ClientSideHelper csh = HelperManager.Get(windowHandle);
			if(csh != null) {
				if(csh.GridMethods.AreGridMethodsLoaded())
					csh.GridMethods.SetLookUpEditBaseValue(windowHandle, value, byDisplayMember);
				else csh.EditorsMethods.SetLookUpEditBaseValue(windowHandle, value, byDisplayMember);
			}
		}
		public int GetSetLookUpEditSelectedIndex(IntPtr windowHandle, int newValue, bool isSet) {
			return HelperManager.Get(windowHandle) == null ? -1 : HelperManager.Get(windowHandle).EditorsMethods.GetSetLookUpEditSelectedIndex(windowHandle, newValue, isSet);
		}
		public void SetListBoxSelectedItems(IntPtr windowHandle, string[] selectedItems) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).EditorsMethods.SetListBoxSelectedItems(windowHandle, selectedItems);
		}
		public void SetCheckedComboBoxEditCheckedItems(IntPtr windowHandle, string[] checkedItems) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).EditorsMethods.SetCheckedComboBoxEditCheckedItems(windowHandle, checkedItems);
		}
		public void SetCheckedListBoxCheckedItems(IntPtr windowHandle, string[] checkedItems) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).EditorsMethods.SetCheckedListBoxCheckedItems(windowHandle, checkedItems);
		}
		public int[] GetCheckedListBoxCheckedIndices(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? new int[] { } : HelperManager.Get(windowHandle).EditorsMethods.GetCheckedListBoxCheckedIndices(windowHandle);
		}
	}
}
