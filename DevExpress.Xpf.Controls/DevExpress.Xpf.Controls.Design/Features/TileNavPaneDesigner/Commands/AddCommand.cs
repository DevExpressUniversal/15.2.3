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
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Navigation;
using Microsoft.Windows.Design.Model;
namespace DevExpress.Xpf.Controls.Design.Features.TileNavPaneDesigner.Commands {
	public abstract class TileNavPaneDesignerCommandBase<T> : CommandBase<T> {
		protected readonly IModelItem modelItem;
		public Action<T> AfterExecute { get; set; }
		public TileNavPaneDesignerCommandBase(IModelItem modelItem, Action<T> afterExecute = null) {
			this.modelItem = modelItem;
			AfterExecute = afterExecute;
		}
		public abstract void ExecuteOverride(T parameter);
		public sealed override void Execute(T parameter) {
			ExecuteOverride(parameter);
			if(AfterExecute != null) AfterExecute(parameter);
		}
	}
	public abstract class AddCommandBase<T> : TileNavPaneDesignerCommandBase<object> where T : NavElementBase {
		public AddCommandBase(IModelItem modelItem) : base(modelItem) { }
		protected abstract string PropertyName { get; }
		public override void ExecuteOverride(object parameter) {
			modelItem.AddTo(PropertyName, modelItem.New<T>().Set("Content", typeof(T).Name));
		}
	}
	public class AddNewTileNavCategoryCommand : AddCommandBase<TileNavCategory> {
		public AddNewTileNavCategoryCommand(IModelItem modelItem)
			: base(modelItem) {}
		protected override string PropertyName {
			get { return "Categories"; }
		}
	}
	public class AddNewTileNavItemCommand : AddCommandBase<TileNavItem> {
		protected override string PropertyName {
			get { return "Items"; }
		}
		public AddNewTileNavItemCommand(IModelItem modelItem) : base(modelItem) {}
	}
	public class AddNewTileNavSubItemCommand : AddCommandBase<TileNavSubItem> {
		protected override string PropertyName {
			get { return "Items"; }
		}
		public AddNewTileNavSubItemCommand(IModelItem modelItem) : base(modelItem) { }
	}
}
