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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class ModelCollectionEditorBase : ChartEditorBase {
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			ChartCollectionBaseModel collection = (ChartCollectionBaseModel)Value;
			Chart chart = ((IOwnedElement)collection.ChartCollection).ChartContainer.Chart;
			DesignerCollectionEditorForm form = CreateForm(collection);
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chart.Container.RenderProvider.LookAndFeel;
			form.Initialize(chart);
			return form;
		}
		protected abstract DesignerCollectionEditorForm CreateForm(ChartCollectionBaseModel collection);
	}
	public class CustomAxisLabelModelCollectionEditor : ModelCollectionEditorBase {
		protected override DesignerCollectionEditorForm CreateForm(ChartCollectionBaseModel collection) {
			return new CustomAxisLabelModelCollectionEditorForm((CustomAxisLabelCollectionModel)collection);
		}
	}
	public class CustomAxisLabelModelCollectionEditorForm : DesignerCollectionEditorForm {
		readonly CustomAxisLabelCollectionModel collection;
		protected override bool SelectableItems { get { return false; } }
		protected override object[] CollectionToArray {
			get {
				object[] itemsArray = new object[collection.Count];
				((ICollection)collection).CopyTo(itemsArray, 0);
				return itemsArray;
			}
		}
		public CustomAxisLabelModelCollectionEditorForm(CustomAxisLabelCollectionModel collection)
			: base() {
			this.collection = collection;
			collection.CommandManager.BeginTransaction();
		}
		protected override string GetItemDisplayText(int index) {
			return collection[index].Name;
		}
		protected override object[] AddItems() {
			collection.AddNewElement(null);
			int index = collection.Count - 1;
			return new object[] { collection[index] };
		}
		protected override void RemoveItem(object item) {
			collection.DeleteElement(((CustomAxisLabelModel)item).ChartElement);
		}
		protected override void Swap(int index1, int index2) {
			collection.Swap(index1, index2);
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			collection.CommandManager.CommitTransaction();
		}
	}
}
