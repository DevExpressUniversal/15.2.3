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
using System.Reflection;
using DevExpress.Utils.Serializing;
namespace DevExpress.Utils.Serializing.Helpers {
	public abstract class CollectionItemSerializationStrategy {
		#region Fields
		SerializeHelper helper;
		ICollection collection;
		object owner;
		MethodInfo mi;
		#endregion
		protected CollectionItemSerializationStrategy(SerializeHelper helper, string name, ICollection collection, object owner) {
			this.helper = helper;
			this.collection = collection;
			this.owner = owner;
			this.mi = helper.Context.GetShouldSerializeCollectionMethodInfo(helper, name, owner);
		}
		#region Properties
		protected internal SerializeHelper Helper { get { return helper; } }
		protected internal ICollection Collection { get { return collection; } }
		protected internal object Owner { get { return owner; } }
		protected internal MethodInfo ShouldSerializeCollectionItemMethodInfo { get { return mi; } }
		#endregion
		public virtual XtraPropertyInfo SerializeCollectionItem(int index, object item) {
			XtraPropertyInfo itemProperty = CreateItemPropertyInfo(index);
			if(ShouldSerializeCollectionItem(itemProperty, item)) {
				if(AssignItemPropertyValue(itemProperty, item))
					return itemProperty;
			}
			return null;
		}
		protected internal virtual XtraPropertyInfo CreateItemPropertyInfo(int index) {
			return CreateItemPropertyInfoCore(index, false);
		}
		protected internal virtual XtraPropertyInfo CreateItemPropertyInfoCore(int index, bool isSimpleCollection) {
			return new XtraPropertyInfo("Item" + index.ToString(), null, null, !isSimpleCollection);
		}
		protected internal virtual bool ShouldSerializeCollectionItem(XtraPropertyInfo itemProperty, object item) {
			return helper.Context.ShouldSerializeCollectionItem(helper, owner, mi, itemProperty, item, 
					new XtraItemEventArgs(helper.RootObject, owner, collection, itemProperty)
				);
		}
		protected internal abstract bool AssignItemPropertyValue(XtraPropertyInfo itemProperty, object item);
	}
}
