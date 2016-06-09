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

extern alias Platform;
using DevExpress.Design.SmartTags;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Xpf.Core.Design {
	public class DXTypeInfoInstanceSource : InstanceSourceBase {
		public static IEnumerable<InstanceSourceBase> FromDXTypeInfoList(IEnumerable<DXTypeInfo> typeInfoList) {
			return typeInfoList.Select(ti => new DXTypeInfoInstanceSource(ti));
		}
		public static IEnumerable<InstanceSourceBase> FromTypeList(IEnumerable<Type> types) {
			return FromDXTypeInfoList(types.Select(t => DXTypeInfo.FromType(t)));
		}
		public DXTypeInfoInstanceSource(DXTypeInfo typeInfo) {
			TypeInfo = typeInfo;
		}
		public DXTypeInfo TypeInfo { get; private set; }
		public override object Resolve(IModelItem item) {
			return item.Context.CreateItem(TypeInfo.ResolveType());
		}
		public override string Name {
			get { return TypeInfo.Name; }
		}
		public override bool IsMatchedObject(object obj) {
			if(obj == null)
				return false;
			if(obj is IModelItem) {
				IModelItem item = (IModelItem)obj;
				return item.ItemType.Name == TypeInfo.Name && item.ItemType.Namespace == TypeInfo.NameSpace;
			}
			return obj.GetType().Name == TypeInfo.Name && obj.GetType().Namespace == TypeInfo.NameSpace;
		}
	}
	public class ObjectInstanceSource : InstanceSourceBase {
		readonly object item;
		public static IEnumerable<InstanceSourceBase> FromList(IEnumerable<object> itemList) {
			return itemList.Select(item => new ObjectInstanceSource(item));
		}
		public ObjectInstanceSource(object item) {
			this.item = item;
		}
		public override bool IsMatchedObject(object obj) {
			if(obj == null)
				return false;
			if(obj is IModelItem) {
				IModelItem modelItem = (IModelItem)obj;
				object currentValue = modelItem.GetCurrentValue();
				return currentValue == null ? false : item.Equals(currentValue);
			}
			return item.Equals(obj);
		}
		public override string Name {
			get { return item.ToString(); }
		}
		public override object Resolve(IModelItem item) {
			return ModelFactory.CreateItem(XpfModelItem.ToModelItem(item).Context, this.item);
		}
		public override string ToString() {
			return Name;
		}
		public override bool Equals(object obj) {
			var objInstanceSource = obj as ObjectInstanceSource;
			if(item == null || objInstanceSource == null)
				return base.Equals(obj);
			return IsMatchedObject(objInstanceSource.item);
		}
		public override int GetHashCode() {
			if(item == null)
				return base.GetHashCode();
			return item.GetHashCode();
		}
	}
}
