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
namespace DevExpress.DemoData.Model {
	public class WebDemo : SimpleDemo {
		public WebDemo(
			Product product, 
			Func<Demo, List<Module>> createModules,
			string name,
			string displayName, 
			string csSolutionPath, 
			string vbSolutionPath,
			string launchPath,
			KnownDXVersion addedIn = KnownDXVersion.Before142, 
			KnownDXVersion updatedIn = KnownDXVersion.Before142, 
			Requirement[] requirements = null) 
			: base(product, createModules, name, displayName, csSolutionPath, 
				  vbSolutionPath, launchPath, null, addedIn, updatedIn, requirements) {
		}
	}
	public class WebModule : SimpleModule {
		public WebModule(Demo demo, string name, string displayName, string group, string type,
			string description, KnownDXVersion addedIn, KnownDXVersion updatedIn = KnownDXVersion.Before142, 
			int featuredPriority = int.MaxValue, int newUpdatedPriority = int.MaxValue, bool isFeatured = false, 
			string[] associatedFiles = null, bool isCodeExample = false, bool showFromDemoChooser = true) 
			: base(demo, name, displayName, group, type, description, addedIn, updatedIn, 
				  featuredPriority, newUpdatedPriority, isFeatured, associatedFiles, isCodeExample, showFromDemoChooser) {
		}
		public override string[] Arguments {
			get {
				return new[] {
					Group,
					Name + (Demo.Platform == Repository.AspPlatform ? ".aspx" : "")
				};
			}
		}
	}
}
