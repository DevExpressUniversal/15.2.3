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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using System.Linq;
using DevExpress.XtraEditors.Helpers;
using System.Drawing.Design;
using DevExpress.XtraEditors.Popup;
namespace DevExpress.XtraEditors.Design {
	public class FormatPredefinedIconUITypeEditor : ObjectPickerEditor {
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return context != null;
		}
		IconSetImageLoader GetImageLoader(ITypeDescriptorContext context) {
			return IconSetImageLoader.GetDefault(null);
		}
		public override void PaintValue(PaintValueEventArgs e) {
			if(e.Value == null) return;
			Image i = GetImageLoader(e.Context).GetImage(e.Value.ToString());
			if(i != null) {
				e.Graphics.DrawImage(i, e.Bounds);
			}
			else {
				e.Graphics.DrawString("X", Control.DefaultFont, Brushes.Red, e.Bounds);
			}
		}
		protected override ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			string current = value == null ? null : value.ToString(); ;
			return new IconPickerFromValuesControl(this, current,  GetImageLoader(context), GetImageLoader(context).GetImageNames(), false, 12, 300);
		}
		class IconPickerFromValuesControl : PickerFromValuesControl {
			IconSetImageLoader imageLoader;
			public IconPickerFromValuesControl(ObjectPickerEditor editor, object editValue, IconSetImageLoader imageLoader, ICollection values, bool sorted, int defaultVisibleItemsCount, int defaultVisibleWidth) :
				base(editor, editValue, values, sorted, defaultVisibleItemsCount, defaultVisibleWidth) {
				this.imageLoader = imageLoader;
			}
			protected override bool SupportCustomDrawItem { get { return true; } }
			protected override void CustomDrawItem(DrawItemEventArgs e) {
				object value = listBox.Items[e.Index];
				string item = value == null ? null : value.ToString();
				Image i = this.imageLoader.GetImage(item.ToString());
				Size imageSize = Rectangle.Inflate(e.Bounds, -1, -1).Size;
				imageSize.Width = imageSize.Height;
				DrawImageItemText(e, item, imageSize);
				if(i != null) DrawImageItemImage(e, i, imageSize);
			}
		}
	}
	public class FormatPredefinedIconSetIconUITypeEditor : UITypeEditor {
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return context != null;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.None;
		}
		public override void PaintValue(PaintValueEventArgs e) {
			if(e.Value == null) return;
			var icon = e.Value as FormatConditionIconSetIcon;
			if(icon == null) return;
			Image i = icon.GetIcon();
			if(i != null) {
				e.Graphics.DrawImage(i, e.Bounds);
			}
			else {
				e.Graphics.DrawString("X", Control.DefaultFont, Brushes.Red, e.Bounds);
			}
		}
	}
	public class FormatPredefinedColorScaleUITypeEditor : ObjectPickerEditor {
		protected override ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			FormatPredefinedColorScale current = null;
			FormatConditionRule2ColorScale rule = DXObjectWrapper.GetInstance(context) as FormatConditionRule2ColorScale;
			if(value != null) {
				current = FormatPredefinedColorScales.Default.Find(rule, value.ToString());
			}
			var res = new PickerFromValuesControl(this, current, FormatPredefinedColorScales.Default.Find(rule.LookAndFeel).OrderBy(q=> q.Title).ToList(), false, 12, 300);
			return res;
		}
		protected override object ConvertFromValue(object oldValue, object newValue) {
			if(newValue != null) {
				FormatPredefinedColorScale scale = newValue as FormatPredefinedColorScale;
				if(scale != null) newValue = scale.Key;
			}
			return newValue;
		}
		public override bool IsDropDownResizable { get { return true; } }
	}
	public class FormatPredefinedAppearancesUITypeEditor : ObjectPickerEditor {
		protected override ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			FormatPredefinedBaseScheme current = null;
			FormatConditionRuleAppearanceBase rule = DXObjectWrapper.GetInstance(context) as FormatConditionRuleAppearanceBase;
			if(value != null) {
				current = FormatPredefinedAppearances.Default.Find(rule, value.ToString());
			}
			var res = new PickerFromValuesControl(this, current, FormatPredefinedAppearances.Default.Find(rule.LookAndFeel).OrderBy(q => q.Title).ToList(), false, 12, 300);
			return res;
		}
		protected override object ConvertFromValue(object oldValue, object newValue) {
			if(newValue != null) {
				FormatPredefinedBaseScheme scale = newValue as FormatPredefinedBaseScheme;
				if(scale != null) newValue = scale.Key;
			}
			return newValue;
		}
		public override bool IsDropDownResizable { get { return true; } }
	}
	public class FormatPredefinedDataBarSchemesUITypeEditor : ObjectPickerEditor {
		protected override ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			FormatPredefinedDataBarScheme current = null;
			FormatConditionRuleDataBar rule = DXObjectWrapper.GetInstance(context) as FormatConditionRuleDataBar;
			if(value != null) {
				current = FormatPredefinedDataBarSchemes.Default.Find(rule, value.ToString());
			}
			var res = new PickerFromValuesControl(this, current, FormatPredefinedDataBarSchemes.Default.Find(rule.LookAndFeel).OrderBy(q => q.Title).ToList(), false, 12, 300);
			return res;
		}
		protected override object ConvertFromValue(object oldValue, object newValue) {
			if(newValue != null) {
				FormatPredefinedDataBarScheme scale = newValue as FormatPredefinedDataBarScheme;
				if(scale != null) newValue = scale.Key;
			}
			return newValue;
		}
		public override bool IsDropDownResizable { get { return true; } }
	}
	public class FormatRuleUITypeEditor : ObjectPickerEditor {
		protected override ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			FormatRuleTypeInfo current = null;
			if(value != null) {
				current = FormatRuleTypeInfo.FromType(value.GetType());
			}
			var res = new PickerFromValuesControl(this, current, FormatRuleTypeInfo.DefaultTypes, false, 12, 300);
			return res;
		}
		protected override object ConvertFromValue(object oldValue, object newValue) {
			if(newValue != null) {
				FormatRuleTypeInfo typeInfo = newValue as FormatRuleTypeInfo;
				if(typeInfo != null) {
					if(typeInfo.FormatRuleType.IsInstanceOfType(oldValue)) return oldValue;
					return typeInfo.CreateInstance();
				}
			}
			return newValue;
		}
		public override bool IsDropDownResizable { get { return true; } }
	}
	public class FormatRuleIconSetUITypeEditor : ObjectPickerEditor {
		protected override ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			object current = null;
				FormatConditionIconSet ic = value as FormatConditionIconSet;
			if(ic != null) {
				current = FormatPredefinedIconSets.Default.FirstOrDefault(q => q.Name == ic.Name && q.CategoryName == ic.CategoryName && ic.Icons.Count == q.Icons.Count);
			}
			var res = new PickerFromValuesControl(this, current, FormatPredefinedIconSets.Default.ToList(), false, 12, 300);
			return res;
		}
		public override bool IsDropDownResizable { get { return true; } }
	}
	public class FormatRuleTypeInfo {
		static List<FormatRuleTypeInfo> defaultTypes;
		public static FormatRuleTypeInfo FromString(string typeInfo) {
			return DefaultTypes.FirstOrDefault(q => q.ToString() == typeInfo);
		}
		public static FormatRuleTypeInfo FromType(Type type) {
			return DefaultTypes.FirstOrDefault(q => q.FormatRuleType.Equals(type));
		}
		public static List<FormatRuleTypeInfo> DefaultTypes {
			get {
				if(defaultTypes == null) {
					defaultTypes = new List<FormatRuleTypeInfo>();
					Create(defaultTypes);
				}
				return defaultTypes;
			}
		}
		static void Create(List<FormatRuleTypeInfo> defaultTypes) {
			defaultTypes.Add(new FormatRuleTypeInfo() { FormatRuleName = "Format based on value", FormatRuleType = typeof(FormatConditionRuleValue) });
			defaultTypes.Add(new FormatRuleTypeInfo() { FormatRuleName = "Format based on date", FormatRuleType = typeof(FormatConditionRuleDateOccuring) });
			defaultTypes.Add(new FormatRuleTypeInfo() { FormatRuleName = "Format based on user defined expression", FormatRuleType = typeof(FormatConditionRuleExpression) });
			defaultTypes.Add(new FormatRuleTypeInfo() { FormatRuleName = "Format only top or bottom ranked values", FormatRuleType = typeof(FormatConditionRuleTopBottom) });
			defaultTypes.Add(new FormatRuleTypeInfo() { FormatRuleName = "Format only values that are above or below average", FormatRuleType = typeof(FormatConditionRuleAboveBelowAverage) });
			defaultTypes.Add(new FormatRuleTypeInfo() { FormatRuleName = "Format only values that contain", FormatRuleType = typeof(FormatConditionRuleContains) });
			defaultTypes.Add(new FormatRuleTypeInfo() { FormatRuleName = "Format only unique or duplicate values", FormatRuleType = typeof(FormatConditionRuleUniqueDuplicate) });
			defaultTypes.Add(new FormatRuleTypeInfo() { FormatRuleName = "Format using 2 color scales", FormatRuleType = typeof(FormatConditionRule2ColorScale) });
			defaultTypes.Add(new FormatRuleTypeInfo() { FormatRuleName = "Format using 3 color scales", FormatRuleType = typeof(FormatConditionRule3ColorScale) });
			defaultTypes.Add(new FormatRuleTypeInfo() { FormatRuleName = "Format using Data bar", FormatRuleType = typeof(FormatConditionRuleDataBar) });
			defaultTypes.Add(new FormatRuleTypeInfo() { FormatRuleName = "Format using icons", FormatRuleType = typeof(FormatConditionRuleIconSet) });
		}
		public string FormatRuleName { get; set; }
		public Type FormatRuleType { get; set; }
		public override string ToString() {
			return FormatRuleName;
		}
		internal FormatConditionRuleBase CreateInstance() {
			return (FormatConditionRuleBase)Activator.CreateInstance(FormatRuleType);
		}
	}
}
