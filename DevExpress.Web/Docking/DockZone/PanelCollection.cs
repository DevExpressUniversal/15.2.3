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
using System.Collections.ObjectModel;
using System.Collections;
using System.Web.UI;
using DevExpress.Web.Internal;
using System.ComponentModel;
namespace DevExpress.Web {
	public class DockPanelCollection : ICollection<ASPxDockPanel> {
		ASPxDockZone zone = null;
		public DockPanelCollection(ASPxDockZone zone) {
			this.zone = zone;
		}
		protected string ZoneUID { get { return this.zone.ZoneUID; } }
		protected Page Page { get { return this.zone.Page; } }
#if !SL
	[DevExpressWebLocalizedDescription("DockPanelCollectionIsReadOnly")]
#endif
		public bool IsReadOnly { get { return false; } }
#if !SL
	[DevExpressWebLocalizedDescription("DockPanelCollectionCount")]
#endif
		public int Count { get { return PanelZoneRelationsMediator.GetZonePanelUIDs(ZoneUID, Page).Count; } }
		public void Add(ASPxDockPanel panel) {
			PanelZoneRelationsMediator.AddRelation(panel.PanelUID, ZoneUID, Page);
		}
		public void Clear() {
			PanelZoneRelationsMediator.RemoveZoneRelations(ZoneUID, Page);
		}
		public bool Contains(ASPxDockPanel panel) {
			return PanelZoneRelationsMediator.GetZonePanelUIDs(ZoneUID, Page).Contains(panel.PanelUID);
		}
		public void CopyTo(ASPxDockPanel[] array, int arrayIndex) {
			PanelZoneRelationsMediator.GetZonePanels(ZoneUID, Page).CopyTo(array, arrayIndex);
		}
		public bool Remove(ASPxDockPanel panel) {
			if (PanelZoneRelationsMediator.GetZonePanelUIDs(ZoneUID, Page).Contains(panel.PanelUID)) {
				PanelZoneRelationsMediator.RemovePanelRelation(panel.PanelUID, Page);
				return true;
			}
			return false;
		}
		public ASPxDockPanel FindByUID(string panelUID) {
			if(PanelZoneRelationsMediator.GetZonePanelUIDs(ZoneUID, Page).Contains(panelUID))
				return PanelZoneRelationsMediator.GetPanel(panelUID, Page);
			return null;
		}
		public IEnumerator<ASPxDockPanel> GetEnumerator() {
			return PanelZoneRelationsMediator.GetZonePanels(ZoneUID, Page).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return (PanelZoneRelationsMediator.GetZonePanels(ZoneUID, Page) as IEnumerable).GetEnumerator();
		}
	}
}
