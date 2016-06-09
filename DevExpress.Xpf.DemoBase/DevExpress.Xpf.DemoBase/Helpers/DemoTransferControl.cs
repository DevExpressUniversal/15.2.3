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
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.DemoData.Helpers;
using DevExpress.Internal.DXWindow;
namespace DevExpress.Xpf.DemoBase.Helpers {
	enum DemoGoneDirection { Left, Right, Back }
	class DemoTransferControl : UniversalContentControl {
		#region DependencyProperties
		public static readonly DependencyProperty GoneDemoToLeftStoryboardProperty;
		public static readonly DependencyProperty GoneDemoToRightStoryboardProperty;
		public static readonly DependencyProperty GoneDemoToBackStoryboardProperty;
		public static readonly DependencyProperty CameDemoFromLeftStoryboardProperty;
		public static readonly DependencyProperty CameDemoFromRightStoryboardProperty;
		public static readonly DependencyProperty CameDemoFromBackStoryboardProperty;
		public static readonly DependencyProperty GoneDemoToLeftFastStoryboardProperty;
		public static readonly DependencyProperty GoneDemoToRightFastStoryboardProperty;
		public static readonly DependencyProperty GoneDemoToBackFastStoryboardProperty;
		public static readonly DependencyProperty CameDemoFromLeftFastStoryboardProperty;
		public static readonly DependencyProperty CameDemoFromRightFastStoryboardProperty;
		public static readonly DependencyProperty CameDemoFromBackFastStoryboardProperty;
		public static readonly DependencyProperty IsDemoGoneProperty;
		public static readonly DependencyProperty IsNextDemoRequestedProperty;
		static DemoTransferControl() {
			Type ownerType = typeof(DemoTransferControl);
			GoneDemoToLeftStoryboardProperty = StoryboardProperty.Register("GoneDemoToLeftStoryboard", ownerType, null, null);
			GoneDemoToRightStoryboardProperty = StoryboardProperty.Register("GoneDemoToRightStoryboard", ownerType, null, null);
			GoneDemoToBackStoryboardProperty = StoryboardProperty.Register("GoneDemoToBackStoryboard", ownerType, null, null);
			CameDemoFromLeftStoryboardProperty = StoryboardProperty.Register("CameDemoFromLeftStoryboard", ownerType, null, null);
			CameDemoFromRightStoryboardProperty = StoryboardProperty.Register("CameDemoFromRightStoryboard", ownerType, null, null);
			CameDemoFromBackStoryboardProperty = StoryboardProperty.Register("CameDemoFromBackStoryboard", ownerType, null, null);
			GoneDemoToLeftFastStoryboardProperty = StoryboardProperty.Register("GoneDemoToLeftFastStoryboard", ownerType, null, null);
			GoneDemoToRightFastStoryboardProperty = StoryboardProperty.Register("GoneDemoToRightFastStoryboard", ownerType, null, null);
			GoneDemoToBackFastStoryboardProperty = StoryboardProperty.Register("GoneDemoToBackFastStoryboard", ownerType, null, null);
			CameDemoFromLeftFastStoryboardProperty = StoryboardProperty.Register("CameDemoFromLeftFastStoryboard", ownerType, null, null);
			CameDemoFromRightFastStoryboardProperty = StoryboardProperty.Register("CameDemoFromRightFastStoryboard", ownerType, null, null);
			CameDemoFromBackFastStoryboardProperty = StoryboardProperty.Register("CameDemoFromBackFastStoryboard", ownerType, null, null);
			IsDemoGoneProperty = DependencyProperty.Register("IsDemoGone", typeof(bool), ownerType, new PropertyMetadata(false, OnIsDemoGoneChanged));
			IsNextDemoRequestedProperty = DependencyProperty.Register("IsNextDemoRequested", typeof(bool?), ownerType, new PropertyMetadata(null, OnIsNextDemoRequestedChanged));
		}
		static void OnIsDemoGoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoTransferControl)d).OnIsDemoGoneChanged(e);
		}
		static void OnIsNextDemoRequestedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoTransferControl)d).OnIsNextDemoRequestedChanged(e);
		}
		#endregion
		Storyboard activeStoryboard;
		FrameworkElement demoPresenter;
		DemoGoneDirection demoGoneDirection = DemoGoneDirection.Back;
		HashSet<Storyboard> startedStoryboards = new HashSet<Storyboard>();
		public DemoTransferControl() {
			DefaultStyleKey = typeof(DemoTransferControl);
			FocusHelper.SetFocusable(this, false);
		}
		public Storyboard GoneDemoToLeftStoryboard { get { return (Storyboard)GetValue(GoneDemoToLeftStoryboardProperty); } set { SetValue(GoneDemoToLeftStoryboardProperty, value); } }
		public Storyboard GoneDemoToRightStoryboard { get { return (Storyboard)GetValue(GoneDemoToRightStoryboardProperty); } set { SetValue(GoneDemoToRightStoryboardProperty, value); } }
		public Storyboard GoneDemoToBackStoryboard { get { return (Storyboard)GetValue(GoneDemoToBackStoryboardProperty); } set { SetValue(GoneDemoToBackStoryboardProperty, value); } }
		public Storyboard CameDemoFromLeftStoryboard { get { return (Storyboard)GetValue(CameDemoFromLeftStoryboardProperty); } set { SetValue(CameDemoFromLeftStoryboardProperty, value); } }
		public Storyboard CameDemoFromRightStoryboard { get { return (Storyboard)GetValue(CameDemoFromRightStoryboardProperty); } set { SetValue(CameDemoFromRightStoryboardProperty, value); } }
		public Storyboard CameDemoFromBackStoryboard { get { return (Storyboard)GetValue(CameDemoFromBackStoryboardProperty); } set { SetValue(CameDemoFromBackStoryboardProperty, value); } }
		public Storyboard GoneDemoToLeftFastStoryboard { get { return (Storyboard)GetValue(GoneDemoToLeftFastStoryboardProperty); } set { SetValue(GoneDemoToLeftFastStoryboardProperty, value); } }
		public Storyboard GoneDemoToRightFastStoryboard { get { return (Storyboard)GetValue(GoneDemoToRightFastStoryboardProperty); } set { SetValue(GoneDemoToRightFastStoryboardProperty, value); } }
		public Storyboard GoneDemoToBackFastStoryboard { get { return (Storyboard)GetValue(GoneDemoToBackFastStoryboardProperty); } set { SetValue(GoneDemoToBackFastStoryboardProperty, value); } }
		public Storyboard CameDemoFromLeftFastStoryboard { get { return (Storyboard)GetValue(CameDemoFromLeftFastStoryboardProperty); } set { SetValue(CameDemoFromLeftFastStoryboardProperty, value); } }
		public Storyboard CameDemoFromRightFastStoryboard { get { return (Storyboard)GetValue(CameDemoFromRightFastStoryboardProperty); } set { SetValue(CameDemoFromRightFastStoryboardProperty, value); } }
		public Storyboard CameDemoFromBackFastStoryboard { get { return (Storyboard)GetValue(CameDemoFromBackFastStoryboardProperty); } set { SetValue(CameDemoFromBackFastStoryboardProperty, value); } }
		public bool IsDemoGone { get { return (bool)GetValue(IsDemoGoneProperty); } set { SetValue(IsDemoGoneProperty, value); } }
		public bool? IsNextDemoRequested { get { return (bool?)GetValue(IsNextDemoRequestedProperty); } set { SetValue(IsNextDemoRequestedProperty, value); } }
		public event EventHandler DemoGone;
		public event EventHandler DemoCame;
		void GoneDemo(bool raiseCompleted = true) {
			Storyboard sb;
			switch(demoGoneDirection) {
				case DemoGoneDirection.Left:
					sb = GoneDemoToLeftStoryboard;
					break;
				case DemoGoneDirection.Right:
					sb = GoneDemoToRightStoryboard;
					break;
				default:
					sb = GoneDemoToBackStoryboard;
					break;
			}
			StopAllStoryboards();
			this.activeStoryboard = sb;
			RunStoryboard(sb, this.demoPresenter, raiseCompleted, OnDemoGoneStoryboardCompleted);
		}
		DemoGoneDirection GetInvertedDemoGoneDirection() {
			return demoGoneDirection == DemoGoneDirection.Left ? DemoGoneDirection.Right : demoGoneDirection == DemoGoneDirection.Right ? DemoGoneDirection.Left : DemoGoneDirection.Back;
		}
		void CameDemo(bool raiseCompleted = true) {
			Storyboard sb;
			switch(GetInvertedDemoGoneDirection()) {
				case DemoGoneDirection.Left:
					sb = CameDemoFromLeftStoryboard;
					break;
				case DemoGoneDirection.Right:
					sb = CameDemoFromRightStoryboard;
					break;
				default:
					sb = CameDemoFromBackStoryboard;
					break;
			}
			StopAllStoryboards();
			this.activeStoryboard = sb;
			RunStoryboard(sb, this.demoPresenter, raiseCompleted, OnDemoCameStoryboardCompleted);
		}
		void RunStoryboard(Storyboard sb, UIElement target, bool raiseCompleted, EventHandler completed) {
			if(sb == null) {
				if(raiseCompleted && completed != null)
					completed(null, EventArgs.Empty);
				return;
			}
			if(!(target.RenderTransform is TransformGroup)) {
				TransformGroup group = new TransformGroup();
				group.Children.Add(new ScaleTransform());
				group.Children.Add(new TranslateTransform());
				target.RenderTransform = group;
			}
			Storyboard.SetTarget(sb, target);
			if(raiseCompleted)
				sb.Completed += completed;
			startedStoryboards.Add(sb);
			sb.Begin(this, true);
		}
		void StopAllStoryboards() {
			StopStoryboard(GoneDemoToLeftStoryboard);
			StopStoryboard(GoneDemoToRightStoryboard);
			StopStoryboard(GoneDemoToBackStoryboard);
			StopStoryboard(CameDemoFromLeftStoryboard);
			StopStoryboard(CameDemoFromRightStoryboard);
			StopStoryboard(CameDemoFromBackStoryboard);
			StopStoryboard(GoneDemoToLeftFastStoryboard);
			StopStoryboard(GoneDemoToRightFastStoryboard);
			StopStoryboard(GoneDemoToBackFastStoryboard);
			StopStoryboard(CameDemoFromLeftFastStoryboard);
			StopStoryboard(CameDemoFromRightFastStoryboard);
			StopStoryboard(CameDemoFromBackFastStoryboard);
		}
		void OnIsDemoGoneChanged(DependencyPropertyChangedEventArgs e) {
			if((bool)e.NewValue) {
				GoneDemo();
			} else {
				CameDemo();
			}
		}
		void OnIsNextDemoRequestedChanged(DependencyPropertyChangedEventArgs e) {
			bool? value = (bool?)e.NewValue;
			if(value.HasValue) {
				demoGoneDirection = value.Value ? DemoGoneDirection.Left : DemoGoneDirection.Right;
			} else {
				demoGoneDirection = DemoGoneDirection.Back;
			}
		}
		void StopStoryboard(Storyboard sb) {
			if(sb == null || !startedStoryboards.Contains(sb)) return;
			sb.Stop(this);
		}
		void OnDemoGoneStoryboardCompleted(object sender, EventArgs e) {
			if(this.activeStoryboard != null)
				this.activeStoryboard.Completed -= OnDemoGoneStoryboardCompleted;
			this.activeStoryboard = null;
			RaiseDemoGone();
		}
		void OnDemoCameStoryboardCompleted(object sender, EventArgs e) {
			this.demoPresenter.RenderTransform = null;
			if(this.activeStoryboard == null) return;
			this.activeStoryboard.Completed -= OnDemoCameStoryboardCompleted;
			this.activeStoryboard = null;
			RaiseDemoCame();
		}
		void RaiseDemoGone() {
			if(DemoGone != null)
				DemoGone(this, EventArgs.Empty);
		}
		void RaiseDemoCame() {
			if(DemoCame != null)
				DemoCame(this, EventArgs.Empty);
		}
		protected override void OnApplyTemplateOverride() {
			base.OnApplyTemplateOverride();
			demoPresenter = (FrameworkElement)GetTemplateChild("DemoPresenter");
		}
	}
}
