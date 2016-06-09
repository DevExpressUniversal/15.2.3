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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
	public class BandHeaderAdorner : Button {
		public static readonly DependencyProperty BandKindProperty;
		public static readonly DependencyProperty BandNameProperty;
		public static readonly DependencyProperty BandNestingLevelProperty;
		public static readonly DependencyProperty MaxBandNestingLevelProperty;
		public static readonly DependencyProperty HeaderLevelWidthProperty;
		public static readonly DependencyProperty BandPanelGapProperty;
		public static readonly DependencyProperty IsBandSelectedProperty;
		public static readonly DependencyProperty DiagramItemProperty;
		static BandHeaderAdorner() {
			DependencyPropertyRegistrator<BandHeaderAdorner>.New()
				.Register(d => d.BandKind, out BandKindProperty, BandKind.Detail, d => d.OnBandKindChanged())
				.Register(d => d.BandName, out BandNameProperty, "")
				.Register(d => d.BandNestingLevel, out BandNestingLevelProperty, -1)
				.Register(d => d.MaxBandNestingLevel, out MaxBandNestingLevelProperty, -1)
				.Register(d => d.HeaderLevelWidth, out HeaderLevelWidthProperty, 5.0)
				.Register(d => d.BandPanelGap, out BandPanelGapProperty, 0.0)
				.Register(d => d.IsBandSelected, out IsBandSelectedProperty, false)
				.Register(d => d.DiagramItem, out DiagramItemProperty, null)
				.OverrideDefaultStyleKey();
		}
		public BandHeaderAdorner(BandDiagramItem diagramItem) {
			diagramItem.GetXRDiagram().BindRenderLayerRootAdornerElement(this);
			AdornerLayer.SetForceBoundsRounding(this, true);
			DiagramItem = diagramItem;
			SetBinding(IsBandSelectedProperty, new Binding(Diagram.DiagramItem.IsSelectedProperty.Name) { Source = diagramItem, Mode = BindingMode.OneWay });
			DiagramInput.SetInputElementFactory(this, () => new BandDiagramItem.BandDiagramItemInputElement(diagramItem));
		}
		protected override void OnClick() {
			base.OnClick();
			var diagramItem = DiagramItem;
			if(diagramItem.IsSelected) {
				diagramItem.Collapsed = !diagramItem.Collapsed;
			}
			diagramItem.GetXRDiagram().SelectItem(diagramItem);
		}
		public double BandPanelGap {
			get { return (double)GetValue(BandPanelGapProperty); }
			set { SetValue(BandPanelGapProperty, value); }
		}
		public BandKind BandKind {
			get { return (BandKind)GetValue(BandKindProperty); }
			set { SetValue(BandKindProperty, value); }
		}
		public string BandName {
			get { return (string)GetValue(BandNameProperty); }
			set { SetValue(BandNameProperty, value); }
		}
		void OnBandKindChanged() {
		}
		public int BandNestingLevel {
			get { return (int)GetValue(BandNestingLevelProperty); }
			set { SetValue(BandNestingLevelProperty, value); }
		}
		public int MaxBandNestingLevel {
			get { return (int)GetValue(MaxBandNestingLevelProperty); }
			set { SetValue(MaxBandNestingLevelProperty, value); }
		}
		public double HeaderLevelWidth {
			get { return (double)GetValue(HeaderLevelWidthProperty); }
			set { SetValue(HeaderLevelWidthProperty, value); }
		}
		public bool IsBandSelected {
			get { return (bool)GetValue(IsBandSelectedProperty); }
			set { SetValue(IsBandSelectedProperty, value); }
		}
		public BandDiagramItem DiagramItem {
			get { return (BandDiagramItem)GetValue(DiagramItemProperty); }
			set { SetValue(DiagramItemProperty, value); }
		}
	}
}
