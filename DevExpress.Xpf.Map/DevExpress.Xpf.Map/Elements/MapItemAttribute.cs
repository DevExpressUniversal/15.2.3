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
using System.Windows;
using DevExpress.Map.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class MapItemAttribute : MapDependencyObject, IMapItemAttribute, IOwnedElement {
		public static readonly DependencyProperty NameProperty = DependencyPropertyManager.Register("Name",
			typeof(string), typeof(MapItemAttribute), new PropertyMetadata(string.Empty, OnAttributeChanged));
		public static readonly DependencyProperty TypeProperty = DependencyPropertyManager.Register("Type",
			typeof(Type), typeof(MapItemAttribute), new PropertyMetadata(OnAttributeChanged));
		public static readonly DependencyProperty ValueProperty = DependencyPropertyManager.Register("Value",
			typeof(object), typeof(MapItemAttribute), new PropertyMetadata(null, OnAttributeChanged));
		static void OnAttributeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapItemAttribute attribute = d as MapItemAttribute;
			if(attribute == null)
				return;
			MapShapeBase shape = attribute.Item as MapShapeBase;
			if(shape != null)
				shape.ApplyTitleOptions();
		}
		object owner;
		protected MapItem Item { get { return owner as MapItem; } }
		[Category(Categories.Common)]
		public string Name {
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		[Category(Categories.Common)]
		public Type Type {
			get { return (Type)GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}
		[Category(Categories.Common)]
		public object Value {
			get { return (object)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		public MapItemAttribute() { }
		public MapItemAttribute(IMapItemAttribute source) {
			Name = source.Name;
			Type = source.Type;
			Value = source.Value;
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set { owner = value; }
		}
		#endregion
		protected override MapDependencyObject CreateObject() {
			return new MapItemAttribute();
		}
	}
	public class MapItemAttributeCollection : MapDependencyObjectCollection<MapItemAttribute> {
		public MapItemAttributeCollection(MapDependencyObject owner) {
			((IOwnedElement)this).Owner = owner;
		}
		public MapItemAttribute this[string name] { 
			get {
				foreach (MapItemAttribute item in this)
					if (item.Name == name)
						return item;
				return null;
			} 
		}
	}
}
