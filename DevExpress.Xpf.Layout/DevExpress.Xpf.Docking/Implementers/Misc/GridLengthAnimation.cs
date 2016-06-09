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
using System.Windows;
using System.Windows.Media.Animation;
namespace DevExpress.Xpf.Docking {
	public class GridLengthAnimation : AnimationTimeline {
		#region static
		public static readonly GridLength ZeroStar = new GridLength(0, GridUnitType.Star);
		public static readonly GridLength Zero = new GridLength(0, GridUnitType.Pixel);
		public static readonly DependencyProperty ToProperty;
		public static readonly DependencyProperty FromProperty;
		static GridLengthAnimation() {
			var dProp = new DependencyPropertyRegistrator<GridLengthAnimation>();
			dProp.Register("To", ref ToProperty, ZeroStar);
			dProp.Register("From", ref FromProperty, ZeroStar);
		}
		#endregion static
		public GridLength To {
			get { return (GridLength)GetValue(ToProperty); }
			set { SetValue(ToProperty, value); }
		}
		public GridLength From {
			get { return (GridLength)GetValue(FromProperty); }
			set { SetValue(FromProperty, value); }
		}
		internal static GridLength CalcCurrentValue(double from, double to, double progress, bool isStar) {
			double factor = (from > to) ? (1.0 - progress) : progress;
			double max = Math.Max(from, to);
			double min = Math.Min(from, to);
			return new GridLength(factor * (max - min) + min, isStar ? GridUnitType.Star : GridUnitType.Pixel);
		}
		protected override Freezable CreateInstanceCore() {
			return new GridLengthAnimation();
		}
		public override Type TargetPropertyType {
			get { return typeof(GridLength); }
		}
		public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock) {
			return CalcCurrentValue(From.Value, To.Value, animationClock.CurrentProgress.Value, To.IsStar);
		}
	}
}
