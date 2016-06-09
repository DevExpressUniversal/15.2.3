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
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DataAccess.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon.Native {
	public delegate ISite GetSiteCallback();
	public class DataSourceComponentNameController {
		internal const string XmlName = "Name";
		internal const string XmlComponentName = "ComponentName";
		string componentName;
		string name;
		string name_13_1;
		GetSiteCallback getSite;
		readonly Locker locker;
		public event EventHandler<NameChangedEventArgs> NameChanged;
		public event EventHandler CaptionChanged;
		public event EventHandler<NameChangingEventArgs> NameChanging;
		public string Name_13_1 { get { return name_13_1; } }
		ISite Site { 
			get {
				if (getSite != null)
					return getSite();
				return null;
			}
		}
		bool Loading { get { return locker.IsLocked; } }
		public string ComponentName {
			get {
				ISite site = Site;
				return site != null ? site.Name : componentName;
			}
			set {
				if(ComponentName != value) {
					string oldName = ComponentName;
					if(!Loading && NameChanging != null)
						NameChanging(this, new NameChangingEventArgs(value));
					ISite site = Site;
					if (!Loading && site != null)
						site.Name = value;
					componentName = value;
					if(!Loading && NameChanged != null)
						NameChanged(this, new NameChangedEventArgs(oldName, ComponentName));
				}
			}
		}
		public string Name {
			get { return name; }
			set {
				name = value;
				if(!Loading && CaptionChanged != null)
					CaptionChanged(this, EventArgs.Empty);
			}
		}
		public DataSourceComponentNameController(string name, Locker locker, GetSiteCallback getSite) {
			this.locker = locker;
			this.name = name;
			this.getSite = getSite;
		}
		public void SaveToXml(XElement element) {
			SaveComponentNameToXml(element);
			if(!string.IsNullOrEmpty(name))
				element.Add(new XAttribute(XmlName, name));
		}
		public void LoadFromXml(XElement element, bool backCompatibility) {
			string componentName = element.GetAttributeValue(XmlComponentName);
			string name = element.GetAttributeValue(XmlName);
			if (backCompatibility) {
				if (!String.IsNullOrEmpty(componentName))
					ComponentName = componentName;
				else if (!String.IsNullOrEmpty(name)) {
					if (ObjectTypeNameGenerator.ContainsWrongCharacters(name))
						name_13_1 = name;
					else
						ComponentName = name;
				}
			}
			else
				ComponentName = componentName;
			Name = name;
		}
		public void SaveComponentNameToXml(XElement element) {
			element.Add(new XAttribute(XmlComponentName, ComponentName));
		}
		public void LoadComponentNameFromXml(XElement element) {
			ComponentName = element.GetAttributeValue(XmlComponentName);
		}
	}
}
