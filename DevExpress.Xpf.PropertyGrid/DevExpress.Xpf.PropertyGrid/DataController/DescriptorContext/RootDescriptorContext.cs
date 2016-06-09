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
using DevExpress.Xpf.PropertyGrid;
using DevExpress.Xpf.Editors.Helpers;
using System.ComponentModel;
namespace DevExpress.XtraVerticalGrid.Data {
	public class RootDescriptorContext : DescriptorContext {
		public RootDescriptorContext(object instance, IServiceProvider serviceProvider, bool isMultiSource)
			: base(serviceProvider, isMultiSource) {
			MultiInstance = instance;
		}
		public override bool CanExpand { get { return true; } }
		protected internal override void ContextAdded() {
			base.ContextAdded();
			SetIsExpanded(true);
		}
		protected internal override void SetIsExpandedInternal(bool? isExpanded) {
			base.SetIsExpandedInternal(isExpanded);
			if (isExpanded.HasValue) {
				var categoryHandles = CategoryHandles;
			}
		}
		protected override PropertyDescriptorCollection GetPropertiesInternal(object source, Attribute[] attributes) {
			if (!IsGetPropertiesSupported)
				return TypeDescriptor.GetProperties(source, attributes, false);
			return base.GetPropertiesInternal(source, attributes);
		}
		protected override object GetValue() {
			return Instance;
		}
		protected override IEnumerable<string> GetValidationError() {
			return null;
		}
	}
}
