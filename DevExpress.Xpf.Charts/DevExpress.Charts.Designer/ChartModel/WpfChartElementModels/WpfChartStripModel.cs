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
using System.Collections.Generic;
using System.Windows.Media;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartStripModel : WpfChartFontModel {
		public override IEnumerable<ChartModelElement> Children {
			get {
				return null;
			}
		}
		public Strip Strip {
			get {
				return (Strip)ChartElement;
			}
		}
		public object MaxLimit {
			get { return Strip.MaxLimit; }
			set {
				if (Strip.MaxLimit != value)
					Strip.MaxLimit = value;
				OnPropertyChanged("MaxLimit");
			}
		}
		public object MinLimit {
			get { return Strip.MinLimit; }
			set {
				if (Strip.MinLimit != value) {
					Strip.MinLimit = value;
					OnPropertyChanged("MinLimit");
				}
			}
		}
		public string AxisLabelText {
			get { return Strip.AxisLabelText; }
			set {
				if(Strip.AxisLabelText != value){
					Strip.AxisLabelText = value;
					OnPropertyChanged("AxisLabelText");
				}
			}
		}
		public string LegendText {
			get { return Strip.LegendText; }
			set {
				if (Strip.LegendText != value) {
					Strip.LegendText = value;
					OnPropertyChanged("LegendText");
				}
			}
		}
		public SolidColorBrush Brush {
			get { return Strip.Brush as SolidColorBrush; }
			set {
				if (Strip.Brush != value) {
					Strip.Brush = value;
					OnPropertyChanged("Brush");
				}
			}
		}
		public WpfChartStripModel(ChartModelElement parent, Strip strip)
			: base(parent, strip) {
				PropertyGridModel = new WpfChartStripPropertyGridModel((WpfChartModel)GetParent<WpfChartModel>(), this);
		}
	}
}
