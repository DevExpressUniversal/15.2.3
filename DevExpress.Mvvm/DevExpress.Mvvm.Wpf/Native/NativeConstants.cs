#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Mvvm.Native {
	static class XmlNamespaceConstants {
		public const string MvvmPrefix = "dxmvvm";
		public const string MvvmNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/mvvm";
		public const string MvvmNamespace = "DevExpress.Mvvm";
		public const string MvvmIntenalPrefix = "dxmvvminternal";
		public const string MvvmInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/mvvm/internal";
		public const string MvvmUINamespace = "DevExpress.Mvvm.UI";
		public const string MvvmInteractivityNamespace = "DevExpress.Mvvm.UI.Interactivity";
		public const string MvvmInteractivityInternalNamespace = "DevExpress.Mvvm.UI.Interactivity.Internal";
	}
	static class MvvmAssemblyHelper {
		const string PublicKey = "PublicKey=0024000004800000940000000602000000240000525341310004000001000100c9634fb4bd8481aa329ba0fbb6fe72063462135ecfb3c8a8a595ce4436df2697f258912e9aa0f705fabfb1d13d6044a55a42cee9fe81bcdce571a4fbeeaa758e1d4a2a20ffd8ea05018133be71da27951f0103ab3cec55f78009ff0d00fb8b02db756437ecee8893c624c8f1c639f342ceda43dd8a443460ca93d0d2990904bb";
		public const string TestsAssemblyName = "DevExpress.Mvvm.Tests, " + PublicKey;
		public const string TestsFreeAssemblyName = "DevExpress.Mvvm.Tests.Free, " + PublicKey;
		public const string MvvmUIAssemblyName = "DevExpress.Mvvm.UI, " + PublicKey;
	}
}
