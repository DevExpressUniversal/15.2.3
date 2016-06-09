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
	public class RemoteObjectPivotGridMethods : IPivotGridCodedUIHelper {
		ClientSideHelpersManager HelperManager;
		internal RemoteObjectPivotGridMethods(ClientSideHelpersManager manager) {
			this.HelperManager = manager;
		}
		public PivotGridElementInfo GetPivotGridElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return HelperManager.Get(windowHandle) == null ? new PivotGridElementInfo() : HelperManager.Get(windowHandle).PivotGridMethods.GetPivotGridElementFromPoint(windowHandle, pointX, pointY);
		}
		public string GetPivotGridElementRectangle(IntPtr windowHandle, PivotGridElementInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).PivotGridMethods.GetPivotGridElementRectangle(windowHandle, elementInfo);
		}
		public void UpdatePivotGridElementInfo(IntPtr windowHandle, ref PivotGridElementInfo elementInfo) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).PivotGridMethods.UpdatePivotGridElementInfo(windowHandle, ref elementInfo);
		}
		public void SetPivotGridElementProperty(IntPtr windowHandle, PivotGridElementInfo elementInfo, PivotElementPropertiesForSet propertyForSet) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).PivotGridMethods.SetPivotGridElementProperty(windowHandle, elementInfo, propertyForSet);
		}
		public PivotGridElementInfo GetCustomizationListBoxItemInfo(IntPtr listBoxHandle, int pointX, int pointY) {
			return HelperManager.Get(listBoxHandle) == null ? new PivotGridElementInfo() : HelperManager.Get(listBoxHandle).PivotGridMethods.GetCustomizationListBoxItemInfo(listBoxHandle, pointX, pointY);
		}
		public PivotGridElementInfo GetCustomizationListBoxItemInfo(IntPtr listBoxHandle, string fieldName) {
			return HelperManager.Get(listBoxHandle) == null ? new PivotGridElementInfo() : HelperManager.Get(listBoxHandle).PivotGridMethods.GetCustomizationListBoxItemInfo(listBoxHandle, fieldName);
		}
		public IntPtr GetCustomizationFormHandle(IntPtr pivotHandle) {
			return HelperManager.Get(pivotHandle) == null ? IntPtr.Zero : new IntPtr(HelperManager.Get(pivotHandle).PivotGridMethods.GetCustomizationFormHandle(pivotHandle));
		}
		public IntPtr GetActiveEditorHandle(IntPtr pivotHandle) {
			return HelperManager.Get(pivotHandle) == null ? IntPtr.Zero : new IntPtr(HelperManager.Get(pivotHandle).PivotGridMethods.GetActiveEditorHandle(pivotHandle));
		}
		public IntPtr GetActiveEditorHandleOrMakeItVisible(IntPtr pivotHandle, int columnIndex, int rowIndex) {
			return HelperManager.Get(pivotHandle) == null ? IntPtr.Zero : new IntPtr(HelperManager.Get(pivotHandle).PivotGridMethods.GetActiveEditorHandleOrMakeItVisible(pivotHandle, columnIndex, rowIndex));
		}
		public int GetPivotFieldValueMinLastLevelIndex(IntPtr pivotHandle, PivotGridElementInfo fieldValueInfo) {
			return HelperManager.Get(pivotHandle) == null ? 0 : HelperManager.Get(pivotHandle).PivotGridMethods.GetPivotFieldValueMinLastLevelIndex(pivotHandle, fieldValueInfo);
		}
	}
}
