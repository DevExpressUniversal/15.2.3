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
using System.Drawing;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.Design.Adapters {
	class BoundsAdapter : ControlAdapterBase, IBoundsAdapter{
		public BoundsAdapter(XRControl xrControl, IServiceProvider servProvider) 
			: base(xrControl, servProvider) {
		}
		public virtual void SetScreenBounds(RectangleF screenRect) {
			IBoundsAdapter bandAdapter = GetControlAdapter(XRControl.Band);
			RectangleF bandRect = bandAdapter.GetScreenBounds();
			screenRect.Offset(-bandRect.X, -bandRect.Y);
			RectangleF rect = this.ZoomService.FromScaledPixels(screenRect, XRControl.Dpi);
			rect = XRControl.Parent.RectangleFFromBand(rect);
			Designer.SetBounds(rect);
		}
		protected IBoundsAdapter GetControlAdapter(XRControl xrControl) {
			return BoundsAdapterService.GetAdapter(xrControl, servProvider);
		}
		public virtual RectangleF GetScreenBounds() {
			return ClientBandBoundsToScreen(GetClientBandBounds());
		}
		public virtual RectangleF GetClientBandBounds() {
			BandViewInfo viewInfo = BandViewSvc.GetViewInfoByBand(XRControl.Band);
			return this.GetClientBandBounds(viewInfo);
		}
		public virtual RectangleF GetClientBandBounds(BandViewInfo viewInfo) {
			if(viewInfo == null)
				return Rectangle.Empty;
			RectangleF rect = XRControl.BoundsF;
			rect.Location = PointF.Empty;
			rect = XRControl.RectangleFToBand(rect);
			rect = this.ZoomService.ToScaledPixels(rect, XRControl.Dpi);
			rect.Offset(viewInfo.ClientBandBounds.Location);
			return rect;
		}
		protected RectangleF ClientBandBoundsToScreen(RectangleF rect) {
			return BandViewSvc.RectangleFToScreen(rect);
		}
	}
}
