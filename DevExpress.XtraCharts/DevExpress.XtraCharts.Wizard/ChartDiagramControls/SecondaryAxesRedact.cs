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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Wizard.ChartAxesControls;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class SecondaryAxesRedact : AddRemoveChartNamedElementControl {
		class ItemContainer : ChartElementNamed {
			Axis2D axis;
			public Axis2D Value { get { return axis; } }
			public ItemContainer(Axis2D axis) : base(axis.Name) {
				this.axis = axis;
			}
			public override string ToString() {
				return this.axis.Name;
			}
			protected override ChartElement CreateObjectForClone() {
				return new ItemContainer(axis);
			}
		}
		public override ChartElement CurrentElement {
			get { return ((ItemContainer)base.CurrentElement).Value; }
		}
		public SecondaryAxesRedact() {
			InitializeComponent();
		}
		protected override void FillComboBox() {
			this.cbElement.Properties.Items.Clear();
			foreach (Axis2D axis in Collection) {
				ItemContainer container = new ItemContainer(axis);
				this.cbElement.Properties.Items.Add(container);
			}
		}
		protected override int Add() {
			SecondaryAxisCollection collection = (SecondaryAxisCollection)Collection;
			Axis2D axis = ChartElementFactory.CreateNewAxis(collection);
			return CommonUtils.AddToSecondaryAxisCollection(collection, axis);
		}
	}
}
