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
using DevExpress.Data.Filtering;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Design {
	#region FilterEditorForm
	public partial class FilterEditorForm : XtraForm {
		string value;
		IFilteredComponent filteredComponent;
		public FilterEditorForm(IFilteredComponent filteredComponent) {
			if (filteredComponent == null)
				Exceptions.ThrowArgumentNullException("filteredControl");
			InitializeComponent();
			this.filteredComponent = filteredComponent;
			filterControl.SourceControl = FilteredComponent;
			filterControl.FilterCriteria = FilteredComponent.RowCriteria;
		}
		public string Value { get { return value; } }
		public IFilteredComponent FilteredComponent { get { return filteredComponent; } }
		internal void SetValue(object val) {
			this.value = (String)val;
			FilteredComponent.RowCriteria = CriteriaOperator.Parse(value);
		}
		internal void End() {
		}
		private void OnBtnOkClick(object sender, EventArgs e) {
			this.value = CriteriaOperator.ToString(filterControl.FilterCriteria);
		}
	}
	#endregion
	#region FilterEditor<T>
	public abstract class FilterEditor<T> : UITypeEditor where T : IPersistentObject {
		FilterEditorForm filterEditorForm;
		protected FilterEditor()
			: base() {
		}
		public FilterEditorForm FilterEditorForm { get { return filterEditorForm; } }
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val) {
			if (context != null
				&& context.Instance != null
				&& provider != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					if (FilterEditorForm != null)
						FilterEditorForm.Dispose();
					this.filterEditorForm = CreateEditorForm(context);
					filterEditorForm.SetValue(val);
					edSvc.ShowDialog(filterEditorForm);
					val = filterEditorForm.Value;
				}
			}
			return val;
		}
		protected internal virtual FilterEditorForm CreateEditorForm(ITypeDescriptorContext context) {
			PersistentObjectStorage<T> objectStorage = (PersistentObjectStorage<T>)context.Instance;
			return new FilterEditorForm(objectStorage as IFilteredComponent);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
	#endregion
	#region AppointmentFilterEditor
	public class AppointmentFilterEditor : FilterEditor<Appointment> {
	}
	#endregion
	#region ResourceFilterEditor
	public class ResourceFilterEditor : FilterEditor<Resource> {
	}
	#endregion
}
