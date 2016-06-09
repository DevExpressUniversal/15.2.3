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
using System.Windows.Forms;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.ExpressApp.Editors;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Win.Editors {
	public class ProtectedContentPropertyEditor : DXPropertyEditor, IProtectedContentEditor {
		private string protectedContentText;
		protected override object CreateControlCore() {
			return new ProtectedContentEdit();
		}
		protected override bool IsMemberSetterRequired() {
			return true;
		}
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemProtectedContentTextEdit();
		}
		protected override void SetupRepositoryItem(RepositoryItem item) {
			base.SetupRepositoryItem(item);
			((RepositoryItemProtectedContentTextEdit)item).ProtectedContentText = protectedContentText;
		}
		public ProtectedContentPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
		protected override void ReadValueCore() {
		}
		public string ProtectedContentText {
			get { return protectedContentText; }
			set { protectedContentText = value; }
		}
	}
	public class RepositoryItemProtectedContentTextEdit : RepositoryItemTextEdit {
		internal const string EditorName = "ProtectedContentEdit";
		internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(ProtectedContentEdit),
					typeof(RepositoryItemProtectedContentTextEdit),
					typeof(TextEditViewInfo), new TextEditPainter(), true, EditImageIndexes.TextEdit,
					typeof(DevExpress.Accessibility.TextEditAccessible)));
			}
		}
		static RepositoryItemProtectedContentTextEdit() {
			RepositoryItemProtectedContentTextEdit.Register();
		}
		private string protectedContentText = EditorsFactory.ProtectedContentDefaultText;
		public override string GetDisplayText(DevExpress.Utils.FormatInfo format, object editValue) {
			return protectedContentText;
		}
		public RepositoryItemProtectedContentTextEdit() {
			ExportMode = ExportMode.DisplayText; 
		}
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			protectedContentText = ((RepositoryItemProtectedContentTextEdit)item).protectedContentText;
		}
		public override bool ReadOnly {
			get { return true; }
			set { ; }
		}
		public string ProtectedContentText {
			get { return protectedContentText; }
			set { protectedContentText = value; }
		}
		public override string EditorTypeName { get { return EditorName; } }
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class ProtectedContentEdit : StringEdit {
		static ProtectedContentEdit() {
			RepositoryItemProtectedContentTextEdit.Register();
		}
		public ProtectedContentEdit() { }
		public override string EditorTypeName { get { return RepositoryItemProtectedContentTextEdit.EditorName; } }
	}
}
