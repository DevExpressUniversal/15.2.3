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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Data.Camera;
using DevExpress.Data.Camera.Interfaces;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Internal;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Flyout;
using DevExpress.Xpf.Utils;
using Size = System.Windows.Size;
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(true)]
	[ToolboxTabName(AssemblyInfo.DXTabCommon)]
	public class CameraControl : Control, ICameraDeviceClient {
		public static readonly DependencyProperty AllowAutoStartProperty;
		public static readonly DependencyProperty ShowSettingsButtonProperty;
		public static readonly DependencyProperty StretchProperty;
		public static readonly DependencyProperty StretchDirectionProperty;
		public static readonly DependencyProperty DeviceProperty;
		static readonly DependencyPropertyKey DevicePropertyKey;
		public static readonly DependencyProperty DeviceSettingsProperty;
		static readonly DependencyPropertyKey DeviceSettingsPropertyKey;
		public static readonly DependencyProperty DeviceInfoProperty;
		public static readonly DependencyProperty BorderTemplateProperty;
		static readonly DependencyPropertyKey PropertyProviderPropertyKey;
		public static readonly DependencyProperty PropertyProviderProperty;
		public static readonly DependencyProperty ShowBorderProperty;
		static readonly DependencyPropertyKey NativeImageSourcePropertyKey;
		public static readonly DependencyProperty NativeImageSourceProperty;
		static CameraControl() {
			var ownerType = typeof(CameraControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			AllowAutoStartProperty = DependencyPropertyRegistrator.Register<CameraControl, bool>(owner => owner.AllowAutoStart, true);
			ShowSettingsButtonProperty = DependencyPropertyRegistrator.Register<CameraControl, bool>(owner => owner.ShowSettingsButton, true, (control, value, newValue) => control.ShowSettingsButtonChanged());
			StretchProperty = DependencyPropertyRegistrator.Register<CameraControl, Stretch>(owner => owner.Stretch, Stretch.Uniform, (control, value, newValue) => control.StretchChanged(value, newValue));
			StretchDirectionProperty = DependencyPropertyRegistrator.Register<CameraControl, StretchDirection>(owner => owner.StretchDirection, StretchDirection.Both, (control, value, newValue) => control.StretchDirectionChanged(value, newValue));
			DevicePropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<CameraControl, CameraDevice>(owner => owner.Device, null,
				(control, value, newValue) => control.DeviceChanged(value, newValue));
			DeviceProperty = DevicePropertyKey.DependencyProperty;
			DeviceSettingsPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<CameraControl, DeviceVideoSettings>(owner => owner.DeviceSettings, null);
			DeviceSettingsProperty = DeviceSettingsPropertyKey.DependencyProperty;
			DeviceInfoProperty = DependencyPropertyRegistrator.Register<CameraControl, DeviceInfo>(owner => owner.DeviceInfo, null,
				(control, value, newValue) => control.DeviceInfoChanged(value, newValue));
			BorderTemplateProperty = DependencyProperty.Register("BorderTemplate", typeof(ControlTemplate), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			PropertyProviderPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<CameraControl, CameraPropertyProvider>(owner => owner.PropertyProvider, null);
			PropertyProviderProperty = PropertyProviderPropertyKey.DependencyProperty;
			ShowBorderProperty = DependencyPropertyRegistrator.Register<CameraControl, bool>(owner => owner.ShowBorder, true);
			NativeImageSourcePropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<CameraControl, ImageSource>(owner => owner.NativeImageSource, null);
			NativeImageSourceProperty = NativeImageSourcePropertyKey.DependencyProperty;
		}
		CameraDevice device;
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		public ControlTemplate BorderTemplate {
			get { return (ControlTemplate)GetValue(BorderTemplateProperty); }
			set { SetValue(BorderTemplateProperty, value); }
		}
		public DeviceInfo DeviceInfo {
			get { return (DeviceInfo)GetValue(DeviceInfoProperty); }
			set { SetValue(DeviceInfoProperty, value); }
		}
		public DeviceVideoSettings DeviceSettings {
			get { return (DeviceVideoSettings)GetValue(DeviceSettingsProperty); }
			private set { SetValue(DeviceSettingsPropertyKey, value); }
		}
		CameraDeviceBase ICameraDeviceClient.Device { get { return Device; } }
		public CameraDevice Device {
			get { return (CameraDevice)GetValue(DeviceProperty); }
			private set { SetValue(DevicePropertyKey, value); }
		}
		public bool AllowAutoStart {
			get { return (bool)GetValue(AllowAutoStartProperty); }
			set { SetValue(AllowAutoStartProperty, value); }
		}
		public bool ShowSettingsButton {
			get { return (bool)GetValue(ShowSettingsButtonProperty); }
			set { SetValue(ShowSettingsButtonProperty, value); }
		}
		public Stretch Stretch {
			get { return (Stretch)GetValue(StretchProperty); }
			set { SetValue(StretchProperty, value); }
		}
		public StretchDirection StretchDirection {
			get { return (StretchDirection)GetValue(StretchDirectionProperty); }
			set { SetValue(StretchDirectionProperty, value); }
		}
		public CameraPropertyProvider PropertyProvider {
			get { return (CameraPropertyProvider)GetValue(PropertyProviderProperty); }
			private set { SetValue(PropertyProviderPropertyKey, value); }
		}
		public ImageSource NativeImageSource {
			get { return (ImageSource)GetValue(NativeImageSourceProperty); }
			private set { SetValue(NativeImageSourcePropertyKey, value); }
		}
		public ICommand StartCommand { get; private set; }
		public ICommand StopCommand { get; private set; }
		public CameraControl() {
			Loaded += CameraControlLoaded;
			Unloaded += CameraControlUnloaded;
			StartCommand = new DelegateCommand(() => Start());
			StopCommand = new DelegateCommand(() => Stop());
			PropertyProvider = new CameraPropertyProvider() { ShowSettingsButton = true, RefreshCommand = new DelegateCommand(new Action(Refresh)) };
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			var contentControl = LayoutHelper.FindElementByType<ContentControl>(this);
			var flyout = LayoutHelper.FindElementByType<FlyoutControl>(contentControl.Content as FrameworkElement);
			flyout.Opened += FlyoutOpened;
		}
		public ImageSource TakeSnapshot() {
			if (Device != null && Device.IsRunning) {
				var bitmap = (System.Drawing.Image)Device.TakeSnapshot();
				return ImageHelper.CreateImageSource(bitmap);
			}
			return null;
		}
		void ShowSettingsButtonChanged() {
			PropertyProvider.ShowSettingsButton = ShowSettingsButton;
		}
		void FlyoutOpened(object sender, EventArgs e) {
			PropertyProvider.Settings = new CameraSettingsProvider(this);
		}
		void CameraControlUnloaded(object sender, RoutedEventArgs e) {
			Stop();
			Device = null;
		}
		void CameraControlLoaded(object sender, RoutedEventArgs e) {
			if (this.IsInDesignTool())
				return;
			CreateDevice();
			UpdateHasDevices();
		}
		void CreateDevice() {
			if (DeviceInfo == null) {
				var defaultDevice = GetDefaultDevice();
				if (defaultDevice != null)
					SetCurrentValue(DeviceInfoProperty, new DeviceInfo() { Moniker = defaultDevice.MonikerString, Name = defaultDevice.Name });
			}
			else if (Device == null) {
				UpdateDevice(DeviceInfo);
			}
		}
		CameraDeviceInfo GetDefaultDevice() {
			return DeviceHelper.GetDevices().FirstOrDefault();
		}
		protected virtual void DeviceChanged(CameraDevice oldValue, CameraDevice newValue) {
			oldValue.Do(x => x.Dispose());
			device = newValue;
			if (newValue != null)
				NativeImageSource = newValue.BitmapSource;
		}
		protected virtual CameraDevice CreateDevice(DeviceInfo deviceInfo) {
			if (deviceInfo == null)
				return null;
			var device = new CameraDevice(new CameraDeviceInfo(deviceInfo.Moniker, deviceInfo.Name));
			device.SetClient(this);
			return device;
		}
		void DeviceInfoChanged(DeviceInfo oldValue, DeviceInfo newValue) {
			UpdateDevice(newValue);
		}
		void UpdateDevice(DeviceInfo info) {
			Device = CreateDevice(info);
			DeviceSettings = CreateDeviceSettings(Device);
			if (AllowAutoStart)
				Start();
		}
		DeviceVideoSettings CreateDeviceSettings(CameraDevice device) {
			if (device == null)
				return null;
			return new DeviceVideoSettings(this);
		}
		public virtual void Start() {
			Device.Do(x => x.Start());
			UpdateIsBusy();
			UpdateHasDevices();
		}
		void UpdateHasDevices() {
			PropertyProvider.HasDevices = (GetAvailableDevices() as IList).Return(x => x.Count, () => 0) > 0;
		}
		public virtual void Stop() {
			Device.Do(x => x.Stop());
		}
		void Refresh() {
			CreateDevice();
			Start();
		}
		void UpdateIsBusy() {
			if (Device != null)
				PropertyProvider.IsBusy = Device.IsBusy;
		}
		#region ICameraDeviceClient
		void ICameraDeviceClient.OnNewFrame() {
			if (this.Dispatcher != null) {
				this.Dispatcher.BeginInvoke(
					System.Windows.Threading.DispatcherPriority.Render,
					new Action(() => {
						if (Device == null || !Device.IsRunning)
							return;
						if (NativeImageSource != Device.BitmapSource)
							NativeImageSource = Device.BitmapSource;
						if (Device.BitmapSource != null)
							Device.BitmapSource.Invalidate();
					}));
			}
		}
		void ICameraDeviceClient.OnDeviceLost(CameraDeviceBase lostDevice) {
			UpdateIsBusy();
			Device = null;
		}
		void ICameraDeviceClient.OnResolutionChanged() { }
		IntPtr ICameraDeviceClient.Handle {
			get { return ((HwndSource)PresentationSource.FromDependencyObject(this)).Return((x) => x.Handle, () => IntPtr.Zero); }
		}
		#endregion
		protected virtual void StretchDirectionChanged(StretchDirection oldValue, StretchDirection newValue) {
		}
		protected virtual void StretchChanged(Stretch oldValue, Stretch newValue) {
		}
		public IEnumerable<DeviceInfo> GetAvailableDevices() {
			List<DeviceInfo> result = new List<DeviceInfo>();
			DeviceHelper.GetDevices().ForEach((x) => {
				if (DeviceInfo != null && DeviceInfo.Moniker == x.MonikerString && x.Name == DeviceInfo.Name)
					result.Add(DeviceInfo);
				else
					result.Add(new DeviceInfo() { Moniker = x.MonikerString, Name = x.Name });
			});
			return result;
		}
	}
	public class DeviceInfo : MarkupExtension {
		public string Moniker { get; set; }
		public string Name { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	[System.Security.SecuritySafeCritical]
	public class CameraDevice : CameraDeviceBase {
		public CameraDevice(CameraDeviceInfo deviceInfo)
			: base(deviceInfo) {
		}
		public InteropBitmap BitmapSource { get; private set; }
		protected override void CreateFrameCore(IntPtr section, int width, int height, IntPtr stride) {
			this.BitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromMemorySection(
							section,
							width,
							height,
							System.Windows.Media.PixelFormats.Bgr32,
							width * System.Windows.Media.PixelFormats.Bgr32.BitsPerPixel / 8,
							0) as InteropBitmap;
		}
		protected override void FreeFrame() {
			this.BitmapSource = null;
		}
	}
	public class BaseCameraSettings : ViewModelBase {
		public string Caption {
			get { return GetProperty<string>(() => Caption); }
			set { SetProperty<string>(() => Caption, value); }
		}
	}
	public class CollectionCameraSettings : BaseCameraSettings {
		public IEnumerable<object> AvaliableValues {
			get { return GetProperty<IEnumerable<object>>(() => AvaliableValues); }
			set { SetProperty<IEnumerable<object>>(() => AvaliableValues, value); }
		}
	}
	public class RangeCameraSettings : BaseCameraSettings {
		public int MinValue {
			get { return GetProperty<int>(() => MinValue); }
			set { SetProperty<int>(() => MinValue, value); }
		}
		public int MaxValue {
			get { return GetProperty<int>(() => MaxValue); }
			set { SetProperty<int>(() => MaxValue, value); }
		}
	}
	public class ResolutionItem {
		public ResolutionItem(System.Drawing.Size res) {
			Resolution = res;
		}
		public System.Drawing.Size Resolution { get; private set; }
		public string DisplayText {
			get { return string.Format("{0} x {1}", Resolution.Width, Resolution.Height); }
		}
	}
	public class CameraPropertyProvider : ViewModelBase {
		public CameraPropertyProvider() {
			ErrorCaption = EditorLocalizer.GetString(EditorStringId.CameraErrorCaption);
			RefreshButtonCaption = EditorLocalizer.GetString(EditorStringId.CameraRefreshButtonCaption);
			NoDevicesErrorCaption = EditorLocalizer.GetString(EditorStringId.CameraNoDevicesErrorCaption);
		}
		public string ErrorCaption {
			get { return GetProperty<string>(() => ErrorCaption); }
			set { SetProperty<string>(() => ErrorCaption, value); }
		}
		public string NoDevicesErrorCaption {
			get { return GetProperty<string>(() => NoDevicesErrorCaption); }
			set { SetProperty<string>(() => NoDevicesErrorCaption, value); }
		}
		public bool ShowSettingsButton {
			get { return GetProperty<bool>(() => ShowSettingsButton); }
			set {
				SetProperty<bool>(() => ShowSettingsButton, value);
				UpdateSettingsButtonVisiblity();
			}
		}
		public bool IsSettingsButtonVisible {
			get { return GetProperty<bool>(() => IsSettingsButtonVisible); }
			set { SetProperty<bool>(() => IsSettingsButtonVisible, value); }
		}
		public bool IsBusy {
			get { return GetProperty<bool>(() => IsBusy); }
			set {
				SetProperty<bool>(() => IsBusy, value);
				UpdateSettingsButtonVisiblity();
			}
		}
		public bool HasDevices {
			get { return GetProperty<bool>(() => HasDevices); }
			set {
				SetProperty<bool>(() => HasDevices, value);
				UpdateSettingsButtonVisiblity();
			}
		}
		public string RefreshButtonCaption {
			get { return GetProperty<string>(() => RefreshButtonCaption); }
			set { SetProperty<string>(() => RefreshButtonCaption, value); }
		}
		public CameraSettingsProvider Settings {
			get { return GetProperty<CameraSettingsProvider>(() => Settings); }
			set { SetProperty<CameraSettingsProvider>(() => Settings, value); }
		}
		public ICommand RefreshCommand { get; set; }
		void UpdateSettingsButtonVisiblity() {
			IsSettingsButtonVisible = ShowSettingsButton && !IsBusy && HasDevices;
		}
	}
	public class CameraSettingsProvider : ViewModelBase {
		public CameraSettingsProvider(CameraControl owner) {
			Owner = owner;
			Device = new CollectionCameraSettings() { Caption = EditorLocalizer.GetString(EditorStringId.CameraDeviceCaption), AvaliableValues = Owner.GetAvailableDevices() };
			ActualDevice = Owner.DeviceInfo;
		}
		public DeviceInfo ActualDevice {
			get { return GetProperty<DeviceInfo>(() => ActualDevice); }
			set {
				SetProperty<DeviceInfo>(() => ActualDevice, value);
				Owner.DeviceInfo = ActualDevice;
				SyncSettings();
			}
		}
		public System.Drawing.Size ActualResolution {
			get { return GetProperty<System.Drawing.Size>(() => ActualResolution); }
			set {
				SetProperty<System.Drawing.Size>(() => ActualResolution, value);
				if (value != Owner.Device.Resolution)
					Owner.Device.Resolution = value;
			}
		}
		public double ActualBrightness {
			get { return GetProperty<double>(() => ActualBrightness); }
			set {
				SetProperty<double>(() => ActualBrightness, value);
				if ((int)value != Owner.DeviceSettings.Brightness.Value)
					Owner.DeviceSettings.Brightness.Value = (int)value;
			}
		}
		public double ActualContrast {
			get { return GetProperty<double>(() => ActualContrast); }
			set {
				SetProperty<double>(() => ActualContrast, value);
				if ((int)value != Owner.DeviceSettings.Contrast.Value)
					Owner.DeviceSettings.Contrast.Value = (int)value;
			}
		}
		public bool CanDesaturate {
			get { return GetProperty<bool>(() => CanDesaturate); }
			set {
				SetProperty<bool>(() => CanDesaturate, value);
				var newValue = value ? Owner.DeviceSettings.Saturation.Min : Owner.DeviceSettings.Saturation.Max;
				if (newValue != Owner.DeviceSettings.Saturation.Value)
					Owner.DeviceSettings.Saturation.Value = newValue;
			}
		}
		public bool EnableSettings {
			get { return GetProperty<bool>(() => EnableSettings); }
			set { SetProperty<bool>(() => EnableSettings, value); }
		}
		public string SettingsCaption { get { return EditorLocalizer.GetString(EditorStringId.CameraSettingsCaption); } }
		public ICommand ResetToDefaultCommand { get; private set; }
		public CollectionCameraSettings Device { get; private set; }
		public CollectionCameraSettings Resolution { get; private set; }
		public RangeCameraSettings Brightness { get; private set; }
		public RangeCameraSettings Contrast { get; private set; }
		public BaseCameraSettings Reset { get; private set; }
		public BaseCameraSettings Desaturate { get; private set; }
		CameraControl Owner { get; set; }
		void SyncSettings() {
			var newDevice = Owner.Device;
			EnableSettings = newDevice != null;
			if (!EnableSettings) return;
			Resolution = new CollectionCameraSettings() { Caption = EditorLocalizer.GetString(EditorStringId.CameraResolutionCaption), AvaliableValues = GetAvaliableResolutions(newDevice) };
			ActualResolution = newDevice.Resolution;
			Brightness = new RangeCameraSettings() { Caption = EditorLocalizer.GetString(EditorStringId.CameraBrightnessCaption), MinValue = Owner.DeviceSettings.Brightness.Min, MaxValue = Owner.DeviceSettings.Brightness.Max, };
			ActualBrightness = Owner.DeviceSettings.Brightness.Value;
			Contrast = new RangeCameraSettings() { Caption = EditorLocalizer.GetString(EditorStringId.CameraContrastCaption), MinValue = Owner.DeviceSettings.Contrast.Min, MaxValue = Owner.DeviceSettings.Contrast.Max, };
			ActualContrast = Owner.DeviceSettings.Contrast.Value;
			Desaturate = new BaseCameraSettings() { Caption = EditorLocalizer.GetString(EditorStringId.CameraDesaturateCaption) };
			CanDesaturate = Owner.DeviceSettings.Saturation.Min == Owner.DeviceSettings.Saturation.Value;
			Reset = new BaseCameraSettings() { Caption = EditorLocalizer.GetString(EditorStringId.CameraResetButtonCaption) };
			ResetToDefaultCommand = new DelegateCommand(ResetToDefault);
		}
		void ResetToDefault() {
			Owner.DeviceSettings.Brightness.ResetToDefault();
			Owner.DeviceSettings.Contrast.ResetToDefault();
			Owner.DeviceSettings.Saturation.ResetToDefault();
			SyncSettings();
		}
		IEnumerable<object> GetAvaliableResolutions(CameraDevice newDevice) {
			List<ResolutionItem> result = new List<ResolutionItem>();
			newDevice.GetAvailiableResolutions().ForEach((x) => result.Add(new ResolutionItem(x)));
			return result;
		}
	}
}
