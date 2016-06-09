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
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public partial class IndicatorsCollectionForm : CollectionEditorForm {
		readonly IndicatorCollection collection;
		protected override object[] CollectionToArray { get { return collection.ToArray(); } }
		protected override bool SelectableItems { get { return true; } }
		IndicatorsCollectionForm() {
			InitializeComponent();
		}
		public IndicatorsCollectionForm(IndicatorCollection collection) : this() {
			this.collection = collection;
		}
		protected override void Swap(int index1, int index2) {
			collection.Swap(index1, index2);
		}
		protected override object[] AddItems() {
			using (IndicatorTypeForm indicatorTypeForm = new IndicatorTypeForm(new SideBySideBarSeriesView())) {
				if (indicatorTypeForm.ShowDialog() == DialogResult.OK && indicatorTypeForm.Indicator != null) {
					Indicator indicator = indicatorTypeForm.Indicator;
					if (indicator != null) {
						indicator.Name = collection.GenerateName();
						collection.Add(indicator);
						return new object[] { indicator };
					}
				}
				return null;
			}
		}
		protected override void RemoveItem(object item) {
			collection.Remove((Indicator)item);
		}
		protected override string GetItemDisplayText(int index) {
			return collection[index].Name;
		}
	}
}
