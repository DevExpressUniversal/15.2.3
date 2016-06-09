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
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.Mvc {
	using DevExpress.Utils;
	using DevExpress.Web.ASPxSpellChecker;
	using DevExpress.XtraSpellChecker;
	using System.Globalization;
	public class SpellCheckerSettings : SettingsBase {
		public SpellCheckerSettings() {
			EnableCallbackCompression = true;
			LevenshteinDistance = 4;
			RightToLeft = DefaultBoolean.Default;
			SuggestionCount = 5;
			SaveStateToCookiesID = string.Empty;
			SettingsSpelling = new ASPxSpellCheckerSpellingSettings(null);
			SettingsText = new ASPxSpellCheckerTextSettings(null);
			SettingsLoadingPanel = new SettingsLoadingPanel(null);
			SettingsForms = new MVCxSpellCheckerFormsSettings(null);
			SettingsDialogFormElements = new SpellCheckerDialogSettings(null);
			Dictionaries = new MVCxSpellCheckerDictionaryCollection();
			ImagesEditors = new SpellCheckerEditorImages(null);
			StylesButton = new SpellCheckerButtonStyles(null);
			StylesDialogForm = new SpellCheckerDialogFormStyles(null);
		}
		public object CallbackRouteValues { get; set; }
		public SpellCheckerClientSideEvents ClientSideEvents { get { return (SpellCheckerClientSideEvents)ClientSideEventsInternal; } }
		public bool EnableCallbackCompression { get; set; }
		public int LevenshteinDistance { get; set; }
		public int SuggestionCount { get; set; }
		public bool SaveStateToCookies { get; set; }
		public string SaveStateToCookiesID { get; set; }
		public ASPxSpellCheckerSpellingSettings SettingsSpelling { get; private set; }
		public ASPxSpellCheckerTextSettings SettingsText { get; private set; }
		public SettingsLoadingPanel SettingsLoadingPanel { get; private set; }
		public MVCxSpellCheckerFormsSettings SettingsForms { get; private set; }
		public SpellCheckerDialogSettings SettingsDialogFormElements { get; private set; }
		public MVCxSpellCheckerDictionaryCollection Dictionaries { get; private set; }
		public CultureInfo Culture { get; set; }
		public string CheckedElementID { get; set; }
		public DefaultBoolean RightToLeft { get; set; }
		public SpellCheckerImages Images { get { return (SpellCheckerImages)ImagesInternal; } }
		public SpellCheckerEditorImages ImagesEditors { get; private set; }
		public SpellCheckerStyles Styles { get { return (SpellCheckerStyles)StylesInternal; } }
		public SpellCheckerButtonStyles StylesButton { get; private set; }
		public SpellCheckerDialogFormStyles StylesDialogForm { get; private set; }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		public ASPxClientLayoutHandler ClientLayout { get; set; }
		public PrepareSuggestionsEventHandler PrepareSuggestions { get; set; }
		public NotInDictionaryWordFoundEventHandler NotInDictionaryWordFound { get; set; }
		public WordAddedEventHandler WordAdded { get; set; } 
		public CustomDictionaryLoadingEventHandler CustomDictionaryLoading { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new SpellCheckerClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new SpellCheckerImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new SpellCheckerStyles(null);
		}
	}
}
