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

using System;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.XtraReports.Design {
	public class ParameterCollectionEditorForm : Utils.UI.CollectionEditorFormBase {
		string validationError;
		bool suppressClosing = false;
		public ParameterCollectionEditorForm(IServiceProvider serviceProvider, Utils.UI.CollectionEditor editor)
			: base(serviceProvider, editor) {
				InitializeComponent();
		}
		protected override bool IsValueValid() {
			var collection = EditValue as ParameterCollection;
			if(!CascadingParametersService.ValidateFilterStrings(collection.Cast<Parameter>(), out validationError))
				return false;
			return base.IsValueValid();
		}
		protected override void ProcessInvalidValue() {
			var dialogResult = NotificationService.ShowMessage<PrintingSystem>(
				this.LookAndFeel, this,
				string.Format("{0}\r\n{1}", validationError, ReportLocalizer.GetString(ReportStringId.Msg_ApplyChangesQuestion)),
				this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
			if(dialogResult == System.Windows.Forms.DialogResult.Cancel) {
				suppressClosing = true;
				return;
			} else if(dialogResult == System.Windows.Forms.DialogResult.No) {
				this.collectionEditorContent.Cancel();
				return;
			}
			this.collectionEditorContent.Finish();
		}
		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			e.Cancel = this.DialogResult == System.Windows.Forms.DialogResult.OK && suppressClosing;
			suppressClosing = false;
		}
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParameterCollectionEditorForm));
			this.SuspendLayout();
			resources.ApplyResources(this, "$this");
			this.Name = "ParameterCollectionEditorForm";
			this.ResumeLayout(false);
		}
	}
}
