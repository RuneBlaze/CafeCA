using UnityEngine;

namespace Cafeo.MapGen
{
    public class MarkovDigger
    {
        public enum State
        {
            DigLeft,
            DigRight,
            DigForward,
        }

        // public Random random;

        // DigLeft, DigRight, DigForward
        public Matrix4x4 transitionMatrix;
        public State state;
        public int k = 0;
        private float forwardForwardProbability;

        public MarkovDigger()
        {
            transitionMatrix = new Matrix4x4();
            transitionMatrix.SetRow(0, new Vector4(0.5f, 0.5f, 0.5f, 0.0f));
            transitionMatrix.SetRow(1, new Vector4(0.5f, 0.5f, 0.5f, 0.0f));
            transitionMatrix.SetRow(2, new Vector4(0.5f, 0.5f, 0.5f, 0.0f));
            transitionMatrix.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            
            forwardForwardProbability = transitionMatrix.GetRow(3).z;
        }

        // element [DigForward, DigForward] should be normalized according to some geometric distribution
        private void RefreshForwardProb()
        {
            int i = (int) State.DigForward;
            transitionMatrix[i, i] = GeomPdf(k, forwardForwardProbability);
        }
        
        private float GeomPdf(int k, float p)
        {
            return p * Mathf.Pow(1 - p, k);
        }

        public void ProgressState()
        {
            // float[] row = new float[4];
            RefreshForwardProb();
            var row = transitionMatrix.GetRow((int) state);
            float sum = row[0] + row[1] + row[2] + row[3];
            float r = Random.Range(0.0f, sum);
            if (r < row[0])
            {
                state = State.DigLeft;
                k = 0;
            }
            else if (r < row[0] + row[1])
            {
                state = State.DigRight;
                k = 0;
            }
            else if (r < row[0] + row[1] + row[2])
            {
                state = State.DigForward;
                k++;
            }
            else
            {
                state = State.DigForward;
                k++;
            }
        }
    }
}