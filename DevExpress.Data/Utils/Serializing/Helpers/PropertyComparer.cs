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
using System.Collections.Generic;
using DevExpress.Compatibility.System.ComponentModel;
#if !SILVERLIGHT
using System.ComponentModel;
#else
using DevExpress.Data.Browsing;
#endif //SILVERLIGHT
namespace DevExpress.Utils.Serializing.Helpers {
	public class PropertyDescriptorComparer : IComparer<SerializablePropertyDescriptorPair> {
		SerializationContext serializationContext;
		object obj;
		public PropertyDescriptorComparer(SerializationContext serializationContext, object obj) {
			this.serializationContext = serializationContext;
			this.obj = obj;
		}
		protected virtual int CompareProperties(SerializablePropertyDescriptorPair x, SerializablePropertyDescriptorPair y) {
			PropertyDescriptor p1 = x.Property,
				p2 = y.Property;
			if(p1 == p2)
				return 0;
			if(p1 == null)
				return -1;
			if(p2 == null)
				return 1;
			XtraSerializableProperty attr1 = x.Attribute,
				attr2 = y.Attribute;
			if(attr1 == attr2)
				return 0;
			if(attr1 == null)
				return 1;
			if(attr2 == null)
				return -1;
			return attr1.Order.CompareTo(attr2.Order);
		}		
		#region IComparer<SerializablePropertyDescriptorPair> Members
		int IComparer<SerializablePropertyDescriptorPair>.Compare(SerializablePropertyDescriptorPair x, SerializablePropertyDescriptorPair y) {
			return CompareProperties(x, y);
		}
		#endregion
	}
}
