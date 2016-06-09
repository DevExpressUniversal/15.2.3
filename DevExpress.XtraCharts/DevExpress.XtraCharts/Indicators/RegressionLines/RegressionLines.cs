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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RegressionLines : ChartElement, IXtraSupportDeserializeCollectionItem, IEnumerable<RegressionLine> {
		List<RegressionLine> deserializedLines = new List<RegressionLine>();
		XYDiagram2DSeriesViewBase View { get { return Owner as XYDiagram2DSeriesViewBase; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		NonTestableProperty
		]
		public RegressionLine this[ValueLevel valueLevel] {
			get {
				XYDiagram2DSeriesViewBase view = View;
				if (view == null) 
					return null;
				foreach (Indicator indicator in view.Indicators) {
					RegressionLine regressionLine = indicator as RegressionLine;
					if (regressionLine != null && regressionLine.ValueLevel == valueLevel)
						return regressionLine;
				}
				RegressionLine newRegressionLine = new RegressionLine(valueLevel);
				newRegressionLine.Visible = false;
				view.Indicators.Add(newRegressionLine);
				return newRegressionLine;
			}
			set {
				value.ValueLevel = valueLevel;
				XYDiagram2DSeriesViewBase view = View;
				if (view != null) {
					IndicatorCollection indicators = view.Indicators;
					int count = indicators.Count;
					for (int i = 0; i < count; i++) {
						RegressionLine regressionLine = indicators[i] as RegressionLine;
						if (regressionLine != null && regressionLine.ValueLevel == valueLevel) {
							indicators.RemoveAt(i);
							indicators.Insert(i, value);
							return;
						}
					}
					indicators.Add(value);
				}
			}
		}
		[
		NonTestableProperty,
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NestedTagProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public List<RegressionLine> LinesSerializable { get { return deserializedLines; } }
		public RegressionLines(XYDiagram2DSeriesViewBase view) : base(view) {
		}
		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<RegressionLine>)this).GetEnumerator();
		}
		IEnumerator<RegressionLine> IEnumerable<RegressionLine>.GetEnumerator() {
			XYDiagram2DSeriesViewBase view = View;
			if (view == null)
				yield return null;
			foreach (Indicator indicator in view.Indicators) {
				RegressionLine regressionLine = indicator as RegressionLine;
				if (regressionLine != null)
					yield return regressionLine;
			}
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return false;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if (propertyName == "LinesSerializable")
				deserializedLines.Add((RegressionLine)e.Item.Value);
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if (propertyName != "LinesSerializable")
				return null;
			RegressionLine regressionLine = new RegressionLine();
			regressionLine.Visible = false;
			return regressionLine;
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new RegressionLines(null);
		}
		internal void OnEndLoading() {
			XYDiagram2DSeriesViewBase view = View;
			if (view != null) {
				foreach (RegressionLine line in deserializedLines)
					view.Indicators.Add(line);
				deserializedLines.Clear();
			}
		}
		protected internal override bool ShouldSerialize() {
			return false;
		}
	}
}
