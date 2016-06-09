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
using System.Drawing;
using System.Security;
using System.Runtime.InteropServices;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.GLGraphics {
	[CLSCompliant(false), SuppressUnmanagedCodeSecurity]
	public class WGLImport {
		[DllImport("gdi32.dll")]
		public static extern int DescribePixelFormat(IntPtr hdc, int iPixelFormat, int nBytes, [In, Out] WGL.PIXELFORMATDESCRIPTOR ppfd);
		[DllImport("gdi32.dll")]
		public static extern int ChoosePixelFormat(IntPtr hdc, [In] WGL.PIXELFORMATDESCRIPTOR ppfd);
		[DllImport("gdi32.dll")]
		public static extern bool SetPixelFormat(IntPtr hdc, int iPixelFormat, WGL.PIXELFORMATDESCRIPTOR ppfd);
		[DllImport("gdi32.dll")]
		public static extern bool SwapBuffers(IntPtr hdc);
		[DllImport("opengl32.dll", EntryPoint = "wglCreateContext")]
		public static extern IntPtr CreateContext(IntPtr hdc);
		[DllImport("opengl32.dll", EntryPoint = "wglDeleteContext")]
		public static extern bool DeleteContext(IntPtr hglrc);
		[DllImport("opengl32.dll", EntryPoint = "wglMakeCurrent")]
		public static extern bool MakeCurrent(IntPtr hdc, IntPtr hglrc);
	}
	[CLSCompliant(false)]
	public class WGL {
		public const uint PFD_DOUBLEBUFFER = 0x00000001;
		public const uint PFD_STEREO = 0x00000002;
		public const uint PFD_DRAW_TO_WINDOW = 0x00000004;
		public const uint PFD_DRAW_TO_BITMAP = 0x00000008;
		public const uint PFD_SUPPORT_GDI = 0x00000010;
		public const uint PFD_SUPPORT_OPENGL = 0x00000020;
		public const uint PFD_GENERIC_FORMAT = 0x00000040;
		public const uint PFD_NEED_PALETTE = 0x00000080;
		public const uint PFD_NEED_SYSTEM_PALETTE = 0x00000100;
		public const uint PFD_SWAP_EXCHANGE = 0x00000200;
		public const uint PFD_SWAP_COPY = 0x00000400;
		public const uint PFD_SWAP_LAYER_BUFFERS = 0x00000800;
		public const uint PFD_GENERIC_ACCELERATED = 0x00001000;
		public const uint PFD_SUPPORT_DIRECTDRAW = 0x00002000;
		public const uint PFD_DIRECT3D_ACCELERATED = 0x00004000;
		public const uint PFD_SUPPORT_COMPOSITION = 0x00008000;
		public const uint PFD_DEPTH_DONTCARE = 0x20000000;
		public const uint PFD_DOUBLEBUFFER_DONTCARE = 0x40000000;
		public const uint PFD_STEREO_DONTCARE = 0x80000000;
		public const byte PFD_TYPE_RGBA = 0;
		public const byte PFD_TYPE_COLORINDEX = 1;
		public const byte PFD_MAIN_PLANE = 0;
		public const byte PFD_OVERLAY_PLANE = 1;
		public const byte PFD_UNDERLAY_PLANE = 255;
		[StructLayout(LayoutKind.Sequential)]
		public class PIXELFORMATDESCRIPTOR {
			public short nSize;
			public short nVersion;
			public uint dwFlags;
			public byte iPixelType;
			public byte cColorBits;
			public byte cRedBits;
			public byte cRedShift;
			public byte cGreenBits;
			public byte cGreenShift;
			public byte cBlueBits;
			public byte cBlueShift;
			public byte cAlphaBits;
			public byte cAlphaShift;
			public byte cAccumBits;
			public byte cAccumRedBits;
			public byte cAccumGreenBits;
			public byte cAccumBlueBits;
			public byte cAccumAlphaBits;
			public byte cDepthBits;
			public byte cStencilBits;
			public byte cAuxBuffers;
			public byte iLayerType;
			public byte bReserved;
			public int dwLayerMask;
			public int dwVisibleMask;
			public int dwDamageMask;
			public PIXELFORMATDESCRIPTOR() {
				nSize = (short)Marshal.SizeOf(this);
				nVersion = 1;
				iPixelType = WGL.PFD_TYPE_RGBA;
				cColorBits = 24;
				cDepthBits = 32;
				iLayerType = WGL.PFD_MAIN_PLANE;
			}
		}
		[SecuritySafeCritical]
		public static int DescribePixelFormat(IntPtr hdc, int iPixelFormat, int nBytes, PIXELFORMATDESCRIPTOR ppfd) { 
			return WGLImport.DescribePixelFormat(hdc, iPixelFormat, nBytes, ppfd);
		}
		[SecuritySafeCritical]
		public static int ChoosePixelFormat(IntPtr hdc, PIXELFORMATDESCRIPTOR ppfd) {
			return WGLImport.ChoosePixelFormat(hdc, ppfd);
		}
		[SecuritySafeCritical]
		public static bool SetPixelFormat(IntPtr hdc, int iPixelFormat, PIXELFORMATDESCRIPTOR ppfd) {
			return WGLImport.SetPixelFormat(hdc, iPixelFormat, ppfd);
		}
		[SecuritySafeCritical]
		public static bool SwapBuffers(IntPtr hdc) {
			return WGLImport.SwapBuffers(hdc);
		}
		[SecuritySafeCritical]
		public static IntPtr CreateContext(IntPtr hdc) {
			return WGLImport.CreateContext(hdc);
		}
		[SecuritySafeCritical]
		public static bool DeleteContext(IntPtr hglrc) {
			return WGLImport. DeleteContext(hglrc);
		}
		[SecuritySafeCritical]
		public static bool MakeCurrent(IntPtr hdc, IntPtr hglrc) {
			return WGLImport.MakeCurrent(hdc, hglrc);
		}
	}
	[CLSCompliant(false), SuppressUnmanagedCodeSecurity]
	public class GLImport {
		[DllImport("opengl32.dll", EntryPoint = "glFinish")]
		public static extern void Finish();
		[DllImport("opengl32.dll", EntryPoint = "glEnable")]
		public static extern void Enable(int cap);
		[DllImport("opengl32.dll", EntryPoint = "glDisable")]
		public static extern void Disable(int cap);
		[DllImport("opengl32.dll", EntryPoint = "glGetIntegerv")]
		public static extern void GetIntegerv(int pname, [Out] int[] param);
		[DllImport("opengl32.dll", EntryPoint = "glGetDoublev")]
		public static extern void GetDoublev(int pname, [Out] double[] param);
		[DllImport("opengl32.dll", EntryPoint = "glClearColor")]
		public static extern void ClearColor(float red, float green, float blue, float alpha);
		[DllImport("opengl32.dll", EntryPoint = "glDepthFunc")]
		public static extern void DepthFunc(int func);
		[DllImport("opengl32.dll", EntryPoint = "glClearDepth")]
		public static extern void ClearDepth(float depth);
		[DllImport("opengl32.dll", EntryPoint = "glClearStencil")]
		public static extern void ClearStencil(int s);
		[DllImport("opengl32.dll", EntryPoint = "glClear")]
		public static extern void Clear(int mask);
		[DllImport("opengl32.dll", EntryPoint = "glViewport")]
		public static extern void Viewport(int x, int y, int width, int height);
		[DllImport("opengl32.dll", EntryPoint = "glMatrixMode")]
		public static extern void MatrixMode(int mode);
		[DllImport("opengl32.dll", EntryPoint = "glPushMatrix")]
		public static extern void PushMatrix();
		[DllImport("opengl32.dll", EntryPoint = "glPopMatrix")]
		public static extern void PopMatrix();
		[DllImport("opengl32.dll", EntryPoint = "glLoadIdentity")]
		public static extern void LoadIdentity();
		[DllImport("opengl32.dll", EntryPoint = "glLoadMatrixd")]
		public static extern void LoadMatrixd([In] double[] m);
		[DllImport("opengl32.dll", EntryPoint = "glMultMatrixd")]
		public static extern void MultMatrixd([In] double[] m);
		[DllImport("opengl32.dll", EntryPoint = "glTranslated")]
		public static extern void Translated(double x, double y, double z);
		[DllImport("opengl32.dll", EntryPoint = "glRotated")]
		public static extern void Rotated(double angle, double x, double y, double z);
		[DllImport("opengl32.dll", EntryPoint = "glScaled")]
		public static extern void Scaled(double x, double y, double z);
		[DllImport("opengl32.dll", EntryPoint = "glOrtho")]
		public static extern void Ortho(double left, double right, double bottom, double top, double zNear, double zFar);
		[DllImport("opengl32.dll", EntryPoint = "glFrustum")]
		public static extern void Frustum(double left, double right, double bottom, double top, double zNear, double zFar);
		[DllImport("opengl32.dll", EntryPoint = "glBegin")]
		public static extern void Begin(int mode);
		[DllImport("opengl32.dll", EntryPoint = "glEnd")]
		public static extern void End();
		[DllImport("opengl32.dll", EntryPoint = "glColor4f")]
		public static extern void Color4f(float read, float green, float blue, float alpha);
		[DllImport("opengl32.dll", EntryPoint = "glColor4b")]
		public static extern void Color4b(byte read, byte green, byte blue, byte alpha);
		[DllImport("opengl32.dll", EntryPoint = "glColor4ub")]
		public static extern void Color4ub(byte read, byte green, byte blue, byte alpha);
		[DllImport("opengl32.dll", EntryPoint = "glVertex3d")]
		public static extern void Vertex3d(double x, double y, double z);
		[DllImport("opengl32.dll", EntryPoint = "glNormal3f")]
		public static extern void Normal3f(float nx, float ny, float nz);
		[DllImport("opengl32.dll", EntryPoint = "glNormal3d")]
		public static extern void Normal3d(double nx, double ny, double nz);
		[DllImport("opengl32.dll", EntryPoint = "glEdgeFlag")]
		public static extern void EdgeFlag(int flag);
		[DllImport("opengl32.dll", EntryPoint = "glLineStipple")]
		public static extern void LineStipple(int factor, ushort pattern);
		[DllImport("opengl32.dll", EntryPoint = "glLineWidth")]
		public static extern void LineWidth(float width);
		[DllImport("opengl32.dll", EntryPoint = "glPointSize")]
		public static extern void PointSize(float size);
		[DllImport("opengl32.dll", EntryPoint = "glClipPlane")]
		public static extern void ClipPlane(int plane, [In] double[] equation);
		[DllImport("opengl32.dll", EntryPoint = "glShadeModel")]
		public static extern void ShadeModel(int mode);
		[DllImport("opengl32.dll", EntryPoint = "glBlendFunc")]
		public static extern void BlendFunc(int sfactor, int dfactor);
		[DllImport("opengl32.dll", EntryPoint = "glLightModeli")]
		public static extern void LightModeli(int pname, int param);
		[DllImport("opengl32.dll", EntryPoint = "glLightModelfv")]
		public static extern void LightModelfv(int pname, float[] param);
		[DllImport("opengl32.dll", EntryPoint = "glLightf")]
		public static extern void Lightf(int light, int pname, float param);
		[DllImport("opengl32.dll", EntryPoint = "glLightfv")]
		public static extern void Lightfv(int light, int pname, [In] float[] param);
		[DllImport("opengl32.dll", EntryPoint = "glColorMaterial")]
		public static extern void ColorMaterial(int face, int mode);
		[DllImport("opengl32.dll", EntryPoint = "glMaterialf")]
		public static extern void Materialf(int face, int pname, float param);
		[DllImport("opengl32.dll", EntryPoint = "glMaterialfv")]
		public static extern void Materialfv(int face, int pname, [In] float[] param);
		[DllImport("opengl32.dll", EntryPoint = "glPixelStorei")]
		public static extern void PixelStorei(int pname, int param);
		[DllImport("opengl32.dll", EntryPoint = "glTexImage1D")]
		public static extern void TexImage1D(int target, int level, int internalformat, int width, int border, int format, int type, [In] float[] pixels);
		[DllImport("opengl32.dll", EntryPoint = "glTexImage2D")]
		public static extern void TexImage2D(int target, int level, int internalformat, int width, int height, int border, int format, int type, byte[] pixels);
		[DllImport("opengl32.dll", EntryPoint = "glTexImage2D")]
		public static extern void TexImage2D(int target, int level, int internalformat, int width, int height, int border, int format, int type, IntPtr pixels);
		[DllImport("opengl32.dll", EntryPoint = "glTexCoord1f")]
		public static extern void TexCoord1f(float f);
		[DllImport("opengl32.dll", EntryPoint = "glTexCoord2f")]
		public static extern void TexCoord2f(float s, float t);
		[DllImport("opengl32.dll", EntryPoint = "glTexCoord2d")]
		public static extern void TexCoord2d(double s, double t);
		[DllImport("opengl32.dll", EntryPoint = "glGenTextures")]
		public static extern void GenTextures(int n, [Out] uint[] textures);
		[DllImport("opengl32.dll", EntryPoint = "glDeleteTextures")]
		public static extern void DeleteTextures(int n, [Out] uint[] textures);
		[DllImport("opengl32.dll", EntryPoint = "glBindTexture")]
		public static extern void BindTexture(int target, uint texture);
		[DllImport("opengl32.dll", EntryPoint = "glTexParameteri")]
		public static extern void TexParameteri(int target, int pname, int param);
		[DllImport("opengl32.dll", EntryPoint = "glTexEnvf")]
		public static extern void TexEnvf(int target, int pname, float param);
		[DllImport("opengl32.dll", EntryPoint = "glHint")]
		public static extern void Hint(int target, int mode);
		[DllImport("opengl32.dll", EntryPoint = "glStencilOp")]
		public static extern void StencilOp(int fail, int zfail, int zpass);
		[DllImport("opengl32.dll", EntryPoint = "glStencilFunc")]
		public static extern void StencilFunc(int func, int refer, uint mask);
		[DllImport("opengl32.dll", EntryPoint = "glAccum")]
		public static extern void Accum(int operation, float val);
		[DllImport("opengl32.dll", EntryPoint = "glClearAccum")]
		public static extern void ClearAccum(float red, float green, float blue, float alpha);
		[DllImport("opengl32.dll", EntryPoint = "glReadPixels")]
		public static extern void ReadPixels(int x, int y, int width, int height, int format, int type, [Out] byte[] pixels);
		[DllImport("opengl32.dll", EntryPoint = "glReadPixels")]
		public static extern void ReadPixels(int x, int y, int width, int height, int format, int type, [Out] IntPtr pixels);
		[DllImport("opengl32.dll", EntryPoint = "glDrawPixels")]
		public static extern void DrawPixels(int width, int height, int format, int type, [In] byte[] pixels);
		[DllImport("opengl32.dll", EntryPoint = "glRasterPos2i")]
		public static extern void RasterPosi(int x, int y);
		[DllImport("opengl32.dll", EntryPoint = "glReadBuffer")]
		public static extern void ReadBuffer(int mode);
		[DllImport("opengl32.dll", EntryPoint = "glDrawBuffer")]
		public static extern void DrawBuffer(int mode);
		[DllImport("opengl32.dll", EntryPoint = "glPolygonOffset")]
		public static extern void PolygonOffset(float factor, float units);
		[DllImport("opengl32.dll", EntryPoint = "glCullFace")]
		public static extern void CullFace(int mode);
		[DllImport("opengl32.dll", EntryPoint = "glColorMask")]
		public static extern void ColorMask(bool red, bool green, bool blue, bool alpha);
		[DllImport("opengl32.dll", EntryPoint = "glDepthMask")]
		public static extern void DepthMask(bool flag);
		[DllImport("opengl32.dll", EntryPoint = "glGetBooleanv")]
		public static extern void GetBooleanv(int pname, [Out] bool[] pars);
		[DllImport("opengl32.dll", EntryPoint = "glGetError")]
		public static extern int GetError();
	}
	[CLSCompliant(false)]
	public class GL {
		public const int BYTE = 0x1400;
		public const int UNSIGNED_BYTE = 0x1401;
		public const int SHORT = 0x1402;
		public const int UNSIGNED_SHORT = 0x1403;
		public const int INT = 0x1404;
		public const int UNSIGNED_INT = 0x1405;
		public const int FLOAT = 0x1406;
		public const int GL_2_BYTES = 0x1407;
		public const int GL_3_BYTES = 0x1408;
		public const int GL_4_BYTES = 0x1409;
		public const int DOUBLE = 0x140A;
		public const int ZERO = 0;
		public const int ONE = 1;
		public const int TRUE = 1;
		public const int FALSE = 0;
		public const int NEVER = 0x0200;
		public const int LESS = 0x0201;
		public const int EQUAL = 0x0202;
		public const int LEQUAL = 0x0203;
		public const int GREATER = 0x0204;
		public const int NOTEQUAL = 0x0205;
		public const int GEQUAL = 0x0206;
		public const int ALWAYS = 0x0207;
		public const int LIGHTING = 0x0B50;
		public const int SHADE_MODEL = 0x0B54;
		public const int COLOR_MATERIAL = 0x0B57;
		public const int DEPTH_TEST = 0x0B71;
		public const int NORMALIZE = 0x0BA1;
		public const int VIEWPORT = 0x0BA2;
		public const int MODELVIEW_MATRIX = 0x0BA6;
		public const int PROJECTION_MATRIX = 0x0BA7;
		public const int BLEND = 0x0BE2;
		public const int STENCIL_TEST = 0x0B90;
		public const int DEPTH_BUFFER_BIT = 0x00000100;
		public const int ACCUM_BUFFER_BIT = 0x00000200;
		public const int STENCIL_BUFFER_BIT = 0x00000400;
		public const int COLOR_BUFFER_BIT = 0x00004000;
		public const int MODELVIEW = 0x1700;
		public const int PROJECTION = 0x1701;
		public const int TEXTURE = 0x1702;
		public const int POINTS = 0x0000;
		public const int LINES = 0x0001;
		public const int LINE_LOOP = 0x0002;
		public const int LINE_STRIP = 0x0003;
		public const int TRIANGLES = 0x0004;
		public const int TRIANGLE_STRIP = 0x0005;
		public const int TRIANGLE_FAN = 0x0006;
		public const int QUADS = 0x0007;
		public const int QUAD_STRIP = 0x0008;
		public const int POLYGON = 0x0009;
		public const int CLIP_PLANE0 = 0x3000;
		public const int CLIP_PLANE1 = 0x3001;
		public const int CLIP_PLANE2 = 0x3002;
		public const int CLIP_PLANE3 = 0x3003;
		public const int CLIP_PLANE4 = 0x3004;
		public const int CLIP_PLANE5 = 0x3005;
		public const int FLAT = 0x1D00;
		public const int SMOOTH = 0x1D01;
		public const int LIGHT_MODEL_LOCAL_VIEWER = 0x0B51;
		public const int LIGHT_MODEL_TWO_SIDE = 0x0B52;
		public const int LIGHT_MODEL_AMBIENT = 0x0B53;
		public const int LIGHT0 = 0x4000;
		public const int LIGHT1 = 0x4001;
		public const int LIGHT2 = 0x4002;
		public const int LIGHT3 = 0x4003;
		public const int LIGHT4 = 0x4004;
		public const int LIGHT5 = 0x4005;
		public const int LIGHT6 = 0x4006;
		public const int LIGHT7 = 0x4007;
		public const int AMBIENT = 0x1200;
		public const int DIFFUSE = 0x1201;
		public const int SPECULAR = 0x1202;
		public const int POSITION = 0x1203;
		public const int SPOT_DIRECTION = 0x1204;
		public const int SPOT_EXPONENT = 0x1205;
		public const int SPOT_CUTOFF = 0x1206;
		public const int CONSTANT_ATTENUATION = 0x1207;
		public const int LINEAR_ATTENUATION = 0x1208;
		public const int QUADRATIC_ATTENUATION = 0x1209;
		public const int SRC_COLOR = 0x0300;
		public const int ONE_MINUS_SRC_COLOR = 0x0301;
		public const int SRC_ALPHA = 0x0302;
		public const int ONE_MINUS_SRC_ALPHA = 0x0303;
		public const int DST_ALPHA = 0x0304;
		public const int ONE_MINUS_DST_ALPHA = 0x0305;
		public const int DST_COLOR = 0x0306;
		public const int ONE_MINUS_DST_COLOR = 0x0307;
		public const int SRC_ALPHA_SATURATE = 0x0308;
		public const int FRONT = 0x0404;
		public const int BACK = 0x0405;
		public const int FRONT_AND_BACK = 0x0408;
		public const int EMISSION = 0x1600;
		public const int SHININESS = 0x1601;
		public const int AMBIENT_AND_DIFFUSE = 0x1602;
		public const int COLOR_INDEXES = 0x1603;
		public const int TEXTURE_1D = 0x0DE0;
		public const int TEXTURE_2D = 0x0DE1;
		public const int UNPACK_ALIGNMENT = 0x0CF5;
		public const int PACK_ALIGNMENT = 0x0D05;
		public const int TEXTURE_MAG_FILTER = 0x2800;
		public const int TEXTURE_MIN_FILTER = 0x2801;
		public const int TEXTURE_WRAP_S = 0x2802;
		public const int TEXTURE_WRAP_T = 0x2803;
		public const int CLAMP = 0x2900;
		public const int REPEAT = 0x2901;
		public const int NEAREST = 0x2600;
		public const int LINEAR = 0x2601;
		public const int TEXTURE_ENV_MODE = 0x2200;
		public const int TEXTURE_ENV_COLOR = 0x2201;
		public const int TEXTURE_ENV = 0x2300;
		public const int MODULATE = 0x2100;
		public const int DECAL = 0x2101;
		public const int POINT_SMOOTH = 0x0B10;
		public const int LINE_SMOOTH = 0x0B20;
		public const int POLYGON_SMOOTH = 0x0B41;
		public const int PERSPECTIVE_CORRECTION_HINT = 0x0C50;
		public const int POINT_SMOOTH_HINT = 0x0C51;
		public const int LINE_SMOOTH_HINT = 0x0C52;
		public const int POLYGON_SMOOTH_HINT = 0x0C53;
		public const int FOG_HINT = 0x0C54;
		public const int DONT_CARE = 0x1100;
		public const int FASTEST = 0x1101;
		public const int NICEST = 0x1102;
		public const int COLOR_INDEX = 0x1900;
		public const int STENCIL_INDEX = 0x1901;
		public const int DEPTH_COMPONENT = 0x1902;
		public const int RED = 0x1903;
		public const int GREEN = 0x1904;
		public const int BLUE = 0x1905;
		public const int ALPHA = 0x1906;
		public const int RGB = 0x1907;
		public const int RGBA = 0x1908;
		public const int LUMINANCE = 0x1909;
		public const int LUMINANCE_ALPHA = 0x190A;
		public const int BGR_EXT = 0x80E0;
		public const int BGRA_EXT = 0x80E1;
		public const int KEEP = 0x1E00;
		public const int REPLACE = 0x1E01;
		public const int INCR = 0x1E02;
		public const int DECR = 0x1E03;
		public const int ACCUM = 0x0100;
		public const int LOAD = 0x0101;
		public const int RETURN = 0x0102;
		public const int MULT = 0x0103;
		public const int ADD = 0x0104;
		public const int POLYGON_OFFSET_FACTOR = 0x8038;
		public const int POLYGON_OFFSET_UNITS = 0x2A00;
		public const int POLYGON_OFFSET_POINT = 0x2A01;
		public const int POLYGON_OFFSET_LINE = 0x2A02;
		public const int POLYGON_OFFSET_FILL = 0x8037;
		public const int CULL_FACE = 0x0B44;
		public const int LINE_STIPPLE = 0x0B24;
		public const int DEPTH_WRITEMASK = 0x0B72;
		public const int COLOR_WRITEMASK = 0x0C23;
		public const int RGBA_MODE = 0x0C31;
		public const int MAX_TEXTURE_SIZE = 0x0D33;
		public const int STENCIL_BITS = 0x0D57;
		[SecuritySafeCritical]
		public static void Finish() {
			GLImport.Finish();
		}
		[SecuritySafeCritical]
		public static void Enable(int cap) {
			GLImport.Enable(cap);
		}
		[SecuritySafeCritical]
		public static void Disable(int cap) {
			GLImport.Disable(cap);
		}
		[SecuritySafeCritical]
		public static void GetIntegerv(int pname, int[] param) {
			GLImport.GetIntegerv(pname, param);
		}
		[SecuritySafeCritical]
		public static void GetDoublev(int pname, [Out] double[] param) {
			GLImport.GetDoublev(pname, param);
		}
		[SecuritySafeCritical]
		public static void ClearColor(float red, float green, float blue, float alpha) {
			GLImport.ClearColor(red, green, blue, alpha);
		}
		[SecuritySafeCritical]
		public static void DepthFunc(int func) {
			GLImport.DepthFunc(func);
		}
		[SecuritySafeCritical]
		public static void ClearDepth(float depth) {
			GLImport.ClearDepth(depth);
		}
		[SecuritySafeCritical]
		public static void ClearStencil(int s) {
			GLImport.ClearStencil(s);
		}
		[SecuritySafeCritical]
		public static void Clear(int mask) {
			GLImport.Clear(mask);
		}
		[SecuritySafeCritical]
		public static void Viewport(int x, int y, int width, int height) {
			GLImport.Viewport(x, y, width, height);
		}
		[SecuritySafeCritical]
		public static void MatrixMode(int mode) { 
			GLImport.MatrixMode(mode); 
		}
		[SecuritySafeCritical]
		public static void PushMatrix() {
			GLImport.PushMatrix(); 
		}
		[SecuritySafeCritical]
		public static void PopMatrix() {
			GLImport.PopMatrix(); 
		}
		[SecuritySafeCritical]
		public static void LoadIdentity() {
			GLImport.LoadIdentity(); 
		}
		[SecuritySafeCritical]
		public static void LoadMatrixd([In] double[] m) { 
			GLImport.LoadMatrixd(m);
		}
		[SecuritySafeCritical]
		public static void MultMatrixd([In] double[] m) { 
			GLImport.MultMatrixd(m); 
		}
		[SecuritySafeCritical]
		public static void Translated(double x, double y, double z) { 
			GLImport.Translated(x, y, z); 
		}
		[SecuritySafeCritical]
		public static void Rotated(double angle, double x, double y, double z) { 
			GLImport.Rotated(angle, x, y, z); 
		}
		[SecuritySafeCritical]
		public static void Scaled(double x, double y, double z) { 
			GLImport.Scaled(x, y, z); 
		}
		[SecuritySafeCritical]
		public static void Ortho(double left, double right, double bottom, double top, double zNear, double zFar) { 
			GLImport.Ortho(left, right, bottom, top, zNear, zFar); 
		}
		[SecuritySafeCritical]
		public static void Frustum(double left, double right, double bottom, double top, double zNear, double zFar) { 
			GLImport.Frustum(left, right, bottom, top, zNear, zFar); 
		}
		[SecuritySafeCritical]
		public static void Begin(int mode) { 
			GLImport.Begin(mode); 
		}
		[SecuritySafeCritical]
		public static void End() {
			GLImport.End(); 
		}
		[SecuritySafeCritical]
		public static void Color4f(float read, float green, float blue, float alpha) { 
			GLImport.Color4f(read, green, blue, alpha); 
		}
		[SecuritySafeCritical]
		public static void Color4b(byte read, byte green, byte blue, byte alpha) { 
			GLImport.Color4b(read, green, blue, alpha); 
		}
		[SecuritySafeCritical]
		public static void Color4ub(byte read, byte green, byte blue, byte alpha) { 
			GLImport.Color4ub(read, green, blue, alpha); 
		}
		[SecuritySafeCritical]
		public static void Vertex3d(double x, double y, double z) {
			GLImport.Vertex3d(x, y, z); 
		}
		[SecuritySafeCritical]
		public static void Normal3f(float nx, float ny, float nz) { 
			GLImport.Normal3f(nx, ny, nz); 
		}
		[SecuritySafeCritical]
		public static void Normal3d(double nx, double ny, double nz) { 
			GLImport.Normal3d(nx, ny, nz); 
		}
		[SecuritySafeCritical]
		public static void EdgeFlag(int flag) { 
			GLImport.EdgeFlag(flag); 
		}
		[SecuritySafeCritical]
		public static void LineStipple(int factor, ushort pattern) { 
			GLImport.LineStipple(factor, pattern); 
		}
		[SecuritySafeCritical]
		public static void LineWidth(float width) { 
			GLImport.LineWidth(width); 
		}
		[SecuritySafeCritical]
		public static void PointSize(float size) { 
			GLImport.PointSize(size) ; 
		}
		[SecuritySafeCritical]
		public static void ClipPlane(int plane, double[] equation) { 
			GLImport.ClipPlane(plane, equation); 
		}
		[SecuritySafeCritical]
		public static void ShadeModel(int mode) { 
			GLImport.ShadeModel(mode); 
		}
		[SecuritySafeCritical]
		public static void BlendFunc(int sfactor, int dfactor) { 
			GLImport.BlendFunc(sfactor, dfactor); 
		}
		[SecuritySafeCritical]
		public static void LightModeli(int pname, int param) { 
			GLImport.LightModeli(pname, param); 
		}
		[SecuritySafeCritical]
		public static void LightModelfv(int pname, float[] param) {
			GLImport.LightModelfv(pname, param); 
		}
		[SecuritySafeCritical]
		public static void Lightf(int light, int pname, float param) { 
			GLImport.Lightf(light, pname, param); 
		}
		[SecuritySafeCritical]
		public static void Lightfv(int light, int pname, float[] param) { 
			GLImport.Lightfv(light, pname, param); 
		}
		[SecuritySafeCritical]
		public static void ColorMaterial(int face, int mode) { 
			GLImport.ColorMaterial(face, mode); 
		}
		[SecuritySafeCritical]
		public static void Materialf(int face, int pname, float param) { 
			GLImport.Materialf(face, pname, param); 
		}
		[SecuritySafeCritical]
		public static void Materialfv(int face, int pname, float[] param) { 
			GLImport.Materialfv(face, pname, param); 
		}
		[SecuritySafeCritical]
		public static void PixelStorei(int pname, int param) { 
			GLImport.PixelStorei(pname, param); 
		}
		[SecuritySafeCritical]
		public static void TexImage1D(int target, int level, int internalformat, int width, int border, int format, int type, float[] pixels) { 
			GLImport.TexImage1D(target, level, internalformat, width, border, format, type, pixels);
		}
		[SecuritySafeCritical]
		public static void TexImage2D(int target, int level, int internalformat, int width, int height, int border, int format, int type, byte[] pixels) { 
			GLImport.TexImage2D(target, level, internalformat, width, height, border, format, type, pixels); 
		}
		[SecuritySafeCritical]
		public static void TexImage2D(int target, int level, int internalformat, int width, int height, int border, int format, int type, IntPtr pixels) { 
			GLImport.TexImage2D(target, level, internalformat, width, height, border, format, type, pixels);
		}
		[SecuritySafeCritical]
		public static void TexCoord1f(float f) { 
			GLImport.TexCoord1f(f); 
		}
		[SecuritySafeCritical]
		public static void TexCoord2f(float s, float t) { 
			GLImport.TexCoord2f(s, t); 
		}
		[SecuritySafeCritical]
		public static void TexCoord2d(double s, double t) { 
			GLImport.TexCoord2d(s, t); 
		}
		[SecuritySafeCritical]
		public static void GenTextures(int n, uint[] textures) { 
			GLImport.GenTextures(n, textures); 
		}
		[SecuritySafeCritical]
		public static void DeleteTextures(int n, uint[] textures) { 
			GLImport.DeleteTextures(n, textures); 
		}
		[SecuritySafeCritical]
		public static void BindTexture(int target, uint texture) { 
			GLImport.BindTexture(target, texture); 
		}
		[SecuritySafeCritical]
		public static void TexParameteri(int target, int pname, int param) { 
			GLImport.TexParameteri(target, pname, param);
		}
		[SecuritySafeCritical]
		public static void TexEnvf(int target, int pname, float param) { 
			GLImport.TexEnvf(target, pname, param); 
		}
		[SecuritySafeCritical]
		public static void Hint(int target, int mode) { 
			GLImport.Hint(target, mode); 
		}
		[SecuritySafeCritical]
		public static void StencilOp(int fail, int zfail, int zpass) { 
			GLImport.StencilOp(fail, zfail, zpass); 
		}
		[SecuritySafeCritical]
		public static void StencilFunc(int func, int refer, uint mask) { 
			GLImport.StencilFunc(func, refer, mask); 
		}
		[SecuritySafeCritical]
		public static void Accum(int operation, float val) { 
			GLImport.Accum(operation, val); 
		}
		[SecuritySafeCritical]
		public static void ClearAccum(float red, float green, float blue, float alpha) { 
			GLImport.ClearAccum(red, green, blue, alpha); 
		}
		[SecuritySafeCritical]
		public static void ReadPixels(int x, int y, int width, int height, int format, int type, byte[] pixels) { 
			GLImport.ReadPixels(x, y, width, height, format, type, pixels); 
		}
		[SecuritySafeCritical]
		public static void ReadPixels(int x, int y, int width, int height, int format, int type, IntPtr pixels) { 
			GLImport.ReadPixels(x, y, width, height, format, type, pixels); 
		}
		[SecuritySafeCritical]
		public static void DrawPixels(int width, int height, int format, int type, byte[] pixels) { 
			GLImport.DrawPixels(width, height, format, type, pixels); 
		}
		[SecuritySafeCritical]
		public static void RasterPosi(int x, int y) { 
			GLImport.RasterPosi(x, y); 
		}
		[SecuritySafeCritical]
		public static void ReadBuffer(int mode) { 
			GLImport.ReadBuffer(mode); 
		}
		[SecuritySafeCritical]
		public static void DrawBuffer(int mode) { 
			GLImport.DrawBuffer(mode); 
		}
		[SecuritySafeCritical]
		public static void PolygonOffset(float factor, float units) { 
			GLImport.PolygonOffset(factor, units); 
		}
		[SecuritySafeCritical]
		public static void CullFace(int mode) { 
			GLImport.CullFace(mode); 
		}
		[SecuritySafeCritical]
		public static void ColorMask(bool red, bool green, bool blue, bool alpha) { 
			GLImport.ColorMask(red, green, blue, alpha); 
		}
		[SecuritySafeCritical]
		public static void DepthMask(bool flag) { 
			GLImport.DepthMask(flag); 
		}
		[SecuritySafeCritical]
		public static void GetBooleanv(int pname, bool[] pars) { 
			GLImport.GetBooleanv(pname, pars); 
		}
		[SecuritySafeCritical]
		public static int GetError() {
			return GLImport.GetError(); 
		}
	}
	[CLSCompliant(false), SuppressUnmanagedCodeSecurity]
	public class GLUImport {
		[DllImport("glu32.dll", EntryPoint = "gluLookAt")]
		public static extern void LookAt(double eyex, double eyey, double eyez, double centerx, double centery, double centerz, double upx, double upy, double upz);
		[DllImport("glu32.dll", EntryPoint = "gluPerspective")]
		public static extern void Perspective(double fovy, double aspect, double zNear, double zFar);
		[DllImport("glu32.dll", EntryPoint = "gluNewQuadric")]
		public static extern IntPtr NewQuadric();
		[DllImport("glu32.dll", EntryPoint = "gluDeleteQuadric")]
		public static extern IntPtr DeleteQuadric(IntPtr state);
		[DllImport("glu32.dll", EntryPoint = "gluQuadricNormals")]
		public static extern IntPtr QuadricNormals(IntPtr qobj, int normals);
		[DllImport("glu32.dll", EntryPoint = "gluPartialDisk")]
		public static extern IntPtr PartialDisk(IntPtr qobj, double innerRadius, double outerRadius, int slices, int loops, double startAngle, double sweepAngle);
		[DllImport("glu32.dll", EntryPoint = "gluScaleImage")]
		public static extern int ScaleImage(int format, int widthin, int heightin, int typein, byte[] datain, int widthout, int heightout, int typeout, byte[] dataout);
		[DllImport("glu32.dll", EntryPoint = "gluScaleImage")]
		public static extern int ScaleImage(int format, int widthin, int heightin, int typein, IntPtr datain, int widthout, int heightout, int typeout, byte[] dataout);
		[DllImport("glu32.dll", EntryPoint = "gluErrorString")]
		public static extern string ErrorString(int errCode);
		[DllImport("glu32.dll", EntryPoint = "gluProject")]
		public static extern int Project(double objx, double objy, double objz, [In] double[] modelMatrix, [In] double[] projMatrix, [In] int[] viewport, out double winx, out double winy, out double winz);
		[DllImport("glu32.dll", EntryPoint = "gluUnProject")]
		public static extern int UnProject(double winx, double winy, double winz, [In] double[] modelMatrix, [In] double[] projMatrix, [In] int[] viewport, out double objx, out double objy, out double objz);
		[DllImport("glu32.dll", EntryPoint = "gluNewTess")]
		public static extern IntPtr NewTess();
		[DllImport("glu32.dll", EntryPoint = "gluTessCallback")]
		public static extern void TessCallback(IntPtr tess, int which, IntPtr fn);
		[DllImport("glu32.dll", EntryPoint = "gluTessProperty")]
		public static extern void TessProperty(IntPtr tess, int which, double value);
		[DllImport("glu32.dll", EntryPoint = "gluTessNormal")]
		public static extern void TessNormal(IntPtr tess, double x, double y, double z);
		[DllImport("glu32.dll", EntryPoint = "gluTessBeginPolygon")]
		public static extern void TessBeginPolygon(IntPtr tess, IntPtr polygon_data);
		[DllImport("glu32.dll", EntryPoint = "gluTessEndPolygon")]
		public static extern void TessEndPolygon(IntPtr tess);
		[DllImport("glu32.dll", EntryPoint = "gluTessBeginContour")]
		public static extern void TessBeginContour(IntPtr tess);
		[DllImport("glu32.dll", EntryPoint = "gluTessEndContour")]
		public static extern void TessEndContour(IntPtr tess);
		[DllImport("glu32.dll", EntryPoint = "gluTessVertex")]
		public static extern void TessVertex(IntPtr tess, [In] double[] coords, IntPtr data);
		[DllImport("glu32.dll", EntryPoint = "gluDeleteTess")]
		public static extern void DeleteTess(IntPtr tess);
	}
	public static class GLU {
		public const int SMOOTH = 100000;
		public const int FLAT = 100001;
		public const int NONE = 100002;
		public const int TESS_BEGIN = 100100;
		public const int TESS_BEGIN_DATA = 100106;
		public const int TESS_EDGE_FLAG = 100104;
		public const int TESS_EDGE_FLAG_DATA = 100110;
		public const int TESS_VERTEX = 100101;
		public const int TESS_VERTEX_DATA = 100107;
		public const int TESS_END = 100102;
		public const int TESS_END_DATA = 100108;
		public const int TESS_COMBINE = 100105;
		public const int TESS_COMBINE_DATA = 100111;
		public const int TESS_ERROR = 100103;
		public const int TESS_ERROR_DATA = 100109;
		public const int TESS_WINDING_RULE = 100140;
		public const int TESS_BOUNDARY_ONLY = 100141;
		public const int TESS_TOLERANCE = 100142;
		public const int TESS_WINDING_ODD = 100130;
		public const int TESS_WINDING_NONZERO = 100131;
		public const int TESS_WINDING_POSITIVE = 100132;
		public const int TESS_WINDING_NEGATIVE = 100133;
		public const int TESS_WINDING_ABS_GEQ_TWO = 100134;
		[SecuritySafeCritical]
		public static void LookAt(double eyex, double eyey, double eyez, double centerx, double centery, double centerz, double upx, double upy, double upz) {
			GLUImport.LookAt(eyex, eyey, eyez, centerx, centery, centerz, upx, upy, upz);
		}
		[SecuritySafeCritical]
		public static void Perspective(double fovy, double aspect, double zNear, double zFar) {
			GLUImport.Perspective(fovy, aspect, zNear, zFar);
		}
		[SecuritySafeCritical]
		public static IntPtr NewQuadric() {
			return GLUImport.NewQuadric();
		}
		[SecuritySafeCritical]
		public static IntPtr DeleteQuadric(IntPtr state) {
			return GLUImport.DeleteQuadric(state);
		}
		[SecuritySafeCritical]
		public static IntPtr QuadricNormals(IntPtr qobj, int normals) {
			return GLUImport.QuadricNormals(qobj, normals);
		}
		[SecuritySafeCritical]
		public static IntPtr PartialDisk(IntPtr qobj, double innerRadius, double outerRadius, int slices, int loops, double startAngle, double sweepAngle) {
			return GLUImport.PartialDisk(qobj, innerRadius, outerRadius, slices, loops, startAngle, sweepAngle);
		}
		[SecuritySafeCritical]
		public static int ScaleImage(int format, int widthin, int heightin, int typein, byte[] datain, int widthout, int heightout, int typeout, byte[] dataout) {
			return GLUImport.ScaleImage(format, widthin, heightin, typein, datain, widthout, heightout, typeout, dataout);
		}
		[SecuritySafeCritical]
		public static int ScaleImage(int format, int widthin, int heightin, int typein, IntPtr datain, int widthout, int heightout, int typeout, byte[] dataout) {
			return GLUImport.ScaleImage(format, widthin, heightin, typein, datain, widthout, heightout, typeout, dataout);
		}
		[SecuritySafeCritical]
		public static string ErrorString(int errCode) {
			return GLUImport.ErrorString(errCode);
		}
		[SecuritySafeCritical]
		public static int Project(double objx, double objy, double objz, double[] modelMatrix, double[] projMatrix, int[] viewport, out double winx, out double winy, out double winz) {
			return GLUImport.Project(objx, objy, objz, modelMatrix, projMatrix, viewport, out winx, out winy, out winz);
		}
		[SecuritySafeCritical]
		public static int UnProject(double winx, double winy, double winz, double[] modelMatrix, double[] projMatrix, int[] viewport, out double objx, out double objy, out double objz) {
			return GLUImport.UnProject(winx, winy, winz, modelMatrix, projMatrix, viewport, out objx, out objy, out objz);
		}
		[SecuritySafeCritical]
		public static IntPtr NewTess() {
			return GLUImport.NewTess();
		}
		[SecuritySafeCritical]
		public static void TessCallback(IntPtr tess, int which, IntPtr fn) {
			GLUImport.TessCallback(tess, which, fn);
		}
		[SecuritySafeCritical]
		public static void TessProperty(IntPtr tess, int which, double value) {
			GLUImport.TessProperty(tess, which, value);
		}
		[SecuritySafeCritical]
		public static void TessNormal(IntPtr tess, double x, double y, double z) {
			GLUImport.TessNormal(tess, x, y, z);
		}
		[SecuritySafeCritical]
		public static void TessBeginPolygon(IntPtr tess, IntPtr polygon_data) {
			GLUImport.TessBeginPolygon(tess, polygon_data);
		}
		[SecuritySafeCritical]
		public static void TessEndPolygon(IntPtr tess) {
			GLUImport.TessEndPolygon(tess);
		}
		[SecuritySafeCritical]
		public static void TessBeginContour(IntPtr tess) {
			GLUImport.TessBeginContour(tess);
		}
		[SecuritySafeCritical]
		public static void TessEndContour(IntPtr tess) {
			GLUImport.TessEndContour(tess);
		}
		[SecuritySafeCritical]
		public static void TessVertex(IntPtr tess, double[] coords, IntPtr data) {
			GLUImport.TessVertex(tess, coords, data);
		}
		[SecuritySafeCritical]
		public static void DeleteTess(IntPtr tess) {
			GLUImport.DeleteTess(tess);
		}
	}
	public class GLHelper {
		class GLMatrix {
			static double CalcDeterminant(double[] matrix, int i, int j) {
				double result;
				double[,] mat = new double[3, 3];
				int x = 0;
				for (int ii = 0; ii < 4; ii++) {
					if (ii == i) continue;
					int y = 0;
					for (int jj = 0; jj < 4; jj++) {
						if (jj == j)
							continue;
						mat[x, y] = matrix[(ii * 4) + jj];
						y++;
					}
					x++;
				}
				result = mat[0, 0] * (mat[1, 1] * mat[2, 2] - mat[2, 1] * mat[1, 2]);
				result -= mat[0, 1] * (mat[1, 0] * mat[2, 2] - mat[2, 0] * mat[1, 2]);
				result += mat[0, 2] * (mat[1, 0] * mat[2, 1] - mat[2, 0] * mat[1, 1]);
				return result;
			}
			static GLMatrix CalculateVMultVt(DiagramVector v) {
				GLMatrix result = new GLMatrix(TransformMatrix.ModelView);
				result.array[0] = v.DX * v.DX;
				result.array[1] = v.DX * v.DY;
				result.array[2] = v.DX * v.DZ;
				result.array[3] = 0.0;
				result.array[4] = v.DY * v.DX;
				result.array[5] = v.DY * v.DY;
				result.array[6] = v.DY * v.DZ;
				result.array[7] = 0.0;
				result.array[8] = v.DZ * v.DX;
				result.array[9] = v.DZ * v.DY;
				result.array[10] = v.DZ * v.DZ;
				result.array[11] = 0.0;
				result.array[12] = 0.0;
				result.array[13] = 0.0;
				result.array[14] = 0.0;
				result.array[15] = 0.0;
				return result;
			}
			static public GLMatrix operator +(GLMatrix m1, GLMatrix m2) {
				GLMatrix result = new GLMatrix(m1.matrixKind);
				for (int i = 0; i < 16; i++)
					result.array[i] = m1.array[i] + m2.array[i];
				return result;
			}
			static public GLMatrix operator -(GLMatrix m1, GLMatrix m2) {
				GLMatrix result = new GLMatrix(m1.matrixKind);
				for (int i = 0; i < 16; i++)
					result.array[i] = m1.array[i] - m2.array[i];
				return result;
			}
			static public GLMatrix operator *(double factor, GLMatrix m) {
				GLMatrix result = new GLMatrix(m.matrixKind);
				for (int i = 0; i < 16; i++)
					result.array[i] = factor * m.array[i];
				return result;
			}
			static public DiagramPoint operator *(DiagramPoint point, GLMatrix m) {
				DiagramPoint res = new DiagramPoint();
				res.X = m.array[0] * point.X + m.array[4] * point.Y + m.array[8] * point.Z + m.array[12] * 1;
				res.Y = m.array[1] * point.X + m.array[5] * point.Y + m.array[9] * point.Z + m.array[13] * 1;
				res.Z = m.array[2] * point.X + m.array[6] * point.Y + m.array[10] * point.Z + m.array[14] * 1;
				return res;
			}
			static public GLMatrix operator *(GLMatrix m1, GLMatrix m2) {
				GLMatrix result = new GLMatrix(m1.matrixKind);
				result.array[0] = m1.array[0] * m2.array[0] + m1.array[4] * m2.array[1] + m1.array[8] * m2.array[2] + m1.array[12] * m2.array[3];
				result.array[1] = m1.array[1] * m2.array[0] + m1.array[5] * m2.array[1] + m1.array[9] * m2.array[2] + m1.array[13] * m2.array[3];
				result.array[2] = m1.array[2] * m2.array[0] + m1.array[6] * m2.array[1] + m1.array[10] * m2.array[2] + m1.array[14] * m2.array[3];
				result.array[3] = m1.array[3] * m2.array[0] + m1.array[7] * m2.array[1] + m1.array[11] * m2.array[2] + m1.array[15] * m2.array[3];
				result.array[4] = m1.array[0] * m2.array[4] + m1.array[4] * m2.array[5] + m1.array[8] * m2.array[6] + m1.array[12] * m2.array[7];
				result.array[5] = m1.array[1] * m2.array[4] + m1.array[5] * m2.array[5] + m1.array[9] * m2.array[6] + m1.array[13] * m2.array[7];
				result.array[6] = m1.array[2] * m2.array[4] + m1.array[6] * m2.array[5] + m1.array[10] * m2.array[6] + m1.array[14] * m2.array[7];
				result.array[7] = m1.array[3] * m2.array[4] + m1.array[7] * m2.array[5] + m1.array[11] * m2.array[6] + m1.array[15] * m2.array[7];
				result.array[8] = m1.array[0] * m2.array[8] + m1.array[4] * m2.array[9] + m1.array[8] * m2.array[10] + m1.array[12] * m2.array[11];
				result.array[9] = m1.array[1] * m2.array[8] + m1.array[5] * m2.array[9] + m1.array[9] * m2.array[10] + m1.array[13] * m2.array[11];
				result.array[10] = m1.array[2] * m2.array[8] + m1.array[6] * m2.array[9] + m1.array[10] * m2.array[10] + m1.array[14] * m2.array[11];
				result.array[11] = m1.array[3] * m2.array[8] + m1.array[7] * m2.array[9] + m1.array[11] * m2.array[10] + m1.array[15] * m2.array[11];
				result.array[12] = m1.array[0] * m2.array[12] + m1.array[4] * m2.array[13] + m1.array[8] * m2.array[14] + m1.array[12] * m2.array[15];
				result.array[13] = m1.array[1] * m2.array[12] + m1.array[5] * m2.array[13] + m1.array[9] * m2.array[14] + m1.array[13] * m2.array[15];
				result.array[14] = m1.array[2] * m2.array[12] + m1.array[6] * m2.array[13] + m1.array[10] * m2.array[14] + m1.array[14] * m2.array[15];
				result.array[15] = m1.array[3] * m2.array[12] + m1.array[7] * m2.array[13] + m1.array[11] * m2.array[14] + m1.array[15] * m2.array[15];
				return result;
			}
			TransformMatrix matrixKind;
			double[] array = new double[16];
			public double[] Array { get { return array; } }
			public GLMatrix(TransformMatrix matrixKind) {
				this.matrixKind = matrixKind;
				array[0] = 1.0; array[4] = 0.0; array[8] = 0.0; array[12] = 0.0;
				array[1] = 0.0; array[5] = 1.0; array[9] = 0.0; array[13] = 0.0;
				array[2] = 0.0; array[6] = 0.0; array[10] = 1.0; array[14] = 0.0;
				array[3] = 0.0; array[7] = 0.0; array[11] = 0.0; array[15] = 1.0;
			}
			public GLMatrix(TransformMatrix matrixKind, double[] matrix) {
				this.matrixKind = matrixKind;
				array = matrix;
			}
			public void PerformTranslateCommand(GraphicsCommand command) {
				if (matrixKind == TransformMatrix.ModelView) {
					TranslateGraphicsCommand translateCommand = command as TranslateGraphicsCommand;
					if (translateCommand != null) {
						GLMatrix translateMatrix = new GLMatrix(matrixKind);
						translateMatrix.array[12] = translateCommand.XTranslate;
						translateMatrix.array[13] = translateCommand.YTranslate;
						translateMatrix.array[14] = translateCommand.ZTranslate;
						GLMatrix result = this * translateMatrix;
						array = result.array;
					}
				}
			}
			public void PerformRotateCommand(GraphicsCommand command) {
				if (matrixKind == TransformMatrix.ModelView) {
					RotateGraphicsCommand rotateCommand = command as RotateGraphicsCommand;
					if (rotateCommand != null) {
						DiagramVector rotateVector = rotateCommand.RotateVector;
						rotateVector.Normalize();
						double angle = rotateCommand.Angle * Math.PI / 180.0;
						GLMatrix vvt = GLMatrix.CalculateVMultVt(rotateVector);
						GLMatrix s = new GLMatrix(matrixKind);
						int firstElement = 0;
						s.array[0] = firstElement;
						s.array[1] = rotateVector.DZ;
						s.array[2] = -rotateVector.DY;
						s.array[3] = 0.0;
						s.array[4] = -rotateVector.DZ;
						s.array[5] = 0.0;
						s.array[6] = rotateVector.DX;
						s.array[7] = 0.0;
						s.array[8] = rotateVector.DY;
						s.array[9] = -rotateVector.DX;
						s.array[10] = 0.0;
						s.array[11] = 0.0;
						s.array[12] = 0.0;
						s.array[13] = 0.0;
						s.array[14] = 0.0;
						s.array[15] = 0.0;
						GLMatrix rotateMatrix = vvt + Math.Cos(angle) * (new GLMatrix(matrixKind) - vvt) + Math.Sin(angle) * s;
						rotateMatrix.array[3] = 0.0;
						rotateMatrix.array[7] = 0.0;
						rotateMatrix.array[11] = 0.0;
						rotateMatrix.array[12] = 0.0;
						rotateMatrix.array[13] = 0.0;
						rotateMatrix.array[14] = 0.0;
						rotateMatrix.array[15] = 1.0;
						GLMatrix result = this * rotateMatrix;
						array = result.array;
					}
				}
			}
			public void PerformTransformCommand(GraphicsCommand command) {
				if (matrixKind == TransformMatrix.ModelView) {
					TransformGraphicsCommand transformCommand = command as TransformGraphicsCommand;
					if (transformCommand != null) {
						double[] values = transformCommand.TransformMatrix;
						GLMatrix transformMatrix = new GLMatrix(matrixKind);
						for (int i = 0; i < 16; i++)
							transformMatrix.array[i] = values[i];
						GLMatrix result = this * transformMatrix;
						array = result.array;
					}
				}
			}
			public void PerformOrthoCommand(GraphicsCommand command) {
				if (matrixKind == TransformMatrix.Projection) {
					OrthographicProjectionGraphicsCommand orthoCommand = command as OrthographicProjectionGraphicsCommand;
					if (orthoCommand != null) {
						double width = orthoCommand.P2.X - orthoCommand.P1.X;
						double height = orthoCommand.P2.Y - orthoCommand.P1.Y;
						double depth = orthoCommand.P2.Z - orthoCommand.P1.Z;
						GLMatrix projectionMatrix = new GLMatrix(matrixKind);
						projectionMatrix.array[0] = 2.0 / width;
						projectionMatrix.array[5] = 2.0 / height;
						projectionMatrix.array[10] = -2.0 / depth;
						projectionMatrix.array[12] = -(orthoCommand.P1.X + orthoCommand.P2.X) / width;
						projectionMatrix.array[13] = -(orthoCommand.P1.Y + orthoCommand.P2.Y) / height;
						projectionMatrix.array[14] = -(orthoCommand.P1.Z + orthoCommand.P2.Z) / depth;
						projectionMatrix.array[15] = 1.0;
						GLMatrix result = this * projectionMatrix;
						array = result.array;
					}
				}
			}
			public void PerformPerspectiveCommand(GraphicsCommand command) {
				if (matrixKind == TransformMatrix.Projection) {
					FrustumProjectionGraphicsCommand frustumCommand = command as FrustumProjectionGraphicsCommand;
					if (frustumCommand != null) {
						double width = frustumCommand.P2.X - frustumCommand.P1.X;
						double height = frustumCommand.P2.Y - frustumCommand.P1.Y;
						double depth = frustumCommand.P2.Z - frustumCommand.P1.Z;
						double doubleDistance = 2.0 * frustumCommand.P1.Z;
						GLMatrix projectionMatrix = new GLMatrix(matrixKind);
						projectionMatrix.array[0] = doubleDistance / width;
						projectionMatrix.array[5] = doubleDistance / height;
						projectionMatrix.array[8] = (frustumCommand.P1.X + frustumCommand.P2.X) / width;
						projectionMatrix.array[9] = (frustumCommand.P1.Y + frustumCommand.P2.Y) / height;
						projectionMatrix.array[10] = -(frustumCommand.P1.Z + frustumCommand.P2.Z) / depth;
						projectionMatrix.array[11] = -1.0;
						projectionMatrix.array[14] = -2.0 * frustumCommand.P1.Z * frustumCommand.P2.Z / depth;
						projectionMatrix.array[15] = 0.0;
						GLMatrix result = this * projectionMatrix;
						array = result.array;
					}
				}
			}
			public void PerformCommand(GraphicsCommand command) {
				PerformTranslateCommand(command);
				PerformRotateCommand(command);
				PerformTransformCommand(command);
				PerformOrthoCommand(command);
				PerformPerspectiveCommand(command);
				if (command.Children != null)
					foreach (GraphicsCommand child in command.Children)
						PerformCommand(child);
			}
			public GLMatrix CalcInverseMatrix() {
				double det = 0.0;
				for (int i = 0; i < 4; i++) {
					det += (i % 2 == 1) ? (-array[i] * CalcDeterminant(array, 0, i)) : (array[i] * CalcDeterminant(array, 0, i));
				}
				det = 1.0 / det;
				double[] inverseMatrix = new double[16];
				for (int i = 0; i < 4; i++) {
					for (int j = 0; j < 4; j++) {
						double detij = CalcDeterminant(array, j, i);
						inverseMatrix[(i * 4) + j] = ((i + j) % 2 == 1) ? (-detij * det) : (detij * det);
					}
				}
				return new GLMatrix(matrixKind, inverseMatrix);
			}
		}
		public const int MinTexCoord = 0;
		public const int MidTexCoord = 127;
		public const int MaxTexCoord = 255;
		public delegate void DrawingFunction();
		public delegate int Gen1DTextureCoord(double x, double y, double z);
		public static int ConvertTexCoord(float texCoord) {
			if (texCoord < 0.0f)
				return MinTexCoord;
			if (texCoord > 1.0f)
				return MaxTexCoord;
			return MinTexCoord + Convert.ToInt32(texCoord * MaxTexCoord);
		}
		public static float[] Create1DTexture(Color color, Color color2) {
			float red, green, blue, alpha;
			OpenGLGraphics.CalculateColorComponents(color, out red, out green, out blue, out alpha);
			float red2, green2, blue2, alpha2;
			OpenGLGraphics.CalculateColorComponents(color2, out red2, out green2, out blue2, out alpha2);
			int maxTexIndex = MaxTexCoord - MinTexCoord;
			float reddiff = (red2 - red) / maxTexIndex;
			float greendiff = (green2 - green) / maxTexIndex;
			float bluediff = (blue2 - blue) / maxTexIndex;
			float alphadiff = (alpha2 - alpha) / maxTexIndex;
			float[] pixels = new float[(maxTexIndex + 1) * 4];
			for (int i = 0, index = 0; i <= maxTexIndex; i++) {
				pixels[index++] = red + reddiff * i;
				pixels[index++] = green + greendiff * i;
				pixels[index++] = blue + bluediff * i;
				pixels[index++] = alpha + alphadiff * i;
			}
			return pixels;
		}
		public static int GetSlicesCount(double sweepAngle) {
			int count = (int)Math.Round(sweepAngle);
			return count < 0 ? -count : count;
		}
		public static void Arc(float position, float radius, float sweepAngle) {
			float radianSweepAngle = -(float)(sweepAngle * Math.PI) / 3600.0f;
			GL.Begin(GL.LINE_STRIP);
			for (int i = 0; i <= 20; i++) {
				double angle = i * radianSweepAngle;
				float z = (float)Math.Sin(angle), y = (float)Math.Cos(angle);
				GL.Normal3d(0.0, y, z);
				GL.Vertex3d(position, y * radius, z * radius);
			}
			GL.End();
		}
		public static void PartialDisk(float radius, float startAngle, float sweepAngle) {
			PartialDisk(radius, startAngle, sweepAngle, null, null);
		}
		public static void PartialDisk(float radius, float startAngle, float sweepAngle, float[] texture, Gen1DTextureCoord textureGenerator) {
			float radianStartAngle = (float)(startAngle * Math.PI) / 180.0f;
			int slicesCount = GetSlicesCount(sweepAngle);
			float radianSweepAngle = (float)(sweepAngle * Math.PI) / 180.0f / slicesCount;
			GL.Normal3d(0.0, 0.0, 1.0);
			GL.Begin(GL.TRIANGLES);
			for (int i = 0, j = 1; i < slicesCount; i++, j++) {
				double angle = i * radianSweepAngle + radianStartAngle;
				double x1 = Math.Cos(angle) * radius;
				double y1 = Math.Sin(angle) * radius;
				angle = j * radianSweepAngle + radianStartAngle;
				double x2 = Math.Cos(angle) * radius;
				double y2 = Math.Sin(angle) * radius;
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(0.0, 0.0, 0.0));
				GL.Vertex3d(0.0, 0.0, 0.0);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(x1, y1, 0.0));
				GL.Vertex3d(x1, y1, 0.0);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(x2, y2, 0.0));
				GL.Vertex3d(x2, y2, 0.0);
			}
			GL.End();
		}
		public static void PartialDisk(float innerRadius, float outerRadius, float startAngle, float sweepAngle) {
			PartialDisk(innerRadius, outerRadius, startAngle, sweepAngle, null, null);
		}
		public static void PartialDisk(float innerRadius, float outerRadius, float startAngle, float sweepAngle, float[] texture, Gen1DTextureCoord textureGenerator) {
			float radianStartAngle = (float)(startAngle * Math.PI) / 180.0f;
			int slicesCount = GetSlicesCount(sweepAngle);
			float radianSweepAngle = (float)(sweepAngle * Math.PI) / 180.0f / slicesCount;
			GL.Normal3d(0.0, 0.0, 1.0);
			GL.Begin(GL.QUADS);
			for (int i = 0, j = 1; i < slicesCount; i++, j++) {
				double angle = i * radianSweepAngle + radianStartAngle;
				double cos = Math.Cos(angle);
				double sin = Math.Sin(angle);
				double ox1 = cos * outerRadius;
				double oy1 = sin * outerRadius;
				double ix1 = cos * innerRadius;
				double iy1 = sin * innerRadius;
				angle = j * radianSweepAngle + radianStartAngle;
				cos = Math.Cos(angle);
				sin = Math.Sin(angle);
				double ox2 = cos * outerRadius;
				double oy2 = sin * outerRadius;
				double ix2 = cos * innerRadius;
				double iy2 = sin * innerRadius;
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(ix2, iy2, 0.0));
				GL.Vertex3d(ix2, iy2, 0.0);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(ix1, iy1, 0.0));
				GL.Vertex3d(ix1, iy1, 0.0);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(ox1, oy1, 0.0));
				GL.Vertex3d(ox1, oy1, 0.0);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(ox2, oy2, 0.0));
				GL.Vertex3d(ox2, oy2, 0.0);
			}
			GL.End();
		}
		public static void PartialCylinder(float height, float radius, float startAngle, float sweepAngle) {
			PartialCylinder(height, radius, startAngle, sweepAngle, null, null);
		}
		public static void PartialCylinder(float height, float radius, float startAngle, float sweepAngle, float[] texture, Gen1DTextureCoord textureGenerator) {
			float radianStartAngle = (float)(startAngle * Math.PI) / 180.0f;
			int slicesCount = GetSlicesCount(sweepAngle);
			float radianSweepAngle = (float)(sweepAngle * Math.PI) / 180.0f / slicesCount;
			for (int i = 0, j = 1; i < slicesCount; i++, j++) {
				double angle = i * radianSweepAngle + radianStartAngle;
				float x1 = (float)Math.Cos(angle);
				float y1 = (float)Math.Sin(angle);
				angle = j * radianSweepAngle + radianStartAngle;
				float x2 = (float)Math.Cos(angle);
				float y2 = (float)Math.Sin(angle);
				DiagramPoint n1 = new DiagramPoint(x1, y1, 0.0);
				DiagramPoint n2 = new DiagramPoint(x2, y2, 0.0);
				x1 *= radius;
				y1 *= radius;
				x2 *= radius;
				y2 *= radius;
				GL.Begin(GL.QUADS);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(x1, y1, 0.0));
				GL.Normal3d(n1.X, n1.Y, n1.Z);
				GL.Vertex3d(x1, y1, height);
				GL.Vertex3d(x1, y1, 0);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(x2, y2, 0.0));
				GL.Normal3d(n2.X, n2.Y, n2.Z);
				GL.Vertex3d(x2, y2, 0.0);
				GL.Vertex3d(x2, y2, height);
				GL.End();
			}
		}
		public static void PartialCone(float height, float radius1, float radius2, float startAngle, float sweepAngle) {
			PartialCone(height, radius1, radius2, startAngle, sweepAngle, null, null);
		}
		public static void PartialCone(float height, float radius1, float radius2, float startAngle, float sweepAngle, float[] texture, Gen1DTextureCoord textureGenerator) {
			float radianStartAngle = (float)(startAngle * Math.PI) / 180.0f;
			int slicesCount = GetSlicesCount(sweepAngle);
			float radianSweepAngle = (float)(sweepAngle * Math.PI) / 180.0f / slicesCount;
			for (int i = 0, j = 1; i < slicesCount; i++, j++) {
				double angle = i * radianSweepAngle + radianStartAngle;
				float x1 = (float)Math.Cos(angle);
				float y1 = (float)Math.Sin(angle);
				angle = j * radianSweepAngle + radianStartAngle;
				float x2 = (float)Math.Cos(angle);
				float y2 = (float)Math.Sin(angle);
				double coneAngle = height != 0 ? Math.Atan2(radius1 - radius2, height) : 0;
				double z = Math.Sin(coneAngle);
				double xy = Math.Cos(coneAngle);
				DiagramVector normal1 = new DiagramVector(x1 * xy, y1 * xy, z);
				DiagramVector normal2 = new DiagramVector(x2 * xy, y2 * xy, z);
				normal1.Normalize();
				normal2.Normalize();
				DiagramPoint vertex1 = new DiagramPoint(x1 * radius2, y1 * radius2, height);
				DiagramPoint vertex2 = new DiagramPoint(x1 * radius1, y1 * radius1, 0);
				DiagramPoint vertex3 = new DiagramPoint(x2 * radius1, y2 * radius1, 0);
				DiagramPoint vertex4 = new DiagramPoint(x2 * radius2, y2 * radius2, height);
				GL.Begin(GL.QUADS);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(vertex1.X, vertex1.Y, 0));
				GL.Normal3d(normal1.DX, normal1.DY, normal1.DZ);
				GL.Vertex3d(vertex1.X, vertex1.Y, vertex1.Z);
				GL.Vertex3d(vertex2.X, vertex2.Y, vertex2.Z);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(vertex3.X, vertex3.Y, 0));
				GL.Normal3d(normal2.DX, normal2.DY, normal2.DZ);
				GL.Vertex3d(vertex3.X, vertex3.Y, vertex3.Z);
				GL.Vertex3d(vertex4.X, vertex4.Y, vertex4.Z);
				GL.End();
			}
		}
		public static void PieSections(float height, float innerRadius, float outerRadius, float facetSize, float startAngle, float endAngle) {
			PieSections(height, innerRadius, outerRadius, facetSize, startAngle, endAngle, null, null);
		}
		public static void PieSections(float height, float innerRadius, float outerRadius, float facetSize, float startAngle, float endAngle, float[] texture, Gen1DTextureCoord textureGenerator) {
			double radianStartAngle = startAngle * Math.PI / 180.0;
			double radianEndAngle = endAngle * Math.PI / 180.0;
			double startAngleCos = Math.Cos(radianStartAngle);
			double startAngleSin = Math.Sin(radianStartAngle);
			double endAngleCos = Math.Cos(radianEndAngle);
			double endAngleSin = Math.Sin(radianEndAngle);
			DiagramPoint innerStart = new DiagramPoint(startAngleCos * innerRadius, startAngleSin * innerRadius);
			DiagramPoint innerEnd = new DiagramPoint(endAngleCos * innerRadius, endAngleSin * innerRadius);
			DiagramPoint outerStart = new DiagramPoint(startAngleCos * outerRadius, startAngleSin * outerRadius);
			DiagramPoint outerEnd = new DiagramPoint(endAngleCos * outerRadius, endAngleSin * outerRadius);
			DiagramPoint bottomCenter = DiagramPoint.Zero;
			DiagramPoint topCenter = new DiagramPoint(0.0, 0.0, height);
			DiagramVector startNormal = MathUtils.CalcNormal(bottomCenter, topCenter, outerStart);
			DiagramVector endNormal = MathUtils.CalcNormal(bottomCenter, topCenter, outerEnd);
			if (facetSize > 0) {
				double internalInnerRadius = innerRadius == 0.0f ? 0.0f : innerRadius + facetSize;
				double internalOuterRadius = outerRadius - facetSize;
				if (internalInnerRadius > internalOuterRadius)
					internalInnerRadius = internalOuterRadius;
				double minDepth = facetSize;
				double maxDepth = height - facetSize;
				double actualInnerStartX = startAngleCos * internalInnerRadius;
				double actualInnerStartY = startAngleSin * internalInnerRadius;
				double actualInnerEndX = endAngleCos * internalInnerRadius;
				double actualInnerEndY = endAngleSin * internalInnerRadius;
				double actualOuterStartX = startAngleCos * internalOuterRadius;
				double actualOuterStartY = startAngleSin * internalOuterRadius;
				double actualOuterEndX = endAngleCos * internalOuterRadius;
				double actualOuterEndY = endAngleSin * internalOuterRadius;
				GL.Normal3d(startNormal.DX, startNormal.DY, startNormal.DZ);
				GL.Begin(GL.QUAD_STRIP);
				if (innerRadius > 0.0f) {
					if (textureGenerator != null)
						SetColor(texture, textureGenerator(innerStart.X, innerStart.Y, 0.0));
					GL.Vertex3d(innerStart.X, innerStart.Y, minDepth);
					GL.Vertex3d(innerStart.X, innerStart.Y, maxDepth);
				}
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(actualInnerStartX, actualInnerStartY, 0.0));
				GL.Vertex3d(actualInnerStartX, actualInnerStartY, 0.0);
				GL.Vertex3d(actualInnerStartX, actualInnerStartY, height);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(actualOuterStartX, actualOuterStartY, 0.0));
				GL.Vertex3d(actualOuterStartX, actualOuterStartY, 0.0);
				GL.Vertex3d(actualOuterStartX, actualOuterStartY, height);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(outerStart.X, outerStart.Y, 0.0));
				GL.Vertex3d(outerStart.X, outerStart.Y, minDepth);
				GL.Vertex3d(outerStart.X, outerStart.Y, maxDepth);
				GL.End();
				GL.Normal3d(endNormal.DX, endNormal.DY, endNormal.DZ);
				GL.Begin(GL.QUAD_STRIP);
				if (innerRadius > 0.0f) {
					if (textureGenerator != null)
						SetColor(texture, textureGenerator(innerEnd.X, innerEnd.Y, 0.0));
					GL.Vertex3d(innerEnd.X, innerEnd.Y, minDepth);
					GL.Vertex3d(innerEnd.X, innerEnd.Y, maxDepth);
				}
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(actualInnerEndX, actualInnerEndY, 0.0));
				GL.Vertex3d(actualInnerEndX, actualInnerEndY, 0.0);
				GL.Vertex3d(actualInnerEndX, actualInnerEndY, height);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(actualOuterEndX, actualOuterEndY, 0.0));
				GL.Vertex3d(actualOuterEndX, actualOuterEndY, 0.0);
				GL.Vertex3d(actualOuterEndX, actualOuterEndY, height);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(outerEnd.X, outerEnd.Y, 0.0));
				GL.Vertex3d(outerEnd.X, outerEnd.Y, minDepth);
				GL.Vertex3d(outerEnd.X, outerEnd.Y, maxDepth);
				GL.End();
			} else {
				GL.Begin(GL.QUADS);
				GL.Normal3d(startNormal.DX, startNormal.DY, startNormal.DZ);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(innerStart.X, innerStart.Y, 0.0));
				GL.Vertex3d(innerStart.X, innerStart.Y, 0.0);
				GL.Vertex3d(innerStart.X, innerStart.Y, height);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(outerStart.X, outerStart.Y, 0.0));
				GL.Vertex3d(outerStart.X, outerStart.Y, height);
				GL.Vertex3d(outerStart.X, outerStart.Y, 0.0);
				GL.Normal3d(endNormal.DX, endNormal.DY, endNormal.DZ);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(innerEnd.X, innerEnd.Y, 0.0));
				GL.Vertex3d(innerEnd.X, innerEnd.Y, 0.0);
				GL.Vertex3d(innerEnd.X, innerEnd.Y, height);
				if (textureGenerator != null)
					SetColor(texture, textureGenerator(outerEnd.X, outerEnd.Y, 0.0));
				GL.Vertex3d(outerEnd.X, outerEnd.Y, height);
				GL.Vertex3d(outerEnd.X, outerEnd.Y, 0.0);
				GL.End();
			}
		}
		public static void SetColor(float[] texture, int index) {
			GL.Color4f(texture[index * 4], texture[index * 4 + 1], texture[index * 4 + 2], texture[index * 4 + 3]);
		}
		public static void CalculateMatrices(GraphicsCommand command, out double[] modelView, out double[] projection) {
			GLMatrix modelViewMatrix = new GLMatrix(TransformMatrix.ModelView);
			GLMatrix projectionMatrix = new GLMatrix(TransformMatrix.Projection);
			modelViewMatrix.PerformCommand(command);
			projectionMatrix.PerformCommand(command);
			modelView = modelViewMatrix.Array;
			projection = projectionMatrix.Array;
		}
		public static void PerformRotationStandard(double[] matrix, double dx, double dy) {
			GLMatrix glMatrix1 = new GLMatrix(TransformMatrix.ModelView);
			RotateGraphicsCommand rotate1 = new RotateGraphicsCommand((float)dy, new DiagramVector(1, 0, 0));
			glMatrix1.PerformRotateCommand(rotate1);
			GLMatrix glMatrix2 = new GLMatrix(TransformMatrix.ModelView, matrix);
			GLMatrix result = glMatrix1 * glMatrix2;
			RotateGraphicsCommand rotate2 = new RotateGraphicsCommand((float)dx, new DiagramVector(0, 1, 0));
			result.PerformRotateCommand(rotate2);
			result.Array.CopyTo(matrix, 0);
		}
		public static void PerformRotation(double[] matrix, double dx, double dy) {
			GLMatrix glMatrix1 = new GLMatrix(TransformMatrix.ModelView);
			RotateGraphicsCommand rotate1 = new RotateGraphicsCommand((float)dx, new DiagramVector(0, 1, 0));
			RotateGraphicsCommand rotate2 = new RotateGraphicsCommand((float)dy, new DiagramVector(1, 0, 0));
			glMatrix1.PerformRotateCommand(rotate1);
			glMatrix1.PerformRotateCommand(rotate2);
			GLMatrix glMatrix2 = new GLMatrix(TransformMatrix.ModelView, matrix);
			GLMatrix result = glMatrix1 * glMatrix2;
			result.Array.CopyTo(matrix, 0);
		}
		public static void PerformRotation(double[] matrix, double dz) {
			GLMatrix glMatrix1 = new GLMatrix(TransformMatrix.ModelView);
			RotateGraphicsCommand rotate = new RotateGraphicsCommand((float)dz, new DiagramVector(0, 0, 1));
			glMatrix1.PerformRotateCommand(rotate);
			GLMatrix glMatrix2 = new GLMatrix(TransformMatrix.ModelView, matrix);
			GLMatrix result = glMatrix1 * glMatrix2;
			result.Array.CopyTo(matrix, 0);
		}
		public static DiagramPoint InverseTransform(double[] matrix, DiagramPoint point) {
			double[] newMatrix = new double[16];
			matrix.CopyTo(newMatrix, 0);
			GLMatrix modelViewMatrix = new GLMatrix(TransformMatrix.ModelView, newMatrix);
			modelViewMatrix.Array[12] = modelViewMatrix.Array[13] = modelViewMatrix.Array[14] = 0.0;
			return point * modelViewMatrix.CalcInverseMatrix();
		}
	}
}
