﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

namespace DevExpress.ExpressApp.PivotChart {
	using System;
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class LocalizationResources {
		private static global::System.Resources.ResourceManager resourceMan;
		private static global::System.Globalization.CultureInfo resourceCulture;
		[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal LocalizationResources() {
		}
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Resources.ResourceManager ResourceManager {
			get {
				if (object.ReferenceEquals(resourceMan, null)) {
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DevExpress.ExpressApp.PivotChart.LocalizationResources", typeof(LocalizationResources).Assembly);
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
		internal static string ChartTabCaption {
			get {
				return ResourceManager.GetString("ChartTabCaption", resourceCulture);
			}
		}
		internal static string ChartTypeCaption {
			get {
				return ResourceManager.GetString("ChartTypeCaption", resourceCulture);
			}
		}
		internal static string ChartWizardText {
			get {
				return ResourceManager.GetString("ChartWizardText", resourceCulture);
			}
		}
		internal static string IncorrectChartTypeErrorMessage {
			get {
				return ResourceManager.GetString("IncorrectChartTypeErrorMessage", resourceCulture);
			}
		}
		internal static string PivotTabCaption {
			get {
				return ResourceManager.GetString("PivotTabCaption", resourceCulture);
			}
		}
		internal static string PrintChartText {
			get {
				return ResourceManager.GetString("PrintChartText", resourceCulture);
			}
		}
		internal static string ShowColumnGrandTotalText {
			get {
				return ResourceManager.GetString("ShowColumnGrandTotalText", resourceCulture);
			}
		}
		internal static string ShowRowGrandTotalText {
			get {
				return ResourceManager.GetString("ShowRowGrandTotalText", resourceCulture);
			}
		}
	}
}
