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
using System.Linq;
using System.Collections.Generic;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	public interface IUndoManager {
		void Add(IUndoUnit undo);
		void Undo();
		IMultyUndoUnit OpenMultyUndoUnit();
		void CloseActiveMultyUndoUnit();
		IMultyUndoUnit ActiveMultyUndoUnit { get; }
		IEnumerable<IUndoUnit> UndoUnits { get; }
	}
	public interface IUndoUnit {
		void Execute();
	}
	public interface IMultyUndoUnit : IUndoUnit {
		void Add(IUndoUnit undo);
		IMultyUndoUnit Parent{ get; }
	}
	public class BaseUndoManager : IUndoManager {
		List<IUndoUnit> items = new List<IUndoUnit>();
		public virtual void Add(IUndoUnit undo) {
			if(ActiveMultyUndoUnit != null)
				ActiveMultyUndoUnit.Add(undo);
			else
				items.Add(undo);
		}
		public virtual IMultyUndoUnit OpenMultyUndoUnit() {
			DefaultMultyUndoUnit multyUndoUnit = new DefaultMultyUndoUnit();
			if(ActiveMultyUndoUnit != null) {
				multyUndoUnit.Parent = ActiveMultyUndoUnit;				
			}
			ActiveMultyUndoUnit = multyUndoUnit;
			return ActiveMultyUndoUnit;
		}
		public virtual void CloseActiveMultyUndoUnit() {
			if(ActiveMultyUndoUnit == null)
				return;
			if(ActiveMultyUndoUnit.Parent != null) {
				ActiveMultyUndoUnit.Parent.Add(ActiveMultyUndoUnit);
				ActiveMultyUndoUnit = ActiveMultyUndoUnit.Parent;
				return;
			}
			IMultyUndoUnit unit = ActiveMultyUndoUnit;
			ActiveMultyUndoUnit = null;
			Add(unit);
		}
		public virtual void Undo() {
			if(ActiveMultyUndoUnit != null)
				CloseActiveMultyUndoUnit();
			if(items.Count == 0)
				return;
			IUndoUnit unit = items[items.Count - 1];
			if(unit != null) {
				items.Remove(unit);
				try {
					unit.Execute();
				}
				catch(Exception ex) {
					Log.SendException(ex);
				}
			}			
		}
		public IMultyUndoUnit ActiveMultyUndoUnit {
			get;
			private set;
		}
		public IEnumerable<IUndoUnit> UndoUnits {
			get { return items; }
		}
	}
	class DefaultMultyUndoUnit : IMultyUndoUnit {
		List<IUndoUnit> items;
		List<IUndoUnit> Items {
			get {
				if(items == null)
					items = new List<IUndoUnit>();
				return items;
			}
		}
		public IMultyUndoUnit Parent { get; internal set; }
		void IUndoUnit.Execute() {
			for(int i = Items.Count - 1; i >= 0; i--) {
				IUndoUnit undo = Items[i];
				if(undo != null) {
					try {
						undo.Execute();
					}
					catch(Exception ex) {
						Log.SendException(ex);
					}
				}
			}
			Items.Clear();
		}
		void IMultyUndoUnit.Add(IUndoUnit undo) {
			if(!Items.Contains(undo))
				Items.Add(undo);
		}
	}
	public class SimpleActionUndoUnit : IUndoUnit {
		readonly Action action;
		public SimpleActionUndoUnit(Action action) {
			this.action = action;
		}
		public void Execute() {
			if(action != null)
				action.Invoke();
		}
	}
}
