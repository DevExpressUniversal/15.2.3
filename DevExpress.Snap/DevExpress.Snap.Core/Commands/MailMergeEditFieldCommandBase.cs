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

using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Options;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Fields;
namespace DevExpress.Snap.Core.Commands {
	public abstract class MailMergeEditFieldCommandBase : EditFieldCommandBase<CalculatedFieldBase, SnapFieldInfo> {
		readonly IDataAccessService dataAccessService;
		protected MailMergeEditFieldCommandBase(IRichEditControl control)
			: base(control) {
				this.dataAccessService = this.Control.GetService(typeof(IDataAccessService)) as IDataAccessService;
		}
		protected IDataAccessService DataAccessService { get { return dataAccessService; } }
		protected SnapMailMergeVisualOptions MailMergeVisulaOptions { get { return DocumentModel.SnapMailMergeVisualOptions; } }
		protected override SnapFieldInfo GetEditedField() {
			return FieldsHelper.GetSelectedField(DocumentModel);
		}
		protected override bool IsEnabled() {
			if (!base.IsEnabled() || this.dataAccessService == null)
				return false;
			if (DocumentModel.SnapMailMergeVisualOptions.DataSourceName == null)
				return false;
			DesignBinding binding = FieldsHelper.GetFieldDesignBinding(DataSourceDispatcher, EditedFieldInfo);
			if (binding == null || binding.IsNull)
				return false;
			return IsEnabledCore(binding);
		}
		protected abstract bool IsEnabledCore(DesignBinding binding);
	}
}
