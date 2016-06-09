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
using System.Windows;
using DevExpress.Map.Native;
using DevExpress.Utils.Design.DataAccess;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	[CustomBindingProperty("Mappings.Latitude", DXMapStrings.LatitudeDataMemberCaption, DXMapStrings.LatitudeDataMemberDescription)]
	[CustomBindingProperty("Mappings.Longitude", DXMapStrings.LongitudeDataMemberCaption, DXMapStrings.LongitudeDataMemberDescription)]
	public class ListSourceCustomBindingPropertiesAttribute : CustomBindingPropertiesAttribute {
	}
	[ListSourceCustomBindingPropertiesAttribute]
	public class ListSourceDataAdapter : DataSourceAdapterBase {
		public static readonly DependencyProperty MappingsProperty = DependencyPropertyManager.Register("Mappings",
			typeof(MapItemMappingInfo), typeof(ListSourceDataAdapter), new PropertyMetadata(null, MappingsPropertyChanged));
		public static readonly DependencyProperty ItemSettingsProperty = DependencyPropertyManager.Register("ItemSettings",
			typeof(MapItemSettings), typeof(ListSourceDataAdapter), new PropertyMetadata(null, ItemSettingsPropertyChanged));
		static MapItemSettingsBase DefaultItemSettings = new MapCustomElementSettings();
		protected internal override MapItemSettingsBase ActualItemSettings { get { return ItemSettings != null ? ItemSettings : DefaultItemSettings; } }
		[Category(Categories.Data)]
		public MapItemSettings ItemSettings {
			get { return (MapItemSettings)GetValue(ItemSettingsProperty); }
			set { SetValue(ItemSettingsProperty, value); }
		}
		[Category(Categories.Data)]
		public MapItemMappingInfo Mappings {
			get { return (MapItemMappingInfo)GetValue(MappingsProperty); }
			set { SetValue(MappingsProperty, value); }
		}
		protected internal override MapItemMappingInfoBase DataMappings { get { return this.Mappings; } }
		protected internal override IMapDataEnumerator CreateDataEnumerator(MapDataController controller) {
			return new ListDataEnumerator(controller);
		}
		protected override MapDependencyObject CreateObject() {
			return new ListSourceDataAdapter();
		}
	}
}
