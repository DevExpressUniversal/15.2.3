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

using DevExpress.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Diagram.Core.Serialization;
namespace DevExpress.Diagram.Core {
	public interface ISelectionPartAdorner {
	}
	public interface ISelectionAdorner {
		bool CanResize { get; set; }
		bool CanRotate { get; set; }
		bool ShowFullUI { get; set; }
	}
	[CustomSerializableObject("DefaultSelectionLayerId")]
	public sealed class DefaultSelectionLayer : ISelectionLayer {
		public static readonly ISelectionLayer Instance = new DefaultSelectionLayer();
		DefaultSelectionLayer() { }
		ISelectionLayerHandler ISelectionLayer.CreateSelectionHandler(IDiagramControl diagram) {
			return new DefaultSelectionLayerHandler(diagram);
		}
	}
	public sealed class SelectionPartsLayerHandler : ISelectionLayerHandler {
		readonly IDiagramControl diagram;
		readonly int minSelectionCount;
		readonly Func<IDiagramItem, bool, IAdornerFactory, IUpdatableAdorner> adornerFactory;
		IEnumerable<IDiagramItem> SelectedItems { get { return diagram.SelectedItems(); } }
		List<IUpdatableAdorner> selectionPartAdorners = new List<IUpdatableAdorner>();
		internal List<IUpdatableAdorner> Adorners { get { return selectionPartAdorners; } }
		public SelectionPartsLayerHandler(IDiagramControl diagram, int minSelectionCount, Func<IDiagramItem, bool, IAdornerFactory, IUpdatableAdorner> adornerFactory) {
			this.diagram = diagram;
			this.minSelectionCount = minSelectionCount;
			this.adornerFactory = adornerFactory;
		}
		void ISelectionLayerHandler.UpdateSelectionAdorners() {
			SelectedItems.ForEach(selectionPartAdorners, (item, adorner) => {
				adorner.Adorner.SetBounds(item);
				adorner.Update();
			});
		}
		void ISelectionLayerHandler.RecreateSelectionAdorners() {
			DestroyAdorners();
			if(SelectedItems.Count() >= minSelectionCount) {
				var newAdorners = SelectedItems.Select(x => adornerFactory(x, x == diagram.PrimarySelection(), diagram.AdornerFactory()));
				selectionPartAdorners.AddRange(newAdorners);
			}
		}
		void ISelectionLayerHandler.Clear() {
			DestroyAdorners();
		}
		void DestroyAdorners() {
			foreach(var item in selectionPartAdorners) {
				item.Adorner.Destroy();
			}
			selectionPartAdorners.Clear();
		}
		bool ISelectionLayerHandler.CanChangeZOrder { get { return false; } }
	}
	public sealed class DefaultSelectionLayerHandler : ISelectionLayerHandler {
		public static readonly double DefaultMultipleSelectionMargin = 7;
		public readonly IDiagramControl Diagram;
		double multipleSelectionMargin = -DefaultMultipleSelectionMargin;
		IEnumerable<IDiagramItem> SelectedItems { get { return Diagram.SelectedItems(); } }
		SelectionPartsLayerHandler partsAdorner;
		public bool IsMultipleSelection { get { return !SelectedItems.IsEmptyOrSingle(); } }
		public IDiagramItem PrimarySelection { get { return Diagram.PrimarySelection(); } }
		public DefaultSelectionLayerHandler(IDiagramControl diagram) {
			this.Diagram = diagram;
			partsAdorner = new SelectionPartsLayerHandler(diagram, 2, (item, isPrimarySelection, factory) => item.Controller.SelectionPartAdornerFactory(factory, isPrimarySelection));
		}
		public SizeInfo<IAdorner>[] GetAdornersSizeInfo(ResizeMode mode) {
			Func<IAdorner, IDiagramItem, SizeInfo<IAdorner>> createSizeInfo = (adorner, item) => SizeInfo.Create(adorner, item, mode);
			if(IsMultipleSelection) {
				return partsAdorner.Adorners
					.Zip(Diagram.SelectedItems(), (adorner, item) => new { Item = item, Adorner = adorner })
					.Where(x => x.Item.CanResize)
					.Select(x => createSizeInfo(x.Adorner.Adorner, x.Item))
					.ToArray();
			} else {
				return new[] { createSizeInfo(selectionAdorner.Adorner, SelectedItems.Single()) };
			}
		}
		IUpdatableAdorner selectionAdorner;
#if DEBUGTEST
		public double MultipleSelectionMarginForTests { get { return multipleSelectionMargin; } }
		public void ClearMultipleSelectionMarginForTests() {
			this.multipleSelectionMargin = 0;
		}
		public IAdorner<ISelectionAdorner> SelectionAdorderForTest { 
			get {
				if(selectionAdorner is CompositeUpdatableAdorner)
					return (IAdorner<ISelectionAdorner>)selectionAdorner.With(x => (CompositeUpdatableAdorner)x).With(x => x.AdornersForTests[0].Adorner);
				return (IAdorner<ISelectionAdorner>)selectionAdorner.With(x => x.Adorner);
			}
		}
		public IAdorner<IShapeParametersAdorner> ShapeParametersAdornerForTest {
			get {
				if(selectionAdorner is CompositeUpdatableAdorner)
					return selectionAdorner.With(x => (CompositeUpdatableAdorner)x).With(x => x.AdornersForTests.Select(a => a.Adorner).OfType<IAdorner<IShapeParametersAdorner>>().FirstOrDefault());
				return null;
			}
		}
		public IEnumerable<IAdorner> SelectionPartAdornersForTest { get { return partsAdorner.Adorners.Select(x => x.Adorner); } }
		public Rect SelectionRectForTests { get { return selectionAdorner == null ? new Rect() : selectionAdorner.Adorner.Bounds; } }
#endif
		void ISelectionLayerHandler.UpdateSelectionAdorners() {
			((ISelectionLayerHandler)partsAdorner).UpdateSelectionAdorners();
			if(selectionAdorner == null)
				return;
			selectionAdorner.Update();
			if(IsMultipleSelection) {
				UpdateMultipleSelectionAdorner();
			}
			else {
				selectionAdorner.Adorner.SetBounds(SelectedItems.Single());
			}
		}
		void ISelectionLayerHandler.RecreateSelectionAdorners() {
			((ISelectionLayerHandler)partsAdorner).RecreateSelectionAdorners();
			DestroyAdorner();
			if(SelectedItems.Any()) {
				selectionAdorner = IsMultipleSelection ?
					DiagramItemController.CreateUpdatableSelectionAdorner(Diagram, this, multipleSelectionMargin) :
					PrimarySelection.Controller.SingleSelectionAdornerFactory(Diagram, this);
			}
		}
		public void UpdateMultipleSelectionAdorner() {
			if(IsMultipleSelection) {
				var points = partsAdorner.Adorners.SelectMany(x => x.Adorner.Bounds.GetPoints().Select(p => p.Rotate(x.Adorner.Angle, x.Adorner.Bounds.GetCenter())));
				Rect selection = MathHelper.GetContainingRect(points.Select(p => p.Rotate(SelectionAdornerAngle)));
				selectionAdorner.Adorner.Bounds = GetDiagramRotationCenter().GetCenteredRect(selection.Size);
			}
		}
		void ISelectionLayerHandler.Clear() {
			((ISelectionLayerHandler)partsAdorner).Clear();
			DestroyAdorner();
		}
		void DestroyAdorner() {
			if(selectionAdorner != null) {
				selectionAdorner.Do(x => x.Adorner.Destroy());
				selectionAdorner = null;
			}
		}
		bool ISelectionLayerHandler.CanChangeZOrder { get { return true; } }
		#region Rotation
		static readonly System.ComponentModel.PropertyDescriptor AngleProperty = ExpressionHelper.GetProperty((DefaultSelectionLayerHandler x) => x.SelectionAdornerAngle);
		public double SelectionAdornerAngle {
			get { return selectionAdorner.Return(x => x.Adorner.Angle, () => 0); }
			set {
				if(selectionAdorner != null && selectionAdorner.Adorner.Angle != value) {
					selectionAdorner.Adorner.Angle = value;
					UpdateMultipleSelectionAdorner();
				}
			}
		}
		internal void SetMultipleSelectionAdornerAngle(Transaction transaction, double angle) {
			if(IsMultipleSelection) {
				transaction.SetProperty(angle, this, AngleProperty, layer => new FakeFinder<DefaultSelectionLayerHandler>(layer));
			}
		}
		public Point GetDiagramRotationCenter() {
			IEnumerable<Rect_Angle> rects = null;
			if(IsMultipleSelection)
				rects = partsAdorner.Adorners.Select(adorner => adorner.Adorner.RotatedBounds());
			else
				rects = SelectedItems.Select(item => item.RotatedDiagramBounds());
			return MathHelper.GetRotationCenter(rects, SelectionAdornerAngle);
		}
		#endregion
	}
	public sealed class EmptySelectionLayer : ISelectionLayer {
		class EmtySelectionLayerHandler : ISelectionLayerHandler {
			void ISelectionLayerHandler.Clear() {
			}
			void ISelectionLayerHandler.UpdateSelectionAdorners() {
			}
			void ISelectionLayerHandler.RecreateSelectionAdorners() {
			}
			bool ISelectionLayerHandler.CanChangeZOrder { get { return false; } }
		}
		public static readonly ISelectionLayer Instance = new EmptySelectionLayer();
		public EmptySelectionLayer() { }
		ISelectionLayerHandler ISelectionLayer.CreateSelectionHandler(IDiagramControl diagram) {
			return new EmtySelectionLayerHandler();
		}
	}
}
