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
using System.Windows.Media;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class SeriesBorder : ChartDependencyObject {
		public static readonly DependencyProperty BrushProperty = DependencyPropertyManager.Register("Brush",
			typeof(SolidColorBrush), typeof(SeriesBorder), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty LineStyleProperty = DependencyPropertyManager.Register("LineStyle",
			typeof(LineStyle), typeof(SeriesBorder), new PropertyMetadata(null, NotifyPropertyChanged));
		[Category(Categories.Common),
		XtraSerializableProperty]
		public SolidColorBrush Brush {
			get { return (SolidColorBrush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}
		[
		Category(Categories.Common),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public LineStyle LineStyle {
			get { return (LineStyle)GetValue(LineStyleProperty); }
			set { SetValue(LineStyleProperty, value); }
		}
		protected override ChartDependencyObject CreateObject() {
			return new SeriesBorder();
		}
	}
}
