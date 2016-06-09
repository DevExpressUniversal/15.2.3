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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Editors {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IRepositoryItemCreator {
		RepositoryItem CreateItem(IGridColumnModelSynchronizer xafGridColumnInfo);
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class RepositoryItemCreator : IRepositoryItemCreator {
		private RepositoryEditorsFactory repositoryFactory = null;
		private bool allowEdit;
		private bool readOnlyEditors;
		public RepositoryItemCreator(RepositoryEditorsFactory repositoryFactory, bool allowEdit, bool readOnlyEditors) {
			Guard.ArgumentNotNull(repositoryFactory, "repositoryFactory");
			this.repositoryFactory = repositoryFactory;
			this.allowEdit = allowEdit;
			this.readOnlyEditors = readOnlyEditors;
		}
		#region IRepositoryItemCreator Members
		public RepositoryItem CreateItem(IGridColumnModelSynchronizer xafGridColumnInfo) {
			RepositoryItem result = null;
			if(xafGridColumnInfo.MemberInfo != null) {
				if(xafGridColumnInfo.IsReplacedColumnByAsyncServerMode) {
					MemberEditorInfoCalculator calculator = new MemberEditorInfoCalculator();
					Type editorType = calculator.GetEditorType(xafGridColumnInfo.ModelMember);
					IInplaceEditSupport propertyEditor = Activator.CreateInstance(editorType, xafGridColumnInfo.ObjectTypeInfo.Type, xafGridColumnInfo.Model) as IInplaceEditSupport;
					result = propertyEditor != null ? ((IInplaceEditSupport)propertyEditor).CreateRepositoryItem() : null;
				}
				else {
					result = repositoryFactory.CreateRepositoryItem(false, xafGridColumnInfo.Model, xafGridColumnInfo.ObjectTypeInfo.Type);
				}
				if(result != null) {
					result.ReadOnly |= !allowEdit || readOnlyEditors;
				}
			}
			return result;
		}
		#endregion
	}
}
