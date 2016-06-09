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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Diagram.Core.Base;
using DevExpress.Internal;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.Diagram.Core.Serialization {
	[Serializable]
	public class DiagramItems {
		public string Items { get; set; }
	}
	public static class DiagramClipboard {
		public static void Save(IDiagramControl diagram, IList<IDiagramItem> items) {
			if(items.Any()) {
				var serializableObj = new DiagramItems() { Items = diagram.SerializeItems(items, StoreRelationsMode.RelativeToSerializingItems) };
				Clipboard.SetDataObject(serializableObj);
			}
		}
		public static void Clear() {
			Clipboard.Clear();
		}
		public static IList<IDiagramItem> GetObjects(IDiagramControl diagram) {
			string xmlString = GetClipboardText();
			if(string.IsNullOrEmpty(xmlString)) return null;
			return diagram.DeserializeItems(xmlString, StoreRelationsMode.RelativeToSerializingItems);
		}
		static string GetClipboardText() {
			var data = (DiagramItems)Clipboard.GetDataObject().GetData(typeof(DiagramItems));
			return data.With(x => x.Items);
		}
	}
	public abstract class DiagramItemsContainerBase {
		readonly IList<IDiagramItem> items;
		internal readonly IDiagramControl Diagram;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public IList<IDiagramItem> Items { get { return items; } }
		internal IEnumerable<IDiagramItem> AllItems { get { return items.SelectMany(x => x.GetSelfAndChildren()); } }
		public DiagramItemsContainerBase(IList<IDiagramItem> items, IDiagramControl diagram) {
			this.items = items;
			Diagram = diagram;
		}
	}
	public sealed class DiagramSerializingItemsContainer : DiagramItemsContainerBase {
		readonly StoreRelationsMode storeRelationsMode;
		internal DiagramSerializingItemsContainer(IDiagramControl diagram, StoreRelationsMode storeRelationsMode, IList<IDiagramItem> items) 
			: base(items, diagram) {
			this.storeRelationsMode = storeRelationsMode;
		}
		public DiagramItemFinderPath GetPath(IDiagramItem item) {
			var rootToIndexMap = Items
				.Select((root, index) => new { root, index })
				.ToDictionary(x => x.root, x => (int?)x.index);
			return DiagramItemFinderPath.Create(item, storeRelationsMode == StoreRelationsMode.RelativeToSerializingItems ? Items : Diagram.Items());
		}
	}
	public sealed class DiagramDeserializingItemsContainer : DiagramItemsContainerBase, IXtraSupportDeserializeCollectionItem {
		readonly StoreRelationsMode storeRelationsMode;
		internal DiagramDeserializingItemsContainer(IDiagramControl diagram, StoreRelationsMode storeRelationsMode)
			: base(new List<IDiagramItem>(), diagram) {
			this.storeRelationsMode = storeRelationsMode;
		}
		#region IXtraSupportDeserializeCollectionItem
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return DiagramItemController.CreateCollectionItem(propertyName, e);
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			DiagramItemController.SetIndexCollectionItem(propertyName, e);
		}
		#endregion
		public IDiagramItem GetItem(DiagramItemFinderPath path) {
			return path.FindItem(Diagram, storeRelationsMode == StoreRelationsMode.RelativeToSerializingItems ? Items : Diagram.Items());
		}
	}
}
