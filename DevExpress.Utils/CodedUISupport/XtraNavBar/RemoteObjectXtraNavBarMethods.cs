﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	public class RemoteObjectXtraNavBarMethods : IXtraNavBarCodedUIHelper {
		ClientSideHelpersManager HelperManager;
		internal RemoteObjectXtraNavBarMethods(ClientSideHelpersManager manager) {
			this.HelperManager = manager;
		}
		public NavBarControlElements GetNavBarElementFromPoint(IntPtr windowHandle, int pointX, int pointY, out string groupCaption, out string itemCaption) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) { groupCaption = itemCaption = null; return NavBarControlElements.Unknown; }
			return clientSideHelper.NavBarMethods.GetNavBarElementFromPoint(windowHandle, pointX, pointY, out groupCaption, out itemCaption);
		}
		public string GetNavBarElementRectangleOrMakeElementVisible(IntPtr windowHandle, NavBarControlElements elementType, string groupCaption, string itemCaption) {
			return GetClientSideHelper(windowHandle) == null ? null : GetClientSideHelper(windowHandle).NavBarMethods.GetNavBarElementRectangleOrMakeElementVisible(windowHandle, elementType, groupCaption, itemCaption);
		}
		public void MakeNavBarElementVisible(IntPtr windowHandle, NavBarControlElements elementType, string groupCaption, string itemCaption) {
			if(GetClientSideHelper(windowHandle) != null)
				GetClientSideHelper(windowHandle).NavBarMethods.MakeNavBarElementVisible(windowHandle, elementType, groupCaption, itemCaption);
		}
		public string[] GetGroupsCaptions(IntPtr windowHandle) {
			return GetClientSideHelper(windowHandle) == null ? null : GetClientSideHelper(windowHandle).NavBarMethods.GetGroupsCaptions(windowHandle);
		}
		protected ClientSideHelper GetClientSideHelper(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle);
		}
		public string[] GetGroupCaptionsInOverflowPanel(IntPtr windowHandle) {
			return GetClientSideHelper(windowHandle) == null ? null : GetClientSideHelper(windowHandle).NavBarMethods.GetGroupCaptionsInOverflowPanel(windowHandle);
		}
		public string[] GetLinksCaptions(IntPtr windowHandle, string groupCaption) {
			return GetClientSideHelper(windowHandle) == null ? null : GetClientSideHelper(windowHandle).NavBarMethods.GetLinksCaptions(windowHandle, groupCaption);
		}
		public string GetNavBarGroupExpandedState(IntPtr windowHandle, string groupCaption) {
			return GetClientSideHelper(windowHandle) == null ? null : GetClientSideHelper(windowHandle).NavBarMethods.GetNavBarGroupExpandedState(windowHandle, groupCaption);
		}
	}
}
