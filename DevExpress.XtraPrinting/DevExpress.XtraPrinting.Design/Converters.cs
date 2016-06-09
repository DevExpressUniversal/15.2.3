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
using System.Globalization;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Specialized;
using DevExpress.XtraPrinting.Preview;
using System.Reflection;
namespace DevExpress.XtraPrinting.Design {
	public abstract class StringValuesConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return value as string;
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string)
				return value as string;
			return base.ConvertFrom(context, culture, value);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
	}
	public class PreviewStatusPanelIDConverter : StringValuesConverter {
		static ArrayList items = new ArrayList();
		static PreviewStatusPanelIDConverter() {
			items.AddRange(Enum.GetNames(typeof(DevExpress.XtraPrinting.Native.StatusPanelID)));
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(items);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
	}	
	public class HeaderFooterConverter : TypeConverter 
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				System.Reflection.ConstructorInfo ci = value.GetType().GetConstructor(new Type[]{typeof(PageHeaderArea), typeof(PageFooterArea)});
				DevExpress.XtraPrinting.PageHeaderFooter c = (PageHeaderFooter)value;
				object ch = c.Header.ShouldSerialize() ? c.Header : null;
				object cf = c.Footer.ShouldSerialize() ? c.Footer : null;
				object instanceDescriptor = new InstanceDescriptor(ci, new object[] {ch, cf});
				return instanceDescriptor;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class PageAreaConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				System.Reflection.ConstructorInfo ci = value.GetType().GetConstructor(new Type[]{typeof(string[]), typeof(System.Drawing.Font), typeof(BrickAlignment)});
				PageArea c = (PageArea)value;
				string[] content = new string[c.Content.Count];
				c.Content.CopyTo(content, 0);
				object instanceDescriptor = new InstanceDescriptor(ci, new object[] {content, c.Font, c.LineAlignment});
				return instanceDescriptor;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
