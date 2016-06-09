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
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.XtraMap {
	public interface IClustererGroupProvider : ISupportObjectChanged {
		Dictionary<object, IEnumerable<MapItem>> GetGroups(IEnumerable<MapItem> sourceItems);
		void OnClusterCreated(MapItem item);
	}
	public abstract class MapClustererGroupProviderBase : IClustererGroupProvider {
		#region ISupportObjectChanged Members
		EventHandler onChanged;
		event EventHandler ISupportObjectChanged.Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected virtual void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		Dictionary<object, IEnumerable<MapItem>> IClustererGroupProvider.GetGroups(IEnumerable<MapItem> sourceItems) {
			return GetGroups(sourceItems);
		}
		void IClustererGroupProvider.OnClusterCreated(MapItem item) {
			OnClusterCreated(item);
		}
		protected virtual void OnClusterCreated(MapItem item) { }
		protected internal abstract Dictionary<object, IEnumerable<MapItem>> GetGroups(IEnumerable<MapItem> sourceItems);
	}
	public class AttributeGroupProvider : MapClustererGroupProviderBase {
		string attributeName = null;
		[Category(SRCategoryNames.Data), DefaultValue(null)]
		public string AttributeName {
			get { return attributeName; }
			set {
				if (attributeName == value)
					return;
				attributeName = value;
				RaiseChanged();
			}
		}
		protected internal override Dictionary<object, IEnumerable<MapItem>> GetGroups(IEnumerable<MapItem> sourceItems) {
			Dictionary<object, IEnumerable<MapItem>> sourceByAttribute = new Dictionary<object, IEnumerable<MapItem>>();
			if (String.IsNullOrEmpty(AttributeName))
				return sourceByAttribute;
			foreach (MapItem item in sourceItems) {
				IMapItemAttribute attribute = item.Attributes[AttributeName];
				if (attribute == null)
					continue;
				if (sourceByAttribute.ContainsKey(attribute.Value))
					((List<MapItem>)sourceByAttribute[attribute.Value]).Add(item);
				else
					sourceByAttribute[attribute.Value] = new List<MapItem> { item };
			}
			return sourceByAttribute;
		}
		protected override void OnClusterCreated(MapItem item) {
			IClusterItem clusterItem = item as IClusterItem;
			if(clusterItem != null && clusterItem.ClusteredItems.Count > 0 && !string.IsNullOrEmpty(AttributeName)) {
				MapItem clusteredItem = clusterItem.ClusteredItems[0] as MapItem;
				IMapItemAttribute attribute = clusteredItem != null ? clusteredItem.Attributes[AttributeName] : null;
				if (attribute != null)
					item.Attributes.Add(new MapItemAttribute() { Name = attribute.Name, Value = attribute.Value, Type = attribute.Type });
			}
		}
		public override string ToString() {
			return "(AttributeGroupProvider)";
		}
	}
}
