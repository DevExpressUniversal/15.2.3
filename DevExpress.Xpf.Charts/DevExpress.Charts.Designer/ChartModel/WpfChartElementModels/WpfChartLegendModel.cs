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

using System.Windows.Controls;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartLegendModel : WpfChartFontModel {
		public Legend Legend { get { return (Legend)ChartElement; } }
		public VerticalPosition VerticalPosition {
			get { return Legend.VerticalPosition; }
			set {
				if (Legend.VerticalPosition != value) {
					Legend.VerticalPosition = value;
					OnPropertyChanged("VerticalPosition");
				}
			}
		}
		public HorizontalPosition HorizontalPosition {
			get { return Legend.HorizontalPosition; }
			set {
				if (Legend.HorizontalPosition != value) {
					Legend.HorizontalPosition = value;
					OnPropertyChanged("HorizontalPosition");
				}
			}
		}
		public bool ReverseItems {
			get { return Legend.ReverseItems; }
			set {
				if (Legend.ReverseItems != value) {
					Legend.ReverseItems = value;
					OnPropertyChanged("ReverseItems");
				}
			}
		}
		public Orientation Orientation {
			get { return Legend.Orientation; }
			set {
				if (Legend.Orientation != value) {
					Legend.Orientation = value;
					OnPropertyChanged("Orientation");
				}
			}
		}
		public WpfChartLegendModel(ChartModelElement parent, Legend legend) : base(parent, legend) {
			PropertyGridModel = new WpfChartLegendPropertyGridModel((WpfChartModel)parent, legend);
		}
	}
}
