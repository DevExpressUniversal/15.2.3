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
namespace DevExpress.XtraSpreadsheet.Model {
	#region VmlShapeLayout
	public class VmlShapeLayout {
		#region Fields
		VmlShapeIdMap idmap; 
		VmlExtensionHandlingBehavior extensionHandlingBehavior;
		#endregion
		public VmlShapeLayout() {
			idmap = new VmlShapeIdMap();
			extensionHandlingBehavior = VmlExtensionHandlingBehavior.Edit;
		}
		#region Properties
		public VmlShapeIdMap Idmap { get { return idmap; } set { idmap = value; } }
		public VmlExtensionHandlingBehavior ExtensionHandlingBehavior { get { return extensionHandlingBehavior; } set { extensionHandlingBehavior = value; } }
		#endregion
	}
	#endregion
	#region VmlShapeIdMap
	public class VmlShapeIdMap {
		#region Fields
		string data;
		VmlExtensionHandlingBehavior extensionHandlingBehavior;
		#endregion
		public VmlShapeIdMap() {
			data = "1";
			extensionHandlingBehavior = VmlExtensionHandlingBehavior.Edit;
		}
		#region Properties
		public string Data { get { return data; } set { data = value; } }
		public VmlExtensionHandlingBehavior ExtensionHandlingBehavior { get { return extensionHandlingBehavior; } set { extensionHandlingBehavior = value; } }
		#endregion
	}
	#endregion
	#region VmlExtensionHandlingBehavior
	public enum VmlExtensionHandlingBehavior {
		View,
		Edit,
		BackwardCompatible
	}
	#endregion
}
