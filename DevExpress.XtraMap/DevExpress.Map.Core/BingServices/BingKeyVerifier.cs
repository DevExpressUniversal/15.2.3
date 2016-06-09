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
namespace DevExpress.Map.Native {
	public static class DXBingKeyVerifier {
		static DXBingKeyVerifierBase verifier;
		internal static DXBingKeyVerifierBase Verifier { get { return verifier; } }
		static DXBingKeyVerifierBase CreateVerifier(string platform) {
			switch(platform) {
				case "win": return new WinDXBingKeyVerifier();
				case "xpf": return new XpfDXBingKeyVerifier();
				default: throw new ArgumentException("Incorrect platform");
			}
		}
		internal static void UnregisterPlatform() {
			verifier = null;
		}
		internal static string GetShortProductName(string fullName) {
			string[] parts = fullName.Split(new string[] { ", Version" }, StringSplitOptions.None);
			return parts.Length > 0 ? parts[0] : fullName;
		}
		public static void RegisterPlatform(string platform) {
			if(verifier == null)
				verifier = CreateVerifier(platform.ToLower());
		}
		public static bool IsKeyRestricted(string key, string product) {
			if(verifier == null)
				return false;
			if(!verifier.PlatformKeys.Contains(key))
				return false;
			return !verifier.VerifiedProducts.Contains(GetShortProductName(product));
		}
	}
	internal abstract class DXBingKeyVerifierBase {
		protected internal const string WinGeneratedKey = "AjHvZv_xAeledX4293nfRBryoygbD7y6X2eXqOUWqDmh3oBxb1ADEvAyTZLkC3RR";
		protected internal const string XpfGeneratedKey = "AhhH2QnNpS36rRPZ1Oi6zevrl5KFBSdlG4Q9ZV_V8lmd-JtoqcShx8-eYYmfXSFk";
		protected internal const string UwpGeneratedKey = "AkZWkMAhAbMjCNfI2Zne2x92MIqhXb-26gPGT8ReMCYZA0dtfZqZUJi6R3xtFY2f";
		protected internal const string VclGeneratedKey = "AubpbILDrKaxhMOpG0LqDlSEY8ZV7S5-KzNJeBj4qCP49S4LEOb_qdQtXWkwxmsl";
		readonly List<string> platformKeys;
		readonly List<string> verifiedProducts;
		protected internal List<string> PlatformKeys { get { return platformKeys; } }
		protected internal List<string> VerifiedProducts { get { return verifiedProducts; } }
		protected DXBingKeyVerifierBase() {
			this.platformKeys = PopulateKeys();
			this.verifiedProducts = PopulateProducts();
		}
		protected List<string> PopulateKeys() {
			return new List<string>() { WinGeneratedKey, XpfGeneratedKey, UwpGeneratedKey, VclGeneratedKey };
		}
		protected abstract List<string> PopulateProducts();
	}
	internal class WinDXBingKeyVerifier : DXBingKeyVerifierBase {
		protected override List<string> PopulateProducts() {
			return new List<string>() {
				"MapMainDemo", 
				"DevExpress.HybridApp.Win", 
				"DevExpress.OutlookInspiredApp.Win", 
				"DevExpress.ProductsDemo.Win" 
			};
		}
	}
	internal class XpfDXBingKeyVerifier : DXBingKeyVerifierBase {
		protected override List<string> PopulateProducts() {
			return new List<string>() {
				"MapDemo",
				"DevExpress.OutlookInspiredApp.Wpf",
				"DemoLauncher"
			};
		}
	}
}
