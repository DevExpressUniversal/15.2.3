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
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
namespace DevExpress.Utils.Design {
	public class UniversalSerializer {
		public static void SerializeObject(object component, SerializationInfo si) {
			if(si == null || component == null) return;
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);
			for(int n = 0; n < properties.Count; n++) {
				PropertyDescriptor pd = properties[n];
				if(pd.IsReadOnly || !pd.ShouldSerializeValue(component)) continue;
				si.AddValue(pd.Name, pd.GetValue(component));
			}
		}
		public static void DeserializeObject(object component, SerializationInfo si) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);
			foreach(SerializationEntry entry in si) {
				PropertyDescriptor pd = properties[entry.Name];
				if(pd == null || pd.IsReadOnly) continue;
				try {
					pd.SetValue(component, Convert.ChangeType(entry.Value, pd.PropertyType));
				} catch { }
			}
		}
	}
}
