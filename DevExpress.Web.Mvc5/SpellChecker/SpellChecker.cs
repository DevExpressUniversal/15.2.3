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
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	using System.IO;
	using System.Web.Mvc;
	using DevExpress.Web.ASPxSpellChecker;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	[ToolboxItem(false)]
	public class MVCxSpellChecker : ASPxSpellChecker, IViewContext {
		ViewContext viewContext;
		public MVCxSpellChecker()
			: this(null) {
		}
		protected internal MVCxSpellChecker(ViewContext viewContext)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
		}
		public object CallbackRouteValues { get; set; }
		public SpellCheckerImages Images { get { return (SpellCheckerImages)base.ImagesInternal; } }
		public new SpellCheckerStyles Styles { get { return base.Styles; } }
		public override bool IsCallback { get { return MvcUtils.CallbackName == ClientID; } }
		protected string CallbackDialogAction { get; set; }
		protected ViewContext ViewContext { get { return viewContext; } }
		protected internal ControllerBase Controller {
			get { return (ViewContext != null) ? ViewContext.Controller : null; }
		}
		protected internal ControllerContext ControllerContext {
			get { return (Controller != null) ? Controller.ControllerContext : null; }
		}
		protected internal TextWriter Writer {
			get { return (ViewContext != null) ? ViewContext.Writer : Utils.Writer; }
		}
		protected override Control CreateUserControl(string virtualPath) {
			DummyPage page = new DummyPage();
			return page.LoadControl(virtualPath);
		}
		protected override void PrepareUserControl(Control userControl, System.Web.UI.Control parent, string id, bool builtInUserControl) {
			DialogHelper.ForceOnInit(userControl);
			base.PrepareUserControl(userControl, parent, id, builtInUserControl);
			DialogHelper.ForceOnLoad(userControl);
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientSpellChecker";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxSpellChecker), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxSpellChecker), Utils.SpellCheckerScriptResourceName);
		}
		protected override SpellCheckerFormsSettings CreateSettingsForms() {
			return new MVCxSpellCheckerFormsSettings(this);
		}
		protected override Control CreateDialogControl(string controlName, Control parent) {
			string dialogAction = ((MVCxSpellCheckerFormsSettings)SettingsForms).GetFormAction(controlName);
			Control dialogControl;
			if(string.IsNullOrEmpty(dialogAction)) {
				dialogControl = CreateDefaultForm(controlName);
				PrepareUserControl(dialogControl, parent, controlName, true);
			} else {
				CallbackDialogAction = dialogAction;
				dialogControl = new LiteralControl(Utils.CallbackHtmlContentPlaceholder);
			}
			return dialogControl;
		}
		protected internal void RenderCallbackResultControl() {
			if(string.IsNullOrEmpty(CallbackDialogAction))
				return;
			if(Writer != null) {
				CustomActionInvoker c = new CustomActionInvoker();
				c.InvokeAction(ControllerContext, CallbackDialogAction);
				Writer.Write(c.ActionResult);
			}
		}
		#region IViewContext Members
		ViewContext IViewContext.ViewContext { get { return ViewContext; } }
		#endregion
	}
}
