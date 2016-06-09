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
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils;
namespace DevExpress.XtraEditors.Design {
	public class EditorButtonTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,	Type destinationType) {
			if (destinationType == null) { throw new ArgumentNullException("destinationType"); }
			if(destinationType == typeof(InstanceDescriptor) && (value is EditorButton)) {
				EditorButton button = (EditorButton)value;
				Type ctorType = button.GetType();
				ConstructorInfo ctor = null;
				object[] parameters = null;
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(button);
				bool shouldSerializeKind = properties["Kind"].ShouldSerializeValue(button);
				int serCount = 0;
				foreach(PropertyDescriptor pd in properties) {
					if(pd.ShouldSerializeValue(button)) serCount ++;
				}
				if(shouldSerializeKind) serCount --;
				ctor = ctorType.GetConstructor(new Type[] {});
				if(shouldSerializeKind) {
					ctor = ctorType.GetConstructor(new Type[] {typeof(ButtonPredefines)});
					parameters = new object[] { button.Kind };
				}
				if(serCount > 0) {
					ctor = ctorType.GetConstructor(new Type[] {typeof(ButtonPredefines), typeof(string), typeof(int), typeof(bool), typeof(bool), typeof(bool), typeof(ImageLocation), typeof(Image), typeof(SuperToolTip)});
					parameters = new object[] { button.Kind, button.Caption, button.Width, button.Enabled, button.Visible, button.IsLeft, button.ImageLocation, button.Image, button.SuperTip };
				}
				if(ctor != null)
					return new InstanceDescriptor(ctor, parameters);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class TokenEditTokenTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) { throw new ArgumentNullException("destinationType"); }
			if(destinationType == typeof(InstanceDescriptor) && (value is TokenEditToken)) {
				TokenEditToken pi = (TokenEditToken)value;
				ConstructorInfo ctor = typeof(TokenEditToken).GetConstructor(new Type[] { typeof(string), typeof(object) });
				object[] pars = new object[] { pi.Description, pi.Value };
				if(ctor != null)
					return new InstanceDescriptor(ctor, pars);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public sealed class ImageComboBoxItemTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == null) { throw new ArgumentNullException("destinationType"); }
			if (destinationType == typeof(InstanceDescriptor) && (value is ImageComboBoxItem)) {
				ImageComboBoxItem pi = (ImageComboBoxItem)value;
				ConstructorInfo ctor = typeof(ImageComboBoxItem).GetConstructor(new Type[] {typeof(string), typeof(object), typeof(int)});
				object[] pars = new object[] { pi.Description, pi.Value, pi.ImageIndex};
				if(ctor != null)
					return new InstanceDescriptor(ctor, pars);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class BreadCrumbSelectedNodeTypeConverter : TypeConverter {
		public static string NoneItem = "(None)";
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string) && value != null) {
				return value.ToString();
			}
			if(value == null) return NoneItem;
			return value;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string valueCore = value as string;
			if(valueCore != null) {
				if(string.Equals(valueCore, NoneItem, StringComparison.Ordinal))
					return null;
				BreadCrumbNode node = GetBreadCrumbProperties(context).Nodes.FindNode(valueCore, true);
				if(node != null) return node;
			}
			return value;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			ArrayList list = new ArrayList();
			list.Add(null);
			foreach(BreadCrumbNode node in GetBreadCrumbProperties(context).GetAllNodes()) {
				list.Add(node);
			}
			return new StandardValuesCollection(list);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		protected RepositoryItemBreadCrumbEdit GetBreadCrumbProperties(ITypeDescriptorContext context) {
			RepositoryItemBreadCrumbEdit properties = context.Instance as RepositoryItemBreadCrumbEdit;
			if(properties == null) {
				BreadCrumbDesignerActionList list = context.Instance as BreadCrumbDesignerActionList;
				if(list != null) properties = list.BreadCrumb.Properties;
			}
			return properties;
		}
	}
	public class ColorItemTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) { throw new ArgumentNullException("destinationType"); }
			if(destinationType == typeof(InstanceDescriptor) && (value is ColorItem)) {
				ColorItem pi = (ColorItem)value;
				ConstructorInfo ctor = typeof(ColorItem).GetConstructor(new Type[] { typeof(Color) });
				object[] pars = new object[] { pi.Color };
				if(ctor != null)
					return new InstanceDescriptor(ctor, pars);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
