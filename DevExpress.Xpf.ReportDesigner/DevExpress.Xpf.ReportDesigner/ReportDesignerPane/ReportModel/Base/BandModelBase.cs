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
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel.Native;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel.Native.BandModel;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public abstract class BandModelBase : XRContainerModelBase<Band, XRControlModelBase, BandDiagramItem> {
		protected internal BandModelBase(XRControlModelFactory.ISource<Band> source, ImageSource icon)
			: base(source, icon) {
			renderer = Report.CreateBandRenderer(XRObject, Report.XRObject);
			Controls.CollectionChanged += (s, e) => Report.DiagramItem.Diagram.InvalidateRenderLayer();
			foundationLogic = new BandModelFoundationLogic(this, Layout, () => RaisePropertyChanged(() => Top));
			innerBottomBandLogic = new InnerBottomBandLogic(this, Layout, () => RaisePropertyChanged(() => Height));
		}
		protected override void RaiseLayoutPropertiesChanged(PropertyChangedEventArgs e) {
			if(e.PropertyName == GetPropertyName(() => Layout.Param3))
				RaisePropertyChanged(() => Width);
			else if(e.PropertyName == GetPropertyName(() => Layout.Param4))
				RaisePropertiesChanged(() => UsableHeight, () => Height);
			else if(e.PropertyName == GetPropertyName(() => Layout.Param1))
				RaisePropertyChanged(() => Left);
			else if(e.PropertyName == GetPropertyName(() => Layout.Ref1))
				foundationLogic.Reset();
			else if(e.PropertyName == GetPropertyName(() => Layout.Ref2))
				innerBottomBandLogic.Reset();
		}
		protected override void OnThisPropertyChanged(PropertyChangedEventArgs e) {
			if(e.PropertyName == GetPropertyName(() => Width)) {
				RaisePropertyChanged(() => Size);
			} else if(e.PropertyName == GetPropertyName(() => Height)) {
				RaisePropertiesChanged(() => Size, () => Bottom, () => UsableBottomAbsolute);
			} else if(e.PropertyName == GetPropertyName(() => Left)) {
				RaisePropertyChanged(() => Position);
			} else if(e.PropertyName == GetPropertyName(() => Top)) {
				RaisePropertiesChanged(() => Position, () => Bottom, () => TopAbsolute);
			} else if(e.PropertyName == GetPropertyName(() => Bottom)) {
				RaisePropertiesChanged(() => Position, () => BottomAbsolute);
			} else if(e.PropertyName == GetPropertyName(() => Collapsed)) {
				RaisePropertyChanged(() => Height);
			}
		}
		readonly BandModelFoundationLogic foundationLogic;
		readonly InnerBottomBandLogic innerBottomBandLogic;
		bool topInvalidated;
		public override double Top {
			get {
				return ReturnWithInvalidated(() => {
					BandKind bandKind = XRObject.BandKind;
					if(bandKind == BandKind.TopMargin) {
						return 0;
					}
					BandModelBase foundationBandModel = foundationLogic.BandModel;
					if(foundationBandModel != null && bandKind == BandKind.SubBand && foundationBandModel.XRObject.BandKind != BandKind.SubBand) {
						return foundationBandModel.UsableHeight;
					}
					return foundationBandModel.Return(x => x.Bottom, () => 0);
				}, () => 0, ref topInvalidated);
			}
			set { Height = Bottom - value; }
		}
		bool bottomInvalidated;
		public override double Bottom {
			get {
				return ReturnWithInvalidated(() => {
					BandKind bandKind = XRObject.BandKind;
					if(bandKind == BandKind.TopMargin) {
						return Height;
					}
					return Top + Height;
				}, () => 0, ref bottomInvalidated);
			}
			set { Height = value - Top; }
		}
		bool heightInvalidated;
		public override double Height {
			get {
				return ReturnWithInvalidated(() => {
					return innerBottomBandLogic.BandModel.Return(x => x.Bottom, () => Layout.Param4);
				}, () => 0, ref heightInvalidated);
			}
			set {
				if(innerBottomBandLogic.BandModel == null)
					Layout.Param4 = value;
			}
		}
		bool usableHeightInvalidated;
		public double UsableHeight {
			get {
				return ReturnWithInvalidated(() => Layout.Param4, () => 0, ref usableHeightInvalidated);
			}
			set { Layout.Param4 = value; }
		}
		bool usableBottomAbsoluteInvalidated;
		public double UsableBottomAbsolute {
			get {
				return ReturnWithInvalidated(()=> TopAbsolute + UsableHeight, ()=> 0, ref usableBottomAbsoluteInvalidated);
			}
			set { UsableHeight = value - TopAbsolute; }
		}
		bool collapsed;
		public bool Collapsed {
			get { return collapsed; }
			set {
				collapsed = value;
				RaisePropertyChanged(() => Collapsed);
			}
		}
		protected override void BeforeDelete(Transaction transaction) {
			base.BeforeDelete(transaction);
			var itemsToDelete = Report.Controls.OfType<XRCrossBandControlModelBase>()
				.Where(x => x.XRObject.StartBand == XRObject && x.XRObject.EndBand == XRObject)
				.Select(x => x.DiagramItem)
				.ToList();
			var itemsToChangeEnd = Report.Controls.OfType<XRCrossBandControlModelBase>()
				.Where(x => x.XRObject.StartBand != XRObject && x.XRObject.EndBand == XRObject)
				.Select(x => x.DiagramItem)
				.ToList();
			var itemsToChangeStart = Report.Controls.OfType<XRCrossBandControlModelBase>()
				.Where(x => x.XRObject.StartBand == XRObject && x.XRObject.EndBand != XRObject)
				.Select(x => x.DiagramItem)
				.ToList();
			foreach(var crossBandDiagramItem in itemsToDelete)
				transaction.RemoveItem(crossBandDiagramItem);
			foreach(var crossBandDiagramItem in itemsToChangeStart) {
				double newTop = BottomAbsolute;
				double newHeight = crossBandDiagramItem.Position.Y + crossBandDiagramItem.Height - newTop;
				transaction.SetItemBounds(crossBandDiagramItem, new Rect(new Point(crossBandDiagramItem.Position.X, newTop), new Size(crossBandDiagramItem.Width, newHeight)));
			}
			foreach(var crossBandDiagramItem in itemsToChangeEnd) {
				double newHeight = TopAbsolute - crossBandDiagramItem.Position.Y;
				transaction.SetItemBounds(crossBandDiagramItem, new Rect(crossBandDiagramItem.Position, new Size(crossBandDiagramItem.Width, newHeight)));
			}
		}
		BandRendererBase renderer;
		protected override void AttachDiagramItem() {
			base.AttachDiagramItem();
			XRDiagramControl.SetRenderer(DiagramItem, renderer);
		}
		readonly List<IXRControlBoundsTracker<XRCrossBandControlModelBase>> crossBandControls = new List<IXRControlBoundsTracker<XRCrossBandControlModelBase>>();
		internal void AddCrossBandControl(XRCrossBandControlModelBase crossBandControl) {
			crossBandControls.Add(XRControlModelBoundsTracker.NewTopAndBottomAbsoluteTracker(crossBandControl, UpdateMinHeight));
			UpdateMinHeight();
		}
		internal void RemoveCrossBandControl(XRCrossBandControlModelBase crossBandControl) {
			int controlIndex = crossBandControls.Select((item, index) => new { item = item, index = index }).Where(x => x.item.Model == crossBandControl).Select(x => x.index).First();
			crossBandControls[controlIndex].Dispose();
			crossBandControls.RemoveAt(controlIndex);
			UpdateMinHeight();
		}
		void UpdateMinHeight() {
			double minHeight = 0.0;
			foreach(var control in crossBandControls) {
				if(control.Model.XRObject.StartBand == XRObject)
					minHeight = Math.Max(minHeight, control.Model.Top - TopAbsolute);
				if(control.Model.XRObject.EndBand == XRObject)
					minHeight = Math.Max(minHeight, BottomAbsolute - control.Model.Bottom);
			}
			DiagramItem.MinHeight = minHeight;
		}
		T ReturnWithInvalidated<T>(Func<T> evaluator, Func<T> fallback, ref bool invalidated) {
			if(invalidated)
				return fallback();
			invalidated = true;
			var result = evaluator();
			invalidated = false;
			return result;
		}
	}
}
