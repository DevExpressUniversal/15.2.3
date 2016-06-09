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
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public class TaskLinkCollectionEditorForm : CollectionEditorForm {
		class SeriesPointItem {
			readonly SeriesPoint point;
			public string Argument { get { return point.Argument; } }
			public string Value1 { get { return point.GetValueString(0); } }
			public string Value2 { get { return point.GetValueString(1); } }
			public SeriesPointItem(SeriesPoint point) {
				this.point = point;
			}
			public override string ToString() {
				return "(SeriesPoint)";
			}
		}
		class TaskLinkItem {
			readonly TaskLink taskLink;
			internal TaskLink Link { get { return taskLink; } }
			public TaskLinkType LinkType { 
				get { return taskLink.LinkType; } 
				set { taskLink.LinkType = value; }
			}
			[TypeConverter(typeof(ExpandableObjectConverter))]
			public SeriesPointItem ParentPoint { get { return new SeriesPointItem(taskLink.ParentPoint); } }
			[TypeConverter(typeof(ExpandableObjectConverter))]
			public SeriesPointItem ChildPoint { get { return new SeriesPointItem(taskLink.ChildPoint); } }
			public TaskLinkItem(TaskLink taskLink) {
				this.taskLink = taskLink;
			}
			public override string ToString() {
				return "(TaskLink)";
			}
		}
		private LabelControl labelControlHeader1;
		readonly SeriesPointRelationCollection relations;
		protected override bool SelectableItems { get { return true; } }
		protected override object[] CollectionToArray { 
			get { 
				List<TaskLinkItem> list = new List<TaskLinkItem>();
				foreach (TaskLink taskLink in relations)
					list.Add(new TaskLinkItem(taskLink));
				return list.ToArray();
			} 
		}
		TaskLinkCollectionEditorForm() {
			InitializeComponent();
			btnUp.Visible = false;
			btnDown.Visible = false;
		}
		public TaskLinkCollectionEditorForm(SeriesPointRelationCollection relations) : this() {
			this.relations = relations;
		}
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskLinkCollectionEditorForm));
			this.labelControlHeader1 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.fListBox)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.labelControlHeader1, "labelControlHeader1");
			this.labelControlHeader1.Name = "labelControlHeader1";
			resources.ApplyResources(this, "$this");
			this.Name = "TaskLinkCollectionEditorForm";
			((System.ComponentModel.ISupportInitialize)(this.fListBox)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override string GetItemDisplayText(int index) {
			return relations[index].ChildPoint.Argument;
		}
		protected override object[] AddItems() {
			SeriesPoint parentPoint = ((IOwnedElement)relations).Owner as SeriesPoint;
			using (PointsListForm form = new PointsListForm(parentPoint)) {
				if (form.ShowDialog() == DialogResult.OK) {
					object[] points = form.GetSelectedPoints();
					List<TaskLinkItem> list = new List<TaskLinkItem>();
					foreach (SeriesPoint point in points) {
						TaskLink link = new TaskLink(point);
						relations.Add(link);
						list.Add(new TaskLinkItem(link));
					}
					return list.ToArray();
				}
			}
			return null;
		}
		protected override void RemoveItem(object item) {
			relations.Remove(((TaskLinkItem)item).Link);
		}
		protected override void Swap(int index1, int index2) {
			relations.Swap(index1, index2);
		}
	}
}
