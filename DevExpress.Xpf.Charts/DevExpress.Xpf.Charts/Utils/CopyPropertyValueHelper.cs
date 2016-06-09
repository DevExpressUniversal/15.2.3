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
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;
namespace DevExpress.Xpf.Charts.Native {
	public class CopyPropertyValueHelper {
		static void CopyPropertyValueCore(DependencyObject recipient, DependencyObject source, DependencyProperty dp, bool copyByRef) {
			if (IsValueSet(source, dp)) {
				object value = source.GetValue(dp);
				DependencyObject dependencyObject = value as DependencyObject;
				Binding binding = BindingOperations.GetBinding(source, dp);
				if (binding != null) {
					BindingOperations.SetBinding(recipient, dp, binding);
					return;
				}
				if (!copyByRef && dependencyObject != null)
					try {
						dependencyObject = XamlReader.Load(XmlReader.Create(new StringReader(XamlWriter.Save(dependencyObject)))) as DependencyObject;
					}
					catch {
					}			 
				recipient.SetValue(dp, dependencyObject == null ? value : dependencyObject);
			}
		}
		public static void CopyPropertyValue(DependencyObject recipient, DependencyObject source, DependencyProperty dp) {
			CopyPropertyValueCore(recipient, source, dp, false);
		}
		public static void CopyPropertyValueByRef(DependencyObject recipient, DependencyObject source, DependencyProperty dp) {
			CopyPropertyValueCore(recipient, source, dp, true);
		}
		public static bool IsValueSet(DependencyObject source, DependencyProperty dp) {
			return !Object.ReferenceEquals(source.ReadLocalValue(dp), DependencyProperty.UnsetValue);
		}
		public static bool VerifyValues(DependencyObject recipient, DependencyObject source, DependencyProperty dp) {
			Binding binding = BindingOperations.GetBinding(source, dp);
			if (binding != null) {
				BindingOperations.SetBinding(recipient, dp, binding);
				return false;
			}			 
			object value = source.ReadLocalValue(dp);
			if (value != null)
				return true;
			recipient.SetValue(dp, null);
			return false;
		}
	}
}
