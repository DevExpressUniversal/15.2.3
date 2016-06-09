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
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartCrosshairAxisLabelOptionsPropertyGridModel : NestedElementPropertyGridModelBase {
		readonly CrosshairAxisLabelOptions crosshairAxisLabelOptions;
		readonly SetAxisPropertyCommand setPropertyCommand;
		protected internal CrosshairAxisLabelOptions CrosshairAxisLabelOptions { get { return crosshairAxisLabelOptions; } }
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		[Category(Categories.Behavior)]
		public string Pattern {
			get { return CrosshairAxisLabelOptions.Pattern; }
			set { SetProperty("Pattern", value); }
		}
		[Category(Categories.Behavior)]
		public bool? Visibility {
			get { return CrosshairAxisLabelOptions.Visibility; }
			set { SetProperty("Visibility", value); }
		}
		[Category(Categories.Brushes)]
		public Brush Background {
			get { return CrosshairAxisLabelOptions.Background; }
			set { SetProperty("Background", value); }
		}
		[Category(Categories.Brushes)]
		public Brush Foreground {
			get { return CrosshairAxisLabelOptions.Foreground; }
			set { SetProperty("Foreground", value); }
		}
		[Category(Categories.Text)]
		public FontFamily FontFamily {
			get { return CrosshairAxisLabelOptions.FontFamily; }
			set { SetProperty("FontFamily", value); }
		}
		[Category(Categories.Text)]
		public double FontSize {
			get { return CrosshairAxisLabelOptions.FontSize; }
			set { SetProperty("FontSize", value); }
		}
		[Category(Categories.Text)]
		public FontStretch FontStretch {
			get { return CrosshairAxisLabelOptions.FontStretch; }
			set { SetProperty("FontStretch", value); }
		}
		[Category(Categories.Text)]
		public FontStyle FontStyle {
			get { return CrosshairAxisLabelOptions.FontStyle; }
			set { SetProperty("FontStyle", value); }
		}
		[Category(Categories.Text)]
		public FontWeight FontWeight {
			get { return CrosshairAxisLabelOptions.FontWeight; }
			set { SetProperty("FontWeight", value); }
		}
		public WpfChartCrosshairAxisLabelOptionsPropertyGridModel() : this(null, null, string.Empty) { }
		public WpfChartCrosshairAxisLabelOptionsPropertyGridModel(ChartModelElement modelElement, CrosshairAxisLabelOptions crosshairAxisLabelOptions, string propertyPath)
			: base(modelElement, propertyPath) {
			this.crosshairAxisLabelOptions = crosshairAxisLabelOptions;
			this.setPropertyCommand = new SetAxisPropertyCommand(ChartModel);
		}
	}
}
