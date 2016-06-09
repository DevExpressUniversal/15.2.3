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
using System.Collections.Generic;
using DevExpress.Web.Design;
using System.Windows.Forms;
namespace DevExpress.XtraReports.Web.Design {
	public class ReportToolbarItemCollectionEditor : DevExpress.Web.Design.CollectionEditor {
		#region inner classes
		class ToolbarItemCollectionEditorForm : CollectionEditorForm {
			public ToolbarItemCollectionEditorForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue)
				: base(component, context, provider, propertyValue) {
			}
			protected override List<CollectionItemType> GetCollectionItemTypes() {
				List<CollectionItemType> types = new List<CollectionItemType>();
				types.Add(CreateCollectionItemType(typeof(ReportToolbarButton)));
				types.Add(CreateCollectionItemType(typeof(ReportToolbarLabel)));
				types.Add(CreateCollectionItemType(typeof(ReportToolbarTextBox)));
				types.Add(CreateCollectionItemType(typeof(ReportToolbarComboBox)));
				types.Add(CreateCollectionItemType(typeof(ReportToolbarSeparator)));
				types.Add(CreateCollectionItemType(typeof(ReportToolbarTemplateItem)));
				return types;
			}
			CollectionItemType CreateCollectionItemType(Type type) {
				return new CollectionItemType(type, type.Name);
			}
			protected override object CreateNewItem() {
				return new ReportToolbarButton();
			}
		}
		#endregion
		public override Form CreateEditorForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue) {
			return new ToolbarItemCollectionEditorForm(component, context, provider, propertyValue);
		}
	}
}
