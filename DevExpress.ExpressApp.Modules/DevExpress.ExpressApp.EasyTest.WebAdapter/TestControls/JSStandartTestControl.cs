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
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls {
	public class JSStandartTestControl : BaseWebTestControl, IControlText, IControlEnabled {
		public JSStandartTestControl(IControlDescription controlDescription) : base(controlDescription) { }
		protected override string ScriptText {
			get {
				return ReadScriptFormResource("DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls.JSStandartTestControl.js");
			}
		}
		protected override string JSControlName {
			get { return "StandartTestControl_JS"; }
		}
		protected override string RegisterControlType {
			get { return "System.Web.UI.WebControls.WebControl"; }
		}
		#region IControlText Members
		public virtual string Text {
			get {
				return ExecuteFunction("GetText", null) as string;
			}
			set {
				ExecuteFunction("SetText", new object[] { value });
			}
		}
		#endregion
		#region IControlEnabled Members
		public virtual bool Enabled {
			get {
				object result = ExecuteFunction("IsEnabled", null);
				return result is bool ? (bool)result : false;
			}
		}
		#endregion
	}
}
