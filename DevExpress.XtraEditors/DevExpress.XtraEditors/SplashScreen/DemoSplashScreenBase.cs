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
using DevExpress.XtraSplashScreen;
using DevExpress.Utils;
using DevExpress.Utils.FormShadow;
namespace DevExpress.XtraSplashScreen {
	public partial class DemoSplashScreenBase : SplashScreen {
		public DemoSplashScreenBase() {
			InitializeComponent();
			CopyrightDataUpdate();
			UpdateLablesLocation();
		}
		protected virtual void CopyrightDataUpdate() {
			labelControl1.Text = string.Format("Copyright © 1998-{0} Developer Express inc.\r\nAll Rights Reserved", DateTime.Now.Year);
		}
		public string DemoText {
			get { return this.labelDemoText.Text; }
			set { this.labelDemoText.Text = value; }
		}
		public string ProductText {
			get { return this.labelProductText.Text; }
			set { this.labelProductText.Text = value; }
		}
		#region Overrides
		public override void ProcessCommand(Enum cmd, object arg) {
			base.ProcessCommand(cmd, arg);
			if(cmd.GetType() != typeof(SplashScreenCommand)) return;
			SplashScreenCommand cmdId = (SplashScreenCommand)cmd;
			if(cmdId == SplashScreenCommand.UpdateLabel) {
				labelControl2.Text = (string)arg;
			}
			if(cmdId == SplashScreenCommand.UpdateLabelDemoText) {
				labelDemoText.Text = (string)arg;
			}
			if(cmdId == SplashScreenCommand.UpdateLabelProductText) {
				labelProductText.Text = (string)arg;
			}
		}
		#endregion
		public enum SplashScreenCommand {
			UpdateLabel,
			UpdateLabelDemoText,
			UpdateLabelProductText
		}
		private void labelDemoText_TextChanged(object sender, EventArgs e) {
			UpdateLablesLocation();
		}
		protected virtual void UpdateLablesLocation() {
			if(labelDemoText.Parent == null)
				return;
			Size sz = labelDemoText.CalcBestSize();
			Size sz2 = labelProductText.CalcBestSize();
			Rectangle parentBounds = labelDemoText.Parent.ClientRectangle;
			const int indentBetweenText = 0;
			int textHeight = sz.Height + indentBetweenText + sz2.Height;
			labelDemoText.Location = new Point(parentBounds.X + (parentBounds.Width - sz.Width) / 2, parentBounds.Y + (parentBounds.Height - textHeight) / 2);
			labelProductText.Location = new Point(parentBounds.X + (parentBounds.Width - sz2.Width) / 2, labelDemoText.Bottom + indentBetweenText);
		}
		private void labelDemoText_ParentChanged(object sender, EventArgs e) {
			UpdateLablesLocation();
		}
		protected override void DrawContent(DevExpress.Utils.Drawing.GraphicsCache cache, Skins.Skin skin) {
			Rectangle bounds = ClientRectangle;
			bounds.Width--; bounds.Height--;
			cache.Graphics.DrawRectangle(cache.GetPen(Color.FromArgb(255, 87,87,87), 1), bounds);
		}
		private void pictureEdit2_ImageChanged(object sender, EventArgs e) {
			if(this.pictureEdit2.Image != null) {
				this.pictureEdit2.Visible = true;
				this.panelControl.Visible = false;
				this.labelDemoText.Visible = false;
			}
			else {
				this.pictureEdit2.Visible = false;
				this.panelControl.Visible = true;
				this.labelDemoText.Visible = true;
			}
		}
		private void labelProductText_TextChanged(object sender, EventArgs e) {
			UpdateLablesLocation();
		}
		private void labelProductText_ParentChanged(object sender, EventArgs e) {
			UpdateLablesLocation();
		}
	}
}
