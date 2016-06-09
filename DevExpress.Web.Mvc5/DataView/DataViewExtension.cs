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
using System.Web.Mvc;
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	using DevExpress.Data.Linq;
	using DevExpress.Web;
	using DevExpress.Web.Mvc.Internal;
	public abstract class DataViewExtensionBase : ExtensionBase {
		public DataViewExtensionBase(DataViewSettingsBase settings)
			: base(settings) {
		}
		public DataViewExtensionBase(DataViewSettingsBase settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new ASPxDataViewBase Control {
			get { return (ASPxDataViewBase)base.Control; }
		}
		protected internal new DataViewSettingsBase Settings {
			get { return (DataViewSettingsBase)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AllButtonPageCount = Settings.AllButtonPageCount;
			Control.AllowPaging = Settings.AllowPaging;
			Control.AlwaysShowPager = Settings.AlwaysShowPager;
			Control.ClientVisible = Settings.ClientVisible;
			Control.EmptyDataText = Settings.EmptyDataText;
			Control.EnableCallbackAnimation = Settings.EnableCallbackAnimation;
			Control.EnablePagingCallbackAnimation = Settings.EnablePagingCallbackAnimation;
			Control.EnablePagingGestures = Settings.EnablePagingGestures;
			Control.LoadingPanelStyle.CopyFrom(Settings.LoadingPanelStyle);
			Control.PagerAlign = Settings.PagerAlign;
			Control.PageIndex = Settings.PageIndex;
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
			Control.BeforeGetCallbackResult += Settings.BeforeGetCallbackResult;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.DataBinding += Settings.DataBinding;
			Control.DataBound += Settings.DataBound;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.EmptyDataTemplate = ContentControlTemplate<DataViewTemplateContainer>.Create(
				Settings.EmptyDataTemplateContent, Settings.EmptyDataTemplateContentMethod, typeof(DataViewTemplateContainer));
			Control.PagerPanelLeftTemplate = ContentControlTemplate<DataViewPagerPanelTemplateContainer>.Create(
				Settings.PagerPanelLeftTemplateContent, Settings.PagerPanelLeftTemplateContentMethod, typeof(DataViewPagerPanelTemplateContainer));
			Control.PagerPanelRightTemplate = ContentControlTemplate<DataViewPagerPanelTemplateContainer>.Create(
				Settings.PagerPanelRightTemplateContent, Settings.PagerPanelRightTemplateContentMethod, typeof(DataViewPagerPanelTemplateContainer));
		}
		public DataViewExtensionBase Bind(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public DataViewExtensionBase BindToSiteMap(string fileName) {
			return BindToSiteMap(fileName, true);
		}
		public DataViewExtensionBase BindToSiteMap(string fileName, bool showStartingNode) {
			BindToSiteMapInternal(fileName, showStartingNode);
			return this;
		}
		public DataViewExtensionBase BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty);
		}
		public DataViewExtensionBase BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public DataViewExtensionBase BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
		public DataViewExtensionBase BindToLINQ(Type contextType, string tableName) {
			return BindToLINQ(contextType.FullName, tableName);
		}
		public DataViewExtensionBase BindToLINQ(string contextTypeName, string tableName) {
			return BindToLINQ(contextTypeName, tableName, null);
		}
		public DataViewExtensionBase BindToLINQ(Type contextType, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToLINQ(contextType.FullName, tableName, selectingMethod);
		}
		public DataViewExtensionBase BindToLINQ(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToLINQ(contextTypeName, tableName, selectingMethod, null);
		}
		public DataViewExtensionBase BindToLINQ(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			BindToLINQDataSourceInternal(contextTypeName, tableName, selectingMethod, exceptionThrownMethod);
			return this;
		}
		public DataViewExtensionBase BindToEF(Type contextType, string tableName) {
			return BindToEF(contextType.FullName, tableName);
		}
		public DataViewExtensionBase BindToEF(string contextTypeName, string tableName) {
			return BindToEF(contextTypeName, tableName, null);
		}
		public DataViewExtensionBase BindToEF(Type contextType, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToEF(contextType.FullName, tableName, selectingMethod);
		}
		public DataViewExtensionBase BindToEF(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToEF(contextTypeName, tableName, selectingMethod, null);
		}
		public DataViewExtensionBase BindToEF(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			BindToEFDataSourceInternal(contextTypeName, tableName, selectingMethod, exceptionThrownMethod);
			return this;
		}
	}
	public class DataViewExtension : DataViewExtensionBase {
		public DataViewExtension(DataViewSettings settings)
			: base(settings) {
		}
		public DataViewExtension(DataViewSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxDataView Control {
			get { return (MVCxDataView)base.Control; }
		}
		protected internal new DataViewSettings Settings {
			get { return (DataViewSettings)base.Settings; }
		} 
		protected override void AssignInitialProperties() {
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.CustomActionRouteValues = Settings.CustomActionRouteValues;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.EnableScrolling = Settings.EnableScrolling;
			Control.SettingsFlowLayout.Assign(Settings.SettingsFlowLayout);
			Control.SettingsTableLayout.Assign(Settings.SettingsTableLayout);
			Control.HideEmptyRows = Settings.HideEmptyRows;
			Control.Layout = Settings.Layout;
			Control.Images.CopyFrom(Settings.Images);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.PagerSettings.Assign((PagerSettingsEx)Settings.PagerSettings);
			base.AssignInitialProperties();
			Control.RightToLeft = Settings.RightToLeft;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.EmptyItemTemplate = ContentControlTemplate<DataViewTemplateContainer>.Create(
				Settings.EmptyItemTemplateContent, Settings.EmptyItemTemplateContentMethod, typeof(DataViewTemplateContainer));
			Control.ItemTemplate = ContentControlTemplate<DataViewItemTemplateContainer>.Create(
				Settings.ItemTemplateContent, Settings.ItemTemplateContentMethod, typeof(DataViewItemTemplateContainer));
		}
		protected override void RenderCallbackResultControl() {
			if(!Control.IsEndlessPagingCallback) {
				base.RenderCallbackResultControl();
				return;
			}
			var writer = Utils.CreateHtmlTextWriter(Writer);
			Control.ContentControl.RenderEndlessPagingItems(writer);
		}
		protected internal override void PrepareControlProperties() {
			base.PrepareControlProperties();
			Control.ValidateProperties();
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.PerformOnInit();
		}
		protected override Control GetCallbackResultControl() {
			return Control.GetCallbackResultControl();
		}
		protected override void BindToDataSource(object dataSource) {
			Control.DataSource = dataSource;
			Control.RequireDataBinding();
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxDataView();
		}
	}
}
