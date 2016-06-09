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
using System.Drawing;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Templates.Bars.Utils {
	public abstract class RepositoryItemAdapterBase {
		private readonly RepositoryItem repositoryItem;
		private readonly Type parameterType;
		protected virtual void RaiseExecute(object parameter) {
			if(Execute != null) {
				object convertedParameter = parameter;
				if(parameter != null) {
					convertedParameter = Convert.ChangeType(parameter, ParameterType);
				}
				Execute(this, new ParametrizedActionControlExecuteEventArgs(convertedParameter));
			}
		}
		protected RepositoryItemAdapterBase(RepositoryItem repositoryItem, Type parameterType) {
			Guard.ArgumentNotNull(repositoryItem, "repositoryItem");
			Guard.ArgumentNotNull(parameterType, "parameterType");
			this.repositoryItem = repositoryItem;
			this.parameterType = parameterType;
		}
		public abstract void SetupRepositoryItem();
		public abstract void SetNullValuePrompt(string nullValuePrompt);
		public abstract void SetExecuteButtonCaption(string caption);
		public abstract void SetExecuteButtonImage(Image image);
		public RepositoryItem RepositoryItem {
			get { return repositoryItem; }
		}
		public Type ParameterType {
			get { return parameterType; }
		}
		public bool ShowExecuteButton { get; set; }
		public event EventHandler<ParametrizedActionControlExecuteEventArgs> Execute;
	}
}
