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
	public interface IXtraEditorsCodedUIHelper {
		int GetButtonEditButtonIndex(IntPtr buttonEditHandle, int x, int y);
		string GetButtonEditButtonRectangle(IntPtr buttonEditHandle, int index);
		int GetBaseListBoxControlItemIndex(IntPtr baseListBoxControlHandle, int x, int y);
		void SetCheckedComboBoxEditCheckedIndices(IntPtr windowHandle, int[] checkedIndices);
		void SetCheckedListBoxCheckedIndices(IntPtr windowHandle, int[] indices);
		string GetBaseListBoxControlItemRectangle(IntPtr baseListBoxControlHandle, int index);
		ZoomTrackBarButtons GetZoomTrackBarControlButton(IntPtr windowHandle, int x, int y);
		string GetZoomTrackBarControlButtonRectangle(IntPtr windowHandle, ZoomTrackBarButtons zoomTrackBarButton);
		void SetRangeTrackBarControlValue(IntPtr windowHandle, int min, int max);
		int GetNavigatorBaseButtonIndex(IntPtr windowHandle, int x, int y);
		string GetNavigatorBaseButtonRectangle(IntPtr windowHandle, int index);
		bool IsNavigatorBaseButtonEnabled(IntPtr windowHandle, int index);
		int GetRadioGroupItemIndex(IntPtr windowHandle, int x, int y);
		bool IsColorDialogOpeningClick(IntPtr windowHandle, int x, int y);
		IntPtr GetFilterControlActiveEditorHandleOrSetActiveEditor(IntPtr windowHandle, int nodeIndex, int elementIndex);
		void SetFilterControlFilterCriteria(IntPtr windowHandle, string criteriaString);
		int GetXtraTabControlHeaderIndex(IntPtr windowHandle, int x, int y);
		string GetXtraTabControlHeaderRectangle(IntPtr windowHandle, int pageIndex);
		bool IsXtraTabControlHeaderEnabled(IntPtr windowHandle, int pageIndex);
		void SetMRUEditItems(IntPtr windowHandle, string[] mRUItems);
		void SetColorEditColor(IntPtr windowHandle, string colorAsString);
		void ListenEditValueChanged(IntPtr windowHandle);
		int[] GetCheckedComboBoxEditCheckedIndices(IntPtr checkedComboBoxEditHandle, IntPtr listBoxHandle);
		IntPtr GetXtraTabControlPageHandleOrSetPageSelected(IntPtr windowHandle, string pageName);
		void SetDateTimeEditValue(IntPtr windowHandle, string valueAsString);
		void SetLookUpEditBaseValue(IntPtr windowHandle, ValueStruct value, bool byDisplayMember);
		int GetSetLookUpEditSelectedIndex(IntPtr windowHandle, int newValue, bool isSet);
		void SetListBoxSelectedItems(IntPtr windowHandle, string[] selectedItems);
		void SetCheckedComboBoxEditCheckedItems(IntPtr windowHandle, string[] checkedItems);
		void SetCheckedListBoxCheckedItems(IntPtr windowHandle, string[] checkedItems);
		int[] GetCheckedListBoxCheckedIndices(IntPtr windowHandle);
	}
}
