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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.Camera;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraEditors.Camera {
	[DXToolboxItem(false)]
	public partial class CameraSettingsControl : XtraUserControl {
		public CameraSettingsControl() {
			InitializeComponent();
			this.SizeChanged += CameraSettingsControl_SizeChanged;
			if(LookAndFeel.GetTouchUI()) {
				Scale(new SizeF(1.4f + LookAndFeel.GetTouchScaleFactor() / 10.0f, 1.4f + LookAndFeel.GetTouchScaleFactor() / 10.0f));
			}
			UpdatePanelLocation();
			Localizer.ActiveChanged += new EventHandler(LocalizatorChanged);
			LocalizatorChanged(this, null);
		}
		protected override void Dispose(bool disposing) {
			if(disposing ){
				if(components != null) {
					components.Dispose();
				}
				Localizer.ActiveChanged -= new EventHandler(LocalizatorChanged);
			}
			base.Dispose(disposing);
		}
		void LocalizatorChanged(object sender, EventArgs e) {
			this.labActiveDevice.Text = Localizer.Active.GetLocalizedString(StringId.CameraSettingsActiveDevice);
			this.labResolution.Text = Localizer.Active.GetLocalizedString(StringId.CameraSettingsResolution);
			this.labBrightness.Text = Localizer.Active.GetLocalizedString(StringId.CameraSettingsBrightness);
			this.labContrast.Text = Localizer.Active.GetLocalizedString(StringId.CameraSettingsContrast);
			this.labDesaturate.Text = Localizer.Active.GetLocalizedString(StringId.CameraSettingsDesaturate);
			this.simpleButtonDefaults.Text = Localizer.Active.GetLocalizedString(StringId.CameraSettingsDefaults);
			this.simpleButtonClose.Text = Localizer.Active.GetLocalizedString(StringId.OK);
		}
		void CameraSettingsControl_SizeChanged(object sender, EventArgs e) {
			UpdatePanelLocation();
		}
		void UpdatePanelLocation() {
			int x = (int)(this.Width / 2) - (mainPanel.Width / 2);
			int y = mainPanel.Location.Y;
			mainPanel.Location = new Point(x, y);
		}
		CameraDevice GetActiveDevice() {
			if(OwnerControl != null)
				return OwnerControl.Device;
			return null;
		}
		string GetActiveDeviceMoniker() {
			var activeDevice = GetActiveDevice();
			if(activeDevice != null)
				return activeDevice.DeviceMoniker;
			return null;
		}
		CameraControl ownerControl;
		public CameraControl OwnerControl {
			get { return ownerControl; }
			set {
				if(ownerControl == value) return;
				ownerControl = value;
				OnOwnerChanged();
			}
		}
		void OnOwnerChanged() {
			if(OwnerControl == null) return;
			UpdateDeviceList();
		}
		void UpdateDeviceList() {
			RefreshDevices();
			SyncWithActiveDevice();
		}
		void SyncWithActiveDevice() {
			string activeMoniker = GetActiveDeviceMoniker();
			var activeDevInfo = devices.FirstOrDefault(x => x.MonikerString.Equals(activeMoniker));
			activeDevInfo = activeDevInfo == null ? noneInfo : activeDevInfo;
			SetCurrentDeviceInfo(activeDevInfo);
			cbDevices.EditValue = activeDevInfo;
		}
		List<CameraDeviceInfo> devices = new List<CameraDeviceInfo>();
		CameraDeviceInfo noneInfo = new CameraDeviceInfo("", "None");
		public void RefreshDevices() {
			devices.Clear();
			cbDevices.Properties.Items.Clear();
			devices = CameraControl.GetDevices();
			devices.Insert(0, noneInfo);
			cbDevices.Properties.Items.AddRange(devices);
		}
		private void cbDevices_QueryPopUp(object sender, CancelEventArgs e) {
			UpdateDeviceList();
		}
		private void cbDevices_EditValueChanged(object sender, EventArgs e) {
			var newInfo = (CameraDeviceInfo)cbDevices.EditValue;
			if(EnsureSelectedDevice(newInfo)) {
				string activeMoniker = GetActiveDeviceMoniker();
				if(newInfo != null && !newInfo.MonikerString.Equals(activeMoniker)) {
					SetCurrentDeviceInfo(newInfo);
				}
			}
			else {
				SetCurrentDeviceInfo(noneInfo);
				UpdateDeviceList();
			}
		}
		bool EnsureSelectedDevice(CameraDeviceInfo devInfo) {
			if(devInfo == noneInfo)
				return true;
			var actualDevices = CameraControl.GetDevices();
			foreach(var actualDevice in actualDevices) {
				if(actualDevice.MonikerString.Equals(devInfo.MonikerString))
					return true;
			}
			return false;
		}
		void SetCurrentDeviceInfo(CameraDeviceInfo devInfo) {
			if(devInfo == null) return;
			var newDevice = CameraControl.GetDevice(devInfo);
			UnsubscribeEditors();
			OwnerControl.Start(newDevice);
			SyncDeviceSettings(newDevice);
			SubscribeEditors();
		}
		void SyncDeviceSettings(CameraDevice device) {
			if(device == null)
				SetEditorsEnabled(false);
			else
				SyncDeviceSettingsCore();
		}
		void SyncDeviceSettingsCore() {
			SetEditorsEnabled(true);
			UpdateTrackBar(tbBrightness, OwnerControl.VideoSettings.Brightness);
			UpdateTrackBar(tbContrast, OwnerControl.VideoSettings.Contrast);
			UpdateCheck(ceDesaturate, OwnerControl.VideoSettings.Saturation);
			UpdateResolutionCombo(cbResolution, OwnerControl.Device);
		}
		void SetEditorsEnabled(bool value) {
			tbBrightness.Enabled = value;
			tbContrast.Enabled = value;
			ceDesaturate.Enabled = value;
			cbResolution.Enabled = value;
		}
		void UnsubscribeEditors() {
			tbBrightness.EditValueChanged -= tbBrightness_EditValueChanged;
			tbContrast.EditValueChanged -= tbContrast_EditValueChanged;
			ceDesaturate.CheckedChanged -= ceDesaturate_CheckedChanged;
			cbResolution.SelectedValueChanged -= cbResolution_SelectedValueChanged;
		}
		void SubscribeEditors() {
			tbBrightness.EditValueChanged += tbBrightness_EditValueChanged;
			tbContrast.EditValueChanged += tbContrast_EditValueChanged;
			ceDesaturate.CheckedChanged += ceDesaturate_CheckedChanged;
			cbResolution.SelectedValueChanged += cbResolution_SelectedValueChanged;
		}
		void UpdateResolutionCombo(ComboBoxEdit combo, CameraDevice cameraDevice) {
			combo.Properties.Items.Clear();
			if(cameraDevice == null) return;
			var resolutions = cameraDevice.GetAvailiableResolutions();
			var current = cameraDevice.Resolution;
			ResolutionItem selectedItem = null;
			foreach(Size res in resolutions) {
				ResolutionItem ri = new ResolutionItem(res);
				combo.Properties.Items.Add(ri);
				if(res == current) selectedItem = ri;
			}
			combo.SelectedItem = selectedItem;
		}
		void UpdateTrackBar(TrackBarControl track, DeviceVideoProperty videoProp) {
			track.Properties.Minimum = videoProp.Min;
			track.Properties.Maximum = videoProp.Max;
			track.Value = videoProp.Value;
		}
		void UpdateCheck(CheckEdit checkEdit, DeviceVideoProperty deviceVideoProperty) {
			checkEdit.Checked = deviceVideoProperty.Value == deviceVideoProperty.Min;
		}
		void cbResolution_SelectedValueChanged(object sender, EventArgs e) {
			var ri = (sender as ComboBoxEdit).SelectedItem as ResolutionItem;
			if(OwnerControl.Device == null || ri == null) return;
			OwnerControl.Device.Resolution = ri.Resolution;
		}
		void ceDesaturate_CheckedChanged(object sender, EventArgs e) {
			if(OwnerControl == null) return;
			int newValue = ceDesaturate.Checked ? OwnerControl.VideoSettings.Saturation.Min : OwnerControl.VideoSettings.Saturation.Default;
			OwnerControl.VideoSettings.Saturation.Value = newValue;
		}
		void tbContrast_EditValueChanged(object sender, EventArgs e) {
			if(OwnerControl == null) return;
			OwnerControl.VideoSettings.Contrast.Value = (sender as TrackBarControl).Value;
		}
		void tbBrightness_EditValueChanged(object sender, EventArgs e) {
			if(OwnerControl == null) return;
			OwnerControl.VideoSettings.Brightness.Value = (sender as TrackBarControl).Value;
		}
		void simpleButtonDefaults_Click(object sender, EventArgs e) {
			if(OwnerControl == null) return;
			OwnerControl.VideoSettings.Brightness.ResetToDefault();
			OwnerControl.VideoSettings.Contrast.ResetToDefault();
			OwnerControl.VideoSettings.Saturation.ResetToDefault();
			SyncDeviceSettings(OwnerControl.Device);
		}
		private void simpleButtonClose_Click(object sender, EventArgs e) {
			Form ownerForm = FindForm();
			if(ownerForm != null)
				ownerForm.Close();
		}
		class ResolutionItem {
			public ResolutionItem(Size res) { Resolution = res; }
			public Size Resolution { get; private set; }
			public override string ToString() {
				return string.Format("{0} x {1}", Resolution.Width, Resolution.Height);
			}
		}
	}
}
