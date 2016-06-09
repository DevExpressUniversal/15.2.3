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
namespace DevExpress.XtraReports.Design.MouseTargets {
	class CrossBandBoxMouseTarget : ControlMouseTarget {
		new XRCrossBandControl XRControl {
			get {
				return (XRCrossBandControl)base.XRControl;
			}
		}
		public CrossBandBoxMouseTarget(XRControl xrControl, IServiceProvider servProvider)
			: base(xrControl, servProvider) {
		}
		public override bool ContainsPoint(Point pt, BandViewInfo viewInfo) {
			if(viewInfo == null)
				return false;
			XRControl[] controls = this.XRControl.GetPrintableControlsForce(viewInfo.Band);
			bool containsPoint = false;
			foreach(XRControl control in controls) {
				RectangleF rect = this.ZoomService.ToScaledPixels(control.BoundsF, control.Dpi);
				rect.Offset(viewInfo.ClientBandBounds.Location);
				containsPoint = containsPoint || rect.Contains(pt);
			}
			return containsPoint;
		}
	}
}
