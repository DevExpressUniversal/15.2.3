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
using DevExpress.Web;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Web.Editors {
	public class ASPxDataWebControlListEditorModelSynchronizer : ModelSynchronizer<ListEditor, IModelListView> {
		private List<IModelSynchronizable> modelSynchronizerList;
		public ASPxDataWebControlListEditorModelSynchronizer(ASPxDataWebControlBase dataBoundControl, ListEditor listEditor, IModelListView model, params ModelSynchronizer[] synchronizersToRun)
			: base(listEditor, model) {
				modelSynchronizerList = new List<IModelSynchronizable>();
			if (dataBoundControl != null) {
				dataBoundControl.DataBound += ModelSynchronizer_ApplyModel;
				dataBoundControl.Disposed += dataBoundControl_Disposed;
			}
			modelSynchronizerList.AddRange(synchronizersToRun);
		}
		private void dataBoundControl_Disposed(object sender, EventArgs e) {
			ASPxDataWebControlBase dataBoundControl = (ASPxDataWebControlBase)sender;
			dataBoundControl.DataBound -= ModelSynchronizer_ApplyModel;
			dataBoundControl.Disposed -= dataBoundControl_Disposed;
		}
		protected override void ApplyModelCore() {
			foreach(IModelSynchronizable synchronizer in modelSynchronizerList) {
				synchronizer.ApplyModel();
			}
		}
		public override void SynchronizeModel() {
			foreach(IModelSynchronizable synchronizer in modelSynchronizerList) {
				synchronizer.SynchronizeModel();
			}
		}
		public override void Dispose() {
			base.Dispose();
			if(modelSynchronizerList != null) {
				modelSynchronizerList.Clear();
				modelSynchronizerList = null;
			}
		}
	}
}
