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
using System.Windows.Media;
using DevExpress.Xpf.TreeMap.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.TreeMap {
	public class TreeMapGroupGradientColorizer : TreeMapPaletteColorizerBase {
		public static readonly DependencyProperty MinProperty = DependencyProperty.Register("Min", typeof(double), typeof(TreeMapGroupGradientColorizer), new FrameworkPropertyMetadata(0.5, null, CoerceMin));
		public static readonly DependencyProperty MaxProperty = DependencyProperty.Register("Max", typeof(double), typeof(TreeMapGroupGradientColorizer), new FrameworkPropertyMetadata(0.9, null, CoerceMin));
		public static readonly DependencyProperty GradientColorProperty = DependencyProperty.Register("GradientColor", typeof(Color), typeof(TreeMapGroupGradientColorizer), new PropertyMetadata(Colors.Transparent));
		static object CoerceMin(DependencyObject d, object value) {
			double minValue = 0;
			double maxValue = 1;
			double result = (double)value;
			if (result > maxValue)
				return maxValue;
			else if (result < minValue)
				return minValue;
			return value;
		}
		public double Min {
			get { return (double)GetValue(MinProperty); }
			set { SetValue(MinProperty, value); }
		}
		public double Max {
			get { return (double)GetValue(MaxProperty); }
			set { SetValue(MaxProperty, value); }
		}
		public Color GradientColor {
			get { return (Color)GetValue(GradientColorProperty); }
			set { SetValue(GradientColorProperty, value); }
		}
		protected override TreeMapDependencyObject CreateObject() {
			return new TreeMapGroupGradientColorizer();
		}
		public override Color? GetItemColor(TreeMapItem item, TreeMapItemGroupInfo group) {
			if (!item.IsGroup && Palette != null && Palette.Count > 0) {
				double ratio = Max;
				if (group.MaxValue > group.MinValue) 
					ratio = Max - (group.MaxValue - item.Value) * ((Max - Min) / (group.MaxValue - group.MinValue));
				return RangeColorizerCalculator.MixColors(GradientColor, Palette[group.GroupIndex], ratio);
			}
			return null;
		}
	}
}
