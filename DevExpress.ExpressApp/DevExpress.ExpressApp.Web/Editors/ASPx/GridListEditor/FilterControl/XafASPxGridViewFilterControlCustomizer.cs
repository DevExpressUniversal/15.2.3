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

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.Web.Editors.ASPx.GridListEditor.FilterControl {
	internal class XafASPxGridViewFilterControlCustomizer {
		private ITypeInfo rootTypeInfo;
		private IModelBOModel boModel;
		public XafASPxGridViewFilterControlCustomizer(ITypeInfo rootTypeInfo, IModelBOModel boModel, IASPxGridViewFilterControl control) {
			Guard.ArgumentNotNull(rootTypeInfo, "rootTypeInfo");
			Guard.ArgumentNotNull(boModel, "boModel");
			Guard.ArgumentNotNull(control, "control");
			this.rootTypeInfo = rootTypeInfo;
			this.boModel = boModel;
			control.CustomizeColumn += FilterControlCustomizer_CustomizeColumn;
		}
		private void FilterControlCustomizer_CustomizeColumn(object sender, CustomizeFilterControlColumnEventArgs e) {
			if(!string.IsNullOrEmpty(e.FullPropertyName)) {
				IMemberInfo mi = rootTypeInfo.FindMember(e.FullPropertyName);
				if(mi != null) {
					IMemberInfo lastMember = mi.LastMember ?? null;
					if(lastMember != null) {
						if(!mi.LastMember.IsVisible) {
							e.Visible = false; 
						}
						else {
							if(mi.LastMember.BindingName != e.PropertyName) {
								e.Visible = false;
							}
							else if(mi.LastMember.Owner != null) {
								IModelClass modelClass = boModel.GetClass(mi.LastMember.Owner.Type);
								if(modelClass != null) {
									IModelMember modelMember = modelClass.FindMember(mi.LastMember.Name);
									if(modelMember != null) {
										e.DisplayName = modelMember.Caption;
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
