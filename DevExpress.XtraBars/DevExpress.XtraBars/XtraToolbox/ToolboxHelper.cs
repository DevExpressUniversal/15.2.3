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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
namespace DevExpress.XtraToolbox {
	public static class ToolboxHelper {
		const string TrimmingEnd = "...";
		public static string[] WrapText(GraphicsCache cache, AppearanceObject obj, string s, int maxWidth) {
			s.Replace(FontCache.TabStopChar, FontCache.SpaceChar);
			s.Replace("\n", " \n ");
			string[] word = s.Split(FontCache.SpaceChar);
			int currentWord = 0;
			string firstLine = GetLine(cache, obj, maxWidth, word, ref currentWord);
			firstLine = CheckLine(cache, obj, maxWidth, firstLine);
			if(currentWord >= word.Length)
				return new string[] { firstLine };
			string secondLine = GetLine(cache, obj, maxWidth, word, ref currentWord);
			if(currentWord < word.Length)
				secondLine += FontCache.SpaceChar + word[currentWord];
			secondLine = CheckLine(cache, obj, maxWidth, secondLine);
			return new string[] { firstLine, secondLine };
		}
		static string CheckLine(GraphicsCache cache, AppearanceObject obj, int maxWidth, string s) {
			if(IsWordFit(cache, obj, s, maxWidth))
				return s;
			return TrimLine(cache, obj, s, maxWidth);
		}
		static string GetLine(GraphicsCache cache, AppearanceObject obj, int maxWidth, string[] word, ref int startWord) {
			int currentWord = startWord;
			string res = string.Empty;
			while(currentWord < word.Length) {
				if(word[currentWord] == "\n") {
					currentWord++;
					break;
				}
				if(currentWord == startWord) {
					res += word[currentWord];
				}
				else if(IsWordFit(cache, obj, res + word[currentWord], maxWidth)) {
					res += FontCache.SpaceChar + word[currentWord];
				}
				else break;
				currentWord++;
			}
			startWord = currentWord;
			return res;
		}
		static bool IsWordFit(GraphicsCache cache, AppearanceObject obj, string s, int maxWidth) {
			return obj.CalcTextSizeInt(cache, s, 0).Width < maxWidth;
		}
		static string TrimLine(GraphicsCache cache, AppearanceObject obj, string s, int maxWidth) {
			int trimmingEndWidth = obj.CalcTextSizeInt(cache, TrimmingEnd, 0).Width;
			int maxTextWidth = maxWidth - trimmingEndWidth;
			while(!IsWordFit(cache, obj, s, maxTextWidth)) {
				if(s.Length == 0)
					break;
				s = s.Substring(0, s.Length - 1);
			}
			return s + TrimmingEnd;
		}
		internal static Rectangle ApplyPaddings(Rectangle rect, Padding padding) {
			if(padding == Padding.Empty) return rect;
			Rectangle contentRect = rect;
			contentRect.X += padding.Left;
			contentRect.Width -= padding.Horizontal;
			contentRect.Y += padding.Top;
			contentRect.Height -= padding.Vertical;
			return contentRect;
		}
		internal static ImageAttributes GetImageAttributes(float opacity) {
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(GetOpacityMatrix(opacity));
			return attributes;
		}
		static ColorMatrix opacityMatrix = new ColorMatrix();
		static ColorMatrix GetOpacityMatrix(float opacity) {
			opacityMatrix.Matrix33 = opacity;
			return opacityMatrix;
		}
	}
	public class ToolboxImageCache {
		Dictionary<ToolboxItem, Image> cache;
		ToolboxViewInfo viewInfo;
		public ToolboxImageCache(ToolboxViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			this.cache = new Dictionary<ToolboxItem, Image>();
		}
		public Image GetItemImage(ToolboxItem item) {
			Image res;
			if(this.cache.TryGetValue(item, out res)) {
				return res;
			}
			res = RaiseGetItemImage(item);
			cache.Add(item, res);
			return res;
		}
		protected Image RaiseGetItemImage(ToolboxItem item) {
			ToolboxGetItemImageEventArgs e = new ToolboxGetItemImageEventArgs(item, viewInfo.GetItemImageSize());
			viewInfo.Toolbox.RaiseGetItemImage(e);
			return e.Image;
		}
		public void RemoveItemImage(ToolboxItem item) {
			if(!cache.ContainsKey(item)) return;
			cache.Remove(item);
		}
		public void Clear() {
			cache.Clear();
		}
	}
	[ToolboxItem(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ToolboxSmoothVScrollBar : XtraEditors.VScrollBar {
		Timer timer;
		bool isRunning;
		const int power = 5;
		public ToolboxSmoothVScrollBar() {
			timer = new Timer();
			timer.Tick += OnTimerTick;
			timer.Interval = 20;
		}
		int path;
		protected virtual int Path {
			get {
				if(Value - path < Minimum)
					return Value;
				if(Value - path > Maximum - LargeChange)
					return -(Maximum - LargeChange - Value);
				return path;
			}
			set { path = value; }
		}
		protected virtual void OnTimerTick(object sender, EventArgs e) {
			int step = Path / power;
			ValueCore -= step;
			path -= step;
			if(step != 0) return;
			Stop();
		}
		protected virtual void Start() {
			if(isRunning) return;
			timer.Start();
			isRunning = true;
		}
		protected virtual void Stop() {
			timer.Stop();
			ValueCore -= path;
			path = 0;
			isRunning = false;
		}
		public void SetValue(int delta) {
			path += delta;
			Start();
		}
		protected override int ValueCore {
			get { return base.ValueCore; }
			set {
				base.ValueCore = value;
				if(base.ValueCore > Maximum - LargeChange)
					base.ValueCore = Maximum - LargeChange;
				if(base.ValueCore < Minimum)
					base.ValueCore = Minimum;
				OnValueChanged(EventArgs.Empty);
			}
		}
	}
	public class ToolboxExpandCollapseAnimationInfo : BaseAnimationInfo {
		bool isExpanding;
		public ToolboxExpandCollapseAnimationInfo(ISupportXtraAnimation obj, object animationId, bool expanding, int ms)
			: base(obj, animationId, 10, (int)(TimeSpan.TicksPerMillisecond * ms / 10)) {
			this.isExpanding = expanding;
			Toolbox = (ToolboxControl)obj.OwnerControl;
			StartWidth = CurrentWidth = Toolbox.Width;
			EndWidth = !expanding ? Toolbox.ViewInfo.GetExpandedWidth() : Toolbox.ViewInfo.GetMinimizedWidth();
			Toolbox.ViewInfo.IsInAnimation = true;
		}
		bool IsExpanding { get { return isExpanding; } }
		public ToolboxControl Toolbox { get; private set; }
		public int StartWidth { get; private set; }
		public int CurrentWidth { get; private set; }
		public int EndWidth { get; private set; }
		public override void FrameStep() {
			float k = ((float)(CurrentFrame)) / FrameCount;
			FrameStepCore(k);
			Invalidate();
			if(!IsFinalFrame) return;
			Toolbox.ViewInfo.AnimationComplete();
		}
		protected virtual void Invalidate() {
			Toolbox.Width = CurrentWidth;
		}
		protected virtual void FrameStepCore(float k) {
			if(IsFinalFrame) k = 1.0f;
			int newWidth = StartWidth + (int)(k * (EndWidth - StartWidth));
			Toolbox.ViewInfo.OnSizeChanged(newWidth - CurrentWidth);
			CurrentWidth = newWidth;
		}
	}
	public class ToolboxFadeAnimationInfo : BaseAnimationInfo {
		public ToolboxFadeAnimationInfo(ISupportXtraAnimation anim, object animationId, int ms)
			: base(anim, animationId, 10, (int)(TimeSpan.TicksPerMillisecond * ms / 10)) {
			Toolbox = (ToolboxControl)anim.OwnerControl;
			StartOpacity = CurrentOpacity = 1;
			EndOpacity = 0;
		}
		public ToolboxControl Toolbox { get; private set; }
		public ToolboxViewInfo ViewInfo { get { return Toolbox.ViewInfo; } }
		public float StartOpacity { get; private set; }
		public float CurrentOpacity { get; private set; }
		public float EndOpacity { get; private set; }
		public override void FrameStep() {
			float k = ((float)(CurrentFrame)) / FrameCount;
			FrameStepCore(k);
			Invalidate();
		}
		protected virtual void Invalidate() {
			Toolbox.Invalidate();
		}
		protected virtual void FrameStepCore(float k) {
			if(IsFinalFrame) k = 1.0f;
			CurrentOpacity = StartOpacity + (k * (EndOpacity - StartOpacity));
			ViewInfo.PrevToolboxBitmapOpacity = CurrentOpacity;
		}
	}
	public class ToolboxRectangles {
		class ToolboxRectangleComparer : IEqualityComparer<ToolboxRectangle> {
			public bool Equals(ToolboxRectangle x, ToolboxRectangle y) {
				return x == y;
			}
			public int GetHashCode(ToolboxRectangle obj) {
				return (int)obj;
			}
		}
		Dictionary<ToolboxRectangle, Rectangle> rectangles;
		public ToolboxRectangles() { }
		Dictionary<ToolboxRectangle, Rectangle> Rectangles {
			get {
				if(rectangles == null)
					rectangles = new Dictionary<ToolboxRectangle, Rectangle>(new ToolboxRectangleComparer());
				return rectangles;
			}
		}
		public virtual void Clear() {
			this.rectangles = null;
		}
		public virtual void Offset(ToolboxRectangle rect, int x, int y) {
			Rectangle r = this[rect];
			r.Offset(x, y);
			this[rect] = r;
		}
		public virtual void SetWidthHeight(ToolboxRectangle rect, int width, int height) {
			Rectangle r = this[rect];
			r.Width = width;
			r.Height = height;
			this[rect] = r;
		}
		public virtual void SetWidth(ToolboxRectangle rect, int width) {
			SetWidthHeight(rect, width, this[rect].Height);
		}
		public virtual void AddWidth(ToolboxRectangle rect, int delta) { AddWidthHeight(rect, delta, 0); }
		public virtual void AddHeight(ToolboxRectangle rect, int delta) { AddWidthHeight(rect, 0, delta); }
		public virtual void AddWidthHeight(ToolboxRectangle rect, int deltaX, int deltaY) {
			Rectangle r = this[rect];
			r.Width += deltaX;
			r.Height += deltaY;
			this[rect] = r;
		}
		Rectangle this[ToolboxRectangle rect] {
			get {
				Rectangle res;
				if(this.rectangles != null && this.rectangles.TryGetValue(rect, out res))
					return res;
				return Rectangle.Empty;
			}
			set { Rectangles[rect] = value; }
		}
		public Rectangle HeaderRect {
			get { return this[ToolboxRectangle.HeaderRect]; }
			set { this[ToolboxRectangle.HeaderRect] = value; }
		}
		public Rectangle ItemsClientRect {
			get { return this[ToolboxRectangle.ItemsClientRect]; }
			set { this[ToolboxRectangle.ItemsClientRect] = value; }
		}
		public Rectangle ItemsContentRect {
			get { return this[ToolboxRectangle.ItemsContentRect]; }
			set { this[ToolboxRectangle.ItemsContentRect] = value; }
		}
		public Rectangle GroupsClientRect {
			get { return this[ToolboxRectangle.GroupsClientRect]; }
			set { this[ToolboxRectangle.GroupsClientRect] = value; }
		}
		public Rectangle GroupsContentRect {
			get { return this[ToolboxRectangle.GroupsContentRect]; }
			set { this[ToolboxRectangle.GroupsContentRect] = value; }
		}
		public Rectangle HeaderCaptionRect {
			get { return this[ToolboxRectangle.HeaderCaptionRect]; }
			set { this[ToolboxRectangle.HeaderCaptionRect] = value; }
		}
		public Rectangle SearchContentRect {
			get { return this[ToolboxRectangle.SearchContentRect]; }
			set { this[ToolboxRectangle.SearchContentRect] = value; }
		}
	}
}
