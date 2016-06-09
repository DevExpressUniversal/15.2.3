#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Xpo;
namespace DevExpress.Persistent.Base.General {
	public class PropertyValueImpl {
		private IPropertyValue owner;
		private XPWeakReference weakReference;
		private string strValue;
		private object DeSerialize(string StrValue) {
			if (String.IsNullOrEmpty(StrValue)) return null;
			return Convert.ChangeType(StrValue, ValueType);
		}
		public PropertyValueImpl(IPropertyValue owner) {
			this.owner = owner;
		}
		public object Value {
			get {
				if(weakReference != null && (weakReference.Target != null)) {
					return weakReference.Target;
				}
				if(strValue != null) {
					return DeSerialize(StrValue);
				}
				return null;
			}
			set {
				if(Value != value) {
					if(value is IXPSimpleObject) {
						if(weakReference != null) {
							weakReference.Target = value;
						}
						else {
							weakReference = new XPWeakReference(((IXPSimpleObject)value).Session, value);
						}
						strValue = null;
					}
					else {
						StrValue = value.ToString();
					}
				}
			}
		}
		public Type ValueType {
			get {
				if (owner.Descriptor == null) return null;
				return owner.Descriptor.ValueType;
			}
		}
		public string StrValue {
			get { return strValue; }
			set { strValue = value; }
		}
		public XPWeakReference WeakReference {
			get { return weakReference; }
			set { weakReference = value; }
		}
	}
}
