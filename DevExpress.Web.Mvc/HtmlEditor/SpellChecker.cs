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
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxHtmlEditor;
	using DevExpress.Web.ASPxHtmlEditor.Internal;
	using DevExpress.Web.ASPxSpellChecker;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	[ToolboxItem(false)]
	public class MVCxHtmlEditorSpellChecker : HtmlEditorSpellChecker {
		protected internal MVCxHtmlEditorSpellChecker(ASPxHtmlEditor editor)
			: base(editor) {
		}
		protected override Control CreateUserControl(string virtualPath) {
			DummyPage page = new DummyPage();
			return page.LoadControl(virtualPath);
		}
		protected override void PrepareUserControl(Control userControl, Control parent, string id, bool builtInControl) {
			DialogHelper.ForceOnInit(userControl);
			base.PrepareUserControl(userControl, parent, id, builtInControl);
			DialogHelper.ForceOnLoad(userControl);
		}
		public override bool IsLoading() {
			return false;
		}
	}
}
