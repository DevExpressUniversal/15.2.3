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
	public class WpfChartResolveOverlappingOptionsPropertyGridModel : PropertyGridModelBase {
		readonly SetResolveOverlappingOptionsPropertyCommand setPropertyCommand;
		readonly AxisLabelResolveOverlappingOptions resolveOverlappingOptions;
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		protected internal AxisLabelResolveOverlappingOptions ResolveOverlappingOptions { get { return resolveOverlappingOptions; } }
		[Category(Categories.Behavior)]
		public bool AllowHide {
			get { return ResolveOverlappingOptions.AllowHide; }
			set { SetProperty("AllowHide", value); }
		}
		[Category(Categories.Behavior)]
		public bool AllowRotate {
			get { return ResolveOverlappingOptions.AllowRotate; }
			set { SetProperty("AllowRotate", value); }
		}
		[Category(Categories.Behavior)]
		public bool AllowStagger {
			get { return ResolveOverlappingOptions.AllowStagger; }
			set { SetProperty("AllowStagger", value); }
		}
		[Category(Categories.Behavior)]
		public int MinIndent {
			get { return ResolveOverlappingOptions.MinIndent; }
			set { SetProperty("MinIndent", value); }
		}
		public WpfChartResolveOverlappingOptionsPropertyGridModel() : this(null, null) { }
		public WpfChartResolveOverlappingOptionsPropertyGridModel(ChartModelElement modelElement, AxisLabelResolveOverlappingOptions resolveOverlappingOptions)
			: base(modelElement) {
			this.resolveOverlappingOptions = resolveOverlappingOptions;
			this.setPropertyCommand = new SetResolveOverlappingOptionsPropertyCommand(ChartModel);
		}
	}
}
