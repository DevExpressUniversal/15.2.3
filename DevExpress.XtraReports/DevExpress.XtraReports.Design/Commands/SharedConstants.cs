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
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.VSPackage {
	static class SharedGuidList {
		public const string guidPackagePkgString = "625908b4-e24e-4d9a-ab5a-51055e9e6d62";
		public const string guidPackageInteractionCmdSetString = "C66F11BE-479F-4684-98BA-8D0A8D403A84";
		public static readonly Guid guidPackage = new Guid(guidPackagePkgString);
		public static readonly Guid guidPackageInteractionCmdSet = new Guid(guidPackageInteractionCmdSetString);
	}
	static class PackageInteractionCmdIDList {
		public const int cmdidDesignToPackage = 0x100;
		public const int cmdidPackageToDesign = 0x101;
		public static readonly CommandID DesignToPackage = new CommandID(SharedGuidList.guidPackageInteractionCmdSet, cmdidDesignToPackage);
		public static readonly CommandID PackageToDesign = new CommandID(SharedGuidList.guidPackageInteractionCmdSet, cmdidPackageToDesign);
	}
	public enum InteractionAction {
		FillComboBoxes = 1,
		SetZoomFactorsText = 2,
		UpdateFont = 3,
		ResetFont = 4,
		SetFontControlsVisibility = 5,
		SetCommandBarEnabled = 6,
		ZoomIn = 101,
		ZoomOut = 102,
		ZoomChanged = 103,
		FontNameChanged = 104,
		FontSizeChanged = 105,
	}
}
