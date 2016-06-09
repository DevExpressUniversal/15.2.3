#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.Persistent.Base {
	public class ClassInfoTypeConverter : ReferenceConverter, IComparer<Type> {
		protected virtual String GetClassCaption(String fullName) {
			return fullName;
		}
		public ClassInfoTypeConverter()
			: base(typeof(Type)) {
		}
		public override Boolean CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(String);
		}
		public override Boolean CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(String);
		}
		public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object val, Type destType) {
			if(destType == typeof(String)) {
				Type classInfo = val as Type;
				if(classInfo != null) {
					return GetClassCaption(classInfo.FullName);
				}
				else {
					return CaptionHelper.NoneValue;
				}
			}
			else {
				return base.ConvertTo(context, culture, val, destType);
			}
		}
		public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object val) {
			Type result = null;
			if(val != null) {
				String caption = val.ToString();
				foreach(ITypeInfo classInfo in XafTypesInfo.Instance.PersistentTypes) {
					if(classInfo.IsVisible && (GetClassCaption(classInfo.FullName) == caption)) {
						result = classInfo.Type;
						break;
					}
				}
			}
			return result;
		}
		public virtual List<Type> GetSourceCollection(ITypeDescriptorContext context) {
			List<Type> result = new List<Type>();
			foreach(ITypeInfo classInfo in XafTypesInfo.Instance.PersistentTypes) {
				if(classInfo.IsVisible
					&& classInfo.IsDomainComponent
				) {
					if(classInfo.Type != null) {
						result.Add(classInfo.Type);
					}
				}
			}
			return result;
		}
		public virtual void AddCustomItems(List<Type> list) {
			list.Insert(0, null);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<Type> list = GetSourceCollection(context);
			list.Sort(this);
			AddCustomItems(list);
			return new StandardValuesCollection(list);
		}
		public Int32 Compare(Type x, Type y) {
			return Comparer<String>.Default.Compare(GetClassCaption(x.FullName), GetClassCaption(y.FullName));
		}
	}
}
