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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace DevExpress.Data.Svg{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class FormatElementAttribute : Attribute {
		readonly string xmlElementName;
		public string XmlElementName { get { return xmlElementName; } }
		public FormatElementAttribute(string xmlElementName) {
			this.xmlElementName = xmlElementName;
		}
	}
	public class FormatElementFactory<ElementType> where ElementType : class {
		readonly Dictionary<string, Type> typeByElementName;
		protected internal Dictionary<string, Type> TypeByElementName { get { return typeByElementName; } }
		public FormatElementFactory() {
			typeByElementName = new Dictionary<string, Type>();
			RegisterSourceTypes();
		}
		string GetAttributeValue(Type provider) {
			foreach (Attribute attr in provider.GetCustomAttributes(true)) {
				FormatElementAttribute formatAttr = attr as FormatElementAttribute;
				if (formatAttr != null)
					return formatAttr.XmlElementName;
			}
			return string.Empty;
		}
		void RegisterTypeByName(string elementName, Type type) {
			typeByElementName[elementName] = type;
		}
		void RegisterTypes(Assembly asm) {
			Type[] types = asm.GetExportedTypes();
			foreach (Type item in types) {
				if (item.IsSubclassOf(typeof(ElementType))) {
					string name = GetAttributeValue(item);
					if (!string.IsNullOrEmpty(name))
						RegisterTypeByName(name, item);
				}
			}
		}
		public void RegisterSourceTypes() {
			RegisterSourceTypes(GetType().GetAssembly());
		}
		public void RegisterSourceTypes(Assembly asm) {
			typeByElementName.Clear();
			RegisterTypes(asm);
		}
		public ElementType CreateInstance(string key) {
			Type formatType;
			if (typeByElementName.TryGetValue(key, out formatType))
				return Activator.CreateInstance(formatType) as ElementType;
			return null;
		}
	}
}
