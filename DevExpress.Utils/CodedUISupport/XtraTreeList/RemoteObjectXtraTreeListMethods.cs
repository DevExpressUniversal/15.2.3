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
	public class RemoteObjectXtraTreeListMethods : IXtraTreeListCodedUIHelper {
		ClientSideHelpersManager HelperManager;
		internal RemoteObjectXtraTreeListMethods(ClientSideHelpersManager manager) {
			this.HelperManager = manager;
		}
		public TreeListElementInfo GetTreeListElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return HelperManager.Get(windowHandle) == null ? new TreeListElementInfo() : HelperManager.Get(windowHandle).TreeListMethods.GetTreeListElementFromPoint(windowHandle, pointX, pointY);
		}
		public string GetTreeListElementRectangle(IntPtr windowHandle, TreeListElementInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).TreeListMethods.GetTreeListElementRectangle(windowHandle, elementInfo);
		}
		public TreeListElementVariableInfo GetTreeListElementVariableInfo(IntPtr windowHandle, TreeListElementInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? new TreeListElementVariableInfo() : HelperManager.Get(windowHandle).TreeListMethods.GetTreeListElementVariableInfo(windowHandle, elementInfo);
		}
		public void ApplyTreeListElementVariableInfo(IntPtr windowHandle, TreeListElementInfo elementInfo, TreeListElementVariableInfo variableInfo) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).TreeListMethods.ApplyTreeListElementVariableInfo(windowHandle, elementInfo, variableInfo);
		}
		public TreeListCustomizationListBoxItemInfo GetTreeListCustomizationListBoxItemInfo(IntPtr windowHandle, int pointX, int pointY) {
			return HelperManager.Get(windowHandle) == null ? new TreeListCustomizationListBoxItemInfo() : HelperManager.Get(windowHandle).TreeListMethods.GetTreeListCustomizationListBoxItemInfo(windowHandle, pointX, pointY);
		}
		public TreeListCustomizationListBoxItemInfo GetTreeListCustomizationListBoxItemInfo(IntPtr windowHandle, string columnName) {
			return HelperManager.Get(windowHandle) == null ? new TreeListCustomizationListBoxItemInfo() : HelperManager.Get(windowHandle).TreeListMethods.GetTreeListCustomizationListBoxItemInfo(windowHandle, columnName);
		}
		public string[] GetColumnsNames(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).TreeListMethods.GetColumnsNames(windowHandle);
		}
		public TreeListElementInfo GetSetFocusedCell(IntPtr windowHandle, TreeListElementInfo focusedCell) {
			return HelperManager.Get(windowHandle) == null ? new TreeListElementInfo() : HelperManager.Get(windowHandle).TreeListMethods.GetSetFocusedCell(windowHandle, focusedCell);
		}
		public IntPtr GetActiveEditorHandle(IntPtr treeListHandle) {
			return HelperManager.Get(treeListHandle) == null ? IntPtr.Zero : new IntPtr(HelperManager.Get(treeListHandle).TreeListMethods.GetActiveEditorHandle(treeListHandle));
		}
		public IntPtr GetActiveEditorHandleOrShowIt(IntPtr treeListHandle, TreeListElementInfo cellInfo) {
			return HelperManager.Get(treeListHandle) == null ? IntPtr.Zero : new IntPtr(HelperManager.Get(treeListHandle).TreeListMethods.GetActiveEditorHandleOrShowIt(treeListHandle, cellInfo));
		}
		public void MakeTreeListElementVisible(IntPtr windowHandle, TreeListElementInfo elementInfo) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).TreeListMethods.MakeTreeListElementVisible(windowHandle, elementInfo);
		}
		public TreeListElementInfo[] GetSelectedNodes(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).TreeListMethods.GetSelectedNodes(windowHandle);
		}
		public void SetSelectedNodes(IntPtr windowHandle, TreeListElementInfo[] nodesInfo) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).TreeListMethods.SetSelectedNodes(windowHandle, nodesInfo);
		}
		public IntPtr GetCustomizationFormHandle(IntPtr treeListHandle) {
			return HelperManager.Get(treeListHandle) == null ? IntPtr.Zero : new IntPtr(HelperManager.Get(treeListHandle).TreeListMethods.GetCustomizationFormHandle(treeListHandle));
		}
	}
}
