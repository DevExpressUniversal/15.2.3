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
using System.Drawing.Design;
using System.ComponentModel.Design;
namespace DevExpress.Utils.UI {
	public static class EditorHelper {
		public static object EditValue(object objectToChange, string propName) {
			return EditValue(objectToChange, propName, null);
		}
		[System.Security.SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		public static object EditValue(object objectToChange, PropertyDescriptor propertyDescriptor, IServiceProvider parentServiceProvider) {
			ITypeDescriptorContext context = new TypeDescriptorContext(objectToChange, propertyDescriptor, parentServiceProvider);
			UITypeEditor editor = propertyDescriptor.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
			if (editor == null)
				return null;
			object obj1 = propertyDescriptor.GetValue(objectToChange);
			object obj2 = editor.EditValue(context, context, obj1);
			if (obj2 != obj1) {
				try {
					propertyDescriptor.SetValue(objectToChange, obj2);
				} catch (CheckoutException) { }
			}
			return obj2;
		}
		public static object EditValue(object objectToChange, string propName, IServiceProvider parentServiceProvider) {
			PropertyDescriptor pd = TypeDescriptor.GetProperties(objectToChange)[propName];
			return EditValue(objectToChange, pd, parentServiceProvider);
		}
		public static object EditValue(object objectToChange, IServiceProvider parentServiceProvider) {
			ITypeDescriptorContext context = new TypeDescriptorContext(null, null, parentServiceProvider);
			UITypeEditor editor = TypeDescriptor.GetEditor(objectToChange.GetType(), typeof(UITypeEditor)) as UITypeEditor;
			if(editor == null)
				return null;
			return editor.EditValue(context, context, objectToChange);
		}
	}
}
