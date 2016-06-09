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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpf.PropertyGrid;
using DevExpress.Xpf.Editors.Helpers;
using System.ComponentModel;
namespace DevExpress.XtraVerticalGrid.Data {
	public class GroupDescriptorContext : DescriptorContext {
		public GroupDescriptorContext(object instance, string name, IServiceProvider serviceProvider, bool isMultiSource, string fieldName)
			: base(serviceProvider, isMultiSource) {
			MultiInstance = instance;
			GroupName = name;
			BaseFieldName = fieldName;
			FieldName = FieldNameHelper.GetCategoryFieldName(fieldName, name);
		}
		public override bool IsLoaded { get { return true; } }
		public override string DisplayName { get { return GroupName; } }
		public override bool IsGetPropertiesSupported { get { return true; } }
		public override PropertyDescriptor PropertyDescriptor { get { return null; } internal set { } }
		string GroupName { get; set; }
		public override bool CanExpand { get { return true; } }
		protected string BaseFieldName { get; set; }
		protected override IEnumerable<RowHandle> CreateVisibleHandles() {
			DescriptorContext instanceContext = DataController.GetDescriptorContext(BaseFieldName);
			return instanceContext.ChildHandles.Where(handle => {
				DescriptorContext context = DataController.GetDescriptorContext(handle);
				if (context.CategoryName == GroupName) {
					return true;
				}
				return false;
			});
		}
		protected override void ParentContextChanged() {
		}
		protected override void VisibleHandlesCreated() {
		}
		protected override object GetValue() {
			return null;
		}
		internal protected override string GetFieldNameForChild() {
			return BaseFieldName;
		}
		public override bool CanResetValue() {
			return false;
		}
		protected override IEnumerable<string> GetValidationError() {
			return null;
		}
		protected override void SubscribeNotifications() {
		}
	}
}
