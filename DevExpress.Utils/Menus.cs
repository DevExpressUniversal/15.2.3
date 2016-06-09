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
using DevExpress.LookAndFeel;
using DevExpress.Utils.Menu;
using DevExpress.Skins;
namespace DevExpress.Utils {
	public class AppearanceItem : MenuItem {
		EventHandler handler;
		ActiveLookAndFeelStyle style;
		public AppearanceItem(string text, EventHandler handler) : this(text, handler, ActiveLookAndFeelStyle.Flat) { }
		public AppearanceItem(string text, EventHandler handler, ActiveLookAndFeelStyle style) {
			this.Text = text;
			this.style = style;
			this.handler = handler;
			this.Click += handler;
		}
		public ActiveLookAndFeelStyle Style { get { return style; } }
		protected override void Dispose(bool disposing) {
			if(disposing && this.handler != null)
				this.Click -= this.handler;
			this.handler = null;
			base.Dispose(disposing);
		}
	}
	public class LookAndFeelMenu : IDisposable {
		protected string about;
		DefaultLookAndFeel lookAndFeel;
		protected MainMenu menu;
		protected Form form;
		protected MenuItem miLookAndFeel;
		protected MenuItem miSkin;
		public LookAndFeelMenu(Form form, DefaultLookAndFeel lookAndFeel, string about) {
			if(form == null) return;
			this.about = about;
			this.lookAndFeel = lookAndFeel;
			this.form = form;
			this.menu = new MainMenu();
			miLookAndFeel = new MenuItem("&Look and Feel");
			miLookAndFeel.MenuItems.Add(new AppearanceItem("&Flat",  new EventHandler(OnSwitchStyle_Click), ActiveLookAndFeelStyle.Flat));	
			miLookAndFeel.MenuItems.Add(new AppearanceItem("&Ultra Flat",  new EventHandler(OnSwitchStyle_Click), ActiveLookAndFeelStyle.UltraFlat));	
			miLookAndFeel.MenuItems.Add(new AppearanceItem("&Style3D",  new EventHandler(OnSwitchStyle_Click), ActiveLookAndFeelStyle.Style3D));	
			miLookAndFeel.MenuItems.Add(new AppearanceItem("&Office2003",  new EventHandler(OnSwitchStyle_Click), ActiveLookAndFeelStyle.Office2003));
			miLookAndFeel.MenuItems.Add(new AppearanceItem("&Native", new EventHandler(OnSwitchStyle_Click), ActiveLookAndFeelStyle.WindowsXP));
			miSkin = new MenuItem("S&kin");
			miSkin.Popup += new EventHandler(OnPopupSkinNames);
			miLookAndFeel.MenuItems.Add(miSkin);
			AddSkinNames();
			if(this.form != null) 
				this.form.Disposed += new EventHandler(form_Dispose);
			AddItems();
		}
		void AddSkinNames() {
			foreach(SkinContainer cnt in SkinManager.Default.Skins) {
				miSkin.MenuItems.Add(new AppearanceItem(cnt.SkinName, new EventHandler(OnSwitchSkin), ActiveLookAndFeelStyle.Skin));
			}
		}
		private void form_Dispose(object sender, EventArgs e) {
			miSkin.Popup -= new EventHandler(OnPopupSkinNames);
			this.Dispose();
		}
		public virtual void Dispose() {
			if(form != null) form.Menu = null;
			if(this.menu != null) this.menu.Dispose();
			if(form != null) form.Disposed -= new EventHandler(form_Dispose);
			this.menu = null;
			this.form = null;
			this.lookAndFeel = null;
			this.miLookAndFeel = null;
		}
		public DefaultLookAndFeel LookAndFeel { get { return lookAndFeel; }}
		protected virtual void AddItems() {
			menu.MenuItems.Add(miLookAndFeel);
			menu.MenuItems.Add(new AppearanceItem("&About", new EventHandler(miAbout_Click)));	
			InitLookAndFeelMenu();
			form.Menu = menu;
		}
		public void InitLookAndFeelMenu() {
			InitLookAndFeelMenu(LookAndFeel);
		}
		private bool UsingXP {
			get { return LookAndFeel.LookAndFeel.UseWindowsXPTheme && DevExpress.Utils.WXPaint.Painter.ThemesEnabled; }
		}
		protected virtual bool Mixed { get { return false; }}
		bool AvailableStyle(LookAndFeelStyle style) {
			return lookAndFeel.LookAndFeel.Style == style && !UsingXP && !Mixed;
		}
		public void InitLookAndFeelMenu(DefaultLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
			miLookAndFeel.Visible = lookAndFeel != null;
			if(lookAndFeel != null) {
				miLookAndFeel.MenuItems[0].Checked = AvailableStyle(LookAndFeelStyle.Flat);
				miLookAndFeel.MenuItems[1].Checked = AvailableStyle(LookAndFeelStyle.UltraFlat);
				miLookAndFeel.MenuItems[2].Checked = AvailableStyle(LookAndFeelStyle.Style3D);
				miLookAndFeel.MenuItems[3].Checked = AvailableStyle(LookAndFeelStyle.Office2003);
				miLookAndFeel.MenuItems[4].Checked = UsingXP && !Mixed;
				InitLookAndFeelEnabled();
			}
		}
		protected virtual void InitLookAndFeelEnabled() {
			if(lookAndFeel != null) 
				miLookAndFeel.MenuItems[4].Enabled = DevExpress.Utils.WXPaint.Painter.ThemesEnabled;
		}
		void OnPopupSkinNames(object sender, EventArgs e) {
			MenuItem items = sender as MenuItem;
			if(items == null || LookAndFeel == null) return;
			foreach(MenuItem item in items.MenuItems) 
				item.Checked = AvailableStyle(LookAndFeelStyle.Skin) && LookAndFeel.LookAndFeel.SkinName == item.Text;
		}
		void OnSwitchSkin(object sender, EventArgs e) {
			OnSwitchStyle_Click(sender, EventArgs.Empty);
			if(LookAndFeel != null) LookAndFeel.LookAndFeel.SetSkinStyle(((AppearanceItem)sender).Text);
		}
		protected virtual void OnSwitchStyle_Click(object sender, System.EventArgs e) {
			AppearanceItem item = sender as AppearanceItem;
			if(item == null || LookAndFeel == null) return;
			bool wxp = item.Style == ActiveLookAndFeelStyle.WindowsXP;
			if(item.Style != ActiveLookAndFeelStyle.Skin)
				LookAndFeel.LookAndFeel.SetStyle((LookAndFeelStyle)item.Style, wxp, LookAndFeel.LookAndFeel.UseDefaultLookAndFeel, LookAndFeel.LookAndFeel.SkinName);
			InitLookAndFeelMenu();
		}
		protected void miAbout_Click(object sender, System.EventArgs e) {
			DevExpress.Utils.About.frmAbout dlg = new DevExpress.Utils.About.frmAbout(about == "" ? "Xtra Products by Developer Express inc." : about);
			dlg.ShowDialog();
		}
	}
}
