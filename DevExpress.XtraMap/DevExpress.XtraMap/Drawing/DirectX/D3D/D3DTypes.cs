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
using DevExpress.XtraMap.Native;
namespace D3D {
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3d {
		double x;
		double y;
		double z;
		public double X { get { return x; } set { x = value; } }
		public double Y { get { return y; } set { y = value; } }
		public double Z { get { return z; } set { z = value; } }
		public Vector3d(double vX, double vY, double vZ) {
			x = vX;
			y = vY;
			z = vZ;
		}
		public static Vector3d operator -(Vector3d a, Vector3d b) {
			return new Vector3d(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}
		public static Vector3d operator +(Vector3d a, Vector3d b) {
			return new Vector3d(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
		public static Vector3d operator *(Vector3d v, double Scalar) {
			return new Vector3d(v.X * Scalar, v.Y * Scalar, v.Z * Scalar);
		}
		public void Normalize() {
			double magnitude = Length();
			if(magnitude > 0.0) {
				double invertedMag = 1.0 / magnitude;
				x *= invertedMag;
				y *= invertedMag;
				z *= invertedMag;
			}
		}
		public double Length() {
			return Math.Sqrt(x * x + y *y + z * z);
		}
		public static double DotProduct(Vector3d a, Vector3d b) {
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3 {
		float x;
		float y;
		float z;
		public float X { get { return x; } set { x = value; } }
		public float Y { get { return y; } set { y = value; } }
		public float Z { get { return z; } set { z = value; } }
		public Vector3(float vX, float vY, float vZ) {
			x = vX;
			y = vY;
			z = vZ;
		}
		public Vector3(Vector2 vector) {
			x = vector.X;
			y = vector.Y;
			z = .0f;
		}
		public static Vector3 operator -(Vector3 a, Vector3 b) {
			return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}
		public static Vector3 operator +(Vector3 a, Vector3 b) {
			return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
		public static Vector3 operator *(Vector3 v, float Scalar) {
			return new Vector3(v.X*Scalar, v.Y * Scalar, v.Z * Scalar);
		}
		public static bool operator ==(Vector3 left, Vector3 right) {
			return left.Equals(right);
		}
		public static bool operator !=(Vector3 left, Vector3 right) {
			return !left.Equals(right);
		}
		public float Length(){
			return Convert.ToSingle(Math.Sqrt (x * x + y * y + z * z));
		}
		public Vector3 Inversion() {
			return new Vector3(-x, -y, -z);
		}
		public static float DotProduct(Vector3 a, Vector3 b) {
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}
		public override int GetHashCode() {
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
		}
		public override bool Equals(object obj) {
			if(obj == null)
				return false;
			if(obj.GetType() != GetType())
				return false;
			return Equals((Vector3)obj);
		}
		public bool Equals(Vector3 vector) {
			return (this.X == vector.X) && (this.Y == vector.Y) && (this.Z == vector.Z);
		}
		public void Normalize(){
			float magnitude = Length();
			if (magnitude > 0.0) {
				float invertedMag = 1.0f / magnitude;
				x *= invertedMag;
				y *= invertedMag;
				z *= invertedMag;
			}
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2 {
		float x;
		float y;
		public float X { get { return x; } set { x = value; } }
		public float Y { get { return y; } set { y = value; } }
		public Vector2(float vX, float vY) {
			x = vX;
			y = vY;
		}
		public Vector2d ToVector2d() {
			return new Vector2d(x, y);
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2d {
		double x;
		double y;
		public double X { get { return x; } set { x = value; } }
		public double Y { get { return y; } set { y = value; } }
		public Vector2d(double vX, double vY) {
			x = vX;
			y = vY;
		}
		public static Vector2d operator -(Vector2d a, Vector2d b) {
			return new Vector2d(a.X - b.X, a.Y - b.Y);
		}
		public Vector2d Inversion() {
			return new Vector2d(-x, -y);
		}
		public Vector2 ToVector2() {
			return new Vector2((float)x, (float)y);
		}
		public Vector2 GetFloatPrecision() {
			float _x = (float)(x - (float)x);
			float _y = (float)(y - (float)y);
			return new Vector2(_x, _y);
		}
	}
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct Matrix4x4{
		public static readonly Matrix4x4 Zero = new Matrix4x4();
		public static readonly Matrix4x4 Identity = new Matrix4x4() { M11 = 1.0f, M22 = 1.0f, M33 = 1.0f, M44 = 1.0f };
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public float M11, M12, M13, M14;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public float M21, M22, M23, M24;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public float M31, M32, M33, M34;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public float M41, M42, M43, M44;
		public float[] ToArray() {
			return new float[] { M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44 };
		}
		public static Matrix4x4 CreateMatrixOrthoLH(float w, float h, float zn, float zf) {
			Matrix4x4 resMat = Identity;
			resMat.M11 = 2.0f / w;
			resMat.M22 = 2.0f / h;
			resMat.M33 = 1.0f / (zf - zn);
			resMat.M43 = zn / (zn - zf);
			return resMat;
		}
		 public static Matrix4x4 CreateMatrixOrthoRH(float w, float h, float zn, float zf) {
			Matrix4x4 resMat = Identity;
			resMat.M11 = 2.0f / w;
			resMat.M22 = 2.0f / h;
			resMat.M33 = 1.0f / (zn - zf);
			resMat.M43 = zn / (zn - zf);
			return resMat;
		}
		 public static Matrix4x4 Multiply(Matrix4x4 m1, Matrix4x4 m2) {
			 Matrix4x4 result = new Matrix4x4();
			 result.M11 = (m1.M11 * m2.M11) + (m1.M12 * m2.M21) + (m1.M13 * m2.M31) + (m1.M14 * m2.M41);
			 result.M12 = (m1.M11 * m2.M12) + (m1.M12 * m2.M22) + (m1.M13 * m2.M32) + (m1.M14 * m2.M42);
			 result.M13 = (m1.M11 * m2.M13) + (m1.M12 * m2.M23) + (m1.M13 * m2.M33) + (m1.M14 * m2.M43);
			 result.M14 = (m1.M11 * m2.M14) + (m1.M12 * m2.M24) + (m1.M13 * m2.M34) + (m1.M14 * m2.M44);
			 result.M21 = (m1.M21 * m2.M11) + (m1.M22 * m2.M21) + (m1.M23 * m2.M31) + (m1.M24 * m2.M41);
			 result.M22 = (m1.M21 * m2.M12) + (m1.M22 * m2.M22) + (m1.M23 * m2.M32) + (m1.M24 * m2.M42);
			 result.M23 = (m1.M21 * m2.M13) + (m1.M22 * m2.M23) + (m1.M23 * m2.M33) + (m1.M24 * m2.M43);
			 result.M24 = (m1.M21 * m2.M14) + (m1.M22 * m2.M24) + (m1.M23 * m2.M34) + (m1.M24 * m2.M44);
			 result.M31 = (m1.M31 * m2.M11) + (m1.M32 * m2.M21) + (m1.M33 * m2.M31) + (m1.M34 * m2.M41);
			 result.M32 = (m1.M31 * m2.M12) + (m1.M32 * m2.M22) + (m1.M33 * m2.M32) + (m1.M34 * m2.M42);
			 result.M33 = (m1.M31 * m2.M13) + (m1.M32 * m2.M23) + (m1.M33 * m2.M33) + (m1.M34 * m2.M43);
			 result.M34 = (m1.M31 * m2.M14) + (m1.M32 * m2.M24) + (m1.M33 * m2.M34) + (m1.M34 * m2.M44);
			 result.M41 = (m1.M41 * m2.M11) + (m1.M42 * m2.M21) + (m1.M43 * m2.M31) + (m1.M44 * m2.M41);
			 result.M42 = (m1.M41 * m2.M12) + (m1.M42 * m2.M22) + (m1.M43 * m2.M32) + (m1.M44 * m2.M42);
			 result.M43 = (m1.M41 * m2.M13) + (m1.M42 * m2.M23) + (m1.M43 * m2.M33) + (m1.M44 * m2.M43);
			 result.M44 = (m1.M41 * m2.M14) + (m1.M42 * m2.M24) + (m1.M43 * m2.M34) + (m1.M44 * m2.M44);
			 return result;
		 }
	   public void Translation(float dX, float dY, float dZ) {
			M41 += dX;
			M42 += dY;
			M43 += dZ;
		}
	   public void Scale(float sX, float sY, float sZ) {
		   M11 *= sX;
		   M22 *= sY;
		   M33 *= sZ;
	   }
	   public void Translation(double dX, double dY, double dZ) {
		   Translation(Convert.ToSingle(dX), Convert.ToSingle(dY), Convert.ToSingle(dZ));
	   }
	   public void Scale(double sX, double sY, double sZ) {
		   Scale(Convert.ToSingle(sX), Convert.ToSingle(sY), Convert.ToSingle(sZ));
	   }
		public static Matrix4x4 CreateTranslation(float x, float y, float z) {
			Matrix4x4 resMat = Identity;
			resMat.M41 = x;
			resMat.M42 = y;
			resMat.M43 = z;
			return resMat;
		}
		public static Matrix4x4 CreateTranslation(double x, double y, double z) {
			return CreateTranslation(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z));
		}
		public static Matrix4x4 CreateScale(double sx, double sy, double sz) {
			return CreateScale(Convert.ToSingle(sx), Convert.ToSingle(sy), Convert.ToSingle(sz));
		}
		public static Matrix4x4 CreateScale(float sx, float sy, float sz) {
			return new Matrix4x4() { M11 = sx, M22 = sx, M33 = sz, M44 = 1.0f };
		}
	}
}
