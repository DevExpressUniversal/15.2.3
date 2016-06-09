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
namespace DevExpress.Utils.CodedUISupport {
	public class RemoteObject : System.MarshalByRefObject {
		Func<IntPtr, bool> reconnectFunc;
		public RemoteObject(Func<IntPtr, bool> reconnectFunc) {
			this.reconnectFunc = reconnectFunc;
		}
		public void AddHelper(object helper, int processId) {
			HelperManager.Add(helper as ClientSideHelper, processId);
		}
		protected ClientSideHelpersManager helperManagerInternal;
		protected ClientSideHelpersManager HelperManager {
			get {
				if(helperManagerInternal == null)
					helperManagerInternal = new ClientSideHelpersManager(reconnectFunc);
				return helperManagerInternal;
			}
		}
		public IServerSideHelper ServerSideHelper;
		public void AddServerSideHelper(object helper) {
			this.ServerSideHelper = helper as IServerSideHelper;
		}
		public bool IsConnected(IntPtr windowHandle) {
			return HelperManager.IsConnected(windowHandle);
		}
		public string GetPropertyFast(IntPtr handle, string propertyName) {
			return GetPropertyFast(handle, propertyName, null);
		}
		public string GetPropertyFast(IntPtr handle, string propertyName, string targetTypeName) {
			return HelperManager.Get(handle) == null ? null : HelperManager.Get(handle).GetPropertyFast(handle, propertyName, targetTypeName);
		}
		public void SetProperty(IntPtr handle, string propertyName, string propertyValue) {
			HandleViaReflection(handle, propertyName, propertyValue, null, null, true);
		}
		public string GetProperty(IntPtr handle, string propertyName) {
			return HandleViaReflection(handle, propertyName, null, null, null, false);
		}
		public void SetProperty(IntPtr handle, string propertyName, string propertyValue, int[] bindingFlags) {
			HandleViaReflection(handle, propertyName, propertyValue, null, bindingFlags, true);
		}
		public void SetProperty(IntPtr handle, string propertyName, string propertyValue, string newValueType, int[] bindingFlags) {
			HandleViaReflection(handle, propertyName, propertyValue, newValueType, bindingFlags, true);
		}
		public void SetProperty(IntPtr handle, string propertyName, string propertyValue, string newValueType) {
			HandleViaReflection(handle, propertyName, propertyValue, newValueType, null, true);
		}
		public string GetProperty(IntPtr handle, string propertyName, int[] bindingFlags) {
			return HandleViaReflection(handle, propertyName, null, null, bindingFlags, false);
		}
		private string GetSetPropertyDefault(IntPtr handle, string propertyName, string propertyValue, bool isSet) {
			return HandleViaReflection(handle, propertyName, propertyValue, null, null, isSet);
		}
		public string RunMethod(IntPtr windowHandle, string methodName, bool useBeginInvoke) {
			return HandleViaReflection(windowHandle, methodName, null, null, null, useBeginInvoke);
		}
		public string RunMethod(IntPtr windowHandle, string methodName, int[] bindingFlags, bool useBeginInvoke) {
			return HandleViaReflection(windowHandle, methodName, null, null, bindingFlags, useBeginInvoke);
		}
		public string HandleViaReflection(IntPtr windowHandle, string membersAsString, string memberValue, string memberValueType, int[] bindingFlags, bool isSet) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).HandleViaReflection(windowHandle, membersAsString, memberValue, memberValueType, bindingFlags, isSet);
		}
		public string GetClassName(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).GetClassName(windowHandle);
		}
		public IntPtr[] GetChildrenHandlesFromName(IntPtr parentHandle, string childName, bool contains, int searchDepth) {
			ClientSideHelper clientSideHelper = HelperManager.Get(parentHandle);
			if(clientSideHelper == null) return new IntPtr[] { };
			int[] result = clientSideHelper.GetChildrenHandlesFromName(parentHandle, childName, contains, searchDepth);
			if(result == null) return new IntPtr[] { };
			IntPtr[] childrenHandles = new IntPtr[result.Length];
			for(int index = 0; index < result.Length; index++)
				childrenHandles[index] = new IntPtr(result[index]);
			return childrenHandles;
		}
		public IntPtr GetParentHandle(IntPtr windowHandle, out int depth) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			depth = 0;
			if(clientSideHelper == null) return IntPtr.Zero;
			return new IntPtr(clientSideHelper.GetParentHandle(windowHandle, out depth));
		}
		public IntPtr[] GetChildrenHandlesFromClassName(IntPtr parentHandle, string className, bool contains, int searchDepth) {
			ClientSideHelper clientSideHelper = HelperManager.Get(parentHandle);
			if(clientSideHelper == null) return new IntPtr[] { };
			int[] result = clientSideHelper.GetChildrenHandlesFromClassName(parentHandle, className, contains, searchDepth);
			if(result == null) return new IntPtr[] { };
			IntPtr[] childrenHandles = new IntPtr[result.Length];
			for(int index = 0; index < result.Length; index++)
				childrenHandles[index] = new IntPtr(result[index]);
			return childrenHandles;
		}
		public IntPtr[] GetHandlesOfSiblingsWithSameClassName(IntPtr windowHandle, string className, int searchDepth) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return new IntPtr[] { };
			int[] result = clientSideHelper.GetHandlesOfSiblingsWithSameClassName(windowHandle, className, searchDepth);
			if(result == null) return new IntPtr[] { };
			IntPtr[] childrenHandles = new IntPtr[result.Length];
			for(int index = 0; index < result.Length; index++)
				childrenHandles[index] = new IntPtr(result[index]);
			return childrenHandles;
		}
		public IntPtr GetPopupHandleOrOpenPopup(IntPtr ownerHandle) {
			ClientSideHelper clientSideHelper = HelperManager.Get(ownerHandle);
			if(clientSideHelper == null) return IntPtr.Zero;
			int result = clientSideHelper.GetPopupHandleOrOpenPopup(ownerHandle);
			return new IntPtr(result);
		}
		public IntPtr GetChildHandleFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) return IntPtr.Zero;
			int result = clientSideHelper.GetChildHandleFromPoint(windowHandle, pointX, pointY);
			return new IntPtr(result);
		}
		public IntPtr[] GetAllChildrenHandles(IntPtr windowHandle, int searchDepth) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper != null) {
				int[] result = clientSideHelper.GetAllChildrenHandles(windowHandle, searchDepth);
				if(result != null) {
					IntPtr[] childrenHandles = new IntPtr[result.Length];
					for(int index = 0; index < result.Length; index++)
						childrenHandles[index] = new IntPtr(result[index]);
					return childrenHandles;
				}
			}
			return new IntPtr[] { };
		}
		public IntPtr[] GetClosestDXChildrenHandles(IntPtr windowHandle) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper != null) {
				int[] result = clientSideHelper.GetClosestDXChildrenHandles(windowHandle);
				if(result != null) {
					IntPtr[] childrenHandles = new IntPtr[result.Length];
					for(int index = 0; index < result.Length; index++)
						childrenHandles[index] = new IntPtr(result[index]);
					return childrenHandles;
				}
			}
			return new IntPtr[] { };
		}
		public AppearanceObjectSerializable GetAppearanceObject(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).GetAppearanceObject(windowHandle);
		}
		RemoteObjectXtraGridMethods gridMethods;
		public RemoteObjectXtraGridMethods GridMethods {
			get {
				if(gridMethods == null)
					gridMethods = new RemoteObjectXtraGridMethods(HelperManager);
				return gridMethods;
			}
		}
		RemoteObjectXtraEditorsMethods xtraEditorsMethods;
		public RemoteObjectXtraEditorsMethods XtraEditorsMethods {
			get {
				if(xtraEditorsMethods == null)
					xtraEditorsMethods = new RemoteObjectXtraEditorsMethods(HelperManager);
				return xtraEditorsMethods;
			}
		}
		RemoteObjectXtraBarsMethods xtraBarsMethods;
		public RemoteObjectXtraBarsMethods BarsMethods {
			get {
				if(xtraBarsMethods == null)
					xtraBarsMethods = new RemoteObjectXtraBarsMethods(HelperManager);
				return xtraBarsMethods;
			}
		}
		RemoteObjectXtraNavBarMethods xtraNavBarMethods;
		public RemoteObjectXtraNavBarMethods NavBarMethods {
			get {
				if(xtraNavBarMethods == null)
					xtraNavBarMethods = new RemoteObjectXtraNavBarMethods(HelperManager);
				return xtraNavBarMethods;
			}
		}
		RemoteObjectXtraPrintingMethods xtraPrintingMethods;
		public RemoteObjectXtraPrintingMethods PrintControlMethods {
			get {
				if(xtraPrintingMethods == null)
					xtraPrintingMethods = new RemoteObjectXtraPrintingMethods(HelperManager);
				return xtraPrintingMethods;
			}
		}
		RemoteObjectPivotGridMethods pivotGridMethods;
		public RemoteObjectPivotGridMethods PivotGridMethods {
			get {
				if(pivotGridMethods == null)
					pivotGridMethods = new RemoteObjectPivotGridMethods(HelperManager);
				return pivotGridMethods;
			}
		}
		RemoteObjectUtilsMethods utilsMethods;
		public RemoteObjectUtilsMethods UtilsMethods {
			get {
				if(utilsMethods == null)
					utilsMethods = new RemoteObjectUtilsMethods(HelperManager);
				return utilsMethods;
			}
		}
		RemoteObjectXtraTreeListMethods treeListMethods;
		public RemoteObjectXtraTreeListMethods TreeListMethods {
			get {
				if(treeListMethods == null)
					treeListMethods = new RemoteObjectXtraTreeListMethods(HelperManager);
				return treeListMethods;
			}
		}
		RemoteObjectXtraVerticalGridMethods verticalGridMethods;
		public RemoteObjectXtraVerticalGridMethods VerticalGridMethods {
			get {
				if(verticalGridMethods == null)
					verticalGridMethods = new RemoteObjectXtraVerticalGridMethods(HelperManager);
				return verticalGridMethods;
			}
		}
		RemoteObjectXtraLayoutMethods xtraLayoutMethods;
		public RemoteObjectXtraLayoutMethods XtraLayoutMethods {
			get {
				if(xtraLayoutMethods == null)
					xtraLayoutMethods = new RemoteObjectXtraLayoutMethods(HelperManager);
				return xtraLayoutMethods;
			}
		}
	}
}
