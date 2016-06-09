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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)]
	public class PagerPopupMenu : ASPxPopupMenu {
		public PagerPopupMenu(ASPxPagerBase skinOwner)
			: base() {
			ParentSkinOwner = skinOwner;
		}
	}
	public class PagerControlBase : ASPxInternalWebControl {
		private ASPxPagerBase fPager = null;
		public ASPxPagerBase Pager {
			get { return fPager; }
		}
		public PagerControlBase(ASPxPagerBase pager) {
			fPager = pager;
		}
		protected bool IsRightToLeft {
			get { return (Pager as ISkinOwner).IsRightToLeft(); }
		}
	}
	public class PagerMainControlLiteDesignMode : PagerMainControlLite {
		protected InternalTable Table { get; private set; }
		public PagerMainControlLiteDesignMode(ASPxPagerBase pager)
			: base(pager) {
		}
		protected override void CreateControlHierarchy() {
			Table = RenderUtils.CreateTable();
			Table.Rows.Add(RenderUtils.CreateTableRow());
			Controls.Add(Table);
			base.CreateControlHierarchy();
		}
		protected override void AddChild(ASPxInternalWebControl child) {
			TableCell cell = RenderUtils.CreateTableCell();
			Table.Rows[0].Cells.Add(cell);
			cell.Controls.Add(child);
			if(child is PagerPageSizeControlLiteDesignMode)
				cell.CssClass = PagerStyles.DesignModePageSizeItemClassName;
		}
		protected override PagerPageSizeControlLite CreatePagerPageSizeControl() {
			return new PagerPageSizeControlLiteDesignMode(Pager, Pager.PageSizeItemSettings, Pager.GetPageSizeDropDownImage());
		}
	}
	public class PagerMainControlLite : PagerControlBase {
		protected HiddenField StateHiddenField { get; private set; }
		public PagerMainControlLite(ASPxPagerBase pager)
			: base(pager) {
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override void CreateControlHierarchy() {
			ModelBuilder builder = new ModelBuilder(Pager, new ModelSection(Pager));
			builder.Build();
			bool showSeparators = Pager.ShowSeparators;
			if(IsRightToLeft)
				builder.Result.Reverse();
			for(int i = 0; i < builder.Result.Count; i++) {
				ModelItem item = builder.Result[i];
				if(showSeparators && item != builder.Result[0])
					AddChild(new PagerSeparatorControlLite(Pager));
				if(item == ModelItem.Summary) {
					if(IsSummaryLeftSpacerNeeded() && !IsRightToLeft || IsSummaryRightSpacerNeeded() && IsRightToLeft)
						AddChild(new PagerSpacerControlLite(Pager));
					AddChild(new PagerSummaryControlLite(Pager));
					if(IsSummaryRightSpacerNeeded() && !IsRightToLeft || IsSummaryLeftSpacerNeeded() && IsRightToLeft)
						AddChild(new PagerSpacerControlLite(Pager));
				}
				else if(item == ModelItem.First)
					AddChild(new PagerButtonControlLite(Pager, Pager.FirstPageButton, Pager.GetFirstButtonImage(), 0));
				else if(item == ModelItem.Prev)
					AddChild(new PagerButtonControlLite(Pager, Pager.PrevPageButton, Pager.GetPrevButtonImage(), Pager.PageIndex - 1));
				else if(item == ModelItem.Ellipsis) {
					if(IsNumericLeftSpacerNeeded() && !IsRightToLeft || IsNumericRightSpacerNeeded() && IsRightToLeft) {
						if(i > 0 && !(builder.Result[i - 1] is ModelNumericItem) && builder.Result[i - 1] != ModelItem.Ellipsis)
							AddChild(new PagerSpacerControlLite(Pager));
					}
					AddChild(new PagerEllipsisControlLite(Pager));
					if(IsNumericRightSpacerNeeded() && !IsRightToLeft || IsNumericLeftSpacerNeeded() && IsRightToLeft) {
						if(i < builder.Result.Count - 1 && !(builder.Result[i + 1] is ModelNumericItem) && builder.Result[i + 1] != ModelItem.Ellipsis)
							AddChild(new PagerSpacerControlLite(Pager));
					}
				}
				else if(item == ModelItem.Next)
					AddChild(new PagerButtonControlLite(Pager, Pager.NextPageButton, Pager.GetNextButtonImage(), Pager.PageIndex + 1));
				else if(item == ModelItem.Last)
					AddChild(new PagerButtonControlLite(Pager, Pager.LastPageButton, Pager.GetLastButtonImage(), Pager.PageCount - 1));
				else if(item == ModelItem.All)
					AddChild(new PagerButtonControlLite(Pager, Pager.AllButton, Pager.GetAllButtonImage(), -1));
				else if(item == ModelItem.PageSize && Pager.IsPageSizeVisible()) {
					if(IsPageSizeLeftSpacerNeeded() && !IsRightToLeft || IsPageSizeRightSpacerNeeded() && IsRightToLeft)
						AddChild(new PagerSpacerControlLite(Pager));
					AddChild(CreatePagerPageSizeControl());
					if(IsPageSizeRightSpacerNeeded() && !IsRightToLeft || IsPageSizeLeftSpacerNeeded() && IsRightToLeft)
						AddChild(new PagerSpacerControlLite(Pager));
				}
				else {
					ModelNumericItem number = item as ModelNumericItem;
					if(number != null) {
						if(IsNumericLeftSpacerNeeded() && !IsRightToLeft || IsNumericRightSpacerNeeded() && IsRightToLeft) {
							if(i > 0 && !(builder.Result[i - 1] is ModelNumericItem) && builder.Result[i - 1] != ModelItem.Ellipsis)
								AddChild(new PagerSpacerControlLite(Pager));
						}
						AddChild(new PagerNumericButtonControlLite(Pager, number.Value));
						if(IsNumericRightSpacerNeeded() && !IsRightToLeft || IsNumericLeftSpacerNeeded() && IsRightToLeft) {
							if(i < builder.Result.Count - 1 && !(builder.Result[i + 1] is ModelNumericItem) && builder.Result[i + 1] != ModelItem.Ellipsis)
								AddChild(new PagerSpacerControlLite(Pager));
						}
					}
				}
			}
			if(Pager.CanStoreIndexes()) {
				StateHiddenField = RenderUtils.CreateHiddenField();
				StateHiddenField.ID = ASPxPagerBase.PagerHiddenFieldId;
				Controls.Add(StateHiddenField);
			}
		}
		protected virtual void AddChild(ASPxInternalWebControl child) {
			Controls.Add(child);
		}
		protected virtual PagerPageSizeControlLite CreatePagerPageSizeControl() {
			return new PagerPageSizeControlLite(Pager, Pager.PageSizeItemSettings, Pager.GetPageSizeDropDownImage());
		}
		protected bool IsSummarySpacersNeeded() {
			return !Pager.Width.IsEmpty && !Pager.IsPageSizeVisible() && Pager.Summary.Visible;
		}
		protected bool IsSummaryLeftSpacerNeeded() {
			return IsSummarySpacersNeeded() && (Pager.Summary.Position == PagerButtonPosition.Right || Pager.Summary.Position == PagerButtonPosition.Inside);
		}
		protected bool IsSummaryRightSpacerNeeded() {
			return IsSummarySpacersNeeded() && (Pager.Summary.Position == PagerButtonPosition.Left || (Pager.Summary.Position == PagerButtonPosition.Inside && !Pager.ShowNumericButtons));
		}
		protected bool IsNumericSpacersNeeded() {
			return !Pager.Width.IsEmpty && !Pager.IsPageSizeVisible() && Pager.ShowNumericButtons && Pager.Summary.Position == PagerButtonPosition.Inside;
		}
		protected bool IsNumericLeftSpacerNeeded() {
			return IsNumericSpacersNeeded() && !Pager.Summary.Visible;
		}
		protected bool IsNumericRightSpacerNeeded() {
			return IsNumericSpacersNeeded();
		}
		protected bool IsPageSizeSpacersNeeded() {
			return !Pager.Width.IsEmpty;
		}
		protected bool IsPageSizeLeftSpacerNeeded() {
			return IsPageSizeSpacersNeeded() && Pager.PageSizeItemSettings.Position == PagerPageSizePosition.Right;
		}
		protected bool IsPageSizeRightSpacerNeeded() {
			return IsPageSizeSpacersNeeded() && Pager.PageSizeItemSettings.Position == PagerPageSizePosition.Left;
		}
		protected override void PrepareControlHierarchy() {
			if(Pager.CanStoreIndexes())
				StateHiddenField.Value = HtmlConvertor.ToJSON(Pager.GetPagerStateObject(), false, false, true);
			RenderUtils.AssignAttributes(Pager, this);
			Pager.GetControlStyle().AssignToControl(this, true);
			if(IsRightToLeft)
				Style["float"] = "right";
			SetLeadClassName();
		}
		void SetLeadClassName() {
			if(DesignMode || Controls.Count < 1)
				return;
			RenderUtils.AppendDefaultDXClassName((WebControl)Controls[0], PagerStyles.LeadClassName);
		}
	}
	public abstract class PagerItemControlLite : PagerControlBase {
		public PagerItemControlLite(ASPxPagerBase pager)
			: base(pager) {
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.B; } }
		protected override sealed void PrepareControlHierarchy() {
			PrepareControlHierarchyCore();
			ApplyItemSpacing();
		}
		protected abstract void PrepareControlHierarchyCore();
		void ApplyItemSpacing() {
			if(Parent.Controls[0] == this)
				return;
			if(Pager.ItemSpacing.IsEmpty)
				return;
			Style[HtmlTextWriterStyle.MarginLeft] = Pager.ItemSpacing.ToString();
		}
	}
	public class PagerSpacerControlLite : PagerItemControlLite {
		public PagerSpacerControlLite(ASPxPagerBase pager)
			: base(pager) {
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(RenderUtils.CreateLiteralControl("&nbsp;"));
		}
		protected override void PrepareControlHierarchyCore() {
			RenderUtils.AppendDefaultDXClassName(this, "dxp-spacer");
		}
	}
	public class PagerSummaryControlLite : PagerItemControlLite {
		public PagerSummaryControlLite(ASPxPagerBase pager)
			: base(pager) {
		}
		protected ITextControl TextControl { get { return (ITextControl)Controls[0]; } }
		protected override void CreateControlHierarchy() {
			Controls.Add(RenderUtils.CreateLiteralControl());
		}
		protected override void PrepareControlHierarchyCore() {
			TextControl.Text = Pager.GetSummaryText();
			Pager.GetSummaryStyle().AssignToControl(this, true);
		}
	}
	public class PagerSeparatorControlLite : PagerItemControlLite {
		public PagerSeparatorControlLite(ASPxPagerBase pager)
			: base(pager) {
		}
		protected override void PrepareControlHierarchyCore() {
			AppearanceStyle style = Pager.GetSeparatorStyle();
			style.AssignToControl(this);
			RenderUtils.SetMargins(this, style.Paddings);
			if(!style.Width.IsEmpty)
				Style[HtmlTextWriterStyle.Width] = style.Width.ToString();
			if(!style.Height.IsEmpty)
				Style[HtmlTextWriterStyle.Height] = style.Height.ToString();
		}
	}
	public class PagerButtonControlLite : PagerItemControlLite {
		PagerButtonProperties settings;
		ImagePropertiesBase image;
		int pageIndex;
		public PagerButtonControlLite(ASPxPagerBase pager, PagerButtonProperties settings, ImagePropertiesBase image, int pageIndex)
			: base(pager) {
			this.settings = settings;
			this.image = image;
			this.pageIndex = pageIndex;
		}
		protected override HtmlTextWriterTag TagKey {
			get { return !ButtonIsDisabled ? HtmlTextWriterTag.A : base.TagKey; }
		}
		protected PagerButtonProperties Settings { get { return settings; } }
		protected ImagePropertiesBase Image { get { return image; } }
		protected int PageIndex { get { return pageIndex; } }
		protected SimpleButtonControl Button { get { return (SimpleButtonControl)Controls[0]; } }
		protected bool HasText { get { return !String.IsNullOrEmpty(Settings.GetText()); } }
		protected bool HasImage { get { return Image != null && !Image.IsEmpty; } }
		protected bool ButtonIsDisabled {
			get { return Settings.IsDisabled(Pager.PageIndex, Pager.PageCount) || !IsEnabled(); }
		}
		protected override void CreateControlHierarchy() {
			SimpleButtonControl button = new SimpleButtonControl(Settings.GetText(), Image, Settings.ImagePosition, string.Empty);
			Controls.Add(button);
		}
		protected override void PrepareControlHierarchyCore() {
			PagerButtonStyle style = Pager.GetButtonStyle(ButtonIsDisabled, HasText, HasImage);
			Button.Enabled = !ButtonIsDisabled;
			Button.ButtonStyle = style;
			Button.ButtonImageSpacing = style.ImageSpacing;
			style.AssignToControl(this, true);
			if(!ButtonIsDisabled) {
				RenderUtils.SetStringAttribute(this, "onclick", Pager.GetItemElementOnClickInternal(Pager.GetButtonID(Settings)));
				RenderUtils.SetStringAttribute(this, "href", Pager.GetButtonNavigateUrl(PageIndex));
			}
			if(RenderUtils.IsHtml5Mode(this) && Pager.IsAccessibilityCompliantRender()) {
				string label = string.Format(AccessibilityUtils.PagerNavigationFormatString, 
					Pager.PrevPageButton.Equals(Settings) ? AccessibilityUtils.PagerPreviousPageText : AccessibilityUtils.PagerNextPageText);
				RenderUtils.SetStringAttribute(this, "aria-label", label);
			}
		}
	}
	public class PagerNumericButtonControlLite : PagerItemControlLite {
		int pageIndex;
		public PagerNumericButtonControlLite(ASPxPagerBase pager, int pageIndex)
			: base(pager) {
			this.pageIndex = pageIndex;
		}
		protected override HtmlTextWriterTag TagKey {
			get { return !IsCurrentPage && IsEnabled() ? HtmlTextWriterTag.A : base.TagKey; } 
		}
		protected int PageIndex { get { return pageIndex; } }
		protected bool IsCurrentPage { get { return (Pager.PageIndex == PageIndex) || (Pager.PageIndex == -1 && Pager.PageCount == 1); } }
		protected ITextControl TextControl { get { return (ITextControl)Controls[0]; } }
		protected override void CreateControlHierarchy() {
			Controls.Add(RenderUtils.CreateLiteralControl());
		}
		protected override void PrepareControlHierarchyCore() {
			TextControl.Text = Pager.GetPageNumberButtonText(IsCurrentPage, PageIndex);
			Pager.GetPageNumberStyle(IsCurrentPage).AssignToControl(this, true);
			if(IsEnabled() && !IsCurrentPage) {
				RenderUtils.SetStringAttribute(this, "href", Pager.GetButtonNavigateUrl(PageIndex));
				if(!Pager.IsSEOEnabled)
					RenderUtils.SetStringAttribute(this, "onclick", Pager.GetItemElementOnClickInternal(Pager.GetNumberButtonID(PageIndex)));
			}
			if(RenderUtils.IsHtml5Mode(this) && Pager.IsAccessibilityCompliantRender()) {
				string label = string.Format(AccessibilityUtils.PagerSummaryFormatString, PageIndex + 1, Pager.PageCount);
				RenderUtils.SetStringAttribute(this, "aria-label", label);
			}
		}
	}
	public class PagerEllipsisControlLite : PagerItemControlLite {
		public PagerEllipsisControlLite(ASPxPagerBase pager)
			: base(pager) {
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(RenderUtils.CreateLiteralControl("&hellip;"));
		}
		protected override void PrepareControlHierarchyCore() {
			Pager.GetEllipsisStyle().AssignToControl(this);
		}
	}
	public class PagerPageSizeControlLite : PagerItemControlLite {
		protected WebControl PageSizeBox { get; set; }
		protected WebControl PageSizeInput { get; set; }
		protected WebControl DropDownButton { get; set; }
		protected SimpleButtonControl ButtonControl { get; set; }
		protected WebControl PageSizeCaption { get; set; }
		protected PageSizeItemSettings Settings { get; set; }
		protected ImagePropertiesBase DropDownImage { get; set; }
		protected ASPxPopupMenu DropDownWindow { get; set; }
		public PagerPageSizeControlLite(ASPxPagerBase pager, PageSizeItemSettings settings, ImagePropertiesBase image)
			: base(pager) {
			Settings = settings;
			DropDownImage = image;
		}
		protected bool IsDisabled {
			get { return !Pager.IsEnabled() || Settings.IsDisabled(Pager.PageIndex, Pager.PageCount); }
		}
		protected override void ClearControlFields() {
			PageSizeBox = null;
			PageSizeCaption = null;
			PageSizeInput = null;
			DropDownButton = null;
		}
		protected override void CreateControlHierarchy() {
			CreatePageSizeCaption();
			CreatePageSizeBox();
			if(!Pager.DesignMode && !IsDisabled)
				CreateDropDownWindow();
		}
		protected override void PrepareControlHierarchyCore() {
			Pager.GetPageSizeItemStyle().AssignToControl(this);
			RenderUtils.SetPaddings(this, Pager.GetPageSizeItemPaddings());
			RenderUtils.AppendDefaultDXClassName(PageSizeCaption, "dx");
			PreparePageSizeBox();
			PageSizeBox.Height = Pager.GetPageSizeItemHeight();
			PageSizeInput.Width = Pager.GetPageSizeItemWidth();
		}
		protected virtual void CreatePageSizeCaption() {
			PageSizeCaption = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			Controls.Add(PageSizeCaption);
			PageSizeCaption.Controls.Add(CreateLabel());
		}
		protected Label CreateLabel() {
			Label label = RenderUtils.CreateLabel();
			label.AssociatedControlID = Pager.GetPageSizeInputID();
			label.Text = Settings.Caption;
			return label;
		}
		protected virtual void CreatePageSizeBox() {
			PageSizeBox = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			PageSizeBox.ID = Pager.GetPageSizeBoxID();
			Controls.Add(PageSizeBox);
			PageSizeInput = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			PageSizeInput.ID = Pager.GetPageSizeInputID();
			PageSizeBox.Controls.Add(PageSizeInput);
			DropDownButton = CreateDropDownButton();
			PageSizeBox.Controls.Add(DropDownButton);
		}
		protected WebControl CreateDropDownButton() {
			WebControl dropDownButton = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			dropDownButton.ID = Pager.GetPageSizeDropDownButtonID();
			ButtonControl = new SimpleButtonControl(string.Empty, DropDownImage, ImagePosition.Right, string.Empty);
			dropDownButton.Controls.Add(ButtonControl);
			ButtonControl.ButtonImageID = Pager.GetPageSizeDropDownButtonImageID();
			return dropDownButton;
		}
		protected void CreateDropDownWindow() {
			DropDownWindow = new ASPxPopupMenu();
			Pager.Controls.Add(DropDownWindow);
			DropDownWindow.ID = Pager.GetPageSizePopupControlID();
			DropDownWindow.ParentSkinOwner = Pager;
			DropDownWindow.SyncSelectionMode = SyncSelectionMode.None;
			DropDownWindow.GutterWidth = Unit.Pixel(0);
			CreateDropDownItems();
		}
		protected void CreateDropDownItems() {
			List<MenuItem> items = Pager.GetPageSizeMenuItems();
			DropDownWindow.Items.AddRange(items);
			MenuItem allItem = DropDownWindow.Items.FindByName("-1");
			if(allItem != null)
				allItem.BeginGroup = true;
			MenuItem selectedItem = DropDownWindow.Items.FindByName(Pager.GetPageSize().ToString());
			if(selectedItem != null)
				DropDownWindow.SelectedItem = selectedItem;
		}
		protected void PreparePageSizeBox() {
			Pager.GetComboBoxStyle(IsDisabled).AssignToControl(PageSizeBox, AttributesRange.Common | AttributesRange.Cell);
			RenderUtils.SetStyleUnitAttribute(PageSizeBox, IsRightToLeft ? "margin-right" : "margin-left", Pager.GetPageSizeItemStyle().CaptionSpacing);
			RenderUtils.SetPaddings(PageSizeBox, Pager.GetPageSizeBoxPaddings(IsDisabled));
			if(!IsDisabled)
				RenderUtils.SetStringAttribute(PageSizeBox, "onmousedown", Pager.GetPageSizeClickHandler());
			PreparePageSizeInput();
			PrepareDropDownButton();
			if(DropDownWindow != null)
				PrepareDropDownWindow();
		}
		protected void PreparePageSizeInput() {
			Pager.GetComboBoxStyle(IsDisabled).AssignToControl(PageSizeInput, AttributesRange.Font);
			PageSizeInput.Attributes.Add("type", "text");
			PageSizeInput.Attributes.Add("readonly", "readonly");
			PageSizeInput.Attributes.Add("value", Pager.GetPageSizeText());
			if(IsDisabled)
				PageSizeInput.Attributes.Add("disabled", "disabled");
			else {
				RenderUtils.SetStringAttribute(PageSizeInput, "onkeydown", Pager.GetPageSizeKeyDownHandler());
				RenderUtils.SetStringAttribute(PageSizeInput, "onblur", Pager.GetPageSizeInputBlurHandler());
			}
			RenderUtils.SetStyleUnitAttribute(PageSizeInput, "border-width", 0);
			RenderUtils.SetStyleUnitAttribute(DropDownButton, IsRightToLeft ? "margin-right" : "margin-left", Pager.GetComboBoxStyle(IsDisabled).DropDownButtonSpacing);
		}
		protected void PrepareDropDownButton() {
			PagerDropDownButtonStyle style = Pager.GetPageSizeDropDownButtonStyle(IsDisabled);
			style.AssignToControl(DropDownButton, AttributesRange.All);
			RenderUtils.SetPaddings(DropDownButton, Pager.GetPageSizeDropDownButtonPaddings(IsDisabled));
			ButtonControl.Enabled = !IsDisabled;
		}
		protected void PrepareDropDownWindow() {
			DropDownWindow.AllowSelectItem = true;
			DropDownWindow.PopupElementID = Pager.GetPageSizeBoxID();
			DropDownWindow.PopupHorizontalAlign = PopupHorizontalAlign.LeftSides;
			DropDownWindow.PopupVerticalAlign = PopupVerticalAlign.Below;
			DropDownWindow.ParentStyles = Pager.GetPageSizeDropDownWindowStyle();
			DropDownWindow.ShowSubMenuShadow = Settings.ShowPopupShadow;
			DropDownWindow.ClientSideEvents.ItemClick = Pager.GetPageSizePopupItemElementOnClickInternal();
		}
	}
	public class PagerPageSizeControlLiteDesignMode : PagerPageSizeControlLite {
		protected InternalTable MainTable { get; private set; }
		protected InternalTable PageSizeBoxTable { get { return PageSizeBox as InternalTable; } }
		public PagerPageSizeControlLiteDesignMode(ASPxPagerBase pager, PageSizeItemSettings settings, ImagePropertiesBase image)
			: base(pager, settings, image) {
		}
		protected override void CreateControlHierarchy() {
			MainTable = RenderUtils.CreateTable();
			Controls.Add(MainTable);
			MainTable.Rows.Add(RenderUtils.CreateTableRow());
			base.CreateControlHierarchy();
		}
		protected override void CreatePageSizeCaption() {
			PageSizeCaption = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			AddChildToMainTable(PageSizeCaption);
			PageSizeCaption.Controls.Add(CreateLabel());
		}
		protected override void CreatePageSizeBox() {
			PageSizeBox = RenderUtils.CreateTable();
			AddChildToMainTable(PageSizeBoxTable);
			PageSizeInput = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			CreatePageSizeBoxTableCell().Controls.Add(PageSizeInput);
			DropDownButton = CreateDropDownButton();
			CreatePageSizeBoxTableCell().Controls.Add(DropDownButton);
		}
		protected TableCell CreatePageSizeBoxTableCell() {
			TableRow row = null;
			if(PageSizeBoxTable.Rows.Count == 0) {
				row = RenderUtils.CreateTableRow();
				PageSizeBoxTable.Rows.Add(row);
			} else
				row = PageSizeBoxTable.Rows[0];
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			return cell;
		}
		protected void AddChildToMainTable(WebControl child) {
			TableCell cell = RenderUtils.CreateTableCell();
			cell.Controls.Add(child);
			MainTable.Rows[0].Cells.Add(cell);
		}
	}
}
