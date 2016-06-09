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

using DevExpress.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Serialization;
namespace DevExpress.Diagram.Core.Native.Generation {
	public static class DXPropertiesGenerator {
		public static string GenerateDX(PropertiesInfo properties) {
			return Generate(properties, GetDPropertyGenerator(properties.OwnerName));
		}
		public static string GenerateXtra(PropertiesInfo properties) {
			return Generate(properties, GetXtraPropertyGenerator(properties.OwnerName));
		}
		static string Generate(PropertiesInfo properties, Action<StringBuilder, string, string, string, string, string, Modifier?, string, bool, string, string, string, Platform, string, bool?> generate) {
			StringBuilder builder = new StringBuilder();
			properties.Properties.ForEach(x => {
				generate(builder, x.Name, x.Type, x.Default, x.Changed, x.Coerce, null, x.Converter, x.Virtual, x.WinAttributes, x.DefaultValueAttribute, x.WpfAttributes, x.Platform, x.Category, x.Browsable);
			});
			if(properties.ReadOnlyProperties != null) {
				properties.ReadOnlyProperties.ForEach(x => {
					generate(builder, x.Name, x.Type, x.Default, null, null, x.Modifier, null, false, null, null, null, x.Platform, null, x.Browsable);
				});
			}
			var template = @"
    partial class {0} {{
{1}
    }}
";
			return string.Format(template, properties.OwnerName, builder.ToString());
		}
		static Action<StringBuilder, string, string, string, string, string, Modifier?, string, bool, string, string, string, Platform, string, bool?> GetDPropertyGenerator(string ownerType) {
			return (builder, name, type, defaultValue, changed, coerce, modifier, converter, @virtual, winAttributes, defaultValueAttribute, wpfAttributes, platform, category, browsable) => {
				if(modifier == null) {
					builder.AppendLine(string.Format("        public static readonly DependencyProperty {0}Property = ", name));
					string callbacks = string.Empty;
					if(!string.IsNullOrEmpty(changed)) {
						var changedWithSubstitutedValue = string.Format(changed, "((" + type + ")e.OldValue)");
						callbacks += string.Format(", (d, e) => (({0})d).{1}", ownerType, changedWithSubstitutedValue);
					}
					if(!string.IsNullOrEmpty(coerce)) {
						callbacks += string.Format(", (d, o) => (({1})d).{0}(({2})o)", coerce, ownerType, type);
					}
					builder.AppendLine(string.Format(
"          DependencyProperty.Register(\"{0}\", typeof({1}), typeof({2}), new PropertyMetadata({3}{4}));",
						name, type, ownerType, GetDefaultValue(defaultValue, type), callbacks));
					builder.AppendLine(string.Format(
GetConverterAttribute(converter) + GetAttributes(wpfAttributes) +
"        public {0} {1} {{\r\n" +
"            get {{ return ({0})GetValue({1}Property); }}\r\n" +
"            set {{ SetValue({1}Property, value); }}\r\n" +
"        }}\r\n", type, name));
				} else {
					builder.AppendLine(string.Format("        static readonly DependencyPropertyKey {0}PropertyKey = ", name));
					string callbacks = string.Empty;
					builder.AppendLine(string.Format(
"          DependencyProperty.RegisterReadOnly(\"{0}\", typeof({1}), typeof({2}), new PropertyMetadata({3}{4}));",
						name, type, ownerType, GetDefaultValue(defaultValue, type), callbacks));
					builder.AppendLine(string.Format("        public static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;", name));
					builder.AppendLine(string.Format(
"        public {0} {1} {{\r\n" +
"            get {{ return ({0})GetValue({1}Property); }}\r\n" +
"            {2} set {{ SetValue({1}PropertyKey, value); }}\r\n" +
"        }}\r\n", type, name, modifier.ToString().ToLower()));
				}
			};
		}
		static Action<StringBuilder, string, string, string, string, string, Modifier?, string, bool, string, string, string, Platform, string, bool?> GetXtraPropertyGenerator(string ownerType) {
			return (builder, name, type, defaultValue, changed, coerce, modifier, converter, @virtual, winAttributes, defaultValueAttribute, _wpfAttributes, platform, category, browsable) => {
				if(platform == Platform.Wpf) return;
				var fieldName = "_" + name;
				var changedCallback = string.Empty;
				var oldValueStorage = string.Empty;
				var changedWithSubstitutedValue = string.Empty;
				if(!string.IsNullOrEmpty(changed)) {
					if(changed.Contains("{0}"))
						oldValueStorage = "                var oldValue = " + name + ";\r\n";
					changedWithSubstitutedValue = string.Format(changed, "oldValue");
					changedCallback += string.Format(
"                this.{0};\r\n", changedWithSubstitutedValue);
				}
				var coercedValue = fieldName;
				if(!string.IsNullOrEmpty(coerce)) {
					coercedValue = string.Format("this.{0}({1})", coerce, fieldName);
				}
				var virtualModifier = @virtual ? "virtual " : string.Empty;
				var propertiesChanged = modifier == null ? "                OnPropertiesChanged();\r\n" : string.Empty;
				var setterModifier = modifier == null ? string.Empty : modifier.ToString().ToLower() + " ";
				builder.AppendLine(string.Format(
"        {0} {2} = {4};\r\n" +
GetConverterAttribute(converter) + GetDefaultValueAttribute(defaultValueAttribute) + GetCategoryAttribute(category) + GetBrowsableAttribute(browsable) + GetAttributes(winAttributes) + 
"        public {6}{0} {1} {{\r\n" +
"            get {{ return {3}; }}\r\n" +
"            {5}set {{\r\n" +
"                if({1} == value) return;\r\n" +
oldValueStorage +
"                {2} = value;\r\n" +
changedCallback +
propertiesChanged +
"            }}\r\n" +
"        }}\r\n", type, name, fieldName, coercedValue, GetDefaultValue(defaultValue, type), setterModifier, virtualModifier));
			};
		}
		static string GetBrowsableAttribute(bool? browsable) {
			if(!browsable.HasValue) return string.Empty;
			return string.Format("        [Browsable({0})]\r\n", browsable.Value ? "true" : "false");
		}
		static string GetDefaultValueAttribute(string defaultValue) {
			return defaultValue.WithString(x => "        [DefaultValue(" + x + ")]\r\n");
		}
		static string GetCategoryAttribute(string category) {
			return category.WithString(x => "        [DXCategory(CategoryName." + x + ")]\r\n");
		}
		static string GetAttributes(string attributes) {
			if(string.IsNullOrEmpty(attributes)) return string.Empty;
			return attributes.WithString(x => "        [" + attributes + "]\r\n");
		}
		static string GetConverterAttribute(string converter) {
			return converter.WithString(x => "        [TypeConverter(typeof(" + x + "))]\r\n");
		}
		static string GetDefaultValue(string defaultValue, string type) {
			return string.IsNullOrEmpty(defaultValue) ? "default(" + type + ")" : defaultValue;
		}
	}
	public static class CommonProperties {
		public static readonly PropertiesInfo DiagramControl =
			new PropertiesInfo("DiagramControl", new[] {
					Property((IDiagramControl x) => x.MinDragDistance, @default: "3d", platform: Platform.Wpf),
					Property((IDiagramControl x) => x.ActiveTool, @default: "DiagramController.DefaultTool", changed: "Controller.OnActiveToolChanged()", coerce: "Controller.CoerceTool", platform: Platform.Wpf),
					Property((IDiagramControl x) => x.PageSize, @default: "new Size(800, 600)", changed: "Controller.OnExtentChanged()", platform: Platform.Wpf),
					Property((IDiagramControl x) => x.ScrollMargin, @default: "new Thickness(20)", changed: "Controller.OnExtentChanged()", platform: Platform.Wpf),
					Property((IDiagramControl x) => x.BringIntoViewMargin, @default: "10d", platform: Platform.Wpf ),
					Property((IDiagramControl x) => x.ZoomFactor, @default: "1d", changed: "Controller.OnZoomFactorChanged()", coerce: "Controller.CoerceZoom", platform: Platform.Wpf),
					Property((IDiagramControl x) => x.GridSize, changed: "OnGridSizeChanged()", platform: Platform.Wpf ),
					Property((IDiagramControl x) => x.ResizingMode, platform: Platform.Wpf),
					Property((IDiagramControl x) => x.SnapToGrid, @default: "true", platform: Platform.Wpf),
					Property((IDiagramControl x) => x.SnapToItems, @default: "true", platform: Platform.Wpf),
					Property((IDiagramControl x) => x.SnapToItemsDistance, @default: "10d", platform: Platform.Wpf),
					Property((IDiagramControl x) => x.GlueToConnectionPointDistance, @default: "10d", platform: Platform.Wpf ),
					Property((IDiagramControl x) => x.GlueToItemDistance, @default: "7d",  platform: Platform.Wpf),
					Property((IDiagramControl x) => x.MeasureUnit, @default: "MeasureUnits.Pixels", platform: Platform.Wpf),
					Property((IDiagramControl x) => x.AllowEmptySelection, @default: "true", changed: "Selection().ValidateSelection()", platform: Platform.Wpf ),
					Property((IDiagramControl x) => x.Theme, @default: "DiagramThemes.Office", changed: "Controller.OnThemeChanged()", platform: Platform.Wpf),
					Property((IDiagramControl x) => x.CanvasSizeMode, @default: "CanvasSizeMode.AutoSize", changed: "Controller.OnCanvasSizeModeChanged()")
				},
				new[] {
					ReadOnlyProperty("RootToolsModel", "SelectionToolsModel<IDiagramItem>", browsable: false),
					ReadOnlyProperty("SelectionModel", "SelectionModel<IDiagramItem>", browsable: false),
					ReadOnlyProperty("SelectionToolsModel", "SelectionToolsModel<IDiagramItem>", browsable: false),
					ReadOnlyProperty("CanUndo", "bool", browsable: false),
					ReadOnlyProperty("CanRedo", "bool", browsable: false),
					ReadOnlyProperty("HasChanges", "bool", Modifier.Protected, browsable: false, platform: Platform.Wpf),
				}
			);
		public static readonly PropertiesInfo DiagramItem =
			new PropertiesInfo("DiagramItem", new[] {
					Property((IDiagramItem x) => x.Position, changed: "OnPositionChanged({0})", winAttributes: "TypeConverter(typeof(PointFloatConverter))", category:"Layout", @virtual: true),
					Property((IDiagramItem x) => x.Weight, @default: "1d", changed: "NotifyChanged(ItemChangedKind.Bounds)", coerce: "CoerceWeight", platform:Platform.Wpf),
					Property((IDiagramItem x) => x.CanDelete, @default: "true", changed: "NotifyInteractionChanged()", @virtual: true, category:"Behavior"),
					Property((IDiagramItem x) => x.CanResize, @default: "true", changed: "NotifyInteractionChanged()", @virtual: true, category:"Behavior"),
					Property((IDiagramItem x) => x.CanMove, @default: "true", @virtual: true, category:"Behavior"),
					Property((IDiagramItem x) => x.CanCopy, @default: "true", changed: "NotifyInteractionChanged()", category:"Behavior", @virtual: true),
					Property((IDiagramItem x) => x.CanRotate, @default: "true", changed: "NotifyInteractionChanged()", category:"Behavior", @virtual: true),
					Property((IDiagramItem x) => x.CanSnapToThisItem, @default: "true", @virtual: true, category:"Behavior"),
					Property((IDiagramItem x) => x.CanSnapToOtherItems, @default: "true", @virtual: true, category:"Behavior"),
					Property((IDiagramItem x) => x.CanSelect, @default: "true", @virtual: true, category:"Behavior"),
					Property((IDiagramItem x) => x.Anchors, category:"Layout", @virtual: true),
					Property((IDiagramItem x) => x.SelectionLayer, @default: "DefaultSelectionLayer.Instance", changed: "OnSelectionLayerChanged()", coerce: "Controller.CoerceSelectionLayer", browsable:false),
					Property((IDiagramItem x) => x.Stroke, changed: "InvalidateVisual()", browsable:false),
					Property((IDiagramItem x) => x.StrokeThickness, @default: "1d", changed: "InvalidateVisual()", browsable:false),
					Property("StrokeDashArray", "DoubleCollection", changed: "InvalidateVisual()", defaultValueAttribute:"null", browsable:false),
					Property((IDiagramItem x) => x.ThemeStyleId, @default: "DiagramItemStyleId.DefaultStyleId", changed: "Controller.UpdateCustomStyle()", browsable:false),
					Property((IDiagramItem x) => x.CustomStyleId, changed: "Controller.UpdateCustomStyle()", defaultValueAttribute:"null", browsable:false),
					Property((IDiagramItem x) => x.ForegroundId, changed: "Controller.UpdateCustomStyle()", defaultValueAttribute:"null", browsable:false),
					Property((IDiagramItem x) => x.BackgroundId, changed: "Controller.UpdateCustomStyle()", defaultValueAttribute:"null", browsable:false),
					Property((IDiagramItem x) => x.StrokeId, changed: "Controller.UpdateCustomStyle()", defaultValueAttribute:"null", browsable:false),
					Property((IDiagramItem x) => x.Angle, changed: "OnAngleChanged()", coerce: "CoerceAngle", category:"Appearance", @virtual: true),
				},
				new[] {
					ReadOnlyProperty("IsSelected", "bool", browsable:false),
				}
			);
		public static readonly PropertiesInfo DiagramShape =
			new PropertiesInfo("DiagramShape", new[] {
					Property((IDiagramShape x) => x.Shape, @default: "BasicShapes.Rectangle", changed: "OnShapeChanged({0})", wpfAttributes: "PropertyGridEditor(TemplateKey = \"propertyGridComboboxEdit\")", category:"Appearance"),
					Property((IDiagramShape x) => x.Content, changed: "Update()", category:"Appearance"),
					Property("Parameters", "DoubleCollection", changed: "UpdateShape()", defaultValueAttribute:"null", browsable:false),
				}
			);
		public static readonly PropertiesInfo DiagramConnector =
			new PropertiesInfo("DiagramConnector", new[] {
					Property((IDiagramConnector x) => x.BeginArrow, changed: "Controller().OnAppearancePropertyChanged()", wpfAttributes: "PropertyGridEditor(TemplateKey = \"propertyGridComboboxEdit\")", category:"Appearance"),
					Property((IDiagramConnector x) => x.EndArrow, changed: "Controller().OnAppearancePropertyChanged()", wpfAttributes: "PropertyGridEditor(TemplateKey = \"propertyGridComboboxEdit\")", category:"Appearance"),
					Property((IDiagramConnector x) => x.BeginArrowSize, changed: "Controller().OnAppearancePropertyChanged()", category: "Appearance"),
					Property((IDiagramConnector x) => x.EndArrowSize, changed: "Controller().OnAppearancePropertyChanged()", category: "Appearance"),
					Property((IDiagramConnector x) => x.Type, changed: "Controller().OnTypeChanged()", @default:"ConnectorType.RightAngle", wpfAttributes: "PropertyGridEditor(TemplateKey = \"propertyGridComboboxEdit\")", category: "Layout"),
					Property((IDiagramConnector x) => x.Text, category: "Appearance"),
					Property((IDiagramConnector x) => x.BeginPoint, changed: "Controller().OnPointChanged(ConnectorPointType.Begin)", category: "Layout"),
					Property((IDiagramConnector x) => x.EndPoint, changed: "Controller().OnPointChanged(ConnectorPointType.End)", category: "Layout"),
					Property((IDiagramConnector x) => x.BeginItem, changed: "Controller().OnItemChanged(ConnectorPointType.Begin)", defaultValueAttribute: "null", category: "Layout"),
					Property((IDiagramConnector x) => x.EndItem, changed: "Controller().OnItemChanged(ConnectorPointType.End)", defaultValueAttribute: "null", category: "Layout"),
					Property((IDiagramConnector x) => x.BeginItemPointIndex, @default: "-1", changed: "Controller().OnItemPointIndexChanged(ConnectorPointType.Begin)", defaultValueAttribute:"-1", category: "Layout"),
					Property((IDiagramConnector x) => x.EndItemPointIndex, @default: "-1", changed: "Controller().OnItemPointIndexChanged(ConnectorPointType.End)", defaultValueAttribute:"-1", category: "Layout"),
					Property((IDiagramConnector x) => x.Points, changed: "Controller().OnPointsChanged()", defaultValueAttribute:"null", platform: Platform.Wpf),
				}
			);
		public static readonly PropertiesInfo DiagramContainer =
			new PropertiesInfo("DiagramContainer", new[] {
					Property((IDiagramContainer x) => x.IsSnapScope, defaultValueAttribute:"false", category: "Behavior", @virtual: true),
					Property((IDiagramContainer x) => x.AdjustBoundsBehavior, @default: "AdjustBoundaryBehavior.AutoAdjust", @virtual: true, category:"Behavior"),
				}
			);
		public static readonly PropertiesInfo DiagramOrgChartBehavior =
			new PropertiesInfo("DiagramOrgChartBehavior", new[] {
					Property((IOrgChartBehavior x) => x.ChildrenPath),
					Property((IOrgChartBehavior x) => x.ChildrenSelector),
					Property((IOrgChartBehavior x) => x.ItemsSource, changed: "controller.OnItemsSourceChanged()"),
					Property((IOrgChartBehavior x) => x.KeyMember),
					Property((IOrgChartBehavior x) => x.ParentMember),
				}
			);
		#region helper methods
		static PropertyInfo Property<T, TProperty>(
	Expression<Func<T, TProperty>> property,
	string @default = null,
	string changed = null,
	string coerce = null,
	bool @virtual = false,
	string converter = null,
	string winAttributes = null,
	string defaultValueAttribute = null,
	string wpfAttributes = null,
	Platform platform = Platform.All,
	string category = null,
	bool? browsable = null
) {
			return Property(
				((MemberExpression)property.Body).Member.Name,
				GetTypeDisplayName(typeof(TProperty)),
				@default: @default,
				changed: changed,
				coerce: coerce,
				@virtual: @virtual,
				converter: converter,
				winAttributes: winAttributes,
				defaultValueAttribute:defaultValueAttribute,
				wpfAttributes: wpfAttributes,
				platform: platform,
				category: category,
				browsable: browsable
			);
		}
		static string GetTypeDisplayName(Type type) {
			if(!type.IsGenericType)
				return type.Name;
			var name = type.Name.Substring(0, type.Name.IndexOf("`"));
			var args = string.Concat(type.GetGenericArguments().Select(x => GetTypeDisplayName(x)).InsertDelimeter(","));
			return name + "<" + args + ">";
		}
		static PropertyInfo Property(
			string name,
			string type,
			string @default = null,
			string changed = null,
			string coerce = null,
			bool @virtual = false,
			string converter = null,
			string winAttributes = null,
			string defaultValueAttribute = null,
			string wpfAttributes = null,
			Platform platform = Platform.All,
			string category = null,
			bool? browsable = null
		) {
			return new PropertyInfo(
				name: name,
				type: type,
				@default: @default,
				changed: changed,
				coerce: coerce,
				@virtual: @virtual,
				converter: converter,
				winAttributes: winAttributes,
				defaultValueAttribute:defaultValueAttribute,
				wpfAttributes: wpfAttributes,
				platform: platform,
				category: category,
				browsable: browsable
			);
		}
		static ReadOnlyPropertyInfo ReadOnlyProperty(
			string name,
			string type,
			Modifier modifier = Modifier.Private,
			string @default = null,
			bool? browsable = null,
			Platform platform = Platform.All
		) {
			return new ReadOnlyPropertyInfo(
				name: name,
				type: type,
				@default: @default,
				modifier: modifier,
				browsable: browsable,
				platform: platform
			);
		}
		#endregion
	}
	public class PropertiesInfo {
		public readonly string OwnerName;
		public readonly PropertyInfo[] Properties;
		public readonly ReadOnlyPropertyInfo[] ReadOnlyProperties;
		public PropertiesInfo(string ownerName, PropertyInfo[] properties, ReadOnlyPropertyInfo[] readOnlyProperties = null) {
			OwnerName = ownerName;
			Properties = properties;
			ReadOnlyProperties = readOnlyProperties;
		}
	}
	public class PropertyInfoBase {
		public readonly string Name, Type, Default;
		public readonly bool? Browsable;
		public readonly Platform Platform;
		public PropertyInfoBase(string name, string type, string @default, bool? browsable, Platform platform) {
			Name = name;
			Type = type;
			Default = @default;
			Browsable = browsable;
			Platform = platform;
		}
	}
	public class PropertyInfo : PropertyInfoBase {
		public readonly string Changed, Coerce, Converter, WinAttributes, DefaultValueAttribute, WpfAttributes, Category;
		public readonly bool Virtual;
		public PropertyInfo(string name, string type, string @default, string changed, string coerce, string converter, string winAttributes, bool @virtual, string defaultValueAttribute, string wpfAttributes, Platform platform, string category, bool? browsable)
			: base(name, type, @default, browsable, platform) {
			Changed = changed;
			Coerce = coerce;
			Converter = converter;
			WinAttributes = winAttributes;
			Virtual = @virtual;
			DefaultValueAttribute = defaultValueAttribute;
			WpfAttributes = wpfAttributes;
			Category = category;
		}
	}
	public enum Platform {
		All, Win, Wpf
	}
	public enum Modifier {
		Private, Internal, Protected
	}
	public class ReadOnlyPropertyInfo : PropertyInfoBase {
		public readonly Modifier Modifier;
		public ReadOnlyPropertyInfo(string name, string type, string @default, Modifier modifier, bool? browsable, Platform platform) : base(name, type, @default, browsable, platform) {
			Modifier = modifier;
		}
	}
}
