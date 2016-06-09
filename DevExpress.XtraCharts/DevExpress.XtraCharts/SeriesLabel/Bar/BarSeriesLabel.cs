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
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum BarSeriesLabelPosition {
		Auto,
		Top,
		Center,
		TopInside,
		BottomInside
	}
	public abstract class BarSeriesLabel : SeriesLabelBase {
		const int DefaultLabelIndent = 2;
		const bool DefaultShowForZeroLabels = false;
		BarSeriesLabelPosition position;
		bool showForZeroValues = DefaultShowForZeroLabels;
		int indent = DefaultLabelIndent;
		protected internal virtual BarSeriesLabelPosition DefaultPosition { get { return BarSeriesLabelPosition.Auto; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BarSeriesLabelShowForZeroValues"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BarSeriesLabel.ShowForZeroValues"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowForZeroValues {
			get { return showForZeroValues; }
			set {
				if (value == showForZeroValues)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				showForZeroValues = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BarSeriesLabelPosition"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BarSeriesLabel.Position"),
		Category(Categories.Behavior),
		TypeConverter(typeof(BarSeriesLabelPositionTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public BarSeriesLabelPosition Position {
			get { return position; }
			set {
				if (value != position) {
					if (!CheckPosition(value))
						throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectBarSeriesLabelPosition));
					SendNotification(new ElementWillChangeNotification(this));
					position = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BarSeriesLabelIndent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BarSeriesLabel.Indent"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int Indent {
			get { return indent; }
			set {
				if (value != indent) {
					if (value < 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectBarSeriesLabelIndent));
					SendNotification(new ElementWillChangeNotification(this));
					indent = value;
					RaiseControlChanged();
				}
			}
		}
		protected BarSeriesLabel()
			: base() {
			position = DefaultPosition;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "ShowForZeroValues")
				return ShouldSerializeShowForZeroValues();
			if (propertyName == "Position")
				return ShouldSerializePosition();
			if (propertyName == "Indent")
				return ShouldSerializeIndent();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeShowForZeroValues() {
			return showForZeroValues != DefaultShowForZeroLabels;
		}
		void ResetShowForZeroValues() {
			ShowForZeroValues = DefaultShowForZeroLabels;
		}
		bool ShouldSerializePosition() {
			return this.position != DefaultPosition;
		}
		void ResetPosition() {
			Position = DefaultPosition;
		}
		bool ShouldSerializeIndent() {
			return this.indent != DefaultLabelIndent;
		}
		void ResetIndent() {
			Indent = DefaultLabelIndent;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeShowForZeroValues() ||
				ShouldSerializePosition() ||
				ShouldSerializeIndent();
		}
		#endregion
		protected internal virtual bool CheckPosition(BarSeriesLabelPosition position) {
			return true;
		}
		protected internal virtual void AssignBarLabelPosition(BarSeriesLabel label) { }
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			BarSeriesLabel label = obj as BarSeriesLabel;
			if (label == null)
				return;
			showForZeroValues = label.showForZeroValues;
			AssignBarLabelPosition(label);
			indent = label.indent;
		}
	}
}
namespace DevExpress.XtraCharts.Design {
	public class BarSeriesLabelPositionTypeConverter : EnumTypeConverter {
		public BarSeriesLabelPositionTypeConverter(Type type)
			: base(type) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection collection = base.GetStandardValues(context);
			BarSeriesLabel label = TypeConverterHelper.GetElement<BarSeriesLabel>(context.Instance);
			if (label == null || label.SeriesBase == null || !(label.SeriesBase.View is StackedBarSeriesView))
				return collection;
			List<BarSeriesLabelPosition> list = new List<BarSeriesLabelPosition>();
			foreach (BarSeriesLabelPosition labelPosition in Enum.GetValues(typeof(BarSeriesLabelPosition)))
				if (labelPosition != BarSeriesLabelPosition.Top && labelPosition != BarSeriesLabelPosition.Auto)
					list.Add(labelPosition);
			return new StandardValuesCollection(list);
		}
	}
}
