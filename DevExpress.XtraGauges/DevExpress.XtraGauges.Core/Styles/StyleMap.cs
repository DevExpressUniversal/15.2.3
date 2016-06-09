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
using System.Collections.Generic;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraGauges.Core.Styles {
	public static class GaugesStyleMapService {
		public static IStyleMap Resolve() {
			return new StyleMapProvider();
		}
		class StyleMapProvider : IStyleMap {
			public IDictionary<Type, string[]> AllowedProperties {
				get { return StyleMap.typeProperties; }
			}
			public IDictionary<string, string> PropertyPathAliases {
				get { return StyleMap.pathAliases; }
			}
			public void EnsureDefaultStyles(StyleCollection styles) {
				StyleMap.EnsureDefaultStyles(styles);
			}
			public bool IsStyleProperty(string propertyName, string propertyPath) {
				return StyleMap.IsStyleProperty(propertyName, propertyPath);
			}
			public bool IsStyleProperty(string propertyName, Type type) {
				return StyleMap.IsStyleProperty(propertyName, type);
			}
			public bool TryGetType(string typeName, out Type type) {
				return StyleMap.TryGetType(typeName, out type);
			}
			public bool TryGetKnownTypePropertyValue(string path, IXtraPropertyCollection store, out object value) {
				return StyleMap.TryGetKnownTypePropertyValue(path, store, out value);
			}
		}
		static class StyleMap {
			static IDictionary<string, Type> types;
			internal static IDictionary<Type, string[]> typeProperties;
			static IDictionary<string, string[]> pathProperties;
			internal static IDictionary<string, string> pathAliases;
			static IDictionary<string, IKnownTypeActivator> knownTypeActivators;
			#region internal classes
			interface IKnownTypeActivator {
				object CreateActivator();
				object GetValue(object activator);
			}
			class KnownTypeActivator<TActivator, T> : IKnownTypeActivator
				where TActivator : new() {
				DevExpress.Utils.Function<T, TActivator> ProvideValue;
				public KnownTypeActivator(DevExpress.Utils.Function<T, TActivator> provideValue) {
					ProvideValue = provideValue;
				}
				public object CreateActivator() {
					return new TActivator();
				}
				public object GetValue(object activator) {
					return ProvideValue((TActivator)activator);
				}
			}
			#endregion internal classes
			static StyleMap() {
				types = new Dictionary<string, Type>();
				types.Add("CircularGauge", typeof(IGauge));
				types.Add("LinearGauge", typeof(IGauge));
				types.Add("DigitalGauge", typeof(IGauge));
				types.Add("ArcScaleComponent", typeof(ArcScale));
				types.Add("ArcScaleNeedleComponent", typeof(ArcScaleNeedle));
				types.Add("ArcScaleBackgroundLayerComponent", typeof(ArcScaleBackgroundLayer));
				types.Add("ArcScaleEffectLayerComponent", typeof(ArcScaleEffectLayer));
				types.Add("ArcScaleSpindleCapComponent", typeof(ArcScaleSpindleCap));
				types.Add("LinearScaleComponent", typeof(LinearScale));
				types.Add("LinearScaleLevelComponent", typeof(LinearScaleLevel));
				types.Add("LinearScaleBackgroundLayerComponent", typeof(LinearScaleBackgroundLayer));
				types.Add("LinearScaleEffectLayerComponent", typeof(LinearScaleEffectLayer));
				types.Add("DigitalBackgroundLayerComponent", typeof(DigitalBackgroundLayer));
				types.Add("DigitalEffectLayerComponent", typeof(DigitalEffectLayer));
				typeProperties = new Dictionary<Type, string[]>();
				typeProperties.Add(typeof(IGauge), new string[] { "Orientation", 
					"AppearanceOn","AppearanceOff",
					"AppearanceOn.BorderBrush", "AppearanceOn.ContentBrush", "AppearanceOn.BorderWidth", 
					"AppearanceOff.BorderBrush", "AppearanceOff.ContentBrush", "AppearanceOff.BorderWidth"});
				typeProperties.Add(typeof(ArcScale), new string[] { "Center", "StartAngle", "EndAngle", "MajorTickmark", "MinorTickmark", "RadiusX", "RadiusY", "AppearanceTickmarkText", "AppearanceScale", "Ranges" });
				typeProperties.Add(typeof(ArcScaleNeedle), new string[] { "ShapeType", "StartOffset", "EndOffset" });
				typeProperties.Add(typeof(ArcScaleBackgroundLayer), new string[] { "ShapeType", "ScaleCenterPos", "Size" });
				typeProperties.Add(typeof(ArcScaleEffectLayer), new string[] { "ShapeType", "ScaleCenterPos", "Size" });
				typeProperties.Add(typeof(ArcScaleSpindleCap), new string[] { "ShapeType", "Size", "ZOrder" });
				typeProperties.Add(typeof(LinearScale), new string[] { "StartPoint", "EndPoint", "MajorTickmark", "MinorTickmark", "AppearanceScale", "AppearanceTickmarkText", "Ranges" });
				typeProperties.Add(typeof(LinearScaleLevel), new string[] { "ShapeType" });
				typeProperties.Add(typeof(LinearScaleBackgroundLayer), new string[] { "ShapeType", "ScaleStartPos", "ScaleEndPos" });
				typeProperties.Add(typeof(LinearScaleEffectLayer), new string[] { "ShapeType", "ScaleStartPos", "ScaleEndPos" });
				typeProperties.Add(typeof(DigitalBackgroundLayer), new string[] { "ShapeType" });
				typeProperties.Add(typeof(DigitalEffectLayer), new string[] { "ShapeType" });
				pathProperties = new Dictionary<string, string[]>();
				pathProperties.Add("Ranges", new string[] { "StartThickness", "EndThickness", "ShapeOffset", "AppearanceRange" });
				pathProperties.Add("MajorTickmark", GetProperties(typeof(IMajorTickmark)));
				pathProperties.Add("MinorTickmark", GetProperties(typeof(IMinorTickmark)));
				pathProperties.Add("AppearanceTickmarkText", GetProperties(typeof(Core.Drawing.BaseTextAppearance)));
				pathProperties.Add("AppearanceScale", GetProperties(typeof(Core.Drawing.BaseScaleAppearance)));
				pathProperties.Add("AppearanceOn", GetProperties(typeof(Core.Drawing.BaseShapeAppearance)));
				pathProperties.Add("AppearanceOff", GetProperties(typeof(Core.Drawing.BaseShapeAppearance)));
				pathProperties.Add("Ranges.AppearanceRange", GetProperties(typeof(Core.Drawing.BaseShapeAppearance)));
				pathAliases = new Dictionary<string, string>();
				pathAliases.Add("AppearanceTickmarkText", "MajorTickmark.TextShape.AppearanceText");
				pathAliases.Add("AppearanceScale", "Appearance");
				knownTypeActivators = new Dictionary<string, IKnownTypeActivator>();
				knownTypeActivators.Add("TextBrush", new KnownTypeActivator<Core.Drawing.BaseTextAppearance, Core.Drawing.BrushObject>(
					delegate(Core.Drawing.BaseTextAppearance appearance) { return appearance.TextBrush; }));
				knownTypeActivators.Add("Brush", new KnownTypeActivator<Core.Drawing.BaseScaleAppearance, Core.Drawing.BrushObject>(
					 delegate(Core.Drawing.BaseScaleAppearance appearance) { return appearance.Brush; }));
				knownTypeActivators.Add("ContentBrush", new KnownTypeActivator<Core.Drawing.BaseShapeAppearance, Core.Drawing.BrushObject>(
					delegate(Core.Drawing.BaseShapeAppearance appearance) { return appearance.ContentBrush; }));
				knownTypeActivators.Add("BorderBrush", new KnownTypeActivator<Core.Drawing.BaseShapeAppearance, Core.Drawing.BrushObject>(
					delegate(Core.Drawing.BaseShapeAppearance appearance) { return appearance.BorderBrush; }));
			}
			public static bool TryGetType(string name, out Type type) {
				return types.TryGetValue(name, out type);
			}
			public static bool IsStyleProperty(string property, Type type) {
				string[] props;
				return typeProperties.TryGetValue(type, out props) && Array.IndexOf(props, property) != -1;
			}
			public static bool IsStyleProperty(string property, string path) {
				string[] props;
				return pathProperties.TryGetValue(RemoveIndexator(path), out props) && Array.IndexOf(props, property) != -1;
			}
			static string RemoveIndexator(string path) {
				int openBracket = path.IndexOf('[');
				int closeBracket = path.IndexOf(']');
				if(openBracket != -1 && closeBracket != -1)
					path = path.Remove(openBracket, closeBracket - openBracket + 1);
				return path;
			}
			public static bool TryGetKnownTypePropertyValue(string path, IXtraPropertyCollection store, out object value) {
				value = null;
				IKnownTypeActivator activator;
				if(knownTypeActivators.TryGetValue(path, out activator)) {
					object target = activator.CreateActivator();
					new DeserializeHelper().DeserializeObject(target, store, null);
					value = activator.GetValue(target);
					return true;
				}
				return false;
			}
			static string[] GetProperties(Type type) {
				List<string> result = new List<string>();
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
				foreach(PropertyDescriptor pd in properties) {
					if(!pd.IsReadOnly) result.Add(pd.Name);
				}
				return result.ToArray();
			}
			public static string ReplaceAliases(string path) {
				string[] pathElements = path.Split('.');
				string property; string alias;
				for(int i = 0; i < pathElements.Length; i++) {
					property = pathElements[i];
					pathElements[i] = pathAliases.TryGetValue(property, out alias) ? alias : property;
				}
				return string.Join(".", pathElements);
			}
			public static void EnsureDefaultStyles(StyleCollection styles) {
				if(styles.Key.Scope == "Circular") {
					if(!styles.Contains(typeof(ArcScaleEffectLayer))) {
						Style style = new Style(typeof(ArcScaleEffectLayer));
						StyleMap.AllowProperties(style);
						style.Setters.Add("ShapeType", EffectLayerShapeType.Empty);
						styles.Add(style);
					}
					if(!styles.Contains(typeof(ArcScaleSpindleCap))) {
						Style style = new Style(typeof(ArcScaleSpindleCap));
						StyleMap.AllowProperties(style);
						style.Setters.Add("ShapeType", SpindleCapShapeType.Empty);
						styles.Add(style);
					}
					Style scaleStyle;
					if(styles.TryGetStyle(typeof(ArcScale), out scaleStyle)) 
						EnsureRangesDefaultStyle(scaleStyle);
				}
				if(styles.Key.Scope == "Linear") {
					if(!styles.Contains(typeof(LinearScaleEffectLayer))) {
						Style style = new Style(typeof(LinearScaleEffectLayer));
						StyleMap.AllowProperties(style);
						style.Setters.Add("ShapeType", EffectLayerShapeType.Empty);
						styles.Add(style);
					}
					Style scaleStyle;
					if(styles.TryGetStyle(typeof(LinearScale), out scaleStyle))
						EnsureRangesDefaultStyle(scaleStyle);
				}
				if(styles.Key.Scope == "Digital") {
					if(!styles.Contains(typeof(DigitalEffectLayer))) {
						Style style = new Style(typeof(DigitalEffectLayer));
						StyleMap.AllowProperties(style);
						style.Setters.Add("ShapeType", DigitalEffectShapeType.Empty);
						styles.Add(style);
					}
				}
			}
			static void EnsureRangesDefaultStyle(Style scaleStyle) {
				object value;
				for(int i = 0; i < 3; i++) {
					string rangeBorderBrush = string.Concat("Ranges[", i.ToString(), "].AppearanceRange.BorderBrush");
					if(!scaleStyle.Setters.TryGetValue(rangeBorderBrush, out value))
						scaleStyle.Setters.Add(rangeBorderBrush, Drawing.BrushObject.Empty);
					string rangeContentBrush = string.Concat("Ranges[", i.ToString(), "].AppearanceRange.ContentBrush");
					if(!scaleStyle.Setters.TryGetValue(rangeContentBrush, out value))
						scaleStyle.Setters.Add(rangeContentBrush, Drawing.BrushObject.Empty);
				}
			}
			static void AllowProperties(Style style) {
				style.Setters.AllowProperties(typeProperties[style.TargetType]);
			}
		}
	}
}
