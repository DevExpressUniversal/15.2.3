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

using System.ComponentModel;
using System.Web.UI;
using DevExpress.ExpressApp.Model;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class CreateCustomItemTemplateEventArgs : HandledEventArgs {
		private ITemplate template;
		private IModelColumn modelColumn;
		private IDataItemTemplateInfoProvider dataItemTemplateInfoProvider;
		public CreateCustomItemTemplateEventArgs(IModelColumn modelColumn, IDataItemTemplateInfoProvider dataItemTemplateInfoProvider) {
			this.modelColumn = modelColumn;
			this.dataItemTemplateInfoProvider = dataItemTemplateInfoProvider;
		}
		public IModelColumn ModelColumn {
			get { return modelColumn; }
		}
		public IDataItemTemplateInfoProvider DataItemTemplateInfoProvider {
			get { return dataItemTemplateInfoProvider; }
		}
		public ITemplate Template {
			get { return template; }
			set { template = value; }
		}
	}
	public class CreateCustomDataItemTemplateEventArgs : CreateCustomItemTemplateEventArgs {
		private bool needCreateDataItemTemplate = false;
		public CreateCustomDataItemTemplateEventArgs(IModelColumn modelColumn, IDataItemTemplateInfoProvider dataItemTemplateInfoProvider)
			: base(modelColumn, dataItemTemplateInfoProvider) {
		}
		public bool CreateDefaultDataItemTemplate {
			get {
				return needCreateDataItemTemplate;
			}
			set {
				needCreateDataItemTemplate = value;
			}
		}
	}
	public class CreateCustomEditItemTemplateEventArgs : CreateCustomItemTemplateEventArgs {
		public CreateCustomEditItemTemplateEventArgs(IModelColumn modelColumn, IDataItemTemplateInfoProvider dataItemTemplateInfoProvider)
			: base(modelColumn, dataItemTemplateInfoProvider) {
		}
	}
	public class CustomizeDataItemTemplateEventArgs : HandledEventArgs {
		private GridViewDataColumn column;
		private IModelColumn modelColumn;
		private bool needCreateDataItemTemplate = false;
		public CustomizeDataItemTemplateEventArgs(IModelColumn modelColumn, GridViewDataColumn column) {
			this.modelColumn = modelColumn;
			this.column = column;
		}
		public bool CreateDefaultDataItemTemplate {
			get {
				return needCreateDataItemTemplate;
			}
			set {
				needCreateDataItemTemplate = value;
			}
		}
		public IModelColumn ModelColumn {
			get { return modelColumn; }
		}
		public GridViewDataColumn Column {
			get { return column; }
		}
	}
}
