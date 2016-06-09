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
using System.Drawing;
using System.Drawing.Imaging;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using System.Collections.Generic;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public enum TransitionAnimation {
		HorizontalSlide,
		VerticalSlide,
		FadeIn,
		SegmentedFade,
		RandomSegmentedFade,
		None
	}
	public class AnimationParameters : DevExpress.Utils.Animation.AnimationParameters {
		public AnimationParameters(TransitionAnimation type, int interval, int count, bool forward) : base(interval, count) {
			Type = type;
			ForwardDirection = forward;
		}				
		public bool ForwardDirection { get; private set; }
		public TransitionAnimation Type { get; private set; }
	}   
	class HorizontalSlideTransitionAnimator : DevExpress.Utils.Animation.SlideTransition {		
		public HorizontalSlideTransitionAnimator(Image from, Image to, AnimationParameters parameters)
			: base(from, to, parameters) {		 
		}
		protected override bool ForwardDirection { get { return Parameters.ForwardDirection; } }		
		public new AnimationParameters Parameters { get { return base.Parameters as AnimationParameters; } }
		protected override Data.Utils.EasingMode DefaultEasingMode { get { return Data.Utils.EasingMode.EaseInOut; } }
	}
	class VerticalSlideTransitionAnimator : DevExpress.Utils.Animation.SlideTransition {		
		public VerticalSlideTransitionAnimator(Image from, Image to, AnimationParameters parameters)
			: base(from, to, parameters) {
		}
		protected override bool ForwardDirection { get { return Parameters.ForwardDirection; } }
		protected override DevExpress.Utils.Animation.TransitionDirection TransitionDirection { get { return DevExpress.Utils.Animation.TransitionDirection.Vertical; } }
		public new AnimationParameters Parameters { get { return base.Parameters as AnimationParameters; } }
		protected override Data.Utils.EasingMode DefaultEasingMode { get { return Data.Utils.EasingMode.EaseInOut; } }
	}
	class FadeInTransitionAnimator : DevExpress.Utils.Animation.FadeTransition {
		public FadeInTransitionAnimator(Image from, Image to, AnimationParameters parameters)
			: base(from, to, parameters) {
		}
		protected override Data.Utils.EasingMode DefaultEasingMode { get { return Data.Utils.EasingMode.EaseInOut; } }
	}
	class SegmentedFadeTransitionAnimator : DevExpress.Utils.Animation.SegmentedFadeTransition {	   
		public SegmentedFadeTransitionAnimator(Image from, Image to, AnimationParameters parameters)
			: base(from, to, parameters) {
		}
		protected override Data.Utils.EasingMode DefaultEasingMode { get { return Data.Utils.EasingMode.EaseInOut; } }
	}
	class RandomSegmentedFadeTransitionAnimator : DevExpress.Utils.Animation.DissolveTransition {
		public RandomSegmentedFadeTransitionAnimator(Image from, Image to, AnimationParameters parameters)
			: base(from, to, parameters) {
		}
		protected override Data.Utils.EasingMode DefaultEasingMode { get { return Data.Utils.EasingMode.EaseInOut; } }
	}
}
