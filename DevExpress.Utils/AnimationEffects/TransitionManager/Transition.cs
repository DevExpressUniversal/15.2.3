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
using System.Windows.Forms;
using System.Collections;
using DevExpress.Data.Utils;
using System.Drawing;
using DevExpress.Utils.Serializing;
using System.Linq;
using DevExpress.Utils.Design;
namespace DevExpress.Utils.Animation {
	interface ICustomTransition : ITransition {
		IEnumerable<Rectangle> Regions { get; set; }
		void Combine(ITransition transition, CustomTransitionEventArgs args, TransitionManagerProperties rootProperties);
		void Combine(Control control, IWaitingIndicatorProperties waitingIndicatorProperties);
		ITransitionAnimator GetCustomTransitionAnimator();
		Image ImageStart { get; }
	}
	public interface ITransition : IDisposable {
		DefaultBoolean ShowWaitingIndicator { get; set; }
		ITransitionAnimator TransitionType { get; set; }
		Control Control { get; set; }
		EasingMode EasingMode { get; set; }
		WaitingAnimatorType WaitingAnimatorType { get; set; }
		IWaitingIndicatorProperties WaitingIndicatorProperties { get; }
		ILineWaitingIndicatorProperties LineWaitingIndicatorProperties { get; }
		IRingWaitingIndicatorProperties RingWaitingIndicatorProperties { get; }
	}
	public enum WaitingAnimatorType { Default, Line, Ring }
	public class Transition : ITransition {
		ITransitionAnimator transitionType;
		IWaitingIndicatorProperties waitingIndicatorPropertiesCore;
		ILineWaitingIndicatorProperties lineWaitingIndicatorPropertiesCore;
		IRingWaitingIndicatorProperties ringWaitingIndicatorPropertiesCore;
		DefaultBoolean showWaitingIndicatorCore;
		WaitingAnimatorType waitingAnimatorTypeCore;
		public Transition() {
			transitionType = new PushTransition();
			showWaitingIndicatorCore = DefaultBoolean.Default;
			waitingAnimatorTypeCore = Animation.WaitingAnimatorType.Default;
		}
		[ DefaultValue(Transitions.Push), Category("Behavior"), TypeConverter(typeof(TransitionAnimatorTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public ITransitionAnimator TransitionType {
			get { return transitionType; }
			set {
				if(transitionType == value) return;				
				transitionType = value;
			}
		}
		[ Category("Behavior"), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowWaitingIndicator {
			get { return showWaitingIndicatorCore; }
			set { showWaitingIndicatorCore = value; }
		}
		[ Category("Behavior")]
		public Control Control { get; set; }
		[ Category("Behavior"), DefaultValue(EasingMode.EaseIn)]
		public EasingMode EasingMode { get; set; }
		[ Category("Behavior"), DefaultValue(WaitingAnimatorType.Default)]
		public WaitingAnimatorType WaitingAnimatorType {
			get { return waitingAnimatorTypeCore; }
			set { waitingAnimatorTypeCore = value; }
		}
		[  Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IWaitingIndicatorProperties WaitingIndicatorProperties {
			get { 
				if(waitingIndicatorPropertiesCore == null)
					waitingIndicatorPropertiesCore = new WaitingIndicatorProperties();
				return waitingIndicatorPropertiesCore; }
		}
		[ Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IRingWaitingIndicatorProperties RingWaitingIndicatorProperties {
			get {
				if(ringWaitingIndicatorPropertiesCore == null)
					ringWaitingIndicatorPropertiesCore = new RingWaitingIndicatorProperties();
				return ringWaitingIndicatorPropertiesCore;
			}
		}
		[ Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ILineWaitingIndicatorProperties LineWaitingIndicatorProperties {
			get {
				if(lineWaitingIndicatorPropertiesCore == null)
					lineWaitingIndicatorPropertiesCore = new LineWaitingIndicatorProperties();
				return lineWaitingIndicatorPropertiesCore;
			}
		}
		bool isDisposing;
		void IDisposable.Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDispose();
			}
		}
		protected virtual void OnDispose() {
			Ref.Dispose(ref transitionType);
			Ref.Dispose(ref waitingIndicatorPropertiesCore);
		}
	}
	public class TransitionCollection : CollectionBase, ICollection<ITransition>, IDisposable {
		public void Add(ITransition item) {
			List.Add(item);
		}
		public bool Contains(ITransition item) {
			return List.Contains(item);
		}
		public void CopyTo(ITransition[] array, int arrayIndex) {
			List.CopyTo(array, arrayIndex);
		}
		public bool IsReadOnly { get { return List.IsReadOnly; } }
		public bool Remove(ITransition item) {
			if(!Contains(item)) return false;
			List.Remove(item);
			return true;
		}
		protected override void OnRemove(int index, object value) {
			base.OnRemove(index, value);
			ITransition element = value as ITransition;
			Ref.Dispose(ref element);
		}
		public new IEnumerator<ITransition> GetEnumerator() {
			foreach(ITransition item in List)
				yield return item;
		}
		public ITransition this[Control control] {
			get {
				if(List == null) return null;
				foreach(ITransition item in List)
					if(item.Control == control) return item;
				return null;
			}
		}
		#region IDisposable Members
		bool isDisposing;
		void IDisposable.Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				for(int i = 0; i < List.Count; i++) {
					ITransition element = List[i] as ITransition;
					Ref.Dispose(ref element);
				}
				List.Clear();
			}
		}
		#endregion
	}
	class TransitionController : IDisposable {
		ITransitionAdornerBootStraper adorner;
		AfterTransitionEndsEventHandler afterTransitionEndsCore;
		BeforeTransitionStartsEventHandler beforeTransitionStartsCore;
		Action AsyncRaiseTransitionEnding;
		public TransitionController() { }
		ITransitionAdornerBootStraper ShowAsyncAdornerElementInfoArgs(ITransition animation, CustomTransitionEventArgs customArgs, TransitionManagerProperties rootProperties) {
			ICustomTransition customTransition = CustomTransition.Create(animation, customArgs, rootProperties);
			BaseAsyncAdornerElementInfo transitionAdornerInfo = TransitionInfoArgs.EnsureAsyncAdornerElementInfo(customTransition);
			IntPtr hWnd = animation.Control.Handle;
			AsyncRaiseTransitionEnding = new Action(delegate { RaiseAfterTransitionEnds(animation); });
			return TransitionAdornerBootStrapper.Show(hWnd, transitionAdornerInfo);
		}
		ITransitionAdornerBootStraper ShowAsyncAdornerElementInfoArgs(Control control, IWaitingIndicatorProperties waitingIndicatorProperties) {
			ICustomTransition customTransition = CustomTransition.Create(control, waitingIndicatorProperties);
			BaseAsyncAdornerElementInfo transitionAdornerInfo = TransitionInfoArgs.EnsureAsyncAdornerElementInfo(customTransition);
			IntPtr hWnd = control.Handle;
			return TransitionAdornerBootStrapper.Show(hWnd, transitionAdornerInfo);
		}
		public bool IsTransition { get { return adorner != null; } }
		int lockTransition = 0;
		public void StartTransition(ITransition animation, Func<ITransition, CustomTransitionEventArgs> raiseCustomTransition, TransitionManagerProperties rootProperties) {
			if(animation == null || animation.Control == null || !animation.Control.IsHandleCreated || IsTransition || RaiseBeforeTransitionStarts(animation)) return;
			lockTransition++;
			adorner = ShowAsyncAdornerElementInfoArgs(animation, raiseCustomTransition(animation), rootProperties);
			adorner.Disposed += adorner_Disposed;
		}
		public void StartWaitingIndicator(Control control, IWaitingIndicatorProperties waitingIndicatorProperties) {
			lockTransition++;
			adorner = ShowAsyncAdornerElementInfoArgs(control, waitingIndicatorProperties);
		}
		void adorner_Disposed(object sender, EventArgs e) {
			if(IsTransition && AsyncRaiseTransitionEnding != null)
				AsyncRaiseTransitionEnding();
			AsyncRaiseTransitionEnding = null;
			if(adorner != null)
				adorner.Disposed -= adorner_Disposed;
			else {
				var senderAsAdorner = sender as ITransitionAdornerBootStraper;
				if(senderAsAdorner != null)
					senderAsAdorner.Disposed -= adorner_Disposed;
			}
			adorner = null;
		}
		public void EndTransition() {
			if(!IsTransition) return;
			if(lockTransition == 0) return;
			lockTransition--;
			adorner.Invalidate();
		}
		public void StopWaitingIndicator() {
			if(lockTransition == 0) return;
			lockTransition--;
			adorner.Dispose();
			adorner = null;
		}
		protected virtual bool RaiseBeforeTransitionStarts(ITransition transition) {
			CancelEventArgs args = new CancelEventArgs();
			if(beforeTransitionStartsCore != null)
				beforeTransitionStartsCore(transition, args);		  
			return args.Cancel;
		}
		protected internal virtual void RaiseAfterTransitionEnds(ITransition transition) {
			if(afterTransitionEndsCore != null)
				afterTransitionEndsCore(transition, System.EventArgs.Empty);
		}	   
		public event BeforeTransitionStartsEventHandler BeforeTransitionStarts {
			add { beforeTransitionStartsCore += value; }
			remove { beforeTransitionStartsCore -= value; }
		}		
		public event AfterTransitionEndsEventHandler AfterTransitionEnds {
			add { afterTransitionEndsCore += value; }
			remove { afterTransitionEndsCore -= value; }
		}
		#region IDisposable Members
		bool isDisposing;
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				Ref.Dispose(ref adorner);
			}
		}
		#endregion
	}
	class CustomTransition : ICustomTransition {
		IWaitingIndicatorProperties waitingIndicatorProperties;
		ILineWaitingIndicatorProperties lineWaitingIndicatorProperties;
		IRingWaitingIndicatorProperties ringWaitingIndicatorProperties;
		Image imageStart, imageEnd;
		IAnimationParameters parametrs;
		ITransitionAnimator transitionTypeCore;
		CustomTransition() { }
		public static ICustomTransition Create(ITransition transition, CustomTransitionEventArgs args, TransitionManagerProperties rootProperties) {
			ICustomTransition customTransition = new CustomTransition();
			customTransition.Combine(transition, args, rootProperties);
			return customTransition;
		}
		bool isAutonomeCore = false;
		public static ICustomTransition Create(Control control, IWaitingIndicatorProperties waitingIndicatorProperties) {
			ICustomTransition customTransition = new CustomTransition();
			customTransition.Combine(control, waitingIndicatorProperties);
			return customTransition;
		}
		Bitmap GetControlImage(Control control) {
			return ScreenCaptureHelper.GetRecursiveImageFromControl(control) as Bitmap;
		}
		internal bool IsAutonome { get { return isAutonomeCore; } }
		void ICustomTransition.Combine(Control control, IWaitingIndicatorProperties waitingIndicatorProperties) {
			isAutonomeCore = true;
			this.Control = control;
			if(Control == null) return;
			this.ShowWaitingIndicator = DefaultBoolean.True;
			if(waitingIndicatorProperties is WaitingIndicatorProperties) {
				this.WaitingAnimatorType = Animation.WaitingAnimatorType.Default;
				this.waitingIndicatorProperties = waitingIndicatorProperties;
			}
			if(waitingIndicatorProperties is LineWaitingIndicatorProperties) {
				this.WaitingAnimatorType = Animation.WaitingAnimatorType.Line;
				this.lineWaitingIndicatorProperties = waitingIndicatorProperties as LineWaitingIndicatorProperties;
			}
			if(waitingIndicatorProperties is RingWaitingIndicatorProperties) {
				this.WaitingAnimatorType = Animation.WaitingAnimatorType.Ring;
				this.ringWaitingIndicatorProperties = waitingIndicatorProperties as RingWaitingIndicatorProperties;
			}
		}
		void ICustomTransition.Combine(ITransition transition, CustomTransitionEventArgs args, TransitionManagerProperties rootProperties) {
			this.Control = transition.Control;
			if(Control == null) return;
			this.transitionTypeCore = (ITransitionAnimator)(args.TransitionType ?? transition.TransitionType).Clone();
			this.transitionTypeCore.EasingFunction = args.EasingFunction;
			this.transitionTypeCore.EasingMode = transition.EasingMode;
			this.Regions = args.Regions;
			this.imageStart = args.ImageStart ?? GetControlImage(Control);			
			this.imageEnd = args.ImageEnd;
			this.WaitingAnimatorType = transition.WaitingAnimatorType;
			this.waitingIndicatorProperties = transition.WaitingIndicatorProperties;
			this.lineWaitingIndicatorProperties = transition.LineWaitingIndicatorProperties;
			this.ringWaitingIndicatorProperties = transition.RingWaitingIndicatorProperties;
			this.ShowWaitingIndicator = transition.ShowWaitingIndicator;
			parametrs = new AnimationParameters(rootProperties.FrameInterval, rootProperties.FrameCount);
			if(this.ShowWaitingIndicator == DefaultBoolean.Default)
				this.ShowWaitingIndicator = rootProperties.ShowWaitingIndicator ? DefaultBoolean.True : DefaultBoolean.False;
		}
		#region ICustomTransition Members
		EasingMode ITransition.EasingMode { get; set; }
		ITransitionAnimator ITransition.TransitionType { get { return transitionTypeCore; } set { } }
		public IEnumerable<Rectangle> Regions { get; set; }
		public DefaultBoolean ShowWaitingIndicator { get; set; }
		public Control Control { get; set; }
		public Image ImageStart { get { return imageStart; } }
		public WaitingAnimatorType WaitingAnimatorType { get; set; }
		public IWaitingIndicatorProperties WaitingIndicatorProperties {
			get { return waitingIndicatorProperties; }
		}
		public ILineWaitingIndicatorProperties LineWaitingIndicatorProperties {
			get { return lineWaitingIndicatorProperties; }
		}
		public IRingWaitingIndicatorProperties RingWaitingIndicatorProperties {
			get { return ringWaitingIndicatorProperties; }
		}
		public ITransitionAnimator GetCustomTransitionAnimator() {
			if(Control == null) return null;
			if(imageEnd == null)
				imageEnd = GetControlImage(Control);
			transitionTypeCore.Restart(imageStart, imageEnd, transitionTypeCore.Parameters);			
			transitionTypeCore.Parameters.Combine(parametrs);
			return transitionTypeCore;
		}
		#endregion        
		bool isDisposing;
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				waitingIndicatorProperties = null;
				lineWaitingIndicatorProperties = null;
				ringWaitingIndicatorProperties = null;
				Ref.Dispose(ref imageStart);
				Ref.Dispose(ref imageEnd);
				parametrs = null;
				if(transitionTypeCore != null)
					transitionTypeCore.Dispose();
				Regions = null;
				Control = null;
			}
		}		
	}
}
