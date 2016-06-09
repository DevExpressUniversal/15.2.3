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
using System.Linq;
using System.Text;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Design;
using DevExpress.Design.SmartTags;
using DevExpress.Utils;
using DevExpress.Data.Utils;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public class CreateXamlMarkupService2010 : ICreateXamlMarkupService2010 {
		IEditingContext editingContext;
		IXamlSourceDocument xamlSourceDocument;
		IXamlObjectElement xamlElement;
		public CreateXamlMarkupService2010(IEditingContext editingContext, IMarkupAccessService2010 markupAccessService) {
			Guard.ArgumentNotNull(editingContext, "editingContext");
			Guard.ArgumentNotNull(markupAccessService, "markupAccessService");
			this.editingContext = editingContext;
			IModelItem modelItem = editingContext.Services.GetService<IModelService>().Root;
			IVirtualModelItem virtualModelItem = markupAccessService.GetModelItem(modelItem);
			IVirtualModelHost virtualModelHost = virtualModelItem.Host;
			IXamlItem xamlRootItem = virtualModelHost.FindNode(virtualModelHost.Root.Identity) as IXamlItem;
			if(xamlRootItem == null)
				throw new InvalidOperationException();
			this.xamlSourceDocument = xamlRootItem.Document;
			IXamlItem xamlItem = (IXamlItem)virtualModelHost.FindNode(virtualModelItem.Identity);
			this.xamlElement = xamlItem.Element;
		}
		public IModelItem CreateXamlMarkup(string xamlTypeName) {
			IVS2010TypeMetadata markupTypeMetadata = xamlSourceDocument.ResolveType(xamlElement, xamlTypeName);
			if(markupTypeMetadata == null)
				markupTypeMetadata = xamlSourceDocument.ResolveType(xamlElement, xamlTypeName + "Extension");
			if(markupTypeMetadata == null) return null;
			return editingContext.CreateItem(markupTypeMetadata);
		}
	}
	public class CreateXamlMarkupService2012 : ICreateXamlMarkupService2012 { }
}
