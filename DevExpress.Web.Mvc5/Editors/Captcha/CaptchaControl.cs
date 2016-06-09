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

using DevExpress.Web;
using DevExpress.Web.Captcha;
using System.ComponentModel;
using System.Linq;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	using System.Collections.Specialized;
	[ToolboxItem(false)]
	public class MVCxCaptcha: ASPxCaptcha {
		public MVCxCaptcha()
			: base() {
		}
		protected internal static int DefaultCodeLengthPropertyValue { get { return ASPxCaptcha.DefaultCodeLength; } }
		protected internal static int DefaultMinCodeLengthPropertyValue { get { return ASPxCaptcha.MinCodeLength; } }
		protected internal static int DefaultEditorWidthPropertyValue { get { return ASPxCaptcha.DefaultEditorWidth; } }
		protected internal static string DefaultCharacterSetPropertyValue { get { return ASPxCaptcha.DefaultCharacterSet; } }
		public new CaptchaImages Images { get { return base.Images; } }
		public new CaptchaStyles Styles { get { return base.Styles; } }
		public new bool EnableCallBacks { get { return true; } }
		public object CallbackRouteValues { get; set; }
		public override bool IsCallback {
			get { return !string.IsNullOrEmpty(MvcUtils.CallbackName) && MvcUtils.CallbackName == ID; }
		}
		protected internal static string GetCodeSessionKey(string id) {
			return GetCodeSessionKey(id, null);
		}
		protected internal static string GetEditorPostDataKey(string id) {
			return GetEditorPostDataKey(id, '$');
		}
		protected internal new void ValidateSubmittedText(NameValueCollection postCollection) {
			base.ValidateSubmittedText(postCollection);
		}
		protected override bool IsValidateTextBox() {
			return IsValidateControl(ID);
		}
		protected internal static bool IsValidateControl(string id) {
			var captchaPostDataKey = MVCxCaptcha.GetEditorPostDataKey(id);
			return HttpUtils.GetRequest().Params.AllKeys.Contains(captchaPostDataKey);
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientCaptcha";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxCaptcha), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxCaptcha), Utils.CaptchaScriptResourceName);
		}
	}
}
