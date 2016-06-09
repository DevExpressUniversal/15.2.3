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

using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class TickmarkOptions : GaugeDependencyObject {
		public static readonly DependencyProperty OffsetProperty = DependencyPropertyManager.Register("Offset",
			typeof(double), typeof(TickmarkOptions), new PropertyMetadata(-27.0, NotifyPropertyChanged));
		public static readonly DependencyProperty FactorLengthProperty = DependencyPropertyManager.Register("FactorLength",
			typeof(double), typeof(TickmarkOptions), new PropertyMetadata(1.0, NotifyPropertyChanged));
		public static readonly DependencyProperty FactorThicknessProperty = DependencyPropertyManager.Register("FactorThickness",
			typeof(double), typeof(TickmarkOptions), new PropertyMetadata(1.0, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("TickmarkOptionsOffset"),
#endif
		Category(Categories.Layout)
		]
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("TickmarkOptionsFactorLength"),
#endif
		Category(Categories.Layout)
		]
		public double FactorLength {
			get { return (double)GetValue(FactorLengthProperty); }
			set { SetValue(FactorLengthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("TickmarkOptionsFactorThickness"),
#endif
		Category(Categories.Layout)
		]
		public double FactorThickness {
			get { return (double)GetValue(FactorThicknessProperty); }
			set { SetValue(FactorThicknessProperty, value); }
		}
		protected virtual bool IsTickVisible(TickmarkInfo info) {
			return true;
		}
		internal ScaleElementLayout CalculateLayout(TickmarkInfo elementInfo, ScaleMapping mapping) {
			if (IsTickVisible(elementInfo) && !mapping.Layout.IsEmpty)
				return new ScaleElementLayout(mapping.GetAngleByPercent(elementInfo.Alpha), mapping.GetPointByPercent(elementInfo.Alpha, Offset), new Point(FactorLength, FactorThickness));
			return null;
		}
	}
	public class MinorTickmarkOptions : TickmarkOptions {
		public static readonly DependencyProperty ShowTicksForMajorProperty = DependencyPropertyManager.Register("ShowTicksForMajor",
			typeof(bool), typeof(MinorTickmarkOptions), new PropertyMetadata(false, NotifyPropertyChanged));
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
			typeof(int), typeof(MinorTickmarkOptions), new PropertyMetadata(10, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("MinorTickmarkOptionsShowTicksForMajor"),
#endif
		Category(Categories.Behavior)
		]
		public bool ShowTicksForMajor {
			get { return (bool)GetValue(ShowTicksForMajorProperty); }
			set { SetValue(ShowTicksForMajorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("MinorTickmarkOptionsZIndex"),
#endif
		Category(Categories.Layout)
		]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		protected override bool IsTickVisible(TickmarkInfo elementInfo) {
			MinorTickmarkInfo minorTick = elementInfo as MinorTickmarkInfo;
			return base.IsTickVisible(elementInfo) && minorTick != null && (!minorTick.BelowMajorTick || ShowTicksForMajor);
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MinorTickmarkOptions();
		}
	}
	public class MajorTickmarkOptions : TickmarkOptions {
		public static readonly DependencyProperty ShowFirstProperty = DependencyPropertyManager.Register("ShowFirst",
			typeof(bool), typeof(MajorTickmarkOptions), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty ShowLastProperty = DependencyPropertyManager.Register("ShowLast",
			typeof(bool), typeof(MajorTickmarkOptions), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
			typeof(int), typeof(MajorTickmarkOptions), new PropertyMetadata(20, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("MajorTickmarkOptionsShowFirst"),
#endif
		Category(Categories.Behavior)
		]
		public bool ShowFirst {
			get { return (bool)GetValue(ShowFirstProperty); }
			set { SetValue(ShowFirstProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("MajorTickmarkOptionsShowLast"),
#endif
		Category(Categories.Behavior)
		]
		public bool ShowLast {
			get { return (bool)GetValue(ShowLastProperty); }
			set { SetValue(ShowLastProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("MajorTickmarkOptionsZIndex"),
#endif
		Category(Categories.Layout)
		]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		protected override bool IsTickVisible(TickmarkInfo elementInfo) {
			MajorTickmarkInfo majorTick = elementInfo as MajorTickmarkInfo;
			return base.IsTickVisible(elementInfo) && majorTick != null &&
				(!majorTick.IsFirstTick || ShowFirst) && (!majorTick.IsLastTick || ShowLast);
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MajorTickmarkOptions();
		}
	}
	[NonCategorized]
	public abstract class TickmarkInfo : ScaleElementInfoBase {
		readonly double alpha;
		internal double Alpha { get { return alpha; } }
		internal TickmarkInfo(PresentationControl presentationControl, PresentationBase presentation, double alpha)
			: base(presentationControl, presentation) {
			this.alpha = alpha;
		}
	}
	public class MinorTickmarkInfo : TickmarkInfo {
		readonly bool belowMajorTick;
		internal bool BelowMajorTick { get { return belowMajorTick; } }
		internal MinorTickmarkInfo(PresentationControl presentationControl, PresentationBase presentation, double alpha, bool belowMajorTick)
			: base(presentationControl, presentation, alpha) {
			this.belowMajorTick = belowMajorTick;
		}
	}
	public class MajorTickmarkInfo : TickmarkInfo {
		readonly double value;
		readonly bool isFirstTick;
		readonly bool isLastTick;
		internal double Value { get { return value; } }
		internal bool IsFirstTick { get { return isFirstTick; } }
		internal bool IsLastTick { get { return isLastTick; } }
		internal MajorTickmarkInfo(PresentationControl presentationControl, PresentationBase presentation, double alpha, double value, bool isFirstTick, bool isLastTick)
			: base(presentationControl, presentation, alpha) {
			this.value = value;
			this.isFirstTick = isFirstTick;
			this.isLastTick = isLastTick;
		}
	}
}
