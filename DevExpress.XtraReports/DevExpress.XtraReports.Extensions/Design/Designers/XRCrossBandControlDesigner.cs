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
using System.Text;
using DevExpress.XtraReports.UI;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Native;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Native;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraReports.Design.Adapters;
using DevExpress.XtraReports.Design.Behaviours;
namespace DevExpress.XtraReports.Design {
	[DesignerBehaviour(typeof(CrossBandDesignerBehaviour))]
	public class XRCrossBandControlDesigner : XRControlDesigner {
		#region inner classes
		class VerticalPositionValidator {
			protected XRCrossBandControl control;
			public VerticalPositionValidator(XRCrossBandControl control) {
				this.control = control;
			}
			public Pair<Band, float> Validate(Band band, float pos) {
				return pos < 0 ? ValidatePreviousPosition(band, pos) :
					pos > band.HeightF ? ValidateNextPosition(band, pos) :
					ValidateCurrentPosition(band, pos);
			}
			protected virtual Pair<Band, float> ValidatePreviousPosition(Band band, float pos) {
				Band prevBand = control.GetPreviousBand(band);
				if(prevBand != null && prevBand != band) {
					float newPos = prevBand.HeightF + pos;
					if(newPos < 0)
						return ValidatePreviousPosition(prevBand, newPos);
					return new Pair<Band, float>(prevBand, newPos);
				}
				return new Pair<Band, float>(band, 0);
			}
			protected virtual Pair<Band, float> ValidateNextPosition(Band band, float pos) {
				Band  nextBand = control.GetNextBand(band);
				if(nextBand != null && nextBand != band) {
					float newPos = pos - band.HeightF;
					if(newPos > nextBand.HeightF)
						return ValidateNextPosition(nextBand, newPos);
					return new Pair<Band,float>(nextBand, newPos);
				}
				return new Pair<Band, float>(band, pos);
			}
			protected virtual Pair<Band, float> ValidateCurrentPosition(Band band, float pos) {
				return new Pair<Band, float>(band, pos);
			}
		}
		class SteppedPositionValidator : VerticalPositionValidator {
			protected float step;
			public SteppedPositionValidator(XRCrossBandControl control, float step)
				: base(control) {
				this.step = step;
			}
			protected override Pair<Band, float> ValidatePreviousPosition(Band band, float pos) {
				Band prevBand = control.GetPreviousBand(band);
				if(prevBand != null && prevBand != band) {
					float value = Divider.GetDivisibleValue(prevBand.HeightF, step);
					if(value > prevBand.HeightF)
						value = Math.Max(0, value - step);
					return new Pair<Band, float>(prevBand, value);
				}
				return new Pair<Band, float>(band, 0);
			}
			protected override Pair<Band, float> ValidateNextPosition(Band band, float pos) {
				Band nextBand = control.GetNextBand(band);
				return nextBand != null && nextBand != band ?
					new Pair<Band, float>(nextBand, 0) :
					new Pair<Band, float>(band, pos);
			}
		}
		class BottomPositionValidator : SteppedPositionValidator {
			public BottomPositionValidator(XRCrossBandControl control, float step)
				: base(control, step) {
			}
			protected override Pair<Band, float> ValidateCurrentPosition(Band band, float pos) {
				Pair<Band, float> value = base.ValidateCurrentPosition(band, pos);
				value.Second = control.ValidateBottom(value.Second, band, step);
				value.Second = Math.Min(value.Second, band.HeightF);
				return value;
			}
		}
		#endregion
		protected XRCrossBandControl CrossBandControl {
			get { return Component as XRCrossBandControl; }
		}
		public virtual Size DefaultControlSize {
			get { return new Size(9, 31); }
		}
		public override bool CanDragInReportExplorer {
			get { return false; }
		}
		public override SelectionRules GetSelectionRules(Band band) {
			SelectionRules rules = GetSelectionRules();
			BorderSide borders = XRControl.GetVisibleBorders(band);
			if((borders & BorderSide.Top) == 0)
				rules &= ~SelectionRules.TopSizeable;
			if((borders & BorderSide.Bottom) == 0)
				rules &= ~SelectionRules.BottomSizeable;
			return rules;
		}
		protected override void CorrectControlHeight() {
		}
		public void SetProperties(Band startBand, Band endBand, PointF startPoint, PointF endPoint, float width) {
			this.XRControl.SuspendLayout();
			try {
				SetLocationProperties(startBand, startPoint);
				SetBottomProperties(endBand, endPoint);
				SetWidthProperty(width);
				XRControlDesignerBase.RaiseComponentChanged(changeService, XRControl);
			} finally { 
				this.XRControl.ResumeLayout();
			}
		}
		void SetLocationProperties(Band startBand, PointF startPoint) {
			ValidateBand(startBand);
			XRControlDesignerBase.RaiseComponentChanging(changeService, XRControl, "StartBand");
			XRControlDesignerBase.RaiseComponentChanging(changeService, XRControl, "StartPointFloat");
			this.CrossBandControl.StartBand = startBand;
			this.CrossBandControl.StartPointF = startPoint;
		}
		void SetBottomProperties(Band endBand, PointF endPoint) {
			ValidateBand(endBand);
			XRControlDesignerBase.RaiseComponentChanging(changeService, XRControl, "EndBand");
			XRControlDesignerBase.RaiseComponentChanging(changeService, XRControl, "EndPointFloat");
			this.CrossBandControl.EndBand = endBand;
			this.CrossBandControl.EndPointF = endPoint;
		}
		void ValidateBand(Band band) {
			if(!LockService.GetInstance(this.DesignerHost).CanChangeComponent(band))
				throw new Exception();
		}
		void SetWidthProperty(float width) {
			XRControlDesignerBase.RaiseComponentChanging(changeService, XRControl, XRComponentPropertyNames.Width);
			this.CrossBandControl.WidthF = width;
		}
		public override void SetLocation(PointF value, SizeF stepSize, RectangleSpecified specified, bool raiseChanged) {
			this.XRControl.SuspendLayout();
			try {
				if((specified & RectangleSpecified.Y) > 0 && (specified & RectangleSpecified.X) > 0) {
					SetLocationCore(value, stepSize);
				} else if((specified & RectangleSpecified.Y) > 0) {
					value.X = this.CrossBandControl.StartPointF.X;
					SetLocationCore(value, stepSize);
				} else if((specified & RectangleSpecified.X) > 0) {
					SetLocationProperties(this.CrossBandControl.StartBand, new PointF(value.X, this.CrossBandControl.StartPointF.Y));
				}
			} finally {
				this.XRControl.ResumeLayout();
			}
			this.CrossBandControl.ValidatePoints();
			if(raiseChanged)
				XRControlDesignerBase.RaiseComponentChanged(this.changeService, XRControl);
		}
		void SetLocationCore(PointF value, SizeF stepSize) {
			Pair<Band, float> newValue = new SteppedPositionValidator(this.CrossBandControl, stepSize.Height).Validate(CrossBandControl.StartBand, value.Y);
			if(newValue.First != this.CrossBandControl.StartBand || newValue.Second != this.CrossBandControl.StartPointF.Y) {
				float delta = GetOffset(this.CrossBandControl.StartBand, this.CrossBandControl.StartPointF.Y, newValue.First, newValue.Second);
				SetLocationProperties(newValue.First, new PointF(value.X, newValue.Second));
				Pair<Band, float> newValue2 = new VerticalPositionValidator(this.CrossBandControl).Validate(this.CrossBandControl.EndBand, this.CrossBandControl.EndPointF.Y + delta);
				SetBottomProperties(newValue2.First, new PointF(value.X, newValue2.Second));
			} else {
				SetLocationProperties(newValue.First, new PointF(value.X, newValue.Second));
			}
		}
		float GetOffset(Band band, float pos, Band newBand, float newPos) {
			if(newBand == CrossBandControl.GetPreviousBand(band)) {
				return -pos - Math.Max(0, newBand.HeightF - newPos); 
			} else if(newBand == CrossBandControl.GetNextBand(band)) {
				return newPos + Math.Max(0, band.HeightF - pos);
			} else {
				return newPos - pos;
			}
		}
		public override void SetRightBottom(PointF value, SizeF stepSize, RectangleSpecified specified, bool raiseChanged) {
			this.XRControl.SuspendLayout();
			try {
				if((specified & RectangleSpecified.Height) > 0) {
					Pair<Band, float> newValue = new BottomPositionValidator(this.CrossBandControl, stepSize.Height).Validate(this.CrossBandControl.EndBand, value.Y);
					SetBottomProperties(newValue.First, new PointF(this.CrossBandControl.EndPointF.X, newValue.Second));
				}
				if((specified & RectangleSpecified.Width) > 0) {
					float right = XRControl.ValidateRight(value.X, stepSize.Width);
					SetWidthProperty(right - this.CrossBandControl.EndPointF.X);
				}
			} finally {
				this.XRControl.ResumeLayout();
			}
			this.CrossBandControl.ValidatePoints();
			if(raiseChanged)
				XRControlDesignerBase.RaiseComponentChanged(this.changeService, XRControl);
		}
		public override void MoveControl(XRControl parent, PointF pixelPoint, RectangleF screenRect) {
			IBoundsAdapter adapter = BoundsAdapterService.GetAdapter(XRControl, fDesignerHost);
			adapter.SetScreenBounds(screenRect);
		}
		protected override void InitializeParent() {
			if(XRControl.Parent == null) {
				XtraReport report = (XtraReport)DesignerHost.RootComponent;
				if(report != null)
					report.CrossBandControls.Add(CrossBandControl);
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRCrossBandControlDesignerActionList(this));
		}
		public override string GetStatus() {
			try {
				return String.Format("{0} {{ Width:{1} }}", Component.Site.Name, XRControl.WidthF);
			} catch { return ""; }
		}
	}
	public class XRCrossBandLineDesigner : XRCrossBandControlDesigner {
		protected override void RegisterActionLists(DesignerActionListCollection list) {			
			list.Add(new XRCrossBandLineDesignerActionList(this));
			list.Add(new XRCrossBandControlDesignerActionList(this));
			list.Add(new XRFormattingControlDesignerActionList(this));
		}
	}
}
