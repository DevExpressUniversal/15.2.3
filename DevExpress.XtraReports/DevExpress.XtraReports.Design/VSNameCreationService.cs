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
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using System.Text;
namespace DevExpress.XtraReports.Design {
	class VSNameCreationService : INameCreationService {
		INameCreationService parentService;
		IContainer container;
		public VSNameCreationService(INameCreationService parentService, IContainer container) {
			this.parentService = parentService;
			this.container = container;
		}
		public string CreateName(IContainer container, Type dataType) {
			return parentService.CreateName(container, dataType);
		}
		public bool IsValidName(string name) {
			return parentService.IsValidName(name) && container.Components[name] == null;
		}
		public void ValidateName(string name) {
			parentService.ValidateName(name);
			if(container.Components[name] != null) {
				throw GetStandardException(name) ?? new ArgumentException("Duplicate component name " + name);
			}
		}
		static Exception GetStandardException(string name) {
			Type type = typeof(CodeDomDesignerLoader).Assembly.GetType("System.Design.SR");
			if(type == null)
				return null;
			MethodInfo getString = type.GetMethod("GetString", new Type[] { typeof(string), typeof(object[]) });
			if(getString == null)
				return null;
			string message = (string)getString.Invoke(null, new object[] { "CodeDomDesignerLoaderDupComponentName", new object[] { name } });
			if(string.IsNullOrEmpty(message))
				return null;
			return new ArgumentException(message) { HelpLink = "CodeDomDesignerLoaderDupComponentName" };
		}
	}
}
