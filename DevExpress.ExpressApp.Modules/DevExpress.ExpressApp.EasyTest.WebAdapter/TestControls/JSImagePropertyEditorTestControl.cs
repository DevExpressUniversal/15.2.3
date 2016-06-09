#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.Text;
using DevExpress.EasyTest.Framework;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using mshtml;
using System.Threading;
using DevExpress.EasyTest.Framework.Utils;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Commands;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls {
	public class JSImagePropertyEditorTestControl : JSStandartTestControl, IControlAct {
		public JSImagePropertyEditorTestControl(IControlDescription controlDescription) : base(controlDescription) { }
		public void Act(string value) {
			if(string.IsNullOrEmpty(value)) {
				bool isBrowseVisible = (bool)ExecuteFunction("IsBrowseVisible", new object[] { });
				if(isBrowseVisible) {
					value = "Browse";
				}
			}
			if(value == "Browse") {
				int left = (int)ExecuteFunction("GetBrowseButtonPosition", new object[] { 0 });
				int top = (int)ExecuteFunction("GetBrowseButtonPosition", new object[] { 1 });
				ExecuteFunction("StartObserveMouseMove", new object[] { });
				ITestControl browserTestControl = Adapter.CreateTestControl(TestControlType.Dialog, "");
				System.Threading.Thread.Sleep(1000);
				browserTestControl.GetInterface<IControlMouse>().MouseMove(left, top);
				System.Threading.Thread.Sleep(500);
				int mX = (int)ExecuteFunction("MousDistanceToControl", new object[] { 0 });
				int mY = (int)ExecuteFunction("MousDistanceToControl", new object[] { 1 });
				browserTestControl.GetInterface<IControlMouse>().MouseClick(left - mX, top - mY);
				System.Threading.Thread.Sleep(3000);
				ExecuteFunction("EndObserveMouseMove", new object[] { });
			}
			else {
				ExecuteFunction("Act", new string[] { value });
			}
		}
		protected override string ScriptText {
			get {
				return ReadScriptFormResource("DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls.JSImagePropertyEditorTestControl.js");
			}
		}
		protected override string JSControlName {
			get { return "ImagePropertyEditorTestControl_JS"; }
		}
		protected override string RegisterControlType {
			get { return "DevExpress.ExpressApp.Web.Editors.ASPx.ASPxImagePropertyEditor"; }
		}
	}
}
