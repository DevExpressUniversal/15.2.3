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
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using DevExpress.Accessibility;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Calendar;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ListControls;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemLookUpEditBase : RepositoryItemPopupBaseAutoSearchEdit {
		bool disposeDataSource = false;
		static readonly object processNewValue = new object();
		BestFitMode bestFitMode;
		string valueMember, displayMember;
		object dataSource;
		bool showFooter;
		public RepositoryItemLookUpEditBase() {
			this.bestFitMode = BestFitMode.None;
			this.showFooter = true;
			base.fTextEditStyle = TextEditStyles.DisableTextEditor;
			base.fNullText = Localizer.Active.GetLocalizedString(StringId.LookUpEditValueIsNull);
			this.valueMember = this.displayMember = string.Empty;
			this.ResetTextEditStyleToStandardInFilterControl = false;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				CheckDestroyDataSource();
			}
			base.Dispose(disposing);
		}
		void CheckDestroyDataSource() {
			try {
				if(disposeDataSource) {
					if(dataSource != null) {
						IDisposable ds = dataSource as IDisposable;
						if(ds != null) ds.Dispose();
					}
				}
				this.disposeDataSource = false;
				this.dataSource = null;
			}
			catch { }
		}
		protected override bool ShouldSerializeNullText() { return (NullText != Localizer.Active.GetLocalizedString(StringId.LookUpEditValueIsNull)); }
		protected override bool DefaultPopupSizeable { get { return true; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditBasePopupSizeable"),
#endif
 DefaultValue(true)]
		public override bool PopupSizeable {
			get { return base.PopupSizeable; }
			set { base.PopupSizeable = value; }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditBaseTextEditStyle"),
#endif
 DefaultValue(TextEditStyles.DisableTextEditor)]
		public override TextEditStyles TextEditStyle { get { return base.TextEditStyle; } set { base.TextEditStyle = value; } }
		[Browsable(false)]
		public override HorzAlignment DefaultAlignment { get { return HorzAlignment.Near; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditBaseShowFooter"),
#endif
 DefaultValue(true)]
		public virtual bool ShowFooter {
			get { return showFooter; }
			set {
				if(ShowFooter == value) return;
				showFooter = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior),  DefaultValue(BestFitMode.None)]
		public BestFitMode BestFitMode {
			get { return bestFitMode; }
			set {
				if(BestFitMode == value) return;
				bestFitMode = value;
				OnBestFitModeChanged();
			}
		}
		protected virtual void OnBestFitModeChanged() {
			OnPropertiesChanged();
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditBaseDataSource"),
#endif
 DefaultValue(null),
#if DXWhidbey
		AttributeProvider(typeof(IListSource))]
#else
		TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
#endif
		public object DataSource {
			get { return dataSource; }
			set { 
				bool allowMembersRefresh = DataSource != null;
				if(DataSource == value) return;
				CheckDestroyDataSource();
				dataSource = value;
				OnDataSourceChanged();
				if(allowMembersRefresh) RefreshMembers();
			}
		}
		protected void SetDataSource(object dataSource, bool disposeDataSource) {
			DataSource = dataSource;
			this.disposeDataSource = disposeDataSource;
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditBaseValueMember"),
#endif
 DefaultValue(""),
#if DXWhidbey
		TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
#else
		TypeConverter("DevExpress.XtraEditors.Design.DataMemberTypeConverter, " + AssemblyInfo.SRAssemblyEditorsDesign)]
#endif
		public virtual string ValueMember {
			get { return valueMember; }
			set {
				if(value == null) value = string.Empty;
				if(ValueMember == value) return;
				string oldValue = ValueMember;
				valueMember = value;
				OnValueMemberChanged(oldValue, ValueMember);
			}
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditBaseDisplayMember"),
#endif
 DefaultValue(""),
#if DXWhidbey
		TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
#else
		TypeConverter("DevExpress.XtraEditors.Design.DataMemberTypeConverter, " + AssemblyInfo.SRAssemblyEditorsDesign)]
#endif
		public virtual string DisplayMember {
			get { return displayMember; }
			set {
				if(value == null) value = string.Empty;
				if(DisplayMember == value) return;
				displayMember = value;
				OnDataSourceChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditBaseProcessNewValue"),
#endif
 DXCategory(CategoryName.Events)]
		public event ProcessNewValueEventHandler ProcessNewValue {
			add { this.Events.AddHandler(processNewValue, value); }
			remove { this.Events.RemoveHandler(processNewValue, value); }
		}
		void RefreshMembers() {
			if(DataSource == null && IsDesignMode) {
				DisplayMember = string.Empty;
				ValueMember = string.Empty;
			}
		}
		protected virtual void OnDataSourceChanged() {
			this.lastProcessedNewValue = null;
			OnPropertiesChanged();
		}
		protected virtual bool AllowAssignDataSource { get { return true; } }
		protected virtual void OnValueMemberChanged(string oldValue, string newValue) { }
		public override void Assign(RepositoryItem item) {
			RepositoryItemLookUpEditBase source = item as RepositoryItemLookUpEditBase;
			BeginUpdate(); 
			try {
				base.Assign(item);
				if(source == null) return;
				this.bestFitMode = source.BestFitMode;
				this.showFooter = source.ShowFooter;
				if(AllowAssignDataSource) this.dataSource = source.DataSource;
				this.displayMember = source.DisplayMember;
				this.valueMember = source.ValueMember;
				Events.AddHandler(processNewValue, source.Events[processNewValue]);
			}
			finally {
				EndUpdate();
			}
		}
		public override void ResetEvents() {
			base.ResetEvents();
			this.lastProcessedNewValue = null;
		}
		internal object lastProcessedNewValue = null;
		int inProcessNewValue = 0;
		protected internal bool InProcessNewValue { get { return inProcessNewValue != 0; } }
		protected internal override bool AllowClosePopup { get { return base.AllowClosePopup && !InProcessNewValue; } }
		protected internal virtual void RaiseProcessNewValue(ProcessNewValueEventArgs e) {
			this.inProcessNewValue++;
			try {
				ProcessNewValueCore(e);
			}
			finally {
				this.inProcessNewValue--;
			}
		}
		void ProcessNewValueCore(ProcessNewValueEventArgs e) {
			try {
				if((lastProcessedNewValue != null && object.Equals(lastProcessedNewValue, e.DisplayValue)) || DisplayValueIsNullText(e.DisplayValue)) {
					return;
				}
			}
			catch { }
			this.lastProcessedNewValue = e.DisplayValue;
			ProcessNewValueEventHandler handler = (ProcessNewValueEventHandler)Events[processNewValue];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected virtual bool DisplayValueIsNullText(object displayValue) {
			return (!string.IsNullOrEmpty(NullText) && object.Equals(displayValue, NullText)) ||
				(!string.IsNullOrEmpty(NullValuePrompt) && object.Equals(displayValue, NullValuePrompt));
		}
		protected bool IsProcessNewValueExists { get { return (Events[processNewValue] != null); } }
		public override bool IsFilterLookUp { get { return true; } }
		protected class LookupEditCustomBindingPropertiesAttribute : DevExpress.Utils.Design.DataAccess.CustomBindingPropertiesAttribute {
			protected string ControlName;
			public LookupEditCustomBindingPropertiesAttribute(string controlName) {
				this.ControlName = controlName;
			}
			public override System.Collections.Generic.IEnumerable<DevExpress.Utils.Design.DataAccess.ICustomBindingProperty> GetCustomBindingProperties() {
				return new DevExpress.Utils.Design.DataAccess.ICustomBindingProperty[] {
						new CustomBindingPropertyAttribute("DisplayMember", "Display Member", GetDisplayMemberDescription()),
						new CustomBindingPropertyAttribute("ValueMember", "Value Member", GetValueMemberDescription())
					};
			}
			protected virtual string GetValueMemberDescription() {
				return string.Format("Gets or sets the field whose values are displayed in the edit box of the {0}.", ControlName);
			}
			protected virtual string GetDisplayMemberDescription() {
				return string.Format("Gets or sets the field name whose values identify dropdown rows of the {0}.", ControlName);
			}
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class LookUpEditBaseViewInfo : PopupBaseAutoSearchEditViewInfo {
		public LookUpEditBaseViewInfo(RepositoryItem item) : base(item) { }
		public new LookUpEditBase OwnerEdit { get { return base.OwnerEdit as LookUpEditBase; } }
		public new RepositoryItemLookUpEditBase Item { get { return base.Item as RepositoryItemLookUpEditBase; } }
	}
}
namespace DevExpress.XtraEditors {
	public abstract class LookUpEditBase : PopupBaseAutoSearchEdit {
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpEditBaseProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemLookUpEditBase Properties { get { return base.Properties as RepositoryItemLookUpEditBase; } }
		protected abstract bool CheckInputNewValue(bool partial);
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpEditBaseProcessNewValue"),
#endif
 DXCategory(CategoryName.Events)]
		public event ProcessNewValueEventHandler ProcessNewValue {
			add { this.Properties.ProcessNewValue += value; }
			remove { this.Properties.ProcessNewValue -= value; }
		}
		protected override void OnEnter(EventArgs e) {
			base.OnEnter(e);
			Properties.lastProcessedNewValue = null;
		}
		public abstract object GetSelectedDataRow();
		public sealed override void Refresh() {
			Refresh(true);
		}
		protected internal virtual void Refresh(bool resetCache) {
			ViewInfo.RefreshDisplayText = true;
			base.Refresh();
		}
		protected internal override void RaisePopupAllowClick(PopupAllowClickEventArgs e) {
			if(Properties.InProcessNewValue) {
				e.Allow = true;
				return;
			}
			base.RaisePopupAllowClick(e);
		}
	}
}
