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

using System.ComponentModel;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Actions {
	[ToolboxItem(false)]
	public class ActionUrl : ActionBase {
		private string urlFormatString;
		private string textFormatString;
		private string textFieldName;
		private bool openInNewWindow = true;
		private string urlFieldName;
		private void SetSelectionDependency(string value) {
			if(string.IsNullOrEmpty(value)) {
				SelectionDependencyType = SelectionDependencyType.Independent;
			}
			else {
				SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			}
		}
		public ActionUrl(Controller owner, string id, string category)
			: base(owner, id, category) {
			SelectionDependencyType = SelectionDependencyType.Independent;
		}
		protected ActionUrl(Controller owner, string id, PredefinedCategory category)
			: this(owner, id, category.ToString()) { }
		protected internal override void RaiseExecute(ActionBaseEventArgs eventArgs) {
		}
		public ActionUrl(IContainer container)
			: base(container) {
			SelectionDependencyType = SelectionDependencyType.Independent;
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionUrlUrlFieldName"),
#endif
 Category("Url")]
		public string UrlFieldName {
			get { return urlFieldName; }
			set {
				urlFieldName = value;
				SetSelectionDependency(value);
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionUrlUrlFormatString"),
#endif
 Category("Url")]
		public string UrlFormatString {
			get { return urlFormatString; }
			set { urlFormatString = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionUrlTextFormatString"),
#endif
 Category("Url")]
		public string TextFormatString {
			get { return textFormatString; }
			set { textFormatString = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionUrlTextFieldName"),
#endif
 Category("Url")]
		public string TextFieldName {
			get { return textFieldName; }
			set {
				textFieldName = value;
				SetSelectionDependency(value);
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionUrlOpenInNewWindow"),
#endif
 Category("Url"), DefaultValue(true)]
		public bool OpenInNewWindow {
			get { return openInNewWindow; }
			set { openInNewWindow = value; }
		}
	}
}
