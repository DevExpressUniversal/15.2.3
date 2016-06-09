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
using System.IO;
using System.Collections.Generic;
namespace DevExpress.XtraLayout.Customization {
	public class UndoManager {
		List<MemoryStream> undoStackCore;
		protected internal int actualUndoStackPosition;
		bool enabled = true;
		ILayoutControl owner;
		public void Reset() {
			actualUndoStackPosition = -1;
			UndoStack.Clear();
		}
		public bool Enabled {
			get {
				if(!owner.OptionsCustomizationForm.EnableUndoManager && owner.UndoManager == this) return false;
				return enabled;
			}
			set { enabled = value; Reset(); }
		}
		public UndoManager(ILayoutControl owner) {
			this.owner = owner;
			if(owner != null) {
				owner.Changed += OnChanged;
				owner.Changing += OnChanging;
			}
			Reset();
		}
		public bool IsUndoAllowed {
			get { return actualUndoStackPosition > 0; }
		}
		public bool IsRedoAllowed {
			get { return actualUndoStackPosition < UndoStack.Count - 1 && actualUndoStackPosition >= 0; }
		}
		public void Dispose() {
			owner.Changed -= OnChanged;
			owner.Changing -= OnChanging;
			Reset();
		}
		protected internal List<MemoryStream> UndoStack {
			get {
				if(undoStackCore == null)
					undoStackCore = new List<MemoryStream>();
				return undoStackCore;
			}
		}
		int lockUndoCount;
		public void LockUndo() {
			lockUndoCount++;
		}
		public void UnlockUndo() {
			lockUndoCount--;
		}
		public bool IsUndoLocked {
			get { return lockUndoCount > 0; }
		}
		protected internal void RestoreUndoUnit() {
			if(IsUndoLocked) return;
			LockUndo();
			Stream stream = UndoStack[actualUndoStackPosition];
			stream.Seek(0, SeekOrigin.Begin);
			if((LayoutControlImplementor)(owner as LayoutControl).implementor != null) {
			LayoutSerializationOptions fakeLayoutSerializationOptions = new LayoutSerializationOptions() { DiscardOldItems = true, RestoreAppearanceItemCaption = true, RestoreAppearanceTabPage = true, RestoreGroupPadding = true, RestoreGroupSpacing = true, RestoreLayoutGroupAppearanceGroup = true, RestoreLayoutItemCustomizationFormText = true, RestoreLayoutItemPadding = true, RestoreLayoutItemSpacing = true, RestoreLayoutItemText = true, RestoreRootGroupPadding = true, RestoreRootGroupSpacing = true, RestoreTabbedGroupPadding = true, RestoreTabbedGroupSpacing = true, RestoreTextToControlDistance = true };
			LayoutSerializationOptions temp = ((LayoutControlImplementor)(owner as LayoutControl).implementor).optionsSerializationCore;
			((LayoutControlImplementor)(owner as LayoutControl).implementor).optionsSerializationCore = fakeLayoutSerializationOptions;		   
				owner.RestoreLayoutFromStream(stream);
				((LayoutControlImplementor)(owner as LayoutControl).implementor).optionsSerializationCore = temp;
		   }else owner.RestoreLayoutFromStream(stream);
			UnlockUndo();
		}
		public void Undo() {
			if(actualUndoStackPosition <= 0) return;
			actualUndoStackPosition--;
			if(actualUndoStackPosition >= 0) {
				RestoreUndoUnit();
			}
		}
		public void Redo() {
			if(UndoStack.Count <= 0) return;
			if(actualUndoStackPosition == UndoStack.Count - 1) return;
			actualUndoStackPosition++;
			if(actualUndoStackPosition <= UndoStack.Count - 1) {
				RestoreUndoUnit();
			}
		}
		protected void SaveToUndoStack() {
			if(IsUndoLocked) return;
			LockUndo();
			MemoryStream stream = new MemoryStream();
			int maxIndex = UndoStack.Count - 1;
			while(actualUndoStackPosition >= 0 && actualUndoStackPosition < maxIndex) {
				UndoStack.RemoveAt(maxIndex);
				maxIndex--;
			}
			owner.SaveLayoutToStream(stream);
			UndoStack.Add(stream);
			actualUndoStackPosition = UndoStack.IndexOf(stream);
			UnlockUndo();
		}
		public event EventHandler UndoStackChanged;
		void OnUndoStackChanged(object sender, EventArgs e) {
			if(UndoStackChanged != null) UndoStackChanged(this, e);
		}
		public void OnChanging(object sender, EventArgs e) {
			if(IsUndoLocked || !Enabled) return;
			if(actualUndoStackPosition < 0) {
				SaveToUndoStack();
			}
		}
		public void OnChanged(object sender, EventArgs e) {
			if(IsUndoLocked || !Enabled) return;
			SaveToUndoStack();
			OnUndoStackChanged(this, EventArgs.Empty);
		}
	}
}
