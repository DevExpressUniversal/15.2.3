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

using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System;
using DevExpress.Utils;
using System.Collections;
namespace DevExpress.Utils.Design {
	public class ImageCollectionImagesConverter : TypeConverter {
		static string None = "(None)";
		protected virtual IContainer GetContainer(ITypeDescriptorContext context) {
			if(context == null) return null;
			if(context.Container != null) return context.Container;
			IImageCollectionHelper imageHelper = GetImageCollectionHelper(context.Instance);
			if (imageHelper != null) {
				return FindContainer(imageHelper.OwnerControl);
			}
			else return null;
		}
		IImageCollectionHelper GetImageCollectionHelper(object obj) {
			IImageCollectionHelper imageHelper = obj as IImageCollectionHelper;
			if (imageHelper != null) return imageHelper;
			const int MaxIterationCount = 5;
			int iteractionIndex = 0;
			while (iteractionIndex ++ <= MaxIterationCount) {
				IDXObjectWrapper wrapper = obj as IDXObjectWrapper;
				if (wrapper == null) return null;
				obj = wrapper.SourceObject;
				imageHelper = wrapper.SourceObject as IImageCollectionHelper;
				if (imageHelper != null) return imageHelper;
			}
			return null;
		}
		IContainer FindContainer(Control control) {
			if(control == null) return null;
			if(control.Container != null) return control.Container;
			return FindContainer(control.Parent);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType.Equals(typeof(string))) {
				if(value == null) return None;
				Component c = value as Component;
				if(c != null && c.Site != null) return c.Site.Name;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type type) {
			if(type != null && type.Equals(typeof(string))) {
				return true;
			}
			return base.CanConvertFrom(context, type);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null) return null;
			IContainer container = GetContainer(context);
			if(container == null) return null;
			if(value is string) {
				string source = value.ToString();
				if(source == None) return null;
				foreach(IComponent c in container.Components) {
					if(c.Site != null && c.Site.Name == source) return c;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			IContainer container = GetContainer(context);
			if(container == null) return null;
			ArrayList array = new ArrayList();
			array.Add(null);
			foreach(IComponent component in container.Components) {
				if(component is ImageList || component is ImageCollection || component is SharedImageCollection) {
					array.Add(component);
				}
			}
			return new StandardValuesCollection(array);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
}
