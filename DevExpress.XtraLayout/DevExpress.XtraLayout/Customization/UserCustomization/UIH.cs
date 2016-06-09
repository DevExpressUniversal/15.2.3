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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraLayout.Customization {
	public class UserInteractionHelper : IDisposable {
		ILayoutControl layout;
		public bool FullCustomization { get; set; }
		protected List<BaseInteraction> interactionsCore;
		List<BaseInteraction> availableInteractions;
		UserInterectionVisualHelper visualHelper;
		public bool allowUndoStackChanged { get; set; }
		bool firstStackUpdate = true;
		Form owner;
		public bool allowChangeSelected {get;set;}
		public UserInteractionHelper(ILayoutControl targetLayoutcontrol, Form ownerForm) {
			interactionsCore = new List<BaseInteraction>();
			layout = targetLayoutcontrol;
			availableInteractions = FillAvailableInteractions();
			layout.ItemSelectionChanged += UpdateInteractions;
			owner = ownerForm;
			visualHelper = CreateUserInteractionVisualHelper();
			FullCustomization = false;
			layout.UndoManager.UndoStackChanged += new EventHandler(UndoManager_UndoStackChanged);
			allowChangeSelected = true;
			allowUndoStackChanged = true;
			UpdateInteractionsCore(true);
		}
		protected virtual UserInterectionVisualHelper CreateUserInteractionVisualHelper() {
			return new UserInterectionVisualHelper(owner, layout);
		}
		void UndoManager_UndoStackChanged(object sender, EventArgs e) {
			if(firstStackUpdate) {
				firstStackUpdate = false;
				return;
			}
			if(allowUndoStackChanged) {
				UpdateInteractionsCore(true);
			} else UpdateInteractionsCore(false);
		}
		public ILayoutControl GetOwner() { return layout; }
		public UserInterectionVisualHelper GetVisualHelper() { return visualHelper; }
		public List<BaseInteraction> GetInteractions() { return interactionsCore; }
		protected List<BaseInteraction> FillAvailableInteractions() {
			List<BaseInteraction> result = new List<BaseInteraction>();
			result.Add(new ButtonsGroupStart(this, null, 4));
			result.Add(new ButtonsGroupStart(this, 4));
			result.Add(new Load(this));
			result.Add(new Save(this));
			result.Add(new Undo(this));
			result.Add(new Redo(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, 4));
			result.Add(new EmptySpace(this));
			result.Add(new BestFit(this));			
			result.Add(new HideItem(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, 4));
			result.Add(new Group(this));
			result.Add(new Ungroup(this));
			result.Add(new Tabbed(this));
			result.Add(new UnTabbed(this));
			result.Add(new AddTab(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, 1));
			result.Add(new GetParent(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, "Text", 3));
			result.Add(new ButtonsGroupStart(this, 1));
			result.Add(new Rename(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, 4, "Text position"));
			result.Add(new Left(this));
			result.Add(new Top(this));
			result.Add(new Bottom(this));		   
			result.Add(new Right(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, 1));
			result.Add(new HideText(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, "Text alignment", 3));
			result.Add(new ButtonsGroupStart(this, 3));
			result.Add(new TextTopLeft(this));
			result.Add(new TextTopCenter(this));
			result.Add(new TextTopRight(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, 3));
			result.Add(new TextMiddleLeft(this));
			result.Add(new TextMiddleCenter(this));
			result.Add(new TextMiddleRight(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, 3));
			result.Add(new TextBottomLeft(this));
			result.Add(new TextBottomCenter(this));
			result.Add(new TextBottomRight(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, "Control alignment", 3));
			result.Add(new ButtonsGroupStart(this, 3));
			result.Add(new ControlTopLeft(this));
			result.Add(new ControlTopCenter(this));
			result.Add(new ControlTopRight(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, 3));
			result.Add(new ControlMiddleLeft(this));
			result.Add(new ControlMiddleCenter(this));
			result.Add(new ControlMiddleRight(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, 3));
			result.Add(new ControlBottomLeft(this));
			result.Add(new ControlBottomCenter(this));
			result.Add(new ControlBottomRight(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this,null, 2));
			result.Add(new ButtonsGroupStart(this, 4));
			result.Add(new SizeConstraintsFreeSizing(this));
			result.Add(new SizeConstraintsLockHeight(this));
			result.Add(new SizeConstraintsLockWidth(this));
			result.Add(new SizeConstraintsLockSize(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, 1));
			result.Add(new SizeConstraintsResetToDefault(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this,null,  1));
			result.Add(new ButtonsGroupStart(this, 1));
			result.Add(new NoSelection(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupEnd(this));
			result.Add(new ButtonsGroupStart(this, 1));
			result.Add(new CustomizeLayout(this));
			result.Add(new ButtonsGroupEnd(this));
			return result;
		}
		public List<BaseLayoutItem> GetSelection() {
			SelectionHelper sh = new SelectionHelper();
			return sh.GetItemsList(layout.RootGroup);
		}
		void UpdateInteractions(object sender, EventArgs e) {
			UpdateInteractionsCore(true);
		}
		public void UpdateInteractions(bool force) {
			UpdateInteractionsCore(force);
		}
		private void UpdateInteractionsCore(bool isFullRefresh) {
			availableInteractions = FillAvailableInteractions();
			firstStackUpdate = false;
			if(!allowChangeSelected) return;
			if(!((ILayoutControl)layout).EnableCustomizationMode) return;
			layout.LongPressControl.Enabled = false;
			List<BaseLayoutItem> selection = GetSelection();
			interactionsCore.Clear();
			foreach(BaseInteraction current in availableInteractions) {
				if(current.CanExecute()) interactionsCore.Add(current);
			}
			if(isFullRefresh) {
				visualHelper.Show(interactionsCore, FullCustomization);
			} else visualHelper.UpdateButtonEnable(interactionsCore, FullCustomization);
		}
		public void Dispose() {
			layout.ItemSelectionChanged -= UpdateInteractions;
			layout.UndoManager.UndoStackChanged -= new EventHandler(UndoManager_UndoStackChanged);
			layout = null;
			if(visualHelper != null) {
				visualHelper.Dispose();
				visualHelper = null;
			}
		}
	}
}
