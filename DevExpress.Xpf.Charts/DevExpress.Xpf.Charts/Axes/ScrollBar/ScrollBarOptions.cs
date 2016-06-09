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
using System.Windows;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum ScrollBarAlignment {
		Near,
		Far
	}
	public class ScrollBarOptions : ChartDependencyObject {
		public static readonly DependencyProperty AlignmentProperty = DependencyPropertyManager.Register("Alignment", 
			typeof(ScrollBarAlignment), typeof(ScrollBarOptions), new PropertyMetadata(ScrollBarAlignment.Near, NotifyPropertyChanged));
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible", typeof(bool), typeof(ScrollBarOptions), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty BarThicknessProperty = DependencyPropertyManager.Register("BarThickness", 
			typeof(double), typeof(ScrollBarOptions), new PropertyMetadata(16.0, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ScrollBarOptionsAlignment"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public ScrollBarAlignment Alignment {
			get { return (ScrollBarAlignment)GetValue(AlignmentProperty); }
			set { SetValue(AlignmentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ScrollBarOptionsVisible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ScrollBarOptionsBarThickness"),
#endif
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public double BarThickness {
			get { return (double)GetValue(BarThicknessProperty); }
			set { SetValue(BarThicknessProperty, value); }
		}
		public ScrollBarOptions() {
		}
		protected override ChartDependencyObject CreateObject() {
			return new ScrollBarOptions();
		}
	}
}
