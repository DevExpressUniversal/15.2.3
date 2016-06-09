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
using System.Reflection;
[assembly: AssemblyTitle("DevExpress.Workflow.Activities")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("eXpressApp Framework")]
[assembly: System.Security.SecurityRules(System.Security.SecurityRuleSet.Level1)]
public class ActivitiesAssemblyInfo {
	[Obsolete("Use the 'TransactionalActivitiesTabName' constant instead.")]
	public const string TransactionalActivititesTabName = "Workflow Transactional Activities";
	public const string TransactionalActivitiesTabName = "Workflow Transactional Activities";
	[Obsolete("Use the 'DXTransactionalActivitiesTabName' constant instead.")]
	public const string DXTransactionalActivititesTabName = "DX." + AssemblyInfo.VersionShort + ": " + TransactionalActivitiesTabName;
	public const string DXTransactionalActivitiesTabName = "DX." + AssemblyInfo.VersionShort + ": " + TransactionalActivitiesTabName;
	[Obsolete("Use the 'DXActivitiesTabName' constant instead.")]
	public const string DXActivititesTabName = "DX." + AssemblyInfo.VersionShort + ": Workflow Activities";
	public const string DXActivitiesTabName = "DX." + AssemblyInfo.VersionShort + ": Workflow Activities";
}
