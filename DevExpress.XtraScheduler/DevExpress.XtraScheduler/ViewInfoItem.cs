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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Drawing {
	public interface IViewInfoItemPainter {
		void DrawTextItem(GraphicsCache cache, ViewInfoTextItem item);
		void DrawVerticalTextItem(GraphicsCache cache, ViewInfoVerticalTextItem item);
		void DrawImageItem(GraphicsCache cache, ViewInfoImageItem item);
		void DrawHorizontalLineItem(GraphicsCache cache, ViewInfoHorizontalLineItem item);
	}
	#region ViewInfoItem (abstract class)
	public abstract class ViewInfoItem : IDisposable {
		bool isDisposed;
		Rectangle bounds;
		internal bool IsDisposed { get { return isDisposed; } }
		public virtual Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		protected internal abstract Size CalcContentSize(GraphicsCache cache, Rectangle availableBounds);
		public abstract void Draw(GraphicsCache cache, IViewInfoItemPainter painter);
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ViewInfoItem() {
			Dispose(false);
		}
		#endregion
	}
	#endregion
	#region ViewInfoAppearanceItem (abstract class)
	public abstract class ViewInfoAppearanceItem : ViewInfoItem {
		AppearanceObject appearance = new AppearanceObject();
		public AppearanceObject Appearance { get { return appearance; } }
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (appearance != null) {
						appearance.Dispose();
						appearance = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
	}
	#endregion
	#region ViewInfoItemCollection
	public class ViewInfoItemCollection : List<ViewInfoItem> {
	}
	#endregion
	#region ViewInfoImageItem
	public class ViewInfoImageItem : ViewInfoItem {
		Image image;
		public ViewInfoImageItem(Image image) {
			if (image == null)
				Exceptions.ThrowArgumentException("image", image);
			this.image = image;
		}
		public Image Image { get { return image; } }
		protected internal override Size CalcContentSize(GraphicsCache cache, Rectangle availableBounds) {
			lock (image)
				return image.Size;
		}
		public override void Draw(GraphicsCache cache, IViewInfoItemPainter painter) {
			painter.DrawImageItem(cache, this);
		}
		internal void SetImage(Image image) {
			this.image = image;
		}
	}
	#endregion
	#region ViewInfoFreeSpaceItem
	public class ViewInfoFreeSpaceItem : ViewInfoItem {
		Size size;
		public ViewInfoFreeSpaceItem() {
			size = new Size(0, 0);
		}
		public Size Size { get { return size; } set { size = value; } }
		protected internal override Size CalcContentSize(GraphicsCache cache, Rectangle availableBounds) {
			return size;
		}
		public override void Draw(GraphicsCache cache, IViewInfoItemPainter painter) {
		}
	}
	#endregion
	#region ViewInfoTextItem
	public class ViewInfoTextItem : ViewInfoAppearanceItem, IViewInfoTextItem {
		string text = String.Empty;
		public string Text { get { return text; } set { text = value; } }
		protected internal override Size CalcContentSize(GraphicsCache cache, Rectangle availableBounds) {
			SizeF size;
			string measureText = GetMeasureText();
			if (String.IsNullOrEmpty(measureText))
				size = Appearance.CalcDefaultTextSize(cache.Graphics);
			else
				size = Appearance.CalcTextSize(cache, measureText, availableBounds.Width);
			return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
		}
		public override void Draw(GraphicsCache cache, IViewInfoItemPainter painter) {
			painter.DrawTextItem(cache, this);
		}
		protected internal virtual string GetMeasureText() {
			return Text;
		}
	}
	#endregion
	#region ViewInfoHorizontalTextItem
	public class ViewInfoHorizontalTextItem : ViewInfoTextItem {
	}
	#endregion
	#region ViewInfoHorizontalAutoHeightTextItem
	public class ViewInfoHorizontalAutoHeightTextItem : ViewInfoHorizontalTextItem {
		public override Rectangle Bounds {
			get {
				return base.Bounds;
			}
			set {
				base.Bounds = value;
			}
		}
		protected internal override Size CalcContentSize(GraphicsCache cache, Rectangle availableBounds) {
			SetTextWrap(cache, availableBounds);
			return base.CalcContentSize(cache, availableBounds);
		}
		protected internal void SetTextWrap(GraphicsCache cache, Rectangle itemBounds) {
			int minLineHeight = (int)Math.Floor(Appearance.Font.SizeInPoints / 72 * cache.Graphics.DpiY);
			if (itemBounds.Width < 2 * minLineHeight)
				Appearance.TextOptions.WordWrap = WordWrap.NoWrap;
			else
				Appearance.TextOptions.WordWrap = WordWrap.Wrap;
		}
	}
	#endregion
	#region ViewInfoHorizontalLineItem
	public class ViewInfoHorizontalLineItem : ViewInfoAppearanceItem {
		protected internal override Size CalcContentSize(GraphicsCache cache, Rectangle availableBounds) {
			return new Size(availableBounds.Width, 1);
		}
		public override void Draw(GraphicsCache cache, IViewInfoItemPainter painter) {
			painter.DrawHorizontalLineItem(cache, this);
		}
	}
	#endregion
	#region ViewInfoVerticalTextItem
	public class ViewInfoVerticalTextItem : ViewInfoTextItem {
		protected internal override Size CalcContentSize(GraphicsCache cache, Rectangle availableBounds) {
			Rectangle bounds = new Rectangle(availableBounds.Left, availableBounds.Top, availableBounds.Height, availableBounds.Width);
			Size result = base.CalcContentSize(cache, bounds);
			if (result != Size.Empty)
				result = SchedulerWinUtils.FitSizeForVerticalText(result);
			return new Size(result.Height, result.Width);
		}
		public override void Draw(GraphicsCache cache, IViewInfoItemPainter painter) {
			painter.DrawVerticalTextItem(cache, this);
		}
		protected internal override string GetMeasureText() {
			string result = base.GetMeasureText();
			if (!string.IsNullOrEmpty(Text))
				result += "i";
			return result;
		}
	}
	#endregion
	#region ViewInfoItemContainer (abstract class)
	public abstract class ViewInfoItemContainer : BorderObjectViewInfo, IDisposable {
		bool isDisposed;
		ViewInfoItemCollection items;
		protected ViewInfoItemContainer() {
			this.items = new ViewInfoItemCollection();
		}
		public ViewInfoItemCollection Items { get { return items; } }
		internal bool IsDisposed { get { return isDisposed; } }
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (items != null) {
					DisposeItems();
					items = null;
				}
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ViewInfoItemContainer() {
			Dispose(false);
		}
		#endregion
		protected internal virtual void DisposeItems() {
			DisposeItemsCore(Items);
		}
		protected internal virtual void DisposeItemsCore(ViewInfoItemCollection items) {
			if (items == null) return;
			int count = items.Count;
			for (int i = 0; i < count; i++)
				DisposeItemCore(items[i]);
		}
		protected internal virtual void DisposeItemCore(ViewInfoItem item) {
			if (item != null) item.Dispose();
		}
	}
	#endregion
}
