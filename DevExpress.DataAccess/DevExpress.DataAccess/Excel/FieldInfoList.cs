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

using System.Collections.Generic;
using System.Xml.Linq;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Excel {
	public class FieldInfoList : List<FieldInfo> {
		const string xml_FieldInfo = "FieldInfo";
		public FieldInfoList() { }
		public FieldInfoList(IEnumerable<FieldInfo> collection) : base(collection) { }
		public void AddRange(FieldInfo[] collection) {
			base.AddRange(collection);
		}
		internal void SaveToXml(XElement fieldInfoList) {
			Guard.ArgumentNotNull(fieldInfoList, "fieldInfoList");
			foreach(var fi in this) {
				var fieldInfo = new XElement(xml_FieldInfo);
				fi.SaveToXml(fieldInfo);
				fieldInfoList.Add(fieldInfo);
			}
		}
		internal void LoadFromXml(XElement fieldInfoList) {
			Guard.ArgumentNotNull(fieldInfoList, "fieldInfoList");
			foreach(var fieldInfo in fieldInfoList.Elements(xml_FieldInfo)) {
				var fi = new FieldInfo();
				fi.LoadFromXml(fieldInfo);
				Add(fi);
			}
		}
	}
}
