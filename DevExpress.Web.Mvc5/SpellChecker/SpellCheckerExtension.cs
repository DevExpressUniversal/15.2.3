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

using System.Web.Mvc;
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	public class SpellCheckerExtension: ExtensionBase {
		static Control callbackResultDummyControl = new Control();
		public SpellCheckerExtension(SpellCheckerSettings settings)
			: base(settings) {
		}
		public SpellCheckerExtension(SpellCheckerSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new SpellCheckerSettings Settings {
			get { return (SpellCheckerSettings)base.Settings; }
			set { base.Settings = value; }
		}
		protected internal new MVCxSpellChecker Control {
			get { return (MVCxSpellChecker)base.Control; }
			set { base.Control = value; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.EnableCallbackCompression = Settings.EnableCallbackCompression;
			Control.LevenshteinDistance = Settings.LevenshteinDistance;
			Control.SuggestionCount = Settings.SuggestionCount;
			Control.SaveStateToCookies = Settings.SaveStateToCookies;
			Control.SaveStateToCookiesID = Settings.SaveStateToCookiesID;
			Control.SettingsSpelling.Assign(Settings.SettingsSpelling);
			Control.SettingsText.Assign(Settings.SettingsText);
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.SettingsForms.Assign(Settings.SettingsForms);
			Control.SettingsDialogFormElements.Assign(Settings.SettingsDialogFormElements);
			Control.Dictionaries.Assign(Settings.Dictionaries);
			Control.Culture = Settings.Culture;
			Control.CheckedElementID = Settings.CheckedElementID;
			Control.Images.Assign(Settings.Images);
			Control.ImagesEditors.Assign(Settings.ImagesEditors);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.StylesButton.CopyFrom(Settings.StylesButton);
			Control.StylesDialogForm.CopyFrom(Settings.StylesDialogForm);
			Control.RightToLeft = Settings.RightToLeft;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.ClientLayout += Settings.ClientLayout;
			Control.PrepareSuggestions += Settings.PrepareSuggestions;
			Control.NotInDictionaryWordFound += Settings.NotInDictionaryWordFound;
			Control.WordAdded += Settings.WordAdded;
			Control.CustomDictionaryLoading += Settings.CustomDictionaryLoading;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxSpellChecker(ViewContext);
		}
		protected override Control GetCallbackResultControl() {
			return callbackResultDummyControl;
		}
		protected override void RenderCallbackResultControl() {
			Control.RenderCallbackResultControl();
		}
	}
}
