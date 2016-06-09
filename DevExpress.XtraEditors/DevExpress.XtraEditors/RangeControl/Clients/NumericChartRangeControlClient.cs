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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Sparkline.Core;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors {
	[ToolboxBitmap(typeof(ToolboxIconsRootNS), "NumericChartRangeControlClient"), ToolboxTabName(AssemblyInfo.DXTabCommon), DXToolboxItem(DXToolboxItemKind.Regular)]
	public class NumericChartRangeControlClient : ChartRangeControlClientBase {
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.NumericChartRangeControlClient.GridOptions"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NumericChartRangeControlClientGridOptions"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public new NumericChartRangeControlClientGridOptions GridOptions {
			get { return (NumericChartRangeControlClientGridOptions)base.GridOptions; }
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.NumericChartRangeControlClient.Range"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NumericChartRangeControlClientRange"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public new NumericChartRangeControlClientRange Range {
			get { return (NumericChartRangeControlClientRange)base.Range; }
		}
		public NumericChartRangeControlClient() : base(SparklineScaleType.Numeric, new NumericChartRangeControlClientGridOptions(), new NumericChartRangeControlClientRange()) { }
		protected override double ValidateValue(object value) {
			return Convert.ToDouble(value);
		}
		protected override object NativeValue(double value) {
			if (SparklineMathUtils.IsValidDouble(value))
				return value;
			return 0.0;
		}
		protected internal override List<object> CreateFakeData() {
			List<object> fakeData = new List<object>();
			Random random = new Random(RandomSeed);
			double unit;
			double baseArgument;
			if (Range.Auto) {
				unit = GridOptions.GridSpacing;
				baseArgument = 0;
			} else {
				baseArgument = Range.Min;
				unit = (Range.Max - Range.Min) / (FakePointsCount - 1);
			}
			for (int seriesIndexer = 0; seriesIndexer < FakeSeriesCount; seriesIndexer++) {
				for (int i = 0; i < FakePointsCount; i++) {
					fakeData.Add(new FakeDataPoint<double>(seriesIndexer.ToString(),  baseArgument + i * unit, random.NextDouble() - 0.5));
				}
			}
			return fakeData;
		}
	}
}
