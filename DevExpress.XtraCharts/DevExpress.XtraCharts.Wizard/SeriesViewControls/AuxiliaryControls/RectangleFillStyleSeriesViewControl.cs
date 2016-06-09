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
using System.Text;
using System.Drawing.Drawing2D;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class RectangleFillStyleSeriesViewControl : ChartUserControl {
		struct GradientItem {
			readonly RectangleGradientMode gradientMode;
			readonly string text;
			public RectangleGradientMode GradientMode { get { return gradientMode; } }
			public GradientItem(RectangleGradientMode gradientMode) {
				this.gradientMode = gradientMode;
				switch (gradientMode) {
					case RectangleGradientMode.TopToBottom:
						text = ChartLocalizer.GetString(ChartStringId.WizGradientTopToBottom);
						break;
					case RectangleGradientMode.BottomToTop:
						text = ChartLocalizer.GetString(ChartStringId.WizGradientBottomToTop);
						break;
					case RectangleGradientMode.LeftToRight:
						text = ChartLocalizer.GetString(ChartStringId.WizGradientLeftToRight);
						break;
					case RectangleGradientMode.RightToLeft:
						text = ChartLocalizer.GetString(ChartStringId.WizGradientRightToLeft);
						break;
					case RectangleGradientMode.TopLeftToBottomRight:
						text = ChartLocalizer.GetString(ChartStringId.WizGradientTopLeftToBottomRight);
						break;
					case RectangleGradientMode.BottomRightToTopLeft:
						text = ChartLocalizer.GetString(ChartStringId.WizGradientBottomRightToTopLeft);
						break;
					case RectangleGradientMode.TopRightToBottomLeft:
						text = ChartLocalizer.GetString(ChartStringId.WizGradientTopRightToBottomLeft);
						break;
					case RectangleGradientMode.BottomLeftToTopRight:
						text = ChartLocalizer.GetString(ChartStringId.WizGradientBottomLeftToTopRight);
						break;
					case RectangleGradientMode.ToCenterHorizontal:
						text = ChartLocalizer.GetString(ChartStringId.WizGradientToCenterHorizontal);
						break;
					case RectangleGradientMode.FromCenterHorizontal:
						text = ChartLocalizer.GetString(ChartStringId.WizGradientFromCenterHorizontal);
						break;
					case RectangleGradientMode.ToCenterVertical:
						text = ChartLocalizer.GetString(ChartStringId.WizGradientToCenterVertical);
						break;
					case RectangleGradientMode.FromCenterVertical:
						text = ChartLocalizer.GetString(ChartStringId.WizGradientFromCenterVertical);
						break;
					default:
						ChartDebug.Fail("Unknown gradient mode.");
						text = ChartLocalizer.GetString(ChartStringId.WizGradientTopToBottom);
						break;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				if (!(obj is GradientItem))
					return false;
				GradientItem item = (GradientItem)obj;
				return gradientMode == item.gradientMode;
			}
			public override int GetHashCode() {
				return gradientMode.GetHashCode();
			}
		}
		RectangleFillStyle fillStyle;
		List<HatchStyle> hatchStyles = new List<HatchStyle>();
		int selectedIndex = -1;
		public RectangleFillStyleSeriesViewControl() {
			InitializeComponent();
		}
		Color Color2 {
			get {
				if (fillStyle.FillMode == FillMode.Hatch)
					return ((HatchFillOptions)fillStyle.Options).Color2;
				else if (fillStyle.FillMode == FillMode.Gradient) 
					return ((RectangleGradientFillOptions)fillStyle.Options).Color2;
				else 
					return Color.Empty;
			}
			set {
				if (fillStyle.FillMode == FillMode.Hatch)
					((HatchFillOptions)fillStyle.Options).Color2 = value;
				else if (fillStyle.FillMode == FillMode.Gradient)
					((RectangleGradientFillOptions)fillStyle.Options).Color2 = value;
			}
		}
		HatchStyle HatchStyle {
			get {
				if (fillStyle.FillMode == FillMode.Hatch)
					return ((HatchFillOptions)fillStyle.Options).HatchStyle;
				return HatchStyle.Cross;
			}
			set {
				if (fillStyle.FillMode == FillMode.Hatch)
					((HatchFillOptions)fillStyle.Options).HatchStyle = value;
			}
		}
		RectangleGradientMode GradientMode {
			get {
				if (fillStyle.FillMode == FillMode.Gradient)
					return ((RectangleGradientFillOptions)fillStyle.Options).GradientMode;
				return default(RectangleGradientMode);
			}
			set {
				if (fillStyle.FillMode == FillMode.Gradient)
					((RectangleGradientFillOptions)fillStyle.Options).GradientMode = value;
			}
		}
		public FillStyle2D Options {
			get { return fillStyle; }
		}
		void cbFillMode_SelectedIndexChanged(object sender, EventArgs e) {
			fillStyle.FillMode = (FillMode)cbFillMode.SelectedIndex;
			UpdateControls();
		}
		void colorEdit_EditValueChanged(object sender, EventArgs e) {
			Color2 = (Color)colorEdit.EditValue;
			UpdateControls();
		}
		void cbHatchStyle_SelectedItemChanged(object sender, EventArgs e) {
			this.selectedIndex = this.cbHatchStyle.SelectedIndex;
			HatchStyle = hatchStyles[this.selectedIndex];
			UpdateControls();
		}
		void cbGradientMode_SelectedIndexChanged(object sender, EventArgs e) {
			GradientMode = ((GradientItem)cbGradientMode.SelectedItem).GradientMode;
			UpdateControls();
		}
		void UpdateControls() {
			colorEdit.Enabled = this.fillStyle.FillMode != FillMode.Empty && this.fillStyle.FillMode != FillMode.Solid;
			cbHatchStyle.Enabled = this.fillStyle.FillMode == FillMode.Hatch;
			cbGradientMode.Enabled = this.fillStyle.FillMode == FillMode.Gradient;
			cbFillMode.SelectedIndex = (int)this.fillStyle.FillMode;
			if(this.fillStyle.FillMode == FillMode.Gradient) {
				RectangleGradientFillOptions options = (RectangleGradientFillOptions)this.fillStyle.Options;
				colorEdit.EditValue = options.Color2;
				cbGradientMode.SelectedItem = new GradientItem(options.GradientMode);
			}
			else if(fillStyle.FillMode == FillMode.Hatch) {
				HatchFillOptions options = (HatchFillOptions)this.fillStyle.Options;
				colorEdit.EditValue = options.Color2;
				cbHatchStyle.SelectedIndex = this.selectedIndex > 0 ? this.selectedIndex : this.hatchStyles.IndexOf(options.HatchStyle);
			}
		}
		public void Initialize(RectangleFillStyle fillStyle) {
			this.fillStyle = fillStyle;
			cbGradientMode.Properties.Items.Clear();
			cbHatchStyle.Properties.Items.Clear();
			hatchStyles.Clear();
			selectedIndex = -1;
			foreach (RectangleGradientMode gradientMode in Enum.GetValues(typeof(RectangleGradientMode)))
				cbGradientMode.Properties.Items.Add(new GradientItem(gradientMode));
			ComboHelper.FillHatchStyle(this.cbHatchStyle);
			foreach(HatchStyle hatchStyle in Enum.GetValues(typeof(HatchStyle)))
				this.hatchStyles.Add(hatchStyle);
			this.Enabled = fillStyle != null;
			if(this.fillStyle != null)
				UpdateControls();
		}
	}
}
