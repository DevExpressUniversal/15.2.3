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
namespace DevExpress.Utils.Serializing.Helpers {
	public class ObjectConverters {
		Dictionary<Type, IOneTypeObjectConverter> converters;
		protected Dictionary<Type, IOneTypeObjectConverter> Converters {
			get { return converters; }
		}
		public ObjectConverters() {
			converters = new Dictionary<Type, IOneTypeObjectConverter>();
		}
		public void RegisterConverter(IOneTypeObjectConverter converter) {
			if(Converters.ContainsKey(converter.Type))
				return;
			Converters.Add(converter.Type, converter);
		}
		public void UnregisterConverter(Type type) {
			Converters.Remove(type);
		}
		public virtual bool IsConverterExists(Type type) {
			return Converters.ContainsKey(type);
		}
		public virtual IOneTypeObjectConverter GetConverter(Type type) {
			if(IsConverterExists(type))
				return Converters[type];
			return null;
		}
		public Type ResolveType(string typeName) {
			foreach(Type type in Converters.Keys)
				if(type.FullName == typeName)
					return type;
			return null;
		}
		public string ConvertToString(object obj) {
			IOneTypeObjectConverter converter = GetConverter(obj.GetType());
			if(converter != null)
				return converter.ToString(obj);
			else
				return null;
		}
		public object ConvertFromString(Type type, string str) {
			IOneTypeObjectConverter converter = GetConverter(type);
			if(converter != null)
				return converter.FromString(str);
			else
				return null;
		}
		public void CopyTo(ObjectConverters toConverters) {
			foreach(Type type in Converters.Keys)
				toConverters.RegisterConverter(Converters[type]);
		}
	}
}
