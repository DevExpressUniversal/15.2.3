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
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
using DevExpress.Web.Mvc.UI;
using DevExpress.Web.Office;
using DevExpress.XtraRichEdit;
using DevExpress.Web.ASPxRichEdit.Internal;
namespace DevExpress.Web.Mvc {
	public class RichEditExtension : ExtensionBase {
		public RichEditExtension(RichEditSettings settings)
			: base(settings) {
		}
		public RichEditExtension(RichEditSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public static RibbonTab[] DefaultRibbonTabs {
			get { return new RichEditDefaultRibbon(null).DefaultRibbonTabs; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.ActiveTabIndex = Settings.ActiveTabIndex;
			Control.AssociatedRibbonID = Settings.AssociatedRibbonName;
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.CustomActionRouteValues = Settings.CustomActionRouteValues;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ConfirmOnLosingChanges = Settings.ConfirmOnLosingChanges;
			Control.EnableClientSideAPI = Settings.EnableClientSideAPI;
			Control.ReadOnly = Settings.ReadOnly;
			Control.Images.CopyFrom(Settings.Images);
			Control.ImagesRuler.CopyFrom(Settings.ImagesRuler);
			Control.RibbonMode = Settings.RibbonMode;
			if(!Settings.RibbonTabs.IsEmpty)
				Control.RibbonTabs.Assign(Settings.RibbonTabs);
			Control.Settings.Assign(Settings.Settings);
			Control.SettingsDocumentSelector.Assign(Settings.SettingsDocumentSelector);
			Control.ShowConfirmOnLosingChanges = Settings.ShowConfirmOnLosingChanges;
			Control.ShowStatusBar = Settings.ShowStatusBar;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.StylesButton.CopyFrom(Settings.StylesButton);
			Control.StylesEditors.CopyFrom(Settings.StylesEditors);
			Control.StylesFileManager.CopyFrom(Settings.StylesFileManager);
			Control.StylesPopupMenu.CopyFrom(Settings.StylesPopupMenu);
			Control.StylesRibbon.CopyFrom(Settings.StylesRibbon);
			Control.StylesRuler.CopyFrom(Settings.StylesRuler);
			Control.StylesStatusBar.CopyFrom(Settings.StylesStatusBar);
			Control.ViewMergedData = Settings.ViewMergedData;
			Control.WorkDirectory = Settings.WorkDirectory;
			Control.Saving += Settings.Saving;
			Control.CalculateDocumentVariable += Settings.CalculateDocumentVariable;
		}
		public RichEditExtension Bind(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public RichEditExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty, string.Empty);
		}
		public RichEditExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public RichEditExtension BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
		public RichEditExtension Open(string filePath) {
			Control.Open(filePath);
			return this;
		}
		public RichEditExtension Open(string filePath, DocumentFormat format) {
			Control.Open(filePath, format);
			return this;
		}
		public RichEditExtension Open(string uniqueDocumentID, DocumentFormat format, Func<Byte[]> contentAccessor) {
			Control.Open(uniqueDocumentID, format, contentAccessor);
			return this;
		}
		public RichEditExtension Open(string uniqueDocumentID, DocumentFormat format, Func<Stream> contentAccessor) {
			Control.Open(uniqueDocumentID, format, contentAccessor);
			return this;
		}
		public RichEditExtension Open(RichEditDocumentInfo documentInfo) {
			Control.Open(documentInfo);
			return this;
		}
		[Obsolete("This method is now obsolete. Use the Open method instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ActionResult LoadDocument(string extensionName, string filePath) {
			RichEditExtension extension = CreateExtension(extensionName);
			extension.Control.Open(filePath);
			return GetCustomCallbackResult(extension.Control);
		}
		public static ActionResult Open(string extensionName, string filePath) {
			RichEditExtension extension = CreateExtension(extensionName);
			extension.Control.Open(filePath);
			return GetCustomCallbackResult(extension.Control);
		}
		public static ActionResult Open(string extensionName, string filePath, DocumentFormat format) {
			RichEditExtension extension = CreateExtension(extensionName);
			extension.Control.Open(filePath, format);
			return GetCustomCallbackResult(extension.Control);
		}
		public static ActionResult Open(string extensionName, string uniqueDocumentID, DocumentFormat format, Func<Byte[]> contentAccessor) {
			RichEditExtension extension = CreateExtension(extensionName);
			extension.Control.Open(uniqueDocumentID, format, contentAccessor);
			return GetCustomCallbackResult(extension.Control);
		}
		public static ActionResult Open(string extensionName, string uniqueDocumentID, DocumentFormat format, Func<Stream> contentAccessor) {
			RichEditExtension extension = CreateExtension(extensionName);
			extension.Control.Open(uniqueDocumentID, format, contentAccessor);
			return GetCustomCallbackResult(extension.Control);
		}
		public static ActionResult Open(string extensionName, RichEditDocumentInfo documentInfo) {
			RichEditExtension extension = CreateExtension(extensionName);
			extension.Control.Open(documentInfo);
			return GetCustomCallbackResult(extension.Control);
		}
		public static void SaveCopy(string extensionName, string documentPath) {
			RichEditExtension extension = CreateExtension(extensionName);
			extension.Control.SaveCopy(documentPath);
		}
		public static void SaveCopy(string extensionName, Stream outputStream, DocumentFormat format) {
			RichEditExtension extension = CreateExtension(extensionName);
			extension.Control.SaveCopy(outputStream, format);
		}
		public static byte[] SaveCopy(string extensionName, DocumentFormat format) {
			RichEditExtension extension = CreateExtension(extensionName);
			return extension.Control.SaveCopy(format);
		}
		public static ActionResult ExportToPDF(string extensionName, string fileName) {
			RichEditExtension extension = CreateExtension(extensionName);
			return ExportUtils.Export(extension, s => extension.Control.ExportToPdf(s), fileName, true, "pdf");
		}
		public static string GetDocumentId(string extensionName) {
			RichEditExtension extension = CreateExtension(extensionName);
			return extension.Control.DocumentId;
		}
		static RichEditExtension CreateExtension(string name) {
			RichEditExtension extension = (RichEditExtension)ExtensionManager.GetExtension(ExtensionType.RichEdit, name, ExtensionCacheMode.Request);
			extension.PrepareWorkSession();
			extension.Control.AssignWorkDirectoryFromWorkSession();
			extension.Control.ApplyRequestCommands();
			return extension;
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareWorkSession();
			string uploadControlID = PostDataCollection[RenderUtils.UploadingCallbackQueryParamName] ??
				 PostDataCollection[RenderUtils.HelperUploadingCallbackQueryParamName];
			if(!string.IsNullOrEmpty(uploadControlID)) {
				MVCxUploadControl uploadControl = Control.GetCurrentDialogUploadControl();
				if(uploadControl != null)
					uploadControl.EnsureUploaded();
			}
		}
		protected internal void PrepareWorkSession() {
			Control.LoadWorkSessionIdFromRequest();
			Control.CheckWorkDirectoryAccess();
		}
		protected override void LoadPostDataInternal() {
			base.LoadPostDataInternal();
			PrepareWorkSession();
		}
		static ContentResult GetCustomCallbackResult(MVCxRichEdit richEdit) {
			return ExtensionBase.GetCustomDataCallbackResult(richEdit.GetCustomCallbackResult());
		}
		protected internal new MVCxRichEdit Control {
			get { return (MVCxRichEdit)base.Control; }
		}
		protected internal new RichEditSettings Settings {
			get { return (RichEditSettings)base.Settings; }
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxRichEdit();
		}
	}
}
