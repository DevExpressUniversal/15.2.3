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
using System.Web.UI;
namespace DevExpress.Web.ASPxSpreadsheet {
	public class SpreadsheetFormsSettings : ASPxSpreadsheetSettingsBase {
		public SpreadsheetFormsSettings(IPropertiesOwner owner)
			: base(owner) { }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsInsertPictureFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string InsertPictureFormPath {
			get { return GetStringProperty("InsertPictureFormPath", ""); }
			set { SetStringProperty("InsertPictureFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsInsertLinkFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string InsertLinkFormPath {
			get { return GetStringProperty("InsertLinkFormPath", ""); }
			set { SetStringProperty("InsertLinkFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsRenameSheetFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string RenameSheetFormPath {
			get { return GetStringProperty("RenameSheetFormPath", ""); }
			set { SetStringProperty("RenameSheetFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsOpenFileFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string OpenFileFormPath {
			get { return GetStringProperty("OpenFileFormPath", ""); }
			set { SetStringProperty("OpenFileFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsSaveFileFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string SaveFileFormPath {
			get { return GetStringProperty("SaveFileFormPath", ""); }
			set { SetStringProperty("SaveFileFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsRowHeightFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string RowHeightFormPath {
			get { return GetStringProperty("RowHeightFormPath", ""); }
			set { SetStringProperty("RowHeightFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsColumnWidthFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string ColumnWidthFormPath {
			get { return GetStringProperty("ColumnWidthFormPath", ""); }
			set { SetStringProperty("ColumnWidthFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsDefaultColumnWidthFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string DefaultColumnWidthFormPath {
			get { return GetStringProperty("DefaultColumnWidthFormPath", ""); }
			set { SetStringProperty("DefaultColumnWidthFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsUnhideSheetFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string UnhideSheetFormPath {
			get { return GetStringProperty("UnhideSheetFormPath", ""); }
			set { SetStringProperty("UnhideSheetFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsChangeChartTypeFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string ChangeChartTypeFormPath {
			get { return GetStringProperty("ChangeChartTypeFormPath", ""); }
			set { SetStringProperty("ChangeChartTypeFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsChartSelectDataFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string ChartSelectDataFormPath {
			get { return GetStringProperty("ChartSelectDataFormPath", ""); }
			set { SetStringProperty("ChartSelectDataFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsModifyChartLayoutFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string ModifyChartLayoutFormPath {
			get { return GetStringProperty("ModifyChartLayoutFormPath", ""); }
			set { SetStringProperty("ModifyChartLayoutFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsChartChangeTitleFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string ChartChangeTitleFormPath {
			get { return GetStringProperty("ChartChangeTitleFormPath", ""); }
			set { SetStringProperty("ChartChangeTitleFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsChartChangeHorizontalAxisTitleFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string ChartChangeHorizontalAxisTitleFormPath {
			get { return GetStringProperty("ChartChangeHorizontalAxisTitleFormPath", ""); }
			set { SetStringProperty("ChartChangeHorizontalAxisTitleFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsChartChangeVerticalAxisTitleFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string ChartChangeVerticalAxisTitleFormPath {
			get { return GetStringProperty("ChartChangeVerticalAxisTitleFormPath", ""); }
			set { SetStringProperty("ChartChangeVerticalAxisTitleFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsModifyChartStyleFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string ModifyChartStyleFormPath {
			get { return GetStringProperty("ModifyChartStyleFormPath", ""); }
			set { SetStringProperty("ModifyChartStyleFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFormsSettingsFindAndReplaceFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string FindAndReplaceFormPath {
			get { return GetStringProperty("FindAndReplaceFormPath", ""); }
			set { SetStringProperty("FindAndReplaceFormPath", "", value); }
		}
		protected internal string GetFormPath(string formName) {
			return GetStringProperty(string.Format("{0}Path", formName), "");
		}
		protected internal void SetFormPath(string formName, string value) {
			SetStringProperty(string.Format("{0}Path", formName), "", value);
		}
	}
	public class SpreadsheetDialogFormSettings : ASPxSpreadsheetSettingsBase {
		public SpreadsheetDialogFormSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertPictureDialog property instead.")]
		public SpreadsheetInsertPictureDialogSettings InsertPictureDialog {
			get {
				return GetDefaultDialogsSettings().InsertPictureDialog;
			}
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertLinkDialog property instead.")]
		public SpreadsheetInsertLinkDialogSettings InsertLinkDialog {
			get { return GetDefaultDialogsSettings().InsertLinkDialog; }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsDialogs.SaveFileDialog property instead.")]
		public SpreadsheetSaveFileDialogSettings SaveFileDialog {
			get {
				return GetDefaultDialogsSettings().SaveFileDialog;
			}
		}
		protected virtual SpreadsheetInsertPictureDialogSettings CreateInsertPictureDialogSettings(IPropertiesOwner owner) {
			return new SpreadsheetInsertPictureDialogSettings(owner);
		}
		protected virtual SpreadsheetInsertLinkDialogSettings CreateInsertLinkDialogSettings(IPropertiesOwner owner) {
			return new SpreadsheetInsertLinkDialogSettings(owner);
		}
		protected virtual SpreadsheetSaveFileDialogSettings CreateSaveFileDialogSettings(IPropertiesOwner owner) {
			return new SpreadsheetSaveFileDialogSettings(owner);
		}
		protected virtual SpreadsheetDialogSettings GetDefaultDialogsSettings() {
			return Spreadsheet.SettingsDialogs;
		}
	}
}
