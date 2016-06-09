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
using System.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Printing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler {
	#region SchedulerOptionsView
	public class SchedulerOptionsView : SchedulerOptionsViewBase {
		#region Fields
		bool hideSelection;
		ToolTipVisibility toolTipVisibility;
		SchedulerResourceHeaderOptions resourceHeadersOptions = new SchedulerResourceHeaderOptions();
		bool enableAnimation = true;
		#endregion
		public SchedulerOptionsView() {
			SubscribeResourceHeaderOptionsEvents();
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerOptionsViewEnableAnimation"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool EnableAnimation {
			get { return enableAnimation; }
			set {
				if (enableAnimation == value)
					return;
				enableAnimation = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableAnimation", !enableAnimation, enableAnimation));
			}
		}
		#region Properties
		#region ResourceHeaders
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerOptionsViewResourceHeaders"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true)]
		public SchedulerResourceHeaderOptions ResourceHeaders { get { return resourceHeadersOptions; } }
		#endregion
		#region ToolTipVisibility
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerOptionsViewToolTipVisibility"),
#endif
DefaultValue(ToolTipVisibility.Standard), XtraSerializableProperty(), NotifyParentProperty(true)]
		public ToolTipVisibility ToolTipVisibility {
			get { return toolTipVisibility; }
			set {
				ToolTipVisibility oldVal = toolTipVisibility;
				if (oldVal == value)
					return;
				toolTipVisibility = value;
				OnChanged(new BaseOptionChangedEventArgs("ToolTipVisibility", oldVal, toolTipVisibility));
			}
		}
		#endregion
		#region HideSelection
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerOptionsViewHideSelection"),
#endif
DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool HideSelection {
			get { return hideSelection; }
			set {
				if (hideSelection == value)
					return;
				hideSelection = value;
				OnChanged(new BaseOptionChangedEventArgs("HideSelection", !hideSelection, hideSelection));
			}
		}
		#endregion
		#endregion
		protected internal virtual void SubscribeResourceHeaderOptionsEvents() {
			resourceHeadersOptions.Changed += new BaseOptionChangedEventHandler(OnResourceHeaderOptionsChanged);
		}
		protected internal virtual void UnsubscribeResourceHeaderOptionsEvents() {
			resourceHeadersOptions.Changed -= new BaseOptionChangedEventHandler(OnResourceHeaderOptionsChanged);
		}
		protected internal virtual void OnResourceHeaderOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged(e);
		}
		protected internal override void ResetCore() {
			base.ResetCore();
			resourceHeadersOptions.Reset();
			ToolTipVisibility = ToolTipVisibility.Standard;
			HideSelection = false;
			EnableAnimation = true;
		}
	}
	#endregion
	#region TimeIndicatorDisplayOptions
	public class TimeIndicatorDisplayOptions : TimeIndicatorDisplayOptionsBase {
		bool showOverAppointment;
		#region ShowOverAppointment
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("TimeIndicatorDisplayOptionsShowOverAppointment"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public bool ShowOverAppointment {
			get {
				return showOverAppointment;
			}
			set {
				if (showOverAppointment == value)
					return;
				bool oldValue = showOverAppointment;
				showOverAppointment = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowOverAppointment", oldValue, showOverAppointment));
			}
		}
		#endregion
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			TimeIndicatorDisplayOptions optionsTimeIndicator = options as TimeIndicatorDisplayOptions;
			if (optionsTimeIndicator != null) {
				BeginUpdate();
				try {
					ShowOverAppointment = optionsTimeIndicator.ShowOverAppointment;
				} finally {
					EndUpdate();
				}
			}
		}
		protected internal override void ResetCore() {
			base.ResetCore();
			ShowOverAppointment = false;
		}
	}
	#endregion
	#region SchedulerResourceHeaderOptions
	public class SchedulerResourceHeaderOptions : SchedulerNotificationOptions {
		#region Fields
		int height;
		bool rotateCaption;
		HeaderImageAlign imageAlign;
		HeaderImageSizeMode imageSizeMode;
		Size fixedImageSize;
		InterpolationMode imageInterpolationMode;
		#endregion
		#region Properties
		#region Height
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerResourceHeaderOptionsHeight"),
#endif
DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int Height {
			get { return height; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Height", value);
				int oldVal = height;
				if (oldVal == value)
					return;
				height = value;
				OnChanged(new BaseOptionChangedEventArgs("Height", oldVal, height));
			}
		}
		#endregion
		#region RotateCaption
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerResourceHeaderOptionsRotateCaption"),
#endif
DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool RotateCaption {
			get { return rotateCaption; }
			set {
				if (rotateCaption == value)
					return;
				rotateCaption = value;
				OnChanged(new BaseOptionChangedEventArgs("RotateCaption", !rotateCaption, rotateCaption));
			}
		}
		#endregion
		#region ImageAlign
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerResourceHeaderOptionsImageAlign"),
#endif
DefaultValue(HeaderImageAlign.Left), XtraSerializableProperty(), NotifyParentProperty(true)]
		public HeaderImageAlign ImageAlign {
			get { return imageAlign; }
			set {
				HeaderImageAlign oldVal = imageAlign;
				if (oldVal == value)
					return;
				imageAlign = value;
				OnChanged(new BaseOptionChangedEventArgs("ImageAlign", oldVal, imageAlign));
			}
		}
		#endregion
		#region ImageSize
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerResourceHeaderOptionsImageSize"),
#endif
XtraSerializableProperty(), NotifyParentProperty(true)]
		public Size ImageSize {
			get { return fixedImageSize; }
			set {
				Size oldVal = fixedImageSize;
				if (oldVal == value)
					return;
				fixedImageSize = value;
				OnChanged(new BaseOptionChangedEventArgs("ImageSize", oldVal, fixedImageSize));
			}
		}
		internal bool ShouldSerializeImageSize() {
			return fixedImageSize != Size.Empty;
		}
		#endregion
		#region ImageSizeMode
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerResourceHeaderOptionsImageSizeMode"),
#endif
DefaultValue(HeaderImageSizeMode.CenterImage), XtraSerializableProperty(), NotifyParentProperty(true)]
		public HeaderImageSizeMode ImageSizeMode {
			get { return imageSizeMode; }
			set {
				HeaderImageSizeMode oldVal = imageSizeMode;
				if (oldVal == value)
					return;
				imageSizeMode = value;
				OnChanged(new BaseOptionChangedEventArgs("ImageSizeMode", oldVal, imageSizeMode));
			}
		}
		#endregion
		#region ImageInterpolationMode
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerResourceHeaderOptionsImageInterpolationMode"),
#endif
DefaultValue(InterpolationMode.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		public InterpolationMode ImageInterpolationMode {
			get { return imageInterpolationMode; }
			set {
				InterpolationMode oldVal = imageInterpolationMode;
				if (oldVal == value)
					return;
				imageInterpolationMode = value;
				OnChanged(new BaseOptionChangedEventArgs("ImageInterpolationMode", oldVal, imageInterpolationMode));
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			Height = 0;
			RotateCaption = true;
			ImageAlign = HeaderImageAlign.Left;
			ImageSizeMode = HeaderImageSizeMode.CenterImage;
			ImageSize = Size.Empty;
			ImageInterpolationMode = InterpolationMode.Default;
		}
	}
	#endregion
	#region SchedulerOptionsBehavior
	public class SchedulerOptionsBehavior : SchedulerOptionsBehaviorBase {
		const bool DefaultUseAsyncMode = true;
		bool useAsyncMode = DefaultUseAsyncMode;
		#region Properties
		#region TouchAllowed
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerOptionsBehaviorTouchAllowed"),
#endif
		DefaultValue(DefaultTouchAllowed), NotifyParentProperty(true)]
		public bool TouchAllowed {
			get { return InnerTouchAllowed; }
			set {
				InnerTouchAllowed = value;
			}
		}
		#endregion
		#region MouseWheelScrollAction
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerOptionsBehaviorMouseWheelScrollAction"),
#endif
		DefaultValue(DefaultMouseWheelScrollAction), NotifyParentProperty(true)]
		public MouseWheelScrollAction MouseWheelScrollAction {
			get { return InnerMouseWheelScrollAction; }
			set {
				InnerMouseWheelScrollAction = value;
			}
		}
		#endregion
		[ DefaultValue(DefaultUseAsyncMode), NotifyParentProperty(true)]
		public bool UseAsyncMode {
			get { return this.useAsyncMode; }
			set { this.useAsyncMode = value; }
		}
		#endregion
	}
	#endregion
	public class SchedulerOptionsPrint : BaseOptions {
		SchedulerPrintStyleKind printStyleKind = SchedulerPrintStyleKind.Default;
		public SchedulerOptionsPrint() {
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerOptionsPrintPrintStyle"),
#endif
DefaultValue(SchedulerPrintStyleKind.Default)]
		public SchedulerPrintStyleKind PrintStyle { get { return printStyleKind; } set { printStyleKind = value; } }
		public override string ToString() {
			return string.Empty;
		}
	}
	public class SchedulerOptionsRangeControl : ScaleBasedRangeControlClientOptions, ISchedulerOptionsRangeControl {
		internal const bool DefaultAutoAdjustMode = true;
		bool allowChangeActiveView;
		bool autoAdjustMode;
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerOptionsRangeControlAllowChangeActiveView"),
#endif
		DefaultValue(true)]
		public bool AllowChangeActiveView {
			get {
				return allowChangeActiveView;
			}
			set {
				if (allowChangeActiveView == value)
					return;
				allowChangeActiveView = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowChangeActiveView", !allowChangeActiveView, allowChangeActiveView));
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerOptionsRangeControlAutoAdjustMode"),
#endif
		DefaultValue(DefaultAutoAdjustMode)]
		public bool AutoAdjustMode {
			get {
				return autoAdjustMode;
			}
			set {
				if (autoAdjustMode == value)
					return;
				autoAdjustMode = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoAdjustMode", !autoAdjustMode, autoAdjustMode));
			}
		}
		protected internal override void ResetCore() {
			base.ResetCore();
			AllowChangeActiveView = true;
			AutoAdjustMode = true;
		}
	}
	#region SchedulerDeferredScrollingOption
	public class SchedulerDeferredScrollingOption : SchedulerNotificationOptions, ISchedulerDeferredScrollingOption {
		bool allow;
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerDeferredScrollingOptionAllow"),
#endif
		XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool Allow {
			get {
				return allow;
			}
			set {
				if (allow == value)
					return;
				allow = value;
				OnChanged(new BaseOptionChangedEventArgs("Allow", !allow, allow));
			}
		}
		event BaseOptionChangedEventHandler ISchedulerDeferredScrollingOption.Changed {
			add { Changed += value; }
			remove { Changed -= value; }
		}
		protected internal override void ResetCore() {
			Allow = false;
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	public class SchedulerOptionsErrorProvider {
		public void NotifyError(string propName, string message) {
			Exceptions.ThrowArgumentOutOfRangeException(propName, message);
		}
	}
}
