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
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Editors {
#pragma warning disable 0618
	public class CheckedListBoxStringPropertyEditor : CheckedListBoxPropertyEditor {
#pragma warning restore 0618
		public CheckedListBoxStringPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
	}
	#region Obsolete 14.2
	[Obsolete("Use the 'DevExpress.ExpressApp.Win.Editors.CheckedListBoxStringPropertyEditor' class instead.")]
	[PropertyEditor(typeof(String), false)]
	public class CheckedListBoxPropertyEditor : DXPropertyEditor {
		public CheckedListBoxPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
		protected override object CreateControlCore() {
			CheckedComboBoxEdit control = new CheckedComboBoxEdit();
			control.Properties.ReadOnly = false;
			control.Properties.SeparatorChar = ';';
			return control;
		}
		protected override DevExpress.XtraEditors.Repository.RepositoryItem CreateRepositoryItem() {
			RepositoryItemCheckedComboBoxEdit repositoryItem = new RepositoryItemCheckedComboBoxEdit();
			repositoryItem.SeparatorChar = ';';
			return repositoryItem;
		}
		public new CheckedComboBoxEdit Control {
			get { return (CheckedComboBoxEdit)base.Control; }
		}
		public static void FillItems(RepositoryItemCheckedComboBoxEdit repositoryItem, ICheckedListBoxItemsProvider provider, string propertyName) {
			if((repositoryItem != null) && (provider != null)) {
				repositoryItem.Items.Clear();
				Dictionary<Object, String> listBoxItems = provider.GetCheckedListBoxItems(propertyName);
				foreach(KeyValuePair<Object, String> pair in listBoxItems.OrderBy(key => key.Value)) {
					repositoryItem.Items.Add(pair.Key, pair.Value);
				}
			}
		}
	}
	#endregion
}
