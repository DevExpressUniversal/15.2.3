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
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartLineStylePropertyGridModel : NestedElementPropertyGridModelBase {
		readonly ICommand setPropertyCommand;
		readonly LineStyle lineStyle;
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		internal LineStyle LineStyle { get { return lineStyle; } }
		[Category(Categories.Common)]
		public int Thickness {
			get { return lineStyle.Thickness; }
			set {
				SetProperty("Thickness", value);
			}
		}
		[Category(Categories.Common)]
		public PenLineCap DashCap {
			get { return lineStyle.DashCap; }
			set {
				SetProperty("DashCap", value);
			}
		}
		[Category(Categories.Common)]
		public PenLineJoin LineJoin {
			get { return lineStyle.LineJoin; }
			set {
				SetProperty("LineJoin", value);
			}
		}
		[Category(Categories.Common)]
		public double MiterLimit {
			get { return lineStyle.MiterLimit; }
			set {
				SetProperty("MiterLimit", value);
			}
		}
		[Category(Categories.Common)]
		public DashStyle DashStyle {
			get { return lineStyle.DashStyle; }
			set { SetProperty("DashStyle", value); }
		}
		public WpfChartLineStylePropertyGridModel() : this(null, null, null, string.Empty) { }
		public WpfChartLineStylePropertyGridModel(ChartModelElement modelElement, LineStyle lineStyle, ICommand setPropertyCommand, string propertyPath) : base(modelElement, propertyPath) {
			this.lineStyle = lineStyle;
			this.setPropertyCommand = setPropertyCommand;
		}
	}
}
