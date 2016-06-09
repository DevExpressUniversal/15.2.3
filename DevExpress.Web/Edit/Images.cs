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
using System.Collections.Generic;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Internal.InternalCheckBox;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public class EditorImages : ImagesBase {
		private const int ErrorImageDefaultSize = 14;
		public const string ErrorImageName = "edtError",
							BinaryImageDesignImageName = "edtBinaryImageDesign",
							BinaryImageDeleteButtonImageName = "edtBinaryImageDelete",
							BinaryImageOpenDialogButtonImageName = "edtBinaryImageOpenDialog",
							CalendarPrevYearImageName = "edtCalendarPrevYear",
							CalendarPrevMonthImageName = "edtCalendarPrevMonth",
							CalendarNextMonthImageName = "edtCalendarNextMonth",
							CalendarNextYearImageName = "edtCalendarNextYear",
							CalendarFastNavPrevYearImageName = "edtCalendarFNPrevYear",
							CalendarFastNavNextYearImageName = "edtCalendarFNNextYear",
							CheckBoxCheckedImageName = "edtCheckBoxChecked",
							CheckBoxUncheckedImageName = "edtCheckBoxUnchecked",
							CheckBoxGrayedImageName = "edtCheckBoxGrayed",
							RadioButtonCheckedImageName = "edtRadioButtonChecked",
							RadioButtonUncheckedImageName = "edtRadioButtonUnchecked",
							RadioButtonGrayedImageName = "edtRadioButtonGrayed",
							TrackBarIncrementButtonImageName = "edtTBIncBtn",
							TrackBarDecrementButtonImageName = "edtTBDecBtn",
							TrackBarMainDragHandleImageName = "edtTBMainDH",
							TrackBarSecondaryDragHandleImageName = "edtTBSecondaryDH",
							ButtonEditEllipsisImageName = "edtEllipsis",
							ButtonEditClearButtonImageName = "edtClear",
							DropDownEditDropDownImageName = "edtDropDown",
							DateEditTimeSectionClockFaceImageName = "edtDETSClockFace",
							DateEditTimeSectionHourHandImageName = "edtDETSHourHand",
							DateEditTimeSectionMinuteHandImageName = "edtDETSMinuteHand",
							DateEditTimeSectionSecondHandImageName = "edtDETSSecondHand",
							SpinEditIncrementImageName = "edtSpinEditIncrementImage",
							SpinEditDecrementImageName = "edtSpinEditDecrementImage",
							SpinEditLargeIncrementImageName = "edtSpinEditLargeIncImage",
							SpinEditLargeDecrementImageName = "edtSpinEditLargeDecImage",
							ImageEmptyImageName = "edtImageEmpty",
							ListEditItemImageName = "edtListEditItem",
							ButtonBackImageName = "edtButtonBack",
							ButtonHoverBackImageName = "edtButtonHoverBack",
							SpinIncButtonBackImageName = "edtSpinIncBtnBack",
							SpinDecButtonBackImageName = "edtSpinDecBtnBack",
							CalendarButtonBackImageName = "edtCalendarButtonBack",
							CalendarButtonHoverBackImageName = "edtCalendarButtonHBack",
							DropDownButtonBackImageName = "edtDropDownBack",
							DropDownButtonHoverBackImageName = "edtDropDownButtonHoverBack",
							TokenBoxTokenRemoveButtonImageName = "edtTokenBoxTokenRemoveButton",
							TokenBoxTokenRemoveButtonHoverImageName = "edtTokenBoxTokenRemoveButtonHover",
							TokenBoxTokenBackgroundImageName = "edtTokenBoxTokenBackground",
							TrackBarLargeTickHImageName = "edtTrackBarLargeTickH",
							TrackBarSmallTickHImageName = "edtTrackBarSmallTickH",
							TrackBarSmallTickVImageName = "edtTrackBarSmallTickV",
							TrackBarLargeTickVImageName = "edtTrackBarLargeTickV",
							TrackBarDoubleSmallTickVImageName = "edtTrackBarDoubleSmallTickV",
							TrackBarDoubleSmallTickHImageName = "edtTrackBarDoubleSmallTickH",
							TrackBarDoubleLargeTickHImageName = "edtTrackBarDoubleLargeTickH",
							TrackBarDoubleLargeTickVImageName = "edtTrackBarDoubleLargeTickV",
							TrackBarBarHighlightVImageName = "edtTrackBarBarHighlightV",
							TrackBarBarHighlightHImageName = "edtTrackBarBarHighlightH",
							TrackBarTrackVImageName = "edtTrackBarTrackV",
							TrackBarTrackHImageName = "edtTrackBarTrackH";
		public EditorImages(ISkinOwner properties)
			: base(properties) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesTrackBarDecrementButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties TrackBarDecrementButton {
			get { return (ButtonImageProperties)GetImageBase(TrackBarDecrementButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesTrackBarIncrementButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties TrackBarIncrementButton {
			get { return (ButtonImageProperties)GetImageBase(TrackBarIncrementButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesTrackBarMainDragHandle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties TrackBarMainDragHandle {
			get { return (ButtonImageProperties)GetImageBase(TrackBarMainDragHandleImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesTrackBarSecondaryDragHandle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties TrackBarSecondaryDragHandle {
			get { return (ButtonImageProperties)GetImageBase(TrackBarSecondaryDragHandleImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesDateEditTimeSectionClockFace"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties DateEditTimeSectionClockFace {
			get { return (ImageProperties)GetImageBase(DateEditTimeSectionClockFaceImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesDateEditTimeSectionHourHand"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties DateEditTimeSectionHourHand {
			get { return (ImageProperties)GetImageBase(DateEditTimeSectionHourHandImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesDateEditTimeSectionMinuteHand"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties DateEditTimeSectionMinuteHand {
			get { return (ImageProperties)GetImageBase(DateEditTimeSectionMinuteHandImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesDateEditTimeSectionSecondHand"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties DateEditTimeSectionSecondHand {
			get { return (ImageProperties)GetImageBase(DateEditTimeSectionSecondHandImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesBinaryImageDeleteButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BinaryImageDeleteButtonImageProperties BinaryImageDeleteButton {
			get { return (BinaryImageDeleteButtonImageProperties)GetImageBase(BinaryImageDeleteButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesBinaryImageOpenDialogButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BinaryImageOpenDialogButtonImageProperties BinaryImageOpenDialogButton {
			get { return (BinaryImageOpenDialogButtonImageProperties)GetImageBase(BinaryImageOpenDialogButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesCalendarPrevYear"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx CalendarPrevYear {
			get { return (ImagePropertiesEx)GetImageBase(CalendarPrevYearImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesCalendarPrevMonth"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx CalendarPrevMonth {
			get { return (ImagePropertiesEx)GetImageBase(CalendarPrevMonthImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesCalendarNextMonth"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx CalendarNextMonth {
			get { return (ImagePropertiesEx)GetImageBase(CalendarNextMonthImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesCalendarNextYear"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx CalendarNextYear {
			get { return (ImagePropertiesEx)GetImageBase(CalendarNextYearImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesCalendarFastNavPrevYear"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties CalendarFastNavPrevYear {
			get { return GetImage(CalendarFastNavPrevYearImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesCalendarFastNavNextYear"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties CalendarFastNavNextYear {
			get { return GetImage(CalendarFastNavNextYearImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesCheckBoxChecked"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public InternalCheckBoxImageProperties CheckBoxChecked {
			get { return base.CheckedImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesCheckBoxUnchecked"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public InternalCheckBoxImageProperties CheckBoxUnchecked {
			get { return base.UncheckedImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesCheckBoxUndefined"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the CheckBoxGrayed property instead.")]
		public InternalCheckBoxImageProperties CheckBoxUndefined { get { return CheckBoxGrayed; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesCheckBoxGrayed"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public InternalCheckBoxImageProperties CheckBoxGrayed {
			get { return base.GrayedImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesRadioButtonChecked"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public InternalCheckBoxImageProperties RadioButtonChecked {
			get { return (InternalCheckBoxImageProperties)GetImageBase(RadioButtonCheckedImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesRadioButtonUnchecked"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public InternalCheckBoxImageProperties RadioButtonUnchecked {
			get { return (InternalCheckBoxImageProperties)GetImageBase(RadioButtonUncheckedImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesRadioButtonUndefined"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public InternalCheckBoxImageProperties RadioButtonUndefined {
			get { return (InternalCheckBoxImageProperties)GetImageBase(RadioButtonGrayedImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesButtonEditEllipsis"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties ButtonEditEllipsis {
			get { return (ButtonImageProperties)GetImageBase(ButtonEditEllipsisImageName); }
		}
		[
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties ButtonEditClearButton {
			get { return (ButtonImageProperties)GetImageBase(ButtonEditClearButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesDropDownEditDropDown"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties DropDownEditDropDown {
			get { return (ButtonImageProperties)GetImageBase(DropDownEditDropDownImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesImageEmpty"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ImageEmpty {
			get { return GetImage(ImageEmptyImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesListEditItem"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ListEditItem {
			get { return GetImage(ListEditItemImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesSpinEditIncrement"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties SpinEditIncrement {
			get { return (ButtonImageProperties)GetImageBase(SpinEditIncrementImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesSpinEditDecrement"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties SpinEditDecrement {
			get { return (ButtonImageProperties)GetImageBase(SpinEditDecrementImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesSpinEditLargeIncrement"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties SpinEditLargeIncrement {
			get { return (ButtonImageProperties)GetImageBase(SpinEditLargeIncrementImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorImagesSpinEditLargeDecrement"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties SpinEditLargeDecrement {
			get { return (ButtonImageProperties)GetImageBase(SpinEditLargeDecrementImageName); }
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(ErrorImageName, ImageFlags.IsPng, ErrorImageDefaultSize, ErrorImageName));
			list.Add(new ImageInfo(TrackBarMainDragHandleImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | 
				ImageFlags.HasPressedState, string.Empty, typeof(ButtonImageProperties), TrackBarMainDragHandleImageName));
			list.Add(new ImageInfo(TrackBarSecondaryDragHandleImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | 
				ImageFlags.HasPressedState, string.Empty, typeof(ButtonImageProperties), TrackBarSecondaryDragHandleImageName));
			list.Add(new ImageInfo(TrackBarIncrementButtonImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | 
				ImageFlags.HasPressedState, string.Empty, typeof(ButtonImageProperties), TrackBarIncrementButtonImageName));
			list.Add(new ImageInfo(TrackBarDecrementButtonImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | 
				ImageFlags.HasPressedState, string.Empty, typeof(ButtonImageProperties), TrackBarDecrementButtonImageName));
			list.Add(new ImageInfo(BinaryImageDesignImageName));
			list.Add(new ImageInfo(BinaryImageDeleteButtonImageName, typeof(BinaryImageDeleteButtonImageProperties), BinaryImageDeleteButtonImageName));
			list.Add(new ImageInfo(BinaryImageOpenDialogButtonImageName, typeof(BinaryImageOpenDialogButtonImageProperties), BinaryImageOpenDialogButtonImageName));
			list.Add(new ImageInfo(CalendarPrevYearImageName, ImageFlags.HasDisabledState, "<<",
				typeof(ImagePropertiesEx), CalendarPrevYearImageName));
			list.Add(new ImageInfo(CalendarPrevMonthImageName, ImageFlags.HasDisabledState, "<",
				typeof(ImagePropertiesEx), CalendarPrevMonthImageName));
			list.Add(new ImageInfo(CalendarNextMonthImageName, ImageFlags.HasDisabledState, ">",
				typeof(ImagePropertiesEx), CalendarNextMonthImageName));
			list.Add(new ImageInfo(CalendarNextYearImageName, ImageFlags.HasDisabledState, ">>",
				typeof(ImagePropertiesEx), CalendarNextYearImageName));
			list.Add(new ImageInfo(CalendarFastNavPrevYearImageName, ImageFlags.Empty, "<", 
				CalendarFastNavPrevYearImageName));
			list.Add(new ImageInfo(CalendarFastNavNextYearImageName, ImageFlags.Empty, ">", 
				CalendarFastNavNextYearImageName));
			list.Add(new ImageInfo(RadioButtonCheckedImageName, ImageFlags.HasDisabledState, string.Empty, typeof(InternalCheckBoxImageProperties), RadioButtonCheckedImageName, InternalCheckboxControl.EditorsSpriteControlName));
			list.Add(new ImageInfo(RadioButtonUncheckedImageName, ImageFlags.HasDisabledState, string.Empty, typeof(InternalCheckBoxImageProperties), RadioButtonUncheckedImageName, InternalCheckboxControl.EditorsSpriteControlName));
			list.Add(new ImageInfo(RadioButtonGrayedImageName, ImageFlags.HasDisabledState, string.Empty, typeof(InternalCheckBoxImageProperties), RadioButtonGrayedImageName, InternalCheckboxControl.EditorsSpriteControlName));
			list.Add(new ImageInfo(ButtonEditEllipsisImageName, ImageFlags.HasDisabledState, "...",
				typeof(ButtonImageProperties), ButtonEditEllipsisImageName));
			list.Add(new ImageInfo(ButtonEditClearButtonImageName, ImageFlags.HasDisabledState, "x",
				typeof(ButtonImageProperties), ButtonEditClearButtonImageName));
			list.Add(new ImageInfo(DropDownEditDropDownImageName, ImageFlags.HasDisabledState, "v",
				typeof(ButtonImageProperties), DropDownEditDropDownImageName));
			list.Add(new ImageInfo(DateEditTimeSectionClockFaceImageName, typeof(ImageProperties), DateEditTimeSectionClockFaceImageName));
			list.Add(new ImageInfo(DateEditTimeSectionHourHandImageName, typeof(ImageProperties), DateEditTimeSectionHourHandImageName));
			list.Add(new ImageInfo(DateEditTimeSectionMinuteHandImageName, typeof(ImageProperties), DateEditTimeSectionMinuteHandImageName));
			list.Add(new ImageInfo(DateEditTimeSectionSecondHandImageName, typeof(ImageProperties), DateEditTimeSectionSecondHandImageName));
			list.Add(new ImageInfo(SpinEditIncrementImageName, ImageFlags.HasDisabledState,
				"+", typeof(ButtonImageProperties), SpinEditIncrementImageName));
			list.Add(new ImageInfo(SpinEditDecrementImageName, ImageFlags.HasDisabledState,
				"-", typeof(ButtonImageProperties), SpinEditDecrementImageName));
			list.Add(new ImageInfo(SpinEditLargeIncrementImageName, ImageFlags.HasDisabledState,
				"+", typeof(ButtonImageProperties), SpinEditLargeIncrementImageName));
			list.Add(new ImageInfo(SpinEditLargeDecrementImageName, ImageFlags.HasDisabledState,
				"-", typeof(ButtonImageProperties), SpinEditLargeDecrementImageName));
			list.Add(new ImageInfo(TokenBoxTokenRemoveButtonImageName, ImageFlags.HasDisabledState, "x", typeof(ImageProperties), TokenBoxTokenRemoveButtonImageName));
			list.Add(new ImageInfo(TokenBoxTokenRemoveButtonHoverImageName, typeof(ImageProperties), TokenBoxTokenRemoveButtonHoverImageName));
			list.Add(new ImageInfo(ImageEmptyImageName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ListEditItemImageName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ButtonBackImageName));
			list.Add(new ImageInfo(SpinIncButtonBackImageName));
			list.Add(new ImageInfo(SpinDecButtonBackImageName));
			list.Add(new ImageInfo(CalendarButtonBackImageName));
			list.Add(new ImageInfo(CalendarButtonHoverBackImageName));
			list.Add(new ImageInfo(DropDownButtonBackImageName));
			list.Add(new ImageInfo(DropDownButtonHoverBackImageName));
			list.Add(new ImageInfo(TokenBoxTokenBackgroundImageName));
		}
		protected override Type GetResourceType() {
			return typeof(ASPxEditBase);
		}
		protected override string GetResourceImagePath() {
			return ASPxEditBase.EditImagesResourcePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxEditBase.EditImagesResourcePath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxEditBase.EditSpriteCssResourceName;
		}		
	}
	public class BinaryImageOpenDialogButtonImageProperties : ImagePropertiesBase {
		public BinaryImageOpenDialogButtonImageProperties()
			: base() {
		}
		public BinaryImageOpenDialogButtonImageProperties(string url)
			: base(url) {
		}
		public BinaryImageOpenDialogButtonImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageOpenDialogButtonImagePropertiesToolTip"),
#endif
 DefaultValue(StringResources.BinaryImage_UploadFile), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public override string ToolTip {
			get { return GetStringProperty("ToolTip", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.BinaryImage_UploadFile)); }
			set {
				SetStringProperty("ToolTip", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.BinaryImage_UploadFile), value);
				Changed();
			}
		}
	}
	public class BinaryImageDeleteButtonImageProperties : ImagePropertiesBase {
		public BinaryImageDeleteButtonImageProperties()
			: base() {
		}
		public BinaryImageDeleteButtonImageProperties(string url)
			: base(url) {
		}
		public BinaryImageDeleteButtonImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageDeleteButtonImagePropertiesToolTip"),
#endif
 DefaultValue(StringResources.BinaryImage_Clear), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public override string ToolTip {
			get { return GetStringProperty("ToolTip", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.BinaryImage_Clear)); }
			set {
				SetStringProperty("ToolTip", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.BinaryImage_Clear), value);
				Changed();
			}
		}
	}
}
