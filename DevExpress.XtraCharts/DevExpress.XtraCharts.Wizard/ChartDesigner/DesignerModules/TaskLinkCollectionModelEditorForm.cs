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

using DevExpress.XtraCharts.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class TaskLinkCollectionModelEditorForm : TaskLinkCollectionEditorForm {
		readonly SeriesPointRelationCollectionModel relations;
		protected override object[] CollectionToArray {
			get {
				object[] array = new object[relations.Count];
				for(int i = 0; i < array.Length; i++)
					array[i] = relations[i];
				return array;
			}
		}
		public TaskLinkCollectionModelEditorForm(SeriesPointRelationCollectionModel relations)
			: base((SeriesPointRelationCollection)relations.ChartCollection) {
			this.relations = relations;
		}
		protected override string GetItemDisplayText(int index) {
			return relations[index].ChildPoint.SeriesPointModel.Argument;
		}
		protected override object[] AddItems() {
			SeriesPointModel parentPoint = relations.Parent as SeriesPointModel;
			using(PointsModelListForm form = new PointsModelListForm(parentPoint)) {
				if(form.ShowDialog() == DialogResult.OK) {
					object[] points = form.GetSelectedPoints();
					List<TaskLinkModel> list = new List<TaskLinkModel>();
					foreach(SeriesPointModel point in points) {
						TaskLink link = new TaskLink((SeriesPoint)point.ChartElement);
						TaskLinkModel linkModel = new TaskLinkModel(link, parentPoint.ChartElement as SeriesPoint, parentPoint.CommandManager);
						relations.AddNewElement(point.ChartElement);
						list.Add(linkModel);
					}
					return list.ToArray();
				}
			}
			return null;
		}
		protected override void RemoveItem(object item) {
			relations.DeleteElement(((TaskLinkModel)item).ChartElement);
		}
		protected override void Swap(int index1, int index2) {
			relations.Swap(index1, index2);
		}
	}
	public class PointsModelListForm : PointsListForm {
		class SeriesPointItem : ListViewItem {
			readonly SeriesPointModel point;
			public SeriesPointModel Point { get { return point; } }
			public SeriesPointItem(SeriesPointModel point) {
				this.point = point;
				Text = point.Argument;
				for(int i = 0; i < point.Values.Length; i++)
					SubItems.Add(((SeriesPoint)point.ChartElement).GetValueString(i));
			}
		}
		readonly SeriesPointModel parentPoint;
		protected override bool DelayedUpdate { get { return true; } }
		public PointsModelListForm(SeriesPointModel parentPoint)
			: base((SeriesPoint)parentPoint.ChartElement) {
			this.parentPoint = parentPoint;
			UpdateForm();
		}
		protected override object GetSeriesObject() {
			return parentPoint.FindParent<DesignerSeriesModelBase>();
		}
		protected override SeriesViewBase GetSeriesView(object seriesObject) {
			return ((DesignerSeriesModelBase)seriesObject).SeriesBase.View;
		}
		protected override ListViewItem CreateItem(object point) {
			return new SeriesPointItem((SeriesPointModel)point);
		}
		protected override IEnumerable<object> EnumeratePoints(object seriesObject) {
			foreach(SeriesPointModel point in ((DesignerSeriesModel)seriesObject).Points)
				yield return point;
		}
		protected override bool IsPointInRelations(object point) {
			return ((SeriesPointRelationCollection)parentPoint.Relations.ChartCollection).GetByChildSeriesPoint((SeriesPoint)((SeriesPointModel)point).ChartElement) == null;
		}
		protected override bool IsParentPoint(object point) {
			return ((SeriesPoint)((SeriesPointModel)point).ChartElement).SeriesPointID == ((SeriesPoint)parentPoint.ChartElement).SeriesPointID;
		}
		protected override object GetPoint(ListViewItem item) {
			return ((SeriesPointItem)item).Point;
		}
	}
}
