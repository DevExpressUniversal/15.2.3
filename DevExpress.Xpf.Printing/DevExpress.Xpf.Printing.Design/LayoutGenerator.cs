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

using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Printing.Design.LayoutCreators;
using System.Windows;
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.Printing.Design {
	public abstract class GenerateLayoutLineProviderBase : CommandActionLineProvider {
		protected GenerateLayoutLineProviderBase(IPropertyLineContext context)
			: base(context) {
		}
		protected abstract LayoutCreatorBase LayoutCreator { get; }
		protected override void OnCommandExecute(object param) {
			if(MessageBox.Show("The current layout will be cleared. Continue?", "Generate layout", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
				return;
			ModelItem modelItemInstance = XpfModelItem.ToModelItem(Context.ModelItem);
			if(modelItemInstance == null)
				return;
			LayoutCreator.CreateLayout(modelItemInstance);
		}
		private void CreateDockManager(ModelItem modelItemInstance) {
			throw new System.NotImplementedException();
		}
	}
	public class GenerateRibbonLayoutLineProvider : GenerateLayoutLineProviderBase {
		public GenerateRibbonLayoutLineProvider(IPropertyLineContext context)
			: base(context) {
		}
		LayoutCreatorBase creator;
		protected override LayoutCreatorBase LayoutCreator {
			get {
				if(creator == null)
					creator = new RibbonLayoutCreator();
				return creator;
			}
		}
		protected override string GetCommandText() {
			return "Generate Ribbon Layout";
		}
	}
	public class GenerateBarsLayoutLineProvider : GenerateLayoutLineProviderBase {
		public GenerateBarsLayoutLineProvider(IPropertyLineContext context)
			: base(context) {
		}
		LayoutCreatorBase creator;
		protected override LayoutCreatorBase LayoutCreator {
			get {
				if(creator == null)
					creator = new BarsLayoutCreator();
				return creator;
			}
		}
		protected override string GetCommandText() {
			return "Generate Bars Layout";
		}
	}
}
