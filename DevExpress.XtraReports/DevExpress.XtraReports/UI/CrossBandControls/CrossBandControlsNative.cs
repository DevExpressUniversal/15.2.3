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

using DevExpress.XtraReports.UI;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Design;
using System.Collections.ObjectModel;
using System.Collections;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Native.CrossBandControls {
	[ToolboxItem(false)]
	public class WrappedXRLine : XRLine {
		Band band;
		XRCrossBandControl cbControl;
		public override XtraReport RootReport { get { return cbControl.RootReport; } }
		public override Band Band { get { return band; } }
		internal override XRControl PrintableParent { get { return band; } }
		public override bool Visible { get { return cbControl.Visible; } set {} }
		public WrappedXRLine(XRCrossBandControl cbControl, Band band) {
			this.cbControl = cbControl;
			this.band = band;
		}
		public void UpdateView(DashStyle style, LineDirection direction, float lineWidth, Color color) {
			this.ForeColor = color;
			this.LineStyle = style;
			this.LineDirection = direction;
			this.fLineWidth = lineWidth;
			this.KeepTogether = this.LineDirection != LineDirection.Vertical;
		}
		protected override void SetBounds(float x, float y, float width, float height, System.Windows.Forms.BoundsSpecified specified) {
			SetBoundsAndUpdateLayout(x, y, width, height, specified);
		}
		internal protected override int GetMinimumHeight() {
			return 0;
		}
		protected override int GetMinimumWidth() {
			return 0;
		}
		internal override XRControl RealControl {
			get {
				return this.cbControl;
			}
		}
		internal void DesignActivate(Color lineColor) {
			this.ForeColor = lineColor;
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			if(LineDirection == LineDirection.Vertical) {
				DevExpress.XtraPrinting.Native.VisualBrickHelper.SetCanOverflow(brick, true);
				brick.SetAttachedValue(BrickAttachedProperties.MergeValue, new DevExpress.Utils.MultiKey(cbControl));
			}
			brick.CanMultiColumn = false;
		}
		protected override float GetLineWidth() {
			return XRConvert.Convert(fLineWidth, Dpi, GraphicsDpi.Pixel); 
		}
		protected override bool IsCrossbandControl {
			get {
				return ((IBrickOwner)RealControl).IsCrossbandControl;
			}
		}
	}
	public abstract class CrossBandControlAnchorer {
		#region static
		public static CrossBandControlAnchorer CreateInstance(XRCrossBandControl cbControl, Band band) {
			if(cbControl.AnchorVertical == VerticalAnchorStyles.Top)
				return new CrossBandControlAnchorerTop(cbControl, band);
			if(cbControl.AnchorVertical == VerticalAnchorStyles.Bottom)
				return new CrossBandControlAnchorerBottom(cbControl, band);
			if(cbControl.AnchorVertical == VerticalAnchorStyles.Both)
				return new CrossBandControlAnchorerBoth(cbControl, band);
			return new CrossBandControlAnchorerNone(cbControl, band);
		}
		#endregion
		#region inner classes
		class CrossBandControlAnchorerNone : CrossBandControlAnchorer {
			public CrossBandControlAnchorerNone(XRCrossBandControl cbControl, Band band)
				: base(cbControl, band) {
			}
			protected override int GetMinBottom(Rectangle rect) {
				if(cbControl.StartBand == band && cbControl.EndBand == band)
					return rect.Bottom;
				return rect.Height;
			}
			protected override void SetAnchorBounds(float oldHeight, float newHeight) {
				if(cbControl.EndBand == band)
					return;
				float delta = Math.Max(newHeight - oldHeight, -cbControl.StartPointF.Y);
				cbControl.StartPointF = new PointF(cbControl.StartPointF.X, cbControl.StartPointF.Y + delta);
			}
			protected override void SetVertLineAnchor(XRControl control) {
				if(band == cbControl.StartBand)
					control.AnchorVertical = VerticalAnchorStyles.Bottom;
				else if(band == cbControl.EndBand)
					control.AnchorVertical = VerticalAnchorStyles.Top;
			}
		}
		class CrossBandControlAnchorerTop : CrossBandControlAnchorer {
			public CrossBandControlAnchorerTop(XRCrossBandControl cbControl, Band band)
				: base(cbControl, band) {
			}
			protected override int GetMinBottom(Rectangle rect) {
				if(cbControl.EndBand == band && cbControl.StartBand == band)
					return rect.Bottom;
				else if(cbControl.StartBand == band)
					return rect.Top + cbControl.GetMinimumHeight();
				return rect.Height;
			}
			protected override void SetAnchorBounds(float oldHeight, float newHeight) {
				return;
			}
			protected override void SetVertLineAnchor(XRControl control) {
				if(band == cbControl.EndBand)
					control.AnchorVertical = VerticalAnchorStyles.Top;
			}
		}
		class CrossBandControlAnchorerBottom : CrossBandControlAnchorer {
			public CrossBandControlAnchorerBottom(XRCrossBandControl cbControl, Band band)
				: base(cbControl, band) {
			}
			protected override int GetMinBottom(Rectangle rect) {
				return cbControl.StartBand == band ? rect.Height : cbControl.GetMinimumHeight();
			}
			protected override void SetAnchorBounds(float oldHeight, float newHeight) {
				if(cbControl.StartBand == band) {
					float delta = Math.Max(-cbControl.StartPointF.Y, newHeight - oldHeight);
					ValidMove(delta);
					return;
				}
				if(cbControl.EndBand == band) {
					float delta = Math.Max(newHeight, cbControl.GetMinimumHeight()) - oldHeight;
					cbControl.EndPointF = new PointF(cbControl.EndPointF.X, Math.Max(cbControl.EndPointF.Y + delta, cbControl.GetMinimumHeight()));
				}
			}
			protected override void SetVertLineAnchor(XRControl control) {
				if(band == cbControl.StartBand)
					control.AnchorVertical = VerticalAnchorStyles.Bottom;
			}
			void ValidMove(float delta) {
				if(delta <= 0)
					MoveStartPoint(delta);
				if(cbControl.EndBand == band)
					MoveEndPoint(delta);
				if(delta > 0)
					MoveStartPoint(delta);
			}
			void MoveEndPoint(float delta) {
				cbControl.EndPointF = new PointF(cbControl.EndPointF.X, cbControl.EndPointF.Y + delta);
			}
			void MoveStartPoint(float delta) {
				cbControl.StartPointF = new PointF(cbControl.StartPointF.X, cbControl.StartPointF.Y + delta);
			}
		}
		class CrossBandControlAnchorerBoth : CrossBandControlAnchorer {
			public CrossBandControlAnchorerBoth(XRCrossBandControl cbControl, Band band)
				: base(cbControl, band) {
			}
			protected override int GetMinBottom(Rectangle rect) {
				return cbControl.EndBand == band ? cbControl.GetMinimumHeight() : rect.Top + cbControl.GetMinimumHeight();
			}
			protected override void SetAnchorBounds(float oldHeight, float newHeight) {
				if(cbControl.EndBand != band)
					return;
				float startPointY = cbControl.StartBand == band ? cbControl.StartPointF.Y : 0;
				startPointY += cbControl.GetMinimumHeight();
				float delta = Math.Max(newHeight, startPointY) - oldHeight;
				cbControl.EndPointF = new PointF(cbControl.EndPointF.X, Math.Max(cbControl.EndPointF.Y + delta, startPointY));
			}
			protected override void SetVertLineAnchor(XRControl control) {
				control.AnchorVertical = VerticalAnchorStyles.Both;
			}
		}
		#endregion
		protected XRCrossBandControl cbControl;
		protected Band band;		
		protected CrossBandControlAnchorer(XRCrossBandControl cbControl, Band band) {
			this.cbControl = cbControl;
			this.band = band;			
		}
		public int GetMinimumBottom() {
			Rectangle rect = Rectangle.Round(cbControl.GetBounds(band));
			if(cbControl.BoundsChanging)
				return rect.Bottom;
			if(cbControl.EndBand != band && cbControl.StartBand != band)
				return 0;
			return GetMinBottom(rect);
		}
		public void SetAnchorBounds(RectangleF oldBounds, RectangleF newBounds) {
			if(cbControl.EndBand != band && cbControl.StartBand != band)
				return;
			SetAnchorBounds(oldBounds.Height, newBounds.Height);
		}		
		public void SetVerticalLineAnchor(XRControl control) {
			if(band == cbControl.StartBand && band == cbControl.EndBand) {
				control.AnchorVertical = cbControl.AnchorVertical;
				return;
			}
			control.AnchorVertical = VerticalAnchorStyles.Both;
			SetVertLineAnchor(control);
		}
		protected abstract int GetMinBottom(Rectangle rect);
		protected abstract void SetAnchorBounds(float oldHeight, float newHeight);
		protected abstract void SetVertLineAnchor(XRControl control);
	}
	public class BorderCollector {
		#region static
		public static BorderCollector CreateInstance(XRCrossBandBox cbControl, bool designMode) {
			if(designMode)
				return new DesignTimeBorderCollector(cbControl);
			return new BorderCollector(cbControl);
		}
		#endregion
		#region inner classes
		class DesignTimeBorderCollector : BorderCollector {
			public DesignTimeBorderCollector(XRCrossBandBox cbControl)
				: base(cbControl) {
			}
			protected override Collection<XRControl> GetBorders(Dictionary<BorderSide, WrappedXRLine> controls, Band band) {
				Collection<XRControl> controlsList = new Collection<XRControl>();
				BorderSide verticalBorderSide = BorderSide.None;
				controlsList.Add(controls[BorderSide.Right]);
				controlsList.Add(controls[BorderSide.Left]);
				if(band == this.cbControl.StartBand) {
					controlsList.Add(controls[BorderSide.Top]);
					verticalBorderSide |= BorderSide.Top;
				}
				if(band == this.cbControl.EndBand) {
					controlsList.Add(controls[BorderSide.Bottom]);
					verticalBorderSide |= BorderSide.Bottom;
				}
				Color borderColor = cbControl.GetEffectiveBorderColor();
				if((cbControl.Borders & BorderSide.Right) != 0)
					controls[BorderSide.Right].DesignActivate(borderColor);
				if((cbControl.Borders & BorderSide.Left) != 0)
					controls[BorderSide.Left].DesignActivate(borderColor);
				if((cbControl.Borders & BorderSide.Top) != 0)
					controls[BorderSide.Top].DesignActivate(borderColor);
				if((cbControl.Borders & BorderSide.Bottom) != 0)
					controls[BorderSide.Bottom].DesignActivate(borderColor);
				return controlsList;
			}
		}
		#endregion
		protected XRCrossBandControl cbControl;
		protected Band band;
		public BorderCollector(XRCrossBandBox cbControl) {
			this.cbControl = cbControl;
		}
		public XRControl[] GetBorderControls(Dictionary<BorderSide, WrappedXRLine> controls, Band band) {
			Collection<XRControl> controlsList = GetBorders(controls, band);
			return (XRControl[])new ArrayList(controlsList).ToArray(typeof(XRControl));
		}
		protected virtual Collection<XRControl> GetBorders(Dictionary<BorderSide, WrappedXRLine> controls, Band band) {
			Collection<XRControl> controlsList = new Collection<XRControl>();
			if(band == cbControl.StartBand && (cbControl.Borders & BorderSide.Top) != 0)
				controlsList.Add(controls[BorderSide.Top]);
			if(band == cbControl.EndBand && (cbControl.Borders & BorderSide.Bottom) != 0)
				controlsList.Add(controls[BorderSide.Bottom]);
			if((cbControl.Borders & BorderSide.Right) != 0)
				controlsList.Add(controls[BorderSide.Right]);
			if((cbControl.Borders & BorderSide.Left) != 0)
				controlsList.Add(controls[BorderSide.Left]);
			return controlsList;
		}
	}
}
