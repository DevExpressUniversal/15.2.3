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

using System.Collections.Generic;
using System.Windows;
using System.Linq;
using DevExpress.TreeMap.Core;
using System.ComponentModel;
using DevExpress.Xpf.TreeMap.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.TreeMap {
	public enum LayoutDirection {
		TopLeftToBottomRight = LayoutAlgorithmDirection.TopLeftToBottomRight,
		BottomLeftToTopRight = LayoutAlgorithmDirection.BottomLeftToTopRight,
		TopRightToBottomLeft = LayoutAlgorithmDirection.TopRightToBottomLeft,
		BottomRightToTopLeft = LayoutAlgorithmDirection.BottomRightToTopLeft,
	}
	public enum SliceAndDiceLayoutMode {
		Auto = SliceAndDiceAlgorithmLayoutMode.Auto,
		Vertical = SliceAndDiceAlgorithmLayoutMode.Vertical,
		Horizontal = SliceAndDiceAlgorithmLayoutMode.Horizontal,
	}
	public abstract class TreeMapLayoutAlgorithmBase : TreeMapDependencyObject {
		public abstract void Calculate(IList<ITreeMapLayoutItem> items, Size size);
	}
	public abstract class TreeMapLayoutAlgorithm : TreeMapLayoutAlgorithmBase { 
		public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction",
			typeof(LayoutDirection), typeof(TreeMapLayoutAlgorithm), new PropertyMetadata(LayoutDirection.TopLeftToBottomRight));
		public LayoutDirection Direction {
			get { return (LayoutDirection)GetValue(DirectionProperty); }
			set { SetValue(DirectionProperty, value); }
		}
	}
	public class SquarifiedLayoutAlgorithm : TreeMapLayoutAlgorithm {
		protected override TreeMapDependencyObject CreateObject() {
			return new SquarifiedLayoutAlgorithm();
		}
		public override void Calculate(IList<ITreeMapLayoutItem> items, Size size) {
			List<ITreeMapLayoutItem> list = items.ToList<ITreeMapLayoutItem>();
			list.Sort(new TreeMapLayoutItemComparer());
			new SquarifiedAlgorithm((LayoutAlgorithmDirection)Direction).Calculate(list, size);
		}
	}
	public class StripedLayoutAlgorithm : TreeMapLayoutAlgorithm {
		const double defaultLastStripeMinThickness = 0.025;
		public static readonly DependencyProperty LastStripeMinThicknessProperty = DependencyProperty.Register("LastStripeMinThickness",
			typeof(double), typeof(StripedLayoutAlgorithm), new FrameworkPropertyMetadata(defaultLastStripeMinThickness, null, CoerceLastStripeMinThickness));
		static object CoerceLastStripeMinThickness(DependencyObject d, object value) {
			double minThickness = 0;
			double maxThickness = 1;
			double treshold = (double)value;
			if (treshold > maxThickness)
				return maxThickness;
			else if (treshold < minThickness)
				return minThickness;
			return value;
		}
		public double LastStripeMinThickness {
			get { return (double)GetValue(LastStripeMinThicknessProperty); }
			set { SetValue(LastStripeMinThicknessProperty, value); }
		}
		protected override TreeMapDependencyObject CreateObject() {
			return new StripedLayoutAlgorithm();
		}
		public override void Calculate(IList<ITreeMapLayoutItem> items, Size size) {
			List<ITreeMapLayoutItem> list = items.ToList<ITreeMapLayoutItem>();
			list.Sort(new TreeMapLayoutItemComparer());
			new StripedAlgorithm((LayoutAlgorithmDirection)Direction, LastStripeMinThickness).Calculate(list, size);
		}
	}
	public class SliceAndDiceLayoutAlgorithm : TreeMapLayoutAlgorithm {
		public static readonly DependencyProperty LayoutModeProperty = DependencyProperty.Register("LayoutMode",
			typeof(SliceAndDiceLayoutMode), typeof(SliceAndDiceLayoutAlgorithm), new PropertyMetadata(SliceAndDiceLayoutMode.Auto));
		public SliceAndDiceLayoutMode LayoutMode {
			get { return (SliceAndDiceLayoutMode)GetValue(LayoutModeProperty); }
			set { SetValue(LayoutModeProperty, value); }
		}
		protected override TreeMapDependencyObject CreateObject() {
			return new SliceAndDiceLayoutAlgorithm();
		}
		public override void Calculate(IList<ITreeMapLayoutItem> items, Size size) {
			List<ITreeMapLayoutItem> list = items.ToList();
			list.Sort(new TreeMapLayoutItemComparer());
			new SliceAndDiceAlgorithm((LayoutAlgorithmDirection)Direction, (SliceAndDiceAlgorithmLayoutMode)LayoutMode).Calculate(list, size);
		}
	}
}
