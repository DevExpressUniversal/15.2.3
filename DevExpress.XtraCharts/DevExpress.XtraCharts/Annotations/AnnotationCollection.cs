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
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class AnnotationRepository : ChartElementNamedCollection, IAnnotationCollection {
		readonly Chart chart;
		protected override string NamePrefix { get { return ChartLocalizer.GetString(ChartStringId.AnnotationPrefix); } }
		public Annotation this[int index] { get { return (Annotation)List[index]; } }
		internal AnnotationRepository(Chart chart) : base() {
			this.chart = chart;
			Owner = chart;
		}
		#region IAnnotationCollection Members
		ChartElement IAnnotationCollection.Owner { get { return Owner; } }
		int IAnnotationCollection.Add(Annotation annotation) {
			chart.ArrangeAnnotation(annotation);
			return Add(annotation);
		}
		#endregion
		public int Add(Annotation annotation) {
			annotation.Owner = chart;
			return base.Add(annotation);
		}
		public void AddRange(Annotation[] coll) {
			foreach (Annotation item in coll)
				Add(item);
		}
		public bool Contains(Annotation annotation) {
			return base.Contains(annotation);
		}
		public bool HasSeriesPointAnchoredAnnotations() {
			for (int i = 0; i < Count; i++) {
				Annotation a = this[i];
				if ((a != null) && (a.AnchorPoint is SeriesPointAnchorPoint) && (((SeriesPointAnchorPoint)a.AnchorPoint).SeriesPoint != null))
					return true;
			}
			return false;
		}
		internal bool Contains(object obj) {
			Annotation annotation = obj as Annotation;
			return annotation != null && Contains(annotation);
		}
		internal void RemoveWithoutChanged(Annotation annotation) {
			base.RemoveWithoutChanged(annotation);
		}
		public int IndexOf(Annotation annotation) {
			return base.IndexOf(annotation);
		}
		public void Insert(int index, Annotation annotation) {
			base.Insert(index, annotation);
		}
		public void Remove(Annotation annotation) {
			base.Remove(annotation);
		}
		public void Swap(Annotation annotation1, Annotation annotation2) {
			base.Swap(annotation1, annotation2);
		}		
		public new Annotation[] ToArray() {
			return (Annotation[])InnerList.ToArray(typeof(Annotation));
		}		
		internal IList<AnnotationViewData> CreateViewData(DiagramViewData diagramViewData, ZPlaneRectangle innerBounds, TextMeasurer textMeasurer) {
			List<AnnotationLayout> anchorPointsLayout = new List<AnnotationLayout>();
			foreach (Annotation annotation in this) {
				ChartAnchorPoint anchorPoint = annotation.AnchorPoint as ChartAnchorPoint;
				if (anchorPoint != null)
					anchorPointsLayout.Add(new AnnotationLayout(annotation, anchorPoint.GetAnchorPoint(innerBounds)));
			}
			Dictionary<Annotation, AnnotationLayout> shapesLayout = new Dictionary<Annotation, AnnotationLayout>();
			foreach (AnnotationLayout shapeLayout in AnnotationHelper.CreateFreAnnotationsShapesLayout(this, null, innerBounds))
				shapesLayout.Add(shapeLayout.Annotation, shapeLayout);
			if (diagramViewData != null) {
				if (diagramViewData.AnnotationsAnchorPointsLayout != null)
					anchorPointsLayout.AddRange(diagramViewData.AnnotationsAnchorPointsLayout);
				if (diagramViewData.AnnotationsShapesLayout != null)
					foreach (AnnotationLayout layout in diagramViewData.AnnotationsShapesLayout)
						if (!shapesLayout.ContainsKey(layout.Annotation))
							shapesLayout.Add(layout.Annotation, layout);
			}
			List<AnnotationViewData> annotationsViewData = new List<AnnotationViewData>();
			foreach (AnnotationLayout anchorPointLayout in anchorPointsLayout) {
				Annotation annotation = anchorPointLayout.Annotation;
				RelativePosition shapePosition = annotation.ShapePosition as RelativePosition;
				AnnotationLayout shapeLayout = null;
				if (shapePosition != null)
					shapeLayout = new AnnotationLayout(annotation, shapePosition.GetShapeLocation(anchorPointLayout.Position));
				else {
					if (!shapesLayout.ContainsKey(annotation))
						continue;
					shapeLayout = shapesLayout[annotation];
				}
				AnnotationViewData viewData = annotation.CalculateViewData(textMeasurer, shapeLayout, anchorPointLayout);
				if (viewData != null)
					annotationsViewData.Add(viewData);
			}
			annotationsViewData.Sort(AnnotationHelper.CompareByAnnotationZOrder);
			return annotationsViewData;
		}
	}
	public class AnnotationCollection : ChartCollectionBase, IAnnotationCollection {
		bool repositoryUpdateLocked = false;
		AnnotationCollectionBehavior collectionBehavior;
		AnnotationRepository AnnotationRepository { get { return collectionBehavior.AnnotationRepository; } }
		public Annotation this[int index] { get { return (Annotation)List[index]; } }
		internal AnnotationCollection(AnnotationCollectionBehavior collectionBehavior) : base() {
			this.collectionBehavior = collectionBehavior;
			Owner = collectionBehavior.Owner;
		}
		#region IAnnotationCollection Members
		ChartElement IAnnotationCollection.Owner { get { return collectionBehavior.Owner; } }
		string IAnnotationCollection.GenerateName(string namePrefix) {
			return AnnotationRepository != null ? AnnotationRepository.GenerateName(namePrefix) : String.Empty;
		}
		int IAnnotationCollection.Add(Annotation annotation) {
			return Add(annotation);
		}
		Annotation[] IAnnotationCollection.ToArray() {
			Update();
			return ToArray();
		}		
		#endregion
		int Add(Annotation annotation) {
			collectionBehavior.ArrangeAnnotation(annotation);
			if (AnnotationRepository != null)
				AnnotationRepository.Add(annotation);
			if (Contains(annotation))
				return IndexOf(annotation);
			return base.AddWithoutChanged(annotation);
		}
		protected internal override void SendControlChanging() {
		}
		protected override void ChangeOwnerForItem(ChartElement item) {		   
		}
		protected override void Dispose(bool disposing) {			
		}
		protected override void DisposeItem(ChartElement item) {			
		}
		protected override void DisposeItemBeforeRemove(ChartElement item) {
		}
		protected internal override void ProcessChanged(ChartUpdateInfoBase changeInfo) {	   
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			if (!repositoryUpdateLocked && AnnotationRepository != null && value is Annotation)
				AnnotationRepository.Remove(value as Annotation);
		}
		protected override void OnClear() {
			if (!repositoryUpdateLocked && AnnotationRepository != null)
				for (int i = 0; i < Count; i++)
					AnnotationRepository.RemoveWithoutChanged(this[i]);
			base.OnClear();
		}
		internal void Update() {
			if (AnnotationRepository != null) {
				try {
					repositoryUpdateLocked = true;
					Clear();
					foreach (Annotation annotation in AnnotationRepository.ToArray()) {
						if (collectionBehavior.CheckAnnotation(annotation) && !Contains(annotation))
							base.AddWithoutChanged(annotation);
					}
				}
				finally {
					repositoryUpdateLocked = false;
				}
			}
		}
		internal void UpdateAnnotationRepository() {
			if (AnnotationRepository != null && Count > 0) {
				for (int i = 0; i < Count; i++) {
					Annotation annotation = this[i];
					AnnotationRepository.BeginUpdate();
					if (!AnnotationRepository.Contains(annotation))
						AnnotationRepository.Add(annotation);
					AnnotationRepository.EndUpdate();
				}
				try {
					repositoryUpdateLocked = true;
					Clear();
				}
				finally {
					repositoryUpdateLocked = false;
				}
			}
		}
		internal void ClearAnnotations() {
			if (AnnotationRepository != null) {
				foreach (Annotation annotation in this)
					AnnotationRepository.RemoveWithoutChanged(annotation);
			}
		}
		public TextAnnotation AddTextAnnotation() {
			return AddTextAnnotation(string.Empty);
		}
		public TextAnnotation AddTextAnnotation(string name) {
			TextAnnotation annotation = new TextAnnotation(name);
			Add(annotation);
			return annotation;
		}
		public TextAnnotation AddTextAnnotation(string name, string text) {
			TextAnnotation annotation = new TextAnnotation(name, text);
			Add(annotation);
			return annotation;
		}
		public ImageAnnotation AddImageAnnotation() {
			return AddImageAnnotation(string.Empty);
		}
		public ImageAnnotation AddImageAnnotation(string name)  {
			return AddImageAnnotation(name, string.Empty);
		}
		public ImageAnnotation AddImageAnnotation(string name, string imageUrl) {
			ImageAnnotation annotation = new ImageAnnotation(name, imageUrl);
			Add(annotation);
			return annotation;
		}
		public ImageAnnotation AddImageAnnotation(string name, Image image) {
			ImageAnnotation annotation = new ImageAnnotation(name, image);
			Add(annotation);
			return annotation;
		}		
		public bool Contains(Annotation annotation) {
			return base.Contains(annotation);
		}
		public int IndexOf(Annotation annotation) {
			return base.IndexOf(annotation);
		}
		public void Remove(Annotation annotation) {
			base.Remove(annotation);
			if (AnnotationRepository != null)
				AnnotationRepository.Remove(annotation);
		}
		public void Swap(Annotation annotation1, Annotation annotation2) {
			base.Swap(annotation1, annotation2);
			if (AnnotationRepository != null)
				AnnotationRepository.Swap(annotation1, annotation2);
		}
		public new void Swap(int index1, int index2) {
			if (index1 < 0 || index1 >= Count)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgItemNotInCollection), "index1");
			if (index2 < 0 || index2 >= Count)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgItemNotInCollection), "index2");
			if (AnnotationRepository != null)
				AnnotationRepository.Swap(this[index1], this[index2]);
			base.Swap(index1, index2);
		}
		public new Annotation[] ToArray() {
			return (Annotation[])InnerList.ToArray(typeof(Annotation));
		}		
	}
}
namespace DevExpress.XtraCharts.Native {
	public interface IAnnotationCollection {
		Annotation this[int index] { get; }
		ChartElement Owner { get; }
		int Add(Annotation annotation);
		string GenerateName(string namePrefix);
		Annotation[] ToArray(); 
		void Swap(int index1, int index2);
		void Remove(Annotation annotation);
	}
	public abstract class AnnotationCollectionBehavior {
		readonly ChartElement owner;
		public Chart Chart { get { return owner.ChartContainer != null ? owner.ChartContainer.Chart : null; } }
		public AnnotationRepository AnnotationRepository { get { return Chart != null ? Chart.AnnotationRepository : null; } }
		public ChartElement Owner { get { return owner; } }
		public AnnotationCollectionBehavior(ChartElement owner) {
			this.owner = owner;
		}
		public abstract void ArrangeAnnotation(Annotation annotation);
		public abstract bool CheckAnnotation(Annotation annotation);
	}
	public class AnnotationCollectionChartBehavior : AnnotationCollectionBehavior {
		readonly Chart chart;
		public AnnotationCollectionChartBehavior(Chart chart) : base (chart) {
			this.chart = chart;
		}
		public override void ArrangeAnnotation(Annotation annotation) {
			chart.ArrangeAnnotation(annotation);
		}
		public override bool CheckAnnotation(Annotation annotation) {
			return annotation.AnchorPoint is ChartAnchorPoint;
		}
	}
	public class AnnotationCollectionPaneBehavior : AnnotationCollectionBehavior {
		readonly XYDiagramPaneBase pane;
		public AnnotationCollectionPaneBehavior(XYDiagramPaneBase pane) : base(pane) {
			this.pane = pane;
		}
		public override void ArrangeAnnotation(Annotation annotation) {
			annotation.ShapePosition = new RelativePosition();
			PaneAnchorPoint anchorPoint = new PaneAnchorPoint();
			anchorPoint.Pane = pane;
			if (pane.Diagram != null) {
				PaneAxesContainer paneAxesData = pane.Diagram.GetPaneAxesData(pane);
				if (paneAxesData != null) {
					Axis2D axisX = (Axis2D)paneAxesData.PrimaryAxisX;
					Axis2D axisY = (Axis2D)paneAxesData.PrimaryAxisY;
					if (axisX != null) {
						anchorPoint.AxisXCoordinate.Axis = axisX;
						anchorPoint.AxisXCoordinate.AxisValue = ((IAxisData)axisX).AxisScaleTypeMap.InternalToNative((axisX.VisualRangeData.Min + axisX.VisualRangeData.Max) / 2);
					}
					if (axisY != null) {
						anchorPoint.AxisYCoordinate.Axis = axisY;
						anchorPoint.AxisYCoordinate.AxisValue = ((IAxisData)axisY).AxisScaleTypeMap.InternalToNative((axisY.VisualRangeData.Min + axisY.VisualRangeData.Max) / 2);
					}
				}
			}
			annotation.AnchorPoint = anchorPoint;
		}
		public override bool CheckAnnotation(Annotation annotation) {
			PaneAnchorPoint anchorPoint = annotation.AnchorPoint as PaneAnchorPoint;
			if (anchorPoint != null && Object.ReferenceEquals(anchorPoint.Pane, pane))
				return true;
			return false;
		}
	}
	public class AnnotationCollectionSeriesPointBehavior : AnnotationCollectionBehavior {
		readonly SeriesPoint point;
		IEnumerable<SeriesPoint> Points {
			get {
				yield return point;
				if (point.SourcePoints != null)
					foreach (SeriesPoint item in point.SourcePoints)
						yield return item;
			}
		}
		public AnnotationCollectionSeriesPointBehavior(SeriesPoint point) : base(point) {
			this.point = point;
		}
		public override void ArrangeAnnotation(Annotation annotation) {
			annotation.ShapePosition = new RelativePosition();
			SeriesPointAnchorPoint anchorPoint = new SeriesPointAnchorPoint();
			anchorPoint.SetSeriesPoint(point);
			annotation.AnchorPoint = anchorPoint;
		}
		public override bool CheckAnnotation(Annotation annotation) {
			SeriesPointAnchorPoint anchorPoint = annotation.AnchorPoint as SeriesPointAnchorPoint;
			if (anchorPoint != null) {
				foreach (SeriesPoint item in Points)
					if (Object.ReferenceEquals(anchorPoint.SeriesPoint, item))
						return true;
			}
			return false;
		}
	}
}
