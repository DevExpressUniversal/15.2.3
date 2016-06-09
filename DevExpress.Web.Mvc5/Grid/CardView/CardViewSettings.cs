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
using System.Linq.Expressions;
namespace DevExpress.Web.Mvc {
	using DevExpress.Data;
	using DevExpress.Web.Data;
	using DevExpress.Web.Mvc.Internal;
	public enum CardViewOperationType { Paging, Filtering, Sorting };
	public class CardViewSettings<CardType> : CardViewSettings {
		public CardViewSettings() {
		}
		public new MVCxCardViewColumnCollection<CardType> Columns {
			get { return (MVCxCardViewColumnCollection<CardType>)base.Columns; }
		}
		public void KeyFields<ValueType>(params Expression<Func<CardType, ValueType>>[] keyExpressions) {
			if(keyExpressions == null) return;
			KeyFieldName = string.Empty;
			List<string> keys = new List<string>();
			foreach(var expression in keyExpressions) {
				if(expression == null) continue;
				string key = ExtensionsHelper.GetFullHtmlFieldName(expression);
				keys.Add(key);
			}
			KeyFieldName = string.Join(WebDataProxy.MultipleKeyFieldSeparator.ToString(), keys);
		}
		protected override MVCxCardViewColumnCollection CreateColumnCollection() {
			return new MVCxCardViewColumnCollection<CardType>();
		}
	}
	public class CardViewSettings : GridSettingsBase {
		public CardViewSettings() {
			EditFormLayoutProperties = new MVCxCardViewFormLayoutProperties();
			CardLayoutProperties = new MVCxCardViewFormLayoutProperties();
			Columns = CreateColumnCollection();
			CustomBindingRouteValuesCollection = new Dictionary<CardViewOperationType, object>();
			EnableCardsCache = true;
			Settings = new ASPxCardViewSettings(null);
			SettingsBehavior = new ASPxCardViewBehaviorSettings(null);
			SettingsEditing = new MVCxCardViewEditingSettings();
			SettingsPager = new MVCxCardViewPagerSettings(this);
			SettingsPopup = new ASPxCardViewPopupControlSettings(null);
			SettingsSearchPanel = new MVCxCardViewSearchPanelSettings();
			SettingsFilterControl = new ASPxCardViewFilterControlSettings(null);
			TotalSummary = new ASPxCardViewSummaryItemCollection(null);
			FormatConditions = new MVCxCardViewFormatConditionCollection();
			SettingsExport = new MVCxCardViewExportSettings();
		}
		public MVCxCardViewFormLayoutProperties CardLayoutProperties { get; private set; }
		public MVCxCardViewColumnCollection Columns { get; private set; }
		public CardViewClientSideEvents ClientSideEvents { get { return (CardViewClientSideEvents)ClientSideEventsInternal; } }
		public IDictionary<CardViewOperationType, object> CustomBindingRouteValuesCollection { get; private set; }
		public MVCxCardViewFormLayoutProperties EditFormLayoutProperties { get; private set; }
		public bool EnableCardsCache { get; set; }
		public CardViewImages Images { get { return (CardViewImages)ImagesInternal; } }
		public ASPxCardViewSettings Settings { get; private set; }
		public ASPxCardViewBehaviorSettings SettingsBehavior { get; private set; }
		public MVCxCardViewEditingSettings SettingsEditing { get; private set; }
		public ASPxCardViewPagerSettings SettingsPager { get; private set; }
		public ASPxCardViewPopupControlSettings SettingsPopup { get; private set; }
		public MVCxCardViewSearchPanelSettings SettingsSearchPanel { get; private set; }
		public ASPxCardViewFilterControlSettings SettingsFilterControl { get; private set; }
		public CardViewStyles Styles { get { return (CardViewStyles)StylesInternal; } }
		public ASPxCardViewSummaryItemCollection TotalSummary { get; private set; }
		public MVCxCardViewFormatConditionCollection FormatConditions { get; private set; }
		public MVCxCardViewExportSettings SettingsExport { get; private set; }
		public ASPxCardViewAfterPerformCallbackEventHandler AfterPerformCallback { get; set; }
		public ASPxCardViewBeforeColumnSortingEventHandler BeforeColumnSorting { get; set; }
		public ASPxCardViewBeforeHeaderFilterFillItemsEventHandler BeforeHeaderFilterFillItems { get; set; }
		public ASPxCardViewDataValidationEventHandler CardValidating { get; set; }
		public ASPxCardViewEditorEventHandler CellEditorInitialize { get; set; }
		public ASPxCardViewCommandButtonEventHandler CommandButtonInitialize { get; set; }
		public ASPxCardViewCustomButtonEventHandler CustomButtonInitialize { get; set; }
		public ASPxCardViewColumnDisplayTextEventHandler CustomColumnDisplayText { get; set; }
		public ASPxCardViewCustomColumnSortEventHandler CustomColumnSort { get; set; }
		public ASPxCardViewClientJSPropertiesEventHandler CustomJSProperties { get; set; }
		public CustomSummaryEventHandler CustomSummaryCalculate { get; set; }
		public ASPxCardViewColumnDataEventHandler CustomUnboundColumnData { get; set; }
		public ASPxCardViewHeaderFilterEventHandler HeaderFilterFillItems { get; set; }
		public ASPxDataInitNewRowEventHandler InitNewCard { get; set; }
		public ASPxCardViewSearchPanelEditorCreateEventHandler SearchPanelEditorCreate { get; set; }
		public ASPxCardViewSearchPanelEditorEventHandler SearchPanelEditorInitialize { get; set; }
		public ASPxCardViewSummaryDisplayTextEventHandler SummaryDisplayText { get; set; }
		public void SetDataItemTemplateContent(Action<CardViewDataItemTemplateContainer> contentMethod) { DataItemTemplateContentMethod = contentMethod; }
		public void SetDataItemTemplateContent(string content) { DataItemTemplateContent = content; }
		public void SetCardTemplateContent(Action<CardViewCardTemplateContainer> contentMethod) { CardTemplateContentMethod = contentMethod; }
		public void SetCardTemplateContent(string content) { CardTemplateContent = content; }
		public void SetCardFooterTemplateContent(Action<CardViewCardFooterTemplateContainer> contentMethod) { CardFooterTemplateContentMethod = contentMethod; }
		public void SetCardFooterTemplateContent(string content) { CardFooterTemplateContent = content; }
		public void SetCardHeaderTemplateContent(Action<CardViewCardHeaderTemplateContainer> contentMethod) { CardHeaderTemplateContentMethod = contentMethod; }
		public void SetCardHeaderTemplateContent(string content) { CardHeaderTemplateContent = content; }
		public void SetEditFormTemplateContent(Action<CardViewEditFormTemplateContainer> contentMethod) { EditFormTemplateContentMethod = contentMethod; }
		public void SetEditFormTemplateContent(string content) { EditFormTemplateContent = content; }
		public void SetTitlePanelTemplateContent(Action<CardViewTitleTemplateContainer> contentMethod) { TitlePanelTemplateContentMethod = contentMethod; }
		public void SetTitlePanelTemplateContent(string content) { TitlePanelTemplateContent = content; }
		public void SetStatusBarTemplateContent(Action<CardViewStatusBarTemplateContainer> contentMethod) { StatusBarTemplateContentMethod = contentMethod; }
		public void SetStatusBarTemplateContent(string content) { StatusBarTemplateContent = content; }
		public void SetHeaderTemplateContent(Action<CardViewHeaderTemplateContainer> contentMethod) { HeaderTemplateContentMethod = contentMethod; }
		public void SetHeaderTemplateContent(string content) { HeaderTemplateContent = content; }
		public void SetEditItemTemplateContent(Action<CardViewEditItemTemplateContainer> contentMethod) { EditItemTemplateContentMethod = contentMethod; }
		public void SetEditItemTemplateContent(string content) { EditItemTemplateContent = content; }
		public void SetPagerBarTemplateContent(Action<CardViewPagerBarTemplateContainer> contentMethod) { PagerBarTemplateContentMethod = contentMethod; }
		public void SetPagerBarTemplateContent(string content) { PagerBarTemplateContent = content; }
		public void SetHeaderPanelTemplateContent(Action<CardViewHeaderPanelTemplateContainer> contentMethod) { HeaderPanelTemplateContentMethod = contentMethod; }
		public void SetHeaderPanelTemplateContent(string content) { HeaderPanelTemplateContent = content; }
		protected internal Action<CardViewDataItemTemplateContainer> DataItemTemplateContentMethod { get; set; }
		protected internal string DataItemTemplateContent { get; set; }
		protected internal Action<CardViewCardTemplateContainer> CardTemplateContentMethod { get; set; }
		protected internal string CardTemplateContent { get; set; }
		protected internal Action<CardViewCardFooterTemplateContainer> CardFooterTemplateContentMethod { get; set; }
		protected internal string CardFooterTemplateContent { get; set; }
		protected internal Action<CardViewCardHeaderTemplateContainer> CardHeaderTemplateContentMethod { get; set; }
		protected internal string CardHeaderTemplateContent { get; set; }
		protected internal Action<CardViewEditFormTemplateContainer> EditFormTemplateContentMethod { get; set; }
		protected internal string EditFormTemplateContent { get; set; }
		protected internal Action<CardViewTitleTemplateContainer> TitlePanelTemplateContentMethod { get; set; }
		protected internal string TitlePanelTemplateContent { get; set; }
		protected internal Action<CardViewStatusBarTemplateContainer> StatusBarTemplateContentMethod { get; set; }
		protected internal string StatusBarTemplateContent { get; set; }
		protected internal Action<CardViewHeaderTemplateContainer> HeaderTemplateContentMethod { get; set; }
		protected internal string HeaderTemplateContent { get; set; }
		protected internal Action<CardViewEditItemTemplateContainer> EditItemTemplateContentMethod { get; set; }
		protected internal string EditItemTemplateContent { get; set; }
		protected internal Action<CardViewPagerBarTemplateContainer> PagerBarTemplateContentMethod { get; set; }
		protected internal string PagerBarTemplateContent { get; set; }
		protected internal Action<CardViewHeaderPanelTemplateContainer> HeaderPanelTemplateContentMethod { get; set; }
		protected internal string HeaderPanelTemplateContent { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new CardViewClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new CardViewImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new CardViewStyles(null);
		}
		protected virtual MVCxCardViewColumnCollection CreateColumnCollection() {
			return new MVCxCardViewColumnCollection();
		}
	}
}
