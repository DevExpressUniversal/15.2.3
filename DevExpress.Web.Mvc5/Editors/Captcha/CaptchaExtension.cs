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
using DevExpress.Web.Internal;
using System.ComponentModel;
using System.Web.Mvc;
using System.Linq;
namespace DevExpress.Web.Mvc {
	public class CaptchaExtension: ExtensionBase {
		public CaptchaExtension(CaptchaSettings settings)
			: base(settings) {
		}
		public CaptchaExtension(CaptchaSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxCaptcha Control {
			get { return (MVCxCaptcha)base.Control; }
		}
		protected internal new CaptchaSettings Settings {
			get { return (CaptchaSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ClientVisible = Settings.ClientVisible;
			Control.CharacterSet = Settings.CharacterSet;
			Control.CodeLength = Settings.CodeLength;
			Control.EnableCallbackAnimation = Settings.EnableCallbackAnimation;
			Control.RefreshButton.Assign(Settings.RefreshButton);
			Control.TextBox.Assign(Settings.TextBox);
			Control.LoadingPanel.Assign(Settings.LoadingPanel);
			Control.Images.Assign(Settings.Images);
			Control.LoadingPanelImage.Assign(Settings.LoadingPanelImage);
			Control.ChallengeImage.Assign(Settings.ChallengeImage);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.LoadingPanelStyle.CopyFrom(Settings.LoadingPanelStyle);
			Control.ValidationSettings.Assign(Settings.ValidationSettings);
			Control.RightToLeft = Settings.RightToLeft;
			Control.ChallengeImageCustomRender += Settings.ChallengeImageCustomRender;
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			if (MVCxCaptcha.IsValidateControl(Settings.Name))
				Control.ValidateSubmittedText(HttpUtils.GetRequest().Params);
		}
		public static string GetCode(string extensionName) {
			var codeSessionKey = MVCxCaptcha.GetCodeSessionKey(extensionName);
			var session = HttpUtils.GetSession();
			return session != null ? (string)session[codeSessionKey] : null;
		}
		public static bool GetIsValid(string extensionName) {
			var editorPostDataKey = MVCxCaptcha.GetEditorPostDataKey(extensionName);
			return HttpUtils.GetValueFromRequest(editorPostDataKey) == GetCode(extensionName);
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxCaptcha();
		}
	}
}
