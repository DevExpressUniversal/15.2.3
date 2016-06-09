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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public abstract class MapItemSettings : MapItemSettingsBase {
	}
	public abstract class MapItemSettingsBase : MapDependencyObject, IOwnedElement {
		public static readonly DependencyProperty IsHitTestVisibleProperty = DependencyPropertyManager.Register("IsHitTestVisible",
		   typeof(bool), typeof(MapItemSettingsBase), new PropertyMetadata(true, UpdateItems));
		protected static void UpdateItems(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapItemSettingsBase settings = d as MapItemSettingsBase;
			if (settings != null)
				settings.UpdateItemsProperties(e);
		}
		[Category(Categories.Behavior)]
		public bool IsHitTestVisible {
			get { return (bool)GetValue(IsHitTestVisibleProperty); }
			set { SetValue(IsHitTestVisibleProperty, value); }
		}
		DependencyPropertyMappingDictionary propertiesMappings;
		object owner;
		protected IMapItemSettingsListener Listener { get { return owner as IMapItemSettingsListener; } }
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set { owner = value; }
		}
		#endregion
		public MapItemSettingsBase() {
			UpdatePropertiesMappings();
		}
		void UpdatePropertiesMappings() {
			propertiesMappings = new DependencyPropertyMappingDictionary();
			FillPropertiesMappings(propertiesMappings);
		}
		void UpdateItemsProperties(DependencyPropertyChangedEventArgs e) {
			if(Listener != null) {
				DependencyProperty itemProperty = propertiesMappings[e.Property];
				Listener.OnPropertyChanged(itemProperty, e.NewValue);
			}
		}
		protected abstract MapItem CreateItemInstance();
		protected virtual void FillPropertiesMappings(DependencyPropertyMappingDictionary mappings) {
			mappings.Add(MapItemSettingsBase.IsHitTestVisibleProperty, MapItem.IsHitTestVisibleProperty);
		}
		internal MapItem CreateItem() {
			MapItem item = CreateItemInstance();
			foreach (DependencyProperty property in propertiesMappings.Keys) {
				DependencyProperty itemProperty = propertiesMappings[property];
				item.SetValue(itemProperty, this.GetValue(property));
			}
			return item;
		}
		protected internal virtual void ApplySource(MapItem mapItem, object source) {
		}
	}
}
namespace DevExpress.Xpf.Map.Native {
	public interface IMapItemSettingsListener {
		void OnPropertyChanged(DependencyProperty property, object value);
		void OnSourceChanged();
	}
	public class DependencyPropertyMappingDictionary : Dictionary<DependencyProperty, DependencyProperty> {
	}
}
