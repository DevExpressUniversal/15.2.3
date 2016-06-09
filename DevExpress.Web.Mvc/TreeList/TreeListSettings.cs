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
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.ASPxTreeList;
using DevExpress.Web.Data;
using DevExpress.Web.Mvc.Internal;
using System.Linq.Expressions;
using System.Collections.Generic;
namespace DevExpress.Web.Mvc {
	public class TreeListSettings<RowType>: TreeListSettings {
		public new MVCxTreeListColumnCollection<RowType> Columns { 
			get { return (MVCxTreeListColumnCollection<RowType>)base.Columns; } 
		}
		public void KeyField<ValueType>(Expression<Func<RowType, ValueType>> keyExpression) {
			KeyFieldName = ExtensionsHelper.GetFullHtmlFieldName(keyExpression);
		}
		public void ParentField(Expression<Func<RowType, ValueType>> parentFieldExpression) {
			ParentFieldName = ExtensionsHelper.GetFullHtmlFieldName(parentFieldExpression);
		}
		public void PreviewField(Expression<Func<RowType, ValueType>> expression) {
			PreviewFieldName = ExtensionsHelper.GetFullHtmlFieldName(expression);
		}
		protected override MVCxTreeListColumnCollection CreateColumnCollection() {
			return new MVCxTreeListColumnCollection<RowType>();
		}
	}
	public class TreeListSettings : SettingsBase {
		MVCxTreeListColumnCollection columns;
		MVCxTreeListCommandColumn commandColumn;
		MVCxTreeListNodeCollection nodes;
		MVCxTreeListSummaryCollection summary;
		MVCxTreeListSettings settings;
		MVCxTreeListSettingsBehavior settingsBehavior;
		MVCxTreeListSettingsCookies settingsCookies;
		MVCxTreeListSettingsCustomizationWindow settingsCustomizationWindow;
		MVCxTreeListSettingsEditing settingsEditing;
		MVCxTreeListSettingsExport settingsExport;
		MVCxTreeListSettingsLoadingPanel settingsLoadingPanel;
		MVCxTreeListSettingsPager settingsPager;
		MVCxTreeListSettingsPopupEditForm settingsPopupEditForm;
		MVCxTreeListSettingsDataSecurity settingsDataSecurity;
		MVCxTreeListSettingsSelection settingsSelection;
		MVCxTreeListSettingsText settingsText;
		EditorImages imagesEditors;
		EditorStyles stylesEditors;
		PagerStyles stylesPager;
		public TreeListSettings() {
			this.columns = CreateColumnCollection();
			this.commandColumn = new MVCxTreeListCommandColumn(this);
			this.nodes = new MVCxTreeListNodeCollection();
			this.summary = new MVCxTreeListSummaryCollection();
			this.imagesEditors = new EditorImages(null);
			this.settings = new MVCxTreeListSettings();
			this.settingsBehavior = new MVCxTreeListSettingsBehavior();
			this.settingsCookies = new MVCxTreeListSettingsCookies();
			this.settingsCustomizationWindow = new MVCxTreeListSettingsCustomizationWindow();
			this.settingsEditing = new MVCxTreeListSettingsEditing();
			this.settingsExport = new MVCxTreeListSettingsExport();
			this.settingsLoadingPanel = new MVCxTreeListSettingsLoadingPanel();
			this.settingsPager = new MVCxTreeListSettingsPager();
			this.settingsPopupEditForm = new MVCxTreeListSettingsPopupEditForm();
			this.settingsSelection = new MVCxTreeListSettingsSelection();
			this.settingsText = new MVCxTreeListSettingsText();
			this.settingsDataSecurity = new MVCxTreeListSettingsDataSecurity();
			this.stylesEditors = new EditorStyles(null);
			this.stylesPager = new PagerStyles(null);
			AutoGenerateColumns = true;
			Caption = string.Empty;
			DataCacheMode = TreeListDataCacheMode.Auto;
			EnablePagingGestures = AutoBoolean.Auto;
			KeyFieldName = string.Empty;
			ParentFieldName = string.Empty;
			PreviewFieldName = string.Empty;
			SummaryText = string.Empty;
		}
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public object CustomDataActionRouteValues { get; set; }
		public bool AccessibilityCompliant { get { return AccessibilityCompliantInternal; } set { AccessibilityCompliantInternal = value; } }
		public bool AutoGenerateColumns { get; set; }
		public bool AutoGenerateServiceColumns { get; set; }
		public string Caption { get; set; }
		public TreeListClientSideEvents ClientSideEvents { get { return (TreeListClientSideEvents)ClientSideEventsInternal; } }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public new AppearanceStyle ControlStyle { get { return (AppearanceStyle)base.ControlStyle; } }
		public TreeListDataCacheMode DataCacheMode { get; set; }
		public bool EnableCallbackAnimation { get; set; }
		public bool EnablePagingCallbackAnimation { get; set; }
		public AutoBoolean EnablePagingGestures { get; set; }
		public TreeListImages Images { get { return (TreeListImages)ImagesInternal; } }
		public EditorImages ImagesEditors { get { return imagesEditors; } }
		public bool KeyboardSupport { get; set; }
		public string KeyFieldName { get; set; }
		public string ParentFieldName { get; set; }
		public string PreviewFieldName { get; set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public object RootValue { get; set; }
		public MVCxTreeListSettings Settings { get { return settings; } }
		public MVCxTreeListSettingsBehavior SettingsBehavior { get { return settingsBehavior; } }
		public MVCxTreeListSettingsCookies SettingsCookies { get { return settingsCookies; } }
		public MVCxTreeListSettingsCustomizationWindow SettingsCustomizationWindow { get { return settingsCustomizationWindow; } }
		public MVCxTreeListSettingsEditing SettingsEditing { get { return settingsEditing; } }
		public MVCxTreeListSettingsExport SettingsExport { get { return settingsExport; } }
		public MVCxTreeListSettingsLoadingPanel SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public MVCxTreeListSettingsPager SettingsPager { get { return settingsPager; } }
		public MVCxTreeListSettingsPopupEditForm SettingsPopupEditForm { get { return settingsPopupEditForm; } }
		public MVCxTreeListSettingsDataSecurity SettingsDataSecurity { get { return settingsDataSecurity; } }
		public MVCxTreeListSettingsSelection SettingsSelection { get { return settingsSelection; } }
		public MVCxTreeListSettingsText SettingsText { get { return settingsText; } }
		public TreeListStyles Styles { get { return (TreeListStyles)StylesInternal; } }
		public EditorStyles StylesEditors { get { return stylesEditors; } }
		public PagerStyles StylesPager { get { return stylesPager; } }
		public string SummaryText { get; set; }
		public MVCxTreeListSummaryCollection Summary { get { return summary; } }
		public MVCxTreeListColumnCollection Columns { get { return columns; } }
		public MVCxTreeListCommandColumn CommandColumn { get { return commandColumn; } }
		public MVCxTreeListNodeCollection Nodes { get { return nodes; } }
		public EventHandler BeforeGetCallbackResult { get { return BeforeGetCallbackResultInternal; } set { BeforeGetCallbackResultInternal = value; } }
		public TreeListColumnEditorEventHandler CellEditorInitialize { get; set; }
		public ASPxClientLayoutHandler ClientLayout { get { return ClientLayoutInternal; } set { ClientLayoutInternal = value; } }
		public TreeListCommandColumnButtonEventHandler CommandColumnButtonInitialize { get; set; }
		public TreeListCustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		public EventHandler DataBinding { get; set; }
		public EventHandler DataBound { get; set; }
		public TreeListHtmlCommandCellEventHandler HtmlCommandCellPrepared { get; set; }
		public TreeListHtmlDataCellEventHandler HtmlDataCellPrepared { get; set; }
		public TreeListHtmlRowEventHandler HtmlRowPrepared { get; set; }
		public ASPxDataInitNewRowEventHandler InitNewNode { get; set; }
		public TreeListNodeValidationEventHandler NodeValidating { get; set; }
		public TreeListCustomSummaryEventHandler CustomSummaryCalculate { get; set; }
		public TreeListCustomNodeSortEventHandler CustomNodeSort { get; set; }
		public TreeListVirtualNodeEventHandler VirtualModeNodeCreated { get; set; }
		protected internal string HeaderCaptionTemplateContent { get; set; }
		protected internal Action<TreeListHeaderTemplateContainer> HeaderCaptionTemplateContentMethod { get; set; }
		protected internal string DataCellTemplateContent { get; set; }
		protected internal Action<TreeListDataCellTemplateContainer> DataCellTemplateContentMethod { get; set; }
		protected internal string PreviewTemplateContent { get; set; }
		protected internal Action<TreeListPreviewTemplateContainer> PreviewTemplateContentMethod { get; set; }
		protected internal string GroupFooterCellTemplateContent { get; set; }
		protected internal Action<TreeListFooterCellTemplateContainer> GroupFooterCellTemplateContentMethod { get; set; }
		protected internal string FooterCellTemplateContent { get; set; }
		protected internal Action<TreeListFooterCellTemplateContainer> FooterCellTemplateContentMethod { get; set; }
		protected internal string EditFormTemplateContent { get; set; }
		protected internal Action<TreeListEditFormTemplateContainer> EditFormTemplateContentMethod { get; set; }
		public void SetHeaderCaptionTemplateContent(string content) {
			HeaderCaptionTemplateContent = content;
		}
		public void SetHeaderCaptionTemplateContent(Action<TreeListHeaderTemplateContainer> contentMethod) {
			HeaderCaptionTemplateContentMethod = contentMethod;
		}
		public void SetDataCellTemplateContent(string content) {
			DataCellTemplateContent = content;
		}
		public void SetDataCellTemplateContent(Action<TreeListDataCellTemplateContainer> contentMethod) {
			DataCellTemplateContentMethod = contentMethod;
		}
		public void SetPreviewTemplateContent(string content) {
			PreviewTemplateContent = content;
		}
		public void SetPreviewTemplateContent(Action<TreeListPreviewTemplateContainer> contentMethod) {
			PreviewTemplateContentMethod = contentMethod;
		}
		public void SetGroupFooterCellTemplateContent(string content) {
			GroupFooterCellTemplateContent = content;
		}
		public void SetGroupFooterCellTemplateContent(Action<TreeListFooterCellTemplateContainer> contentMethod) {
			GroupFooterCellTemplateContentMethod = contentMethod;
		}
		public void SetFooterCellTemplateContent(string content) {
			FooterCellTemplateContent = content;
		}
		public void SetFooterCellTemplateContent(Action<TreeListFooterCellTemplateContainer> contentMethod) {
			FooterCellTemplateContentMethod = contentMethod;
		}
		public void SetEditFormTemplateContent(string content) {
			EditFormTemplateContent = content;
		}
		public void SetEditFormTemplateContent(Action<TreeListEditFormTemplateContainer> contentMethod) {
			EditFormTemplateContentMethod = contentMethod;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new TreeListClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new TreeListImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new TreeListStyles(null);
		}
		protected override AppearanceStyleBase CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected virtual MVCxTreeListColumnCollection CreateColumnCollection(){
			return new MVCxTreeListColumnCollection();
		}
	}
}
