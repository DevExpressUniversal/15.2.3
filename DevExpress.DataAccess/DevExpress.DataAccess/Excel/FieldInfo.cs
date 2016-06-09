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
using System.Xml.Linq;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Excel {
	public sealed class FieldInfo {
		const string xml_Name = "Name";
		const string xml_Type = "Type";
		const string xml_Selected = "Selected";
		public string Name { get; set; }
		public Type Type { get; set; }
		[DefaultValue(true)]
		public bool Selected { get; set; }
		public FieldInfo() {
			Selected = true;
		}
		public override bool Equals(object other) {
			var fieldInfo = other as FieldInfo;
			if (fieldInfo == null)
				return false;
			return string.Equals(Name, fieldInfo.Name) && Type == fieldInfo.Type && Selected == fieldInfo.Selected;
		}
		public override int GetHashCode() {
			return 0;
		}
		internal void SaveToXml(XElement fieldInfo) {
			Guard.ArgumentNotNull(fieldInfo, "fieldInfo");
			if (!string.IsNullOrEmpty(Name))
				fieldInfo.Add(new XAttribute(xml_Name, Name));
			if(Type != null)
				fieldInfo.Add(new XAttribute(xml_Type, Type.FullName));
			fieldInfo.Add(new XAttribute(xml_Selected, Selected));
		}
		internal void LoadFromXml(XElement fieldInfo) {
			Guard.ArgumentNotNull(fieldInfo, "fieldInfo");
			Name = fieldInfo.GetAttributeValue(xml_Name);
			Type = Type.GetType(fieldInfo.GetAttributeValue(xml_Type));
			Selected = bool.Parse(fieldInfo.GetAttributeValue(xml_Selected));
		}
	}
}
