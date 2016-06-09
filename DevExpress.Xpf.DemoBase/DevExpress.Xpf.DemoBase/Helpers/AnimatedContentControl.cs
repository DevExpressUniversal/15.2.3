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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.DemoBase.Helpers {
	public class AnimatedContentControl : ContentControl {
		UIElement currentControl;
		Action onAnimationCompleted;
		Grid content;
		List<Tuple<object, UIElement>> controls = new List<Tuple<object, UIElement>>();
		Tuple<object, object> lastContentChanged;
		protected override void OnContentChanged(object oldContent, object newContent) {
			Debug.Assert(newContent != null);
			Debug.Assert(controls.Count < 10);
			UIElement control;
			if(ContentTemplateSelector == null || content == null) {
				lastContentChanged = Tuple.Create(oldContent, newContent);
				return;
			}
			var tuple = controls.FirstOrDefault(t => Equals(t.Item1, newContent));
			if (tuple == null) {
				var template = ContentTemplateSelector.SelectTemplate(newContent, this);
				control = new Border {
					Child = (UIElement)template.LoadContent(),
					RenderTransform = new TranslateTransform(DesiredSize.Width, 0)
				};
				tuple = Tuple.Create(newContent, control);
				content.Children.Add(control);
				controls.Add(tuple);
			} else {
				control = tuple.Item2;
			}
			if(currentControl == null) {
				((TranslateTransform)control.RenderTransform).X = 0;
				currentControl = control;
				return;
			}
			SwapTo(control);
		}
		void SwapTo(UIElement control) {
			onAnimationCompleted = () => {
				Animate(control, 0);
				currentControl = control;
			};
			Animate(currentControl, DesiredSize.Width);
		}
		void Animate(UIElement control, double toX) {
			var animation = new DoubleAnimation {
				To = toX,
				Duration = new Duration(TimeSpan.FromMilliseconds(250)),
				EasingFunction = new CubicEase()
			};
			EventHandler handler = null;
			handler = (s, e) => {
				animation.Completed -= handler;
				onAnimationCompleted.Do(a => a());
				onAnimationCompleted = null;
			};
			animation.Completed += handler;
			control.RenderTransform.BeginAnimation(TranslateTransform.XProperty, animation);
		}
		protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector) {
			if(lastContentChanged != null && content != null) {
				OnContentChanged(lastContentChanged.Item1, lastContentChanged.Item2);
			}
		}
		public override void OnApplyTemplate() {
			content = (Grid)GetTemplateChild("content");
			if(ContentTemplateSelector != null && lastContentChanged != null) {
				OnContentChanged(lastContentChanged.Item1, lastContentChanged.Item2);
			}
		}
		static AnimatedContentControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedContentControl), new FrameworkPropertyMetadata(typeof(AnimatedContentControl)));
		}
	}
}
