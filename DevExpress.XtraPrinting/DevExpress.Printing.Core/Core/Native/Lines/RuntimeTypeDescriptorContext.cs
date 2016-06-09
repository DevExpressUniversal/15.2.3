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
using DevExpress.Compatibility.System.ComponentModel;
using System.Collections;
using DevExpress.Utils.Design;
#if SILVERLIGHT
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.XtraPrinting.Native.Lines {
	public class RuntimeTypeDescriptorContext : ITypeDescriptorContext {
		#region static
		public static object[] GetStandardValues(ITypeDescriptorContext context, TypeConverter converter) {
			object[] values = null;
#if !DXRESTRICTED
			if (converter != null && converter.GetStandardValuesSupported(context)) {
				ICollection standardValues = converter.GetStandardValues(context);
				values = new object[standardValues.Count];
				standardValues.CopyTo(values, 0);
			}
#endif
			return values;
		}
#endregion
		private object instance;
		private PropertyDescriptor propDesc;
		public object Instance { get { return this.instance; } }
		public System.ComponentModel.PropertyDescriptor PropertyDescriptor { 
			get {
#if !SILVERLIGHT
				return this.propDesc;
#else
				throw new NotSupportedException("Use GetService(typeof(DevExpress.Data.Browsing.PropertyDescriptor) instead.");
#endif
			} 
		}
		public virtual IContainer Container { get { return null; } }
		public RuntimeTypeDescriptorContext(PropertyDescriptor propDesc, object instance) {
			this.propDesc = propDesc;
			this.instance = instance;
		}
		public virtual void OnComponentChanged() { }
		public virtual bool OnComponentChanging() { return true; }
		public virtual object GetService(Type serviceType) {
			if(serviceType == typeof(PropertyDescriptor))
				return propDesc;
			return null;
		}
	}
}
