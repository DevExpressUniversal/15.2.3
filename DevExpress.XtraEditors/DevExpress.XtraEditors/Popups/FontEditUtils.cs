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

using System.Drawing;
using DevExpress.XtraEditors.Repository;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraEditors {
	public interface IRecentlyUsedItems {
		void RememberItem(object item);
	}
	public class FontServiceBase {
		static FontStyle[] styles = new FontStyle[] { FontStyle.Regular, FontStyle.Bold, FontStyle.Italic, FontStyle.Underline, FontStyle.Strikeout };
		public static FontStyle GetFirstAvailableFontStyle(FontFamily fontFamily) {
			foreach(FontStyle style in styles) {
				if(fontFamily.IsStyleAvailable(style)) return style;
			}
			#if DEBUGTEST
			System.Diagnostics.Debug.Assert(false, "Font \"" + fontFamily.Name + "\" doesn't supports any style");
			#endif
			return FontStyle.Regular;
		}
		internal static bool HasAnyFontStyle(FontFamily fontFamily) {
			foreach(FontStyle style in styles) 
				if(fontFamily.IsStyleAvailable(style)) return true;
			return false;
		}
		public static void FillRepositoryItemComboBox(RepositoryItemComboBox comboBox, ICollection items) {
			if(comboBox == null)
				return;
			comboBox.Items.BeginUpdate();
			try {
			comboBox.Items.Clear();
			comboBox.Items.AddRange(items);
			}
			finally {
				comboBox.Items.EndUpdate();
			}
		}
		public static StringCollection GetFontFamiliesNames() {
			return GetFontFamiliesNames(true);
		}
		public static StringCollection GetFontFamiliesNames(bool onlyRegularStyleFont) {
			StringCollection families = new StringCollection();
			foreach(FontFamily family in FontFamily.Families) {
				if(family.IsStyleAvailable(FontStyle.Regular) ||
					(!onlyRegularStyleFont && HasAnyFontStyle(family)))
					families.Add(family.Name);
				family.Dispose();
			}
			return families;
		}
		public static void FillFontFamilies(RepositoryItemComboBox fontNameBox) {
			FillFontFamilies(fontNameBox, true);
		}
		public static void FillFontFamilies(RepositoryItemComboBox fontNameBox, bool onlyRegularStyleFont) {
			FillRepositoryItemComboBox(fontNameBox, GetFontFamiliesNames(onlyRegularStyleFont));
		}
	}
	public static class FontItemPaintHelper {
		public static void DrawFontBoxItem(ListBoxDrawItemEventArgs e, RepositoryItem fontNameBox) {
			DrawFontBoxItem(e, fontNameBox, true);
		}
		public static void DrawFontBoxItem(ListBoxDrawItemEventArgs e, RepositoryItem fontNameBox, bool showPreview) {
			if(e.Item.ToString() == string.Empty) return;
			using(SolidBrush foreBrush = new SolidBrush(e.Appearance.ForeColor) , backBrush = new SolidBrush(e.Appearance.BackColor)) {
				if(e.State == DrawItemState.Selected) {
					if(!DrawItemBar(e, fontNameBox as RepositoryItemComboBox))
						e.Graphics.FillRectangle(backBrush, e.Bounds);
				}
				using(FontFamily family = CreateFontFamily(e.Item.ToString())) {
					using(Font font = new Font(family, e.Appearance.Font.Size, FontServiceBase.GetFirstAvailableFontStyle(family))) {
						DrawFontName(e.Graphics, e.Item.ToString(), font, e.Appearance.Font, foreBrush, e.Bounds, showPreview, e.Appearance.TextOptions.RightToLeft);
						e.Handled = true;
					}
				}
			}
		}
		static FontFamily CreateFontFamily(string name) {
			try {
				return new FontFamily(name);
			} catch {
				return new FontFamily(DevExpress.Utils.AppearanceObject.DefaultFont.Name);
			}
		}
		public static bool DrawItemBar(ListBoxDrawItemEventArgs e, RepositoryItemComboBox fontEdit) {
			if(BaseListBoxControl.GetHighlightedItemStyle(fontEdit.LookAndFeel, fontEdit.HighlightedItemStyle) != HighlightStyle.Skinned) return false;
			SkinElement element = CommonSkins.GetSkin(fontEdit.LookAndFeel)[CommonSkins.SkinHighlightedItem];
			if(element == null) return false;
			SkinElementInfo info = new SkinElementInfo(element, e.Bounds);
			if(info == null) return false;
			info.ImageIndex = 1; 
			ObjectPainter.DrawObject(e.Cache, DevExpress.Skins.SkinElementPainter.Default, info);
			return true;
		}
		static void DrawFontName(Graphics gr, string name, Font font, Font normalFont, Brush brush, Rectangle bounds, bool showPreview) {
			DrawFontName(gr, name, font, normalFont, brush, bounds, showPreview, false);
		}
		static void DrawFontName(Graphics gr, string name, Font font, Font normalFont, Brush brush, Rectangle bounds, bool showPreview, bool rightToLeft) {
			using(StringFormat sf = new StringFormat()) {
				sf.FormatFlags |= StringFormatFlags.NoWrap;
				if(rightToLeft) sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				if(font.Height < bounds.Height || font.Height / bounds.Height > 2)
					sf.LineAlignment = StringAlignment.Center;
				if(!DevExpress.Utils.ControlUtils.IsSymbolFont(font)) {
					try { 
					gr.DrawString(name, font, brush, bounds, sf);
					}
					catch {
						gr.DrawString(name, normalFont, brush, bounds, sf); 
					}
				}
				else {
					gr.DrawString(name, normalFont, brush, bounds, sf);
					if(showPreview) {
						SizeF size = gr.MeasureString(name + "w", normalFont);
						RectangleF newBounds = bounds;
						newBounds.Offset(rightToLeft ? -size.Width : size.Width, 0);
						gr.DrawString(name, font, brush, newBounds, sf);
					}
				}
			}
		}
	}
	public abstract class ResentlyItemsHolderBase {
		int maxRecentlyUsedItemCount = 7;
		ArrayList recentItems = new ArrayList();
		public int Count { get { return recentItems.Count; } }
		public void RememberRecentItem(object item) {
			int oldRecentItemsCount = recentItems.Count;
			recentItems.Remove(item);
			recentItems.Insert(0, item);
			if(recentItems.Count > MaxRecentlyUsedItemCount) {
				recentItems.RemoveRange(recentItems.Count - 1, recentItems.Count - MaxRecentlyUsedItemCount);
			}
			UpdateItems(oldRecentItemsCount);
		}
		protected abstract void InsertItem(int index, object item);
		protected abstract void ClearItems(int itemsCount);
		protected abstract void BeginUpdate();
		protected abstract void EndUpdate();
		protected abstract bool EqualsAtIndex(int index, object item);
		internal ArrayList RecentItems { get { return recentItems; } }
		internal int MaxRecentlyUsedItemCount {
			get { return maxRecentlyUsedItemCount; }
			set { 
				maxRecentlyUsedItemCount = value; 
			}
		}
		void AddRecentItems(int addCount) {
			for(int i = 0; i < addCount; i++)
				InsertItem(i, recentItems[i]);
		}
		internal void UpdateItems(int oldRecentItemsCount) {
			if(!NeedsUpdate(oldRecentItemsCount)) return;
			BeginUpdate();
			ClearItems(oldRecentItemsCount);
			AddRecentItems(recentItems.Count);
			EndUpdate();
		}
		bool NeedsUpdate(int oldRecentItemsCount) {
			if(oldRecentItemsCount != recentItems.Count || oldRecentItemsCount == MaxRecentlyUsedItemCount) return true;
			for(int i = 0; i < oldRecentItemsCount; i++) {
				if(EqualsAtIndex(i, recentItems[i])) return true;
			}
			return false;
		}
	}
	public class RecentlyUsedItemsComboBox : RepositoryItemComboBox, IRecentlyUsedItems {
		#region InnerClasses
		internal class ResentlyItemsHolder : ResentlyItemsHolderBase {
			RepositoryItemComboBox repositoryItemComboBox;
			public ResentlyItemsHolder(RepositoryItemComboBox repositoryItemComboBox) {
				this.repositoryItemComboBox = repositoryItemComboBox;
			}
			internal void RefreshItems() {
				if(Count > MaxRecentlyUsedItemCount) {
					int i = Count;
					int c = RecentItems.Count - MaxRecentlyUsedItemCount;
					RecentItems.RemoveRange(Count - c, c);
					UpdateItems(i);
				}
			}
			internal void ClearItems() {
				this.RecentItems.Clear();
			}
			protected override void InsertItem(int index, object item) {
				repositoryItemComboBox.Items.Insert(index, item.ToString());
			}
			protected override void ClearItems(int itemsCount) {
				for(int i = 0; i < itemsCount; i++)
					repositoryItemComboBox.Items.RemoveAt(0);
			}
			protected override void BeginUpdate() {
				repositoryItemComboBox.BeginUpdate();
			}
			protected override void EndUpdate() {
				repositoryItemComboBox.EndUpdate();
			}
			protected override bool EqualsAtIndex(int index, object item) {
				return repositoryItemComboBox.Items[index].ToString() == item.ToString();
			}
		}
		#endregion
		internal ResentlyItemsHolder resentlyItemsHolder;
		public RecentlyUsedItemsComboBox()
			: base() {
			resentlyItemsHolder = new ResentlyItemsHolder(this);
			DrawItem += new ListBoxDrawItemEventHandler(OnFontBoxDrawItem);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DrawItem -= new ListBoxDrawItemEventHandler(OnFontBoxDrawItem);
			}
			base.Dispose(disposing);
		}
		void IRecentlyUsedItems.RememberItem(object item) {
			Font font = item as Font;
			if(font != null && font.Name != item.ToString()) return; 
			if(item.ToString() == string.Empty) return;
			resentlyItemsHolder.RememberRecentItem(item);
		}
		public void StoreRecentItem(object item) {
			((IRecentlyUsedItems)this).RememberItem(item);
		}
		static Color BorderColor {
			get {
				if(NativeVista.IsVista) return SystemColors.ControlDark;
				return SystemColors.InactiveBorder;
			}
		}
		void OnFontBoxDrawItem(object sender, ListBoxDrawItemEventArgs e) {
			DrawFontBoxItem(e, this);
			if(e.Index == resentlyItemsHolder.Count - 1) {
				using(Pen pen = new Pen(BorderColor)) {
					e.Graphics.DrawLine(pen, new Point(e.Bounds.Left, e.Bounds.Bottom), new Point(e.Bounds.Right, e.Bounds.Bottom));
					e.Graphics.DrawLine(pen, new Point(e.Bounds.Left, e.Bounds.Bottom - 2), new Point(e.Bounds.Right, e.Bounds.Bottom - 2));
				}
			}
		}
		protected virtual void DrawFontBoxItem(ListBoxDrawItemEventArgs e, RepositoryItem item) {
			FontItemPaintHelper.DrawFontBoxItem(e, item);
		}
	}
}
