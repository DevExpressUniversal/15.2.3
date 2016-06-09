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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI.Design.WebControls;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils.Design;
using DevExpress.Web.Internal;
using DevExpress.XtraEditors.Design;
namespace DevExpress.Web.Design {
	public class StandardValuesConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return false;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value == null)
				return string.Empty;
			if(value is string)
				return value;
			throw GetConvertFromException(value);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			object[] objects = { };
			return new StandardValuesCollection(objects);
		}
	}
	public class CursorConverter : StandardValuesConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			object[] objects = { "auto", "crosshair", "default", "pointer", "move", "text", "wait", "help",
				"e-resize", "ne-risize", "nw-resize", "n-resize", "se-resize", "sw-resize", "s-resize", "w-resize"};
			return new StandardValuesCollection(objects);
		}
	}
	public class ThemeTypeConverter : StandardValuesConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<string> themes = new List<string>(ThemesProvider.GetThemes(true));
			themes.Insert(0, ThemesProvider.DefaultTheme);
			return new StandardValuesCollection(themes);
		}
	}
	public class BackgroundHorizontalPositionConverter : StandardValuesConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			object[] objects = { "left", "center", "right" };
			return new StandardValuesCollection(objects);
		}
	}
	public class BackgroundVerticalPositionConverter : StandardValuesConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			object[] objects = { "top", "center", "bottom" };
			return new StandardValuesCollection(objects);
		}
	}
	public class IconIDConverter : StandardValuesConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<string> icons = new List<string>();
			foreach(DXImageGalleryCategory category in DXImageGalleryStorage.Default.DataModel.Categories) {
				foreach(DXImageGalleryItem item in category.Items)
					icons.Add(GetIconIDByResourcePath(item.Name));
			}
			return new StandardValuesCollection(icons);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return false;
		}
		public static string GetIconIDByResourcePath(string path) {
			Regex regex = new Regex("\\W+", RegexOptions.Singleline);
			string[] parts = path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
			if(parts.Length == 3) {
				string id = parts[1] + "_" + Path.GetFileNameWithoutExtension(parts[2]);
				if(parts[0].ToLowerInvariant().Contains(IconsHelper.GrayScalePostfix))
					id += IconsHelper.GrayScalePostfix;
				else if(parts[0].ToLowerInvariant().Contains(IconsHelper.Office2013Postfix))
					id += IconsHelper.Office2013Postfix;
				else if(parts[0].ToLowerInvariant().Contains(IconsHelper.DevAVPostfix))
					id += IconsHelper.DevAVPostfix;
				return regex.Replace(id, string.Empty);
			}
			return path;
		}
	}
	public class ColorSchemeConverter : TypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return value;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			return value;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(ASPxWebClientUIControl.AvailableColorSchemes);
		}
	}
	public static class ConverterHelper {
		public static object DiscoverObjectInstance(object instance) {
			return DiscoverObjectInstance<Object>(instance);
		}
		public static T DiscoverObjectInstance<T>(object instance) {
			return DiscoverObjectInstance<T>(instance, (object o) => { return o is T ? (T)o : default(T); });
		}
		public static T DiscoverObjectInstance<T>(object instance, Func<object, T> FindInstance) {
			if(instance == null)
				return default(T);
			var wrapper = instance as IDXObjectWrapper;
			if(wrapper != null && wrapper.SourceObject is T)
				return (T)wrapper.SourceObject;
			var objects = GetFilterObjects(instance);
			if(objects == null)
				return FindInstance(instance);
			foreach(var item in objects) {
				var result = DiscoverObjectInstance<T>(item.SourceObject, FindInstance);
				if(result != null)
					return result;
			}
			return default(T);
		}
		static FilterObject[] GetFilterObjects(object instance) {
			if(instance is FilterObject)
				return new FilterObject[] { (FilterObject)instance };
			if(instance is FilterObject[])
				return (FilterObject[])instance;
			return null;
		}
	}
	public class DXDataSourceIDConverter<T> : DataSourceIDConverter where T : ASPxDataWebControl {
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return base.GetStandardValues(new DXTypeDescriptorContext<T>(context));
		}
	}
	public class DXTypeDescriptorContext<T> : ITypeDescriptorContext where T : ASPxDataWebControl {
		public DXTypeDescriptorContext(ITypeDescriptorContext sourceContext)
			: base() {
			SourceContext = sourceContext;
		}
		ITypeDescriptorContext SourceContext { get; set; }
		IContainer ITypeDescriptorContext.Container { get { return SourceContext.Container; } }
		object ITypeDescriptorContext.Instance {
			get {
				var instance = SourceContext.Instance;
				return instance is T ? instance : ConverterHelper.DiscoverObjectInstance<T>(SourceContext.Instance);
			}
		}
		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor { get { return SourceContext.PropertyDescriptor; } }
		object IServiceProvider.GetService(Type serviceType) {
			return SourceContext.GetService(serviceType);
		}
		void ITypeDescriptorContext.OnComponentChanged() {
			SourceContext.OnComponentChanged();
		}
		bool ITypeDescriptorContext.OnComponentChanging() {
			return SourceContext.OnComponentChanging();
		}
	}
	public abstract class StringListConverterBase : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<string> list = new List<string>();
			FillList(context, list);
			list.Sort();
			list.Insert(0, string.Empty);
			return new StandardValuesCollection(list);
		}
		protected abstract void FillList(ITypeDescriptorContext context, List<string> list);
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return false;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is string && value.ToString() == "(None)") return string.Empty;
			if(value is string) return value.ToString();
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(value.ToString() == string.Empty) return "(None)";
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class GridFormatConditionExpressionEditor: UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			var edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(edSvc == null)
				return null;
			var designerHost = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			using(ExpressionEditorForm form = new GridFormatRuleExpressionEditorForm(DXObjectWrapper.GetInstance(context), designerHost, value)) {
				if(edSvc.ShowDialog(form) == DialogResult.OK)
					return form.Expression;
			}
			return base.EditValue(context, provider, value);
		}
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return System.Drawing.Design.UITypeEditorEditStyle.Modal;
		}
	}
	public class ColumnsPropertiesCommonEditor : TypeEditorBase {
		public override Form CreateEditorForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue) {
			var editProperties = context.Instance as EditPropertiesBase;
			var comboBoxPropertiesDesigner = new ColumnsPropertiesCommonFormDesigner(component, provider, editProperties);
			var form = new WrapperEditorForm(comboBoxPropertiesDesigner, false);
			form.Text = String.Format("{0} Editor", comboBoxPropertiesDesigner.GetActiveDesignerItem().Caption);
			return form;
		}
	}
	public class GridViewUnboundExpressionEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null || provider == null)
				return value;
			var column = GetGridViewDataColumn(context.Instance);
			var columnInfo = column != null ? new DataColumnInfoWrapper(column) : null;
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(edSvc != null) {
				IDesignerHost designerHost = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				using(ExpressionEditorForm form = new UnboundColumnExpressionEditorForm(columnInfo, designerHost)) {
					if(edSvc.ShowDialog(form) == DialogResult.OK) {
						return form.Expression;
					}
				}
			}
			return base.EditValue(context, provider, value);
		}
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return System.Drawing.Design.UITypeEditorEditStyle.Modal;
		}
		protected GridViewDataColumn GetGridViewDataColumn(object instance){
			var dataColumn = instance is GridViewDataColumn ? (GridViewDataColumn)instance : ConverterHelper.DiscoverObjectInstance<GridViewDataColumn>(instance);
			if(dataColumn == null && instance is IEnumerable) {
				var objects = (IEnumerable)instance;
				foreach(var item in objects){
					dataColumn = ConverterHelper.DiscoverObjectInstance<GridViewDataColumn>(item);
					if(dataColumn != null)
						break;
				}
			}
			return dataColumn;
		}
	}
}
