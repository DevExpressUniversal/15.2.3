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

using System.Windows.Input;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Grid.LookUp.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.Grid.LookUp {
	public class LookUpEditStrategy : LookUpEditStrategyBase, ISelectorEditStrategy {
		new LookUpEdit Editor { get { return base.Editor as LookUpEdit; } }
		internal new GridControlVisualClientOwner VisualClient { get { return base.VisualClient as GridControlVisualClientOwner; } }
		protected override bool IsLockedByValueChanging { get { return base.IsLockedByValueChanging || PopupSizeChangeLocker.IsLocked; } }
		public LookUpEditStrategy(LookUpEdit editor)
			: base(editor) {
		}
		public virtual bool AllowPopupProcessGestures(Key key, ModifierKeys modifiers) {
			if (!VisualClient.IsSearchControlFocused)
				return false;
			if (key == Key.Escape) {
				return !VisualClient.IsSearchTextEmpty;
			}
			return true;
		}
		readonly Locker PopupSizeChangeLocker = new Locker();
		public virtual void SetInitialPopupSize() {
			PopupSizeChangeLocker.DoLockedAction(() => Editor.SetInitialPopupSizeInternal());
		}
		protected override object GetVisibleListSouce() {
			object baseSource = base.GetVisibleListSouce();
			if (IsAsyncServerMode)
				return new AsyncServerModeListSource((AsyncVisibleListWrapper)baseSource);
			if (IsSyncServerMode)
				return new SyncServerModeListSource((SyncVisibleListWrapper)baseSource);
			return baseSource;
		}
	}
}
