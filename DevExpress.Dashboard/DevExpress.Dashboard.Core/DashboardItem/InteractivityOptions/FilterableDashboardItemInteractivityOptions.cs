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
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public class FilterableDashboardItemInteractivityOptions {
		const string xmlInteractivityOptionsElementName = "InteractivityOptions";
		const string xmlIgnoreMasterFilters = "IgnoreMasterFilters";
		readonly bool defaultIgnoreMasterFilters = false;
		bool ignoreMasterFilters;
		[
		Category(CategoryNames.Interactivity)
		]
		public virtual bool IgnoreMasterFilters {
			get { return ignoreMasterFilters; }
			set {
				if (value != ignoreMasterFilters) {
					ignoreMasterFilters = value;
					OnChanged(ChangeReason.Interactivity);
				}
			}
		}
		internal event EventHandler<ChangedEventArgs> Changed;
		protected internal FilterableDashboardItemInteractivityOptions(bool defaultIgnoreMasterFilters) {
			this.defaultIgnoreMasterFilters = defaultIgnoreMasterFilters;
			this.ignoreMasterFilters = defaultIgnoreMasterFilters;
		}
		internal virtual void CopyTo(FilterableDashboardItemInteractivityOptions destination) {
			destination.IgnoreMasterFilters = IgnoreMasterFilters;
		}
		internal void SaveToXml(XElement rootEelement) {
			if (ShouldSerializeToXml) {
				XElement element = new XElement(xmlInteractivityOptionsElementName);
				SaveContentToXml(element);
				rootEelement.Add(element);
			}
		}
		internal void LoadFromXml(XElement rootElement) {
			XElement element = rootElement.Element(xmlInteractivityOptionsElementName);
			if (element != null)
				LoadContentFromXml(element);
		}
		protected virtual bool ShouldSerializeToXml { get { return ignoreMasterFilters != defaultIgnoreMasterFilters; } }
		protected virtual void SaveContentToXml(XElement element) {
			if (ignoreMasterFilters != defaultIgnoreMasterFilters)
				element.Add(new XAttribute(xmlIgnoreMasterFilters, ignoreMasterFilters));
		}
		protected virtual void LoadContentFromXml(XElement element) {
			string ignoreMasterFiltersString = XmlHelper.GetAttributeValue(element, xmlIgnoreMasterFilters);
			if (!String.IsNullOrEmpty(ignoreMasterFiltersString))
				ignoreMasterFilters = XmlHelper.FromString<bool>(ignoreMasterFiltersString);
		}
		protected void OnChanged(ChangeReason reason) {
			if (Changed != null)
				Changed(this, new ChangedEventArgs(reason, this, null));
		}
	}	
}
