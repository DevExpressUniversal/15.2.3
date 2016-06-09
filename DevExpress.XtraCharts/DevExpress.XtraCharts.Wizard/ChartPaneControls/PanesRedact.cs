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

namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class PanesRedact : AddRemoveChartNamedElementControl {
		class ItemContainer : ChartElementNamed {
			XYDiagramPane pane;
			public XYDiagramPane Pane { get { return pane; } }
			public ItemContainer(XYDiagramPane pane) : base(pane.Name) {
				this.pane = pane;
			}
			public override string ToString() {
				return this.pane.Name;
			}
			protected override ChartElement CreateObjectForClone() {
				return new ItemContainer(pane);
			}
		}
		public override ChartElement CurrentElement {
			get { return ((ItemContainer)base.CurrentElement).Pane; }
		}
		public PanesRedact() {
			InitializeComponent();
		}
		protected override void FillComboBox() {
			this.cbElement.Properties.Items.Clear();
			foreach(XYDiagramPane pane in Collection) {
				ItemContainer container = new ItemContainer(pane);
				this.cbElement.Properties.Items.Add(container);
			}
		}
		protected override int Add() {
			XYDiagramPaneCollection collection = (XYDiagramPaneCollection)Collection;
			return collection.Add(new XYDiagramPane(collection.GenerateName()));
		}
	}
}
