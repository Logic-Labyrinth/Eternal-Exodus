#if ENABLED_UNITY_MATHEMATICS
using Unity.Mathematics;
#endif

namespace LexUtils.Extensions {
    public static class MathfExtension {
        #region Min

#if ENABLED_UNITY_MATHEMATICS
        public static half Min(half a, half b) => (a < b) ? a : b;

        public static half Min(params half[] values) {
            int num = values.Length;
            if (num == 0) return (half) 0;

            half num2 = values[0];
            for (int i = 1; i < num; i++)
                if (values[i] < num2)
                    num2 = values[i];

            return num2;
        }
#endif

        public static double Min(double a, double b) {
            return (a < b) ? a : b;
        }

        public static double Min(params double[] values) {
            int num = values.Length;
            if (num == 0) return 0f;

            double num2 = values[0];
            for (int i = 1; i < num; i++)
                if (values[i] < num2)
                    num2 = values[i];

            return num2;
        }

        #endregion

        #region Max

#if ENABLED_UNITY_MATHEMATICS
        public static half Max(half a, half b) => (a > b) ? a : b;

        public static half Max(params half[] values) {
            int num = values.Length;
            if (num == 0) return (half) 0;

            half num2 = values[0];
            for (int i = 1; i < num; i++)
                if (values[i] > num2)
                    num2 = values[i];

            return num2;
        }
#endif

        public static double Max(double a, double b) => (a > b) ? a : b;

        public static double Max(params double[] values) {
            int num = values.Length;
            if (num == 0) return 0f;

            double num2 = values[0];
            for (int i = 1; i < num; i++)
                if (values[i] > num2)
                    num2 = values[i];

            return num2;
        }

        #endregion

        public static int Remap(this int value, int fromLow, int fromHigh, int toLow, int toHigh) {
            return toLow + (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow);
        }

        public static float Remap(this float value, float fromLow, float fromHigh, float toLow, float toHigh) {
            return toLow + (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow);
        }

        public static double Remap(this double value, double fromLow, double fromHigh, double toLow, double toHigh) {
            return toLow + (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow);
        }
    }
}