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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.LookAndFeel;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraPivotGrid.Forms;
namespace DevExpress.XtraPivotGrid.Forms {
	public partial class PrefilterForm : XtraForm {
		public static readonly Point DefaultLocation = new Point(-10000, -10000);
		readonly IPrefilterFormOwner formOwner;
		protected IPrefilterFormOwner FormOwner { get { return formOwner; } }
		protected Control ControlOwner { get { return FormOwner.ControlOwner; } }
		protected CriteriaOperator Criteria {
			get { return filterControl.FilterCriteria; }
		}
		protected IFilteredComponent SourceControl {
			get { return filterControl.SourceControl as IFilteredComponent; }
			set { filterControl.SourceControl = value; }
		}
		PrefilterForm() {
			InitializeComponent();
		}
		public PrefilterForm(IPrefilterFormOwner owner) {
			if (owner == null) throw new Exception("PrefilterFormOwner cannot be null.");
			this.formOwner = owner;
			InitializeComponent();
			PivotGridControl pivot;
			if (owner != null && (pivot = owner.ControlOwner as PivotGridControl) != null)
				this.filterControl.MenuManager = pivot.MenuManager;
			Visible = false;
			RegisterAsOwnedForm(this.ControlOwner);
		}
		public DialogResult ShowPrefilterForm(Point location, bool showApplyButton) {
			filterControl.SourceControl = FormOwner.FilteredComponent;
			filterControl.FilterCriteria = FormOwner.FilteredComponent.RowCriteria;
			filterControl.ShowOperandTypeIcon = FormOwner.ShowOperandTypeIcon;
			SetLocation(location, this.ControlOwner);
			ShowApplyButton(showApplyButton);
			CheckApplyButtonEnabled();
			return ShowDialog();
		}
		void ShowApplyButton(bool showApplyButton) {
			btnApply.Visible = showApplyButton;
			btnCancel.Left = showApplyButton ? btnApply.Left - btnCancel.Width - 6 : btnApply.Left;
			btnOK.Left = btnCancel.Left - btnOK.Width - 6;
		}
		void SetLocation(Point location, Control controlOwner) {
			if (!location.IsEmpty) {
				if (location == DefaultLocation) {
					if (controlOwner != null && controlOwner.FindForm() != null) {
						Form parentForm = controlOwner.FindForm();
						location = parentForm.PointToScreen(new Point(parentForm.ClientRectangle.Left, parentForm.ClientRectangle.Top));
						location.Offset((parentForm.ClientSize.Width - this.Width) / 2, (parentForm.ClientSize.Height - this.Height) / 2);
					}
					else
						location = new Point((Screen.PrimaryScreen.Bounds.Width - this.Width) / 2,
							(Screen.PrimaryScreen.Bounds.Height - this.Height) / 2);
				}
				Location = location;
			}
		}
		void RegisterAsOwnedForm(Control controlOwner) {
			if (controlOwner != null && controlOwner.FindForm() != null) {
				if (controlOwner.FindForm().MdiParent != null)
					controlOwner.FindForm().MdiParent.AddOwnedForm(this);
				else
					controlOwner.FindForm().AddOwnedForm(this);
			}
		}
		void ApplyCriteria() {
			SourceControl.RowCriteria = Criteria;
			CheckApplyButtonEnabled();
		}
		void CheckApplyButtonEnabled() {
			btnApply.Enabled = !object.Equals(Criteria, SourceControl.RowCriteria);
		}
		void btnApply_Click(object sender, EventArgs e) {
			ApplyCriteria();
		}
		void btnOK_Click(object sender, EventArgs e) {
			ApplyCriteria();
			DialogResult = DialogResult.OK;
			Close();
		}
		void btnCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
		void filterControl_FilterChanged(object sender, FilterChangedEventArgs e) {
			CheckApplyButtonEnabled();
		}
		void PrefilterForm_KeyDown(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Escape:
					Close();
					break;
			}
		}
	}
}
namespace DevExpress.XtraPivotGrid {
	public interface IPrefilterFormOwner {
		Control ControlOwner { get; }
		IFilteredComponent FilteredComponent { get; }
		bool ShowOperandTypeIcon { get; }
		void SetPrefilterVisible(bool visible);
	}
	public interface IPrefilterOwner : IPrefilterOwnerBase, IPrefilterFormOwner {
		UserLookAndFeel ActiveLookAndFeel { get; }
	}
	public class Prefilter : PrefilterBase, IPrefilterFormOwner, IXtraSerializable {
		PrefilterFormHelper prefilterFormHelper = new PrefilterFormHelper();
		protected internal new IPrefilterOwner Owner { get { return (IPrefilterOwner)base.Owner; } }
		bool showOperandTypeIcon = false;
		public Prefilter(IPrefilterOwner owner)
			: base(owner) {
		}
		public override void Dispose() {
			prefilterFormHelper.DisposeForm();
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PrefilterCriteria"),
#endif
		Editor(typeof(CriteriaEditor), typeof(UITypeEditor)), 
		]
		public override CriteriaOperator Criteria {
			get { return base.Criteria; }
			set { base.Criteria = value; }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PrefilterShowOperandTypeIcon"),
#endif
		DefaultValue(false), XtraSerializableProperty(),
		NotifyParentProperty(true)
		]
		public bool ShowOperandTypeIcon {
			get { return showOperandTypeIcon; }
			set { showOperandTypeIcon = value; }
		}
		public void ChangePrefilterVisible() {
			if(IsPrefilterFormShowing) 
				HideForm();
			else 
				ShowForm();
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PrefilterIsPrefilterFormShowing"),
#endif
		Browsable(false)]
		public bool IsPrefilterFormShowing { get { return prefilterFormHelper.IsPrefilterFormShowing; } }
		public void ShowForm() {
			if(!IsPrefilterFormShowing) 
				prefilterFormHelper.ShowPrefilterForm(this, Owner.ActiveLookAndFeel, true);
		}
		public void HideForm() {
			if(IsPrefilterFormShowing)
				prefilterFormHelper.DestroyPrefilterForm();
		}
		void IPrefilterFormOwner.SetPrefilterVisible(bool visible) {
			Owner.SetPrefilterVisible(visible);
		}
		bool IPrefilterFormOwner.ShowOperandTypeIcon {
			get { return Owner.ShowOperandTypeIcon; }
		}
		#region IPrefilterFormOwner Members
		Control IPrefilterFormOwner.ControlOwner {
			get { return Owner.ControlOwner; }
		}
		IFilteredComponent IPrefilterFormOwner.FilteredComponent {
			get { return Owner.FilteredComponent; }
		}
		#endregion
		public override string ToString() {
			return object.ReferenceEquals(Criteria, null) ? string.Empty : Criteria.ToString();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset() {
			prefilterFormHelper.DestroyPrefilterForm();
			Enabled = true;
			ShowOperandTypeIcon = false;
			Clear();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerialize() {
			return Enabled != true || !object.ReferenceEquals(Criteria, null) || ShowOperandTypeIcon;
		}
		#region IXtraSerializable Members
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			var visitor = new ColumnNamesCriteriaVisitor(true);
			if(!object.ReferenceEquals(null, Criteria))
				Criteria.Accept(visitor);
		}
		void IXtraSerializable.OnEndSerializing() { }
		void IXtraSerializable.OnStartDeserializing(DevExpress.Utils.LayoutAllowEventArgs e) { }
		void IXtraSerializable.OnStartSerializing() { }
		#endregion
	}
	class PrefilterFormHelper {
		PrefilterForm fPrefilterForm;
		Rectangle fPrefilterFormBounds = Rectangle.Empty;
		public bool IsPrefilterFormShowing { get { return fPrefilterForm != null; } }
		public DialogResult ShowPrefilterForm(IPrefilterFormOwner owner, UserLookAndFeel lookAndFeel, bool showApplyButton) {
			owner.SetPrefilterVisible(true);
			fPrefilterForm = new PrefilterForm(owner);
			fPrefilterForm.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			if(!fPrefilterFormBounds.IsEmpty) fPrefilterForm.Bounds = fPrefilterFormBounds;
			if(owner.ControlOwner != null)
				fPrefilterForm.RightToLeft = owner.ControlOwner.RightToLeft;
			DialogResult result = fPrefilterForm.ShowPrefilterForm(PrefilterForm.DefaultLocation, showApplyButton);
			DestroyPrefilterForm();
			owner.SetPrefilterVisible(false);
			return result;
		}
		public void DestroyPrefilterForm() {
			if(IsPrefilterFormShowing) {
				fPrefilterFormBounds = fPrefilterForm.Bounds;
				DisposeForm();
			}
		}
		public void DisposeForm() {
			fPrefilterForm.Dispose();
			fPrefilterForm = null;
		}
	}
	public abstract class CriteriaEditorBase : UITypeEditor {
		PrefilterFormHelper helper = new PrefilterFormHelper();
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		protected abstract UserLookAndFeel GetLookAndFeel(PrefilterBase prefilter, IServiceProvider provider);
		protected abstract IPrefilterFormOwner GetPrefilterFormOwner(PrefilterBase prefilter);
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context.Instance is PrefilterBase) {
				PrefilterBase prefilter = (PrefilterBase)context.Instance;
				if(helper.ShowPrefilterForm(GetPrefilterFormOwner(prefilter), GetLookAndFeel(prefilter, provider), false) == DialogResult.OK) {
					PropertyDescriptor criteriaDescriptor = TypeDescriptor.GetProperties(prefilter)["Criteria"];
					IComponentChangeService changeService = (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));
					changeService.OnComponentChanging(prefilter, criteriaDescriptor);
					changeService.OnComponentChanged(prefilter, criteriaDescriptor, value, prefilter.Criteria);
					return prefilter.Criteria;
				} else
					return value;
			}
			return base.EditValue(context, provider, value);
		}
	}
	public class CriteriaEditor : CriteriaEditorBase {
		protected override UserLookAndFeel GetLookAndFeel(PrefilterBase prefilter, IServiceProvider provider) {
			return ((Prefilter)prefilter).Owner.ActiveLookAndFeel;
		}
		protected override IPrefilterFormOwner GetPrefilterFormOwner(PrefilterBase prefilter) {
			return (Prefilter)prefilter;
		}
	}
}
