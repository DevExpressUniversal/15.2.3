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
	public class ClientSideXtraTreeListMethods : ClientSideHelperBase {
		internal ClientSideXtraTreeListMethods(RemoteObject remoteObject) : base(remoteObject) { }
		const string HelperTypeName = "DevExpress.XtraTreeList.CodedUISupport.XtraTreeListCodedUIHelper";
		IXtraTreeListCodedUIHelper xtraTreeListCodedUIHelper;
		IXtraTreeListCodedUIHelper Helper {
			get {
				if(xtraTreeListCodedUIHelper == null)
					xtraTreeListCodedUIHelper = this.GetHelper<IXtraTreeListCodedUIHelper>(HelperTypeName, AssemblyInfo.SRAssemblyTreeList);
				return xtraTreeListCodedUIHelper;
			}
		}
		public TreeListElementInfo GetTreeListElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetTreeListElementFromPoint(windowHandle, pointX, pointY);
			});
		}
		public string GetTreeListElementRectangle(IntPtr windowHandle, TreeListElementInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetTreeListElementRectangle(windowHandle, elementInfo);
			});
		}
		public TreeListElementVariableInfo GetTreeListElementVariableInfo(IntPtr windowHandle, TreeListElementInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetTreeListElementVariableInfo(windowHandle, elementInfo);
			});
		}
		public void ApplyTreeListElementVariableInfo(IntPtr windowHandle, TreeListElementInfo elementInfo, TreeListElementVariableInfo variableInfo) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.ApplyTreeListElementVariableInfo(windowHandle, elementInfo, variableInfo);
			});
		}
		public TreeListCustomizationListBoxItemInfo GetTreeListCustomizationListBoxItemInfo(IntPtr windowHandle, int pointX, int pointY) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetTreeListCustomizationListBoxItemInfo(windowHandle, pointX, pointY);
			});
		}
		public TreeListCustomizationListBoxItemInfo GetTreeListCustomizationListBoxItemInfo(IntPtr windowHandle, string columnName) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetTreeListCustomizationListBoxItemInfo(windowHandle, columnName);
			});
		}
		public string[] GetColumnsNames(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetColumnsNames(windowHandle);
			});
		}
		public TreeListElementInfo GetSetFocusedCell(IntPtr windowHandle, TreeListElementInfo focusedCell) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetSetFocusedCell(windowHandle, focusedCell);
			});
		}
		public int GetActiveEditorHandle(IntPtr treeListHandle) {
			return RunClientSideMethod(Helper, treeListHandle, delegate() {
				return Helper.GetActiveEditorHandle(treeListHandle).ToInt32();
			});
		}
		public int GetActiveEditorHandleOrShowIt(IntPtr treeListHandle, TreeListElementInfo cellInfo) {
			return RunClientSideMethod(Helper, treeListHandle, delegate() {
				return Helper.GetActiveEditorHandleOrShowIt(treeListHandle, cellInfo).ToInt32();
			});
		}
		public void MakeTreeListElementVisible(IntPtr windowHandle, TreeListElementInfo elementInfo) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.MakeTreeListElementVisible(windowHandle, elementInfo);
			});
		}
		public TreeListElementInfo[] GetSelectedNodes(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetSelectedNodes(windowHandle);
			});
		}
		public void SetSelectedNodes(IntPtr windowHandle, TreeListElementInfo[] nodesInfo) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetSelectedNodes(windowHandle, nodesInfo);
			});
		}
		public int GetCustomizationFormHandle(IntPtr treeListHandle) {
			return RunClientSideMethod(Helper, treeListHandle, delegate() {
				return Helper.GetCustomizationFormHandle(treeListHandle).ToInt32();
			});
		}
	}
}
