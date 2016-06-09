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
	public class ClientSideXtraLayoutMethods : ClientSideHelperBase {
		internal ClientSideXtraLayoutMethods(RemoteObject remoteObject) : base(remoteObject) { }
		const string HelperTypeName = "DevExpress.XtraLayout.CodedUISupport.XtraLayoutCodedUIHelper";
		IXtraLayoutCodedUIHelper xtraLayoutCodedUIHelper;
		IXtraLayoutCodedUIHelper Helper {
			get {
				if(xtraLayoutCodedUIHelper == null)
					xtraLayoutCodedUIHelper = this.GetHelper<IXtraLayoutCodedUIHelper>(HelperTypeName, AssemblyInfo.SRAssemblyLayoutControl);
				return xtraLayoutCodedUIHelper;
			}
		}
		public LayoutControlElementInfo GetLayoutControlElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetLayoutControlElementFromPoint(windowHandle, pointX, pointY);
			});
		}
		public string GetLayoutControlElementRectangle(IntPtr windowHandle, LayoutControlElementInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetLayoutControlElementRectangle(windowHandle, elementInfo);
			});
		}
		public LayoutControlElementInfo GetControlParentItem(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetControlParentItem(windowHandle);
			});
		}
		public int GetLayoutControlItemControlHandle(IntPtr windowHandle, LayoutControlElementInfo elementInfo) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetLayoutControlItemControlHandle(windowHandle, elementInfo).ToInt32();
			});
		}
		public ValueStruct GetLayoutControlElementProperty(IntPtr windowHandle, LayoutControlElementInfo elementInfo, LayoutControlElementsProperties property) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetLayoutControlElementProperty(windowHandle, elementInfo, property);
			});
		}
		public void MakeVisible(IntPtr windowHandle, LayoutControlElementInfo elementInfo) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.MakeVisible(windowHandle, elementInfo);
			});
		}
	}
}
