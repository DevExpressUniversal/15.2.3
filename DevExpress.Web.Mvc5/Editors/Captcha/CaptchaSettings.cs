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

using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Captcha;
namespace DevExpress.Web.Mvc {
	public class CaptchaSettings: SettingsBase {
		public CaptchaSettings() {
			ClientVisible = true;
			CharacterSet = MVCxCaptcha.DefaultCharacterSetPropertyValue;
			CodeLength = MVCxCaptcha.DefaultCodeLengthPropertyValue;
			RightToLeft = DefaultBoolean.Default;
			LoadingPanelImage = new ImageProperties();
			ChallengeImage = new CaptchaImageProperties(null);
			LoadingPanelStyle = new LoadingPanelStyle();
			ValidationSettings = new CaptchaValidationSettings();
			RefreshButton = new RefreshButtonProperties(null);
			TextBox = new CaptchaTextBoxProperties(null);
			LoadingPanel = new SettingsLoadingPanel(null);
		}
		public object CallbackRouteValues { get; set; }
		public CallbackClientSideEventsBase ClientSideEvents { get { return (CallbackClientSideEventsBase)ClientSideEventsInternal; } }
		public bool ClientVisible { get; set; }
		public string CharacterSet { get; set; }
		public int CodeLength { get; set; }
		public bool EnableCallbackAnimation { get; set; }
		public RefreshButtonProperties RefreshButton { get; private set; }
		public CaptchaTextBoxProperties TextBox { get; private set; }
		public SettingsLoadingPanel LoadingPanel { get; private set; }
		public CaptchaImages Images { get { return (CaptchaImages)ImagesInternal; } }
		public ImageProperties LoadingPanelImage { get; private set; }
		public CaptchaImageProperties ChallengeImage { get; private set; }
		public CaptchaStyles Styles { get { return (CaptchaStyles)StylesInternal; } }
		public LoadingPanelStyle LoadingPanelStyle { get; private set; }
		public CaptchaValidationSettings ValidationSettings { get; private set; }
		public DefaultBoolean RightToLeft { get; set; }
		public ChallengeImageCustomRenderEventHandler ChallengeImageCustomRender { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new CallbackClientSideEventsBase();
		}
		protected override ImagesBase CreateImages() {
			return new CaptchaImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new CaptchaStyles(null);
		}
	}
}
