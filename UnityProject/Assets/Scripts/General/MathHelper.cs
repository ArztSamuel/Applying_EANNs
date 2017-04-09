/// Author: Samuel Arzt
/// Date: March

#region Includes
using System;
#endregion

/// <summary>
/// Static class for different Math operations and constants.
/// </summary>
public static class MathHelper
{
    #region Methods
    #region Activation Functions
    /// <summary>
    /// The standard sigmoid function.
    /// </summary>
    /// <param name="xValue">The input value.</param>
    /// <returns>The calculated output.</returns>
    public static double SigmoidFunction(double xValue)
    {
        if (xValue > 10) return 1.0;
        else if (xValue < -10) return 0.0;
        else return 1.0 / (1.0 + Math.Exp(-xValue));
    }

    /// <summary>
    /// The standard TanH function.
    /// </summary>
    /// <param name="xValue">The input value.</param>
    /// <returns>The calculated output.</returns>
    public static double TanHFunction(double xValue)
    {
        if (xValue > 10) return 1.0;
        else if (xValue < -10) return -1.0;
        else return Math.Tanh(xValue);
    }

    /// <summary>
    /// The SoftSign function as proposed by Xavier Glorot and Yoshua Bengio (2010): 
    /// "Understanding the difficulty of training deep feedforward neural networks".
    /// </summary>
    /// <param name="xValue">The input value.</param>
    /// <returns>The calculated output.</returns>
    public static double SoftSignFunction(double xValue)
    {
        return xValue / (1 + Math.Abs(xValue));
    }
    #endregion
    #endregion
}

