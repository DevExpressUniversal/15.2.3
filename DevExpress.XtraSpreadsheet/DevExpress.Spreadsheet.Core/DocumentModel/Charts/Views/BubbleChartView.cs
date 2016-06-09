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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class BubbleChartView : ChartViewWithVaryColors {
		#region Static Members
		internal static ChartType GetChartType(bool bubble3D) {
			return bubble3D ? ChartType.Bubble3D : ChartType.Bubble;
		}
		#endregion
		#region Fields
		int bubbleScale = 100;
		#endregion
		public BubbleChartView(IChart parent)
			: base(parent) {
		}
		#region Properties
		#region Bubble3D
		public bool Bubble3D {
			get { return Info.Bubble3D; }
			set {
				if(Bubble3D == value)
					return;
				SetPropertyValue(SetBubble3DCore, value);
			}
		}
		DocumentModelChangeActions SetBubble3DCore(ChartViewInfo info, bool value) {
			info.Bubble3D = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region BubbleScale
		public int BubbleScale {
			get { return bubbleScale; }
			set {
				ValueChecker.CheckValue(value, 0, 300, "BubbleScale");
				if(bubbleScale == value)
					return;
				SetBubbleScale(value);
			}
		}
		void SetBubbleScale(int value) {
			BubbleScalePropertyChangedHistoryItem historyItem = new BubbleScalePropertyChangedHistoryItem(DocumentModel, this, bubbleScale, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetBubbleScaleCore(int value) {
			this.bubbleScale = value;
			Parent.Invalidate();
		}
		#endregion
		#region ShowNegBubbles
		public bool ShowNegBubbles {
			get { return Info.ShowNegBubbles; }
			set {
				if(ShowNegBubbles == value)
					return;
				SetPropertyValue(SetShowNegBubblesCore, value);
			}
		}
		DocumentModelChangeActions SetShowNegBubblesCore(ChartViewInfo info, bool value) {
			info.ShowNegBubbles = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SizeRepresents
		public SizeRepresentsType SizeRepresents {
			get { return Info.SizeRepresents; }
			set {
				if(SizeRepresents == value)
					return;
				SetPropertyValue(SetSizeRepresentsCore, value);
			}
		}
		DocumentModelChangeActions SetSizeRepresentsCore(ChartViewInfo info, SizeRepresentsType value) {
			info.SizeRepresents = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Bubble; } }
		public override ChartType ChartType { get { return GetChartType(GetActualBubble3D()); } }
		public override AxisGroupType AxesType { get { return AxisGroupType.XY; } }
		public override IChartView CloneTo(IChart parent) {
			BubbleChartView result = new BubbleChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		protected internal override IChartView Duplicate() {
			BubbleChartView result = new BubbleChartView(Parent);
			result.CopyFromWithoutSeries(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override DataLabelPosition DefaultDataLabelPosition {
			get { return DataLabelPosition.Right; }
		}
		public override ISeries CreateSeriesInstance() {
			BubbleSeries result = new BubbleSeries(this);
			result.Bubble3D = GetActualBubble3D();
			return result;
		}
		#endregion
		bool GetActualBubble3D() {
			if (Series.Count == 0)
				return Bubble3D;
			BubbleSeries series = Series[0] as BubbleSeries;
			return series.Bubble3D;
		}
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			BubbleChartView view = value as BubbleChartView;
			if (view != null)
				CopyFromCore(view);
		}
		void CopyFromCore(BubbleChartView value) {
			BubbleScale = value.BubbleScale;
		}
	}
}
