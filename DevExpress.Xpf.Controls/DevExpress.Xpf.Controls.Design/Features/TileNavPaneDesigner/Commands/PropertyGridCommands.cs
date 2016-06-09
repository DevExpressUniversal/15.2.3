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

using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Core.Design.SmartTags;
using DevExpress.Xpf.Navigation;
using DevExpress.Xpf.PropertyGrid;
using System.Windows.Threading;
namespace DevExpress.Xpf.Controls.Design.Features.TileNavPaneDesigner.Commands {
	public abstract class ChangePropertyCommand : TileNavPaneDesignerCommandBase<CellValueChangedEventArgs> {
		public ChangePropertyCommand(IModelItem modelItem) : base(modelItem) { }
		public abstract string PropertyName { get; }
		public override void ExecuteOverride(CellValueChangedEventArgs parameter) {
			Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
			if(dispatcher != null) {
				AsyncHelper.DoWithDispatcherForce(dispatcher, () =>
				{
					var selectedElement = ((PropertyGridControl)parameter.Source).SelectedObject as NavElementBase;
					if(selectedElement != null) {
						var selectedIndex = selectedElement.Owner.SelectedIndex;
						modelItem.At(PropertyName, selectedElement.Owner.SelectedIndex).Set(parameter.Row.Path, parameter.NewValue);
						selectedElement.Owner.SelectedIndex = selectedIndex;
					}
				});
			}
		}
	}
	public class ChangeCategoryPropertyCommand : ChangePropertyCommand {
		public ChangeCategoryPropertyCommand(IModelItem modelItem) : base(modelItem) { }
		public override string PropertyName {
			get { return "Categories"; }
		}
	}
	public class ChangeItemPropertyCommand : ChangePropertyCommand {
		public ChangeItemPropertyCommand(IModelItem modelItem) : base(modelItem) { }
		public override string PropertyName {
			get { return "Items"; }
		}
	}
	public class ChangeSubItemPropertyCommand : ChangePropertyCommand {
		public ChangeSubItemPropertyCommand(IModelItem modelItem) : base(modelItem) { }
		public override string PropertyName {
			get { return "Items"; }
		}
	}
}
