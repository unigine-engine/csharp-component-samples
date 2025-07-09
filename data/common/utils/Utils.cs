#region Math Variables
#if UNIGINE_DOUBLE
using Scalar = System.Double;
using Vec2 = Unigine.dvec2;
using Vec3 = Unigine.dvec3;
using Vec4 = Unigine.dvec4;
using Mat4 = Unigine.dmat4;
#else
using Scalar = System.Single;
using Vec2 = Unigine.vec2;
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
using WorldBoundBox = Unigine.BoundBox;
using WorldBoundSphere = Unigine.BoundSphere;
using WorldBoundFrustum = Unigine.BoundFrustum;
#endif
#endregion

using Unigine;
using System.Collections.Generic;

public class Utils
{
	static private float GetT(float t, float alpha, in Vec3 p0, in Vec3 p1)
	{
		Vec3 d = p1 - p0;
		float a = (float)MathLib.Dot(d, d);
		float b = MathLib.Pow(a, alpha * 0.5f);
		return (b + t);
	}

	static public Vec3 CatmullRomCentripetal(in Vec3 p0, in Vec3 p1, in Vec3 p2, in Vec3 p3, float t, float alpha = 0.5f)
	{
		float t0 = 0.0f;
		float t1 = GetT(t0, alpha, p0, p1);
		float t2 = GetT(t1, alpha, p1, p2);
		float t3 = GetT(t2, alpha, p2, p3);
		t = MathLib.Lerp(t1, t2, t);
		Vec3 A1 = p0 * ((t1 - t) / (t1 - t0)) + p1 * ((t - t0) / (t1 - t0));
		Vec3 A2 = p1 * ((t2 - t) / (t2 - t1)) + p2 * ((t - t1) / (t2 - t1));
		Vec3 A3 = p2 * ((t3 - t) / (t3 - t2)) + p3 * ((t - t2) / (t3 - t2));
		Vec3 B1 = A1 * ((t2 - t) / (t2 - t0)) + A2 * ((t - t0) / (t2 - t0));
		Vec3 B2 = A2 * ((t3 - t) / (t3 - t1)) + A3 * ((t - t1) / (t3 - t1));
		Vec3 C = B1 * ((t2 - t) / (t2 - t1)) + B2 * ((t - t1) / (t2 - t1));
		return C;
	}

	static public List<float> GetLengthCatmullRomCentripetal(in Vec3 p0, in Vec3 p1, in Vec3 p2, in Vec3 p3, int subdivisions)
	{
		List<float> res = new List<float>();
		Vec3 start = CatmullRomCentripetal(p0, p1, p2, p3, 0);
		for (int i = 1; i < subdivisions; i++)
		{
			Vec3 end = CatmullRomCentripetal(p0, p1, p2, p3, (float)i / (subdivisions - 1));
			res.Add((float)MathLib.Length(end - start));
			start = end;
		}

		return res;
	}

	static public quat Squad(in quat q0,  in quat q1, in quat q2, in quat q3, float t)
	{
		quat q0m = GetLength(q1 - q0) < GetLength(q1 + q0) ? q0 : -q0;
		quat q2m = GetLength(q1 - q2) < GetLength(q1 + q2) ? q2 : -q2;
		quat q3m = GetLength(q2m - q3) < GetLength(q2m + q3) ? q3 : -q3;

		// calculate helper quaternions (tangent values)
		// https://www.geometrictools.com/Documentation/Quaternions.pdf (page 9 [31])
		quat q1inv = MathLib.Inverse(q1);
		quat q2inv = MathLib.Inverse(q2m);
		quat a = (q1 * QExp((QLog(q1inv * q2m) + QLog(q1inv * q0m)) * -0.25f));
		quat b = (q2m * QExp((QLog(q2inv * q3m) + QLog(q2inv * q1)) * -0.25f));

		return MathLib.Slerp(MathLib.Slerp(q1, q2m, t), MathLib.Slerp(a, b, t), 2.0f * t * (1.0f - t)).Normalized;

		quat QExp(in quat q)
		{
			float a = MathLib.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z);
			float sin_a = MathLib.Sin(a);
			vec4 r = new vec4();
			r.w = MathLib.Cos(a);
			if (MathLib.Abs(sin_a) >= 1.0e-15)
			{
				float coeff = sin_a / a;
				r.x = q.x * coeff;
				r.y = q.y * coeff;
				r.z = q.z * coeff;
			}

			return new quat(r);
		}

		quat QLog(in quat q)
		{
			float a = MathLib.Acos(q.w);
			float sin_a = MathLib.Sin(a);
			if (MathLib.Abs(sin_a) >= 1.0e-15)
			{
				float coeff = a / sin_a;
				vec4 r = new vec4(q.x * coeff, q.y * coeff, q.z * coeff, 0);
				return new quat(r);
			}
			return new quat(vec4.ZERO);
		}

		float GetLength(in quat q)
		{
			return MathLib.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
		}
	}
}
