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
	using DevExpress.Web.Mvc.Internal;
	public class MVCxHtmlEditorCustomDialog : HtmlEditorCustomDialog {
		public MVCxHtmlEditorCustomDialog() : base() { }
		public MVCxHtmlEditorCustomDialog(string name, string caption)
			: base(name, caption) {
		}
		public MVCxHtmlEditorCustomDialog(string name, string caption, string formAction)
			: base(name, caption, formAction) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string FormPath {
			get { return base.FormPath; }
			set { base.FormPath = value; }
		}
		public string FormAction {
			get { return base.FormPath; }
			set { base.FormPath = value; }
		}
	}
	public class MVCxHtmlEditorCustomDialogs : HtmlEditorCustomDialogs {
		public new MVCxHtmlEditorCustomDialog this[int index] {
			get { return (GetItem(index) as MVCxHtmlEditorCustomDialog); }
		}
		protected internal MVCxHtmlEditorCustomDialogs()
			: base() {
		}
		protected internal MVCxHtmlEditorCustomDialogs(ASPxHtmlEditor owner)
			: base(owner) {
		}
		public void Add(Action<MVCxHtmlEditorCustomDialog> method) {
			method(Add());
		}
		public new MVCxHtmlEditorCustomDialog Add() {
			return (MVCxHtmlEditorCustomDialog)AddInternal(new MVCxHtmlEditorCustomDialog());
		}
		public new MVCxHtmlEditorCustomDialog Add(string name, string caption) {
			return (MVCxHtmlEditorCustomDialog)AddInternal(new MVCxHtmlEditorCustomDialog(name, caption));
		}
		public new MVCxHtmlEditorCustomDialog Add(string name, string caption, string formAction) {
			return (MVCxHtmlEditorCustomDialog)AddInternal(new MVCxHtmlEditorCustomDialog(name, caption, formAction));
		}
	}
}
namespace DevExpress.Web.Mvc.Internal {
	using DevExpress.Web.ASPxHtmlEditor.Internal;
	[ToolboxItem(false)]
	public class MVCxHtmlEditorMediaFileSelector : ASPxHtmlEditorMediaFileSelector {
		public MVCxHtmlEditorMediaFileSelector()
			: base() {
		}
		protected override ASPxFileManager CreateFileManager() {
			return new MVCxHtmlEditorFileManager();
		}
		protected override ASPxUploadControl CreateUploadControl() {
			return new MVCxHtmlEditorUploadControl();
		}
	}
}
