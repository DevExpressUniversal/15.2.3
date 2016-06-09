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

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[
	DesignerSerializer(TypeNames.DataItemContainerCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class ValueMap : ChoroplethMap {
		const string ValueKey = "Value";
		readonly MapColorizerInternal colorizerInternal;
		readonly NameBox valueNameBox = new NameBox("ValueName");
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ValueMapValue"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Measure Value {
			get { return (Measure)GetDataItem(ValueKey); }
			set { SetDataItem(ValueKey, value); }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ValueMapValueName"),
#endif
		DefaultValue(null),
		Localizable(true)
		]
		public string ValueName { get { return valueNameBox.Name; } set { valueNameBox.Name = value; } }
		string DefaultValueName {
			get {
				Measure value = Value;
				return value != null ? value.DisplayName : string.Empty;
			}
		}
		internal string ValueDisplayName { get { return valueNameBox.DisplayName; } } 
		[
		Editor(TypeNames.TypeListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMapPaletteConverter),
		DefaultValue(null)
		]
		public MapPalette Palette {
			get { return colorizerInternal.Palette; }
			set { colorizerInternal.Palette = value; }
		}
		[
		Editor(TypeNames.TypeListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMapScaleConverter),
		DefaultValue(null)
		]
		public MapScale Scale {
			get { return colorizerInternal.Scale; }
			set { colorizerInternal.Scale = value; }
		}
		protected internal override string DefaultName { get { return DefaultValueName; } }
		protected internal override string LayerId { get { return "GridMeasureColumn"; } }
		public ValueMap()
			: this(null) {
		}
		public ValueMap(Measure value)
			: base(new DataItemDescription[] { 
				new DataItemDescription(ValueKey, DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue), value) }) {
			colorizerInternal = new MapColorizerInternal(this);
			colorizerInternal.OnChanged += (sender, e) => OnChanged(ChangeReason.View);
			valueNameBox.NameChanged += (sender, e) => OnChanged(ChangeReason.View);
			valueNameBox.RequestDefaultName += (sender, e) => e.DefaultName = DefaultValueName;
		}
		protected override DataItemContainer CreateInstance() {
			return new ValueMap();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			valueNameBox.SaveToXml(element);
			colorizerInternal.SaveToXml(element);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			valueNameBox.LoadFromXml(element);
			colorizerInternal.LoadFromXml(element);
		}
		internal override void OnEndLoading() {
			base.OnEndLoading();
			colorizerInternal.OnEndLoading();
		}
		protected internal override EditNameDescription GetEditNameDescription() {
			if (Value != null)
				return new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionMapValue), 
					new IEditNameProvider[] { this, valueNameBox });
			return null;
		}
		protected internal override DataItemContainerActualContent GetActualContent() {
			DataItemContainerActualContent content = new DataItemContainerActualContent();
			if (Value != null)
				content.Measures.Add(Value);
			return content;
		}
	}
}
