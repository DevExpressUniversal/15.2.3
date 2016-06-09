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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
namespace DevExpress.XtraReports.Design.LightSwitch {
	public class QueryParameterCollectionEditor : CollectionEditor {
		DesignerTransaction transaction;
		ITypeDescriptorContext currentContext;
		bool skipChangingEvents;
		bool skipChangedEvents;
		public QueryParameterCollectionEditor(Type type)
			: base(type) { }
		protected override void CancelChanges() {
			base.CancelChanges();
			if (transaction != null) {
				transaction.Cancel();
				transaction = null;
			}
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (provider != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc == null) {
					return value;
				}
				this.currentContext = context;
				CollectionForm form = this.CreateCollectionForm();
				ITypeDescriptorContext tempCurrentContext = this.currentContext;
				form.EditValue = value;
				IComponentChangeService componentChangeService = null;
				IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
				this.skipChangingEvents = false;
				this.skipChangedEvents = false;
				try {
					try {
						if (designerHost != null)
							transaction = designerHost.CreateTransaction(this.CollectionItemType.Name + "s changes");
					}
					catch (CheckoutException exception) {
						if (exception != CheckoutException.Canceled) {
							throw exception;
						}
						return value;
					}
					componentChangeService = (designerHost != null) ? ((IComponentChangeService)designerHost.GetService(typeof(IComponentChangeService))) : null;
					if (componentChangeService != null) {
						componentChangeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
						componentChangeService.ComponentChanging += new ComponentChangingEventHandler(OnComponentChanging);
					}
					DialogResult res = edSvc.ShowDialog(form);
					if (res == DialogResult.OK) {
						value = form.EditValue;
						return value;
					}
				}
				finally {
					form.EditValue = null;
					this.currentContext = tempCurrentContext;
					if (transaction != null)
						transaction.Commit();
					if (componentChangeService != null) {
						componentChangeService.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
						componentChangeService.ComponentChanging -= new ComponentChangingEventHandler(OnComponentChanging);
					}
					form.Dispose();
				}
			}
			return value;
		}
		void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
			if (!this.skipChangedEvents && sender != this.currentContext.Instance) {
				this.skipChangedEvents = true;
				this.currentContext.OnComponentChanged();
			}
		}
		void OnComponentChanging(object sender, ComponentChangingEventArgs e) {
			if (!this.skipChangingEvents && sender != this.currentContext.Instance) {
				this.skipChangingEvents = true;
				this.currentContext.OnComponentChanging();
			}
		}
	}
}
