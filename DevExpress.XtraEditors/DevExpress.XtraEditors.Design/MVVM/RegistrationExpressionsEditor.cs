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
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms.Design;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.MVVM;
using DevExpress.XtraEditors.Design;
namespace DevExpress.XtraEditors.MVVM.Design {
	public class RegistrationExpressionsEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null || context.Instance == null || provider == null)
				return value;
			var edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(edSvc == null)
				return value;
			using(var form = new RegistrationExpressionsRepositoryEditorForm(context.Instance)) {
				edSvc.ShowDialog(form);
				return value;
			}
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null)
				return UITypeEditorEditStyle.Modal;
			return base.GetEditStyle(context);
		}
	}
	[ToolboxItem(false)]
	public class RegistrationExpressionsRepositoryEditorForm : XtraDesignForm {
		public const string settings = "Software\\Developer Express\\Designer\\MVVM\\";
		RegistrationExpressionsRepositoryEditor editor = null;
		PropertyStore store = null;
		object editValue;
		public RegistrationExpressionsRepositoryEditorForm(object editValue) {
			this.editValue = editValue;
			InitializeComponent();
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			IServiceProvider host = null;
			ILookAndFeelService serv = null;
			if(editValue is Component)
				host = (editValue as Component).Site;
			if(host != null)
				serv = host.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
			if(serv != null && !RegistryDesignerSkinHelper.CanUseDefaultControlDesignersSkin)
				serv.InitializeRootLookAndFeel(this.LookAndFeel);
			else
				LookAndFeel.SetSkinStyle("DevExpress Design");
			editor.InitFrame(editValue, "", null);
			ProductInfo = new DevExpress.Utils.About.ProductInfo(DevExpress.Utils.About.ProductInfoHelper.WinMVVM, typeof(DevExpress.Utils.MVVM.MVVMContext),
				DevExpress.Utils.About.ProductKind.DXperienceWin, DevExpress.Utils.About.ProductInfoStage.Registered);
			this.store = new PropertyStore(settings);
			Store.Restore();
			RestoreProperties();
		}
		protected override string AssemblyName {
			get {
				string result = typeof(MVVMContext).Assembly.ToString();
				return result.Substring(0, result.IndexOf(", Culture"));
			}
		}
		protected PropertyStore Store {
			get { return store; }
		}
		protected override void OnClosed(EventArgs e) {
			if(Store != null) {
				StoreProperties();
				Store.Store();
			}
			base.OnClosed(e);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.editor = new RegistrationExpressionsRepositoryEditor();
			this.SuspendLayout();
			this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editor.HavePG = true;
			this.editor.ID = 6;
			this.editor.Location = new System.Drawing.Point(8, 10);
			this.editor.Name = "editor";
			this.editor.Size = new System.Drawing.Size(608, 462);
			this.editor.TabIndex = 0;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.ClientSize = new System.Drawing.Size(670, 500);
			this.Controls.Add(this.editor);
			this.Name = "RegistrationExpressionsRepositoryEditorForm";
			this.Padding = new System.Windows.Forms.Padding(8, 10, 8, 14);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Services, Behaviors & Messages";
			this.ResumeLayout(false);
		}
		#endregion
		protected virtual void RestoreProperties() {
			if(Store == null) return;
			Store.RestoreForm(this);
			editor.pnlMain.Width = Store.RestoreIntProperty("MainPanelWidth", 190);
			if(editor.pnlMain.Width >= this.Width)
				editor.pnlMain.Width = this.Width / 2;
		}
		protected virtual void StoreProperties() {
			if(Store == null) return;
			Store.AddProperty("MainPanelWidth", editor.pnlMain.Width);
			Store.AddForm(this);
		}
	}
	[ToolboxItem(false)]
	public class RegistrationExpressionsRepositoryEditor : BaseRepositoryEditor {
		public RegistrationExpressionsRepositoryEditor() : base(0) { }
		protected MVVMContext MVVMContext {
			get { return EditingObject as DevExpress.Utils.MVVM.MVVMContext; }
		}
		protected override IServiceProvider GetPropertyGridServiceProvider() {
			return (MVVMContext != null) ? MVVMContext.Site : base.GetPropertyGridServiceProvider();
		}
		RegistrationExpression[] expressions;
		protected override ArrayList GetSortElementList() {
			expressions = MVVMContext.GetServiceRegistrationExpressions();
			return new ArrayList(expressions.Select(e => e.Name).ToArray());
		}
		protected override object GetDefaultElement() {
			return expressions.FirstOrDefault();
		}
		protected override bool ShouldUpdateRepositoryItem(object element) {
			return element is RegistrationExpression;
		}
		protected override bool GetElementVisible(object element) {
			return element is RegistrationExpression;
		}
		protected override string GetDataRepositoryItemName(object element) {
			return ((RegistrationExpression)element).Name;
		}
		protected override string GetDataRepositoryItemType(object element) {
			return ((RegistrationExpression)element).TypeName;
		}
		protected override int GetDataRepositoryItemImageIndex(object element, string type) {
			return Array.IndexOf(expressions, expressions.FirstOrDefault(e => e.GetType().IsAssignableFrom(element.GetType())));
		}
		protected override object GetElement(int index) {
			return expressions[index];
		}
		protected override object GetElementByName(string name) {
			return expressions.Where(e => e.Name == name).FirstOrDefault();
		}
		protected override int FindElement(object element) {
			return Array.IndexOf(expressions, expressions.FirstOrDefault(e => RegistrationExpression.AreEqual(e, element as RegistrationExpression)));
		}
		protected override string GetElementText(object element) {
			return ((RegistrationExpression)element).ToString();
		}
		protected override int GetElementCount() {
			return expressions.Length;
		}
		protected override System.Drawing.Image GetElementImage(object element) {
			if(element is DispatcherServiceRegistrationExpression)
				return DevExpress.XtraEditors.Design.Properties.Resources.DispatcherService_16x16;
			if(element is MessageBoxServiceRegistrationExpression)
				return DevExpress.XtraEditors.Design.Properties.Resources.MessageBoxService_16x16;
			if(element is DialogServiceRegistrationExpression)
				return DevExpress.XtraEditors.Design.Properties.Resources.DialogService_16x16;
			if(element is DocumentManagerServiceRegistrationExpression)
				return DevExpress.XtraEditors.Design.Properties.Resources.DocumentManagerService_16x16;
			if(element is WindowedDocumentManagerServiceRegistrationExpression)
				return DevExpress.XtraEditors.Design.Properties.Resources.WindowedDocumentManagerService_16x16;
			if(element is ConfirmationBehaviorRegistrationExpression)
				return DevExpress.XtraEditors.Design.Properties.Resources.ConfirmationBehavior_16x16;
			if(element is EventToCommandBehaviorRegistrationExpression)
				return DevExpress.XtraEditors.Design.Properties.Resources.EventToCommand_16x16;
			if(element is NotificationServiceRegistrationExpression)
				return DevExpress.XtraEditors.Design.Properties.Resources.NotificationService_16x16;
			if(element is SplashScreenServiceRegistrationExpression)
				return DevExpress.XtraEditors.Design.Properties.Resources.DXSplashScreenService_16x16;
			if(element is LayoutSerializationServiceRegistrationExpression)
				return DevExpress.XtraEditors.Design.Properties.Resources.LayoutSerializationService_16x16;
			return null;
		}
		protected override void AddNewItem(object item) {
			ICloneable cloneable = item as ICloneable;
			if(cloneable != null) {
				MVVMContext.RegistrationExpressions.Add(cloneable.Clone() as RegistrationExpression);
				UpdateButtonText();
				RefreshListBox();
				listView1.SelectedItem = itemsCore[itemsCore.Count - 1];
			}
		}
		protected override void OnRemoveItem(object component) {
			MVVMContext.RegistrationExpressions.Remove(component as RegistrationExpression);
		}
		protected override bool CanRemoveComponent(object element) {
			return MVVMContext.RegistrationExpressions.Contains(element as RegistrationExpression);
		}
		protected override IList GetComponentCollection() {
			return MVVMContext.RegistrationExpressions;
		}
	}
}
