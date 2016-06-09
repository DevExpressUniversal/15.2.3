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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Extensions.Localization;
using DevExpress.Utils.UI;
using DevExpress.XtraEditors;
namespace DevExpress.Snap.Extensions.Native {
	public class ParameterCollectionEditorForm : CollectionEditorFormBase {
		ParameterCollectionEditorForm()
			: base(null, null) {
			InitializeComponent();
		}
		public ParameterCollectionEditorForm(IServiceProvider provider, CollectionEditor collectionEditor)
			: base(provider, collectionEditor) {
				InitializeComponent();
		}
		private void InitializeComponent() {
			parametersErrorDialogCaption = SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.ParametersErrorDialogCaption);
			parametersErrorDialogTextInvalidCharacters = SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.ParametersErrorDialogTextInvalidCharacters);
			parametersErrorDialogTextNoName = SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.ParametersErrorDialogTextNoName);
			parametersErrorDialogTextIdenticalNames = SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.ParametersErrorDialogTextIdenticalNames);
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParameterCollectionEditorForm));
			this.SuspendLayout();
			resources.ApplyResources(this, "$this");
			this.Name = "ParameterCollectionEditorForm";
			this.ResumeLayout(false);
		}
		List<Parameter> incorrectParameters = new List<Parameter>();
		List<Parameter> sameNameParameters = new List<Parameter>();
		bool emptyParameters = false;
		string parametersErrorDialogCaption;
		string parametersErrorDialogTextInvalidCharacters;
		string parametersErrorDialogTextNoName;
		string parametersErrorDialogTextIdenticalNames;
		protected override bool IsValueValid() {
			incorrectParameters.Clear();
			sameNameParameters.Clear();
			emptyParameters = false;
			ParameterCollection p1 = (ParameterCollection)EditValue;
			for(int i = 0; i < p1.Count; i++) {
				if(!IsThereIncorrectCharacter(p1[i].Name))
					incorrectParameters.Add(p1[i]);
				if(string.IsNullOrEmpty(p1[i].Name))
					emptyParameters = true;
				if(i < p1.Count - 1) {
					for(int j = (i + 1); j < p1.Count; j++) {
						if(string.Equals(p1[i].Name, p1[j].Name))
							sameNameParameters.Add(p1[i]);
					}
				}
			}
			return incorrectParameters.Count == 0 && !emptyParameters && sameNameParameters.Count == 0;
		}
		protected override void OnFormClosing(System.Windows.Forms.FormClosingEventArgs e) {
			if(incorrectParameters.Count != 0 || sameNameParameters.Count != 0 || emptyParameters) {
				e.Cancel = true;
				incorrectParameters.Clear();
				sameNameParameters.Clear();
				emptyParameters = false;
				return;
			}
			base.OnFormClosing(e);
		}
		bool IsThereIncorrectCharacter(string parameterName) {
			if(parameterName != null) {
				foreach(char ch in parameterName) {
					if(!DevExpress.Data.Filtering.Helpers.CriteriaLexer.CanContinueColumn(ch))
						return false;
				}
			}
			return true;
		}
		protected override void ProcessInvalidValue() {
			string res = String.Empty;
			if(incorrectParameters.Count != 0)
				res = parametersErrorDialogTextInvalidCharacters + ConvertToString(incorrectParameters);
			else if(sameNameParameters.Count != 0)
				res = parametersErrorDialogTextIdenticalNames + ConvertToString(sameNameParameters);
			else if(emptyParameters)
				res = parametersErrorDialogTextNoName;
			XtraMessageBox.Show(res, parametersErrorDialogCaption);
		}
		string ConvertToString(List<Parameter> parameters) {
			string res = String.Empty;
			foreach(var item in parameters)
				res += (string.IsNullOrEmpty(res) ? String.Empty : ", ") + item.Name;
			return res;
		}
	}
	public class ParameterCollectionEditor : CollectionEditor {
		public ParameterCollectionEditor(Type type)
			: base(type) { }
		protected override IList GetEditValue(object value) {
			ParameterCollection collection = new ParameterCollection();
			collection.AddRangeByValue((IList)value);
			return (IList)collection;
		}
		protected override object ApplyNewEditValue(object oldValue, object formEditValue) {
			ParameterCollection p1 = (ParameterCollection)oldValue;
			ParameterCollection p2 = (ParameterCollection)formEditValue;
			p1.Clear();
			for(int i = 0; i < p2.Count; i++)
				p1.Add(p2[i].Clone());
			return p1;
		}
		protected override CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			ParameterCollectionEditorForm form = new ParameterCollectionEditorForm(serviceProvider, this);
			form.ShowDescription = CultureHelper.IsEnglishCulture(CultureInfo.CurrentCulture);
			return form;
		}
	}
}
