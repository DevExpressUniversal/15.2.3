#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Win.Utils;
namespace DevExpress.ExpressApp.Win.Core {
	public class SplashScreenForm : XtraForm {
		private const int PaddingValue = 10;
		public SplashScreenForm(string displayText)
			: base() {
			this.StartPosition = FormStartPosition.CenterScreen;
			this.FormBorderStyle = FormBorderStyle.None;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.HelpButton = false;
			this.ShowInTaskbar = true;
			this.Text = Application.ProductName;
			this.Tag = null;
			NativeMethods.SetExecutingApplicationIcon(this);
			Panel place = new Panel();
			place.Location = new Point(0, 0);
			place.Dock = DockStyle.Fill;
			place.BorderStyle = BorderStyle.FixedSingle;
			this.Controls.Add(place);
			Label waitLabel = new Label();
			waitLabel.AutoSize = true;
			waitLabel.Location = new System.Drawing.Point(PaddingValue, PaddingValue);
			if(string.IsNullOrEmpty(displayText)) {
				displayText = "Loading...";
			}
			waitLabel.Text = displayText;
			place.Controls.Add(waitLabel);
			PictureBox picture = new PictureBox();
			picture.SizeMode = PictureBoxSizeMode.AutoSize;
			picture.Image = this.Icon.ToBitmap();
			picture.Location = new System.Drawing.Point(PaddingValue, PaddingValue);
			place.Controls.Add(picture);
			waitLabel.Location = new Point(picture.Left + picture.Width + PaddingValue, (picture.Height - waitLabel.Height) / 2 + PaddingValue);
			this.Size = new Size(waitLabel.Left + waitLabel.Width + PaddingValue, waitLabel.Top*2 + waitLabel.Height);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
		}
	}
	public class SplashScreen : ISplash {
		static private SplashScreenForm form;
		private static string caption;
		private static bool isStarted = false;
		#region ISplash Members
		public void Start() {
			isStarted = true;
			form = new SplashScreenForm(caption);
			form.Show();
			Application.DoEvents();
		}
		public void Stop() {
			if(form != null) {
				form.Hide();
				form.Close();
				form = null;
			}
			isStarted = false;
		}
		public void SetDisplayText(string displayText) {
			caption = displayText;
		}
		public bool IsStarted {
			get {
				return isStarted;
			}
		}
		#endregion
	}
}
