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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DevExpress.Utils.Serializing;
using DevExpress.Internal;
namespace DevExpress.Diagram.Core.Base {
	public sealed class DiagramItemTypeRegistrationInfo {
		readonly Type itemType;
		public DiagramItemTypeRegistrationInfo(Type itemType) {
			this.itemType = itemType;
		}
		public IDiagramItem Create(XtraItemEventArgs args) {
			return Activator.CreateInstance(itemType) as IDiagramItem;
		}
	}
	public class DiagramItemTypeRegistrator {
		readonly Dictionary<string, DiagramItemTypeRegistrationInfo> ItemRepository;
		public DiagramItemTypeRegistrator() {
			this.ItemRepository = new Dictionary<string, DiagramItemTypeRegistrationInfo>();
		}
		public void Register(params Type[] itemTypes) {
			itemTypes.ForEach(x => RegirsterCore(x));
		}
		void RegirsterCore(Type itemType) {
			string key = GetItemKind(itemType);
			ItemRepository[key] = new DiagramItemTypeRegistrationInfo(itemType);
		}
		public IDiagramItem Create(string itemKind, XtraItemEventArgs args) {
			if(!ItemRepository.ContainsKey(itemKind)) {
				throw new ArgumentException(string.Format("The '{0}' ItemKind isn't registered", itemKind));
			}
			return ItemRepository[itemKind].Create(args);
		}
		public static readonly string ItemKindProperty = DevExpress.Internal.ExpressionHelper.GetPropertyName((DiagramItemController x) => x.ItemKind);
		public static string GetItemKind(Type itemType) {
			return itemType.Name;
		}
		public static string GetItemKind(IDiagramItem item) { return GetItemKind(item.GetType()); }
	}
	public static class DiagramItemTypeRegistratorExtensions {
		public static bool IsRootItemKind(this IDiagramControl diagram, string itemKind) {
			IDiagramItem rootItem = diagram.RootItem();
			return DiagramItemTypeRegistrator.GetItemKind(rootItem) == itemKind;
		}
	}
}
