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
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraMap {
	public class MapOverlayCollection : SupportCallbackCollection<MapOverlay> {
		readonly InnerMap map;
		protected internal MapOverlayCollection(InnerMap map) {
			this.map = map;
		}
		protected override void ItemCallbackCore() {
			if(Callback != null)
				Callback();
			base.ItemCallbackCore();
		}
		protected override void OnInsertComplete(int index, MapOverlay value) {
			((IOwnedElement)value).Owner = map;
			base.OnInsertComplete(index, value);
		}
		protected override void OnRemoveComplete(int index, MapOverlay value) {
			((IOwnedElement)value).Owner = null;
			base.OnRemoveComplete(index, value);
		}
		protected override bool OnClear() {
			for(int i = 0; i < Count; i++)
				((IOwnedElement)this[i]).Owner = null;
			return base.OnClear();
		}
	}
	public class MapOverlayItemCollection : SupportCallbackCollection<MapOverlayItemBase> {
		readonly MapOverlay overlay;
		protected internal MapOverlayItemCollection(MapOverlay overlay) {
			this.overlay = overlay;
		}
		protected override void ItemCallbackCore() {
			if(Callback != null)
				Callback();
			base.ItemCallbackCore();
		}
		protected override void OnInsertComplete(int index, MapOverlayItemBase value) {
			IOwnedElement owned = value as IOwnedElement;
			if(owned != null)
				owned.Owner = overlay;
			base.OnInsertComplete(index, value);
		}
		protected override void OnRemoveComplete(int index, MapOverlayItemBase value) {
			IOwnedElement owned = value as IOwnedElement;
			if(owned != null)
				owned.Owner = null;
			base.OnRemoveComplete(index, value);
		}
		protected override bool OnClear() {
			for(int i = 0; i < Count; i++) {
				IOwnedElement owned = this[i] as IOwnedElement;
				if(owned != null)
					owned.Owner = null;
			}
			return base.OnClear();
		}
	}
}
