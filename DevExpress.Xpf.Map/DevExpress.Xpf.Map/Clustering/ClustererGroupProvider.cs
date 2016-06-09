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

using DevExpress.Map.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
namespace DevExpress.Xpf.Map {
	public abstract class MapClustererGroupProviderBase : MapDependencyObject {
		public abstract Dictionary<object, IEnumerable<MapItem>> GetGroups(IEnumerable<MapItem> sourceItems);
		public abstract void OnClusterCreated(MapItem item);
	}
	public class AttributeGroupProvider : MapClustererGroupProviderBase {
		public static readonly DependencyProperty AttributeNameProperty = DependencyPropertyManager.Register("AttributeName",
		   typeof(string), typeof(AttributeGroupProvider), new PropertyMetadata(null));
		[Category(Categories.Data)]
		public string AttributeName {
			get { return (string)GetValue(AttributeNameProperty); }
			set { SetValue(AttributeNameProperty, value); }
		}
		public override Dictionary<object, IEnumerable<MapItem>> GetGroups(IEnumerable<MapItem> sourceItems) {
			Dictionary<object, IEnumerable<MapItem>> sourceByAttribute = new Dictionary<object, IEnumerable<MapItem>>();
			if(String.IsNullOrEmpty(AttributeName))
				return sourceByAttribute;
			foreach(MapItem item in sourceItems) {
				IMapItemAttribute attribute = item.Attributes[AttributeName];
				if(attribute == null)
					continue;
				if(sourceByAttribute.ContainsKey(attribute.Value))
					((List<MapItem>)sourceByAttribute[attribute.Value]).Add(item);
				else
					sourceByAttribute[attribute.Value] = new List<MapItem> { item };
			}
			return sourceByAttribute;
		}
		public override void OnClusterCreated(MapItem item) {
			IClusterItem clusterItem = item as IClusterItem;
			if(clusterItem != null && clusterItem.ClusteredItems.Count > 0 && !string.IsNullOrEmpty(AttributeName)) {
				MapItem groupItem = clusterItem.ClusteredItems[0] as MapItem;
				IMapItemAttribute attribute = groupItem != null ? groupItem.Attributes[AttributeName] : null;
				if(attribute != null)
					item.Attributes.Add(new MapItemAttribute() { Name = attribute.Name, Value = attribute.Value, Type = attribute.Type });
			}
		}
		protected override MapDependencyObject CreateObject() {
			return new AttributeGroupProvider();
		}
	}
}
