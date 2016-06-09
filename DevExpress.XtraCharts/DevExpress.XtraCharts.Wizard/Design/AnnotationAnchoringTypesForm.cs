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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Design {
	public partial class AnnotationAnchoringTypesForm : ItemKindChoosingForm {
		#region inner class
		class AnchorPointItem : ListViewItem {
			readonly bool enabled;
			public bool Enabled { get { return enabled; } }
			public AnchorPointItem(string name, int imageIndex, string toolTipText, bool enabled) : base(name, imageIndex) {
				this.enabled = enabled;
				if (!enabled)
					this.ToolTipText = toolTipText;
			}
		}
		#endregion
		AnnotationAnchorPoint currentAnchorPoint;
		Annotation annotation;
		AnchorPointItem SelectedItem { 
			get {
				return listView.SelectedItems.Count > 0 ? listView.SelectedItems[0] as AnchorPointItem : null;
			} 
		}
		public AnnotationAnchorPoint EditValue { 
			get { return currentAnchorPoint; } 
			set { 
				currentAnchorPoint = value;
				annotation = ((IOwnedElement)value).Owner as Annotation;
				foreach (ListViewItem item in listView.Items)
					item.Selected = item.Text == StringResourcesUtils.GetStringId(currentAnchorPoint);
			} 
		}
		AnnotationAnchoringTypesForm() {
			InitializeComponent();
		}
		public AnnotationAnchoringTypesForm(Annotation annotation) : this() {
			AddItem(new ChartAnchorPoint(), String.Empty, true);
			AddItem(new PaneAnchorPoint(), ChartLocalizer.GetString(ChartStringId.IncorrectDiagramTypeToolTipText), 
				AnnotationHelper.IsPaneAnchorPointSupported(annotation));
			AddItem(new SeriesPointAnchorPoint(), ChartLocalizer.GetString(ChartStringId.IncorrectSeriesCollectionToolTipText), 
				AnnotationHelper.IsSeriesPointAnchorPointSupported(annotation));
		}
		void AddItem(AnnotationAnchorPoint anchorPoint, string toolTipText, bool enabled) {
			imageList.Images.Add(ImageResourcesUtils.GetImageFromResources(anchorPoint, enabled));
			listView.Items.Add(new AnchorPointItem(StringResourcesUtils.GetStringId(anchorPoint), imageList.Images.Count - 1, toolTipText, enabled));
		}
		protected override void UpdateButtons() { 
			btnOk.Enabled = SelectedItem != null && SelectedItem.Enabled;
		}
		protected override void CloseForm() {
			SeriesPointAnchorPoint anchorPoint = currentAnchorPoint as SeriesPointAnchorPoint;
			if (anchorPoint != null) {
				IChartContainer container = CommonUtils.FindChartContainer(annotation);
				using (SeriesPointListForm form = new SeriesPointListForm(container.Chart.Series)) {
					if (form.ShowDialog() != DialogResult.OK)
						return;
					anchorPoint.SeriesPoint = form.EditValue;
				}
			}
			DialogResult = DialogResult.OK;
		}
		protected override void SelectedIndexChanged() {
			if (listView.SelectedItems.Count > 0) {
				IChartContainer container = CommonUtils.FindChartContainer(annotation);
				AnnotationAnchorPoint anchorPoint = AnnotationHelper.CreateChartAnchorPoint(annotation, container.Chart);
				if (listView.SelectedItems[0].Text == StringResourcesUtils.GetStringId(anchorPoint)) {
					currentAnchorPoint = anchorPoint;
					return;
				}
				anchorPoint = AnnotationHelper.CreatePaneAnchorPoint(annotation, container.Chart);
				if (listView.SelectedItems[0].Text == StringResourcesUtils.GetStringId(anchorPoint)) {
					currentAnchorPoint = anchorPoint;
					return;
				}
				anchorPoint = new SeriesPointAnchorPoint();
				if (listView.SelectedItems[0].Text == StringResourcesUtils.GetStringId(anchorPoint))
					currentAnchorPoint = anchorPoint;
			}			
		}	 
	}
}
