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
	public class ClientSideXtraNavBarMethods : ClientSideHelperBase {
		internal ClientSideXtraNavBarMethods(RemoteObject remoteObject) : base(remoteObject) { }
		const string HelperTypeName = "DevExpress.XtraNavBar.CodedUISupport.XtraNavBarCodedUIHelper";
		IXtraNavBarCodedUIHelper xtraNavBarCodedUIHelper;
		IXtraNavBarCodedUIHelper Helper {
			get {
				if(xtraNavBarCodedUIHelper == null)
					xtraNavBarCodedUIHelper = this.GetHelper<IXtraNavBarCodedUIHelper>(HelperTypeName, AssemblyInfo.SRAssemblyNavBar);
				return xtraNavBarCodedUIHelper;
			}
		}
		public NavBarControlElements GetNavBarElementFromPoint(IntPtr windowHandle, int pointX, int pointY, out string groupCaption, out string itemCaption) {
			string _groupCaption = null, _itemCaption = null;
			NavBarControlElements result = RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetNavBarElementFromPoint(windowHandle, pointX, pointY, out _groupCaption, out _itemCaption);
			});
			groupCaption = _groupCaption;
			itemCaption = _itemCaption;
			return result;
		}
		public string GetNavBarElementRectangleOrMakeElementVisible(IntPtr windowHandle, NavBarControlElements elementType, string groupCaption, string itemCaption) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetNavBarElementRectangleOrMakeElementVisible(windowHandle, elementType, groupCaption, itemCaption);
			});
		}
		public void MakeNavBarElementVisible(IntPtr windowHandle, NavBarControlElements elementType, string groupCaption, string itemCaption) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.MakeNavBarElementVisible(windowHandle, elementType, groupCaption, itemCaption);
			});
		}
		public string[] GetGroupsCaptions(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetGroupsCaptions(windowHandle);
			});
		}
		public string[] GetGroupCaptionsInOverflowPanel(IntPtr windowHandle) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetGroupCaptionsInOverflowPanel(windowHandle);
			});
		}
		public string[] GetLinksCaptions(IntPtr windowHandle, string groupCaption) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetLinksCaptions(windowHandle, groupCaption);
			});
		}
		public string GetNavBarGroupExpandedState(IntPtr windowHandle, string groupCaption) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetNavBarGroupExpandedState(windowHandle, groupCaption);
			});
		}
	}
}
