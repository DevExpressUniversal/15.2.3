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
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
		"System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SimpleMarker : MarkerBase {
		int defaultSize;
		int size;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SimpleMarkerSize"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SimpleMarker.Size"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int Size {
			get { return size; } 
			set {
				if (value < 1)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectMarkerSize));
				SendNotification(new ElementWillChangeNotification(this));
				size = value;
				RaiseControlChanged();
			}
		}
		internal SimpleMarker(ChartElement owner, int defaultSize) : base(owner) {
			InitializeSize(defaultSize);
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Size")
				return ShouldSerializeSize();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeSize() {
			return this.size != this.defaultSize;
		}
		void ResetSize() {
			Size = this.defaultSize;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeSize();
		}
		#endregion
		void InitializeSize(int defaultSize) {
			this.defaultSize = defaultSize;
			size = defaultSize;
		}
		protected override ChartElement CreateObjectForClone() {
			return new SimpleMarker(null, 0);
		}
		protected virtual internal IPolygon CalculatePolygon(GRealPoint2D origin, bool isSelected) {
			return base.CalculatePolygon(origin, isSelected, size);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SimpleMarker marker = obj as SimpleMarker;
			if (marker == null)
				return;
			size = marker.size;
		}
		public override bool Equals(object obj) {
			SimpleMarker marker = obj as SimpleMarker;
			return marker != null && size == marker.size && base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
