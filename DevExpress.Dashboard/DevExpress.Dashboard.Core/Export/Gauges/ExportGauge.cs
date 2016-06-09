#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Drawing;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraGauges.Base;
using System;
namespace DevExpress.DashboardExport {
	public class ExportGauge : IDisposable {
		public GaugeContainer Container { get; private set; }
		public Rectangle Bounds { get; private set; }
		public IGauge IGauge { get { return Container.Gauges[0]; } }
		public GaugeModel GaugeModel { get; set; }
		public ExportGauge(GaugeContainer container, GaugeModel model) {
			Guard.ArgumentNotNull(container, "container");
			Container = container;
			GaugeModel = model;
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing)
				Container.Dispose();			
		}
		public void SetBounds(Rectangle gaugeBounds, Rectangle containerBounds) {
			Bounds = containerBounds;
			Container.Size = containerBounds.Size;
			IGauge.Bounds = new Rectangle(new Point(gaugeBounds.X - containerBounds.X, gaugeBounds.Y - containerBounds.Y), gaugeBounds.Size);
		}
		public Image GetImage() {
			return Container.GetImage();
		}
	}
}
