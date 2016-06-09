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
using System.Web;
using System.Web.UI;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.ASPxScheduler.Controls;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxTreeList;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.UI;
using DevExpress.XtraReports.Web;
namespace DevExpress.Web.Mvc.Internal {
	public class ExtensionRegistrationItem {
		ExtensionRegistrator registrator;
		ExtensionSuite suite;
		Type registratorClass;
		public ExtensionRegistrationItem(ExtensionSuite suite, Type registratorClass) {
			this.suite = suite;
			this.registratorClass = registratorClass;
		}
		public ExtensionSuite Suite { get { return suite; } }
		public Type RegistratorClass { get { return registratorClass; } }
		public Type ExtensionClass { get { return Registrator.ExtensionClass; } }
		public Type SettingsClass { get { return Registrator.SettingsClass; } }
		protected ExtensionRegistrator Registrator {
			get {
				if(registrator == null)
					registrator = (ExtensionRegistrator)Activator.CreateInstance(RegistratorClass);
				return registrator;
			}
		}
		public void RegisterScripts() {
			Registrator.RegisterScripts();
		}
		public void RegisterStyleSheets(string theme, string skinID) {
			Registrator.RegisterStyleSheets(theme, skinID);
		}
	}
	public abstract class ExtensionRegistrator {
		const string ScriptControlsSetKey = "Script";
		Dictionary<string, ASPxWebControl[]> controlsCache = new Dictionary<string,ASPxWebControl[]>();
		public abstract Type ExtensionClass { get; }
		public abstract Type SettingsClass { get; }
		protected abstract ASPxWebControl[] GetControls();
		ASPxWebControl[] GetControlsSet() {
			return GetControlsSet(null, null);
		}
		protected virtual string GetControlsSetKey(string theme, string skinID) {
			if(theme != null && skinID != null) {
				string finalMsTheme = Utils.GetFinalMsTheme(theme);
				string finalDxTheme = Utils.GetFinalDxTheme(theme);
				return string.Format("{0}_{1}_{2}",
					!string.IsNullOrEmpty(finalMsTheme) ? finalMsTheme : Utils.GetFinalMsStyleSheetTheme(),
					!string.IsNullOrEmpty(finalDxTheme) ? finalDxTheme : Utils.GetFinalDxStyleSheetTheme(),
					skinID
				);
			}
			return ScriptControlsSetKey;
		}
		static object controlsCacheLock = new Object();
		ASPxWebControl[] GetControlsSet(string theme, string skinID) {
			lock(controlsCacheLock) {
				string key = GetControlsSetKey(theme, skinID);
				if(!controlsCache.ContainsKey(key)) {
					ASPxWebControl[] controls = GetControls();
					foreach(ASPxWebControl control in controls) {
						RenderUtils.EnsureChildControlsRecursive(control, true);
						if(theme != null && skinID != null)
							ApplyThemeRecurcive(control, theme, skinID, Utils.IsThemeSpecified(theme));
					}
					controlsCache[key] = controls;
				}
				return controlsCache[key];
			}
		}
		protected virtual void ApplyThemeRecurcive(Control control, string theme, string skinID, bool isThemeSpecified) {
			ASPxWebControl aspxWebControl = control as ASPxWebControl;
			if(aspxWebControl != null) {
				aspxWebControl.SkinID = skinID;
				if(isThemeSpecified)
					Utils.ApplyTheme(aspxWebControl, theme);
				else
					Utils.ApplyStyleSheetTheme(aspxWebControl);
			}
			foreach(Control child in control.Controls)
				ApplyThemeRecurcive(child, theme, skinID, isThemeSpecified);
		}
		public void RegisterScripts() {
			ASPxWebControl[] controls = GetControlsSet();
			foreach(ASPxWebControl control in controls)
				RegisterScripts(control);
		}
		static object registerScriptsLockObj = new object();
		static void RegisterScripts(Control control) {
			lock(registerScriptsLockObj) { 
				ASPxWebControl aspxWebControl = control as ASPxWebControl;
				if(aspxWebControl != null) {
					aspxWebControl.RegisterClientIncludeScripts();
					aspxWebControl.RegisterClientScriptBlocks();
				}
				foreach(Control child in control.Controls)
					RegisterScripts(child);
			}
		}
		public void RegisterStyleSheets(string theme, string skinID) {
			ASPxWebControl[] controls = GetControlsSet(theme, skinID);
			foreach(ASPxWebControl control in controls) {
				RegisterStyleSheets(control, theme, skinID);
				ControlRibbonImages images = GetRibbonImages(control);
				if(images != null)
					RegisterRibbonIconsHelper.RegisterSpriteResourceByOwner(images);
			}
		}
		static object registerStyleSheetsLockObj = new object();
		static void RegisterStyleSheets(Control control, string theme, string skinID) {
			lock(registerStyleSheetsLockObj) { 
				ASPxWebControl aspxWebControl = control as ASPxWebControl;
				if(aspxWebControl != null)
					aspxWebControl.RegisterStyleSheets();
				foreach(Control child in control.Controls)
					RegisterStyleSheets(child, theme, string.Empty);
			}
		}
		protected virtual ControlRibbonImages GetRibbonImages(ASPxWebControl control) {
			return null;
		}
	}
	public enum ExtensionCacheMode { None, Static, Request }
	public class ExtensionManager {
		static Dictionary<ExtensionType, ExtensionRegistrationItem> registrationItems = new Dictionary<ExtensionType, ExtensionRegistrationItem>();
		static Dictionary<ExtensionSuite, List<ExtensionType>> extensionTypes = new Dictionary<ExtensionSuite, List<ExtensionType>>();
		static Dictionary<ExtensionType, Dictionary<string, ExtensionBase>> staticExtentions = new Dictionary<ExtensionType, Dictionary<string, ExtensionBase>>();
		static Dictionary<ExtensionType, Dictionary<string, ExtensionBase>> requestExtentions = new Dictionary<ExtensionType, Dictionary<string, ExtensionBase>>();
		static ExtensionManager() {
			CreateRegistrationItems();
		}
		public static Dictionary<ExtensionType, ExtensionRegistrationItem> RegistrationItems { get { return registrationItems; } }
		public static Dictionary<ExtensionType, Dictionary<string, ExtensionBase>> StaticExtensions { get { return staticExtentions; } }
		public static Dictionary<ExtensionType, Dictionary<string, ExtensionBase>> RequestExtensions {
			get {
				if(HttpContext.Current == null)
					return requestExtentions;
				return HttpUtils.GetContextObject<Dictionary<ExtensionType, Dictionary<string, ExtensionBase>>>("RequestExtensions");
			}
		}
		static object lockGetExtensionTypes = new Object();
		public static List<ExtensionType> GetExtensionTypes(ExtensionSuite extensionSuite) {
			lock(lockGetExtensionTypes) {
				if(!extensionTypes.ContainsKey(extensionSuite)) {
					List<ExtensionType> list = new List<ExtensionType>();
					foreach(ExtensionType extensionType in RegistrationItems.Keys) {
						if(RegistrationItems[extensionType].Suite == extensionSuite || extensionSuite == ExtensionSuite.All)
							list.Add(extensionType);
					}
					extensionTypes[extensionSuite] = list;
				}
				return extensionTypes[extensionSuite];
			}
		}
		public static ExtensionBase GetExtension(ExtensionType extensionType, string name, ExtensionCacheMode cacheMode) {
			return GetExtension(extensionType, name, cacheMode, string.Empty);
		}
		static object lockGetExtension = new Object();
		public static ExtensionBase GetExtension(ExtensionType extensionType, string name, ExtensionCacheMode cacheMode, string keySuffix) {
			string key = name + "_" + keySuffix;
			Dictionary<ExtensionType, Dictionary<string, ExtensionBase>> cache = null;
			if(cacheMode == ExtensionCacheMode.Static)
				cache = StaticExtensions;
			else if(cacheMode == ExtensionCacheMode.Request)
				cache = RequestExtensions;
			if(cache != null && cache.ContainsKey(extensionType) && cache[extensionType].ContainsKey(key))
				return cache[extensionType][key];
			lock(lockGetExtension) {
				if(cache != null && !cache.ContainsKey(extensionType))
					AddExtensionTypeToCache(cache, extensionType);
				SettingsBase settings = CreateSettings(extensionType, name);
				ExtensionBase extension = CreateExtension(extensionType, settings);
				if(cache != null && !cache[extensionType].ContainsKey(key))
					cache[extensionType].Add(key, extension);
				return extension;
			}
		}
		internal static void AddExtensionTypeToCache(Dictionary<ExtensionType, Dictionary<string, ExtensionBase>> cache, ExtensionType extensionType) {
			if (cache != null && !cache.ContainsKey(extensionType))
				cache.Add(extensionType, new Dictionary<string, ExtensionBase>());
		}
		internal static ExtensionBase CreateExtension(ExtensionType extensionType, SettingsBase settings) {
			ExtensionBase extension = ExtensionsFactory.CreateExtension(RegistrationItems[extensionType].ExtensionClass, settings, null);
			extension.PrepareControlProperties();
			return extension;
		}
		internal static SettingsBase CreateSettings(ExtensionType extensionType, string name) {
			return ExtensionsFactory.CreateSettings(RegistrationItems[extensionType].SettingsClass, name);
		}
		static void AddRegistrationItem(ExtensionType type, ExtensionSuite suite, Type registratorClassType) {
			RegistrationItems.Add(type, new ExtensionRegistrationItem(suite, registratorClassType));
		}
		static void AddRegistrationItem(ExtensionType type, ExtensionSuite suite, string registratorClassName) {
			Type registratorClassType = Type.GetType(registratorClassName, false, true);
			if(registratorClassType != null) {
				AddRegistrationItem(type, suite, registratorClassType);
			}
		}
		static void CreateRegistrationItems() {
			AddRegistrationItem(ExtensionType.PivotGrid, ExtensionSuite.PivotGrid, typeof(PivotGridExtensionRegistrator));
			AddRegistrationItem(ExtensionType.PivotCustomizationExtension, ExtensionSuite.PivotGrid, typeof(PivotCustomizationExtensionRegistrator));
			AddRegistrationItem(ExtensionType.GridView, ExtensionSuite.GridView, typeof(GridViewExtensionRegistrator));
			AddRegistrationItem(ExtensionType.GridLookup, ExtensionSuite.GridView, typeof(GridLookupExtensionRegistrator));
			AddRegistrationItem(ExtensionType.CardView, ExtensionSuite.CardView, typeof(CardViewExtensionRegistrator));
			AddRegistrationItem(ExtensionType.HtmlEditor, ExtensionSuite.HtmlEditor, typeof(HtmlEditorExtensionRegistrator));
			AddRegistrationItem(ExtensionType.SpellChecker, ExtensionSuite.SpellChecker, typeof(SpellCheckerRegistrator));
			AddRegistrationItem(ExtensionType.CallbackPanel, ExtensionSuite.NavigationAndLayout, typeof(CallbackPanelExtensionRegistrator));
			AddRegistrationItem(ExtensionType.Panel, ExtensionSuite.NavigationAndLayout, typeof(PanelExtensionRegistrator));
			AddRegistrationItem(ExtensionType.DataView, ExtensionSuite.NavigationAndLayout, typeof(DataViewExtensionRegistrator));
			AddRegistrationItem(ExtensionType.DockManager, ExtensionSuite.NavigationAndLayout, typeof(DockManagerExtensionRegistrator));
			AddRegistrationItem(ExtensionType.DockPanel, ExtensionSuite.NavigationAndLayout, typeof(DockPanelExtensionRegistrator));
			AddRegistrationItem(ExtensionType.DockZone, ExtensionSuite.NavigationAndLayout, typeof(DockZoneExtensionRegistrator));
			AddRegistrationItem(ExtensionType.FileManager, ExtensionSuite.NavigationAndLayout, typeof(FileManagerExtensionRegistrator));
			AddRegistrationItem(ExtensionType.ImageGallery, ExtensionSuite.NavigationAndLayout, typeof(ImageGalleryExtensionRegistrator));
			AddRegistrationItem(ExtensionType.ImageSlider, ExtensionSuite.NavigationAndLayout, typeof(ImageSliderExtensionRegistrator));
			AddRegistrationItem(ExtensionType.ImageZoom, ExtensionSuite.NavigationAndLayout, typeof(ImageZoomExtensionRegistrator));
			AddRegistrationItem(ExtensionType.ImageZoomNavigator, ExtensionSuite.NavigationAndLayout, typeof(ImageZoomNavigatorExtensionRegistrator));
			AddRegistrationItem(ExtensionType.FormLayout, ExtensionSuite.NavigationAndLayout, typeof(FormLayoutExtensionRegistrator));
			AddRegistrationItem(ExtensionType.LoadingPanel, ExtensionSuite.NavigationAndLayout, typeof(LoadingPanelExtensionRegistrator));
			AddRegistrationItem(ExtensionType.Menu, ExtensionSuite.NavigationAndLayout, typeof(MenuExtensionRegistrator));
			AddRegistrationItem(ExtensionType.PopupMenu, ExtensionSuite.NavigationAndLayout, typeof(PopupMenuExtensionRegistrator));
			AddRegistrationItem(ExtensionType.NavBar, ExtensionSuite.NavigationAndLayout, typeof(NavBarExtensionRegistrator));
			AddRegistrationItem(ExtensionType.Ribbon, ExtensionSuite.NavigationAndLayout, typeof(RibbonExtensionRegistrator));
			AddRegistrationItem(ExtensionType.PopupControl, ExtensionSuite.NavigationAndLayout, typeof(PopupControlExtensionRegistrator));
			AddRegistrationItem(ExtensionType.RatingControl, ExtensionSuite.NavigationAndLayout, typeof(RatingControlExtensionRegistrator));
			AddRegistrationItem(ExtensionType.RoundPanel, ExtensionSuite.NavigationAndLayout, typeof(RoundPanelExtensionRegistrator));
			AddRegistrationItem(ExtensionType.Splitter, ExtensionSuite.NavigationAndLayout, typeof(SplitterExtensionRegistrator));
			AddRegistrationItem(ExtensionType.PageControl, ExtensionSuite.NavigationAndLayout, typeof(PageControlExtensionRegistrator));
			AddRegistrationItem(ExtensionType.TabControl, ExtensionSuite.NavigationAndLayout, typeof(TabControlExtensionRegistrator));
			AddRegistrationItem(ExtensionType.TreeView, ExtensionSuite.NavigationAndLayout, typeof(TreeViewExtensionRegistrator));
			AddRegistrationItem(ExtensionType.UploadControl, ExtensionSuite.NavigationAndLayout, typeof(UploadControlExtensionRegistrator));
			AddRegistrationItem(ExtensionType.ButtonEdit, ExtensionSuite.Editors, typeof(ButtonEditExtensionRegistrator));
			AddRegistrationItem(ExtensionType.Button, ExtensionSuite.Editors, typeof(ButtonExtensionRegistrator));
			AddRegistrationItem(ExtensionType.BinaryImage, ExtensionSuite.Editors, typeof(BinaryImageExtensionRegistrator));
			AddRegistrationItem(ExtensionType.Calendar, ExtensionSuite.Editors, typeof(CalendarExtensionRegistrator));
			AddRegistrationItem(ExtensionType.Captcha, ExtensionSuite.Editors, typeof(CaptchaExtensionRegistrator));
			AddRegistrationItem(ExtensionType.CheckBox, ExtensionSuite.Editors, typeof(CheckBoxExtensionRegistrator));
			AddRegistrationItem(ExtensionType.CheckBoxList, ExtensionSuite.Editors, typeof(CheckBoxListExtensionRegistrator));
			AddRegistrationItem(ExtensionType.ColorEdit, ExtensionSuite.Editors, typeof(ColorEditExtensionRegistrator));
			AddRegistrationItem(ExtensionType.TrackBar, ExtensionSuite.Editors, typeof(TrackBarExtensionRegistrator));
			AddRegistrationItem(ExtensionType.ComboBox, ExtensionSuite.Editors, typeof(ComboBoxExtensionRegistrator));
			AddRegistrationItem(ExtensionType.DateEdit, ExtensionSuite.Editors, typeof(DateEditExtensionRegistrator));
			AddRegistrationItem(ExtensionType.DropDownEdit, ExtensionSuite.Editors, typeof(DropDownEditExtensionRegistrator));
			AddRegistrationItem(ExtensionType.HyperLink, ExtensionSuite.Editors, typeof(HyperLinkExtensionRegistrator));
			AddRegistrationItem(ExtensionType.Image, ExtensionSuite.Editors, typeof(ImageExtensionRegistrator));
			AddRegistrationItem(ExtensionType.Label, ExtensionSuite.Editors, typeof(LabelExtensionRegistrator));
			AddRegistrationItem(ExtensionType.ListBox, ExtensionSuite.Editors, typeof(ListBoxExtensionRegistrator));
			AddRegistrationItem(ExtensionType.Memo, ExtensionSuite.Editors, typeof(MemoExtensionRegistrator));
			AddRegistrationItem(ExtensionType.ProgressBar, ExtensionSuite.Editors, typeof(ProgressBarExtensionRegistrator));
			AddRegistrationItem(ExtensionType.RadioButton, ExtensionSuite.Editors, typeof(RadioButtonExtensionRegistrator));
			AddRegistrationItem(ExtensionType.RadioButtonList, ExtensionSuite.Editors, typeof(RadioButtonListExtensionRegistrator));
			AddRegistrationItem(ExtensionType.SpinEdit, ExtensionSuite.Editors, typeof(SpinEditExtensionRegistrator));
			AddRegistrationItem(ExtensionType.TextBox, ExtensionSuite.Editors, typeof(TextBoxExtensionRegistrator));
			AddRegistrationItem(ExtensionType.TimeEdit, ExtensionSuite.Editors, typeof(TimeEditExtensionRegistrator));
			AddRegistrationItem(ExtensionType.TokenBox, ExtensionSuite.Editors, typeof(TokenBoxExtensionRegistrator));
			AddRegistrationItem(ExtensionType.ValidationSummary, ExtensionSuite.Editors, typeof(ValidationSummaryRegistrator));
			AddRegistrationItem(ExtensionType.Chart, ExtensionSuite.Chart, typeof(ChartExtensionRegistrator));
			AddRegistrationItem(ExtensionType.ReportDesigner, ExtensionSuite.Report, typeof(ReportDesignerExtensionRegistrator));
			AddRegistrationItem(ExtensionType.WebDocumentViewer, ExtensionSuite.Report, typeof(WebDocumentViewerExtensionRegistrator));
			AddRegistrationItem(ExtensionType.DocumentViewer, ExtensionSuite.Report, typeof(DocumentViewerExtensionRegistrator));
			AddRegistrationItem(ExtensionType.ReportViewer, ExtensionSuite.Report,typeof(ReportViewerExtensionRegistrator));
			AddRegistrationItem(ExtensionType.ReportToolbar, ExtensionSuite.Report,typeof(ReportToolbarExtensionRegistrator));
			AddRegistrationItem(ExtensionType.Scheduler, ExtensionSuite.Scheduler, typeof(SchedulerExtensionRegistrator));
			AddRegistrationItem(ExtensionType.TimeZoneEdit, ExtensionSuite.Scheduler, typeof(TimeZoneEditExtensionRegistrator));
			AddRegistrationItem(ExtensionType.AppointmentRecurrenceForm, ExtensionSuite.Scheduler, typeof(AppointmentRecurrenceFormExtensionRegistrator));
			AddRegistrationItem(ExtensionType.SchedulerStatusInfo, ExtensionSuite.Scheduler, typeof(SchedulerStatusInfoExtensionRegistrator));
			AddRegistrationItem(ExtensionType.DateNavigator, ExtensionSuite.Scheduler, typeof(DateNavigatorExtensionRegistrator));
			AddRegistrationItem(ExtensionType.TreeList, ExtensionSuite.TreeList, typeof(TreeListExtensionRegistrator));
			AddRegistrationItem(ExtensionType.Icons16x16, ExtensionSuite.Icons, typeof(Icons16x16Registrator));
			AddRegistrationItem(ExtensionType.Icons16x16gray, ExtensionSuite.Icons, typeof(Icons16x16grayRegistrator));
			AddRegistrationItem(ExtensionType.Icons16x16office2013, ExtensionSuite.Icons, typeof(Icons16x16office2013Registrator));
			AddRegistrationItem(ExtensionType.Icons32x32, ExtensionSuite.Icons, typeof(Icons32x32Registrator));
			AddRegistrationItem(ExtensionType.Icons32x32gray, ExtensionSuite.Icons, typeof(Icons32x32grayRegistrator));
			AddRegistrationItem(ExtensionType.Icons32x32office2013, ExtensionSuite.Icons, typeof(Icons32x32office2013Registrator));
			AddRegistrationItem(ExtensionType.Icons16x16devav, ExtensionSuite.Icons, typeof(Icons16x16devavRegistrator));
			AddRegistrationItem(ExtensionType.Icons32x32devav, ExtensionSuite.Icons, typeof(Icons32x32devavRegistrator));
			string dashboardAssemblyName =
#if(MVC_FULL_TRUST)
			AssemblyInfo.SRAssemblyDashboardWebMVC5 + AssemblyInfo.FullAssemblyVersionExtension;
#else
			AssemblyInfo.SRAssemblyDashboardWebMVC + AssemblyInfo.FullAssemblyVersionExtension;
#endif
			AddRegistrationItem(ExtensionType.DashboardViewer, ExtensionSuite.DashboardViewer, "DevExpress.DashboardWeb.Mvc.Internal.DashboardViewerExtensionRegistrator, " + dashboardAssemblyName);
			AddRegistrationItem(ExtensionType.Spreadsheet, ExtensionSuite.Spreadsheet, typeof(SpreadsheetExtensionRegistrator));
			AddRegistrationItem(ExtensionType.RichEdit, ExtensionSuite.RichEdit, typeof(RichEditExtensionRegistrator));
		}
		internal static void RegisterScripts(ExtensionType extensionType) {
			RegistrationItems[extensionType].RegisterScripts();
		}
		internal static void RegisterStyleSheets(ExtensionType extensionType, string theme, string skinID) {
			RegistrationItems[extensionType].RegisterStyleSheets(theme, skinID);
		}
	}
	public class PivotGridExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(PivotGridExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(PivotGridSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxTextBox(), new MVCxPivotGrid(), new MVCxPopupControl() };
		}
	}
	public class PivotCustomizationExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(PivotCustomizationExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(PivotGridSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxPivotCustomizationControl(new MVCxPivotGrid()) };
		}
	}
	public class GridViewExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(GridViewExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(GridViewSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxGridView grid = new MVCxGridView();
			grid.Columns.Add(new GridViewDataColumn());
			grid.Settings.ShowFilterRow = true;
			grid.Settings.ShowFilterRowMenu = true;
			grid.SettingsBehavior.ColumnResizeMode = ColumnResizeMode.Control;
			grid.Settings.HorizontalScrollBarMode = ScrollBarMode.Visible;
			grid.Settings.VerticalScrollBarMode = ScrollBarMode.Visible;
			grid.SettingsEditing.Mode = GridViewEditingMode.Batch;
			grid.SettingsPager.AlwaysShowPager = true;
			return new ASPxWebControl[] { new MVCxTextBox(), new MVCxButton(), grid, new MVCxPopupControl(), new MVCxMenu() };
		}
	}
	public class GridLookupExtensionRegistrator: ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(GridLookupExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(GridLookupSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxGridView grid = new MVCxGridView();
			grid.Columns.Add(new GridViewDataColumn());
			grid.Settings.ShowFilterRow = true;
			grid.Settings.ShowFilterRowMenu = true;
			grid.SettingsBehavior.ColumnResizeMode = ColumnResizeMode.Control;
			grid.Settings.HorizontalScrollBarMode = ScrollBarMode.Visible;
			grid.Settings.VerticalScrollBarMode = ScrollBarMode.Visible;
			grid.SettingsPager.AlwaysShowPager = true;
			return new ASPxWebControl[] { new MVCxTextBox(), grid, new MVCxPopupControl(), new MVCxGridLookup() };
		}
	}
	public class CardViewExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(CardViewExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(CardViewSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxCardView() };
		}
	}
	public class TreeListExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(TreeListExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(TreeListSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxTreeList treeList = new MVCxTreeList();
			treeList.Columns.Add(new TreeListDataColumn());
			treeList.SettingsPager.AlwaysShowPager = true;
			return new ASPxWebControl[] { new MVCxTextBox(), treeList, new MVCxPopupControl() };
		}
	}
	public class SpreadsheetExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(SpreadsheetExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(SpreadsheetSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxSpreadsheet(), new ASPxRibbon() };
		}
		protected override ControlRibbonImages GetRibbonImages(ASPxWebControl control) {
			MVCxSpreadsheet spreadsheet = control as MVCxSpreadsheet;
			return spreadsheet != null ? new SpreadsheetRibbonImages(spreadsheet, spreadsheet.Images.MenuIconSet) : null;
		}
	}
	public class RichEditExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(RichEditExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(RichEditSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxRichEdit() };
		}
		protected override ControlRibbonImages GetRibbonImages(ASPxWebControl control) {
			MVCxRichEdit richEdit = control as MVCxRichEdit;
			return richEdit != null ? new RichEditRibbonImages(richEdit, richEdit.Images.MenuIconSet) : null;
		}
	}
	public class HtmlEditorExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(HtmlEditorExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(HtmlEditorSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxHtmlEditor htmlEditor = new MVCxHtmlEditor();
			htmlEditor.CreateDefaultToolbars(false);
			ToolbarComboBox comboBox = new ToolbarComboBox();
			comboBox.CommandName = "dummy_item1";
			comboBox.DefaultCaption = "dummy_item1";
			comboBox.Items.Add("dummy_item1_item", "dummy_item1_item");
			htmlEditor.Toolbars[0].Items.Add(comboBox);
			ToolbarDropDownItemPicker itemPicker = new ToolbarDropDownItemPicker();
			itemPicker.CommandName = "dummy_item2";
			itemPicker.Items.Add("dummy_item2_item", "dummy_item2_item");
			htmlEditor.Toolbars[0].Items.Add(itemPicker);
			ToolbarDropDownMenu dropDownMenu = new ToolbarDropDownMenu();
			dropDownMenu.CommandName = "dummy_item3";
			dropDownMenu.Items.Add("dummy_item3_item", "dummy_item3_item");
			htmlEditor.Toolbars[0].Items.Add(dropDownMenu);
			return new ASPxWebControl[] { new MVCxTextBox(), htmlEditor, new MVCxPopupControl(), new MVCxHtmlEditorSpellChecker(htmlEditor) };
		}
		protected override ControlRibbonImages GetRibbonImages(ASPxWebControl control) {
			MVCxHtmlEditor htmlEditor = control as MVCxHtmlEditor;
			return htmlEditor != null ? new HERibbonImages(htmlEditor, htmlEditor.Images.MenuIconSet) : null;
		}
	}
	public class SpellCheckerRegistrator: ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(SpellCheckerExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(SpellCheckerSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxSpellChecker() };
		}
	}
	public class CallbackPanelExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(CallbackPanelExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(CallbackPanelSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxCallbackPanel() { FixedPosition = PanelFixedPosition.WindowTop } };
		}
	}
	public class PanelExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(PanelExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(PanelSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxPanel() { FixedPosition = PanelFixedPosition.WindowTop } };
		}
	}
	public class DataViewExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(DataViewExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(DataViewSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxDataView dataView = new MVCxDataView();
			dataView.AlwaysShowPager = true;
			return new ASPxWebControl[] { dataView };
		}
	}
	public class DockManagerExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(DockManagerExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(DockManagerSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxDockManager() };
		}
	}
	public class DockPanelExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(DockPanelExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(DockPanelSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxDockPanel() };
		}
	}
	public class DockZoneExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(DockZoneExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(DockZoneSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxDockZone() };
		}
	}
	public class FileManagerExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(FileManagerExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(FileManagerSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxGridView grid = new MVCxGridView();
			grid.Columns.Add(new GridViewDataColumn());
			grid.Settings.ShowHeaderFilterButton = true;
			grid.Settings.VerticalScrollBarMode = ScrollBarMode.Visible;
			grid.SettingsBehavior.ColumnResizeMode = ColumnResizeMode.NextColumn;
			grid.SettingsBehavior.AllowSelectByRowClick = true;
			grid.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;
			return new ASPxWebControl[] { new MVCxTextBox(), new MVCxFileManager(), grid };
		}
	}
	public class ImageGalleryExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ImageGalleryExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ImageGallerySettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxImageGallery imageGallery = new MVCxImageGallery();
			imageGallery.AlwaysShowPager = true;
			return new ASPxWebControl[] { imageGallery };
		}
	}
	public class ImageSliderExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ImageSliderExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ImageSliderSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxImageSlider() };
		}
	}
	public class ImageZoomExtensionRegistrator: ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ImageZoomExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ImageZoomSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxImageZoom() };
		}
	}
	public class ImageZoomNavigatorExtensionRegistrator: ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ImageZoomNavigatorExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ImageZoomNavigatorSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxImageZoomNavigator() };
		}
	}
	public class FormLayoutExtensionRegistrator: ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(FormLayoutExtension<>); }
		}
		public override Type SettingsClass {
			get { return typeof(FormLayoutSettings<>); }
		}
		protected override ASPxWebControl[] GetControls() {
			var formLayout = new MVCxFormLayout();
			formLayout.Items.Add(new DevExpress.Web.LayoutItem("layout1"));
			return new ASPxWebControl[] { new MVCxFormLayout() };
		}
	}
	public class LoadingPanelExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(LoadingPanelExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(LoadingPanelSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxLoadingPanel() };
		}
	}
	public class MenuExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(MenuExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(MenuSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxMenu menu = new MVCxMenu();
			menu.Items.Add("dummy_item").Items.Add("dummy_item");
			menu.EnableSubMenuScrolling = true;
			return new ASPxWebControl[] { menu };
		}
	}
	public class PopupMenuExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(PopupMenuExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(PopupMenuSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxPopupMenu popupMenu = new MVCxPopupMenu();
			popupMenu.Items.Add("dummy_item").Items.Add("dummy_item");
			return new ASPxWebControl[] { popupMenu };
		}
	}
	public class NavBarExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(NavBarExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(NavBarSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxNavBar navBar = new MVCxNavBar();
			navBar.Groups.Add("dummy_group").Items.Add("dummy_item");
			return new ASPxWebControl[] { navBar };
		}
	}
	public class RibbonExtensionRegistrator: ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(RibbonExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(RibbonSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new ASPxRibbon() };
		}
	}
	public class PopupControlExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(PopupControlExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(PopupControlSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxPopupControl() };
		}
	}
	public class RatingControlExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(RatingControlExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(RatingControlSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new ASPxRatingControl() };
		}
	}
	public class RoundPanelExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(RoundPanelExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(RoundPanelSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxRoundPanel panel = new MVCxRoundPanel();
			panel.EnableClientSideAPI = true;
			return new ASPxWebControl[] { panel };
		}
	}
	public class SplitterExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(SplitterExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(SplitterSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxSplitter splitter = new MVCxSplitter();
			splitter.Panes.Add("dummy_pane");
			return new ASPxWebControl[] { splitter };
		}
	}
	public class PageControlExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(PageControlExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(PageControlSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxPageControl pageControl = new MVCxPageControl();
			pageControl.TabPages.Add("dummy_tabpage");
			return new ASPxWebControl[] { pageControl };
		}
	}
	public class TabControlExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(TabControlExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(TabControlSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxTabControl tabControl = new MVCxTabControl();
			tabControl.Tabs.Add("dummy_tab");
			return new ASPxWebControl[] { tabControl };
		}
	}
	public class TreeViewExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(TreeViewExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(TreeViewSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxTreeView tree = new MVCxTreeView();
			tree.Nodes.Add("dummy_node");
			return new ASPxWebControl[] { tree };
		}
	}
	public class UploadControlExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(UploadControlExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(UploadControlSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxUploadControl upload = new MVCxUploadControl();
			upload.ShowAddRemoveButtons = true;
			upload.ShowProgressPanel = true;
			upload.ShowUploadButton = true;
			return new ASPxWebControl[] { upload };
		}
	}
	public class ButtonExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ButtonExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ButtonSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxButton() };
		}
	}
	public class BinaryImageExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(BinaryImageEditExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(BinaryImageEditSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxBinaryImage image = new MVCxBinaryImage();
			image.Properties.EnableClientSideAPI = true;
			return new ASPxWebControl[] { image };
		}
	}
	public class ButtonEditExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ButtonEditExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ButtonEditSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxButtonEdit edit = new MVCxButtonEdit();
			edit.Buttons.Add("dummy_button");
			edit.MaskSettings.Mask = "dummy_mask";
			edit.DisplayFormatString = "dummy_format";
			return new ASPxWebControl[] { edit };
		}
	}
	public class CalendarExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(CalendarExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(CalendarSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxCalendar() };
		}
	}
	public class CaptchaExtensionRegistrator: ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(CaptchaExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(CaptchaSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxCaptcha() };
		}
	}
	public class CheckBoxExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(CheckBoxExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(CheckBoxSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxCheckBox() };
		}
	}
	public class CheckBoxListExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(CheckBoxListExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(CheckBoxListSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxCheckBoxList() };
		}
	}
	public class ColorEditExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ColorEditExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ColorEditSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxColorEdit edit = new MVCxColorEdit();
			edit.DisplayFormatString = "dummy_format";
			return new ASPxWebControl[] { edit };
		}
	}
	public class TrackBarExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(TrackBarExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(TrackBarSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxTrackBar() };
		}
	}
	public class ComboBoxExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ComboBoxExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ComboBoxSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxComboBox edit = new MVCxComboBox();
			edit.DisplayFormatString = "dummy_format";
			return new ASPxWebControl[] { edit };
		}
	}
	public class TokenBoxExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(TokenBoxExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(TokenBoxSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxTokenBox() };
		}
	}
	public class DateEditExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(DateEditExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(DateEditSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxDateEdit edit = new MVCxDateEdit();
			edit.UseMaskBehavior = true;
			edit.DisplayFormatString = "dummy_format";
			edit.TimeSectionProperties.Visible = true;
			return new ASPxWebControl[] { edit };
		}
	}
	public class DropDownEditExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(DropDownEditExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(DropDownEditSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxDropDownEdit edit = new MVCxDropDownEdit();
			edit.DisplayFormatString = "dummy_format";
			return new ASPxWebControl[] { edit };
		}
	}
	public class HyperLinkExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(HyperLinkExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(HyperLinkSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxHyperLink link = new MVCxHyperLink();
			link.Properties.EnableClientSideAPI = true;
			return new ASPxWebControl[] { link };
		}
	}
	public class ImageExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ImageEditExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ImageEditSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxImage image = new MVCxImage();
			image.Properties.EnableClientSideAPI = true;
			return new ASPxWebControl[] { image };
		}
	}
	public class LabelExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(LabelExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(LabelSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxLabel label = new MVCxLabel();
			label.AssociatedControlID = "dummy_id";
			label.Properties.EnableClientSideAPI = true;
			return new ASPxWebControl[] { label };
		}
	}
	public class ListBoxExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ListBoxExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ListBoxSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxListBox() };
		}
	}
	public class MemoExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(MemoExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(MemoSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxMemo edit = new MVCxMemo();
			edit.DisplayFormatString = "dummy_format";
			return new ASPxWebControl[] { edit };
		}
	}
	public class ProgressBarExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ProgressBarExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ProgressBarSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxProgressBar() };
		}
	}
	public class RadioButtonExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(RadioButtonExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(RadioButtonSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxRadioButton() };
		}
	}
	public class RadioButtonListExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(RadioButtonListExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(RadioButtonListSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxRadioButtonList() };
		}
	}
	public class SpinEditExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(SpinEditExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(SpinEditSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxSpinEdit edit = new MVCxSpinEdit();
			edit.DisplayFormatString = "dummy_format";
			return new ASPxWebControl[] { edit };
		}
	}
	public class TextBoxExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(TextBoxExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(TextBoxSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxTextBox edit = new MVCxTextBox();
			edit.MaskSettings.Mask = "dummy_mask";
			edit.DisplayFormatString = "dummy_format";
			return new ASPxWebControl[] { edit };
		}
	}
	public class TimeEditExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(TimeEditExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(TimeEditSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxTimeEdit edit = new MVCxTimeEdit();
			edit.DisplayFormatString = "dummy_format";
			return new ASPxWebControl[] { edit };
		}
	}
	public class ValidationSummaryRegistrator: ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ValidationSummaryExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ValidationSummarySettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxValidationSummary() };
		}
	}
	public class ChartExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ChartControlExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ChartControlSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			MVCxChartControl chart = new MVCxChartControl();
			chart.EnableClientSideAPI = true;
			return new ASPxWebControl[] { chart };
		}
	}
	public class ReportDesignerExtensionRegistrator : ExtensionRegistrator {
		static ReportDesignerExtensionRegistrator() {
			MVCxReportDesigner.StaticInitialize();
		}
		public override Type ExtensionClass {
			get { return typeof(ReportDesignerExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ReportDesignerSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxReportDesigner() };
		}
	}
	public class WebDocumentViewerExtensionRegistrator : ExtensionRegistrator {
		static WebDocumentViewerExtensionRegistrator() {
			MVCxWebDocumentViewer.StaticInitialize();
		}
		public override Type ExtensionClass {
			get { return typeof(WebDocumentViewerExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(WebDocumentViewerSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxWebDocumentViewer() };
		}
	}
	public class DocumentViewerExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(DocumentViewerExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(DocumentViewerSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxDocumentViewer() };
		}
		protected override ControlRibbonImages GetRibbonImages(ASPxWebControl control) {
			MVCxDocumentViewer documentViewer = control as MVCxDocumentViewer;
			return documentViewer != null ? new DocumentViewerRibbonImages(documentViewer, documentViewer.SettingsRibbon.IconSet) : null;
		}
	}
	public class ReportViewerExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ReportViewerExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ReportViewerSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxReportViewer() };
		}
	}
	public class ReportToolbarExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(ReportToolbarExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(ReportToolbarSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxReportToolbar() };
		}
	}
	public class SchedulerExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(SchedulerExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(SchedulerSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			var scheduler = new MVCxScheduler();
			scheduler.ID = "dummyScheduler";
			return new ASPxWebControl[] { new MVCxTimeZoneEdit(scheduler), new ASPxSchedulerAppointmentToolTipControl(scheduler), scheduler, new MVCxViewNavigator(scheduler), new MVCxViewVisibleInterval(scheduler), new MVCxViewSelector(scheduler), new AppointmentRecurrenceForm(),
						new MVCxViewNavigator(scheduler), new MVCxResourceNavigator(scheduler), new ASPxSchedulerStatusInfo()};
		}
	}
	public class TimeZoneEditExtensionRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(TimeZoneEditExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(TimeZoneEditSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxTimeZoneEdit(null) };
		}
	}
	public class AppointmentRecurrenceFormExtensionRegistrator: ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(AppointmentRecurrenceFormExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(AppointmentRecurrenceFormSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new AppointmentRecurrenceForm() };
		}
	}
	public class SchedulerStatusInfoExtensionRegistrator: ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(SchedulerStatusInfoExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(SchedulerStatusInfoSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new ASPxSchedulerStatusInfo() };
		}
	}
	public class DateNavigatorExtensionRegistrator: ExtensionRegistrator {
		public override Type ExtensionClass {
			get { return typeof(DateNavigatorExtension); }
		}
		public override Type SettingsClass {
			get { return typeof(SchedulerSettings); }
		}
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxDateNavigator(null) };
		}
	}
	public class Icons16x16Registrator: ExtensionRegistrator {
		public override Type ExtensionClass { get { return null; } }
		public override Type SettingsClass { get { return null; } }
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxDummyIconControl(ExtensionType.Icons16x16) };
		}
	}
	public class Icons16x16grayRegistrator: ExtensionRegistrator {
		public override Type ExtensionClass { get { return null; } }
		public override Type SettingsClass { get { return null; } }
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxDummyIconControl(ExtensionType.Icons16x16gray) };
		}
	}
	public class Icons16x16office2013Registrator : ExtensionRegistrator {
		public override Type ExtensionClass { get { return null; } }
		public override Type SettingsClass { get { return null; } }
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxDummyIconControl(ExtensionType.Icons16x16office2013) };
		}
	}
	public class Icons32x32Registrator : ExtensionRegistrator {
		public override Type ExtensionClass { get { return null; } }
		public override Type SettingsClass { get { return null; } }
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxDummyIconControl(ExtensionType.Icons32x32) };
		}
	}
	public class Icons32x32grayRegistrator: ExtensionRegistrator {
		public override Type ExtensionClass { get { return null; } }
		public override Type SettingsClass { get { return null; } }
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxDummyIconControl(ExtensionType.Icons32x32gray) };
		}
	}
	public class Icons32x32office2013Registrator : ExtensionRegistrator {
		public override Type ExtensionClass { get { return null; } }
		public override Type SettingsClass { get { return null; } }
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxDummyIconControl(ExtensionType.Icons32x32office2013) };
		}
	}
	public class Icons16x16devavRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass { get { return null; } }
		public override Type SettingsClass { get { return null; } }
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxDummyIconControl(ExtensionType.Icons16x16devav) };
		}
	}
	public class Icons32x32devavRegistrator : ExtensionRegistrator {
		public override Type ExtensionClass { get { return null; } }
		public override Type SettingsClass { get { return null; } }
		protected override ASPxWebControl[] GetControls() {
			return new ASPxWebControl[] { new MVCxDummyIconControl(ExtensionType.Icons32x32devav) };
		}
	}
}
