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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Presentation.Metadata;
using DevExpress.Workflow.Activities.Design;
using System.ComponentModel;
using DevExpress.Workflow.Activities;
using System.Activities.Core.Presentation;
namespace DevExpress.Workflow.Activities.Design {
	public class ActivitiesMetadata : IRegisterMetadata
	{
		public void Register()
		{
			DesignerMetadata metaData = new DesignerMetadata();
			metaData.Register();
			AttributeTableBuilder builder = new AttributeTableBuilder();
			builder.AddCustomAttributes(typeof(NoPersistScope), new DesignerAttribute(typeof(UniversalSequenceDesigner)));
			builder.AddCustomAttributes(typeof(DeleteObject<>), new DesignerAttribute(typeof(ObjectKeyActivityDesigner)));
			builder.AddCustomAttributes(typeof(GetObjectByKey<>), new DesignerAttribute(typeof(ObjectKeyActivityDesigner)));
			builder.AddCustomAttributes(typeof(FindObjectByCriteria<>), new DesignerAttribute(typeof(CriteriaActivityDesigner)));
			builder.AddCustomAttributes(typeof(GetObjectsByCriteria<>), new DesignerAttribute(typeof(CriteriaActivityDesigner)));
			builder.AddCustomAttributes(typeof(TransactionalDeleteObject<>), new DesignerAttribute(typeof(ObjectKeyActivityDesigner)));
			builder.AddCustomAttributes(typeof(TransactionalGetObjectByKey<>), new DesignerAttribute(typeof(ObjectKeyActivityDesigner)));
			builder.AddCustomAttributes(typeof(TransactionalFindObjectByCriteria<>), new DesignerAttribute(typeof(CriteriaActivityDesigner)));
			builder.AddCustomAttributes(typeof(TransactionalGetObjectsByCriteria<>), new DesignerAttribute(typeof(CriteriaActivityDesigner)));
			MetadataStore.AddAttributeTable(builder.CreateTable());
		}
	}
}
