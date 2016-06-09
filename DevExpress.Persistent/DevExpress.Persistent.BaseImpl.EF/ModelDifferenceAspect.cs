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
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.Persistent.BaseImpl.EF {
	[ImageName("ModelEditor_ModelMerge")]
	[DefaultProperty("DispayName")]
	public class ModelDifferenceAspect : IModelDifferenceAspect {
		private String name;
		private String xml;
		public ModelDifferenceAspect()
			: base() {
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		[NotMapped]
		public String DisplayName {
			get {
				return string.IsNullOrEmpty(name) ? CaptionHelper.GetLocalizedText("Texts", "DefaultAspectText", "(Default language)") : name;
			}
		}
		[VisibleInListView(false), VisibleInLookupListView(false)]
		public String Name {
			get { return name; }
			set { name = value; }
		}
		[FieldSize(FieldSizeAttribute.Unlimited)]
		public String Xml {
			get { return xml; }
			set { xml = value; }
		}
		[RuleRequiredField(null, DefaultContexts.Save)]
		public virtual ModelDifference Owner { get; set; }
		IModelDifference IModelDifferenceAspect.Owner {
			get { return Owner; }
			set { Owner = value as ModelDifference; }
		}
	}
}
