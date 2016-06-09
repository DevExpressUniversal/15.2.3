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
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon {
	public abstract class MapLegendBase {
		const string xmlVisible = "Visible";
		const string xmlPosition = "Position";
		const bool DefaultVisible = false;
		const MapLegendPosition DefaultPosition = MapLegendPosition.TopLeft;
		readonly Locker locker = new Locker();
		readonly IChangeService changeService;
		bool visible = DefaultVisible;
		MapLegendPosition position = DefaultPosition;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("MapLegendBaseVisible"),
#endif
		DefaultValue(DefaultVisible)
		]
		public bool Visible {
			get { return visible; }
			set {
				if(value != visible) {
					visible = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("MapLegendBasePosition"),
#endif
		DefaultValue(DefaultPosition)
		]
		public MapLegendPosition Position {
			get { return position; }
			set {
				if(value != position) {
					position = value;
					OnChanged();
				}
			}
		}
		protected abstract string XmlElementName { get; }
		protected IChangeService ChangeService { get { return changeService; } }
		protected MapLegendBase(IChangeService changeService) {
			Guard.ArgumentNotNull(changeService, "changeService");
			this.changeService = changeService;
		}
		internal void BeginUpdate() {
			if(!locker.IsLocked)
				locker.Lock();
		}
		internal void EndUpdate() {
			if(locker.IsLocked) {
				locker.Unlock();
				RaiseChanged();
			}
		}
		internal void SaveToXml(XElement rootElement) {
			XElement element = new XElement(XmlElementName);
			if(visible != DefaultVisible)
				element.Add(new XAttribute(xmlVisible, visible));
			if(position != DefaultPosition)
				element.Add(new XAttribute(xmlPosition, position));
			SaveToXmlInternal(element);
			rootElement.Add(element);
		}
		internal void LoadFromXml(XElement rootElement) {
			XElement element = rootElement.Element(XmlElementName);
			if(element != null) {
				string attribute = XmlHelper.GetAttributeValue(element, xmlVisible);
				if(!String.IsNullOrEmpty(attribute))
					visible = XmlHelper.FromString<bool>(attribute);
				attribute = XmlHelper.GetAttributeValue(element, xmlPosition);
				if(!String.IsNullOrEmpty(attribute))
					position = XmlHelper.EnumFromString<MapLegendPosition>(attribute);
				LoadFromXmlInternal(element);
			}
		}
		protected abstract void SaveToXmlInternal(XElement element);
		protected abstract void LoadFromXmlInternal(XElement element);
		protected void OnChanged() {
			if(!locker.IsLocked)
				RaiseChanged();
		}
		void RaiseChanged() {
			changeService.OnChanged(new ChangedEventArgs(ChangeReason.View, this, null));
		}
		protected void CopyFrom(MapLegendBase source) {
			Visible = source.Visible;
			Position = source.Position;
		}
	}
}
