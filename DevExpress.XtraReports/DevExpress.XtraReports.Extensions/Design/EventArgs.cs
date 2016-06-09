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
namespace DevExpress.XtraReports.Design {
	using System.Windows.Forms;
	using DevExpress.XtraReports.UI;
	using System.Drawing;
	public class BandMouseEventArgs : MouseEventArgs {
		BandViewInfo viewInfo;
		public Band Band { get { return viewInfo.Band; } }
		public BandViewInfo ViewInfo { get { return viewInfo; } }
		public BandMouseEventArgs(MouseEventArgs e)
			: base(e.Button, e.Clicks, e.X, e.Y, e.Delta) {
		}
		public BandMouseEventArgs(MouseButtons mouseButton, Point pt, BandViewInfo viewInfo)
			: base(mouseButton, 0, pt.X, pt.Y, 0) {
			this.viewInfo = viewInfo;
		}
	}
	public class BoundsEventArgs : EventArgs {
		RectangleF[] bounds;
		public RectangleF[] Bounds {
			get { return bounds; }
		}
		public BoundsEventArgs(RectangleF[] bounds) {
			this.bounds = bounds;
		}
	}
	public delegate void BoundsEventHandler(object sender, BoundsEventArgs e);
}
