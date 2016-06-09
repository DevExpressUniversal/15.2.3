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

using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Data.Summary;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Selection;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using WarningException = System.Exception;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using Visual = System.Windows.UIElement;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using FrameworkContentElement = System.Windows.DependencyObject;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
using ContentPresenter = DevExpress.Xpf.Core.XPFContentPresenter;
using System;
#else
using System.Timers;
using System;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public interface IScrollingStrategyOwner {
		int Left { get; set; }
		int RowsLeft { get; set; }
		int LeftLevelCount { get; }
		int CellsExtendWidth { get; }
		int LeftVisibleCount { get; }
		bool IsLastCellFullyVisible { get; }
		int MaxLevel { get; }
		double TopOffset { get; }
	}
	public interface IHorizontalScrollingStrategy {
		int LeftCoerce(int left);
		int ExtendWidth { get; }
		int GetViewPortWidth(int current);
	}
	public class FixedHorizontalScrollingStrategy : IHorizontalScrollingStrategy {
		IScrollingStrategyOwner owner;
		public FixedHorizontalScrollingStrategy(IScrollingStrategyOwner owner) {
			this.owner = owner;
		}
		int IHorizontalScrollingStrategy.LeftCoerce(int newValue) {
			return newValue;
		}
		int IHorizontalScrollingStrategy.ExtendWidth {
			get { return owner.CellsExtendWidth; }
		}
		int IHorizontalScrollingStrategy.GetViewPortWidth(int current) {
			return current;
		}
	}
	public class UnfixedHorizontalScrollingStrategy : IHorizontalScrollingStrategy {
		IScrollingStrategyOwner owner;
		public UnfixedHorizontalScrollingStrategy(IScrollingStrategyOwner owner) {
			this.owner = owner;
		}
		int IHorizontalScrollingStrategy.LeftCoerce(int newValue) {
			int newCoord = newValue;
			int levelCount = owner.LeftLevelCount;
			newCoord += owner.RowsLeft;
			if(newCoord > levelCount) {
				newCoord -= levelCount;
				owner.RowsLeft = levelCount;
			} else {
				if(!owner.IsLastCellFullyVisible || newCoord <= owner.RowsLeft)
					owner.RowsLeft = Math.Max(0, newCoord);
				newCoord = 0;
			}
			return newCoord;
		}
		int IHorizontalScrollingStrategy.ExtendWidth {
			get { return owner.CellsExtendWidth + owner.LeftLevelCount; }
		}
		int IHorizontalScrollingStrategy.GetViewPortWidth(int current) {
			if(owner.CellsExtendWidth == 1 && !owner.IsLastCellFullyVisible)
				return owner.LeftVisibleCount;
			if(owner.LeftLevelCount == owner.RowsLeft)
				return current;
			return current + owner.LeftVisibleCount;
		}
	}
}
