using OFC.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFC.Rendering
{
    public class Camera
    {
        // View Data
        float viewYaw;
        float viewPitch;
        float viewRoll;
        Vector3f viewFrom;
        Vector3f viewTo;
        Vector3f viewUp;
        public Matrix4f viewMatrix = new Matrix4f();

        // Projection Data
        float projFoV;
        float projAspect;
        float projZNear;
        float projZFar;

        public Matrix4f projMatrix = new Matrix4f();

        // Properties
        public Vector3f Right => new(viewMatrix.Components[0], viewMatrix.Components[4], viewMatrix.Components[8]);
        public Vector3f Up    => new(viewMatrix.Components[1], viewMatrix.Components[5], viewMatrix.Components[9]);
        public Vector3f Front => new(viewMatrix.Components[2], viewMatrix.Components[6], viewMatrix.Components[10]);

        public Vector3f Position => viewFrom;

        public Matrix4f ProjectionMatrix => projMatrix;
        public Matrix4f ViewMatrix => viewMatrix;
        //public Matrix4f ViewProjectionMatrix => projMatrix * viewMatrix;
        //public Matrix4f ClipMatrix => <to-do>

        public Camera(float FoV, float aspect, float zNear, float zFar) 
        { 
            viewFrom = new Vector3f();
            viewTo   = new Vector3f();
            viewUp   = Vector3f.UnitY;

            projFoV = FoV;
            projAspect = aspect;
            projZNear = zNear;
            projZFar = zFar;

            projMatrix = Matrix4f.CreatePerspective(projFoV, projAspect, projZNear, projZFar);
        }

        public void Update()
        {
            //Clamp our yaw-pitch-roll values
            viewYaw   = (viewYaw + 3600) % 360;
            viewPitch = Math.Clamp(viewPitch, -89, 89); 

            // Calculate new view vectors - If we cache the deg->rad conversion, this will be faster.
            viewTo.X = MathF.Cos(viewYaw   * FastMathF.DegRad) * MathF.Cos(viewPitch * FastMathF.DegRad);
            viewTo.Z = MathF.Sin(viewYaw   * FastMathF.DegRad) * MathF.Cos(viewPitch * FastMathF.DegRad);
            viewTo.Y = MathF.Sin(viewPitch * FastMathF.DegRad);
            viewTo.Normalize();

            // Build view matrix
            viewMatrix = Matrix4f.CreateLookAt(viewFrom, viewFrom + viewTo, viewUp);
        }


        public void SetRotation(float yaw, float pitch, float roll)
        {
            viewYaw = yaw;
            viewPitch = pitch;
            viewRoll = roll;
        }

        public void AddRotation(float yaw, float pitch, float roll)
        {
            viewYaw += yaw;
            viewPitch += pitch;
            viewRoll += roll;
        }

        /// <summary>
        /// Sets the camera position
        /// </summary>
        /// <param name="position">New position</param>
        public void SetPosition(Vector3f position)
        {
            viewFrom = position;
        }

        /// <summary>
        /// Sets the camera position
        /// </summary>
        /// <param name="x">new X Position</param>
        /// <param name="y">new Y Position</param>
        /// <param name="z">new Z Position</param>
        public void SetPosition(float x, float y, float z)
        {
            viewFrom.X = x;
            viewFrom.Y = y;
            viewFrom.Z = z;
        }

        /// <summary>
        /// Adds a position delta to the current camera position
        /// </summary>
        /// <param name="position">Delta position</param>
        public void AddPosition(Vector3f position)
        {
            viewFrom += position;
        }

        /// <summary>
        /// Adds a position delta to the current camera position
        /// </summary>
        /// <param name="x">Delta X position</param>
        /// <param name="y">Delta Y position</param>
        /// <param name="z">Delta Z position</param>
        public void AddPosition(float x, float y, float z)
        {
            viewFrom.X += x;
            viewFrom.Y += y;
            viewFrom.Z += z;
        }
    }
}
