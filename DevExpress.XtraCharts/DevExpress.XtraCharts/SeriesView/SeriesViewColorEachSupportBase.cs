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

using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class SeriesViewColorEachSupportBase : XYDiagramSeriesViewBase, IColorEachSupportView {
		const bool DefaultColorEach = false;
		bool colorEach = DefaultColorEach;
		protected internal override bool ActualColorEach { get { return ColorEach; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesViewColorEachSupportBaseColorEach"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesViewColorEachSupportBase.ColorEach"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool ColorEach {
			get { return colorEach; }
			set {
				if (value != colorEach) {
					SendNotification(new ElementWillChangeNotification(this));
					colorEach = value;
					RaiseControlChanged();
				}
			}
		}
		protected SeriesViewColorEachSupportBase() : base() { }
		#region XtraShouldSerialize
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "ColorEach")
				return ShouldSerializeColorEach();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeColorEach() {
			return colorEach != DefaultColorEach;
		}
		void ResetColorEach() {
			ColorEach = DefaultColorEach;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeColorEach();
		}
		#endregion
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			IColorEachSupportView view = obj as IColorEachSupportView;
			if (view != null)
				colorEach = view.ColorEach;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			SeriesViewColorEachSupportBase view = (SeriesViewColorEachSupportBase)obj;
			return colorEach == view.colorEach;
		}
	}
}
