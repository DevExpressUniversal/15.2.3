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

using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Editors;
using System.ComponentModel;
using System.Windows;
namespace DevExpress.Xpf.Charts.RangeControlClient {
	public class RangeControlClientBarView : RangeControlClientView {
		const int defaultBarDistance = 2;
		public static readonly DependencyProperty BarDistanceProperty =
			DependencyProperty.Register("BarDistance", typeof(int), typeof(RangeControlClientBarView),
			new PropertyMetadata(defaultBarDistance, (d, e) => ((RangeControlClientBarView)d).BarDistanceChanged((int)e.NewValue)));
		[Category(Categories.Layout)]
		public int BarDistance {
			get { return (int)GetValue(BarDistanceProperty); }
			set { SetValue(BarDistanceProperty, value); }
		}
		public RangeControlClientBarView()
			: base(new BarSparklineControl()) {
			DefaultStyleKey = typeof(RangeControlClientBarView);
		}
		void BarDistanceChanged(int value) {
			((BarSparklineControl)Sparkline).BarDistance = value;
		}
	}
}
