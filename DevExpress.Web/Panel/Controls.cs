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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class CollapsiblePanelControl : ASPxInternalWebControl {
		ASPxCollapsiblePanel panel;
		private WebControl mainDiv = null;
		private WebControl scrollableContainer = null;
		private WebControl animationContainer = null;
		private Table mainTable = null;
		private TableCell mainTableCell = null;
		private WebControl expandBar = null;
		private WebControl expandBarTemplateContainer = null;
		private WebControl expandedTemplateContainer = null;
		private WebControl expandButton = null;
		private Image expandButtonImageControl = null;
		public CollapsiblePanelControl(ASPxCollapsiblePanel panel)
			: base() {
			this.panel = panel;
		}
		public ASPxCollapsiblePanel Panel {
			get { return panel; }
		}
		protected WebControl MainControl {
			get { return (MainDiv != null) ? MainDiv : MainTable; }
		}
		protected WebControl MainDiv {
			get { return mainDiv; }
		}
		protected WebControl ScrollableContainer {
			get { return scrollableContainer; }
		}
		protected WebControl AnimationContainer {
			get { return animationContainer; }
		}
		protected WebControl ScrollableDiv {
			get {
				if(ScrollableContainer != null)
					return ScrollableContainer;
				if(AnimationContainer != null)
					return AnimationContainer;
				return MainDiv;
			}
		}
		protected Table MainTable {
			get { return mainTable; }
		}
		protected TableCell MainTableCell {
			get { return mainTableCell; }
		}
		protected WebControl ExpandBar {
			get { return expandBar; }
		}
		protected WebControl ExpandBarTemplateContainer {
			get { return expandBarTemplateContainer; }
		}
		protected WebControl ExpandedTemplateContainer {
			get { return expandedTemplateContainer; }
		}
		protected WebControl ExpandButton {
			get { return expandButton; }
		}
		protected Image ExpandButtonImageControl {
			get { return expandButtonImageControl; }
		}
		protected override void ClearControlFields() {
			this.mainDiv = null;
			this.scrollableContainer = null;
			this.animationContainer = null;
			this.mainTable = null;
			this.mainTableCell = null;
			this.expandBar = null;
			this.expandButton = null;
			this.expandButtonImageControl = null;
			this.expandBarTemplateContainer = null;
			this.expandedTemplateContainer = null;
		}
		protected override void CreateControlHierarchy() {
			if(!Panel.Collapsible)
				CreatePanelHierarchy(this);
			else if(DesignMode) 
				CreateDesignModeHierarchy();
			else {
				CreateExpandBar(this);
				CreatePanelHierarchy(this);
			}
		}
		protected void CreatePanelHierarchy(Control parent) {
			if(Panel.RenderMode != RenderMode.Table)
				CreateDivHierarchy(parent);
			else
				CreateTableHierarchy(parent);
		}
		protected void CreateDesignModeHierarchy() {
			Table table = RenderUtils.CreateTable();
			Controls.Add(table);
			if(Panel.GetExpandEffect() == PanelExpandEffect.PopupToBottom || panel.GetExpandEffect() == PanelExpandEffect.Slide) {
				TableRow barRow = RenderUtils.CreateTableRow();
				table.Rows.Add(barRow);
				TableCell barCell = RenderUtils.CreateTableCell();
				barRow.Cells.Add(barCell);
				CreateExpandBar(barCell);
			}
			TableRow row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			if(Panel.GetExpandEffect() == PanelExpandEffect.PopupToRight) {
				TableCell barCell = RenderUtils.CreateTableCell();
				row.Cells.Add(barCell);
				CreateExpandBar(barCell);
			}
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			CreatePanelHierarchy(cell);
			if(Panel.GetExpandEffect() == PanelExpandEffect.PopupToLeft) {
				TableCell barCell = RenderUtils.CreateTableCell();
				row.Cells.Add(barCell);
				CreateExpandBar(barCell);
			}
			if(Panel.GetExpandEffect() == PanelExpandEffect.PopupToTop) {
				TableRow barRow = RenderUtils.CreateTableRow();
				table.Rows.Add(barRow);
				TableCell barCell = RenderUtils.CreateTableCell();
				barRow.Cells.Add(barCell);
				CreateExpandBar(barCell);
			}
		}
		protected void CreateDivHierarchy(Control parent) {
			this.mainDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(MainDiv);
			CreateContent(MainDiv);
		}
		protected void CreateTableHierarchy(Control parent) {
			this.mainTable = RenderUtils.CreateTable();
			parent.Controls.Add(MainTable);
			TableRow row = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(row);
			this.mainTableCell = RenderUtils.CreateTableCell();
			MainTableCell.VerticalAlign = VerticalAlign.Top;
			row.Cells.Add(MainTableCell);
			CreateContent(MainTableCell);
		}
		protected void CreateContent(Control parent) {
			if(Panel.PanelContent == null)
				return;
			if(Panel.HasScrollableContainer() && !Panel.HasAnimationContainer()) {
				this.scrollableContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				RenderUtils.AppendDefaultDXClassName(ScrollableContainer, PanelStyles.ScrollableContentContainerCssMarker);
				parent.Controls.Add(ScrollableContainer);
				parent = ScrollableContainer;
			}
			if(Panel.Collapsible) {
				if(Panel.HasAnimationContainer()) {
					this.animationContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
					RenderUtils.AppendDefaultDXClassName(AnimationContainer, PanelStyles.AnimationContentContainerCssMarker);
					if(Panel.HasScrollableContainer())
						RenderUtils.AppendDefaultDXClassName(AnimationContainer, PanelStyles.ScrollableContentContainerCssMarker);
					parent.Controls.Add(AnimationContainer);
					parent = AnimationContainer;
				}
				if(Panel.ExpandedPanelTemplate != null) {
					WebControl container = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
					RenderUtils.AppendDefaultDXClassName(container, PanelStyles.ContentContainerCssMarker);
					parent.Controls.Add(container);
					CreateExpandedTemplate(parent);
					parent = container;
				}
			}
			parent.Controls.Add(Panel.PanelContent);
		}
		protected void CreateExpandBar(Control parent) {
			if(DesignMode) {
				this.expandBar = parent as WebControl;
				RenderUtils.SetStyleStringAttribute(ExpandBar, "display", "table");
			} 
			else {
				this.expandBar = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				parent.Controls.Add(ExpandBar);
			}
			if(Panel.ExpandBarTemplate != null && Panel.GetExpandButtonPosition() != PanelExpandButtonPosition.Far)
				CreateExpandBarTemplate(ExpandBar);
			if(Panel.IsExpandButtonVisible()) {
				this.expandButton = RenderUtils.CreateWebControl(HtmlTextWriterTag.A);
				ExpandBar.Controls.Add(ExpandButton);
				if(Panel.ExpandButtonTemplate != null)
					CreateExpandButtonTemplate(ExpandButton);
				else {
					this.expandButtonImageControl = RenderUtils.CreateImage();
					ExpandButton.Controls.Add(ExpandButtonImageControl);
				}
			}
			if(Panel.ExpandBarTemplate != null && Panel.GetExpandButtonPosition() == PanelExpandButtonPosition.Far)
				CreateExpandBarTemplate(ExpandBar);
		}
		protected void CreateExpandBarTemplate(Control parent) {
			this.expandBarTemplateContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(ExpandBarTemplateContainer);
			TemplateContainerBase container = new TemplateContainerBase(0, this);
			container.AddToHierarchy(ExpandBarTemplateContainer, ASPxCollapsiblePanel.ExpandBarTemplateID);
			Panel.ExpandBarTemplate.InstantiateIn(container);
		}
		protected void CreateExpandButtonTemplate(Control parent) {
			TemplateContainerBase container = new TemplateContainerBase(0, this);
			container.AddToHierarchy(parent, string.Empty);
			Panel.ExpandButtonTemplate.InstantiateIn(container);
		}
		protected void CreateExpandedTemplate(Control parent) {
			this.expandedTemplateContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(ExpandedTemplateContainer);
			TemplateContainerBase container = new TemplateContainerBase(0, this);
			container.AddToHierarchy(ExpandedTemplateContainer, string.Empty);
			Panel.ExpandedPanelTemplate.InstantiateIn(container);
		}
		protected override void PrepareControlHierarchy() {
			if(ExpandBar != null)
				PrepareExpandBar();
			RenderUtils.AssignAttributes(Panel, MainControl);
			RenderUtils.SetVisibility(MainControl, Panel.IsClientVisible(), true);
			Panel.GetPanelPaddings().AssignToControl(MainControl);
			Panel.GetPanelStyle().AssignToControl(MainControl, AttributesRange.Common | AttributesRange.Font);
			MainControl.Height = Panel.GetPanelHeight();
			MainControl.Width = Panel.GetPanelWidth();
			if(Panel.GetFixedPositionOverlapZIndex(false) > 0)
				MainControl.Style.Add("z-index", Panel.GetFixedPositionOverlapZIndex(false).ToString());
			if(Panel.IsDefaultButtonAssigned() && IsEnabled())
				Panel.AddDefaultButtonScript(MainControl);
			if(Panel.Collapsible && !Panel.HasCollapsingAdaptivity() && !DesignMode) {
				RenderUtils.AppendDefaultDXClassName(MainControl, PanelStyles.CollapsibleCssMarker);
				if(Panel.NeedExpandOnPageLoad())
					RenderUtils.AppendDefaultDXClassName(MainControl, PanelStyles.ExpandedCssMarker);
			}
			if(Panel.IsPositionFixed() && !DesignMode) {
				RenderUtils.AppendDefaultDXClassName(MainControl, PanelStyles.FixedPositionCssMarker);
				RenderUtils.AppendDefaultDXClassName(MainControl, GetFixedPositionCssClass());
			}
			if(ScrollableDiv != null && Panel.ScrollBars != ScrollBars.None) {
				RenderUtils.SetScrollBars(ScrollableDiv, Panel.ScrollBars);
				if(ScrollableDiv != MainDiv) {
					Unit height = Panel.GetPanelHeight();
					Unit width = Panel.GetPanelWidth();
					ScrollableDiv.Height = height.Type == UnitType.Percentage ? Unit.Percentage(100) : height;
					ScrollableDiv.Width = width.Type == UnitType.Percentage ? Unit.Percentage(100) : width;
					Panel.GetPanelPaddings().AssignToControl(ScrollableDiv);
					RenderUtils.ClearPaddings(MainDiv);
				}
			}
			if(ExpandedTemplateContainer != null)
				RenderUtils.AppendDefaultDXClassName(ExpandedTemplateContainer, PanelStyles.ExpandedTemplateContainerCssMarker);
		}
		protected void PrepareExpandBar() {
			RenderUtils.AssignAttributes(Panel, ExpandBar, true);
			RenderUtils.SetVisibility(ExpandBar, Panel.IsClientVisible(), true);
			ExpandBar.ID = ASPxCollapsiblePanel.ExpandBarID;
			RenderUtils.AppendDefaultDXClassName(ExpandBar, PanelStyles.ExpandBarCssMarker);
			if(Panel.GetFixedPositionOverlapZIndex(true) > 0)
				ExpandBar.Style.Add("z-index", Panel.GetFixedPositionOverlapZIndex(true).ToString());
			if(Panel.Collapsible && !Panel.HasCollapsingAdaptivity()) {
				RenderUtils.AppendDefaultDXClassName(ExpandBar, PanelStyles.CollapsibleCssMarker);
			}
			if(Panel.IsPositionFixed()) {
				RenderUtils.AppendDefaultDXClassName(ExpandBar, PanelStyles.FixedPositionCssMarker);
				RenderUtils.AppendDefaultDXClassName(ExpandBar, GetFixedPositionCssClass());
			}
			AppearanceStyleBase expandBarStyle = Panel.GetExpandBarStyle();
			expandBarStyle.AssignToControl(ExpandBar);
			expandBarStyle.Paddings.AssignToControl(ExpandBar);
			ExpandBar.Height = expandBarStyle.Height;
			ExpandBar.Width = expandBarStyle.Width;
			if(ExpandBarTemplateContainer != null) {
				RenderUtils.AppendDefaultDXClassName(ExpandBarTemplateContainer, PanelStyles.ExpandBarTemplateContainerCssMarker);
				RenderUtils.AppendDefaultDXClassName(ExpandBarTemplateContainer, GetExpandBarTemplatePositionCssClass());
			}
			if(ExpandButton != null) {
				ExpandButton.ID = ASPxCollapsiblePanel.ExpandButtonID;
				AppearanceStyleBase expandButtonStyle = Panel.GetExpandButtonStyle();
				expandButtonStyle.AssignToControl(ExpandButton);
				expandButtonStyle.Paddings.AssignToControl(ExpandButton);
				RenderUtils.AppendDefaultDXClassName(ExpandButton, GetExpandButtonPositionCssClass());
				if(ExpandButtonImageControl != null) {
					ExpandButtonImageControl.ID = ASPxCollapsiblePanel.ExpandButtonImageID;
					Panel.GetExpandButtonImage().AssignToControl(ExpandButtonImageControl, DesignMode);
				}
			}
		}		
		protected string GetFixedPositionCssClass() {
			switch(Panel.FixedPosition) {
				case PanelFixedPosition.WindowLeft:
					return PanelStyles.LeftFixedPositionCssMarker;
				case PanelFixedPosition.WindowRight:
					return PanelStyles.RightFixedPositionCssMarker;
				case PanelFixedPosition.WindowTop:
					return PanelStyles.TopFixedPositionCssMarker;
				case PanelFixedPosition.WindowBottom:
					return PanelStyles.BottomFixedPositionCssMarker;
			}
			return string.Empty;
		}
		protected string GetExpandButtonPositionCssClass() {
			switch(Panel.GetExpandButtonPosition()) {
				case PanelExpandButtonPosition.Far:
					return PanelStyles.FarButtonPositionCssMarker;
				case PanelExpandButtonPosition.Near:
					return PanelStyles.NearButtonPositionCssMarker;
				case PanelExpandButtonPosition.Center:
					return (ExpandBarTemplateContainer != null) ? PanelStyles.FarButtonPositionCssMarker : PanelStyles.CenterButtonPositionCssMarker;
			}
			return string.Empty;
		}
		protected string GetExpandBarTemplatePositionCssClass() {
			switch(Panel.GetExpandButtonPosition()) {
				case PanelExpandButtonPosition.Far:
					return PanelStyles.NearButtonPositionCssMarker;
				case PanelExpandButtonPosition.Near:
					return PanelStyles.FarButtonPositionCssMarker;
				case PanelExpandButtonPosition.Center:
					return PanelStyles.NearButtonPositionCssMarker;
			}
			return string.Empty;
		}
	}
}
