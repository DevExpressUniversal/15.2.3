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
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
namespace DevExpress.XtraPrinting.Design.Resources {
	public class ResXResourceService {
		object resXResourceService;
		Type resXResourceServiceType;
		public ResXResourceService(IServiceProvider serviceProvider, ResourcePickerService rpSvc) {
			resXResourceServiceType = GetVSType(serviceProvider);
			Type resourcePickerServiceType = ResourcePickerService.GetVSType(serviceProvider);
			ConstructorInfo ci = resXResourceServiceType.GetConstructor(new Type[] { resourcePickerServiceType });
			resXResourceService = ci.Invoke(new object[] { rpSvc.Instance });
		}
		public IResourceReader GetResXResourceReader(string resXFullName, bool useResXDataNodes) {
			return (IResourceReader)CallMethod("GetResXResourceReader", resXFullName, useResXDataNodes);
		}
		public IResourceWriter GetResXResourceWriter(string resXFullName) {
			return (IResourceWriter)CallMethod("GetResXResourceWriter", resXFullName);
		}
		object CallMethod(String name, params object[] args) {
			Type[] argTypes = Array.ConvertAll<object, Type>(args, delegate(object arg) { return arg.GetType(); });
			MethodInfo mi = resXResourceServiceType.GetMethod(name, argTypes);
			return mi.Invoke(resXResourceService, args);
		}
		static Type GetVSType(IServiceProvider serviceProvider) {
			return VSTypeHelper.GetType(serviceProvider, "Microsoft.VisualStudio.Windows.Forms", "Microsoft.VisualStudio.Windows.Forms.ResXResourceService");
		}
	}
}
