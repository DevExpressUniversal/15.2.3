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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.Utils;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum AxisLabelResolveOverlappingMode {
		None = ResolveOverlappingModeCore.None,
		HideOverlapped = ResolveOverlappingModeCore.HideOverlapped
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(AxisLabelResolveOverlappingOptionsTypeConverter))
	]
	public class AxisLabelResolveOverlappingOptions : ChartElement, IAxisLabelResolveOverlappingOptions {
		const int DefaultMinIndent = 5;
		const bool DefaultAllowStagger = true;
		const bool DefaultAllowRotate = true;
		const bool DefaultAllowHide = true;
		bool allowStagger = DefaultAllowStagger;
		bool allowRotate = DefaultAllowRotate;
		bool allowHide = DefaultAllowHide;
		int minIndent = DefaultMinIndent;
		internal bool AllowResolveOverlapping { get { return ResolveOverlappingUtils.AllowResolveOverlapping(this); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelResolveOverlappingOptionsAllowStagger"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabelResolveOverlappingOptions.AllowStagger"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Localizable(true),
		XtraSerializableProperty
		]
		public bool AllowStagger {
			get {
				return allowStagger;
			}
			set {
				if (allowStagger != value) {
					SendNotification(new ElementWillChangeNotification(this));
					allowStagger = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelResolveOverlappingOptionsAllowRotate"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabelResolveOverlappingOptions.AllowRotate"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Localizable(true),
		XtraSerializableProperty
		]
		public bool AllowRotate {
			get {
				return allowRotate;
			}
			set {
				if (allowRotate != value) {
					SendNotification(new ElementWillChangeNotification(this));
					allowRotate = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelResolveOverlappingOptionsAllowHide"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabelResolveOverlappingOptions.AllowHide"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Localizable(true),
		XtraSerializableProperty
		]
		public bool AllowHide {
			get {
				return allowHide;
			}
			set {
				if (allowHide != value) {
					SendNotification(new ElementWillChangeNotification(this));
					allowHide = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabelResolveOverlappingOptionsMinIndent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabelResolveOverlappingOptions.MinIndent"),
		Localizable(true),
		XtraSerializableProperty
		]
		public int MinIndent {
			get {
				return minIndent;
			}
			set {
				if (minIndent != value) {
					SendNotification(new ElementWillChangeNotification(this));
					minIndent = value;
					RaiseControlChanged();
				}
			}
		}
		internal AxisLabelResolveOverlappingOptions() : base () { }
		#region ShouldSerialize & Reset
		bool ShouldSerializeAllowStagger() {
			return allowStagger != DefaultAllowStagger;
		}
		void ResetAllowStagger() {
			AllowStagger = DefaultAllowStagger;
		}
		bool ShouldSerializeAllowRotate() {
			return allowRotate != DefaultAllowRotate;
		}
		void ResetAllowRotate() {
			AllowRotate = DefaultAllowRotate;
		}
		bool ShouldSerializeAllowHide() {
			return allowHide != DefaultAllowHide;
		}
		void ResetAllowHide() {
			AllowHide = DefaultAllowHide;
		}
		bool ShouldSerializeMinIndent() {
			return minIndent != DefaultMinIndent;
		}
		void ResetMinIndent() {
			MinIndent = DefaultMinIndent;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() ||
				ShouldSerializeAllowStagger() ||
				ShouldSerializeAllowRotate() ||
				ShouldSerializeAllowHide() ||
				ShouldSerializeMinIndent();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "AllowStagger":
					return ShouldSerializeAllowStagger();
				case "AllowRotate":
					return ShouldSerializeAllowRotate();
				case "AllowHide":
					return ShouldSerializeAllowHide();
				case "MinIndent":
					return ShouldSerializeMinIndent();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new AxisLabelResolveOverlappingOptions();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			AxisLabelResolveOverlappingOptions options = obj as AxisLabelResolveOverlappingOptions;
			if (options != null) {
				allowStagger = options.allowStagger;
				allowRotate = options.allowRotate;
				allowHide = options.allowHide;
				minIndent = options.minIndent;
			}
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public static class ResolveOverlappingUtils {
		public static bool AllowResolveOverlapping(AxisLabelResolveOverlappingOptions axisLabelOverlappingOptions) {
			return axisLabelOverlappingOptions.AllowStagger || axisLabelOverlappingOptions.AllowRotate || axisLabelOverlappingOptions.AllowHide;	 
		}
	}
}
