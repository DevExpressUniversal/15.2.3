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

using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class TrackBarControl : ASPxInternalWebControl {
		private 
			ASPxTrackBar trackBar = null;
			WebControl mainElement = null;
			WebControl decrementButton = null;
			WebControl incrementButton = null;
			WebControl scale = null;
			WebControl track = null;
			WebControl mainDragHandle = null;
			WebControl secondaryDragHandle = null;
			WebControl barHighlight = null;
			WebControl contentContainer = null;
			KeyboardSupportInputHelper input = null;
			TrackBarTickModeStrategy behaviorStrategy = null;
		public TrackBarControl(ASPxTrackBar trackBar) {
			this.trackBar = trackBar;
			this.behaviorStrategy = TrackBar.IsItemMode ? new TrackBarItemModeStrategy(this, TrackBar) :
				new TrackBarTickModeStrategy(this, TrackBar);
		}
		protected KeyboardSupportInputHelper KbInput { get { return input; } }
		protected bool IsHorizontal {
			get { return TrackBar.Orientation == Orientation.Horizontal; }
		}
		protected internal WebControl Scale {
			get { return scale; }
		}
		protected ASPxTrackBar TrackBar {
			get { return trackBar; }
		}
		protected override void ClearControlFields() {
			this.mainElement = null;
			this.decrementButton = null;
			this.incrementButton = null;
			this.scale = null;
			this.track = null;
			this.mainDragHandle = null;
			this.barHighlight = null;
			this.contentContainer = null;
			this.secondaryDragHandle = null;
			this.input = null;
		}
		protected WebControl CreateAnchor() {
			WebControl anchor = RenderUtils.CreateWebControl(HtmlTextWriterTag.A);
			anchor.Attributes["href"] = RenderUtils.AccessibilityEmptyUrl;
			return anchor;
		}
		protected WebControl CreateButton(bool incButton) {
			WebControl button = CreateAnchor();
			if(!DesignMode)
				button.Controls.Add(CreateButtonToolTip(incButton));
			return button;
		}
		protected WebControl CreateButtonToolTip(bool incButton) {
			WebControl toolTip = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			toolTip.Controls.Add(new LiteralControl(incButton ? TrackBar.IncrementButtonToolTip : TrackBar.DecrementButtonToolTip));
			return toolTip;
		}
		protected WebControl CreateContentElement() {
			return RenderUtils.CreateDiv();
		}
		protected WebControl CreateContentContainer() {
			WebControl contentContainerElement = RenderUtils.CreateDiv();
			if(TrackBar.ShowChangeButtons) {
				this.decrementButton = CreateButton(false);
				this.incrementButton = CreateButton(true);
			}
			this.track = CreateTrack();
			if(TrackBar.ShowChangeButtons)
				contentContainerElement.Controls.Add(TrackBar.IsNormalDirection ? this.incrementButton : this.decrementButton);
			if(TrackBar.ScalePosition != ScalePosition.None) {
				this.scale = CreateScale();
				contentContainerElement.Controls.Add(this.scale);
			}
			contentContainerElement.Controls.Add(this.track);
			if(TrackBar.ShowChangeButtons)
				contentContainerElement.Controls.Add(TrackBar.IsNormalDirection ? this.decrementButton : this.incrementButton);
			this.input = CreateInput();
			contentContainerElement.Controls.Add(this.input);
			return contentContainerElement;
		}
		protected override void CreateControlHierarchy() {
			this.mainElement = DesignMode ? CreateDesignMainElement() : CreateMainElement();
			Controls.Add(this.mainElement);
		}
		protected WebControl CreateDesignMainElement() {
			DesignModeRenderHelper.MainTable mainElement = DesignModeRenderHelper.GetMainTable(TrackBar.Orientation);
			if(TrackBar.ShowChangeButtons) {
				this.decrementButton = CreateButton(false);
				this.incrementButton = CreateButton(true);
			}
			if(TrackBar.ShowChangeButtons)
				mainElement.FirstTableCell.Controls.Add(this.decrementButton);
			this.track = CreateTrack();
			if(IsHorizontal)
				this.track.Height = Unit.Parse("100%");
			mainElement.SecondTableCell.Controls.Add(this.track);
			if(TrackBar.ShowChangeButtons)
				mainElement.ThirdTableCell.Controls.Add(this.incrementButton);
			return mainElement;
		}
		protected KeyboardSupportInputHelper CreateInput() {
			return new KeyboardSupportInputHelper(ASPxTrackBar.InputId);
		}
		protected WebControl CreateMainElement() {
			WebControl mainElement = RenderUtils.CreateDiv();
			this.contentContainer = CreateContentContainer();
			mainElement.Controls.Add(this.contentContainer);
			return mainElement;
		}
		protected WebControl CreateScale() {
			WebControl scale = RenderUtils.CreateWebControl(HtmlTextWriterTag.Ul);
			this.behaviorStrategy.CompleteScaleContent(scale);
			return scale;
		}
		protected WebControl CreateDragHandle() {
			WebControl result = CreateAnchor();
			WebControl contentContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			contentContainer.Controls.Add(new LiteralControl(TrackBar.DragHandleToolTip));
			result.Controls.Add(contentContainer);
			if(DesignMode && IsHorizontal)
				result.Style["left"] = "0";
			return result;
		}
		protected WebControl CreateBarHighlight() {
			WebControl barHighlight = RenderUtils.CreateDiv();
			barHighlight.ID = ASPxTrackBar.BarHighlightId;
			return barHighlight;
		}
		protected WebControl CreateTrack() {
			WebControl track = RenderUtils.CreateDiv();
			track.ID = ASPxTrackBar.TrackId;
			this.barHighlight = CreateBarHighlight();
			track.Controls.Add(this.barHighlight);
			if(!DesignMode || (DesignMode && TrackBar.ShowDragHandles)) {
				this.mainDragHandle = CreateDragHandle();
				track.Controls.Add(this.mainDragHandle);
				if(TrackBar.AllowRangeSelection && !DesignMode) {
					this.secondaryDragHandle = CreateDragHandle();
					track.Controls.Add(this.secondaryDragHandle);
				}
			}
			return track;
		}
		protected void PrepareButton(TrackBarButtonStyle style, ButtonImageProperties image, string toolTip, WebControl buttonElement) {
			style.AssignToControl(buttonElement);
			style.Paddings.AssignToControl(buttonElement);
			ResolveImageUrl(image);
			image.AssignToControl(buttonElement, DesignMode, !TrackBar.IsEnabled());
			buttonElement.ToolTip = toolTip;
		}
		protected void PrepareButtons() {
			PrepareButton(TrackBar.GetIncrementButtonStyle(), TrackBar.GetIncrementButtonImage(), TrackBar.IncrementButtonToolTip, this.incrementButton);
			PrepareButton(TrackBar.GetDecrementButtonStyle(), TrackBar.GetDecrementButtonImage(), TrackBar.DecrementButtonToolTip, this.decrementButton);
			if(DesignMode) {
				DesignModeRenderHelper.PrepareButton(this.incrementButton, IsHorizontal);
				DesignModeRenderHelper.PrepareButton(this.decrementButton, IsHorizontal);
			}
		}
		protected void PrepareContentContainer() {
			if(TrackBar.ScalePosition != ScalePosition.None && !DesignMode)
				PrepareScale();
			PrepareTrack();
			if(TrackBar.ShowChangeButtons)
				PrepareButtons();
			if(!DesignMode)
				PrepareInput();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareMainElement();
		}
		protected void PrepareDragHandles() {
			PrepareButton(TrackBar.GetMainDragHandleStyle(), TrackBar.GetMainDragHandleImage(), TrackBar.DragHandleToolTip, this.mainDragHandle);
			if(DesignMode)
				DesignModeRenderHelper.PrepareDragHandle(this.mainDragHandle, IsHorizontal);
			if(TrackBar.AllowRangeSelection && this.secondaryDragHandle != null)
				PrepareButton(TrackBar.GetSecondaryDragHandleStyle(), TrackBar.GetSecondaryDragHandleImage(), TrackBar.DragHandleToolTip, this.secondaryDragHandle);
		}
		protected virtual void PrepareInput() {
			this.input.Input.Attributes.Add("name", TrackBar.UniqueID);
			this.input.TabIndex = TrackBar.TabIndex;
			this.input.Input.AccessKey = TrackBar.AccessKey;
			this.behaviorStrategy.PrepareInput(this.input);
		}
		protected void PrepareMainElement() {
			RenderUtils.AssignAttributes(TrackBar, this.mainElement);
			this.mainElement.TabIndex = 0;
			this.mainElement.AccessKey = string.Empty;
			RenderUtils.SetVisibility(this.mainElement, TrackBar.IsClientVisible(), true);
			TrackBar.GetControlStyle().AssignToControl(this.mainElement);
			PrepareContentContainer();
		}
		protected void PrepareScale() {
			TrackBar.GetScaleStyle().AssignToControl(this.scale);
			this.behaviorStrategy.PrepareScaleContent();
		}
		protected void PrepareBarHighlight() {
			TrackBar.GetBarHighlightStyle().AssignToControl(this.barHighlight);
			if(DesignMode) {
				string imageName = IsHorizontal ?
					EditorImages.TrackBarBarHighlightHImageName : EditorImages.TrackBarBarHighlightVImageName;
				this.barHighlight.Style[HtmlTextWriterStyle.BackgroundImage] = TrackBar.GetDesignImageUrl(imageName);
				DesignModeRenderHelper.PrepareBarHighlight(this.barHighlight, IsHorizontal);
			}
		}
		protected void PrepareTrack() {
			TrackBar.GetTrackStyle().AssignToControl(this.track);
			if(DesignMode) {
				string imageName = IsHorizontal ?
					EditorImages.TrackBarTrackHImageName : EditorImages.TrackBarTrackVImageName;
				this.track.Style[HtmlTextWriterStyle.BackgroundImage] = TrackBar.GetDesignImageUrl(imageName);
				DesignModeRenderHelper.PrepareTrack(this.track, IsHorizontal);
			}
			PrepareBarHighlight();
			if(!DesignMode || (DesignMode && TrackBar.ShowDragHandles))
				PrepareDragHandles();
		}
		private static class DesignModeRenderHelper {
			public class MainTable : InternalTable {
				private Orientation orientation;
				public MainTable(Orientation orientation)
					: base() {
					this.orientation = orientation;
					Rows.Add(RenderUtils.CreateTableRow());
					if(IsHorizontal) {
						Rows[0].Cells.Add(RenderUtils.CreateTableCell());
						Rows[0].Cells.Add(RenderUtils.CreateTableCell());
						Rows[0].Cells.Add(RenderUtils.CreateTableCell());
					}
					else {
						Rows[0].Cells.Add(RenderUtils.CreateTableCell());
						Rows.Add(RenderUtils.CreateTableRow());
						Rows[1].Cells.Add(RenderUtils.CreateTableCell());
						Rows.Add(RenderUtils.CreateTableRow());
						Rows[2].Cells.Add(RenderUtils.CreateTableCell());
					}
					CellPadding = 0;
					CellSpacing = 0;
				}
				public WebControl FirstTableCell {
					get { return Rows[0].Cells[0]; }
				}
				public WebControl SecondTableCell {
					get { return Rows[IsHorizontal ? 0 : 1].Cells[IsHorizontal ? 1 : 0]; }
				}
				public HtmlTextWriterStyle SizeStyle {
					get { return IsHorizontal ? HtmlTextWriterStyle.Width : HtmlTextWriterStyle.Height; }
				}
				public WebControl ThirdTableCell {
					get { return Rows[IsHorizontal ? 0 : 2].Cells[IsHorizontal ? 2 : 0]; }
				}
				protected bool IsHorizontal {
					get { return this.orientation == Orientation.Horizontal; }
				}
				protected override void PrepareControlHierarchy() {
					Style[HtmlTextWriterStyle.Visibility] = "visible";
					FirstTableCell.Style[SizeStyle] = "0%";
					SecondTableCell.Style[SizeStyle] = "100%";
					ThirdTableCell.Style[SizeStyle] = "0%";
					this.CssClass += " " + (IsHorizontal ? EditorStyles.TBHorizontalSystemClassName :
					   EditorStyles.TBVerticalSystemClassName);
				}
			}
			public static MainTable GetMainTable(Orientation orientation) {
				return new MainTable(orientation);
			}
			public static void PrepareBarHighlight(WebControl barHighlight, bool isHorizontal) {
				barHighlight.Style[isHorizontal ? HtmlTextWriterStyle.Width : HtmlTextWriterStyle.Height] = "10px";
				if(isHorizontal) {
					barHighlight.Style[HtmlTextWriterStyle.Position] = "absolute";
					barHighlight.Style[HtmlTextWriterStyle.Left] = "inherit";
					barHighlight.Style[HtmlTextWriterStyle.Top] = "auto";
				}
				else
					barHighlight.Style[HtmlTextWriterStyle.Position] = "relative";
			}
			public static void PrepareButton(WebControl button, bool isHorizontal) {
				if(isHorizontal) {
					button.Style[HtmlTextWriterStyle.Margin] = "0px";
					button.Style[HtmlTextWriterStyle.Position] = "inherit";
				}
				else {
					button.Style[HtmlTextWriterStyle.Margin] = "auto";
					button.Style[HtmlTextWriterStyle.Left] = "inherit";
					button.Style[HtmlTextWriterStyle.Position] = "relative";
				}
				button.Style["float"] = string.Empty;
			}
			public static void PrepareDragHandle(WebControl dragHandle, bool isHorizontal) {
				if(isHorizontal)
					dragHandle.Style[HtmlTextWriterStyle.Left] = "10px";
				dragHandle.Style[HtmlTextWriterStyle.Position] = "relative";
			}
			public static void PrepareTrack(WebControl track, bool isHorizontal) {
				if(isHorizontal) {
					track.Style[HtmlTextWriterStyle.Margin] = "0px";
					track.Style[HtmlTextWriterStyle.Position] = "inherit";
				}
				else {
					track.Style[HtmlTextWriterStyle.Left] = "inherit";
					track.Style[HtmlTextWriterStyle.Margin] = "auto"; 
					track.Style[HtmlTextWriterStyle.Height] = "100%";
					track.Style[HtmlTextWriterStyle.Position] = "relative";
				}
			}
		}
		protected void ResolveImageUrl(ButtonImageProperties properties) {
			if(Page != null) {
				properties.Url = Page.ResolveClientUrl(properties.Url);
				properties.UrlDisabled = Page.ResolveClientUrl(properties.UrlDisabled);
				properties.UrlHottracked = Page.ResolveClientUrl(properties.UrlHottracked);
				properties.UrlPressed = Page.ResolveClientUrl(properties.UrlPressed);
			}
		}
	}
	internal class TrackBarTickModeStrategy {
		private 
			ASPxTrackBar trackBar = null;
			TrackBarControl trackBarControl = null;
		public TrackBarTickModeStrategy(TrackBarControl trackBarControl, ASPxTrackBar trackBar) {
			this.trackBarControl = trackBarControl;
			this.trackBar = trackBar;
		}
		public virtual void CompleteScaleContent(WebControl scale) {
			scale.Controls.Add(CreateTick(true));
			if(TrackBar.ShowSmallTicks)
				scale.Controls.Add(CreateTick(false));
		}
		public virtual void PrepareInput(KeyboardSupportInputHelper input) {
			input.Input.Attributes.Add("value",
			   HtmlConvertor.ToJSON(new object[] {
				   TrackBar.Value, 
				   TrackBar.GetValueByIndex(TrackBar.PositionEnd)
			   }, true));
		}
		public virtual void PrepareScaleContent() {
			string tickLabelText = string.Format(TrackBar.ScaleLabelFormatString, TrackBar.MinValue);
			PrepareTick(FirstTick, true, tickLabelText);
			if(TrackBar.ShowSmallTicks)
				PrepareTick(SecondTick, false, string.Empty);
		}
		protected WebControl CreateTick(bool isLargeTick) {
			WebControl tickElement = RenderUtils.CreateWebControl(HtmlTextWriterTag.Li);
			if(isLargeTick)
				tickElement.Controls.Add(RenderUtils.CreateWebControl(HtmlTextWriterTag.Span));
			tickElement.Controls.Add(new LiteralControl("&nbsp;"));
			if(TrackBar.ScalePosition == ScalePosition.Both)
				tickElement.Controls.Add(RenderUtils.CreateWebControl(HtmlTextWriterTag.Span));
			return tickElement;
		}
		protected WebControl GetLabelFromTick(WebControl tick, bool topLeftLabel) {
			return tick.Controls[topLeftLabel ? 0 : 2] as WebControl;
		}
		protected WebControl FirstTick {
			get { return TrackBarControl.Scale.Controls[0] as WebControl; }
		}
		protected void PrepareTick(WebControl tickElement, bool isLargeTick, string labelText) {
			TrackBar.GetTickStyle(isLargeTick && !TrackBar.EqualTickMarks).AssignToControl(tickElement);
			if(isLargeTick) {
				WebControl label = GetLabelFromTick(tickElement, true);
				TrackBar.GetLabelStyle(true).AssignToControl(label);
				label.Controls.Add(new LiteralControl(labelText));
				if(TrackBar.ScalePosition == ScalePosition.Both) {
					WebControl secondLabel = GetLabelFromTick(tickElement, false);
					secondLabel.Controls.Add(new LiteralControl(labelText));
					TrackBar.GetLabelStyle(false).AssignToControl(secondLabel);
				}
			}
		}
		protected WebControl SecondTick {
			get { return TrackBarControl.Scale.Controls[1] as WebControl; }
		}
		protected ASPxTrackBar TrackBar {
			get { return trackBar; }
		}
		protected TrackBarControl TrackBarControl {
			get { return trackBarControl; }
		}   
	}
	internal class TrackBarItemModeStrategy : TrackBarTickModeStrategy {
		public TrackBarItemModeStrategy(TrackBarControl trackBarControl, ASPxTrackBar trackBar) :
			base(trackBarControl, trackBar) {
		}
		public override void CompleteScaleContent(WebControl scale) {
			scale.Controls.Add(CreateItem());
		}
		public override void PrepareScaleContent() {
			string itemLabelText = string.Format(TrackBar.ScaleLabelFormatString, TrackBar.Items[0].Text);
			PrepareItem(Item, itemLabelText);
		}
		protected WebControl CreateItem() {
			WebControl itemElement = RenderUtils.CreateWebControl(HtmlTextWriterTag.Li);
			itemElement.Controls.Add(RenderUtils.CreateWebControl(HtmlTextWriterTag.Span));
			itemElement.Controls.Add(new LiteralControl("&nbsp;"));
			return itemElement;
		}
		protected WebControl Item {
			get { return base.FirstTick; }
		}
		protected void PrepareItem(WebControl itemElement, string labelText) {
			TrackBar.GetItemStyle().AssignToControl(itemElement);
			WebControl label = GetLabelFromTick(itemElement, true);
			TrackBar.GetLabelStyle(false).AssignToControl(label);
			label.Controls.Add(new LiteralControl(labelText));
		   }
	}
}
