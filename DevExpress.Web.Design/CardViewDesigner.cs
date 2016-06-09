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
using System.ComponentModel.Design;
using System.Linq;
using System.Web.UI.Design;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class CardViewDesignerEditForm : LayoutViewDesignerEditorForm {
		public CardViewDesignerEditForm(ASPxCardView cardView)
			: base(new CardViewCommonFormDesigner(cardView, cardView.Site)) {
		}
	}
	public class CardViewCommonFormDesigner : CommonFormDesigner {
		CardViewColumnsOwner columnsOwner;
		public CardViewCommonFormDesigner(ASPxCardView cardView, IServiceProvider provider)
			: base(cardView, provider) {
			ItemsImageIndex = ColumnsItemImageIndex;
		}
		public CardViewCommonFormDesigner(ASPxCardView cardView)
			: this(cardView, ((IComponent)cardView).Site) {}
		public override ItemsEditorOwner ItemsOwner {
			get {
				columnsOwner = columnsOwner ?? new CardViewColumnsOwner(CardView);
				return columnsOwner;
			}
		}
		ASPxCardView CardView { get { return (ASPxCardView)Control; } }
		DesignerGroup SummaryGroup { get { return Groups[SummaryGroupCaption]; } }
		protected override void CreateMainGroupItems() {
			CreateItemsItem();
			AddFormLayoutItem(CardView.CardLayoutProperties, "CardLayoutProperties", "Card Layout Items");
			AddFormLayoutItem(CardView.EditFormLayoutProperties, "EditFormLayoutProperties", GridViewCommonFormDesigner.EditFormLayoutItems_NavBarItemCaption);
			CreateSummaryGroupItems();
			CreateClientSideEventsItem();
		}
		protected override void CreateItemsItem() {
			MainGroup.Add(CreateDesignerItem("General", "General", typeof(CardViewGeneralFrame), CardView, ItemsItemImageIndex, null));
			base.CreateItemsItem();
		}
		void AddFormLayoutItem(CardViewFormLayoutProperties formLayoutProperties, string propertyName, string caption) {
			var owner = new CardViewFormLayoutItemsOwner(Control, formLayoutProperties, ItemsOwner);
			MainGroup.Add(CreateDesignerItem(propertyName, caption, typeof(FormLayoutItemsEditorFrame), Control, ItemsImageIndex, owner));
		}
		void CreateSummaryGroupItems() {
			Groups.Add(SummaryGroupCaption, SummaryGroupCaption, GetDefaultGroupImage(SummaryGroupImageIndex), false);
			SummaryGroup.Add(CreateDesignerItem(new CardViewSummaryItemsOwner(CardView, Provider, "Total Summary", CardView.TotalSummary), typeof(ItemsEditorFrame), TotalSummaryItemImageIndex));
		}
	}
	public class CardViewDesignerHelper : GridDesignerHelperBase {
		public CardViewDesignerHelper(CardViewDesigner designer)
			: base(designer) {
		}
		protected ASPxCardView CardView { get { return (ASPxCardView)GridBase; } }
		protected override string GridPlaceholderName { get { return "CardView"; } }
		protected override PropertiesBase GridTemplates { get { return CardView.Templates; } }
		protected override List<string> GetControlTemplateNames() {
			var templates = base.GetControlTemplateNames();
			templates.Add("Card");
			templates.Add("CardHeader");
			templates.Add("CardFooter");
			templates.Add("CustomizationPanel");
			templates.Add("EditItem");
			return templates;
		}
		protected override IWebGridDataColumn CreateDataColumnCore(Type dataType) {
			return CardViewEditColumn.CreateColumn(dataType);
		}
		protected override void ProcessIdentityColumn(IWebGridDataColumn column) {
			((CardViewEditColumn)column).Visible = false;
		}
		protected internal void EnsureLayoutCommandItem() {
			if(CardView.CardLayoutProperties.Items.IsEmpty) {
				var prop = CardView.GenerateDefaultLayout(true);
				CardView.CardLayoutProperties.Assign(prop);
			} else {
				var item = new CardViewCommandLayoutItem() { HorizontalAlign = CardView.CommandLayoutItemDefaultHorizontalAlign };
				CardView.CardLayoutProperties.Items.Insert(0, item);
			}
		}
		protected override void RegenerateColumns(ASPxGridBase grid, IDataSourceViewSchema schema) {
			base.RegenerateColumns(grid, schema);
			var cardView = grid as ASPxCardView;
			cardView.CardLayoutProperties.Items.Clear();
		}
	}
	public class CardViewDesignerActionList : GridViewDesignerActionListBase {
		public CardViewDesignerActionList(CardViewDesigner designer)
			: base(designer) {
			CardView = designer.CardView;
		}
		protected ASPxCardView CardView { get; private set; }
		protected CardViewCommandLayoutItem FirstCommandItem { get { return CardView.CardLayoutProperties.Items.OfType<CardViewCommandLayoutItem>().FirstOrDefault(); } }
		protected override ASPxGridBase Grid { get { return CardView; } }
		protected new CardViewDesigner Designer { get { return base.Designer as CardViewDesigner; } }
		public override System.ComponentModel.Design.DesignerActionItemCollection GetSortedActionItems() {
			var result = base.GetSortedActionItems();			
			result.Add(new DesignerActionPropertyItem("ShowHeaderPanel",
				StringResources.CardViewActionList_ShowHeaderPanel,
				StringResources.GridViewActionList_ChecksCategory,
				StringResources.CardViewActionList_ShowHeaderPanelDescription));
			result.Add(new DesignerActionPropertyItem("ShowSummaryPanel",
				StringResources.CardViewActionList_ShowSummaryPanel,
				StringResources.GridViewActionList_ChecksCategory,
				StringResources.CardViewActionList_ShowSummaryPanelDescription));
			return result;
		}
		public override bool ShowSelectCheckBox {
			get {
				var commandItem = FirstCommandItem;
				return commandItem != null && commandItem.ShowSelectCheckbox;
			}
			set {
				var commandItem = GetOrCreateCommandItem();
				commandItem.ShowSelectCheckbox = value;
				Designer.FireControlPropertyChanged("Columns");
			}
		}
		public bool ShowHeaderPanel {
			get { return CardView.Settings.ShowHeaderPanel; }
			set {
				CardView.Settings.ShowHeaderPanel = value;
				Designer.FireControlPropertyChanged("Settings");
			}
		}
		public bool ShowSummaryPanel {
			get { return CardView.Settings.ShowSummaryPanel; }
			set {
				CardView.Settings.ShowSummaryPanel = value;
				Designer.FireControlPropertyChanged("Settings");
			}
		}
		public override bool ShowDeleteButton {
			get {
				var commandItem = FirstCommandItem;
				return commandItem != null && commandItem.ShowDeleteButton;
			}
			set {
				GetOrCreateCommandItem().ShowDeleteButton = value;
				Designer.FireControlPropertyChanged("Columns");
			}
		}
		public override bool ShowEditButton {
			get {
				var commandItem = FirstCommandItem;
				return commandItem != null && commandItem.ShowEditButton;
			}
			set {
				GetOrCreateCommandItem().ShowEditButton = value;
				Designer.FireControlPropertyChanged("Columns");
			}
		}
		public override bool ShowNewButton {
			get {
				var commandItem = FirstCommandItem;
				return commandItem != null && commandItem.ShowNewButton;
			}
			set {
				GetOrCreateCommandItem().ShowNewButton = value;
				Designer.FireControlPropertyChanged("Columns");
			}
		}
		protected CardViewCommandLayoutItem GetOrCreateCommandItem() {
			var result = FirstCommandItem;
			result = result ?? CreateCommandItem();
			return result;
		}
		protected CardViewCommandLayoutItem CreateCommandItem() {
			Designer.Helper.EnsureLayoutCommandItem();
			return FirstCommandItem;
		}
	}
	public class CardViewDesigner : GridDesignerBase {
		CardViewDesignerHelper helper;
		public ASPxCardView CardView { get { return (ASPxCardView)Grid; } }
		public CardViewDesignerHelper Helper {
			get {
				helper = helper ?? new CardViewDesignerHelper(this);
				return helper;
			}
		}
		public override GridDesignerHelperBase BaseHelper { get { return Helper; } }
		protected override TemplateGroupCollection TemplateGroups { get { return Helper.TemplateGroups; } }
		public override ASPxWebControlDesignerActionList ActionList { get { return new CardViewDesignerActionList(this); } }
		public override void RunDesigner() {
			CommonDesignerServiceRegisterHelper.AddWebControlDesigner(CardView.Site, CardView.ID, this);
			ShowDialog(new CardViewDesignerEditForm(CardView));
			CommonDesignerServiceRegisterHelper.RemoveWebControlDesigner(CardView.Site, CardView.ID);
		}
	}
	public class CardViewFormatConditionCommonEditor: TypeEditorBase {
		public override Form CreateEditorForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue) {
			ASPxCardView grid = (ASPxCardView)component;
			var designer = new FormatConditionCommonFormDesigner(grid, provider);
			return new WrapperEditorForm(designer, false);
		}
	}
	public class FormatConditionCommonFormDesigner: CommonFormDesigner {
		public FormatConditionCommonFormDesigner(ASPxCardView component, IServiceProvider provider)
			: base(component, provider) {
		}
		public new ASPxCardView Control { get { return (ASPxCardView)base.Control; } }
		protected override void CreateMainGroupItems() {
			MainGroup.Add(CreateDesignerItem(new CardViewFormatConditionItemsOwner(Control, Provider, Control.FormatConditions), typeof(ItemsEditorFrame), ButtonsItemImageIndex));
		}
	}
	public class CardViewFormatConditionItemsOwner: ItemsEditorOwner {
		public CardViewFormatConditionItemsOwner(ASPxCardView gridView, IServiceProvider provider, CardViewFormatConditionCollection formatConditions)
			: base(gridView, "Format Conditions", provider, formatConditions) {
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(CardViewFormatConditionHighlight), "Highlight condition");
			AddItemType(typeof(CardViewFormatConditionTopBottom), "Top/Bottom condition");
			AddItemType(typeof(CardViewFormatConditionColorScale), "Color scale condition");
			AddItemType(typeof(CardViewFormatConditionIconSet), "Icon set condition");
		}
		public override Type GetDefaultItemType() {
			return typeof(CardViewFormatConditionHighlight);
		}
	}
}
