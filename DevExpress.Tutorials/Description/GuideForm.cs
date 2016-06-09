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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.Description.Controls {
	public partial class GuideForm : XtraForm, IGuideForm {
		public GuideForm() {
			InitializeComponent();
		}
		void btHide_Click(object sender, EventArgs e) {
			Close();
			Disposed += GuideForm_Disposed;
		}
		void GuideForm_Disposed(object sender, EventArgs e) {
			this.formClosed = null;
		}
		public virtual void Show(GuideControl owner, GuideControlDescription description) {
			string text = lbDescription.Text;
			if(!string.IsNullOrEmpty(description.Description)) {
				text = description.Description;
			}
			else {
				text = string.Format("<b>{0}</b><br>", description.GetTypeName()) + text;
			}
			lbDescription.Text = text;
			Rectangle bounds = GetDisplayBounds(description);
			if(!bounds.IsEmpty) {
				StartPosition = FormStartPosition.Manual;
				Bounds = bounds;
			}
			TopMost = false;
			Show(owner.Window);
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Escape) Close();
			return base.ProcessDialogKey(keyData);
		}
		Rectangle GetDisplayBounds(GuideControlDescription description) {
			Rectangle bounds = description.ScreenBounds;
			Rectangle workingArea = Screen.FromRectangle(bounds).WorkingArea;
			Rectangle res = TryAllBounds(workingArea, bounds, (bounds.Width / (float)bounds.Height) > 2.5f);
			if(!res.IsEmpty) {
				Point distance1 = new Point(Math.Abs(Control.MousePosition.X - res.X), Math.Abs(Control.MousePosition.Y - res.Y));
				Point distance2 = new Point(Math.Abs(Control.MousePosition.X - res.Right), Math.Abs(Control.MousePosition.Y - res.Bottom));
				distance1.X = Math.Min(distance1.X, distance2.X);
				distance1.Y = Math.Min(distance1.Y, distance2.Y);
				if(distance1.X > 300 || distance1.Y > 300) res = Rectangle.Empty;
			}
			if(res.IsEmpty) res = TryAllBounds(workingArea, new Rectangle(Control.MousePosition, new Size(16, 16)), true);
			if(res.IsEmpty) {
				res = DevExpress.Skins.RectangleHelper.GetCenterBounds(workingArea, Size);
			}
			return res;
		}
		Rectangle TryAllBounds(Rectangle workingArea, Rectangle bounds, bool bottomFirst) {
			Rectangle res;
			if(TryBounds(workingArea, bounds, new Rectangle(bounds.X, bounds.Bottom + 8, Size.Width, Size.Height), true, out res)) return res;
			if(bottomFirst) {
				if(TryBounds(workingArea, bounds, new Rectangle(bounds.X, bounds.Y - Size.Height - 8, Size.Width, Size.Height), true, out res)) return res;
			}
			if(TryBounds(workingArea, bounds, new Rectangle(bounds.X - Size.Width - 8, bounds.Y - Size.Height, Size.Width, Size.Height), false, out res)) return res;
			if(TryBounds(workingArea, bounds, new Rectangle(bounds.Right + 8, bounds.Y - Size.Height, Size.Width, Size.Height), false, out res)) return res;
			if(!bottomFirst) {
				if(TryBounds(workingArea, bounds, new Rectangle(bounds.X, bounds.Y - Size.Height, Size.Width, Size.Height), true, out res)) return res;
			}
			return Rectangle.Empty;
		}
		Rectangle ApplyCenterbounds(Rectangle workingArea, Rectangle controlBounds, Rectangle formBounds, bool horz) {
			Point center = Point.Empty;
			Rectangle bounds = formBounds;
			if(formBounds.Width <= controlBounds.Width)
				center.X = controlBounds.X + ((controlBounds.Width - formBounds.Width) / 2);
			else
				center.X = controlBounds.X + ((controlBounds.Width - formBounds.Width) / 2);
			if(formBounds.Height <= controlBounds.Height)
				center.Y = controlBounds.Y + ((controlBounds.Height - formBounds.Height) / 2);
			else
				center.Y = controlBounds.Y + ((controlBounds.Height - formBounds.Height) / 2);
			if(horz)
				formBounds.X = center.X;
			else
				formBounds.Y = center.Y;
			if(workingArea.Contains(formBounds)) return formBounds;
			return bounds;
		}
		bool TryBounds(Rectangle workingArea, Rectangle controlBounds, Rectangle formBounds, bool horz, out Rectangle bounds) {
			bounds = formBounds;
			if(workingArea.Contains(formBounds)) {
				bounds = ApplyCenterbounds(workingArea, controlBounds, formBounds, horz);
				return true; 
			}
			if(workingArea.Y <= formBounds.Y && workingArea.Bottom >= formBounds.Bottom) {
				if(workingArea.X > formBounds.X) formBounds.X += (workingArea.X - formBounds.X);
				if(workingArea.Right < formBounds.Right) formBounds.X -= (formBounds.Right - workingArea.Right);
				bounds = formBounds;
				if(!formBounds.IntersectsWith(controlBounds)) {
					bounds = ApplyCenterbounds(workingArea, controlBounds, formBounds, horz);
					if(!controlBounds.IntersectsWith(bounds)) return true;
				}
			}
			if(workingArea.X <= formBounds.X && workingArea.Right >= formBounds.Right) {
				if(workingArea.Y > formBounds.Y) formBounds.Y += (workingArea.Y - formBounds.Y);
				if(workingArea.Bottom < formBounds.Bottom) formBounds.Y -= (formBounds.Bottom - workingArea.Bottom);
				bounds = formBounds;
				if(!formBounds.IntersectsWith(controlBounds)) {
					bounds = ApplyCenterbounds(workingArea, controlBounds, formBounds, horz);
					if(!controlBounds.IntersectsWith(bounds)) return true;
				}
			}
			bounds = Rectangle.Empty;
			return false;
		}
		private void lbDescription_HyperlinkClick(object sender, Utils.HyperlinkClickEventArgs e) {
			using(Process process = new Process()) {
				process.StartInfo = new ProcessStartInfo(e.Link);
				process.Start();
			}
		}
		event EventHandler formClosed;
		event EventHandler IGuideForm.FormClosed {
			add { formClosed += value; }
			remove { formClosed -= value; }
		}
		bool IGuideForm.IsHandle(IntPtr hwnd) { return false; }
		protected override void OnFormClosed(FormClosedEventArgs e) {
			base.OnFormClosed(e);
			if(formClosed != null) formClosed(this, EventArgs.Empty);
		}
	}
}
