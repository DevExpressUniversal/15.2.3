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
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Spreadsheet;
using DevExpress.Web.ASPxSpreadsheet.Internal;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
using DevExpress.Web.Mvc.UI;
using DevExpress.Web.Office;
namespace DevExpress.Web.Mvc {
	public class SpreadsheetExtension : ExtensionBase {
		static Control callbackResultDummyControl = new Control();
		public SpreadsheetExtension(SpreadsheetSettings settings)
			: base(settings) {
		}
		public SpreadsheetExtension(SpreadsheetSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.ActiveTabIndex = Settings.ActiveTabIndex;
			Control.AssociatedRibbonID = Settings.AssociatedRibbonName;
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.CustomActionRouteValues = Settings.CustomActionRouteValues;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.EnableClientSideAPI = Settings.EnableClientSideAPI;
			Control.Images.CopyFrom(Settings.Images);
			Control.ImagesFileManager.CopyFrom(Settings.ImagesFileManager);
			Control.ReadOnly = Settings.ReadOnly;
			Control.RibbonMode = Settings.RibbonMode;
			Control.ShowFormulaBar = Settings.ShowFormulaBar;
			if(!Settings.RibbonTabs.IsEmpty) 
				Control.RibbonTabs.Assign(Settings.RibbonTabs);			
			Control.WorkDirectory = Settings.WorkDirectory;
#pragma warning disable 618
			Control.SettingsDialogForm.Assign(Settings.SettingsDialogForm);
#pragma warning restore 618
			Control.SettingsDialogs.Assign(Settings.SettingsDialogs);
			Control.SettingsDocumentSelector.Assign(Settings.SettingsDocumentSelector);
			Control.SettingsForms.Assign(Settings.SettingsForms);
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.ShowConfirmOnLosingChanges = Settings.ShowConfirmOnLosingChanges;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.StylesButton.CopyFrom(Settings.StylesButton);
			Control.StylesEditors.CopyFrom(Settings.StylesEditors);
			Control.StylesFileManager.CopyFrom(Settings.StylesFileManager);
			Control.StylesPopupMenu.CopyFrom(Settings.StylesPopupMenu);
			Control.StylesRibbon.CopyFrom(Settings.StylesRibbon);
			Control.StylesTabControl.CopyFrom(Settings.StylesTabControl);
			Control.StylesFormulaBar.CopyFrom(Settings.StylesFormulaBar);
			Control.StylesFormulaAutoCompete.CopyFrom(Settings.StylesFormulaAutoCompete);
			Control.DocumentSelectorFileUploading += Settings.DocumentSelectorFileUploading;
			Control.DocumentSelectorFolderCreating += Settings.DocumentSelectorFolderCreating;
			Control.DocumentSelectorItemDeleting += Settings.DocumentSelectorItemDeleting;
			Control.DocumentSelectorItemMoving += Settings.DocumentSelectorItemMoving;
			Control.DocumentSelectorItemRenaming += Settings.DocumentSelectorItemRenaming;
		}
		public SpreadsheetExtension PerformMailMerge(string pathToTemplateXlsx, object mailMergeDataSource) {
			return PerformMailMerge(pathToTemplateXlsx, mailMergeDataSource, "\"OneWorksheet\"");
		}
		public SpreadsheetExtension PerformMailMerge(string pathToTemplateXlsx, object mailMergeDataSource, string mailMergeMode) {
			return PerformMailMerge(pathToTemplateXlsx, mailMergeDataSource, mailMergeMode, 0, DocumentFormat.OpenXml);
		}
		public SpreadsheetExtension PerformMailMerge(string pathToTemplateXlsx, object mailMergeDataSource, string mailMergeMode,
			int documentIndex, DocumentFormat documentFormat) {
			Control.LoadWorkSessionFromRequest();
			IWorkbook workbook = Control.Document;
			workbook.LoadDocument(pathToTemplateXlsx);
			workbook.DefinedNames.GetDefinedName("MAILMERGEMODE").RefersTo = mailMergeMode;
			workbook.MailMergeDataSource = mailMergeDataSource;
			IWorkbook resultBook = workbook.GenerateMailMergeDocuments()[documentIndex];
			if(resultBook != null) {
				using(MemoryStream result = new MemoryStream()) {
					resultBook.SaveDocument(result, documentFormat);
					result.Seek(0, SeekOrigin.Begin);
					Control.Document.LoadDocument(result, documentFormat);
				}
			}
			return this;
		}
		public SpreadsheetExtension Open(string filePath) {
			Control.Open(filePath);
			return this;
		}
		public SpreadsheetExtension Open(string filePath, DocumentFormat format) {
			Control.Open(filePath, format);
			return this;
		}
		public SpreadsheetExtension Open(string uniqueDocumentID, DocumentFormat format, Func<Byte[]> contentAccessor) {
			Control.Open(uniqueDocumentID, format, contentAccessor);
			return this;
		}
		public SpreadsheetExtension Open(string uniqueDocumentID, DocumentFormat format, Func<Stream> contentAccessor) {
			Control.Open(uniqueDocumentID, format, contentAccessor);
			return this;
		}
		public SpreadsheetExtension Open(SpreadsheetDocumentInfo documentInfo) {
			Control.Open(documentInfo);
			return this;
		}
		public static void SaveCopy(string extensionName, string documentPath) {
			SpreadsheetExtension extension = CreateExtension(extensionName);
			extension.Control.SaveCopy(documentPath);
		}
		public static void SaveCopy(string extensionName, Stream outputStream, DocumentFormat format) {
			SpreadsheetExtension extension = CreateExtension(extensionName);
			extension.Control.SaveCopy(outputStream, format);
		}
		public static byte[] SaveCopy(string extensionName, DocumentFormat format) {
			SpreadsheetExtension extension = CreateExtension(extensionName);
			return extension.Control.SaveCopy(format);
		}
		public static IWorkbook GetCurrentDocument(string extensionName) {
			SpreadsheetExtension extension = CreateExtension(extensionName);
			return extension.Control.Document;
		}
		public static string GetDocumentId(string extensionName) {
			SpreadsheetExtension extension = CreateExtension(extensionName);
			return extension.Control.DocumentId;
		}
		[Obsolete("This method is now obsolete. Use the Open property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ActionResult LoadDocument(string extensionName, string filePath) {
			SpreadsheetExtension extension = CreateExtension(extensionName);
			extension.Control.Open(filePath);
			return GetCustomCallbackResult(extension.Control);
		}
		public static ActionResult Open(string extensionName, string filePath) {
			SpreadsheetExtension extension = CreateExtension(extensionName);
			extension.Control.Open(filePath);
			return GetCustomCallbackResult(extension.Control);
		}
		public static ActionResult Open(string extensionName, string filePath, DocumentFormat format) {
			SpreadsheetExtension extension = CreateExtension(extensionName);
			extension.Control.Open(filePath, format);
			return GetCustomCallbackResult(extension.Control);
		}
		public static ActionResult Open(string extensionName, string uniqueDocumentID, DocumentFormat format, Func<Byte[]> contentAccessor) {
			SpreadsheetExtension extension = CreateExtension(extensionName);
			extension.Control.Open(uniqueDocumentID, format, contentAccessor);
			return GetCustomCallbackResult(extension.Control);
		}
		public static ActionResult Open(string extensionName, string uniqueDocumentID, DocumentFormat format, Func<Stream> contentAccessor) {
			SpreadsheetExtension extension = CreateExtension(extensionName);
			extension.Control.Open(uniqueDocumentID, format, contentAccessor);
			return GetCustomCallbackResult(extension.Control);
		}
		public static ActionResult Open(string extensionName, SpreadsheetDocumentInfo documentInfo) {
			SpreadsheetExtension extension = CreateExtension(extensionName);
			extension.Control.Open(documentInfo);
			return GetCustomCallbackResult(extension.Control);
		}
		public static ActionResult CloseDocument(string extensionName) {
			SpreadsheetExtension extension = CreateExtension(extensionName);
#pragma warning disable 618
			extension.Control.Close();
#pragma warning restore 618
			return GetCustomCallbackResult(extension.Control);
		}
		[Obsolete("This method is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static FileStreamResult DownloadFile(string extensionName) {
			return null;
		}
		public static ActionResult GetCustomActionResult(string extensionName) {
			SpreadsheetExtension extension = CreateExtension(extensionName);
			return GetCustomCallbackResult(extension.Control);
		}
		static SpreadsheetExtension CreateExtension(string name) {
			SpreadsheetExtension extension = (SpreadsheetExtension)ExtensionManager.GetExtension(ExtensionType.Spreadsheet, name, ExtensionCacheMode.Request);
			extension.PrepareControl();
			extension.LoadPostData();
			return extension;
		}
		static ContentResult GetCustomCallbackResult(MVCxSpreadsheet spreadsheet) {
			return ExtensionBase.GetCustomDataCallbackResult(spreadsheet.GetCustomeCallbackResult(MvcUtils.CallbackArgument));
		}
		protected override Control GetCallbackResultControl() {
			return callbackResultDummyControl;
		}
		protected override void RenderCallbackResultControl() {
			Control.RenderCallbackResultControl();
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.PerformOnLoad();
			string uploadControlID = PostDataCollection[RenderUtils.UploadingCallbackQueryParamName] ??
				PostDataCollection[RenderUtils.HelperUploadingCallbackQueryParamName];
			if(!string.IsNullOrEmpty(uploadControlID)) {
				MVCxUploadControl uploadControl = Control.GetCurrentDialogUploadControl();
				if(uploadControl != null)
					uploadControl.EnsureUploaded();
			}
		}
		public static RibbonTab[] DefaultRibbonTabs {
			get { return new SpreadsheetDefaultRibbon(null).DefaultRibbonTabs; }
		}
		protected internal new MVCxSpreadsheet Control {
			get { return (MVCxSpreadsheet)base.Control; }
		}
		protected internal new SpreadsheetSettings Settings {
			get { return (SpreadsheetSettings)base.Settings; }
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxSpreadsheet();
		}
	}
}
