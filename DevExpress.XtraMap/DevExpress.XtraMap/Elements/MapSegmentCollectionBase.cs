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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraMap {
	public abstract class MapSegmentCollectionBase<T> : SupportCallbackCollection<T> where T : MapSegmentBase {
		readonly object owner;
		protected object Owner { get { return owner; } }
		protected MapPathBase<T> MapPath { get { return owner as MapPathBase<T>; } }
		protected MapSegmentCollectionBase(object owner)
			: base(DXCollectionUniquenessProviderType.None) {
			this.owner = owner;
		}
		protected override void OnInsertComplete(int index, T value) {
			IOwnedElement ownedElement = value as IOwnedElement;
			if(ownedElement != null)
				ownedElement.Owner = owner;
			base.OnInsertComplete(index, value);
		}
		protected override void OnRemoveComplete(int index, T value) {
			IOwnedElement ownedElement = value as IOwnedElement;
			if(ownedElement != null)
				ownedElement.Owner = null;
			base.OnRemoveComplete(index, value);
		}
		protected override bool OnClear() {
			for(int i = 0; i < Count; i++) {
				IOwnedElement ownedElement = this[i] as IOwnedElement;
				if(ownedElement != null)
					ownedElement.Owner = null;
			}
			return base.OnClear();
		}
		protected override void ItemCallbackCore() {
			if(MapPath != null)
				MapPath.OnSegmentsChanged();
			base.ItemCallbackCore();
		}
	}
}
