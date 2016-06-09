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

using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraEditors.Camera {
	public partial class TakePictureForm : XtraForm {
		CameraControl cam;
		PictureEdit picedit;
		TakePictureDialog dialog;
		bool inCameraView;
		public TakePictureForm(TakePictureDialog dialog)
			: this() {
				this.dialog = dialog;
		}
		public TakePictureForm() {
			InitializeComponent();
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Size = new Size(358, 345);
			this.MinimumSize = new Size(350, 200);
			sbSave.Text = Localizer.Active.GetLocalizedString(StringId.TakePictureDialogSave);
			sbCancel.Text = Localizer.Active.GetLocalizedString(StringId.Cancel);
			sbCapture.Enabled = false;
			sbSave.Enabled = false;
			sbCancel.Enabled = true;
			CreateCameraControl();
			CreatePreviewControl();
			sbCancel.DialogResult = DialogResult.Cancel;
			sbCapture.Click += sbCapture_Click;
			sbSave.Click += sbSave_Click;
			cam.DeviceChanged += cam_DeviceChanged;
			if(LookAndFeel.GetTouchUI()) {
				Scale(new SizeF(1.4f + LookAndFeel.GetTouchScaleFactor() / 10.0f, 1.4f + LookAndFeel.GetTouchScaleFactor() / 10.0f));
			}
			ShowCameraView();
		}
		protected override void OnShown(EventArgs e) {
			this.Text = GetWindowCaption();
		}
		string GetWindowCaption() {
			if(this.dialog != null) {
				if(!string.IsNullOrEmpty(this.dialog.Caption))
					return this.dialog.Caption;
			}
			return Localizer.Active.GetLocalizedString(StringId.TakePictureDialogTitle);
		}
		void CreatePreviewControl() {
			picedit = new PictureEdit();
			picedit.Dock = DockStyle.Fill;
			picedit.Properties.ShowMenu = false;
			picedit.Properties.SizeMode = XtraEditors.Controls.PictureSizeMode.Zoom;
			picedit.Visible = false;
			this.panelView.Controls.Add(picedit);
		}
		void CreateCameraControl() {
			cam = new CameraControl();
			cam.Dock = DockStyle.Fill;
			cam.VideoStretchMode = VideoStretchMode.ZoomInside;
			cam.AutoStartDefaultDevice = true;
			this.panelView.Controls.Add(cam);
		}
		protected override void OnClosed(EventArgs e) {
			if(cam != null) {
				cam.Stop();
				cam.Device = null;
			}
		}
		void cam_DeviceChanged(object sender, CameraDeviceChangedEventArgs e) {
			if(e.Device == null || cam.ErrorState != CameraDeviceErrorState.Normal)
				SetEnableCapture(false);
			else
				SetEnableCapture(true);
		}
		void sbSave_Click(object sender, EventArgs e) {
			if(this.dialog != null)
				this.dialog.Image = Snapshot;
			this.DialogResult = DialogResult.OK;
		}
		void sbCapture_Click(object sender, EventArgs e) {
			if(inCameraView)
				DoCapture();
			else
				DoCancel();
		}
		void DoCancel() {
			Snapshot = null;
		}
		void DoCapture() {
			Snapshot = cam.TakeSnapshot();
		}
		Image snapshot;
		Image Snapshot {
			get { return snapshot; }
			set {
				if(snapshot == value) return;
				snapshot = value;
				OnSnapshotChanged();
			}
		}
		void OnSnapshotChanged() {
			if(Snapshot == null)
				ShowCameraView();
			else
				ShowPreviewView();
		}
		void ShowPreviewView() {
			inCameraView = false;
			cam.Visible = false;
			picedit.Image = Snapshot;
			picedit.Visible = true;
			SetEnabledSave(true);
			ChangeCaptureButtonText();
		}
		void ShowCameraView() {
			inCameraView = true;
			picedit.Image = null;
			picedit.Visible = false;
			cam.Visible = true;
			cam.Start();
			SetEnabledSave(false);
			SetEnableCapture(cam.Device != null);
			ChangeCaptureButtonText();
		}
		void ChangeCaptureButtonText() {
			if(inCameraView) {
				sbCapture.Text = Localizer.Active.GetLocalizedString(StringId.TakePictureDialogCapture);
			}
			else {
				sbCapture.Text = Localizer.Active.GetLocalizedString(StringId.TakePictureDialogTryAgain);
			}
		}
		void SetEnableCapture(bool value) { sbCapture.Enabled = value; }
		void SetEnabledSave(bool value) { sbSave.Enabled = value; }
	}
	public class TakePictureDialog {
		public TakePictureDialog() { }
		public Image Image { get; internal set; }
		public string Caption { get; set; }
		public DialogResult ShowDialog() {
			TakePictureForm form = new TakePictureForm(this);
			DialogResult result = form.ShowDialog();
			form.Dispose();
			return result;
		}
		public DialogResult ShowDialog(string caption) {
			this.Caption = caption;
			return ShowDialog();
		}
	}
}
