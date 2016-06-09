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
using System.ComponentModel;
namespace DevExpress.Utils {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class SerializationOrderAttribute : Attribute {
		int orderCore = int.MaxValue;
		public int Order {
			get { return orderCore; }
			set { orderCore = value; }
		}
		public static readonly SerializationOrderAttribute Unknown = new SerializationOrderAttribute();
	}
	public class SerializationOrderComparer : System.Collections.Generic.IComparer<Component> {
		public int Compare(Component x, Component y) {
			if(x == y) return 0;
			SerializationOrderAttribute element1 = GetSerializationOrderAttribute(x);
			SerializationOrderAttribute elemetn2 = GetSerializationOrderAttribute(y);
			return element1.Order.CompareTo(elemetn2.Order);
		}
		static SerializationOrderAttribute GetSerializationOrderAttribute(object component) {
			object[] attributes = component.GetType().GetCustomAttributes(typeof(SerializationOrderAttribute), true);
			return (attributes.Length == 1) ? (SerializationOrderAttribute)attributes[0] : SerializationOrderAttribute.Unknown;
		}
	}
}
