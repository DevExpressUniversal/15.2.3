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
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
using DevExpress.Utils;
using DevExpress.Map;
using System.Drawing.Design;
namespace DevExpress.XtraMap {
	public abstract class MiniMapBehavior : ISupportObjectChanged {
		event EventHandler ISupportObjectChanged.Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal virtual CoordPoint Center { get { return null; } }
		protected internal abstract double CalculateZoomLevel(double value);
		#region ISupportObjectChanged Members
		EventHandler onChanged;
		protected virtual void RaiseChanged() {
			if(onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
	}
	public class FixedMiniMapBehavior : MiniMapBehavior {
		double zoomLevel = InnerMap.DefaultZoomLevel;
		CoordPoint centerPoint = InnerMap.DefaultCenterPoint;
		[Category(SRCategoryNames.Behavior), 
#if !SL
	DevExpressXtraMapLocalizedDescription("FixedMiniMapBehaviorZoomLevel"),
#endif
		DefaultValue(InnerMap.DefaultZoomLevel)]
		public double ZoomLevel {
			get { return zoomLevel; }
			set {
				if (zoomLevel != value) {
					zoomLevel = value;
					OnZoomLevelChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("FixedMiniMapBehaviorCenterPoint"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category(SRCategoryNames.Behavior),
		TypeConverter("DevExpress.XtraMap.Design.CoordPointTypeConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign),
		RefreshProperties(RefreshProperties.All)]
		public CoordPoint CenterPoint {
			get { return centerPoint; }
			set {
				if (centerPoint != value) {
					centerPoint = value;
					OnCenterPointChanged();
				}
			}
		}
		void ResetCenterPoint() { CenterPoint = GeoPointFactory.Instance.CreatePoint(0, 0); }
		bool ShouldSerializeCenterPoint() { return !Object.Equals(CenterPoint, GeoPointFactory.Instance.CreatePoint(0, 0)); }
		protected internal override CoordPoint Center { get { return CenterPoint; } }
		protected virtual void OnZoomLevelChanged() {
			RaiseChanged();
		}
		protected virtual void OnCenterPointChanged() {
			RaiseChanged();
		}
		protected internal override double CalculateZoomLevel(double value) {
			return ZoomLevel;
		}
		public override string ToString() {
			return "(FixedMiniMapBehavior)";
		}
	}
	public class DynamicMiniMapBehavior : MiniMapBehavior {
		internal const double DefaultZoomOffset = -3;
		double zoomOffset = DefaultZoomOffset;
		double minZoomLevel = InnerMap.DefaultMinZoomLevel;
		double maxZoomLevel = InnerMap.DefaultMaxZoomLevel;
		[Category(SRCategoryNames.Behavior), 
#if !SL
	DevExpressXtraMapLocalizedDescription("DynamicMiniMapBehaviorZoomOffset"),
#endif
		DefaultValue(DefaultZoomOffset)]
		public double ZoomOffset {
			get { return zoomOffset; }
			set {
				if (zoomOffset != value) {
					zoomOffset = MathUtils.MinMax(value, MinZoomOffset, MaxZoomOffset);
					OnZoomOffsetChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("DynamicMiniMapBehaviorMinZoomLevel"),
#endif
		Category(SRCategoryNames.Map), DefaultValue(InnerMap.DefaultMinZoomLevel)]
		public double MinZoomLevel {
			get { return minZoomLevel; }
			set {
				double zoomLevel = ValidateMinMaxZoomLevel(value);
				zoomLevel = ValidateMinZoomLevel(zoomLevel);
				minZoomLevel = zoomLevel;
				OnMinMaxZoomLevelChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("DynamicMiniMapBehaviorMaxZoomLevel"),
#endif
		Category(SRCategoryNames.Map), DefaultValue(InnerMap.DefaultMaxZoomLevel)]
		public double MaxZoomLevel {
			get { return maxZoomLevel; }
			set {
				double zoomLevel = ValidateMinMaxZoomLevel(value);
				zoomLevel = ValidateMaxZoomLevel(zoomLevel);
				maxZoomLevel = zoomLevel;
				OnMinMaxZoomLevelChanged();
			}
		}
		protected virtual double MinZoomOffset { get { return -20; } }
		protected virtual double MaxZoomOffset { get { return 20; } }
		internal double ValidateMinMaxZoomLevel(double value) {
			return MathUtils.MinMax(value, InnerMap.LimitMinZoomLevel, InnerMap.LimitMaxZoomLevel);
		}
		internal double ValidateMinZoomLevel(double value) {
			return value > MaxZoomLevel ? MaxZoomLevel : value;
		}
		internal double ValidateMaxZoomLevel(double value) {
			return value < MinZoomLevel ? MinZoomLevel : value;
		}
		protected virtual void OnZoomOffsetChanged() {
			RaiseChanged();
		}
		protected virtual void OnMinMaxZoomLevelChanged() {
			RaiseChanged();
		}
		protected internal override double CalculateZoomLevel(double value) {
			return MathUtils.MinMax(value + ZoomOffset, MinZoomLevel, MaxZoomLevel);
		}
		public override string ToString() {
			return "(DynamicMiniMapBehavior)";
		}
	}
}
