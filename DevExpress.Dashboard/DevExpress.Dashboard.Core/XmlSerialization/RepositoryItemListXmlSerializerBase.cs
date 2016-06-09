#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Xml.Linq;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Native {
	public class RepositoryItemListXmlSerializerBase<TRepositoryItem, TItem> : ListXmlSerializer<TItem>
		where TRepositoryItem : class
		where TItem : class, TRepositoryItem {
		readonly XmlRepository<TRepositoryItem> itemRepository;
		public RepositoryItemListXmlSerializerBase(string xmlListName, XmlRepository<TRepositoryItem> itemRepository)
			: this(xmlListName, itemRepository, false) {
		}
		public RepositoryItemListXmlSerializerBase(string xmlListName, XmlRepository<TRepositoryItem> itemRepository, bool allowUnknownItemsOnLoad)
			: base(xmlListName, allowUnknownItemsOnLoad) {
			this.itemRepository = itemRepository;
		}
		protected override XElement SaveItemToXml(TItem item) {
			XmlSerializer<TRepositoryItem> serializer = itemRepository.GetSerializer(item);
			return serializer != null ? serializer.SaveToXml(item) : null;
		}
		protected override TItem LoadItemFromXml(XElement itemElement) {
			XmlSerializer<TRepositoryItem> serializer = itemRepository.GetSerializer(itemElement.Name.LocalName);
			return serializer != null ? (TItem)serializer.LoadFromXml(itemElement) : null;
		}
	}
}
