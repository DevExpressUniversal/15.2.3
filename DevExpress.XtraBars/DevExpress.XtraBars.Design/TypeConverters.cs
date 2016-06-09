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

using System.Drawing.Design;
using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.Ribbon;
using System.Collections.Generic;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.Ribbon.Design;
namespace DevExpress.XtraBars.TypeConverters {
	public class BarItemAppearanceConverter : ExpandableObjectConverter {
		public BarItemAppearanceConverter() {
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection properties = base.GetProperties(context, value, attributes);
			if(!ShouldUseBorderColorProperty(value))
				return properties;
			return AddBorderColorPropertyToPDCollection(properties, value);
		}
		protected virtual bool ShouldUseBorderColorProperty(object value) {
			BarItemAppearance itemAppearance = value as BarItemAppearance;
			return itemAppearance != null ? BarUtilites.IsBelongsToRadialMenuManager(itemAppearance) : false;
		}
		protected virtual PropertyDescriptorCollection AddBorderColorPropertyToPDCollection(PropertyDescriptorCollection properties, object value) {
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value);
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor pd in properties)
				list.Add(pd);
			PropertyDescriptor borderColorPd = pdc["BorderColor"];
			if(borderColorPd == null)
				return properties;
			List<Attribute> attrList = new List<Attribute>();
			foreach(Attribute attribute in borderColorPd.Attributes)
				attrList.Add(attribute);
			attrList.RemoveAll(attribute => attribute.GetType() == typeof(BrowsableAttribute));
			list.Add(new BorderColorPropertyDescriptor(borderColorPd, attrList.ToArray()));
			return new PropertyDescriptorCollection(list.ToArray(), ((IList)properties).IsReadOnly);
		}
		#region BorderColor property descriptor
		protected class BorderColorPropertyDescriptor : SimplePropertyDescriptor {
			PropertyDescriptor pd;
			public BorderColorPropertyDescriptor(PropertyDescriptor pd, Attribute[] attributes)
				: base(pd.ComponentType, pd.Name, pd.PropertyType, attributes) {
				this.pd = pd;
			}
			public override object GetValue(object component) {
				return pd.GetValue(component);
			}
			public override void SetValue(object component, object value) {
				pd.SetValue(component, value);
			}
			public override bool ShouldSerializeValue(object component) {
				return pd.ShouldSerializeValue(component);
			}
		}
		#endregion
	}
	public class ApplicationButtonDropDownControlTypeConverter : TypeConverter {
		public static string noneItem = "(None)";
		public ApplicationButtonDropDownControlTypeConverter() {
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType.IsSubclassOf(typeof(Control)) || sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType.IsSubclassOf(typeof(Control)) || destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is string) return context.Container.Components[(string)value];
			if((string)value == noneItem)
				return null;
			return value;
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string) && value != null) {
				IComponent component = (IComponent)value;
				return component.Site != null ? component.Site.Name : noneItem;
			}
			if(value == null)
				return noneItem;
			return value;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			ArrayList list = new ArrayList();
			list.Add(null);
			foreach(IComponent comp in context.Container.Components) {
				if(ShouldAddComponentToList(context, comp))
					list.Add(comp);
			}
			return new StandardValuesCollection(list);
		}
		protected virtual bool ShouldAddComponentToList(ITypeDescriptorContext context, IComponent comp) {
			if(!(comp is Control) && !(comp is PopupControl))
				return false;
			RibbonControl ribbon = (RibbonControl)GetControl(context);
			if(ribbon.StatusBar == comp) return false;
			Control c = GetControl(context);
			while(c != null) {
				if(c == comp) return false;
				c = c.Parent;
			}
			return true;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		protected virtual Control GetControl(ITypeDescriptorContext context) {
			return context.Instance as Control;
		}
	}
	public class RibbonActionListApplicationButtonDropDownControlTypeConverter : ApplicationButtonDropDownControlTypeConverter {
		protected override Control GetControl(ITypeDescriptorContext context) {
			RibbonDesignerActionList al = context.Instance as RibbonDesignerActionList;
			if(al != null) {
				return al.Component as Control;
			}
			return base.GetControl(context);
		}
	}
	public sealed class StandaloneBarDockControlTypeConverter : ComponentConverter {
		public StandaloneBarDockControlTypeConverter(Type type)
			: base(type) {
		}
		public static string[] validProp = new string[] { "BackColor", "BackgroundImage", "BackgroundImageLayout", "Margin", "MaximumSize", "MinimumSize", "Modifiers", "Padding", "Tag", "Visible", "Appearance", "Anchor", "Name", "AutoSize", "AutoSizeInLayoutControl", "IsVertical", "Size", "Location", "Dock", "Row", "RowSpan", "Column", "ColumnSpan" };
		public override PropertyDescriptorCollection GetProperties(
			ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection res = base.GetProperties(context, value, attributes);
			PropertyDescriptorCollection newColl = new PropertyDescriptorCollection(null);
			for(int n = res.Count - 1; n >= 0; n--) {
				PropertyDescriptor desc = res[n];
				if(Array.IndexOf(validProp, desc.Name) != -1) newColl.Add(desc);
			}
			StandaloneBarDockControl barDockControl = value as StandaloneBarDockControl;
			if(barDockControl != null && IsInLayoutControl(barDockControl)) {
				newColl.Remove(newColl["AutoSize"]);
			}
			return newColl;
		}
		static bool IsInLayoutControl(StandaloneBarDockControl bdc) {
			return bdc.Parent != null && bdc.Parent.GetType().Name.EndsWith("LayoutControl");
		}
	}
	public sealed class BarDockControlTypeConverter : ComponentConverter {
		public BarDockControlTypeConverter(Type type) : base(type) { 
		}
		public static string[] validProp = new string[] { "BackColor", "BackgroundImage", "Appearance", "DockStyle"};
		public override PropertyDescriptorCollection GetProperties(
			ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection res = base.GetProperties(context, value, attributes);
			PropertyDescriptorCollection newColl = new PropertyDescriptorCollection(null);
			for(int n = res.Count - 1; n >= 0; n--) {
				PropertyDescriptor desc = res[n];
				if(Array.IndexOf(validProp, desc.Name) != -1) newColl.Add(desc);
			}
			return newColl;
		}
	}
	public sealed class GalleryControlClientTypeConverter : ComponentConverter {
		public GalleryControlClientTypeConverter(Type type) : base(type) { }
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return new PropertyDescriptorCollection(null);
		}
	}
	public sealed class DockWindowTypeConverter : ComponentConverter {
		public DockWindowTypeConverter(Type type) : base(type) { 
		}
		public static string[] validCategories = new string[] { "DockWindow", "Appearance"};
		public override PropertyDescriptorCollection GetProperties(
			ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection res = base.GetProperties(context, value, attributes);
			PropertyDescriptorCollection newColl = new PropertyDescriptorCollection(null);
			for(int n = res.Count - 1; n >= 0; n--) {
				PropertyDescriptor desc = res[n];
				if(desc.DesignTimeOnly || Array.IndexOf(validCategories, desc.Category) != -1) newColl.Add(desc);
			}
			return newColl;
		}
	}
	public class BarShortcutTypeConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return KeysList;
		}
		static StandardValuesCollection keysList = null;
		public static StandardValuesCollection KeysList {
			get {
				if(keysList != null) return keysList;
				keysList = CreateList();
				return keysList;
			}
		}
		static StandardValuesCollection CreateList() {
			ArrayList list = new ArrayList();
			list.Add(BarShortcut.Empty);
			FieldInfo[] fis = typeof(Shortcut).GetFields();
			foreach(FieldInfo fi in fis) {
				if(fi.IsSpecialName || !fi.IsStatic) continue;
				Shortcut sh = (Shortcut)fi.GetValue(null);
				if(sh == Shortcut.None) continue;
				list.Add(new BarShortcut(sh));
			}
			return new StandardValuesCollection(list);
		}
	}
	public class PaintStyleNameTypeConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			ArrayList list = new ArrayList();
			BarAndDockingController controller = context.Instance as BarAndDockingController;
			if(controller == null) return null;
			list.Add("Default");
			foreach(DevExpress.XtraBars.Styles.BarManagerPaintStyle ps in controller.PaintStyles) {
				list.Add(ps.Name);
			}
			return new StandardValuesCollection(list);
		}
	}
}
