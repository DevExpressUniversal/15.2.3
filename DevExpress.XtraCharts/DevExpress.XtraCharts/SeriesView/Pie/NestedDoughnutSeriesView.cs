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
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using System.Drawing;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(NestedDoughnutSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class NestedDoughnutSeriesView : DoughnutSeriesView, INestedDoughnutSeriesView {
		const double DefaultWeight = 1.0;
		const double DefaultInnerIndent = 5.0;
		const int DefaultHolePercent = 40;
		double weight = DefaultWeight;
		double innerIndent = DefaultInnerIndent;
		bool isOutside = true;
		object group = null;
		protected override bool NeedSeriesGroupsInteraction { get { return true; } }
		protected override int DefaultHoleRadiusPercent { get { return DefaultHolePercent; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnNestedDoughnut); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("NestedDoughnutSeriesViewWeight"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.NestedDoughnutSeriesView.Weight"),
		XtraSerializableProperty
		]
		public double Weight {
			get { return weight; }
			set {
				if (weight != value) {
					if (value <= 0.0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectNestedDoughnutWeight));
					SendNotification(new ElementWillChangeNotification(this));
					weight = value;
					NestedDoughnutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("NestedDoughnutSeriesViewInnerIndent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.NestedDoughnutSeriesView.InnerIndent"),
		XtraSerializableProperty
		]
		public double InnerIndent {
			get { return innerIndent; }
			set {
				if (innerIndent != value) {
					if (value < 0.0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectNestedDoughnutInnerIndent));
					SendNotification(new ElementWillChangeNotification(this));
					innerIndent = value;
					NestedDoughnutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("NestedDoughnutSeriesViewGroup"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.NestedDoughnutSeriesView.Group"),
		TypeConverter(typeof(ObjectTypeConverter)),
		Editor("DevExpress.XtraCharts.Design.SeriesGroupTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		NonTestableProperty
		]
		public object Group {
			get { return group; }
			set {
				if (!object.Equals(group, value)) {
					SendNotification(new ElementWillChangeNotification(this));
					group = value;
					NestedDoughnutChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public string GroupSerializable {
			get { return ObjectToStringConversion.ObjectToString(Group); }
			set { Group = ObjectToStringConversion.StringToObject(value); }
		}
		public NestedDoughnutSeriesView() : base() {
		}
		public NestedDoughnutSeriesView(int[] explodedPointIds) : base(explodedPointIds) {
		}
		#region INestedDoughnutSeriesView
		bool? INestedDoughnutSeriesView.IsOutside {
			get { return isOutside; }
			set { isOutside = value.Value; }
		}
		bool INestedDoughnutSeriesView.HasExplodedPoints(IRefinedSeries refinedSeries) {
			return HasExplodedPoints(refinedSeries);
		}
		double INestedDoughnutSeriesView.HoleRadiusPercent {
			get { return HolePercent; }
		}
		#endregion
		#region ISupportSeriesGroups implementation
		object ISupportSeriesGroups.SeriesGroup { get { return Group; } set { Group = value; } }
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Weight")
				return ShouldSerializeWeight();
			if (propertyName == "InnerIndent")
				return ShouldSerializeInnerIndent();
			if (propertyName == "GroupSerializable")
				return ShouldSerializeGroupSerializable();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeWeight() {
			return weight != DefaultWeight;
		}
		void ResetWeight() {
			Weight = DefaultWeight;
		}
		bool ShouldSerializeInnerIndent() {
			return innerIndent != DefaultInnerIndent;
		}
		void ResetInnerIndent() {
			InnerIndent = DefaultInnerIndent;
		}
		bool ShouldSerializeGroupSerializable() {
			return GroupSerializable != string.Empty;
		}
		bool ShouldSerializeGroup() {
			return Group != null;
		}
		void ResetGroup() {
			Group = null;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeWeight() || ShouldSerializeInnerIndent();
		}
		#endregion
		void NestedDoughnutChanged() {
			RaiseControlChanged(new SeriesGroupsInteractionUpdateInfo(this));
		}
		float CalculatePieSemiaxis(INestedDoughnutRefinedSeries nestedDoughnutSeries, double domenSize) {
			double radius = domenSize / 2.0;
			double wholeThickness = (1 - nestedDoughnutSeries.HoleRadius) * radius;
			double sizeWithoutIndents = wholeThickness - nestedDoughnutSeries.TotalGroupIndentInPixels;
			double outerOffsetInPixels = sizeWithoutIndents * nestedDoughnutSeries.StartOffset + nestedDoughnutSeries.StartOffsetInPixels;
			return (float)((radius - outerOffsetInPixels) * 2.0);
		}
		protected internal override SizeF CalculatePieSize(IRefinedSeries refinedSeries, SizeF domainSize) {
			INestedDoughnutRefinedSeries nestedDoughnutSeries = (INestedDoughnutRefinedSeries)refinedSeries;
			float majorSemiaxis = CalculatePieSemiaxis(nestedDoughnutSeries, domainSize.Width);
			float minorSemiaxis = CalculatePieSemiaxis(nestedDoughnutSeries, domainSize.Height);
			return new SizeF(majorSemiaxis, minorSemiaxis);
		}
		protected internal override double CalculateActualHolePercent(IRefinedSeries refinedSeries, SizeF domainSize) {
			INestedDoughnutRefinedSeries nestedDoughnutSeries = (INestedDoughnutRefinedSeries)refinedSeries;
			double radius = Math.Max(domainSize.Width, domainSize.Height) / 2.0;
			double wholeThickness = (1 - nestedDoughnutSeries.HoleRadius) * radius;
			double sizeWithoutIndents = wholeThickness - nestedDoughnutSeries.TotalGroupIndentInPixels;
			double outerOffsetInPixels = sizeWithoutIndents * nestedDoughnutSeries.StartOffset + nestedDoughnutSeries.StartOffsetInPixels;
			double holePercent = ((1.0 - ((sizeWithoutIndents * nestedDoughnutSeries.NormalizedWeight) / (radius - outerOffsetInPixels))) * 100.0);
			return Math.Min(holePercent, 100.0);
		}
		protected internal override double GetExplodedFactor(IRefinedSeries refinedSeries) {
			INestedDoughnutRefinedSeries nestedDoughnutSeries = (INestedDoughnutRefinedSeries)refinedSeries;
			return nestedDoughnutSeries.ExplodedFactor;
		}
		protected override bool CanExplodePoint(IPiePoint pointInfo) {
			return isOutside;
		}
		protected internal override bool IsExploded(IRefinedSeries refinedSeries) {
			INestedDoughnutRefinedSeries nestedDoughnutSeries = (INestedDoughnutRefinedSeries)refinedSeries;
			return nestedDoughnutSeries.IsExploded;
		}
		protected override ChartElement CreateObjectForClone() {
			return new NestedDoughnutSeriesView();
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.FullStackedGroupViewPointPatterns;
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new NestedDoughnutSeriesLabel();
		}
		protected override SeriesInteractionContainer CreateSeriesGroupsContainer() {
			return new NestedDoughnutInteractionContainer(this);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			NestedDoughnutSeriesView view = obj as NestedDoughnutSeriesView;
			if (view == null)
				return;
			weight = view.weight;
			innerIndent = view.innerIndent;
			group = view.group;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			NestedDoughnutSeriesView view = (NestedDoughnutSeriesView)obj;
			return weight == view.weight && innerIndent == view.innerIndent && object.Equals(group, view.group);
		}
	}
}
