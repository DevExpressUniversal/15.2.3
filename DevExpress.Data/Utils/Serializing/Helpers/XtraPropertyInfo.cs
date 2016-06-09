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
using DevExpress.Utils.Serializing;
namespace DevExpress.Utils.Serializing.Helpers {
	public class XtraPropertyInfo {
		bool isKey;
		string name;
		Type propertyType;
		IXtraPropertyCollection childProperties;
		ObjectConverterImplementation objectConverterImpl;
		public XtraPropertyInfo(XtraObjectInfo info, IXtraPropertyCollection childProperties)
			: this(info) {
			this.childProperties = childProperties;
		}
		public XtraPropertyInfo(XtraObjectInfo info) {
			this.name = MakeXtraObjectInfoName(info.Name);
			this.propertyType = typeof(string);
			this.Value = info.Name;
			this.isKey = true;
			this.childProperties = new XtraPropertyCollection();
			SetObjectConverterImpl(ObjectConverter.Instance);
		}
		public bool IsXtraObjectInfo {
			get {
				return Name.Length > 0 && Name[0] == '$';
			}
		}
		public XtraPropertyInfo()
			: this("", typeof(object), null) {
		}
		public XtraPropertyInfo(string name)
			: this(name, typeof(object), null) {
		}
		public XtraPropertyInfo(string name, Type propertyType, object val)
			: this(name, propertyType, val, false) {
		}
		public XtraPropertyInfo(string name, Type propertyType, object val, bool isKey) {
			childProperties = null;
			this.name = name;
			this.Value = val;
			this.isKey = isKey;
			this.propertyType = propertyType;
			if (IsKey)
				childProperties = new XtraPropertyCollection();
			SetObjectConverterImpl(ObjectConverter.Instance);
		}
		public static string MakeXtraObjectInfoName(string name) {
			return "$" + name;
		}
		public void SetObjectConverterImpl(ObjectConverterImplementation objectConverterImpl) {
			this.objectConverterImpl = objectConverterImpl;
		}
		protected virtual ObjectConverterImplementation ObjectConverterImplementation { get { return objectConverterImpl; } }
		public Type PropertyType {
			get { return propertyType; }
			set { propertyType = value; }
		}
		public string Name {
			get { return name; }
			set { name = value; }
		}
		public bool IsNull { get; set; }
		public object Value;
		public bool IsKey {
			get { return isKey; }
		}
		public virtual object ValueToObject(Type type) {
#if !SILVERLIGHT
			if(Value is string)
				return ObjectConverterImplementation.StringToObject(Value.ToString(), type);
#endif
			return Value;
		}
		public IXtraPropertyCollection ChildProperties { get { return childProperties; } }
		public bool HasChildren { get { return ChildProperties != null && ChildProperties.Count > 0; } }
		public override string ToString() {
			return Name != null ? Name : base.ToString();
		}
	}
}
