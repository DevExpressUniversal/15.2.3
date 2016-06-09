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
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Model.NodeGenerators {
	public static class ModelNodesGeneratorSettings {
		private static readonly Dictionary<Type, string> customPrefixByType = new Dictionary<Type, string>();
		public static string GetIdPrefix(Type type) {
			Guard.ArgumentNotNull(type, "type");
			string prefix;
			if(!TryGetCustomIdPrefix(type, out prefix)) {
				prefix = ModelBOModelClassNodesGenerator.GetTypeNameId(type);
			}
			return prefix;
		}
		private static bool TryGetCustomIdPrefix(Type type, out string prefix) {
			lock(customPrefixByType) {
				return customPrefixByType.TryGetValue(type, out prefix);
			}
		}
		public static void SetIdPrefix(Type type, string prefix) {
			Guard.ArgumentNotNull(type, "type");
			Guard.ArgumentNotNullOrEmpty(prefix, "prefix");
			lock(customPrefixByType) {
				customPrefixByType[type] = prefix;
			}
		}
#if DebugTest
		public static void DebugTest_Reset() {
			lock(customPrefixByType) {
				customPrefixByType.Clear();
			}
		}
#endif
	}
}
