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

using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraCharts.Designer.Native {
	public class AnchorPointSeriesPointModelEditor : ChartEditorBase {
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			SeriesPointAnchorPointModel anchorPointModel = Instance as SeriesPointAnchorPointModel;
			if(anchorPointModel == null)
				return null;
			DesignerChartModel chartModel = anchorPointModel.FindParent<DesignerChartModel>();
			if(chartModel == null)
				return null;
			SeriesCollectionModel seriesModel = chartModel.Series;
			if(seriesModel == null)
				return null;
			AnchorPointSeriesPointModelForm form = new AnchorPointSeriesPointModelForm(seriesModel);
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chartModel.Chart.Container.RenderProvider.LookAndFeel;
			SeriesPointModel point = Value as SeriesPointModel;
			if(point != null)
				form.EditValue = point;
			return form;
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			if(dialogResult == DialogResult.OK) {
				SeriesPointModel newPoint = ((AnchorPointSeriesPointModelForm)form).EditValue;
				if(newPoint != null)
					Value = newPoint;
			}
		}
	}
	public class AnchorPointSeriesPointModelForm : SeriesPointListForm {
		readonly SeriesCollectionModel collectionModel;
		SeriesPointModel point;
		bool isInitialized = false;
		protected override bool IsInitialized { get { return isInitialized; } }
		protected override bool IsPointSelected { get { return point != null; }}
		public new SeriesPointModel EditValue {
			get { return point; }
			set {
				point = value;
				UpdateControls();
			}
		}
		public AnchorPointSeriesPointModelForm(SeriesCollectionModel collection)
			: base(collection.SeriesCollection) {
			this.collectionModel = collection;
			isInitialized = true;
			InitSeriesListBox();
		}
		protected override object GetSeriesObject(int index) {
			return collectionModel[index];
		}
		protected override object GetPointObject(int seriesIndex, int pointIndex) {
			return collectionModel[seriesIndex].Points[pointIndex];
		}
		protected override Series GetDrawnSeries(object item) {
			return ((DesignerSeriesModel)item).Series;
		}
		protected override void SetPointValue(object value) {
			this.point = value as SeriesPointModel;
		}
	}
}
