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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native.LayoutUI;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Native {
	public class SparklineBox : InlinePictureBox {
		public SparklineBox() { }
		PieceTable pieceTable;
		public SparklineBox(PieceTable pieceTable)
			: base() {
			this.pieceTable = pieceTable;
		}
		const int Margin = 20;
		SNSparklineField sparklineField;
		readonly HotZoneCollection hotZones = new HotZoneCollection();
		public HotZoneCollection HotZones {
			get {
				if(hotZones.Count == 0)
					InitializeHotZones();
				return hotZones;
			}
		}
		public bool IsEmpty {
			get {
				var run = GetRun(pieceTable);
				sparklineField = GetSparklineField((SparklineRun)run);
				return (sparklineField == null || sparklineField.IsEmpty);
			}
		}
		#region Calculations
		void InitializeHotZones() {
			if(ActualSizeBounds.IsEmpty)
				return;
			float radius = CalculateRadius(ActualSizeBounds);
			PointF hotZoneCenter = GetHotZoneCenter(ActualSizeBounds, radius);
			hotZones.Add(new DropValuesSparklineHotZone(hotZoneCenter, radius));
		}
		float CalculateRadius(Rectangle bounds) {
			int maxVertDiameter = bounds.Height - 2 * Margin;
			const float HotZonesTotalWidthToSparklineWidthRatio = 4.0f / 7.0f;
			int maxHorzDiameter = (int)(bounds.Width * HotZonesTotalWidthToSparklineWidthRatio);
			return Math.Min(maxVertDiameter, maxHorzDiameter) / 2;
		}
		PointF GetHotZoneCenter(Rectangle bounds, float radius) {
			float distanceToHorzBorder = bounds.Y + bounds.Height / 2;
			float distanceToVertBorder = bounds.X + bounds.Width / 2;
			return new PointF { X = distanceToVertBorder, Y = distanceToHorzBorder };
		}
		#endregion
		public override Box CreateBox() {
			return new SparklineBox(pieceTable);
		}
		protected internal override void ExportHotZones(Painter painter) {
			if(!IsEmpty)
				return;
			using(SparklineBoxHotZonePainter hotZonePainter = new SparklineBoxHotZonePainter(painter)) {
				foreach(IRoundHotZone hotZone in HotZones) {
					hotZone.Accept(hotZonePainter);
				}
			}
		}
		SNSparklineField GetSparklineField(SparklineRun sparklineRun) {
			if(sparklineRun == null) return null;
			Field field = sparklineRun.PieceTable.FindNonTemplateFieldByRunIndex(this.StartPos.RunIndex);
			if(field == null)
				return null;
			SNSparklineField sparklineField = new SnapFieldCalculatorService().ParseField(sparklineRun.PieceTable, field) as SNSparklineField;
			return sparklineField;
		}
	}
}
