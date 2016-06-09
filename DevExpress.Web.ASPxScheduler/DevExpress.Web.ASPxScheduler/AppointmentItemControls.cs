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
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Utils;
namespace DevExpress.Web.ASPxScheduler.Drawing {
	public abstract class AppointmentTemplateItems {
		AppointmentTextItem title;
		AppointmentTextItem startTimeText;
		AppointmentTextItem endTimeText;
		AppointmentStatusControl statusControl;
		DXCollection<AppointmentImageItem> images;
		AppearanceStyleBase aptStyle;
		protected AppointmentTemplateItems() {
			images = new DXCollection<AppointmentImageItem>();
			statusControl = CreateAppointmentStatusControl();
		}
		public AppointmentTextItem Title { get { return title; } set { title = value; } }
		public AppointmentTextItem StartTimeText { get { return startTimeText; } set { startTimeText = value; } }
		public AppointmentTextItem EndTimeText { get { return endTimeText; } set { endTimeText = value; } }
		public AppearanceStyleBase AppointmentStyle { get { return aptStyle; } set { aptStyle = value; } }
		public DXCollection<AppointmentImageItem> Images { get { return images; } set { images = value; } }
		public AppointmentStatusControl StatusControl { get { return statusControl; } set { statusControl = value; } }
		protected internal abstract AppointmentStatusControl CreateAppointmentStatusControl();
		public virtual void RefreshStyles() {
			Title.RefreshStyle(AppointmentStyle);
			StartTimeText.RefreshStyle(AppointmentStyle);
			EndTimeText.RefreshStyle(AppointmentStyle);
			foreach(AppointmentImageItem item in Images)
				item.RefreshStyle(AppointmentStyle);
		}
	}
	public class HorizontalAppointmentTemplateItems : AppointmentTemplateItems {
		AppointmentTextItem startContinueText;
		AppointmentTextItem endContinueText;
		AppointmentImageItem startContinueArrow;
		AppointmentImageItem endContinueArrow;
		WebControl startTimeClock;
		WebControl endTimeClock;
		public AppointmentTextItem StartContinueText { get { return startContinueText; } set { startContinueText = value; } }
		public AppointmentTextItem EndContinueText { get { return endContinueText; } set { endContinueText = value; } }
		public AppointmentImageItem StartContinueArrow { get { return startContinueArrow; } set { startContinueArrow = value; } }
		public AppointmentImageItem EndContinueArrow { get { return endContinueArrow; } set { endContinueArrow = value; } }
		public WebControl StartTimeClock { get { return startTimeClock; } set { startTimeClock = value; } }
		public WebControl EndTimeClock { get { return endTimeClock; } set { endTimeClock = value; } }
		protected internal override AppointmentStatusControl CreateAppointmentStatusControl() {
			return new HorizontalAppointmentStatusControl();
		}
		public override void RefreshStyles() {
			base.RefreshStyles();
			StartContinueText.RefreshStyle(AppointmentStyle);
			EndContinueText.RefreshStyle(AppointmentStyle);
			StartContinueArrow.RefreshStyle(AppointmentStyle);
			EndContinueArrow.RefreshStyle(AppointmentStyle);
		}
	}
	public class VerticalAppointmentTemplateItems : AppointmentTemplateItems {
		AppointmentHorizontalSeparatorItem horizontalSeparator;
		AppointmentTextItem description;
		public AppointmentHorizontalSeparatorItem HorizontalSeparator { get { return horizontalSeparator; } set { horizontalSeparator = value; } }
		public AppointmentTextItem Description { get { return description; } set { description = value; } }
		protected internal override AppointmentStatusControl CreateAppointmentStatusControl() {
			return new VerticalAppointmentStatusControl();
		}
		public override void RefreshStyles() {
			base.RefreshStyles();
			HorizontalSeparator.RefreshStyle(AppointmentStyle);
			Description.RefreshStyle(AppointmentStyle);
		}
	}
	#region AppointmentTemplateItem
	public abstract class AppointmentTemplateItem {
		#region Fields
		bool visible;
		AppearanceStyleBase style;
		#endregion
		protected AppointmentTemplateItem() {
			Initialize();
		}
		#region Properties
		public bool Visible { get { return visible; } set { visible = value; } }
		public AppearanceStyleBase Style { get { return style; } }
		#endregion
		protected internal virtual void Initialize() {
			visible = true;
			style = GetStyle(null);
		}
		protected internal virtual StylesHelper CreateStylesHelper() {
			ASPxScheduler control = ASPxScheduler.ActiveControl;
			StylesHelper stylesHelper = StylesHelper.Create(control.ActiveView, control.ViewInfo, control.Styles);
			return stylesHelper;
		}
		protected internal abstract AppearanceStyleBase GetStyle(AppearanceStyleBase appointmentStyle);
		internal virtual void RefreshStyle(AppearanceStyleBase appointmentStyle) {
		}
		protected void SetStyle(AppearanceStyleBase style) {
			this.style = style;
		}
	}
	#endregion
	public class AppointmentImageItem : AppointmentTemplateItem {
		ImageProperties imageProperties;
		public AppointmentImageItem(ImageProperties imageProperties) {
			this.imageProperties = imageProperties;
		}
		public ImageProperties ImageProperties { get { return imageProperties; } set { imageProperties = value; } }
		protected internal override AppearanceStyleBase GetStyle(AppearanceStyleBase appointmentStyle) {
			return new AppearanceStyleBase();
		}
	}
	#region AppointmentTextItem
	public class AppointmentTextItem : AppointmentTemplateItem, IViewInfoTextItem {
		string text = String.Empty;
		public AppointmentTextItem() {
		}
		public string Text { get { return text; } set { text = value; } }
		protected internal override AppearanceStyleBase GetStyle(AppearanceStyleBase appointmentStyle) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			StylesHelper stylesHelper = CreateStylesHelper();
			if (appointmentStyle == null)
				appointmentStyle = stylesHelper.GetAppointmentStyle();
			style.CopyFontFrom(appointmentStyle);
			style.Wrap = appointmentStyle.Wrap;
			return style;
		}
		internal override void RefreshStyle(AppearanceStyleBase appointmentStyle) {
			AppearanceStyleBase newStyle = GetStyle(appointmentStyle);
			SetStyle(newStyle);
		}
	}
	#endregion
	public class AppointmentHorizontalSeparatorItem : AppointmentTemplateItem {
		public AppointmentHorizontalSeparatorItem() {
		}
		protected internal override AppearanceStyleBase GetStyle(AppearanceStyleBase appointmentStyle) {
			StylesHelper helper = CreateStylesHelper();
			AppointmentInnerBordersStyle borderStyle = helper.GetAppointmentInnerBordersStyle();
			AppointmentHorizontalSeparatorStyle separatorStyle = helper.GetAppointmentHorizontalSeparatorStyle();
			separatorStyle.CopyFrom(borderStyle);
			if (separatorStyle.BorderTop.IsEmpty) {
				separatorStyle.BorderTop.BorderColor = helper.GetAppointmentStyle().GetBorderColorTop();
				separatorStyle.BorderTop.BorderStyle = helper.GetAppointmentStyle().GetBorderStyleTop();
				separatorStyle.BorderTop.BorderWidth = helper.GetAppointmentStyle().GetBorderWidthTop();
			}
			return separatorStyle;
		}
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region ASPxSchedulerImageControl
	public class ASPxSchedulerImageControl : ASPxInternalWebControl {
		Image image;
		ImageProperties imageProperties;
		public ASPxSchedulerImageControl(ImageProperties imageProperties) {
			this.imageProperties = imageProperties;
			this.EnableViewState = false;
		}
		protected internal ImageProperties ImageProperties { get { return imageProperties; } }
		protected internal Image Image { get { return image; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.image = RenderUtils.CreateImage();
			this.Controls.Add(image);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			bool designMode = ASPxScheduler.ActiveControl.DesignMode;
			imageProperties.AssignToControl(image, designMode);
			SchedulerWebEventHelper.AddOnDragStartEvent(image, ASPxSchedulerScripts.GetPreventOnDragStart());
			image.ID = this.ID;
		}
	}
	#endregion
	#region AppointmentClockItemControl
	public class AppointmentClockItemControl : ASPxInternalWebControl {
		#region Fields
		WebControl mainDiv;
		Image face;
		Image hourArrow;
		Image minuteArrow;
		ImageProperties faceImage;
		ImageProperties hourImage;
		ImageProperties minuteImage;
		#endregion
		public AppointmentClockItemControl(ImageProperties faceImage, ImageProperties hourImage, ImageProperties minuteImage) {
			this.faceImage = faceImage;
			this.hourImage = hourImage;
			this.minuteImage = minuteImage;
			this.EnableViewState = false;
		}
		#region Properties
		public ImageProperties FaceImage { get { return faceImage; } }
		public ImageProperties HourImage { get { return hourImage; } }
		public ImageProperties MinuteImage { get { return minuteImage; } }
		internal Image Face { get { return face; } }
		internal Image HourArrow { get { return hourArrow; } }
		internal Image MinuteArrow { get { return minuteArrow; } }
		internal WebControl MainDiv { get { return mainDiv; } }
		#endregion
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.mainDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(mainDiv);
			this.face = RenderUtils.CreateImage();
			mainDiv.Controls.Add(face);
			this.hourArrow = RenderUtils.CreateImage();
			mainDiv.Controls.Add(hourArrow);
			this.minuteArrow = RenderUtils.CreateImage();
			mainDiv.Controls.Add(minuteArrow);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			this.mainDiv.Style.Add(HtmlTextWriterStyle.Position, "relative");
			this.face.Style.Add(HtmlTextWriterStyle.Position, "absolute");
			this.hourArrow.Style.Add(HtmlTextWriterStyle.Position, "absolute");
			this.minuteArrow.Style.Add(HtmlTextWriterStyle.Position, "absolute");
			bool designMode = ASPxScheduler.ActiveControl.DesignMode;
			Unit imageSideSize = Unit.Pixel(15);
			this.faceImage.Width = imageSideSize;
			this.faceImage.Height = imageSideSize;
			this.faceImage.AssignToControl(face, designMode);
			this.hourImage.Width = imageSideSize;
			this.hourImage.Height = imageSideSize;
			this.hourImage.AssignToControl(hourArrow, designMode);
			this.minuteImage.Width = imageSideSize;
			this.minuteImage.Height = imageSideSize;
			this.minuteImage.AssignToControl(minuteArrow, designMode);
			SchedulerWebEventHelper.AddOnDragStartEvent(this.face, ASPxSchedulerScripts.GetPreventOnDragStart());
			SchedulerWebEventHelper.AddOnDragStartEvent(this.hourArrow, ASPxSchedulerScripts.GetPreventOnDragStart());
			SchedulerWebEventHelper.AddOnDragStartEvent(this.minuteArrow, ASPxSchedulerScripts.GetPreventOnDragStart());
			mainDiv.Width = face.Width;
			mainDiv.Height = face.Height;
		}
	}
	#endregion//TODO: test it!
	public abstract class AppointmentStatusControl : ASPxInternalWebControl {
		#region Fields
		Unit horizontalStatusHeight = Unit.Pixel(5);
		Unit verticalStatusWidth = Unit.Pixel(5);
		System.Drawing.Color color = System.Drawing.Color.Empty;
		int startOffset;
		int endOffset;
		readonly WebControl backGroundDiv;
		readonly WebControl foregroundDiv;
		string backGroundId;
		string foreGroundId;
		#endregion
		protected AppointmentStatusControl() {
			this.EnableViewState = false;
			this.backGroundDiv = new WebControl(HtmlTextWriterTag.Div);
			this.foregroundDiv = new WebControl(HtmlTextWriterTag.Div);
		}
		#region Properties
		protected internal int StartOffset { get { return startOffset; } set { startOffset = value; } }
		protected internal int EndOffset { get { return endOffset; } set { endOffset = value; } }
		protected internal System.Drawing.Color Color { get { return color; } set { color = value; } }
		protected internal Unit HorizontalStatusHeigh { get { return horizontalStatusHeight; } }
		protected internal Unit VerticalStatusWidth { get { return verticalStatusWidth; } }
		protected internal WebControl BackgroundDiv { get { return backGroundDiv; } }
		protected internal WebControl ForegroundDiv { get { return foregroundDiv; } }
		protected internal string BackgroundId { get { return backGroundId; } }
		protected internal string ForegroundId { get { return foreGroundId; } }
		#endregion
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			backGroundDiv.Controls.Add(foregroundDiv);
			this.Controls.Add(backGroundDiv);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareBackgroundDiv();
			PrepareForegroundDiv();
		}
		protected internal virtual void PrepareBackgroundDiv() {
			AppearanceStyleBase backgroundDivStyle = GetBackgroundDivStyle();
			backgroundDivStyle.AssignToControl(backGroundDiv);
			SetBackgroundDivSize();
			backGroundDiv.Style.Add(HtmlTextWriterStyle.Overflow, "hidden");
		}
		protected internal virtual void PrepareForegroundDiv() {
			AppearanceStyleBase foregroundDivStyle = GetForegroundDivStyle();
			foregroundDivStyle.AssignToControl(foregroundDiv);
			SetForegroundDivSize();
			foregroundDiv.Style.Add(HtmlTextWriterStyle.Position, "absolute");
			foregroundDiv.Style.Add(HtmlTextWriterStyle.Overflow, "hidden");
		}
		protected internal virtual AppearanceStyleBase GetBackgroundDivStyle() {
			StylesHelper stylesHelper = CreateStylesHelper();
			AppearanceStyleBase appointmentStyle = stylesHelper.GetAppointmentStyle();
			AppointmentInnerBordersStyle backgroundDivStyle = stylesHelper.GetAppointmentInnerBordersStyle();
			SetBackgroundDivBorders(backgroundDivStyle, appointmentStyle);
			backgroundDivStyle.BackColor = BackColor;
			backgroundDivStyle.ForeColor = ForeColor;
			return backgroundDivStyle;
		}
		protected internal virtual AppearanceStyleBase GetForegroundDivStyle() {
			StylesHelper stylesHelper = CreateStylesHelper();
			AppearanceStyleBase appointmentStyle = stylesHelper.GetAppointmentStyle();
			AppointmentInnerBordersStyle foregroundDivStyle = stylesHelper.GetAppointmentInnerBordersStyle();
			SetForegroundDivBorders(foregroundDivStyle, appointmentStyle);
			foregroundDivStyle.BackColor = Color;
			return foregroundDivStyle;
		}
		protected internal virtual StylesHelper CreateStylesHelper() {
			ASPxScheduler control = ASPxScheduler.ActiveControl;
			ISchedulerWebViewInfoBase viewInfo = control.ContainerControl.Renderer.ViewInfo;
			StylesHelper stylesHelper = StylesHelper.Create(control.ActiveView, viewInfo, control.Styles);
			return stylesHelper;
		}
		public virtual void AssignId(int aptIndex) {
			this.foreGroundId = String.Format("AptStatus{0}Fore", aptIndex.ToString());
			this.backGroundId = String.Format("AptStatus{0}Back", aptIndex.ToString());
			string idPrefix = String.Format("{0}_{1}_", ASPxScheduler.ActiveControl.ClientID, SchedulerIdHelper.AppointmentsBlockId);
			this.ForegroundDiv.Attributes.Add("id", idPrefix + foreGroundId);
			this.BackgroundDiv.Attributes.Add("id", idPrefix + backGroundId);
		}
		protected internal abstract void SetBackgroundDivBorders(AppearanceStyleBase mainDivStyle, AppearanceStyleBase appointmentStyle);
		protected internal abstract void SetForegroundDivBorders(AppearanceStyleBase mainDivStyle, AppearanceStyleBase appointmentStyle);
		protected internal abstract void SetBackgroundDivSize();
		protected internal abstract void SetForegroundDivSize();
	}
	public class HorizontalAppointmentStatusControl : AppointmentStatusControl {
		public HorizontalAppointmentStatusControl()
			: base() {
		}
		private void SetBorder(Border border, AppearanceStyleBase appointmentStyle) {
			if (border.IsEmpty) {
				border.BorderColor = appointmentStyle.GetBorderColorTop();
				border.BorderStyle = appointmentStyle.GetBorderStyleTop();
				border.BorderWidth = appointmentStyle.GetBorderWidthTop();
			}
		}
		protected internal override void SetBackgroundDivBorders(AppearanceStyleBase divStyle, AppearanceStyleBase appointmentStyle) {
			SetBorder(divStyle.BorderBottom, appointmentStyle);
			divStyle.BorderTop.BorderStyle = BorderStyle.None;
			divStyle.BorderLeft.BorderStyle = BorderStyle.None;
			divStyle.BorderRight.BorderStyle = BorderStyle.None;
		}
		protected internal override void SetForegroundDivBorders(AppearanceStyleBase divStyle, AppearanceStyleBase appointmentStyle) {
			SetBorder(divStyle.BorderLeft, appointmentStyle);
			SetBorder(divStyle.BorderRight, appointmentStyle);
			divStyle.BorderTop.BorderStyle = BorderStyle.None;
			divStyle.BorderBottom.BorderStyle = BorderStyle.None;
			if (StartOffset == 0)
				divStyle.BorderLeft.BorderStyle = BorderStyle.None;
			if (EndOffset == 0)
				divStyle.BorderRight.BorderStyle = BorderStyle.None;
		}
		protected internal override void SetBackgroundDivSize() {
			BackgroundDiv.Height = HorizontalStatusHeigh;
		}
		protected internal override void SetForegroundDivSize() {
			ForegroundDiv.Height = HorizontalStatusHeigh;
		}
	}
	public class VerticalAppointmentStatusControl : AppointmentStatusControl {
		public VerticalAppointmentStatusControl()
			: base() {
		}
		private void SetBorder(Border border, AppearanceStyleBase appointmentStyle) {
			if (border.IsEmpty) {
				border.BorderColor = appointmentStyle.GetBorderColorLeft();
				border.BorderStyle = appointmentStyle.GetBorderStyleLeft();
				border.BorderWidth = appointmentStyle.GetBorderWidthLeft();
			}
		}
		protected internal override void SetBackgroundDivBorders(AppearanceStyleBase mainDivStyle, AppearanceStyleBase appointmentStyle) {
			SetBorder(mainDivStyle.BorderRight, appointmentStyle);
			mainDivStyle.BorderLeft.BorderStyle = BorderStyle.None;
			mainDivStyle.BorderTop.BorderStyle = BorderStyle.None;
			mainDivStyle.BorderBottom.BorderStyle = BorderStyle.None;
		}
		protected internal override void SetForegroundDivBorders(AppearanceStyleBase divStyle, AppearanceStyleBase appointmentStyle) {
			SetBorder(divStyle.BorderTop, appointmentStyle);
			SetBorder(divStyle.BorderBottom, appointmentStyle);
			divStyle.BorderLeft.BorderStyle = BorderStyle.None;
			divStyle.BorderRight.BorderStyle = BorderStyle.None;
			if (StartOffset == 0)
				divStyle.BorderTop.BorderStyle = BorderStyle.None;
			if (EndOffset == 0)
				divStyle.BorderBottom.BorderStyle = BorderStyle.None;
		}
		protected internal override void SetBackgroundDivSize() {
			BackgroundDiv.Width = VerticalStatusWidth;
		}
		protected internal override void SetForegroundDivSize() {
			ForegroundDiv.Width = VerticalStatusWidth;
		}
	}
}
