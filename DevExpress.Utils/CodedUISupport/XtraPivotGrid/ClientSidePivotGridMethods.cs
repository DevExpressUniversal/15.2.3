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
	public class ClientSidePivotGridMethods : ClientSideHelperBase {
		internal ClientSidePivotGridMethods(RemoteObject remoteObject) : base(remoteObject) { }
		const string HelperTypeName = "DevExpress.XtraPivotGrid.CodedUISupport.PivotGridCodedUIHelper";
		IPivotGridCodedUIHelper pivotGridCodedUIHelper;
		IPivotGridCodedUIHelper Helper {
			get {
				if(pivotGridCodedUIHelper == null)
					pivotGridCodedUIHelper = this.GetHelper<IPivotGridCodedUIHelper>(HelperTypeName, AssemblyInfo.SRAssemblyPivotGrid);
				return pivotGridCodedUIHelper;
			}
		}
		public PivotGridElementInfo GetPivotGridElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetPivotGridElementFromPoint(windowHandle, pointX, pointY);
			});
		}
		public string GetPivotGridElementRectangle(IntPtr windowHandle, PivotGridElementInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetPivotGridElementRectangle(windowHandle, elementInfo);
			});
		}
		public void UpdatePivotGridElementInfo(IntPtr windowHandle, ref PivotGridElementInfo elementInfo) {
			PivotGridElementInfo _elementInfo = elementInfo;
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.UpdatePivotGridElementInfo(windowHandle, ref _elementInfo);
			});
			elementInfo = _elementInfo;
		}
		public void SetPivotGridElementProperty(IntPtr windowHandle, PivotGridElementInfo elementInfo, PivotElementPropertiesForSet propertyForSet) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetPivotGridElementProperty(windowHandle, elementInfo, propertyForSet);
			});
		}
		public PivotGridElementInfo GetCustomizationListBoxItemInfo(IntPtr listBoxHandle, int pointX, int pointY) {
			return RunClientSideMethod(Helper, listBoxHandle, delegate() {
				return Helper == null ? new PivotGridElementInfo() : Helper.GetCustomizationListBoxItemInfo(listBoxHandle, pointX, pointY);
			});
		}
		public PivotGridElementInfo GetCustomizationListBoxItemInfo(IntPtr listBoxHandle, string fieldName) {
			return RunClientSideMethod(Helper, listBoxHandle, delegate() {
				return Helper.GetCustomizationListBoxItemInfo(listBoxHandle, fieldName);
			});
		}
		public int GetCustomizationFormHandle(IntPtr pivotHandle) {
			return RunClientSideMethod(Helper, pivotHandle, delegate() {
				return Helper.GetCustomizationFormHandle(pivotHandle).ToInt32();
			});
		}
		public int GetActiveEditorHandle(IntPtr pivotHandle) {
			return RunClientSideMethod(Helper, pivotHandle, delegate() {
				return Helper.GetActiveEditorHandle(pivotHandle).ToInt32();
			});
		}
		public int GetActiveEditorHandleOrMakeItVisible(IntPtr pivotHandle, int columnIndex, int rowIndex) {
			return RunClientSideMethod(Helper, pivotHandle, delegate() {
				return Helper.GetActiveEditorHandleOrMakeItVisible(pivotHandle,  columnIndex, rowIndex).ToInt32();
			});
		}
		public int GetPivotFieldValueMinLastLevelIndex(IntPtr pivotHandle, PivotGridElementInfo fieldValueInfo) {
			return RunClientSideMethod(Helper, pivotHandle, delegate() {
				return Helper.GetPivotFieldValueMinLastLevelIndex(pivotHandle, fieldValueInfo);
			});
		}
	}
}
