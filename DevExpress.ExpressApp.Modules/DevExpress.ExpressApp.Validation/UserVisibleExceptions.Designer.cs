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

namespace DevExpress.ExpressApp.Validation {
	using System;
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class UserVisibleExceptions {
		private static global::System.Resources.ResourceManager resourceMan;
		private static global::System.Globalization.CultureInfo resourceCulture;
		[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal UserVisibleExceptions() {
		}
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Resources.ResourceManager ResourceManager {
			get {
				if (object.ReferenceEquals(resourceMan, null)) {
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DevExpress.ExpressApp.Validation.UserVisibleExceptions", typeof(UserVisibleExceptions).Assembly);
					resourceMan = temp;
				}
				return resourceMan;
			}
		}
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Globalization.CultureInfo Culture {
			get {
				return resourceCulture;
			}
			set {
				resourceCulture = value;
			}
		}
		internal static string ActionErrorMessageHeaderFormat {
			get {
				return ResourceManager.GetString("ActionErrorMessageHeaderFormat", resourceCulture);
			}
		}
		internal static string ActionErrorMessageObjectFormat {
			get {
				return ResourceManager.GetString("ActionErrorMessageObjectFormat", resourceCulture);
			}
		}
		internal static string AllContextsErrorMessageHeader {
			get {
				return ResourceManager.GetString("AllContextsErrorMessageHeader", resourceCulture);
			}
		}
		internal static string DeleteErrorMessageHeader {
			get {
				return ResourceManager.GetString("DeleteErrorMessageHeader", resourceCulture);
			}
		}
		internal static string DeleteErrorMessageObjectFormat {
			get {
				return ResourceManager.GetString("DeleteErrorMessageObjectFormat", resourceCulture);
			}
		}
		internal static string SaveErrorMessageHeader {
			get {
				return ResourceManager.GetString("SaveErrorMessageHeader", resourceCulture);
			}
		}
		internal static string SaveErrorMessageObjectFormat {
			get {
				return ResourceManager.GetString("SaveErrorMessageObjectFormat", resourceCulture);
			}
		}
		internal static string ValidationSucceededMessageHeader {
			get {
				return ResourceManager.GetString("ValidationSucceededMessageHeader", resourceCulture);
			}
		}
	}
}
