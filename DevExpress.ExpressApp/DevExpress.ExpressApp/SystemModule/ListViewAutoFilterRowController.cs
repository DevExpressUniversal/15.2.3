#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.SystemModule {
	public class ListViewAutoFilterRowAttribute : Attribute {
		public static ListViewAutoFilterRowAttribute Default = new ListViewAutoFilterRowAttribute(false);
		private bool showAutoFilterRow = true;
		public ListViewAutoFilterRowAttribute() : this(true) { }
		public ListViewAutoFilterRowAttribute(bool showAutoFilterRow) {
			this.showAutoFilterRow = showAutoFilterRow;
		}
		public bool ShowAutoFilterRow {
			get { return showAutoFilterRow; }
			set { showAutoFilterRow = value; }
		}
	}
	public class AutoFilterRowListViewController : ViewController, IModelExtender {
		public AutoFilterRowListViewController() {
			TargetViewType = ViewType.ListView;
		}
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelClass, IModelClassShowAutoFilterRow>();
			extenders.Add<IModelListView, IModelListViewShowAutoFilterRow>();
		}
		#endregion
	}
	[DomainLogic(typeof(IModelListViewShowAutoFilterRow))]
	public static class ShowAutoFilterRowDomainLogic {
		public static bool Get_ShowAutoFilterRow(IModelListViewShowAutoFilterRow modelListView) {
			if(((IModelListView)modelListView).ModelClass != null) {
				return ((IModelClassShowAutoFilterRow)((IModelListView)modelListView).ModelClass).DefaultListViewShowAutoFilterRow;
			}
			return false;
		}
	}
	[DomainLogic(typeof(IModelClassShowAutoFilterRow))]
	public static class ModelClassShowAutoFilterRowDomainLogic {
		public static bool Get_DefaultListViewShowAutoFilterRow(IModelClass modelClass) {
			ListViewAutoFilterRowAttribute attr = modelClass.TypeInfo.FindAttribute<ListViewAutoFilterRowAttribute>();
			if(attr == null) {
				attr = ListViewAutoFilterRowAttribute.Default;
			}
			return attr.ShowAutoFilterRow;
		}
	}
	public interface IModelListViewShowAutoFilterRow {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewShowAutoFilterRowShowAutoFilterRow"),
#endif
 Category("Behavior")]
		[ModelValueCalculator("ModelClass", "DefaultListViewShowAutoFilterRow")]
		bool ShowAutoFilterRow { get; set; }
	}
	public interface IModelClassShowAutoFilterRow {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassShowAutoFilterRowDefaultListViewShowAutoFilterRow"),
#endif
 Category("Behavior")]
		bool DefaultListViewShowAutoFilterRow { get; set; }
	}
}
