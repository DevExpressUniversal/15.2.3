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
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class AxisLabelResolveOverlappingOptions : ChartDependencyObject, IAxisLabelResolveOverlappingOptions {
		public static readonly DependencyProperty AllowHideProperty = DependencyPropertyManager.Register("AllowHide",
			typeof(bool), typeof(AxisLabelResolveOverlappingOptions), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty AllowRotateProperty = DependencyPropertyManager.Register("AllowRotate",
			typeof(bool), typeof(AxisLabelResolveOverlappingOptions), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty AllowStaggerProperty = DependencyPropertyManager.Register("AllowStagger",
			typeof(bool), typeof(AxisLabelResolveOverlappingOptions), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty MinIndentProperty = DependencyPropertyManager.Register("MinIndent",
			typeof(int), typeof(AxisLabelResolveOverlappingOptions), new PropertyMetadata(5, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisLabelResolveOverlappingOptionsAllowHide"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool AllowHide {
			get { return (bool)GetValue(AllowHideProperty); }
			set { SetValue(AllowHideProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisLabelResolveOverlappingOptionsAllowRotate"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool AllowRotate {
			get { return (bool)GetValue(AllowRotateProperty); }
			set { SetValue(AllowRotateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisLabelResolveOverlappingOptionsAllowStagger"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool AllowStagger {
			get { return (bool)GetValue(AllowStaggerProperty); }
			set { SetValue(AllowStaggerProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisLabelResolveOverlappingOptionsMinIndent"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int MinIndent {
			get { return (int)GetValue(MinIndentProperty); }
			set { SetValue(MinIndentProperty, value); }
		}
		protected override ChartDependencyObject CreateObject() {
			return new AxisLabelResolveOverlappingOptions();
		}
	}
}
