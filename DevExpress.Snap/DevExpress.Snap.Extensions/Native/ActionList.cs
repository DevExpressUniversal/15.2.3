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

using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design;
using System;
using System.ComponentModel;
namespace DevExpress.Snap.Extensions.Native {
	public abstract class ActionListBase : IDesignerActionList {
		#region static
		static protected ActionPropertyItem CreatePropertyItem(string memberName, string displayName) {
			return new ActionPropertyItem(memberName, displayName);
		}
		static protected DesignerActionMethodItemBase CreateMethodItem(Delegate action, string displayName) {
			return new DesignerActionMethodItemBase(action, displayName);
		}
		#endregion
		#region Properties
		public abstract ComponentImplementation Component { get; }
		IComponent IDesignerActionList.Component { get { return Component; } }
		public object PropertiesContainer { get { return this; } }
		#endregion
		protected PropertyDescriptor GetPropertyDescriptor(string name, object editedObject) {
			return TypeDescriptor.GetProperties(editedObject)[name];
		}
		protected void AddPropertyItem(ActionItemCollection actionItems, string name, string reflectName, object editedObject) {
			PropertyDescriptor property = GetPropertyDescriptor(name, editedObject);
			if (property != null)
				actionItems.Add(CreatePropertyItem(reflectName, property.DisplayName));
		}
		protected void AddMethodItem(ActionItemCollection actionItems, string name, Action action) {
			actionItems.Add(CreateMethodItem(action, name));
		}
		public PropertyDescriptor FilterProperty(string name, PropertyDescriptor property) {
			return property;
		}
		public IDesignerActionItemCollection GetSortedActionItems() {
			ActionItemCollection actionItems = new ActionItemCollection();
			FillActionItemCollection(actionItems);
			return actionItems;
		}
		protected virtual void FillActionItemCollection(ActionItemCollection actionItems) {
		}
	}
	public class ActionList<T> : ActionListBase {
		readonly ComponentImplementation component;
		public ActionList(T editedObject) {
			this.component = new ComponentImplementation(editedObject, null);
		}
		public override ComponentImplementation Component { get { return component; } }
		protected virtual T EditedObject { get { return (T)Component.CustomObject; } }		
		protected virtual void SetPropertyValue(string name, object val) {
			SetPropertyValue(EditedObject, name, val);
		}
		protected virtual void SetPropertyValue(object component, string name, object val) {
			PropertyDescriptor property = TypeDescriptor.GetProperties(component)[name];
			if (property != null)
				property.SetValue(component, val);
		}
	}
}
