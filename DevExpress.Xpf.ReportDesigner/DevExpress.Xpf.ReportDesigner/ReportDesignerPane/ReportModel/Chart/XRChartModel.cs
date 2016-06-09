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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public sealed class XRChartElementModelFactory : ModelFactory<XRChartModel, ChartElement, XRChartElementModelBase> {
		internal XRChartElementModelFactory(XRChartModel owner)
			: base(owner, x => x.XRObject) {
			Initialize(this, owner.Report.Owner.ModelRegistries);
		}
	}
	public class XRChartModel : XRControlModelBase<XRChart, XRChartDiagramItem> {
		XRChartElementModelFactory factory;
		public XRChartElementModelFactory Factory {
			get {
				if(factory == null)
					factory = new XRChartElementModelFactory(this);
				return factory;
			}
		}
		protected override void InitializeNewXRControl(XRChart xrChart, XtraReportModel model) {
			base.InitializeNewXRControl(xrChart, model);
			xrChart.Series.Add(new Series("1", ViewType.Bar));
		}
		protected internal XRChartModel(XRControlModelFactory.ISource<XRChart> source, ImageSource icon)
			: base(source, icon) {
			XRObject.Chart.ViewController.Update();
			XRObject.Chart.ViewController.OnEndLoading();
			var elements = ListAdapter<ChartElement>.FromTwoLists(
				ListAdapter<Series>.FromObjectList(XRObject.Series),
				ListAdapter<XtraCharts.Diagram>.FromObject(() => XRObject.Diagram, x => XRObject.Diagram = x, null));
			var chartTitles = ListAdapter<ChartTitle>.FromObjectList(XRObject.Titles);
			elements = ListAdapter<ChartElement>.FromTwoLists(chartTitles, elements);
			Elements = XRObjectModelCollection<XRChartElementModelBase>.Create(this, elements, Factory);
			IChartInteractionProvider chartInteractionProvider = XRObject;
			chartInteractionProvider.ObjectHotTracked += OnChartObjectHotTrackedOrSelected;
			chartInteractionProvider.ObjectSelected += OnChartObjectHotTrackedOrSelected;
		}
		void OnChartObjectHotTrackedOrSelected(object sender, HotTrackEventArgs e) {
			Report.DiagramItem.Diagram.InvalidateRenderLayer();
		}
		public ObservableCollection<XRChartElementModelBase> Elements { get; private set; }
		DiagramCursor? HighlightObjectAt(Point point) {
			IChartInteractionProvider chartInteractionProvider = XRObject;
			object hotTrackedObject = null;
			HotTrackEventHandler handler = (sender, e) => hotTrackedObject = e.Object;
			chartInteractionProvider.ObjectHotTracked += handler;
			try {
				XRObject.Chart.HighlightObjectsAt(new System.Drawing.Point((int)point.X, (int)point.Y));
			} finally {
				chartInteractionProvider.ObjectHotTracked -= handler;
			}
			return IsChartElement(hotTrackedObject) ? DiagramCursor.Default : (DiagramCursor?)null;
		}
		DiagramItem SelectObjectAt(Point point) {
			IChartInteractionProvider chartInteractionProvider = XRObject;
			ChartElement selectedObject = null;
			HotTrackEventHandler handler = (sender, e) => selectedObject = e.Object as ChartElement;
			chartInteractionProvider.ObjectSelected += handler;
			try {
				XRObject.Chart.SelectObjectsAt(new System.Drawing.Point((int)point.X, (int)point.Y));
			} finally {
				chartInteractionProvider.ObjectSelected -= handler;
			}
			return (IsChartElement(selectedObject) ? Factory.GetModel(selectedObject) : (XRModelBase)this).DiagramItem;
		}
		bool IsChartElement(object obj) {
			return obj != null && obj != XRObject;
		}
		public void UpdateSelection() {
			var selectedChartElement = (XRDiagramItemBase.GetDiagram(DiagramItem).PrimarySelection.With(x => GetXRModel(x)) as XRChartElementModelBase).With(x => x.Chart == this ? x.XRObject : null);
			if(selectedChartElement == null) {
				XRObject.Chart.SelectHitTestable(null);
				XRObject.Chart.SelectHitElement(null);
				Report.DiagramItem.Diagram.InvalidateRenderLayer();
			} else {
				XRObject.Chart.SelectHitTestable(selectedChartElement);
			}
		}
		protected override void AttachDiagramItem() {
			base.AttachDiagramItem();
			DiagramItem.ItemFactory = x =>  Factory.GetModel(x, true).DiagramItem;
			BindDiagramItems(Elements, DiagramItem.Items);
			DiagramItem.SelectItemAtCallback = SelectObjectAt;
			DiagramItem.HighlightItemAtCallback = HighlightObjectAt;
		}
	}
}
