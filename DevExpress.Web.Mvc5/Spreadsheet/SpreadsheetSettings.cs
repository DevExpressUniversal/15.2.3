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
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.ASPxSpreadsheet;
	public class SpreadsheetSettings : SettingsBase {
		ASPxSpreadsheetLoadingPanelSettings settingsLoadingPanel;
		MVCxSpreadsheetFormsSettings settingsForms;
		SpreadsheetDialogFormSettings settingsDialogForm;
		SpreadsheetDialogSettings settingsDialogs;
		SpreadsheetDocumentSelectorSettings settingsDocumentSelector;
		SpreadsheetTabControlStyles stylesTabControl;
		SpreadsheetButtonStyles stylesButton;
		SpreadsheetEditorsStyles stylesEditors;
		SpreadsheetMenuStyles stylesPopupMenu;
		SpreadsheetRibbonStyles stylesRibbon;
		SpreadsheetFileManagerStyles stylesFileManager;
		SpreadsheetFileManagerImages imagesFileManager;
		SpreadsheetRibbonTabCollection ribbonTabs;
		SpreadsheetFormulaBarStyles stylesFormulaBar;
		SpreadsheetFormulaAutoCompeteStyles stylesFormulaAutoCompete;
		public SpreadsheetSettings()
			: base() {
			this.settingsForms = new MVCxSpreadsheetFormsSettings();
			this.settingsDialogForm = new SpreadsheetDialogFormSettings(null);
			this.settingsDialogs = new SpreadsheetDialogSettings(null);
			this.settingsDocumentSelector = new SpreadsheetDocumentSelectorSettings(null);
			this.settingsLoadingPanel = new ASPxSpreadsheetLoadingPanelSettings(null);
			this.stylesButton = new SpreadsheetButtonStyles(null);
			this.stylesEditors = new SpreadsheetEditorsStyles(null);
			this.imagesFileManager = new SpreadsheetFileManagerImages(null);
			this.stylesFileManager = new SpreadsheetFileManagerStyles(null);
			this.stylesPopupMenu = new SpreadsheetMenuStyles(null);
			this.stylesRibbon = new SpreadsheetRibbonStyles(null);
			this.stylesTabControl = new SpreadsheetTabControlStyles(null);
			this.ribbonTabs = new SpreadsheetRibbonTabCollection(null);
			this.stylesFormulaBar = new SpreadsheetFormulaBarStyles(null);
			this.stylesFormulaAutoCompete = new SpreadsheetFormulaAutoCompeteStyles(null);
			ActiveTabIndex = 1;
			WorkDirectory = "~/App_Data/WorkDirectory";
			RibbonMode = SpreadsheetRibbonMode.Ribbon;
			ShowConfirmOnLosingChanges = true;
			ShowFormulaBar = true;
		}
		public int ActiveTabIndex { get; set; }
		public string AssociatedRibbonName { get; set; }
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DownloadRouteValues { get; set; }
		public SpreadsheetClientSideEvents ClientSideEvents { get { return (SpreadsheetClientSideEvents)ClientSideEventsInternal; } }
		public bool EnableClientSideAPI { get { return EnableClientSideAPIInternal; } set { EnableClientSideAPIInternal = value; } }
		public SpreadsheetImages Images { get { return (SpreadsheetImages)ImagesInternal; } }
		public SpreadsheetFileManagerImages ImagesFileManager { get { return imagesFileManager; } }
		public bool ReadOnly { get; set; }
		public bool ShowFormulaBar { get; set; }
		public SpreadsheetRibbonMode RibbonMode { get; set; }
		public SpreadsheetRibbonTabCollection RibbonTabs { get { return ribbonTabs; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SpreadsheetDialogFormSettings SettingsDialogForm { get { return settingsDialogForm; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SpreadsheetDialogSettings SettingsDialogs {get {return settingsDialogs; } }
		public SpreadsheetDocumentSelectorSettings SettingsDocumentSelector { get { return settingsDocumentSelector; } }
		public MVCxSpreadsheetFormsSettings SettingsForms { get { return settingsForms; } }
		public ASPxSpreadsheetLoadingPanelSettings SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public bool ShowConfirmOnLosingChanges { get; set; }
		public SpreadsheetStyles Styles { get { return (SpreadsheetStyles)StylesInternal; } }
		public SpreadsheetButtonStyles StylesButton { get { return stylesButton; } }
		public SpreadsheetFileManagerStyles StylesFileManager { get { return stylesFileManager; } }
		public SpreadsheetEditorsStyles StylesEditors { get { return stylesEditors; } }
		public SpreadsheetMenuStyles StylesPopupMenu { get { return stylesPopupMenu; } }
		public SpreadsheetRibbonStyles StylesRibbon { get { return stylesRibbon; } }
		public SpreadsheetTabControlStyles StylesTabControl { get { return stylesTabControl; } }
		public SpreadsheetFormulaBarStyles StylesFormulaBar { get { return stylesFormulaBar; } }
		public SpreadsheetFormulaAutoCompeteStyles StylesFormulaAutoCompete { get { return stylesFormulaAutoCompete; } }
		[Obsolete("This property is now obsolete. Use the SettingsDocumentSelector.EditingSettings.TemporaryFolder property instead."), 
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string TemporaryFolder { get; set; }
		public string WorkDirectory { get; set; }
		public FileManagerFolderCreateEventHandler DocumentSelectorFolderCreating { get; set; }
		public FileManagerItemRenameEventHandler DocumentSelectorItemRenaming { get; set; }
		public FileManagerItemDeleteEventHandler DocumentSelectorItemDeleting { get; set; }
		public FileManagerItemMoveEventHandler DocumentSelectorItemMoving { get; set; }
		public FileManagerFileUploadEventHandler DocumentSelectorFileUploading { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new SpreadsheetClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new SpreadsheetImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new SpreadsheetStyles(null);
		}
	}
	public class MVCxSpreadsheetFormsSettings : SpreadsheetFormsSettings {
		public MVCxSpreadsheetFormsSettings() : this(null) { }
		public MVCxSpreadsheetFormsSettings(IPropertiesOwner owner) : base(owner) { }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string ChangeChartTypeFormPath { get { return base.ChangeChartTypeFormPath; } set { base.ChangeChartTypeFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string ChartChangeHorizontalAxisTitleFormPath { get { return base.ChartChangeHorizontalAxisTitleFormPath; } set { base.ChartChangeHorizontalAxisTitleFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string ChartChangeTitleFormPath { get { return base.ChartChangeTitleFormPath; } set { base.ChartChangeTitleFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string ChartChangeVerticalAxisTitleFormPath { get { return base.ChartChangeVerticalAxisTitleFormPath; } set { base.ChartChangeVerticalAxisTitleFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string ChartSelectDataFormPath { get { return base.ChartSelectDataFormPath; } set { base.ChartSelectDataFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string ColumnWidthFormPath { get { return base.ColumnWidthFormPath; } set { base.ColumnWidthFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string DefaultColumnWidthFormPath { get { return base.DefaultColumnWidthFormPath; } set { base.DefaultColumnWidthFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string FindAndReplaceFormPath { get { return base.FindAndReplaceFormPath; } set { base.FindAndReplaceFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string InsertLinkFormPath { get { return base.InsertLinkFormPath; } set { base.InsertLinkFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string InsertPictureFormPath { get { return base.InsertPictureFormPath; } set { base.InsertPictureFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string ModifyChartLayoutFormPath { get { return base.ModifyChartLayoutFormPath; } set { base.ModifyChartLayoutFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string ModifyChartStyleFormPath { get { return base.ModifyChartStyleFormPath; } set { base.ModifyChartStyleFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string OpenFileFormPath { get { return base.OpenFileFormPath; } set { base.OpenFileFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string RenameSheetFormPath { get { return base.RenameSheetFormPath; } set { base.RenameSheetFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string RowHeightFormPath { get { return base.RowHeightFormPath; } set { base.RowHeightFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string SaveFileFormPath { get { return base.SaveFileFormPath; } set { base.SaveFileFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string UnhideSheetFormPath { get { return base.UnhideSheetFormPath; } set { base.UnhideSheetFormPath = value; } }		
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string ChangeChartTypeFormAction { get { return base.ChangeChartTypeFormPath; } set { base.ChangeChartTypeFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string ChartChangeHorizontalAxisTitleFormAction { get { return base.ChartChangeHorizontalAxisTitleFormPath; } set { base.ChartChangeHorizontalAxisTitleFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string ChartChangeTitleFormAction { get { return base.ChartChangeTitleFormPath; } set { base.ChartChangeTitleFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string ChartChangeVerticalAxisTitleFormAction { get { return base.ChartChangeVerticalAxisTitleFormPath; } set { base.ChartChangeVerticalAxisTitleFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string ChartSelectDataFormAction { get { return base.ChartSelectDataFormPath; } set { base.ChartSelectDataFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string ColumnWidthFormAction { get { return base.ColumnWidthFormPath; } set { base.ColumnWidthFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string DefaultColumnWidthFormAction { get { return base.DefaultColumnWidthFormPath; } set { base.DefaultColumnWidthFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string FindAndReplaceFormAction { get { return base.FindAndReplaceFormPath; } set { base.FindAndReplaceFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string InsertLinkFormAction { get { return base.InsertLinkFormPath; } set { base.InsertLinkFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string InsertPictureFormAction { get { return base.InsertPictureFormPath; } set { base.InsertPictureFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string ModifyChartLayoutFormAction { get { return base.ModifyChartLayoutFormPath; } set { base.ModifyChartLayoutFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string ModifyChartStyleFormAction { get { return base.ModifyChartStyleFormPath; } set { base.ModifyChartStyleFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string OpenFileFormAction { get { return base.OpenFileFormPath; } set { base.OpenFileFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string RenameSheetFormAction { get { return base.RenameSheetFormPath; } set { base.RenameSheetFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string RowHeightFormAction { get { return base.RowHeightFormPath; } set { base.RowHeightFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string SaveFileFormAction { get { return base.SaveFileFormPath; } set { base.SaveFileFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string UnhideSheetFormAction { get { return base.UnhideSheetFormPath; } set { base.UnhideSheetFormPath = value; } }
		protected internal string GetFormAction(string formName) {
			return GetFormPath(formName);
		}
	}
}
