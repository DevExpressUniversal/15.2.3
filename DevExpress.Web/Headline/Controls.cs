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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class HeadlineControlBase : ASPxInternalWebControl {
		private ASPxHeadline fHeadline = null;
		public ASPxHeadline Headline {
			get { return fHeadline; }
		}
		public HeadlineControlBase(ASPxHeadline headline)
			: base() {
			fHeadline = headline;
		}
	}
	public class HeadlineControl: HeadlineControlBase {
		private TableCell fLeftPanelCell = null;
		private TableCell fLeftSpacingCell = null;
		private TableCell fMainCell = null;
		private WebControl fMainDiv = null;
		private Table fMainTable = null;
		private TableCell fRightPanelCell = null;
		private TableCell fRightSpacingCell = null;
		public HeadlineControl(ASPxHeadline headline)
			: base(headline) {
		}
		protected override void ClearControlFields() {
			fLeftPanelCell = null;
			fLeftSpacingCell = null;
			fMainCell = null;
			fMainDiv = null;
			fMainTable = null;
			fRightPanelCell = null;
			fRightSpacingCell = null;
		}
		protected override void CreateControlHierarchy() {
			if (Headline.HasMainDiv()) {
				fMainDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				Controls.Add(fMainDiv);
				fMainDiv.Controls.Add(new HeadlineMainPanelControl(Headline));
			}
			else {
				fMainTable = RenderUtils.CreateTable(true);
				Controls.Add(fMainTable);
				TableRow mainRow = RenderUtils.CreateTableRow();
				fMainTable.Rows.Add(mainRow);
				if (Headline.IsLeftPanelNeeded()) {
					fLeftPanelCell = RenderUtils.CreateTableCell();
					mainRow.Cells.Add(fLeftPanelCell);
					fLeftPanelCell.Controls.Add(new HeadlineLeftPanelControl(Headline));
				}
				if (Headline.HasLeftPanelSpacingCell()) {
					fLeftSpacingCell = RenderUtils.CreateIndentCell();
					mainRow.Cells.Add(fLeftSpacingCell);
				}
				fMainCell = RenderUtils.CreateTableCell();
				mainRow.Cells.Add(fMainCell);
				fMainCell.Controls.Add(new HeadlineMainPanelControl(Headline));
				if (Headline.HasRightPanelSpacingCell()) {
					fRightSpacingCell = RenderUtils.CreateIndentCell();
					mainRow.Cells.Add(fRightSpacingCell);
				}
				if (Headline.IsRightPanelNeeded()) {
					fRightPanelCell = RenderUtils.CreateTableCell();
					mainRow.Cells.Add(fRightPanelCell);
					fRightPanelCell.Controls.Add(new HeadlineRightPanelControl(Headline));
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			if (fMainTable != null) {
				RenderUtils.AssignAttributes(Headline, fMainTable);
				Headline.GetControlStyle().AssignToControl(fMainTable, AttributesRange.Common | AttributesRange.Font);
				fMainTable.ToolTip = Headline.GetToolTip();
				fMainTable.Height = Unit.Empty;
			}
			if (fMainCell != null) {
				if (!Headline.Width.IsEmpty)
					fMainCell.Width = Unit.Percentage(100);
				fMainCell.VerticalAlign = VerticalAlign.Top;
				if (Headline.IsSimpleRender()) {
					Headline.GetContentStyle().AssignToControl(fMainCell, AttributesRange.All);
					RenderUtils.SetPaddings(fMainCell, Headline.GetContentPaddings());
					RenderUtils.SetLineHeight(fMainCell, Headline.GetContentLineHeight());
					fMainCell.ToolTip = Headline.GetContentToolTip();
				}
				fMainCell.HorizontalAlign = HorizontalAlign.NotSet;	 
				RenderUtils.SetHorizontalAlign(fMainCell, Headline.HorizontalAlign); 
			}
			if (fMainDiv != null) {
				RenderUtils.AssignAttributes(Headline, fMainDiv);
				Headline.GetControlStyle().AssignToControl(fMainDiv, AttributesRange.Common | AttributesRange.Font);
				fMainDiv.ToolTip = Headline.GetToolTip();
				fMainDiv.Height = Unit.Empty;
				RenderUtils.SetHorizontalAlign(fMainDiv, Headline.HorizontalAlign);
				if (Headline.IsSimpleRender()) {
					Headline.GetContentStyle().AssignToControl(fMainDiv, AttributesRange.All);
					RenderUtils.SetPaddings(fMainDiv, Headline.GetContentPaddings());
					RenderUtils.SetLineHeight(fMainDiv, Headline.GetContentLineHeight());
				}
			}
			if (fLeftPanelCell != null) {
				Headline.GetLeftPanelStyle().AssignToControl(fLeftPanelCell, AttributesRange.All);
				RenderUtils.SetPaddings(fLeftPanelCell, Headline.GetLeftPanelPaddings());
				fLeftPanelCell.VerticalAlign = VerticalAlign.Top;
			}
			if (fRightPanelCell != null) {
				Headline.GetRightPanelStyle().AssignToControl(fRightPanelCell, AttributesRange.All);
				RenderUtils.SetPaddings(fRightPanelCell, Headline.GetRightPanelPaddings());
				fRightPanelCell.VerticalAlign = VerticalAlign.Top;
			}
			if (fLeftSpacingCell != null)
				RenderUtils.PrepareIndentCell(fLeftSpacingCell, Headline.GetLeftPanelSpacing(), Unit.Empty);
			if (fRightSpacingCell != null)
				RenderUtils.PrepareIndentCell(fRightSpacingCell, Headline.GetRightPanelSpacing(), Unit.Empty);
		}
	}
	public class HeadlineMainPanelControl: HeadlineControlBase {
		public HeadlineMainPanelControl(ASPxHeadline headline)
			: base(headline) {
		}
		protected override void CreateControlHierarchy() {
			if (Headline.IsDateInTop()) {
				HeadlineDateControl dateControl = new HeadlineDateControl(Headline);
				Controls.Add(dateControl);
			}
			if (Headline.HasHeader()) {
				HeadlineHeaderControl headerControl = new HeadlineHeaderControl(Headline);
				Controls.Add(headerControl);
			}
			if (Headline.IsDateInBelowHeader()) {
				HeadlineDateControl dateControl = new HeadlineDateControl(Headline);
				Controls.Add(dateControl);
			}
			HeadlineContentControl contentControl = new HeadlineContentControl(Headline);
			Controls.Add(contentControl);
			if (Headline.IsTailRequired() && Headline.IsTailInNewLine()) {
				HeadlineTailControl tailControl = new HeadlineTailControl(Headline);
				Controls.Add(tailControl);
			}
			if (Headline.IsDateInBottom()) {
				HeadlineDateControl dateControl = new HeadlineDateControl(Headline);
				Controls.Add(dateControl);
			}
		}
	}
	public class HeadlineDateControl: HeadlineControlBase {
		private const string DateSpacingString = " "; 
		private LiteralControl fDateSpacing = null;
		private WebControl fDiv = null;
		private WebControl fSpan = null;
		private LiteralControl fText = null;
		public HeadlineDateControl(ASPxHeadline headline)
			: base(headline) {
		}
		protected override void ClearControlFields() {
			fDateSpacing = null;
			fDiv = null;
			fSpan = null;
			fText = null;
		}
		protected override void CreateControlHierarchy() {
			if (Headline.IsDateInHeader()) {
				if (Headline.DateHorizontalPosition == DateHorizontalPosition.Right) {
					fDateSpacing = RenderUtils.CreateLiteralControl();
					Controls.Add(fDateSpacing);
				}
				fSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				Controls.Add(fSpan);
				CreateDateText(fSpan);
				if (Headline.DateHorizontalPosition == DateHorizontalPosition.Left) {
					fDateSpacing = RenderUtils.CreateLiteralControl();
					Controls.Add(fDateSpacing);
				}
			}
			else {
				fDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				Controls.Add(fDiv);
				CreateDateText(fDiv);
			}
		}
		protected override void PrepareControlHierarchy() {
			if (fDateSpacing != null)
				fDateSpacing.Text = DateSpacingString;
			if (fDiv != null) {
				if (Headline.IsDateInLeftPanel())
					Headline.GetDateLeftPanelStyle().AssignToControl(fDiv, AttributesRange.All);
				else if (Headline.IsDateInRightPanel())
					Headline.GetDateRightPanelStyle().AssignToControl(fDiv, AttributesRange.All);
				else
					Headline.GetDateStyle().AssignToControl(fDiv, AttributesRange.All);
				RenderUtils.SetPaddings(fDiv, Headline.GetDatePaddings());
				if(Headline.DateHorizontalPosition == DateHorizontalPosition.Right)
					RenderUtils.SetHorizontalAlign(fDiv, HorizontalAlign.Right);
				if (Headline.HasDateSpacing())
					RenderUtils.SetVerticalMargins(fDiv, Unit.Empty, Headline.GetDateSpacing());
				if (Headline.IsDateInPanelTop() || (Headline.IsDateInPanel() && 
						(!Headline.HasImage() || !Headline.IsDateAndImageInSamePanel()))) {
					if (Headline.HasHeader())
						RenderUtils.SetLineHeight(fDiv, Headline.GetHeaderLineHeight());
					else
						RenderUtils.SetLineHeight(fDiv, Headline.GetContentLineHeight());
				}
			}
			if (fSpan != null) {
				Headline.GetDateHeaderStyle().AssignToControl(fSpan, AttributesRange.All);
				RenderUtils.SetPaddings(fSpan, Headline.GetDatePaddings());
			}
			if (fText != null)
				fText.Text = Headline.GetDate();
		}
		protected void CreateDateText(Control parent) {
			fText = RenderUtils.CreateLiteralControl();
			parent.Controls.Add(fText);
		}
	}
	public class HeadlineTailControl : HeadlineControlBase {
		private const string TailSpacingString = " "; 
		private WebControl fClickableSpan = null;
		private WebControl fSpan = null;
		private Image fImage = null;
		private HyperLink fLink = null;
		private HyperLink fImageLink = null;
		private LiteralControl fText = null;
		private WebControl fDiv = null;
		private LiteralControl fTailSpacing = null;
		public HeadlineTailControl(ASPxHeadline headline)
			: base(headline) {
		}
		protected override void ClearControlFields() {
			fClickableSpan = null;
			fSpan = null;
			fImage = null;
			fLink = null;
			fImageLink = null;
			fText = null;
			fDiv = null;
			fTailSpacing = null;
		}
		protected override void CreateControlHierarchy() {
			if (Headline.TailPosition != TailPosition.NewLine) {
				fTailSpacing = RenderUtils.CreateLiteralControl();
				Controls.Add(fTailSpacing);
				CreateTail(this);
			}
			else {
				fDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				Controls.Add(fDiv);
				CreateTail(fDiv);
			}
		}
		protected override void PrepareControlHierarchy() {
			if(fTailSpacing != null)
				fTailSpacing.Text = TailSpacingString;
			if(fSpan != null) {
				Headline.GetTailStyle().AssignToControl(fSpan, AttributesRange.All);
				RenderUtils.SetLineHeight(fSpan, Headline.GetTailLineHeight());
			}
			if(fText != null)
				fText.Text = Headline.GetTailText();
			if(fLink != null) {
				RenderUtils.PrepareHyperLink(fLink, "", Headline.NavigateUrl, Headline.Target, "", Headline.IsEnabled());
				if(Headline.TailPosition == TailPosition.NewLine)
					RenderUtils.PrepareHyperLinkStyle(fLink, Headline.GetTailDivStyle());
				else {
					RenderUtils.PrepareHyperLinkStyle(fLink, Headline.GetTailStyle());
					RenderUtils.AppendDefaultDXClassName(fLink, Headline.GetCssClassNamePrefix());
				}
			}
			if (fImageLink != null) {
				RenderUtils.PrepareHyperLink(fImageLink, "", Headline.NavigateUrl, Headline.Target, "", Headline.IsEnabled());
			}
			if (fImage != null) {
				Headline.TailImage.AssignToControl(fImage, DesignMode);
				if (Browser.IsIE) {
					if (Headline.TailPosition != TailPosition.NewLine &&
						Headline.GetContentToolTip() != "") {
						fImage.ToolTip = Headline.GetContentToolTip();
					} else if (Headline.TailPosition == TailPosition.NewLine &&
						Headline.GetToolTip() != "") {
						fImage.ToolTip = Headline.GetToolTip();
					}
				}
				if(fText != null) {
					if(Headline.TailImagePosition == TailImagePosition.BeforeTailText)
						RenderUtils.SetHorizontalMargins(fImage, Unit.Empty, Headline.GetTailImageSpacing());
					else
						RenderUtils.SetHorizontalMargins(fImage, Headline.GetTailImageSpacing(), Unit.Empty);
				}
			}
			if(fDiv != null) {
				Headline.GetTailDivStyle().AssignToControl(fDiv, AttributesRange.All);
				RenderUtils.SetPaddings(fDiv, Headline.GetTailPaddings());
				if(Headline.IsDateInBottom())
					RenderUtils.SetVerticalMargins(fDiv, Unit.Empty, Headline.GetTailSpacing());
			}
			if(Headline.GetTailOnClick() != "") {
				if(fLink != null) {
					if(!((Headline.TailPosition == TailPosition.KeepWithLastWord) && Headline.ContentHasLink())) {
					RenderUtils.SetStringAttribute(fLink, "onclick", Headline.GetTailOnClick());
						if(fImageLink != null)
							RenderUtils.SetStringAttribute(fImageLink, "onclick", Headline.GetTailOnClick());
					}
				} else {
					WebControl clickableControl = fClickableSpan;
					if(clickableControl == null)
						clickableControl = fSpan;
					if(clickableControl == null)
						clickableControl = fImage;
					if(clickableControl != null && !(Headline.TailPosition == TailPosition.KeepWithLastWord && Headline.ContentHasLink())) {
						RenderUtils.SetStringAttribute(clickableControl, "onclick", Headline.GetTailOnClick());
						RenderUtils.SetCursor(clickableControl, RenderUtils.GetPointerCursor());
					}
				}
			}
			if(fClickableSpan != null)
				RenderUtils.AppendDefaultDXClassName(fClickableSpan, Headline.GetCssClassNamePrefix());
			if(fSpan != null)
				RenderUtils.AppendDefaultDXClassName(fSpan, Headline.GetCssClassNamePrefix());
		}
		protected void CreateTail(Control parent) {
			CreateTailContent(parent);
		}
		protected void CreateTailContent(Control parent) {
			if((Headline.GetTailOnClick() != "") && (Headline.TailText != "") && (Headline.TailPosition == TailPosition.NewLine) ||
					(Headline.TailHasLink() && !Headline.HasTailImage())) {
				fClickableSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				parent.Controls.Add(fClickableSpan);
				parent = fClickableSpan;
			}
			if (Headline.HasTailImage() && Headline.TailImagePosition == TailImagePosition.BeforeTailText)
				CreateTailImage(parent);
			if (Headline.TailText != "") {
				if (Headline.TailPosition != TailPosition.NewLine && 
						(!Headline.TailHasLink() || Headline.HasTailImage())) {
					fSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
					parent.Controls.Add(fSpan);
					CreateTailText(fSpan);
				}
				else 
					CreateTailText(parent);
			}
			if(Headline.HasTailImage() && Headline.TailImagePosition == TailImagePosition.AfterTailText) {
				if(!Headline.TailImageHasLink() && fSpan != null)
					CreateTailImage(fSpan);
				else
					CreateTailImage(parent);
			}
		}
		protected void CreateTailImage(Control parent) {
			fImage = RenderUtils.CreateImage();
			if (Headline.TailHasLink() && Headline.TailImageHasLink()) {
				fImageLink = RenderUtils.CreateHyperLink();
				parent.Controls.Add(fImageLink);
				fImageLink.Controls.Add(fImage);
			} else {
				parent.Controls.Add(fImage);
			}
		}
		protected void CreateTailText(Control parent) {
			fText = RenderUtils.CreateLiteralControl();
			if (Headline.TailHasLink()) {
				fLink = RenderUtils.CreateHyperLink();
				parent.Controls.Add(fLink);
				fLink.Controls.Add(fText);
			} else {
				parent.Controls.Add(fText);
			}
		}
	}
	public class HeadlineHeaderControl: HeadlineControlBase {
		private WebControl fDiv = null;
		private HyperLink fLink = null;
		private LiteralControl fText = null;
		public HeadlineHeaderControl(ASPxHeadline headline)
			: base(headline) {
		}
		protected override void ClearControlFields() {
			fDiv = null;
			fLink = null;
			fText = null;
		}
		protected override void CreateControlHierarchy() {
			fDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(fDiv);
			if (Headline.IsDateInHeader() && Headline.DateHorizontalPosition == DateHorizontalPosition.Left) {
				HeadlineDateControl dateControl = new HeadlineDateControl(Headline);
				fDiv.Controls.Add(dateControl);
			}
			if (Headline.HeaderHasLink()) {
				fLink = RenderUtils.CreateHyperLink();
				fDiv.Controls.Add(fLink);
			}
			else {
				fText = RenderUtils.CreateLiteralControl();
				fDiv.Controls.Add(fText);
			}
			if (Headline.IsDateInHeader() && Headline.DateHorizontalPosition == DateHorizontalPosition.Right) {
				HeadlineDateControl dateControl = new HeadlineDateControl(Headline);
				fDiv.Controls.Add(dateControl);
			}
		}
		protected override void PrepareControlHierarchy() {
			Headline.GetHeaderStyle().AssignToControl(fDiv, AttributesRange.All);
			RenderUtils.SetPaddings(fDiv, Headline.GetHeaderPaddings());
			RenderUtils.SetVerticalMargins(fDiv, Unit.Empty, Headline.GetHeaderSpacing());
			RenderUtils.SetLineHeight(fDiv, Headline.GetHeaderLineHeight());
			if (fLink != null) {
				RenderUtils.PrepareHyperLink(fLink, Headline.HtmlEncode(Headline.GetHeaderText()), Headline.NavigateUrl,
					Headline.Target, "", Headline.IsEnabled());
				RenderUtils.PrepareHyperLinkStyle(fLink, Headline.GetHeaderStyle());
				if(Headline.GetTailOnClick() != "")
					RenderUtils.SetStringAttribute(fLink, "onclick", Headline.GetTailOnClick());
			}
			if (fText != null)
				fText.Text = Headline.HtmlEncode(Headline.GetHeaderText());
		}
	}
	public abstract class HeadlinePanelControl: HeadlineControlBase {
		private Image fImage = null;
		private HyperLink fLink = null;
		public HeadlinePanelControl(ASPxHeadline headline)
			: base(headline) {
		}
		protected override void ClearControlFields() {
			fImage = null;
			fLink = null;
		}
		protected override void CreateControlHierarchy() {
			if (IsDateInPanel() && Headline.IsDateInPanelTop()) {
				HeadlineDateControl dateControl = new HeadlineDateControl(Headline);
				Controls.Add(dateControl);
			}
			if (IsImageInPanel()) {
				fImage = RenderUtils.CreateImage();
				if (Headline.ImageHasLink()) {
					fLink = RenderUtils.CreateHyperLink();
					Controls.Add(fLink);
					fLink.Controls.Add(fImage);
				} else {
					Controls.Add(fImage);
				}
			}
			if (IsDateInPanel() && !Headline.IsDateInPanelTop()) {
				HeadlineDateControl dateControl = new HeadlineDateControl(Headline);
				Controls.Add(dateControl);
			}
		}
		protected override void PrepareControlHierarchy() {
			if (fImage != null) {
				Headline.Image.AssignToControl(fImage, DesignMode);
				if (Headline.HasImageSpacing())
					RenderUtils.SetVerticalMargins(fImage, Unit.Empty, GetPanelImageSpacing());
			}
			if (fLink != null) {
				RenderUtils.PrepareHyperLink(fLink, "", Headline.NavigateUrl, Headline.Target, "", Headline.IsEnabled());
				if(Headline.GetTailOnClick() != "")
					RenderUtils.SetStringAttribute(fLink, "onclick", Headline.GetTailOnClick());
			}
		}
		protected abstract bool IsDateInPanel();
		protected abstract bool IsImageInPanel();
		protected abstract Unit GetPanelImageSpacing();
	}
	public class HeadlineLeftPanelControl: HeadlinePanelControl {
		public HeadlineLeftPanelControl(ASPxHeadline headline)
			: base(headline) {
		}
		protected override bool IsDateInPanel() {
			return Headline.IsDateInLeftPanel();
		}
		protected override bool IsImageInPanel() {
			return Headline.IsImageInLeftPanel();
		}
		protected override Unit GetPanelImageSpacing() {
			return Headline.GetLeftPanelImageSpacing();
		}
	}
	public class HeadlineRightPanelControl: HeadlinePanelControl {
		public HeadlineRightPanelControl(ASPxHeadline headline)
			: base(headline) {
		}
		protected override bool IsDateInPanel() {
			return Headline.IsDateInRightPanel();
		}
		protected override bool IsImageInPanel() {
			return Headline.IsImageInRightPanel();
		}
		protected override Unit GetPanelImageSpacing() {
			return Headline.GetRightPanelImageSpacing();
		}
	}
	public class HeadlineContentControl : HeadlineControlBase {
		private WebControl fDiv = null;
		private WebControl fSpan = null;
		private HyperLink fLink = null;
		private LiteralControl fText = null;
		private LiteralControl fLastWord = null;
		public HeadlineContentControl(ASPxHeadline headline)
			: base(headline) {
		}
		protected override void ClearControlFields() {
			fDiv = null;
			fSpan = null;
			fLink = null;
			fText = null;
			fLastWord = null;
		}
		protected override void CreateControlHierarchy() {
			Control parent = this;
			if (!Headline.IsSimpleRender()) {
				fDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				Controls.Add(fDiv);
				parent = fDiv;
			}
			CreateMainBlocks(parent);
		}
		protected override void PrepareControlHierarchy() {
			if (fDiv != null) {
				Headline.GetContentStyle().AssignToControl(fDiv, AttributesRange.All);
				RenderUtils.SetPaddings(fDiv, Headline.GetContentPaddings());
				RenderUtils.SetLineHeight(fDiv, Headline.GetContentLineHeight());
				fDiv.ToolTip = Headline.GetContentToolTip();
			}
			if(fSpan != null)
				fSpan.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");			
			if (fLink != null) {
				RenderUtils.PrepareHyperLink(fLink, "", Headline.NavigateUrl, Headline.Target, "", Headline.IsEnabled());
				RenderUtils.PrepareHyperLinkStyle(fLink, Headline.GetContentStyle());
				if (!Headline.TailImageHasLink())
					fLink.Style.Add(HtmlTextWriterStyle.Display, "block");
				if(Headline.GetTailOnClick() != "")
					RenderUtils.SetStringAttribute(fLink, "onclick", Headline.GetTailOnClick());
			}
			if (fText != null)
				fText.Text = Headline.GetTextPart();
			if (fLastWord != null)
				fLastWord.Text = Headline.GetLastWordPart();
			if (Headline.IsDateInBottom() || Headline.IsTailInNewLine())
				RenderUtils.SetVerticalMargins(fDiv, Unit.Empty, Headline.GetContentSpacing());
			if((Headline.GetTailOnClick() != "") && Headline.HasTailSpan() &&
				(Headline.NavigateUrl == "") && (Headline.GetContentText() == "")) {
				WebControl clickableControl = fSpan;
				if(clickableControl == null)
					clickableControl = fDiv;
				RenderUtils.SetStringAttribute(clickableControl, "onclick", Headline.GetTailOnClick());
				RenderUtils.SetCursor(clickableControl, RenderUtils.GetPointerCursor());				
			}
		}
		protected void CreateMainBlocks(Control parent) {
			if(Headline.ContentHasLink()) {
				if(!Headline.TailImageHasLink() || Headline.IsLastWordExists()) {
					fLink = RenderUtils.CreateHyperLink();
					parent.Controls.Add(fLink);
					parent = fLink;
				}
			}
			CreateContentText(parent);
			if((Headline.HasTail() && Headline.TailPosition != TailPosition.NewLine) || Headline.HasContentEllipsis() || Headline.IsLastWordExists())
				CreateTail(parent);
		}
		protected void CreateContentText(Control parent) {
			fText = RenderUtils.CreateLiteralControl();
			if(Headline.ContentHasLink() && Headline.TailImageHasLink() && !Headline.IsLastWordExists()) {
				fLink = RenderUtils.CreateHyperLink();
				parent.Controls.Add(fLink);
				fLink.Controls.Add(fText);
			} else
				parent.Controls.Add(fText);
		}
		protected void CreateTail(Control parent) {
			if (Headline.HasTailSpan()) {
				fSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				parent.Controls.Add(fSpan);
				parent = fSpan;
			}
			CreateTailControls(parent);
		}
		protected void CreateTailControls(Control parent) {
			if (Headline.IsLastWordExists()) {
				fLastWord = RenderUtils.CreateLiteralControl();
				parent.Controls.Add(fLastWord);
			}
			if (Headline.IsTailRequired() && Headline.TailPosition != TailPosition.NewLine) {
				HeadlineTailControl tailControl = new HeadlineTailControl(Headline);
				parent.Controls.Add(tailControl);
			}
		}
	}
}
