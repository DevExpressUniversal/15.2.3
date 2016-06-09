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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using DevExpress.XtraTab;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System.ComponentModel.Design;
namespace DevExpress.XtraEditors.Design {
	public class FramesUtils {
		public static XtraTabControl CreateTabProperty(Control parent, Control main, string mainCaption) {
			return CreateTabProperty(parent, new Control[] { main }, new string[] { mainCaption });
		}
		public static XtraTabControl CreateTabProperty(Control parent, Control[] ctrls, string[] captions) {
			XtraTabControl tc = new XtraTabControl();
			tc.SuspendLayout();
			tc.Bounds = parent.DisplayRectangle;
			tc.TabPages.Clear();
			if(ctrls != null) {
				for(int i = 0; i < ctrls.Length; i++) {
					tc.TabPages.Add(captions[i]);
					if(ctrls[i] != null) {
						tc.TabPages[i].SuspendLayout();
						tc.TabPages[i].Controls.Add(ctrls[i]);
						ctrls[i].SuspendLayout();
						ctrls[i].Dock = DockStyle.Fill;
						ctrls[i].ResumeLayout(false);
						tc.TabPages[i].ResumeLayout(false);
					}
				}
			}
			parent.Controls.Add(tc);
			tc.Dock = DockStyle.Fill;
			tc.BringToFront();
			tc.ResumeLayout();
			return tc;
		}
		public static bool SelectItem(DevExpress.XtraEditors.ListBoxControl lb, int n) {
			bool b = true;
			if(lb.Items.Count > n) lb.SelectedIndex = n;
			else if(lb.Items.Count > 0) lb.SelectedIndex = lb.Items.Count - 1;
			else b = false;
			return b;
		}
	}
	[ToolboxItem(false)]
	public class AppearancesPreview : DevExpress.XtraEditors.GroupControl {
		object[] appearances = null;
		Image appearanceImage;
		public AppearancesPreview() : this(null) { }
		public AppearancesPreview(Image image) {
			this.Text = Properties.Resources.AppearancePreviewText;
			appearanceImage = image;
			this.DockPadding.All = 8;
		}
		public void SetAppearance(object[] objects) {
			appearances = objects;
			this.Invalidate();
		}
		[DefaultValue(null)]
		public Image AppearanceImage {
			get { return appearanceImage; }
			set { appearanceImage = value; }
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			Rectangle r = this.DisplayRectangle;
			string errorString = "";
			if(appearances != null && appearances.Length == 1) {
				AppearanceObject appMain = appearances[0] as AppearanceObject;
				if(appMain == null) {
					StyleFormatConditionBase formatCondition = appearances[0] as StyleFormatConditionBase;
					if(formatCondition != null)
						appMain = formatCondition.Appearance;
				}
				if(appMain != null && (appMain.Options.UseBackColor || appMain.Options.UseForeColor || appMain.Options.UseFont)) {
					DevExpress.Utils.Drawing.GraphicsCache cache = new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics);
					if(AppearanceImage != null) {
						using(TextureBrush brush = new TextureBrush(AppearanceImage)) {
							e.Graphics.FillRectangle(brush, r);
						}
					}
					appMain.FillRectangle(cache, r);
					if(appMain.BorderColor != Color.Empty) {
						Pen pen = new Pen(appMain.GetBorderBrush(cache), 1);
						e.Graphics.DrawRectangle(pen, r);
					}
					r.Inflate(-8, -8);
					appMain.DrawString(cache, Properties.Resources.AppearanceObjectPreviewCaption, r, appMain.GetForeBrush(cache), appMain.GetStringFormat());
					cache.Dispose();
				}
				else errorString = Properties.Resources.AppObjectEmptyCaption;
			}
			else errorString = Properties.Resources.SelectWarning1;
			if(appearances == null) errorString = Properties.Resources.SelectWarning2;
			if(errorString != "") {
				using(StringFormat sf = new StringFormat()) {
					GraphicsCache cache = new GraphicsCache(e);
					sf.Alignment = sf.LineAlignment = StringAlignment.Center;
					cache.DrawString(errorString,  DevExpress.Utils.Design.WindowsFormsDesignTimeSettings.DefaultDesignTimeFont, SystemBrushes.ControlText, r, sf);
					cache.Dispose();
				}
			}
		}
	}
}
