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
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Media.Animation;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars;
namespace DevExpress.Xpf.Utils.Native {
	public class VisualStateHelper {
		public static void UpdateStates(FrameworkElement owner, string groupName, Dictionary<string, DependencyProperty> properties) {
			if (owner == null)
				return;
			IList groups = GetVisualStateGroups(owner);
			foreach (VisualStateGroup group in groups) {
				if (!group.Name.Equals(groupName))
					continue;
				foreach (VisualState state in group.States) {
					if (state.Storyboard == null)
						continue;
					foreach (Timeline timeLine in state.Storyboard.Children) {
						PropertyPath propertyPath = Storyboard.GetTargetProperty(timeLine);
						if (!propertyPath.Path.Contains("(0)")) {
							string path = GetCorrectPath(propertyPath);
							DependencyProperty prop = GetDependencyPropertyByPath(path, properties);
							if (prop != null)
								Storyboard.SetTargetProperty(timeLine, new PropertyPath(prop));
						}
						if (String.IsNullOrEmpty(Storyboard.GetTargetName(timeLine)))
							Storyboard.SetTarget(timeLine, owner);
					}
				}
			}
		}
		public static IList GetVisualStateGroups(FrameworkElement owner) {
			IList groups = VisualStateManager.GetVisualStateGroups(owner);
			if (groups.Count != 0)
				return groups;
			if (VisualTreeHelper.GetChildrenCount(owner) == 0)
				return groups;
			FrameworkElement elem = VisualTreeHelper.GetChild(owner, 0) as FrameworkElement;
			if (elem == null)
				return groups;
			groups = VisualStateManager.GetVisualStateGroups(elem);
			return groups;
		}
		static DependencyProperty GetDependencyPropertyByPath(string path, Dictionary<string, DependencyProperty> properties) {
			if (!properties.ContainsKey(path))
				return null;
			return properties[path];
		}
		static string GetCorrectPath(PropertyPath propertyPath) {
			return propertyPath.Path.Contains(':') ? propertyPath.Path.Split(':')[1].Split(')')[0] : String.Empty;
		}
	}
}
namespace DevExpress.Xpf.Core.Native {
	public class ValueSetter : DependencyObject {
		public static readonly DependencyProperty ThicknessProperty = DependencyProperty.RegisterAttached("Thickness", typeof(Thickness), typeof(ValueSetter), new PropertyMetadata(new Thickness(99), new PropertyChangedCallback(OnThicknessChanged)));
		public static readonly DependencyProperty VisibilityProperty = DependencyProperty.RegisterAttached("Visibility", typeof(Visibility?), typeof(ValueSetter), new PropertyMetadata(null, new PropertyChangedCallback(OnVisibilityChanged)));
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ValueSetter), new PropertyMetadata(new CornerRadius(99), new PropertyChangedCallback(OnCornerRadiusChanged)));
		public static readonly DependencyProperty HideBorderSideProperty = DependencyProperty.RegisterAttached("HideBorderSide", typeof(HideBorderSide?), typeof(ValueSetter), new PropertyMetadata(null, new PropertyChangedCallback(OnHideBorderSideChanged)));
		public static readonly DependencyProperty VerticalAlignmentProperty = DependencyProperty.RegisterAttached("VerticalAlignment", typeof(VerticalAlignment?), typeof(ValueSetter), new PropertyMetadata(null, new PropertyChangedCallback(OnVerticalAlignmentChanged)));
		public static readonly DependencyProperty HorizontalAlignmentProperty = DependencyProperty.RegisterAttached("HorizontalAlignment", typeof(HorizontalAlignment?), typeof(ValueSetter), new PropertyMetadata(null, new PropertyChangedCallback(OnHorizontalAlignmentChanged)));
		public static readonly DependencyProperty FontWeightProperty = DependencyProperty.RegisterAttached("FontWeight", typeof(FontWeight), typeof(ValueSetter), new PropertyMetadata(FontWeights.Normal, new PropertyChangedCallback(OnFontWeightChanged)));
		public static Thickness GetThickness(DependencyObject d) { return (Thickness)d.GetValue(ThicknessProperty); }
		public static void SetThickness(DependencyObject d, Thickness value) { d.SetValue(ThicknessProperty, value); }
		public static Visibility? GetVisibility(DependencyObject d) { return (Visibility?)d.GetValue(VisibilityProperty); }
		public static void SetVisibility(DependencyObject d, Visibility value) { d.SetValue(VisibilityProperty, value); }
		public static CornerRadius GetCornerRadius(DependencyObject d) { return (CornerRadius)d.GetValue(CornerRadiusProperty); }
		public static void SetCornerRadius(DependencyObject d, CornerRadius value) { d.SetValue(CornerRadiusProperty, value); }
		public static HideBorderSide? GetHideBorderSide(DependencyObject d) { return (HideBorderSide?)d.GetValue(HideBorderSideProperty); }
		public static void SetHideBorderSide(DependencyObject d, HideBorderSide value) { d.SetValue(HideBorderSideProperty, value); }
		public static VerticalAlignment? GetVerticalAlignment(DependencyObject d) { return (VerticalAlignment?)d.GetValue(VerticalAlignmentProperty); }
		public static void SetVerticalAlignment(DependencyObject d, VerticalAlignment value) { d.SetValue(VerticalAlignmentProperty, value); }
		public static HorizontalAlignment? GetHorizontalAlignment(DependencyObject d) { return (HorizontalAlignment?)d.GetValue(HorizontalAlignmentProperty); }
		public static void SetHorizontalAlignment(DependencyObject d, HorizontalAlignment value) { d.SetValue(HorizontalAlignmentProperty, value); }
		public static FontWeight GetFontWeight(DependencyObject d) { return (FontWeight)d.GetValue(FontWeightProperty); }
		public static void SetFontWeight(DependencyObject d, FontWeight value) { d.SetValue(FontWeightProperty, value); }
		static KeyTime ZeroTime = TimeSpan.FromMilliseconds(0);
		private static void OnThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ObjectAnimationUsingKeyFrames anim = o as ObjectAnimationUsingKeyFrames;
			if (anim == null)
				return;
			anim.KeyFrames.Clear();
			anim.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = ZeroTime, Value = e.NewValue });
		}
		private static void OnVisibilityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ObjectAnimationUsingKeyFrames anim = o as ObjectAnimationUsingKeyFrames;
			if (anim == null)
				return;
			anim.KeyFrames.Clear();
			anim.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = ZeroTime, Value = ((Visibility?)e.NewValue).Value});
		}
		private static void OnCornerRadiusChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ObjectAnimationUsingKeyFrames anim = o as ObjectAnimationUsingKeyFrames;
			if (anim == null)
				return;
			anim.KeyFrames.Clear();
			anim.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = ZeroTime, Value = e.NewValue });
		}
		private static void OnHideBorderSideChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ObjectAnimationUsingKeyFrames anim = o as ObjectAnimationUsingKeyFrames;
			if (anim == null)
				return;
			anim.KeyFrames.Clear();
			anim.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = ZeroTime, Value = ((HideBorderSide?)e.NewValue).Value });
		}
		private static void OnVerticalAlignmentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ObjectAnimationUsingKeyFrames anim = o as ObjectAnimationUsingKeyFrames;
			if (anim == null)
				return;
			anim.KeyFrames.Clear();
			anim.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = ZeroTime, Value = ((VerticalAlignment?)e.NewValue).Value });
		}
		private static void OnHorizontalAlignmentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ObjectAnimationUsingKeyFrames anim = o as ObjectAnimationUsingKeyFrames;
			if(anim == null)
				return;
			anim.KeyFrames.Clear();
			anim.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = ZeroTime, Value = ((HorizontalAlignment?)e.NewValue).Value });
		}
		static void OnFontWeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ObjectAnimationUsingKeyFrames anim = o as ObjectAnimationUsingKeyFrames;
			if (anim == null)
				return;
			anim.KeyFrames.Clear();
			anim.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = ZeroTime, Value = e.NewValue });
		}
	}
}
