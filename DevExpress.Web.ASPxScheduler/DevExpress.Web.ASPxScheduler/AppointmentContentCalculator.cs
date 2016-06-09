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
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Services;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Drawing.Internal;
namespace DevExpress.Web.ASPxScheduler.Drawing {
	#region ImagePropertiesCollection
	public class ImagePropertiesCollection : DXCollection<ImageProperties> {
		protected internal virtual ImageProperties GetImageProperties(int imageIndex) {
			if ((imageIndex < 0) || (imageIndex >= this.Count))
				return null;
			else
				return this[imageIndex];
		}
	}
	#endregion
	#region AppointmentViewInfoItemCollection
	public class AppointmentItemCollection : DXCollection<AppointmentTemplateItem> {
	}
	#endregion
	public interface IAppointmentImageProvider {
		ImageProperties GetAppointmentImage(AppointmentImageType type);
	}
	#region AppointmentImageProvider
	public class AppointmentImageProvider : AppointmentImageProviderCore, IAppointmentImageProvider {
		protected internal new ImagePropertiesCollection DefaultAppointmentImages { get { return (ImagePropertiesCollection)base.DefaultAppointmentImages; } }
		public AppointmentImageProvider(ImagePropertiesCollection defaultImages)
			: base(defaultImages) {
		}
		public virtual ImageProperties GetAppointmentImage(AppointmentImageType type) {
			int index = GetAppointmentImageIndex(type);
			return index >= 0 ? DefaultAppointmentImages[index] : null;
		}
	}
	#endregion
	public class AppointmentImageInfo : AppointmentImageInfoCore {
		ImageProperties imageProperties;
		public ImageProperties ImageProperties { get { return imageProperties; } set { imageProperties = value; } }
	}
	public class AppointmentImageInfoCollection : AppointmentImageInfoCoreCollection<AppointmentImageInfo> {
		#region Events
		ImagePropertiesCollection images;
		#endregion
		public AppointmentImageInfoCollection(AppointmentImageProvider provider)
			: base(provider) {
		}
		#region Properties
		public ImagePropertiesCollection Images { get { return images; } set { images = value; } }
		public ImagePropertiesCollection InnerImages { get { return images != null ? images : ImageProvider.DefaultAppointmentImages; } }
		protected internal new AppointmentImageProvider ImageProvider { get { return (AppointmentImageProvider)base.ImageProvider; } }
		#endregion
		protected internal override void SetImage(AppointmentImageType type, AppointmentImageInfo imageInfo) {
			ImageProperties imageProperties = ImageProvider.GetAppointmentImage(type);
			imageInfo.ImageProperties = imageProperties;
		}
	}
	public abstract class AppointmentContentLayoutCalculator : AppointmentContentLayoutCalculatorCore<AppointmentItemCollection, AppointmentImageInfoCollection> {
		int contentHorizontalGap = 2;
		readonly AppointmentImageProvider appointmentImageProvider;
		#region Events
		#region AppointmentViewInfoCustomizing
		AppointmentViewInfoCustomizingEventHandler onAppointmentViewInfoCustomizing;
		public event AppointmentViewInfoCustomizingEventHandler AppointmentViewInfoCustomizing { add { onAppointmentViewInfoCustomizing += value; } remove { onAppointmentViewInfoCustomizing -= value; } }
		protected internal virtual void RaiseAppointmentViewInfoCustomizing(AppointmentViewInfo aptViewInfo) {
			if (onAppointmentViewInfoCustomizing != null) {
				AppointmentViewInfoCustomizingEventArgs args = new AppointmentViewInfoCustomizingEventArgs(aptViewInfo);
				onAppointmentViewInfoCustomizing(this, args);
			}
		}
		#endregion
		#region InitAppointmentImages
		AppointmentImagesEventHandler onInitAppointmentImages;
		public event AppointmentImagesEventHandler InitAppointmentImages { add { onInitAppointmentImages += value; } remove { onInitAppointmentImages -= value; } }
		protected internal override void RaiseInitAppointmentImages(IAppointmentViewInfo viewInfo, AppointmentImageInfoCollection imageInfos) {
			if (onInitAppointmentImages != null) {
				AppointmentImagesEventArgs args = new AppointmentImagesEventArgs(viewInfo, imageInfos);
				onInitAppointmentImages(this, args);
			}
		}
		#endregion
		#endregion
		protected AppointmentContentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo)
			: base(viewInfo) {
			appointmentImageProvider = CreateAppointmentImageProvider();
		}
		#region Properties
		protected internal AppointmentImageProvider AppointmentImageProvider { get { return appointmentImageProvider; } }
		protected internal new ISchedulerWebViewInfoBase ViewInfo { get { return (ISchedulerWebViewInfoBase)base.ViewInfo; } }
		protected internal ASPxScheduler Control { get { return ViewInfo.View.Control; } }
		protected internal int ContentHorizontalGap { get { return contentHorizontalGap; } }
		protected internal AppointmentDisplayOptions AppointmentDisplayOptions { get { return ViewInfo.AppointmentDisplayOptions; } }
		protected internal virtual AppointmentStatusDisplayType StatusDisplayType { get { return AppointmentDisplayOptions.StatusDisplayType; } }
		#endregion
		protected internal abstract void CalculateTextItems(AppointmentControl aptControl);
		protected internal override IViewInfoTextItem CreateTextItemInstance() {
			return new AppointmentTextItem();
		}
		protected internal override IAppointmentFormatStringService GetFormatStringProvider() {
			return (IAppointmentFormatStringService)ASPxScheduler.ActiveControl.GetService(typeof(IAppointmentFormatStringService));
		}
		protected internal override IViewInfoTextItem CreateClockTextItemInstance() {
			AppointmentTextItem item = new AppointmentTextItem();
			item.Style.Wrap = DefaultBoolean.True;
			return item;
		}
		protected internal virtual AppointmentImageProvider CreateAppointmentImageProvider() {
			ImagePropertiesCollection defaulImages = GetDefaultAppointmentImages();
			return new AppointmentImageProvider(defaulImages);
		}
		public virtual void CalculateContentLayout(AppointmentControlCollection aptControls) {
			int count = aptControls.Count;
			for (int i = 0; i < count; i++)
				CalculateContentLayout(aptControls[i]);
		}
		protected internal virtual void CalculateContentLayout(AppointmentControl aptControl) {
			AppointmentViewInfo aptViewInfo = aptControl.ViewInfo;
			CalculateViewInfoProperties(aptViewInfo);
			RaiseAppointmentViewInfoCustomizing(aptViewInfo);
			CalculateTemplateItems(aptControl);
			aptControl.TemplateItems.RefreshStyles();
		}
		protected internal virtual void CalculateTemplateItems(AppointmentControl aptControl) {
			CalculateAppointmentStyleItem(aptControl);
			CalculateStatusItem(aptControl);
			CalculateImageItems(aptControl);
			CalculateTextItems(aptControl);
			CalculateClockItems(aptControl);
		}
		protected internal virtual void CalculateImageItems(AppointmentControl aptControl) {
			AppointmentItemCollection imageItems = CreateAppointmentViewInfoImageItems(aptControl.ViewInfo);
			aptControl.TemplateItems.Images.AddRange(imageItems);
		}
		protected internal virtual void CalculateStatusItem(AppointmentControl aptControl) {
			AppointmentStatusControl statusItem = aptControl.TemplateItems.StatusControl;
			CalculateStatusOffset(aptControl);
			AppointmentViewInfo aptViewInfo = aptControl.ViewInfo;
			statusItem.Color = aptViewInfo.StatusColor;
			statusItem.BackColor = aptViewInfo.StatusBackgroundColor;
			statusItem.Visible = (aptViewInfo.StatusDisplayType != AppointmentStatusDisplayType.Never);
		}
		protected internal virtual void CalculateAppointmentStyleItem(AppointmentControl aptControl) {
			CalculateAppointmentStyleBorders(aptControl.ViewInfo);
			aptControl.TemplateItems.AppointmentStyle = aptControl.ViewInfo.AppointmentStyle;
		}
		protected internal virtual void CalculateClockItems(AppointmentControl aptControl) {
			AppointmentTemplateItems items = aptControl.TemplateItems;
			AppointmentViewInfo aptViewInfo = aptControl.ViewInfo;
			items.StartTimeText = (AppointmentTextItem)CreateStartClockTextItem(aptViewInfo);
			items.EndTimeText = (AppointmentTextItem)CreateEndClockTextItem(aptViewInfo);
			items.StartTimeText.Visible = !aptViewInfo.ShowTimeAsClock && aptViewInfo.ShowStartTime;
			items.EndTimeText.Visible = !aptViewInfo.ShowTimeAsClock && aptViewInfo.ShowEndTime;
		}
		protected internal virtual void CalculateViewInfoProperties(AppointmentViewInfo aptViewInfo) {
			ContentCalculatorHelper.CalculateViewInfoOptions(aptViewInfo);
			IAppointmentStatus status = Control.GetStatus(aptViewInfo.Appointment.StatusKey);
			aptViewInfo.Status = status;
			aptViewInfo.AppointmentStyle = GetAppointmentStyle(aptViewInfo);
			aptViewInfo.StatusColor = status.GetColor();
		}
		protected internal virtual AppearanceStyleBase GetAppointmentStyle(AppointmentViewInfo aptViewInfo) {
			StylesHelper stylesHelper = CreateStylesHelper();
			AppearanceStyleBase appointmentStyle = stylesHelper.GetAppointmentStyle();
			AppearanceStyleBase result = new AppearanceStyleBase();
			result.BackColor = Control.GetLabelColor(aptViewInfo.Appointment.LabelKey);
			result.CopyFrom(appointmentStyle);
			return result;
		}
		protected internal virtual StylesHelper CreateStylesHelper() {
			ASPxScheduler control = ASPxScheduler.ActiveControl;
			StylesHelper stylesHelper = StylesHelper.Create(control.ActiveView, control.ViewInfo, control.Styles);
			return stylesHelper;
		}
		protected internal virtual void CalculateAppointmentStyleBorders(AppointmentViewInfo viewInfo) {
			AppearanceStyleBase appointmentStyle = viewInfo.AppointmentStyle;
			if (!viewInfo.HasLeftBorder)
				appointmentStyle.BorderLeft.BorderStyle = BorderStyle.None;
			if (!viewInfo.HasTopBorder)
				appointmentStyle.BorderTop.BorderStyle = BorderStyle.None;
			if (!viewInfo.HasRightBorder)
				appointmentStyle.BorderRight.BorderStyle = BorderStyle.None;
			if (!viewInfo.HasBottomBorder)
				appointmentStyle.BorderBottom.BorderStyle = BorderStyle.None;
		}
		protected internal virtual void CalculateStatusOffset(AppointmentControl aptControl) {
			AppointmentStatusDisplayType statusDisplayType = aptControl.ViewInfo.StatusDisplayType;
			AppointmentStatusControl statusControl = aptControl.TemplateItems.StatusControl;
			switch (statusDisplayType) {
				case AppointmentStatusDisplayType.Bounds:
					statusControl.StartOffset = 0;
					statusControl.EndOffset = 0;
					break;
				case AppointmentStatusDisplayType.Time:
					TimeInterval aptInterval = aptControl.ViewInfo.AppointmentInterval;
					TimeInterval viewInfoInterval = aptControl.ViewInfo.Interval;
					statusControl.StartOffset = AppointmentTimeScaleHelper.CalculateStartOffset(aptInterval.Start, viewInfoInterval);
					statusControl.EndOffset = AppointmentTimeScaleHelper.CalculateEndOffset(aptInterval.End, viewInfoInterval);
					break;
			}
		}
		protected internal override AppointmentImageInfoCollection CreateImageInfoCollection() {
			AppointmentImageInfoCollection imageInfos = new AppointmentImageInfoCollection(AppointmentImageProvider);
			imageInfos.Images = AppointmentImageProvider.DefaultAppointmentImages;
			return imageInfos;
		}
		protected internal override void FillAppointmentViewInfoImageItems(AppointmentImageInfoCollection imageInfos, AppointmentItemCollection imageItems) {
			int count = imageInfos.Count;
			for (int i = 0; i < count; i++) {
				if (imageInfos[i].Visible) {
					ImageProperties imageProperties = GetAppointmentImageProperties(imageInfos, i);
					if (imageProperties != null)
						imageItems.Add(new AppointmentImageItem(imageProperties));
				}
			}
		}
		protected internal virtual ImageProperties GetAppointmentImageProperties(AppointmentImageInfoCollection imageInfos, int index) {
			AppointmentImageInfo imageInfo = imageInfos[index];
			ImageProperties imageProperties = imageInfo.ImageProperties;
			if (imageProperties == null)
				imageProperties = GetImagePropertiesFromInnerImages(imageInfos, imageInfo.ImageIndex);
			return imageProperties;
		}
		protected internal virtual ImageProperties GetImagePropertiesFromInnerImages(AppointmentImageInfoCollection imageInfos, int imageIndex) {
			return imageInfos.InnerImages.GetImageProperties(imageIndex);
		}
		protected internal virtual ImagePropertiesCollection GetDefaultAppointmentImages() {
			ImagePropertiesCollection images = new ImagePropertiesCollection();
			images.Add(GetImageProperties(AppointmentImages.DayClockName));
			images.Add(GetImageProperties(AppointmentImages.NightClockName));
			images.Add(GetImageProperties(AppointmentImages.RecurrenceName));
			images.Add(GetImageProperties(AppointmentImages.NoRecurrenceName));
			images.Add(GetImageProperties(AppointmentImages.ReminderName));
			return images;
		}
		protected internal virtual ImageProperties GetImageProperties(string imageName) {
			ImageProperties imageProperties = Control.Images.Appointment.GetImageProperties(Control.Page, imageName);
			imageProperties.AlternateText = imageName;
			return imageProperties;
		}
	}
	public class HorizontalAppointmentContentLayoutCalculator : AppointmentContentLayoutCalculator {
		public HorizontalAppointmentContentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo)
			: base(viewInfo) {
		}
		protected internal new HorizontalAppointmentContentCalculatorHelper ContentCalculatorHelper { get { return (HorizontalAppointmentContentCalculatorHelper)base.ContentCalculatorHelper; } }
		protected internal override void CalculateTemplateItems(AppointmentControl aptControl) {
			base.CalculateTemplateItems(aptControl);
			CalculateContinueItems(aptControl);
		}
		protected internal virtual void CalculateContinueItems(AppointmentControl aptControl) {
			CreateContinueItems(aptControl);
			CalculateContinueItemsVisibility(aptControl);
		}
		protected internal virtual void CreateContinueItems(AppointmentControl aptControl) {
			HorizontalAppointmentTemplateItems templateItems = (HorizontalAppointmentTemplateItems)aptControl.TemplateItems;
			templateItems.StartContinueArrow = CreateStartContinueImageItem();
			templateItems.EndContinueArrow = CreateEndContinueImageItem();
			templateItems.StartContinueText = CreateStartContinueTextItem(aptControl.ViewInfo);
			templateItems.EndContinueText = CreateEndContinueTextItem(aptControl.ViewInfo);
		}
		protected internal virtual void CalculateContinueItemsVisibility(AppointmentControl aptControl) {
			HorizontalAppointmentTemplateItems templateItems = (HorizontalAppointmentTemplateItems)aptControl.TemplateItems;
			AppointmentViewInfoOptions options = aptControl.ViewInfo.Options;			
			templateItems.StartContinueArrow.Visible = options.StartContinueItemDisplayType != AppointmentContinueArrowDisplayType.Never;
			templateItems.EndContinueArrow.Visible = options.EndContinueItemDisplayType != AppointmentContinueArrowDisplayType.Never;
			templateItems.StartContinueText.Visible = ContentCalculatorHelper.ShouldShowContinueTextItems(options.StartContinueItemDisplayType);
			templateItems.EndContinueText.Visible = ContentCalculatorHelper.ShouldShowContinueTextItems(options.EndContinueItemDisplayType);
		}
		protected internal override void CalculateClockItems(AppointmentControl aptControl) {
			base.CalculateClockItems(aptControl);
			HorizontalAppointmentTemplateItems items = (HorizontalAppointmentTemplateItems)aptControl.TemplateItems;
			AppointmentViewInfo aptViewInfo = aptControl.ViewInfo;
			items.StartTimeClock = CreateClockImageItem(aptViewInfo.AppointmentInterval.Start);
			items.EndTimeClock = CreateClockImageItem(aptViewInfo.AppointmentInterval.End);
			items.StartTimeClock.Visible = aptViewInfo.ShowTimeAsClock && aptViewInfo.Options.ShowStartTime;
			items.EndTimeClock.Visible = aptViewInfo.ShowTimeAsClock && aptViewInfo.Options.ShowEndTime;
		}
		protected internal virtual AppointmentImageItem CreateStartContinueImageItem() {
			ImageProperties imageProperties = GetImageProperties(AppointmentImages.StartArrowName);
			return new AppointmentImageItem(imageProperties);
		}
		protected internal virtual AppointmentImageItem CreateEndContinueImageItem() {
			ImageProperties imageProperties = GetImageProperties(AppointmentImages.EndArrowName);
			return new AppointmentImageItem(imageProperties);
		}
		protected internal virtual AppointmentTextItem CreateStartContinueTextItem(AppointmentViewInfo aptViewInfo) {
			AppointmentTextItem item = CreateContinueTextItemInstance();
			item.Text = GetStartContinueItemText(aptViewInfo);
			return item;
		}
		protected internal virtual AppointmentTextItem CreateEndContinueTextItem(AppointmentViewInfo aptViewInfo) {
			AppointmentTextItem item = CreateContinueTextItemInstance();
			item.Text = GetEndContinueItemText(aptViewInfo);
			return item;
		}
		protected internal virtual AppointmentTextItem CreateContinueTextItemInstance() {
			AppointmentTextItem item = new AppointmentTextItem();
			item.Style.Wrap = DefaultBoolean.False;
			return item;
		}
		protected internal override AppointmentContentCalculatorHelper CreateContentLayoutCalculatorHelper() {
			return new HorizontalAppointmentContentCalculatorHelper(ViewInfo, StatusDisplayType, FormatProvider);
		}
		protected internal override void CalculateTextItems(AppointmentControl aptControl) {
			AppointmentItemCollection textItems = CreateAppointmentViewInfoTextItems(aptControl.ViewInfo);
			aptControl.TemplateItems.Title = (AppointmentTextItem)GetTitleTextItem(textItems);
		}
		protected internal virtual WebControl CreateClockImageItem(DateTime time) {
			ImageProperties clockImage = GetClockFaceImage(time);
			ImageProperties hourArrowImage = GetHourArrowImage(time);
			ImageProperties minuteImage = GetMinuteArrowImage(time);
			AppointmentClockItemControl clockControl = new AppointmentClockItemControl(clockImage, hourArrowImage, minuteImage);
			return clockControl;
		}
		protected internal virtual ImageProperties GetClockFaceImage(DateTime time) {
			int imageIndex = CalculateClockImageIndex(time);
			return AppointmentImageProvider.DefaultAppointmentImages[imageIndex];
		}
		protected internal virtual ImageProperties GetHourArrowImage(DateTime time) {
			int imageIndex = CalculateHourArrowImageIntex(time);
			if (IsNightTime(time))
				return Control.Images.NightHours.GetImageProperties(Control.Page, ClockImageNames.NightHours[imageIndex]);
			else
				return Control.Images.DayHours.GetImageProperties(Control.Page, ClockImageNames.DayHours[imageIndex]);
		}
		protected internal virtual ImageProperties GetMinuteArrowImage(DateTime time) {
			int imageIndex = CalculateMinuteArrowImageIntex(time);
			if (IsNightTime(time))
				return Control.Images.NightMinutes.GetImageProperties(Control.Page, ClockImageNames.NightMinutes[imageIndex]);
			else
				return Control.Images.DayMinutes.GetImageProperties(Control.Page, ClockImageNames.DayMinutes[imageIndex]);
		}
		protected internal virtual int CalculateHourArrowImageIntex(DateTime time) {
			int hour = time.Hour;
			int imageIndex = hour < 12 ? hour : hour - 12;
			return imageIndex;
		}
		protected internal virtual int CalculateMinuteArrowImageIntex(DateTime time) {
			int minute = time.Minute;
			int imageIndex = minute / 5;
			return imageIndex;
		}	   
		protected internal virtual string GetStartContinueItemText(IAppointmentViewInfo aptViewInfo) {
			return ContentCalculatorHelper.GetStartContinueItemText(aptViewInfo);
		}
		protected internal virtual string GetEndContinueItemText(IAppointmentViewInfo aptViewInfo) {
			return ContentCalculatorHelper.GetEndContinueItemText(aptViewInfo);
		}
	}
	public class WeekViewApointmnetContentLayoutCalculator : HorizontalAppointmentContentLayoutCalculator {
		public WeekViewApointmnetContentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo)
			: base(viewInfo) {
		}
		protected internal override IViewInfoTextItem CreateClockTextItemInstance() {
			AppointmentTextItem item = new AppointmentTextItem();
			item.Style.Wrap = DefaultBoolean.False;
			return item;
		}
	}
	#region DayViewLongAppointmentContentLayoutCalculator
	public class DayViewLongAppointmentContentLayoutCalculator : HorizontalAppointmentContentLayoutCalculator {
		public DayViewLongAppointmentContentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo)
			: base(viewInfo) {
		}
		protected internal override AppointmentStatusDisplayType StatusDisplayType {
			get {
				return ((DayViewAppointmentDisplayOptions)AppointmentDisplayOptions).AllDayAppointmentsStatusDisplayType;
			}
		}
	}
	#endregion
	#region DayViewShortAppointmentContentLayoutCalculator
	public class DayViewShortAppointmentContentLayoutCalculator : VerticalAppointmentContentLayoutCalculator {
		public DayViewShortAppointmentContentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo)
			: base(viewInfo) {
		}
	}
	#endregion
	public class VerticalAppointmentContentLayoutCalculator : AppointmentContentLayoutCalculator {
		public VerticalAppointmentContentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo)
			: base(viewInfo) {
		}
		protected internal new VerticalAppointmentContentCalculatorHelper ContentCalculatorHelper { get { return (VerticalAppointmentContentCalculatorHelper)base.ContentCalculatorHelper; } }
		protected internal override AppointmentContentCalculatorHelper CreateContentLayoutCalculatorHelper() {
			return new VerticalAppointmentContentCalculatorHelper(ViewInfo, StatusDisplayType, FormatProvider);
		}
		protected internal override void CalculateTemplateItems(AppointmentControl aptControl) {
			base.CalculateTemplateItems(aptControl);
			CalculateHorizontalSeparatorItem(aptControl);
		}
		protected internal virtual void CalculateHorizontalSeparatorItem(AppointmentControl aptControl) {
			VerticalAppointmentTemplateItems templateItems = (VerticalAppointmentTemplateItems)aptControl.TemplateItems;
			templateItems.HorizontalSeparator = new AppointmentHorizontalSeparatorItem();
			XtraSchedulerDebug.Assert(templateItems.Description != null);
			if (String.IsNullOrEmpty(templateItems.Description.Text))
				templateItems.HorizontalSeparator.Visible = false;
		}
		protected internal override void CalculateTextItems(AppointmentControl aptControl) {
			AppointmentItemCollection aptTextItems = CreateAppointmentViewInfoTextItems(aptControl.ViewInfo);
			VerticalAppointmentTemplateItems templateItems = (VerticalAppointmentTemplateItems)aptControl.TemplateItems;
			templateItems.Title = (AppointmentTextItem)GetTitleTextItem(aptTextItems);
			AppointmentTextItem description = (AppointmentTextItem)GetDescriptionTextItem(aptTextItems);
			templateItems.Description = description;
		}
	}
	public class TimelineViewAppointmentContentLayoutCalculator : HorizontalAppointmentContentLayoutCalculator {
		public TimelineViewAppointmentContentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo)
			: base(viewInfo) {
		}
		protected internal override AppointmentContentCalculatorHelper CreateContentLayoutCalculatorHelper() {
			return new TimelineAppointmentContentLayoutCalculatorHelper(ViewInfo, StatusDisplayType, FormatProvider);
		}
	}
}
