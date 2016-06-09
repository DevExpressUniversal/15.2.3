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
using DevExpress.Utils.Serializing;
namespace DevExpress.Utils.Serializing.Helpers {
	#region CollectionItemSerializationStrategyCollection
	public class CollectionItemSerializationStrategyCollection : CollectionItemSerializationStrategy {
		#region Fields
		XtraSerializationFlags parentFlags;
		OptionsLayoutBase options;
		XtraSerializableProperty attr;
		string name;
		#endregion
		public CollectionItemSerializationStrategyCollection(SerializeHelper helper, string name, ICollection collection, object owner, XtraSerializationFlags parentFlags, OptionsLayoutBase options, XtraSerializableProperty attr)
			: base(helper, name, collection, owner) {
			this.parentFlags = parentFlags;
			this.options = options;
			this.attr = attr;
			this.name = name;
		}
		#region Properties
		protected internal XtraSerializationFlags ParentFlags { get { return parentFlags; } }
		protected internal OptionsLayoutBase Options { get { return options; } }
		#endregion
		protected internal override bool AssignItemPropertyValue(XtraPropertyInfo itemProperty, object item) {
			int index = SerializeHelper.UndefinedObjectIndex;
			if(Helper.TrySerializeCollectionItemCacheIndex(name, attr, itemProperty.ChildProperties, item, ref index))
				return true;
			itemProperty.ChildProperties.AddRange(Helper.SerializeObject(item, parentFlags, options));
			SerializeHelper.AddIndexPropertyInfo(itemProperty, index);
			return true;
		}
	}
	#endregion
}
