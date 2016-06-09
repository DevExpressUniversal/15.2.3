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
	public abstract class MoveLeftCommandBase : TileNavPaneDesignerCommandBase<int> {
		public MoveLeftCommandBase(IModelItem modelItem) : base(modelItem) { }
		public override bool CanExecute(int parameter) {
			return parameter > 0;
		}
		public abstract string PropertyName { get; }
		public override void ExecuteOverride(int parameter) {
			modelItem.Move(PropertyName, parameter, parameter - 1);
		}
	}
	public class MoveTileNavCategoryLeftCommand : MoveLeftCommandBase{
		public MoveTileNavCategoryLeftCommand(IModelItem modelItem)
			: base(modelItem) {
		}
		public override string PropertyName {
			get { return "Categories"; }
		}
	}
	public class MoveTileNavItemLeftCommand : MoveLeftCommandBase {
		public MoveTileNavItemLeftCommand(IModelItem modelItem)
			: base(modelItem) {
																													}
		public override string PropertyName {
			get { return "Items"; }
		}
	}
	public class MoveTileNavSubItemLeftCommand : MoveLeftCommandBase {
		public MoveTileNavSubItemLeftCommand(IModelItem modelItem) : base(modelItem) { }
		public override string PropertyName {
			get { return "Items"; }
		}
	}
	public abstract class MoveRightCommandBase : TileNavPaneDesignerCommandBase<int> {
		public MoveRightCommandBase(IModelItem modelItem) : base(modelItem) { }
		public override bool CanExecute(int parameter) {
			return parameter >= 0 && modelItem != null && parameter < modelItem.CountOf(PropertyName) - 1;
		}
		public abstract string PropertyName { get; }
		public override void ExecuteOverride(int parameter) {
			modelItem.Move(PropertyName, parameter, parameter + 1);
		}
	}
	public class MoveTileNavCategoryRightCommand : MoveRightCommandBase {
		public MoveTileNavCategoryRightCommand(IModelItem modelItem)
			: base(modelItem) { }
		public override string PropertyName {
			get { return "Categories"; }
		}
	}
	public class MoveTileNavItemRightCommand : MoveRightCommandBase {
		public MoveTileNavItemRightCommand(IModelItem modelItem) : base(modelItem) { }
		public override string PropertyName {
			get { return "Items"; }
		}
	}
	public class MoveTileNavSubItemRightCommand : MoveRightCommandBase {
		public MoveTileNavSubItemRightCommand(IModelItem modelItem) : base(modelItem) { }
		public override string PropertyName {
			get { return "Items"; }
		}
	}
}
