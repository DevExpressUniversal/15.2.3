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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using DevExpress.Web.Internal;
using DevExpress.Web.Rendering;
namespace DevExpress.Web {
	public class CardViewTemplates : PropertiesBase {
		ITemplate cardHeader, cardFooter, card, editForm, dataItem, editItem, header, pagerBar, titlePanel, statusBar, headerPanel;
		public CardViewTemplates(IPropertiesOwner owner)
			: base(owner) {
		}
		[AutoFormatEnable]
		[Browsable(false)]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(CardViewCardHeaderTemplateContainer))]
		public virtual ITemplate CardHeader {
			get { return cardHeader; }
			set { 
				if(cardHeader == value) return;
				cardHeader = value;
				Changed();
			} 
		}
		[AutoFormatEnable]
		[Browsable(false)]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(CardViewCardFooterTemplateContainer))]
		public virtual ITemplate CardFooter {
			get { return cardFooter; }
			set {
				if(cardFooter == value) return;
				cardFooter = value;
				Changed();
			} 
		}
		[AutoFormatEnable]
		[Browsable(false)]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(CardViewCardTemplateContainer))]
		public virtual ITemplate Card {
			get { return card; }
			set {
				if(card == value) return;
				card = value;
				Changed();
			}
		}
		[AutoFormatEnable]
		[Browsable(false)]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(CardViewEditFormTemplateContainer), BindingDirection.TwoWay)]
		public virtual ITemplate EditForm {
			get { return editForm; }
			set {
				if(editForm == value) return;
				editForm = value;
				Changed();
			}
		}
		[AutoFormatEnable]
		[Browsable(false)]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(CardViewDataItemTemplateContainer))]
		public virtual ITemplate DataItem {
			get { return dataItem; }
			set {
				if(dataItem == value) return;
				dataItem = value;
				Changed();
			}
		}
		[AutoFormatEnable]
		[Browsable(false)]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(CardViewEditItemTemplateContainer), BindingDirection.TwoWay)]
		public virtual ITemplate EditItem {
			get { return editItem; }
			set {
				if(editItem == value) return;
				editItem = value;
				Changed();
			}
		}
		[AutoFormatEnable]
		[Browsable(false)]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(CardViewHeaderTemplateContainer))]
		public virtual ITemplate Header {
			get { return header; }
			set {
				if(header == value) return;
				header = value;
				Changed();
			}
		}
		[AutoFormatEnable]
		[Browsable(false)]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(CardViewPagerBarTemplateContainer))]
		public virtual ITemplate PagerBar {
			get { return pagerBar; }
			set {
				if(pagerBar == value) return;
				pagerBar = value;
				Changed();
			}
		}
		[AutoFormatEnable]
		[Browsable(false)]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(CardViewPagerBarTemplateContainer))]
		public virtual ITemplate TitlePanel {
			get { return titlePanel; }
			set {
				if(titlePanel == value) return;
				titlePanel = value;
				Changed();
			}
		}
		[AutoFormatEnable]
		[Browsable(false)]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(CardViewPagerBarTemplateContainer))]
		public virtual ITemplate StatusBar {
			get { return statusBar; }
			set {
				if(statusBar == value) return;
				statusBar = value;
				Changed();
			}
		}
		[AutoFormatEnable]
		[Browsable(false)]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(CardViewHeaderPanelTemplateContainer))]
		public virtual ITemplate HeaderPanel {
			get { return headerPanel; }
			set {
				if(headerPanel == value) return;
				headerPanel = value;
				Changed();
			}
		}
	}
	public class CardViewEditItemTemplateContainer : CardViewDataItemTemplateContainer {
		public CardViewEditItemTemplateContainer(ASPxCardView grid, int visibleIndex, object dataItem, CardViewColumn column, CardViewColumnLayoutItem layoutItem)
			: base(grid, visibleIndex, dataItem, column, layoutItem) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("CardViewEditItemTemplateContainerValidationGroup")]
