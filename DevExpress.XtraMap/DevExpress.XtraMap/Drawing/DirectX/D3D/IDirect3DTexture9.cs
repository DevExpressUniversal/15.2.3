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

using System.Runtime.InteropServices;
using System;
using System.Diagnostics.CodeAnalysis;
namespace D3D {
	[ComImport, Guid("85C31227-3DE5-4f00-9B3A-F11AC38C18B5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[CLSCompliant(false)]
	public interface IDirect3DTexture9 {
		[return: MarshalAs(UnmanagedType.Interface)]
		IDirect3DDevice9 GetDevice();
		void SetPrivateData_Placeholder();
		void GetPrivateData_Placeholder();
		void FreePrivateData_Placeholder();
		void SetPriority_Placeholder();
		void GetPriority_Placeholder();
		void PreLoad_Placeholder();
		void GetType_Placeholder();
		void SetLOD_Placeholder();
		void GetLOD_Placeholder();
		void GetLevelCount_Placeholder();
		void SetAutoGenFilterType_Placeholder();
		void GetAutoGenFilterType_Placeholder();
		void GenerateMipSubLevels_Placeholder();
		void GetLevelDesc_Placeholder();
		[PreserveSig]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		int GetSurfaceLevel(uint Level, out IDirect3DSurface9 pSurfaceLevel);
		[PreserveSig]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
		int LockRect(uint Level, ref D3DLOCKED_RECT pLockedRect, ref RECT pRect, uint Flags);
		int UnlockRect(uint Level);
		void AddDirtyRect_Placeholder();  
	}
}
