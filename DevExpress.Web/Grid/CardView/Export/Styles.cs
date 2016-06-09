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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting;
namespace DevExpress.Web {
	public class CardViewExportGroupStyle : GridExportAppearanceBase {
		[ NotifyParentProperty(true), DefaultValue(1)]
		public override int BorderSize {
			get { return ViewStateUtils.GetIntProperty(ViewState, "BorderSize", 1); }
			set {
				CommonUtils.CheckNegativeValue(value, "BorderSize");
				ViewStateUtils.SetIntProperty(ViewState, "BorderSize", 1, value);
			}
		}
		[ NotifyParentProperty(true), DefaultValue(BorderSide.All)]
		public override BorderSide BorderSides {
			get { return (BorderSide)ViewStateUtils.GetIntProperty(ViewState, "BorderSides", (int)BorderSide.All); }
			set { ViewStateUtils.SetIntProperty(ViewState, "BorderSides", (int)BorderSide.All, (int)value); }
		}
	}
	public class CardViewExportTabStyle : CardViewExportGroupStyle {
		[ NotifyParentProperty(true), DefaultValue(BorderSide.Left | BorderSide.Right | BorderSide.Top)]
		public override BorderSide BorderSides {
			get { return (BorderSide)ViewStateUtils.GetIntProperty(ViewState, "BorderSides", (int)(BorderSide.Left | BorderSide.Right | BorderSide.Top)); }
			set { ViewStateUtils.SetIntProperty(ViewState, "BorderSides", (int)(BorderSide.Left | BorderSide.Right | BorderSide.Top), (int)value); }
		}
	}
	public class CardViewExportDisabledTabStyle : CardViewExportGroupStyle { 
	}
	public class CardViewExportCheckStyle : CardViewExportAppearance {
		[ NotifyParentProperty(true), DefaultValue(1)]
		public override int BorderSize {
			get { return ViewStateUtils.GetIntProperty(ViewState, "BorderSize", 1); }
			set {
				CommonUtils.CheckNegativeValue(value, "BorderSize");
				ViewStateUtils.SetIntProperty(ViewState, "BorderSize", 1, value);
			}
		}
		[ NotifyParentProperty(true), DefaultValue(BorderSide.All)]
		public override BorderSide BorderSides {
			get { return (BorderSide)ViewStateUtils.GetIntProperty(ViewState, "BorderSides", (int)BorderSide.All); }
			set { ViewStateUtils.SetIntProperty(ViewState, "BorderSides", (int)BorderSide.All, (int)value); }
		}
	}
	public class CardViewExportAppearance : GridExportAppearanceBase {
		[ NotifyParentProperty(true), DefaultValue(0)]
		public override int BorderSize { 
			get { return ViewStateUtils.GetIntProperty(ViewState, "BorderSize", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "BorderSize");
				ViewStateUtils.SetIntProperty(ViewState, "BorderSize", 0, value);
			}
		}
		[ NotifyParentProperty(true)]
		public override Color BorderColor { get { return base.BorderColor; } set { base.BorderColor = value; } }
		[ NotifyParentProperty(true), DefaultValue(BorderSide.None)]
		public override BorderSide BorderSides {
			get { return (BorderSide)ViewStateUtils.GetIntProperty(ViewState, "BorderSides", (int)BorderSide.None); }
			set { ViewStateUtils.SetIntProperty(ViewState, "BorderSides", (int)BorderSide.None, (int)value); }
		}
	}
	public class CardViewExportTotalSummaryPanelStyle : GridExportAppearanceBase {
		[ NotifyParentProperty(true), DefaultValue(2)]
		public override int BorderSize {
			get { return ViewStateUtils.GetIntProperty(ViewState, "BorderSize", 2); }
			set {
				CommonUtils.CheckNegativeValue(value, "BorderSize");
				ViewStateUtils.SetIntProperty(ViewState, "BorderSize", 2, value);
			}
		}
		[ NotifyParentProperty(true), DefaultValue(BorderSide.Top | BorderSide.Bottom)]
		public override BorderSide BorderSides {
			get { return (BorderSide)ViewStateUtils.GetIntProperty(ViewState, "BorderSides", (int)(BorderSide.Top | BorderSide.Bottom)); }
			set { ViewStateUtils.SetIntProperty(ViewState, "BorderSides", (int)(BorderSide.Top | BorderSide.Bottom), (int)value); }
		}
	}
	public class CardViewExportOptionalAppearance : GridExportOptionalAppearanceBase {
		[ NotifyParentProperty(true), DefaultValue(DefaultBoolean.Default)]
		public override DefaultBoolean Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
	}
	public class CardViewExportStyles : PropertiesBase {
		public CardViewExportStyles(IPropertiesOwner owner)
			: base(owner) {
			Default = new CardViewExportAppearance();
			Card = new CardViewExportGroupStyle();
			Cell = new CardViewExportAppearance();
			Footer = new CardViewExportAppearance();
			Group = new CardViewExportGroupStyle();
			GroupCaption = new CardViewExportAppearance();
			TabbedGroup = new CardViewExportGroupStyle();
			Tab = new CardViewExportTabStyle();
			DisabledTab = new CardViewExportDisabledTabStyle();
			HyperLink = new CardViewExportAppearance();
			Image = new CardViewExportAppearance();
			Caption = new CardViewExportAppearance();
			Check = new CardViewExportCheckStyle();
			AlternatingRowCell = new CardViewExportOptionalAppearance();
			TotalSummaryItemStyle = new CardViewExportAppearance();
			TotalSummaryPanelStyle = new CardViewExportTotalSummaryPanelStyle();
		}
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportAppearance Default { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportGroupStyle Card { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportAppearance Cell { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportAppearance Footer { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportGroupStyle Group { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportAppearance GroupCaption { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportGroupStyle TabbedGroup { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportTabStyle Tab { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportDisabledTabStyle DisabledTab { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportAppearance HyperLink { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportAppearance Caption { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportCheckStyle Check { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportAppearance Image { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportOptionalAppearance AlternatingRowCell { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportAppearance TotalSummaryItemStyle { get; private set; }
		[ PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportTotalSummaryPanelStyle TotalSummaryPanelStyle { get; private set; }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var styles = source as CardViewExportStyles;
			if(styles != null) {
				Default.Assign(styles.Default);
				Card.Assign(styles.Card);
				Group.Assign(styles.Group);
				GroupCaption.Assign(styles.GroupCaption);
				TabbedGroup.Assign(styles.TabbedGroup);
				Tab.Assign(styles.Tab);
				DisabledTab.Assign(styles.DisabledTab);
				Caption.Assign(styles.Caption);
				Check.Assign(styles.Check);
				Cell.Assign(styles.Cell);
				Footer.Assign(styles.Footer);
				HyperLink.Assign(styles.HyperLink);
				Image.Assign(styles.Image);
				AlternatingRowCell.Assign(styles.AlternatingRowCell);
				TotalSummaryItemStyle.Assign(styles.TotalSummaryItemStyle);
				TotalSummaryPanelStyle.Assign(styles.TotalSummaryPanelStyle);
			}
		}
	}
}
namespace DevExpress.Web.Export {
	public class CardViewStyleHelper : ExportStyleHelperBase {
		protected const int
			CardStyleCacheKey = 7,
			GroupStyleCacheKey = 8,
			GroupCaptionStyleCacheKey = 9,
			TabbedGroupStyleCacheKey = 10,
			TabStyleCacheKey = 11,
			DisabledTabStyleCacheKey = 12,
			CaptionStyleCacheKey = 13,
			CheckStyleCacheKey = 14,
			TotalSummaryStyleCacheKey = 15;
		CardViewExportStyles renderStyles;
		public CardViewStyleHelper(CardViewExportStyles styles, ASPxCardView cardView)
			: base(styles) {
			CardView = cardView;
		}
		CardViewExportStyles RenderStyles {
			get {
				if(renderStyles == null)
					renderStyles = new CardViewExportStyles(null);
				return renderStyles;
			}
		}
		ASPxCardView CardView { get; set; }
		protected override void InitStyles(PropertiesBase userStyles) {
			SetupBuiltInDefault();
			Import(RenderStyles.Default);
			SetupBuiltInStyles();
			var gridViewExportUserStyles = (CardViewExportStyles)userStyles;
			Import(gridViewExportUserStyles.Default);
			Import(gridViewExportUserStyles);
		}
		void SetupBuiltInDefault() {
			RenderStyles.Default.BackColor = Color.Transparent;
			RenderStyles.Default.ForeColor = Color.FromArgb(34, 34, 34);
			RenderStyles.Default.BorderColor = Color.FromArgb(159, 159, 159);
			RenderStyles.Default.Paddings.PaddingLeft = RenderStyles.Default.Paddings.PaddingRight =
				RenderStyles.Default.Paddings.PaddingTop = RenderStyles.Default.Paddings.PaddingBottom = Unit.Pixel(1);
		}
		void SetupBuiltInStyles() {
			RenderStyles.Card.BackColor = Color.White;
			RenderStyles.HyperLink.ForeColor = Color.Blue;
			RenderStyles.HyperLink.Font.Underline = true;
			RenderStyles.HyperLink.BackColor = Color.Empty;
			RenderStyles.TabbedGroup.BackColor = Color.White;
			RenderStyles.Tab.BackColor = Color.White;
			RenderStyles.Tab.Paddings.PaddingLeft = RenderStyles.Tab.Paddings.PaddingRight = Unit.Pixel(8);
			RenderStyles.Tab.Paddings.PaddingTop = RenderStyles.Tab.Paddings.PaddingBottom = Unit.Pixel(3);
			RenderStyles.DisabledTab.BackColor = Color.FromArgb(224, 224, 224);
			RenderStyles.DisabledTab.Paddings.PaddingLeft = RenderStyles.DisabledTab.Paddings.PaddingRight = Unit.Pixel(8);
			RenderStyles.DisabledTab.Paddings.PaddingTop = RenderStyles.DisabledTab.Paddings.PaddingBottom = Unit.Pixel(3);
			RenderStyles.Caption.ForeColor = Color.FromArgb(115, 115, 115);
			RenderStyles.Check.BackColor = Color.FromArgb(240, 240, 240);
			RenderStyles.Check.BorderColor = Color.FromArgb(74, 74, 74);
			RenderStyles.GroupCaption.ForeColor = Color.FromArgb(115, 115, 115);
			RenderStyles.GroupCaption.BackColor = Color.White;
			RenderStyles.TotalSummaryPanelStyle.Paddings.PaddingTop = RenderStyles.DisabledTab.Paddings.PaddingBottom = Unit.Pixel(14);
			RenderStyles.AlternatingRowCell.BackColor = Color.FromArgb(0xededeb);
			RenderStyles.Footer.BackColor = Color.LightYellow;
		}
		void Import(Style commonStyle) {
			RenderStyles.Card.CopyFrom(commonStyle);
			RenderStyles.Group.CopyFrom(commonStyle);
			RenderStyles.GroupCaption.CopyFrom(commonStyle);
			RenderStyles.TabbedGroup.CopyFrom(commonStyle);
			RenderStyles.Tab.CopyFrom(commonStyle);
			RenderStyles.DisabledTab.CopyFrom(commonStyle);
			RenderStyles.Caption.CopyFrom(commonStyle);
			RenderStyles.Check.CopyFrom(commonStyle);
			RenderStyles.Cell.CopyFrom(commonStyle);
			RenderStyles.HyperLink.CopyFrom(commonStyle);
			RenderStyles.Image.CopyFrom(commonStyle);
			RenderStyles.AlternatingRowCell.CopyFrom(commonStyle);
			RenderStyles.Footer.CopyFrom(commonStyle);
			RenderStyles.TotalSummaryItemStyle.CopyFrom(commonStyle);
			RenderStyles.TotalSummaryPanelStyle.CopyFrom(commonStyle);
		}
		void Import(CardViewExportStyles styles) {
			RenderStyles.Card.CopyFrom(styles.Card);
			RenderStyles.Group.CopyFrom(styles.Group);
			RenderStyles.GroupCaption.CopyFrom(styles.GroupCaption);
			RenderStyles.TabbedGroup.CopyFrom(styles.TabbedGroup);
			RenderStyles.Tab.CopyFrom(styles.Tab);
			RenderStyles.DisabledTab.CopyFrom(styles.DisabledTab);
			RenderStyles.Caption.CopyFrom(styles.Caption);
			RenderStyles.Check.CopyFrom(styles.Check);
			RenderStyles.Cell.CopyFrom(styles.Cell);
			RenderStyles.HyperLink.CopyFrom(styles.HyperLink);
			RenderStyles.Image.CopyFrom(styles.Image);
			RenderStyles.AlternatingRowCell.CopyFrom(styles.AlternatingRowCell);
			RenderStyles.Footer.CopyFrom(styles.Footer);
			RenderStyles.TotalSummaryPanelStyle.CopyFrom(styles.TotalSummaryPanelStyle);
		}
		protected override BrickStyle CreateFooterStyle(BrickGraphics graph, HorizontalAlign align) {
			var style = new CardViewExportAppearance();
			style.CopyFrom(RenderStyles.Footer);
			if(style.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = align;
			return ConvertStyle(graph, style);
		}
		public BrickStyle GetCardStyle(BrickGraphics graph, LayoutItemBase layoutItem, int cardIndex) {
			var key = new object[] { CardStyleCacheKey, layoutItem, cardIndex };
			if(StyleCache.ContainsKey(key))
				return StyleCache[key];
			var style = new GridViewExportAppearance();
			style.CopyFrom(RenderStyles.Card);
			var conditionalStyle = CardView.RenderHelper.GetConditionalFormatItemStyle(cardIndex);
			if(conditionalStyle != null)
				style.CopyFrom(conditionalStyle);
			StyleCache[key] = ConvertStyle(graph, style);
			return StyleCache[key];
		}
		bool DrawFormatConditionsStyles(int cardIndex) {
			return CardView.RenderHelper.GetConditionalFormatItemStyle(cardIndex) != null;
		}
		ASPxGridBase GetGrid(IWebGridDataColumn column) {
			return column != null ? column.Adapter.Grid : null;
		}
		public BrickStyle GetGroupStyle(BrickGraphics graph, LayoutGroup groupItem) {
			var key = new object[] { GroupStyleCacheKey, groupItem };
			if(!StyleCache.ContainsKey(key)) {
				var style = CreateStyle<CardViewExportGroupStyle>(graph, RenderStyles.Group);
				if(groupItem.GroupBoxDecoration == GroupBoxDecoration.HeadingLine)
					style.Sides = BorderSide.Top;
				else if(groupItem.GroupBoxDecoration == GroupBoxDecoration.None)
					style.Sides = BorderSide.None;
				StyleCache[key] = style;
			}
			return StyleCache[key];
		}
		public BrickStyle GetGroupCaptionStyle(BrickGraphics graph, LayoutItemBase layoutItem) {
			var key = new object[] { GroupCaptionStyleCacheKey, layoutItem };
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreateStyle(graph, RenderStyles.GroupCaption);
			return StyleCache[key];
		}
		public BrickStyle GetTabbedGroupStyle(BrickGraphics graph, LayoutItemBase layoutItem) {
			var key = new object[] { TabbedGroupStyleCacheKey, layoutItem };
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreateStyle<CardViewExportGroupStyle>(graph, RenderStyles.TabbedGroup);
			return StyleCache[key];
		}
		public BrickStyle GetTabStyle(BrickGraphics graph, LayoutItemBase layoutItem) {
			var key = new object[] { TabStyleCacheKey, layoutItem };
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreateStyle<CardViewExportTabStyle>(graph, RenderStyles.Tab);
			return StyleCache[key];
		}
		public BrickStyle GetDisabledTabStyle(BrickGraphics graph, LayoutItemBase layoutItem) {
			var key = new object[] { DisabledTabStyleCacheKey, layoutItem };
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreateStyle<CardViewExportDisabledTabStyle>(graph, RenderStyles.DisabledTab);
			return StyleCache[key];
		}
		public BrickStyle GetCaptionStyle(BrickGraphics graph, LayoutItemBase layoutItem) {
			var key = new object[] { CaptionStyleCacheKey, layoutItem };
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreateStyle(graph, RenderStyles.Caption);
			return StyleCache[key];
		}
		public BrickStyle GetCheckStyle(BrickGraphics graph, LayoutItemBase layoutItem) {
			var key = new object[] { CheckStyleCacheKey, layoutItem };
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreateStyle(graph, RenderStyles.Check);
			return StyleCache[key];
		}
		public BrickStyle GetTotalSummaryItemStyle(BrickGraphics graph, ASPxSummaryItemBase summaryItem) {
			var key = new object[] { TotalSummaryStyleCacheKey, summaryItem };
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreateStyle(graph, RenderStyles.TotalSummaryItemStyle);
			return StyleCache[key];
		}
		public BrickStyle GetTotalSummaryPanelStyle(BrickGraphics graph) {
			return ConvertStyle(graph, RenderStyles.TotalSummaryPanelStyle);
		}
		BrickStyle CreateStyle<T>(BrickGraphics graph, T renderStyle) where T : GridExportAppearanceBase {
			var style = (T)Activator.CreateInstance(typeof(T));
			style.CopyFrom(renderStyle);
			return ConvertStyle(graph, style);
		}
		protected override BrickStyle CreateCellBrickStyle(BrickGraphics graph, IWebGridDataColumn webColumn, int rowIndex, bool alternating, HorizontalAlign align, bool isLink, bool isImage, bool useCondFormating) {
			var style = new CardViewExportAppearance();
			style.CopyFrom(RenderStyles.Cell);
			var column = (CardViewColumn)webColumn;
			style.CopyFrom(column.ExportCellStyle);
			if(useCondFormating)
				style.CopyFrom(GetFormatConditionsCellStyle(webColumn, rowIndex));
			if(alternating)
				style.CopyFrom(RenderStyles.AlternatingRowCell);
			if(isLink)
				style.CopyFrom(RenderStyles.HyperLink);
			if(isImage)
				style.CopyFrom(RenderStyles.Image);
			if(style.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = align;
			return ConvertStyle(graph, style);
		}
		public override BrickStyle GetCellStyle(BrickGraphics graph, IWebGridDataColumn column, int rowIndex, bool alternating, HorizontalAlign align, bool isLink, bool isImage = false) {
			return column != null ? base.GetCellStyle(graph, column, rowIndex, alternating, align, isLink, isImage) : BrickStyle.CreateDefault();
		}
	}
}
