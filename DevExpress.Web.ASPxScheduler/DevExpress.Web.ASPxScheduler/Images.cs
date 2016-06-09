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
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Localization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxScheduler {
	public class ASPxSchedulerImages : ImagesBase {
		#region Fields
		internal const string ResourceImagesPath = "Images.";
		internal const string SmartTagName = "SmartTag";
		internal const string TimeMarkerName = "TimeMarker";
		ClockImages dayHours;
		ClockImages dayMinutes;
		ClockImages nightHours;
		ClockImages nightMinutes;
		ToolTipImages toolTip;
		ResourceNavigatorImages resourceNavigator;
		ViewNavigatorImages viewNavigator;
		AppointmentImages appointment;
		AppointmentInplaceEditorImages inplaceEditor;
		MenuImages menu;
		NavigationButtonImages navigationButton;
		MoreButtonImages moreButton;
		StatusInfoImages statusInfo;
		HeaderButtonImageProperties formCloseButton;
		SchedulerFormEditorsImages formEditors;
		ButtonImageProperties smartTag;
		ImageProperties timeIndicator;
		#endregion
		public ASPxSchedulerImages(ISkinOwner owner)
			: base(owner) {
			this.dayHours = new DayHoursClockImages(owner);
			this.dayMinutes = new DayMinutesClockImages(owner);
			this.nightHours = new NightHoursClockImages(owner);
			this.nightMinutes = new NightMinutesClockImages(owner);
			this.toolTip = new ToolTipImages(owner);
			this.resourceNavigator = new ResourceNavigatorImages(owner);
			this.viewNavigator = new ViewNavigatorImages(owner);
			this.appointment = new AppointmentImages(owner);
			this.inplaceEditor = new AppointmentInplaceEditorImages(owner);
			this.menu = new MenuImages(owner);
			this.navigationButton = new NavigationButtonImages(owner);
			this.moreButton = new MoreButtonImages(owner);
			this.smartTag = new ButtonImageProperties(this);
			this.timeIndicator = new ImageProperties(this);
			this.statusInfo = new StatusInfoImages(owner);
			this.formCloseButton = new HeaderButtonImageProperties(this);
			this.formEditors = new SchedulerFormEditorsImages(owner);
		}
		#region Properties
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ClockImages DayHours { get { return dayHours; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ClockImages DayMinutes { get { return dayMinutes; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ClockImages NightHours { get { return nightHours; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ClockImages NightMinutes { get { return nightMinutes; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ToolTipImages ToolTip { get { return toolTip; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public AppointmentImages Appointment { get { return appointment; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public AppointmentInplaceEditorImages AppointmentInplaceEditor { get { return inplaceEditor; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ResourceNavigatorImages ResourceNavigator { get { return resourceNavigator; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ViewNavigatorImages ViewNavigator { get { return viewNavigator; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public MenuImages Menu { get { return menu; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public NavigationButtonImages NavigationButton { get { return navigationButton; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public MoreButtonImages MoreButton { get { return moreButton; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties SmartTag { get { return smartTag; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties TimeIndicator { get { return timeIndicator; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public StatusInfoImages StatusInfo { get { return statusInfo; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public HeaderButtonImageProperties FormCloseButton { get { return formCloseButton; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public SchedulerFormEditorsImages FormEditors { get { return formEditors; } }
		#endregion
		protected override Type GetResourceType() {
			return typeof(ASPxScheduler);
		}
		protected override string GetResourceImagePath() {
			return ResourceImagesPath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ResourceImagesPath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { DayHours, DayMinutes, NightHours, NightMinutes,
					ToolTip, ResourceNavigator, 
					ViewNavigator, Appointment, AppointmentInplaceEditor, NavigationButton, StatusInfo, FormCloseButton, FormEditors, TimeIndicator });
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(SmartTagName, ImageFlags.IsPng, 6, 9, String.Empty,
				typeof(ButtonImageProperties), SmartTagName));
			list.Add(new ImageInfo(TimeMarkerName, ImageFlags.IsPng, 3, 5, String.Empty,
				typeof(ImageProperties), TimeMarkerName));
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxScheduler.SchedulerSpriteCssResourceName;
		}
		public virtual ImageProperties GetStatusInfoImage(Page page) {
			ImageProperties image = new ImageProperties();
			image.CopyFrom(this.statusInfo.GetImageProperties(page, StatusInfoImages.InfoImageName));
			image.CopyFrom(this.statusInfo.Info);
			return image;
		}
		public virtual ImageProperties GetInplaceEditorCancelImage(Page page) {
			ImageProperties image = new ImageProperties();
			image.CopyFrom(this.inplaceEditor.GetImageProperties(page, AppointmentInplaceEditorImages.CancelName));
			image.CopyFrom(this.inplaceEditor.Cancel);
			return image;
		}
		public virtual ImageProperties GetInplaceEditorSaveImage(Page page) {
			ImageProperties image = new ImageProperties();
			image.CopyFrom(this.inplaceEditor.GetImageProperties(page, AppointmentInplaceEditorImages.SaveName));
			image.CopyFrom(this.inplaceEditor.Save);
			return image;
		}
		public virtual ImageProperties GetInplaceEditorEditFormImage(Page page) {
			ImageProperties image = new ImageProperties();
			image.CopyFrom(this.inplaceEditor.GetImageProperties(page, AppointmentInplaceEditorImages.EditFormName));
			image.CopyFrom(this.inplaceEditor.EditForm);
			return image;
		}
		public virtual ImageProperties GetWarningImage(Page page) {
			ImageProperties image = new ImageProperties();
			image.CopyFrom(this.statusInfo.GetImageProperties(page, StatusInfoImages.WarningImageName));
			image.CopyFrom(this.statusInfo.Warning);
			return image;
		}
		public override void CopyFrom(ImagesBase source) {
			base.CopyFrom(source);
			ASPxSchedulerImages schedulerImages = source as ASPxSchedulerImages;
			if (schedulerImages != null) {
				DayHours.CopyFrom(schedulerImages.DayHours);
				DayMinutes.CopyFrom(schedulerImages.DayMinutes);
				NightHours.CopyFrom(schedulerImages.NightHours);
				NightMinutes.CopyFrom(schedulerImages.NightMinutes);
				ToolTip.CopyFrom(schedulerImages.ToolTip);
				Appointment.CopyFrom(schedulerImages.Appointment);
				AppointmentInplaceEditor.CopyFrom(schedulerImages.AppointmentInplaceEditor);
				ResourceNavigator.CopyFrom(schedulerImages.ResourceNavigator);
				ViewNavigator.CopyFrom(schedulerImages.ViewNavigator);
				Menu.CopyFrom(schedulerImages.Menu);
				NavigationButton.CopyFrom(schedulerImages.NavigationButton);
				MoreButton.CopyFrom(schedulerImages.MoreButton);
				SmartTag.CopyFrom(schedulerImages.SmartTag);
				TimeIndicator.CopyFrom(schedulerImages.TimeIndicator);
				StatusInfo.CopyFrom(schedulerImages.StatusInfo);
				FormCloseButton.CopyFrom(schedulerImages.FormCloseButton);
				FormEditors.CopyFrom(schedulerImages.FormEditors);
			}
		}
	}
	public class ToolTipImages : ImagesBaseNoLoadingPanel {
		#region Fields
		internal const string Category = "Tooltip";
		internal const string BottomRightName = "BottomRight";
		internal const string BottomLeftName = "BottomLeft";
		internal const string TopLeftName = "TopLeft";
		internal const string TopRightName = "TopRight";
		internal const string BottomStemName = "BottomStem";
		internal const string EmptyName = "Empty";
		#endregion
		public ToolTipImages(ISkinOwner owner)
			: base(owner) {
		}
		#region Properties
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties TopLeftCorner { get { return GetImage(TopLeftName); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties TopRightCorner { get { return GetImage(TopRightName); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties BottomLeftCorner { get { return GetImage(BottomLeftName); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties BottomRightCorner { get { return GetImage(BottomStemName); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties BottomStem { get { return GetImage(BottomStemName); } }
		protected override bool KeepDefaultSizes { get { return true; } }
		#endregion
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			ImageInfo[] imageInfos = new ImageInfo[] {
				new ImageInfo(BottomStemName, ImageFlags.IsPng, 40, 53, BottomStemName), 
				new ImageInfo(BottomLeftName, ImageFlags.IsPng, 7, 7, BottomLeftName), 
				new ImageInfo(BottomRightName, ImageFlags.IsPng, 7, 7, BottomRightName), 
				new ImageInfo(TopLeftName, ImageFlags.IsPng, 7, 7, TopLeftName), 
				new ImageInfo(TopRightName, ImageFlags.IsPng, 7, 7, TopRightName), 
				new ImageInfo(EmptyName, ImageFlags.Empty, 1, 1, EmptyName) 
			};
			for (int i = imageInfos.Length - 1; i >= 0; i--)
				list.Add(imageInfos[i]);
		}
		protected override Type GetResourceType() {
			return typeof(ASPxScheduler);
		}
		protected override string GetResourceImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxScheduler.SchedulerSpriteCssResourceName;
		}
		protected override string GetImageCategory() {
			return Category;
		}
	}
	public class NavigationButtonImages : ImagesBaseNoLoadingPanel {
		internal const string Category = "NavigationButton";
		internal const string ForwardName = "Forward";
		internal const string BackwardName = "Backward";
		public NavigationButtonImages(ISkinOwner owner)
			: base(owner) {
		}
		#region Properties
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ButtonImageProperties Forward {
			get { return (ButtonImageProperties)GetImageBase(ForwardName); }
		}
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ButtonImageProperties Backward {
			get { return (ButtonImageProperties)GetImageBase(BackwardName); }
		}
		#endregion
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			ImageFlags flags = ImageFlags.IsPng | ImageFlags.HasDisabledState | ImageFlags.HasHottrackState;
			list.Add(new ImageInfo(BackwardName, flags, 19, 41, "<",
				typeof(ButtonImageProperties), BackwardName)); 
			list.Add(new ImageInfo(ForwardName, flags, 19, 41, ">",
				typeof(ButtonImageProperties), ForwardName)); 
		}
		protected override Type GetResourceType() {
			return typeof(ASPxScheduler);
		}
		protected override string GetResourceImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetImageCategory() {
			return Category;
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxScheduler.SchedulerSpriteCssResourceName;
		}
	}
	public class StatusInfoImages : ImagesBaseNoLoadingPanel {
		internal const string Category = "StatusInfo";
		internal const string ErrorImageName = "Error";
		internal const string WarningImageName = "Warning";
		internal const string InfoImageName = "Info";
		public StatusInfoImages(ISkinOwner owner)
			: base(owner) {
		}
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ImageProperties Error { get { return GetImage(ErrorImageName); } }
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ImageProperties Warning { get { return GetImage(WarningImageName); } }
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ImageProperties Info { get { return GetImage(InfoImageName); } }
		protected override bool KeepDefaultSizes { get { return true; } }
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			list.Add(new ImageInfo(ErrorImageName, ImageFlags.IsPng, Unit.Pixel(48),
				ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_Error), ErrorImageName));
			list.Add(new ImageInfo(WarningImageName, ImageFlags.IsPng, Unit.Pixel(48),
				ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_Warning), WarningImageName));
			list.Add(new ImageInfo(InfoImageName, ImageFlags.IsPng, Unit.Pixel(48),
				ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_Info), InfoImageName));
		}
		protected override Type GetResourceType() {
			return typeof(ASPxScheduler);
		}
		protected override string GetResourceImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetImageCategory() {
			return Category;
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxScheduler.SchedulerSpriteCssResourceName;
		}
	}
	public class MoreButtonImages : ImagesBaseNoLoadingPanel {
		internal const string Category = "MoreButton";
		internal const string TopName = "Top";
		internal const string BottomName = "Bottom";
		public MoreButtonImages(ISkinOwner owner)
			: base(owner) {
		}
		#region Properties
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ButtonImageProperties Top {
			get { return (ButtonImageProperties)GetImageBase(TopName); }
		}
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ButtonImageProperties Bottom {
			get { return (ButtonImageProperties)GetImageBase(BottomName); }
		}
		#endregion
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			ImageFlags flags = ImageFlags.IsPng | ImageFlags.HasHottrackState;
			list.Add(new ImageInfo(TopName, flags, 21, 17, "<", typeof(ButtonImageProperties), TopName));
			list.Add(new ImageInfo(BottomName, flags, 21, 17, ">", typeof(ButtonImageProperties), BottomName));
		}
		protected override Type GetResourceType() {
			return typeof(ASPxScheduler);
		}
		protected override string GetResourceImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetImageCategory() {
			return Category;
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxScheduler.SchedulerSpriteCssResourceName;
		}
	}
	public class MenuImages : ImagesBaseNoLoadingPanel {
		#region Fields
		internal const string Category = "Menu";
		internal const string DeleteAppointmentName = "Delete";
		internal const string GotoDateName = "GoToDate";
		internal const string NewAppointmentName = "NewAppointment";
		internal const string NewRecurringAppointmentName = "RecurringAppointment";
		SchedulerCommonMenuImages commonImages;
		#endregion
		public MenuImages(ISkinOwner owner)
			: base(owner) {
			this.commonImages = new SchedulerCommonMenuImages();
		}
		#region Properties
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties DeleteAppointment { get { return GetImage(DeleteAppointmentName); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties GotoDate { get { return GetImage(GotoDateName); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties NewAppointment { get { return GetImage(NewAppointmentName); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties NewRecurringAppointment { get { return GetImage(NewRecurringAppointmentName); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public SchedulerCommonMenuImages CommonImages { get { return commonImages; } }
		#endregion
		public override void CopyFrom(ImagesBase source) {
			base.CopyFrom(source);
			MenuImages menuImage = source as MenuImages;
			if (menuImage != null) {
				DeleteAppointment.CopyFrom(menuImage.DeleteAppointment);
				GotoDate.CopyFrom(menuImage.GotoDate);
				NewAppointment.CopyFrom(menuImage.NewAppointment);
				NewRecurringAppointment.CopyFrom(menuImage.NewRecurringAppointment);
				CommonImages.CopyFrom(menuImage.CommonImages);
			}
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			string[] names = new string[] { DeleteAppointmentName, GotoDateName, NewAppointmentName, NewRecurringAppointmentName };
			for (int i = names.Length - 1; i >= 0; i--)
				list.Add(new ImageInfo(names[i], ImageFlags.IsPng, 16, 16, names[i]));
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { CommonImages });
		}
		protected override Type GetResourceType() {
			return typeof(ASPxScheduler);
		}
		protected override string GetResourceImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetImageCategory() {
			return Category;
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxScheduler.SchedulerSpriteCssResourceName;
		}
	}
	public class AppointmentImages : ImagesBaseNoLoadingPanel {
		internal const string Category = "Appointment";
		internal const string RecurrenceName = "Recurrence";
		internal const string NoRecurrenceName = "NoRecurrence";
		internal const string ReminderName = "Reminder";
		internal const string DayClockName = "DayClock";
		internal const string NightClockName = "NightClock";
		internal const string StartArrowName = "StartArrow";
		internal const string EndArrowName = "EndArrow";
		public AppointmentImages(ISkinOwner owner)
			: base(owner) {
		}
		protected override bool KeepDefaultSizes {
			get { return true; }
		}
		#region Properties
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("AppointmentImagesRecurrence"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Recurrence { get { return GetImage(RecurrenceName); } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("AppointmentImagesNoRecurrence"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties NoRecurrence { get { return GetImage(NoRecurrenceName); } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("AppointmentImagesReminder"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Reminder { get { return GetImage(ReminderName); } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("AppointmentImagesDayClock"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties DayClock { get { return GetImage(DayClockName); } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("AppointmentImagesNightClock"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties NightClock { get { return GetImage(NightClockName); } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("AppointmentImagesStartArrow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties StartArrow { get { return GetImage(StartArrowName); } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("AppointmentImagesEndArrow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties EndArrow { get { return GetImage(EndArrowName); } }
		#endregion
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			string[] aptNames = new string[] { RecurrenceName, NoRecurrenceName, 
				ReminderName, DayClockName, NightClockName, 
				StartArrowName, EndArrowName };
			for (int i = aptNames.Length - 1; i >= 0; i--)
				list.Add(new ImageInfo(aptNames[i], ImageFlags.IsPng, 15, 15, aptNames[i]));
		}
		protected override Type GetResourceType() {
			return typeof(ASPxScheduler);
		}
		protected override string GetResourceImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetImageCategory() {
			return Category;
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxScheduler.SchedulerSpriteCssResourceName;
		}
	}
	public abstract class ClockImages : ImagesBaseNoLoadingPanel {
		internal const string Category = "AppointmentClockArrows";
		string[] imageNames;
		protected ClockImages(ISkinOwner owner)
			: base(owner) {
			CreateImageNames();
		}
		#region Properties
		protected internal string[] ImageNames { get { return imageNames; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Min00 { get { return GetImage(imageNames[0]); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Min05 { get { return GetImage(imageNames[1]); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Min10 { get { return GetImage(imageNames[2]); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Min15 { get { return GetImage(imageNames[3]); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Min20 { get { return GetImage(imageNames[4]); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Min25 { get { return GetImage(imageNames[5]); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Min30 { get { return GetImage(imageNames[6]); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Min35 { get { return GetImage(imageNames[7]); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Min40 { get { return GetImage(imageNames[8]); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Min45 { get { return GetImage(imageNames[9]); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Min50 { get { return GetImage(imageNames[10]); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Min55 { get { return GetImage(imageNames[11]); } }
		protected override bool KeepDefaultSizes { get { return true; } }
		#endregion
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			CreateImageNames();
			for (int i = ImageNames.Length - 1; i >= 0; i--)
				list.Add(new ImageInfo(ImageNames[i], ImageFlags.IsPng, 15, 15, ImageNames[i]));
		}
		protected internal virtual void CreateImageNames() {
			if (ImageNames == null)
				this.imageNames = GetClockImageNames();
		}
		protected override Type GetResourceType() {
			return typeof(ASPxScheduler);
		}
		protected override string GetResourceImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetImageCategory() {
			return Category;
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxScheduler.SchedulerSpriteCssResourceName;
		}
		protected internal abstract string[] GetClockImageNames();
	}
	public class AppointmentInplaceEditorImages : ImagesBaseNoLoadingPanel {
		internal const int ImageSize = 19;
		internal const string Category = "InplaceEditor";
		internal const string SaveName = "Save";
		internal const string CancelName = "Cancel";
		internal const string EditFormName = "EditForm";
		public AppointmentInplaceEditorImages(ISkinOwner owner)
			: base(owner) {
		}
		#region Properties
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Save { get { return GetImage(SaveName); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties Cancel { get { return GetImage(CancelName); } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageProperties EditForm { get { return GetImage(EditFormName); } }
		#endregion
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(SaveName, ImageFlags.IsPng, ImageSize, ImageSize, SaveName));
			list.Add(new ImageInfo(CancelName, ImageFlags.IsPng, ImageSize, ImageSize, CancelName));
			list.Add(new ImageInfo(EditFormName, ImageFlags.IsPng, ImageSize, ImageSize, EditFormName));
		}
		protected override string GetImageCategory() {
			return Category;
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxScheduler.SchedulerSpriteCssResourceName;
		}
	}
	public class SchedulerCommonMenuImages : DevExpress.Web.MenuImages {
		public SchedulerCommonMenuImages()
			: base(null) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageProperties LoadingPanel {
			get { return base.LoadingPanel; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string ImageFolder { get { return base.ImageFolder; } set { base.ImageFolder = value; } }
	}
	public class SchedulerFormEditorsImages : DevExpress.Web.EditorImages {
		public SchedulerFormEditorsImages(ISkinOwner owner)
			: base(owner) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageProperties LoadingPanel {
			get { return base.LoadingPanel; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string ImageFolder { get { return base.ImageFolder; } set { base.ImageFolder = value; } }
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class DayHoursClockImages : ClockImages {
		public DayHoursClockImages(ISkinOwner owner)
			: base(owner) {
		}
		protected internal override string[] GetClockImageNames() {
			return ClockImageNames.DayHours;
		}
	}
	public class NightHoursClockImages : ClockImages {
		public NightHoursClockImages(ISkinOwner owner)
			: base(owner) {
		}
		protected internal override string[] GetClockImageNames() {
			return ClockImageNames.NightHours;
		}
	}
	public class DayMinutesClockImages : ClockImages {
		public DayMinutesClockImages(ISkinOwner owner)
			: base(owner) {
		}
		protected internal override string[] GetClockImageNames() {
			return ClockImageNames.DayMinutes;
		}
	}
	public class NightMinutesClockImages : ClockImages {
		public NightMinutesClockImages(ISkinOwner owner)
			: base(owner) {
		}
		protected internal override string[] GetClockImageNames() {
			return ClockImageNames.NightMinutes;
		}
	}
	#region ClockImageNames
	public static class ClockImageNames {
		public const string Day_hour_00 = "day_hour_00";
		public const string Day_hour_05 = "day_hour_05";
		public const string Day_hour_10 = "day_hour_10";
		public const string Day_hour_15 = "day_hour_15";
		public const string Day_hour_20 = "day_hour_20";
		public const string Day_hour_25 = "day_hour_25";
		public const string Day_hour_30 = "day_hour_30";
		public const string Day_hour_35 = "day_hour_35";
		public const string Day_hour_40 = "day_hour_40";
		public const string Day_hour_45 = "day_hour_45";
		public const string Day_hour_50 = "day_hour_50";
		public const string Day_hour_55 = "day_hour_55";
		public const string Night_hour_00 = "night_hour_00";
		public const string Night_hour_05 = "night_hour_05";
		public const string Night_hour_10 = "night_hour_10";
		public const string Night_hour_15 = "night_hour_15";
		public const string Night_hour_20 = "night_hour_20";
		public const string Night_hour_25 = "night_hour_25";
		public const string Night_hour_30 = "night_hour_30";
		public const string Night_hour_35 = "night_hour_35";
		public const string Night_hour_40 = "night_hour_40";
		public const string Night_hour_45 = "night_hour_45";
		public const string Night_hour_50 = "night_hour_50";
		public const string Night_hour_55 = "night_hour_55";
		public const string Day_minute_00 = "day_minute_00";
		public const string Day_minute_05 = "day_minute_05";
		public const string Day_minute_10 = "day_minute_10";
		public const string Day_minute_15 = "day_minute_15";
		public const string Day_minute_20 = "day_minute_20";
		public const string Day_minute_25 = "day_minute_25";
		public const string Day_minute_30 = "day_minute_30";
		public const string Day_minute_35 = "day_minute_35";
		public const string Day_minute_40 = "day_minute_40";
		public const string Day_minute_45 = "day_minute_45";
		public const string Day_minute_50 = "day_minute_50";
		public const string Day_minute_55 = "day_minute_55";
		public const string Night_minute_00 = "night_minute_00";
		public const string Night_minute_05 = "night_minute_05";
		public const string Night_minute_10 = "night_minute_10";
		public const string Night_minute_15 = "night_minute_15";
		public const string Night_minute_20 = "night_minute_20";
		public const string Night_minute_25 = "night_minute_25";
		public const string Night_minute_30 = "night_minute_30";
		public const string Night_minute_35 = "night_minute_35";
		public const string Night_minute_40 = "night_minute_40";
		public const string Night_minute_45 = "night_minute_45";
		public const string Night_minute_50 = "night_minute_50";
		public const string Night_minute_55 = "night_minute_55";
		public static string[] DayHours = new string[] {
			Day_hour_00, Day_hour_05, Day_hour_10, Day_hour_15, Day_hour_20, Day_hour_25, Day_hour_30, Day_hour_35, Day_hour_40,Day_hour_45, Day_hour_50, Day_hour_55 
		};
		public static string[] NightHours = new string[] {
			Night_hour_00, Night_hour_05, Night_hour_10, Night_hour_15, Night_hour_20, Night_hour_25, Night_hour_30, Night_hour_35, Night_hour_40,Night_hour_45, Night_hour_50, Night_hour_55 
		};
		public static string[] DayMinutes = new string[] {
			Day_minute_00, Day_minute_05, Day_minute_10, Day_minute_15, Day_minute_20, Day_minute_25, Day_minute_30, Day_minute_35, Day_minute_40,Day_minute_45, Day_minute_50, Day_minute_55 
		};
		public static string[] NightMinutes = new string[] {
			Night_minute_00, Night_minute_05, Night_minute_10, Night_minute_15, Night_minute_20, Night_minute_25, Night_minute_30, Night_minute_35, Night_minute_40,Night_minute_45, Night_minute_50, Night_minute_55 
		};
	}
	#endregion
	public abstract class ImagesBaseNoLoadingPanel : ImagesBase {
		protected ImagesBaseNoLoadingPanel(ISkinOwner owner)
			: base(owner) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageProperties LoadingPanel { get { return ImageProperties.Empty; } }
	}
}
