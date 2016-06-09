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

namespace DevExpress.Utils.Filtering {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	public interface IControlCreator {
		IEditorGeneratorItemWrapper CreateItem(ElementBindingInfo bindingInfo);
		IEditorGeneratorGroupWrapper CreateGroup(ElementBindingInfo bindingInfo);
		void AddItemToGroup(IEditorGeneratorItemWrapper item, IEditorGeneratorGroupWrapper group);
		void BeginCreateLayout();
		void BestSize();
		void EndCreateLayout();
		object DataSource { get; set; }
		string DataMember { get; set; }
	}
	public class EditorGenerator {
		public EditorGenerator(IControlCreator creator) {
			controlCreatorCore = creator;
		}
		IControlCreator controlCreatorCore;
		public IControlCreator ControlCreator {
			get { return controlCreatorCore; }
		}
		public virtual void CreateLayout(Type type) {
			ElementBindingInfoHelper helper = CreateElementBindingInfoHelper();
			List<ElementBindingInfo> list = helper
				.CreateDataLayoutElementsBindingInfo(type)
				.GetAllBindings();
			ControlCreator.BeginCreateLayout();
			var groups = list
				.GroupBy(x => x.GroupName)
				.OrderBy(x => x.Key)
				.ToList();
			foreach(var group in groups) {
				IEditorGeneratorGroupWrapper newGroup = CreateGroup(group.FirstOrDefault());
				foreach(var item in group) {
					IEditorGeneratorItemWrapper newItem = CreateItem(item);
					ControlCreator.AddItemToGroup(newItem, newGroup);
				}
			}
			ControlCreator.BestSize();
			ControlCreator.EndCreateLayout();
		}
		protected virtual IEditorGeneratorItemWrapper CreateItem(ElementBindingInfo bindingInfo) {
			return ControlCreator.CreateItem(bindingInfo);
		}
		protected virtual IEditorGeneratorGroupWrapper CreateGroup(ElementBindingInfo bindingInfo) {
			return ControlCreator.CreateGroup(bindingInfo);
		}
		protected virtual ElementBindingInfoHelper CreateElementBindingInfoHelper() {
			return new ElementBindingInfoHelper();
		}
	}
}
