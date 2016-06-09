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
using DevExpress.XtraBars.Ribbon;
using DevExpress.Utils;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
namespace DevExpress.Snap.Extensions.UI {
	[TypeConverter(typeof(ContextRibbonPageCategoryItem.ContextRibbonPageCategoryTypeConverter))]
	public class ContextRibbonPageCategoryItem : IDisposable {
		class ContextRibbonPageCategoryTypeConverter : TypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string))
					return true;
				return base.CanConvertTo(context, destinationType);
			}
			[System.Security.SecuritySafeCritical]
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor) && value is ContextRibbonPageCategoryItem) {
					ConstructorInfo ci = typeof(ContextRibbonPageCategoryItem).GetConstructor(new Type[] { typeof(RibbonPageCategory) });
					return new InstanceDescriptor(ci, new object[] { ((ContextRibbonPageCategoryItem)value).PageCategory });
				}
				else if (destinationType == typeof(string)) {
					return typeof(ContextRibbonPageCategoryItem).Name;
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
			public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
				return true;
			}
			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
				return TypeDescriptor.GetProperties(value);
			}
		}
		readonly RibbonPageCategory pageCategory;
		public ContextRibbonPageCategoryItem(RibbonPageCategory pageCategory) {
			Guard.ArgumentNotNull(pageCategory, "item");
			this.pageCategory = pageCategory;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
		public RibbonPageCategory PageCategory { get { return pageCategory; } }
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing && pageCategory != null) {
				pageCategory.Dispose();
			}
		}
		~ContextRibbonPageCategoryItem() {
			Dispose(false);
		}
	}
}
