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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.Data.Utils;
namespace DevExpress.Utils {
	public class WaitDialogForm : XtraForm {
		private System.Windows.Forms.PictureBox pic;
		private string caption = "";
		private string title = "";
		public WaitDialogForm() : this("") {}
		public WaitDialogForm(string caption) : this(caption, "") {}
		public WaitDialogForm(string caption, string title) : this(caption, title, new Size(260, 50), null) {}
		public WaitDialogForm(string caption, Size size) : this(caption, "", size, null) {}
		public WaitDialogForm(string caption, string title, Size size) : this(caption, title, size, null) {}
		public WaitDialogForm(string caption, string title, Size size, Form parent) {
			this.caption = caption;
			this.title = title == "" ? "Loading Data. Please Wait." : title;
			this.pic = new System.Windows.Forms.PictureBox();
			this.FormBorderStyle = FormBorderStyle.None;
			this.ControlBox = false;
			this.ClientSize = size;
			if(parent == null)
				this.StartPosition = FormStartPosition.CenterScreen;
			else {
				this.StartPosition = FormStartPosition.Manual;
				Left = parent.Left + (parent.Width - Width) / 2;
				Top = parent.Top + (parent.Height - Height) / 2;
			}
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.Paint += new PaintEventHandler(WaitDialogPaint);
			pic.Size = new Size(16, 16);
			pic.Location = new Point(8, ClientSize.Height / 2 - 16);
			pic.Image = ImageTool.ImageFromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.Utils.wait.gif"));
			this.Controls.Add(pic);
			this.Show();
			this.Refresh();
		}
		public override bool AllowFormSkin {
			get {
				return false;
			}
		}
		public string GetCaption() { return Caption; }
		public void SetCaption(string newCaption) { Caption = newCaption; }
		public string Caption {
			get { return caption; }
			set { caption = value;
				this.Refresh();
			}
		} 
		Font boldFont = null;
		Font font = null;
		Font RegularFont {
			get {
				if(font == null)
					font = new Font(AppearanceObject.DefaultFont.FontFamily, 9, FontStyle.Regular);
				return font;
			}
		}
		Font BoldFont {
			get {
				if(boldFont == null)
					boldFont = new Font(AppearanceObject.DefaultFont.FontFamily, 9, FontStyle.Bold);
				return boldFont;
			}
		}
		private void WaitDialogPaint(object sender, PaintEventArgs e) {
			Rectangle r = e.ClipRectangle;
			GraphicsCache cache = new GraphicsCache(e);
			using(StringFormat sf = new StringFormat()) {
				Brush brush = cache.GetSolidBrush(LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText));
				sf.Alignment = sf.LineAlignment = StringAlignment.Center;
				sf.Trimming = StringTrimming.EllipsisCharacter;
				if(LookAndFeel.ActiveLookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
					ObjectPainter.DrawObject(cache, new SkinTextBorderPainter(LookAndFeel), new BorderObjectInfoArgs(null, r, null));
				} else {
					ControlPaint.DrawBorder3D(e.Graphics, r, Border3DStyle.RaisedInner);
				}
				r.X += 30; r.Width -= 30;
				r.Height /= 3; 
				r.Y += r.Height / 2;
				e.Graphics.DrawString(title, BoldFont, brush, r, sf);
				r.Y += r.Height;	
				e.Graphics.DrawString(caption, RegularFont, brush, r, sf);
				cache.Dispose();
			}
		}
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			pic.Image = null;
			boldFont = null;
			font = null;
			base.OnClosing(e);
		}
	}
	public class FilesHelper {
		public static string FindingFileName(string path, string name) {
			return FindingFileName(path, name, true);
		}
		public static string FindingFileName(string path, string name, bool showMessage) {
			string s="\\";
			for(int i = 0 ; i <= 10 ; i++) {
				if(System.IO.File.Exists(path + s + name)) 
					return System.IO.Path.GetFullPath(path + s + name);
				else 
					s += "..\\";
			}
			string currentDir = Environment.CurrentDirectory;
			if(currentDir != path)
				return FindingFileName(currentDir, name, showMessage);
			if(showMessage) MessageBox.Show("File " + name + " is not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
			return "";
		}
	}
}
