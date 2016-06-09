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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
namespace DevExpress.ExpressApp.Xpo {
	public static class XpoTypesInfoHelper {
		private static XpoTypeInfoSource xpoTypeInfoSource;
		public static void ForceInitialize() {
			lock(XafTypesInfo.Instance) {
				if(xpoTypeInfoSource == null) {
					TypesInfo typesInfo = (TypesInfo)XafTypesInfo.Instance;
					xpoTypeInfoSource = new XpoTypeInfoSource(typesInfo);
					((TypesInfo)XafTypesInfo.Instance).AddEntityStore(xpoTypeInfoSource);
				}
			}
		}
		public static ITypesInfo GetTypesInfo() {
			return XafTypesInfo.Instance;
		}
		public static XpoTypeInfoSource GetXpoTypeInfoSource() {
			if(xpoTypeInfoSource == null) {
				ForceInitialize();
			}
			return xpoTypeInfoSource;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void Reset() {
			xpoTypeInfoSource = null;
		}
	}
}
