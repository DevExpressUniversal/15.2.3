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
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using System.ComponentModel.Design;
using System.Threading.Tasks;
namespace DevExpress.XtraBars.Alerter {
	[DXToolboxItem(true),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	 Designer("DevExpress.XtraBars.Design.AlertControlDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	 Description("Supports displaying alert windows."),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "AlertControl")
	]
	public class AlertControl : Component, ISupportLookAndFeel, IAlertControl, DevExpress.Utils.MVVM.Services.INotificationProvider {
		AlertButtonCollection items;
		AlertFormControlBoxPosition controlBoxPosition = AlertFormControlBoxPosition.Top;
		AlertFormLocation formLocation = AlertFormLocation.BottomRight;
		AlertFormDisplaySpeed formDisplaySpeed = AlertFormDisplaySpeed.Moderate;
		AlertFormShowingEffect formShowingEffect = AlertFormShowingEffect.FadeIn;
		int autoFormDelay = 7000;
		bool showToolTips = true;
		bool showCloseButton = true;
		bool showPinButton = true;
		bool allowHtmlText = false;
		object images = null;
		bool autoHeight = false;
		bool allowHotTrack = true;
		int formMaxCount = 0;
		Timer timer = null;
		UserLookAndFeel lookAndFeel;
		PopupMenu menu = null;
		AppearanceCaptionObject appearanceCaption;
		AppearanceObject appearanceText;
		AppearanceSelectedObject appearanceHotTrackedText;
		List<AlertForm> postponedForms = new List<AlertForm>();
		[Category(CategoryName.Events), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlAlertClick")
#else
	Description("")
#endif
]
		public event AlertClickEventHandler AlertClick;
		[Category(CategoryName.Events), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlButtonClick")
#else
	Description("")
#endif
]
		public event AlertButtonClickEventHandler ButtonClick;
		[Category(CategoryName.Events), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlButtonDownChanged")
#else
	Description("")
#endif
]
		public event AlertButtonDownChangedEventHandler ButtonDownChanged;
		[Category(CategoryName.Events), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlBeforeFormShow")
#else
	Description("")
#endif
]
		public event AlertFormEventHandler BeforeFormShow;
		[Category(CategoryName.Events), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlFormLoad")
#else
	Description("")
#endif
]
		public event AlertFormLoadEventHandler FormLoad;
		[Category(CategoryName.Events), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlFormClosing")
#else
	Description("")
#endif
]
		public event AlertFormClosingEventHandler FormClosing;
		[Category(CategoryName.Events), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlMouseFormEnter")
#else
	Description("")
#endif
]
		public event AlertEventHandler MouseFormEnter;
		[Category(CategoryName.Events), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlMouseFormLeave")
#else
	Description("")
#endif
]
		public event AlertEventHandler MouseFormLeave;
		[Category(CategoryName.Events)]
		public event GetDesiredFormWidthEventHandler GetDesiredAlertFormWidth;
		public AlertControl() : this(null) { }
		public AlertControl(IContainer container) {
			if(container != null)
				container.Add(this);
			this.items = CreateButtonCollection();
			this.lookAndFeel = new UserLookAndFeel(null);
			this.LookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
			this.appearanceCaption = new AppearanceCaptionObject();
			this.appearanceText = new AppearanceObject();
			this.appearanceHotTrackedText = new AppearanceSelectedObject();
			this.timer = new Timer();
			this.timer.Enabled = false;
			this.timer.Interval = 100;
			this.timer.Tick += new EventHandler(timer_Tick);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				LookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
				LookAndFeel.Dispose();
			}
			base.Dispose(disposing);
		}
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) { }
		protected virtual AlertButtonCollection CreateButtonCollection() {
			return new AlertButtonCollection(this);
		}
		#region Properties
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlFormMaxCount"),
#endif
 Category(CategoryName.Behavior), DefaultValue(0)]
		public virtual int FormMaxCount {
			get { return formMaxCount; }
			set {
				if(value < 0) value = 0;
				if(formMaxCount == value) return;
				formMaxCount = value;
				timer.Start();
			}
		}
		protected virtual bool ShouldSerializeControlBoxPosition() { return ControlBoxPosition != AlertFormControlBoxPosition.Top; }
		protected virtual void ResetControlBoxPosition() { ControlBoxPosition = AlertFormControlBoxPosition.Top; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlControlBoxPosition"),
#endif
 Category(CategoryName.Appearance)]
		public virtual AlertFormControlBoxPosition ControlBoxPosition {
			get { return controlBoxPosition; }
			set {
				if(controlBoxPosition == value) return;
				controlBoxPosition = value;
			}
		}
		protected virtual bool ShouldSerializeFormLocation() { return FormLocation != AlertFormLocation.BottomRight; }
		protected virtual void ResetFormLocation() { FormLocation = AlertFormLocation.BottomRight; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlAutoHeight"),
#endif
 Category(CategoryName.Behavior), DefaultValue(false)]
		public bool AutoHeight {
			get { return autoHeight; }
			set {
				if(autoHeight == value) return;
				autoHeight = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlAllowHotTrack"),
#endif
 Category(CategoryName.Behavior), DefaultValue(true)]
		public bool AllowHotTrack {
			get { return allowHotTrack; }
			set {
				if(allowHotTrack == value) return;
				allowHotTrack = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlFormLocation"),
#endif
 Category(CategoryName.Behavior)]
		public virtual AlertFormLocation FormLocation {
			get { return formLocation; }
			set {
				if(formLocation == value) return;
				formLocation = value;
			}
		}
		protected virtual bool ShouldSerializeFormShowingEffect() { return FormShowingEffect != AlertFormShowingEffect.FadeIn; }
		protected virtual void ResetFormShowingEffect() { FormShowingEffect = AlertFormShowingEffect.FadeIn; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlFormShowingEffect"),
#endif
 Category(CategoryName.Behavior)]
		public virtual AlertFormShowingEffect FormShowingEffect {
			get { return formShowingEffect; }
			set {
				if(formShowingEffect == value) return;
				formShowingEffect = value;
			}
		}
		protected virtual bool ShouldSerializeFormDisplaySpeed() { return FormDisplaySpeed != AlertFormDisplaySpeed.Moderate; }
		protected virtual void ResetFormDisplaySpeed() { FormDisplaySpeed = AlertFormDisplaySpeed.Moderate; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlFormDisplaySpeed"),
#endif
 Category(CategoryName.Behavior)]
		public virtual AlertFormDisplaySpeed FormDisplaySpeed {
			get { return formDisplaySpeed; }
			set {
				if(formDisplaySpeed == value) return;
				formDisplaySpeed = value;
			}
		}
		[DefaultValue(7000), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlAutoFormDelay"),
#endif
 Category(CategoryName.Behavior)]
		public virtual int AutoFormDelay {
			get { return autoFormDelay; }
			set {
				if(autoFormDelay == value) return;
				if(value < 1000) value = 1000;
				autoFormDelay = value;
			}
		}
		[DefaultValue(true), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlShowToolTips"),
#endif
 Category(CategoryName.Behavior)]
		public virtual bool ShowToolTips {
			get { return showToolTips; }
			set {
				if(showToolTips == value) return;
				showToolTips = value;
			}
		}
		[DefaultValue(true), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlShowCloseButton"),
#endif
 Category(CategoryName.Appearance)]
		public virtual bool ShowCloseButton {
			get { return showCloseButton; }
			set {
				if(showCloseButton == value) return;
				showCloseButton = value;
			}
		}
		[DefaultValue(true), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlShowPinButton"),
#endif
 Category(CategoryName.Appearance)]
		public virtual bool ShowPinButton {
			get { return showPinButton; }
			set {
				if(showPinButton == value) return;
				showPinButton = value;
			}
		}
		bool ShouldSerializeButtons() { return items.Count != 0; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor)),
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlButtons"),
#endif
 Category(CategoryName.Appearance)]
		public virtual AlertButtonCollection Buttons { get { return items; } }
		[Category(CategoryName.Appearance), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlImages"),
#endif
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter)),
		DefaultValue(null)
		]
		public virtual object Images {
			get { return images; }
			set {
				if(images == value) return;
				images = value;
			}
		}
		[DefaultValue(null), Category(CategoryName.Behavior), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlPopupMenu")
#else
	Description("")
#endif
]
		public virtual PopupMenu PopupMenu {
			get { return menu; }
			set {
				if(menu == value) return;
				menu = value;
			}
		}
		bool ShouldSerializeAppearanceCaption() { return AppearanceCaption.ShouldSerialize(); }
		bool ShouldSerializeAppearanceText() { return AppearanceText.ShouldSerialize(); }
		bool ShouldSerializeAppearanceHotTrackedText() { return AppearanceHotTrackedText.ShouldSerialize(); }
		void ResetAppearanceCaption() { AppearanceCaption.Reset(); }
		void ResetAppearanceText() { AppearanceText.Reset(); }
		void ResetAppearanceHotTrackedText() { AppearanceHotTrackedText.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		 Category(CategoryName.Appearance), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlAppearanceCaption")
#else
	Description("")
#endif
]
		public virtual AppearanceCaptionObject AppearanceCaption { get { return appearanceCaption; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		 Category(CategoryName.Appearance), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlAppearanceText")
#else
	Description("")
#endif
]
		public virtual AppearanceObject AppearanceText { get { return appearanceText; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		 Category(CategoryName.Appearance), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlAppearanceHotTrackedText")
#else
	Description("")
#endif
]
		public virtual AppearanceObject AppearanceHotTrackedText { get { return appearanceHotTrackedText; } }
		[DefaultValue(false), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlAllowHtmlText"),
#endif
 Category(CategoryName.Appearance)]
		public virtual bool AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(allowHtmlText == value) return;
				allowHtmlText = value;
			}
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<AlertForm> AlertFormList {
			get {
				List<AlertForm> list = new List<AlertForm>();
				for(int i = 0; i < Application.OpenForms.Count; i++) {
					AlertForm aForm = Application.OpenForms[i] as AlertForm;
					if(aForm != null && this.Equals(aForm.AlertControl))
						list.Add(aForm);
				}
				return list;
			}
		}
		protected virtual Point GetCurrentLocation(Form owner) {
			if(owner == null ||
				(owner.Location.X == -32000 && owner.Location.Y == -32000)) return new Point(0, 0);
			return new Point(owner.Location.X + owner.Width / 2, owner.Location.Y + owner.Height / 2);
		}
		public void Show(Form owner) { Show(owner, new AlertInfo(string.Empty, string.Empty)); }
		public void Show(Form owner, string caption, string text) { Show(owner, new AlertInfo(caption, text)); }
		public void Show(Form owner, string caption, string text, string hotTrackedText) { Show(owner, new AlertInfo(caption, text, hotTrackedText)); }
		public void Show(Form owner, string caption, string text, Image image) { Show(owner, new AlertInfo(caption, text, null, image)); }
		public void Show(Form owner, string caption, string text, string hotTrackedText, Image image) { Show(owner, new AlertInfo(caption, text, hotTrackedText, image)); }
		public void Show(Form owner, string caption, string text, string hotTrackedText, Image image, object tag) { Show(owner, new AlertInfo(caption, text, hotTrackedText, image, tag)); }
		public void Show(Form owner, AlertInfo info) {
			StartPostponedForms();
			AlertForm frm = CreateAlertForm(GetCurrentLocation(owner), this, info);
			AlertFormEventArgs args = new AlertFormEventArgs(frm);
			RaiseBeforeFormShow(args);
			if(!args.Cancel) {
				if(this.AlertFormList.Count >= FormMaxCount && FormMaxCount > 0) {
					frm.SetPosponed(owner, args.Location);
					postponedForms.Add(frm);
					timer.Start();
				}
				else
					frm.ShowForm(owner, args.Location);
			}
			else frm.Dispose();
		}
		void timer_Tick(object sender, EventArgs e) {
			StartPostponedForms();
		}
		void StartPostponedForms() {
			if(postponedForms.Count == 0) {
				timer.Stop();
			}
			else {
				AlertForm aForm = postponedForms[0];
				if(this.AlertFormList.Count < FormMaxCount || FormMaxCount == 0) {
					postponedForms.RemoveAt(0);
					if(!aForm.IsDisposed)
						aForm.ShowForm(aForm.PostponedArgs.Owner, aForm.PostponedArgs.Location);
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<AlertForm> PostponedFormList {
			get { return postponedForms; }
		}
		protected virtual AlertForm CreateAlertForm(Point location, AlertControl control, AlertInfo info) {
			return new AlertForm(location, control, info);
		}
		public bool RaiseAlertClick(AlertInfo info, AlertFormCore form) {
			if(AlertClick == null) return false;
			AlertClickEventArgs args = new AlertClickEventArgs(info, form);
			AlertClick(this, args);
			return args.ActivateOwner;
		}
		public void RaiseButtonClick(AlertButton bi, AlertInfo info, AlertFormCore form) {
			if(ButtonClick == null || bi == null || bi.Predefined) return;
			ButtonClick(this, new AlertButtonClickEventArgs(bi, info, form));
		}
		public void RaiseButtonDownChanged(AlertButton bi, AlertInfo info, AlertFormCore form) {
			if(ButtonDownChanged == null || bi == null || bi.Predefined) return;
			ButtonDownChanged(this, new AlertButtonDownChangedEventArgs(bi, info, form));
		}
		protected internal void RaiseBeforeFormShow(AlertFormEventArgs args) {
			if(BeforeFormShow == null) return;
			BeforeFormShow(this, args);
		}
		public void RaiseFormLoad(AlertFormCore form) {
			if(FormLoad == null) return;
			FormLoad(this, new AlertFormLoadEventArgs(form));
		}
		public void RaiseFormClosing(AlertFormClosingEventArgs args) {
			if(FormClosing == null) return;
			FormClosing(this, args);
		}
		public int RaiseGetDesiredAlertFormWidth(AlertFormWidthEventArgs args) {
			if(GetDesiredAlertFormWidth == null)
				return -1;
			GetDesiredAlertFormWidth(this, args);
			return args.Width;
		}
		public bool RaiseMouseFromEnter(AlertEventArgs args) {
			if(MouseFormEnter == null) return false;
			MouseFormEnter(this, args);
			return args.Cancel;
		}
		public bool RaiseMouseFromLeave(AlertEventArgs args) {
			if(MouseFormLeave == null) return false;
			MouseFormLeave(this, args);
			return args.Cancel;
		}
		#region ISupportLookAndFeel Members
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IgnoreChildren { get { return false; } }
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		 Category(CategoryName.Appearance), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AlertControlLookAndFeel")
#else
	Description("")
#endif
]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		#endregion
		Internal.IPredefinedToastNotification DevExpress.Utils.MVVM.Services.INotificationProvider.CreateNotification(string header, string body, string body2, Bitmap image) {
			return new CustomNotification(this, header, body, body2, image);
		}
		class CustomNotification : DevExpress.Internal.IPredefinedToastNotification {
			AlertForm alertForm;
			AlertControl alertControl;
			TaskCompletionSource<Internal.ToastNotificationResultInternal> source;
			public CustomNotification(AlertControl alertControl, string header, string body, string body2, Bitmap image) {
				this.alertControl = alertControl;
				alertForm = alertControl.CreateAlertForm(alertControl.GetCurrentLocation(null), alertControl, new AlertInfo(header, body, body2, image));
			}
			void RegisterEvents() {
				alertControl.AlertClick += AlertControl_AlertClick;
				alertControl.FormClosing += AlertControl_FormClosing;
				Application.ApplicationExit += Application_ApplicationExit;
			}
			void UnregisterEvents() {
				alertControl.AlertClick -= AlertControl_AlertClick;
				alertControl.FormClosing -= AlertControl_FormClosing;
				Application.ApplicationExit -= Application_ApplicationExit;
			}
			void Application_ApplicationExit(object sender, EventArgs e) {
				SetTaskResult(Internal.ToastNotificationResultInternal.ApplicationHidden);
			}
			void AlertControl_AlertClick(object sender, AlertClickEventArgs e) {
				UnregisterEvents();
			}
			void AlertControl_FormClosing(object sender, AlertFormClosingEventArgs e) {
				switch(e.CloseReason) {
					case AlertFormCloseReason.TimeUp:
						SetTaskResult(Internal.ToastNotificationResultInternal.TimedOut);
						break;
					case AlertFormCloseReason.UserClosing:
						SetTaskResult(Internal.ToastNotificationResultInternal.UserCanceled);
						break;
					default:
						break;
				}
			}
			void SetTaskResult(Internal.ToastNotificationResultInternal value) {
				if(source == null) return;
				lock(source) {
					source.TrySetResult(value);
				}
				UnregisterEvents();
			}
			public void Hide() {
				AlertControl_FormClosing(null, new AlertFormClosingEventArgs(null, AlertFormCloseReason.UserClosing));
				this.alertForm.Close();
			}
			public System.Threading.Tasks.Task<Internal.ToastNotificationResultInternal> ShowAsync() {
				source = new TaskCompletionSource<Internal.ToastNotificationResultInternal>();
				RegisterEvents();
				alertForm.ShowForm(null);
				return source.Task;
			}
		}
	}
	public delegate void AlertClickEventHandler(object sender, AlertClickEventArgs e);
	public delegate void AlertButtonClickEventHandler(object sender, AlertButtonClickEventArgs e);
	public delegate void AlertButtonDownChangedEventHandler(object sender, AlertButtonDownChangedEventArgs e);
	public delegate void AlertFormEventHandler(object sender, AlertFormEventArgs e);
	public delegate void AlertEventHandler(object sender, AlertEventArgs e);
	public delegate void AlertFormClosingEventHandler(object sender, AlertFormClosingEventArgs e);
	public delegate void AlertFormLoadEventHandler(object sender, AlertFormLoadEventArgs e);
	public delegate void GetDesiredFormWidthEventHandler(object sender, AlertFormWidthEventArgs e);
	public class AlertClickEventArgs : EventArgs {
		AlertInfo info;
		AlertFormCore form;
		bool activateOwner = true;
		public AlertClickEventArgs(AlertInfo info, AlertFormCore form) {
			this.info = info;
			this.form = form;
		}
		public AlertInfo Info { get { return info; } }
		public AlertFormCore AlertForm { get { return form; } }
		public bool ActivateOwner {
			get { return activateOwner; }
			set { activateOwner = value; }
		}
	}
	public class AlertButtonClickEventArgs : AlertClickEventArgs {
		AlertButton bi;
		public AlertButtonClickEventArgs(AlertButton bi, AlertInfo info, AlertFormCore form)
			: base(info, form) {
			this.bi = bi;
		}
		public AlertButton Button { get { return bi; } }
		public string ButtonName { get { return bi.Name; } }
	}
	public class AlertButtonDownChangedEventArgs : AlertButtonClickEventArgs {
		public AlertButtonDownChangedEventArgs(AlertButton bi, AlertInfo info, AlertFormCore form) : base(bi, info, form) { }
		public bool Down {
			get { return Button.Down; }
			set {
				Button.Down = value;
			}
		}
	}
	public class AlertFormLoadEventArgs : EventArgs {
		AlertFormCore form;
		public AlertFormLoadEventArgs(AlertFormCore form) {
			this.form = form;
		}
		public AlertFormCore AlertForm { get { return form; } }
		public AlertButtonCollection Buttons {
			get {
				if(form != null) return form.Buttons;
				return null;
			}
		}
	}
}
