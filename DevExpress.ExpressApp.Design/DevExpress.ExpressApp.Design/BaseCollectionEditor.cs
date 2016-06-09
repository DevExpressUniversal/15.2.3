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
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Collections;
using System.Windows.Forms;
using DevExpress.Persistent.Base;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Design {
	public abstract class BaseCollectionEditor : UITypeEditor {
		public BaseCollectionEditor() : base() { }
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		protected Type GetListType(Type type) {
			Type[] interfaceTypes = type.GetInterfaces();
			foreach(Type interfaceType in interfaceTypes) {
				if(interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
					return interfaceType.GetGenericArguments()[0];
				}
			}
			return null;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			Type listType = GetListType(context.PropertyDescriptor.PropertyType);
			ICollectionEditorForm form = CreateEditorForm(listType);
			if(string.IsNullOrEmpty(form.Text)) {
				form.Text = listType.Name + " Collection Editor";
			}
			form.DataSource = GetDataSource(context);
			form.EditValue = (ICollection)context.PropertyDescriptor.GetValue(context.Instance);
			if(((Form)form).ShowDialog() == DialogResult.OK) {
				return GetEditValue(form.EditValue, context.PropertyDescriptor.PropertyType);
			}
			return base.EditValue(context, provider, value);
		}
		protected abstract IList GetDataSource(ITypeDescriptorContext context);
		private IList GetEditValue(IEnumerable iCollection, Type propertyType) {
			IList result = (IList)ReflectionHelper.CreateObject(propertyType);
			foreach(object item in iCollection) {
				result.Add(item);
			}
			return result;
		}
		protected abstract ICollectionEditorForm CreateEditorForm(Type listType);
	}
	public interface ICollectionEditorForm {
		string Text { get; set; }
		IList DataSource { get; set; }
		ICollection EditValue { get; set; }
	}
}
