/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using System;
#endregion

/// <summary>
/// Class representing a fully connceted feedforward neural network.
/// </summary>
public class NeuralNetwork
{
    #region Members
    /// <summary>
    /// The individual neural layers of this network.
    /// </summary>
    public NeuralLayer[] Layers
    {
        get;
        private set;
    }

    /// <summary>
    /// An array of unsigned integers representing the node count 
    /// of each layer of the network from input to output layer.
    /// </summary>
    public uint[] Topology
    {
        get;
        private set;
    }

    /// <summary>
    /// The amount of overall weights of the connections of this network.
    /// </summary>
    public int WeightCount
    {
        get;
        private set;
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initialises a new fully connected feedforward neural network with given topology.
    /// </summary>
    /// <param name="topology">An array of unsigned integers representing the node count of each layer from input to output layer.</param>
    public NeuralNetwork(params uint[] topology)
    {
        this.Topology = topology;

        //Calculate overall weight count
        WeightCount = 0;
        for (int i = 0; i < topology.Length - 1; i++)
            WeightCount += (int) ((topology[i] + 1) * topology[i + 1]); // + 1 for bias node

        //Initialise layers
        Layers = new NeuralLayer[topology.Length - 1];
        for (int i = 0; i<Layers.Length; i++)
            Layers[i] = new NeuralLayer(topology[i], topology[i + 1]);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Processes the given inputs using the current network's weights.
    /// </summary>
    /// <param name="inputs">The inputs to be processed.</param>
    /// <returns>The calculated outputs.</returns>
    public double[] ProcessInputs(double[] inputs)
    {
        //Check arguments
        if (inputs.Length != Layers[0].NeuronCount)
            throw new ArgumentException("Given inputs do not match network input amount.");

        //Process inputs by propagating values through all layers
        double[] outputs = inputs;
        foreach (NeuralLayer layer in Layers)
            outputs = layer.ProcessInputs(outputs);

        return outputs;
        
    }

    /// <summary>
    /// Sets the weights of this network to random values in given range.
    /// </summary>
    /// <param name="minValue">The minimum value a weight may be set to.</param>
    /// <param name="maxValue">The maximum value a weight may be set to.</param>
    public void SetRandomWeights(double minValue, double maxValue)
    {
        if (Layers != null)
        {
            foreach (NeuralLayer layer in Layers)
                layer.SetRandomWeights(minValue, maxValue);
        }
    }

    /// <summary>
    /// Returns a new NeuralNetwork instance with the same topology and 
    /// activation functions, but the weights set to their default value.
    /// </summary>
    public NeuralNetwork GetTopologyCopy()
    {
        NeuralNetwork copy = new NeuralNetwork(this.Topology);
        for (int i = 0; i < Layers.Length; i++)
            copy.Layers[i].NeuronActivationFunction = this.Layers[i].NeuronActivationFunction;

        return copy;
    }

    /// <summary>
    /// Copies this NeuralNetwork including its topology and weights.
    /// </summary>
    /// <returns>A deep copy of this NeuralNetwork</returns>
    public NeuralNetwork DeepCopy()
    {
        NeuralNetwork newNet = new NeuralNetwork(this.Topology);
        for (int i = 0; i < this.Layers.Length; i++)
            newNet.Layers[i] = this.Layers[i].DeepCopy();

        return newNet;
    }

    /// <summary>
    /// Returns a string representing this network in layer order.
    /// </summary>
    public override string ToString()
    {
        string output = "";

        for (int i = 0; i<Layers.Length; i++)
            output += "Layer " + i + ":\n" + Layers[i].ToString();

        return output;
    }
    #endregion
}
