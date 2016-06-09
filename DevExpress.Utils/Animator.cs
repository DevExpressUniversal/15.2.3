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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using System.Diagnostics;
using System.ComponentModel;
namespace DevExpress.Utils.Drawing.Animation {
	public interface ISupportXtraAnimation {
		Control OwnerControl { get; }
		bool CanAnimate { get; }
	}
	public interface IXtraAnimationListener {
		void OnAnimation(BaseAnimationInfo info);
		void OnEndAnimation(BaseAnimationInfo info);
	}
	public interface IAnimatedItem {
		Rectangle AnimationBounds { get; }
		int GetAnimationInterval(int frameIndex);
		int[] AnimationIntervals { get; }
		int AnimationInterval { get; }
		int FramesCount { get; }
		bool IsAnimated { get; }
		void UpdateAnimation(BaseAnimationInfo info);
		object Owner { get; }
		AnimationType AnimationType { get; }
		void OnStart();
		void OnStop();
	}
	public class ObjectPaintInfo {
		ObjectPainter painter;
		ObjectInfoArgs info;
		public ObjectPaintInfo(ObjectPainter painter, ObjectInfoArgs info) {
			this.painter = painter;
			this.info = info;
		}
		public ObjectPainter Painter { get { return painter; } }
		public ObjectInfoArgs Info { get { return info; } }
		public void Draw(GraphicsCache cache) {
			ObjectPainter.DrawObject(cache, Painter, Info);
		}
		public Rectangle Bounds { get { return Info == null ? Rectangle.Empty : Info.Bounds; } }
		public bool IsEmpty { get { return Painter == null || Info == null || Info.Bounds.Width < 1 || info.Bounds.Height < 1; } }
	}
	public enum AnimationType { Default, Simple, Cycle }
	public abstract class BaseAnimationInfo : IDisposable {
		int deltaTick;
		int frameCount;
		int prevFrame;
		int[] deltaTicks;
		long beginTick;
		long currentTick;
		object animationId;
		ISupportXtraAnimation animObject;
		AnimationType animationType;
		bool finalFrameDrawn = false, forceLastFrame = false;
		public BaseAnimationInfo(ISupportXtraAnimation obj, object animationId, int deltaTick, int frameCount) {
			this.animObject = obj;
			this.deltaTick = deltaTick;
			this.frameCount = frameCount;
			this.animationId = animationId;
			this.beginTick = -1;
			this.currentTick = -1;
			this.prevFrame = -1;
			this.animationType = AnimationType.Default;
		}
		public BaseAnimationInfo(ISupportXtraAnimation obj, object animationId, int[] deltaTicks, int frameCount) : this(obj, animationId, -1, frameCount) {
			this.deltaTicks = deltaTicks;
		}
		public BaseAnimationInfo(ISupportXtraAnimation obj, object animationId, int[] deltaTicks, int frameCount, AnimationType type) : this(obj, animationId, deltaTicks, frameCount) {
			this.animationType = type;
		}
		public BaseAnimationInfo(ISupportXtraAnimation obj, object animationId, int deltaTick, int frameCount, AnimationType type)
			: this(obj, animationId, deltaTick, frameCount) {
			this.animationType = type;
		}
		public virtual void Dispose() { }
		public virtual void ForceLastFrameStep() {
			this.forceLastFrame = true;
			FrameStep();
		}
		public abstract void FrameStep();
		public bool IsFinalFrame { get { return CurrentFrame >= FrameCount - 1; } }
		public bool IsFinalFrameDrawn { get { return finalFrameDrawn; } set { finalFrameDrawn = value; } }
		public int DeltaTick { get { return deltaTick; } set { deltaTick = value; } }
		public int[] DeltaTicks { get { return deltaTicks; } set { deltaTicks = value; } }
		public int FrameCount { get { return frameCount; } set { frameCount = value; } }
		public long BeginTick { get { return beginTick; } set { beginTick = value; } }
		public AnimationType AnimationType { get { return animationType; } set { animationType = value; } }
		public long CurrentTick {
			get { return currentTick; }
			set {
				prevFrame = CurrentFrame;
				currentTick = value;
			}
		} 
		public ISupportXtraAnimation AnimatedObject { get { return animObject; } set { animObject = value; } }
		public object AnimationId { get { return animationId; } }
		public virtual int PrevFrame { get { return prevFrame; } }
		public int AnimationLength {
			get {
				if(DeltaTicks != null) { 
					int summ = 0;
					for(int i = 0; i < DeltaTicks.Length; i++) summ += DeltaTicks[i];
					return summ;
				}
				return DeltaTick * FrameCount;
			}
		}
		int GetFrameByTicks() {
			long dt = CurrentTick - BeginTick;
			if(AnimationType == AnimationType.Cycle) {
				int animLen = AnimationLength;
				if(animLen != 0) dt %= animLen;	
			}
			for(int i = 0; i < DeltaTicks.Length; i++) {
				dt -= DeltaTicks[i];
				if(dt < 0) return i;
			}
			return FrameCount - 1;
		}
		public virtual int CurrentFrame {
			get {
				if(forceLastFrame) return FrameCount - 1;
				if(DeltaTicks != null) return Math.Min(FrameCount, GetFrameByTicks());
				if(DeltaTick == 0) return 0;
				long dt = CurrentTick - BeginTick;
				if(AnimationType == AnimationType.Cycle && AnimationLength != 0) dt %= AnimationLength;
				return Math.Min(FrameCount, (int)(dt / DeltaTick));
			}
			set {
				frameCount = value;
			}
		}
	}
	public class ColorAnimationInfo : BaseAnimationInfo {
		public ColorAnimationInfo(ISupportXtraAnimation obj, object animationId, int ms, Color startColor, Color endColor) : base(obj, animationId, 10, (int)(TimeSpan.TicksPerMillisecond * ms / 10), AnimationType.Simple) {
			StartColor = startColor;
			EndColor = endColor;
			Helper = new SplineAnimationHelper();
			Helper.Init(0, 1, 1);
			CurrentColor = StartColor;
		}
		protected SplineAnimationHelper Helper { get;set; }
		public Color StartColor { get; private set; }
		public Color EndColor { get; private set; }
		public Color CurrentColor { get; private set; }
		public override void FrameStep() {
			float k = (float)CurrentFrame / FrameCount;
			k = (float)Helper.CalcSpline(k);
			CurrentColor = Color.FromArgb(CalcColorChannel(StartColor.A, EndColor.A, k),
									CalcColorChannel(StartColor.R, EndColor.R, k),
									CalcColorChannel(StartColor.G, EndColor.G, k),
									CalcColorChannel(StartColor.B, EndColor.B, k));
			Invalidate();
		}
		protected  void Invalidate() {
			if(AnimatedObject is Control)
				((Control)AnimatedObject).Invalidate();
		}
		protected int CalcColorChannel(int start, int end, float k) {
			return start + (int)((end - start) * k);
		}
	}
	public class FloatAnimationInfo : BaseAnimationInfo {
		const int framesCount = 100;
		bool useSpline;
		SplineAnimationHelper helper;
		public FloatAnimationInfo(ISupportXtraAnimation obj, object animationId, int ms, float start, float end, bool useSpline) : base(obj, animationId, CalcTicksCount(ms), framesCount) {
			Start = start;
			End = end;
			this.control = null;
			this.useSpline = useSpline;
			if(this.useSpline) {
				helper = new SplineAnimationHelper();
				helper.Init(0.0, 1.0, 1.0);
			}
			Value = Start;
		}   
		public FloatAnimationInfo(ISupportXtraAnimation obj, object animationId, int ms, float start, float end) : this(obj, animationId, ms, start, end, false) {
		}
		static int CalcTicksCount(int ms) {
			return (int)(ms * TimeSpan.TicksPerMillisecond / framesCount);
		}
		public override void FrameStep() {
			FrameStepCore(((float)(CurrentFrame)) / FrameCount);
			Invalidate();
		}
		protected virtual void FrameStepCore(float k) {
			IsStarted = true;
			if(k > 1) k = 1f;
			if(this.useSpline)
				k = (float)helper.CalcSpline(k);
			Value = Start + k * (End - Start);
		}
		public override void Dispose() {
			IsStarted = false;
			base.Dispose();
		}
		public bool IsStarted { get; private set; }
		Control control = null;
		protected virtual void Invalidate() {
			if(control == null)
				control = base.AnimatedObject as Control;
			if(control != null)
				control.Invalidate();
		}
		public float Start { get; protected set; }
		public float End { get; protected set; }
		public float Value { get; protected set; }
	}
	public delegate void DrawTextInvoker(GraphicsCache cache, object info);
	public delegate void CustomAnimationInvoker(BaseAnimationInfo animationInfo);
	public delegate Bitmap BitmapAnimationImageCallback(BaseAnimationInfo animationInfo);
	public class CustomAnimationInfo : BaseAnimationInfo {
		CustomAnimationInvoker method;
		public CustomAnimationInfo(ISupportXtraAnimation obj, object animationId, int deltaTick, int frameCount, CustomAnimationInvoker method) : this(obj, animationId, deltaTick, frameCount, AnimationType.Default, method) { }
		public CustomAnimationInfo(ISupportXtraAnimation obj, object animationId, int deltaTick, int frameCount, AnimationType type, CustomAnimationInvoker method) : base(obj, animationId, deltaTick, frameCount, type) {
			this.method = method;
		}
		public CustomAnimationInfo(ISupportXtraAnimation obj, object animationId, int[] deltaTicks, int frameCount, CustomAnimationInvoker method) : this(obj, animationId, deltaTicks, frameCount, AnimationType.Default, method) { }
		public CustomAnimationInfo(ISupportXtraAnimation obj, object animationId, int[] deltaTicks, int frameCount, AnimationType type, CustomAnimationInvoker method)
			: base(obj, animationId, deltaTicks, frameCount, type) {
			this.method = method;
		}
		public override void FrameStep() {
			if(Method != null) Method(this);
		}
		public override void Dispose() {
			this.method = null;
			base.Dispose();
		}
		public CustomAnimationInvoker Method { get { return method; } }
	}
	public class DoubleAnimationInfo : BaseAnimationInfo {
		public DoubleAnimationInfo(ISupportXtraAnimation supportAnimation, object animationId, double start, double end, int animLength) : base(supportAnimation, animationId, (int)(TimeSpan.TicksPerMillisecond * animLength / 300), 300) {
			Start = start;
			End = end;
		}
		public double Start { get; private set; }
		public double End { get; private set; }
		public double Value { get; private set; }
		public override void FrameStep() {
			Value = ((float)(CurrentFrame)) / FrameCount;
			if(IsFinalFrame) Value = 1.0;
			Value = Start + (End - Start) * Value;
		}
	}
	public class FormOpacityAnimationInfo : BaseAnimationInfo {
		Form form;
		bool fadeIn;
		public FormOpacityAnimationInfo(ISupportXtraAnimation obj, object animationId, Form frm, bool fadeIn, int lengthMsec)
			: base(obj, animationId, 1000 * 1000 / 50, (int)(lengthMsec * 0.001f * 50)) {
			this.form = frm;
			this.fadeIn = fadeIn;
		}
		public Form Form { get { return form; } }
		public bool FadeIn { get { return fadeIn; } }
		public override void FrameStep() {
			if(FadeIn) {
				DoSetFadeInOpacity();
				if(!Form.Visible)
					Form.Show();
			}
			else {
				DoSetFadeOutOpacity();
				if(IsFinalFrame) {
					Form.Hide();
				}
			}
		}
		protected virtual void DoSetFadeInOpacity() {
			Form.Opacity = ((float)CurrentFrame) / FrameCount;
		}
		protected virtual void DoSetFadeOutOpacity() {
			Form.Opacity = 1.0f - (float)(CurrentFrame) / FrameCount;
		}
	}
	public class FadeAnimationInfo : BitmapFadeAnimationInfo {
		public FadeAnimationInfo(ISupportXtraAnimation obj, object animationId, Rectangle bounds, int lengthMSec, bool fadeIn)
			: base(obj, animationId, (Bitmap)null, bounds, lengthMSec, fadeIn) {
		}
		protected override void DrawFrame(GraphicsCache cache) {
			if(IsFinalFrame)
				IsFinalFrameDrawn = true;
		}
	}
	public class BitmapFadeAnimationInfo : BitmapAnimationInfo {
		bool fadeIn;
		public BitmapFadeAnimationInfo(ISupportXtraAnimation obj, object animationId, Bitmap source, Rectangle bounds, int lengthMSec, bool fadeIn)
			: base(obj, animationId, source, (Bitmap)null, bounds, lengthMSec) {
			this.fadeIn = fadeIn;
		}
		protected override bool PrepareDraw() {
			return true;
		}
		protected override float CalcAlpha() {
			if(FadeIn) return base.CalcAlpha();
			return 1f - base.CalcAlpha();
		}
		public bool FadeIn { get { return fadeIn; } }
		protected override void DrawFrame(GraphicsCache cache) {
			if(!IsFinalFrame)
				cache.Paint.DrawImage(cache.Graphics, Source, Bounds, new Rectangle(Point.Empty, Bounds.Size), Attributes);
			else {
				IsFinalFrameDrawn = true;
			}
		}
		public override bool IsPostAnimation { get { return true; } }
	}
	public interface IXtraObjectWithBounds {
		Rectangle AnimatedBounds { get; set; }
		void OnEndBoundAnimation(BoundsAnimationInfo anim);
	}
	public class EditorAnimationInfo : CustomAnimationInfo {
		object link;
		object viewInfo;
		public EditorAnimationInfo(object link, ISupportXtraAnimation obj, int deltaTick, int frameCount, CustomAnimationInvoker method) : this(link, obj, deltaTick, frameCount, AnimationType.Default, method) { }
		public EditorAnimationInfo(object link, ISupportXtraAnimation obj, int deltaTick, int frameCount, AnimationType type, CustomAnimationInvoker method) 
			: this(link, null, obj, deltaTick, frameCount, type, method) { }
		public EditorAnimationInfo(object link, ISupportXtraAnimation obj, int[] deltaTicks, int frameCount, CustomAnimationInvoker method) : this(link, obj, deltaTicks, frameCount, AnimationType.Default, method) { }
		public EditorAnimationInfo(object link, ISupportXtraAnimation obj, int[] deltaTicks, int frameCount, AnimationType type, CustomAnimationInvoker method)
			: this(link, null, obj, deltaTicks, frameCount, type, method) { }
		public EditorAnimationInfo(object link, object viewInfo, ISupportXtraAnimation obj, int deltaTick, int frameCount, CustomAnimationInvoker method) : this(link, viewInfo, obj, deltaTick, frameCount, AnimationType.Default, method) { }
		public EditorAnimationInfo(object link, object viewInfo, ISupportXtraAnimation obj, int deltaTick, int frameCount, AnimationType type, CustomAnimationInvoker method)
			: base(obj, link, deltaTick, frameCount, type, method) {
			this.link = link;
			this.viewInfo = viewInfo;
		}
		public EditorAnimationInfo(object link, object viewInfo, ISupportXtraAnimation obj, int[] deltaTicks, int frameCount, CustomAnimationInvoker method) : this(link, viewInfo, obj, deltaTicks, frameCount, AnimationType.Default, method) { }
		public EditorAnimationInfo(object link, object viewInfo, ISupportXtraAnimation obj, int[] deltaTicks, int frameCount, AnimationType type, CustomAnimationInvoker method)
			: base(obj, link, deltaTicks, frameCount, type, method) {
			this.link = link;
			this.viewInfo = viewInfo;
		}
		public object Link { get { return link; } }
		public object ViewInfo { get { return viewInfo; } set { viewInfo = value; } }
	}
	public class BoundsAnimationInfo : BaseAnimationInfo {
		public static int MaxFrameCount { get { return 100; } }
		Rectangle beginBounds;
		Rectangle endBounds;
		Rectangle currBounds;
		bool growUp;
		IXtraObjectWithBounds objectWithBounds;
		public BoundsAnimationInfo(ISupportXtraAnimation obj, IXtraObjectWithBounds objectWithBounds, object animationId, Rectangle beginRect, Rectangle endRect, int ms, bool allowSpline, bool reverseSpline) : this(obj, objectWithBounds, animationId, false, beginRect, endRect, (int)(TimeSpan.TicksPerMillisecond * ms / (TimeSpan.TicksPerMillisecond * 10))) {
			AllowSpline = allowSpline;
			ReverseSpline = reverseSpline;
			if(AllowSpline) {
				Helper = new SplineAnimationHelper();
				Helper.Init(0.0, 1.0, 1.0);
			}
		}
		public BoundsAnimationInfo(ISupportXtraAnimation obj, IXtraObjectWithBounds objectWithBounds, object animationId, bool growUp, Rectangle beginRect, Rectangle endRect, int frameCount)
			: base(obj, animationId, (int)TimeSpan.TicksPerMillisecond * 10, frameCount) {
			this.beginBounds = beginRect;
			this.endBounds = endRect;
			this.currBounds = BeginBounds;
			this.objectWithBounds = objectWithBounds;
			this.growUp = growUp;
		}
		protected SplineAnimationHelper Helper { get; private set; }
		public bool AllowSpline { get; private set; }
		public bool ReverseSpline { get; private set; }
		public Rectangle BeginBounds { get { return beginBounds; } }
		public Rectangle EndBounds { get { return endBounds; } }
		public Rectangle CurrentBounds { get { return currBounds; } }
		public IXtraObjectWithBounds ObjectWithBounds { get { return objectWithBounds; } }
		public bool GrowUp { get { return growUp; } }
		protected virtual void CalcCurrentBounds() {
			float df = FrameCount == 0? 1.0f: ((float)CurrentFrame) / ((float)FrameCount);
			if(AllowSpline) {
				df = ReverseSpline ? 1.0f - df : df;
				df = (float)Helper.CalcSpline(df);
			}
			currBounds.X = BeginBounds.X + (int)((EndBounds.X - BeginBounds.X) * df);
			currBounds.Y = BeginBounds.Y + (int)((EndBounds.Y - BeginBounds.Y) * df);
			currBounds.Width = BeginBounds.Width + (int)((EndBounds.Width - BeginBounds.Width) * df);
			currBounds.Height = BeginBounds.Height + (int)((EndBounds.Height - BeginBounds.Height) * df);
		}
		public override void FrameStep() {
			CalcCurrentBounds();
			ObjectWithBounds.AnimatedBounds = CurrentBounds;
			if(CurrentFrame == FrameCount) ObjectWithBounds.OnEndBoundAnimation(this);
		}
	}
	public class BitmapAnimationInfo : BaseAnimationInfo {
		const long FrameSpeed = 1 * TimeSpan.TicksPerMillisecond; 
		const int MaxFrameCount = 60;
		Bitmap source;
		Bitmap destination;
		Rectangle bounds;
		ImageAttributes attributes;
		ColorMatrix matrix;
		BitmapAnimationImageCallback destinationCallback;
		public BitmapAnimationInfo(ISupportXtraAnimation obj, object animationId, Bitmap source, BitmapAnimationImageCallback destinationCallback, Rectangle bounds, int lengthMSec)
			: this(obj, animationId, source, null, destinationCallback, bounds, lengthMSec) {
		}
		public BitmapAnimationInfo(ISupportXtraAnimation obj, object animationId, Bitmap source, Bitmap destination, Rectangle bounds, int lengthMSec)
			: this(obj, animationId, source, destination, null, bounds, lengthMSec) { }
		public BitmapAnimationInfo(ISupportXtraAnimation obj, object animationId, Bitmap source, Bitmap destination, BitmapAnimationImageCallback destinationCallback, Rectangle bounds, int lengthMSec) :
						base(obj, animationId, 1, 1) {
			long length = ((long)lengthMSec) * TimeSpan.TicksPerMillisecond;
			FrameCount = (int)Math.Max(length / FrameSpeed, 2);
			FrameCount = Math.Min(MaxFrameCount, FrameCount);
			DeltaTick = (int)Math.Max(1, length / FrameCount);
			this.bounds = bounds;
			this.destination = destination;
			this.destinationCallback = destinationCallback;
			this.source = source;
			this.attributes = CreateAttributes();
		}
		public ImageAttributes Attributes { get { return attributes; } }
		public virtual bool IsPostAnimation { get { return false; } }
		public BitmapAnimationImageCallback DestinationCallback { get { return destinationCallback; } }
		public Bitmap Destination { get { return destination; } set { destination = value; } }
		public Bitmap Source { get { return source; } set { source = value; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		int madeSteps = 0;
		public override void FrameStep() {
			this.madeSteps++;
			if(AnimatedObject.OwnerControl != null && AnimatedObject.OwnerControl.IsHandleCreated) {
				AnimatedObject.OwnerControl.Invalidate(Bounds);
			}
		}
		protected virtual bool PrepareDraw() {
			if(Destination == null && DestinationCallback != null) {
				Destination = DestinationCallback(this);
			}
			if(Destination == null) return false;
			return true;
		}
		public virtual bool Draw(GraphicsCache cache) {
			if(!PrepareDraw()) return false;
			UpdateAttributes();
			DrawFrame(cache);
			return true;
		}
		protected virtual void DrawFrame(GraphicsCache cache) {
			ImageAttributes attr = this.attributes;
			if(!IsFinalFrame)
				cache.Paint.DrawImage(cache.Graphics, Source, Bounds, new Rectangle(Point.Empty, Bounds.Size), null);
			else {
				IsFinalFrameDrawn = true;
				attr = null;
			}
			cache.Paint.DrawImage(cache.Graphics, Destination, Bounds, new Rectangle(Point.Empty, Bounds.Size), attr);
		}
		public override void Dispose() {
			if(attributes != null) {
				this.attributes.Dispose();
				this.attributes = null;
			}
			if(source != null) source.Dispose();
			if(destination != null) destination.Dispose();
			this.source = this.destination = null;
			base.Dispose();
		}
		protected virtual ImageAttributes CreateAttributes() {
			ImageAttributes res = new ImageAttributes();
			this.matrix = new ColorMatrix();
			this.matrix.Matrix33 = 0.1f;
			res.SetColorMatrix(matrix);
			return res;
		}
		protected virtual float CalcAlpha() {
			float percent = ((float)CurrentFrame / (float)Math.Max(1, FrameCount));
			float alpha = 0;
			if(percent < 0.7f)
				alpha = Math.Min(1, percent * 0.6f);
			else
				alpha = Math.Min(1, percent);
			return alpha;
		}
		public void UpdateAttributes() {
			float alpha = CalcAlpha();
			if(this.matrix.Matrix33 == alpha) return;
			this.matrix.Matrix33 = alpha;
			this.attributes.SetColorMatrix(matrix);
		}
	}
	public class AnimationInfoCollection : CollectionBase {
		class HashInfo {
			Hashtable hash = new Hashtable();
			public HashInfo(BaseAnimationInfo info1, BaseAnimationInfo info2) {
				hash[info1.AnimationId] = info1;
				hash[info2.AnimationId] = info2;
			}
			public Hashtable Hash { get { return hash; } }
			public BaseAnimationInfo this[object animationId] {
				get { return Hash[animationId] as BaseAnimationInfo; }
			}
			public object Remove(object animationId) {
				Hash.Remove(animationId);
				if(Hash.Count < 2 && Hash.Count > 0) {
					IEnumerator enu = Hash.Values.GetEnumerator();
					enu.MoveNext();
					return enu.Current;
				}
				return this;
			}
		}
		const int MaxAnimatingObjectCount = 100;
		Hashtable hash = new Hashtable();
		public int Add(BaseAnimationInfo animInfo) {
			if(List.Count >= MaxAnimatingObjectCount) {
				BaseAnimationInfo toRemove = this[0];
				if(!toRemove.IsFinalFrameDrawn) {
					toRemove.ForceLastFrameStep();
				}
				RemoveAt(0);
			}
			return List.Add(animInfo);
		}
		public int GetAnimationsCountByObject(ISupportXtraAnimation animObj) {
			object res = hash[animObj];
			if(res is BaseAnimationInfo) return 1;
			if(res is HashInfo) return (res as HashInfo).Hash.Count;
			return 0;
		}
		public BaseAnimationInfo this[int index] { get { return List[index] as BaseAnimationInfo; } }
		public void Remove(BaseAnimationInfo animInfo) { 
			if(List.Contains(animInfo)) 
				List.Remove(animInfo); 
		}
		public void Remove(ISupportXtraAnimation animObj, object animId) {
			BaseAnimationInfo info = this[animObj, animId];
			if(info != null) Remove(info);
		}
		public void Remove(ISupportXtraAnimation animObj) {
			hash.Remove(animObj);
			for(int n = Count - 1; n >= 0; n--) {
				if(this[n].AnimatedObject == animObj) RemoveAt(n);
			}
		}
		public BaseAnimationInfo this[ISupportXtraAnimation animObj, object animId] { 
			get {
				if(animObj == null) return null;
				object res = hash[animObj];
				BaseAnimationInfo ani = res as BaseAnimationInfo;
				if(ani != null) {
					if(Object.Equals(ani.AnimationId, animId)) return ani;
					return null;
				}
				HashInfo info = res as HashInfo;
				if(info != null) return info[animId];
				return null;
			} 
		}
		public int Find(ISupportXtraAnimation animObj, object animId) {
			if(animObj == null) return -1;
			for(int n = List.Count - 1; n >= 0; n--) {
				BaseAnimationInfo anim = this[n];
				if(anim.AnimatedObject == animObj && anim.AnimationId == animId) return n;
			}
			return -1;
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			BaseAnimationInfo item = (BaseAnimationInfo)value;
			object animObject = hash[item.AnimatedObject];
			if(animObject == null) {
				hash[item.AnimatedObject] = item;
			}
			else {
				if(animObject is BaseAnimationInfo) {
					hash[item.AnimatedObject] = new HashInfo(animObject as BaseAnimationInfo, item);
				}
				else {
					HashInfo hashInfo = animObject as HashInfo;
					hashInfo.Hash[item.AnimationId] = item;
				}
			}
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			BaseAnimationInfo item = (BaseAnimationInfo)value;
			object obj = hash[item.AnimatedObject];
			if(obj == null) return;
			if(obj is BaseAnimationInfo) {
				hash.Remove(item.AnimatedObject);
			}
			else {
				HashInfo info = obj as HashInfo;
				hash[item.AnimatedObject] = info.Remove(item.AnimationId);
			}
			item.Dispose();
		}
	}
	public class XtraAnimator {
		[ThreadStatic]
		static XtraAnimator current;
		public static XtraAnimator Current {
			get {
				if(current == null) current = new XtraAnimator();
				return current;
			}
		}
		public static void RemoveObject(ISupportXtraAnimation obj, object animId) { 
			if(current == null) return;
			Current.Animations.Remove(obj, animId);
			Current.CheckTimerStop();
		}
		public static void RemoveObject(ISupportXtraAnimation obj) {
			if(current == null) return;
			Current.Animations.Remove(obj);
			Current.CheckTimerStop();
		}
		AnimationInfoCollection animations;
		Timer timer;
		Stopwatch stopwatch;
		XtraAnimator() {
			animations = new AnimationInfoCollection();
			timer = new Timer();
			timer.Interval = int.MaxValue;
			timer.Tick += new EventHandler(FrameStep);
			stopwatch = Stopwatch.StartNew();
		}
		protected Stopwatch Stopwatch { get { return stopwatch; } }
		public AnimationInfoCollection Animations { get { return animations; } }
		public bool CanAnimate(UserLookAndFeel lookAndFeel) {
			if(lookAndFeel == null) return false;
			switch(lookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Skin: return true;
				case ActiveLookAndFeelStyle.WindowsXP: return true;
				case ActiveLookAndFeelStyle.Office2003: return true;
			}
			return false;
		}
		public Bitmap CreateBitmap(ObjectPainter painter, ObjectInfoArgs info) { return CreateBitmap(painter, info, false); }
		public Bitmap CreateBitmap(ObjectPainter painter, ObjectInfoArgs info, bool allowStringDrawing) { return CreateBitmap(new ObjectPaintInfo(painter, info), allowStringDrawing); }
		public Bitmap CreateBitmap(ObjectPainter painter, ObjectInfoArgs info, Rectangle bounds) { return CreateBitmap(painter, info, bounds, false); }		
		public Bitmap CreateBitmap(ObjectPainter painter, ObjectInfoArgs info, Rectangle bounds, bool allowStringDrawing) { return CreateBitmap(new ObjectPaintInfo(painter, info), bounds, allowStringDrawing); }
		public Bitmap CreateBitmap(ObjectPaintInfo foreInfo, Rectangle bounds) { return CreateBitmap(foreInfo, bounds, false); }
		public Bitmap CreateBitmap(ObjectPaintInfo foreInfo, Rectangle bounds, bool allowStringDrawing) { return CreateBitmap(null, foreInfo, bounds, allowStringDrawing); }
		public Bitmap CreateBitmap(ObjectPaintInfo foreInfo) { return CreateBitmap(null, foreInfo, false); }
		public Bitmap CreateBitmap(ObjectPaintInfo foreInfo, bool allowStringDrawing) { return CreateBitmap(null, foreInfo, allowStringDrawing); }
		public Bitmap CreateBitmap(ObjectPaintInfo backInfo, ObjectPaintInfo foreInfo) { return CreateBitmap(backInfo, foreInfo, false); }
		public Bitmap CreateBitmap(ObjectPaintInfo backInfo, ObjectPaintInfo foreInfo, bool allowStringDrawing) {
			if(foreInfo == null) return null;
			return CreateBitmap(backInfo, foreInfo, foreInfo.Bounds, allowStringDrawing);
		}
		public Bitmap CreateBitmap(ObjectPaintInfo backInfo, ObjectPaintInfo foreInfo, Rectangle foreBounds) { return CreateBitmap(backInfo, foreInfo, foreBounds, false); }
#pragma warning disable 0618
		public Bitmap CreateBitmap(ObjectPaintInfo backInfo, ObjectPaintInfo foreInfo, Rectangle foreBounds, bool allowStringDrawing) {
			if(foreInfo == null || foreInfo.IsEmpty || DisableBitmapAnimation) return null;
			Bitmap bmp = new Bitmap(foreBounds.Width, foreBounds.Height);
			using(Graphics gimage = Graphics.FromImage(bmp)) {
				if(!allowStringDrawing) DevExpress.Utils.Paint.XPaint.lockStringDrawing++;
				try {
					BufferedGraphics context = null;
					if(allowStringDrawing) {
						context = BufferedGraphicsManager.Current.Allocate(gimage, new Rectangle(Point.Empty, bmp.Size));
					}
					Graphics g = context == null ? gimage : context.Graphics;
					g.TranslateTransform(-foreBounds.X, -foreBounds.Y);
					using(GraphicsCache cache = new GraphicsCache(g)) {
						if(backInfo != foreInfo && backInfo != null && !backInfo.IsEmpty) backInfo.Draw(cache);
						foreInfo.Draw(cache);
					}
					if(context != null) context.Render();
				}
				finally {
					if(!allowStringDrawing) DevExpress.Utils.Paint.XPaint.lockStringDrawing--;
				}
			}
			return bmp;
		}
#pragma warning restore 0618
		public void AddEditorAnimation(object link, ISupportXtraAnimation obj, IAnimatedItem item, CustomAnimationInvoker method) {
			if(item.AnimationIntervals != null)
				AddEditorAnimation(link, obj, item.AnimationIntervals, item.FramesCount, item.AnimationType, method);
			else if(item.AnimationInterval > 0)
				AddEditorAnimation(link, obj, item.AnimationInterval, item.FramesCount, item.AnimationType, method);
		}
		public void AddEditorAnimation(object link, ISupportXtraAnimation obj, int deltaTick, int frameCount, AnimationType type, CustomAnimationInvoker method) { 
			animations.Remove(obj, link);
			AddCore(new EditorAnimationInfo(link, obj, deltaTick, frameCount, type, method));
		}
		public void AddEditorAnimation(object link, object viewInfo, ISupportXtraAnimation obj, IAnimatedItem item, CustomAnimationInvoker method) {
			if(item.AnimationIntervals != null)
				AddEditorAnimation(link, viewInfo, obj, item.AnimationIntervals, item.FramesCount, item.AnimationType, method);
			else if(item.AnimationInterval > 0)
				AddEditorAnimation(link, viewInfo, obj, item.AnimationInterval, item.FramesCount, item.AnimationType, method);
		}
		public void AddEditorAnimation(object link, object viewInfo, ISupportXtraAnimation obj, int deltaTick, int frameCount, AnimationType type, CustomAnimationInvoker method) {
			animations.Remove(obj, link);
			AddCore(new EditorAnimationInfo(link, viewInfo, obj, deltaTick, frameCount, type, method));
		}
		public void AddEditorAnimation(object link, ISupportXtraAnimation obj, int[] deltaTicks, int frameCount, AnimationType type, CustomAnimationInvoker method) {
			animations.Remove(obj, link);
			AddCore(new EditorAnimationInfo(link, obj, deltaTicks, frameCount, type, method));
		}
		public void AddEditorAnimation(object link, object viewInfo, ISupportXtraAnimation obj, int[] deltaTicks, int frameCount, AnimationType type, CustomAnimationInvoker method) {
			animations.Remove(obj, link);
			AddCore(new EditorAnimationInfo(link, viewInfo, obj, deltaTicks, frameCount, type, method));
		}
		public void AddBoundsAnimation(ISupportXtraAnimation obj, IXtraObjectWithBounds objectWithBounds, object animId, bool growUp, Rectangle beginRect, Rectangle endRect, int length) {
			if(obj == null) return;
			animations.Remove(obj, animId);
			if(beginRect == endRect || length == 0) return;
			AddCore(new BoundsAnimationInfo(obj, objectWithBounds, animId, growUp, beginRect, endRect, length));
		}
		public void AddBitmapAnimation(ISupportXtraAnimation obj, object animId, int length, ObjectPaintInfo backInfo, ObjectPaintInfo foreInfo, BitmapAnimationImageCallback callBack, bool allowStringDrawing) {
			AddBitmapAnimation(obj, animId, length, foreInfo.Bounds, CreateBitmap(backInfo, foreInfo, allowStringDrawing), callBack);
		}
		public void AddBitmapAnimation(ISupportXtraAnimation obj, object animId, int length, ObjectPaintInfo backInfo, ObjectPaintInfo foreInfo, BitmapAnimationImageCallback callBack) {
			AddBitmapAnimation(obj, animId, length, foreInfo.Bounds, CreateBitmap(backInfo, foreInfo), callBack);
		}
		public void AddBitmapAnimation(ISupportXtraAnimation obj, object animId, int length, Rectangle bounds, Bitmap bitmap, BitmapAnimationImageCallback callBack) {
			if(obj == null) return;
			animations.Remove(obj, animId);
			if(bitmap == null || length == 0) return;
			AddCore(new BitmapAnimationInfo(obj, animId, bitmap, callBack, bounds, length));
		}
		public void AddBitmapFadeAnimation(ISupportXtraAnimation obj, object animId, int length, ObjectPaintInfo backInfo, ObjectPaintInfo foreInfo, bool fadeIn) {
			AddBitmapFadeAnimation(obj, animId, length, foreInfo.Bounds, CreateBitmap(backInfo, foreInfo), fadeIn);
		}
		public void AddBitmapFadeAnimation(ISupportXtraAnimation obj, object animId, int length, Rectangle bounds, Bitmap bitmap, bool fadeIn) {
			if(obj == null) return;
			animations.Remove(obj, animId);
			if(bitmap == null || length == 0) return;
			AddCore(new BitmapFadeAnimationInfo(obj, animId, bitmap, bounds, length, fadeIn));
		}
		public void AddFadeAnimation(ISupportXtraAnimation obj, object animId, int length, Rectangle bounds, bool fadeIn) {
			if(obj == null) return;
			animations.Remove(obj, animId);
			if(length == 0) return;
			AddCore(new FadeAnimationInfo(obj, animId, bounds, length, fadeIn));
		}
		public void AddObject(ISupportXtraAnimation obj, object animId, int deltaTick, int frameCount, CustomAnimationInvoker method) {
			animations.Remove(obj, animId);
			AddCore(new CustomAnimationInfo(obj, animId, deltaTick, frameCount, method));
		}
		public void AddAnimation(BaseAnimationInfo info) {
			AddCore(info);
		}
		protected void AddCore(BaseAnimationInfo info) {
			if(!info.AnimatedObject.CanAnimate) return;
			animations.Add(info);
			CheckTimerStart();
		}
		void RemoveObjectCore(ISupportXtraAnimation obj, object animId) {
			animations.Remove(obj, animId);
			CheckTimerStop();
		}
		void RemoveObjectCore(ISupportXtraAnimation obj) {
			animations.Remove(obj);
			CheckTimerStop();
		}
		public int CalcRibbonGroupAnimationLength(ObjectState oldState, ObjectState state) {
			return 400;
		}
		public int CalcRibbonItemAnimationLength(ObjectState oldState, ObjectState state) {
			return CalcBarAnimationLength(oldState, state);
		}
		public int CalcAnimationLength(ObjectState oldState, ObjectState state) {
			oldState &= (~ObjectState.Selected);
			state &= (~ObjectState.Selected);
			if(IsPressed(oldState) || IsPressed(state)) return 100;
			if(oldState == ObjectState.Normal && IsHot(state)) return 100;
			if(IsHot(oldState) && state == ObjectState.Normal) return 200;
			return 0;
		}
		public int CalcBarAnimationLength(ObjectState oldState, ObjectState state) {
			oldState &= (~ObjectState.Selected);
			state &= (~ObjectState.Selected);
			if(IsPressed(oldState) || IsPressed(state)) return 500;
			if(oldState == ObjectState.Normal && IsHot(state)) return 100 + 100;
			if(IsHot(oldState) && state == ObjectState.Normal) return 100 + 200;
			return 0;
		}
		bool IsHot(ObjectState state) { return (state & ObjectState.Hot) != 0; }
		bool IsPressed(ObjectState state) { return (state & ObjectState.Pressed) != 0; }
		void CheckTimerStart() {
			if((timer.Interval != 1 || !timer.Enabled) && animations.Count > 0) { 
				timer.Interval = 1;
				timer.Start();
			}
		}
		void CheckTimerStop() {
			if((timer.Interval == 1 || timer.Enabled) && animations.Count == 0) {
				timer.Stop();
				timer.Interval = int.MaxValue;
			}
		}
		bool lockTimer = false;
		internal const string SRObsoleteDisableBitmapAnimation = "Do not modify the DisableBitmapAnimation property manually. This property is not intended to be used in User code.";
		bool disableBitmapAnimationCore = false;
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(XtraAnimator.SRObsoleteDisableBitmapAnimation)]
		public bool DisableBitmapAnimation {
			get { return disableBitmapAnimationCore; }
			set {
				if(disableBitmapAnimationCore != value)
					disableBitmapAnimationCore = value;
			}
		}
		void CallListener(BaseAnimationInfo info) {
			IXtraAnimationListener listener = info.AnimatedObject as IXtraAnimationListener;
			if(listener != null)
				listener.OnAnimation(info);
		}
		void CallListenerEndAnimation(BaseAnimationInfo info) {
			IXtraAnimationListener listener = info.AnimatedObject as IXtraAnimationListener;
			if(listener != null)
				listener.OnEndAnimation(info);
		}
		public void FrameStep(object sender, EventArgs e) {
			if(lockTimer) return;
			this.lockTimer = true;
			try {
				timer.Interval = int.MaxValue;
				long ticks = stopwatch.Elapsed.Ticks;
				for(int animIndex = 0; animIndex < animations.Count; animIndex++) {
					BaseAnimationInfo ani = animations[animIndex];
					if(ani.BeginTick == -1) {
						ani.BeginTick = ticks;
						ani.CurrentTick = ticks;
						ani.FrameStep();
						CallListener(ani);
						continue;
					}
					ani.CurrentTick = ticks;
					if(ani.PrevFrame != ani.CurrentFrame) {
						ani.FrameStep();
						CallListener(ani);
					}
					if(ani.CurrentFrame >= ani.FrameCount && ani.AnimationType != AnimationType.Cycle) {
						CallListenerEndAnimation(ani);
						animations.Remove(ani);
					}
				}
				CheckTimerStart();
			}
			finally {
				this.lockTimer = false;
			}
		}
		public BaseAnimationInfo Get(ISupportXtraAnimation obj, object animationId) {
			return animations[obj, animationId];
		}
		public bool DrawFrame(GraphicsCache cache, ISupportXtraAnimation obj, object animationId) {
			return DrawFrame(cache, animations[obj, animationId]);
		}
		public bool DrawFrame(GraphicsCache cache, BaseAnimationInfo info) {
			return DrawFrameCore(cache, info, false);
		}
		public bool DrawPostFrame(GraphicsCache cache, ISupportXtraAnimation obj, object animationId) {
			return DrawPostFrame(cache, animations[obj, animationId]);
		}
		public bool DrawPostFrame(GraphicsCache cache, BaseAnimationInfo info) {
			return DrawFrameCore(cache, info, true);
		}
		protected bool DrawFrameCore(GraphicsCache cache, BaseAnimationInfo info, bool postDraw) {
			BitmapAnimationInfo binfo = info as BitmapAnimationInfo;
			if(binfo != null) {
				if(binfo.IsPostAnimation != postDraw) return false;
				return binfo.Draw(cache);
			}
			return false;
		}
		public bool IsRequireDrawPostFrame(ISupportXtraAnimation obj, object animationId) {
			return IsRequireDrawPostFrame(animations[obj, animationId]);
		}
		public bool IsRequireDrawPostFrame(BaseAnimationInfo info) {
			BitmapAnimationInfo binfo = info as BitmapAnimationInfo;
			return binfo != null && binfo.IsPostAnimation;
		}
		public void LockDrawString() {
			DevExpress.Utils.Paint.XPaint.lockStringDrawing++;
		}
		public void UnlockDrawString() {
			DevExpress.Utils.Paint.XPaint.lockStringDrawing--;
		}
		public void DrawAnimationHelper(GraphicsCache cache, ISupportXtraAnimation obj, object animationId, ObjectPainter painter, ObjectInfoArgs info,
											DrawTextInvoker drawTextMethod, object textInfo) {
			DrawAnimationHelper(cache, obj, animationId, painter, info, new ObjectTextSimplePainter(), new ObjectTextSimpleInfoArgs(drawTextMethod, textInfo));
		}
		public void DrawAnimationHelper(GraphicsCache cache, ISupportXtraAnimation obj, object animationId, ObjectPainter painter, ObjectInfoArgs info, 
											ObjectPainter textPainter, ObjectInfoArgs textInfo) {
			if(!cache.IsNeedDrawRect(info.Bounds)) return;
			if(DrawFrame(cache, obj, animationId)) {
				ObjectPainter.DrawObject(cache, textPainter, textInfo);
				return;
			}
			bool lockDrawString = IsRequireDrawPostFrame(obj, animationId);
			try {
				if(lockDrawString) LockDrawString();
				ObjectPainter.DrawObject(cache, painter, info);
			}
			finally {
				if(lockDrawString) UnlockDrawString();
			}
			if(DrawPostFrame(cache, obj, animationId)) {
				ObjectPainter.DrawObject(cache, textPainter, textInfo);
			}
		}
		public static int GetImageFrameCount(Image img) {
			const int FrameDelayProperty = 0x5100;
			int i;
			for(i = 0; i < img.PropertyIdList.Length; i++)
				if(img.PropertyIdList[i] == FrameDelayProperty) break;
			if(i == img.PropertyIdList.Length) return 0;
			if(img.FrameDimensionsList == null || img.FrameDimensionsList.Length == 0 || img.FrameDimensionsList[0] != FrameDimension.Time.Guid)
				return 0;
			return img.GetFrameCount(FrameDimension.Time);
		}
		public static int[] GetImageFrameDelay(Image img) {
			if(img == null) return null;
			const int FrameDelayProperty = 0x5100;
			int frameCount = GetImageFrameCount(img);
			if(frameCount == 0) return null;
			PropertyItem property = img.GetPropertyItem(FrameDelayProperty);
			if(property == null) return null;
			int[] frameDelay = new int[frameCount];
			byte[] array = property.Value;
			for(int i = 0; i < frameCount; i++) {
				frameDelay[i] = ((int)array[i * 4]) + (((int)array[i * 4 + 1]) << 8) + (((int)array[i * 4 + 2]) << 16) + (((int)array[i * 4 + 3]) << 24);
				if(frameDelay[i] == 0 && i > 0) frameDelay[i] = 10;
			}
			return frameDelay;
		}
	}
	public class ObjectTextSimplePainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			ObjectTextSimpleInfoArgs ee = (ObjectTextSimpleInfoArgs)e;
			if(ee.DrawTextMethod  != null) ee.DrawTextMethod(e.Cache, ee.TextInfo);
		}
	}
	public class ObjectTextSimpleInfoArgs : ObjectInfoArgs {
		DrawTextInvoker drawTextMethod;
		object textInfo;
		public ObjectTextSimpleInfoArgs(DrawTextInvoker drawTextMethod, object textInfo) {
			this.drawTextMethod = drawTextMethod;
			this.textInfo = textInfo;
		}
		public DrawTextInvoker DrawTextMethod { get { return drawTextMethod; } }
		public object TextInfo { get { return textInfo; } }
	}
	public class AnimatedImageHelper {
		Image image;
		public AnimatedImageHelper(Image img) {
			this.image = img;
		}
		public Image Image { 
			get { return image; } 
			set { 
				image = value;
				OnImageChanged();
			} 
		}
		private void OnImageChanged() {
			this.framesCountCore = -1;
		}
		public int AnimationInterval {
			get {
				int[] animInt = AnimationIntervals;
				if(animInt == null) return 0;
				return animInt[0];	
			}
		}
		public int[] AnimationIntervals {
			get {
				if(Image == null) return null;
				int[] frameDelay = XtraAnimator.GetImageFrameDelay(Image);
				if(frameDelay == null) return null;
				for(int i = 0; i < frameDelay.Length; i++)
					frameDelay[i] *= 100000;
				return frameDelay;
			}
		}
		public AnimationType AnimationType {
			get { return AnimationType.Cycle; }
		}
		public int FramesCount {
			get {
				int res = FramesCountCore;
				if(res > 1)
					res++;
				return res;
			}
		}
		int framesCountCore = -1;
		internal int FramesCountCore {
			get {
				if(Image == null) return 0;
				if(framesCountCore > -1)
					return framesCountCore;
				for(int i = 0; i < Image.FrameDimensionsList.Length; i++) {
					if(Image.FrameDimensionsList[0] == FrameDimension.Time.Guid) {
						framesCountCore = XtraAnimator.GetImageFrameCount(Image);
						break;
					}
				}
				if(framesCountCore == -1)
					framesCountCore = 0;
				return framesCountCore;
			}
		}
		public int GetAnimationInterval(int frameIndex) {
			int[] animInt = AnimationIntervals;
			if(animInt == null || animInt.Length <= frameIndex) return 0;
			return animInt[frameIndex];
		}
		public bool IsAnimated {
			get { return FramesCount > 1; }
		}
		public void UpdateAnimation(BaseAnimationInfo info) {
			if(info.IsFinalFrame) return;
			if(Image != null && info.CurrentFrame < FramesCountCore) {
				Image.SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
			}
		}
	}
	public struct PointD {
		public double X { get; set; }
		public double Y { get; set; }
	}
	public struct SplineNode {
		public static SplineNode Empty;
		public PointD P1;
		public PointD P2;
		public PointD P3;
		public PointD P4;
		public double A;
		public double B;
		public double C;
		public void CalcParams() {
			A = (-P1.Y + 3.0f * (P3.Y - P4.Y) + P2.Y);
			B = (-6.0f * P3.Y + 3.0f * (P1.Y + P4.Y));
			C = 3.0f * (P3.Y - P1.Y);  
		}
		public double Calc(double t) {
			t = (t - P1.X) / (P2.X - P1.X);
			double t2 = t * t;
			double t3 = t2 * t;
			return t3 * A + t2 * B + t * C + P1.Y;
		}
	}
	public interface ISupportAnimatedScroll {
		void OnScroll(double currentScrollValue);
		void OnScrollFinish();
	}
	public class SplineAnimationHelper {
		SplineNode node;
		SplineNode node2;
		public SplineAnimationHelper() {
			this.node = SplineNode.Empty;
			this.node2 = SplineNode.Empty;
		}
		public void Init(double start, double end, double time) {
			double middleX = time * 0.3f;
			double middleY = start + (end - start) * 0.87f;
			node.P1.X = 0;
			node.P1.Y = start;
			node.P2.X = middleX;
			node.P2.Y = middleY;
			node2.P1.X = middleX;
			node2.P1.Y = middleY;
			node2.P2.X = time;
			node2.P2.Y = end;
			double k = (end - start) / time;
			node.P3.X = node.P1.X + (node.P2.X - node.P1.X) * 0.2f;
			node.P3.Y = node.P1.Y;
			node.P4.X = node.P2.X - (node.P2.X - node.P1.X) * 0.2f;
			node.P4.Y = node.P2.Y - (node.P2.X - node.P1.X) * 0.2f * k;
			node2.P3.X = node2.P1.X + (node2.P2.X - node2.P1.X) * 0.2f;
			node2.P3.Y = node2.P1.Y + (node2.P2.X - node2.P1.X) * 0.2f * k;
			node2.P4.X = node2.P2.X - (node2.P2.X - node2.P1.X) * 0.2f;
			node2.P4.Y = node2.P2.Y;
			node.CalcParams();
			node2.CalcParams();
		}
		public double CalcSpline(double time) {
			if(time < node.P2.X)
				return node.Calc(time);
			return node2.Calc(time);
		}
	}
	public class AnimatedScrollHelper {
		SplineNode node;
		SplineNode node2;
		Timer timer;
		ISupportAnimatedScroll owner;
		long startTime;
		double currentTime;
		bool smooth;
		bool animating = false;
		public AnimatedScrollHelper(ISupportAnimatedScroll owner) {
			this.owner = owner;
			node = SplineNode.Empty;
			node2 = SplineNode.Empty;
			this.timer = new Timer();
			InitializeTimer();
		}
		protected virtual void InitializeTimer() {
			Timer.Interval = 1;
			Timer.Tick += new EventHandler(OnTimerTick);
		}
		public void Cancel() {
			if(!animating) return;
			this.animating = false;
			Timer.Stop();
			owner.OnScroll(node2.P2.Y);
			owner.OnScrollFinish();
		}
		public bool Animating { get { return animating; } }
		protected virtual void OnTimerTick(object sender, EventArgs e) {
			this.currentTime = (DateTime.Now.Ticks - this.startTime) * 0.0001f * 0.001f;
			this.currentTime = Math.Min(this.currentTime, node2.P2.X);
			double currValue = Calc(CurrentTime);
			owner.OnScroll(currValue);
			if(CurrentTime == node2.P2.X) {
				Timer.Stop();
				this.animating = false;
				owner.OnScrollFinish();
			}
			return;
		}
		protected SplineNode Node { get { return node; } }
		protected Timer Timer { get { return timer; } }
		public double CurrentTime { get { return currentTime; } }
		public bool IsSmoothScroll { get { return smooth; } }
		void Init(float start, float end, float time, bool bSmooth) {
			this.smooth = bSmooth;
			float middleX = time * 0.3f;
			float middleY = start + (end - start) * 0.87f;
			node.P1.X = 0;
			node.P1.Y = start;
			node.P2.X = middleX;
			node.P2.Y = middleY;
			node2.P1.X = middleX;
			node2.P1.Y = middleY;
			node2.P2.X = time;
			node2.P2.Y = end;
			float k = (end - start) / time;
			node.P3.X = node.P1.X + (node.P2.X - node.P1.X) * 0.2f;
			node.P3.Y = node.P1.Y;
			node.P4.X = node.P2.X - (node.P2.X - node.P1.X) * 0.2f;
			node.P4.Y = node.P2.Y - (node.P2.X - node.P1.X) * 0.2f * k;
			node2.P3.X = node2.P1.X + (node2.P2.X - node2.P1.X) * 0.2f;
			node2.P3.Y = node2.P1.Y + (node2.P2.X - node2.P1.X) * 0.2f * k;
			node2.P4.X = node2.P2.X - (node2.P2.X - node2.P1.X) * 0.2f;
			node2.P4.Y = node2.P2.Y;
			node.CalcParams();
			node2.CalcParams();
		}
		public void Scroll(float from, float to, float time, bool bSmooth) {
			Timer.Stop();
			Init(from, to, time, bSmooth);
			this.startTime = DateTime.Now.Ticks;
			Timer.Start();
			animating = true;
		}
		double Calc(double time) {
			if(!IsSmoothScroll)
				return CalcLine(time);
			return CalcSpline(time);   
		}
		double CalcLine(double time) {
			return node.P1.Y + (node2.P2.Y - node.P1.Y) / (node2.P2.X - node.P1.X) * time;
		}
		double CalcSpline(double time) { 
			if(time < node.P2.X)
				return node.Calc(time);
			return node2.Calc(time);
		}
	}
	public interface ISupportXtraAnimationEx : ISupportXtraAnimation {
		void OnFrameStep(BaseAnimationInfo info);
		void OnEndAnimation(BaseAnimationInfo info);
	}
	public class DoubleSplineAnimationInfo : BaseAnimationInfo {
		public DoubleSplineAnimationInfo(ISupportXtraAnimation owner, object animationId, double start, double end, int animLength)
			: base(owner, animationId, (int)(TimeSpan.TicksPerMillisecond * animLength / 300), 300, AnimationType.Simple) {
			Start = start;
			End = end;
			ValueHelper = new SplineAnimationHelper();
			ValueHelper.Init((float)Start, (float)End, 1.0);
		}
		protected ISupportXtraAnimationEx AnimatedObjectEx { get { return AnimatedObject as ISupportXtraAnimationEx; } }
		public double Start { get; private set; }
		public double End { get; private set; }
		public double Value { get; private set; }
		SplineAnimationHelper ValueHelper { get; set; }
		public override void FrameStep() {
			double k = ((float)(CurrentFrame)) / FrameCount;
			if(IsFinalFrame) k = 1.0;
			Value = ValueHelper.CalcSpline(k);
			if(AnimatedObjectEx != null)
				AnimatedObjectEx.OnFrameStep(this);
			if(IsFinalFrame) {
				AnimatedObjectEx.OnEndAnimation(this);
			}
		}
	}
}
