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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.AnnotationControls {
	internal partial class AnnotationGeneralControl : ChartUserControl {
		Annotation annotation;
		MethodInvoker updateMethod;
		Action0 changeNameMethod;
		Action0 updateLayoutCallback;
		bool layoutUpdatesLocked;
		bool AnnotationAutoSize {
			get {
				TextAnnotation textAnnotation = annotation as TextAnnotation;
				if (textAnnotation != null)
					return textAnnotation.AutoSize;
				ImageAnnotation imageAnnotation = annotation as ImageAnnotation;
				if (imageAnnotation != null)
					return imageAnnotation.SizeMode == ChartImageSizeMode.AutoSize;
				return false;
			}
			set {
				TextAnnotation textAnnotation = annotation as TextAnnotation;
				if (textAnnotation != null)
					textAnnotation.AutoSize = value;
				else {
					ImageAnnotation imageAnnotation = annotation as ImageAnnotation;
					if (imageAnnotation != null)
						imageAnnotation.SizeMode = value ? ChartImageSizeMode.AutoSize : ChartImageSizeMode.Stretch;
				}
			}
		}
		public Action0 UpdateLayoutCallback { get { return updateLayoutCallback; } set { updateLayoutCallback = value; } }
		public AnnotationGeneralControl() {
			InitializeComponent();
		}
		public void Initialize(Annotation annotation, MethodInvoker updateMethod, Action0 changeNameMethod) {
			this.annotation = annotation;
			this.updateMethod = updateMethod;
			this.changeNameMethod = changeNameMethod;
			UpdateLayoutControls();
			UpdateLabelModeControls();
			spnAngle.EditValue = annotation.Angle;
			chVisible.Checked = annotation.Visible;
			chLabelMode.Checked = annotation.LabelMode;
			txtName.Text = annotation.Name;
			spnZOrder.EditValue = annotation.ZOrder;
			UpdateControls();
		}
		void UpdateControls() {
			pnlGeneral.Enabled = annotation.Visible;
			updateMethod();
		}
		internal void UpdateLayoutControls() {
			layoutUpdatesLocked = true;
			try {
				chAutoSize.Checked = AnnotationAutoSize;
				spnWidth.EditValue = annotation.Width;
				spnHeight.EditValue = annotation.Height;
			}
			finally {
				layoutUpdatesLocked = false;
			}
		}
		internal void UpdateLabelModeControls() {
			sepZOrder.Visible = AnnotationLabelModeUtils.IsLabelModeSupported(annotation);
			pnlLabelModeBack.Visible = AnnotationLabelModeUtils.IsLabelModeSupported(annotation);
		}
		void RaiseUpdateLayoutCallback() {
			if (updateLayoutCallback != null)
				updateLayoutCallback();
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			annotation.Visible = chVisible.Checked;
			UpdateControls();
		}
		void txtName_EditValueChanged(object sender, EventArgs e) {
			annotation.Name = txtName.EditValue.ToString();
			changeNameMethod();
		}
		void spnZOrder_EditValueChanged(object sender, EventArgs e) {
			annotation.ZOrder = Convert.ToInt32(spnZOrder.EditValue);
		}
		void chAutoSize_CheckedChanged(object sender, EventArgs e) {
			if (!layoutUpdatesLocked) {
				AnnotationAutoSize = chAutoSize.Checked;
				UpdateLayoutControls();
				RaiseUpdateLayoutCallback();
			}
		}
		void spnWidth_EditValueChanged(object sender, EventArgs e) {
			if (!layoutUpdatesLocked) {
				annotation.Width = Convert.ToInt32(spnWidth.EditValue);
				UpdateLayoutControls();
				RaiseUpdateLayoutCallback();
			}
		}
		void spnHeight_EditValueChanged(object sender, EventArgs e) {
			if (!layoutUpdatesLocked) {
				annotation.Height = Convert.ToInt32(spnHeight.EditValue);
				UpdateLayoutControls();
				RaiseUpdateLayoutCallback();
			}
		}
		void spnAngle_EditValueChanged(object sender, EventArgs e) {
			annotation.Angle = Convert.ToInt32(spnAngle.EditValue);
		}
		void chLabelMode_CheckedChanged(object sender, EventArgs e) {
			annotation.LabelMode = chLabelMode.Checked;
		}
	}
}
