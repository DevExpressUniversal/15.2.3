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

using DevExpress.Diagram.Core;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Diagram.Native;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
	public class XRDiagramRootMarginsAdorner : Control {
		public static readonly DependencyProperty AdornerLeftMarginProperty;
		public static readonly DependencyProperty AdornerRightMarginProperty;
		static XRDiagramRootMarginsAdorner() {
			DependencyPropertyRegistrator<XRDiagramRootMarginsAdorner>.New()
				.Register(d => d.AdornerLeftMargin, out AdornerLeftMarginProperty, 0.0)
				.Register(d => d.AdornerRightMargin, out AdornerRightMarginProperty, 0.0)
				.OverrideDefaultStyleKey()
			;
		}
		public XRDiagramRootMarginsAdorner(XRDiagramControl diagram) {
			diagram.BindRenderLayerRootAdornerElement(this);
			AdornerLayer.SetForceBoundsRounding(this, true);
		}
		public double AdornerLeftMargin {
			get { return (double)GetValue(AdornerLeftMarginProperty); }
			set { SetValue(AdornerLeftMarginProperty, value); }
		}
		public double AdornerRightMargin {
			get { return (double)GetValue(AdornerRightMarginProperty); }
			set { SetValue(AdornerRightMarginProperty, value); }
		}
	}
}
