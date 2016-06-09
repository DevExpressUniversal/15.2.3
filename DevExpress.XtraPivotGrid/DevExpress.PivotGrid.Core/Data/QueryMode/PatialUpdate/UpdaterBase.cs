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
namespace DevExpress.PivotGrid.QueryMode {
	abstract class UpdaterBase<TColumn> where TColumn : QueryColumn {
		IPartialUpdaterOwner<TColumn> owner;
		protected IPartialUpdaterOwner<TColumn> Owner { get { return owner; } }
		protected QueryAreas<TColumn> Areas { get { return owner.Areas; } }
		public UpdaterBase(IPartialUpdaterOwner<TColumn> owner) {
			this.owner = owner;
		}
		public abstract void Update();
		protected static void UpdateFieldValues(AreaFieldValues fv, Action<List<GroupInfo>> action, int level) {
			fv.BeginUpdate();
			if(level == 0)
				fv.ForEachGroupInfo((g, l) => {
					if(l != 0)
						return;
					action(g);
				}, level);
			else
				fv.ForEachGroupInfo((g, l) => {
					if(l != level)
						return;
					action(g);
				}, level);
			fv.EndUpdate();
		}
	}
}
