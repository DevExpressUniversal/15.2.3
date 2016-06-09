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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
namespace DevExpress.XtraMap.OpenGL {
	[SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity, SecurityCritical]
	static class GLUUnsafe {
		internal const string GLULibraryName = "glu32.dll";
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern void gluTessProperty(IntPtr tess, int which, double data);
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity, SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern void gluTessVertex(IntPtr tess, IntPtr location, IntPtr data);
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity, SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern IntPtr gluNewTess();
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity, SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern void gluDeleteTess(IntPtr tess);
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity, SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern void gluTessBeginContour(IntPtr tess);
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity, SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern void gluTessBeginPolygon(IntPtr tess, IntPtr data);
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity, SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern void gluTessCallback(IntPtr tess, int which, GLU.tessBeginCallback proc);
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity, SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern void gluTessCallback(IntPtr tess, int which, GLU.tessVertexCallback proc);
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity, SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern void gluTessCallback(IntPtr tess, int which, GLU.tessEndCallback proc);
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity, SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern void gluTessCallback(IntPtr tess, int which, GLU.tessErrorCallback proc);
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity, SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern void gluTessCallback(IntPtr tess, int which, GLU.tessCombineCallback proc);
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity, SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern void gluTessCallback(IntPtr tess, int which, GLU.tessEdgeCallback proc);
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity, SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern void gluTessEndContour(IntPtr tess);
		[DllImport(GLULibraryName, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity, SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
		internal static extern void gluTessEndPolygon(IntPtr tess);
	}
	public static class GLU {
		internal delegate void tessVertexCallback(IntPtr data);
		internal delegate void tessBeginCallback(int type);
		internal delegate void tessEndCallback();
		internal delegate void tessErrorCallback(int type);
		internal delegate void tessCombineCallback(IntPtr d1, IntPtr d2, IntPtr d3, IntPtr dataOut);
		internal delegate void tessEdgeCallback(bool val);
		[SecuritySafeCritical]
		internal static void gluTessProperty(IntPtr tess, int which, double data) {
			GLUUnsafe.gluTessProperty(tess, which, data);
		}
		[SecuritySafeCritical]
		internal static void gluTessVertex(IntPtr tess, IntPtr location, IntPtr data) {
			GLUUnsafe.gluTessVertex(tess, location, data);
		}
		[SecuritySafeCritical]
		internal static IntPtr gluNewTess() {
			return GLUUnsafe.gluNewTess();
		}
		[SecuritySafeCritical]
		internal static void gluDeleteTess(IntPtr tess) {
			GLUUnsafe.gluDeleteTess(tess);
		}
		[SecuritySafeCritical]
		internal static void gluTessBeginContour(IntPtr tess) {
			GLUUnsafe.gluTessBeginContour(tess);
		}
		[SecuritySafeCritical]
		internal static void gluTessBeginPolygon(IntPtr tess, IntPtr data) {
			GLUUnsafe.gluTessBeginPolygon(tess, data);
		}
		[SecuritySafeCritical]
		internal static void gluTessCallback(IntPtr tess, int which, tessBeginCallback proc) {
			GLUUnsafe.gluTessCallback(tess, which, proc);
		}
		[SecuritySafeCritical]
		internal static void gluTessCallback(IntPtr tess, int which, tessVertexCallback proc) {
			GLUUnsafe.gluTessCallback(tess, which, proc);
		}
		[SecuritySafeCritical]
		internal static void gluTessCallback(IntPtr tess, int which, tessEndCallback proc) {
			GLUUnsafe.gluTessCallback(tess, which, proc);
		}
		[SecuritySafeCritical]
		internal static void gluTessCallback(IntPtr tess, int which, tessErrorCallback proc) {
			GLUUnsafe.gluTessCallback(tess, which, proc);
		}
		[SecuritySafeCritical]
		internal static void gluTessCallback(IntPtr tess, int which, tessCombineCallback proc) {
			GLUUnsafe.gluTessCallback(tess, which, proc);
		}
		[SecuritySafeCritical]
		internal static void gluTessCallback(IntPtr tess, int which, tessEdgeCallback proc) {
			GLUUnsafe.gluTessCallback(tess, which, proc);
		}
		[SecuritySafeCritical]
		internal static void gluTessEndContour(IntPtr tess) {
			GLUUnsafe.gluTessEndContour(tess);
		}
		[SecuritySafeCritical]
		internal static void gluTessEndPolygon(IntPtr tess) {
			GLUUnsafe.gluTessEndPolygon(tess);
		}
		#region Constants
		public const int GLU_EXT_object_space_tess = 1;
		public const int GLU_EXT_nurbs_tessellator = 1;
		public const int GLU_FALSE = 0;
		public const int GLU_TRUE = 1;
		public const int GLU_VERSION_1_1 = 1;
		public const int GLU_VERSION_1_2 = 1;
		public const int GLU_VERSION = 100800;
		public const int GLU_EXTENSIONS = 100801;
		public const int GLU_INVALID_ENUM = 100900;
		public const int GLU_INVALID_VALUE = 100901;
		public const int GLU_OUT_OF_MEMORY = 100902;
		public const int GLU_INCOMPATIBLE_GL_VERSION = 100903;
		public const int GLU_INVALID_OPERATION = 100904;
		public const int GLU_OUTLINE_POLYGON = 100240;
		public const int GLU_OUTLINE_PATCH = 100241;
		public const int GLU_ERROR = 100103;
		public const int GLU_NURBS_ERROR1 = 100251;
		public const int GLU_NURBS_ERROR2 = 100252;
		public const int GLU_NURBS_ERROR3 = 100253;
		public const int GLU_NURBS_ERROR4 = 100254;
		public const int GLU_NURBS_ERROR5 = 100255;
		public const int GLU_NURBS_ERROR6 = 100256;
		public const int GLU_NURBS_ERROR7 = 100257;
		public const int GLU_NURBS_ERROR8 = 100258;
		public const int GLU_NURBS_ERROR9 = 100259;
		public const int GLU_NURBS_ERROR10 = 100260;
		public const int GLU_NURBS_ERROR11 = 100261;
		public const int GLU_NURBS_ERROR12 = 100262;
		public const int GLU_NURBS_ERROR13 = 100263;
		public const int GLU_NURBS_ERROR14 = 100264;
		public const int GLU_NURBS_ERROR15 = 100265;
		public const int GLU_NURBS_ERROR16 = 100266;
		public const int GLU_NURBS_ERROR17 = 100267;
		public const int GLU_NURBS_ERROR18 = 100268;
		public const int GLU_NURBS_ERROR19 = 100269;
		public const int GLU_NURBS_ERROR20 = 100270;
		public const int GLU_NURBS_ERROR21 = 100271;
		public const int GLU_NURBS_ERROR22 = 100272;
		public const int GLU_NURBS_ERROR23 = 100273;
		public const int GLU_NURBS_ERROR24 = 100274;
		public const int GLU_NURBS_ERROR25 = 100275;
		public const int GLU_NURBS_ERROR26 = 100276;
		public const int GLU_NURBS_ERROR27 = 100277;
		public const int GLU_NURBS_ERROR28 = 100278;
		public const int GLU_NURBS_ERROR29 = 100279;
		public const int GLU_NURBS_ERROR30 = 100280;
		public const int GLU_NURBS_ERROR31 = 100281;
		public const int GLU_NURBS_ERROR32 = 100282;
		public const int GLU_NURBS_ERROR33 = 100283;
		public const int GLU_NURBS_ERROR34 = 100284;
		public const int GLU_NURBS_ERROR35 = 100285;
		public const int GLU_NURBS_ERROR36 = 100286;
		public const int GLU_NURBS_ERROR37 = 100287;
		public const int GLU_AUTO_LOAD_MATRIX = 100200;
		public const int GLU_CULLING = 100201;
		public const int GLU_SAMPLING_TOLERANCE = 100203;
		public const int GLU_DISPLAY_MODE = 100204;
		public const int GLU_PARAMETRIC_TOLERANCE = 100202;
		public const int GLU_SAMPLING_METHOD = 100205;
		public const int GLU_U_STEP = 100206;
		public const int GLU_V_STEP = 100207;
		public const int GLU_OBJECT_PARAMETRIC_ERROR_EXT = 100208;
		public const int GLU_OBJECT_PATH_LENGTH_EXT = 100209;
		public const int GLU_PATH_LENGTH = 100215;
		public const int GLU_PARAMETRIC_ERROR = 100216;
		public const int GLU_DOMAIN_DISTANCE = 100217;
		public const int GLU_MAP1_TRIM_2 = 100210;
		public const int GLU_MAP1_TRIM_3 = 100211;
		public const int GLU_POINT = 100010;
		public const int GLU_LINE = 100011;
		public const int GLU_FILL = 100012;
		public const int GLU_SILHOUETTE = 100013;
		public const int GLU_SMOOTH = 100000;
		public const int GLU_FLAT = 100001;
		public const int GLU_NONE = 100002;
		public const int GLU_OUTSIDE = 100020;
		public const int GLU_INSIDE = 100021;
		public const int GLU_TESS_BEGIN = 100100;
		public const int GLU_BEGIN = 100100;
		public const int GLU_TESS_VERTEX = 100101;
		public const int GLU_VERTEX = 100101;
		public const int GLU_TESS_END = 100102;
		public const int GLU_END = 100102;
		public const int GLU_TESS_ERROR = 100103;
		public const int GLU_TESS_EDGE_FLAG = 100104;
		public const int GLU_EDGE_FLAG = 100104;
		public const int GLU_TESS_COMBINE = 100105;
		public const int GLU_TESS_BEGIN_DATA = 100106;
		public const int GLU_TESS_VERTEX_DATA = 100107;
		public const int GLU_TESS_END_DATA = 100108;
		public const int GLU_TESS_ERROR_DATA = 100109;
		public const int GLU_TESS_EDGE_FLAG_DATA = 100110;
		public const int GLU_TESS_COMBINE_DATA = 100111;
		public const int GLU_NURBS_MODE_EXT = 100160;
		public const int GLU_NURBS_TESSELLATOR_EXT = 100161;
		public const int GLU_NURBS_RENDERER_EXT = 100162;
		public const int GLU_NURBS_BEGIN_EXT = 100164;
		public const int GLU_NURBS_VERTEX_EXT = 100165;
		public const int GLU_NURBS_NORMAL_EXT = 100166;
		public const int GLU_NURBS_COLOR_EXT = 100167;
		public const int GLU_NURBS_TEX_COORD_EXT = 100168;
		public const int GLU_NURBS_END_EXT = 100169;
		public const int GLU_NURBS_BEGIN_DATA_EXT = 100170;
		public const int GLU_NURBS_VERTEX_DATA_EXT = 100171;
		public const int GLU_NURBS_NORMAL_DATA_EXT = 100172;
		public const int GLU_NURBS_COLOR_DATA_EXT = 100173;
		public const int GLU_NURBS_TEX_COORD_DATA_EXT = 100174;
		public const int GLU_NURBS_END_DATA_EXT = 100175;
		public const int GLU_CW = 100120;
		public const int GLU_CCW = 100121;
		public const int GLU_INTERIOR = 100122;
		public const int GLU_EXTERIOR = 100123;
		public const int GLU_UNKNOWN = 100124;
		public const int GLU_TESS_WINDING_RULE = 100140;
		public const int GLU_TESS_BOUNDARY_ONLY = 100141;
		public const int GLU_TESS_TOLERANCE = 100142;
		public const int GLU_TESS_ERROR1 = 100151;
		public const int GLU_TESS_ERROR2 = 100152;
		public const int GLU_TESS_ERROR3 = 100153;
		public const int GLU_TESS_ERROR4 = 100154;
		public const int GLU_TESS_ERROR5 = 100155;
		public const int GLU_TESS_ERROR6 = 100156;
		public const int GLU_TESS_ERROR7 = 100157;
		public const int GLU_TESS_ERROR8 = 100158;
		public const int GLU_TESS_MISSING_BEGIN_POLYGON = 100151;
		public const int GLU_TESS_MISSING_BEGIN_CONTOUR = 100152;
		public const int GLU_TESS_MISSING_END_POLYGON = 100153;
		public const int GLU_TESS_MISSING_END_CONTOUR = 100154;
		public const int GLU_TESS_COORD_TOO_LARGE = 100155;
		public const int GLU_TESS_NEED_COMBINE_CALLBACK = 100156;
		public const int GLU_TESS_WINDING_ODD = 100130;
		public const int GLU_TESS_WINDING_NONZERO = 100131;
		public const int GLU_TESS_WINDING_POSITIVE = 100132;
		public const int GLU_TESS_WINDING_NEGATIVE = 100133;
		public const int GLU_TESS_WINDING_ABS_GEQ_TWO = 100134;
		public const double GLU_TESS_MAX_COORD = 1.0e150;
		#endregion Constants
	}
}
