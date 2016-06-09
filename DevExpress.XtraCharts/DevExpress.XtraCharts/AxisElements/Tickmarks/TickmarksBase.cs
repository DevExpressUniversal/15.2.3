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
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class TickmarksBase : ChartElement {
		const bool DefaultVisible = true;
		const bool DefaultMinorVisible = true;
		const bool DefaultCrossAxis = false;
		const int DefaultThickness = 1;
		const int DefaultMinorThickness = 1;
		const int DefaultLength = 5;
		const int DefaultMinorLength = 2;
		bool visible = DefaultVisible;
		bool minorVisible = DefaultMinorVisible;
		bool crossAxis = DefaultCrossAxis;
		int thickness = DefaultThickness;
		int minorThickness = DefaultMinorThickness;
		int length = DefaultLength;
		int minorLength = DefaultMinorLength;
		protected internal AxisBase Axis { get { return (AxisBase)base.Owner; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TickmarksBaseVisible"),
#endif
		DXDisplayName(typeof(ResFinder), 
		DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TickmarksBase.Visible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return visible; }
			set {
				if (value != visible) {
					SendNotification(new ElementWillChangeNotification(this));
					visible = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TickmarksBaseMinorVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TickmarksBase.MinorVisible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool MinorVisible {
			get { return minorVisible; }
			set {
				if (value != minorVisible) {
					SendNotification(new ElementWillChangeNotification(this));
					minorVisible = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TickmarksBaseCrossAxis"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TickmarksBase.CrossAxis"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool CrossAxis {
			get { return crossAxis; }
			set {
				if (value != crossAxis) {
					SendNotification(new ElementWillChangeNotification(this));
					crossAxis = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TickmarksBaseThickness"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TickmarksBase.Thickness"),
		XtraSerializableProperty
		]
		public int Thickness {
			get { return thickness; }
			set {
				if (value < 1)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectTickmarkThickness));
				if (value != thickness) {
					SendNotification(new ElementWillChangeNotification(this));
					thickness = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TickmarksBaseMinorThickness"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TickmarksBase.MinorThickness"),
		XtraSerializableProperty
		]
		public int MinorThickness {
			get { return minorThickness; }
			set {
				if (value < 1)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectTickmarkMinorThickness));
				if (value != minorThickness) {
					SendNotification(new ElementWillChangeNotification(this));
					minorThickness = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TickmarksBaseLength"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TickmarksBase.Length"),
		XtraSerializableProperty
		]
		public int Length {
			get { return length; }
			set {
				if (value < 1)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectTickmarkLength));
				if (value != length) {
					SendNotification(new ElementWillChangeNotification(this));
					length = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TickmarksBaseMinorLength"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TickmarksBase.MinorLength"),
		XtraSerializableProperty
		]
		public int MinorLength {
			get { return minorLength; }
			set {
				if (value < 1)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectTickmarkMinorLength));
				if (value != minorLength) {
					SendNotification(new ElementWillChangeNotification(this));
					minorLength = value;
					RaiseControlChanged();
				}
			}
		}
		protected TickmarksBase(AxisBase axis) : base(axis) {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Visible")
				return ShouldSerializeVisible();
			if (propertyName == "MinorVisible")
				return ShouldSerializeMinorVisible();
			if (propertyName == "CrossAxis")
				return ShouldSerializeCrossAxis();
			if (propertyName == "Thickness")
				return ShouldSerializeThickness();
			if (propertyName == "MinorThickness")
				return ShouldSerializeMinorThickness();
			if (propertyName == "Length")
				return ShouldSerializeLength();
			if (propertyName == "MinorLength")
				return ShouldSerializeMinorLength();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeVisible() {
			return this.visible != DefaultVisible;
		}
		void ResetVisible() {
			Visible = DefaultVisible;
		}
		bool ShouldSerializeMinorVisible() {
			return this.minorVisible != DefaultMinorVisible;
		}
		void ResetMinorVisible() {
			MinorVisible = DefaultMinorVisible;
		}
		bool ShouldSerializeCrossAxis() {
			return this.crossAxis != DefaultCrossAxis;
		}
		void ResetCrossAxis() {
			CrossAxis = DefaultCrossAxis;
		}
		bool ShouldSerializeThickness() {
			return this.thickness != DefaultThickness;
		}
		void ResetThickness() {
			Thickness = DefaultThickness;
		}
		bool ShouldSerializeMinorThickness() {
			return this.minorThickness != DefaultMinorThickness;
		}
		void ResetMinorThickness() {
			MinorThickness = DefaultMinorThickness;
		}
		bool ShouldSerializeLength() {
			return this.length != DefaultLength;
		}
		void ResetLength() {
			Length = DefaultLength;
		}
		bool ShouldSerializeMinorLength() {
			return this.minorLength != DefaultMinorLength;
		}
		void ResetMinorLength() {
			MinorLength = DefaultMinorLength;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeVisible() ||
				ShouldSerializeMinorVisible() ||
				ShouldSerializeCrossAxis() ||
				ShouldSerializeThickness() ||
				ShouldSerializeMinorThickness() ||
				ShouldSerializeLength() ||
				ShouldSerializeMinorLength();
		}
		#endregion
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			TickmarksBase tickmarks = obj as TickmarksBase;
			if (tickmarks == null)
				return;
			visible = tickmarks.visible;
			minorVisible = tickmarks.minorVisible;
			crossAxis = tickmarks.crossAxis;
			thickness = tickmarks.thickness;
			minorThickness = tickmarks.minorThickness;
			length = tickmarks.length;
			minorLength = tickmarks.minorLength;
		}
	}
}
