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

using DevExpress.Utils.Paint;
using DevExpress.Data.Camera;
using DevExpress.Data.Camera.Interfaces;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Camera {
	public enum VideoStretchMode { Default, ZoomOutside, ZoomInside, Stretch }
	internal enum CameraDeviceErrorState { Normal, Lost, IsBusy }
	[DXToolboxItem(true)]
	[ToolboxTabName(AssemblyInfo.DXTabCommon)]
	[ToolboxBitmap(typeof(ToolboxIconsRootNS), "CameraControl")]
	[Description("Displays a video stream captured from a video input device, such as a webcam.")]
	[Designer("DevExpress.XtraEditors.Design.CameraControlDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),]
	public class CameraControl : BaseStyleControl, ICameraDeviceClient, IContextItemCollectionOptionsOwner, IContextItemCollectionOwner {
		private static readonly object contextButtonCustomize = new object();
		private static readonly object contextButtonClick = new object();
		private static readonly object deviceChanged = new object();
		public CameraControl() {
			LockFireChanged();
			contextButtonOptions = CreateContextButtonOptions();
			UnlockFireChanged();
		}
		bool autoStartDefaultDevice = true;
		[DefaultValue(true), DXCategory(CategoryName.Behavior)]
		public bool AutoStartDefaultDevice {
			get { return autoStartDefaultDevice; }
			set { autoStartDefaultDevice = value; }
		}
		string deviceNotFoundString = string.Empty;
		[DefaultValue(""), DXCategory(CategoryName.Options)]
		public string DeviceNotFoundString {
			get { return deviceNotFoundString; }
			set {
				if(deviceNotFoundString == value) return;
				deviceNotFoundString = value;
				OnPropertiesChanged();
			}
		}
		bool showSettingsButton = true;
		[DefaultValue(true), DXCategory(CategoryName.Appearance)]
		public bool ShowSettingsButton {
			get { return showSettingsButton; }
			set {
				if(showSettingsButton == value) return;
				showSettingsButton = value;
				UpdateContextButtons();
			}
		}
		VideoStretchMode stretchMode = VideoStretchMode.Default;
		void ResetVideoStretchMode() { VideoStretchMode = VideoStretchMode.Default; }
		bool ShouldSerializeVideoStretchMode() { return VideoStretchMode != VideoStretchMode.Default; }
		[DXCategory(CategoryName.Options)]
		public VideoStretchMode VideoStretchMode {
			get { return stretchMode; }
			set {
				if(stretchMode == value) return;
				stretchMode = value;
				OnPropertiesChanged();
			}
		}
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			OnPropertiesChanged();
		}
		void UpdateContextButtons() {
			CameraViewInfo.ResetContextButtonsViewInfo();
			CameraViewInfo.ResetContextButtonsHandler();
		}
		DeviceVideoSettings videoSettings;
		[Browsable(false), DXCategory(CategoryName.Options)]
		public DeviceVideoSettings VideoSettings {
			get {
				if(videoSettings == null)
					videoSettings = new DeviceVideoSettings(this);
				return videoSettings;
			}
		}
		CameraDeviceErrorState errorState = CameraDeviceErrorState.Normal;
		internal CameraDeviceErrorState ErrorState {
			get { return errorState; }
			set {
				if(errorState == value) return;
				errorState = value;
				OnPropertiesChanged();
			}
		}
		void ResetErrorState() { errorState = CameraDeviceErrorState.Normal; }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				StopDevice(Device);
				DestroySettingsForm();
			}
			base.Dispose(disposing);
		}
		CameraDevice _device;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CameraDevice Device {
			get { return _device; }
			set {
				if(_device != null && _device.Equals(value)) return;
				var oldDevice = Device;
				_device = value;
				OnDeviceChanged(oldDevice, value);
			}
		}
		CameraDeviceBase ICameraDeviceClient.Device { get { return Device; } }
		protected virtual void OnDeviceChanged(CameraDevice oldDevice, CameraDevice newDevice) {
			StopDevice(oldDevice);
			ResetErrorState();
			OnPropertiesChanged();
			RaiseDeviceChanged();
		}
		void StartDevice(CameraDevice device) {
			ResetErrorState();
			if(device != null) {
				device.SetClient(this);
				device.Start();
				OnPropertiesChanged();
			}
		}
		void StopDevice(CameraDevice device) {
			if(device != null) {
				device.Stop();
				device.SetClient(null);
				OnPropertiesChanged();
			}
		}
		public void Start() {
			StartDevice(Device);
		}
		public void Start(CameraDevice device) {
			Device = device;
			StartDevice(Device);
		}
		public void Stop() {
			StopDevice(Device);
		}
		public Bitmap TakeSnapshot() {
			if(Device != null) {
				return Device.TakeSnapshot();
			}
			return null;
		}
		public static CameraDevice GetDefaultDevice() {
			var devices = GetDevices();
			if(devices.Count > 0)
				return GetDevice(devices[0]);
			return null;
		}
		public static CameraDevice GetDevice(CameraDeviceInfo deviceInfo) {
			if(deviceInfo != null && !string.IsNullOrEmpty(deviceInfo.MonikerString))
				return new CameraDevice(deviceInfo);
			return null;
		}
		public static List<CameraDeviceInfo> GetDevices() {
			return DeviceHelper.GetDevices();
		}
		void ICameraDeviceClient.OnNewFrame() {
			if(!this.IsHandleCreated) return;
			this.BeginInvoke(new Action(() => { this.Invalidate(); }));
		}
		void ICameraDeviceClient.OnDeviceLost(CameraDeviceBase lostDevice) {
			lostDevice = lostDevice as CameraDevice;
			if(!this.IsHandleCreated) return;
			Stop();
			if(lostDevice != null && lostDevice.IsBusy)
				ErrorState = CameraDeviceErrorState.IsBusy;
			else
				ErrorState = CameraDeviceErrorState.Lost;
			RaiseDeviceChanged();
		}
		void ICameraDeviceClient.OnResolutionChanged() {
			OnPropertiesChanged();
		}
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			if(_device != null)
				_device.WndProc(ref m);
			base.WndProc(ref m);
		}
		protected override BaseControlPainter CreatePainter() {
			return new CameraControlPainter();
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new CameraControlViewInfo(this);
		}
		protected virtual CameraControlHandler CreateHandler() {
			return new CameraControlHandler(this);
		}
		CameraControlHandler handler;
		protected internal CameraControlHandler Handler {
			get {
				if(handler == null)
					handler = CreateHandler();
				return handler;
			}
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			Handler.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			Handler.OnMouseLeave(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			Handler.OnMouseMove(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			Handler.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			Handler.OnMouseUp(e);
		}
		public void OnOptionsChanged(string propertyName, object oldValue, object newValue) {
			OnPropertiesChanged();
		}
		CameraControlViewInfo CameraViewInfo { get { return ViewInfo as CameraControlViewInfo; } }
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			if(CanShowToolTip) {
				var contextToolTipInfo = CameraViewInfo.ContextButtonsViewInfo.GetToolTipInfo(point);
				if(contextToolTipInfo != null)
					return contextToolTipInfo;
				return base.GetToolTipInfo(point);
			}
			return null;
		}
		protected virtual bool CanShowToolTip { get { return !IsDesignMode && ShowToolTips; } }
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			DoAutoStart();
		}
		void DoAutoStart() {
			if(!AutoStartDefaultDevice || Device != null || IsDesignMode) return;
			var device = GetDefaultDevice();
			if(device != null)
				Start(device);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(!Visible) DestroySettingsForm();
		}
		#region ContextButtons
		ContextAnimationType IContextItemCollectionOptionsOwner.AnimationType {
			get { return ContextAnimationType.OpacityAnimation; }
		}
		bool IContextItemCollectionOwner.IsRightToLeft {
			get { return IsRightToLeft; }
		}
		void IContextItemCollectionOwner.OnCollectionChanged() {
			UpdateContextButtons();
		}
		void IContextItemCollectionOwner.OnItemChanged(ContextItem item, string propertyName, object oldValue, object newValue) {
			OnPropertiesChanged();
		}
		ContextItemCollection contextButtons;
		[Category(CategoryName.Options)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ContextItemCollection ContextButtons {
			get {
				if(contextButtons == null) {
					contextButtons = CreateContextButtons();
					contextButtons.Options = ContextButtonOptions;
				}
				return contextButtons;
			}
		}
		ContextItemCollectionOptions contextButtonOptions;
		[Category(CategoryName.Options)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public ContextItemCollectionOptions ContextButtonOptions {
			get {
				if(contextButtonOptions == null)
					contextButtonOptions = CreateContextButtonOptions();
				return contextButtonOptions;
			}
		}
		protected virtual ContextItemCollection CreateContextButtons() {
			return new ContextItemCollection(this);
		}
		protected virtual ContextItemCollectionOptions CreateContextButtonOptions() {
			return new CameraContextButtonOptions(this);
		}
		protected internal void RaiseContextItemClick(ContextItemClickEventArgs e) {
			ContextItemClickEventHandler handler = Events[contextButtonClick] as ContextItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseContextButtonCustomize(ContextItem contextItem) {
			CameraContextButtonCustomizeEventHandler handler = Events[contextButtonCustomize] as CameraContextButtonCustomizeEventHandler;
			if(handler != null)
				handler(this, new CameraContextButtonCustomizeEventArgs() { Item = contextItem });
		}
		protected internal void RaiseDeviceChanged() {
			CameraDeviceChangedEventHandler handler = Events[deviceChanged] as CameraDeviceChangedEventHandler;
			if(handler != null)
				handler(this, new CameraDeviceChangedEventArgs(Device));
		}
		public event CameraContextButtonCustomizeEventHandler ContextButtonCustomize {
			add { Events.AddHandler(contextButtonCustomize, value); }
			remove { Events.RemoveHandler(contextButtonCustomize, value); }
		}
		public event ContextItemClickEventHandler ContextButtonClick {
			add { Events.AddHandler(contextButtonClick, value); }
			remove { Events.RemoveHandler(contextButtonClick, value); }
		}
		public event CameraDeviceChangedEventHandler DeviceChanged {
			add { Events.AddHandler(deviceChanged, value); }
			remove { Events.RemoveHandler(deviceChanged, value); }
		}
		#endregion ContextButtons
		#region SettingsForm
		protected internal void OnSettingsButtonClick() {
			ShowSettingsForm();
		}
		public void ShowSettingsForm() {
			DestroySettingsForm();
			settingsForm = CreateSettingsForm();
			UpdateSettingsFormLocation(settingsForm);
			settingsForm.Show(this);
		}
		XtraForm settingsForm;
		protected virtual XtraForm CreateSettingsForm() {
			CameraSettingsControl csc = new CameraSettingsControl();
			csc.OwnerControl = this;
			csc.Dock = DockStyle.Fill;
			XtraForm form = new XtraForm();
			form.Text = Localizer.Active.GetLocalizedString(StringId.CameraSettingsCaption);
			form.ClientSize = csc.Size;
			form.Controls.Add(csc);
			form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			form.StartPosition = FormStartPosition.Manual;
			form.MaximizeBox = false;
			return form;
		}
		protected virtual void UpdateSettingsFormLocation(XtraForm form) {
			Point pt = this.PointToScreen(Point.Empty);
			pt.X += this.Width;
			if(pt.X + form.Width > Screen.FromControl(this).Bounds.Right)
				pt.X -= form.Width;
			form.Location = pt;
		}
		void DestroySettingsForm() {
			if(settingsForm != null && !settingsForm.IsDisposed) {
				settingsForm.Dispose();
			}
			settingsForm = null;
		}
		#endregion SettingsForm
	}
	public class CameraDeviceChangedEventArgs : EventArgs {
		public CameraDeviceChangedEventArgs(CameraDevice device) { this.Device = device; }
		public CameraDevice Device { get; private set; }
	}
	public class CameraContextButtonCustomizeEventArgs : EventArgs {
		public ContextItem Item { get; internal set; }
	}
	public delegate void CameraDeviceChangedEventHandler(object sender, CameraDeviceChangedEventArgs e);
	public delegate void CameraContextButtonCustomizeEventHandler(object sender, CameraContextButtonCustomizeEventArgs e);
	public class CameraControlViewInfo : BaseStyleControlViewInfo, ISupportContextItems {
		public CameraControlViewInfo(BaseStyleControl owner) : base(owner) { }
		CameraControl CamControl { get { return Owner as CameraControl; } }
		CameraDevice CurrentDevice { get { return CamControl.Device; } }
		public bool CanDrawStreaming { get { return CurrentDevice != null && CurrentDevice.IsRunning; } }
		public Bitmap CurrentFrame { get { return CurrentDevice.CurrentFrame; } }
		public Rectangle FrameRect { get; private set; }
		protected override void CalcContentRect(Rectangle bounds) {
			bounds = CalcContentPadding(bounds);
			base.CalcContentRect(bounds);
			if(CurrentDevice != null && CurrentFrame != null) {
				FrameRect = CalcFrameRect(CurrentFrame.Size);
			}
			else {
				FrameRect = Rectangle.Empty;
			}
			CalcContextButtonsViewInfo();
		}
		Rectangle CalcContentPadding(Rectangle bounds) {
			Padding p = OwnerControl.Padding;
			int w = Math.Max(0, bounds.Width - p.Horizontal);
			int h = Math.Max(0, bounds.Height - p.Vertical);
			bounds.Width = w;
			bounds.Height = h;
			bounds.X += p.Left;
			bounds.Y += p.Top;
			return bounds;
		}
		Rectangle CalcFrameRect(Size frameSize) {
			Rectangle result = Rectangle.Empty;
			float scaleX = ((float)ContentRect.Width) / frameSize.Width;
			float scaleY = ((float)ContentRect.Height) / frameSize.Height;
			float currScale;
			bool zoomInside = CamControl.VideoStretchMode == VideoStretchMode.ZoomInside || CamControl.VideoStretchMode == VideoStretchMode.Default;
			bool stretch = CamControl.VideoStretchMode == VideoStretchMode.Stretch;
			if(zoomInside) 
				currScale = scaleX > scaleY ? scaleY : scaleX; 
			else
				currScale = scaleX > scaleY ? scaleX : scaleY; 
			frameSize = new Size(
				(int)(frameSize.Width * currScale + 0.5f),
				(int)(frameSize.Height * currScale + 0.5f));
			result.Size = frameSize;
			result.Location = new Point(
				ContentRect.X + ContentRect.Width / 2 - frameSize.Width / 2,
				ContentRect.Y + ContentRect.Height / 2 - frameSize.Height / 2);
			if(stretch) {
				result = ContentRect;
			}
			return result;
		}
		public string InfoString {
			get {
				if(CamControl != null) {
					if(CamControl.IsDesignMode)
						return Localizer.Active.GetLocalizedString(StringId.CameraDesignTimeInfo);
					if(CamControl.ErrorState == CameraDeviceErrorState.IsBusy)
						return Localizer.Active.GetLocalizedString(StringId.CameraDeviceIsBusy);
					return string.IsNullOrEmpty(CamControl.DeviceNotFoundString) ? Localizer.Active.GetLocalizedString(StringId.CameraDeviceNotFound) : CamControl.DeviceNotFoundString;
				}
				return string.Empty;
			}
		}
		AppearanceObject infoStringAppearance;
		public virtual AppearanceObject InfoStringAppearance{
			get {
				if(infoStringAppearance == null) 
					infoStringAppearance = new AppearanceObject();
				return infoStringAppearance;
			}
		}
		StringFormat infoStringStringFormat;
		public virtual StringFormat InfoStringStringFormat {
			get {
				if(infoStringStringFormat == null) {
					infoStringStringFormat = new StringFormat();
					infoStringStringFormat.Alignment = StringAlignment.Center;
					infoStringStringFormat.LineAlignment = StringAlignment.Center;
				}
				return infoStringStringFormat;
			}
		}
		#region ContextButtons
		Rectangle ISupportContextItems.ActivationBounds { get { return ClientRect; } }
		bool ISupportContextItems.CloneItems { get { return true; } }
		ContextItemCollection ISupportContextItems.ContextItems { get { return CamControl.ContextButtons; } }
		Control ISupportContextItems.Control { get { return CamControl; } }
		bool ISupportContextItems.DesignMode { get { return CamControl.IsDesignMode; } }
		Rectangle ISupportContextItems.DisplayBounds { get { return ContentRect; } }
		Rectangle ISupportContextItems.DrawBounds { get { return ContentRect; } }
		ItemHorizontalAlignment ISupportContextItems.GetCaptionHorizontalAlignment(ContextButton btn) { return ItemHorizontalAlignment.Left; }
		ItemVerticalAlignment ISupportContextItems.GetCaptionVerticalAlignment(ContextButton btn) { return ItemVerticalAlignment.Center; }
		ItemHorizontalAlignment ISupportContextItems.GetGlyphHorizontalAlignment(ContextButton btn) { return ItemHorizontalAlignment.Left; }
		ItemLocation ISupportContextItems.GetGlyphLocation(ContextButton btn) { return ItemLocation.Left; }
		int ISupportContextItems.GetGlyphToCaptionIndent(ContextButton btn) { return 3; }
		ItemVerticalAlignment ISupportContextItems.GetGlyphVerticalAlignment(ContextButton btn) { return ItemVerticalAlignment.Center; }
		UserLookAndFeel ISupportContextItems.LookAndFeel { get { return CamControl.LookAndFeel; } }
		ContextItemCollectionOptions ISupportContextItems.Options { get { return CamControl.ContextButtonOptions; } }
		void ISupportContextItems.RaiseContextItemClick(ContextItemClickEventArgs e) {
			if(CamControl != null)
				CamControl.RaiseContextItemClick(e);
		}
		void ISupportContextItems.RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e) {
		}
		void ISupportContextItems.RaiseCustomizeContextItem(ContextItem item) {
			if(CamControl != null)
				CamControl.RaiseContextButtonCustomize(item);
		}
		void ISupportContextItems.Redraw(Rectangle rect) {
			if(CamControl == null) return;
			CamControl.Invalidate(rect);
		}
		void ISupportContextItems.Update() { 
			if(CamControl == null) return;
			CamControl.Update();
		}
		void ISupportContextItems.Redraw() {
			if(CamControl == null) return;
			CamControl.Invalidate(ClientRect);
		}
		bool ISupportContextItems.ShowOutsideDisplayBounds { get { return false; } }
		public void ResetContextButtonsViewInfo() { contextButtonsViewInfo = null; }
		public void ResetContextButtonsHandler() { contextButtonsHandler = null; }
		ContextItemCollectionViewInfo contextButtonsViewInfo;
		protected internal ContextItemCollectionViewInfo ContextButtonsViewInfo {
			get {
				if(contextButtonsViewInfo == null)
					contextButtonsViewInfo = new ContextItemCollectionViewInfo(GetButtons(), ((ISupportContextItems)this).Options, this);
				return contextButtonsViewInfo;
			}
		}
		ContextItemCollectionHandler contextButtonsHandler;
		protected internal ContextItemCollectionHandler ContextButtonsHandler {
			get {
				if(contextButtonsHandler == null)
					contextButtonsHandler = new ContextItemCollectionHandler(ContextButtonsViewInfo);
				return contextButtonsHandler;
			}
		}
		Rectangle PrevDisplayBounds { get; set; }
		protected virtual void CalcContextButtonsViewInfo() {
			if(((ISupportContextItems)this).ContextItems == null)
				return;
			if(PrevDisplayBounds != ((ISupportContextItems)this).DisplayBounds) {
				ContextButtonsViewInfo.InvalidateViewInfo();
			}
			PrevDisplayBounds = ((ISupportContextItems)this).DisplayBounds;
			ContextButtonsViewInfo.CalcItems();
		}
		protected virtual ContextItemCollection GetButtons() {
			ContextItemCollection result = new ContextItemCollection(CamControl);
			AddDefaultButtons(result);
			foreach(ICloneable userButton in ((ISupportContextItems)this).ContextItems) {
				result.Add((ContextItem)userButton.Clone());
			}
			return result;
		}
		protected virtual void AddDefaultButtons(ContextItemCollection coll) {
			if(CamControl.ShowSettingsButton) {
				var settingsItem = new CameraSettingsContextButton();
				coll.Add(settingsItem);
			}
		}
		#endregion ContextButtons
	}
	public class CameraControlHandler {
		public CameraControl Control { get; private set; }
		public CameraControlViewInfo ControlInfo { get { return Control.ViewInfo as CameraControlViewInfo; } }
		public ContextItemCollectionHandler ContextButtonsHandler { get { return ControlInfo.ContextButtonsHandler; } }
		public CameraControlHandler(CameraControl control) {
			this.Control = control;
		}
		public virtual void OnMouseEnter(EventArgs e) {
			ContextButtonsHandler.OnMouseEnter(e);
		}
		public virtual void OnMouseLeave(EventArgs e) {
			ContextButtonsHandler.OnMouseLeave(e);
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			ContextButtonsHandler.OnMouseMove(e);
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			ContextButtonsHandler.OnMouseDown(e);
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			ContextButtonsHandler.OnMouseUp(e);
		}
	}
	public class CameraControlPainter : BaseControlPainter {
		CameraControlViewInfo GetViewInfo(ControlGraphicsInfoArgs info) {
			if(info == null) return null;
			return info.ViewInfo as CameraControlViewInfo;
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			var vInfo = GetViewInfo(info);
			if(vInfo.CanDrawStreaming)
				DrawStreaming(info, vInfo);
			else
				DrawInfoString(info, vInfo);
			DrawContextButtons(info, vInfo);
		}
		void DrawInfoString(ControlGraphicsInfoArgs info, CameraControlViewInfo vInfo) {
			vInfo.InfoStringAppearance.DrawString(info.Cache, vInfo.InfoString, vInfo.ClientRect, vInfo.InfoStringStringFormat);
		}
		void DrawContextButtons(ControlGraphicsInfoArgs info, CameraControlViewInfo vInfo) {
			if((vInfo as ISupportContextItems).ContextItems != null)
				new ContextItemCollectionPainter().Draw(new ContextItemCollectionInfoArgs(vInfo.ContextButtonsViewInfo, info, vInfo.ClientRect));
		}
		void DrawStreaming(ControlGraphicsInfoArgs info, CameraControlViewInfo vInfo) {
			DrawCurrentFrame(info, vInfo);
		}
		void DrawCurrentFrame(ControlGraphicsInfoArgs info, CameraControlViewInfo vInfo) {
			if(vInfo != null && vInfo.CurrentFrame != null) {
				info.Graphics.DrawImage(vInfo.CurrentFrame, vInfo.FrameRect);
			}
		}
	}
	class CameraSettingsContextButton : ContextButton {
		public CameraControl Control { get { return base.CollectionOwner as CameraControl; } }
		public CameraSettingsContextButton() {
			this.Glyph = SettingsGlyph;
			this.Alignment = ContextItemAlignment.TopFar;
		}
		public override void RaiseContextItemClick(MouseEventArgs e, ContextItemViewInfo itemInfo) {
			OnClick();
			base.RaiseContextItemClick(e, itemInfo);
		}
		void OnClick() {
			var ctrl = GetControl();
			if(ctrl != null)
				ctrl.OnSettingsButtonClick();
		}
		CameraControl GetControl() {
			var originItem = OriginItem as CameraSettingsContextButton;
			if(originItem != null) return originItem.Control;
			return null;
		}
		Image settingsGlyph;
		Image SettingsGlyph {
			get {
				if(settingsGlyph == null)
					settingsGlyph = Image.FromStream(GetType().Assembly.GetManifestResourceStream("DevExpress.XtraEditors.Camera.Settings.png"));
				return settingsGlyph;
			}
		}
	}
	public class CameraContextButtonOptions : ContextItemCollectionOptions {
		public CameraContextButtonOptions() : this(null) { }
		public CameraContextButtonOptions(IContextItemCollectionOptionsOwner owner)
			: base(owner) {
			this.NormalStateOpacity = defaultNormalOpacity;
			this.TopPanelPadding = defaultTopPanelPadding;
			this.BottomPanelPadding = defaultBottomPanelPadding;
		}
		float defaultNormalOpacity = 0.5f;
		[DefaultValue(0.5f)]
		public new float NormalStateOpacity {
			get { return base.NormalStateOpacity; }
			set { base.NormalStateOpacity = value; }
		}
		Padding defaultTopPanelPadding = new Padding(5);
		void ResetTopPanelPadding() { TopPanelPadding = defaultTopPanelPadding; }
		bool ShouldSerializeTopPanelPadding() { return TopPanelPadding != defaultTopPanelPadding; }
		public new Padding TopPanelPadding {
			get { return base.TopPanelPadding; }
			set { base.TopPanelPadding = value; }
		}
		Padding defaultBottomPanelPadding = new Padding(5);
		void ResetBottomPanelPadding() { BottomPanelPadding = defaultBottomPanelPadding; }
		bool ShouldSerializeBottomPanelPadding() { return BottomPanelPadding != defaultBottomPanelPadding; }
		public new Padding BottomPanelPadding {
			get { return base.BottomPanelPadding; }
			set { base.BottomPanelPadding = value; }
		}
	}
	[System.Security.SecuritySafeCritical]
	public class CameraDevice : CameraDeviceBase {
		public CameraDevice(CameraDeviceInfo deviceInfo)
			: base(deviceInfo) {
		}
		public Bitmap CurrentFrame { get; private set; }
		protected override void CreateFrameCore(IntPtr section, int width, int height, IntPtr stride) {
			this.CurrentFrame = new Bitmap(width, height, width * BitsPerPixel / 8, CurrentPixelFormat, stride);
		}
		protected override void FreeFrame() {
			if(this.CurrentFrame != null) {
				this.CurrentFrame.Dispose();
				this.CurrentFrame = null;
			}
		}
	}
}
