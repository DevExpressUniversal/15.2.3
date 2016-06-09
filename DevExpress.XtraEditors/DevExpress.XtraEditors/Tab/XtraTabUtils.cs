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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using System.Drawing.Imaging;
namespace DevExpress.XtraTab {
	public enum TabHeaderLocation { Left, Right, Top, Bottom };
	public enum TabOrientation { Default, Horizontal, Vertical };
	public enum TabPageImagePosition { Near, Far, Center, None};
	public enum TabButtonShowMode { Default, Always, Never, WhenNeeded };
	[ListBindable(false)]
	[Editor("DevExpress.XtraTab.Design.XtraTabPageCollectionEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class XtraTabPageCollection : CollectionBase, IList , IEnumerable<XtraTabPage>{
		XtraTabControl tabControl;
		public XtraTabPageCollection(XtraTabControl tabControl) {
			this.tabControl = tabControl;
		}
		public event CollectionChangeEventHandler CollectionChanged;
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("XtraTabPageCollectionItem")]
#endif
		public virtual XtraTabPage this[int index] { get { return List[index] as XtraTabPage; } }
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("XtraTabPageCollectionTabControl")]
#endif
		public virtual XtraTabControl TabControl { get { return tabControl; } }
		public virtual XtraTabPage Add() { return Add(""); }
		public virtual XtraTabPage Add(string text) {
			XtraTabPage page = CreatePage();
			if(text != string.Empty) page.Text = text;
			Add(page);
			return page;
		}
		public virtual void Add(XtraTabPage page) {
			if(Contains(page)) return;
			List.Add(page);
		}
		public virtual void AddRange(XtraTabPage[] pages) {
			foreach(XtraTabPage page in pages) Add(page);
		}
		public virtual void Remove(XtraTabPage page) {
			if(List.Contains(page)) List.Remove(page);
		}
		public virtual void Move(int newPosition, XtraTabPage page) {
			int oldIndex = IndexOf(page);
			if(newPosition < 0) newPosition = 0;
			if(newPosition >= Count) newPosition = Count - (oldIndex < newPosition ? 0 : 1);
			if(oldIndex == -1 || oldIndex == newPosition) return;
			InnerList.RemoveAt(oldIndex);
			if(oldIndex < newPosition) newPosition --;
			InnerList.Insert(newPosition, page);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		public virtual void Insert(int position) {
			Insert(position, CreatePage());
		}
		public virtual void Insert(int position, XtraTabPage page) {
			if(List.Contains(page)) return;
			List.Insert(position, page);
		}
		protected override void OnInsert(int position, object value) {
			if(!(value is XtraTabPage)) return;
			base.OnInsert(position, value);
		}
		protected override void OnClear() {
			TabControl.BeginUpdate();
			try {
				for(int n = Count - 1; n >= 0; n--) {
					RemoveAt(n);
				}
			}
			finally {
				TabControl.EndUpdate();
			}
		}
		protected override void OnInsertComplete(int position, object value) {
			base.OnInsertComplete(position, value);
			XtraTabPage page = value as XtraTabPage;
			page.SetTabControl(TabControl);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, page));
		}
		protected override void OnRemoveComplete(int position, object value) {
			base.OnRemoveComplete(position, value);
			XtraTabPage page = value as XtraTabPage;
			page.SetTabControl(null);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, page));
		}
		public virtual bool Contains(XtraTabPage page) { return List.Contains(page); }
		public virtual int IndexOf(XtraTabPage page) { return List.IndexOf(page); }
		protected virtual XtraTabPage CreatePage() { return new XtraTabPage(); }
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		#region IEnumerable<XtraTabPage>
		IEnumerator<XtraTabPage> IEnumerable<XtraTabPage>.GetEnumerator() {
			return new IListEnumerator<XtraTabPage>(this.List);
		}
		sealed class IListEnumerator<T> : IEnumerator<T> {
			IEnumerator _listEnumerator;
			internal IListEnumerator(IList list) {
				_listEnumerator = list.GetEnumerator();
			}
			public void Dispose() {
				_listEnumerator = null;
			}
			public bool MoveNext() {
				return _listEnumerator.MoveNext();
			}
			void IEnumerator.Reset() {
				_listEnumerator.Reset();
			}
			public T Current { 
				get { return (T)_listEnumerator.Current; } 
			}
			object IEnumerator.Current {
				get { return _listEnumerator.Current; }
			}
		}
		#endregion IEnumerable<XtraTabPage>
	}
	static class ColoredTabSkinElementPainter {
		public static void Draw(GraphicsCache cache, SkinElementInfo info, Image img) {
			if(img != null) {
				int headerClip = info.Element.Properties.GetInteger(TabSkinProperties.ColoredTabClipMargin);
				System.Drawing.Drawing2D.GraphicsState state = null;
				if(headerClip > 0) {
					state = cache.Graphics.Save();
					ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
					Rectangle clip = new Rectangle(
						info.Bounds.Left + headerClip, info.Bounds.Top + headerClip,
						info.Bounds.Width - headerClip * 2, info.Bounds.Height - headerClip);
					cache.Graphics.ExcludeClip(clip);
				}
				Image oldImage = info.GetActualImage();
				info.SetActualImage(img, true);
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
				info.SetActualImage(oldImage, false);
				if(state != null)
					cache.Graphics.Restore(state);
			}
			else ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
	}
	static class ColoredTabElementsCache {
		static IDictionary<Color, Image> tabHeaderImages = new Dictionary<Color, Image>();
		static IDictionary<Color, Image> tabPaneImages = new Dictionary<Color, Image>();
		public static Color ConvertColor(Color value, Color? color) {
			if(!color.HasValue) return value;
			return Color.FromArgb(Convert(value.ToArgb(), color.Value.ToArgb()));
		}
		static int ConvertChannel(double color, double value) {
			if(value < 128)
				return (int)(4.0 * value * color * (255.0 - value)) >> 0x10;
			return 255 + ((int)(4.0 * value * (255.0 - color) * (value - 255.0)) >> 0x10);
		}
		static int Convert(int value, int color) {
			byte alpha = (byte)((value >> 0x18) & 0xffL);
			double r = (double)((value >> 0x10) & 0xffL);
			double g = (double)((value >> 0x08) & 0xffL);
			double b = (double)(value & 0xffL);
			double rc = (double)((color >> 0x10) & 0xffL);
			double gc = (double)((color >> 0x08) & 0xffL);
			double bc = (double)(color & 0xffL);
			byte red = (byte)ConvertChannel(rc, r);
			byte green = (byte)ConvertChannel(gc, g);
			byte blue = (byte)ConvertChannel(bc, b);
			return (int)(((uint)((((red << 0x10) | (green << 8)) | blue) | (alpha << 0x18))) & 0xffffffffL);
		}
		[System.Security.SecuritySafeCritical]
		static Image GetColoredImage(SkinElementInfo info, Color color) {
			if(!info.HasActualImage) return null;
			Image sourceImage = info.GetActualImage();
			int w = sourceImage.Width; int h = sourceImage.Height;
			Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
			bmp.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
			using(Graphics g = Graphics.FromImage(bmp)) {
				g.DrawImageUnscaled(sourceImage, 0, 0);
			}
			BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, bmp.PixelFormat);
			int argbColor = color.ToArgb();
			try {
				int argb; int offset = 0;
				for(int i = 0; i < w * h; i++) {
					argb = System.Runtime.InteropServices.Marshal.ReadInt32(bmpData.Scan0, offset);
					argb = Convert(argb, argbColor);
					System.Runtime.InteropServices.Marshal.WriteInt32(bmpData.Scan0, offset, argb);
					offset += sizeof(int);
				}
			}
			finally { bmp.UnlockBits(bmpData); }
			return bmp;
		}
		static Image GetCachedImage(IDictionary<Color, Image> cache, Color color, SkinElementInfo info) {
			Image image;
			if(!cache.TryGetValue(color, out image)) {
				image = GetColoredImage(info, color);
				cache.Add(color, image);
			}
			return image;
		}
		static void ResetCachedImage(IDictionary<Color, Image> cache, Color color) {
			Image image;
			if(cache.TryGetValue(color, out image)) {
				if(image != null)
					image.Dispose();
				cache.Remove(color);
			}
		}
		static void ResetCache(IDictionary<Color, Image> cache) {
			foreach(KeyValuePair<Color, Image> pair in cache) {
				if(pair.Value != null)
					pair.Value.Dispose();
			}
			cache.Clear();
		}
		public static Image GetTabHeaderImage(Drawing.ColoredTabSkinElementInfo info) {
			Color? color = info.Color;
			return color.HasValue ? GetCachedImage(tabHeaderImages, color.Value, info) : null;
		}
		public static Image GetTabPaneImage(Drawing.ColoredTabSkinElementInfo info) {
			Color? color = info.Color;
			return color.HasValue ? GetCachedImage(tabPaneImages, color.Value, info) : null;
		}
		public static void Reset(Color color) {
			ResetCachedImage(tabHeaderImages, color);
			ResetCachedImage(tabPaneImages, color);
		}
		public static void Reset() {
			ResetCache(tabHeaderImages);
			ResetCache(tabPaneImages);
		}
	}
}
