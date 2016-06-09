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
	public class WpfChartScrollBarOptionsPropertyGridModel : NestedElementPropertyGridModelBase {
		readonly ScrollBarOptions scrollBarOptions;
		readonly ChartCommandBase command;
		protected internal ScrollBarOptions ScrollBarOptions { get { return scrollBarOptions; } }
		protected override ICommand SetObjectPropertyCommand { get { return command; } }
		[Category(Categories.Layout)]
		public ScrollBarAlignment Alignment {
			get { return ScrollBarOptions.Alignment; }
			set { SetProperty("Alignment", value); }
		}
		[Category(Categories.Appearance)]
		public double BarThickness {
			get { return ScrollBarOptions.BarThickness; }
			set { SetProperty("BarThickness", value); }
		}
		[Category(Categories.Behavior)]
		public bool Visible {
			get { return ScrollBarOptions.Visible; }
			set { SetProperty("Visible", value); }
		}
		public WpfChartScrollBarOptionsPropertyGridModel() : this(null, null, null, string.Empty) { 
		}
		public WpfChartScrollBarOptionsPropertyGridModel(ChartModelElement modelElement, ScrollBarOptions scrollBarOptions, ChartCommandBase command, string propertyPath)
			: base(modelElement, propertyPath) {
			this.scrollBarOptions = scrollBarOptions;
				this.command = command;
		}
	}
}
