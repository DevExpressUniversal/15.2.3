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

namespace DevExpress.XtraGauges.Core {
	class ResFinder { }
}
namespace DevExpress.XtraGauges.Core.Customization {
	using System.ComponentModel;
	using DevExpress.Utils.Design;
	using DevExpress.XtraGauges.Core.Styles;
	using Compatibility.System.ComponentModel;
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.XtraGauges.Core.ResFinder)),
	]
	public enum DashboardGaugeType { Circular, Linear }
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.XtraGauges.Core.ResFinder)),
	]
	public enum DashboardGaugeTheme { FlatLight, FlatDark }
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.XtraGauges.Core.ResFinder)),
	]
	public enum DashboardGaugeStyle { Vertical, Horizontal, Full, ThreeFourth, Half, QuarterLeft, QuarterRight }
	public static class DashboardGaugeCreator {
		static DashboardGaugeCreator() {
			Factory = new DashboardGaugeModelProviderFactory();
		}
		public static IDashboardGaugeModelProviderFactory Factory { get; private set; }
		public static void BuildModel(IDashboardGauge gauge) {
			IDashboardGaugeModelProvider provider = Factory.Resolve(gauge.Type);
			if(provider != null)
				provider.BuildModel(gauge);
		}
		public static IDashboardGauge CreateGauge(DashboardGaugeType type, DashboardGaugeStyle style, DashboardGaugeTheme theme, bool showMarker, System.IServiceProvider serviceProvider = null) {
			DashboardGauge gauge = new DashboardGauge();
			gauge.Type = type;
			gauge.Theme = theme;
			gauge.Style = style;
			gauge.ShowMarker = showMarker;
			BuildModel(gauge);
			ApplyStyle(gauge, serviceProvider ?? StyleCollectionHelper.ServiceProvider);
			return gauge;
		}
		static void ApplyStyle(DashboardGauge gauge, System.IServiceProvider serviceProvider) {
			StyleCollectionKey key = new StyleCollectionKey(gauge.Type.ToString());
			key.Name = StyleCollectionKey.CheckThemeName(gauge.Theme.ToString(), serviceProvider);
			key.Tag = gauge.Style.ToString();
			StyleCollection styleCollection = StyleCollectionHelper.Load(key, serviceProvider);
			if(styleCollection != null)
				styleCollection.Apply(gauge);
		}
	}
}
