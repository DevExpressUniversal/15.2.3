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
	using DevExpress.Utils;
	using DevExpress.Web;
	using DevExpress.Web.Localization;
	using DevExpress.Web.Internal;
	public abstract class DataViewSettingsBase : SettingsBase {
		DataViewFlowLayoutSettings settingsFlowLayout;
		DataViewTableLayoutSettings settingsTableLayout;
		SettingsLoadingPanel settingsLoadingPanel;
		PagerSettingsEx settingsPager;
		public DataViewSettingsBase() {
			this.settingsFlowLayout = CreateFlowLayoutSettings();
			this.settingsTableLayout = CreateTableLayoutSettings();
			this.settingsLoadingPanel = new SettingsLoadingPanel(null);
			this.settingsPager = CreatePagerSettings();
			AllButtonPageCount = 0;
			AllowPaging = true;
			AlwaysShowPager = false;
			EmptyDataText = String.Empty;
			EnablePagingGestures = AutoBoolean.Auto;
			PageIndex = 0;
			PagerAlign = PagerAlign.Center;
		}
		public bool AccessibilityCompliant { get { return AccessibilityCompliantInternal; } set { AccessibilityCompliantInternal = value; } }
		public int AllButtonPageCount { get; set; }
		public bool AllowPaging { get; set; }
		public bool AlwaysShowPager { get; set; }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public new DataViewStyle ControlStyle { get { return (DataViewStyle)base.ControlStyle; } }
		public string EmptyDataText { get; set; }
		public bool EnableCallbackAnimation { get; set; }
		public bool EnablePagingCallbackAnimation { get; set; }
		public AutoBoolean EnablePagingGestures { get; set; }
		public ImagesBase Images { get { return ImagesInternal; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int LoadingPanelDelay { get { return SettingsLoadingPanel.Delay; } set { SettingsLoadingPanel.Delay = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImagePosition LoadingPanelImagePosition { get { return SettingsLoadingPanel.ImagePosition; } set { SettingsLoadingPanel.ImagePosition = value; } }
		public LoadingPanelStyle LoadingPanelStyle { get { return new LoadingPanelStyle(); } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string LoadingPanelText { get { return SettingsLoadingPanel.Text; } set { SettingsLoadingPanel.Text = value; } }
		public int PageIndex { get; set; }
		public PagerAlign PagerAlign { get; set; }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowLoadingPanel { get { return SettingsLoadingPanel.Enabled; } set { SettingsLoadingPanel.Enabled = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowLoadingPanelImage { get { return SettingsLoadingPanel.ShowImage; } set { SettingsLoadingPanel.ShowImage = value; } }
		public SettingsLoadingPanel SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public DataViewStyles Styles { get { return (DataViewStyles)StylesInternal; } }
		public EventHandler BeforeGetCallbackResult { get; set; }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		public EventHandler DataBinding { get; set; }
		public EventHandler DataBound { get; set; }
		protected internal DataViewFlowLayoutSettings SettingsFlowLayoutInternal { get { return settingsFlowLayout; } }
		protected internal DataViewTableLayoutSettings SettingsTableLayoutInternal { get { return settingsTableLayout; } }
		protected internal PagerSettingsEx PagerSettingsInternal { get { return settingsPager; } }
		protected internal string EmptyDataTemplateContent { get; set; }
		protected internal Action<DataViewTemplateContainer> EmptyDataTemplateContentMethod { get; set; }
		protected internal string PagerPanelLeftTemplateContent { get; set; }
		protected internal Action<DataViewPagerPanelTemplateContainer> PagerPanelLeftTemplateContentMethod { get; set; }
		protected internal string PagerPanelRightTemplateContent { get; set; }
		protected internal Action<DataViewPagerPanelTemplateContainer> PagerPanelRightTemplateContentMethod { get; set; }
		public void SetEmptyDataTemplateContent(Action<DataViewTemplateContainer> contentMethod) {
			EmptyDataTemplateContentMethod = contentMethod;
		}
		public void SetEmptyDataTemplateContent(string content) {
			EmptyDataTemplateContent = content;
		}
		public void SetPagerPanelLeftTemplateContent(Action<DataViewPagerPanelTemplateContainer> contentMethod) {
			PagerPanelLeftTemplateContentMethod = contentMethod;
		}
		public void SetPagerPanelLeftTemplateContent(string content) {
			PagerPanelLeftTemplateContent = content;
		}
		public void SetPagerPanelRightTemplateContent(Action<DataViewPagerPanelTemplateContainer> contentMethod) {
			PagerPanelRightTemplateContentMethod = contentMethod;
		}
		public void SetPagerPanelRightTemplateContent(string content) {
			PagerPanelRightTemplateContent = content;
		}
		protected virtual DataViewFlowLayoutSettings CreateFlowLayoutSettings() {
			return new DataViewFlowLayoutSettings(null);
		}
		protected virtual DataViewTableLayoutSettings CreateTableLayoutSettings() {
			return new DataViewTableLayoutSettings(null);
		}
		protected abstract PagerSettingsEx CreatePagerSettings();
		protected override AppearanceStyleBase CreateControlStyle() {
			return new DataViewStyle();
		}
		protected override ImagesBase CreateImages() {
			return new ImagesBase(null);
		}
		protected override StylesBase CreateStyles() {
			return new DataViewStyles(null);
		}
	}
	public class DataViewSettings : DataViewSettingsBase {
		public DataViewSettings() {
			HideEmptyRows = true;
			Layout = Layout.Table;
		}
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public CallbackClientSideEventsBase ClientSideEvents { get { return (CallbackClientSideEventsBase)ClientSideEventsInternal; } }
		[Obsolete("This property is now obsolete. Use the SettingsTableLayout.RowsPerPage property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int RowPerPage { 
			get { return SettingsTableLayout.RowsPerPage; }
			set { SettingsTableLayout.RowsPerPage = value; } 
		}
		[Obsolete("This property is now obsolete. Use the SettingsTableLayout.ColumnCount property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int ColumnCount { 
			get { return SettingsTableLayout.ColumnCount; } 
			set { SettingsTableLayout.ColumnCount = value; } 
		}
		public bool EnableScrolling { get; set; }
		public bool HideEmptyRows { get; set; }
		public Layout Layout { get; set; }
		public DataViewFlowLayoutSettings SettingsFlowLayout { get { return SettingsFlowLayoutInternal; } }
		public DataViewTableLayoutSettings SettingsTableLayout { get { return SettingsTableLayoutInternal; } }
		public MVCxDataViewPagerSettings PagerSettings { get { return (MVCxDataViewPagerSettings)PagerSettingsInternal; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		protected internal string EmptyItemTemplateContent { get; set; }
		protected internal Action<DataViewTemplateContainer> EmptyItemTemplateContentMethod { get; set; }
		protected internal string ItemTemplateContent { get; set; }
		protected internal Action<DataViewItemTemplateContainer> ItemTemplateContentMethod { get; set; }
		public void SetEmptyItemTemplateContent(Action<DataViewTemplateContainer> contentMethod) {
			EmptyItemTemplateContentMethod = contentMethod;
		}
		public void SetEmptyItemTemplateContent(string content) {
			EmptyItemTemplateContent = content;
		}
		public void SetItemTemplateContent(Action<DataViewItemTemplateContainer> contentMethod) {
			ItemTemplateContentMethod = contentMethod;
		}
		public void SetItemTemplateContent(string content) {
			ItemTemplateContent = content;
		}
		protected override PagerSettingsEx CreatePagerSettings() {
			return new MVCxDataViewPagerSettings(null) { DataViewSettings = this };
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new CallbackClientSideEventsBase();
		}
	}
	public class MVCxDataViewPagerSettings : DataViewPagerSettings {
		public MVCxDataViewPagerSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		protected internal DataViewSettings DataViewSettings { get; set; }
		protected override PageSizeItemSettings CreatePageSizeItemSettings(IPropertiesOwner owner) {
			return new MVCxDataViewPagerPageSizeItemSettings(owner);
		}
	}
	public class MVCxDataViewPagerPageSizeItemSettings : DataViewPagerPageSizeItemSettings {
		public MVCxDataViewPagerPageSizeItemSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		protected internal new MVCxDataViewPagerSettings PagerSettings {
			get { return Owner as MVCxDataViewPagerSettings; }
		}
		protected internal override bool IsDataViewTableLayout() {
			return PagerSettings.DataViewSettings.Layout == Layout.Table;
		}
	}
}
