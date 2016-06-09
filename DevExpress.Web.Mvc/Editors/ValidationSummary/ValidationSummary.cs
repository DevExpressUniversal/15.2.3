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

using System.ComponentModel;
using System.Web.Mvc;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Mvc.UI;
	[ToolboxItem(false)]
	public class MVCxValidationSummary: ASPxValidationSummary {
		ViewContext viewContext;
		public MVCxValidationSummary()
			: this(null) {
		}
		protected internal MVCxValidationSummary(ViewContext viewContext)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
		}
		public new ValidationSummaryStyles Styles {
			get { return (ValidationSummaryStyles)base.Styles; }
		}
		protected override StylesBase CreateStyles() {
			return new ValidationSummaryStyles(this);
		}
		protected internal ViewContext ViewContext { get { return viewContext; } }
		protected internal Controller Controller {
			get {
				if(ViewContext == null)
					return null;
				return ViewContext.Controller as Controller;
			}
		}
		protected internal ModelStateDictionary ModelState {
			get { return Controller != null ? Controller.ModelState : null; }
		}
		protected internal void LoadErrors() {
			foreach(var error in EditorExtension.ErrorTexts) {
				AddError(error.Key);
			}
			LoadErrorsFromModelState();
		}
		void LoadErrorsFromModelState() {
			if(ModelState == null)
				return;
			foreach(var modelState in ModelState) {
				AddError(modelState.Key);
			}
		}
	}
}
