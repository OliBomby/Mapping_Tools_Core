/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the osuTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing detailed licensing details.
 *
 * Contributions by Georg W�chter.
 */

using System;
using System.Collections.Generic;

namespace Mapping_Tools_Core.MathUtil {
    /// <summary>
    /// Represents a bezier curve with as many points as you want.
    /// </summary>
    [Serializable]
    public struct BezierCurve {
        private readonly List<Vector2> points;

        /// <summary>
        /// The parallel value.
        /// </summary>
        /// <remarks>This value defines whether the curve should be calculated as a
        /// parallel curve to the original bezier curve. A value of 0.0f represents
        /// the original curve, 5.0f i.e. stands for a curve that has always a distance
        /// of 5.0f to the orignal curve at any point.</remarks>
        public double Parallel;

        /// <summary>
        /// Gets the points of this curve.
        /// </summary>
        /// <remarks>The first point and the last points represent the anchor points.</remarks>
        public IList<Vector2> Points {
            get {
                return points;
            }
        }

        /// <summary>
        /// Constructs a new <see cref="BezierCurve"/>.
        /// </summary>
        /// <param name="points">The points.</param>
        public BezierCurve(IEnumerable<Vector2> points) {
            if( points == null ) {
                throw new ArgumentNullException("points", "Must point to a valid list of Vector2 structures.");
            }

            this.points = new List<Vector2>(points);
            this.Parallel = 0.0f;
        }

        /// <summary>
        /// Constructs a new <see cref="BezierCurve"/>.
        /// </summary>
        /// <param name="points">The points.</param>
        public BezierCurve(params Vector2[] points) {
            if( points == null ) {
                throw new ArgumentNullException("points", "Must point to a valid list of Vector2 structures.");
            }

            this.points = new List<Vector2>(points);
            this.Parallel = 0.0f;
        }

        /// <summary>
        /// Constructs a new <see cref="BezierCurve"/>.
        /// </summary>
        /// <param name="parallel">The parallel value.</param>
        /// <param name="points">The points.</param>
        public BezierCurve(double parallel, params Vector2[] points) {
            if( points == null ) {
                throw new ArgumentNullException("points", "Must point to a valid list of Vector2 structures.");
            }

            this.Parallel = parallel;
            this.points = new List<Vector2>(points);
        }

        /// <summary>
        /// Constructs a new <see cref="BezierCurve"/>.
        /// </summary>
        /// <param name="parallel">The parallel value.</param>
        /// <param name="points">The points.</param>
        public BezierCurve(double parallel, IEnumerable<Vector2> points) {
            if( points == null ) {
                throw new ArgumentNullException("points", "Must point to a valid list of Vector2 structures.");
            }

            this.Parallel = parallel;
            this.points = new List<Vector2>(points);
        }


        /// <summary>
        /// Calculates the point with the specified t.
        /// </summary>
        /// <param name="t">The t value, between 0.0f and 1.0f.</param>
        /// <returns>Resulting point.</returns>
        public Vector2 CalculatePoint(double t) {
            return BezierCurve.CalculatePoint(points, t, Parallel);
        }

        /// <summary>
        /// Calculates the length of this bezier curve.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <returns>Length of curve.</returns>
        /// <remarks>The precision gets better as the <paramref name="precision"/>
        /// value gets smaller.</remarks>
        public double CalculateLength(double precision) {
            return BezierCurve.CalculateLength(points, precision, Parallel);
        }

        /// <summary>
        /// Calculates the length of the specified bezier curve.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="precision">The precision value.</param>
        /// <returns>The precision gets better as the <paramref name="precision"/>
        /// value gets smaller.</returns>
        public static double CalculateLength(IList<Vector2> points, double precision) {
            return BezierCurve.CalculateLength(points, precision, 0.0f);
        }

        /// <summary>
        /// Calculates the length of the specified bezier curve.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="precision">The precision value.</param>
        /// <param name="parallel">The parallel value.</param>
        /// <returns>Length of curve.</returns>
        /// <remarks><para>The precision gets better as the <paramref name="precision"/>
        /// value gets smaller.</para>
        /// <para>The <paramref name="parallel"/> parameter defines whether the curve should be calculated as a
        /// parallel curve to the original bezier curve. A value of 0.0f represents
        /// the original curve, 5.0f represents a curve that has always a distance
        /// of 5.0f to the orignal curve.</para></remarks>
        public static double CalculateLength(IList<Vector2> points, double precision, double parallel) {
            double length = 0.0f;
            Vector2 old = BezierCurve.CalculatePoint(points, 0.0f, parallel);

            for( double i = precision; i < ( 1.0f + precision ); i += precision ) {
                Vector2 n = CalculatePoint(points, i, parallel);
                length += ( n - old ).Length;
                old = n;
            }

            return length;
        }

        /// <summary>
        /// Calculates the point on the given bezier curve with the specified t parameter.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="t">The t parameter, a value between 0.0f and 1.0f.</param>
        /// <returns>Resulting point.</returns>
        public static Vector2 CalculatePoint(IList<Vector2> points, double t) {
            return BezierCurve.CalculatePoint(points, t, 0.0f);
        }

        /// <summary>
        /// Calculates the point on the given bezier curve with the specified t parameter.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="t">The t parameter, a value between 0.0f and 1.0f.</param>
        /// <param name="parallel">The parallel value.</param>
        /// <returns>Resulting point.</returns>
        /// <remarks>The <paramref name="parallel"/> parameter defines whether the curve should be calculated as a
        /// parallel curve to the original bezier curve. A value of 0.0f represents
        /// the original curve, 5.0f represents a curve that has always a distance
        /// of 5.0f to the orignal curve.</remarks>
        public static Vector2 CalculatePoint(IList<Vector2> points, double t, double parallel) {
            Vector2 r = new Vector2();
            double c = 1.0d - t;
            double temp;
            int i = 0;

            foreach( Vector2 pt in points ) {
                temp = MathHelper.BinomialCoefficient(points.Count - 1, i) * System.Math.Pow(t, i) *
                        System.Math.Pow(c, (points.Count - 1) - i);

                r.X += temp * pt.X;
                r.Y += temp * pt.Y;
                i++;
            }

            if( parallel == 0.0f ) {
                return r;
            }

            Vector2 perpendicular;

            if( t != 0.0f ) {
                perpendicular = r - CalculatePointOfDerivative(points, t);
            }
            else {
                perpendicular = points[1] - points[0];
            }

            return r + Vector2.Normalize(perpendicular).PerpendicularRight * parallel;
        }

        /// <summary>
        /// Calculates the point with the specified t of the derivative of the given bezier function.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="t">The t parameter, value between 0.0f and 1.0f.</param>
        /// <returns>Resulting point.</returns>
        private static Vector2 CalculatePointOfDerivative(IList<Vector2> points, double t) {
            Vector2 r = new Vector2();
            double c = 1.0d - t;
            double temp;
            int i = 0;

            foreach( Vector2 pt in points ) {
                temp = MathHelper.BinomialCoefficient(points.Count - 2, i) * System.Math.Pow(t, i) *
                        System.Math.Pow(c, (points.Count - 2) - i);

                r.X += temp * pt.X;
                r.Y += temp * pt.Y;
                i++;
            }

            return r;
        }
    }
}
