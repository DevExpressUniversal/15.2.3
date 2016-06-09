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
	public class RemoteObjectXtraPrintingMethods : IXtraPrintingCodedUIHelper {
		ClientSideHelpersManager HelperManager;
		internal RemoteObjectXtraPrintingMethods(ClientSideHelpersManager manager) {
			this.HelperManager = manager;
		}
		public PrintControlElements GetPrintingElementFromPoint(IntPtr windowHandle, int pointX, int pointY, out string path, out string sideMargin) {
			ClientSideHelper clientSideHelper = HelperManager.Get(windowHandle);
			if(clientSideHelper == null) {
				path = null;
				sideMargin = null;
				return PrintControlElements.Unknown;
			}
			PrintControlElements result = (PrintControlElements)clientSideHelper.PrintingMethods.GetPrintingElementFromPoint(windowHandle, pointX, pointY, out path, out sideMargin);
			return result;
		}
		public string GetPrintingElementRectangleOrMakeElementVisible(IntPtr windowHandle, PrintControlElements elementType, string path, string sideMargin) {
			return GetClientSideHelper(windowHandle) == null ? null : GetClientSideHelper(windowHandle).PrintingMethods.GetPrintingElementRectangleOrMakeElementVisible(windowHandle, elementType, path, sideMargin);
		}
		public string GetBrickType(IntPtr windowHandle, string path) {
			return GetClientSideHelper(windowHandle) == null ? null : GetClientSideHelper(windowHandle).PrintingMethods.GetBrickType(windowHandle, path);
		}
		public string GetBrickText(IntPtr windowHandle, string path) {
			return GetClientSideHelper(windowHandle) == null ? null : GetClientSideHelper(windowHandle).PrintingMethods.GetBrickText(windowHandle, path);
		}
		public string GetBrickCheckedState(IntPtr windowHandle, string path) {
			return GetClientSideHelper(windowHandle) == null ? null : GetClientSideHelper(windowHandle).PrintingMethods.GetBrickCheckedState(windowHandle, path);
		}
		public string GetMargin(IntPtr windowHandle, string marginSide) {
			return GetClientSideHelper(windowHandle) == null ? null : GetClientSideHelper(windowHandle).PrintingMethods.GetMargin(windowHandle, marginSide);
		}
		public void SetMargin(IntPtr windowHandle, string marginSide, string marginValueAsString) {
			if(GetClientSideHelper(windowHandle) == null) return;
			GetClientSideHelper(windowHandle).PrintingMethods.SetMargin(windowHandle, marginSide, marginValueAsString);
		}
		public String[] GetBricksOnPage(IntPtr windowHandle, int pageIndex) {
			return GetClientSideHelper(windowHandle) == null ? null : GetClientSideHelper(windowHandle).PrintingMethods.GetBricksOnPage(windowHandle, pageIndex);
		}
		public String[] GetInnerBricks(IntPtr windowHandle, string brickPath) {
			return GetClientSideHelper(windowHandle) == null ? null : GetClientSideHelper(windowHandle).PrintingMethods.GetInnerBricks(windowHandle, brickPath);
		}
		public bool GetBrickSelectedState(IntPtr windowHandle, string brickPath) {
			return GetClientSideHelper(windowHandle) == null ? false : GetClientSideHelper(windowHandle).PrintingMethods.GetBrickSelectedState(windowHandle, brickPath);
		}
		protected ClientSideHelper GetClientSideHelper(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle);
		}
	}
}
