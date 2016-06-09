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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel.Native;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public abstract class XRCrossBandControlModelBase : XRControlModelBase {
		protected XRCrossBandControlModelBase(XRControlModelFactory.ISource<XRCrossBandControl> source, ImageSource icon)
			: base(source, icon) {
			UpdateEndBand();
			UpdateStartBand();
		}
		public new XRCrossBandControl XRObject { get { return (XRCrossBandControl)base.XRObject; } }
		public new XRCrossBandControlDiagramItem DiagramItem { get { return (XRCrossBandControlDiagramItem)base.DiagramItem; } }
		readonly Locker updateLayoutLocker = new Locker();
		protected override void RaiseLayoutPropertiesChanged(PropertyChangedEventArgs e) {
			updateLayoutLocker.DoIfNotLocked(() => {
				if(e.PropertyName == GetPropertyName(() => Layout.Param3))
					RaisePropertyChanged(() => Width);
				else if(e.PropertyName == GetPropertyName(() => Layout.Param4))
					RaisePropertyChanged(() => Bottom);
				else if(e.PropertyName == GetPropertyName(() => Layout.Param1))
					RaisePropertyChanged(() => Left);
				else if(e.PropertyName == GetPropertyName(() => Layout.Param2))
					RaisePropertyChanged(() => Top);
				else if(e.PropertyName == GetPropertyName(() => Layout.Ref1))
					UpdateStartBand();
				else if(e.PropertyName == GetPropertyName(() => Layout.Ref2))
					UpdateEndBand();
			});
		}
		protected override void OnThisPropertyChanged(PropertyChangedEventArgs e) {
			if(e.PropertyName == GetPropertyName(() => Width))
				RaisePropertyChanged(() => Size);
			else if(e.PropertyName == GetPropertyName(() => Bottom))
				RaisePropertiesChanged(() => Height, () => BottomAbsolute);
			else if(e.PropertyName == GetPropertyName(() => Height))
				RaisePropertyChanged(() => Size);
			else if(e.PropertyName == GetPropertyName(() => Left))
				RaisePropertyChanged(() => Position);
			else if(e.PropertyName == GetPropertyName(() => Top))
				RaisePropertiesChanged(() => Position, () => Height, () => TopAbsolute);
		}
		BandModelBase startBand;
		IXRControlBoundsTracker<BandModelBase> startBandModelTracker;
		void UpdateStartBand() {
			SetStartBand(Layout.Ref1.With(x => (BandModelBase)Report.Factory.GetModel(x)));
			RaisePropertyChanged(() => Top);
		}
		void SetStartBand(BandModelBase newStartBand) {
			if(startBand != null)
				startBand.RemoveCrossBandControl(this);
			startBand = newStartBand;
			if(startBand != null)
				startBand.AddCrossBandControl(this);
			startBandModelTracker = startBand.With(x => XRControlModelBoundsTracker.NewTopAndBottomAbsoluteTracker(x, () => RaisePropertyChanged(() => Top)));
		}
		BandModelBase endBand;
		IXRControlBoundsTracker<BandModelBase> endBandModelTracker;
		void UpdateEndBand() {
			SetEndBand(Layout.Ref2.With(x => (BandModelBase)Report.Factory.GetModel(x)));
			RaisePropertyChanged(() => Bottom);
		}
		void SetEndBand(BandModelBase newEndBand) {
			if(endBand != null)
				endBand.RemoveCrossBandControl(this);
			endBand = newEndBand;
			if(endBand != null)
				endBand.AddCrossBandControl(this);
			endBandModelTracker = endBand.With(x => XRControlModelBoundsTracker.NewTopAndBottomAbsoluteTracker(x, () => RaisePropertyChanged(() => Bottom)));
		}
		public override double Top {
			get { return Math.Min(Layout.Param2 + startBand.Return(x => x.TopAbsolute, () => 0.0), startBand.Return(x => x.BottomAbsolute, () => double.PositiveInfinity)); }
			set { SetTopBottom(value, Bottom); }
		}
		public override double Bottom {
			get { return Math.Min(endBand.Return(x => x.UsableBottomAbsolute - Layout.Param4, () => 0.0), endBand.Return(x => x.UsableBottomAbsolute, () => double.PositiveInfinity)); }
			set { SetTopBottom(Top, value); }
		}
		void SetTopBottom(double top, double bottom) {
			double prevTop = Top;
			double prevBottom = Bottom;
			BandModelBase startBand = FindBand(x => x.TopAbsolute <= top && x.BottomAbsolute > top)
				?? FindBand(x => x.XRObject.BandKind == BandKind.TopMargin);
			BandModelBase endBand = FindBand(x => x.TopAbsolute < bottom && x.BottomAbsolute >= bottom)
				?? FindBand(x => x.XRObject.BandKind == BandKind.BottomMargin);
			updateLayoutLocker.DoLockedAction(() => {
				XRObject.StartBand = null;
				XRObject.EndBand = null;
				XRObject.StartBand = startBand.XRObject;
				XRObject.EndBand = endBand.XRObject;
				XRControlInvalidater.InvalidateStartBandEndBand(XRObject);
				SetStartBand(startBand);
				SetEndBand(endBand);
				Layout.Param2 = top - startBand.TopAbsolute;
				Layout.Param4 = endBand.UsableBottomAbsolute - bottom;
			});
			if(Top != prevTop)
				RaisePropertyChanged(() => Top);
			if(Bottom != prevBottom)
				RaisePropertyChanged(() => Bottom);
		}
		public override double Height {
			get { return Bottom - Top; }
			set { Bottom = Top + value; }
		}
		public override Point Position {
			get { return base.Position; }
			set {
				Left = value.X;
				SetTopBottom(value.Y, value.Y + Height);
			}
		}
		BandModelBase FindBand(Func<BandModelBase, bool> predicate) {
			IEnumerable<BandModelBase> items = Report.Controls
				.OfType<BandModelBase>()
				.Flatten(x => x.Controls.OfType<BandModelBase>())
				.Where(x => x.Parent != null && x.XRObject.BandKind != BandKind.DetailReport);
			BandModelBase bandModel = items.LastOrDefault(predicate);
			return bandModel;
		}
		protected override void AttachDiagramItem() {
			base.AttachDiagramItem();
			BindDiagramItemToXRObject(() => DiagramItem.BorderThickness = new Thickness(XRObject.BorderWidth), () => XRObject.BorderWidth);
		}
		protected override void BindAnchorsProperty() {
			DiagramItem.Anchors = Sides.None;
		}
	}
}
