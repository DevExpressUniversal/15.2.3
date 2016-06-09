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

using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
#if SILVERLIGHT
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class AnimatedPanel : Panel, IWeakEventListener {
		#region static
		public static readonly DependencyProperty ItemSpacingProperty =
			DependencyProperty.Register("ItemSpacing", typeof(double), typeof(AnimatedPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
		#endregion
		public double ItemSpacing {
			get { return (double)GetValue(ItemSpacingProperty); }
			set { SetValue(ItemSpacingProperty, value); }
		}
		AppBar owner;
		AppBar Owner {
			get {
				if(owner == null) owner = LayoutHelper.FindParentObject<AppBar>(this);
				return owner;
			}
		}
		List<Generation> controlInfo {
			get {
				AppBar bar = Owner;
				if(bar == null) return null;
				return bar.controlInfo;
			}
		}
		int itemsGeneration {
			get { return Owner.itemsGeneration; }
			set { Owner.itemsGeneration = value; }
		}
		internal Panel EffectsLayer { get; set; }
		public AnimatedPanel() {
		}
		internal void AttachToVisualTree(ItemsControl owner) {
			DevExpress.Xpf.WindowsUI.Base.AssertionException.IsNotNull(owner);
			CollectionChangedEventManager.AddListener(owner.Items, this);
			((IWeakEventListener)this).ReceiveWeakEvent(null, this, null);
		}
		internal void DetachFromVisualTree(ItemsControl owner) {
			DevExpress.Xpf.WindowsUI.Base.AssertionException.IsNotNull(owner);
			CollectionChangedEventManager.RemoveListener(owner.Items, this);
		}
		protected override Size MeasureOverride(Size availableSize) {
			targetHeight = 0;
			foreach(var generation in controlInfo) {
				MeasureInternal(generation, availableSize);
				targetHeight = Math.Max(targetHeight, NormalSize.Height);
			}
			foreach(UIElement child in Children) {
				AppBarSeparator sep = child as AppBarSeparator;
				if(sep != null) {
					sep.Height = Math.Max(0.0, targetHeight - sep.Margin.Top - sep.Margin.Bottom);
				}
			}
			if(EffectsLayer != null) {
				EffectsLayer.Height = actualHeight;
			}
			if(frameNo < 2) {
				actualHeight = targetHeight;
			}
			return new Size(NormalSize.Width, actualHeight);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Size final = base.ArrangeOverride(finalSize);
			if(finalSize.Height == 0) {
				return final;
			}
			foreach(UIElement child in Children) {
				child.Arrange(new Rect(new Point(), child.DesiredSize));
			}
			foreach(var generation in controlInfo) {
				ArrangeInternal(generation, finalSize);
			}
			return final;
		}
		int frameNo = 0;
		double targetHeight, actualHeight;
		double EffectiveItemSpacing {
			get { return Owner.IsCompact ? 0 : ItemSpacing; }
		}
		internal void CompositionTarget_Rendering(object sender, EventArgs e) {
			List<Tuple<UIElement, Generation>> deleted = null;
			foreach(Generation gen in controlInfo) {
				foreach(UIElement child in gen.Keys) {
					ControlInfo info = gen[child];
					TranslateTransform transform = (TranslateTransform)child.RenderTransform;
					if(info.TargetX != transform.X) {
						transform.X = Math.Round(StepTo(transform.X, info.TargetX));
					}
					if(info.TargetY != transform.Y) {
						transform.Y = Math.Round(StepTo(transform.Y, info.TargetY));
					}
					if(info.FadingIn && !info.IsNew) {
						child.SetCurrentValue(OpacityProperty, child.Opacity + 0.05);
						if(child.Opacity >= info.Opacity.Value) {
							child.SetCurrentValue(OpacityProperty, info.Opacity.Value);
							info.FadingIn = false;
						}
					}
					if(info.FadingOut) {
						child.SetCurrentValue(OpacityProperty, child.Opacity - 0.05);
						if(child.Opacity <= 0) {
							child.SetCurrentValue(OpacityProperty, 0.0);
							info.FadingOut = false;
							(deleted ?? (deleted = new List<Tuple<UIElement, Generation>>())).Add(new Tuple<UIElement, Generation>(child, gen));
						}
					}
					if(info.Stabilized) {
						child.SetCurrentValue(OpacityProperty, info.Opacity.Value);
					}
					info.Frames++;
				}
			}
			if(deleted != null) {
				foreach(var tuple in deleted) {
					EffectsLayer.Children.Remove(tuple.Item1);
					tuple.Item1.Opacity = 1;
					tuple.Item2.Remove(tuple.Item1);
					if(tuple.Item2.Count == 0 && controlInfo.Count > 1) {
						controlInfo.Remove(tuple.Item2);
					}
				}
				InvalidateMeasure();
			}
			if(ActualHeight != targetHeight) {
				actualHeight = StepTo(ActualHeight, targetHeight);
				InvalidateMeasure();
			}
			++frameNo;
		}
		double ShiftY(double height, FrameworkElement element) {
			if(element.VerticalAlignment == VerticalAlignment.Center)
				return (height - element.DesiredSize.Height) * 0.5;
			else if(element.VerticalAlignment == VerticalAlignment.Bottom)
				return height - element.DesiredSize.Height;
			return 0;
		}
		Dictionary<UIElement, Point> DoTestArrange(Generation generation, Size finalSize, List<UIElement> left, List<UIElement> center, List<UIElement> right) {
			Dictionary<UIElement, Point> result = new Dictionary<UIElement, Point>();
			UIElement rightmost = null;
			foreach(FrameworkElement element in generation.Keys) {
				switch(element.HorizontalAlignment) {
					case HorizontalAlignment.Left:
						left.Add(element);
						break;
					case HorizontalAlignment.Stretch:
						left.Add(element);
						break;
					case HorizontalAlignment.Center:
						center.Add(element);
						break;
					case HorizontalAlignment.Right:
						right.Add(element);
						break;
				}
			}
			if(rightmost != null)
				right.Add(rightmost);
			Func<List<UIElement>, double> measureGroup = (group) => {
				double groupWidth = 0;
				foreach(UIElement element in group) {
					groupWidth += element.DesiredSize.Width;
				}
				return groupWidth + EffectiveItemSpacing * (group.Count - 1);
			};
			double leftWidth = measureGroup(left) + EffectiveItemSpacing;
			double centerWidth = measureGroup(center) ;
			double rightWidth = measureGroup(right) + EffectiveItemSpacing;
			double rightX = finalSize.Width - rightWidth;
			double maxCenterX = rightX - centerWidth;
			double leftX = 0;
			double centerX = Math.Min((finalSize.Width - centerWidth) * 0.5, maxCenterX);
			if(centerX < leftWidth + leftX) centerX = leftWidth + leftX;
			Func<List<UIElement>, double, double> arrangeGroup = (group, startX) => {
				double shiftX = 0;
				foreach(FrameworkElement element in group) {
					double shiftY = this.ShiftY(finalSize.Height, element);
					if(generation.Keys.Contains(element)) {
						ControlInfo info = generation[element];
						info.TargetX = Math.Round(startX + shiftX);
						info.TargetY = Math.Round(shiftY);
					}
					result.Add(element, new Point(startX + shiftX, shiftY));
					shiftX += element.DesiredSize.Width + EffectiveItemSpacing;
				}
				return 0;
			};
			arrangeGroup(left, leftX);
			arrangeGroup(center, centerX);
			arrangeGroup(right, rightX);
			return result;
		}
		void ArrangeInternal(Generation generaton, Size finalSize) {
			List<UIElement> left = new List<UIElement>();
			List<UIElement> center = new List<UIElement>();
			List<UIElement> right = new List<UIElement>();
			Dictionary<UIElement, Point> testArrange = DoTestArrange(generaton, finalSize, left, center, right);
			foreach(UIElement child in testArrange.Keys) {
				if(!generaton.ContainsKey(child)) {
					return;
				}
				ControlInfo info = generaton[child];
				TranslateTransform transform = (TranslateTransform)child.RenderTransform;
				if(info.IsNew) {
					transform.X = info.TargetX;
					transform.Y = info.TargetY;
					if(!info.Opacity.HasValue) {
						info.Opacity = child.Opacity;
						if(info.AllowOpactityChange)
							child.SetCurrentValue(OpacityProperty, 0.0);
					}
				}
			}
		}
		double StepTo(double actual, double target) {
			double delta = Math.Abs(actual - target);
			if(delta <= 1.0) actual = target;
			else {
				double step = delta * 0.075 + 1.0;
				if(actual > target) actual -= step;
				else actual += step;
			}
			return actual;
		}
		void MeasureInternal(Dictionary<UIElement, ControlInfo> controlInfo, Size availableSize) {
			double normalWidth = 0.0;
			double compactWidth = 0.0;
			double normalHeight = 0.0;
			double compactHeight = 0.0;
			foreach(UIElement child in controlInfo.Keys) {
				child.Measure(availableSize);
				AppBarButton button = child as AppBarButton;
				AppBarSeparator sep = child as AppBarSeparator;
				if(button != null) {
					button.IsCompact = Owner.IsCompact;
					normalHeight = Math.Max(normalHeight, button.NormalSize.Height);
					compactHeight = Math.Max(compactHeight, button.CompactSize.Height);
					normalWidth += button.NormalSize.Width;
					compactWidth += button.CompactSize.Width;
				}
				else {
					IAppBarElement element = child as IAppBarElement;
					if(element != null) {
						element.IsCompact = Owner.IsCompact;
					}
					normalWidth += child.DesiredSize.Width;
					compactWidth += child.DesiredSize.Width;
					if(sep == null) {
						normalHeight = Math.Max(normalHeight, child.DesiredSize.Height);
						compactHeight = Math.Max(compactHeight, child.DesiredSize.Height);
					}
				}
			}
			normalHeight = Math.Max(normalHeight, Owner.PartBackButton.ActualHeight);
			compactHeight = Math.Max(compactHeight, Owner.PartBackButton.ActualHeight);
			normalHeight = Math.Max(normalHeight, Owner.PartExitButton.ActualHeight);
			compactHeight = Math.Max(compactHeight, Owner.PartExitButton.ActualHeight);
			NormalSize = new Size(normalWidth + ItemSpacing * (Math.Max(controlInfo.Count - 1, 0)), normalHeight);
			CompactSize = new Size(compactWidth, compactHeight);
		}
		bool OnItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(e != null) {
				Generation lastGen;
				int index;
				switch(e.Action) {
					case NotifyCollectionChangedAction.Reset:
						ProcessCollectionReset();
						break;
					case NotifyCollectionChangedAction.Add:
						lastGen = controlInfo.Last();
						index = e.NewStartingIndex;
						foreach(object item in e.NewItems) {
							FrameworkElement element = Children[index++] as FrameworkElement;
							lastGen.Add(element, new ControlInfo() { FadingIn = true, Opacity = element.Opacity });							
							element.SetCurrentValue(OpacityProperty, 0.0);
							element.RenderTransform = new TranslateTransform();
						}
						break;
					case NotifyCollectionChangedAction.Remove:
						lastGen = controlInfo.Last();
						index = e.OldStartingIndex;
						foreach(UIElement element in lastGen.Keys) {
							if(!Children.Contains(element)) {
								ControlInfo info = lastGen[element];
								info.FadingOut = true;
								if(!EffectsLayer.Children.Contains(element)) {
									EffectsLayer.Children.Add(element);
								}
							}
						}
						break;
				}
			}
			else {
				ProcessCollectionReset();
				return true;
			}
			InvalidateMeasure();
			return true;
		}
		void ProcessCollectionReset() {
			itemsGeneration++;
			Generation newGen;
			if(controlInfo.Count() > 0 && controlInfo.Last().Count == 0) {
				newGen = controlInfo.Last();
			}
			else {
				newGen = new Generation();
			}
			if(Children.Count > 0) {
				foreach(UIElement child in Children) {
					newGen.Add(child, new ControlInfo() { AllowOpactityChange = IsLoaded });
					child.RenderTransform = new TranslateTransform();
					foreach(Generation oldGen in controlInfo) {
						if(oldGen.ContainsKey(child)) {
							if(EffectsLayer.Children.Contains(child)) {
								EffectsLayer.Children.Remove(child);
							}
						}
					}
				}
			}
			if(!controlInfo.Contains(newGen)) {
				controlInfo.Add(newGen);
			}
			if(controlInfo.Count() > 1) {
				Generation prevGen = controlInfo[controlInfo.Count() - 2];
				foreach(var pair in prevGen) {
					if(Children.Contains(pair.Key)) continue;
					pair.Value.FadingOut = true;
					if(!EffectsLayer.Children.Contains(pair.Key)) {
						EffectsLayer.Children.Add(pair.Key);
					}
				}
			}
			InvalidateMeasure();
		}
		public Size NormalSize { get; private set; }
		public Size CompactSize { get; private set; }
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(sender == null) {
				CollectionChangedEventManager.RemoveListener(Owner.Items as INotifyCollectionChanged, this);
				return true;
			}
			return OnItemCollectionChanged(sender, e as NotifyCollectionChangedEventArgs);
		}
	}
}
