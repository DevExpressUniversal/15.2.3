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
using System.Windows.Forms;
using DevExpress.Skins;
namespace DevExpress.XtraEditors.Design {
	public partial class TrackBarAutoPopulateLabelsForm : XtraForm {
		TrackBarAutoPopulatingInfo info;
		static TrackBarAutoPopulateLabelsForm() {
			SkinManager.EnableFormSkins();
		}
		public TrackBarAutoPopulateLabelsForm(TrackBarControl trackBar) {
			InitializeComponent();
			this.info = new TrackBarAutoPopulatingInfo(trackBar);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			InitializeControls();
		}
		void InitializeControls() {
			this.seMinimum.EditValue = Info.Minimun;
			this.seMaximum.EditValue = Info.Maximum;
			this.seStep.EditValue = Info.Step;
			this.seMinimum.Properties.MinValue = this.seMaximum.Properties.MinValue = Info.Minimun;
			this.seMinimum.Properties.MaxValue = this.seMaximum.Properties.MaxValue = Info.Maximum;
			this.seStep.Properties.MinValue = 1;
			this.seStep.Properties.MaxValue = Info.Maximum;
		}
		void SaveInfo() {
			Info.Minimun = GetSpinValue(this.seMinimum);
			Info.Maximum = GetSpinValue(this.seMaximum);
			Info.Step = GetSpinValue(this.seStep);
		}
		int GetSpinValue(SpinEdit spinEdit) {
			decimal res = (decimal)spinEdit.EditValue;
			return decimal.ToInt32(res);
		}
		void btnOk_Click(object sender, EventArgs e) {
			SaveInfo();
			DialogResult = DialogResult.OK;
		}
		public TrackBarAutoPopulatingInfo Info { get { return info; } }
	}
	public class TrackBarAutoPopulatingInfo {
		static readonly int Threshold = 50;
		public TrackBarAutoPopulatingInfo(TrackBarControl control) {
			InitDefaults(control);
		}
		void InitDefaults(TrackBarControl control) {
			this.Minimun = control.Properties.Minimum;
			this.Maximum = control.Properties.Maximum;
			this.Step = 1;
			if(Maximum - Minimun + 1 > Threshold) {
				Step = (int)((double)(Maximum - Minimun + 1) / 10 + 0.5);
			}
		}
		public int Minimun { get; set; }
		public int Maximum { get; set; }
		public int Step { get; set; }
	}
}
