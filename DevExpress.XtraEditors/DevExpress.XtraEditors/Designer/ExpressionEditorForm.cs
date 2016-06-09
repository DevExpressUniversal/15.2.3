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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data;
using DevExpress.Data.ExpressionEditor;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraEditors.Design {
	public abstract class ExpressionEditorForm : XtraForm, IExpressionEditor {
		internal static ExpressionEditorForm CreateInstance(object context, IDesignerHost designerHost) {
			return new UnboundColumnExpressionEditorForm(context, designerHost);
		}
		#region fields
		public SimpleButton buttonCancel;
		MemoEdit expressionMemoEdit;
		LayoutItemButton plusItemButton;
		LayoutItemButton layoutItemButton2;
		LayoutItemButton layoutItemButton3;
		LayoutItemButton layoutItemButton4;
		LayoutItemButton layoutItemButton5;
		LabelControl labelControl1;
		LayoutItemButton layoutItemButton6;
		LayoutItemButton layoutItemButton7;
		LayoutItemButton layoutItemButton8;
		LayoutItemButton layoutItemButton9;
		LayoutItemButton layoutItemButton10;
		LayoutItemButton layoutItemButton11;
		LabelControl labelControl2;
		LayoutItemButton layoutItemButton12;
		LayoutItemButton layoutItemButton13;
		LayoutItemButton layoutItemButton14;
		LabelControl labelControl3;
		MemoEdit descriptionControl;
		ListBoxControl listOfInputTypes;
		ListBoxControl listOfInputParameters;
		ComboBoxEdit functionsTypes;
		LayoutItemButton layoutItemButton15;
		LabelControl labelControl4;
		public SimpleButton buttonOK;
		Hashtable buttonStrings;
		ImageCollection icons;
		ComponentResourceManager resources;
		object contextInstance;
		IServiceProvider serviceProvider;
		#endregion
		ExpressionEditorLogic _internalEditorLogic;
		protected readonly IMemoEdit ExpressionMemoEdit;
		readonly ISelector ListOfInputTypes;
		readonly ISelector ListOfInputParameters;
		readonly ISelector FunctionsTypes;
		public ExpressionEditorForm() : this(null, null) { }
		public ExpressionEditorForm(object contextInstance, IServiceProvider servProvider) {
			this.contextInstance = ExtractContext(contextInstance);
			this.serviceProvider = servProvider;
			InitializeComponent();
			SetParentLookAndFeel();
			UpdateIcons();
			AssingButtonImages();
			functionsTypes.EditValue = "(All)";
			InitializeButtons();
			ExpressionMemoEdit = new MemoEditWrapper(expressionMemoEdit);
			ListOfInputTypes = new ListBoxControlWrappper(listOfInputTypes);
			ListOfInputParameters = new ListBoxControlWrappper(listOfInputParameters);
			FunctionsTypes = new ComboBoxEditWrappper(functionsTypes);
			InitializeEditorLogic();
			if(WindowsFormsSettings.GetIsRightToLeft(this))
				CalcRightToLeftLayout();
		}
		void CalcRightToLeftLayout() {
			this.buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			this.buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			buttonCancel.Left = this.ClientSize.Width - buttonCancel.Right;
			buttonOK.Left = this.ClientSize.Width - buttonOK.Right;
			Rectangle rectTemp = new Rectangle(descriptionControl.Location, descriptionControl.Size);
			AnchorStyles aTemp = descriptionControl.Anchor;
			descriptionControl.Anchor = listOfInputTypes.Anchor;
			listOfInputTypes.Anchor = aTemp;
			descriptionControl.Bounds = listOfInputTypes.Bounds;
			listOfInputTypes.Bounds = rectTemp;
			layoutItemButton10.Anchor = layoutItemButton11.Anchor = layoutItemButton12.Anchor = layoutItemButton13.Anchor = layoutItemButton14.Anchor = 
				layoutItemButton15.Anchor = layoutItemButton2.Anchor = layoutItemButton3.Anchor = layoutItemButton4.Anchor = layoutItemButton5.Anchor = 
				layoutItemButton6.Anchor = layoutItemButton7.Anchor = layoutItemButton8.Anchor = layoutItemButton9.Anchor = 
				labelControl1.Anchor = labelControl2.Anchor = plusItemButton.Anchor = labelControl4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		}
		protected virtual void UpdateIcons() {
			Image iconsImage = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Images.FormulaWizardIcons.png", typeof(BaseEdit).Assembly);
			iconsImage = ImageColorizer.GetColoredImage(iconsImage, CommonSkins.GetSkin(this.LookAndFeel)[CommonSkins.SkinForm].Color.GetForeColor());
			icons = Utils.Controls.ImageHelper.CreateImageCollectionCore(iconsImage, new Size(24, 24), Color.Empty);
		}
		protected virtual void AssingButtonImages() {
			plusItemButton.Image = icons.Images[0];
			layoutItemButton2.Image = icons.Images[1];
			layoutItemButton3.Image = icons.Images[2];
			layoutItemButton4.Image = icons.Images[3];
			layoutItemButton5.Image = icons.Images[4];
			layoutItemButton15.Image = icons.Images[5];
			layoutItemButton6.Image = icons.Images[6];
			layoutItemButton7.Image = icons.Images[7];
			layoutItemButton8.Image = icons.Images[8];
			layoutItemButton9.Image = icons.Images[9];
			layoutItemButton11.Image = icons.Images[10];
			layoutItemButton10.Image = icons.Images[11];
			layoutItemButton12.Image = icons.Images[12];
			layoutItemButton13.Image = icons.Images[13];
			layoutItemButton14.Image = icons.Images[14];
		}
		protected ExpressionEditorLogic fEditorLogic { get { return _internalEditorLogic; } }
		protected virtual void InitializeEditorLogic() {
			this._internalEditorLogic = CreateExpressionEditorLogic();
			this._internalEditorLogic.Initialize();
		}
		protected virtual object ExtractContext(object contextInstance) {
			if(contextInstance is IDataColumnInfoProvider) {
				contextInstance = ((IDataColumnInfoProvider)contextInstance).GetInfo(null);
			}
			return contextInstance;
		}
		protected abstract ExpressionEditorLogic CreateExpressionEditorLogic();
		protected virtual string CaptionName { get { return string.Empty; } }
		protected virtual ISupportLookAndFeel LookAndFeelInfo { get { return ContextInstance as ISupportLookAndFeel; } }
		protected object ContextInstance { get { return contextInstance; } }
		protected IServiceProvider ServiceProvider { get { return serviceProvider; } }
		public string Expression { get { return fEditorLogic.GetExpression(); } }
		public void SetMenuManager(IDXMenuManager manager) {
			expressionMemoEdit.MenuManager = manager;
			descriptionControl.MenuManager = manager;
		}
		void InitializeComponent() {
			resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpressionEditorForm));
			this.buttonOK = new DevExpress.XtraEditors.SimpleButton();
			this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
			this.expressionMemoEdit = new DevExpress.XtraEditors.MemoEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.descriptionControl = new DevExpress.XtraEditors.MemoEdit();
			this.listOfInputTypes = new DevExpress.XtraEditors.ListBoxControl();
			this.listOfInputParameters = new DevExpress.XtraEditors.ListBoxControl();
			functionsTypes = new ComboBoxEdit();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.layoutItemButton15 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.layoutItemButton12 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.layoutItemButton13 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.layoutItemButton14 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.layoutItemButton11 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.layoutItemButton10 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.layoutItemButton9 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.layoutItemButton8 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.layoutItemButton7 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.layoutItemButton6 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.layoutItemButton5 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.layoutItemButton4 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.layoutItemButton3 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.layoutItemButton2 = new DevExpress.XtraReports.Native.LayoutItemButton();
			this.plusItemButton = new DevExpress.XtraReports.Native.LayoutItemButton();
			((System.ComponentModel.ISupportInitialize)(this.expressionMemoEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.listOfInputTypes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.listOfInputParameters)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.buttonOK.Click += new EventHandler(buttonOK_Click);
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.expressionMemoEdit.Properties.ScrollBars = ScrollBars.Vertical;
			this.expressionMemoEdit.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.expressionMemoEdit.Properties.HideSelection = false;
			resources.ApplyResources(this.expressionMemoEdit, "expressionEdit");
			this.expressionMemoEdit.Name = "expressionEdit";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.AutoSizeMode = LabelAutoSizeMode.None;
			this.labelControl2.AutoSizeMode = LabelAutoSizeMode.None;
			this.labelControl3.AutoSizeMode = LabelAutoSizeMode.None;
			this.labelControl4.AutoSizeMode = LabelAutoSizeMode.None;
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.labelControl2.LineVisible = true;
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.labelControl3, "labelControl3");
			this.labelControl3.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.labelControl3.LineVisible = true;
			this.labelControl3.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
			this.labelControl3.Name = "labelControl3";
			this.descriptionControl.TabStop = false;
			this.descriptionControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.descriptionControl.Properties.ReadOnly = true;
			this.descriptionControl.Properties.ScrollBars = ScrollBars.None;
			this.descriptionControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			resources.ApplyResources(this.descriptionControl, "descriptionControl");
			this.descriptionControl.Name = "descriptionControl";
			this.listOfInputTypes.HighlightedItemStyle = DevExpress.XtraEditors.HighlightStyle.Skinned;
			this.listOfInputTypes.HotTrackItems = LookAndFeelIsSkin();
			this.listOfInputTypes.HotTrackSelectMode = DevExpress.XtraEditors.HotTrackSelectMode.SelectItemOnClick;
			resources.ApplyResources(this.listOfInputTypes, "listOfInputTypes");
			this.listOfInputTypes.Name = "listOfInputTypes";
			this.listOfInputTypes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			this.listOfInputTypes.SelectedValueChanged += new EventHandler(listOfInputTypes_SelectedValueChanged);
			this.listOfInputParameters.HighlightedItemStyle = DevExpress.XtraEditors.HighlightStyle.Skinned;
			this.listOfInputParameters.HorizontalScrollbar = true;
			this.listOfInputParameters.HotTrackItems = true;
			this.listOfInputParameters.HotTrackSelectMode = DevExpress.XtraEditors.HotTrackSelectMode.SelectItemOnClick;
			this.listOfInputParameters.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			resources.ApplyResources(this.listOfInputParameters, "listOfInputParameters");
			this.listOfInputParameters.Name = "listOfInputParameters";
			this.listOfInputParameters.MouseDoubleClick +=new MouseEventHandler(listOfInputParameters_MouseDoubleClick);
			this.listOfInputParameters.SelectedValueChanged += new EventHandler(listOfInputParameters_SelectedValueChanged);
			this.functionsTypes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.functionsTypes.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.functionsTypes, "functionsTypes");
			this.functionsTypes.Name = "this.functionsTypes";
			this.functionsTypes.SelectedIndexChanged += new EventHandler(functionsTypes_SelectedIndexChanged);
			resources.ApplyResources(this.labelControl4, "labelControl4");
			this.labelControl4.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.labelControl4.LineVisible = true;
			this.labelControl4.Name = "labelControl4";
			resources.ApplyResources(this.layoutItemButton15, "layoutItemButton15");
			this.layoutItemButton15.Name = "layoutItemButton15";
			resources.ApplyResources(this.layoutItemButton12, "layoutItemButton12");
			this.layoutItemButton12.Name = "layoutItemButton12";
			resources.ApplyResources(this.layoutItemButton13, "layoutItemButton13");
			this.layoutItemButton13.Name = "layoutItemButton13";
			resources.ApplyResources(this.layoutItemButton14, "layoutItemButton14");
			this.layoutItemButton14.Name = "layoutItemButton14";
			resources.ApplyResources(this.layoutItemButton11, "layoutItemButton11");
			this.layoutItemButton11.Name = "layoutItemButton11";
			resources.ApplyResources(this.layoutItemButton10, "layoutItemButton10");
			this.layoutItemButton10.Name = "layoutItemButton10";
			resources.ApplyResources(this.layoutItemButton9, "layoutItemButton9");
			this.layoutItemButton9.Name = "layoutItemButton9";
			resources.ApplyResources(this.layoutItemButton8, "layoutItemButton8");
			this.layoutItemButton8.Name = "layoutItemButton8";
			resources.ApplyResources(this.layoutItemButton7, "layoutItemButton7");
			this.layoutItemButton7.Name = "layoutItemButton7";
			resources.ApplyResources(this.layoutItemButton6, "layoutItemButton6");
			this.layoutItemButton6.Name = "layoutItemButton6";
			resources.ApplyResources(this.layoutItemButton5, "layoutItemButton5");
			this.layoutItemButton5.Name = "layoutItemButton5";
			resources.ApplyResources(this.layoutItemButton4, "layoutItemButton4");
			this.layoutItemButton4.Name = "layoutItemButton4";
			resources.ApplyResources(this.layoutItemButton3, "layoutItemButton3");
			this.layoutItemButton3.Name = "layoutItemButton3";
			resources.ApplyResources(this.layoutItemButton2, "layoutItemButton2");
			this.layoutItemButton2.Name = "layoutItemButton2";
			resources.ApplyResources(this.plusItemButton, "plusItemButton");
			this.plusItemButton.Name = "plusItemButton";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			resources.ApplyResources(this, "$this");
			this.Text = resources.GetString(CaptionName);
			this.Controls.Add(this.expressionMemoEdit);
			this.Controls.Add(this.layoutItemButton15);
			this.Controls.Add(this.labelControl4);
			this.Controls.Add(this.listOfInputTypes);
			this.Controls.Add(this.listOfInputParameters);
			this.Controls.Add(this.functionsTypes);
			this.Controls.Add(this.descriptionControl);
			this.Controls.Add(this.labelControl3);
			this.Controls.Add(this.layoutItemButton12);
			this.Controls.Add(this.layoutItemButton13);
			this.Controls.Add(this.layoutItemButton14);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.layoutItemButton11);
			this.Controls.Add(this.layoutItemButton10);
			this.Controls.Add(this.layoutItemButton9);
			this.Controls.Add(this.layoutItemButton8);
			this.Controls.Add(this.layoutItemButton7);
			this.Controls.Add(this.layoutItemButton6);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.layoutItemButton5);
			this.Controls.Add(this.layoutItemButton4);
			this.Controls.Add(this.layoutItemButton3);
			this.Controls.Add(this.layoutItemButton2);
			this.Controls.Add(this.plusItemButton);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Name = "ExpressionEditorForm";
			this.ControlBox = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.LookAndFeel.StyleChanged += new EventHandler(LookAndFeel_StyleChanged);
			((System.ComponentModel.ISupportInitialize)(this.expressionMemoEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.listOfInputTypes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.listOfInputParameters)).EndInit();
			this.ResumeLayout(false);
		}
		void InitializeButtons() {
			buttonStrings = new Hashtable();
			buttonStrings[plusItemButton] = StandardOperations.Plus;
			buttonStrings[layoutItemButton2] = StandardOperations.Minus;
			buttonStrings[layoutItemButton3] = StandardOperations.Multiply;
			buttonStrings[layoutItemButton4] = StandardOperations.Divide;
			buttonStrings[layoutItemButton5] = StandardOperations.Modulo;
			buttonStrings[layoutItemButton6] = StandardOperations.Equal;
			buttonStrings[layoutItemButton7] = StandardOperations.NotEqual;
			buttonStrings[layoutItemButton8] = StandardOperations.Less;
			buttonStrings[layoutItemButton9] = StandardOperations.LessOrEqual;
			buttonStrings[layoutItemButton10] = StandardOperations.LargerOrEqual;
			buttonStrings[layoutItemButton11] = StandardOperations.Larger;
			buttonStrings[layoutItemButton12] = StandardOperations.And;
			buttonStrings[layoutItemButton13] = StandardOperations.Or;
			buttonStrings[layoutItemButton14] = StandardOperations.Not;
			foreach(Control control in this.Controls)
				if(control is LayoutItemButton && control.Name != "layoutItemButton15")
					control.Click += new EventHandler(control_Click);
			this.layoutItemButton15.Click += new System.EventHandler(this.layoutItemButton15_Click);
		}
		void buttonOK_Click(object sender, EventArgs e) {
			if(fEditorLogic.CanCloseWithOKResult()) {
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}
		void listOfInputTypes_SelectedValueChanged(object sender, EventArgs e) {
			fEditorLogic.OnInputTypeChanged();
		}
		void functionsTypes_SelectedIndexChanged(object sender, EventArgs e) {
			fEditorLogic.OnFunctionTypeChanged();
		}
		void listOfInputParameters_SelectedValueChanged(object sender, EventArgs e) {
			fEditorLogic.OnInputParametersChanged();
		}
		void listOfInputParameters_MouseDoubleClick(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Right)
				return;
			fEditorLogic.OnInsertInputParameter();
		}
		void control_Click(object sender, EventArgs e) {
			fEditorLogic.OnInsertOperation((string)buttonStrings[sender]);
		}
		void layoutItemButton15_Click(object sender, EventArgs e) {
			fEditorLogic.OnWrapExpression();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			fEditorLogic.OnLoad();
		}
		protected virtual bool CanUseDefaultControlDesignersSkin() {
			return RegistryDesignerSkinHelper.CanUseDefaultControlDesignersSkin;
		}
		protected virtual void SetParentLookAndFeel() {
			if(ServiceProvider != null) {
				ILookAndFeelService serv = null;
				if(ServiceProvider != null) {
					serv = ServiceProvider.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
					if(serv != null && !CanUseDefaultControlDesignersSkin())
						serv.InitializeRootLookAndFeel(this.LookAndFeel);
					else {
						LookAndFeel.SetSkinStyle(DevExpress.Skins.SkinRegistrator.DesignTimeSkinName);
					}
				}
			}
			else {
				if(LookAndFeelInfo != null && LookAndFeelInfo.LookAndFeel != null) LookAndFeel.ParentLookAndFeel = LookAndFeelInfo.LookAndFeel;
			}
			UpdateIcons();
			AssingButtonImages();
		}
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			this.listOfInputTypes.HotTrackItems = LookAndFeelIsSkin();
		}
		bool LookAndFeelIsSkin() {
			return this.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin;
		}
		protected void HideParameters() {
			if(listOfInputParameters.Visible)
				listOfInputParameters.Visible = false;
		}
		protected void ShowParameters() {
			if(!listOfInputParameters.Visible)
				listOfInputParameters.Visible = true;
		}
		protected Rectangle GetParametersEditorBounds() {
			return listOfInputParameters.Bounds;
		}
		protected string GetResourceString(string name) {
			return resources.GetString(name);
		}
		protected void SetDescription(string description) {
			this.descriptionControl.Text = description;
		}
		#region IExpressionEditor Members
		string IExpressionEditor.GetResourceString(string stringId) {
			string result = GetResourceString(stringId);
			return result;
		}
		void IExpressionEditor.ShowFunctionsTypes() {
			if(functionsTypes.Visible)
				return;
			listOfInputParameters.Location = new Point(listOfInputParameters.Location.X, listOfInputParameters.Location.Y + 25);
			listOfInputParameters.Size = new Size(listOfInputParameters.Size.Width, listOfInputParameters.Size.Height - 25);
			functionsTypes.Visible = true;
		}
		void IExpressionEditor.HideFunctionsTypes() {
			if(!functionsTypes.Visible)
				return;
			listOfInputParameters.Location = new Point(listOfInputParameters.Location.X, listOfInputParameters.Location.Y - 25);
			listOfInputParameters.Size = new Size(listOfInputParameters.Size.Width, listOfInputParameters.Size.Height + 25);
			functionsTypes.Visible = false;
		}
		ExpressionEditorLogic IExpressionEditor.EditorLogic { get { return fEditorLogic; } }
		IMemoEdit IExpressionEditor.ExpressionMemoEdit { get { return ExpressionMemoEdit; } }
		ISelector IExpressionEditor.ListOfInputTypes { get { return ListOfInputTypes; } }
		ISelector IExpressionEditor.ListOfInputParameters { get { return ListOfInputParameters; } }
		ISelector IExpressionEditor.FunctionsTypes { get { return FunctionsTypes; } }
		string IExpressionEditor.FilterCriteriaInvalidExpressionMessage { get { return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaInvalidExpression); } }
		string IExpressionEditor.FilterCriteriaInvalidExpressionExMessage { get { return Localizer.Active.GetLocalizedString(StringId.FilterCriteriaInvalidExpressionEx); } }
		void IExpressionEditor.ShowError(string error) {
			XtraMessageBox.Show(LookAndFeel, this, error, Localizer.Active.GetLocalizedString(StringId.CaptionError), MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		string IExpressionEditor.GetFunctionTypeStringID(string functionType) {
			return "functionsTypes.Properties." + functionType;
		}
		void IExpressionEditor.SetDescription(string description) {
			SetDescription(description);
		}
		#endregion
	}
	public class ExpressionEditorBase : UITypeEditor {
		protected virtual ExpressionEditorForm CreateForm(object instance, IDesignerHost designerHost, object value) {
			if(instance is StyleFormatConditionBase) return new ConditionExpressionEditorForm(instance, designerHost);
			if(instance is FormatConditionRuleBase) return new FormatRuleExpressionEditorForm(instance, designerHost, value);
			return new UnboundColumnExpressionEditorForm(instance, designerHost);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null || provider == null) return null;
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(edSvc != null) {
				IDesignerHost designerHost = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				using(ExpressionEditorForm form = CreateForm(DXObjectWrapper.GetInstance(context), designerHost, value)) {
					if(edSvc.ShowDialog(form) == DialogResult.OK) {
						return form.Expression;
					}
				}
			}
			return base.EditValue(context, provider, value);
		}
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return System.Drawing.Design.UITypeEditorEditStyle.Modal;
		}
	}
	public class ExpressionEditorFormEx : ExpressionEditorForm {
		public ExpressionEditorFormEx(object contextInstance, IDesignerHost designerHost)
			: base(contextInstance, designerHost) {
			ControlBox = true;
			MaximizeBox = false;
			MinimizeBox = false;
			this.Icon = ResourceImageHelper.CreateIconFromResources(
						"DevExpress.XtraEditors.Images.expression.ico", typeof(ExpressionEditorFormEx).Assembly);
		}
		protected override ExpressionEditorLogic CreateExpressionEditorLogic() {
			return new ExpressionEditorLogicEx(this, ContextInstance as IDataColumnInfo);
		}
	}
	public class ConditionExpressionEditorForm : ExpressionEditorFormEx {
		public ConditionExpressionEditorForm(object contextInstance, IDesignerHost designerHost)
			: base(contextInstance, designerHost) {
		}
		protected override string CaptionName { get { return "Condition.Caption"; } }
	}
	public class FormatRuleExpressionEditorForm : ExpressionEditorFormEx {
		object value;
		bool allowInitialize = false;
		public FormatRuleExpressionEditorForm(object contextInstance, IDesignerHost designerHost, object value)
			: base(contextInstance, designerHost) {
				this.value = value;
				this.allowInitialize = true;
				InitializeEditorLogic();
		}
		protected override void InitializeEditorLogic() {
			if(!allowInitialize) return;
			base.InitializeEditorLogic();
		}
		protected override string CaptionName { get { return "FormatRule.Caption"; } }
		protected override ExpressionEditorLogic CreateExpressionEditorLogic() {
			FormatConditionRuleBase format = ContextInstance as FormatConditionRuleBase;
			FormatRuleBase rule = null;
			if(format != null) rule = format.Owner as FormatRuleBase;
			return new RuleExpressionEditorLogicEx(this, rule == null ? null : rule.GetColumnInfo(), value == null ? "" : value.ToString());
		}
		class RuleExpressionEditorLogicEx : ExpressionEditorLogicEx {
			string expression;
			internal RuleExpressionEditorLogicEx(IExpressionEditor editor, IDataColumnInfo columnInfo, string expression) : base(editor, columnInfo) {
				this.expression = expression;
			}
			protected override string GetExpressionMemoEditText() { return expression; }
		}
	}
	public class UnboundColumnExpressionEditorForm : ExpressionEditorFormEx {
		public UnboundColumnExpressionEditorForm(object contextInstance, IDesignerHost designerHost)
			: base(contextInstance, designerHost) {
		}
		protected override string CaptionName { get { return "UnboundColumn.Caption"; } }
	}
	public class MemoEditWrapper : IMemoEdit {
		readonly MemoEdit edit;
		public MemoEditWrapper(MemoEdit edit) {
			this.edit = edit;
		}
		#region IMemoEdit Members
		int IMemoEdit.SelectionStart { get { return edit.SelectionStart; } set { edit.SelectionStart = value; } }
		int IMemoEdit.SelectionLength { get { return edit.SelectionLength; } set { edit.SelectionLength = value; } }
		string IMemoEdit.Text { get { return edit.Text; } set { edit.Text = value; } }
		string IMemoEdit.SelectedText { get { return edit.SelectedText; } }
		int IMemoEdit.GetLineLength(int lineIndex) {
			return edit.Lines[lineIndex].Length;
		}
		void IMemoEdit.Paste(string text) {
			edit.MaskBox.Paste(text);
		}
		void IMemoEdit.Focus() {
			edit.Focus();
		}
		#endregion
	}
	public class ListBoxControlWrappper : ISelector {
		readonly ListBoxControl listBox;
		public ListBoxControlWrappper(ListBoxControl listBox) {
			this.listBox = listBox;
		}
		#region ISelector Members
		void ISelector.SetItemsSource(object[] items, ColumnSortOrder sortOrder) {
			listBox.Items.Clear();
			listBox.SortOrder = (SortOrder)sortOrder;
			listBox.Items.AddRange(items);
		}
		int ISelector.ItemCount { get { return listBox.Items.Count; } }
		object ISelector.SelectedItem { get { return listBox.SelectedItem; } }
		int ISelector.SelectedIndex { get { return listBox.SelectedIndex; } set { listBox.SelectedIndex = value; } }
		#endregion
	}
	public class ComboBoxEditWrappper : ISelector {
		readonly ComboBoxEdit comboBox;
		public ComboBoxEditWrappper(ComboBoxEdit listBox) {
			this.comboBox = listBox;
		}
		#region ISelector Members
		void ISelector.SetItemsSource(object[] items, ColumnSortOrder sortOrder) {
			comboBox.Properties.Items.Clear();
			comboBox.Properties.Items.AddRange(items);
		}
		int ISelector.ItemCount { get { return comboBox.Properties.Items.Count; } }
		object ISelector.SelectedItem { get { return comboBox.SelectedItem; } }
		int ISelector.SelectedIndex { get { return comboBox.SelectedIndex; } set { comboBox.SelectedIndex = value; } }
		#endregion
	}
}