#endif
		public string ValidationGroup { get { return Grid.EditTemplateValidationGroup; } }
		protected internal override string GetID() {
			return string.Format("edit{0}_{1}", VisibleIndexPrefix, LayoutItem != null ? LayoutItem.Path : string.Empty);
		}
		protected override bool NeedLoadPostData { get { return true; } }
	}
	public class CardViewDataItemTemplateContainer : CardViewCardBaseTemplateContainer {
		public CardViewDataItemTemplateContainer(ASPxCardView grid, int visibleIndex, object dataItem, CardViewColumn column, CardViewColumnLayoutItem layoutItem)
			: base(grid, visibleIndex, dataItem) {
			Column = column;
			LayoutItem = layoutItem;
		}
		protected CardViewColumnLayoutItem LayoutItem { get; private set; }
		public CardViewColumn Column { get; private set; }
		public string Text {
			get {
				string text = Column != null ? RenderHelper.TextBuilder.GetRowDisplayText(Column, VisibleIndex) : String.Empty;
				return string.IsNullOrEmpty(text) ? "&nbsp;" : text;
			}
		}
		protected CardViewRenderHelper RenderHelper { get { return CardView.RenderHelper; } }
		protected override string IdPrefix { get { return "cell"; } }
		protected internal override string GetID() {
			return string.Format("{0}{1}_{2}", IdPrefix, VisibleIndexPrefix, LayoutItem.Path);
		}
	}
	public class CardViewCardTemplateContainer : CardViewCardBaseTemplateContainer{
		public CardViewCardTemplateContainer (ASPxCardView grid, int visibleIndex, object dataItem)
			:base(grid, visibleIndex, dataItem){
		}
		protected override string IdPrefix { get { return "cv"; } }
	}
	public class CardViewCardHeaderTemplateContainer : CardViewCardBaseTemplateContainer {
		public CardViewCardHeaderTemplateContainer(ASPxCardView grid, int visibleIndex, object dataItem)
			: base(grid, visibleIndex, dataItem) {
		}
		protected override string IdPrefix { get { return "ch"; } }
	}
	public class CardViewCardFooterTemplateContainer : CardViewCardBaseTemplateContainer {
		public CardViewCardFooterTemplateContainer(ASPxCardView grid, int visibleIndex, object dataItem)
			: base(grid, visibleIndex, dataItem) {
		}
		protected override string IdPrefix { get { return "cf"; } }
	}
	public abstract class CardViewCardBaseTemplateContainer : CardViewBaseTemplateContainer {
		public CardViewCardBaseTemplateContainer(ASPxCardView grid, int visibleIndex, object dataItem)
			: base(grid, visibleIndex, dataItem) {
		}
		protected abstract string IdPrefix { get; }
#if !SL
	[DevExpressWebLocalizedDescription("CardViewCardBaseTemplateContainerKeyValue")]
#endif
		public object KeyValue { get { return Grid.DataProxy.GetRowKeyValue(VisibleIndex); } }
#if !SL
	[DevExpressWebLocalizedDescription("CardViewCardBaseTemplateContainerKeyValue")]
#endif
		public int VisibleIndex { get { return ItemIndex; } }
		protected virtual string VisibleIndexPrefix { get { return VisibleIndex < 0 ? "new" : VisibleIndex.ToString(); } }
		protected internal override string GetID() {
			return IdPrefix + VisibleIndexPrefix;
		}
	}
	public class CardViewHeaderTemplateContainer : CardViewBaseTemplateContainer {
		public CardViewHeaderTemplateContainer(CardViewColumn column, GridHeaderLocation headerLocation)
			: base(column.CardView, column.CardView.GetColumnGlobalIndex(column), column) {
			HeaderLocation = headerLocation;
		}
#if !SL
	[DevExpressWebLocalizedDescription("CardViewHeaderTemplateContainerColumn")]
#endif
		public CardViewColumn Column { get { return (CardViewColumn)DataItem; } }
#if !SL
	[DevExpressWebLocalizedDescription("CardViewHeaderTemplateContainerHeaderLocation")]
#endif
		public GridHeaderLocation HeaderLocation { get; private set; }
		protected internal override string GetID() {
			string location = "";
			if(HeaderLocation == GridHeaderLocation.Group) {
				location = "G";
			}
			if(HeaderLocation == GridHeaderLocation.Customization) {
				location = "C";
			}
			return string.Format("header{0}{1}", location, Grid.GetColumnGlobalIndex(Column));
		}
	}
	public class CardViewPagerBarTemplateContainer : CardViewBaseTemplateContainer {
		public CardViewPagerBarTemplateContainer(ASPxCardView grid, GridViewPagerBarPosition position, string pagerId)
			: base(grid) {
			PagerId = pagerId;
		}
#if !SL
	[DevExpressWebLocalizedDescription("CardViewPagerBarTemplateContainerPosition")]
#endif
		public GridViewPagerBarPosition Position { get; private set; }
		protected internal string PagerId { get; private set; }
		protected override bool NeedLoadPostData { get { return true; } }
		protected string GetIDSuffix() { return Position == GridViewPagerBarPosition.Top ? "T" : "B"; }
		protected internal override string GetID() { return "PagerBar" + GetIDSuffix(); }
	}
	public class CardViewEditFormTemplateContainer : CardViewCardBaseTemplateContainer {
		public CardViewEditFormTemplateContainer(ASPxCardView grid, int visibleIndex, object dataItem)
			:base(grid, visibleIndex, dataItem){
		}
#if !SL
	[DevExpressWebLocalizedDescription("CardViewEditFormTemplateContainerValidationGroup")]
#endif
		public string ValidationGroup { get { return Grid.EditTemplateValidationGroup; } }
#if !SL
	[DevExpressWebLocalizedDescription("CardViewEditFormTemplateContainerCancelAction")]
#endif
		public string CancelAction { get { return Scripts.GetCancelEditFunction(); } }
#if !SL
	[DevExpressWebLocalizedDescription("CardViewEditFormTemplateContainerUpdateAction")]
#endif
		public string UpdateAction { get { return Scripts.GetUpdateEditFunction(); } }
		protected override string IdPrefix { get { return "ef"; } }
		protected ASPxGridScripts Scripts { get { return Grid.RenderHelper.Scripts; } }
		protected override bool NeedLoadPostData { get { return true; } }
		public override void DataBind() {
			GridRenderHelper.EnsureTemplateReplacements(this);
			base.DataBind();
		}
	}
	public class CardViewTitleTemplateContainer : CardViewBaseTemplateContainer {
		public CardViewTitleTemplateContainer(ASPxCardView grid)
			: base(grid) {
		}
		protected override bool NeedLoadPostData { get { return true; } }
		protected internal override string GetID() { return "Title"; }
	}
	public class CardViewStatusBarTemplateContainer : CardViewBaseTemplateContainer {
		public CardViewStatusBarTemplateContainer(ASPxCardView grid)
			: base(grid) {
		}
		protected override bool NeedLoadPostData { get { return true; } }
		protected internal override string GetID() { return "StatusBar"; }
	}
	public class CardViewHeaderPanelTemplateContainer : CardViewBaseTemplateContainer {
		public CardViewHeaderPanelTemplateContainer(ASPxCardView grid)
			: base(grid) {
		}
		protected internal override string GetID() { return "HeaderPanel"; }
	}
	public abstract class CardViewBaseTemplateContainer : GridBaseTemplateContainer {
		public CardViewBaseTemplateContainer(ASPxCardView grid)
			: base(grid) {
		}
		public CardViewBaseTemplateContainer(ASPxCardView grid, int index, object dataItem)
			: base(grid, index, dataItem) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("CardViewBaseTemplateContainerCardView")]
#endif
		public ASPxCardView CardView { get { return (ASPxCardView)base.Grid; } }
	}
}
