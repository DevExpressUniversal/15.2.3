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
using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.XtraMap;
namespace DevExpress.XtraMap.Design {
	public class DataProviderPickerEditor : GenericTypePickerEditor<MapDataProviderBase> {
		protected override GenericTypePickerControl<MapDataProviderBase> CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new DataProviderPickerControl(this, value);
		}
		public override object CreateInstanceByType(Type type) {
			object providerInstance = base.CreateInstanceByType(type);
			OpenStreetMapDataProvider osmDataProvider = providerInstance as OpenStreetMapDataProvider;
			if(osmDataProvider != null) {
				osmDataProvider.TileUriTemplate = DesignSR.OpenStreetInvalidTileUriTemplate;
				return osmDataProvider;
			}
			return providerInstance;
		}
	}
	public class DataProviderPickerControl : GenericTypePickerControl<MapDataProviderBase> {
		public DataProviderPickerControl(GenericTypePickerEditor<MapDataProviderBase> editor, object editValue)
			: base(editor, editValue) {
		}
	}
	public class InformationDataProviderPickerEditor : GenericTypePickerEditor<InformationDataProviderBase> {
		protected override GenericTypePickerControl<InformationDataProviderBase> CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new InformationDataProviderPickerControl(this, value);
		}
	}
	public class InformationDataProviderPickerControl : GenericTypePickerControl<InformationDataProviderBase> {
		public InformationDataProviderPickerControl(GenericTypePickerEditor<InformationDataProviderBase> editor, object editValue)
			: base(editor, editValue) {
		}
	}
	public class MapItemValueProviderPickerEditor : GenericTypePickerEditor<IMeasuredItemValueProvider> {
		protected override GenericTypePickerControl<IMeasuredItemValueProvider> CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new MapItemValueProviderPickerControl(this, value);
		}
	}
	public class MapItemValueProviderPickerControl : GenericTypePickerControl<IMeasuredItemValueProvider> {
		public MapItemValueProviderPickerControl(GenericTypePickerEditor<IMeasuredItemValueProvider> editor, object editValue)
			: base(editor, editValue) {
		}
	}
}
