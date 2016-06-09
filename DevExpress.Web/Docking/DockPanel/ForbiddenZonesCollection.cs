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
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class ForbiddenZoneItem : CollectionItem {
		public ForbiddenZoneItem() {
		}
		public ForbiddenZoneItem(string zoneUID) {
			ZoneUID = zoneUID;
		}
		protected internal ASPxDockPanel Panel {
			get {
				if(Collection == null)
					return null;
				return (Collection as ForbiddenZoneCollection).Panel;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ForbiddenZoneItemZoneUID"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.ForbiddenZoneUIDEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		AutoFormatDisable]
		public string ZoneUID
		{
			get { return GetStringProperty("ZoneUID", string.Empty); }
			set { SetStringProperty("ZoneUID", string.Empty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxDockZone Zone {
			get { return PanelZoneRelationsMediator.GetZone(ZoneUID, Panel.Page); }
			set { ZoneUID = value != null ? value.ZoneUID : string.Empty; }
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ForbiddenZoneItem src = source as ForbiddenZoneItem;
			if(src != null)
				ZoneUID = src.ZoneUID;
		}
		public override string ToString() {
			return string.IsNullOrEmpty(ZoneUID) ? GetType().Name : ZoneUID;
		}
	}
	public class ForbiddenZoneCollection : Collection<ForbiddenZoneItem>, ICollection<ASPxDockZone> {
		public ForbiddenZoneCollection(ASPxDockPanel panel)
			: base(panel) {
		}
		protected internal ASPxDockPanel Panel {
			get { return Owner as ASPxDockPanel; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ForbiddenZoneCollectionIsReadOnly")]
#endif
		public bool IsReadOnly { get { return false; } }
		public void Add(ASPxDockZone item) {
			if(!Contains(item))
				Add(new ForbiddenZoneItem(item.ZoneUID));
		}
		public bool Contains(ASPxDockZone item) {
			foreach(ForbiddenZoneItem forbiddenZone in this) {
				if(forbiddenZone.ZoneUID == item.ZoneUID)
					return true;
			}
			return false;
		}
		public void CopyTo(ASPxDockZone[] array, int arrayIndex) {
			ASPxDockZone[] zones = new ASPxDockZone[Count];
			for(int i = 0; i < Count; i++)
				zones[i] = this[i].Zone;
			Array.Copy(zones, 0, array, arrayIndex, Count);
		}
		public bool Remove(ASPxDockZone item) {
			ForbiddenZoneItem forbiddenZoneToRemove = null;
			foreach(ForbiddenZoneItem forbiddenZone in this) {
				if(forbiddenZone.ZoneUID == item.ZoneUID) {
					forbiddenZoneToRemove = forbiddenZone;
					break;
				}
			}
			if(forbiddenZoneToRemove != null) {
				Remove(forbiddenZoneToRemove);
				return true;
			}
			return false;
		}
		IEnumerator<ASPxDockZone> IEnumerable<ASPxDockZone>.GetEnumerator() {
			foreach(ForbiddenZoneItem forbiddenZone in this)
				yield return forbiddenZone.Zone;
		}
	}
}
