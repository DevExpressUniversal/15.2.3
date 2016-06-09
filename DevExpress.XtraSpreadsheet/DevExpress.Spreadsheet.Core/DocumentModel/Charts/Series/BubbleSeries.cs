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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class BubbleSeries : SeriesWithErrorBarsAndTrendlines, ISupportsInvertIfNegative {
		#region Fields
		bool invertIfNegative = false;
		IDataReference bubbleSize;
		bool bubble3D = true;
		#endregion
		public BubbleSeries(IChartView view)
			: base(view) {
			this.bubbleSize = DataReference.Empty;
		}
		#region Properties
		#region InvertIfNegative
		public bool InvertIfNegative {
			get { return invertIfNegative; }
			set {
				if(invertIfNegative == value)
					return;
				SetInvertIfNegative(value);
			}
		}
		void SetInvertIfNegative(bool value) {
			InvertIfNegativePropertyChangedHistoryItem historyItem = new InvertIfNegativePropertyChangedHistoryItem(DocumentModel, this, invertIfNegative, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetInvertIfNegativeCore(bool value) {
			this.invertIfNegative = value;
			Parent.Invalidate();
		}
		#endregion
		#region BubbleSize
		public IDataReference BubbleSize {
			get { return bubbleSize; }
			set {
				if (value == null)
					value = DataReference.Empty;
				if (!value.IsNumber && !value.Equals(DataReference.Empty))
					throw new ArgumentException("String reference is not allowed for bubble size!");
				if (bubbleSize.Equals(value))
					return;
				SetBubbleSize(value);
			}
		}
		void SetBubbleSize(IDataReference value) {
			BubbleSizePropertyChangedHistoryItem historyItem = new BubbleSizePropertyChangedHistoryItem(DocumentModel, this, bubbleSize, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetBubbleSizeCore(IDataReference value) {
			this.bubbleSize = value;
			Parent.Invalidate();
		}
		#endregion
		#region Bubble3D
		public bool Bubble3D {
			get { return bubble3D; }
			set {
				if (bubble3D == value)
					return;
				SetBubble3D(value);
			}
		}
		void SetBubble3D(bool value) {
			Bubble3DPropertyChangedHistoryItem historyItem = new Bubble3DPropertyChangedHistoryItem(DocumentModel, this, bubble3D, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetBubble3DCore(bool value) {
			this.bubble3D = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		#region ISeries members
		public override ChartSeriesType SeriesType { get { return ChartSeriesType.Bubble; } }
		public override ChartType ChartType { get { return BubbleChartView.GetChartType(bubble3D); } }
		public override ISeries CloneTo(IChartView view) {
			BubbleSeries result = new BubbleSeries(view);
			result.CopyFrom(this);
			return result;
		}
		public override bool IsCompatible(IChartView view) {
			if(view == null)
				return false;
			return view.ViewType == ChartViewType.Bubble;
		}
		public override void Visit(ISeriesVisitor visitor) {
			visitor.Visit(this);
		}
		public override void OnDataChanged() {
			base.OnDataChanged();
			if (bubbleSize != null)
				bubbleSize.OnContentVersionChanged();
		}
		#endregion
		public override void CopyFrom(ISeries value) {
			base.CopyFrom(value);
			ISupportsInvertIfNegative serieWithInvertIfNegative = value as ISupportsInvertIfNegative;
			if (serieWithInvertIfNegative != null)
				CopyFromCore(serieWithInvertIfNegative);
			BubbleSeries series = value as BubbleSeries;
			if (series != null)
				CopyFromCore(series);
		}
		void CopyFromCore(BubbleSeries value) {
			Bubble3D = value.Bubble3D;
			BubbleSize = value.BubbleSize.CloneTo(DocumentModel);
		}
		void CopyFromCore(ISupportsInvertIfNegative value) {
			InvertIfNegative = value.InvertIfNegative;
		}
		protected override bool CanRemoveOnResetToStyle(DataPoint dataPoint) {
			return dataPoint.InvertIfNegative == InvertIfNegative && dataPoint.Bubble3D == Bubble3D;
		}
		protected override bool IsCompatibleLabelPosition(DataLabelPosition position) {
			return
				position == DataLabelPosition.Default ||
				position == DataLabelPosition.Left ||
				position == DataLabelPosition.Top ||
				position == DataLabelPosition.Right ||
				position == DataLabelPosition.Bottom ||
				position == DataLabelPosition.Center;
		}
		public override System.Collections.Generic.IEnumerable<IDataReference> GetDataReferences() {
			foreach (IDataReference dataReference in base.GetDataReferences())
				yield return dataReference;
			yield return BubbleSize;
		}
		#region Notifications
		public override void OnRangeInserting(InsertRangeNotificationContext context) {
			base.OnRangeInserting(context);
		}
		public override void OnRangeRemoving(RemoveRangeNotificationContext context) {
			base.OnRangeRemoving(context);
		}
		#endregion
	}
}
