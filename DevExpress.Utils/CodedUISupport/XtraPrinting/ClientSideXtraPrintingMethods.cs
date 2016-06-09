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
	public class ClientSideXtraPrintingMethods : ClientSideHelperBase {
		internal ClientSideXtraPrintingMethods(RemoteObject remoteObject) : base(remoteObject) { }
		const string HelperTypeName = "DevExpress.XtraPrinting.CodedUISupport.XtraPrintingCodedUIHelper";
		IXtraPrintingCodedUIHelper xtraPrintingCodedUIHelper;
		IXtraPrintingCodedUIHelper Helper {
			get {
				if(xtraPrintingCodedUIHelper == null)
					xtraPrintingCodedUIHelper = this.GetHelper<IXtraPrintingCodedUIHelper>(HelperTypeName, AssemblyInfo.SRAssemblyPrinting);
				return xtraPrintingCodedUIHelper;
			}
		}
		public int GetPrintingElementFromPoint(IntPtr windowHandle, int pointX, int pointY, out string path, out string sideMargin) {
			string _path = null, _sideMargin = null;
			int result = RunClientSideMethod(Helper, windowHandle, delegate() {
				return (int)Helper.GetPrintingElementFromPoint(windowHandle, pointX, pointY, out _path, out _sideMargin);
			});
			path = _path;
			sideMargin = _sideMargin;
			return result;
		}
		public string GetPrintingElementRectangleOrMakeElementVisible(IntPtr windowHandle, PrintControlElements elementType, string path, string sideMargin) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetPrintingElementRectangleOrMakeElementVisible(windowHandle, elementType, path, sideMargin);
			});
		}
		public string GetBrickType(IntPtr windowHandle, string path) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBrickType(windowHandle, path);
			});
		}
		public string GetBrickText(IntPtr windowHandle, string path) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBrickText(windowHandle, path);
			});
		}
		public string GetBrickCheckedState(IntPtr windowHandle, string path) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBrickCheckedState(windowHandle, path);
			});
		}
		public string GetMargin(IntPtr windowHandle, string marginSide) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetMargin(windowHandle, marginSide);
			});
		}
		public void SetMargin(IntPtr windowHandle, string marginSide, string marginValueAsString) {
			RunClientSideMethod(Helper, windowHandle, delegate() {
				Helper.SetMargin(windowHandle, marginSide, marginValueAsString);
			});
		}
		public String[] GetBricksOnPage(IntPtr windowHandle, int pageIndex) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBricksOnPage(windowHandle, pageIndex);
			});
		}
		public String[] GetInnerBricks(IntPtr windowHandle, string brickPath) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetInnerBricks(windowHandle, brickPath);
			});
		}
		public bool GetBrickSelectedState(IntPtr windowHandle, string brickPath) {
			return RunClientSideMethod(Helper, windowHandle, delegate() {
				return Helper.GetBrickSelectedState(windowHandle, brickPath);
			});
		}
	}
}
