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
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using System.ComponentModel;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartStripPropertyGridModel : PropertyGridModelBase {
		readonly Strip strip;
		readonly SetStripPropertyCommand setPropertyCommand;
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		[Category(Categories.Behavior)]
		public bool Visible {
			get { return strip.Visible; }
			set {
				SetProperty("Visible", value);
			}
		}
		[Category(Categories.Behavior)]
		public object MinLimit {
			get { return strip.MinLimit; }
			set {
				SetProperty("MinLimit", value);
			}
		}
		[Category(Categories.Behavior)]
		public object MaxLimit {
			get { return strip.MaxLimit; }
			set {
				SetProperty("MaxLimit", value);
			}
		}
		[Category(Categories.Brushes)]
		public Brush Brush {
			get { return strip.Brush; }
			set {
				SetProperty("Brush", value);
			}
		}
		[Category(Categories.Presentation)]
		public string AxisLabelText {
			get { return strip.AxisLabelText; }
			set {
				SetProperty("AxisLabelText", value);
			}
		}
		[Category(Categories.Presentation)]
		public string LegendText {
			get { return strip.LegendText; }
			set {
				SetProperty("LegendText", value);
			}
		}
		[Category(Categories.Presentation)]
		public Color BorderColor {
			get { return strip.BorderColor; }
			set {
				SetProperty("BorderColor", value);
			}
		}
		public WpfChartStripPropertyGridModel(WpfChartModel chartModel, WpfChartStripModel stripModel) : base(chartModel) {
			this.strip = stripModel.Strip;
			setPropertyCommand = new SetStripPropertyCommand(chartModel);
		}
	}
}
