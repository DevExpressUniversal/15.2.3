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
using DevExpress.Web;
using DevExpress.Web.ASPxHtmlEditor;
using System.ComponentModel;
using Microsoft.SharePoint;
using DevExpress.Web;
using System.Web.UI;
using System.Collections.Generic;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Collections;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint.WebPartPages;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using System.Web;
using System.IO;
using System.Threading;
using DevExpress.SharePoint.Internal;
using System.Globalization;
using DevExpress.Web.ASPxSpellChecker;
using System.Text;
namespace DevExpress.SharePoint {
	public class SPxHtmlEditor : ASPxHtmlEditor {
		private static string[] DisableBasicFormattingCommands = new string[] { "bold", "italic", "underline", "strikethrough" ,"fontsize", 
			"fontname", "forecolor", "backcolor", "justifycenter", "justifyleft", "justifyright", "justifyfull", "indent", "outdent" };
		private bool disableBasicFormattingButtons = false;
		public SPxHtmlEditor()
			: base() {
			EnableViewState = false;
			InitializeSpellCheckerSettings();
		}
		public bool AllowHtmlSourceEditing {
			get { return Settings.AllowHtmlView; }
			set { Settings.AllowHtmlView = value; }
		}
		public bool DisableBasicFormattingButtons {
			get { return disableBasicFormattingButtons; }
			set {
				disableBasicFormattingButtons = value;
				LayoutChanged();
			}
		}
		public int LCID { get { return Thread.CurrentThread.CurrentCulture.LCID; } }
		public string GetUploadImageFolder() {
			string folder = SettingsDialogs.InsertImageDialog.SettingsImageUpload.UploadFolder;
			if (folder == "~/")
				folder = "/PublishingImages/";
			return folder;
		}
		protected override void CreateControlHierarchy() {
			CreateToolbars();
			base.CreateControlHierarchy();
		}
		protected override HtmlEditorSpellChecker CreateSpellCheckerInstance() {
			return new SPHtmlEditorSpellChecker(this);
		}
		protected virtual void FillUnallowableToolbarCommandNames(List<string> commandNames) {
			if (DisableBasicFormattingButtons)
				commandNames.AddRange(DisableBasicFormattingCommands);
		}
		protected virtual void FillAllowableToolbarCommandNames(List<string> commandNames) {
		}
		protected virtual void FillCssFilePaths(List<string> cssFilePaths) {
		}
		protected void CreateToolbars() {
			List<string> cssFilePaths = new List<string>();
			FillCssFilePaths(cssFilePaths);
			for (int i = 0; i < cssFilePaths.Count; i++)
				CssFiles.Add(new HtmlEditorCssFile(cssFilePaths[i]));
			List<string> unallowableCommandNames = new List<string>();
			FillUnallowableToolbarCommandNames(unallowableCommandNames);
			List<string> allowableCommandNames = new List<string>();
			FillAllowableToolbarCommandNames(allowableCommandNames);
			EnsureToolbars();
			for (int i = 0; i < Toolbars.Count; i++) {
				HtmlEditorToolbar toolBar = Toolbars[i];
				for (int j = toolBar.Items.Count - 1; j >= 0; j--) {
					HtmlEditorToolbarItem item = toolBar.Items[j];
					string commandName = item.CommandName.ToLower();
					if (unallowableCommandNames.Contains(commandName) ||
						(allowableCommandNames.Count > 0 && !allowableCommandNames.Contains(commandName)))
						toolBar.Items.RemoveAt(item.Index);
				}
			}
		}
		protected void EnsureToolbars() {
			if (Toolbars.IsEmpty) {
				Toolbars.CreateDefaultToolbars(IsRightToLeft());
				Toolbars[0].Items.Add(new ToolbarCheckSpellingButton(true));
			}
		}
		protected void InitializeSpellCheckerSettings() {
			SettingsSpellChecker.Culture = new CultureInfo("en-US");
			string basePath = "~/resources/DevExpress/Dictionaries/";
			ASPxSpellCheckerISpellDictionary dictionary = new ASPxSpellCheckerISpellDictionary();
			dictionary.DictionaryPath = basePath + "american.xlg";
			dictionary.AlphabetPath = basePath + "englishAlphabet.txt";
			dictionary.GrammarPath = basePath + "english.aff";
			dictionary.Encoding = Encoding.GetEncoding(1252);
			dictionary.Culture = new CultureInfo("en-US");
			dictionary.CacheKey = "ispellDic";
			SettingsSpellChecker.Dictionaries.Add(dictionary);
		}
	}
}
namespace DevExpress.SharePoint.Internal {
	public class SPHtmlEditorSpellChecker : HtmlEditorSpellChecker {
		public SPHtmlEditorSpellChecker(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
			PopulateSettingsForms();
		}
		protected void PopulateSettingsForms() {
			foreach(string formName in FormNames)
				SetFormPath(
					formName,
					CommonUtils.GetSPDefaultFormUrl(formName, typeof(DevExpress.Web.ASPxSpellChecker.ASPxSpellChecker))
				);
		}
	}
	public class SPxHtmlEditorToolPart : ToolPart {
		public SPxHtmlEditorToolPart() {
		}
		protected SPxHtmlEditorWebPart ParentWebPart {
			get { return (SPxHtmlEditorWebPart)base.ParentToolPane.SelectedWebPart; }
		}
		public override void ApplyChanges() {
			ParentWebPart.Html = ParentWebPart.ContentEditor.Html;
		}
		protected override void OnPreRender(EventArgs e) {
			ParentWebPart.InEditMode = true;
			ParentWebPart.LiteralContent.Visible = false;
			ParentWebPart.ContentEditor.Visible = true;
			base.OnPreRender(e);
		}
	}
}
