#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Xml.Linq;
namespace DevExpress.DashboardCommon.Native {
	public class NameBox : IEditNameProvider {
		readonly string xmlName;
		string name;
		public string Name {
			get { return name; }
			set {
				if (value != name) {
					name = value;
					if (NameChanged != null)
						NameChanged(this, EventArgs.Empty);
				}
			}
		}
		public string EditName { get { return Name; } set { Name = value; } }
		public string DisplayName { 
			get {
				if (IsNameExist)
					return Name;
				if (RequestDefaultName != null) {
					RequestDefaultNameEventArgs e = new RequestDefaultNameEventArgs();
					RequestDefaultName(this, e);
					return e.DefaultName;
				}
				return null;
			} 
		}
		bool IsNameExist { get { return !string.IsNullOrEmpty(name); } }
		public event EventHandler NameChanged;
		public event EventHandler<RequestDefaultNameEventArgs> RequestDefaultName;
		public NameBox(string xmlName) {
			this.xmlName = xmlName;
		}
		public void SaveToXml(XElement element) {
			if (ShouldSerialize())
				element.Add(new XAttribute(xmlName, name));
		}
		public void LoadFromXml(XElement element) {
			string attr = LoadNameFromXml(element);
			if (!string.IsNullOrEmpty(attr))
				name = attr;
		}
		public string LoadNameFromXml(XElement element) {
			return element.GetAttributeValue(xmlName);
		}
		public bool ShouldSerialize() {
			return IsNameExist;
		}
	}
	public class RequestDefaultNameEventArgs : EventArgs {
		public string DefaultName { get; set; }
	}
}
