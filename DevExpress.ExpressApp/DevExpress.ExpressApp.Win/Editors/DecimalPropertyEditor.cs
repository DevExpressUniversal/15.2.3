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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Drawing;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Win.Editors {
	public class DecimalPropertyEditor : FloatPropertyEditor {
		protected override object CreateControlCore() {
			return new DecimalEdit();
		}
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemDecimalEdit();
		}
		public DecimalPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
	}
	public class RepositoryItemDecimalEdit : RepositoryItemSingleEdit {
		protected internal new const string EditorName = "DecimalEdit";
		protected internal new static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(DecimalEdit),
					typeof(RepositoryItemDecimalEdit), typeof(BaseSpinEditViewInfo),
					new ButtonEditPainter(), true, EditImageIndexes.SpinEdit, typeof(DevExpress.Accessibility.BaseSpinEditAccessible)));
			}
		}
		static RepositoryItemDecimalEdit() {
			RepositoryItemDecimalEdit.Register();
		}
		public RepositoryItemDecimalEdit(string editMask, string displayFormat)
			: this() {
			Init(editMask, displayFormat);
		}
		public RepositoryItemDecimalEdit() : base() { }
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class DecimalEdit : SingleEdit {
		static DecimalEdit() {
			RepositoryItemDecimalEdit.Register();
		}
		public DecimalEdit() { }
		public DecimalEdit(string editMask, string displayFormat) : base(editMask, displayFormat) { }
		public override string EditorTypeName { get { return RepositoryItemDecimalEdit.EditorName; } }
	}
}
