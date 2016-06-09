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
using System.Web.UI;
using System.Collections;
using DevExpress.Web.Internal;
using System.Web;
namespace DevExpress.Web.Internal {
	public static class PanelZoneRelationsMediator {
		const char DesignTimeStorageKeySeparator = '_';
		const string
			RelationsDictionaryKey = "e677924c-54ce-4b14-96b4-2d08aed5c793",
			ZoneControlsDictionaryKey = "939ce798-515e-4c67-b69a-d49b40967a74",
			PanelControlsDictionaryKey = "0a617168-373d-4a16-bff4-c2cfd7c60d57",
			PanelInitialLayoutStateDictionaryKey = "ddbae298-edd7-4046-a6b1-89710355f94d",
			ManagerKey = "75025c0b-ff9d-407e-9a2f-ef6c73bddc60";
		static Hashtable designModeStorage = null;
		static Hashtable DesignModeStorage {
			get {
				if(designModeStorage == null)
					designModeStorage = new Hashtable();
				return designModeStorage;
			}
		}
		static string GetDesignModeStorageKey(string key, Page page) {
			string pageName = page != null ? page.GetType().FullName : string.Empty;
			return pageName + DesignTimeStorageKeySeparator + key;
		}
		static void SetStorageValue<T>(string key, T value, Page page) {
			if(HttpContext.Current != null)
				HttpUtils.SetContextValue<T>(key, value);
			else
				DesignModeStorage[GetDesignModeStorageKey(key, page)] = value;
		}
		static T GetStorageValue<T>(string key, T defaultValue, Page page) {
			if(HttpContext.Current != null)
				return HttpUtils.GetContextValue<T>(key, defaultValue);
			string designTimeKey = GetDesignModeStorageKey(key, page);
			return DesignModeStorage.Contains(designTimeKey) ? (T)DesignModeStorage[designTimeKey] : defaultValue;
		}
		static Hashtable GetDictionary(string key, Page page) {
			if(GetStorageValue<Hashtable>(key, null, page) == null)
				SetStorageValue<Hashtable>(key, new Hashtable(), page);
			return GetStorageValue<Hashtable>(key, null, page);
		}
		static Hashtable GetRelationsDictionary(Page page) {
			return GetDictionary(RelationsDictionaryKey, page);
		}
		static Hashtable GetZoneControlsDictionary(Page page) {
			return GetDictionary(ZoneControlsDictionaryKey, page);
		}
		static Hashtable GetPanelControlsDictionary(Page page) {
			return GetDictionary(PanelControlsDictionaryKey, page);
		}
		internal static Hashtable GetPanelInitialLayoutStateDictionary(Page page) {
			return GetDictionary(PanelInitialLayoutStateDictionaryKey, page);
		}
		internal static ASPxDockManager GetManager(Page page) {
			return GetStorageValue<ASPxDockManager>(ManagerKey, null, page);
		}
		public static void RegisterZone(ASPxDockZone zone, Page page) {
			Hashtable zoneControlsDictionary = GetZoneControlsDictionary(page);
			if(!zone.DesignMode) {
				if(string.IsNullOrEmpty(zone.ZoneUID))
					throw new ArgumentNullException("ZoneUID");
				if(zoneControlsDictionary.ContainsKey(zone.ZoneUID))
					throw new ArgumentException(StringResources.Docking_ErrorNonUniqueZoneUID);
			}
			zoneControlsDictionary[zone.ZoneUID] = zone;
		}
		public static void RegisterPanel(ASPxDockPanel panel, Page page) {
			Hashtable panelControlsDictionary = GetPanelControlsDictionary(page);
			Hashtable panelInitialLayoutStateDictionary = GetPanelInitialLayoutStateDictionary(page);
			if(!panel.DesignMode) {
				if(string.IsNullOrEmpty(panel.PanelUID))
					throw new ArgumentNullException("PanelUID");
				if(panelControlsDictionary.ContainsKey(panel.PanelUID))
					throw new ArgumentException(StringResources.Docking_ErrorNonUniquePanelUID);
			}
			panelControlsDictionary[panel.PanelUID] = panel;
			panelInitialLayoutStateDictionary[panel.PanelUID] = panel.GetLayoutState();
		}
		public static void RegisterManager(ASPxDockManager manager, Page page) {
			if(GetManager(page) != null)
				throw new InvalidOperationException(StringResources.Docking_ErrorMultipleManagers);
			SetStorageValue<ASPxDockManager>(ManagerKey, manager, page);
		}
		public static void AddRelation(string panelUID, string zoneUID, Page page) {
			Hashtable relationsDictionary = GetRelationsDictionary(page);
			relationsDictionary[panelUID] = zoneUID;
		}
		public static void RemoveZoneRelations(string zoneUID, Page page) {
			Hashtable relationsDictionary = GetRelationsDictionary(page);
			List<string> panelUIDs = GetZonePanelUIDs(zoneUID, page);
			foreach(string panelUID in panelUIDs)
				relationsDictionary[panelUID] = string.Empty;
		}
		public static void RemovePanelRelation(string panelUID, Page page) {
			Hashtable relationsDictionary = GetRelationsDictionary(page);
			relationsDictionary[panelUID] = string.Empty;
		}
		public static string GetPanelZoneID(string panelUID, Page page) {
			Hashtable relationsDictionary = GetRelationsDictionary(page);
			return (relationsDictionary[panelUID] as string) ?? string.Empty;
		}
		public static ASPxDockZone GetPanelZone(string panelUID, Page page) {
			string zoneUID = GetPanelZoneID(panelUID, page);
			if(!string.IsNullOrEmpty(zoneUID))
				return GetZone(zoneUID, page);
			return null;
		}
		public static ASPxDockZone GetZone(string zoneUID, Page page) {
			Hashtable zoneControlsDictionary = GetZoneControlsDictionary(page);
			return zoneControlsDictionary[zoneUID] as ASPxDockZone;
		}
		public static IEnumerable<ASPxDockZone> GetZones(Page page) {
			Hashtable zoneControlsDictionary = GetZoneControlsDictionary(page);
			foreach(ASPxDockZone zone in zoneControlsDictionary.Values)
				yield return zone;
		}
		public static ASPxDockPanel GetPanel(string panelUID, Page page) {
			Hashtable panelControlsDictionary = GetPanelControlsDictionary(page);
			return panelControlsDictionary[panelUID] as ASPxDockPanel;
		}
		public static IEnumerable<ASPxDockPanel> GetPanels(Page page) {
			Hashtable panelControlsDictionary = GetPanelControlsDictionary(page);
			foreach(ASPxDockPanel panel in panelControlsDictionary.Values)
				yield return panel;
		}
		public static List<string> GetZonePanelUIDs(string zoneUID, Page page) {
			Hashtable relationsDictionary = GetRelationsDictionary(page);
			List<string> panelsUIDs = new List<string>();
			foreach(DictionaryEntry entry in relationsDictionary) {
				if(entry.Value.ToString() == zoneUID)
					panelsUIDs.Add(entry.Key.ToString());
			}
			return panelsUIDs;
		}
		public static List<ASPxDockPanel> GetZonePanels(string zoneUID, Page page) {
			Hashtable panelControlsDictionary = GetPanelControlsDictionary(page);			
			List<string> panelUIDs = GetZonePanelUIDs(zoneUID, page);
			List<ASPxDockPanel> panels = new List<ASPxDockPanel>();
			foreach(string panelUID in panelUIDs) {
				ASPxDockPanel panel = panelControlsDictionary[panelUID] as ASPxDockPanel;
				if(panel != null)
					panels.Add(panel);
			}
			return panels;
		}
	}
}
