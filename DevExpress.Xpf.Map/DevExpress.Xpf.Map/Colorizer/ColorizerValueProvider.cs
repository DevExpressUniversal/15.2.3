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
using System.ComponentModel;
using System.Windows;
using DevExpress.Map.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public abstract class NamedAttributeProviderBase : MapDependencyObject {
		public static readonly DependencyProperty AttributeNameProperty = DependencyPropertyManager.Register("AttributeName",
			typeof(string), typeof(NamedAttributeProviderBase), new PropertyMetadata(string.Empty, NotifyPropertyChanged));
		[Category(Categories.Appearance)]
		public string AttributeName {
			get { return (string)GetValue(AttributeNameProperty); }
			set { SetValue(AttributeNameProperty, value); }
		}
		protected virtual object ExtractAttributeValue(IMapItemAttribute attribute) {
			object value = attribute.Value;
			IList valueArr = value as IList;
			return valueArr != null ? ConvertFromValueArray(valueArr) : value;
		}
		protected object ConvertFromValueArray(IList list) {
			return (list != null && list.Count > 0) ? list[0] : null;
		}
		protected object GetAttributeValue(IMapItemAttributeOwner owner) {
			if(owner != null && !string.IsNullOrEmpty(AttributeName)) {
				IMapItemAttribute attr = owner.GetAttribute(AttributeName);
				if (attr != null)
					return ExtractAttributeValue(attr);
			}
			return null;
		}
	}
	public abstract class AttributeValueProviderBase : NamedAttributeProviderBase {
		protected virtual double GetValue(MapItem item) {
			if (item == null) return double.NaN;
			object value = GetAttributeValue(item);
			return value != null ? Convert.ToDouble(value) : double.NaN;
		}
	}
}
