/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using System;
#endregion

/// <summary>
/// Class representing a single layer of a fully connected feedforward neural network.
/// </summary>
public class NeuralLayer
{
    #region Members
    private static Random randomizer = new Random();

    /// <summary>
    /// Delegate representing the activation function of an artificial neuron.
    /// </summary>
    /// <param name="xValue">The input value of the function.</param>
    /// <returns>The calculated output value of the function.</returns>
    public delegate double ActivationFunction(double xValue);

    /// <summary>
    /// The activation function used by the neurons of this layer.
    /// </summary>
    /// <remarks>The default activation function is the sigmoid function (see <see cref="MathHelper.SigmoidFunction(double)"/>).</remarks>
    public ActivationFunction NeuronActivationFunction = MathHelper.SigmoidFunction;

    /// <summary>
    /// The amount of neurons in this layer.
    /// </summary>
    public uint NeuronCount
    {
        get;
        private set;
    }

    /// <summary>
    /// The amount of neurons this layer is connected to, i.e., the amount of neurons of the next layer.
    /// </summary>
    public uint OutputCount
    {
        get;
        private set;
    }

    /// <summary>
    /// The weights of the connections of this layer to the next layer.
    /// E.g., weight [i, j] is the weight of the connection from the i-th weight
    /// of this layer to the j-th weight of the next layer.
    /// </summary>
    public double[,] Weights
    {
        get;
        private set;
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initialises a new neural layer for a fully connected feedforward neural network with given 
    /// amount of node and with connections to the given amount of nodes of the next layer.
    /// </summary>
    /// <param name="nodeCount">The amount of nodes in this layer.</param>
    /// <param name="outputCount">The amount of nodes in the next layer.</param>
    /// <remarks>All weights of the connections from this layer to the next are initialised with the default double value.</remarks>
    public NeuralLayer(uint nodeCount, uint outputCount)
    {
        this.NeuronCount = nodeCount;
        this.OutputCount = outputCount;

        Weights = new double[nodeCount + 1, outputCount]; // + 1 for bias node
    }
    #endregion

    #region Methods
    /// <summary>
    /// Sets the weights of this layer to the given values.
    /// </summary>
    /// <param name="weights">
    /// The values to set the weights of the connections from this layer to the next to.
    /// </param>
    /// <remarks>
    /// The values are ordered in neuron order. E.g., in a layer with two neurons with a next layer of three neurons 
    /// the values [0-2] are the weights from neuron 0 of this layer to neurons 0-2 of the next layer respectively and 
    /// the values [3-5] are the weights from neuron 1 of this layer to neurons 0-2 of the next layer respectively.
    /// </remarks>
    public void SetWeights(double[] weights)
    {
        //Check arguments
        if (weights.Length != this.Weights.Length)
            throw new ArgumentException("Input weights do not match layer weight count.");

        // Copy weights from given value array
        int k = 0;
        for (int i = 0; i < this.Weights.GetLength(0); i++)
            for (int j = 0; j < this.Weights.GetLength(1); j++)
                this.Weights[i, j] = weights[k++];
    }

    /// <summary>
    /// Processes the given inputs using the current weights to the next layer.
    /// </summary>
    /// <param name="inputs">The inputs to be processed.</param>
    /// <returns>The calculated outputs.</returns>
    public double[] ProcessInputs(double[] inputs)
    {
        //Check arguments
        if (inputs.Length != NeuronCount)
            throw new ArgumentException("Given xValues do not match layer input count.");

        //Calculate sum for each neuron from weighted inputs and bias
        double[] sums = new double[OutputCount];
        //Add bias (always on) neuron to inputs
        double[] biasedInputs = new double[NeuronCount + 1];
        inputs.CopyTo(biasedInputs, 0);
        biasedInputs[inputs.Length] = 1.0;

        for (int j = 0; j < this.Weights.GetLength(1); j++)
            for (int i = 0; i < this.Weights.GetLength(0); i++)
                sums[j] += biasedInputs[i] * Weights[i, j];

        //Apply activation function to sum, if set
        if (NeuronActivationFunction != null)
        {
            for (int i = 0; i < sums.Length; i++)
                sums[i] = NeuronActivationFunction(sums[i]);
        }

        return sums;
    }

    /// <summary>
    /// Copies this NeuralLayer including its weights.
    /// </summary>
    /// <returns>A deep copy of this NeuralLayer</returns>
    public NeuralLayer DeepCopy()
    {
        //Copy weights
        double[,] copiedWeights = new double[this.Weights.GetLength(0), this.Weights.GetLength(1)];

        for (int x = 0; x < this.Weights.GetLength(0); x++)
            for (int y = 0; y < this.Weights.GetLength(1); y++)
                copiedWeights[x, y] = this.Weights[x, y];

        //Create copy
        NeuralLayer newLayer = new NeuralLayer(this.NeuronCount, this.OutputCount);
        newLayer.Weights = copiedWeights;
        newLayer.NeuronActivationFunction = this.NeuronActivationFunction;

        return newLayer;
    }

    /// <summary>
    /// Sets the weights of the connection from this layer to the next to random values in given range.
    /// </summary>
    /// <param name="minValue">The minimum value a weight may be set to.</param>
    /// <param name="maxValue">The maximum value a weight may be set to.</param>
    public void SetRandomWeights(double minValue, double maxValue)
    {
        double range = Math.Abs(minValue - maxValue);
        for (int i = 0; i < Weights.GetLength(0); i++)
            for (int j = 0; j < Weights.GetLength(1); j++)
                Weights[i, j] = minValue + (randomizer.NextDouble() * range); //random double between minValue and maxValue
    }

    /// <summary>
    /// Returns a string representing this layer's connection weights.
    /// </summary>
    public override string ToString()
    {
        string output = "";

        for (int x = 0; x < Weights.GetLength(0); x++)
        {
            for (int y = 0; y < Weights.GetLength(1); y++)
                output += "[" + x + "," + y + "]: " + Weights[x, y];

            output += "\n";
        }

        return output;
    }
    #endregion
}
