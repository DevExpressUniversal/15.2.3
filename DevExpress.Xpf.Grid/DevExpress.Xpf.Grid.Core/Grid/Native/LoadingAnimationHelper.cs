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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
namespace DevExpress.Xpf.Grid.Native {
	public class LoadingAnimationHelper {
		ISupportLoadingAnimation owner;
		public Storyboard StoryToLoadedState { get; set; }
		public Timeline CustomAnimation { get; set; }
		public PropertyPath CustomPropertyPath { get; set; }
		public Effect AddedEffect { get; set; }
		public LoadingAnimationHelper(ISupportLoadingAnimation owner) {
			this.owner = owner;
		}
		public void ApplyAnimation() {
			if(!owner.IsReady)
				ApplyNotLoadedAnimation();
			else
				ApplyLoadedAnimation();
		}
		void ApplyNotLoadedAnimation() {
			if(owner.Element == null) {
				return;
			}
			if(StoryToLoadedState != null) {
				StoryToLoadedState.Stop();
			}
			if(owner.DataView.RowAnimationKind != RowAnimationKind.None) {
				owner.Element.Opacity = 0;
			}
		}
		void ApplyLoadedAnimation() {
			if(owner.Element == null) { return; }
			owner.Element.ClearValue(FrameworkElement.OpacityProperty);
			if(owner.DataView.RowAnimationKind == RowAnimationKind.None) {
				if(StoryToLoadedState != null) {
					StoryToLoadedState.Stop();
					StoryToLoadedState = null;
				}
				return;
			}
			StopAnimation();
			ResetCustomAnimationProperties();
			RaiseRowAnimationBegin();
			RowAnimationKind realAnimationKind = GetRealAnimationKind();
			SetStoryboardTargetProperty(realAnimationKind);
			if(realAnimationKind == RowAnimationKind.Custom) {
				owner.Element.Effect = AddedEffect;
			}
			StoryToLoadedState.Children.Add(CreateAnimation(realAnimationKind));
			Storyboard.SetTargetProperty(StoryToLoadedState, CreatePropertyPath(realAnimationKind));
			StoryToLoadedState.Begin();
		}
		void StopAnimation() {
			if(StoryToLoadedState != null) {
				StoryToLoadedState.Stop();
				StoryToLoadedState.Children.Clear();
			} else {
				StoryToLoadedState = new Storyboard();
			}
		}
		void ResetCustomAnimationProperties() {
			CustomAnimation = null;
			CustomPropertyPath = null;
			AddedEffect = null;
		}
		RowAnimationKind GetRealAnimationKind() {
			RowAnimationKind realAnimationKind = owner.DataView.RowAnimationKind;
			if(realAnimationKind == RowAnimationKind.Custom && (CustomPropertyPath == null || CustomAnimation == null)) {
				realAnimationKind = RowAnimationKind.Opacity;
			}
			return realAnimationKind;
		}
		void SetStoryboardTargetProperty(RowAnimationKind kind) {
			if(kind == RowAnimationKind.Opacity) {
				Storyboard.SetTarget(StoryToLoadedState, owner.Element);
			} else {
				Storyboard.SetTarget(StoryToLoadedState, owner as DependencyObject);
			}
		}
		Timeline CreateAnimation(RowAnimationKind animationKind) {
			switch(animationKind) {
				case RowAnimationKind.Opacity:
					return CreateOpacityAnimation();
				case RowAnimationKind.Custom:
					return CustomAnimation;
				default:
					return null;
			}
		}
		PropertyPath CreatePropertyPath(RowAnimationKind animationKind) {
			switch(animationKind) {
				case RowAnimationKind.Opacity:
					return new PropertyPath("Opacity");
				case RowAnimationKind.Custom:
					return CustomPropertyPath;
				default:
					return null;
			}
		}
		DoubleAnimation CreateOpacityAnimation() {
			DoubleAnimation animation = new DoubleAnimation();
			animation.From = 0;
			animation.To = 1;
			animation.FillBehavior = FillBehavior.Stop;
			animation.Duration = owner.DataView.RowOpacityAnimationDuration;
			return animation;
		}
		void RaiseRowAnimationBegin() {
			if(owner.DataView.RowAnimationKind == RowAnimationKind.Custom) {
				owner.DataView.RaiseRowAnimationBegin(this, owner.IsGroupRow);
			}
		}
	}
	public interface ISupportLoadingAnimation {
		DataViewBase DataView { get; }
		bool IsReady { get; }
		FrameworkElement Element { get; }
		bool IsGroupRow { get; }
	}
}
