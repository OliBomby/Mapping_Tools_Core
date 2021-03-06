/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the osuTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing detailed licensing details.
 *
 * Contributions by Georg W�chter.
 */

using System;

namespace Mapping_Tools_Core.MathUtil {
    /// <summary>
    /// Represents a quadric bezier curve with two anchor and one control point.
    /// </summary>
    [Serializable]
    public struct BezierCurveQuadric {
        /// <summary>
        /// Start anchor point.
        /// </summary>
        public Vector2 StartAnchor;

        /// <summary>
        /// End anchor point.
        /// </summary>
        public Vector2 EndAnchor;

        /// <summary>
        /// Control point, controls the direction of both endings of the curve.
        /// </summary>
        public Vector2 ControlPoint;

        /// <summary>
        /// The parallel value.
        /// </summary>
        /// <remarks>This value defines whether the curve should be calculated as a
        /// parallel curve to the original bezier curve. A value of 0.0f represents
        /// the original curve, 5.0f i.e. stands for a curve that has always a distance
        /// of 5.f to the orignal curve at any point.</remarks>
        public double Parallel;

        /// <summary>
        /// Constructs a new <see cref="BezierCurveQuadric"/>.
        /// </summary>
        /// <param name="startAnchor">The start anchor.</param>
        /// <param name="endAnchor">The end anchor.</param>
        /// <param name="controlPoint">The control point.</param>
        public BezierCurveQuadric(Vector2 startAnchor, Vector2 endAnchor, Vector2 controlPoint) {
            this.StartAnchor = startAnchor;
            this.EndAnchor = endAnchor;
            this.ControlPoint = controlPoint;
            this.Parallel = 0.0f;
        }

        /// <summary>
        /// Constructs a new <see cref="BezierCurveQuadric"/>.
        /// </summary>
        /// <param name="parallel">The parallel value.</param>
        /// <param name="startAnchor">The start anchor.</param>
        /// <param name="endAnchor">The end anchor.</param>
        /// <param name="controlPoint">The control point.</param>
        public BezierCurveQuadric(double parallel, Vector2 startAnchor, Vector2 endAnchor, Vector2 controlPoint) {
            this.Parallel = parallel;
            this.StartAnchor = startAnchor;
            this.EndAnchor = endAnchor;
            this.ControlPoint = controlPoint;
        }

        /// <summary>
        /// Calculates the point with the specified t.
        /// </summary>
        /// <param name="t">The t value, between 0.0f and 1.0f.</param>
        /// <returns>Resulting point.</returns>
        public Vector2 CalculatePoint(double t) {
            Vector2 r = new Vector2();
            double c = 1.0f - t;

            r.X = ( c * c * StartAnchor.X ) + ( 2 * t * c * ControlPoint.X ) + ( t * t * EndAnchor.X );
            r.Y = ( c * c * StartAnchor.Y ) + ( 2 * t * c * ControlPoint.Y ) + ( t * t * EndAnchor.Y );

            if( Parallel == 0.0f ) {
                return r;
            }

            Vector2 perpendicular;

            if( t == 0.0f ) {
                perpendicular = ControlPoint - StartAnchor;
            }
            else {
                perpendicular = r - CalculatePointOfDerivative(t);
            }

            return r + Vector2.Normalize(perpendicular).PerpendicularRight * Parallel;
        }

        /// <summary>
        /// Calculates the point with the specified t of the derivative of this function.
        /// </summary>
        /// <param name="t">The t, value between 0.0f and 1.0f.</param>
        /// <returns>Resulting point.</returns>
        private Vector2 CalculatePointOfDerivative(double t) {
            Vector2 r = new Vector2
            {
                X = (1.0f - t) * StartAnchor.X + t * ControlPoint.X,
                Y = (1.0f - t) * StartAnchor.Y + t * ControlPoint.Y
            };

            return r;
        }

        /// <summary>
        /// Calculates the length of this bezier curve.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <returns>Length of curve.</returns>
        /// <remarks>The precision gets better when the <paramref name="precision"/>
        /// value gets smaller.</remarks>
        public double CalculateLength(double precision) {
            double length = 0.0f;
            Vector2 old = CalculatePoint(0.0f);

            for( double i = precision; i < ( 1.0f + precision ); i += precision ) {
                Vector2 n = CalculatePoint(i);
                length += ( n - old ).Length;
                old = n;
            }

            return length;
        }
    }
}
