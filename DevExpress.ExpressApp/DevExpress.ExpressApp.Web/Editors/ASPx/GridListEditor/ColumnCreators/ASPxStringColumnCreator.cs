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

using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	internal class ASPxStringColumnCreator : DataColumnCreatorBase {
		private int rowCount;
		public ASPxStringColumnCreator(IModelColumn columnInfo)
			: base(columnInfo) {
			rowCount = columnInfo.RowCount;
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
		protected override GridViewDataColumn CreateGridViewColumnCore() {
			GridViewDataColumn result = null;
			if(rowCount <= 1) {
				result = new GridViewDataTextColumn();
				SetupTextBox((TextBoxProperties)result.PropertiesEdit);
			}
			else {
				result = new GridViewDataMemoColumn();
				SetupASPxMemo((MemoProperties)result.PropertiesEdit);
			}
			return result;
		}
		protected void SetupTextBox(TextBoxProperties properties) {
			properties.Password = Model.IsPassword;
			properties.MaxLength = GetMaxLength(Model);
		}
		protected void SetupASPxMemo(MemoProperties properties) {
			properties.Rows = Model.RowCount;
			properties.MaxLength = GetMaxLength(Model);
		}
		protected int GetMaxLength(IModelMemberViewItem columnInfo) {
			return PropertyEditorHelper.CalcMaxLengthCore(columnInfo.MaxLength, columnInfo.ModelMember.MemberInfo.ValueMaxLength);
		}
	}
}
