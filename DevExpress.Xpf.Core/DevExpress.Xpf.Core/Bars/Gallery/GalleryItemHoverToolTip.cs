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

using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
namespace DevExpress.Xpf.Bars {
	public class GalleryItemHoverToolTip : ToolTip {
		#region static
		private static readonly object endGrowAnimationEventHandler;
		private static readonly object endShrinkAnimationEventHandler;
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty LargeGlyphSizeProperty;
		public static readonly DependencyProperty SmallGlyphSizeProperty;
		public static readonly DependencyProperty AllowAnimationProperty;
		public static readonly DependencyProperty DurationProperty;
		static GalleryItemHoverToolTip() {
			GlyphProperty = DependencyPropertyManager.Register("Glyph", typeof(ImageSource), typeof(GalleryItemHoverToolTip), new FrameworkPropertyMetadata(null));
			LargeGlyphSizeProperty = DependencyPropertyManager.Register("LargeGlyphSize", typeof(Size), typeof(GalleryItemHoverToolTip), new FrameworkPropertyMetadata(new Size(double.NaN, double.NaN)));
			SmallGlyphSizeProperty = DependencyPropertyManager.Register("SmallGlyphSize", typeof(Size), typeof(GalleryItemHoverToolTip), new FrameworkPropertyMetadata(new Size(double.NaN, double.NaN)));
			AllowAnimationProperty = DependencyPropertyManager.Register("AllowAnimation", typeof(bool), typeof(GalleryItemHoverToolTip), new FrameworkPropertyMetadata(false));
			DurationProperty = DependencyPropertyManager.Register("Duration", typeof(int), typeof(GalleryItemHoverToolTip), new FrameworkPropertyMetadata(100));
			endGrowAnimationEventHandler = new object();
			endShrinkAnimationEventHandler = new object();
		}
		#endregion
		#region dep props
		public Size LargeGlyphSize {
			get { return (Size)GetValue(LargeGlyphSizeProperty); }
			set { SetValue(LargeGlyphSizeProperty, value); }
		}
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		public Size SmallGlyphSize {
			get { return (Size)GetValue(SmallGlyphSizeProperty); }
			set { SetValue(SmallGlyphSizeProperty, value); }
		}
		public bool AllowAnimation {
			get { return (bool)GetValue(AllowAnimationProperty); }
			set { SetValue(AllowAnimationProperty, value); }
		}
		public int Duration {
			get { return (int)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}
		#endregion
		public GalleryItemHoverToolTip() {
			DefaultStyleKey = typeof(GalleryItemHoverToolTip);
			Closed += new RoutedEventHandler(OnTooltipClosed);
		}
		void OnTooltipClosed(object sender, RoutedEventArgs e) {
			StopHoverAnimation();			
		}
		EventHandlerList events;
		protected EventHandlerList Events {
			get {
				if(events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		public event EventHandler EndGrowAnimation {
			add { Events.AddHandler(endGrowAnimationEventHandler, value); }
			remove { Events.RemoveHandler(endGrowAnimationEventHandler, value); }
		}
		public event EventHandler EndShrinkAnimation {
			add { Events.AddHandler(endShrinkAnimationEventHandler, value); }
			remove { Events.RemoveHandler(endShrinkAnimationEventHandler, value); }
		}
		protected void RaiseEventByHandler(object eventHandler, EventArgs args) {
			EventHandler h = Events[eventHandler] as EventHandler;
			if(h != null) h(this, args);
		}
		enum HoverAnimationDirection { Forward, Reverse };
		HoverAnimationDirection AnimationDirection { get; set; }
		Storyboard hoverStoryboard = null;
		DoubleAnimation widthAnimation = null;
		DoubleAnimation heightAnimation = null;
		public Image Image { get; private set; }
		private Image LargeImage { get; set; }
		public override void OnApplyTemplate() {
			StopHoverAnimation();
			base.OnApplyTemplate();
			Image = GetTemplateChild("PART_Image") as Image;
			LargeImage = GetTemplateChild("PART_LargeImage") as Image;
			CreateHoverAnimation();
		}
		protected override Size MeasureOverride(Size constraint) {
			return base.MeasureOverride(constraint);
		}
		public void ShrinkAndClose() {		   
			ReverseHoverAnimation();		   
		}
		public void Grow() {
			ForwardHoverAnimation();			
		}
		protected virtual DoubleAnimation CreateWidthAnimation() {
			Size minHoverSize = GetMinGlyphSize();
			Size maxHoverSize = GetMaxGlyphSize();
			DoubleAnimation anm = new DoubleAnimation();
			anm.From = minHoverSize.Width;
			anm.To = maxHoverSize.Width;			
			anm.Duration = new Duration(TimeSpan.FromMilliseconds(GetActualAnimationTime()));
			Storyboard.SetTarget(anm, Image);
			Storyboard.SetTargetProperty(anm, new PropertyPath(FrameworkElement.WidthProperty));
			return anm;
		}
		protected virtual DoubleAnimation CreateHeightAnimation() {
			Size minHoverSize = GetMinGlyphSize();
			Size maxHoverSize = GetMaxGlyphSize();
			DoubleAnimation anm = new DoubleAnimation();
			anm.From = minHoverSize.Height;
			anm.To = maxHoverSize.Height;
			anm.Duration = new Duration(TimeSpan.FromMilliseconds(GetActualAnimationTime()));
			Storyboard.SetTarget(anm, Image);
			Storyboard.SetTargetProperty(anm, new PropertyPath(FrameworkElement.HeightProperty));
			return anm;
		}
		void CreateHoverAnimation() {
			widthAnimation = CreateWidthAnimation();
			heightAnimation = CreateHeightAnimation();
			hoverStoryboard = new Storyboard();
			hoverStoryboard.Children.Add(widthAnimation);
			hoverStoryboard.Children.Add(heightAnimation);
			hoverStoryboard.FillBehavior = FillBehavior.HoldEnd;
			hoverStoryboard.Completed += new EventHandler(OnHoverAnimationComplete);
			AnimationDirection = HoverAnimationDirection.Forward;
		}
		void OnHoverAnimationComplete(object sender, EventArgs e) {
			if(AnimationDirection == HoverAnimationDirection.Reverse) {
				RaiseEventByHandler(endShrinkAnimationEventHandler, new EventArgs());
			}
			else {
				RaiseEventByHandler(endGrowAnimationEventHandler, new EventArgs());
			}
		}
		void StopHoverAnimation() {
			if(hoverStoryboard != null) {
				hoverStoryboard.Stop();
				hoverStoryboard.Completed -= OnHoverAnimationComplete;
			}
			widthAnimation = null;
			heightAnimation = null;
			hoverStoryboard = null;
		}
		void ForwardHoverAnimation() {
			double maxRange = GetMaxGlyphSize().Width - GetMinGlyphSize().Width;
			double to = GetMaxGlyphSize().Width;
			double from = Image.Width;
			double range = to - from;
			widthAnimation.To = to;
			widthAnimation.From = from;
			if(maxRange == 0)
				maxRange = 1;
			widthAnimation.Duration = TimeSpan.FromMilliseconds(GetActualAnimationTime() * range / maxRange);
			heightAnimation.To = GetMaxGlyphSize().Height;
			heightAnimation.From = Image.Height;
			heightAnimation.Duration = TimeSpan.FromMilliseconds(GetActualAnimationTime() * range / maxRange);
			AnimationDirection = HoverAnimationDirection.Forward;
			hoverStoryboard.Begin();			
		}
		void ReverseHoverAnimation() {
			if(hoverStoryboard == null) return;
			hoverStoryboard.Pause();
			double maxRange = GetMaxGlyphSize().Width - GetMinGlyphSize().Width;
			double to = GetMinGlyphSize().Width;
			double from = Image.Width;
			double range = from - to;
			if(maxRange == 0)
				maxRange = 1;
			widthAnimation.To = to;
			widthAnimation.From = from;
			widthAnimation.Duration = TimeSpan.FromMilliseconds(GetActualAnimationTime() * range / maxRange + 1);
			heightAnimation.To = GetMinGlyphSize().Height;
			heightAnimation.From = Image.Height;
			heightAnimation.Duration = TimeSpan.FromMilliseconds(GetActualAnimationTime() * range / maxRange + 1);
			AnimationDirection = HoverAnimationDirection.Reverse;
			hoverStoryboard.Begin();
		}
		Size GetMaxGlyphSize() {
			if(LargeImage == null) return GetMinGlyphSize();			
			return DesiredSize;
		}
		Size GetMinGlyphSize() {
			double width = double.IsNaN(SmallGlyphSize.Width) ? 0 : SmallGlyphSize.Width;
			double height = double.IsNaN(SmallGlyphSize.Height) ? 0 : SmallGlyphSize.Height;
			return new Size(width, height);
		}
		double GetActualAnimationTime() {
			if(!AllowAnimation) return 0;
			return Duration;
		}
	}
}
