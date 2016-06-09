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
using System.ComponentModel;
using DevExpress.Web.Design;
using System.Windows.Forms;
namespace DevExpress.Web.Design {
	public class FilterControlColumnsCollectionEditor : DevExpress.Web.Design.CollectionEditor {
		public override Form CreateEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue) {
			return new FilterControlColumnsCollectionEditorForm(component, context, provider, propertyValue);
		}
	}
	public class FilterControlColumnsCollectionEditorForm : CollectionEditorForm {
		static List<CollectionItemType> itemTypes;
		static FilterControlColumnsCollectionEditorForm() {
			itemTypes = new List<CollectionItemType>();
			itemTypes.Add(new CollectionItemType(typeof(FilterControlTextColumn), "Text Column"));
			itemTypes.Add(new CollectionItemType(typeof(FilterControlButtonEditColumn), "Button Edit Column"));
			itemTypes.Add(new CollectionItemType(typeof(FilterControlMemoColumn), "Memo Column"));
			itemTypes.Add(new CollectionItemType(typeof(FilterControlHyperLinkColumn), "Hyperlink Column"));
			itemTypes.Add(new CollectionItemType(typeof(FilterControlCheckColumn), "Check Column"));
			itemTypes.Add(new CollectionItemType(typeof(FilterControlDateColumn), "Date Column"));
			itemTypes.Add(new CollectionItemType(typeof(FilterControlSpinEditColumn), "Spin Edit Column"));
			itemTypes.Add(new CollectionItemType(typeof(FilterControlComboBoxColumn), "Combobox Column"));
			itemTypes.Add(new CollectionItemType(typeof(FilterControlComboBoxColumn), "Object Column"));
		}
		public FilterControlColumnsCollectionEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue)
			: base(component, context, provider, propertyValue) {
		}
		protected override List<CollectionItemType> GetCollectionItemTypes() { return itemTypes; }
	}	
}
