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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Templates.ActionContainers.Items;
using DevExpress.ExpressApp.Win.Templates.Bars.Utils;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Templates.Bars.ActionControls {
	public class BarEditItemParametrizedActionControl : BarItemActionControl<BarEditItem>, IParametrizedActionControl {
		private RepositoryItemAdapterBase repositoryItemAdapter;
		private bool showExecuteButton = true;
		private void repositoryItemAdapter_Execute(object sender, ParametrizedActionControlExecuteEventArgs e) {
			RaiseExecute(e.Parameter);
		}
		protected override void UpdatePaintStyle() {
			UpdateAppearance();
		}
		protected override void UpdateCaption() {
			UpdateAppearance();
		}
		protected override void UpdateImage() {
			UpdateAppearance();
		}
		protected virtual void UpdateAppearance() {
			BarItem.PaintStyle = GetActualPaintStyle();
			BarItem.Caption = GetActualCaption();
			BarItem.Glyph = GetImage(GetActualImageName());
			BarItem.LargeGlyph = GetLargeImage(GetActualImageName());
			RepositoryItemAdapter.SetExecuteButtonCaption(GetActualExecuteButtonCaption());
			RepositoryItemAdapter.SetExecuteButtonImage(GetActualExecuteButtonImage());
		}
		protected BarItemPaintStyle GetActualPaintStyle() {
			BarItemPaintStyle result = BarItemPaintStyle.Standard;
			switch(PaintStyle) {
				case ActionItemPaintStyle.Default:
					result = BarItemPaintStyle.Standard;
					break;
				case ActionItemPaintStyle.Caption:
					result = BarItemPaintStyle.Caption;
					break;
				case ActionItemPaintStyle.CaptionAndImage:
					result = BarItemPaintStyle.CaptionGlyph;
					break;
				case ActionItemPaintStyle.Image:
					result = BarItemPaintStyle.CaptionGlyph;
					break;
			}
			return result;
		}
		protected string GetActualCaption() {
			string caption = null;
			if(PaintStyle == ActionItemPaintStyle.Caption || PaintStyle == ActionItemPaintStyle.CaptionAndImage) {
				caption = Caption;
			}
			return caption;
		}
		protected string GetActualImageName() {
			string imageName = null;
			if(PaintStyle == ActionItemPaintStyle.Image || PaintStyle == ActionItemPaintStyle.CaptionAndImage) {
				imageName = ImageName;
			}
			return imageName;
		}
		protected string GetActualExecuteButtonCaption() {
			string caption = null;
			if(PaintStyle == ActionItemPaintStyle.Caption || string.IsNullOrEmpty(ExecuteButtonImageName)) { 
				caption = ExecuteButtonCaption;
			}
			return caption;
		}
		protected Image GetActualExecuteButtonImage() {
			Image image = null;
			if(PaintStyle != ActionItemPaintStyle.Caption) {
				image = GetImage(ExecuteButtonImageName);
			}
			return image;
		}
		protected virtual void UpdateNullValuePrompt() {
			RepositoryItemAdapter.SetNullValuePrompt(NullValuePrompt);
		}
		protected override void OnEndInit() {
			base.OnEndInit();
			if(BarItem.Edit == null) {
				string message = string.Format("Cannot initialize the '{0}' Action Control because its 'BarItem.Edit' property is null.", ActionId);
				throw new InvalidOperationException(message);
			}
			RecreateRepositoryItemAdapter();
		}
		protected void RecreateRepositoryItemAdapter() {
			if(RepositoryItemAdapter != null) {
				RepositoryItemAdapter.Execute -= repositoryItemAdapter_Execute;
			}
			repositoryItemAdapter = CreateRepositoryItemAdapter();
			RepositoryItemAdapter.ShowExecuteButton = ShowExecuteButton;
			RepositoryItemAdapter.SetupRepositoryItem();
			RepositoryItemAdapter.Execute += repositoryItemAdapter_Execute;
		}
		protected virtual RepositoryItemAdapterBase CreateRepositoryItemAdapter() {
			RepositoryItem repositoryItem = BarItem.Edit;
			RepositoryItemAdapterBase repositoryItemAdapter = null;
			if(ParameterType == null) {
				repositoryItemAdapter = new RepositoryItemAdapter(repositoryItem, typeof(object));
			}
			else if(repositoryItem is RepositoryItemButtonEditWithClearButton) {
				repositoryItemAdapter = new RepositoryItemButtonEditWithClearButtonAdapter(BarItem, (RepositoryItemButtonEditWithClearButton)repositoryItem, ParameterType);
			}
			else if(repositoryItem is RepositoryItemButtonEdit) {
				repositoryItemAdapter = new RepositoryItemButtonEditAdapter((RepositoryItemButtonEdit)repositoryItem, ParameterType);
			}
			else if(repositoryItem is RepositoryItemTextEdit) {
				repositoryItemAdapter = new RepositoryItemTextEditAdapter((RepositoryItemTextEdit)repositoryItem, ParameterType);
			}
			else {
				repositoryItemAdapter = new RepositoryItemAdapter(repositoryItem, ParameterType);
			}
			return repositoryItemAdapter;
		}
		protected void RaiseExecute(object parameter) {
			BindingHelper.EndCurrentEdit(Form.ActiveForm);
			if(IsConfirmed() && Execute != null) {
				try {
					Execute(this, new ParametrizedActionControlExecuteEventArgs(parameter));
				}
				catch(ActionExecutionException e) {
					Application.OnThreadException(e.InnerException);
				}
			}
		}
		public BarEditItemParametrizedActionControl() { }
		public BarEditItemParametrizedActionControl(string id, BarEditItem barItem) : base(id, barItem) { }
		public void SetNullValuePrompt(string nullValuePrompt) {
			NullValuePrompt = nullValuePrompt;
			UpdateNullValuePrompt();
		}
		public void SetExecuteButtonCaption(string caption) {
			ExecuteButtonCaption = caption;
			UpdateAppearance();
		}
		public void SetExecuteButtonImage(string imageName) {
			ExecuteButtonImageName = imageName;
			UpdateAppearance();
		}
		public void SetParameterType(Type type) {
			Guard.ArgumentNotNull(type, "type");
			ParameterType = type;
			RecreateRepositoryItemAdapter();
			UpdateNullValuePrompt();
			UpdateAppearance();
		}
		public virtual void SetParameterValue(object value) {
			BarItem.EditValue = value;
		}
		protected RepositoryItemAdapterBase RepositoryItemAdapter {
			get { return repositoryItemAdapter; }
		}
		protected string NullValuePrompt { get; set; }
		protected string ExecuteButtonCaption { get; set; }
		protected string ExecuteButtonImageName { get; set; }
		protected Type ParameterType { get; set; }
		[Description("Specifies whether the execute button will be added to the editor.")]
		public bool ShowExecuteButton {
			get { return showExecuteButton; }
			set {
				if(ShowExecuteButton != value) {
					CheckCanSet("ShowExecuteButton");
					showExecuteButton = value;
				}
			}
		}
		public event EventHandler<ParametrizedActionControlExecuteEventArgs> Execute;
	}
}
