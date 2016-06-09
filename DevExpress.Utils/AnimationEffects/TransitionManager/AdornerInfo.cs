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
using System.Windows.Forms;
using DevExpress.Data.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Internal;
namespace DevExpress.Utils.Animation {
	public delegate void CustomTransitionEventHandler(ITransition transition, CustomTransitionEventArgs e);
	public delegate void BeforeTransitionStartsEventHandler(ITransition transition, System.ComponentModel.CancelEventArgs e);
	public delegate void AfterTransitionEndsEventHandler(ITransition transition, EventArgs e);
	public class CustomTransitionEventArgs {
		public CustomTransitionEventArgs() { }
		public ITransitionAnimator TransitionType { get; set; }
		public Image ImageStart { get; set; }
		public Image ImageEnd { get; set; }
		public IEasingFunction EasingFunction { get; set; }
		public IEnumerable<Rectangle> Regions { get; set; }
	}
	class TransitionInfoArgs : BaseAsyncAdornerElementInfoArgs, IWaitingIndicatorInfoArgs {
		ICustomTransition customTransitionCore;
		ITransitionAnimator transitionAnimatorCore;
		IWaitingIndicatorInfo waitingIndicatorInfoCore;
		IntPtr adornerHandle;
		AsyncSkinInfo skinInfoCore;
		public TransitionInfoArgs(ICustomTransition animation) {
			InitOptions(animation);			
		}
		public static BaseAsyncAdornerElementInfo EnsureAsyncAdornerElementInfo(ICustomTransition customTransition) {
			TransitionInfoArgs args = new TransitionInfoArgs(customTransition);			
			return new BaseAsyncAdornerElementInfo(new TransitionOpaquePainter(), args);
		}
		void InitOptions(ICustomTransition customTransition) {
			this.customTransitionCore = customTransition;
			InitSkinInfo();
			Bounds = new Rectangle(Point.Empty, CustomTransition.Control.ClientSize);
			SetDirty();
		}
		protected override IEnumerable<Rectangle> GetRegionsCore(bool opaque) {
			return CustomTransition.Regions ?? base.GetRegionsCore(opaque);
		}
		protected override ObjectPainter GetPainter() {
			if(transitionAnimatorCore == null)
				return new WaitingTransitionPainter();
			return new TransitionPainter();
		}
		protected override void BeginAsync(IntPtr adornerHandle) {
			this.adornerHandle = adornerHandle;
			if(CustomTransition.ShowWaitingIndicator == DefaultBoolean.False) return;
			waitingIndicatorInfoCore = CreateWaitingIndicatorInfo();
			WaitingInfo.WaitingAnimator.Invalidate += AnimatorInvalidate;
		}
		protected virtual IWaitingIndicatorInfo CreateWaitingIndicatorInfo() {
			if(CustomTransition.WaitingAnimatorType == WaitingAnimatorType.Ring)
				return new RingAnimatorInfo(CustomTransition.RingWaitingIndicatorProperties as RingWaitingIndicatorProperties, SkinInfo, (customTransitionCore as CustomTransition).IsAutonome);
			if(CustomTransition.WaitingAnimatorType == WaitingAnimatorType.Line)
				return new LineAnimatorInfo(CustomTransition.LineWaitingIndicatorProperties as LineWaitingIndicatorProperties, SkinInfo, (customTransitionCore as CustomTransition).IsAutonome);
			return new WaitingIndicatorInfo(CustomTransition.WaitingIndicatorProperties, SkinInfo);
		}
		void DestroyWaitingIndicatorInfo() {
			if(WaitingInfo == null) return;
			WaitingInfo.WaitingAnimator.Invalidate -= AnimatorInvalidate;
			Ref.Dispose(ref waitingIndicatorInfoCore);
			waitingIndicatorInfoCore = null;
		}
		void AnimatorInvalidate(object sender, EventArgs e) {
			LayeredWindowMessanger.PostInvalidate(adornerHandle);
		}
		protected internal void UpdateAsync() {
			DestroyWaitingIndicatorInfo();
			if(this.CustomTransition.TransitionType != null) {
				transitionAnimatorCore = CreateTransitionAnimator();
				TransitionAnimator.Invalidate += AnimatorInvalidate;
				TransitionAnimator.Complete += TransitionAnimatorComplete;
			}
		}
		protected virtual ITransitionAnimator CreateTransitionAnimator() {
			return customTransitionCore.GetCustomTransitionAnimator();
		}
		protected virtual void DestroyTransitionAnimator() {
			if(TransitionAnimator == null) return;
			TransitionAnimator.Complete -= TransitionAnimatorComplete;
			TransitionAnimator.Invalidate -= AnimatorInvalidate;
			transitionAnimatorCore.StopAnimation();
			transitionAnimatorCore = null;
		}
		protected override void EndAsync() {
			DestroyCore();
		}
		void TransitionAnimatorComplete(object sender, EventArgs e) {
			LayeredWindowMessanger.PostClose(adornerHandle);
		}
		protected override void Destroy() {
			LayeredWindowMessanger.PostClose(adornerHandle);
			DestroyCore();
		}
		void DestroyCore() {
			DestroyWaitingIndicatorInfo();
			DestroyTransitionAnimator();
			this.adornerHandle = IntPtr.Zero;
			Ref.Dispose(ref skinInfoCore);
			Ref.Dispose(ref transitionAnimatorCore);
			Ref.Dispose(ref customTransitionCore);
		}
		void InitSkinInfo() {
			ISupportLookAndFeel lookAndFeel = FindLookAndFeel(CustomTransition.Control);
			if(lookAndFeel != null)
				skinInfoCore = new AsyncSkinInfo(lookAndFeel.LookAndFeel);
		}
		ISupportLookAndFeel FindLookAndFeel(Control control) {
			if(control == null) return null;
			ISupportLookAndFeel provider = control as ISupportLookAndFeel;
			if(provider == null) return FindLookAndFeel(control.Parent);
			return provider;
		}
		public override AsyncSkinInfo SkinInfo { get { return skinInfoCore; } }
		protected ICustomTransition CustomTransition { get { return customTransitionCore; } }
		public ITransitionAnimator TransitionAnimator { get { return transitionAnimatorCore; } }
		public IWaitingIndicatorInfo WaitingInfo { get { return waitingIndicatorInfoCore; } }
		internal Image ImageStart { get { return CustomTransition.ImageStart; } }
	}
	class WaitingIndicatorInfo : BaseWaitingIndicatorInfo {
		WaitingIndicatorPainter painter;
		ILoadingAnimator waitingAnimatorCore;
		public WaitingIndicatorInfo(IWaitingIndicatorProperties properties, AsyncSkinInfo skinInfo)
			: base(properties) {
			painter = CreatePainter(skinInfo);
			waitingAnimatorCore = CreateLoadingAnimator(skinInfo);
		}
		public override WaitingIndicatorPainter Painter { get { return painter; } }
		WaitingIndicatorPainter CreatePainter(AsyncSkinInfo info) {
			if(info != null) return new WaitingIndicatorSkinPainter(info.LookAndFeel);
			return new WaitingIndicatorPainter();
		}
		ILoadingAnimator CreateLoadingAnimator(AsyncSkinInfo info) {
			DevExpress.Skins.ISkinProvider provider = info == null ? null : info.LookAndFeel;
			return new LoadingAnimator(LoadingImages.GetImage(provider, true), true);
		}
		public override ILoadingAnimator WaitingAnimator { get { return waitingAnimatorCore; } }
		protected override void OnDispose() {
			Ref.Dispose(ref waitingAnimatorCore);
			base.OnDispose();
			painter = null;
		}
		public override string Caption { get { return Properties.Caption; } }
		public override string Description { get { return Properties.Description; } }
	}
}
