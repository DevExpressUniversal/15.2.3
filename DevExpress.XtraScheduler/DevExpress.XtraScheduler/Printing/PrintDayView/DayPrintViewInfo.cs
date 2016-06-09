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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Printing.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	public class DayPrintViewInfo : DayViewInfo, ISupportClear {
		#region Fields
		GraphicsInfo gInfo;
		#endregion
		public DayPrintViewInfo(DayPrintView view, GraphicsInfo gInfo)
			: base(view) {
			if (gInfo == null)
				Exceptions.ThrowArgumentNullException("gInfo");
			this.gInfo = gInfo;
		}
		#region Properties
		internal GraphicsInfo GInfo { get { return gInfo; } }
		public bool FitIntoBounds { get { return !CellsPreliminaryLayoutResult.DateTimeScrollBarVisible; } }
		public new DayPrintView View { get { return (DayPrintView)base.View; } }
		#endregion
		protected internal override void ApplySelection(SchedulerViewCellContainer cellContainer) {
		}
		public override void CalcPreliminaryLayout() {
			CalcPreliminaryLayoutCore(gInfo.Cache);
		}
		public override void CalcFinalLayout() {
			CalcFinalLayoutCore(gInfo.Cache);
		}
		public override void CalcScrollBarVisibility() {
			CalcScrollBarVisibilityCore(gInfo.Cache);
		}
		protected internal override void ExecuteNavigationButtonsLayoutCalculator(GraphicsCache cache) {
		}
		protected internal override void OnTimer(object sender, EventArgs e) {
		}
		#region ISupportClear Members
		public void Clear() {
			if (View != null) {
				View.Dispose();
			}
			this.Dispose();
		}
		#endregion
	}
}
