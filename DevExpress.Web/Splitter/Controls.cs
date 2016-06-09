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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class SplitterContentControlCollection : ContentControlCollection {
		public SplitterContentControlCollection(Control owner) : base(owner) { }
		public new SplitterContentControl this[int i] { get { return (SplitterContentControl)base[i]; } }
		protected override Type GetChildType() { return typeof(SplitterContentControl); }
	}
	public class SplitterContentControl : ContentControl {
		public SplitterContentControl() : base() { }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
	}
}
namespace DevExpress.Web.Internal {
	public enum SplitterButtons {
		Separator = 0,
		Backward = 1,
		Forward = 2
	}
	public class SplitterControl : SplitterPanesTable {
		public SplitterControl(SplitterPane pane) : base(pane) { }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(Splitter, this);
			if(!DesignMode) {
				Width = Unit.Empty;
				Height = Unit.Empty;
			}
			RenderUtils.SetVisibility(this, Splitter.IsClientVisible(), true);
			Splitter.GetControlStyle().AssignToControl(this);
		}
	}
	public class SplitterPanesTable : InternalTable {
		SplitterPane pane;
		TableRow currentRow;
		public SplitterPanesTable(SplitterPane pane) {
			this.pane = pane;
			CellPadding = 0;
			if(!RenderUtils.IsHtml5Mode(this)) {
				CellSpacing = -1;
				RenderUtils.SetStringAttribute(this, "cellspacing", "0");
			}
			else
				RenderUtils.SetStyleStringAttribute(this, "border-collapse", "separate");
		}
		protected bool IsVertical { get { return Pane.Panes.Orientation == Orientation.Vertical; } }
		protected ASPxSplitter Splitter { get { return Pane.Splitter; } }
		protected SplitterRenderHelper RenderHelper { get { return Splitter.RenderHelper; } }
		protected SplitterPane Pane { get { return pane; } }
		protected TableRow CurrentRow { get { return currentRow; } }
		protected TableRow GetCreateRow() {
			if(!IsVertical && (CurrentRow != null))
				return CurrentRow;
			this.currentRow = RenderUtils.CreateTableRow();
			Rows.Add(CurrentRow);
			return CurrentRow;
		}
		protected override void CreateControlHierarchy() {
			foreach(SplitterPane child in Pane.Panes.GetVisibleItems()) {
				if(RenderHelper.IsVisibleSeparator(child))
					GetCreateRow().Cells.Add(new SplitterSeparatorCell(child));
				GetCreateRow().Cells.Add(new SplitterPaneCell(child));
			}
		}
		protected override void PrepareControlHierarchy() {
			ID = RenderHelper.GetPanesTableID(Pane);
			CssClass = RenderHelper.GetPanesTableClassName();
		}
	}
	public class SplitterPaneCell : InternalTableCell {
		SplitterPane pane;
		SplitterPanesTable panesTable;
		WebControl contentContainer;
		public SplitterPaneCell(SplitterPane pane) {
			this.pane = pane;
			RenderUtils.SetStringAttribute(this, "valign", "top");
		}
		protected ASPxSplitter Splitter { get { return Pane.Splitter; } }
		protected SplitterRenderHelper RenderHelper { get { return Splitter.RenderHelper; } }
		protected SplitterPane Pane { get { return pane; } }
		protected SplitterPanesTable PanesTable { get { return panesTable; } set { panesTable = value; } }
		protected WebControl ContentContainer { get { return contentContainer; } set { contentContainer = value; } }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			PanesTable = null;
			ContentContainer = null;
		}
		protected override void CreateControlHierarchy() {
			if(Pane.HasVisibleChildren) {
				ContentContainer = RenderUtils.CreateDiv();
				PanesTable = new SplitterPanesTable(Pane);
				ContentContainer.Controls.Add(PanesTable);
			}
			else {
				if(DesignMode)
					Controls.Add(RenderUtils.CreateEmptySpaceControl(Unit.Pixel(50), Unit.Pixel(0)));
				if(Pane.HasContentUrl)
					ContentContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				else
					ContentContainer = Pane.ContentControlInternal;
			}
			Controls.Add(ContentContainer);
		}
		protected override void PrepareControlHierarchy() {
			ID = RenderHelper.GetPaneID(Pane);
			ContentContainer.ID = RenderHelper.GetContentContainerID(Pane);
			if(DesignMode && !Pane.IsVertical)
				Width = Pane.DesignModeSize;
			SplitterPaneStyle paneStyle = RenderHelper.GetPaneStyle(Pane);
			paneStyle.AssignToControl(this, AttributesRange.All & ~AttributesRange.Cell);
			if(paneStyle.HorizontalAlign != HorizontalAlign.NotSet)
			Attributes.Add("align", paneStyle.HorizontalAlign.ToString().ToLowerInvariant());
			if(!Pane.HasContentUrl) {
				RenderUtils.SetWrap(ContentContainer, paneStyle.Wrap);
				if(DesignMode)
					ContentContainer.Style[HtmlTextWriterStyle.Padding] = "0px";
				else
					RenderUtils.SetPaddings(ContentContainer, paneStyle.Paddings);
			}
			if(!Pane.HasVisibleChildren) {
				if(!Pane.HasContentUrl)
					RenderHelper.ApplyDivScrollBarsAttribute(Pane, ContentContainer);
				ContentContainer.Enabled = RenderHelper.IsEnabled(Pane);
				Unit contentContainerSize = DesignMode ? Unit.Percentage(100) : 1;
				ContentContainer.Width = contentContainerSize;
				ContentContainer.Height = contentContainerSize;
			}
			RenderUtils.AppendDefaultDXClassName(ContentContainer, RenderHelper.GetContentContentContainerClassName(Pane));
		}
	}
	public class SplitterSeparatorCell : InternalTableCell {
		SplitterPane pane;
		WebControl spaceControl;
		public SplitterSeparatorCell(SplitterPane pane) {
			this.pane = pane;
			this.spaceControl = null;
		}
		protected SplitterRenderHelper RenderHelper { get { return Pane.Splitter.RenderHelper; } }
		protected SplitterPane Pane { get { return pane; } }
		protected SplitterPane ParentPane { get { return (Pane != null) ? Pane.Parent : null; } }
		protected WebControl SpaceControl { get { return spaceControl; } }
		protected override void CreateControlHierarchy() {
			bool isAddSpaceControl = !DesignMode || Pane.Separator.IsVertical;
			if(isAddSpaceControl) {
				this.spaceControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				Controls.Add(SpaceControl);
			}
			if(RenderHelper.IsButtonsVisible(Pane)) {
				SplitterSeparatorButtons separatorButtons = new SplitterSeparatorButtons(Pane);
				if(isAddSpaceControl)
					SpaceControl.Controls.Add(separatorButtons);
				else
					Controls.Add(separatorButtons);
			}
		}
		protected override void PrepareControlHierarchy() {
			ID = RenderHelper.GetSeparatorID(Pane);
			RenderUtils.AppendMSTouchDraggableClassNameIfRequired(this);
			SplitterSeparatorStyle separatorStyle = RenderHelper.GetSeparatorStyle(Pane);
			separatorStyle.AssignToControl(this);
			RenderUtils.SetPaddings(this, separatorStyle.Paddings);
			HorizontalAlign = HorizontalAlign.Center;
			Unit separatorSize = RenderHelper.GetSeparatorSize(Pane);
			if(Pane.Separator.IsVertical)
				Width = separatorSize;
			else
				Height = separatorSize;
			if(SpaceControl != null) {
				SpaceControl.CssClass = RenderHelper.GetSeparatorSpacerClassName();
				if(Pane.Separator.IsVertical)
					SpaceControl.Width = Width;
			}
		}
	}
	public class SplitterSeparatorButtons: ASPxInternalWebControl {
		SplitterPane pane;
		TableRow currentRow;
		TableCell collapseForwardCell;
		Image collapseForwardImage;
		TableCell collapseBackwardCell;
		Image collapseBackwardImage;
		TableCell separatorCell;
		Image separatorImage;
		public SplitterSeparatorButtons(SplitterPane pane) {
			this.pane = pane;
		}
		protected SplitterRenderHelper RenderHelper { get { return Pane.Splitter.RenderHelper; } }
		protected SplitterPane Pane { get { return pane; } }
		protected SplitterPane ParentPane { get { return (Pane != null) ? Pane.Parent : null; } }
		protected bool IsVertical { get { return !Pane.IsVertical; } }
		protected TableRow CurrentRow { get { return currentRow; } }
		protected TableCell CollapseForwardCell { get { return collapseForwardCell; } }
		protected Image CollapseForwardImage { get { return collapseForwardImage; } }
		protected TableCell CollapseBackwardCell { get { return collapseBackwardCell; } }
		protected Image CollapseBackwardImage { get { return collapseBackwardImage; } }
		protected TableCell SeparatorCell { get { return separatorCell; } }
		protected Image SeparatorImage { get { return separatorImage; } }
		protected bool ShowSeparatorImage {
			get { return RenderHelper.IsButtonVisible(SplitterButtons.Separator, Pane); }
		}
		protected bool ShowCollapseBackwardButton {
			get { return RenderHelper.IsButtonVisible(SplitterButtons.Backward, Pane); }
		}
		protected bool ShowCollapseForwardButton {
			get { return RenderHelper.IsButtonVisible(SplitterButtons.Forward, Pane); }
		}
		protected TableRow CreateRow(Table parent) {
			if(!IsVertical && (CurrentRow != null))
				return CurrentRow;
			this.currentRow = RenderUtils.CreateTableRow();
			parent.Rows.Add(CurrentRow);
			return CurrentRow;
		}
		protected TableCell CreateCell(Table parentTable) {
			TableCell cell = RenderUtils.CreateTableCell();
			CreateRow(parentTable).Cells.Add(cell);
			return cell;
		}
		protected override void ClearControlFields() {
			this.collapseForwardCell = null;
			this.collapseForwardImage = null;
			this.collapseBackwardCell = null;
			this.collapseBackwardImage = null;
			this.separatorCell = null;
			this.separatorImage = null;
		}
		protected override void CreateControlHierarchy() {
			if(!ShowCollapseForwardButton && !ShowCollapseBackwardButton) {
				this.separatorImage = RenderUtils.CreateImage();
				Controls.Add(SeparatorImage);
			}
			else {
				Table table = RenderUtils.CreateTable();
				Controls.Add(table);
				if(ShowCollapseBackwardButton || !DesignMode) {
					this.collapseBackwardCell = CreateCell(table);
					this.collapseBackwardImage = RenderUtils.CreateImage();
					CollapseBackwardCell.Controls.Add(CollapseBackwardImage);
				}
				if(ShowSeparatorImage && (ShowCollapseBackwardButton && ShowCollapseForwardButton) || (!ShowCollapseBackwardButton && !ShowCollapseForwardButton)) {
					this.separatorCell = CreateCell(table);
					this.separatorImage = RenderUtils.CreateImage();
					SeparatorCell.Controls.Add(SeparatorImage);
				}
				if(ShowCollapseForwardButton || !DesignMode) {
					this.collapseForwardCell = CreateCell(table);
					this.collapseForwardImage = RenderUtils.CreateImage();
					CollapseForwardCell.Controls.Add(CollapseForwardImage);
				}
			}
		}
		protected void PrepareButtonCell(TableCell cell, SplitterButtons button) {
			cell.ID = RenderHelper.GetButtonID(button, Pane);
			SplitterSeparatorButtonStyle style = RenderHelper.GetSeparatorButtonStyle(Pane);
			style.AssignToControl(cell);
			style.Paddings.AssignToControl(cell);
		}
		protected void PrepareButtonImage(Image image, SplitterButtons button) {
			image.ID = RenderHelper.GetButtonImageID(button, Pane);
			RenderHelper.GetButtonImage(button, Pane).AssignToControl(image, DesignMode);
		}
		protected override void PrepareControlHierarchy() {
			if(CollapseForwardCell != null)
				PrepareButtonCell(CollapseForwardCell, SplitterButtons.Forward);
			if(CollapseForwardImage != null)
				PrepareButtonImage(CollapseForwardImage, SplitterButtons.Forward);
			if(CollapseBackwardCell != null)
				PrepareButtonCell(CollapseBackwardCell, SplitterButtons.Backward);
			if(CollapseBackwardImage != null)
				PrepareButtonImage(CollapseBackwardImage, SplitterButtons.Backward);
			if(SeparatorCell != null)
				SeparatorCell.ID = RenderHelper.GetButtonID(SplitterButtons.Separator, Pane);
			if(SeparatorImage != null)
				PrepareButtonImage(SeparatorImage, SplitterButtons.Separator);
		}
	}
	public class ResizingPointerControl : ASPxInternalWebControl {
		ASPxSplitter splitter;
		public ResizingPointerControl(ASPxSplitter splitter) {
			this.splitter = splitter;
		}
		protected internal ASPxSplitter Splitter { get { return splitter; } }
		protected internal SplitterRenderHelper RenderHelper { get { return Splitter.RenderHelper; } }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override void PrepareControlHierarchy() {
			ID = RenderHelper.GetResizingPointerID();
			RenderHelper.GetResizingPointerStyle().AssignToControl(this);
			if(!DesignMode) {
				this.Style.Add("z-index", RenderUtils.LoadingDivZIndex.ToString());
				this.Style.Add("display", "none");
				this.Style.Add("position", "absolute");
			}
			else
				this.Visible = false;
		}
	}
}
