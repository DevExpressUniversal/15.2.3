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
using System.Windows.Input;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartNavigationOptionsPropertyGridModel : WpfChartElementPropertyGridModelBase {
		readonly NavigationOptionsBase navigationOptions;
		protected internal NavigationOptionsBase NavigationOptions { get { return navigationOptions; } }
		[Category(Categories.Navigation)]
		public bool UseKeyboard {
			get { return NavigationOptions.UseKeyboard; }
			set {
				SetProperty("UseKeyboard", value);
			}
		}
		[Category(Categories.Navigation)]
		public bool UseMouse {
			get { return NavigationOptions.UseMouse; }
			set {
				SetProperty("UseMouse", value);
			}
		}
		[Category(Categories.Navigation)]
		public bool UseTouchDevice {
			get { return NavigationOptions.UseTouchDevice; }
			set {
				SetProperty("UseTouchDevice", value);
			}
		}
		public WpfChartNavigationOptionsPropertyGridModel() : this(null, null) { 
		}
		public WpfChartNavigationOptionsPropertyGridModel(WpfChartModel chartModel, NavigationOptionsBase navigationOptions) : base(chartModel, "Diagram.NavigationOptions.") {
			this.navigationOptions = navigationOptions;
		}
	}
	public class WpfChartXYNavigationOptionsPropertyGridModel : WpfChartNavigationOptionsPropertyGridModel {
		new NavigationOptions NavigationOptions { get { return base.NavigationOptions as NavigationOptions; } }
		[Category(Categories.Navigation)]
		public bool UseScrollBars {
			get { return NavigationOptions.UseScrollBars; }
			set {
				SetProperty("UseScrollBars", value);
			}
		}
		[Category(Categories.Navigation)]
		public double AxisXMaxZoomPercent {
			get { return NavigationOptions.AxisXMaxZoomPercent; }
			set {
				SetProperty("AxisXMaxZoomPercent", value);
			}
		}
		[Category(Categories.Navigation)]
		public double AxisYMaxZoomPercent {
			get { return NavigationOptions.AxisYMaxZoomPercent; }
			set {
				SetProperty("AxisYMaxZoomPercent", value);
			}
		}
		public WpfChartXYNavigationOptionsPropertyGridModel() : this(null, null) { 
		}
		public WpfChartXYNavigationOptionsPropertyGridModel(WpfChartModel chartModel, NavigationOptionsBase navigationOptions) : base(chartModel, navigationOptions) {
		}
	}
}
