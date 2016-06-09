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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class ColorizerKeyItemCollection : GenericColorizerItemCollection<ColorizerKeyItem> {
	}
	public class ColorizerKeyItem : MapDependencyObject, IOwnedElement {
		public static readonly DependencyProperty KeyProperty = DependencyPropertyManager.Register("Key",
			typeof(object), typeof(ColorizerKeyItem), new PropertyMetadata(null, ItemPropertyChanged));
		public static readonly DependencyProperty NameProperty = DependencyPropertyManager.Register("Name",
			typeof(string), typeof(ColorizerKeyItem), new PropertyMetadata(null, ItemPropertyChanged));
		protected static void ItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ColorizerKeyItem item = d as ColorizerKeyItem;
			if (item != null && item.Owner != null) {
				item.Owner.Invalidate();
			}
		}
		[Category(Categories.Appearance), TypeConverter(typeof(StringToObjectTypeConverter))]
		public object Key {
			get { return GetValue(KeyProperty); }
			set { SetValue(KeyProperty, value); }
		}
		[Category(Categories.Appearance)]
		public string Name {
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		object owner;
		MapColorizer Owner { get { return owner as MapColorizer; } }
		string KeyText { get { return Key != null ? Key.ToString() : string.Empty; } }
		internal string Text { get { return Name ?? KeyText; } }
		internal Color Color { get; set; }
		#region IOwnedElement Members
		object IOwnedElement.Owner {
			get { return owner; }
			set { owner = value; }
		}
		#endregion
		protected override MapDependencyObject CreateObject() {
			return new ColorizerKeyItem();
		}
	}
	public class StringToObjectTypeConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if (value == null || value.GetType() == typeof(string))
				return value;
			return base.ConvertFrom(context, culture, value);
		}
	}
}
