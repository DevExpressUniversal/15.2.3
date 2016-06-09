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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.XtraBars.InternalItems {
	internal class BarSystemMenuItem : BarSubItem {
		BarItems items;
		WeakReference mdiChild;
		ArrayList tempItems;
		internal BarSystemMenuItem(BarManager barManager)
			: base(barManager, true) {
			tempItems = new ArrayList();
			mdiChild = null;
			items = new BarItems(barManager);
			BarButtonItem bItem = new BarButtonItem(barManager, true);
			bItem.Caption = "Minimize";
			items.Add(bItem);
			this.AddItem(bItem);
		}
		protected internal override BarItemPaintStyle CalcRealPaintStyle(BarItemLink link) {
			return BarItemPaintStyle.CaptionInMenu;
		}
		protected internal override bool IsPrivateItem { get { return true; } }
		protected internal override void OnGetItemData() {
			if(IsOpened()) return;
			UpdateData();
			base.OnGetItemData();
		}
		public Form MdiChild {
			get { return mdiChild == null ? null : mdiChild.Target as Form; }
			set {
				if(MdiChild == value) return;
				if(value == null) mdiChild = null;
				else mdiChild = new WeakReference(value);
				UpdateData();
			}
		}
		void InternalClearItems() {
			ItemLinks.Clear();
			ItemsCollectionChanged();
			foreach(BarItem item in tempItems) {
				item.Dispose();
			}
			tempItems.Clear();
		}
		internal IntPtr GetChildSystemMenu(Form frm) {
			if(frm == null) return IntPtr.Zero;
			return BarNativeMethods.GetSystemMenu(frm.Handle, false);
		}
		protected virtual void UpdateData() {
			InternalClearItems();
			if(MdiChild == null) return;
			Icon icon = ExtractIcon(MdiChild);
			if(icon != null) {
				Bitmap temp = icon.ToBitmap();
				Glyph = new Bitmap(temp, new Size(16, 16));
				temp.Dispose();
			}
			else
				Glyph = new Bitmap(16, 16);
			IntPtr menu = GetChildSystemMenu(MdiChild);
			if(menu == IntPtr.Zero) return;
			BuildSystemMenu(this, menu);
		}
		Icon ExtractIcon(Form frm) {
			if(frm.Icon == null) return null;
			return new Icon(frm.Icon, 16, 16);
		}
		const int MIIM_STATE = 0x00000001,
			MIIM_ID = 0x00000002,
			MIIM_SUBMENU = 0x00000004,
			MIIM_CHECKMARKS = 0x00000008,
			MIIM_TYPE = 0x00000010,
			MIIM_DATA = 0x00000020,
			MF_SEPARATOR = 0x00000800,
			MF_ENABLED = 0x00000000,
			MF_GRAYED = 0x00000001,
			MF_DISABLED = 0x00000002,
			MF_UNCHECKED = 0x00000000,
			MF_CHECKED = 0x00000008,
			MF_USECHECKBITMAPS = 0x00000200,
			MF_STRING = 0x00000000,
			MF_BITMAP = 0x00000004,
			MF_OWNERDRAW = 0x00000100,
			MF_POPUP = 0x00000010,
			MF_MENUBARBREAK = 0x00000020,
			MF_MENUBREAK = 0x00000040,
			MF_UNHILITE = 0x00000000,
			MF_HILITE = 0x00000080,
			MF_BYPOSITION = 0x00000400,
			MF_DEFAULT = 0x00001000,
			MFS_GRAYED = 0x00000003,
			MFS_DISABLED = MFS_GRAYED,
			MFS_CHECKED = MF_CHECKED,
			MFS_HILITE = MF_HILITE,
			MFS_ENABLED = MF_ENABLED,
			MFS_UNCHECKED = MF_UNCHECKED,
			MFS_UNHILITE = MF_UNHILITE,
			MFS_DEFAULT = MF_DEFAULT,
			WM_SYSCOMMAND = 0x0112,
			HBMMENU_POPUP_CLOSE = 8,
			HBMMENU_POPUP_RESTORE = 9,
			HBMMENU_POPUP_MAXIMIZE = 10,
			HBMMENU_POPUP_MINIMIZE = 11;
		protected void BuildSystemMenu(BarSubItem subItem, IntPtr menu) {
			int[] bmpIndexes = new int[] {HBMMENU_POPUP_CLOSE, HBMMENU_POPUP_RESTORE,
											 HBMMENU_POPUP_MAXIMIZE,HBMMENU_POPUP_MINIMIZE};
			string[] bmpNames = new String[] { "CloseImage", "RestoreImage", "MaxImage", "MinImage" };
			bool beginGroup = false;
			int count = BarNativeMethods.GetMenuItemCount(menu);
			if(count == 0) return;
			for(uint n = 0; n < count; n++) {
				IntPtr buf = BarNativeMethods.AllocCoTaskMem(1024);
				string s, shortCutText;
				shortCutText = s = "";
				BarNativeMethods.MENUITEMINFO m = new BarNativeMethods.MENUITEMINFO();
				m.cbSize = (uint)BarNativeMethods.SizeOf(m);
				m.fMask = MIIM_CHECKMARKS | MIIM_ID | MIIM_STATE | MIIM_SUBMENU | MIIM_TYPE;
				m.dwTypeData = buf;
				m.cch = 1024;
				IntPtr ptr = IntPtr.Zero;
				BarNativeMethods.GetMenuItemInfo(menu, n, true, ref m);
				if((m.fType & MF_SEPARATOR) != 0) {
					beginGroup = true;
					continue;
				}
				bool shouldCopyString = m.cch != 0;
				if((m.fType & MF_BITMAP) != 0) {
					BarNativeMethods.GetMenuString(menu, n, buf, 1024, MF_BYPOSITION);
					shouldCopyString = true;
				}
				if(shouldCopyString)
					s = BarNativeMethods.PtrToStringAnsi(buf);
				else
					s = String.Empty;
				int ps = s.IndexOf((char)9);
				if(ps != -1) {
					shortCutText = s.Substring(ps + 1);
					s = s.Substring(0, ps);
				}
				BarItem item = null;
				if(m.hSubMenu == IntPtr.Zero) {
					BarButtonItem btn = new BarButtonItem(null, true);
					if((m.fState & MFS_CHECKED) != 0) {
						btn.ButtonStyle = BarButtonStyle.Check;
						btn.Down = true;
					}
					btn.Tag = m.wID;
					btn.ItemClick += new ItemClickEventHandler(ItemClicked);
					item = btn;
				}
				else {
					BarSubItem sub = new BarSubItem(Manager, true);
					BuildSystemMenu(sub, m.hSubMenu);
					item = sub;
				}
				item.Manager = subItem.Manager;
				item.Caption = s;
				item.Enabled = (m.fState & (MFS_DISABLED | MFS_GRAYED)) == 0;
				if(shortCutText.Length > 0) {
					try {
						KeysConverter conv = new KeysConverter();
						Keys keys = (Keys)conv.ConvertFromString(shortCutText);
						item.ItemShortcut = new BarShortcut(keys);
					}
					catch {
					}
				}
				Bitmap glyph = null;
				try {
					int hbmp = m.hbmpItem.ToInt32();
					if(Array.IndexOf(bmpIndexes, hbmp) != -1)
						glyph = Manager.GetController().GetBitmap(bmpNames[Array.IndexOf(bmpIndexes, hbmp)]);
					else {
						if((m.fState & MFS_CHECKED) != 0) {
							if(m.hbmpChecked != IntPtr.Zero)
								glyph = Bitmap.FromHbitmap(m.hbmpChecked);
						}
						else {
							if(m.hbmpUnchecked != IntPtr.Zero)
								glyph = Bitmap.FromHbitmap(m.hbmpUnchecked);
						}
						if(glyph == null && (m.fType & MF_BITMAP) != 0)
							glyph = Bitmap.FromHbitmap(m.dwTypeData);
					}
					if(glyph != null) {
						glyph = glyph.Clone() as Bitmap;
						glyph.MakeTransparent();
					}
					item.Glyph = glyph;
				}
				catch {
				}
				subItem.AddItem(item).BeginGroup = beginGroup;
				tempItems.Add(item);
				beginGroup = false;
				BarNativeMethods.FreeCoTaskMem(buf);
			}
		}
		private void ItemClicked(object sender, ItemClickEventArgs e) {
			if(e.Item == null) return;
			BarNativeMethods.SendMessage(MdiChild.Handle, WM_SYSCOMMAND, new IntPtr((uint)e.Item.Tag), IntPtr.Zero);
		}
	}
	public class BarMdiButtonItem : BarLargeButtonItem {
		public enum SystemItemType { Minimize, Restore, Close };
		WeakReference childFormRef;
		SystemItemType itemType;
		Image largeGlyphPressed;
		internal BarMdiButtonItem(BarManager manager, SystemItemType itemType)
			: base() {
			this.largeGlyphPressed = null;
			this.ShowCaptionOnBar = false;
			this.fIsPrivateItem = true;
			this.childFormRef = null;
			this.itemType = itemType;
			this.Manager = manager;
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				FromClosedEventRecord.UnRegister(this);
			base.Dispose(disposing);
		}
		[Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image LargeGlyphPressed {
			get { return largeGlyphPressed; }
			set {
				if(LargeGlyphPressed == value) return;
				if(largeGlyphPressed != null) largeGlyphPressed.Dispose();
				largeGlyphPressed = UpdateImage(value);
			}
		}
		public virtual Form ChildForm {
			get { return GetChildForm(); }
			set {
				Form targetForm = GetChildForm();
				if(value == targetForm) return;
				FromClosedEventRecord.UnRegister(this);
				if(value == null)
					childFormRef = null;
				else {
					childFormRef = new WeakReference(value);
					FromClosedEventRecord.Register(this);
				}
			}
		}
		Form GetChildForm() {
			return (childFormRef != null) ? (childFormRef.Target as Form) : null;
		}
		public SystemItemType ItemType {
			get { return itemType; }
		}
		protected internal override bool CanKeyboardSelect {
			get { return false; }
		}
		protected internal override BarItemPaintStyle CalcRealPaintStyle(BarItemLink link) {
			return BarItemPaintStyle.CaptionInMenu;
		}
		protected internal override void OnClick(BarItemLink link) {
			Form target = GetChildForm();
			if(target != null)
				DevExpress.Skins.XtraForm.FormPainter.PostSysCommand(target, target.Handle, GetSCCommand(ItemType));
		}
		static int GetSCCommand(SystemItemType itemType) {
			switch(itemType) {
				case SystemItemType.Minimize:
					return NativeMethods.SC.SC_MINIMIZE;
				case SystemItemType.Restore:
					return NativeMethods.SC.SC_RESTORE;
			}
			return NativeMethods.SC.SC_CLOSE;
		}
		static class FromClosedEventRecord {
			static IDictionary<int, List<WeakReference>> mapping;
			static FromClosedEventRecord() {
				mapping = new Dictionary<int, List<WeakReference>>();
			}
			public static void Register(BarMdiButtonItem item) {
				Register(item.GetChildForm(), item);
			}
			public static void UnRegister(BarMdiButtonItem item) {
				UnRegister(item.GetChildForm());
			}
			static void Register(Form form, BarMdiButtonItem item) {
				if(form == null) return;
				int formKey = form.GetHashCode();
				List<WeakReference> itemsList;
				if(!mapping.TryGetValue(formKey, out itemsList)) {
					form.FormClosed += FormClosed;
					itemsList = new List<WeakReference>();
					mapping.Add(formKey, itemsList);
				}
				itemsList.Add(new WeakReference(item));
			}
			static void UnRegister(Form form) {
				if(form == null) return;
				int formKey = form.GetHashCode();
				List<WeakReference> itemsList;
				if(mapping.TryGetValue(formKey, out itemsList)) {
					form.FormClosed -= FormClosed;
					mapping.Remove(formKey);
					foreach(var itemRef in itemsList) {
						BarMdiButtonItem item = itemRef.Target as BarMdiButtonItem;
						if(item != null)
							item.childFormRef = null;
					}
					itemsList.Clear();
				}
			}
			static void FormClosed(object sender, FormClosedEventArgs e) {
				UnRegister(sender as Form);
			}
		}
	}
}
