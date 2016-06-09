#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
namespace DevExpress.DashboardWin.Design {
	public abstract class CreatableListPropertyTypeConverter<TValue> : ListPropertyTypeConverter<TValue> where TValue : class, new() {
		protected override bool IsSupportNullValue { get { return true; } }
		protected override string NullValueCaption { get { return "(none)"; } }
		protected override ICollection<TValue> GetValues(ITypeDescriptorContext context) {
			TValue value = context.PropertyDescriptor.GetValue(context.Instance) as TValue;
			if(value == null)
				value = CreateValue();
			return new TValue[] { value };
		}
		protected override string GetValueCaption(ITypeDescriptorContext context, TValue value) {
			return string.Format("({0})", value.GetType().Name);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
		protected virtual TValue CreateValue() {
			return new TValue();
		}
	}
	public class CreatableDataItemPropertyTypeConverter<TDataItem> : CreatableListPropertyTypeConverter<TDataItem> where TDataItem : DataItem, new() {
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class CreatableMeasurePropertyTypeConverter : CreatableDataItemPropertyTypeConverter<Measure> {
		internal override PropertiesListProvider<Measure> CreatePropertiesProvider(ITypeDescriptorContext context, Measure value, Attribute[] attributes) {
			return new MeasurePropertiesProvider(context, value, attributes);
		}
	}
	public class CreatableDimensionPropertyTypeConverter : CreatableDataItemPropertyTypeConverter<Dimension> {
		internal override PropertiesListProvider<Dimension> CreatePropertiesProvider(ITypeDescriptorContext context, Dimension value, Attribute[] attributes) {
			return new DimensionPropertiesProvider(context, value, attributes);
		}
	}
	public class MeasurePropertyTypeConverter : ListPropertyTypeConverter<Measure> {
		protected override bool IsSupportNullValue { get { return false; } }
		protected override string NullValueCaption { get { return string.Empty; } }
		protected override ICollection<Measure> GetValues(ITypeDescriptorContext context) {
			SelectedContextService selectedContextService = GetSelectedContextService(context);
			return selectedContextService.GetUniqueMeasures();
		}
		protected override string GetValueCaption(ITypeDescriptorContext context, Measure value) {
			return value != null ? value.DisplayName : string.Empty;
		}
	}
	public class SortByMeasurePropertyTypeConverter : MeasurePropertyTypeConverter {
		protected override bool IsSupportNullValue { get { return true; } }
	}
	public class CreatableMapPaletteConverter : ListPropertyTypeConverter<MapPalette> {
		protected override bool IsSupportNullValue { get { return true; } }
		protected override string NullValueCaption { get { return "(none)"; } }
		protected override ICollection<MapPalette> GetValues(ITypeDescriptorContext context) {
			return new MapPalette[] { new GradientPalette(), new CustomPalette() };
		}
		protected override string GetValueCaption(ITypeDescriptorContext context, MapPalette value) {
			return string.Format("({0})", value.GetType().Name);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class CreatableMapScaleConverter : ListPropertyTypeConverter<MapScale> {
		protected override bool IsSupportNullValue { get { return false; } }
		protected override string NullValueCaption { get { return null; } }
		protected override ICollection<MapScale> GetValues(ITypeDescriptorContext context) {
			return new MapScale[] { new UniformScale(), new CustomScale() };
		}
		protected override string GetValueCaption(ITypeDescriptorContext context, MapScale value) {
			return string.Format("({0})", value.GetType().Name);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class DataProcessingModeConverter : TypeConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<ListItem> list = new List<ListItem>();
			foreach(DataProcessingMode value in Enum.GetValues(typeof(DataProcessingMode))) {
				list.Add(new ListItem(value, value.ToString()));
			}
			return new StandardValuesCollection(list);
		}
	}
}
